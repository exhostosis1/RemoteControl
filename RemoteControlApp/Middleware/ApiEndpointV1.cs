using Shared;
using Shared.Interfaces;
using Shared.Interfaces.Web;
using System.Net;
using System.Text;

namespace RemoteControlApp.Middleware
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ApiEndpointV1 : IMiddleware
    {
        private readonly ControllerMethods _methods;

        private const string ApiVersion = "v1";

        public ApiEndpointV1(HttpEventHandler _, ControllerMethods methods)
        {
            _methods = methods;
        }

        public void ProcessRequest(IContext context)
        {
            var (controller, action, param) = context.Request.Path.ParsePath(ApiVersion);

            if (!_methods.ContainsKey(controller) || !_methods[controller].ContainsKey(action))
            {
                context.Response.StatusCode = HttpStatusCode.NotFound;
                return;
            }

            var result = _methods[controller][action](param);

            if (!string.IsNullOrEmpty(result))
            {
                context.Response.ContentType = "application/json";
                context.Response.Payload = Encoding.UTF8.GetBytes(result);
            }
        }
    }

    public static partial class AppExtensions
    {
        public static IRemoteControlApp UseApiV1Enpoint(this IRemoteControlApp app)
        {
            app.UseMiddleware<ApiEndpointV1>();
            return app;
        }
    }
}
