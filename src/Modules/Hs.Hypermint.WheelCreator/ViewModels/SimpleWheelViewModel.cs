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
using System.Collections.Generic;
using System.Drawing.Text;
using Hypermint.Base;
using System.Windows.Media.Imaging;
using System.IO;

namespace Hs.Hypermint.WheelCreator.ViewModels
{
    public class SimpleWheelViewModel : ViewModelBase
    {
        public SimpleWheelViewModel(ISelectedService selectedService, IEventAggregator ea)
        {
            _selectedService = selectedService;
            _eventAggregator = ea;

            Patterns = Enum.GetNames(typeof(WheelCreator.Patterns));

            _eventAggregator.GetEvent<GenerateWheelEvent>().Subscribe(x =>
            {
                //GenerateWheelSource = new BitmapImage(new Uri(Path.GetFullPath(x)));
                GenerateWheelSource = SelectedService.SetBitmapFromUri(new Uri(Path.GetFullPath(x)));
            });
        }

        private ISelectedService _selectedService;
        private IEventAggregator _eventAggregator;

        #region Commands        
        public DelegateCommand SelectFontCommand { get; private set; } 
        #endregion

        #region Properties
        private bool borderOn;
        public bool BorderOn
        {
            get { return borderOn; }
            set { SetProperty(ref borderOn, value); }
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

        private string[] patterns;
        public string[] Patterns
        {
            get { return patterns; }
            set { SetProperty(ref patterns, value); }
        }

        private double fontPointSize = 36;
        public double FontPointSize
        {
            get { return fontPointSize; }
            set { SetProperty(ref fontPointSize, value); }
        }

        private string selectedPattern;
        public string SelectedPattern
        {
            get { return selectedPattern; }
            set { SetProperty(ref selectedPattern, value); }
        }

        private ImageSource generateWheelSource;
        public ImageSource GenerateWheelSource
        {
            get { return generateWheelSource; }
            set { SetProperty(ref generateWheelSource, value); }
        }

        #endregion

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }

        private System.Drawing.Color ColorFromMediaColor(System.Windows.Media.Color clr)
        {
            return System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
        }

        private void GeneratePreview()
        {

            //var textfillcolor = ColorFromMediaColor(TextColor);
            var bordercolor = ColorFromMediaColor(BorderColor);
            var backgroundColor = ColorFromMediaColor(BackgroundColor);
            //var textStroke = ColorFromMediaColor(TextStrokeColor);
            var shadowColor = ColorFromMediaColor(BGShadowColor);

            try
            {
                //if (PreviewText == "") return;

                /*
                WheelGen.GenerateLogo(
                    textfillcolor,
                    bordercolor,
                    strokeColor: textStroke,
                    backgroundColor: backgroundColor,
                    shadowColor: shadowColor,
                    BackgroundShadow: shadowColor,
                    BackgroundShadowOn: BackgroundShadowOn,
                    borderSize: 4,
                    border: true,
                    textDesc: PreviewText,
                    fontName: FontName);
                */

                /*
                WheelGen.AnnotateWheel("preview.png", PreviewText, FontName,
                    FontPointSize, SelectedPattern, textfillcolor,
                    textStroke, StrokeWidth, shadowColor, WheelWidth, wheelHeight);
                */

                var imagePath = "preview.png";

                _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish(imagePath);
            }
            catch (Exception ex)
            {


            }


        }

        private List<System.Drawing.FontFamily> BuildFonts()
        {
            var fontFamilies = new List<System.Drawing.FontFamily>();

            InstalledFontCollection installedFontCollection = new InstalledFontCollection();

            // Get the array of FontFamily objects.
            foreach (var item in installedFontCollection.Families)
            {
                fontFamilies.Add(item);
            }

            return fontFamilies;
        }
        
    }
}
