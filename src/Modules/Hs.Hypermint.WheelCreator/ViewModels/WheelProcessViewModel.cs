using System;
using Hs.Hypermint.WheelCreator.Repo;
using Hs.Hypermint.WheelCreator.Services;
using Hypermint.Base.Base;
using Prism.Commands;
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

        private bool previewCreated = true;
        public bool PreviewCreated
        {
            get { return previewCreated; }
            set { SetProperty(ref previewCreated, value); }
        }

        private string processWheelInfo;
        public string ProcessWheelInfo
        {
            get { return processWheelInfo; }
            set { SetProperty(ref processWheelInfo, value); }
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

            var gameCount = WheelNamesList.Count;

            ProcessWheelInfo = "Processing wheels" + gameCount;

            ITextImageService srv = new TextImage();
            var exportPath = "Exports\\Wheels\\" + _selectedService.CurrentSystem + "\\";

            if (!Directory.Exists(exportPath))
                Directory.CreateDirectory(exportPath);

            int i = 1;
            foreach (var wheel in WheelNamesList)
            {
                if (!ProcessCancel)
                {
                    ProcessWheelInfo = "Processing wheels : " + i + " of " + gameCount;

                    string exportName = wheel.RomName + ".png";

                    if(_selectedService.CurrentSystem.ToLower().Contains("main menu"))
                        ProcessWheelSetting.PreviewText = wheel.RomName;
                    else
                        ProcessWheelSetting.PreviewText = wheel.Description;

                    using (var image = await srv.GenerateCaptionAsync(ProcessWheelSetting))
                    {
                        var imagePath = Path.Combine(exportPath, exportName);

                        image.Write(imagePath);

                        if(PreviewCreated)
                            _eventAgg.GetEvent<PreviewGeneratedEvent>().Publish(imagePath);
                    }                    

                    i++;
                    
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
