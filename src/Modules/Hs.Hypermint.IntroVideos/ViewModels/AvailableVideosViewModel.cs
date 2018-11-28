using Hypermint.Base;
using Hypermint.Base.Constants;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Model;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Input;
using System.Collections;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public class AvailableVideosViewModel : VideoIntroViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private IFileFolderChecker _fileChecker;
        private ISettingsHypermint _settings;

        public ICommand RandomVideoCommand { get; set; }

        public AvailableVideosViewModel(IEventAggregator eventAggregator, ISettingsHypermint settings, IFileFolderChecker fileChecker) : base(eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _fileChecker = fileChecker;
            _settings = settings;

            RandomVideoCommand = new DelegateCommand(() => { });

            base.AddSelectedCommand = new DelegateCommand(() => AddSelected());

            //Events
            _eventAggregator.GetEvent<AddSelectedVideosEvent>().Subscribe(AddSelected);
            _eventAggregator.GetEvent<AddToAvailableVideosEvent>().Subscribe(AddVideo);
            _eventAggregator.GetEvent<RandomVideosEvent>().Subscribe((amount) => GrabRandomVideos(amount));
            _eventAggregator.GetEvent<SystemSelectedEvent>().Subscribe(x => SystemChanged(x));
        }

        #region Public Methods
        /// <summary>
        /// Adds and removes the selected videos to the process list.
        /// </summary>
        public override void AddSelected()
        {
            if (SelectedItems.Count <= 0) return;

            foreach (var video in SelectedItems.ToArray())
            {
                Videos.Remove(video);

                _eventAggregator.GetEvent<AddToProcessVideoListEvent>().Publish(video);
            }

            SelectedItems.Clear();
        }

        public void AddVideo(IntroVideo vid)
        {
            Videos.Add(vid);
        }

        public override void OnVideoSelectionChanged(IList videos)
        {
            base.OnVideoSelectionChanged(videos);
        }

        #endregion

        #region Support Methods

        private void GrabRandomVideos(int amount)
        {
            //Return if amount is Zero or not enough videos in the list to add
            if (amount == 0) return;
            if (amount > Videos.Count) return;

            if (amount == Videos.Count)
            {
                foreach (var video in Videos)
                {
                    Videos.Remove(video);

                    _eventAggregator.GetEvent<AddToProcessVideoListEvent>().Publish(video);
                }
            }
            else
            {
                // Add random videos from the incoming video amount
                var random = new Random(Videos.Count);

                for (int i = 0; i < amount; i++)
                {
                    var randomVideo = Videos[random.Next(Videos.Count)];

                    Videos.Remove(randomVideo);
                    _eventAggregator.GetEvent<AddToProcessVideoListEvent>().Publish(randomVideo);
                }
            }

        }

        [Obsolete("Dont scan videos when system is clicked. Add option to do that in view.")]
        private void ScanVideosForSystem(string hyperSpinVideoPath, string videoExtFilter = "*.mp4")
        {
            try
            {
                var videoFiles = _fileChecker.GetFiles(hyperSpinVideoPath + "\\", videoExtFilter);

                foreach (var video in videoFiles)
                {
                    Videos.Add(new IntroVideo()
                    {
                        FileName = video
                    });
                }

                //VideosAvailableHeader = "Videos Available: " + scannedVideos.Count;
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Update the video list for this system
        /// </summary>
        /// <param name="systemName"></param>
        private void SystemChanged(string systemName)
        {
            try
            {
                Videos.Clear();

                var systemVideoPath = _settings.HypermintSettings.HsPath + "\\Media\\" + systemName + "\\" + Root.Video;

                ScanVideosForSystem(systemVideoPath);
            }
            catch (Exception)
            {

            }

        }

        #endregion
    }
}
