using System;

namespace DCS.Alternative.Launcher.ServiceModel
{
    public class IoCRegistrationTypeException : Exception
    {
        private const string REGISTER_ERROR_TEXT =
            "Cannot register type {0} - abstract classes or interfaces are not valid implementation types for {1}.";

        public IoCRegistrationTypeException(Type type, string factory)
            : base(string.Format(REGISTER_ERROR_TEXT, type.FullName, factory))
        {
        }

        public IoCRegistrationTypeException(Type type, string factory, Exception innerException)
            : base(string.Format(REGISTER_ERROR_TEXT, type.FullName, factory), innerException)
        {
        }
    }
}