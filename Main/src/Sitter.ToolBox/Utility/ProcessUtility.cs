using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using Sitter.Toolbox.Utility;

namespace Sitter.ToolBox.Utility
{
    public class ServiceKillerBirther
    {
        public static Process GetProcessByName(string processName)
        {
            Process[] p = Process.GetProcessesByName(processName);
            if (p.Length > 0)
            {
                return p[0];
            }
            return null;
        }

        public static void Kill(SmartFeedback feedback, bool execute, string processName)
        {
            feedback.Text("kill process '" + processName + "'").LineBreak();

            Process p = GetProcessByName(processName);

            if (p != null)
            {
                if (execute)
                {
                    p.Kill();
                    feedback.Text(" killed", null, Colors.Green);
                }
            }
            else
            {
                feedback.Text(" no process by that name", null, Colors.Green);
            }
            feedback.LineBreak();
        }

        public static void Birth(FileInfo exe, SmartFeedback feedback, bool execute)
        {
            feedback.Text("Start process '" + exe.FullName + "'");
            if (!exe.Exists)
            {
                feedback.Text(" file not found", null, feedback.ErrorColor);
            }
            else
            {
                if (execute)
                {
                    Process.Start(exe.FullName);
                    feedback.Text(" started", null, Colors.Green);
                }
            }

            feedback.LineBreak();
        }
    }
}