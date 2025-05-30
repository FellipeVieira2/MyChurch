namespace Mychurch.Common.WebClients.Asaas
{
    public interface IAsaasWebClient
    {
        Task<object> CriarClienteAsync(object clienteRequest);
        Task<object> ConsultarClienteAsync(string id);

        Task<object> CriarCobrancaAsync(object cobrancaRequest);
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
