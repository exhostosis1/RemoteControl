namespace HtmlParser
{
    public class Attribute
    {
        private string _name;
        public string Name {
            get
            {
                return _name;
            }
            set 
            {
                _name = value?.ToLower() ?? "";
            } 
        }

        public string Value { get; set; }

        public bool Directive => string.IsNullOrEmpty(Value);

        public Attribute(): this("", "") { }

        public Attribute(string name) : this(name, "") { }

        public Attribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString() => $"{Name}{(!string.IsNullOrEmpty(Value) ? $"=\"{Value}\"" : "")}";

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
