using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Drawing;
using System.IO;

namespace FileActivities
{

    /// <summary>
    /// This activity provides informations about a file.
    /// </summary>
    [ToolboxBitmap(typeof(FileInfo), "FileInfo.png")]
    public sealed class FileInfo : NativeActivity
    {
        public InArgument<string> FileName { get; set; }

        public OutArgument<DateTime> Created { get; set; }

        public OutArgument<DateTime> Modified { get; set; } 

        public OutArgument<long> Length { get; set; } 

        public OutArgument<bool> IsReadOnly { get; set; } 

        public OutArgument<string> Extension { get; set; } 

        public OutArgument<string> Directory { get; set; } 

        public OutArgument<string> FileNameWithoutExtension { get; set; } 
        
        protected override void Execute(NativeActivityContext context)
        {
            var filename = context.GetValue<string>(FileName);

            if (!File.Exists(filename)) 
            {
                throw new FileNotFoundException(filename);
            }

            context.SetValue(Extension, Path.GetExtension(filename));
            context.SetValue(Directory, Path.GetDirectoryName(filename));
            context.SetValue(FileNameWithoutExtension, Path.GetFileNameWithoutExtension(filename));
            
            var fileinfo = new System.IO.FileInfo(filename);
            
            context.SetValue(Created, fileinfo.CreationTime);
            context.SetValue(Length, fileinfo.Length);
            context.SetValue(IsReadOnly, fileinfo.IsReadOnly);
            context.SetValue(Modified, fileinfo.LastWriteTime);

        }
    }
}
