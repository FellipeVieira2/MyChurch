using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using QRCoder;

namespace MyChurch.Application.Church.Commands.GenerateOnboardingQrCode
{
    public class GenerateOnboardingQrCodeCommand : IRequest<string>
    {
        public int ChurchId { get; set; }
        public GenerateOnboardingQrCodeCommand(int churchId)
        {
            ChurchId = churchId;
        }
        public class GenerateOnboardingQrCodeCommandHandler : IRequestHandler<GenerateOnboardingQrCodeCommand, string>
        {
            private readonly IUnitOfWork _unitOfWork;
            public GenerateOnboardingQrCodeCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<string> Handle(GenerateOnboardingQrCodeCommand request, CancellationToken cancellationToken)
            {
                var church = _unitOfWork.Churchs.Query().FirstOrDefault(x => x.Id == request.ChurchId);
                if (church == null)
                    ValidationException.ThrowException("Church", "Igreja não encontrada");

                if (string.IsNullOrEmpty(church.OnboardingQrCode))
                {
                    var onboardingUrl = $"https://www.mychurchlab.net/onboarding?church={church.Id}";
                    church.OnboardingQrCode = GenerateQrCodeBase64(onboardingUrl);
                    _unitOfWork.Churchs.Update(church);
                    await _unitOfWork.CommitAsync();
                }
                return church.OnboardingQrCode;
            }

            private string GenerateQrCodeBase64(string url)
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeBytes = qrCode.GetGraphic(20);
                return "data:image/png;base64," + Convert.ToBase64String(qrCodeBytes);
            }
        }
    }
}
