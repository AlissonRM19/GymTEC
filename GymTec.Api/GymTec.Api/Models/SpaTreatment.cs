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
        public string Name { get; set; } // Ensure this property exists and matches the expected usage
        public bool IsDefault { get; set; }
        public ICollection<SpaReservation> SpaReservations { get; set; }
        public ICollection<SpaBranchTreatment> SpaBranchTreatments { get; set; }
    }
}

