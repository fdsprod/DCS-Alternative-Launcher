using System;

namespace DCS.Alternative.Launcher.ServiceModel
{
    public class IoCWeakReferenceException : Exception
    {
        private const string ERROR_TEXT = "Unable to instantiate {0} - referenced object has been reclaimed";

        public IoCWeakReferenceException(Type type)
            : base(string.Format(ERROR_TEXT, type.FullName))
        {
        }

        public IoCWeakReferenceException(Type type, Exception innerException)
            : base(string.Format(ERROR_TEXT, type.FullName), innerException)
        {
        }
    }
}