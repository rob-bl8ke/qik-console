using System;
using System.CommandLine;
using QikConsole;

namespace CygSoft.Qik.QikConsole
{
    using static System.Console;

    public enum CommandType
    {
        GenerateSimple,
        GenerateMatrix
    }

    public interface ICommandFactory
    {
        public Command Create(CommandType commandType);
    }

    public class CommandFactory : ICommandFactory
    {
        private readonly NLog.ILogger logger;
        private readonly IProjectsFile projectsFile;
        private readonly IFileFunctions fileFunctions;

        public CommandFactory(IProjectsFile projectsFile, IFileFunctions fileFunctions, NLog.ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException($"{nameof(logger)} cannot be null.");
            this.projectsFile = projectsFile ?? throw new ArgumentNullException($"{nameof(projectsFile)} cannot be null.");
            this.fileFunctions = fileFunctions ?? throw new ArgumentNullException($"{nameof(fileFunctions)} cannot be null.");
        }

        public Command Create(CommandType commandType)
        {
            switch (commandType)
            {
                case CommandType.GenerateSimple:
                    return new GenerateSimpleCommand(projectsFile, fileFunctions, logger).Configure();

                case CommandType.GenerateMatrix:
                    return new GenerateMatrixCommand(projectsFile, fileFunctions, logger).Configure();
                
                default:
                    throw new NotImplementedException();
            }
        }
    }
}