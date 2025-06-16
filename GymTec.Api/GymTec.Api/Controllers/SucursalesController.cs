using System.Collections.Generic;
using System.Threading.Tasks;
using GymTec.Api.Data;
using GymTec.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTec.Api.Controllers
{
    /// <summary>
    /// CRUD de Sucursales.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SucursalesController : ControllerBase
    {
        private readonly GymTecContext _context;

        public SucursalesController(GymTecContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/Sucursales
        /// Obtiene todas las sucursales.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sucursal>>> GetAll()
        {
            return Ok(await _context.Sucursales.AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// GET /api/Sucursales/{id}
        /// Obtiene una sucursal por Id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Sucursal>> GetById(int id)
        {
            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null)
                return NotFound($"Sucursal con Id '{id}' no encontrada.");
            return Ok(sucursal);
        }

        /// <summary>
        /// POST /api/Sucursales
        /// Crea nueva sucursal.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Sucursal>> Create([FromBody] Sucursal dto)
        {
            // Validación mínima: nombre no vacío
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("Nombre de sucursal es requerido.");
            _context.Sucursales.Add(dto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        /// <summary>
        /// PUT /api/Sucursales/{id}
        /// Actualiza sucursal existente.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Sucursal dto)
        {
            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null)
                return NotFound($"Sucursal con Id '{id}' no encontrada.");

            // Actualiza campos si vienen
            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                sucursal.Nombre = dto.Nombre;
            if (!string.IsNullOrWhiteSpace(dto.Provincia))
                sucursal.Provincia = dto.Provincia;
            if (!string.IsNullOrWhiteSpace(dto.Canton))
                sucursal.Canton = dto.Canton;
            if (!string.IsNullOrWhiteSpace(dto.Distrito))
                sucursal.Distrito = dto.Distrito;
            if (!string.IsNullOrWhiteSpace(dto.Direccion))
                sucursal.Direccion = dto.Direccion;
            if (!string.IsNullOrWhiteSpace(dto.Telefono))
                sucursal.Telefono = dto.Telefono;

            _context.Sucursales.Update(sucursal);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// DELETE /api/Sucursales/{id}
        /// Elimina sucursal. Se debe validar que no existan dependencias si es necesario.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null)
                return NotFound($"Sucursal con Id '{id}' no encontrada.");

            // Si existen SpaBranchTreatment o reservas, podrías impedir eliminación:
            var tieneTratamientos = await _context.SpaBranchTreatments.AnyAsync(sbt => sbt.SucursalId == id);
            var tieneReservas = await _context.SpaReservations.AnyAsync(r => r.SucursalId == id);
            if (tieneTratamientos || tieneReservas)
                return BadRequest("No se puede eliminar la sucursal porque tiene tratamientos o reservas asociadas.");

            _context.Sucursales.Remove(sucursal);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
