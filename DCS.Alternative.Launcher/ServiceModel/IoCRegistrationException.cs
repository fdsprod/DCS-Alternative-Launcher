using System;

namespace DCS.Alternative.Launcher.ServiceModel
{
    public class IoCRegistrationException : Exception
    {
        private const string CONVERT_ERROR_TEXT = "Cannot convert current registration of {0} to {1}";
        private const string GENERIC_CONSTRAINT_ERROR_TEXT = "Type {1} is not valid for a registration of type {0}";

        public IoCRegistrationException(Type type, string method)
            : base(string.Format(CONVERT_ERROR_TEXT, type.FullName, method))
        {
        }

        public IoCRegistrationException(Type type, string method, Exception innerException)
            : base(string.Format(CONVERT_ERROR_TEXT, type.FullName, method), innerException)
        {
        }

        public IoCRegistrationException(Type registerType, Type implementationType)
            : base(string.Format(GENERIC_CONSTRAINT_ERROR_TEXT, registerType.FullName, implementationType.FullName))
        {
        }

        public IoCRegistrationException(Type registerType, Type implementationType, Exception innerException)
            : base(string.Format(GENERIC_CONSTRAINT_ERROR_TEXT, registerType.FullName, implementationType.FullName), innerException)
        {
        }
    }
}