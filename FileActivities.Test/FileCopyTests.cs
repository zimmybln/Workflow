using System;
using System.Activities;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace FileActivities.Test
{
    [TestFixture()]
    public class FileCopyTests 
    {
        [Test]
        public void CopyFile()
        {
            var target = FromRootDirectory("Data\\Target");
            var source = FromRootDirectory("Data\\Source\\SomeFile1.txt");

            var filecopy = new FileCopy()
            {
                SourceFiles = new InArgument<List<string>>(context => new List<string>(new[] { source })),
                TargetDirectory = new InArgument<string>(target)
            };
            
            WorkflowInvoker.Invoke(filecopy);

            Assert.IsTrue(File.Exists(FromRootDirectory("Data\\Target\\SomeFile1.txt")), "The expected file does not exists");

        }

        [Test]
        public void CopyFiles()
        {
            
        }

        [Test]
        public void CopyFileInUse()
        {
            
        }

        public string FromRootDirectory(string directory)
        {
            var path = Path.GetDirectoryName(new Uri(this.GetType().Assembly.CodeBase).LocalPath);

            return Path.Combine(path, directory);
        }


        [OneTimeSetUp]
        public void OnTestInitialize()
        {
            // create target directory or delete all target files
            var target = FromRootDirectory("Data\\Target");

            if (Directory.Exists(target))
            {
                Directory.GetFiles(target, "*.*").ToList().ForEach(File.Delete);
            }
            else
            {
                Directory.CreateDirectory(target);
            }

        }

    }
}
