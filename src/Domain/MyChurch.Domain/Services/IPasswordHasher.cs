namespace MyChurch.Domain.Services
{
    /// <summary>
    /// Interface para serviço de hashing de senhas
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Gera um hash seguro da senha usando BCrypt
        /// </summary>
        /// <param name="password">Senha em texto plano</param>
        /// <returns>Hash da senha</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifica se a senha corresponde ao hash armazenado
        /// </summary>
        /// <param name="password">Senha em texto plano</param>
        /// <param name="hashedPassword">Hash armazenado no banco</param>
        /// <returns>True se a senha estiver correta</returns>
        bool VerifyPassword(string password, string hashedPassword);
    }
}
