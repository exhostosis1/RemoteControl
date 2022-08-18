namespace Shared.Interfaces.Web.Attributes
{
    public class ControllerAttribute : Attribute
    {
        public string Name { get; set; }

        public ControllerAttribute(string name)
        {
            Name = name;
        }
    }
}