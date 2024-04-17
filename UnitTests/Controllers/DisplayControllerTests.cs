using MainApp.ControlProviders.Interfaces;
using MainApp.Servers.ApiControllers;
using MainApp.Servers.Middleware;
using MainApp.Servers.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Controllers;

public class DisplayControllerTests : IDisposable
{
    private readonly DisplayController _controller;
    private readonly Mock<IDisplayControl> _provider;

    public DisplayControllerTests()
    {
        var logger = Mock.Of<ILogger>();
        _provider = new Mock<IDisplayControl>(MockBehavior.Strict);

        _controller = new DisplayController(_provider.Object, logger);
    }

    [Fact]
    public void DarkenTest()
    {
        _provider.Setup(x => x.DisplayOff());

        var result = _controller.Darken();
        Assert.True(result is OkResult);

        _provider.Verify(x => x.DisplayOff(), Times.Once);
    }

    [Fact]
    public void GetMethodsTest()
    {
        var methodNames = new[]
        {
            "darken"
        };

        var methods = _controller.GetActions();
        Assert.True(methods.Count == methodNames.Length && methods.All(x => methodNames.Contains(x.Key)) && methods.All(
            x => x.Value.Method.ReturnType == typeof(IActionResult)));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}