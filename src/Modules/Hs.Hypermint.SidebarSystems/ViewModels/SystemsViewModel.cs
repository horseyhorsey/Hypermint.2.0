using Hs.HyperSpin.Database;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Prism.Events;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using Hypermint.Base.Constants;
using Hypermint.Base.Base;
using System.Diagnostics;
using Hypermint.Base.Services;
using System.Windows.Media.Imaging;
using System;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using Prism.Commands;
using Hypermint.Base.Events;

namespace Hs.Hypermint.SidebarSystems.ViewModels
{
    public class SystemsViewModel : ViewModelBase, IDropTarget
    {
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

        private bool reOrderSystems;
        public bool ReOrderSystems
        {
            get { return reOrderSystems; }
            set { SetProperty(ref reOrderSystems, value); }
        }

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

        IMainMenuRepo _mainMenuRepo;
        ISettingsRepo _settingsRepo;
        IEventAggregator _eventAggregator;
        ISelectedService _selectedService;

        public DelegateCommand SaveMainMenuCommand { get; private set; }
        public DelegateCommand AddSystemCommand { get; private set; } 


        public SystemsViewModel()
        {
            _eventAggregator.GetEvent<AddNewSystemEvent>().Publish("SystemsView");
        }

        public SystemsViewModel(IMainMenuRepo main, IEventAggregator eventAggregator,
            ISettingsRepo settings, ISelectedService selectedService)
        {
            _mainMenuRepo = main;
            _eventAggregator = eventAggregator;

            _settingsRepo = settings;
            _settingsRepo.LoadHypermintSettings();

            _selectedService = selectedService;

            // Setup the main menu database to read in all systems

            _mainMenuXmlPath = "";

            _mainMenuXmlPath = Path.Combine(
                    _settingsRepo.HypermintSettings.HsPath, Root.Databases,
                    @"Main Menu\Main Menu.xml");

            UpdateSystems(_mainMenuXmlPath);

            _eventAggregator.GetEvent<MainMenuSelectedEvent>().Subscribe(UpdateSystems);
            _eventAggregator.GetEvent<SystemFilteredEvent>().Subscribe(FilterSystemsByText);

            SaveMainMenuCommand = new DelegateCommand(SaveMainMenu);

            AddSystemCommand = new DelegateCommand(AddSystem);

        }

        private void AddSystem()
        {
            
        }

        private void SaveMainMenu()
        {
            if (_selectedService.CurrentMainMenu == null) return;

            try
            {
                _eventAggregator.GetEvent<SaveMainMenuEvent>()
                    .Publish(_selectedService.CurrentMainMenu);
            }
            catch (Exception) { }

        }

        private void UpdateSystems(string mainMenuXml)
        {            
            if (File.Exists(mainMenuXml))
            {
                _mainMenuXmlPath = mainMenuXml;
                _mainMenuRepo.BuildMainMenuItems(mainMenuXml, _settingsRepo.HypermintSettings.RlMediaPath + @"\Icons\");
                SystemItems = new ListCollectionView(_mainMenuRepo.Systems);

                //Subscribe here again?? 
                //##    Existing unsubscribes when the system list is changed.
                SystemItems.CurrentChanged += SystemItems_CurrentChanged;

                SystemsCount = _mainMenuRepo.Systems.Count - 1;
                SystemsHeader = "Systems: " + SystemsCount;

                _eventAggregator.GetEvent<SystemsGenerated>().Publish("");
                
            }
        }

        /// <summary>
        /// Filter the systems list
        /// </summary>
        /// <param name="obj"></param>
        private void FilterSystemsByText(string filter)
        {
            //Unsubscribe when the filter is being set
            //Avoiding the systems databases loading on each filter change            
            SystemItems.CurrentChanged -= SystemItems_CurrentChanged;

            if (SystemItems != null)
            {
                ICollectionView cv;

                cv = CollectionViewSource.GetDefaultView(SystemItems);

                cv.Filter = o =>
                {
                    var m = o as MainMenu;

                    var textFiltered = m.Name.ToUpper().Contains(filter.ToUpper());
                    return textFiltered;
                };

            }

            SystemItems.CurrentChanged += SystemItems_CurrentChanged;

        }

        private void SystemItems_CurrentChanged(object sender, System.EventArgs e)
        {

            MainMenu system = SystemItems.CurrentItem as MainMenu;

            if (system != null)
            {

                _selectedService.CurrentSystem = system.Name;

                if (!ReOrderSystems)
                {
                    SetSystemImage();

                    this._eventAggregator.GetEvent<SystemSelectedEvent>().Publish(system.Name);
                }
            }
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
                _selectedService.SystemImage = setImage(imagePath);

        }

        private BitmapImage setImage(string imagePath)
        {
            Uri uriSource;
            uriSource = new Uri(imagePath);
            return new BitmapImage(uriSource);
        }

        public void DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as MainMenu;
            var targetItem = dropInfo.TargetItem as MainMenu;

            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }

        }

        public void Drop(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as MainMenu;
            var targetItem = dropInfo.TargetItem as MainMenu;

            var AddInIndex = _mainMenuRepo.Systems.IndexOf(targetItem);

            if (AddInIndex == 0)
                AddInIndex = 1;

            _mainMenuRepo.Systems.Remove(sourceItem);
            _mainMenuRepo.Systems.Insert(AddInIndex, sourceItem);
        }
    }
}
