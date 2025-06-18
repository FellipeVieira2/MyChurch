using System;
using Xunit;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Tests.Entities
{
    public class ChurchTests
    {
        [Fact]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var address = new Address("Rua A", "Cidade B", "SP", "12345-678", "Brasil", "Centro") { Number = "100" };
            var name = "Igreja Teste";
            var phone = "11999999999";
            var description = "Descrição da igreja";

            // Act
            var church = new Domain.Entities.Church(name, phone, address, description);

            // Assert
            Assert.Equal(name, church.Name);
            Assert.Equal(phone, church.Phone);
            Assert.Equal(address, church.Address);
            Assert.Equal(description, church.Description);
            Assert.True((DateTime.UtcNow - church.Created).TotalSeconds < 2);
            Assert.Equal(0.05m, church.PlatformFee);
        }

        [Fact]
        public void Update_ShouldUpdateNameAndPhoneAndSetUpdated()
        {
            // Arrange
            var address = new Address("Rua A", "Cidade B", "SP", "12345-678", "Brasil", "Centro") { Number = "100" };
            var church = new Domain.Entities.Church("OldName", "OldPhone", address, "Desc");
            var oldCreated = church.Created;

            // Act
            church.Update("NewName", "NewPhone");

            // Assert
            Assert.Equal("NewName", church.Name);
            Assert.Equal("NewPhone", church.Phone);
            Assert.NotNull(church.Updated);
            Assert.True(church.Updated > oldCreated);
        }

        [Fact]
        public void UpdateLogo_ShouldSetLogoFileNameAndUpdated()
        {
            // Arrange
            var address = new Address("Rua A", "Cidade B", "SP", "12345-678", "Brasil", "Centro") { Number = "100" };
            var church = new Domain.Entities.Church("Name", "Phone", address, "Desc");

            // Act
            church.UpdateLogo("logo.png");

            // Assert
            Assert.Equal("logo.png", church.LogoFileName);
            Assert.NotNull(church.Updated);
        }
    }
}
