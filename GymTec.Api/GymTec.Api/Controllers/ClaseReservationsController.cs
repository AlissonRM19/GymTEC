using System;
using System.Linq;
using System.Threading.Tasks;
using GymTec.Api.Data;
using GymTec.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTec.Api.Controllers
{
    /// <summary>
    /// Controlador para reservas de Clases grupales.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClaseReservationsController : ControllerBase
    {
        private readonly GymTecContext _context;

        public ClaseReservationsController(GymTecContext context)
        {
            _context = context;
        }

        /// <summary>
        /// POST /api/ClaseReservations
        /// Crea una reserva de clase. Validaciones:
        /// - User existe y rol Cliente.
        /// - Clase existe, es grupal y no está llena.
        /// - No duplicar reserva del mismo usuario para la misma clase.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult<ClaseReservation>> Create([FromBody] ClaseReservation dto)
        {
            // 1. Validar usuario autenticado coincide
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdStr, out var userIdToken))
                return Unauthorized();
            if (dto.UserId != userIdToken)
                return Forbid("No puedes reservar para otro usuario.");

            // 2. Validar existencia de usuario y rol Cliente
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null || user.Role != UserRole.Cliente)
                return BadRequest("Usuario inválido o no es Cliente.");

            // 3. Validar existencia de clase
            var clase = await _context.Clases.FindAsync(dto.ClaseId);
            if (clase == null)
                return BadRequest("Clase no encontrada.");

            // 4. Validar que sea clase grupal
            if (!clase.IsGrupal)
                return BadRequest("No puedes reservar una clase que no es grupal.");

            // 5. Validar fecha de reserva coincide con la fecha de la clase
            if (dto.ReservationDate.Date != clase.Fecha.Date)
                return BadRequest("La fecha de reserva debe coincidir con la fecha de la clase.");

            // 6. Validar capacidad: contar reservas existentes para esa clase
            var countReservas = await _context.ClaseReservations
                .CountAsync(r => r.ClaseId == dto.ClaseId && r.ReservationDate.Date == dto.ReservationDate.Date);
            if (countReservas >= clase.Capacidad)
                return BadRequest("La clase ya está llena.");

            // 7. Validar que el usuario no haya reservado ya la misma clase
            bool yaReservado = await _context.ClaseReservations.AnyAsync(r =>
                r.UserId == dto.UserId && r.ClaseId == dto.ClaseId && r.ReservationDate.Date == dto.ReservationDate.Date);
            if (yaReservado)
                return BadRequest("Ya reservaste esta clase.");

            // 8. Crear reserva
            dto.Id = Guid.NewGuid();
            _context.ClaseReservations.Add(dto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        /// <summary>
        /// GET /api/ClaseReservations/{id}
        /// Obtiene reserva de clase (dueño o admin).
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<ClaseReservation>> GetById(Guid id)
        {
            var reserva = await _context.ClaseReservations.FindAsync(id);
            if (reserva == null)
                return NotFound();
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (!Guid.TryParse(userIdStr, out var userIdToken))
                return Unauthorized();
            var user = await _context.Users.FindAsync(userIdToken);
            if (reserva.UserId != userIdToken && user?.Role != UserRole.Administrador)
                return Forbid();
            return Ok(reserva);
        }

        // PUT/DELETE análogos si se requiere cancelar reserva antes de cierto plazo, etc.
    }
}
