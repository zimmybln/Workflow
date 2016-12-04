using System;
using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActivityDesign.Editors;

namespace FileActivities
{
    public sealed class ListDirectories : NativeActivity<IEnumerable<String>>
    {
        [RequiredArgument]
        [Editor(typeof(FileActivities.Designs.SelectFolderDesigner), typeof(DialogPropertyValueEditor))]
        public InArgument<string> Directory { get; set; }
        
        public InArgument<string> Pattern { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            List<String> files = new List<string>();

            var directory = context.GetValue(Directory);
            var pattern = context.GetValue(Pattern);

            if (String.IsNullOrEmpty(pattern))
            {
                pattern = "*.*";
            }

            try
            {
                files.AddRange(System.IO.Directory.GetFiles(directory, pattern));
            }
            catch (Exception)
            {
                // what to do with the exception here?
            }


            Result.Set(context, files);
        }
    }
}
