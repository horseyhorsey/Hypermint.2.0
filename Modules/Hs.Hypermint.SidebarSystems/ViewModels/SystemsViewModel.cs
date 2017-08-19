using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Prism.Events;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using Hypermint.Base.Constants;
using Hypermint.Base.Services;
using System.Windows.Media.Imaging;
using System;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using Hypermint.Base.Events;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using Frontends.Models.Hyperspin;
using System.Xml;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    public class SystemsViewModel : ViewModelBase, IDropTarget
    {
        #region Fields
        ISettingsHypermint _settingsRepo;
        IEventAggregator _eventAggregator;
        ISelectedService _selectedService;
        IDialogCoordinator _dialogService;
        IHyperspinManager _hyperspinManager;
        #endregion

        #region Constructors

        public SystemsViewModel(IHyperspinManager hyperspinManager,
            IHyperspinXmlDataProvider hsDataProvider,
            IEventAggregator eventAggregator,
            ISettingsHypermint settings,
            IDialogCoordinator dialogService,
            ISelectedService selectedService)
        {
            _eventAggregator = eventAggregator;
            _settingsRepo = settings;
            _selectedService = selectedService;
            _dialogService = dialogService;
            _hyperspinManager = hyperspinManager;

            if (_settingsRepo.HypermintSettings == null)
                _settingsRepo.LoadHypermintSettings();

            // Setup the main menu database to read in all systems
            _mainMenuXmlPath = "";
            _mainMenuXmlPath = Path.Combine(
                    _settingsRepo.HypermintSettings.HsPath, Root.Databases,
                    @"Main Menu\Main Menu.xml");

            SystemItems = new ListCollectionView(_hyperspinManager.Systems);
            SystemItems.CurrentChanged += SystemItems_CurrentChanged;

            //When main menu is selected
            _eventAggregator.GetEvent<MainMenuSelectedEvent>().Subscribe(async x =>
            {
                await OnMainMenuSelected(x);
            });

            _eventAggregator.GetEvent<SystemFilteredEvent>().Subscribe(FilterSystemsByText);
            _eventAggregator.GetEvent<ReorderingSystemsEvent>().Subscribe(x => ReOrderSystems = x);

            //UpdateSystemsAsync(_mainMenuXmlPath);

            //_eventAggregator.GetEvent<AddNewSystemEvent>().Publish("SystemsView");

        }
        #endregion

        #region Properties
        private ICollectionView _systemItems;
        public ICollectionView SystemItems
        {
            get { return _systemItems; }
            set { SetProperty(ref _systemItems, value); }
        }

        private bool systemListEnabled = true;
        public bool SystemListEnabled
        {
            get { return systemListEnabled; }
            set { SetProperty(ref systemListEnabled, value); }
        }

        private string _mainMenuXmlPath;

        private string systemsHeader = "Systems";
        public string SystemsHeader
        {
            get { return systemsHeader; }
            set { SetProperty(ref systemsHeader, value); }
        }

        private int systemCount;
        public int SystemsCount
        {
            get { return systemCount; }
            set { SetProperty(ref systemCount, value); }
        }

        private bool ReOrderSystems;
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the current drag state. Check if allowed to move the system first
        /// </summary>
        /// <param name="dropInfo">Information about the drag.</param>
        public void DragOver(IDropInfo dropInfo) => OnDragOver(dropInfo);

        public void Drop(IDropInfo dropInfo) => OnDrop(dropInfo);
        #endregion

        #region Support Methods

        /// <summary>
        /// Generate directorys for a hyperspin system
        /// </summary>
        /// <param name="hsPath">Path to hyperspin</param>
        /// <param name="systemName">The system name</param>
        [Obsolete]
        private void CreateMediaDirectorysForNewSystem(string hsPath, string systemName)
        {
            var newSystemMediaPath = Path.Combine(hsPath, Root.Media, systemName);

            CreateDefaultHyperspinFolders(newSystemMediaPath);
        }

        /// <summary>
        /// Creates all needed folders for a hyperspin system
        /// </summary>
        /// <param name="hyperSpinSystemMediaDirectory"></param>
        [Obsolete]
        private void CreateDefaultHyperspinFolders(string hyperSpinSystemMediaDirectory)
        {
            for (int i = 1; i < 5; i++)
            {
                Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\Images\\Artwork" + i);
            }
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Backgrounds);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.GenreBackgrounds);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.GenreWheel);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Letters);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Pointer);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Special);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Images.Wheels);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Root.Themes);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Sound.BackgroundMusic);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Sound.SystemExit);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Sound.SystemStart);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Sound.WheelSounds);
            Directory.CreateDirectory(hyperSpinSystemMediaDirectory + "\\" + Root.Video);
        }

        /// <summary>
        /// Filter the systems list
        /// </summary>
        /// <param name="obj"></param>
        private void FilterSystemsByText(string filter)
        {
            if (SystemItems != null)
            {
                //Unsubscribe when the filter is being set
                //Avoiding the systems databases loading on each filter change            
                SystemItems.CurrentChanged -= SystemItems_CurrentChanged;

                ICollectionView cv;

                cv = CollectionViewSource.GetDefaultView(SystemItems);

                cv.Filter = o =>
                {
                    var m = o as MainMenu;

                    var textFiltered = m.Name.ToUpper().Contains(filter.ToUpper());
                    return textFiltered;
                };

                SystemItems.CurrentChanged += SystemItems_CurrentChanged;
            }
        }

        private void OnDrop(IDropInfo dropInfo)
        {
            if (ReOrderSystems)
            {
                var sourceItem = dropInfo.Data as MainMenu;
                var targetItem = dropInfo.TargetItem as MainMenu;

                var AddInIndex = _hyperspinManager.Systems.IndexOf(targetItem);

                if (AddInIndex == 0) AddInIndex = 1;

                _hyperspinManager.Systems.Remove(sourceItem);
                _hyperspinManager.Systems.Insert(AddInIndex, sourceItem);

                var targetIndex = _hyperspinManager.Systems.IndexOf(sourceItem);

                SystemItems.MoveCurrentToPosition(targetIndex);
            }
        }

        private void OnDragOver(IDropInfo dropInfo)
        {
            if (ReOrderSystems)
            {
                var sourceItem = dropInfo.Data as MainMenu;

                if (_hyperspinManager.Systems.IndexOf(sourceItem) == 0) return;

                var targetItem = dropInfo.TargetItem as MainMenu;

                if (sourceItem != null && targetItem != null)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Copy;
                }

            }
        }

        /// <summary>
        /// Called when [main menu selected].
        /// </summary>
        /// <param name="x">The x.</param>
        private async Task OnMainMenuSelected(string x)
        {
            try { await UpdateSystemsAsync(x); }
            catch (XmlException xmlEx)
            {
                _hyperspinManager.Systems.Clear();
                _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(xmlEx.Message);
            }
            catch (Exception ex) { _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(ex.Message); }
            finally
            {

            }
        }

        /// <summary>
        /// Sets the image.
        /// </summary>
        /// <param name="imagePath">The image path.</param>
        /// <returns></returns>
        private BitmapImage SetImage(string imagePath)
        {
            Uri uriSource;
            uriSource = new Uri(imagePath);
            return new BitmapImage(uriSource);
        }

        /// <summary>
        /// Set wheel image for the system
        /// </summary>
        /// <param name="path"></param>
        private void SetSystemImage()
        {
            var imagePath = _settingsRepo.HypermintSettings.HsPath +
                "\\Media\\Main Menu\\Images\\Wheel\\" +
                _selectedService.CurrentSystem + ".png";

            if (File.Exists(imagePath))
                _selectedService.SystemImage = SetImage(imagePath);
            else
                _selectedService.SystemImage = null;
        }

        /// <summary>
        /// Handles the CurrentChanged event of the SystemItems control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SystemItems_CurrentChanged(object sender, System.EventArgs e)
        {
            if (ReOrderSystems) return;

            MainMenu system = SystemItems.CurrentItem as MainMenu;

            if (system != null)
            {
                _selectedService.CurrentSystem = system.Name;

                if (!ReOrderSystems)
                {
                    _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish("");

                    SetSystemImage();

                    this._eventAggregator.GetEvent<SystemSelectedEvent>().Publish(system.Name);
                }
            }
        }

        /// <summary>
        /// Updates the systems from hs manager asynchronous.
        /// </summary>
        /// <param name="mainMenuXml">The main menu XML.</param>
        /// <returns></returns>
        private async Task UpdateSystemsAsync(string mainMenuXml)
        {
            await _hyperspinManager.PopulateMainMenuSystems(mainMenuXml);

            SystemsCount = _hyperspinManager.Systems.Count - 1;
            SystemsHeader = "Systems: " + SystemsCount;

#warning    Nothing is used by this message
            _eventAggregator.GetEvent<SystemsGenerated>().Publish("");
        }

        #endregion

    }
}
