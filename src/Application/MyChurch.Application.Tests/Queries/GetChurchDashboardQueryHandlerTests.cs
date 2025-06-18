using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MyChurch.Application.Church.Queries.GetChurch;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using System.Collections.Generic;
using System.Linq;
using MyChurch.Application.Tests.Helpers;

namespace MyChurch.Application.Tests.Queries
{
    public class GetChurchDashboardQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnDashboardDto_WhenMemberIsAdmin()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<GetChurchDashboardQueryHandler>>();
            var address = new Address("Rua A", "Cidade B", "SP", "12345-678", "Brasil", "Centro") { Number = "100" };
            var member = new Domain.Entities.Member { Id = 1, ChurchId = 10, Role = UserRole.Admin, IsActive = true, Church = null };
            var church = new Domain.Entities.Church("Igreja Teste", "11999999999", address, "Desc")
            {
                Id = 10,
                Members = new List<Domain.Entities.Member> { member },
                Events = new List<Domain.Entities.Event>(),
                CashFlowEntries = new List<CashFlowEntry>()
            };
            member.Church = church;
            mockUnitOfWork.Setup(u => u.Members.Query()).Returns(new List<Domain.Entities.Member> { member }.AsQueryable().BuildMockDbSet().Object);
            mockUnitOfWork.Setup(u => u.Churchs.Query()).Returns(new List<Domain.Entities.Church> { church }.AsQueryable().BuildMockDbSet().Object);
            mockUnitOfWork.Setup(u => u.Donations.Query()).Returns(new List<Domain.Entities.Donation>().AsQueryable().BuildMockDbSet().Object);
            var handler = new GetChurchDashboardQueryHandler(mockUnitOfWork.Object, mockLogger.Object);
            var query = new GetChurchDashboardQuery { UserId = 1 };
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            Assert.Equal(1, result.TotalActiveMembers);
            Assert.Equal(0, result.TotalEvents);
        }
    }
}
