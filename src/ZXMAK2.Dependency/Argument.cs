

namespace ZXMAK2.Dependency
{
    public class Argument
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        public Argument(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
