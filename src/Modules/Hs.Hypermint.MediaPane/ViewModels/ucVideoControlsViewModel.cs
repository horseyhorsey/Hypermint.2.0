using Hypermint.Base.Base;
using Hypermint.Base.Events;
using Prism.Commands;
using Prism.Events;
using System;
using System.Windows.Threading;

namespace Hs.Hypermint.MediaPane.ViewModels
{
    public class ucVideoControlsViewModel : ViewModelBase
    {
        public ucVideoControlsViewModel(IEventAggregator ea)
        {
            _eventAgg = ea;

            _eventAgg.GetEvent<MediaCommandEvent>().Subscribe(x =>
            {
                MediaPlayer(x);
            });
        }       

        #region Properties

        private string _videoLength;
        public string VideoLength
        {
            get { return _videoLength; }
            set { SetProperty(ref _videoLength, value); }
        }

        private double _currentSliderValue;
        /// <summary>
        /// Gets or Sets the CurrenSliderValue
        /// </summary>
        public double SliderValue
        {
            get { return _currentSliderValue; }
            set { SetProperty(ref _currentSliderValue, value); }
        }

        private double _VolumerSlider;
        /// <summary>
        /// Gets or Sets the VolumerSliders value
        /// </summary>
        public double VolumerSlider    
        {
            get { return _VolumerSlider; }
            set { SetProperty(ref _VolumerSlider, value); }
        }

        private DispatcherTimer timer = new DispatcherTimer();
        private IEventAggregator _eventAgg;

        #endregion

        #region Commands

        /// <summary>
        /// TODO: Send message to media element
        /// </summary>
        public DelegateCommand<int> PlayCommand { get; private set; }

        #endregion

        #region Support Methods
        private void MediaPlayer(int x)
        {
            switch (x)
            {
                case 0:
                    //Stop
                    break;
                case 1:
                    //Play
                    break;
                case 2:
                    //Pause
                    break;
                default:
                    break;
            }
        }

        private void SetSeekBarToZero()
        {
            var ts = new TimeSpan(0, 0, 0, 0);
            VideoLength = ts.ToString();
            SliderValue = 0;
        }
        #endregion
    }
}