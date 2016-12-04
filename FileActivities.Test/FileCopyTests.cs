using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActivityTests;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace FileActivities.Test
{
    [TestFixture()]
    public class FileCopyTests : ActivityTestBase
    {

        [TestCase("SomeFile1.txt")]
        public void CopySingleFileAsArgument(string fileName)
        {
            var target = FromRootDirectory("Data\\Target");
            var source = FromRootDirectory($"Data\\Source\\{fileName}");

            var filecopy = new FileCopy()
            {
                SourceFiles = new InArgument<List<string>>(context => new List<string>(new[] { source })),
                TargetDirectory = target
            };

            var app = new WorkflowTestApplication(filecopy);

            var result = app.Run();

            var expectedfilename = FromRootDirectory($"Data\\Target\\{fileName}");

            Assert.IsTrue((result["Result"] as List<string>)?.Contains(expectedfilename), "The expected file is not a part of the result");

            Assert.IsTrue(File.Exists(expectedfilename), "The expected file does not exists");
        }

        [TestCase("SomeFile2.txt")]
        public void CopySingleFileAsVariable(string fileName)
        {
            var target = FromRootDirectory("Data\\Target");
            var source = FromRootDirectory($"Data\\Source\\{fileName}");

            var sourcevariable = new Variable<List<string>>()
            {
                Name = "Source"
            };

            var sequence = new Sequence()
            {
                Variables = { sourcevariable },
                Activities =
                {
                    new Assign<List<string>>()
                    {
                        To = OutArgument<List<string>>.FromVariable(sourcevariable),
                        Value = new InArgument<List<string>>(context => new List<string>(new[] {source}))
                    },
                    new FileCopy()
                    {
                        SourceFiles = new InArgument<List<string>>(sourcevariable),
                        TargetDirectory = new InArgument<string>(target)
                    }
                }
            };

            var app = new WorkflowTestApplication(sequence);

            app.Run();

            var expectedfilename = FromRootDirectory($"Data\\Target\\{fileName}");

            Assert.IsTrue(File.Exists(expectedfilename), "The expected file does not exists");

        }

        [TestCase("SomeFile3.txt")]
        public void CopySingleFileAsInput(string fileName)
        {
            var target = FromRootDirectory("Data\\Target");
            var source = FromRootDirectory($"Data\\Source\\{fileName}");

            var filecopy = new FileCopy()
            {
                TargetDirectory = new InArgument<string>(target)
            };

            var app = new WorkflowTestApplication(filecopy);

            app.Inputs.Add("SourceFiles", new List<string>(new[] {source}));

            app.Run();

            Assert.IsTrue(File.Exists(FromRootDirectory($"Data\\Target\\{fileName}")), "The expected file does not exists");
        }

        [Test]
        public void CopyFilesNotExists()
        {
            var target = FromRootDirectory("Data\\Target");
            var source = FromRootDirectory("Data\\Source\\SomeFileNotExists.txt");

            var filecopy = new FileCopy()
            {
                TargetDirectory = new InArgument<string>(target)
            };

            var app = new WorkflowTestApplication(filecopy);

            app.Inputs.Add("SourceFiles", new List<string>(new[] { source }));

            var result = app.Run();

            Assert.IsTrue(result.Exception != null && result.Exception.GetType() == typeof(FileNotFoundException), "exception FileNotFoundException wasn't thrown");
        }

        [OneTimeSetUp]
        public void OnTestInitialize()
        {
            // create source directory and ensure the existence of files
            var source = FromRootDirectory("Data\\Source");
            var target = FromRootDirectory("Data\\Target");

            EnsureDirectory(source, false);

            CreateTextFiles(source, 10, i => $"SomeFile{i + 1}.txt");
            
            EnsureDirectory(target, true);
        }

    }
}
