namespace MyChurch.Domain.Services
{
    /// <summary>
    /// Interface para validação de documentos brasileiros (CPF, RG, etc.)
    /// </summary>
    public interface IDocumentValidator
    {
        /// <summary>
        /// Valida um CPF
        /// </summary>
        /// <param name="cpf">CPF com ou sem formatação</param>
        /// <returns>True se o CPF for válido</returns>
        bool IsValidCpf(string cpf);

        /// <summary>
        /// Valida um RG
        /// </summary>
        /// <param name="rg">RG com ou sem formatação</param>
        /// <returns>True se o RG for válido</returns>
        bool IsValidRg(string rg);

        /// <summary>
        /// Remove formatação de um documento (pontos, traços, etc.)
        /// </summary>
        /// <param name="document">Documento formatado</param>
        /// <returns>Documento apenas com dígitos</returns>
        string RemoveFormatting(string document);
    }
}
