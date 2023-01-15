using Autostart;
using Moq;
using Shared.Logging.Interfaces;
using Shared.TaskServiceWrapper;

namespace Tests;

public class WinTaskAutostartServiceTests
{
    private readonly MockTaskService _taskService;
    private readonly WinTaskAutostartService _winTaskAutostartService;
    private static MockTask? _task;
    private static MockTaskFolder _rootFolder;


    private class MockPrincipalCollection: ITaskPrincipal
    {
        public string UserId { get; set; }
    }

    private class MockTriggerCollection: ITriggerCollection
    {
        public void AddLogonTrigger(string userId)
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
        public int DeleteCount { get; set; }
        public int RegisterCount { get; set; }

        public void DeleteTask(string name, bool exceptionOnNotExists)
        {
            DeleteCount++;
            Tasks.RemoveAll(x => x.Name == name);
        }

        public void RegisterTaskDefinition(string name, ITaskDefinition definition)
        {
            RegisterCount++;

            var task = new MockTask()
            {
                Name = name,
                Enabled = true
            };
            _task = task;
            Tasks.Add(task);
        }
    }

    private class MockTask: ITask
    {
        public int Count { get; set; }
        public string Name { get; set; }

        public bool Enabled
        {
            get
            {
                Count++;
                return true;
            }
            set
            {

            }
        }
    }

    private class MockTaskDefinition: ITaskDefinition
    {
        public IActionCollection Actions { get; } = new MockActionCollection();
        public ITriggerCollection Triggers { get; } = new MockTriggerCollection();
        public ITaskPrincipal Principal { get; } = new MockPrincipalCollection();
    }

    private class MockTaskService: ITaskService
    {
        public int FincCount { get; set; }

        public ITaskFolder RootFolder { get; }

        public MockTaskService()
        {
            var folder = new MockTaskFolder();
            _rootFolder = folder;
            RootFolder = folder;
        }
     
        public ITaskDefinition NewTask()
        {
            return new MockTaskDefinition();
        }

        public ITask? FindTask(string name)
        {
            FincCount++;
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

        Assert.True(_taskService.FincCount == 1);
        Assert.True(_task == null);
    }

    [Fact]
    public void SetAutostart_StateUnderTest_ExpectedBehavior()
    {
        _winTaskAutostartService.SetAutostart(true);
        var result = _winTaskAutostartService.CheckAutostart();

        Assert.True(result);

        Assert.True(_rootFolder.DeleteCount == 1);
        Assert.True(_rootFolder.RegisterCount == 1);

        Assert.True(File.Exists(Path.Combine(AppContext.BaseDirectory, "run.bat")));

        Assert.True(_taskService.FincCount == 1);
        Assert.True(_task?.Count == 1);
    }
}