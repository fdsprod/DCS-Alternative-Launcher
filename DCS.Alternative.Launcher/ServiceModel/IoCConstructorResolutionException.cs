using System;

namespace DCS.Alternative.Launcher.ServiceModel
{
    public class IoCConstructorResolutionException : Exception
    {
        private const string ERROR_TEXT = "Unable to resolve constructor for {0} using provided Expression.";

        public IoCConstructorResolutionException(Type type)
            : base(string.Format(ERROR_TEXT, type.FullName))
        {
        }

        public IoCConstructorResolutionException(Type type, Exception innerException)
            : base(string.Format(ERROR_TEXT, type.FullName), innerException)
        {
        }

        public IoCConstructorResolutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public IoCConstructorResolutionException(string message)
            : base(message)
        {
        }
    }
}