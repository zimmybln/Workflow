using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Activities.Expressions;
using System.Diagnostics;
using System.IO;

namespace BuildActivities
{

    /// <summary>
    /// https://msdn.microsoft.com/de-de/library/ms164311.aspx
    /// </summary>
    public sealed class MSBuild : NativeActivity<string>
    {
        [RequiredArgument]
        public InArgument<string> ExecutablePath { get; set; }
        
        [RequiredArgument]
        public InArgument<string> ProjectPath { get; set; } 
        
        public InArgument<string> OutputDirectory { get; set; }  

        // Wenn durch die Aktivität ein Wert zurückgegeben wird, erfolgt eine Ableitung von CodeActivity<TResult>
        // und der Wert von der Ausführmethode zurückgegeben.
        protected override void Execute(NativeActivityContext context)
        {
            
            string executable = context.GetValue(this.ExecutablePath);

            string project = context.GetValue(this.ProjectPath);
            string outputdirectory = context.GetValue(this.OutputDirectory);


            // construct the command line
            var propertydict = new Dictionary<string, string>();

            if (!String.IsNullOrWhiteSpace(outputdirectory) && Directory.Exists(outputdirectory))
            {
                propertydict.Add("OutDir", outputdirectory);
            }
            
            var commandline = new StringBuilder($"{(char)34}{project}{(char)34} /m ");

            if (propertydict.Any())
            {
                commandline.Append(" /property:");

                propertydict.ToList().ForEach(p => commandline.Append($"{p.Key}={p.Value}"));

                commandline.Append(" ");
            }
            
            try
            {
                var startinfo = new ProcessStartInfo();
                startinfo.FileName = executable;
                startinfo.Arguments = commandline.ToString();
                startinfo.UseShellExecute = false;
                startinfo.RedirectStandardOutput = true;
                startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                startinfo.CreateNoWindow = true;

                var process = new Process {StartInfo = startinfo};

                if (process.Start())
                {
                    process.WaitForExit();

                    // issue: wrong encoding for german characters
                    var result = process.StandardOutput.ReadToEnd();
                    

                    Result.Set(context, result);
                }

            }
            catch (Exception)
            {
                throw;
            }

        }


        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            var path = (ExecutablePath?.Expression as Literal<string>)?.Value;

            if (!String.IsNullOrEmpty(path) && !File.Exists(path))
            {
                metadata.AddValidationError($"MSBuild executable file '{path}' is invalid or does not exist");
            }

            var project = (ProjectPath?.Expression as Literal<string>)?.Value;

            if (!String.IsNullOrEmpty(project) && !File.Exists(project))
            {
                metadata.AddValidationError($"MSBuild project or solution file '{project}' is invalid or does not exist");
            }
        }
    }
}
