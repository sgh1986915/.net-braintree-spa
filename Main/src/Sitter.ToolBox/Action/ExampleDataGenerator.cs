using System.Diagnostics;
using MySitterHub.DAL.General;
using Sitter.ToolBox.Action;
using Sitter.Toolbox.Utility;

namespace Sitter.ToolBox.Controls
{
    public class ExampleDataGenerator
    {
        private readonly DataCommandType _command;
        private readonly bool _execute;
        private readonly SmartFeedback _feedback;
        private readonly Stopwatch _sw = new Stopwatch();

        public ExampleDataGenerator(SmartFeedback feedback, bool execute, DataCommandType command)
        {
            _feedback = feedback;
            _execute = execute;
            _command = command;
        }

        public void CreateExampleData()
        {
            if (_command == DataCommandType.DropDbAndCreateExampleData)
            {
                _feedback.Text("STEP - Drop All collections", _feedback.HeaderFontSize, _feedback.HeaderColor).LineBreak();
                if (_execute)
                {
                    MongoCnn.GetDbConnection().Drop();
                    _feedback.Text("done").LineBreak().LineBreak();
                }

                _feedback.Text("STEP - Example MongoDB Data: parents, sitters and jobs", _feedback.HeaderFontSize, _feedback.HeaderColor).LineBreak();

                if (_execute)
                {
                    new ExampleDataMaker().InsertExampleData();
                }
            }

            if (_command == DataCommandType.CreateJobReadyForFinalizedPayment)
            {
                _feedback.Text("STEP - Insert Job ready for payment", _feedback.HeaderFontSize, _feedback.HeaderColor).LineBreak();
                if (_execute)
                {
                    new ExampleDataMaker().InsertJobReadyForPayment();
                }
            }

            _feedback.LineBreak();
        }
    }
}