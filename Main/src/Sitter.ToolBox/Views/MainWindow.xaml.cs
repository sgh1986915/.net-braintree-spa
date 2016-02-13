using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Sitter.ToolBox.Controls;
using Sitter.Toolbox.Utility;

namespace Sitter.ToolBox.Views
{
    public partial class MainWindow : Window
    {

        private Dictionary<Button, object> _navButtonRegistry;
        private MainWindowViewModel MyViewModel { get; set; }
        private readonly SmartFeedback _feedback;
        private Button _currentButton;

        public MainWindow()
        {
            MainWindowInstance = this;

            InitializeComponent();
            RegisterNavButtons();

            MyViewModel = new MainWindowViewModel();
            this.DataContext = MyViewModel;
            
            _feedback = new SmartFeedback(flowParagraph, 17, Colors.Blue).SetDefaultFontSize(11).SetDefaultColor(Colors.Black).SetDefaultFontFamily("Consolas");
        }

        private void RegisterNavButtons()
        {
            // New buttons added to UI should be registered with their associated user control type.
            // This registration is used during dirty flagging/clearing to enable/disable nav.

            _navButtonRegistry = new Dictionary<Button, object>();
            _navButtonRegistry.Add(btnExampleData, typeof(FakeMongoData));
            //_navButtonRegistry.Add(btnDbBackup, typeof(DbBackup));

            foreach (Control c in leftNavPanel.Children)
            {
                Button b = c as Button;

                if (b != null)
                {
                    b.Click += btnLeftNav_Click;
                }
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Select first button
            const int lastIndexSelected = 0; //TODO: store this in user settings.
            Button b1 = leftNavPanel.Children[lastIndexSelected] as Button;
            if (b1 != null)
            {
                LoadControl(b1);
            }
        }
        
        private void btnLeftNav_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadControl((Button) sender);
            }
            catch (Exception ex)
            {
                UIErrorHandler.DisplayError(ex);
            }
        }

        private void LoadControl(Button actionButton)
        {
            if (_currentButton != null)
                _currentButton.FontWeight = FontWeights.Normal;
            _currentButton = actionButton;
            _currentButton.FontWeight = FontWeights.Bold;

            SetDefaultAlignments();

            actionButton.Focus();

            object uc = _navButtonRegistry[actionButton];

            // Convert from a Type to an instance of the type
            if (!(uc is UserControl))
            {
                uc = (UserControl) Activator.CreateInstance((Type) uc);
                _navButtonRegistry[actionButton] = uc;
            }

            contentMain.Content = uc;

            var configUserControl = ((ActionUserControl) uc);
            MyViewModel.ConfigTitle = configUserControl.ConfigTitle;

            DoApplyChanges(false);
        }

        private void SetDefaultAlignments()
        {
            contentMain.VerticalContentAlignment = VerticalAlignment.Stretch;
            contentMain.VerticalAlignment = VerticalAlignment.Stretch;
            contentMain.HorizontalAlignment = HorizontalAlignment.Stretch;
            contentMain.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        }

        public void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            DoApplyChanges(false);
        }

        public void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            DoApplyChanges(true);
        }

        private void DoApplyChanges(bool execute)
        {
            progressBar.Visibility = Visibility.Visible;
            IsCancelVisible = Visibility.Visible;
            pnlActions.IsEnabled = false;
            ActionUserControl uc = contentMain.Content as ActionUserControl;
           
            _feedback.Clear();
            if (uc != null)
            {
                bool hideDoneMessage = uc.HideDoneMessage;
                Task t = new Task(() =>
                {
                    try
                    {
                        _feedback.DeactivateAutoScroll = uc.DeactivateAutoScroll;
                        
                        uc.ApplyChanges(_feedback, execute);
                    }
                    catch (ActionCancelException)
                    {
                        _feedback.Text("User cancelled", null, _feedback.ErrorColor).LineBreak();
                    }
                    catch (Exception ex)
                    {
                        _feedback.Text(ex.ToString(), null, _feedback.ErrorColor);
                    }
                    finally
                    {
                        ActionComplete(execute, hideDoneMessage);
                        uc.IsUcLoaded = true;
                        _feedback.DeactivateAutoScroll = uc.DeactivateAutoScroll;
                    }
                    
                });
                t.Start();
            }
        }

        public void ActionComplete(bool execute, bool hideDoneMessage)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action<bool, bool>(ActionComplete), execute, hideDoneMessage);
                return;
            }

            string msg = (execute ? "Apply" : "Validate") + " DONE";
            if (!hideDoneMessage)
                _feedback.LineBreak().Text(msg, null, null, true);
            progressBar.Visibility = Visibility.Hidden;
            pnlActions.IsEnabled = true;
            CancelRequested = false;
            IsCancelVisible = Visibility.Collapsed;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ActionUserControl uc = contentMain.Content as ActionUserControl;
            IsCancelVisible = Visibility.Collapsed;
            CancelRequested = true;
        }

        #region Statics

        public static bool CancelRequested { get; set; }
        public static MainWindow MainWindowInstance { get; set; }

        #endregion

        public Visibility IsCancelVisible
        {
            get { return btnCancel.Visibility; }
            set
            {
                btnCancel.Visibility = value;
            }
        }
    }
}
