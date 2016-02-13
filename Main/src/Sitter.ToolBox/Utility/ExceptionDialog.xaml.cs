using System;
using System.Windows;
using System.Windows.Input;

namespace Sitter.Toolbox.Utility
{
    public partial class ExceptionDialog : Window
    {
        public ExceptionDialog()
        {
            InitializeComponent();
            DataContext = new ExceptionDialogViewModel();
        }

        public ExceptionDialogViewModel MyViewModel
        {
            get { return (ExceptionDialogViewModel) DataContext; }
        }

        public static void ShowExceptionDialog(Exception ex, string header = null)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<Exception, string>(ShowExceptionDialog), ex, header);
                return;
            }


            Mouse.OverrideCursor = null; // Turn off wait cursor

            var dialog = new ExceptionDialog();

            string hm = header == null ? null : header + "\r\n";
            string exType = ex == null ? "" : ex.GetType().Name;
            string em = string.Format("{0}: {1}", exType, ex == null ? "" : ex.Message);
            dialog.MyViewModel.Header = string.Format("{0}{1}", hm, em);
            dialog.MyViewModel.ErrorDetail = (ex == null ? "" : ex.ToString());
            Window win = UIControlsHelper.GetCurrentActiveWindow();
            if (win != null)
                dialog.Owner = win;

            dialog.ShowDialog();
        }

        public void btnShowDetail_Click(object sender, RoutedEventArgs e)
        {
            // btnShowDetail.Visibility = Visibility.Collapsed;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}