using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using ActivityTests;
using BuildActivities.Designs;
using BuildActivities.Test.Extensions;
using NUnit.Framework;

namespace BuildActivities.Test
{
    [TestFixture]
    public class MsBuildTests : ActivityTestBase
    {
        private TestDesignerOptionsExtensions _designerOptionsExtensions;

        [TestCase("DemoClassLibrary1.zip")]
        public void JustCompile(string fileName)
        {
            var project = FromRootDirectory($"Data\\Source\\{fileName}");
            var target = FromRootDirectory($"Data\\Target\\{MethodBase.GetCurrentMethod().Name}");

            ZipFile.ExtractToDirectory(project, target);

            var solution =
                FromRootDirectory(
                    $"Data\\Target\\{MethodBase.GetCurrentMethod().Name}\\{Path.GetFileNameWithoutExtension(fileName)}{".sln"}");

            var build = new MSBuild()
            {
                ProjectFile = solution
            };

            var app = new WorkflowTestApplication(build);

            var result = app.Run(_designerOptionsExtensions);

            Debug.WriteLine(result.Outputs["Result"]);

            Assert.IsTrue(result.Exception == null, "The execution an exception occured");


        }

        [OneTimeSetUp]
        public void OnTestInitialize()
        {
            var target = FromRootDirectory("Data\\Target");

            ResetDirectory(target);

            _designerOptionsExtensions = new TestDesignerOptionsExtensions();

            var msbuildextension = new BuildDesignerOptions();

            msbuildextension.Executable = @"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe";

            _designerOptionsExtensions.AddExtension(msbuildextension);

        }
    }
}
