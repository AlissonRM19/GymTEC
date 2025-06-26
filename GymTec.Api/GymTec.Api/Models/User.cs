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
        public string Cedula { get; set; }
        public string NombreCompleto { get; set; }
        public string? Provincia { get; set; }
        public string? Canton { get; set; }
        public string? Distrito { get; set; }
        public string Correo { get; set; }
        public string PasswordMd5 { get; set; }
        public UserRole Role { get; set; }
        public Guid? InstructorId { get; set; } // Instructor asignado
        public int? SucursalId { get; set; }
        public int? TipoPlanillaId { get; set; }
        public decimal? Salario { get; set; }
        public Sucursal? Sucursal { get; set; }
        public TipoPlanilla? TipoPlanilla { get; set; }
        public ICollection<HorasTrabajada> HorasTrabajadas { get; set; }
        public ICollection<ClasesImpartida> ClasesImpartidas { get; set; }
        public ICollection<SpaReservation> SpaReservations { get; set; }
        public ICollection<ClaseReservation> ClaseReservations { get; set; }
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
