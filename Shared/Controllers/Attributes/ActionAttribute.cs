using System;

namespace Shared.Controllers.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ActionAttribute : Attribute
{
    public string Name { get; }

    public ActionAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Action must have a name");
            
        Name = name;
    }
}