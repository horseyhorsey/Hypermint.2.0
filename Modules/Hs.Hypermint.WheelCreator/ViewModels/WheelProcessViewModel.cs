using System;
using Hs.Hypermint.WheelCreator.Repo;
using Hs.Hypermint.WheelCreator.Services;
using Hypermint.Base;
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
        public DelegateCommand OpenExportFolderCommand { get; private set; }

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

        private bool overwriteImage = false;
        private IFolderExplore _folderExplorer;

        public bool OverwriteImage
        {
            get { return overwriteImage; }
            set { SetProperty(ref overwriteImage, value); }
        }

        private bool processWheels = true;
        public bool ProcessWheels
        {
            get { return processWheels; }
            set { SetProperty(ref processWheels, value); }
        }

        private bool processLetters;
        public bool ProcessLetters
        {
            get { return processLetters; }
            set { SetProperty(ref processLetters, value); }
        }

        private bool processGenres;
        public bool ProcessGenres
        {
            get { return processGenres; }
            set { SetProperty(ref processGenres, value); }
        }        

        public WheelProcessViewModel(IEventAggregator ea,
            IGameRepo gameRepo,
            ISelectedService selectedService,
            IFolderExplore folderExplore)
        {
            _eventAgg = ea;
            _gameRepo = gameRepo;
            _selectedService = selectedService;
            _folderExplorer = folderExplore;

            PresetsUpdated("");

            ProcessWheelsCommand = new DelegateCommand(async () =>
            {
                if (_gameRepo.GamesList != null)
                {
                    Cancellable = true;

                    if (ProcessWheels)
                        await ProcessWheelsAsync();
                    else if (ProcessLetters)
                        await ProcessLettersAsync();
                    else if (ProcessGenres)
                        await ProcessGenresAsync();
                }

            });

            ProcessCancelCommand = new DelegateCommand(() =>
            {
                Cancellable = false;
                ProcessCancel = true;
            });

            GeneratePreviewCommand = new DelegateCommand(GeneratePreview);

            OpenExportFolderCommand = new DelegateCommand(() =>
            {
                var exportPath = "Exports\\Wheels\\" + _selectedService.CurrentSystem + "\\";

                if (ProcessLetters)
                    exportPath = "Exports\\Letters\\" + _selectedService.CurrentSystem + "\\";
                else if (ProcessGenres)
                    exportPath = "Exports\\Genres\\" + _selectedService.CurrentSystem + "\\";

                _folderExplorer.OpenFolder(exportPath);
            });

            _eventAgg.GetEvent<PresetWheelsUpdatedEvent>().Subscribe(x =>
            {
                PresetsUpdated(x);
            });
        }

        private void PresetsUpdated(string x)
        {
            ITextImageService srv = new TextImage();

            Presets = new ListCollectionView(srv.GetTextPresets());

            Presets.CurrentChanged += Presets_CurrentChanged;
        }

        private void Presets_CurrentChanged(object sender, EventArgs e)
        {
            var settingsPath = "preset\\wheel\\text\\";

            if (!Directory.Exists(Path.GetDirectoryName(settingsPath))) return;

            if (Presets?.CurrentItem != null)
            {
                var name = Presets.CurrentItem.ToString();

                ITextImageService ts = new TextImage();
                ProcessWheelSetting = ts.DeserializePreset(settingsPath + name);
            }
        }

        private async void GeneratePreview()
        {
            ITextImageService srv = new TextImage();

            using (var image = await srv.GenerateCaptionAsync(ProcessWheelSetting))
            {
                image.Write("preview.png");

                var imagePath = "preview.png";
                
                _eventAgg.GetEvent<GenerateWheelEvent>().Publish(imagePath);
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
                string exportName = wheel.RomName + ".png";
                var imagePath = Path.Combine(exportPath, exportName);

                if (!ProcessCancel)
                {
                    ProcessWheelInfo = "Processing wheels : " + i + " of " + gameCount;

                    if (!File.Exists(imagePath))
                    {
                        if (_selectedService.CurrentSystem.ToLower().Contains("main menu"))
                            ProcessWheelSetting.PreviewText = wheel.RomName;
                        else
                            ProcessWheelSetting.PreviewText = wheel.Description;

                        using (var image = await srv.GenerateCaptionAsync(ProcessWheelSetting))
                        {
                            image.Write(imagePath);

                            if (PreviewCreated)
                                _eventAgg.GetEvent<GenerateWheelEvent>().Publish(imagePath);
                        }
                    }
                    else
                    {
                        if (OverwriteImage)
                        {
                            if (_selectedService.CurrentSystem.ToLower().Contains("main menu"))
                                ProcessWheelSetting.PreviewText = wheel.RomName;
                            else
                                ProcessWheelSetting.PreviewText = wheel.Description;

                            using (var image = await srv.GenerateCaptionAsync(ProcessWheelSetting))
                            {
                                image.Write(imagePath);

                                if (PreviewCreated)
                                    _eventAgg.GetEvent<GenerateWheelEvent>().Publish(imagePath);
                            }
                        }
                    }

                    i++;
                }

                else
                {
                    ProcessWheelInfo = "Processing wheels cancelled";
                    break;
                }

            }
            
            Cancellable = false;
            ProcessRunning = false;
            ProcessCancel = false;

            ProcessWheelInfo = "Processing wheels complete";
        }

        private async Task ProcessLettersAsync()
        {
            ProcessRunning = true;

            ProcessWheelInfo = "Processing letters";

            string letters = @"!('0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            ITextImageService srv = new TextImage();
            var exportPath = "Exports\\Letters\\" + _selectedService.CurrentSystem + "\\";

            if (!Directory.Exists(exportPath))
                Directory.CreateDirectory(exportPath);

            foreach (var letter in letters)
            {
                if (!ProcessCancel)
                {
                    string exportName = letter + ".png";
                    var imagePath = Path.Combine(exportPath, exportName);

                    ProcessWheelSetting.PreviewText = letter.ToString();

                    using (var image = await srv.GenerateCaptionAsync(ProcessWheelSetting))
                    {
                        image.Write(imagePath);

                        if (PreviewCreated)
                            _eventAgg.GetEvent<GenerateWheelEvent>().Publish(imagePath);
                    }
                }
                else
                {
                    break;
                }
            }

            ProcessWheelInfo = "Processing letters complete";
            Cancellable = false;
            ProcessRunning = false;
            ProcessCancel = false;
        }

        private async Task ProcessGenresAsync()
        {
            ProcessRunning = true;

            ProcessWheelInfo = "Processing genres";

            var genreList = new List<string>();
            foreach (var game in _gameRepo.GamesList)
            {
                if (!string.IsNullOrEmpty(game.Genre))
                    if (!genreList.Contains(game.Genre))
                        genreList.Add(game.Genre);
            }

            var genreCount = genreList.Count;
            if (genreCount == 0)
            {
                ResetProcess();
                return;
            }
            
            var exportPath = "Exports\\Genres\\" + _selectedService.CurrentSystem + "\\";

            if (!Directory.Exists(exportPath))
                Directory.CreateDirectory(exportPath);

            int i = 1;
            foreach (var genre in genreList)
            {
                if (!ProcessCancel)
                {
                    ProcessWheelInfo = "Processing genres : " + i + " of " + genreCount;
                    ProcessWheelSetting.PreviewText = genre.ToString();

                    string exportName = genre + ".png";
                    var imagePath = Path.Combine(exportPath, exportName);             

                    if (!File.Exists(imagePath))                    
                        await SaveImageAsync(imagePath);                    
                    else if (OverwriteImage)
                        await SaveImageAsync(imagePath);

                    i++;
                }
                else
                {
                    ResetProcess();
                    break;
                }
            }

            ProcessWheelInfo = "Processing genres complete";

            ResetProcess();
        }

        private async Task SaveImageAsync(string imagePath)
        {
            ITextImageService srv = new TextImage();

            using (var image = await srv.GenerateCaptionAsync(ProcessWheelSetting))
            {
                image.Write(imagePath);

                if (PreviewCreated)
                    _eventAgg.GetEvent<GenerateWheelEvent>().Publish(imagePath);
            }
        }

        private void ResetProcess()
        {
            Cancellable = false;
            ProcessRunning = false;
            ProcessCancel = false;
        }
    }

    public class WheelNames
    {
        public string RomName { get; set; }
        public string Description { get; set; }
    }
}
