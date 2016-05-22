using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Commands;
using Prism.Events;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System;
using System.IO;
using System.Collections;
using Hs.Hypermint.WheelCreator.Services;
using Hs.Hypermint.WheelCreator.Repo;
using Hypermint.Base.Events;
using ImageMagick;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Data;
using Hs.Hypermint.WheelCreator.Models;

namespace Hs.Hypermint.WheelCreator.ViewModels
{
    [Serializable]
    public class TextWheelViewModel : ViewModelBase
    {
        [XmlIgnore]
        public DelegateCommand SelectFontCommand { get; private set; }
        [XmlIgnore]
        public DelegateCommand SavePresetCommand { get; private set; }
        [XmlIgnore]
        public DelegateCommand GeneratePreviewCommand { get; private set; }

        #region Properties

        private WheelTextSetting currentWheelSetting;
        public WheelTextSetting CurrentWheelSetting
        {
            get { return currentWheelSetting; }
            set { SetProperty(ref currentWheelSetting, value); }
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

        public TextWheelViewModel(IEventAggregator eventAggregator)
        {
            _eventAgg = eventAggregator;

            GravityOptions = Enum.GetNames(typeof(Gravity));

            SelectFontCommand = new DelegateCommand(SelectFont);

            SavePresetCommand = new DelegateCommand(SavePreset);

            GeneratePreviewCommand = new DelegateCommand(GeneratePreview);

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

                _eventAgg.GetEvent<PreviewGeneratedEvent>().Publish(imagePath);
            }
        }

        private void GetPresets()
        {
            ITextImageService srv = new TextImage();
            string[] textPresets = srv.GetTextPresets();

            Presets = new ListCollectionView(textPresets);

            Presets.CurrentChanged += Presets_CurrentChanged;

        }

        private void Presets_CurrentChanged(object sender, EventArgs e)
        {
            var settingsPath = "preset\\wheel\\text\\";

            if (!Directory.Exists(Path.GetDirectoryName(settingsPath)))
                Directory.CreateDirectory(settingsPath);

            var name = Presets.CurrentItem.ToString();

            ITextImageService ts = new TextImage();
            CurrentWheelSetting = ts.DeserializePreset(settingsPath + name);

            SetUIValuesFromPreset(CurrentWheelSetting);

        }

        private void SetUIValuesFromPreset(WheelTextSetting setting)
        {
            CurrentWheelSetting = setting;
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

            GetPresets();

        }

        private void SelectFont()
        {
            FontDialog fontDlg = new FontDialog();
            DialogResult result = fontDlg.ShowDialog();
            string font;
            if (result != DialogResult.Cancel)
            {
                font = fontDlg.Font.Name;
                var fontEdit = new StringBuilder(fontDlg.Font.Name);
                font = fontEdit.ToString();
                CurrentWheelSetting.FontName = font.ToString();
            }
        }
        #endregion



    }
}
