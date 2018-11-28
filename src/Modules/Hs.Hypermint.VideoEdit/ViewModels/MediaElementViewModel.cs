using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Model;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hs.Hypermint.VideoEdit.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class MediaElementViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;
        private ISelectedService _selected;
        public IMediaPlayer _mediaElement;


        #region Commands
        public ICommand MarkVideoRangeCommand { get; set; }
        public DelegateCommand<IMediaPlayer> MediaElementLoadedCommand { get; set; }
        public ICommand VideoControlCommand { get; set; }
        public ICommand AddToProcessListCommand { get; set; }
        public ICommand NavigateAwayCommand { get; set; } 
        #endregion

        public MediaElementViewModel(IEventAggregator eventAggregator, ISelectedService selected)
        {
            _eventAggregator = eventAggregator;
            _selected = selected;

            MarkVideoRangeCommand = new DelegateCommand<string>(MarkVideoRange);
            MediaElementLoadedCommand = new DelegateCommand<IMediaPlayer>(MediaElementLoaded);
            VideoControlCommand = new DelegateCommand<string>(OnVideoControl);
            NavigateAwayCommand = new DelegateCommand(() => _eventAggregator.GetEvent<NavigateRequestEvent>().Publish("HsMediaAuditView"));

            _eventAggregator.GetEvent<VideoSourceEvent>().Subscribe(OnVideoSourceUpdated);

            //Add the selection start and end to process list
            AddToProcessListCommand = new DelegateCommand(AddToProcessList);
        }

        private void AddToProcessList()
        {
            if (SelectionStart.HasValue && SelectionEnd.HasValue)
            {
                var trimVideoItem = new TrimVideo
                {
                    Start = SelectionStart.Value,
                    End = SelectionEnd.Value,
                    SystemName= _selected.CurrentSystem,
                    RomName = _selected.CurrentRomname,
                    File = VideoSource
                };

                _eventAggregator.GetEvent<VideoProcessItemAddedEvent>()
                    .Publish(trimVideoItem);

                SelectionStart = null; SelectionEnd = null;
            }
        }

        private void MarkVideoRange(string inOut)
        {
            try
            {
                if (inOut == "In")
                    SelectionStart = _mediaElement.GetCurrentTime();
                else if (inOut == "Out")
                    SelectionEnd = _mediaElement.GetCurrentTime();
            }
            catch { }
        }

        public string VideoPreviewHeader { get; set; }
        public string VideoSource { get; set; }
        public TimeSpan? SelectionStart { get; set; }
        public TimeSpan? SelectionEnd { get; set; }
        public TimeSpan SliderValue { get; set; }

        #region Support Methods
        /// <summary>
        /// Called when the Loaded event happens on the a MediaElement
        /// </summary>
        /// <param name="mediaPlayer">The media player.</param>
        private void MediaElementLoaded(IMediaPlayer mediaPlayer)
        {
            _mediaElement = mediaPlayer;
            SliderValue = _mediaElement.GetCurrentTime();
        }

        /// <summary>
        /// Called when a button is fired in the view
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OnVideoControl(string obj)
        {
            switch (obj)
            {
                case "pause":
                    _mediaElement.Pause();
                    break;
                case "play":
                    _mediaElement.Play();
                    break;
                case "stop":
                    _mediaElement.Stop();
                    break;
                default:
                    break;
            }
        }

        private void OnVideoSourceUpdated(string obj)
        {
            VideoSource = obj;

            VideoPreviewHeader = obj;
        }

        #endregion
    }
}
