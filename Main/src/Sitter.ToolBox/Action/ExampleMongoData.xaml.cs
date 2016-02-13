using System.Windows;
using System.Windows.Media;
using MySitterHub.DAL.General;
using Sitter.Toolbox.Utility;
using Sitter.ToolBox.Views;

namespace Sitter.ToolBox.Controls
{
    public partial class FakeMongoData : ActionUserControl
    {
        private DataCommandType _command;

        public FakeMongoData()
        {
            InitializeComponent();
            ConfigTitle = "Create Example Data in MongoDB and run Processing";
        }

        public override void ApplyChanges(SmartFeedback feedback, bool execute)
        {
            new ExampleDataGenerator(feedback, execute, _command).CreateExampleData();

#if OFF
            DisplayDatabaseInfo(feedback, execute);
#endif
        }

        public void DisplayDatabaseInfo(SmartFeedback feedback, bool execute)
        {
            const string cnn = "mongodb://localhost";
            const string dataFolder = @"c:\mongodb\data";
            const string logFolder = @"c:\mongodb\log\mongod.log";
            const string mongoConfigFile = @"c:\mongodb\mongod.cfg";

            feedback.LineBreak().LineBreak().LineBreak();
            feedback.Text("STEP - Display MongoDB Info", feedback.HeaderFontSize, feedback.HeaderColor).LineBreak();
            feedback.Text("MongoDB connection:").Text(cnn, null, Colors.Blue).LineBreak();
            feedback.Text("MongoDB data folder:").Text(dataFolder, null, Colors.Blue).LineBreak();
            feedback.Text("MongoDB log folder:").Text(logFolder, null, Colors.Blue).LineBreak();
            feedback.Text("MongoDB config file:").Text(mongoConfigFile, null, Colors.Blue).LineBreak();

            string dbName = "";
            if (execute)
            {
                dbName = MongoCnn.GetDbConnection().Name;
            }
            feedback.Text("MongoDB database:").Text(dbName, null, Colors.Blue).LineBreak();
            feedback.LineBreak();
        }

        private void ChkDropMethods_OnChecked(object sender, RoutedEventArgs e)
        {
            if (rbDropAllCollections.IsChecked == true)
                _command = DataCommandType.DropDbAndCreateExampleData;
            else
                _command = DataCommandType.CreateJobReadyForFinalizedPayment;

            MainWindow.MainWindowInstance.btnValidate_Click(null, null);
        }

        private void RbCreate_OnUnchecked(object sender, RoutedEventArgs e)
        {
            //throw new System.NotImplementedException();
        }

        #region Form Events

        #endregion
    }

    public enum DataCommandType
    {
        DropDbAndCreateExampleData,
        CreateJobReadyForFinalizedPayment
    }
}