using Microsoft.Extensions.Logging;
using Moq;
using MyChurch.Application.Church.Commands.UpdateChurch;
using MyChurch.Application.Tests.Helpers;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Tests.Commands
{
    public class UpdateChurchCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldUpdateChurch_WhenMemberIsValid()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<UpdateChurchCommand.UpdateChurchCommandHandler>>();
            var member = new Domain.Entities.Member { Id = 1, ChurchId = 10 };
            var address = new Address("Rua A", "Cidade B", "SP", "12345-678", "Brasil", "Centro") { Number = "100" };
            var church = new Domain.Entities.Church("OldName", "OldPhone", address, "Desc") { Id = 10 };
            mockUnitOfWork.Setup(u => u.Members.Query()).Returns(new List<Domain.Entities.Member> { member }.AsQueryable().BuildMockDbSet().Object);
            mockUnitOfWork.Setup(u => u.Churchs.Query()).Returns(new List<Domain.Entities.Church> { church }.AsQueryable().BuildMockDbSet().Object);
            var handler = new UpdateChurchCommand.UpdateChurchCommandHandler(mockUnitOfWork.Object, mockLogger.Object);
            var command = new UpdateChurchCommand
            {
                Id = 10,
                UserId = 1,
                Name = "Nova Igreja",
                Phone = "11988887777",

            };
            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.Equal("Nova Igreja", church.Name);
            Assert.Equal("11988887777", church.Phone);
            mockUnitOfWork.Verify(u => u.Churchs.Update(church), Times.Once);
            mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
