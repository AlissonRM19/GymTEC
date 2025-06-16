using GymTec.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GymTec.Api.Data
{
    /// <summary>
    /// DbContext para GymTec, usa PostgreSQL vía Npgsql.</summary>
    public class GymTecContext : DbContext
    {
        public GymTecContext(DbContextOptions<GymTecContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<TipoPlanilla> TiposPlanilla { get; set; }

        public DbSet<HorasTrabajada> HorasTrabajadas { get; set; }  // mapear con EF o crear manualmente la tabla
        
        public DbSet<ClasesImpartida> ClasesImpartidas { get; set; }      // idem

        public DbSet<SpaTreatment> SpaTreatments { get; set; }

        public DbSet<Clase> Clases { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<SpaBranchTreatment> SpaBranchTreatments { get; set; }
        public DbSet<SpaReservation> SpaReservations { get; set; }
        public DbSet<ClaseReservation> ClaseReservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).ValueGeneratedOnAdd();
                entity.HasIndex(u => u.Cedula).IsUnique();
                entity.HasIndex(u => u.Correo).IsUnique();
                entity.Property(u => u.Cedula).IsRequired().HasMaxLength(20);
                entity.Property(u => u.NombreCompleto).IsRequired().HasMaxLength(200);
                entity.Property(u => u.Correo).IsRequired().HasMaxLength(200);
                entity.Property(u => u.PasswordMd5).IsRequired().HasMaxLength(32);
                entity.Property(u => u.Role).IsRequired();
                entity.Property(u => u.Provincia).HasMaxLength(100);
                entity.Property(u => u.Canton).HasMaxLength(100);
                entity.Property(u => u.Distrito).HasMaxLength(100);
                entity.Property(u => u.SucursalId).IsRequired(false);
                entity.Property(u => u.TipoPlanillaId).IsRequired(false);
                entity.Property(u => u.Salario).HasColumnType("numeric(12,2)").IsRequired(false);
                entity.HasOne(u => u.Sucursal)
                      .WithMany(s => s.Users)
                      .HasForeignKey(u => u.SucursalId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(u => u.TipoPlanilla)
                      .WithMany(tp => tp.Users)
                      .HasForeignKey(u => u.TipoPlanillaId)
                      .OnDelete(DeleteBehavior.SetNull);
                // Conversión enum Role a string o integer 
                entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(50);

                entity.HasMany(u => u.SpaReservations)
                      .WithOne(r => r.User)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.ClaseReservations)
                      .WithOne(r => r.User)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TipoPlanilla>(entity =>
            {
                entity.ToTable("TiposPlanilla");
                entity.HasKey(tp => tp.Id);
                entity.Property(tp => tp.Descripcion).IsRequired().HasMaxLength(50);
                entity.HasIndex(tp => tp.Descripcion).IsUnique();
            });

            // Configuración de HorasTrabajada
            modelBuilder.Entity<HorasTrabajada>(entity =>
            {
                entity.ToTable("horas_trabajadas");
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Fecha).IsRequired();
                entity.Property(h => h.HoraEntrada).IsRequired();
                entity.Property(h => h.HoraSalida).IsRequired();
                entity.HasOne(h => h.User)
                      .WithMany(u => u.HorasTrabajadas)
                      .HasForeignKey(h => h.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración ClasesImpartida
            modelBuilder.Entity<ClasesImpartida>(entity =>
            {
                entity.ToTable("clases_impartidas");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Fecha).IsRequired();
                entity.Property(c => c.Detalle).HasColumnType("text");
                entity.HasOne(c => c.User)
                      .WithMany(u => u.ClasesImpartidas)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // Configuración SpaTreatment
            modelBuilder.Entity<SpaTreatment>(entity =>
            {
                entity.ToTable("SpaTreatments");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
                entity.Property(s => s.Name).IsRequired().HasMaxLength(200);
                entity.HasIndex(s => s.Name).IsUnique();
                entity.Property(s => s.IsDefault).IsRequired();
                entity.HasMany(t => t.SpaReservations)
                      .WithOne(r => r.SpaTreatment)
                      .HasForeignKey(r => r.SpaTreatmentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(t => t.SpaBranchTreatments)
                      .WithOne(sbt => sbt.SpaTreatment)
                      .HasForeignKey(sbt => sbt.SpaTreatmentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración Clase
            modelBuilder.Entity<Clase>(entity =>
            {
                entity.ToTable("Clases");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();
                entity.Property(c => c.Tipo).IsRequired().HasMaxLength(100);
                entity.Property(c => c.IsGrupal).IsRequired();
                entity.Property(c => c.Capacidad).IsRequired();
                entity.Property(c => c.Fecha).IsRequired();
                entity.Property(c => c.HoraInicio).IsRequired();
                entity.Property(c => c.HoraFin).IsRequired();
                entity.Property(c => c.InstructorId).IsRequired(false);
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(c => c.InstructorId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(c => c.ClaseReservations)
                      .WithOne(r => r.Clase)
                      .HasForeignKey(r => r.ClaseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración Sucursal
            modelBuilder.Entity<Sucursal>(entity =>
            {
                entity.ToTable("Sucursales");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
                entity.Property(s => s.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(s => s.Provincia).HasMaxLength(100);
                entity.Property(s => s.Canton).HasMaxLength(100);
                entity.Property(s => s.Distrito).HasMaxLength(100);
                entity.Property(s => s.Direccion).HasMaxLength(300);
                entity.Property(s => s.Telefono).HasMaxLength(50);
                entity.HasMany(s => s.SpaReservations)
                      .WithOne(r => r.Sucursal)
                      .HasForeignKey(r => r.SucursalId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(s => s.SpaBranchTreatments)
                      .WithOne(sbt => sbt.Sucursal)
                      .HasForeignKey(sbt => sbt.SucursalId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración SpaBranchTreatment (composite PK)
            modelBuilder.Entity<SpaBranchTreatment>(entity =>
            {
                entity.ToTable("SpaBranchTreatments");
                entity.HasKey(sbt => new { sbt.SucursalId, sbt.SpaTreatmentId });
                entity.HasOne(sbt => sbt.Sucursal)
                      .WithMany(s => s.SpaBranchTreatments)
                      .HasForeignKey(sbt => sbt.SucursalId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(sbt => sbt.SpaTreatment)
                      .WithMany(t => t.SpaBranchTreatments)
                      .HasForeignKey(sbt => sbt.SpaTreatmentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // SpaReservation
            modelBuilder.Entity<SpaReservation>(entity =>
            {
                entity.ToTable("SpaReservations");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id).ValueGeneratedOnAdd();

                // Relación con User
                entity.HasOne(r => r.User)
                      .WithMany(u => u.SpaReservations)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con Sucursal
                entity.HasOne(r => r.Sucursal)
                      .WithMany(s => s.SpaReservations)
                      .HasForeignKey(r => r.SucursalId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con SpaTreatment
                entity.HasOne(r => r.SpaTreatment)
                      .WithMany(t => t.SpaReservations)
                      .HasForeignKey(r => r.SpaTreatmentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(r => r.ReservationDate).IsRequired();
                entity.Property(r => r.StartTime).IsRequired();
                entity.Property(r => r.EndTime).IsRequired();
            });

            // ClaseReservation
            modelBuilder.Entity<ClaseReservation>(entity =>
            {
                entity.ToTable("ClaseReservations");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id).ValueGeneratedOnAdd();

                // Relación con User
                entity.HasOne(r => r.User)
                      .WithMany(u => u.ClaseReservations)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con Clase
                entity.HasOne(r => r.Clase)
                      .WithMany(c => c.ClaseReservations)
                      .HasForeignKey(r => r.ClaseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(r => r.ReservationDate).IsRequired();
            });
        }
    }
}

