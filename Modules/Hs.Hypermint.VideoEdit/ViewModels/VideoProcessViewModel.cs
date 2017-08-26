using Hs.Hypermint.VideoEdit.Events;
using Hypermint.Base;
using Hypermint.Base.Model;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Threading.Tasks;
using Hs.Hypermint.VideoEdit.Helpers;
using Hypermint.Base.Interfaces;

namespace Hs.Hypermint.VideoEdit.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class VideoProcessViewModel : BindableBase
    {
        private ISettingsHypermint _settings;
        private IEventAggregator _eventAggregator;

        public VideoProcessViewModel(IEventAggregator eventAggregator, ISettingsHypermint settings)
        {
            _settings = settings;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<VideoProcessItemAddedEvent>().Subscribe(OnVideoProcessAdded);
            _eventAggregator.GetEvent<VideoProcessItemRemoveEvent>().Subscribe(x => this.VideoProcessItems.Remove(x));

            ProcessListCommand = new DelegateCommand(async () => await ProcessList());

            VideoProcessItems = new ObservableCollection<VideoProcessViewModelItem>();
        }

        private async Task ProcessList()
        {
            await Task.Run(() =>
            {
                var ff = _settings.HypermintSettings.Ffmpeg;
                foreach (var video in VideoProcessItems)
                {
                    try
                    {
                        VideoHelper.TrimVideoRange(ff, video.File, @"C:\Temp\OutputProcess.mp4", video.StartTime, video.EndTime);
                    }
                    catch (Exception ex) { }
                }                
            });

            VideoProcessItems.Clear();
        }

        private void OnVideoProcessAdded(TrimVideo trimVideo)
        {
            VideoProcessItems.Add(new VideoProcessViewModelItem(_eventAggregator)
            {
                File = trimVideo.File,
                StartTime = trimVideo.Start,
                EndTime = trimVideo.End,
                Duration = trimVideo.End - trimVideo.Start,
                Overwrite = false,
                SystemName = trimVideo.SystemName,
            });

            trimVideo = null;

        }

        public ICommand ProcessListCommand { get; set; }

        public ObservableCollection<VideoProcessViewModelItem> VideoProcessItems { get; set; }
    }
}
