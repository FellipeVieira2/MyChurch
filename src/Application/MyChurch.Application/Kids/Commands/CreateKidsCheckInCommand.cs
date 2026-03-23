using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using QRCoder;

namespace MyChurch.Application.Kids.Commands
{
    public class CreateKidsCheckInCommand : JwtMemberDto, IRequest<KidsCheckInDto>
    {
        public int ChildId { get; set; }
        public string EnvironmentName { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class CreateKidsCheckInCommandHandler : IRequestHandler<CreateKidsCheckInCommand, KidsCheckInDto>
    {
        private readonly IUnitOfWork _uow;

        public CreateKidsCheckInCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<KidsCheckInDto> Handle(CreateKidsCheckInCommand request, CancellationToken cancellationToken)
        {
            var actor = await _uow.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (actor == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");

            if (!actor.FamilyId.HasValue)
                ValidationException.ThrowException("Family", "Você precisa estar vinculado a uma família para realizar o check-in.");

            var child = await _uow.Children.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ChildId && x.FamilyId == actor.FamilyId.Value && x.IsActive, cancellationToken);

            if (child == null)
                ValidationException.ThrowException("Child", "Criança não encontrada para a sua família.");

            var environmentName = request.EnvironmentName?.Trim();
            if (string.IsNullOrWhiteSpace(environmentName))
                ValidationException.ThrowException("EnvironmentName", "Informe a ala ou ambiente do check-in.");

            var hasActiveCheckIn = await _uow.KidsCheckIns.Query()
                .AsNoTracking()
                .AnyAsync(x => x.ChildId == request.ChildId && x.CheckedOutAt == null, cancellationToken);

            if (hasActiveCheckIn)
                ValidationException.ThrowException("CheckIn", "Já existe um check-in ativo para esta criança.");

            var checkedInAt = DateTime.UtcNow;
            var token = Guid.NewGuid().ToString("N");

            var checkIn = new KidsCheckIn
            {
                ChildId = child.Id,
                ChurchId = actor.ChurchId,
                CheckedInByMemberId = actor.Id,
                CheckedInAt = checkedInAt,
                EnvironmentName = environmentName,
                PickupToken = token,
                PickupTokenExpiresAt = checkedInAt.AddHours(12),
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
            };

            await _uow.KidsCheckIns.Create(checkIn);
            await _uow.CommitAsync();

            return new KidsCheckInDto
            {
                Id = checkIn.Id,
                ChildId = child.Id,
                ChildName = child.FullName,
                EnvironmentName = checkIn.EnvironmentName,
                CheckedInAt = checkIn.CheckedInAt,
                PickupTokenExpiresAt = checkIn.PickupTokenExpiresAt,
                PickupToken = token,
                QrCodeBase64 = GenerateQrCode(token),
                Notes = checkIn.Notes
            };
        }

        private static string GenerateQrCode(string payload)
        {
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(data);
            var bytes = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
        }
    }
}
