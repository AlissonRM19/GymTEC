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
    /// Asociación entre Sucursal y SpaTreatment: gestiona disponibilidad de tratamientos en cada sucursal.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SpaBranchTreatmentsController : ControllerBase
    {
        private readonly GymTecContext _context;

        public SpaBranchTreatmentsController(GymTecContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/SpaBranchTreatments
        /// Lista todas las asociaciones (útil en admin). Devuelve sucursalId y spaTreatmentId.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpaBranchTreatment>>> GetAll()
        {
            return Ok(await _context.SpaBranchTreatments
                        .AsNoTracking()
                        .ToListAsync());
        }

        /// <summary>
        /// GET /api/SpaBranchTreatments/sucursal/{sucursalId}
        /// Obtiene tratamientos disponibles en una sucursal.
        /// </summary>
        [HttpGet("sucursal/{sucursalId:int}")]
        public async Task<ActionResult<IEnumerable<SpaTreatment>>> GetBySucursal(int sucursalId)
        {
            // Validar existencia de sucursal
            var sucursal = await _context.Sucursales.FindAsync(sucursalId);
            if (sucursal == null)
                return NotFound($"Sucursal con Id '{sucursalId}' no encontrada.");

            // Obtener tratamientos disponibles
            var tratamientos = await _context.SpaBranchTreatments
                .Where(sbt => sbt.SucursalId == sucursalId)
                .Include(sbt => sbt.SpaTreatment)
                .Select(sbt => sbt.SpaTreatment)
                .ToListAsync();

            return Ok(tratamientos);
        }

        /// <summary>
        /// POST /api/SpaBranchTreatments
        /// Asocia un tratamiento a una sucursal. Body: { "sucursalId": 1, "spaTreatmentId": "guid" }
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] SpaBranchTreatment dto)
        {
            // Validar existencia de sucursal
            var sucursal = await _context.Sucursales.FindAsync(dto.SucursalId);
            if (sucursal == null)
                return NotFound($"Sucursal con Id '{dto.SucursalId}' no encontrada.");
            // Validar existencia de tratamiento
            var tratamiento = await _context.SpaTreatments.FindAsync(dto.SpaTreatmentId);
            if (tratamiento == null)
                return NotFound($"SpaTreatment con Id '{dto.SpaTreatmentId}' no encontrada.");

            // Verificar duplicado
            var existe = await _context.SpaBranchTreatments
                .AnyAsync(sbt => sbt.SucursalId == dto.SucursalId && sbt.SpaTreatmentId == dto.SpaTreatmentId);
            if (existe)
                return Conflict("La asociación ya existe.");

            // Crear
            _context.SpaBranchTreatments.Add(dto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBySucursal), new { sucursalId = dto.SucursalId }, null);
        }

        /// <summary>
        /// DELETE /api/SpaBranchTreatments?sucursalId=1&spaTreatmentId={guid}
        /// Desasocia tratamiento de sucursal.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int sucursalId, [FromQuery] Guid spaTreatmentId)
        {
            var sbt = await _context.SpaBranchTreatments
                .FindAsync(sucursalId, spaTreatmentId);
            if (sbt == null)
                return NotFound("La asociación no existe.");

            _context.SpaBranchTreatments.Remove(sbt);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
