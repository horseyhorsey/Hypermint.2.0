using System;
using GongSolutions.Wpf.DragDrop;
using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace Hypermint.Shell.ViewModels
{
    public class ShellViewModel : ViewModelBase, IDropTarget
    {
        public DelegateCommand<string> NavigateCommand { get; set; }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }

        private readonly IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;

        public ShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            NavigateCommand = new DelegateCommand<string>(Navigate);

            _eventAggregator.GetEvent<ErrorMessageEvent>().Subscribe(DisplayError);
        }

        private void DisplayError(string error)
        {
            ErrorMessage = error;
        }

        private void Navigate(string uri)
        {            
            
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }
    }
}
