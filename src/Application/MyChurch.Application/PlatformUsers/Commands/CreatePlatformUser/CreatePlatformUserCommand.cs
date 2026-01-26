using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Services;

namespace MyChurch.Application.PlatformUsers.Commands.CreatePlatformUser
{
    public class CreatePlatformUserCommand : PlatformUserJwtDto, IRequest<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Photo { get; set; }
        public UserRole Role { get; set; } = UserRole.PlatformAdmin;
        public bool IsActive { get; set; } = true;
    }

    public class CreatePlatformUserCommandHandler : IRequestHandler<CreatePlatformUserCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;

        public CreatePlatformUserCommandHandler(IUnitOfWork uow, IPasswordHasher passwordHasher)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
        }

        public async Task<int> Handle(CreatePlatformUserCommand request, CancellationToken cancellationToken)
        {
            var actor = await _uow.PlatformUsers.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == request.PlatformUserId, cancellationToken);

            if (actor == null || !actor.IsActive || actor.Role != UserRole.PlatformAdmin)
                ValidationException.ThrowException("PlatformUser", "Only PlatformAdmin can manage platform users.");

            var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email))
                ValidationException.ThrowException("Email", "Email is required.");

            var exists = await _uow.PlatformUsers.Query()
                .AsNoTracking()
                .AnyAsync(u => u.Email.ToLower() == email, cancellationToken);

            if (exists)
                ValidationException.ThrowException("Email", "Email already exists.");

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                ValidationException.ThrowException("Password", "Password must be at least 6 characters.");

            var entity = new PlatformUser
            {
                Name = request.Name?.Trim() ?? string.Empty,
                Email = email,
                Photo = request.Photo,
                IsActive = request.IsActive,
                Role = request.Role,
                Created = DateTime.UtcNow,
                PasswordHash = _passwordHasher.HashPassword(request.Password)
            };

            await _uow.PlatformUsers.Create(entity);
            await _uow.CommitAsync();
            return entity.Id;
        }
    }
}
