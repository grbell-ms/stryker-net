using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ProjectVersionInputTests
    {
        [Fact]
        public void ShouldHaveHelptext()
        {
            var target = new ProjectVersionInput();
            target.HelpText.ShouldBe(@"Project version used in reporters. | default: ''");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ProjectVersionCannotBeEmpty(string value)
        {
            var input = new ProjectVersionInput { };
            input.SuppliedInput = value;

            var exception = Should.Throw<InputException>(() => {
                input.Validate(null, reporters: new[] { Reporter.Dashboard }, true);
            });

            exception.Message.ShouldBe("When the stryker dashboard is enabled the project version is required. Please provide a project version.");
        }

        [Theory]
        [InlineData("test")]
        [InlineData("myversion")]
        public void FallbackVersionCannotBeInput(string value)
        {
            var input = new ProjectVersionInput { };
            input.SuppliedInput = value;

            var exception = Should.Throw<InputException>(() => {
                input.Validate(value, reporters: new[] { Reporter.Dashboard }, true);
            });

            exception.Message.ShouldBe("Project version cannot be the same as the fallback version. Please provide a different version for one of them.");
        }
    }
}
