using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MyChurch.Domain.Entities;

namespace MyChurch.Domain.Contracts
{
    public interface IEngagementEventRepository : IGenericRepository<EngagementEvent>
    {
        Task<int> CalculateScoreForMemberAsync(int memberId, int days);
    }
}
