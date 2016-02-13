using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Sitter.Toolbox.Utility
{

    public class SmartFeedback
    {
        public SmartFeedback(Paragraph paragraph, double? headerFontSize = null, Color? headerColor = null, Color? disabledColor = null, Color? subHeaderColor = null)
        {
            _paragraph = paragraph;
            _defaultFontSize = 12d;
            _defaultColor = Colors.Black;
            _defaultFontFamily = new FontFamily();
            _headerFontSize = headerFontSize.HasValue ? headerFontSize.Value : 20;
            _headerColor = headerColor.HasValue ? headerColor.Value : Colors.Black;
            _subHeaderColor = subHeaderColor.HasValue ? subHeaderColor.Value : Colors.Green;
            _disabledColor = disabledColor.HasValue ? disabledColor.Value : Colors.Gray;
            _dataColor = Colors.Blue;
            _actionTakenColor = Colors.Green;
            _errorFontSize = 12;
            _errorColor = Colors.Red;
            DeactivateAutoScroll = false;
        }

        #region Fields

        private readonly Paragraph _paragraph;
        private double _defaultFontSize;
        private Color _defaultColor;
        private FontFamily _defaultFontFamily;
        private readonly double _headerFontSize;
        private readonly Color _headerColor;
        private readonly Color _subHeaderColor;
        private readonly Color _disabledColor;
        private readonly Color _dataColor;
        private readonly Color _actionTakenColor;
        private readonly double _errorFontSize;
        private readonly Color _errorColor;

        #endregion // Fields

        #region Properties

        public bool DeactivateAutoScroll { get; set; }

        public Paragraph Paragraph
        {
            get { return _paragraph; }
        }

        public double HeaderFontSize
        {
            get { return _headerFontSize; }
        }

        public Color HeaderColor
        {
            get { return _headerColor; }
        }

        public Color SubHeaderColor
        {
            get { return _subHeaderColor; }
        }

        public Color DisabledColor
        {
            get { return _disabledColor; }
        }

        public Color DataColor
        {
            get { return _dataColor; }
        }

        public Color ActionTakenColor
        {
            get { return _actionTakenColor; }
        }

        public Color ErrorColor
        {
            get { return _errorColor; }
        }

        public double ErrorFontSize
        {
            get { return _errorFontSize; }
        }

        #endregion // Properties

        #region Methods (Fluid Interface)

        public SmartFeedback SetDefaultFontSize(double fontSize)
        {
            _defaultFontSize = fontSize;
            return this;
        }

        public SmartFeedback SetDefaultColor(Color defaultColor)
        {
            _defaultColor = defaultColor;
            return this;
        }

        public SmartFeedback SetDefaultFontFamily(string fontFamily)
        {
            _defaultFontFamily = new FontFamily(fontFamily);
            return this;
        }

        public SmartFeedback Image(double width, double height, string imageName, string imagePackUrl)
        {
            // Marshal to UI Thread
            if (!_paragraph.Dispatcher.CheckAccess())
            {
                _paragraph.Dispatcher.Invoke(new Func<double, double, string, string, SmartFeedback>(Image), width, height, imageName, imagePackUrl);
                return this;
            }
            // example imagePackUrl: "pack://application:,,,/ReportingPluginDeploy;component/Assets/"
            string source = string.Format("{0}{1}", imagePackUrl, imageName);
            Image image = new Image();
            image.Width = width;
            image.Height = height;
            image.HorizontalAlignment = HorizontalAlignment.Center;
            image.VerticalAlignment = VerticalAlignment.Center;
            image.Source = new BitmapImage(new Uri(source));
            InlineUIContainer container = new InlineUIContainer(image);
            container.BaselineAlignment = BaselineAlignment.Center;
            _paragraph.Inlines.Add(container);
            return this;
        }

        public SmartFeedback Text(string msg, double? fontSize = null, Color? color = null, bool isBold = false, string updateKey = null)
        {
            // Marshal to UI Thread
            if (!_paragraph.Dispatcher.CheckAccess())
            {
                _paragraph.Dispatcher.Invoke(new Func<string, double?, Color?, bool, string, SmartFeedback>(Text), msg, fontSize, color, isBold, updateKey);
                return this;
            }

            Span s = new Span(new Run(msg) { Tag = updateKey });
            s.Foreground = new SolidColorBrush(color == null ? _defaultColor : color.Value);
            s.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
            s.FontSize = fontSize.HasValue ? fontSize.Value : _defaultFontSize;
            s.FontFamily = _defaultFontFamily;
            _paragraph.Inlines.Add(s);

            return this;
        }

        public SmartFeedback LineBreak()
        {
            // Marshal to UI Thread
            if (!_paragraph.Dispatcher.CheckAccess())
            {
                _paragraph.Dispatcher.Invoke(new Func<SmartFeedback>(LineBreak));
                return this;
            }

            ScrollToEnd(); //test

            _paragraph.Inlines.Add(new LineBreak());
            return this;
        }

        public SmartFeedback AddCheck(bool? doCheck = null, double? fontSize = null)
        {
            // Marshal to UI Thread
            if (!_paragraph.Dispatcher.CheckAccess())
            {
                _paragraph.Dispatcher.Invoke(new Func<bool?, double?, SmartFeedback>(AddCheck), doCheck, fontSize);
                return this;
            }

            if (doCheck.HasValue && doCheck.Value == false)
                return this;
            Span s = new Span(new Run(""));
            s.FontSize = fontSize.HasValue ? fontSize.Value : _defaultFontSize;
            s.FontFamily = new FontFamily("Webdings");
            s.Foreground = new SolidColorBrush(Colors.Green);
            _paragraph.Inlines.Add(s);
            return this;
        }

        public SmartFeedback AddX(bool doX = true, double? fontSize = null)
        {
            // Marshal to UI Thread
            if (!_paragraph.Dispatcher.CheckAccess())
            {
                _paragraph.Dispatcher.Invoke(new Func<bool, double?, SmartFeedback>(AddX), doX, fontSize);
                return this;
            }

            Span s = new Span(new Run(""));
            s.FontSize = fontSize.HasValue ? fontSize.Value : _defaultFontSize;
            s.FontFamily = new FontFamily("Webdings");
            s.Foreground = new SolidColorBrush(Colors.Red);
            _paragraph.Inlines.Add(s);
            return this;
        }

        public Run UpdateText(string key, string updatedText)
        {
            // Marshal to UI Thread
            if (!_paragraph.Dispatcher.CheckAccess())
            {
                _paragraph.Dispatcher.Invoke(new Func<string, string, Run>(UpdateText), key, updatedText);
                return null;
            }

            if (key == null)
                return null;

            var result = UIControlsHelper.FindChildren<Run>(_paragraph, m => (m.Tag == null ? null : m.Tag.ToString()) == key).FirstOrDefault();
            if (result != null)
            {
                result.Text = updatedText;
            }

            return result;
        }

        public SmartFeedback Hyperlink(string text, string url, int spacesBefore = 2, RequestNavigateEventHandler navHandler = null)
        {
            // Marshal to UI Thread
            if (!_paragraph.Dispatcher.CheckAccess())
            {
                _paragraph.Dispatcher.Invoke(new Func<string, string, int, RequestNavigateEventHandler, SmartFeedback>(Hyperlink), text, url, spacesBefore, navHandler);
                return this;
            }

            Hyperlink h = new Hyperlink();
            try
            {
                h.NavigateUri = new Uri(url, UriKind.RelativeOrAbsolute);
            }
            catch (UriFormatException e)
            {
                Text("bad uri:" + e.Message, ErrorFontSize, ErrorColor);
                return this;
            }

            h.Inlines.Add(new Run(text));
            h.RequestNavigate += navHandler ?? Hyperlink_RequestNavigate;

            if (spacesBefore > 0)
                _paragraph.Inlines.Add(new Run(new string(' ', spacesBefore)));
            _paragraph.Inlines.Add(h);
            return this;
        }

        public static string FormatFileLink(string fullFileName)
        {
            return "file://" + fullFileName;
        }

        public SmartFeedback Clear()
        {
            // Marshal to UI Thread
            if (!_paragraph.Dispatcher.CheckAccess())
            {
                _paragraph.Dispatcher.Invoke(new Func<SmartFeedback>(Clear));
                return this;
            }

            _paragraph.Inlines.Clear();
            return this;
        }

        /// <summary>
        /// Note, this method should be called from the UI thread. An exception may be thrown otherwise.
        /// </summary>
        /// <returns>Plain text content of the RichTextBox</returns>
        public string GetContentAsText()
        {
            // Marshal to UI Thread
            if (!_paragraph.Dispatcher.CheckAccess())
            {
                object result = _paragraph.Dispatcher.Invoke(new Func<string>(GetContentAsText));
                return (string)result;
            }

            var textRange = new TextRange(_paragraph.ContentStart, _paragraph.ContentEnd);
            return textRange.Text;
        }

        public void ScrollToEnd()
        {
            if (DeactivateAutoScroll)
            {
                return;
            }

            // Marshal to UI Thread
            if (!_paragraph.Dispatcher.CheckAccess())
            {
                _paragraph.Dispatcher.Invoke(ScrollToEnd);
                return;
            }

            var fd = _paragraph.Parent as FlowDocument;
            if (fd != null)
            {
                var richTextBox = fd.Parent as RichTextBox;
                if (richTextBox != null)
                {
                    richTextBox.ScrollToEnd();
                }
            }
        }

        #endregion // Methods

        #region Private Methods

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                if (e.Uri != null)
                {
                    if (e.Uri.Scheme == "file")
                    {
                        string item = System.Uri.UnescapeDataString(e.Uri.AbsolutePath);

                        if (!Directory.Exists(item) && !File.Exists(item))
                        {

                            MessageBox.Show("Directory not found");
                        }
                        else
                        {
                            Process.Start(new ProcessStartInfo(item));
                        }
                    }
                    else if (e.Uri.Scheme == "http" || e.Uri.Scheme == "https")
                    {
                        Process.Start(new ProcessStartInfo(e.Uri.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                string url = (e == null || e.Uri == null) ? null : e.Uri.ToString();
                MessageBox.Show("unable to navigate to URL. '" + url + "' Error:" + ex);
            }

            e.Handled = true;
        }

        #endregion
    }
}

