namespace GymTec.Api.Models
{
    /// <summary>
    /// Configuración para JWT: clave secreta, issuer, audience y tiempo de expiración en minutos.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Clave secreta para firmar el token. Debe ser suficientemente larga y segura.
        /// </summary>
        public string SecretKey { get; set; } = null!;

        /// <summary>
        /// Issuer (emisor) del token.
        /// </summary>
        public string Issuer { get; set; } = null!;

        /// <summary>
        /// Audience (destinatario esperado) del token.
        /// </summary>
        public string Audience { get; set; } = null!;

        /// <summary>
        /// Duración en minutos del token antes de expirar.
        /// </summary>
        public int ExpiryMinutes { get; set; }
    }
}
