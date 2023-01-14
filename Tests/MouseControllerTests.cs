using ApiControllers;
using Moq;
using Shared.ApiControllers.Results;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Tests;

public class MouseControllerTests: IDisposable
{
    private readonly MouseController _mouseController;
    private readonly IMouseControlProvider _mouseControlProvider;

    public MouseControllerTests()
    {
        var logger = Mock.Of<ILogger<MouseController>>();
        _mouseControlProvider = Mock.Of<IMouseControlProvider>();
        _mouseController = new MouseController(_mouseControlProvider, logger);
    }

    [Fact]
    public void LeftTest()
    {
        var result = _mouseController.Left(null);
        Assert.True(result is OkResult);
        Mock.Get(_mouseControlProvider).Verify(x => x.ButtonPress(MouseButtons.Left, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void RightTest()
    {
        var result = _mouseController.Right(null);
        Assert.True(result is OkResult);
        Mock.Get(_mouseControlProvider).Verify(x => x.ButtonPress(MouseButtons.Right, KeyPressMode.Click), Times.Once);
    }

    [Fact]
    public void MiddleTest()
    {
        var result = _mouseController.Middle(null);
        Assert.True(result is OkResult);
        Mock.Get(_mouseControlProvider).Verify(x => x.ButtonPress(MouseButtons.Middle, KeyPressMode.Click), Times.Once);
    }

    [Fact] 
    public void WheelUpTest()
    {
        var result = _mouseController.WheelUp(null);
        Assert.True(result is OkResult);
        Mock.Get(_mouseControlProvider).Verify(x => x.Wheel(true), Times.Once);
    }

    [Fact]
    public void WheelDownTest()
    {
        var result = _mouseController.WheelDown(null);
        Assert.True(result is OkResult);
        Mock.Get(_mouseControlProvider).Verify(x => x.Wheel(false), Times.Once);
    }

    [Fact]
    public void DragStartTest()
    {
        var result = _mouseController.DragStart(null);
        Assert.True(result is OkResult);
        Mock.Get(_mouseControlProvider).Verify(x => x.ButtonPress(MouseButtons.Left, KeyPressMode.Down), Times.Once);
    }

    [Fact]
    public void DragStopTest()
    {
        var result = _mouseController.DragStop(null);
        Assert.True(result is OkResult);
        Mock.Get(_mouseControlProvider).Verify(x => x.ButtonPress(MouseButtons.Left, KeyPressMode.Up), Times.Once);
    }

    [Fact]
    public void MoveTest()
    {
        var result = _mouseController.Move("asbasdf");
        Assert.True(result is OkResult);
        Mock.Get(_mouseControlProvider).Verify(x => x.Move(It.IsAny<int>(), It.IsAny<int>()), Times.Never);

        result = _mouseController.Move("{ x: 11, y: 12 }");
        Assert.True(result is OkResult);
        Mock.Get(_mouseControlProvider).Verify(x => x.Move(11, 12), Times.Once);

        result = _mouseController.Move(null);
        Assert.True(result is ErrorResult);
    }

    public void Dispose()
    {

    }
}