using Autostart;
using Moq;
using Shared.Logging.Interfaces;
using Shared.TaskServiceWrapper;

namespace Tests;

public class WinTaskAutostartServiceTests
{
    private readonly ITaskService _taskService;
    private readonly WinTaskAutostartService _winTaskAutostartService;

    private class MockLogonTrigger: ITrigger
    {
        public string UserId { get; set; }
    }

    private class MockPrincipalCollection: ITaskPrincipal
    {
        public string UserId { get; set; }
    }

    private class MockTriggerCollection: ITriggerCollection
    {
        public void Add(ITrigger trigger)
        {
        }
    }

    private class MockActionCollection: IActionCollection
    {
        public void Add(string path, string? arguments = null, string? workingDirectory = null)
        {
        }
    }

    private class MockTaskFolder: ITaskFolder
    {
        public List<MockTask> Tasks { get; set; } = new();

        public void DeleteTask(string name, bool exceptionOnNotExists)
        {
            Tasks.RemoveAll(x => x.Name == name);
        }

        public void RegisterTaskDefinition(string name, ITaskDefinition definition)
        {
            Tasks.Add(new MockTask{ Name = name });
        }
    }

    private class MockTask: ITask
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }

    private class MockTaskDefinition: ITaskDefinition
    {
        public IActionCollection Actions { get; } = new MockActionCollection();
        public ITriggerCollection Triggers { get; } = new MockTriggerCollection();
        public ITaskPrincipal Principal { get; } = new MockPrincipalCollection();
    }

    private class MockTaskService: ITaskService
    {
        public ITaskFolder RootFolder { get; } = new MockTaskFolder();
     
        public ITaskDefinition NewTask()
        {
            return new MockTaskDefinition();
        }

        public ITask? FindTask(string name)
        {
            return (RootFolder as MockTaskFolder)?.Tasks.FirstOrDefault(x => x.Name == name);
        }
    }

    public WinTaskAutostartServiceTests()
    {
        _taskService = new MockTaskService();
        var logger = Mock.Of<ILogger<WinTaskAutostartService>>();

        _winTaskAutostartService = new WinTaskAutostartService(_taskService, logger);
    }

    [Fact]
    public void CheckAutostart_StateUnderTest_ExpectedBehavior()
    {
        var result = _winTaskAutostartService.CheckAutostart();

        Assert.False(result);
    }

    [Fact]
    public void SetAutostart_StateUnderTest_ExpectedBehavior()
    {
        _winTaskAutostartService.SetAutostart(true);
        var result = _winTaskAutostartService.CheckAutostart();

        Assert.True(result);
    }
}