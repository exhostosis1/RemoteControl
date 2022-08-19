namespace Shared.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ControllerAttribute : Attribute
    {
        public string Name { get; set; }

        public ControllerAttribute(string name)
        {
            Name = name;
        }
    }
}