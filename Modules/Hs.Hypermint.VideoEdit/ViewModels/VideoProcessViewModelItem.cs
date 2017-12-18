using Hs.Hypermint.VideoEdit.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace Hs.Hypermint.VideoEdit.ViewModels
{
    public class VideoProcessViewModelItem : BindableBase
    {
        public VideoProcessViewModelItem(IEventAggregator ea)
        {
            _eventAggregator = ea;
            RemoveCommand = new DelegateCommand(Remove);
        }

        private void Remove()
        {
            _eventAggregator.GetEvent<VideoProcessItemRemoveEvent>().Publish(this);
        }

        public string SystemName { get; set; }
        public string File { get; set; }
        public bool Overwrite { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan Duration { get; set; }

        private IEventAggregator _eventAggregator;

        public ICommand RemoveCommand { get; set; }
    }
    
}
