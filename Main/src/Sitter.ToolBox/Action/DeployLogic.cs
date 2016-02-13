using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Sitter.Toolbox.Utility;
using Sitter.ToolBox.Views;

namespace Sitter.ToolBox.Controls
{
    public class DeployLogic
    {
        private bool _execute;
        private SmartFeedback _feedback;

        public DeployLogic(SmartFeedback feedback, bool execute)
        {
            _feedback = feedback;
            _execute = execute;
        }

        public void Deploy(DeployEnvironment environment)
        {
            bool startStop = false;
            string displayProfile = "";
            switch (environment)
            {
                case DeployEnvironment.LocalTemp:
                    displayProfile = "Deploy to Local Temp Folder";
                    break;
                case DeployEnvironment.LocalBuildServer:
                    displayProfile = "Deploy to Local Build Server";
                    startStop = true;
                    break;
                case DeployEnvironment.AmazonQa:
                    displayProfile = "Deploy to AWS QA Server";
                    break;
            }

            _feedback.Text("Deploy Profile: " + displayProfile, _feedback.HeaderFontSize, _feedback.HeaderColor);
            _feedback.LineBreak();

            _feedback.Hyperlink("deploy folder", DeployPathConstants.TaasAppDeployFolder);
            _feedback.LineBreak();
            _feedback.LineBreak();

            _feedback.Text("delete deploy folder: " + DeployPathConstants.TaasAppDeployFolder);
            if (_execute)
            {
                if (Directory.Exists(DeployPathConstants.TaasAppDeployFolder))
                    Directory.Delete(DeployPathConstants.TaasAppDeployFolder, true);
                Directory.CreateDirectory(DeployPathConstants.TaasAppDeployFolder);
                _feedback.Text(" deleted and created", null, _feedback.ActionTakenColor);
            }
            _feedback.LineBreak();
            _feedback.LineBreak();

            CopyTaasConsoleFiles();
            CopyToolBox();
            CopyRealTime();
            CopyTaasClientFiles();

            if (startStop)
            {
                _feedback.Text("STEP - Stop Service (if deploying to local build server)", _feedback.HeaderFontSize, _feedback.HeaderColor, true);
                _feedback.LineBreak();
            }
        }

        private void CopyTaasConsoleFiles()
        {
            QuickFolderCopier copier = new QuickFolderCopier(_feedback, _execute);
            copier.Title = "Console.Host";
            copier.SourceDir = DeployPathConstants.SourceConsoleHostBin;
            copier.DestDir = DeployPathConstants.DestConsoleHost;
            copier.DoCopy();
        }

        private void CopyTaasClientFiles()
        {
            QuickFolderCopier copier = new QuickFolderCopier(_feedback, _execute);
            copier.Title = "Taas.Client";
            copier.SourceDir = DeployPathConstants.SourceTaasClient;
            copier.DestDir = DeployPathConstants.DestTaasClient;
            copier.Exclusions.AddRange(new String[]
            {
                @"TaaS.Client\bin",
                @"TaaS.Client\obj",
                @"TaaS.Client\Properties",
                @"TaaS.Client\node_modules",
                @"TaaS.Client\report",
                //@"Taas.Client\src\server", //still using this for data review can remove once we stop serving the static .json
                //@"TaaS.Client\test",
                @"TaaS.Client\thirdparty\kendoui\2014.2.903",
                @"TaaS.Client\.idea"
            });
            copier.DoCopy();

        }


        private void CopyToolBox()
        {
            QuickFolderCopier copier = new QuickFolderCopier(_feedback, _execute);
            copier.Title = "Taas.ToolBox";
            copier.SourceDir = DeployPathConstants.SourceTaasToolBox;
            copier.DestDir = DeployPathConstants.DestTaasToolbox;
            copier.Exclusions.Add(@"Taas.ToolBox\bin\x64\Debug\TestResults");
            copier.DoCopy();

        }

