using MainApp.Servers.DataObjects;
using MainApp.Servers.Middleware;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Middleware;

public class StaticFilesMiddlewareTests : IDisposable
{
    private readonly StaticFilesMiddleware _middleware;
    private const string Dir = "www";
    private readonly string _path = Path.Combine(AppContext.BaseDirectory, Dir);

    private const string IndexContents = """

                                         <html>
                                         <title>test</title>
                                         <body>test body</body>
                                         </html>

                                         """;

    private const string FileContents = "contents";

    public StaticFilesMiddlewareTests()
    {
        var logger = Mock.Of<ILogger>();
        _middleware = new StaticFilesMiddleware(logger, Dir);

        if (!Directory.Exists(_path))
            Directory.CreateDirectory(_path);

        File.WriteAllText(Path.Combine(_path, "index.html"), IndexContents);
        File.WriteAllText(Path.Combine(_path, "index.htm"), IndexContents);
        File.WriteAllText(Path.Combine(_path, "file.txt"), FileContents);
        File.WriteAllText(Path.Combine(_path, "file.ico"), FileContents);
        File.WriteAllText(Path.Combine(_path, "file.js"), FileContents);
        File.WriteAllText(Path.Combine(_path, "file.mjs"), FileContents);
        File.WriteAllText(Path.Combine(_path, "file.css"), FileContents);
    }

    [Theory]
    [InlineData("/index.html", RequestStatus.Custom, IndexContents)]
    [InlineData("/index.htm", RequestStatus.Custom, IndexContents)]
    [InlineData("/file.txt", RequestStatus.Custom, FileContents)]
    [InlineData("/file.ico", RequestStatus.Custom, FileContents)]
    [InlineData("/file.js", RequestStatus.Custom, FileContents)]
    [InlineData("/file.mjs", RequestStatus.Custom, FileContents)]
    [InlineData("/file.css", RequestStatus.Custom, FileContents)]
    [InlineData("..", RequestStatus.NotFound, "")]
    [InlineData("/unexisting.file", RequestStatus.Custom, "")]
    internal async Task RequestTest(string path, RequestStatus status, string response)
    {
        var context = new RequestContext
        {
            Path = path
        };
        
        await _middleware.ProcessRequestAsync(context, null!);

        Assert.Equal(status, context.Status);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Directory.Delete(_path, true);
    }
}