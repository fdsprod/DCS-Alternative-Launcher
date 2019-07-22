using System.Linq;
using System.Xml;

namespace DCS.Alternative.Launcher.Xml
{
    public static class XmlHelper
    {
        public static string RemoveInvalidXmlChars(string content)
        {
            return new string(content.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray());
        }
    }
}