using System;
using System.Xml;

namespace XmlEditor.Models.Services.Exceptions
{
    public class XmlFileException : XmlException
    {
        public XmlFileException(string message) : base(message)
        {
        }
    }
}
