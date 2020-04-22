using GHACU.CLI;
using Xunit;

namespace GHACU.Tests.CLI
{
    public class OptionValidationExceptionTest
    {
        [Fact]
        public void Create_ReturnsValidMessage()
        {
            var message = "Test Message";
            Assert.Equal(message, new OptionValidationException(message).Message);
        }
    }
}
