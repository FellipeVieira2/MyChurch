using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Services;

namespace MyChurch.Infrastructure.Services
{
    public class ReviewVerificationService : IReviewVerificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        // Período válido para review após visita (6 meses)
        private const int ValidReviewPeriodInMonths = 6;

        public ReviewVerificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> HasMemberVisitedChurchAsync(int memberId, int churchId, CancellationToken cancellationToken = default)
        {
            // Verifica se existe presença do membro em cultos desta igreja
            var hasPresence = await _unitOfWork.WorshipPresences.Query()
                .AnyAsync(wp => 
                    wp.MemberId == memberId &&
                    _unitOfWork.WorshipServices.Query()
                        .Any(ws => ws.Id == wp.WorshipServiceId && ws.ChurchId == churchId),
                    cancellationToken);

            return hasPresence;
        }

        public async Task<bool> HasVisitorVisitedChurchAsync(int visitorId, int churchId, CancellationToken cancellationToken = default)
        {
            // Verifica se existe presença do visitante em cultos desta igreja
            var hasPresence = await _unitOfWork.WorshipPresences.Query()
                .AnyAsync(wp => 
                    wp.VisitorId == visitorId &&
                    _unitOfWork.WorshipServices.Query()
                        .Any(ws => ws.Id == wp.WorshipServiceId && ws.ChurchId == churchId),
                    cancellationToken);

            return hasPresence;
        }

        public async Task<int?> GetLatestPresenceIdAsync(int memberId, int churchId, CancellationToken cancellationToken = default)
        {
            var latestPresence = await _unitOfWork.WorshipPresences.Query()
                .Where(wp => 
                    wp.MemberId == memberId &&
                    _unitOfWork.WorshipServices.Query()
                        .Any(ws => ws.Id == wp.WorshipServiceId && ws.ChurchId == churchId))
                .OrderByDescending(wp => wp.Timestamp)
                .Select(wp => (int?)wp.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return latestPresence;
        }

        public async Task<(bool canReview, string? reason)> CanMemberReviewChurchAsync(int memberId, int churchId, CancellationToken cancellationToken = default)
        {
            // Verifica se o membro tem presença confirmada
            var latestPresence = await _unitOfWork.WorshipPresences.Query()
                .Where(wp => 
                    wp.MemberId == memberId &&
                    _unitOfWork.WorshipServices.Query()
                        .Any(ws => ws.Id == wp.WorshipServiceId && ws.ChurchId == churchId))
                .OrderByDescending(wp => wp.Timestamp)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestPresence == null)
            {
                return (false, "Você precisa visitar esta igreja antes de avaliá-la.");
            }

            // Verifica se a visita foi nos últimos 6 meses
            var visitCutoffDate = DateTime.UtcNow.AddMonths(-ValidReviewPeriodInMonths);
            if (latestPresence.Timestamp < visitCutoffDate)
            {
                return (false, $"Sua última visita foi há mais de {ValidReviewPeriodInMonths} meses. Visite novamente para poder avaliar.");
            }

            // Verifica se o membro já avaliou esta igreja
            var hasExistingReview = await _unitOfWork.Reviews.Query()
                .AnyAsync(r => 
                    r.ReviewerId == memberId && 
                    r.EntityId == churchId && 
                    r.EntityType == "Church",
                    cancellationToken);

            if (hasExistingReview)
            {
                return (false, "Você já avaliou esta igreja.");
            }

            return (true, null);
        }
    }
}
