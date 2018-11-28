﻿using System;
using Hypermint.Base;
using Prism.Commands;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using Prism.Events;
using Hypermint.Base.Events;
using System.Xml.Serialization;
using System.Xml;
using Hypermint.Base.Models;
using Hypermint.Base.Interfaces;

namespace Hs.Hypermint.ImageEdit.ViewModels
{
    public class ImagePresetViewModel : ViewModelBase
    {
        private string presetPath = string.Empty;        

        #region Constructors
        public ImagePresetViewModel(ISettingsHypermint settingsHypermint, IEventAggregator eventAgg)
        {
            _eventAgg = eventAgg;

            presetPath = settingsHypermint.HypermintSettings.PresetPath + @"\image";

            SavePresetCommand = new DelegateCommand(() =>
            {
                _eventAgg.GetEvent<ImagePresetRequestEvent>().Publish("");
            });

            _eventAgg.GetEvent<ImagePresetRequestedEvent>().Subscribe((x) =>
            {
                SavePreset(x);
            });

            GetPresets("");

        }
        #endregion

        #region Properties
        private ICollectionView presets;
        public ICollectionView Presets
        {
            get { return presets; }
            set { SetProperty(ref presets, value); }
        }

        private string presetNameText;
        public string PresetNameText
        {
            get { return presetNameText; }
            set { SetProperty(ref presetNameText, value); }
        }

        public ImageEditPreset CurrentPreset { get; set; }

        private IEventAggregator _eventAgg;
        #endregion

        #region Commands
        public DelegateCommand SavePresetCommand { get; private set; }
        #endregion

        #region Public Methods
        public string[] GetImagePresets()
        {
            if (!Directory.Exists(presetPath)) Directory.CreateDirectory(presetPath);

            var imagePresets = Directory.GetFiles(presetPath, "*.xml");

            if (imagePresets == null) return new string[0];

            for (int i = 0; i < imagePresets.Length; i++)
            {
                imagePresets[i] = Path.GetFileName(imagePresets[i]);
            }

            return imagePresets;
        }
        public ImageEditPreset DeserializePreset(string presetFile)
        {
            if (!File.Exists(presetFile)) return null;

            try
            {
                using (TextReader sr = new StreamReader(presetFile))
                using (var reader = XmlReader.Create(sr))
                {
                    var serializer = new XmlSerializer(typeof(ImageEditPreset));

                    ImageEditPreset setting = (ImageEditPreset)serializer.Deserialize(reader);

                    return setting;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
        #endregion        

        #region Support Methods
        private void GetPresets(string selectedName = "")
        {
            var imgPresets = GetImagePresets();

            Presets = new ListCollectionView(imgPresets);

            if (imgPresets.Length == 0) return;

            Presets.CurrentChanged += Presets_CurrentChanged;

            if (!string.IsNullOrWhiteSpace(selectedName))
                Presets.MoveCurrentTo(selectedName);

            _eventAgg.GetEvent<ImagePresetsUpdatedEvent>().Publish("");

        }
        private void Presets_CurrentChanged(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.GetDirectoryName(presetPath)))
                Directory.CreateDirectory(presetPath);

            PresetNameText = Presets.CurrentItem.ToString().Replace(".xml", "");

            var preset = DeserializePreset(presetPath + PresetNameText + ".xml");

            _eventAgg.GetEvent<ImagePresetUpdateUiEvent>().Publish(preset);

        }
        private void SavePreset(object x)
        {
            var presetFromUi = (ImageEditPreset)x;

            if (string.IsNullOrWhiteSpace(PresetNameText)) return;

            presetFromUi.Name = PresetNameText;

            if (!Directory.Exists(Path.GetDirectoryName(presetPath)))
                Directory.CreateDirectory(presetPath);

            using (TextWriter sw = new StreamWriter(presetPath + presetFromUi.Name + ".xml"))
            using (var writer = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true }))
            {
                var xmlNameSpace = new XmlSerializerNamespaces();
                xmlNameSpace.Add("", "");
                var xmlRootAttr = new XmlRootAttribute("Preset");

                var serializer = new XmlSerializer(typeof(ImageEditPreset), xmlRootAttr);

                serializer.Serialize(writer, presetFromUi, xmlNameSpace);
            }

            GetPresets(PresetNameText + ".xml");
        } 
        #endregion

    }
}
