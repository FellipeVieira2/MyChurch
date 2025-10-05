using MyChurch.Domain.Services;
using System.Text.RegularExpressions;

namespace MyChurch.Infrastructure.Services
{
    /// <summary>
    /// Implementação do validador de documentos brasileiros
    /// </summary>
    public class DocumentValidator : IDocumentValidator
    {
        /// <summary>
        /// Valida um CPF brasileiro
        /// </summary>
        public bool IsValidCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Remove formatação
            cpf = RemoveFormatting(cpf);

            // CPF deve ter 11 dígitos
            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais (CPF inválido)
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Valida primeiro dígito verificador
            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += int.Parse(cpf[i].ToString()) * (10 - i);

            int remainder = sum % 11;
            int firstDigit = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cpf[9].ToString()) != firstDigit)
                return false;

            // Valida segundo dígito verificador
            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(cpf[i].ToString()) * (11 - i);

            remainder = sum % 11;
            int secondDigit = remainder < 2 ? 0 : 11 - remainder;

            return int.Parse(cpf[10].ToString()) == secondDigit;
        }

        /// <summary>
        /// Valida um RG brasileiro
        /// Nota: RG não possui algoritmo de validação padrão em todo o Brasil,
        /// então validamos apenas o formato básico
        /// </summary>
        public bool IsValidRg(string rg)
        {
            if (string.IsNullOrWhiteSpace(rg))
                return false;

            // Remove formatação
            rg = RemoveFormatting(rg);

            // RG deve ter entre 7 e 9 dígitos (pode variar por estado)
            if (rg.Length < 7 || rg.Length > 9)
                return false;

            // Verifica se contém apenas dígitos (alguns RGs podem ter letra no final, mas vamos aceitar apenas números)
            if (!rg.All(char.IsDigit))
                return false;

            // Verifica se não são todos dígitos iguais
            if (rg.All(c => c == rg[0]))
                return false;

            return true;
        }

        /// <summary>
        /// Remove formatação de documentos (pontos, traços, barras)
        /// </summary>
        public string RemoveFormatting(string document)
        {
            if (string.IsNullOrWhiteSpace(document))
                return string.Empty;

            return Regex.Replace(document, @"[^\d]", string.Empty);
        }
    }
}
