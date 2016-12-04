using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Drawing;
using System.IO;

namespace FileActivities 
{

    [ToolboxBitmap(typeof(FileDelete), "FileDelete.png")]
    public sealed class FileDelete : NativeActivity
    {
        public InArgument<List<string>> SourceFiles { get; set; }

        // Wenn durch die Aktivität ein Wert zurückgegeben wird, erfolgt eine Ableitung von CodeActivity<TResult>
        // und der Wert von der Ausführmethode zurückgegeben.
        protected override void Execute(NativeActivityContext context)
        {
            List<string> sourcefiles = context.GetValue<List<string>>(SourceFiles);

            if (sourcefiles != null && sourcefiles.Any())
            {               
                foreach (var file in sourcefiles)
                {
                    File.Delete(file);
                }
            }

        }
    }
}
