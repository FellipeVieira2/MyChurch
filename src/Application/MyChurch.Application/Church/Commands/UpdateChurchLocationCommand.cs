using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Infrastructure.Services;

namespace MyChurch.Application.Church.Commands
{
    public class UpdateChurchLocationCommand : IRequest<bool>
    {
        public int ChurchId { get; set; }
    }

    public class UpdateChurchLocationCommandHandler : IRequestHandler<UpdateChurchLocationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly GoogleGeocodingService _geocodingService;
        public UpdateChurchLocationCommandHandler(IUnitOfWork unitOfWork, GoogleGeocodingService geocodingService)
        {
            _unitOfWork = unitOfWork;
            _geocodingService = geocodingService;
        }
        public async Task<bool> Handle(UpdateChurchLocationCommand request, CancellationToken cancellationToken)
        {
            var church = _unitOfWork.Churchs.Query().Include(x => x.Address).FirstOrDefault(x => x.Id == request.ChurchId);
            if (church == null || church.Address == null) return false;
            var address = $"{church.Address.Street}, {church.Address.Number}, {church.Address.Neighborhood}, {church.Address.City}, {church.Address.State}, {church.Address.Country}, {church.Address.ZipCode}";
            (double? lat, double? lng) = await _geocodingService.GetLatLongAsync(address);
            if (lat == null || lng == null) return false;
            church.Latitude = lat;
            church.Longitude = lng;
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
