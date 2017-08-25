using Prism.Events;
using Hypermint.Base;
using Hypermint.Base.Services;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Input;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class DatabaseDetailsViewModel : ViewModelBase
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private IDialogCoordinator _dialogService;
        private IHyperspinManager _hyperspinManager;
        private ISelectedService _selectedService;
        #endregion

        #region Constructors
        public DatabaseDetailsViewModel()
        {

        }

        public DatabaseDetailsViewModel(ISelectedService selectedService, IEventAggregator eventAggregator,
            IDialogCoordinator dialogService, IHyperspinManager hyperspinManager)
        {
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            _hyperspinManager = hyperspinManager;
            _selectedService = selectedService;

            _eventAggregator.GetEvent<GameSelectedEvent>().Subscribe(UpdateHeader);
        }

        #endregion

        #region Commands        
        public ICommand AuditScanStart { get; private set; }        
        public ICommand ScanRomsCommand { get; private set; }
        #endregion

        private string databaseHeaderInfo = "Hyperspin Database Editor";
        public string DatabaseHeaderInfo
        {
            get { return databaseHeaderInfo; }
            set { SetProperty(ref databaseHeaderInfo, value); }
        }


        /// <summary>
        /// Updates the header with a selected rom or selected rom count
        /// </summary>
        /// <param name="obj">The object.</param>
        private void UpdateHeader(string[] obj)
        {
            var selectedCount = _selectedService.SelectedGames.Count;
            if (selectedCount > 1)
            {
                DatabaseHeaderInfo = $"Hyperspin Database Editor: Roms selected {selectedCount}";
            }
            else if (selectedCount == 1)
            {
                DatabaseHeaderInfo = $"Hyperspin Database Editor: Roms selected {_selectedService.SelectedGames[0].RomName}";
            }
            else
            {
                DatabaseHeaderInfo = $"Hyperspin Database Editor";
            }
        }
    }
}
