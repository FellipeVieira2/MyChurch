using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.WorshipService.Commands.Presence
{
    public class RegisterVisitorWorshipPresenceCommand : IRequest<int>
    {
        public int WorshipServiceId { get; set; }
        public int VisitorId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double MaxDistanceMeters { get; set; } = 150;
    }

    public class RegisterVisitorWorshipPresenceCommandHandler : IRequestHandler<RegisterVisitorWorshipPresenceCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RegisterVisitorWorshipPresenceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(RegisterVisitorWorshipPresenceCommand request, CancellationToken cancellationToken)
        {
            if (request.Latitude < -90 || request.Latitude > 90 || request.Longitude < -180 || request.Longitude > 180)
                ValidationException.ThrowException("Geo", "Coordenadas inválidas");

            var worship = await _unitOfWork.WorshipServices.Query().FirstOrDefaultAsync(w => w.Id == request.WorshipServiceId, cancellationToken);
            if (worship == null)
                ValidationException.ThrowException("Worship", "Culto não encontrado");
            if (worship.Status != WorshipServiceStatus.InProgress)
                ValidationException.ThrowException("Worship", "Culto não está em andamento");

            var church = await _unitOfWork.Churchs.Query().FirstOrDefaultAsync(c => c.Id == worship.ChurchId, cancellationToken);
            if (church == null || !church.Latitude.HasValue || !church.Longitude.HasValue)
                ValidationException.ThrowException("Church", "Igreja sem coordenadas");

            var visitor = await _unitOfWork.Visitors.Query().FirstOrDefaultAsync(v => v.Id == request.VisitorId, cancellationToken);
            if (visitor == null)
                ValidationException.ThrowException("Visitor", "Visitante inválido");

            var distance = Haversine(church.Latitude.Value, church.Longitude.Value, request.Latitude, request.Longitude) * 1000.0;
            if (distance > request.MaxDistanceMeters)
                ValidationException.ThrowException("Presence", $"Fora da área. Distância: {distance:F1}m");

            var already = await _unitOfWork.WorshipPresences.Query()
                .AnyAsync(p => p.WorshipServiceId == request.WorshipServiceId && p.VisitorId == request.VisitorId, cancellationToken);
            if (already)
                return 0;

            var presence = new Domain.Entities.WorshipPresence
            {
                WorshipServiceId = request.WorshipServiceId,
                VisitorId = request.VisitorId,
                Timestamp = DateTime.UtcNow,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };
            _unitOfWork.WorshipPresences.Create(presence);

            visitor.LastVisitAt = DateTime.UtcNow;
            visitor.Score += 10;
            if (visitor.Status == null || visitor.Status == VisitorStatus.New)
                visitor.Status = VisitorStatus.Engaging;
            _unitOfWork.Visitors.Update(visitor);

            await _unitOfWork.CommitAsync();
            return presence.Id;
        }
        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0;
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            lat1 = ToRad(lat1);
            lat2 = ToRad(lat2);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        private static double ToRad(double value) => value * Math.PI / 180;
    }
}
