using ApiControllers;
using Moq;
using Shared;
using Shared.ApiControllers.Results;
using Shared.ControlProviders.Provider;
using Shared.Logging.Interfaces;

namespace UnitTests.Controllers;

public class DisplayControllerTests : IDisposable
{
    private readonly DisplayController _controller;
    private readonly Mock<IDisplayControlProvider> _provider;

    public DisplayControllerTests()
    {
        var logger = Mock.Of<ILogger<DisplayController>>();
        _provider = new Mock<IDisplayControlProvider>(MockBehavior.Strict);

        _controller = new DisplayController(_provider.Object, logger);
    }

    [Fact]
    public void DarkenTest()
    {
        _provider.Setup(x => x.DisplayOff());

        var result = _controller.Darken(null);
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

        var methods = _controller.GetMethods();
        Assert.True(methods.Count == methodNames.Length && methods.All(x => methodNames.Contains(x.Key)) && methods.All(
            x => x.Value.Target == _controller && x.Value.Method.ReturnType == typeof(IActionResult) &&
                 x.Value.Method.GetParameters().Length == 1 &&
                 x.Value.Method.GetParameters()[0].ParameterType == typeof(string)));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}