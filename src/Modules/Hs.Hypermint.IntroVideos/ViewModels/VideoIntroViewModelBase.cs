using GongSolutions.Wpf.DragDrop;
using Hypermint.Base;
using Hypermint.Base.Model;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using Prism.Events;
using Hypermint.Base.Events;
using Prism.Logging;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public abstract class VideoIntroViewModelBase : HypermintViewModelBase, IDropTarget
    {
        private IEventAggregator _eventAggregator;

        #region Commands
        public ICommand AddSelectedCommand { get; set; }
        public ICommand RemoveSelectedCommand { get; set; }
        public ICommand SelectionProcessChanged { get; set; }
        #endregion

        public VideoIntroViewModelBase(ILoggerFacade loggerFacade, IEventAggregator eventAggregator) : base (loggerFacade)
        {
            _eventAggregator = eventAggregator;

            SelectedItems = new List<IntroVideo>();
            Videos = new ObservableCollection<IntroVideo>();

            AddSelectedCommand = new DelegateCommand(() => AddSelected());
            RemoveSelectedCommand = new DelegateCommand(RemoveSelected);
            SelectionProcessChanged = new DelegateCommand<IList>(videos =>
            {
                OnVideoSelectionChanged(videos);
            });
        }

        #region Properties        

        private ObservableCollection<IntroVideo> _videos;
        /// <summary>
        /// Gets or sets the videos.
        /// </summary>
        public ObservableCollection<IntroVideo> Videos
        {
            get { return _videos; }
            set { SetProperty(ref _videos, value); }
        }        

        public List<IntroVideo> SelectedItems { get; set; }

        #endregion

        #region Public Methods

        public virtual void AddSelected()
        {

        }

        public virtual void RemoveSelected()
        {

        }

        public virtual void OnVideoSelectionChanged(IList videos)
        {
            if (videos != null && videos.Count > 0)
            {
                SelectedItems.Clear();
                foreach (var video in videos)
                {
                    SelectedItems.Add(video as IntroVideo);
                }
            }

            if (videos.Count == 1)
                _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish((videos[0] as IntroVideo).FileName);
        }

        public void DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as IntroVideo;
            var targetItem = dropInfo.TargetItem as IntroVideo;

            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }

        }

        public void Drop(IDropInfo dropInfo)
        {
            OnDropped(dropInfo);
        }

        #endregion

        #region Support Methods

        private void OnDropped(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as IntroVideo;
            var targetItem = dropInfo.TargetItem as IntroVideo;

            var AddInIndex = Videos.IndexOf(targetItem);

            Videos.Remove(sourceItem);
            Videos.Insert(AddInIndex, sourceItem);            
        }

        #endregion

    }
}
