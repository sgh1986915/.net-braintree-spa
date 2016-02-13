using System.Windows;
using Sitter.ToolBox.Controls;
using Sitter.Toolbox.Utility;
using Sitter.ToolBox.Views;

namespace Sitter.ToolBox.Controls
{
    public partial class Deploy
    {
        private DeployEnvironment _environment;

        public Deploy()
        {
            InitializeComponent();
            ConfigTitle = "Prep Deploy";

            // Events
            rbLocalBuild.Visibility = Visibility.Collapsed; //temp
            rbLocalTemp.Visibility = Visibility.Collapsed; //temp
            rbAmazonQA.Visibility = Visibility.Collapsed; //temp

            rbLocalTemp.IsChecked = true;
            rbLocalTemp.Checked += Radio_OnChecked;
            rbLocalBuild.Checked += Radio_OnChecked;
            rbAmazonQA.Checked += Radio_OnChecked;
            
            DeactivateAutoScroll = true;
        }

        public override void ApplyChanges(SmartFeedback feedback, bool execute)
        {
            new DeployLogic(feedback, execute).Deploy(_environment);
        }

        #region Form Events

        private void Radio_OnChecked(object sender, RoutedEventArgs e)
        {
            if (rbLocalTemp.IsChecked == true)
            {
                _environment = DeployEnvironment.LocalTemp;
            }
            else if (rbLocalBuild.IsChecked == true)
            {
                _environment = DeployEnvironment.LocalBuildServer;
            }
            else if (rbAmazonQA.IsChecked == true)
            {
                _environment = DeployEnvironment.AmazonQa;
            }

            if (IsUcLoaded)
                MainWindow.MainWindowInstance.btnValidate_Click(null, null);
        }

        #endregion
    }

    public enum DeployEnvironment
    {
        LocalTemp,
        LocalBuildServer,
        AmazonQa
    }
}