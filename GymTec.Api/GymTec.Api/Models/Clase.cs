using System;

namespace GymTec.Api.Models
{
    /// <summary>
    /// Representa una clase (grupal o personalizada).</summary>
    public class Clase
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; } = null!;
        public bool IsGrupal { get; set; }
        public int Capacidad { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public Guid? InstructorId { get; set; }
        public ICollection<ClaseReservation> ClaseReservations { get; set; } = new List<ClaseReservation>();
    }
}

