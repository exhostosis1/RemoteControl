namespace WebApiProvider.Attributes
{
    public class ActionAttribute : Attribute
    {
        public string Name { get; set; }

        public ActionAttribute(string name)
        {
            Name = name;
        }
    }
}
