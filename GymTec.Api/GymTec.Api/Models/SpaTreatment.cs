using System;
using System.Collections.Generic;

namespace GymTec.Api.Models
{
    /// <summary>
    /// Representa un tratamiento de Spa. Los marcados como IsDefault=true no se pueden modificar/eliminar (trigger en BD).
    /// </summary>
    public class SpaTreatment
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsDefault { get; set; }

        public ICollection<SpaReservation> SpaReservations { get; set; } = new List<SpaReservation>();

        // Navegación inversa para asociaciones con sucursal
        public ICollection<SpaBranchTreatment> SpaBranchTreatments { get; set; } = new List<SpaBranchTreatment>();
    }
}

