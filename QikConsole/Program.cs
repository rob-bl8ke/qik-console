using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.CommandLine;
using CygSoft.Qik;
using CygSoft.Qik.QikConsole;

using NLog;
using NLog.Extensions.Logging;
using QikConsole;

//
// TODO: Find out how to dependency inject the logger in order to mock the interface for unit tests.
//  - Do we need these extensions? Check your QikConsole.csproj file. Remove them if you can't find a use for them.

// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.DependencyInjection.Extensions;

class Program
{
        public static int Main(string[] args)
        {
            ICommandFactory commandFactory = null;
            // FileSettings settings = null;
            ServiceProvider serviceProvider = null;

            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();

            //
            // Keep for now, since we may want to use configuration settings again!
            // settings = config.GetSection(nameof(FileSettings)).Get<FileSettings>();

            LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
            
            var services = new ServiceCollection()
                .AddSingleton<IInterpreter, Interpreter>()
                .AddSingleton<ILogger>(logger => LogManager.Setup().LoadConfigurationFromSection(config).GetCurrentClassLogger())
                .AddSingleton<IFileFunctions>(ah => new FileFunctions())
                .AddSingleton<IProjectsFile, XmlProjectsFile>()
                .AddSingleton<ICommandFactory, CommandFactory>()
                .AddSingleton<ConsoleMessages>()
            ;

            serviceProvider = services.BuildServiceProvider();
            commandFactory = serviceProvider.GetService<ICommandFactory>();
            
            var consoleMessages = serviceProvider.GetService<ConsoleMessages>();
            consoleMessages.DisplayWelcomeHeader();

            var rootCommand = new RootCommand("Qik Console Application");
            var genCommand = commandFactory.Create(CommandType.Generate);
            rootCommand.Add(genCommand);

            //
            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }
}