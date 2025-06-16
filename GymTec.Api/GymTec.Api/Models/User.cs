using System;
using System.ComponentModel.DataAnnotations;

namespace GymTec.Api.Models
{
    /// <summary>
    /// Representa un usuario del sistema (Administrador, Instructor, DependienteSpa, DependienteTienda, Cliente).
    /// </summary>
    public class User
    {
        public Guid Id { get; set; }

        [Required, MaxLength(20)]
        public string Cedula { get; set; } = null!;

        [Required, MaxLength(200)]
        public string NombreCompleto { get; set; } = null!;

        [MaxLength(100)]
        public string? Provincia { get; set; }

        [MaxLength(100)]
        public string? Canton { get; set; }

        [MaxLength(100)]
        public string? Distrito { get; set; }

        [Required, MaxLength(200), EmailAddress]
        public string Correo { get; set; } = null!;

        [Required, StringLength(32, MinimumLength = 32)]
        public string PasswordMd5 { get; set; } = null!;

        [Required]
        public UserRole Role { get; set; }

        // —— Campos para nómina ——:
        public int? SucursalId { get; set; }
        public int? TipoPlanillaId { get; set; }
        public decimal? Salario { get; set; }

        // Navegaciones (opcionales)
        public Sucursal? Sucursal { get; set; }
        public TipoPlanilla? TipoPlanilla { get; set; }

        // Colecciones de navegación para planilla:
        public ICollection<HorasTrabajada> HorasTrabajadas { get; set; } = new List<HorasTrabajada>();
        public ICollection<ClasesImpartida> ClasesImpartidas { get; set; } = new List<ClasesImpartida>();

        public ICollection<SpaReservation> SpaReservations { get; set; } = new List<SpaReservation>();
        public ICollection<ClaseReservation> ClaseReservations { get; set; } = new List<ClaseReservation>();
    }

    /// <summary>
    /// Roles permitidos en el sistema.</summary>
    public enum UserRole
    {
        Administrador,
        Instructor,
        DependienteSpa,
        DependienteTienda,
        Cliente
    }
}
