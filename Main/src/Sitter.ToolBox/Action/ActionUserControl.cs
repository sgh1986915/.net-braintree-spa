using System;
using System.Windows.Controls;
using Sitter.Toolbox.Utility;

namespace Sitter.ToolBox.Controls
{
    public class ActionUserControl : UserControl
    {
        public string ConfigTitle { get; set; }

        public bool IsUcLoaded { get; set; }

        public bool HideDoneMessage { get; set; }

        public virtual void ApplyChanges(SmartFeedback feedback, bool execute)
        {

        }


        public bool DeactivateAutoScroll { get; set; }

    }

    public class ActionCancelException : Exception
    {
        public ActionCancelException()
        {
        }

        public ActionCancelException(string message)
            : base(message)
        {
        }

    }
}
