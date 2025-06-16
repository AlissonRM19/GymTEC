using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymTec.Api.Data;
using GymTec.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTec.Api.Controllers
{
    /// <summary>
    /// Controlador para CRUD de Clases (grupales o personalizadas).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClasesController : ControllerBase
    {
        private readonly GymTecContext _context;

        public ClasesController(GymTecContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/Clases
        /// Obtiene todas las clases.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clase>>> GetAll()
        {
            return Ok(await _context.Clases.AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// GET /api/Clases/{id}
        /// Obtiene una clase por Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Clase>> GetById(Guid id)
        {
            var c = await _context.Clases.FindAsync(id);
            if (c == null)
                return NotFound($"Clase con Id '{id}' no encontrada.");
            return Ok(c);
        }

        /// <summary>
        /// POST /api/Clases
        /// Crea una nueva clase.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Clase>> Create([FromBody] ClaseCreateDto dto)
        {
            // Validaciones básicas
            if (dto.Capacidad <= 0)
                return BadRequest("La capacidad debe ser mayor a 0.");
            if (dto.Fecha.Date < DateTime.UtcNow.Date)
                return BadRequest("La fecha de la clase no puede ser en el pasado.");

            var clase = new Clase
            {
                Id = Guid.NewGuid(),
                Tipo = dto.Tipo,
                IsGrupal = dto.IsGrupal,
                Capacidad = dto.Capacidad,
                Fecha = dto.Fecha,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin,
                InstructorId = dto.InstructorId
            };

            // (Opcional) aquí podrías verificar que InstructorId exista en Users y sea rol Instructor
            if (clase.InstructorId.HasValue)
            {
                var instructor = await _context.Users.FindAsync(clase.InstructorId.Value);
                if (instructor == null || instructor.Role != UserRole.Instructor)
                    return BadRequest("InstructorId inválido o no corresponde a un instructor válido.");
            }

            _context.Clases.Add(clase);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = clase.Id }, clase);
        }

        /// <summary>
        /// PUT /api/Clases/{id}
        /// Actualiza una clase existente.
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ClaseUpdateDto dto)
        {
            var existing = await _context.Clases.FindAsync(id);
            if (existing == null)
                return NotFound($"Clase con Id '{id}' no encontrada.");

            // Actualizar campos si se envían
            if (!string.IsNullOrWhiteSpace(dto.Tipo))
                existing.Tipo = dto.Tipo;
            if (dto.Capacidad.HasValue)
            {
                if (dto.Capacidad.Value <= 0)
                    return BadRequest("La capacidad debe ser mayor a 0.");
                existing.Capacidad = dto.Capacidad.Value;
            }
            if (dto.Fecha.HasValue)
            {
                if (dto.Fecha.Value.Date < DateTime.UtcNow.Date)
                    return BadRequest("La fecha de la clase no puede ser en el pasado.");
                existing.Fecha = dto.Fecha.Value;
            }
            if (dto.HoraInicio.HasValue)
                existing.HoraInicio = dto.HoraInicio.Value;
            if (dto.HoraFin.HasValue)
                existing.HoraFin = dto.HoraFin.Value;
            if (dto.IsGrupal.HasValue)
                existing.IsGrupal = dto.IsGrupal.Value;
            if (dto.InstructorId.HasValue)
            {
                var instructor = await _context.Users.FindAsync(dto.InstructorId.Value);
                if (instructor == null || instructor.Role != UserRole.Instructor)
                    return BadRequest("InstructorId inválido o no corresponde a un instructor válido.");
                existing.InstructorId = dto.InstructorId;
            }

            _context.Clases.Update(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// DELETE /api/Clases/{id}
        /// Elimina una clase.
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _context.Clases.FindAsync(id);
            if (existing == null)
                return NotFound($"Clase con Id '{id}' no encontrada.");
            _context.Clases.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// DTO para creación de clase.
        /// </summary>
        public class ClaseCreateDto
        {
            public string Tipo { get; set; } = null!;
            public bool IsGrupal { get; set; }
            public int Capacidad { get; set; }
            public DateTime Fecha { get; set; }
            public TimeSpan HoraInicio { get; set; }
            public TimeSpan HoraFin { get; set; }
            public Guid? InstructorId { get; set; }
        }

        /// <summary>
        /// DTO para actualización de clase.
        /// </summary>
        public class ClaseUpdateDto
        {
            public string? Tipo { get; set; }
            public bool? IsGrupal { get; set; }
            public int? Capacidad { get; set; }
            public DateTime? Fecha { get; set; }
            public TimeSpan? HoraInicio { get; set; }
            public TimeSpan? HoraFin { get; set; }
            public Guid? InstructorId { get; set; }
        }
    }
}
