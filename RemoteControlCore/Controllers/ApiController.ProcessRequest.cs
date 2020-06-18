using System;
using RemoteControlCore.Abstract;
using RemoteControlCore.Utility;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace RemoteControlCore.Controllers
{
    internal partial class ApiController : AbstractController
    {
        private readonly Dictionary<string, MethodInfo> _methods;

        public override void ProcessApiRequest(string message, Stream stream)
        {
            var (methodName, param) = Strings.ParseAddresString(message);

            if (string.IsNullOrWhiteSpace(methodName)) return;

            if (!_methods.ContainsKey(methodName)) return;

            var result = _methods[methodName].Invoke(this, new object[]{param});

            if (param == "init")
            {
                var bytes = Encoding.UTF8.GetBytes((string)result);
                var response = new byte[bytes.Length + 2];
                response[0] = 0x81; // denotes this is the final message and it is in text
                response[1] = (byte)(bytes.Length); // payload size = message - header size
                Array.Copy(bytes, 0, response, 2, bytes.Length);
                stream.Write(response, 0, response.Length);
            }
        }
    }
}
