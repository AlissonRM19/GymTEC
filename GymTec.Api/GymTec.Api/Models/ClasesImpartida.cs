using System;

namespace GymTec.Api.Models
{
    /// <summary>
    /// Representa un registro de clase impartida para un empleado (User con TipoPlanilla Clase).
    /// </summary>
    public class ClasesImpartida
    {
        public int Id { get; set; }

        /// <summary>
        /// FK al usuario que es empleado por clase.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Fecha en que se impartió la clase.
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Detalle opcional de la clase impartida.
        /// </summary>
        public string? Detalle { get; set; }

        /// <summary>
        /// Navegación al User.
        /// </summary>
        public User User { get; set; } = null!;
    }
}

