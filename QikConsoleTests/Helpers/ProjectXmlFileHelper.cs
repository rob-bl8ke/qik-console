using System;
using System.IO;
using System.Xml.Linq;
using Moq;


namespace QikConsoleTests
{
    public class ProjectXmlFileHelper
    {
        public static string GetRootElementName()
        {
            string fileText = FileHelpers.GetProjectXml();

            if (!string.IsNullOrEmpty(fileText))
                {
                XDocument xDocument = XDocument.Parse(fileText);
                return xDocument.Root.Name.ToString();
            }
            return "";
        }
    }
}
