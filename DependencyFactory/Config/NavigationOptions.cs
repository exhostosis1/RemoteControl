using System;

namespace DependencyFactory.Config
{
    public sealed class NavigationOption
    {
        internal Guid Value;
        public NavigationOption()
        {
            Value = Guid.NewGuid();
        }
    }
}
