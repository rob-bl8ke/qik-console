using System.Collections.Generic;
using CygSoft.Qik.QikConsole;

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
        public string ScriptPath { get; set; }
        public List<Input> Inputs { get; set; } = new();
        public List<Fragment> Fragments { get; set; } = new();
        public List<Document> Documents { get; set; } = new ();
    }
}