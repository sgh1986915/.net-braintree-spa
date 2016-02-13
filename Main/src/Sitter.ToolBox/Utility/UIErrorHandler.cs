using System;
using System.Windows;

namespace Sitter.Toolbox.Utility
{
    public static class UIErrorHandler
    {

        public static void DisplayError(Exception ex, string header = null)
        {

            ExceptionDialog.ShowExceptionDialog(ex, header);
        }

        /// <summary>
        ///   Simply display messagebox. Note, message is not logged.
        /// </summary>
        public static void DisplayMessage(string message)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<string>(DisplayMessage), message);
                return;
            }

            var win = UIControlsHelper.GetCurrentActiveWindow();
            if (win != null)
            {
                MessageBox.Show(win, message);
            }
            else
            {
                MessageBox.Show(message);
            }
        }

        public static string FormatErrorForUserDisplay(string header, Exception ex = null, string location = null)
        {
            string msg = ex == null ? null : string.Format(" Message:{0}. ", ex.Message);
            string loc = location == null ? null : string.Format(" Location:{0}.", location);
            return string.Format("Error: {0}.{1}{2}", header, msg, loc);
        }
    }
}