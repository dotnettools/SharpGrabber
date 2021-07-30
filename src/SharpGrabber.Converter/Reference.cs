namespace DotNetTools.SharpGrabber.Converter
{
    public class Reference<T> where T : struct
    {
        public static implicit operator T(Reference<T> val)
        {
            return val.Value;
        }

        private T _value;

        public Reference(T value)
        {
            _value = value;
        }

        public Reference() : this(default) { }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
