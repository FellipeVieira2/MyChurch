using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Church.Queries.SearchNearby
{
    public class GetNearbyChurchesQuery : IRequest<List<NearbyChurchDto>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double RadiusKm { get; set; } = 5;
        public int? MaxResults { get; set; } = 50;
    }

    public class NearbyChurchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double DistanceKm { get; set; }
    }

    public class GetNearbyChurchesQueryHandler : IRequestHandler<GetNearbyChurchesQuery, List<NearbyChurchDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetNearbyChurchesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<NearbyChurchDto>> Handle(GetNearbyChurchesQuery request, CancellationToken cancellationToken)
        {
            const double earthRadiusKm = 6371;
            double latDelta = (request.RadiusKm / earthRadiusKm) * (180 / Math.PI);
            double lonDelta = latDelta / Math.Cos(request.Latitude * Math.PI / 180);

            var minLat = request.Latitude - latDelta;
            var maxLat = request.Latitude + latDelta;
            var minLon = request.Longitude - lonDelta;
            var maxLon = request.Longitude + lonDelta;

            var baseQuery = _unitOfWork.Churchs.Query()
                .Where(c => c.Latitude.HasValue && c.Longitude.HasValue &&
                            c.Latitude.Value >= minLat && c.Latitude.Value <= maxLat &&
                            c.Longitude.Value >= minLon && c.Longitude.Value <= maxLon);

            var temp = await baseQuery
                .Select(c => new { c.Id, c.Name, c.Description, c.Latitude, c.Longitude })
                .ToListAsync(cancellationToken);

            var list = temp
                .Select(c => new NearbyChurchDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,
                    DistanceKm = Haversine(request.Latitude, request.Longitude, c.Latitude!.Value, c.Longitude!.Value)
                })
                .Where(c => c.DistanceKm <= request.RadiusKm)
                .OrderBy(c => c.DistanceKm)
                .Take(request.MaxResults ?? 50)
                .ToList();

            return list;
        }

        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            double dLat = ToRad(lat2 - lat1);
            double dLon = ToRad(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        private static double ToRad(double value) => value * Math.PI / 180;
    }
}
