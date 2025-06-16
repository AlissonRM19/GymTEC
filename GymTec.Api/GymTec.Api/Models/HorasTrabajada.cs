using System;

namespace GymTec.Api.Models
{
    /// <summary>
    /// Representa un registro de horas trabajadas para un empleado (User con TipoPlanilla Horas).
    /// </summary>
    public class HorasTrabajada
    {
        public int Id { get; set; }

        /// <summary>
        /// FK al usuario que es empleado por horas.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Fecha del registro.
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Hora de entrada.
        /// </summary>
        public TimeSpan HoraEntrada { get; set; }

        /// <summary>
        /// Hora de salida.
        /// </summary>
        public TimeSpan HoraSalida { get; set; }

        /// <summary>
        /// Navegación al User.
        /// </summary>
        public User User { get; set; } = null!;
    }
}

