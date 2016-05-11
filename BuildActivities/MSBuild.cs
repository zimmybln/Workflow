using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using BuildActivities.Designs;
using Designer.Contracts;
using Designer.Contracts.Attributes;


namespace BuildActivities
{

    /// <summary>
    /// https://msdn.microsoft.com/de-de/library/ms164311.aspx
    /// </summary>
    [DesignerActivityOptions(typeof(BuildDesignerOptionsFactory), typeof(BuildDesignerOptions))]
    public sealed class MSBuild : NativeActivity<string>
    {       
        [RequiredArgument]
        [Editor(typeof(SelectExecutableFileEditor), typeof(DialogPropertyValueEditor))]
        public InArgument<string> ProjectFile { get; set; } 
        
        public InArgument<string> OutputDirectory { get; set; }  

        // Wenn durch die Aktivität ein Wert zurückgegeben wird, erfolgt eine Ableitung von CodeActivity<TResult>
        // und der Wert von der Ausführmethode zurückgegeben.
        protected override void Execute(NativeActivityContext context)
        {
            string executable;

            string project = context.GetValue(this.ProjectFile);
            string outputdirectory = context.GetValue(this.OutputDirectory);
             
            var options = context.GetExtension<IDesignerOptionsExtension>();

            var buildoptions = options.GetOptions<BuildDesignerOptions>();

            if (buildoptions != null)
            {
                executable = buildoptions.Executable;
            }

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
                //var startinfo = new ProcessStartInfo();
                //startinfo.FileName = executable;
                //startinfo.Arguments = commandline.ToString();
                //startinfo.UseShellExecute = false;
                //startinfo.RedirectStandardOutput = true;
                //startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                //startinfo.CreateNoWindow = true;

                //var process = new Process {StartInfo = startinfo};

                //if (process.Start())
                //{
                //    process.WaitForExit();

                //    // issue: wrong encoding for german characters
                //    var result = process.StandardOutput.ReadToEnd();
                    

                //    Result.Set(context, result);
                //}

            }
            catch (Exception)
            {
                throw;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        { 
            base.CacheMetadata(metadata);

            var project = (ProjectFile?.Expression as Literal<string>)?.Value;

            if (!String.IsNullOrEmpty(project) && !File.Exists(project))
            {
                // metadata.AddValidationError($"MSBuild project or solution file '{project}' is invalid or does not exist");
            }

            metadata.RequireExtension(typeof(IDesignerOptionsExtension));
        }
    }
}
