using System;
using System.Activities;
using System.Diagnostics;
using System.IO;
using ActivityTests;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace FileActivities.Test
{
    [TestFixture()]
    public class FileInfoTests
    {
        [Test]
        public void GetFileInfo()
        {
            var source = FromRootDirectory("Data\\Source\\SomeFile1.txt");

            var fileinfoactivity = new FileInfo()
            {
                FileName = source
            };

            var app = new WorkflowTestApplication(fileinfoactivity);

            var result = app.Run();

            var created = (DateTime)result["Created"];
            var length = (long)result["Length"];

            Assert.IsTrue(created < DateTime.Now, "created isn't past");
            Assert.IsTrue(length > 0, "length isn't a correct value");
        } 

        public string FromRootDirectory(string directory)
        {
            var path = Path.GetDirectoryName(new Uri(this.GetType().Assembly.CodeBase).LocalPath);

            return Path.Combine(path, directory);
        }
    }
}