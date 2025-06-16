using System;
using System.Collections.Generic;

namespace GymTec.Api.Models
{
    /// <summary>
    /// Representa una sucursal de gimnasio/Spa.</summary>
    public class Sucursal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Provincia { get; set; }
        public string? Canton { get; set; }
        public string? Distrito { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();

        public ICollection<SpaReservation> SpaReservations { get; set; } = new List<SpaReservation>();

        public ICollection<SpaBranchTreatment> SpaBranchTreatments { get; set; } = new List<SpaBranchTreatment>();

        // Si se quiere navegación inversa a clases (si aplicara):
        // public ICollection<Clase> Clases { get; set; } = new List<Clase>();
    }
}

