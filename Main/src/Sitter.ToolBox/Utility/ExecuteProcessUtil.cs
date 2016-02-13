using System.IO;
using Sitter.Toolbox.Utility;

namespace Sitter.ToolBox.Utility
{
    public class ExecuteProcessUtil
    {
        public static string ExecuteProcess(SmartFeedback feedback, string exePath, string args, bool execute)
        {
            string result = null;
            if (!File.Exists(exePath))
            {
                feedback.Text("file not found:" + exePath, null, feedback.ErrorColor).LineBreak();
                result = "sln file not found";
            }
            else
            {
                //    const string msBuild = @"c:\windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe";
                //    slnFullName += " /t:rebuild /verbosity:minimal "; // /verbosity:quiet  /verbosity:minimal /verbosity:detailed, 
                //    feedback.LineBreak().Text(msBuild + " " + slnFullName, null, Colors.Gray).LineBreak();
                //    if (execute)
                //    {
                //        var proc = new Process
                //        {
                //            StartInfo = new ProcessStartInfo
                //            {
                //                FileName = msBuild,
                //                Arguments = slnFullName,
                //                UseShellExecute = false,
                //                RedirectStandardOutput = true,
                //                CreateNoWindow = true
                //            }
                //        };
                //        proc.Start();
                //        string all = null;
                //        while (!proc.StandardOutput.EndOfStream)
                //        {
                //            string line = proc.StandardOutput.ReadLine();
                //            all += line + "\n";
                //            feedback.Text(line, null, Colors.Blue).LineBreak();
                //        }
                //        if (all != null && all.Contains(": error M"))
                //        {
                //            buildResult = all;
                //        }
                //        else
                //        {
                //            buildResult = null;
                //        }

                //        proc.WaitForExit();
                //        Thread.Sleep(200);

                //        feedback.Text("BUILD DONE", null, Colors.Black, true).LineBreak();

                //    }
                //    else
                //    {
                //        buildResult = null;
                //    }
            }

            return result;
        }
    }
}