using System;

namespace GymTec.Api.Models
{
    /// <summary>
    /// Reserva de clase grupal por un usuario.</summary>
    public class ClaseReservation
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;  // navegación al usuario que reserva
        public Guid ClaseId { get; set; }
        public Clase Clase { get; set; } = null!;
        public DateTime ReservationDate { get; set; }
    }
}
