using ApiControllers;
using Moq;
using Shared.ApiControllers.Results;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;

namespace Tests;

public class DisplayControllerTests: IDisposable
{
    public DisplayControllerTests()
    {

    }

    [Fact]
    public void DarkenTest()
    {
        var logger = Mock.Of<ILogger<DisplayController>>();
        var displayProvider = Mock.Of<IDisplayControlProvider>();

        var displayController = new DisplayController(displayProvider, logger);

        var result = displayController.Darken(null);
        Assert.True(result is OkResult);

        Mock.Get(displayProvider).Verify(x => x.Darken(), Times.Once);
    }

    public void Dispose()
    {
    }
}