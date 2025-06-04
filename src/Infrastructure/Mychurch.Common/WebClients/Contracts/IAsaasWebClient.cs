using Mychurch.Common.WebClients.Asaas.Models.Requests;
using Mychurch.Common.WebClients.Asaas.Models.Responses;

namespace Mychurch.Common.WebClients.Asaas
{
    public interface IAsaasWebClient
    {
        Task<AsaasCustomerResponseDto> CriarClienteAsync(AsaasCustomerRequestDto clienteRequest);
        Task<object> ConsultarClienteAsync(string id);
        Task<AsaasPaymentResponseDto> CriarCobrancaAsync(object cobrancaRequest);
        Task<object> ConsultarCobrancaAsync(string id);
        Task<object> CancelarCobrancaAsync(string id);
        Task<object> GerarPixQrCodeAsync(string paymentId);
        Task<object> CriarAssinaturaAsync(object assinaturaRequest);
        Task<object> ConsultarAssinaturaAsync(string id);
        Task<object> CancelarAssinaturaAsync(string id);
        Task<object> CriarTransferenciaAsync(object transferenciaRequest);
        Task<object> ConsultarTransferenciaAsync(string id);
    }
}
