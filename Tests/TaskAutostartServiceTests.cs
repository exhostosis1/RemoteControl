using Autostart;
using Moq;
using Shared.Logging.Interfaces;
using Shared.TaskServiceWrapper;

namespace Tests;

public class TaskAutostartServiceTests
{
    private readonly ITaskService _taskService;
    private readonly TaskAutostartService _winTaskAutostartService;
    private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "run.bat");

    public TaskAutostartServiceTests()
    {
        var mockService = new Mock<ITaskService>(MockBehavior.Loose);
        var mockTask = new Mock<ITaskDefinition>(MockBehavior.Loose);
        mockTask.SetupGet(x => x.Actions).Returns(new List<TaskAction>());
        mockTask.SetupGet(x => x.Triggers).Returns(new List<TaskTrigger>());

        mockService.Setup(x => x.NewTask(It.IsAny<string>(), It.IsAny<string>())).Returns(mockTask.Object);

        _taskService = mockService.Object;

        var logger = Mock.Of<ILogger<TaskAutostartService>>();

        _winTaskAutostartService = new TaskAutostartService(_taskService, logger);

        if(File.Exists(_filePath))
            File.Delete(_filePath);
    }

    [Fact]
    public void CheckAutostart_StateUnderTest_ExpectedBehavior()
    {
        var result = _winTaskAutostartService.CheckAutostart();

        Assert.False(result);

        Mock.Get(_taskService).Verify(x => x.FindTask(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void SetAutostart_StateUnderTest_ExpectedBehavior()
    {
        _winTaskAutostartService.SetAutostart(true);

        Assert.True(File.Exists(Path.Combine(AppContext.BaseDirectory, "run.bat")));

        Mock.Get(_taskService).Verify(x => x.DeleteTask(It.IsAny<string>(), false), Times.Once);
        Mock.Get(_taskService).Verify(x => x.RegisterNewTask(It.IsAny<ITaskDefinition>()), Times.Once);

        File.Delete(_filePath);
    }
}