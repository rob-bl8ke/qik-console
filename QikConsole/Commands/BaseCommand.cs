using System;
using System.CommandLine;

namespace CygSoft.Qik.QikConsole
{
    using static System.Console;

    public abstract class BaseCommand
    {
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