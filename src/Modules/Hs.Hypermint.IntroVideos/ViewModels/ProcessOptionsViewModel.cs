using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Models;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public class ProcessOptionsViewModel : ViewModelBase
    {
        const string exportPath = "exports\\videos\\";

        #region AviSynthProperties
        private bool overlay;
        public bool Overlay
        {
            get { return overlay; }
            set { SetProperty(ref overlay, value); }
        }
        private bool resizeOverlay;
        public bool ResizeOverlay
        {
            get { return resizeOverlay; }
            set { SetProperty(ref resizeOverlay, value); }
        }
        private int startFrame = 60;
        public int StartFrame
        {
            get { return startFrame; }
            set { SetProperty(ref startFrame, value); }
        }
        private int endFrame = 300;
        public int EndFrame
        {
            get { return endFrame; }
            set { SetProperty(ref endFrame, value); }
        }
        private int dissolveAmount = 60;
        public int DissolveAmount
        {
            get { return dissolveAmount; }
            set { SetProperty(ref dissolveAmount, value); }
        }
        private int fadeIn = 60;
        public int FadeIn
        {
            get { return fadeIn; }
            set { SetProperty(ref fadeIn, value); }
        }
        private int fadeOut = 60;
        public int FadeOut
        {
            get { return fadeOut; }
            set { SetProperty(ref fadeOut, value); }
        }
        private int resizeWidth = 200;
        public int ResizeWidth
        {
            get { return resizeWidth; }
            set { SetProperty(ref resizeWidth, value); }
        }
        private int resizeHeight = 100;
        public int ResizeHeight
        {
            get { return resizeHeight; }
            set { SetProperty(ref resizeHeight, value); }
        }
        private int wheelPosX = 3;
        public int WheelPosX
        {
            get { return wheelPosX; }
            set { SetProperty(ref wheelPosX, value); }
        }
        private int wheelPosY = 380;        
        public int WheelPosY
        {
            get { return wheelPosY; }
            set { SetProperty(ref wheelPosY, value); }
        }
        #endregion

        private List<string> _scripts;
        private ICollectionView scripts;
        public ICollectionView Scripts
        {
            get { return scripts; }
            set { SetProperty(ref scripts, value); }
        }

        private int videoQuality = 10;
        public int VideoQuality
        {
            get { return videoQuality; }
            set { SetProperty(ref videoQuality, value); }
        }

        #region Services
        private IAviSynthScripter _avisynthScripter;
        private IFolderExplore _folderExplorer;
        private IEventAggregator _eventAggregator;
        #endregion

        public DelegateCommand SaveScriptCommand { get; private set; }
        public DelegateCommand OpenExportFolderCommand { get; private set; }
        public DelegateCommand ProcessScriptCommand { get; private set; } 

        public ProcessOptionsViewModel(
            IAviSynthScripter aviSynthScripter,
            IEventAggregator ea,
            ISettingsRepo settings,
            ISelectedService selected,
            IFolderExplore folderexplorer)
        {
            _avisynthScripter = aviSynthScripter;
            _folderExplorer = folderexplorer;
            _eventAggregator = ea;
            _settings = settings;
            _selectedService = selected;

            AviSynthOptions = new AviSynthOption();

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(x => SystemChanged(x));

            SaveScriptCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<GetProcessVideosEvent>().Publish("");
            });

            _eventAggregator.GetEvent<ReturnProcessVideosEvent>().Subscribe(x =>
            {
                SaveScript((string[])x);
            });

            ProcessScriptCommand = new DelegateCommand(() =>
            {
                try
                {
                    var ffmpegExe = ConfigurationManager.AppSettings["ffmpeg:ExeLocation"].ToString();

                    var selectedScript = Scripts.CurrentItem as string;

                    if (selectedScript == null) return;

                    var scriptPath = GetSystemExportPath() + selectedScript + ".avs";
                    var videoPath = GetSystemExportPath() + selectedScript + ".mp4";                                        

                    Process.Start(ffmpegExe,
                         "-i " + scriptPath + " -vcodec libx264 -crf " + VideoQuality + " " + videoPath);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }

            });
            

            OpenExportFolderCommand = new DelegateCommand(() =>
            {                
                _folderExplorer.OpenFolder("exports\\videos\\" + _selectedService.CurrentSystem.Replace(' ','_'));                
            });

        }

        private void SystemChanged(string x)
        {
            Scripts = null;

            try
            {
                GetScriptsInExportFolder();
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }
            
        }

        private string GetSystemExportPath() => @"exports\\videos\\" + _selectedService.CurrentSystem.Replace(' ', '_') + "\\";

        private AviSynthOption aviSynthOptions;
        private ISettingsRepo _settings;
        private ISelectedService _selectedService;

        public AviSynthOption AviSynthOptions
        {
            get { return aviSynthOptions; }
            set { SetProperty(ref aviSynthOptions, value); }
        }

        private void GetScriptsInExportFolder()
        {
            var path = exportPath + _selectedService.CurrentSystem.Replace(' ', '_');

            if (!Directory.Exists(path)) return;

            _scripts = new List<string>();
            foreach (var file in Directory.EnumerateFiles(path, "*.avs"))
            {
                _scripts.Add(Path.GetFileNameWithoutExtension(file));
            }

            Scripts = new ListCollectionView(_scripts);
        }

        private void SaveScript(string[] videos)
        {            
            aviSynthOptions = new AviSynthOption()
            {
                DissolveAmount = DissolveAmount,
                StartFrame = StartFrame,
                EndFrame = EndFrame,
                FadeIn = FadeIn,
                FadeOut = FadeOut,
                ResizeHeight = ResizeHeight,
                ResizeWidth = ResizeWidth,
                WheelPosX = WheelPosX,
                WheelPosY = WheelPosY,
            };

            var wheelPath = _settings.HypermintSettings.HsPath + "\\Media\\" + _selectedService.CurrentSystem + "\\" +
                Images.Wheels + "\\";

            var scriptCreated = _avisynthScripter.CreateScript(videos, aviSynthOptions, _selectedService.CurrentSystem, Overlay, ResizeOverlay, wheelPath, @"exports\videos\");

            GetScriptsInExportFolder();

            Scripts.MoveCurrentTo(scriptCreated);

        }
    }
}
