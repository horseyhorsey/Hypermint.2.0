using Hypermint.Base.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Hs.Hypermint.VideoEdit.Views
{
    /// <summary>
    /// Interaction logic for MediaElementView
    /// </summary>
    public partial class MediaElementView : UserControl, IMediaPlayer
    {
        #region Fields
        private bool isPlaying;
        private DispatcherTimer timer = new DispatcherTimer();
        public bool IsSeekingMedia { get; private set; }
        #endregion

        #region Constructor
        public MediaElementView()
        {
            InitializeComponent();
            MediaElement.ScrubbingEnabled = true;
            MediaElement.Pause();
        }
        #endregion

        #region Media Player Interface
        void IMediaPlayer.Pause()
        {
            if (MediaElement.CanPause)
            {
                MediaElement.Pause();

                isPlaying = false;

                StopTimer();
            }
        }

        void IMediaPlayer.Play()
        {
            if (!isPlaying)
            {
                StartTimer();

                MediaElement.Play();

                isPlaying = true;
            }
        }

        void IMediaPlayer.Stop()
        {
            MediaElement.Stop();

            isPlaying = false;

            StopTimer();
        }

        TimeSpan IMediaPlayer.GetCurrentTime()
        {
            return MediaElement.Position;
        }
        #endregion

        #region Time Slider
        private void timelineSlider_SeekStarted(object sender, DragStartedEventArgs e)
        {
            IsSeekingMedia = true;
        }

        private void timelineSlider_SeekCompleted(object sender, DragCompletedEventArgs e)
        {
            IsSeekingMedia = false;
            MediaElement.Position = TimeSpan.FromSeconds(timelineSlider.Value);            
        }

        private void timelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media_wheelTime.Text = TimeSpan.FromSeconds(timelineSlider.Value).ToString(@"hh\:mm\:ss");
        }

        private void SetSeekBarToZero()
        {
            var ts = new TimeSpan(0, 0, 0, 0);
            media_wheelTime.Text = ts.ToString();
            timelineSlider.Value = 0;
        }
        #endregion

        #region Media Events
        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            SetSeekBarToZero();

            try
            {
                timelineSlider.Maximum = MediaElement.NaturalDuration.TimeSpan.TotalMilliseconds / 1000;                
            }
            catch { }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            SetSeekBarToZero();
            StopTimer();
            MediaElement.Stop();
            isPlaying = false;
            MediaElement.Position = TimeSpan.FromSeconds(timelineSlider.Value);
        }
        #endregion

        #region Timer
        private void StartTimer()
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void StopTimer()
        {
            timer.Tick -= timer_Tick;
            timer.Stop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((MediaElement.Source != null) && (MediaElement.NaturalDuration.HasTimeSpan) && (!IsSeekingMedia))
            {
                //timelineSlider.Minimum = 0;
                //timelineSlider.Maximum = MediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                timelineSlider.Value = MediaElement.Position.TotalSeconds;
            }
        }

        #endregion
    }
}
