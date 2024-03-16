using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using static QikConsole.Project;

namespace QikConsole
{
    public class ProjectsFile : IProjectsFile
    {
        private XDocument document;

        public void Load(string path)
        {
            if (!Path.Exists(path) || Path.GetExtension(path.ToLower()) is not ".xml") throw new ApplicationException("Projects file should be an xml file.");
            try
            {
                document = XDocument.Parse(Open(path));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Could not load xml file.", ex);
            }
        }

        public Project GetProject(string key)
        {
            if (document is null || document.Root is null)
                throw new ApplicationException("Projects file not loaded.");

            var project = document.Element("projects").Descendants()
                .Where(proj => (string)proj.Attribute("key") == key)
                .Select(proj => new Project()
                {
                    Key = proj.Attribute("key").Value,
                    Title = proj.Attribute("value").Value,

                    ScriptFile = proj.Element("settings")
                        .Elements("setting")
                        .Where(setting => setting.Attribute("key").Value == "scriptPath")
                        .Select(setting => setting.Attribute("value").Value)
                        .Single(),

                    Fragments = proj.Element("fragments").Descendants()
                        .Select(frag => new Fragment()
                        {
                            Id = frag.Attribute("key").Value,
                            Path = frag.Attribute("path").Value
                        }).ToList(),

                    Documents = proj.Elements("documents").Descendants()
                        .Select(doc => new QikConsole.Project.Document
                        {
                            Outputs = doc.Elements("outputs").Descendants()
                                .Select(o => o.Attribute("path").Value).ToArray(),
                            Structure = doc.Elements("parts").Descendants()
                                .Select(o => o.Attribute("key").Value).ToArray()
                        })
                        .ToList()
                })
                .SingleOrDefault();

            return project;
        }

        private string Open(string filePath)
        {
            string fileText = null;

            if (File.Exists(filePath))
            {
                using (var file = new FileStream(filePath, FileMode.Open))
                using (var reader = new StreamReader(file))
                {
                    fileText = reader.ReadToEnd();
                }
            }

            return fileText;
        }
    }
}