        private void CopyRealTime()
        {
            QuickFolderCopier copier = new QuickFolderCopier(_feedback, _execute);
            copier.Title = "RealTime";
            copier.SourceDir = DeployPathConstants.SourceTaasRealTime;
            copier.DestDir = DeployPathConstants.DestTaasRealTime;
            copier.DoCopy();
        }
    }

    public class QuickFolderCopier
    {
        public QuickFolderCopier(SmartFeedback feedback, bool execute)
        {
            _feedback = feedback;
            _execute = execute;
            Exclusions = new List<string>();
        }

        private SmartFeedback _feedback;
        private bool _execute;

        public string SourceDir { get; set; }
        public string DestDir { get; set; }
        public List<string> Exclusions { get; set; }
        public string Title { get; set; } 

        public void DoCopy()
        {
            var sourceDir = new DirectoryInfo(SourceDir);
            var destDir = new DirectoryInfo(DestDir);

            _feedback.Text("STEP - Copy Folder  " + Title, _feedback.HeaderFontSize, _feedback.HeaderColor, true);
            _feedback.LineBreak();

            DeleteDestDir(destDir);
            _feedback.Text("copy directory structure to: " + destDir.FullName).LineBreak();
            
            CopyAll(sourceDir, destDir, Exclusions);

            _feedback.LineBreak();

        }

        private void DeleteDestDir(DirectoryInfo destDir)
        {
            if (destDir.Exists)
            {
                _feedback.Text("delete dest dir: " + destDir.FullName);
                if (_execute)
                {
                    destDir.Delete(true);
                    _feedback.Text("deleted", null, Colors.Green);
                }
                _feedback.LineBreak();
            }
        }

        private void CopyAll(DirectoryInfo source, DirectoryInfo dest, List<string> exclusions)
        {
            if (!Directory.Exists(dest.FullName))
            {
                if (_execute)
                    Directory.CreateDirectory(dest.FullName);
            }
            if (MainWindow.CancelRequested) throw new ActionCancelException();

            int counter = 0;
            _feedback.Text("source: ").Text(source.FullName, null, _feedback.DataColor);

            foreach (string fiName in Directory.GetFiles(source.FullName))
            {
                FileInfo fiSource = null;
                try
                {
                    fiSource = new FileInfo(fiName);
                }
                catch (PathTooLongException)
                {
                    _feedback.LineBreak().Text("file name too long:" + fiName + " (" + fiName.Length + "), ", null, Colors.Red);
                }
                if (fiSource == null)
                    continue;

                counter++;

                if (_execute)
                {
                    fiSource.CopyTo(Path.Combine(dest.FullName, fiSource.Name), true);
                    var destFile = new FileInfo(Path.Combine(dest.FullName, fiSource.Name));
                    if (destFile.IsReadOnly)
                        destFile.IsReadOnly = false;
                }
            }

            _feedback.Text(" file count:").Text(counter.ToString(), null, _feedback.DataColor).LineBreak();

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                string subDirName = Path.Combine(dest.FullName, diSourceSubDir.Name);
                try
                {
                    bool excluded = exclusions.Any(m => subDirName.ContainsSubstring(m));
                    if (excluded)
                    {
                        _feedback.Text("skipping directory " + subDirName, null, _feedback.ActionTakenColor).LineBreak();
                    }
                    else
                    {
                        var nextTargetSubDir = new DirectoryInfo(subDirName);
                        CopyAll(diSourceSubDir, nextTargetSubDir, exclusions); // recurse
                    }
                }
                catch (PathTooLongException)
                {
                    _feedback.LineBreak().Text("directory name too long:" + subDirName + " (" + subDirName.Length + "), ", null, Colors.Red).LineBreak();
                }
            }
        }

        private const int MaxDirectoryNameLength = 247;
        private const int MaxFileNameLength = 259;

    }

    public static class StringExt
    {
        public static bool ContainsSubstring(this string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}