using GymTec.Api.Data;
using GymTec.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GymTec.Api.Controllers
{
    /// <summary>
    /// Controlador para CRUD de usuarios (Administradores, Instructores, Dependientes Spa/Tienda, Clientes).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly GymTecContext _context;

        public UsersController(GymTecContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/Users
        /// Obtiene todos los usuarios.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var list = await _context.Users.AsNoTracking().ToListAsync();
            // No devolver PasswordMd5 en lista
            list.ForEach(u => u.PasswordMd5 = null!);
            return Ok(list);
        }

        /// <summary>
        /// GET /api/Users/{id}
        /// Obtiene un usuario por Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<User>> GetById(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound($"Usuario con Id '{id}' no encontrado.");
            user.PasswordMd5 = null!;
            return Ok(user);
        }

        /// <summary>
        /// POST /api/Users
        /// Crea un nuevo usuario. El campo PasswordMd5 llega en texto claro y se convierte a MD5.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] UserCreateDto dto)
        {
            // Validar existencia de cédula o correo duplicado
            if (await _context.Users.AnyAsync(u => u.Cedula == dto.Cedula))
                return Conflict($"Ya existe un usuario con cédula '{dto.Cedula}'.");
            if (await _context.Users.AnyAsync(u => u.Correo == dto.Correo))
                return Conflict($"Ya existe un usuario con correo '{dto.Correo}'.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Cedula = dto.Cedula,
                NombreCompleto = dto.NombreCompleto,
                Provincia = dto.Provincia,
                Canton = dto.Canton,
                Distrito = dto.Distrito,
                Correo = dto.Correo,
                PasswordMd5 = ComputeMd5(dto.Password), // texto claro => MD5
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // No devolver PasswordMd5
            user.PasswordMd5 = null!;
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        /// <summary>
        /// PUT /api/Users/{id}
        /// Actualiza un usuario existente; si se envía Password, se convierte a MD5.
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound($"Usuario con Id '{id}' no encontrado.");

            // Verificar cambios de cédula o correo
            if (!string.IsNullOrWhiteSpace(dto.Cedula) && dto.Cedula != user.Cedula)
            {
                if (await _context.Users.AnyAsync(u => u.Cedula == dto.Cedula && u.Id != id))
                    return Conflict($"Otra cuenta ya usa la cédula '{dto.Cedula}'.");
                user.Cedula = dto.Cedula;
            }
            if (!string.IsNullOrWhiteSpace(dto.Correo) && dto.Correo != user.Correo)
            {
                if (await _context.Users.AnyAsync(u => u.Correo == dto.Correo && u.Id != id))
                    return Conflict($"Otra cuenta ya usa el correo '{dto.Correo}'.");
                user.Correo = dto.Correo;
            }

            // Actualizar otros campos si vienen
            if (!string.IsNullOrWhiteSpace(dto.NombreCompleto))
                user.NombreCompleto = dto.NombreCompleto;
            if (!string.IsNullOrWhiteSpace(dto.Provincia))
                user.Provincia = dto.Provincia;
            if (!string.IsNullOrWhiteSpace(dto.Canton))
                user.Canton = dto.Canton;
            if (!string.IsNullOrWhiteSpace(dto.Distrito))
                user.Distrito = dto.Distrito;

            if (dto.Role.HasValue)
                user.Role = dto.Role.Value;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordMd5 = ComputeMd5(dto.Password);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class AsignarInstructorDto
        {
            public Guid ClienteId { get; set; }
        }

        [HttpGet("clientes-sin-instructor")]
        // [Authorize(Roles = "Instructor")] ← lo comentamos por ahora
        public async Task<ActionResult<IEnumerable<object>>> GetClientesSinInstructor()
        {
            var clientesSinInstructor = await _context.Users
                .Where(u => u.Role == UserRole.Cliente && u.InstructorId == null)
                .Select(u => new
                {
                    u.Id,
                    u.NombreCompleto,
                    u.Correo
                })
                .ToListAsync();

            return Ok(clientesSinInstructor);
        }


        [HttpPut("asignar-instructor")]
        public async Task<IActionResult> AsignarInstructor([FromBody] AsignarInstructorDto dto)
        {
            var instructorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(instructorIdStr, out var instructorId))
                return Unauthorized();

            var cliente = await _context.Users.FindAsync(dto.ClienteId);
            if (cliente == null || cliente.Role != UserRole.Cliente)
                return NotFound("Cliente no encontrado.");

            cliente.InstructorId = instructorId;

            await _context.SaveChangesAsync();
            return Ok("Instructor asignado.");
        }

        /// <summary>
        /// DELETE /api/Users/{id}
        /// Elimina un usuario.
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound($"Usuario con Id '{id}' no encontrado.");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// POST /api/Users/login
        /// Autentica un usuario.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var passwordMd5 = ComputeMd5(login.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Correo == login.Correo && u.PasswordMd5 == passwordMd5);

            if (user == null)
                return Unauthorized("Correo o contraseña inválidos");

            return Ok(new
            {
                token = "token-ficticio-por-ahora",
                user.NombreCompleto,
                user.Role,
                user.Id
            });
        }

        /// <summary>
        /// Calcula MD5 en minúsculas a partir de texto claro.
        /// </summary>
        private static string ComputeMd5(string input)
        {
            using var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    /// <summary>
    /// DTO para creación de usuario.
    /// </summary>
    public class UserCreateDto
    {
        public string Cedula { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string? Provincia { get; set; }
        public string? Canton { get; set; }
        public string? Distrito { get; set; }
        public string Correo { get; set; } = null!;
        public string Password { get; set; } = null!; // texto claro
        public UserRole Role { get; set; }
    }

    /// <summary>
    /// DTO para autenticación de usuario.
    /// </summary>
    public class LoginDto
    {
        public string Correo { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    /// <summary>
    /// DTO para actualización de usuario.
    /// </summary>
    public class UserUpdateDto
    {
        public string? Cedula { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Provincia { get; set; }
        public string? Canton { get; set; }
        public string? Distrito { get; set; }
        public string? Correo { get; set; }
        public string? Password { get; set; } // texto claro
        public UserRole? Role { get; set; }
    }
}
