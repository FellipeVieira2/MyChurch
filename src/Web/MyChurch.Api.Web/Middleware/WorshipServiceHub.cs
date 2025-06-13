using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace MyChurch.Api.Web.Middleware
{
    public class WorshipServiceHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorshipServiceHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Entrar em um grupo do culto (por WorshipServiceId)
        public async Task JoinWorship(int worshipServiceId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"worship_{worshipServiceId}");

            // Busca atividades atuais e envia para o usuário
            var activities = await _unitOfWork.WorshipActivities.Query()
                .Where(a => a.WorshipServiceId == worshipServiceId && a.IsCurrent)
                .Include(a => a.Bibles)
                .Include(a => a.Hymns)
                .ToListAsync();
            var activityDtos = activities.Select(WorshipActivityDto.New); // Fix: Explicitly reference the method with the correct type
            await Clients.Caller.SendAsync("CurrentActivities", activityDtos);
        }

        // Entrar em um grupo de admins do culto
        public async Task JoinWorshipAdmin(int worshipServiceId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"worship_{worshipServiceId}_admin");
        }

        // Sair do grupo do culto
        public async Task LeaveWorship(int worshipServiceId)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"worship_{worshipServiceId}");
    }
}
