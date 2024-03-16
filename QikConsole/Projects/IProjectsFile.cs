namespace QikConsole
{
    public interface IProjectsFile
    {
        void Load(string path);
        public Project GetProject(string key);
    }
}