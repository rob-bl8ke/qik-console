using NUnit.Framework;
using System.Linq;
using QikConsole;


namespace QikConsoleTests
{
    [TestFixture]
    class XmlProjectFileTests
    {
        [Test]
        public void Should_Get_Project_Xml_File()
        {
            string scriptText = FileHelpers.GetProjectXml();
            Assert.That(!string.IsNullOrEmpty(scriptText));
        }

        [Test]
        public void Should_Get_Root_Element()
        {
            Assert.AreEqual("projects", ProjectXmlFileHelper.GetRootElementName());
        }

        [Test]
        public void Should_Get_Project_Settings()
        {
            XmlProjectsFile file = new XmlProjectsFile();
            file.Load(FileHelpers.GetProjectXmlFilePath());
            Project project = file.GetProject("nt");

            Assert.IsNotNull(project);
            Assert.AreEqual("nt", project.Key);
            Assert.AreEqual("New Task", project.Title);
            Assert.AreEqual(@"C:\Dev\Qik-Gen\project.qik", project.ScriptPath);
        }

        [Test]
        public void Should_Get_Project_Fragments()
        {
            XmlProjectsFile file = new XmlProjectsFile();
            file.Load(FileHelpers.GetProjectXmlFilePath());
            Project project = file.GetProject("nt");

            Assert.AreEqual(2, project.Fragments.Count());
            Assert.AreEqual("fragment1", project.Fragments[0].Id);
            Assert.AreEqual(@"..\fragments\fragment1.qikt", project.Fragments[0].Path);
            Assert.AreEqual("fragment2", project.Fragments[1].Id);
            Assert.AreEqual(@"..\fragments\fragment2.qikt", project.Fragments[1].Path);
        }

        [Test]
        public void Should_Get_Project_Document_Path()
        {
            XmlProjectsFile file = new XmlProjectsFile();
            file.Load(FileHelpers.GetProjectXmlFilePath());
            Project project = file.GetProject("nt");

            Assert.AreEqual(".\\documents\\document1.qikt", project.Documents[0].Path);
        }

        [Test]
        public void Should_Get_Project_Document_Key()
        {
            XmlProjectsFile file = new XmlProjectsFile();
            file.Load(FileHelpers.GetProjectXmlFilePath());
            Project project = file.GetProject("nt");

            Assert.AreEqual("document1", project.Documents[0].Key);
        }

        [Test]
        public void Should_Get_Project_Document_Outputs()
        {
            XmlProjectsFile file = new XmlProjectsFile();
            file.Load(FileHelpers.GetProjectXmlFilePath());
            Project project = file.GetProject("nt");

            Assert.AreEqual(2, project.Documents[0].Outputs.Count());
            Assert.AreEqual(@"..\_testing\usecase-diagrams.md", project.Documents[0].Outputs[0]);
            Assert.AreEqual("usecase-diagrams.md", project.Documents[0].Outputs[1]);
        }
    }
}
