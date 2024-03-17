using System.IO;
using System.Text;
using System.Text.Json;
using CygSoft.Qik.QikConsole;

namespace QikConsole
{
    public class JsonLegacyProjectFile
    {
        public void Write(Project project, string path)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
            var result = new StringBuilder();

            result.Append(JsonSerializer.Serialize<Project>(project, options));

            WriteTextFile(path, result.ToString());
        }

        public JsonLegacyProject Read(string path)
        {
            var text = ReadTextFile(path);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var project = JsonSerializer.Deserialize<JsonLegacyProject>(text, options);
            return project;
        }

        private string ReadTextFile(string filePath)
        {
            string contents = null;
            
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    contents = sr.ReadToEnd();
                }
            }
            return contents;
        }

        private void WriteTextFile(string path, string contents)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(contents);
                streamWriter.Flush();
            }
        }
    }
}