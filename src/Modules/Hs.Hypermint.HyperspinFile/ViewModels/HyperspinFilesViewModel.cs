using Hs.Hypermint.HyperspinFile.Models;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace Hs.Hypermint.HyperspinFile.ViewModels
{
    public class HyperspinFilesViewModel : ViewModelBase
    {
        
        private IEventAggregator _eventAggregator;
        public DelegateCommand RunAuditCommand { get; private set; }

        private string columnHeader = "";
        private string groupBoxHeader = "Unused Files: ";
        public string GroupBoxHeader
        {
            get { return groupBoxHeader; }
            set { SetProperty(ref groupBoxHeader, value); }
        }

        private ICollectionView unusedMediaFiles;
        public ICollectionView UnusedMediaFiles
        {
            get { return unusedMediaFiles; }
            set { SetProperty(ref unusedMediaFiles, value); }
        }

        private List<UnusedMediaFile> UnusedWheels;
        private List<UnusedMediaFile> UnusedArtwork1;
        private List<UnusedMediaFile> UnusedArtwork2;
        private List<UnusedMediaFile> UnusedArtwork3;
        private List<UnusedMediaFile> UnusedArtwork4;
        private List<UnusedMediaFile> UnusedThemes;
        private List<UnusedMediaFile> UnusedVideos;

        private string mediaTypeName = "Controls";
        private ISettingsRepo _settingsRepo;
        private IAuditer _auditRepo;
        private ISelectedService _selectedService;

        public string MediaTypeName
        {
            get { return mediaTypeName; }
            set { SetProperty(ref mediaTypeName, value); }
        }

        public HyperspinFilesViewModel(IEventAggregator ea, ISettingsRepo settings, IAuditer auditRepo, 
            ISelectedService selectedService)
        {
            _eventAggregator = ea;
            _settingsRepo = settings;
            _auditRepo = auditRepo;
            _selectedService = selectedService;            

            // Run the auditer for hyperspin
            RunAuditCommand = new DelegateCommand(() =>
                {
                    _eventAggregator.GetEvent<AuditHyperSpinEvent>().Publish("");
                });

            _eventAggregator.GetEvent<GameSelectedEvent>().Subscribe(SetCurrentName);

            _eventAggregator.GetEvent<AuditHyperSpinEndEvent>().Subscribe(BuildUnusedMediaList);
        }

        private void BuildUnusedMediaList(string obj)
        {
            UnusedWheels = new List<UnusedMediaFile>();
            BuildList(ref UnusedWheels, Images.Wheels);

            UnusedArtwork1 = new List<UnusedMediaFile>();
            BuildList(ref UnusedArtwork1, Images.Artwork1);

            UnusedArtwork2 = new List<UnusedMediaFile>();
            BuildList(ref UnusedArtwork2, Images.Artwork2);

            UnusedArtwork3 = new List<UnusedMediaFile>();
            BuildList(ref UnusedArtwork3, Images.Artwork3);

            UnusedArtwork4 = new List<UnusedMediaFile>();
            BuildList(ref UnusedArtwork4, Images.Artwork4);

            UnusedThemes = new List<UnusedMediaFile>();
            BuildList(ref UnusedThemes, Root.Themes);

            UnusedVideos = new List<UnusedMediaFile>();
            BuildList(ref UnusedVideos, Root.Video);

        }

        private void BuildList(ref List<UnusedMediaFile> mediaList, string mediaPath)
        {
            var pathToScan = Path.Combine(
                _settingsRepo.HypermintSettings.HsPath,
                Root.Media, _selectedService.CurrentSystem,
                mediaPath);

            var files = Directory.GetFiles(pathToScan);

            foreach (string file in files)
            {
                var fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                var fileNameExt = Path.GetExtension(file);

                try
                {
                    if (fileNameNoExt.ToLower() != "thumbs" && !file.ToLower().Contains("default.zip"))
                    {
                        var fileMatchedToGame = _auditRepo.AuditsGameList.Where(x => x.RomName == fileNameNoExt).Any();

                        if (!fileMatchedToGame)
                            mediaList.Add(new UnusedMediaFile()
                            {
                                FileName = fileNameNoExt,
                                Extension = fileNameExt
                            });
                    }
                }
                catch (Exception e)
                {

                }

            }
        }

        private void SetCurrentName(string[] romAndColumn)
        {

            if (romAndColumn[1] != columnHeader)
            {
                columnHeader = romAndColumn[1];

                GroupBoxHeader = "Unused Files: " + columnHeader;

                switch (columnHeader)
                {
                    case "Wheel":
                        UnusedMediaFiles = new ListCollectionView(UnusedWheels);
                        break;
                    case "Artwork1":
                        UnusedMediaFiles = new ListCollectionView(UnusedArtwork1);
                        break;
                    case "Artwork2":
                        UnusedMediaFiles = new ListCollectionView(UnusedArtwork2);
                        break;
                    case "Artwork3":
                        UnusedMediaFiles = new ListCollectionView(UnusedArtwork3);
                        break;
                    case "Artwork4":
                        UnusedMediaFiles = new ListCollectionView(UnusedArtwork4);
                        break;
                    case "Theme":
                        UnusedMediaFiles = new ListCollectionView(UnusedThemes);
                        break;
                    case "Videos":
                        UnusedMediaFiles = new ListCollectionView(UnusedVideos);
                        break;
                    default:
                        UnusedMediaFiles = null;
                        break;
                }
                
            }
        }
    }
}
