using Microsoft.Extensions.Logging;
using MyChurch.Domain.Services;

namespace MyChurch.Infrastructure.Services
{
    /// <summary>
    /// Implementação de hashing de senhas usando BCrypt
    /// BCrypt é considerado uma das melhores práticas para hash de senhas
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private readonly ILogger<PasswordHasher> _logger;
        private const int WORK_FACTOR = 12; // Fator de trabalho recomendado (entre 10-13)

        public PasswordHasher(ILogger<PasswordHasher> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gera hash seguro usando BCrypt com work factor 12
        /// O work factor determina quantas iterações são feitas (2^12 = 4096)
        /// </summary>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Attempted to hash null or empty password");
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }

            try
            {
                var hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: WORK_FACTOR);
                _logger.LogDebug("Password hashed successfully");
                return hash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error hashing password");
                throw;
            }
        }

        /// <summary>
        /// Verifica se a senha em texto plano corresponde ao hash armazenado
        /// </summary>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Attempted to verify null or empty password");
                return false;
            }

            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                _logger.LogWarning("Attempted to verify against null or empty hash");
                return false;
            }

            try
            {
                var isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                _logger.LogDebug("Password verification: {Result}", isValid ? "Success" : "Failed");
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying password");
                return false;
            }
        }
    }
}
