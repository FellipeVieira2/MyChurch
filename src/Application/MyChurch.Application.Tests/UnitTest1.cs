using Xunit;

namespace MyChurch.Application.Tests
{
    public class SampleTests
    {
        [Fact]
        public void SampleTest_ShouldPass()
        {
            // Arrange
            int a = 2;
            int b = 2;

            // Act
            int result = a + b;

            // Assert
            Assert.Equal(4, result);
        }
    }
}
