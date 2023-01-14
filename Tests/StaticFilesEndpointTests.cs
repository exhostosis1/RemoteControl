using Moq;
using Servers.Endpoints;
using Shared.DataObjects.Http;
using Shared.Logging.Interfaces;
using System.Net;
using System.Text;

namespace Tests;

public class StaticFilesEndpointTests: IDisposable
{
    public StaticFilesEndpointTests()
    {

    }

    [Fact]
    public void RequestTest()
    {
        var directory = "www";
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var indexContents = @"
<html>
<title>test</title>
<body>test body</body>
</html>
";

        var fileContents = "contents";

        File.WriteAllText(Path.Combine(path, "index.html"), indexContents);
        File.WriteAllText(Path.Combine(path, "file.txt"), fileContents);

        var logger = Mock.Of<ILogger<StaticFilesEndpoint>>();

        var endpoint = new StaticFilesEndpoint(logger, directory);

        var context = new Context("/index.html");
        endpoint.ProcessRequest(context);
        var response = Encoding.UTF8.GetString(context.Response.Payload);
        Assert.True(context.Response is { StatusCode: HttpStatusCode.OK, ContentType: "text/html" } && response == indexContents);

        context = new Context("/file.txt");
        endpoint.ProcessRequest(context);
        response = Encoding.UTF8.GetString(context.Response.Payload);
        Assert.True(context.Response is { StatusCode: HttpStatusCode.OK, ContentType: "text/plain" } && response == fileContents);

        context = new Context("..");
        endpoint.ProcessRequest(context);
        Assert.True(context.Response.StatusCode == HttpStatusCode.NotFound);
    }

    public void Dispose()
    {

    }
}