using System;

namespace Shared.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionAttribute : Attribute
    {
        public string Name { get; set; }

        public ActionAttribute(string name)
        {
            Name = name;
        }
    }
}
