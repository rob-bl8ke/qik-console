using System.IO;
using System.Reflection;

namespace QikConsoleTests
{
    public class FileHelpers
    {
        // public static string GetFolder() => "..\\..\\..\\..\\QikTests\\Files";
        // public static string GetSubFolder(string appendedPath) => Path.Combine(GetFolder(), appendedPath);
        // public static string ResolvePath(string fileName) => Path.Combine(GetFolder(), fileName);
        // public static string ReadText(string fileName) => File.ReadAllText(ResolvePath(fileName));
        // public static void DeleteFile(string fileName) => File.Delete(ResolvePath(fileName));
        // public static void DeleteDirectory(string directoryName) => Directory.Delete(ResolvePath(directoryName), recursive: true);
        // public static void CreateProjectXml(string directoryName) => File.Create(Path.Combine(GetFilesFolder(), "project.xml"), 1024, FileOptions.None);
        // public static void DeleteProjectXml(string directoryName) => File.Delete(Path.Combine(GetFilesFolder(), "project.xml"));

        public static string GetProjectXml() => File.ReadAllText(Path.Combine(GetFilesFolder(), "projects.xml"));
        public static string GetProjectXmlFilePath() => Path.Combine(GetFilesFolder(), "projects.xml");
        private static string GetFilesFolder() => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Files");
    }
}
