﻿using Servers.ApiControllers;
using Servers.Middleware;
using Servers.Results;
using ControlProviders;

namespace UnitTests;

public class UtilsTest : IDisposable
{
    public UtilsTest()
    {

    }

    [Theory]
    [InlineData("6186e4b2-0484-4217-9af4-17cf151b699e")]
    [InlineData("741E9F8B-2FA5-4A3D-964D-2DE5746CF848")]
    [InlineData("{1d82de0a-ac6c-4706-80cd-930c1b97186c}")]
    public void GuidRegexTest(string input)
    {
        Assert.Matches(Utils.GuidRegex(), input);
    }

    [Theory]
    [InlineData("6186e4b2048442179af417cf151b699e")]
    [InlineData("6186e4b2-0484-4217-9af4-17cf151b699")]
    [InlineData("abcdefg")]
    public void GuidRegexFailTest(string input)
    {
        Assert.DoesNotMatch(Utils.GuidRegex(), input);
    }

    [Theory]
    [InlineData("http://www.localhost:1241/api/v1/controller/action/parameter", "controller", "action", "parameter")]
    [InlineData("http://www.localhost:1241/api/v1/controller/action/", "controller", "action", "")]
    [InlineData("http://www.localhost:1241/api/v1/controller/action", "controller", "action", "")]
    [InlineData("/api/v1/controller/action/parameter/asdf", "controller", "action", "parameter")]
    public void ApiRegexTest(string input, string resultController, string resultAction, string resultParameter)
    {
        var result = ApiUtils.TryParsePath(input, out var controller, out var action, out var parameter);

        Assert.True(result);

        Assert.Equal(resultController, controller);
        Assert.Equal(resultAction, action);
        Assert.Equal(resultParameter, parameter);
    }

    [Theory]
    [InlineData("api/v1/controller/action/parameter")]
    [InlineData("http://localhost/api/controller/action/parameter")]
    [InlineData("/api/1/controller/action/parameter")]
    public void ApiRegexFailTest(string input)
    {
        var result = ApiUtils.TryParsePath(input, out _, out _, out _);

        Assert.False(result);
    }

    [Theory]
    [InlineData("http://www.localhost:1241/api/v1/controller/action/parameter", "v1")]
    [InlineData("http://www.localhost:1241/api/v25/controller/action/", "v25")]
    [InlineData("http://www.localhost:1241/api/v3/controller/action", "v3")]
    [InlineData("/api/v5/controller/action/parameter", "v5")]
    public void ApiVersionRegexTest(string input, string resultVersion)
    {
        var result = ApiUtils.TryGetApiVersion(input, out var version);

        Assert.True(result);
        Assert.Equal(version, resultVersion);

    }

    [Theory]
    [InlineData("api/v1/controller/action/parameter")]
    [InlineData("http://localhost/api/controller/action/parameter")]
    [InlineData("/api/v/controller/action/parameter")]
    public void ApiRegexVersionFailTest(string input)
    {
        var result = ApiUtils.TryGetApiVersion(input, out _);
        Assert.False(result);
    }

    [Theory]
    [InlineData("{ 55; 66; }", 55, 66)]
    [InlineData("70 80", 70, 80)]
    [InlineData("12,3", 12, 3)]
    [InlineData("{1,2,3,4,5}", 1, 2)]
    public void TryGetCoordsTest(string input, int resultX, int resultY)
    {
        var result = CoordsHelper.TryGetCoords(input, out var x, out var y);

        Assert.True(result);
        Assert.Equal(resultX, x);
        Assert.Equal(resultY, y);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("string")]
    [InlineData("{ feadf, 3 }")]
    public void TryGetCoordsFailTest(string input)
    {
        var result = CoordsHelper.TryGetCoords(input, out _, out _);
        Assert.False(result);
    }

    private class Api1Controller : BaseApiController
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult Method1(string? _)
        {
            return new OkResult();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult Method2(string? _)
        {
            return new OkResult();
        }
    }

    private class Api2Controller : BaseApiController
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult Method2(string? _) => new OkResult();
    }

    private class Api3Controller : BaseApiController
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public int Method3(string? _)
        {
            return 0;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult Method4(string? _) => new OkResult();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult Method5(int _)
        {
            return new OkResult();
        }
    }

    [Fact]
    public void GetControllersWithMethodsTest()
    {
        var controllers = new BaseApiController[]
        {
            new Api1Controller(),
            new Api2Controller(),
            new Api3Controller()
        };

        var result = controllers.GetControllersWithActions();

        Assert.Equal(3, result.Count);

        Assert.Equal(2, result["api1"].Count);
        Assert.Single(result["api2"]);
        Assert.Equal(2, result["api3"].Count);

        Assert.True(result.All(x => x.Value.All(x =>
            x.Value.Method.ReturnType == typeof(IActionResult))));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}