using System;
using System.Collections.Generic;
using System.Linq;

namespace DCS.Alternative.Launcher.ServiceModel
{
    public class IoCAutoRegistrationException : Exception
    {
        private const string ERROR_TEXT = "Duplicate implementation of type {0} found ({1}).";

        public IoCAutoRegistrationException(Type registerType, IEnumerable<Type> types)
            : base(string.Format(ERROR_TEXT, registerType, GetTypesString(types)))
        {
        }

        public IoCAutoRegistrationException(Type registerType, IEnumerable<Type> types, Exception innerException)
            : base(string.Format(ERROR_TEXT, registerType, GetTypesString(types)), innerException)
        {
        }

        private static string GetTypesString(IEnumerable<Type> types)
        {
            var typeNames = from type in types
                select type.FullName;

            return string.Join(",", typeNames.ToArray());
        }
    }
}