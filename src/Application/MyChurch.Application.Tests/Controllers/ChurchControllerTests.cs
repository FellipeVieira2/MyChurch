//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using MyChurch.Api.Web.Controllers;
//using MyChurch.Application.Church.Commands.CreateChurchCommand;

//namespace MyChurch.Application.Tests.Controllers
//{
//    public class ChurchControllerTests
//    {
//        [Fact]
//        public async Task CreateChurch_ShouldReturnOk()
//        {
//            // Arrange
//            var mediator = new Mock<IMediator>();
//            mediator.Setup(m => m.Send(It.IsAny<CreateChurchCommand>(), default)).ReturnsAsync(1);
//            var controller = new ChurchController(mediator.Object); // Passa o mock para o construtor
//            var command = new CreateChurchCommand { Name = "Igreja Teste", Description = "Desc", Phone = "11999999999", PlanId = 1, Document = "123", Address = new CreateChurchCommand.AddressChurchCreate { Street = "Rua A", City = "Cidade B", State = "SP", ZipCode = "12345-678", Country = "Brasil", Neighborhood = "Centro", Number = "100" } };
//            // Act
//            var result = await controller.CreateChurch(command);
//            // Assert
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            Assert.Equal(1, okResult.Value);
//        }
//    }
//}
