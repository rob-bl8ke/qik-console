using System.Collections.Generic;

namespace QikConsole
{

    public class Project
    {
        public class Fragment
        {
            public string Id { get; set; }
            public string Path { get; set; }
        }

        public class Document
        {
            public string[] Outputs { get; set; }
            public string[] Structure { get; set; }
        }

        public string Key { get; set; }
        public string Title  { get; set; }
        public string ScriptFile { get; set; }
        public List<Fragment> Fragments { get; set; } = new List<Fragment>();
        public List<Document> Documents { get; set; } = new List<Document>();
    }
}