using System;

namespace DependencyFactory.Config
{
    public sealed class NavigationOption
    {
        private object Value;
        internal NavigationOption(object value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public static bool operator == (NavigationOption left, NavigationOption right)
        {
            return left.Value == right.Value;
        }

        public static bool operator != (NavigationOption left, NavigationOption right)
        {
            return left.Value != right.Value;
        }

        public override bool Equals(object that)
        {
            if (that.GetType() != typeof(NavigationOption))
                throw new ArgumentException();

            return this.Value == ((NavigationOption)that).Value;
        }
    }
}
