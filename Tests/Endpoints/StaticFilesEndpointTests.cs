using Moq;
using Shared.DataObjects.Http;
using Shared.Logging.Interfaces;
using System.Net;
using System.Text;
using Servers.Middleware;

namespace Tests.Endpoints;

public class StaticFilesEndpointTests : IDisposable
{
    public StaticFilesEndpointTests()
    {

    }

    [Fact]
    public void RequestTest()
    {
        var directory = "www";
        var path = Path.Combine(AppContext.BaseDirectory, directory);

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

        var logger = Mock.Of<ILogger<StaticFilesMiddleware>>();

        var endpoint = new StaticFilesMiddleware(logger, directory);

        var res = Mock.Of<HttpContextResponse>();

        var context = new HttpContext(new HttpContextRequest("/index.html"), res);
        endpoint.ProcessRequest(context);
        var response = Encoding.UTF8.GetString(context.HttpResponse.Payload);
        Assert.True(context.HttpResponse is { StatusCode: HttpStatusCode.OK, ContentType: "text/html" } && response == indexContents);

        context = new HttpContext(new HttpContextRequest("/file.txt"), res);
        endpoint.ProcessRequest(context);
        response = Encoding.UTF8.GetString(context.HttpResponse.Payload);
        Assert.True(context.HttpResponse is { StatusCode: HttpStatusCode.OK, ContentType: "text/plain" } && response == fileContents);

        context = new HttpContext(new HttpContextRequest(".."), res);
        endpoint.ProcessRequest(context);
        Assert.True(context.HttpResponse.StatusCode == HttpStatusCode.NotFound);
    }

    public void Dispose()
    {

    }
}