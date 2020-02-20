namespace Halogen.Core.Diagnostics
{
    [System.Serializable]
    public class HalogenException : System.Exception
    {
        public HalogenException() { }
        public HalogenException(string message) : base(message) { }
        public HalogenException(string message, System.Exception inner) : base(message, inner) { }
        protected HalogenException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [System.Serializable]
    public class IOException : HalogenException
    {
        public IOException() { }
        public IOException(string message) : base(message) { }
        public IOException(string message, System.Exception inner) : base(message, inner) { }
        protected IOException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}