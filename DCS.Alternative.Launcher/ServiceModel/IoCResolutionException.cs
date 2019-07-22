using System;

namespace DCS.Alternative.Launcher.ServiceModel
{
    public class IoCResolutionException : Exception
    {
        private const string ERROR_TEXT = "Unable to resolve type: {0}";

        public IoCResolutionException(Type type)
            : base(string.Format(ERROR_TEXT, type.FullName))
        {
        }

        public IoCResolutionException(Type type, Exception innerException)
            : base(string.Format(ERROR_TEXT, type.FullName), innerException)
        {
        }
    }
}