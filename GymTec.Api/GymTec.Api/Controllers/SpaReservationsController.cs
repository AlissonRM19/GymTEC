using System;
using System.Threading.Tasks;
using GymTec.Api.Data;
using GymTec.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTec.Api.Controllers
{
    /// <summary>
    /// Controlador para reservas de Spa.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SpaReservationsController : ControllerBase
    {
        private readonly GymTecContext _context;

        public SpaReservationsController(GymTecContext context)
        {
            _context = context;
        }

        /// <summary>
        /// POST /api/SpaReservations
        /// Crea una reserva de Spa. Validaciones:
        /// - User existe y rol Cliente.
        /// - Sucursal existe.
        /// - SpaTreatment existe.
        /// - Tratamiento disponible en sucursal.
        /// - Horario válido: StartTime < EndTime.
        /// - No solapamiento con otra reserva del mismo usuario a la misma hora.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Cliente")] // solo clientes pueden reservar
        public async Task<ActionResult<SpaReservation>> Create([FromBody] SpaReservation dto)
        {
            // 1. Validar UserId coincide con usuario autenticado, o extraer del token:
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdStr, out var userIdToken))
                return Unauthorized("Token inválido.");
            if (dto.UserId != userIdToken)
                return Forbid("No puedes crear reserva para otro usuario.");

            // 2. Validar existencia de usuario en BD y rol Cliente
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null || user.Role != UserRole.Cliente)
                return BadRequest("Usuario inválido o no es Cliente.");

            // 3. Validar existencia de sucursal
            var sucursal = await _context.Sucursales.FindAsync(dto.SucursalId);
            if (sucursal == null)
                return BadRequest($"Sucursal con Id '{dto.SucursalId}' no encontrada.");

            // 4. Validar existencia de tratamiento
            var tratamiento = await _context.SpaTreatments.FindAsync(dto.SpaTreatmentId);
            if (tratamiento == null)
                return BadRequest($"SpaTreatment con Id '{dto.SpaTreatmentId}' no encontrada.");

            // 5. Validar que tratamiento esté disponible en la sucursal
            var disponible = await _context.SpaBranchTreatments
                .AnyAsync(sbt => sbt.SucursalId == dto.SucursalId && sbt.SpaTreatmentId == dto.SpaTreatmentId);
            if (!disponible)
                return BadRequest("Tratamiento no disponible en la sucursal seleccionada.");

            // 6. Validar horarios
            if (dto.EndTime <= dto.StartTime)
                return BadRequest("EndTime debe ser mayor que StartTime.");

            // 7. Validar no solapamiento: ninguna otra reserva del mismo usuario en la misma fecha/hora
            bool solapa = await _context.SpaReservations.AnyAsync(r =>
                r.UserId == dto.UserId
                && r.ReservationDate == dto.ReservationDate
                && ((dto.StartTime >= r.StartTime && dto.StartTime < r.EndTime)
                    || (dto.EndTime > r.StartTime && dto.EndTime <= r.EndTime)
                    || (dto.StartTime <= r.StartTime && dto.EndTime >= r.EndTime))
            );
            if (solapa)
                return BadRequest("Tienes otra reserva que solapa con este horario.");

            // 8. Crear reserva
            dto.Id = Guid.NewGuid();
            _context.SpaReservations.Add(dto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        /// <summary>
        /// GET /api/SpaReservations/{id}
        /// Obtiene reserva por Id (solo quien la creó o admin).
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<SpaReservation>> GetById(Guid id)
        {
            var reserva = await _context.SpaReservations.FindAsync(id);
            if (reserva == null)
                return NotFound("Reserva no encontrada.");

            // Verificar acceso: si es el usuario dueño o rol Administrador
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdStr, out var userIdToken))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userIdToken);
            if (user == null)
                return Unauthorized();

            if (reserva.UserId != userIdToken && user.Role != UserRole.Administrador)
                return Forbid();

            return Ok(reserva);
        }

        // Podrías agregar PUT/DELETE de reservas con validaciones similares.
    }
}
