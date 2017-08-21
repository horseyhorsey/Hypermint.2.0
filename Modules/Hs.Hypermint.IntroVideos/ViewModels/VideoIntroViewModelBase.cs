using GongSolutions.Wpf.DragDrop;
using Hypermint.Base;
using Hypermint.Base.Model;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public abstract class VideoIntroViewModelBase : ViewModelBase, IDropTarget
    {
        #region Commands
        public ICommand AddSelectedCommand { get; set; }
        public ICommand RemoveSelectedCommand { get; set; }
        #endregion

        public VideoIntroViewModelBase()
        {
            SelectedVideos = new List<IntroVideo>();
            Videos = new ObservableCollection<IntroVideo>();

            AddSelectedCommand = new DelegateCommand(AddSelected);
            RemoveSelectedCommand = new DelegateCommand(RemoveSelected);
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

        public List<IntroVideo> SelectedVideos { get; set; }

        #endregion

        #region Public Methods

        public virtual void AddSelected()
        {

        }

        public virtual void RemoveSelected()
        {

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
