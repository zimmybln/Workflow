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
    public sealed class FileCopy : NativeActivity
    {
        // Aktivitätseingabeargument vom Typ "string" definieren
        public InArgument<List<string>> SourceFiles { get; set; }

        public InArgument<string> TargetDirectory { get; set; } 

        // Wenn durch die Aktivität ein Wert zurückgegeben wird, erfolgt eine Ableitung von CodeActivity<TResult>
        // und der Wert von der Ausführmethode zurückgegeben.
        protected override void Execute(NativeActivityContext context)
        {
            // Laufzeitwert des Texteingabearguments abrufen
            string target = context.GetValue<string>(TargetDirectory);
            List<string> sourcefiles = context.GetValue<List<string>>(SourceFiles);

            if (sourcefiles != null && sourcefiles.Any())
            {
                foreach (var file in sourcefiles)
                {
                    var filename = Path.GetFileName(file);
                    var targetfilename = Path.Combine(target, filename);


                    File.Copy(file, targetfilename);
                }
            }
            
        }
    }
}
