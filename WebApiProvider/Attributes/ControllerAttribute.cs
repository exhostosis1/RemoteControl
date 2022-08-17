namespace WebApiProvider.Attributes
{
    internal class ControllerAttribute : Attribute
    {
        public string Name { get; set; }

        public ControllerAttribute(string name)
        {
            Name = name;
        }
    }
}