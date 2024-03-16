using CygSoft.Qik;
using CygSoft.Qik.QikConsole;
using CygSoft.Qik.Functions;
using NUnit.Framework;

namespace QikConsoleTests
{
    [TestFixture]
    class PlaceholderTerminalTests
    {
        [Test]
        public void Should_Get_The_Value_For_A_Placeholder_Symbol()
        {
            var interpreter = new Interpreter();
            var symbolTerminal = interpreter.Interpret(new FunctionFactory(PluginHelpers.StubPluginLoader), "@InputVar => \"Hello World\";");
            var terminal = new PlaceholderTerminal(symbolTerminal, "@{", "}");

            Assert.AreEqual("Hello World", terminal.GetPlaceholderValue("@{InputVar}"));
        }

        [Test]
        public void Should_Get_The_Updated_Value_For_A_Placeholder_Symbol()
        {
            var script =
                @"
                    @Entity => ""Hello World"";
                    @TestDesc => @Entity == ""Hello World"" ? ""happy"" : ""sad"";
                ";

            var interpreter = new Interpreter();
            var symbolTerminal = interpreter.Interpret(new FunctionFactory(PluginHelpers.StubPluginLoader), script);
            var terminal = new PlaceholderTerminal(symbolTerminal, "@{", "}");
            var originalValue = terminal.GetPlaceholderValue("@{TestDesc}");
            
            terminal.SetSymbolValue("@Entity", "Goodbye World");
            var newValue = terminal.GetPlaceholderValue("@{TestDesc}");

            Assert.AreEqual("happy", originalValue);
            Assert.AreEqual("sad", newValue);
        }

        [Test]
        public void Should_Get_Current_Input_Symbols()
        {
            var script =
                @"
                    @Entity => ""Hello World"";
                    @Release => ""Release"";
                    @TestDesc => camelCase(@Entity) == ""Hello World"" ? ""happy"" : ""sad"";
                ";

            var interpreter = new Interpreter();
            var symbolTerminal = interpreter.Interpret(new FunctionFactory(PluginHelpers.StubPluginLoader), script);
            var terminal = new PlaceholderTerminal(symbolTerminal, "@{", "}");

            Assert.AreEqual(2, terminal.InputSymbols.Length);
            Assert.Contains("@Entity", terminal.InputSymbols);
            Assert.Contains("@Release", terminal.InputSymbols);
        }

        [Test]
        public void Should_Get_Current_Placeholders()
        {
            var script =
                @"
                    @Entity => ""Hello World"";
                    @Release => ""Release"";
                    @TestDesc => camelCase(@Entity) == ""Hello World"" ? ""happy"" : ""sad"";
                ";

            var interpreter = new Interpreter();
            var symbolTerminal = interpreter.Interpret(new FunctionFactory(PluginHelpers.StubPluginLoader), script);
            var terminal = new PlaceholderTerminal(symbolTerminal, "@{", "}");

            Assert.AreEqual(3, terminal.Placeholders.Length);
            Assert.Contains("@{Entity}", terminal.Placeholders);
            Assert.Contains("@{Release}", terminal.Placeholders);
            Assert.Contains("@{TestDesc}", terminal.Placeholders);
        }
    }
}