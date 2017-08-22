using GongSolutions.Wpf.DragDrop;
using Hypermint.Base;
using Hypermint.Base.Model;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System;
using System.Collections;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public abstract class VideoIntroViewModelBase : ViewModelBase, IDropTarget
    {
        #region Commands
        public ICommand AddSelectedCommand { get; set; }
        public ICommand RemoveSelectedCommand { get; set; }
        public ICommand SelectionProcessChanged { get; set; }
        #endregion

        public VideoIntroViewModelBase()
        {
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
            //if (items == null)
            //{
            //    SelectedAvailableItemsCount = 0;
            //    SelectedAvailableVideos.Clear();
            //    return;
            //}
            //else
            //{
            //    SelectedAvailableItemsCount = items.Count;
            //}

            //try
            //{
            //    SelectedAvailableVideos.Clear();
            //    foreach (var item in items)
            //    {
            //        var video = item as IntroVideo;
            //        if (video.FileName != null)
            //            SelectedAvailableVideos.Add(video);
            //    }

            //    if (SelectedAvailableItemsCount > 1)
            //        SelectedAvailableHeader = "Selected videos: " + SelectedAvailableItemsCount;
            //    else if (SelectedAvailableItemsCount == 1)
            //    {
            //        var video = items[0] as IntroVideo;
            //        SelectedAvailableHeader = "Selected item: " + video.FileName;
            //        _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish(video.FileName);
            //    }
            //    else
            //        SelectedAvailableHeader = "";
            //}
            //catch (Exception)
            //{


            //}
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
