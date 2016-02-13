using Sitter.Toolbox.Model;

namespace Sitter.ToolBox.Views
{
    public class MainWindowViewModel :ViewModelBase
    {

        public string Title
        {
            get { return "TCS Toolbox"; }
        }

        private string _configTitle = string.Empty;
        public string ConfigTitle
        {
            get { return _configTitle; }
            set
            {
                _configTitle = value;
                OnPropertyChanged(()=> ConfigTitle);
            }
        }
    }
}
