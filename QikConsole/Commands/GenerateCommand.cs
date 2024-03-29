using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using NLog;
using QikConsole;
using static System.Console;

namespace CygSoft.Qik.QikConsole
{
    public class GenerateCommand : BaseCommand
    {
        private readonly ConsoleMessages consoleMessages;
        private readonly IProjectsFile projectsFile;
        private readonly IFileFunctions fileFunctions;
        private readonly ILogger logger;

        public GenerateCommand(ConsoleMessages consoleMessages, IProjectsFile projectsFile, IFileFunctions fileFunctions, ILogger logger)
        {
            this.consoleMessages = consoleMessages;
            this.projectsFile = projectsFile;
            this.fileFunctions = fileFunctions;
            this.logger = logger;
        }

        public override Command Configure()
        {
            var fileOption = new Option<string>(new[] { "--file", "-f" }, "The path to the project configuration file with inputs, settings, and document structure.")
            {
                IsRequired = true,
                Arity = ArgumentArity.ExactlyOne
            };

            var projectOption = new Option<string>(new[] { "--projectKey", "-p" }, "The project to generate.")
            {
                IsRequired = true,
                Arity = ArgumentArity.ExactlyOne
            };

            var typeOption = new Option<string>(new[] { "--type", "-t" }, "Type of project to support. To support the legacy JSON project set to 'legacyjsonproject'.")
            {
                IsRequired = false,
                Arity = ArgumentArity.ExactlyOne
            };

            var inputsOption = new Option<string>(new[] { "--inputs", "-i" }, "Assign inputs to any input variables.")
            {
                IsRequired = false,
                Arity = ArgumentArity.ExactlyOne
            };

            var cmd = new Command("gen", "Generates from a single input set.")
            {
                fileOption,
                projectOption,
                typeOption,
                inputsOption
            };

            // These variables much match identifically with the type parameters or they'll come through as null.
            cmd.Handler = CommandHandler.Create((Action<string, string, string, string>)((file, projectKey, type, inputs) =>
            {
                Execute(file, projectKey, type, inputs);
            }));

            return cmd;
        }

        private void Execute(string filePath, string projectKey, string typeKey, string inputs)
        {
            try
            {
                if (string.IsNullOrEmpty(typeKey) && Path.GetExtension(filePath).ToLower() == ".xml")
                {
                    var generator = new XmlProjectsGenerator(projectsFile, fileFunctions, logger);
                    generator.Execute(filePath, projectKey, inputs);
                }
                else if (typeKey == "legacyjsonproject")
                {
                    var generator = new JsonLegacyProjectGenerator(new JsonLegacyProjectFile(), fileFunctions, logger);
                    generator.Execute(filePath, projectKey, inputs);
                }
                else
                {
                    WriteLine("Generation type not supported");
                }
            }
            catch (Exception ex)
            {
                consoleMessages.DisplayConsoleError(ex);
            }
        }
    }
}