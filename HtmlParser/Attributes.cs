using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HtmlParser
{
    public class Attributes: IEnumerable<Attribute>
    {
        private readonly HashSet<Attribute> _attributes = new HashSet<Attribute>();

        public string this[string key] 
        {
            get => GetAttribute(key)?.Value; 
            set
            {
                var result = GetAttribute(key);

                if (result == null)
                    throw new ArgumentException("no such attribute");

                result.Value = value;
            }
        }

        public Attribute GetAttribute(string key) => _attributes.FirstOrDefault(x => x.Name == key.ToLower());

        public ICollection<string> Keys => _attributes.Select(x => x.Name).ToList();

        public ICollection<string> Values => _attributes.Select(x => x.Value).ToList();

        public int Count => _attributes.Count;

        public void Add(string name, string value) => Add(new Attribute(name, value));

        public void Add(string name) => Add(name, "");

        public void Add(Attribute attribute) => _attributes.Add(attribute);

        public void Clear() => _attributes.Clear();

        public bool Contains(string key) => _attributes.Any(x => x.Name == key.ToLower());

        public void CopyTo(Attribute[] array, int index) => _attributes.CopyTo(array, index);

        public void Remove(string key) => _attributes.RemoveWhere(x => x.Name == key.ToLower());

        IEnumerator IEnumerable.GetEnumerator() => _attributes.GetEnumerator();

        public IEnumerator<Attribute> GetEnumerator() => _attributes.GetEnumerator();
    }
}
