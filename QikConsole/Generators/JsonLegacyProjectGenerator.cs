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
    public class JsonLegacyProjectGenerator
    {
        private Dictionary<string, string> fragmentsDictionary = new();
        private readonly JsonLegacyProjectFile projectFile;
        private readonly IFileFunctions fileFunctions;
        private readonly ILogger logger;

        public JsonLegacyProjectGenerator(JsonLegacyProjectFile projectFile, IFileFunctions fileFunctions, NLog.ILogger logger)
        {
            this.projectFile = projectFile;
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
            var project = projectFile.Read(filePath);
            
            fragmentsDictionary = new Dictionary<string, string>();
            GenerateFragments(filePath, inputs, project);
            GenerateDocuments(filePath, project);
        }

        private void GenerateFragments(string path, Input[] inputs, JsonLegacyProject project)
        {
            var scriptPath = Path.Combine(Path.GetDirectoryName(path), project.ScriptPath);
            var script = fileFunctions.ReadTextFile(scriptPath);
            var interpreter = new Interpreter();
            var symbolTerminal = interpreter.Interpret(new FunctionFactory(new PluginLoader()), script);
            var terminal = new PlaceholderTerminal(symbolTerminal, "@{", "}");

            // Will override the project file
            foreach(var input in inputs)
            {
                terminal.SetSymbolValue($"@{input.Symbol}", input.Value);
            }
            
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

        private void GenerateDocuments(string path, JsonLegacyProject project)
        {
            foreach (var document in project.Documents)
            {
                StringBuilder builder = new StringBuilder();
                foreach(var structure in document.Structure)
                {
                    if (fragmentsDictionary.ContainsKey(structure))
                    {
                        builder.AppendLine(fragmentsDictionary[structure]);
                    }
                }
                
                foreach (var outputPath in document.OutputFilePaths)
                {
                    var filePath = fileFunctions.GetRootedFilePath(path, outputPath);

                    if (fileFunctions.FileExists(filePath)) fileFunctions.DeleteFile(filePath);
                    
                    fileFunctions.WriteTextFile(filePath, builder.ToString());
                }
            }
        }
    }
}
