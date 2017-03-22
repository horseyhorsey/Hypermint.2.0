using Hypermint.Base.Base;
using Prism.Commands;
using Prism.Events;
using System.Text;
using System.Windows.Forms;
using System;
using System.IO;
using Hs.Hypermint.WheelCreator.Services;
using Hs.Hypermint.WheelCreator.Repo;
using Hypermint.Base.Events;
using ImageMagick;
using System.Xml.Serialization;
using System.Xml;
using System.ComponentModel;
using System.Windows.Data;
using Hs.Hypermint.WheelCreator.Models;
using Hypermint.Base;
using System.Drawing;
using Hypermint.Base.Services;
using Hypermint.Base.Interfaces;

namespace Hs.Hypermint.WheelCreator.ViewModels
{
    [Serializable]
    public class TextWheelViewModel : ViewModelBase
    {
        #region Commands                
        [XmlIgnore]
        public DelegateCommand SelectFontLocalCommand { get; set; }
        [XmlIgnore]
        public DelegateCommand SelectFontCommand { get; private set; }
        [XmlIgnore]
        public DelegateCommand SavePresetCommand { get; private set; }
        [XmlIgnore]
        public DelegateCommand GeneratePreviewCommand { get; private set; }
        #endregion



        #region Properties

        private WheelTextSetting currentWheelSetting;
        public WheelTextSetting CurrentWheelSetting
        {
            get { return currentWheelSetting; }
            set { SetProperty(ref currentWheelSetting, value); }
        }

        private string fontName;
        public string FontName
        {
            get { return fontName; }
            set { SetProperty(ref fontName, value); }
        }

        private ICollectionView presets;
        [XmlIgnore]
        public ICollectionView Presets
        {
            get { return presets; }
            set { SetProperty(ref presets, value); }
        }

        private string[] gravityOptions;
        [XmlIgnore]
        public string[] GravityOptions
        {
            get { return gravityOptions; }
            set { SetProperty(ref gravityOptions, value); }
        }

        #endregion

        private IEventAggregator _eventAgg;
        private ISettingsRepo _settingsRepo;       
        private IFileFolderService _fileFolderService;

        public TextWheelViewModel(IEventAggregator eventAggregator, ISettingsRepo settings, IFileFolderService findDir)
        {
            _eventAgg = eventAggregator;
            _settingsRepo = settings;
            _fileFolderService = findDir;

            GravityOptions = Enum.GetNames(typeof(Gravity));

            SelectFontCommand = new DelegateCommand(SelectFont);

            SavePresetCommand = new DelegateCommand(SavePreset);

            GeneratePreviewCommand = new DelegateCommand(GeneratePreview);

            SelectFontLocalCommand = new DelegateCommand(() =>
            {
                var fontDir = Path.Combine(_settingsRepo.HypermintSettings.RlMediaPath, "Fonts");

                string result = "";

                if (Directory.Exists(fontDir))
                    result =_fileFolderService.SetFileDialog(fontDir);
                else
                    result =_fileFolderService.SetFileDialog();

                CurrentWheelSetting.FontName = result;
                FontName = result;
            });

            GetPresets();

        }

        public TextWheelViewModel()
        {

        }

        #region Methods
        private async void GeneratePreview()
        {
            ITextImageService srv = new TextImage();

            Gravity grav = (Gravity)Enum.Parse(typeof(Gravity), CurrentWheelSetting.Gravity);

            using (var image = await srv.GenerateCaptionAsync(CurrentWheelSetting))
            {
                image.Write("preview.png");

                var imagePath = "preview.png";

                _eventAgg.GetEvent<GenerateWheelEvent>().Publish(imagePath);
            }


        //using (MagickImage img = new MagickImage())
        //{
        //    img.Settings.Font = @"I:\RocketLauncher\Media\Fonts\amstrad_cpc464.ttf";
        //    img.Settings.FontStyle = FontStyleType.Italic;
        //    img.Settings.FontWeight = FontWeight.Bold;
        //    img.Settings.FillColor = new MagickColor("purple");
        //    img.Settings.TextGravity = Gravity.Center;
        //    img.Read("label:Magick.NET \nis chined", 400, 175);
        //    img.Trim();
        //    var imagePath = "preview.png";
        //    img.Write(imagePath);
        //    _eventAgg.GetEvent<GenerateWheelEvent>().Publish(imagePath);
        //}

        }

    private void GetPresets(string selectedName = "")
        {
            ITextImageService srv = new TextImage();

            string[] textPresets = srv.GetTextPresets();

            Presets = new ListCollectionView(textPresets);

            if (!string.IsNullOrWhiteSpace(selectedName))
                Presets.MoveCurrentTo((string)selectedName);

            _eventAgg.GetEvent<PresetWheelsUpdatedEvent>().Publish("");

            Presets.CurrentChanged += Presets_CurrentChanged;

        }

        private void Presets_CurrentChanged(object sender, EventArgs e)
        {
            var settingsPath = "preset\\wheel\\text\\";

            if (!Directory.Exists(Path.GetDirectoryName(settingsPath)))
                Directory.CreateDirectory(settingsPath);

            var name = Presets.CurrentItem?.ToString();

            if (name != null)
            {
                ITextImageService ts = new TextImage();
                CurrentWheelSetting = ts.DeserializePreset(settingsPath + name);

                SetUIValuesFromPreset(CurrentWheelSetting);
            }
        }

        private void SetUIValuesFromPreset(WheelTextSetting setting)
        {
            CurrentWheelSetting = setting;
            FontName = CurrentWheelSetting.FontName;
        }

        private void SavePreset()
        {
            if (string.IsNullOrWhiteSpace(CurrentWheelSetting.Name)) return;

            var settingsPath = "preset\\wheel\\text\\";

            if (!Directory.Exists(Path.GetDirectoryName(settingsPath)))
                Directory.CreateDirectory(settingsPath);

            using (TextWriter sw = new StreamWriter(settingsPath + currentWheelSetting.Name + ".xml"))
            using (var writer = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true }))
            {
                var xmlNameSpace = new XmlSerializerNamespaces();
                xmlNameSpace.Add("", "");
                var xmlRootAttr = new XmlRootAttribute("Preset");

                var serializer = new XmlSerializer(typeof(WheelTextSetting), xmlRootAttr);

                serializer.Serialize(writer, CurrentWheelSetting, xmlNameSpace);
            }

            GetPresets(currentWheelSetting.Name + ".xml");

        }

        /// <summary>
        /// Select font via windows font dialog
        /// </summary>
        private void SelectFont()
        {
            if (CurrentWheelSetting == null) return;
            
            FontDialog fontDlg = new FontDialog();
            DialogResult result = fontDlg.ShowDialog();
            string font;
            if (result != DialogResult.Cancel)
            {
                font = fontDlg.Font.Name;
                var fontEdit = new StringBuilder(fontDlg.Font.Name);
                font = fontEdit.ToString();

                CurrentWheelSetting.FontName = font;
                FontName = font;
            }

        }

        private void SelectFontLocal()
        {
            
        }
        #endregion



    }
}
