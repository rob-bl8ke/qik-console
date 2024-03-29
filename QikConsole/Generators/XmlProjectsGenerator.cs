using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CygSoft.Qik.Functions;
using NLog;
using QikConsole;

namespace CygSoft.Qik.QikConsole
{
    public class XmlProjectsGenerator
    {
        private Dictionary<string, string> fragmentsDictionary = new();
        private readonly IProjectsFile projectsFile;
        private readonly IFileFunctions fileFunctions;
        private readonly ILogger logger;

        public XmlProjectsGenerator(IProjectsFile projectsFile, IFileFunctions fileFunctions, ILogger logger)
        {
            this.projectsFile = projectsFile;
            this.fileFunctions = fileFunctions;
            this.logger = logger;
        }
        
        public void Execute(string filePath, string projectKey, string inputs)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !fileFunctions.FileExists(filePath))
            {
                throw new ApplicationException("Please specify a valid path. See --help for more information.");
            }
            else
            {
                try
                {
                    var inputList = inputs is not null ? SetInputs(inputs) : new Input[0];
                    Generate(filePath, projectKey, inputList);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Generate exception ocurred.");
                    throw;
                }
            }
        }

        private Input[] SetInputs(string inputs)
        {
            var keyValues = inputs.Split(";")
                .Select(a =>
                {
                    var parts = a.Split('=');
                    return new Input()
                    {
                        Symbol = parts[0],
                        Value = parts[1]   //maybe you need to check something here
                    };
                });

            return keyValues.ToArray();
        }

        private void Generate(string filePath, string projectKey, Input[] inputs)
        {
            projectsFile.Load(filePath);
            var project = projectsFile.GetProject(projectKey);
            
            fragmentsDictionary = new Dictionary<string, string>();
            var terminal = InitializeTerminal(filePath, inputs, project);
            GenerateFragments(filePath, project, terminal);
            GenerateDocuments(filePath, project, terminal);
        }

        private PlaceholderTerminal InitializeTerminal(string path, Input[] inputs, Project project)
        {
            var scriptPath = Path.Combine(Path.GetDirectoryName(path), project.ScriptPath);
            var script = fileFunctions.ReadTextFile(scriptPath);
            var interpreter = new Interpreter();
            var symbolTerminal = interpreter.Interpret(new FunctionFactory(new PluginLoader()), script);
            var terminal = new PlaceholderTerminal(symbolTerminal, "@{", "}");


            foreach (var input in project.Inputs)
            {
                terminal.SetSymbolValue($"@{input.Symbol}", input.Value);
            }

            // Will override the project file
            foreach(var input in inputs)
            {
                terminal.SetSymbolValue($"@{input.Symbol}", input.Value);
            }

            return terminal;
        }

        private void GenerateFragments(string path, Project project, PlaceholderTerminal terminal)
        {   
            foreach (var frag in project.Fragments)
            {
                var fullPath = Path.Combine(Path.GetDirectoryName(path), frag.Path);
                var templateText = fileFunctions.ReadTextFile(fullPath);

                foreach (var placeholder in terminal.Placeholders)
                {
                    templateText = templateText.Replace(placeholder, terminal.GetPlaceholderValue(placeholder));
                }

                fragmentsDictionary.Add(frag.Id, templateText);
            }
        }

        private void GenerateDocuments(string path, Project project, PlaceholderTerminal terminal)
        {
            foreach (var document in project.Documents)
            {
                var fullPath = Path.Combine(Path.GetDirectoryName(path), document.Path);
                var templateText = fileFunctions.ReadTextFile(fullPath);

                foreach (var placeholder in terminal.Placeholders)
                {
                    templateText = templateText.Replace(placeholder, terminal.GetPlaceholderValue(placeholder));
                }

                foreach (var fragmentKey in fragmentsDictionary.Keys)
                {
                    if (fragmentsDictionary.TryGetValue(fragmentKey, out string insertionText))
                    {
                        templateText = templateText.Replace("@{" + fragmentKey + "}", insertionText);
                    }
                }

                foreach (var outputPath in document.Outputs)
                {
                    var filePath = fileFunctions.GetRootedFilePath(path, outputPath);

                    if (fileFunctions.FileExists(filePath)) fileFunctions.DeleteFile(filePath);
                    fileFunctions.WriteTextFile(filePath, templateText);
                }
            }
        }
    }
}
