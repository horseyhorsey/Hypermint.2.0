using MediaToolkit.Model;
using System;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public class ProcessVideosViewModel : VideoIntroViewModelBase
    {
        public ProcessVideosViewModel()
        {            
        }

        #region Support Methods

        /// <summary>
        /// Scans all the videos for its format. Resolution, framerate
        /// </summary>
        private void ScanFormat()
        {
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
                            catch (Exception) { }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.InnerException.Message);
                }
            }

        }
        #endregion
    }

    
}
