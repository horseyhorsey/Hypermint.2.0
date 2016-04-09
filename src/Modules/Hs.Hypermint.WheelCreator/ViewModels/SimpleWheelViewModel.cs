using System;
using Hypermint.Base.Base;
using Prism.Commands;
using Hs.Hypermint.WheelCreator.Tools;
using Hypermint.Base.Services;
using Prism.Events;
using Hypermint.Base.Events;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using Xceed.Wpf.Toolkit;
using System.Windows.Media;
using System.Runtime.CompilerServices;

namespace Hs.Hypermint.WheelCreator.ViewModels
{
    public class SimpleWheelViewModel : ViewModelBase
    {
        private ISelectedService _selectedService;
        private IEventAggregator _eventAggregator;
        #region Commands
        public DelegateCommand GeneratePreviewCommand { get; private set; }
        public DelegateCommand SelectFontCommand { get; private set; } 
        #endregion


        #region Properties
        private string previewText;
        public string PreviewText
        {
            get { return previewText; }
            set { SetProperty(ref previewText, value); }
        }

        private string fontName;
        public string FontName
        {
            get { return fontName; }
            set { SetProperty(ref fontName, value); }
        }

        private bool borderOn;
        public bool BorderOn
        {
            get { return borderOn; }
            set { SetProperty(ref borderOn, value); }
        }

        private System.Windows.Media.Color textColor;
        public System.Windows.Media.Color TextColor
        {
            get { return textColor; }
            set { SetProperty(ref textColor, value); }
        }

        private System.Windows.Media.Color textStrokeColor;
        public System.Windows.Media.Color TextStrokeColor
        {
            get { return textStrokeColor; }
            set { SetProperty(ref textStrokeColor, value); }
        }

        private System.Windows.Media.Color borderColor;
        public System.Windows.Media.Color BorderColor
        {
            get { return borderColor; }
            set { SetProperty(ref borderColor, value); }
        }

        private System.Windows.Media.Color backgroundColor;
        public System.Windows.Media.Color BackgroundColor
        {
            get { return backgroundColor; }
            set { SetProperty(ref backgroundColor, value); }
        }

        private System.Windows.Media.Color bGShadowColor;
        public System.Windows.Media.Color BGShadowColor
        {
            get { return bGShadowColor; }
            set { SetProperty(ref bGShadowColor, value); }
        }

        private bool backgroundShadowOn;
        public bool BackgroundShadowOn
        {
            get { return backgroundShadowOn; }
            set { SetProperty(ref backgroundShadowOn, value); }
        }

        #endregion

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }

        public SimpleWheelViewModel(ISelectedService selectedService, IEventAggregator ea)
        {
            _selectedService = selectedService;
            _eventAggregator = ea;

            GeneratePreviewCommand = new DelegateCommand(GeneratePreview);
            SelectFontCommand = new DelegateCommand(SelectFont);
        }

        private void SelectFont()
        {
            FontDialog fontDlg = new FontDialog();
            System.Windows.Forms.DialogResult result = fontDlg.ShowDialog();
            string font;
            if (result != System.Windows.Forms.DialogResult.Cancel)
            {
                font = fontDlg.Font.Name;
                StringBuilder fontEdit = new StringBuilder(fontDlg.Font.Name);
                fontEdit.Replace(" ", "-");
                font = fontEdit.ToString();
                FontName = font.ToString();
            }
        }

        private System.Drawing.Color ColorFromMediaColor(System.Windows.Media.Color clr)
        {
            return System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
        }

        private void GeneratePreview()
        {
            var textfillcolor = ColorFromMediaColor(TextColor);
            var bordercolor = ColorFromMediaColor(BorderColor);
            var backgroundColor = ColorFromMediaColor(BackgroundColor);
            var textStroke = ColorFromMediaColor(TextStrokeColor);
            var shadowColor = ColorFromMediaColor(BGShadowColor);

            try
            {
                if (PreviewText == "") return;

               WheelGen.GenerateLogo(
                   textfillcolor,
                   bordercolor,
                   strokeColor: textStroke, 
                   backgroundColor: backgroundColor,
                   shadowColor: shadowColor,
                   BackgroundShadow:shadowColor,
                   BackgroundShadowOn: BackgroundShadowOn,
                   borderSize:4,
                   border:true, 
                   textDesc:PreviewText,
                   fontName: FontName);

                var imagePath = "preview.png";

                _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish(imagePath);
            }
            catch (Exception ex)
            {

                
            }


        }
    }
}
