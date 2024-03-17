using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CygSoft.Qik.QikConsole;
using static QikConsole.Project;

namespace QikConsole
{
    public class XmlProjectsFile : IProjectsFile
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
            
            var projectEl = document.Element("projects")
                .Elements("project")
                .Where(proj => (string)proj.Attribute("key") == key);

            var project = projectEl
                .Select(proj => new Project()
                {
                    Key = proj.Attribute("key").Value,
                    Title = proj.Attribute("value").Value,

                    ScriptPath = proj.Element("settings")
                        .Elements("setting")
                        .Where(setting => setting.Attribute("key").Value == "scriptPath")
                        .Select(setting => setting.Attribute("value").Value)
                        .Single(),

                    Fragments = proj.Element("fragments")
                        .Elements("fragment")
                        .Select(frag => new Fragment()
                        {
                            Id = frag.Attribute("key").Value,
                            Path = frag.Attribute("path").Value
                        }).ToList(),

                    Documents = proj.Elements("documents")
                        .Elements("document")
                        .Select(doc => new QikConsole.Project.Document
                        {
                            Outputs = doc.Elements("outputs")
                                .Elements("output")
                                .Select(o => o.Attribute("path").Value).ToArray(),
                            Structure = doc.Elements("parts")
                                .Elements("part")
                                .Select(o => o.Attribute("key").Value).ToArray()
                        })
                        .ToList()
                })
                .SingleOrDefault();

            var inputs = new List<Input>();

            inputs.AddRange(projectEl.Elements("inputs").Elements("input")
                .Select(input => new Input
                {
                    Symbol = input.Attribute("symbol").Value,
                    Value = input.Attribute("value").Value,
                }));
            
            inputs.AddRange(projectEl.Elements("inputs").Elements("auto")
                .Select(input => new Input
                {
                    Symbol = input.Attribute("symbol").Value,
                    Value = input.Attribute("value").Value,
                }));
            
            var references = projectEl.Elements("inputs").Elements("global").Select(rf => rf.Attribute("key").Value);
            var globals = document.Element("projects").Elements("globals").Elements("input");

            inputs.AddRange(references.Join(globals, rf => rf, g => g.Attribute("key").Value, (rf, g) => new Input{
                Symbol = g.Attribute("symbol").Value,
                Value = g.Attribute("value").Value
            }));

            project.Inputs = inputs;

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