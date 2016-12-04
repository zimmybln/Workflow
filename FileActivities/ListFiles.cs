using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileActivities
{
    public sealed class ListFiles : NativeActivity<IEnumerable<String>>
    {
        [RequiredArgument]
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
