// Controllers/AuthController.cs

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GymTec.Api.Data;
using GymTec.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GymTec.Api.Controllers
{
    /// <summary>
    /// Controlador para autenticación y generación de JWT.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly GymTecContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthController(GymTecContext context, IOptions<JwtSettings> jwtOptions)
        {
            _context = context;
            _jwtSettings = jwtOptions.Value;
        }

        /// <summary>
        /// POST /api/Auth/login
        /// Recibe credenciales (correo + contraseña clara), valida y retorna JWT si es correcto.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Correo y contraseña son requeridos.");

            // Hash MD5 de la contraseña clara
            var hash = ComputeMd5(request.Password);

            // Buscar usuario con ese correo y hash
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Correo == request.Correo && u.PasswordMd5 == hash);

            if (user == null)
                return Unauthorized("Credenciales inválidas.");

            // Generar token JWT
            var token = GenerateJwtToken(user);

            return Ok(new LoginResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            });
        }

        /// <summary>
        /// Genera el JWT para el usuario autenticado.
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            // Claims: sub = user.Id, email, role
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Correo),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Calcula MD5 en minúsculas de input.
        /// </summary>
        private static string ComputeMd5(string input)
        {
            using var md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    /// <summary>
    /// DTO para petición de login.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Correo electrónico registrado.
        /// </summary>
        public string Correo { get; set; } = null!;
        /// <summary>
        /// Contraseña en texto claro.
        /// </summary>
        public string Password { get; set; } = null!;
    }

    /// <summary>
    /// DTO para respuesta de login con token.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// Token JWT.
        /// </summary>
        public string Token { get; set; } = null!;
        /// <summary>
        /// Fecha UTC de expiración del token.
        /// </summary>
        public DateTime Expiration { get; set; }
    }
}

