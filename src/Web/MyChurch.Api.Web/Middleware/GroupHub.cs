using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;
using MyChurch.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Api.Web.Middleware
{
    public class GroupHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        public GroupHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task JoinGroup(int groupId)
        {
            var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                var isMember = await _unitOfWork.GroupMembers.Query().AnyAsync(x => x.GroupId == groupId && x.MemberId == userId);
                if (isMember)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
                }
            }
        }
        public async Task LeaveGroup(int groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString());
        }
    }
}
