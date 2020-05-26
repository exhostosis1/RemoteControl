using System;

namespace RemoteControlCore.Attributes
{
    class RouteAttribute : Attribute
    {
        public string MethodName { get; }

        public RouteAttribute(string methodName)
        {
            this.MethodName = methodName;
        }
    }
}
