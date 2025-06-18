using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MyChurch.Application.Church.Commands.CreateChurchWithAdminMember;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Application.Subscription.Commands.CreateSubscription;
using Mychurch.Common.WebClients.Asaas;
using MyChurch.Infrastructure.Utils.S3;
using MyChurch.Application.Dtos;
using System.Collections.Generic;

public class CreateChurchWithAdminMemberCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateChurchAndAdminMember()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockSender = new Mock<MediatR.ISender>();
        var mockLogger = new Mock<ILogger<CreateChurchWithAdminMemberCommandHandler>>();
        var mockAsaasWebClient = new Mock<IAsaasWebClient>();
        var mockS3Helper = new Mock<IS3Helper>();

        mockUnitOfWork.Setup(u => u.Churchs.Create(It.IsAny<Church>())).Verifiable();
        mockUnitOfWork.Setup(u => u.Members.Create(It.IsAny<Member>())).Verifiable();
        mockUnitOfWork.Setup(u => u.Churchs.Update(It.IsAny<Church>())).Verifiable();
        mockUnitOfWork.Setup(u => u.CommitAsync()).Returns((Task<bool>)Task.CompletedTask);
        mockAsaasWebClient.Setup(a => a.CriarClienteAsync(It.IsAny<Mychurch.Common.WebClients.Asaas.Models.Requests.AsaasCustomerRequestDto>()))
            .ReturnsAsync(new Mychurch.Common.WebClients.Asaas.Models.Responses.AsaasCustomerResponseDto { Id = "asaas-customer-id" });
        mockS3Helper.Setup(s => s.UploadFileAsync(It.IsAny<System.IO.Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("logo-url");
        mockSender.Setup(s => s.Send(It.IsAny<CreateSubscriptionCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new CreateSubscriptionResultDto { CheckoutUrl = "checkout-url", PixQrCode = null, Payload = null });

        var handler = new CreateChurchWithAdminMemberCommandHandler(
            mockUnitOfWork.Object,
            mockLogger.Object,
            mockS3Helper.Object,
            mockAsaasWebClient.Object,
            mockSender.Object
        );

        var command = new CreateChurchWithAdminMemberCommand
        {
            Name = "Igreja Teste",
            Description = "Desc",
            Phone = "11999999999",
            PlanId = 1,
            BillingType = "PIX",
            Logo = null,
            Address = new CreateChurchWithAdminMemberCommand.AddressChurchWithAdminCreate
            {
                Street = "Rua A",
                City = "Cidade B",
                State = "SP",
                ZipCode = "12345-678",
                Country = "Brasil",
                Neighborhood = "Centro",
                Number = "100"
            },
            AdminName = "Admin Teste",
            AdminEmail = "admin@teste.com",
            AdminDocuments = new List<CreateChurchWithAdminMemberCommand.MemberDocumentDtoCreateChurch> {
                new CreateChurchWithAdminMemberCommand.MemberDocumentDtoCreateChurch { Type = MemberDocumentType.CPF, Number = "12345678900" }
            },
            Document = "12345678900",
            AdminPhone = "11999999999",
            AdminBirthDate = DateTime.UtcNow.AddYears(-30),
            AdminIsBaptized = true,
            AdminBaptizedDate = DateTime.UtcNow.AddYears(-10),
            AdminIsTither = true,
            AdminPassword = "senha123",
            AdminBirthCity = "Cidade B",
            AdminBirthState = "SP",
            Ministry = "Ministério",
            MemberSince = DateTime.UtcNow.AddYears(-1),
            Notes = "Notas",
            AdminAddress = new CreateChurchWithAdminMemberCommand.AddressChurchWithAdminCreate
            {
                Street = "Rua A",
                City = "Cidade B",
                State = "SP",
                ZipCode = "12345-678",
                Country = "Brasil",
                Neighborhood = "Centro",
                Number = "100"
            },
            MaritalStatus = MyChurch.Domain.Enum.MaritalStatus.Divorciado
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("checkout-url", result.CheckoutUrl);
        mockUnitOfWork.Verify(u => u.Churchs.Create(It.IsAny<Church>()), Times.Once);
        mockUnitOfWork.Verify(u => u.Members.Create(It.IsAny<Member>()), Times.Once);
        mockUnitOfWork.Verify(u => u.CommitAsync(), Times.AtLeastOnce);
    }
}