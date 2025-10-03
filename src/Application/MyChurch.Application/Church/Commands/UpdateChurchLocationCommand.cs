using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Infrastructure.Services;
using FluentValidation;

namespace MyChurch.Application.Church.Commands
{
    public class UpdateChurchLocationCommand : IRequest<UpdateChurchLocationResult>
    {
        public int ChurchId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool AutoGeocode { get; set; } = false; // Se true, ignora lat/lng e busca via endereço
    }

    public class UpdateChurchLocationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class UpdateChurchLocationCommandValidator : AbstractValidator<UpdateChurchLocationCommand>
    {
        public UpdateChurchLocationCommandValidator()
        {
            RuleFor(x => x.ChurchId)
                .GreaterThan(0)
                .WithMessage("ID da igreja inválido");

            When(x => !x.AutoGeocode, () =>
            {
                RuleFor(x => x.Latitude)
                    .InclusiveBetween(-90, 90)
                    .When(x => x.Latitude.HasValue)
                    .WithMessage("Latitude deve estar entre -90 e 90");

                RuleFor(x => x.Longitude)
                    .InclusiveBetween(-180, 180)
                    .When(x => x.Longitude.HasValue)
                    .WithMessage("Longitude deve estar entre -180 e 180");
            });
        }
    }

    public class UpdateChurchLocationCommandHandler : IRequestHandler<UpdateChurchLocationCommand, UpdateChurchLocationResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly GoogleGeocodingService _geocodingService;

        public UpdateChurchLocationCommandHandler(IUnitOfWork unitOfWork, GoogleGeocodingService geocodingService)
        {
            _unitOfWork = unitOfWork;
            _geocodingService = geocodingService;
        }

        public async Task<UpdateChurchLocationResult> Handle(UpdateChurchLocationCommand request, CancellationToken cancellationToken)
        {
            var church = await _unitOfWork.Churchs.Query()
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Id == request.ChurchId, cancellationToken);

            if (church == null)
            {
                return new UpdateChurchLocationResult
                {
                    Success = false,
                    Message = "Igreja não encontrada"
                };
            }

            double? latitude = request.Latitude;
            double? longitude = request.Longitude;

            // Se AutoGeocode = true, busca coordenadas via endereço
            if (request.AutoGeocode && church.Address != null)
            {
                try
                {
                    var address = $"{church.Address.Street}, {church.Address.Number}, {church.Address.Neighborhood}, {church.Address.City}, {church.Address.State}, {church.Address.Country}, {church.Address.ZipCode}";
                    (latitude, longitude) = await _geocodingService.GetLatLongAsync(address);

                    if (!latitude.HasValue || !longitude.HasValue)
                    {
                        return new UpdateChurchLocationResult
                        {
                            Success = false,
                            Message = "Não foi possível geocodificar o endereço"
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new UpdateChurchLocationResult
                    {
                        Success = false,
                        Message = $"Erro ao geocodificar: {ex.Message}"
                    };
                }
            }

            // Atualiza usando método validado da entidade
            try
            {
                church.UpdateLocation(latitude, longitude);
                await _unitOfWork.CommitAsync();

                return new UpdateChurchLocationResult
                {
                    Success = true,
                    Message = "Localização atualizada com sucesso",
                    Latitude = church.Latitude,
                    Longitude = church.Longitude
                };
            }
            catch (ArgumentException ex)
            {
                return new UpdateChurchLocationResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
