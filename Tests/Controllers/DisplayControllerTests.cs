using ApiControllers;
using Moq;
using Shared;
using Shared.ApiControllers.Results;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;

namespace Tests.Controllers;

public class DisplayControllerTests : IDisposable
{
    private readonly DisplayController _controller;
    private readonly IControlProvider _provider;

    public DisplayControllerTests()
    {
        var logger = Mock.Of<ILogger<DisplayController>>();
        _provider = Mock.Of<IControlProvider>();

        _controller = new DisplayController(_provider, logger);
    }

    [Fact]
    public void DarkenTest()
    {
        var result = _controller.Darken(null);
        Assert.True(result is OkResult);

        Mock.Get(_provider).Verify(x => x.DisplayOff(), Times.Once);
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
    }
}