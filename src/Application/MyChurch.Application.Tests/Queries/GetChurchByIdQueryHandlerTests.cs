using Microsoft.Extensions.Logging;
using Moq;
using MyChurch.Application.Church.Queries.GetChurch;
using MyChurch.Application.Tests.Helpers;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Tests.Queries
{
    public class GetChurchByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnChurchDto_WhenMemberIsAdmin()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<GetChurchByIdQueryHandler>>();
            var address = new Address("Rua A", "Cidade B", "SP", "12345-678", "Brasil", "Centro") { Number = "100" };
            var member = new Domain.Entities.Member { Id = 1, Role = MyChurch.Domain.Enum.UserRole.Admin };
            var church = new Domain.Entities.Church("Igreja Teste", "11999999999", address, "Desc")
            {
                Id = 10,
                Members = new List<Domain.Entities.Member> { member },
                Address = address
            };
            mockUnitOfWork.Setup(u => u.Churchs.Query()).Returns(new List<Domain.Entities.Church> { church }.AsQueryable().BuildMockDbSet().Object);
            var handler = new GetChurchByIdQueryHandler(mockUnitOfWork.Object, mockLogger.Object);
            var query = new GetChurchByIdQuery { Id = 10, UserId = 1 };
            // Act
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            Assert.Equal(10, result.Id);
            Assert.Equal("Igreja Teste", result.Name);
            Assert.NotNull(result.Members);
        }
    }
}
