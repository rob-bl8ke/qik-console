using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Linq;
using QikConsole;

namespace CygSoft.Qik.QikConsole
{
    using static System.Console;

    public class GenerateMatrixCommand : BaseCommand
    {
        public GenerateMatrixCommand(IProjectsFile projectsFile, IFileFunctions fileFunctions, NLog.ILogger logger) : base (projectsFile, fileFunctions, logger){}

        public override Command Configure()
        {
            var fileOption = new Option<string>( new[] { "--file", "-f" }, "The path to a Qik project configuration file.");
            fileOption.IsRequired = true;
            fileOption.Arity = ArgumentArity.ExactlyOne;

            // Support File Columns
            // Input1=1;Input2=2
            var inputsOption =  new Option<string>(new[] { "--inputs", "-i" }, "Assign inputs to any input variables.");
            inputsOption.IsRequired = true;
            inputsOption.Arity = ArgumentArity.ExactlyOne;

            var matrixFileOption = new Option<string>( new[] { "--matrix-file", "-m" }, "The path to a Qik project configuration file.");
            matrixFileOption.IsRequired = true;
            matrixFileOption.Arity = ArgumentArity.ExactlyOne;

            var ignoreHeaders  = new Option<bool>( new[] { "--headers", "-h" }, "Ignore the first header line.");
            ignoreHeaders.IsRequired = false;
            ignoreHeaders.Arity = ArgumentArity.ZeroOrOne;

            var cmd = new Command("matrix", "Modifies a symbol value.")
            {
                fileOption,
                inputsOption,
                matrixFileOption,
                ignoreHeaders
            };

            cmd.Handler = CommandHandler.Create<string, string, string, bool>((Action<string, string, string, bool>)((file, inputs, matrixFile, headers) =>
            {
                Console.WriteLine("Not supported yet... sorry!");
            }));

            return cmd;
        }
    }
}
