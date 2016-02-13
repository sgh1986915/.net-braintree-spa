using System;
using System.Diagnostics;
using System.Windows;
using Sitter.Toolbox.Model;

namespace Sitter.Toolbox.Utility
{
    public  class ExceptionDialogViewModel
    {
        public ExceptionDialogViewModel()
        {
            EmailErrorCommand = new RelayCommand(DoEmailError);
            CopyErrorToClipBoardCommand = new RelayCommand(DoCopyErrorToClipBoard);
        }

        public string Header { get; set; }

        public string HeaderDisplay
        {
            get
            {
                return Header;
            }
        }

        public string ErrorDetail { get; set; }

        public const string SupportEmail = "";
        
        public double DefaultDetailHeight
        {
            get { return 300; }
        }

        #region Methods 

        public RelayCommand EmailErrorCommand { get; set; }

        public RelayCommand CopyErrorToClipBoardCommand { get; set; }
        
        public void DoEmailError(object param)
        {
            const string subject = "Error report ";

            string body = FormatErrorDetail();

            body += GetEmailBodyFooter();

            SendHelpEmailInDefaultMailClient(SupportEmail, subject, body);
        }

        public static string GetEmailBodyFooter()
        {
            string footer = null;
            footer += "\r\n";
            footer += "\r\n";
            footer += string.Format("\r\n\r\nDate: {0} {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
            //string version = DIManager.Instance.VersionCheck.ReportingPluginVersion;
            //footer += string.Format("\r\n\r\nReporting Plugin Version: {0}", version);
            return footer;
        }

        private string FormatErrorDetail()
        {
            return string.Format("Error:\r\n{0}\r\n\r\nDetail:\r\n{1}", Header, ErrorDetail);
        }

        public static void SendHelpEmailInDefaultMailClient(string to, string subject, string body)
        {
            try
            {
                string link = string.Format("mailto:{0}?subject={1}&body={2}", to, subject, body);
                link = Uri.EscapeUriString(link);
                Process.Start(link);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void DoCopyErrorToClipBoard(object param)
        {
            try
            {
                Clipboard.SetText(FormatErrorDetail());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion // Methods


    }
}
 