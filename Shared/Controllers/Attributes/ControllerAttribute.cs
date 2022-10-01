using System;

namespace Shared.Controllers.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ControllerAttribute : Attribute
{
    public string Name { get; }

    public ControllerAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Controller must have a name");

        Name = name;
    }
}