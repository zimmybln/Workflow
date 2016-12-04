using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityTests
{
    public abstract class ActivityTestBase
    {

        /// <summary>
        /// Combines a path relative to execution directory
        /// </summary>
        protected string FromRootDirectory(string directory)
        {
            var path = Path.GetDirectoryName(new Uri(this.GetType().Assembly.CodeBase).LocalPath);

            return Path.Combine(path, directory);
        }

        protected void EnsureDirectory(string directory, bool isEmpty)
        {
            if (Directory.Exists(directory) && isEmpty)
            {
                Directory.GetFiles(directory, "*.*").ToList().ForEach(File.Delete);
            }
            else
            {
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// Deletes all files and directories.
        /// </summary>
        protected void ResetDirectory(string directory)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                File.Delete(file);
            }
            
            foreach (var dir in Directory.GetDirectories(directory))
            {
                ResetDirectory(dir);
                Directory.Delete(dir);
            }
        }

        protected void CreateTextFiles(string directory, int number, Func<int, string> namecreationFunc)
        {
            for (int i = 0; i < number; i++)
            {
                string filename = directory + "\\" + namecreationFunc(i);

                if (!File.Exists(filename))
                {
                    using (var filestream = File.OpenWrite(filename))
                    using (var writer = new StreamWriter(filestream))
                    {
                        writer.Write("This is some content");
                    }
                }
            }
        }
    }
}
