using System;
using System.CommandLine;
using QikConsole;

namespace CygSoft.Qik.QikConsole
{
    using static System.Console;

    public abstract class BaseCommand
    {
        protected readonly NLog.ILogger logger;
        protected readonly IProjectsFile projectsFile;
        protected readonly IFileFunctions fileFunctions;

        public BaseCommand(IProjectsFile projectsFile, IFileFunctions fileFunctions, NLog.ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException($"{nameof(logger)} cannot be null.");
            this.projectsFile = projectsFile ?? throw new ArgumentNullException($"{nameof(projectsFile)} cannot be null.");
            this.fileFunctions = fileFunctions ?? throw new ArgumentNullException($"{nameof(fileFunctions)} cannot be null.");
        }

        public abstract Command Configure();

        protected void DisplayWelcomeHeader()
        {
            ForegroundColor = ConsoleColor.Blue;
            WriteLine(new Resources().GetWelcomeHeader());
            ForegroundColor = ConsoleColor.White;
        }

        protected void DisplayConsoleError(Exception ex)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("An error occurred!");
            WriteLine($"\t{ex.Message}");
            WriteLine("Please check the error logs");
            ForegroundColor = ConsoleColor.White;
        }
    }
}