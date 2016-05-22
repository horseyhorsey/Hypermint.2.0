using System;
using Hs.Hypermint.WheelCreator.Repo;
using Hs.Hypermint.WheelCreator.Services;
using Hypermint.Base.Base;
using Prism.Commands;
using ImageMagick;
using Hs.Hypermint.WheelCreator.Models;
using Prism.Events;
using Hypermint.Base.Events;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Windows.Data;
using Hypermint.Base.Interfaces;
using System.Collections.Generic;
using Hypermint.Base.Services;
using System.Threading.Tasks;
using System.Threading;

namespace Hs.Hypermint.WheelCreator.ViewModels
{
    public class WheelProcessViewModel : ViewModelBase
    {
        private ICollectionView presets;
        [XmlIgnore]
        public ICollectionView Presets
        {
            get { return presets; }
            set { SetProperty(ref presets, value); }
        }

        public DelegateCommand ProcessWheelsCommand { get; private set; }
        public DelegateCommand GeneratePreviewCommand { get; private set; }
        public DelegateCommand ProcessCancelCommand { get; private set; } 
        private IEventAggregator _eventAgg;
        private IGameRepo _gameRepo;
        private ISelectedService _selectedService;

        private bool processCancel = false;
        public bool ProcessCancel
        {
            get { return processCancel; }
            set { SetProperty(ref processCancel, value); }
        }

        private bool processRunning;
        public bool ProcessRunning
        {
            get { return !processRunning; }
            set { SetProperty(ref processRunning, value); }
        }

        private bool cancellable = false;
        public bool Cancellable
        {
            get { return cancellable; }
            set { SetProperty(ref cancellable, value); }
        }

        private WheelTextSetting processWheelSetting;

        public WheelTextSetting ProcessWheelSetting
        {
            get { return processWheelSetting; }
            set { SetProperty(ref processWheelSetting, value); }
        }

        public WheelProcessViewModel(IEventAggregator ea, IGameRepo gameRepo,
            ISelectedService selectedService)
        {
            _eventAgg = ea;
            _gameRepo = gameRepo;
            _selectedService = selectedService;

            ITextImageService srv = new TextImage();

            Presets = new ListCollectionView(srv.GetTextPresets());

            Presets.CurrentChanged += Presets_CurrentChanged;

            ProcessWheelsCommand = new DelegateCommand(async () =>
            {
                Cancellable = true;

                await ProcessWheelsAsync();

            });

            ProcessCancelCommand = new DelegateCommand(() =>
            {
                Cancellable = false;
                ProcessCancel = true;                
            });

            GeneratePreviewCommand = new DelegateCommand(GeneratePreview);
        }

        private void Presets_CurrentChanged(object sender, EventArgs e)
        {
            var settingsPath = "preset\\wheel\\text\\";

            if (!Directory.Exists(Path.GetDirectoryName(settingsPath))) return;

            var name = Presets.CurrentItem.ToString();

            ITextImageService ts = new TextImage();
            ProcessWheelSetting = ts.DeserializePreset(settingsPath + name);

        }

        private async void GeneratePreview()
        {
            ITextImageService srv = new TextImage();

            using (var image = await srv.GenerateCaptionAsync(ProcessWheelSetting))
            {
                image.Write("preview.png");

                var imagePath = "preview.png";

                _eventAgg.GetEvent<PreviewGeneratedEvent>().Publish(imagePath);
            }
        }

        private async Task ProcessWheelsAsync()
        {
            ProcessRunning = true;

            var WheelNamesList = new List<WheelNames>();
            foreach (var game in _gameRepo.GamesList)
            {
                WheelNamesList.Add(new WheelNames
                {
                    RomName = game.RomName,
                    Description = game.Description
                });
            }

            ITextImageService srv = new TextImage();
            var exportPath = "Exports\\Wheels\\" + _selectedService.CurrentSystem + "\\";

            if (!Directory.Exists(exportPath))
                Directory.CreateDirectory(exportPath);

            foreach (var wheel in WheelNamesList)
            {
                if (!ProcessCancel)
                {
                    var exportName = wheel.RomName + ".png";

                    ProcessWheelSetting.PreviewText = wheel.Description;

                    using (var image = await srv.GenerateCaptionAsync(ProcessWheelSetting))
                    {
                        var imagePath = Path.Combine(exportPath, exportName);

                        image.Write(imagePath);

                        _eventAgg.GetEvent<PreviewGeneratedEvent>().Publish(imagePath);
                    }
                }
                else
                {
                    Cancellable = false;
                    ProcessRunning = false;
                    ProcessCancel = false;
                    break;
                }
                
            }
        }
    }

    public class WheelNames
    {
        public string RomName { get; set; }
        public string Description { get; set; }
    }
}
