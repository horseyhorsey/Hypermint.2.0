using Hypermint.Base.Events;
using Prism.Events;
using System;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public class AvailableVideosViewModel : VideoIntroViewModelBase
    {
        private IEventAggregator _eventAggregator;

        public AvailableVideosViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<RandomVideosEvent>().Subscribe((amount) => GrabRandomVideos(amount));
        }

        public AvailableVideosViewModel()
        {

        }

        #region Public Methods
        /// <summary>
        /// Adds and removes the selected videos to the process list.
        /// </summary>
        public override void AddSelected()
        {
            if (SelectedVideos.Count <= 0) return;

            foreach (var video in SelectedVideos)
            {
                Videos.Remove(video);

                _eventAggregator.GetEvent<AddToProcessVideoListEvent>().Publish(video);
            }
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

        #endregion
    }
}
