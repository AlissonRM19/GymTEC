using System;

namespace GymTec.Api.Models
{
    /// <summary>
    /// Asociación entre Sucursal y SpaTreatment: indica que un tratamiento está disponible en cierta sucursal.</summary>
    public class SpaBranchTreatment
    {
        public int SucursalId { get; set; }
        public Guid SpaTreatmentId { get; set; }

        public Sucursal Sucursal { get; set; } = null!;
        public SpaTreatment SpaTreatment { get; set; } = null!;
    }
}

