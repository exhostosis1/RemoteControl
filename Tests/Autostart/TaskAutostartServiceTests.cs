using Autostart;
using Moq;
using Shared.Logging.Interfaces;
using System.Linq.Expressions;
using Shared.Wrappers.TaskServiceWrapper;

namespace Tests.Autostart;

public class TaskAutostartServiceTests
{
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly TaskAutostartService _winTaskAutostartService;
    private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "run.bat");

    private readonly Mock<ILogger<TaskAutostartService>> _logger;

    private readonly Expression<Func<ITaskService, ITaskDefinition?>> _findExpression = x => x.FindTask(It.IsAny<string>());
    private readonly Expression<Action<ITaskService>> _deleteExpression = x => x.DeleteTask(It.IsAny<string>(), false);
    private readonly Expression<Func<ITaskService, bool>> _registerExpression;

    public TaskAutostartServiceTests()
    {
        _taskServiceMock = new Mock<ITaskService>(MockBehavior.Strict);
        Mock<ITaskDefinition> taskDefinitionMock = new(MockBehavior.Strict);

        var actionCollectionMock = new Mock<TaskActionCollection>(MockBehavior.Loose);
        var triggerCollectionMock = new Mock<TaskTriggerCollection>(MockBehavior.Loose);

        taskDefinitionMock.SetupGet(x => x.Actions).Returns(actionCollectionMock.Object);
        taskDefinitionMock.SetupGet(x => x.Triggers).Returns(triggerCollectionMock.Object);

        _taskServiceMock.Setup(x => x.NewTask(It.IsAny<string>(), It.IsAny<string>())).Returns(taskDefinitionMock.Object);

        _logger = new Mock<ILogger<TaskAutostartService>>(MockBehavior.Loose);

        _winTaskAutostartService = new TaskAutostartService(_taskServiceMock.Object, _logger.Object);

        _taskServiceMock.Verify(x => x.NewTask(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        actionCollectionMock.Verify(x => x.Add(It.IsAny<TaskAction>()), Times.Once);
        triggerCollectionMock.Verify(x => x.Add(It.IsAny<TaskLogonTrigger>()), Times.Once);

        _registerExpression = x => x.RegisterNewTask(taskDefinitionMock.Object);

        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    [Fact]
    public void CheckAutostartTaskAndFileExistTest()
    {
        var td = new Mock<ITaskDefinition>(MockBehavior.Strict);
        td.SetupGet(x => x.Enabled).Returns(true);
        _taskServiceMock.Setup(_findExpression).Returns(td.Object);
        File.Create(_filePath).Close();

        var result = _winTaskAutostartService.CheckAutostart();

        Assert.True(result);
        _taskServiceMock.Verify(_findExpression, Times.Once);
        File.Delete(_filePath);
    }

    [Fact]
    public void CheckAutostartTaskDoesNotExistFileExistsTest()
    {
        _taskServiceMock.Setup(_findExpression).Returns(null as ITaskDefinition);
        File.Create(_filePath).Close();

        var result = _winTaskAutostartService.CheckAutostart();

        Assert.False(result);
        _taskServiceMock.Verify(_findExpression, Times.Once);
        File.Delete(_filePath);
    }

    [Fact]
    public void CheckAutostartTaskExistsFileDoesNotExistTest()
    {
        var td = new Mock<ITaskDefinition>(MockBehavior.Strict);
        td.SetupGet(x => x.Enabled).Returns(true);
        _taskServiceMock.Setup(_findExpression).Returns(td.Object);

        var result = _winTaskAutostartService.CheckAutostart();

        Assert.False(result);
        _taskServiceMock.Verify(_findExpression, Times.Once);
    }

    [Fact]
    public void CheckAutostartTaskAndFileDoNotExistTest()
    {
        _taskServiceMock.Setup(_findExpression).Returns(null as ITaskDefinition);

        var result = _winTaskAutostartService.CheckAutostart();

        Assert.False(result);
        _taskServiceMock.Verify(_findExpression, Times.Once);
    }

    [Fact]
    public void SetAutostartTrueTest()
    {
        _taskServiceMock.Setup(_deleteExpression);
        _taskServiceMock.Setup(_registerExpression);

        _winTaskAutostartService.SetAutostart(true);

        _taskServiceMock.Verify(_deleteExpression, Times.Once);
        Assert.True(File.Exists(_filePath));
        _taskServiceMock.Verify(_registerExpression, Times.Once);
        
        File.Delete(_filePath);
    }

    [Fact]
    public void SetAutostartFalseTest()
    {
        _taskServiceMock.Setup(_deleteExpression);

        _winTaskAutostartService.SetAutostart(false);

        _taskServiceMock.Verify(_deleteExpression, Times.Once);
        Assert.False(File.Exists(_filePath));
    }

    [Fact]
    public void SetAutostartUnauthorizedExceptionTest()
    {
        _taskServiceMock.Setup(_deleteExpression);
        _taskServiceMock.Setup(x => x.RegisterNewTask(It.IsAny<ITaskDefinition>()))
            .Throws<UnauthorizedAccessException>();

        _winTaskAutostartService.SetAutostart(true);

        _logger.Verify(x => x.LogError($"Cannot write {_filePath} due to access restrictions"), Times.Once);
        File.Delete(_filePath);
    }

    [Fact]
    public void SetAutostartDirectoryNotFoundExceptionTest()
    {
        _taskServiceMock.Setup(_deleteExpression);
        _taskServiceMock.Setup(x => x.RegisterNewTask(It.IsAny<ITaskDefinition>()))
            .Throws<DirectoryNotFoundException>();

        _winTaskAutostartService.SetAutostart(true);

        _logger.Verify(x => x.LogError($"Cannot find directory to write {_filePath}"), Times.Once);
        File.Delete(_filePath);
    }

    [Fact]
    public void SetAutostartCommonExceptionTest()
    {
        _taskServiceMock.Setup(_deleteExpression);
        _taskServiceMock.Setup(x => x.RegisterNewTask(It.IsAny<ITaskDefinition>()))
            .Throws(() => new Exception("test message"));

        _winTaskAutostartService.SetAutostart(true);

        _logger.Verify(x => x.LogError("test message"), Times.Once);

        File.Delete(_filePath);
    }
}