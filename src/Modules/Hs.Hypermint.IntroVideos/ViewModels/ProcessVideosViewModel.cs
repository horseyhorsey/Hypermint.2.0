using Hypermint.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Model;
using MediaToolkit.Model;
using Prism.Events;
using Prism.Logging;
using System;
using System.Collections;
using System.Linq;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public class ProcessVideosViewModel : VideoIntroViewModelBase
    {
        private IEventAggregator _eventAggregator;

        public ProcessVideosViewModel(ILoggerFacade loggerFacade, IEventAggregator eventAggregator): base(loggerFacade, eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AddToProcessVideoListEvent>().Subscribe(video => AddVideo(video));
            _eventAggregator.GetEvent<ClearProcessVideosEvent>().Subscribe(ClearAllVideos);

            //Returns the videos to the save script options
            _eventAggregator.GetEvent<GetProcessVideosEvent>().Subscribe(ReturnProcessVideos);

            _eventAggregator.GetEvent<RemoveSelectedProcessVideosEvent>().Subscribe(RemoveSelected);
            _eventAggregator.GetEvent<ScanVideoFormatEvent>().Subscribe(ScanFormat);
        }

        /// <summary>
        /// Returns the process video list as IEnumerable<string> (filenames)
        /// </summary>
        private void ReturnProcessVideos()
        {
            if (Videos?.Count > 0)
            {
                _eventAggregator.GetEvent<ReturnProcessVideosEvent>()
                    .Publish(Videos.Select((x) => x.FileName));
            }
        }

        private void ClearAllVideos()
        {
            if (SelectedItems.Count <= 0) return;

            foreach (var video in Videos)
            {
                _eventAggregator.GetEvent<AddToAvailableVideosEvent>().Publish(video as IntroVideo);
            }

            Videos.Clear();
        }

        #region Support Methods

        /// <summary>
        /// Adds the video to the Videos collection
        /// </summary>
        /// <param name="video">The video.</param>
        private void AddVideo(IntroVideo video)
        {
            Videos.Add(video);
        }

        /// <summary>
        /// Called when the datagrid video selection changes
        /// </summary>
        /// <param name="videos">The videos.</param>
        public override void OnVideoSelectionChanged(IList videos)
        {
            base.OnVideoSelectionChanged(videos);
        }

        /// <summary>
        /// Removes the selected from the list and and adds back to the available videos
        /// </summary>
        public override void RemoveSelected()
        {
            if (SelectedItems.Count <= 0) return;

            foreach (var video in SelectedItems.ToArray())
            {
                _eventAggregator.GetEvent<AddToAvailableVideosEvent>().Publish(video as IntroVideo);

                Videos.Remove(video);
            }

            SelectedItems.Clear();
        }

        /// <summary>
        /// Scans all the videos for its format. Resolution, framerate
        /// </summary>
        private void ScanFormat()
        {
            Log("");
            foreach (var video in Videos)
            {
                try
                {
                    if (video.FrameRate == 0)
                    {
                        if (System.IO.File.Exists((video.FileName)))
                        {
                            var inputFile = new MediaFile { Filename = video.FileName };

                            try
                            {
                                using (var engine = new MediaToolkit.Engine())
                                {
                                    engine.GetMetadata(inputFile);
                                    video.Format = inputFile.Metadata.VideoData.FrameSize;
                                    video.FrameRate = inputFile.Metadata.VideoData.Fps;
                                    video.Duration = inputFile.Metadata.Duration.Duration();
                                    engine.Dispose();
                                }
                            }
                            catch (Exception ex) { Log(ex.Message, Prism.Logging.Category.Exception); }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.Message, Prism.Logging.Category.Exception);
                }
            }

        }

        #endregion
    }


}
