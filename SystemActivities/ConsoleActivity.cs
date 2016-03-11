using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Diagnostics;

namespace SystemActivities
{

    public sealed class ConsoleActivity : NativeActivity<String>
    {
        public InArgument<string> CommandLine { get; set; } 

        protected override void Execute(NativeActivityContext context)
        {
            string commandline = context.GetValue(this.CommandLine);

            try
            {
                var startinfo = new ProcessStartInfo();
                startinfo.FileName = "cmd.exe";
                startinfo.UseShellExecute = false;
                startinfo.RedirectStandardInput = true;
                startinfo.RedirectStandardOutput = true;
                startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                startinfo.CreateNoWindow = true;
                
                var process = new Process();
                process.StartInfo = startinfo;

                if (process.Start())
                {
                    process.StandardInput.WriteLine(commandline);
                    process.StandardInput.Flush();
                    process.StandardInput.Close();

                    process.WaitForExit();

                    var result = process.StandardOutput.ReadToEnd();

                    Result.Set(context, result);
                }

            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
