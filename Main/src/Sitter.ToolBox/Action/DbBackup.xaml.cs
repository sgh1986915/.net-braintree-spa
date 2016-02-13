using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using MySitterHub.DAL.General;
using Sitter.Toolbox.Utility;

namespace Sitter.ToolBox.Controls
{
    public partial class DbBackup : ActionUserControl
    {

        public DbBackup()
        {
            InitializeComponent();
            ConfigTitle = "Backup MongoDB";
        }

        public override void ApplyChanges(SmartFeedback feedback, bool execute)
        {
            DoBackup(feedback, execute);

        }

        public void DoBackup(SmartFeedback feedback, bool execute)
        {

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = @"c:\program files\mongoDb 2.6 Standard\bin\mongodump";
            info.Arguments = "-d mysitterhub";

            Process p = new Process();

            p.Start();

        }


     
    }
}