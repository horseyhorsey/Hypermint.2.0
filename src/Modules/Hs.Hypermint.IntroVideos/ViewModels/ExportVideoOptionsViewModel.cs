using Hypermint.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Extensions;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Models;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public class ExportVideoOptionsViewModel : HypermintViewModelBase
    {
        #region Fields
        private IAviSynthScripter _avisynthScripter;
        private IEventAggregator _eventAggregator;
        private ISettingsHypermint _settings;
        private ISelectedService _selectedService;
        private readonly string exportPath;
        #endregion

        #region Constructors
        public ExportVideoOptionsViewModel(ILoggerFacade loggerFacade, IAviSynthScripter aviSynthScripter, IEventAggregator ea,
                    ISettingsHypermint settings, ISelectedService selected) : base(loggerFacade)
        {
            _avisynthScripter = aviSynthScripter;
            _eventAggregator = ea;
            _settings = settings;
            _selectedService = selected;
            exportPath = Path.Combine(settings.HypermintSettings.ExportPath, "Videos");

            AviSynthOptions = new AviSynthOption();

            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(x => SystemChanged(x));

            //Save the process videos to a an avi synth script.
            SaveScriptCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<GetProcessVideosEvent>().Publish();
            });

            _eventAggregator.GetEvent<ReturnProcessVideosEvent>().Subscribe(x =>
            {
                SaveScript(x);
            });

            ProcessScriptCommand = new DelegateCommand(() =>
            {
                ProcessScript();
            });

            OpenExportFolderCommand = new DelegateCommand(() =>
            {
                var dir = GetSystemExportPath();
                var result = new DirectoryInfo(dir).Open();
            });

        }

        #endregion

        #region Commands        
        public ICommand OpenExportFolderCommand { get; set; }
        public ICommand ProcessScriptCommand { get; set; }
        public ICommand SaveScriptCommand { get; set; }
        #endregion

        #region Properties

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

        private List<string> _scripts;

        #endregion        

        #region AviSynthProperties
        private AviSynthOption aviSynthOptions;
        public AviSynthOption AviSynthOptions
        {
            get { return aviSynthOptions; }
            set { SetProperty(ref aviSynthOptions, value); }
        }

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

        #region Support Methods

        /// <summary>
        /// Gets the system export path for videos
        /// </summary>
        /// <returns></returns>
        private string GetSystemExportPath() => Path.Combine(exportPath, _selectedService.CurrentSystem);

        /// <summary>
        /// Gets the scripts in export folder.
        /// </summary>
        private void GetScriptsInExportFolder()
        {            
            var path = GetSystemExportPath();
            Log($"Export path: {path}");

            if (!Directory.Exists(path)) return;

            _scripts = new List<string>();
            foreach (var file in Directory.EnumerateFiles(path, "*.avs"))
            {
                _scripts.Add(Path.GetFileNameWithoutExtension(file));
            }

            Scripts = new ListCollectionView(_scripts);
        }

        /// <summary>
        /// Processes the script with FFmpeg
        /// </summary>
        private void ProcessScript()
        {
            try
            {
                var selectedScript = Scripts.CurrentItem as string;
                if (selectedScript == null) return;

                var scriptPath = "\"" + $"{GetSystemExportPath() + selectedScript}.avs" + "\"";
                var videoPath = "\"" + $"{GetSystemExportPath() + selectedScript}.mp4" + "\"";

                Log($"{scriptPath} - {videoPath}");

                if (!File.Exists(_settings.HypermintSettings.Ffmpeg))
                {
                    Log($"Ffmpeg not available at {_settings.HypermintSettings.Ffmpeg}", Category.Warn);
                    throw new FileNotFoundException("Ffmpeg not found. Set the Ffmpeg path in settings to process.");
                }
                    

                var args = "-i " + scriptPath + " -vcodec libx264 -crf " + VideoQuality + " " + videoPath;
                Log($"Running Ffmpeg: {args}");
                Process.Start(_settings.HypermintSettings.Ffmpeg, args);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                Log(ex.Message, Category.Exception);
            }
        }

        /// <summary>
        /// Saves the script.
        /// </summary>
        /// <param name="videos">The videos.</param>
        private void SaveScript(IEnumerable<string> videos)
        {
            Log("");
            var aviSynthOptions = new AviSynthOption()
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

            //Setup script export options
            var scriptOptions = new ScriptOptions(videos.ToArray(),aviSynthOptions, 
                _selectedService.CurrentSystem, Overlay, ResizeOverlay, wheelPath,
                GetSystemExportPath());            

            //Create script from builder
            string scriptCreated = _avisynthScripter.CreateScript(scriptOptions);
            Log($"Script created: {scriptCreated}");

            //Get scripts and select the created
            GetScriptsInExportFolder();

            if (Scripts == null)
                Scripts = new ListCollectionView(_scripts);

            Scripts.MoveCurrentTo(scriptCreated);

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
                Log(ex.Message, Category.Exception);
                Debug.WriteLine(ex.Message);
            }

        }

        #endregion
    }
}
