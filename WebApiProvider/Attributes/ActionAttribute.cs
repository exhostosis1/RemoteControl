namespace WebApiProvider.Attributes
{
    internal class ActionAttribute : Attribute
    {
        public string Name { get; set; }

        public ActionAttribute(string name)
        {
            Name = name;
        }
    }
}
