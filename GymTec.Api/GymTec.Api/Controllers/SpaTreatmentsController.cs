using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymTec.Api.Data;
using GymTec.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTec.Api.Controllers
{
    /// <summary>
    /// Controlador para CRUD de tratamientos de Spa.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SpaTreatmentsController : ControllerBase
    {
        private readonly GymTecContext _context;

        public SpaTreatmentsController(GymTecContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/SpaTreatments
        /// Obtiene todos los tratamientos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpaTreatment>>> GetAll()
        {
            return Ok(await _context.SpaTreatments.AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// GET /api/SpaTreatments/{id}
        /// Obtiene un tratamiento por Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SpaTreatment>> GetById(Guid id)
        {
            var t = await _context.SpaTreatments.FindAsync(id);
            if (t == null)
                return NotFound($"Tratamiento de Spa con Id '{id}' no encontrado.");
            return Ok(t);
        }

        /// <summary>
        /// POST /api/SpaTreatments
        /// Crea un nuevo tratamiento de Spa. Si IsDefault=true, asegúrate de que el trigger en BD lo maneje.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SpaTreatment>> Create([FromBody] SpaTreatmentCreateDto dto)
        {
            // Verificar nombre duplicado
            if (await _context.SpaTreatments.AnyAsync(s => s.Name == dto.Name))
                return Conflict($"Ya existe un tratamiento con nombre '{dto.Name}'.");
            var treatment = new SpaTreatment
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                IsDefault = dto.IsDefault
            };
            _context.SpaTreatments.Add(treatment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = treatment.Id }, treatment);
        }

        /// <summary>
        /// PUT /api/SpaTreatments/{id}
        /// Actualiza un tratamiento. Si el tratamiento es default (IsDefault=true),
        /// el trigger en BD debe impedir cambios (o aquí puedes validar).
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SpaTreatmentUpdateDto dto)
        {
            var existing = await _context.SpaTreatments.FindAsync(id);
            if (existing == null)
                return NotFound($"Tratamiento de Spa con Id '{id}' no encontrado.");

            // Si es default y no permites modificar nombre:
            if (existing.IsDefault)
                return BadRequest("No se puede modificar un tratamiento por defecto.");

            // Verificar duplicado de nombre si cambia
            if (!string.Equals(existing.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await _context.SpaTreatments.AnyAsync(s => s.Name == dto.Name && s.Id != id))
                    return Conflict($"Ya existe un tratamiento con nombre '{dto.Name}'.");
            }

            existing.Name = dto.Name;
            // No permitir cambiar IsDefault aquí en actualización normal
            _context.SpaTreatments.Update(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// DELETE /api/SpaTreatments/{id}
        /// Elimina un tratamiento. Si es default o está asociado a algún Spa, el trigger en BD o la lógica debe impedirlo.
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _context.SpaTreatments.FindAsync(id);
            if (existing == null)
                return NotFound($"Tratamiento de Spa con Id '{id}' no encontrado.");

            // Si es default, no permitir
            if (existing.IsDefault)
                return BadRequest("No se puede eliminar un tratamiento por defecto.");

            // Aquí podrías verificar asociación con Spa concreto antes de eliminar;
            // Se asume que habrá otra tabla de Spa sucursal-tratamiento y trigger en BD.
            _context.SpaTreatments.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// DTO para crear tratamiento de Spa.
        /// </summary>
        public class SpaTreatmentCreateDto
        {
            public string Name { get; set; } = null!;
            public bool IsDefault { get; set; }
        }

        /// <summary>
        /// DTO para actualizar tratamiento de Spa.
        /// </summary>
        public class SpaTreatmentUpdateDto
        {
            public string Name { get; set; } = null!;
        }
    }
}

