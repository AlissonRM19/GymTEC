using System;

namespace GymTec.Api.Models
{
    /// <summary>
    /// Reserva de Spa por un cliente en una sucursal y tratamiento específicos.</summary>
    public class SpaReservation
    {
        public Guid Id { get; set; }

        // FK al usuario
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;  // navegación al usuario que reserva
        
        // FK a sucursal
        public int SucursalId { get; set; }
        public Sucursal Sucursal { get; set; } = null!;
        
        // FK a SpaTreatment
        public Guid SpaTreatmentId { get; set; }
        public SpaTreatment SpaTreatment { get; set; } = null!;

        public DateTime ReservationDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}

