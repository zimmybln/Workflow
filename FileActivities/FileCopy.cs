using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Drawing;
using System.IO;
using Designer.Contracts;

namespace FileActivities
{
    [ToolboxBitmap(typeof(FileCopy), "FileCopy.png")]
    public sealed class FileCopy : NativeActivity<List<string>>
    {
        public InArgument<List<string>> SourceFiles { get; set; }

        public InArgument<string> TargetDirectory { get; set; } 

        protected override void Execute(NativeActivityContext context)
        {
            string target = context.GetValue<string>(TargetDirectory);
            List<string> sourcefiles = context.GetValue<List<string>>(SourceFiles);
            List<string> targetfiles = new List<string>();

            // check if is anything to do
            if (sourcefiles == null || !sourcefiles.Any())
            {
                Result.Set(context, targetfiles);
                return;
            }

            // check existence before copy files
            foreach (var file in sourcefiles)
            {
                if (!File.Exists(file))
                    throw new FileNotFoundException("file not exists", file);
            }
            
            foreach (var file in sourcefiles)
            {
                var filename = Path.GetFileName(file);
                var targetfilename = Path.Combine(target, filename);

                File.Copy(file, targetfilename);

                targetfiles.Add(targetfilename);
            }

            Result.Set(context, targetfiles);
            
        }
    }
}
