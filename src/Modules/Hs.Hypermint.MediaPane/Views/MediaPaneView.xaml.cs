using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Hs.Hypermint.MediaPane.Views
{
    /// <summary>
    /// Interaction logic for MediaPaneView
    /// </summary>
    public partial class MediaPaneView : UserControl
    {
        #region Properties
        private bool IsSeekingMedia;

        private bool isPlaying;

        private DispatcherTimer timer = new DispatcherTimer();
        #endregion

        public MediaPaneView()
        {
            InitializeComponent();

            mediaElement.LoadedBehavior = MediaState.Manual;
        }

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
            if ((mediaElement.Source != null) &&
                (mediaElement.NaturalDuration.HasTimeSpan) &&
                (!IsSeekingMedia))
            {
                timelineSlider.Minimum = 0;
                timelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                timelineSlider.Value = mediaElement.Position.TotalSeconds;

            }
        }
        #endregion

        #region MediaPlayerElement
        private void timelineSlider_SeekStarted(object sender, DragStartedEventArgs e)
        {
            IsSeekingMedia = true;
        }

        private void timelineSlider_SeekCompleted(object sender, DragCompletedEventArgs e)
        {
            IsSeekingMedia = false;
            mediaElement.Position = TimeSpan.FromSeconds(timelineSlider.Value);

        }

        private void timelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media_wheelTime.Text = TimeSpan.FromSeconds(timelineSlider.Value).ToString(@"hh\:mm\:ss");
        }

        private void PauseButtonMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Pause();
        }

        private void Pause()
        {
            if (mediaElement.CanPause)
            {
                mediaElement.Pause();
                isPlaying = false;
            }
        }

        private void PlayButtonMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StartMediaElementPlay();
        }

        private void StartMediaElementPlay()
        {
            if (!isPlaying)
            {
                StartTimer();

                mediaElement.Play();

                isPlaying = true;
            }
        }

        private void StopButtonMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SetSeekBarToZero();

            mediaElement.Stop();

            isPlaying = false;
        }

        private void SetSeekBarToZero()
        {
            var ts = new TimeSpan(0, 0, 0, 0);
            media_wheelTime.Text = ts.ToString();
            timelineSlider.Value = 0;
        }

        private void ChangeMediaVolume(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = volumeSlider.Value;
        }

        private void MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            SetSeekBarToZero();

            try
            {
                timelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;

            }
            catch (Exception)
            {

            }
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            SetSeekBarToZero();
            StopTimer();
            mediaElement.Stop();
            isPlaying = false;
            mediaElement.Position = TimeSpan.FromSeconds(timelineSlider.Value);

        }


        #endregion

    }
}