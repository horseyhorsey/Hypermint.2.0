using System;
using GongSolutions.Wpf.DragDrop;
using Hypermint.Base;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Hypermint.Base.Services;

namespace Hypermint.Shell.ViewModels
{
    public class ShellViewModel : ViewModelBase, IDropTarget
    {
        public DelegateCommand<string> NavigateCommand { get; set; }

        #region Fields
        private readonly IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private IHyperspinManager _hsManager;
        private ISelectedService _service; 
        #endregion

        #region Constructor
        public ShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ISelectedService service, IHyperspinManager hsManager)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _hsManager = hsManager;
            _service = service;

            NavigateCommand = new DelegateCommand<string>(Navigate);

            _eventAggregator.GetEvent<ErrorMessageEvent>().Subscribe(DisplayError);
        } 
        #endregion

        #region Properties
        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        } 
        #endregion

        #region Support Methods
        private void DisplayError(string error)
        {
            ErrorMessage = error;
        }

        private void Navigate(string uri)
        {
            _eventAggregator.GetEvent<NavigateRequestEvent>().Publish(uri);
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
