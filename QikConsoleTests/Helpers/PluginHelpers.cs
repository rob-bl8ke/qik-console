using CygSoft.Qik;
using CygSoft.Qik.Functions;
using Moq;

namespace QikConsoleTests
{
    public class PluginHelpers
    {
        internal static IPluginLoader StubPluginLoader { get => new Mock<IPluginLoader>().Object; }
    }
}
