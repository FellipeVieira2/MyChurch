using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MyChurch.Application.Church.Commands.UpdateBankingInfo;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Application.Dtos;
using System.Collections.Generic;
using Moq.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore; // Adicionado para usar ReturnsDbSet

namespace MyChurch.Application.Tests.Commands
{
    public class UpdateBankingInfoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldUpdateBankingInfo_WhenAdminMember()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<UpdateBankingInfoCommandHandler>>();
            var member = new Domain.Entities.Member { Id = 1, ChurchId = 10, Role = MyChurch.Domain.Enum.UserRole.Admin };
            var bankingInfos = new List<BankingInfo>();

            // Fix: Use the correct setup for DbSet with Moq
            var mockMemberDbSet = new Mock<DbSet<Domain.Entities.Member>>();
            mockMemberDbSet.As<IQueryable<Domain.Entities.Member>>().Setup(m => m.Provider).Returns(new List<Domain.Entities.Member> { member }.AsQueryable().Provider);
            mockMemberDbSet.As<IQueryable<Domain.Entities.Member>>().Setup(m => m.Expression).Returns(new List<Domain.Entities.Member> { member }.AsQueryable().Expression);
            mockMemberDbSet.As<IQueryable<Domain.Entities.Member>>().Setup(m => m.ElementType).Returns(new List<Domain.Entities.Member> { member }.AsQueryable().ElementType);
            mockMemberDbSet.As<IQueryable<Domain.Entities.Member>>().Setup(m => m.GetEnumerator()).Returns(new List<Domain.Entities.Member> { member }.AsQueryable().GetEnumerator());

            mockUnitOfWork.Setup(u => u.Members.Query()).Returns(mockMemberDbSet.Object);

            var mockBankingInfoDbSet = new Mock<DbSet<BankingInfo>>();
            mockBankingInfoDbSet.As<IQueryable<BankingInfo>>().Setup(m => m.Provider).Returns(bankingInfos.AsQueryable().Provider);
            mockBankingInfoDbSet.As<IQueryable<BankingInfo>>().Setup(m => m.Expression).Returns(bankingInfos.AsQueryable().Expression);
            mockBankingInfoDbSet.As<IQueryable<BankingInfo>>().Setup(m => m.ElementType).Returns(bankingInfos.AsQueryable().ElementType);
            mockBankingInfoDbSet.As<IQueryable<BankingInfo>>().Setup(m => m.GetEnumerator()).Returns(bankingInfos.AsQueryable().GetEnumerator());

            mockUnitOfWork.Setup(u => u.BankingInfos.Query()).Returns(mockBankingInfoDbSet.Object);

            var handler = new UpdateBankingInfoCommandHandler(mockUnitOfWork.Object, mockLogger.Object);
            var command = new UpdateBankingInfoCommand
            {
                UserId = 1,
                BankName = "Banco X",
                Agency = "1234",
                Account = "56789",
                AccountDigit = "0",
                AccountType = "Corrente",
                HolderName = "Titular",
                HolderDocument = "12345678900",
                PixKey = "pix@pix.com",
                PixKeyType = "EMAIL"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            mockUnitOfWork.Verify(u => u.BankingInfos.Create(It.IsAny<BankingInfo>()), Times.Once);
            mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            Assert.Equal("Banco X", result.BankName);
            Assert.Equal("pix@pix.com", result.PixKey);
        }
    }
}
