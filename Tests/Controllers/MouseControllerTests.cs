using ApiControllers;
using Moq;
using Shared;
using Shared.ApiControllers.Results;
using Shared.ControlProviders.Provider;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace UnitTests.Controllers;

public class MouseControllerTests : IDisposable
{
    private readonly MouseController _mouseController;
    private readonly Mock<IMouseControlProvider> _mouseControlProvider;
    private readonly ILogger<MouseController> _logger;

    public MouseControllerTests()
    {
        _logger = Mock.Of<ILogger<MouseController>>();
        _mouseControlProvider = new Mock<IMouseControlProvider>(MockBehavior.Strict);
        _mouseController = new MouseController(_mouseControlProvider.Object, _logger);
    }

    [Fact]
    public void LeftTest()
    {
        _mouseControlProvider.Setup(x => x.MouseKeyPress(MouseButtons.Left, KeyPressMode.Click));

        var result = _mouseController.Left(null);
        Assert.True(result is OkResult);
        _mouseControlProvider.Verify(x => x.MouseKeyPress(MouseButtons.Left, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void RightTest()
    {
        _mouseControlProvider.Setup(x => x.MouseKeyPress(MouseButtons.Right, KeyPressMode.Click));

        var result = _mouseController.Right(null);
        Assert.True(result is OkResult);
        _mouseControlProvider.Verify(x => x.MouseKeyPress(MouseButtons.Right, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void MiddleTest()
    {
        _mouseControlProvider.Setup(x => x.MouseKeyPress(MouseButtons.Middle, KeyPressMode.Click));

        var result = _mouseController.Middle(null);
        Assert.True(result is OkResult);
        _mouseControlProvider.Verify(x => x.MouseKeyPress(MouseButtons.Middle, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void WheelUpTest()
    {
        _mouseControlProvider.Setup(x => x.MouseWheel(true));

        var result = _mouseController.WheelUp(null);
        Assert.True(result is OkResult);
        _mouseControlProvider.Verify(x => x.MouseWheel(true), Times.Once);
    }

    [Fact]
    public void WheelDownTest()
    {
        _mouseControlProvider.Setup(x => x.MouseWheel(false));

        var result = _mouseController.WheelDown(null);
        Assert.True(result is OkResult);
        _mouseControlProvider.Verify(x => x.MouseWheel(false), Times.Once);
    }

    [Fact]
    public void DragStartTest()
    {
        _mouseControlProvider.Setup(x => x.MouseKeyPress(MouseButtons.Left, KeyPressMode.Down));

        var result = _mouseController.DragStart(null);
        Assert.True(result is OkResult);
        _mouseControlProvider.Verify(x => x.MouseKeyPress(MouseButtons.Left, KeyPressMode.Down), Times.Once);
    }

    [Fact]
    public void DragStopTest()
    {
        _mouseControlProvider.Setup(x => x.MouseKeyPress(MouseButtons.Left, KeyPressMode.Up));

        var result = _mouseController.DragStop(null);
        Assert.True(result is OkResult);
        _mouseControlProvider.Verify(x => x.MouseKeyPress(MouseButtons.Left, KeyPressMode.Up), Times.Once);
    }

    [Theory]
    [InlineData("{ x: 11, y: 12 }", 11, 12)]
    [InlineData("34, 44", 34, 44)]
    [InlineData("213 -1", 213, -1)]
    public void MoveTest(string input, int x, int y)
    {
        _mouseControlProvider.Setup(p => p.MouseMove(It.IsAny<int>(), It.IsAny<int>()));

        var result = _mouseController.Move(input);
        Assert.True(result is OkResult);
        
        _mouseControlProvider.Verify(p => p.MouseMove(x, y), Times.Once());
    }

    [Theory]
    [InlineData("asdfasdf")]
    [InlineData(null)]
    public void MoveFailTest(string? input)
    {
        var result = _mouseController.Move(input);
        Assert.True(result is ErrorResult {Result: "Wrong coordinates" });
        Mock.Get(_logger).Verify(x => x.LogError($"Cannot move mouse by {input}"));
    }

    [Fact]
    public void GetMethodsTest()
    {
        var methodNames = new[]
        {
            "left",
            "right",
            "middle",
            "wheelup",
            "wheeldown",
            "dragstart",
            "dragstop",
            "move"
        };

        var methods = _mouseController.GetMethods();
        Assert.True(methods.Count == methodNames.Length && methods.All(x => methodNames.Contains(x.Key)) && methods.All(
            x => x.Value.Target == _mouseController && x.Value.Method.ReturnType == typeof(IActionResult) &&
                 x.Value.Method.GetParameters().Length == 1 &&
                 x.Value.Method.GetParameters()[0].ParameterType == typeof(string)));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}