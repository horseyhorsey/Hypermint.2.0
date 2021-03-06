﻿using Hypermint.Base;
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
using Hypermint.Base.Model;

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
            _eventAggregator.GetEvent<NewSystemCreatedEvent>().Subscribe(OnSystemCreated);

            //UpdateSystemsAsync(_mainMenuXmlPath);
            //_eventAggregator.GetEvent<AddNewSystemEvent>().Publish("SystemsView");
        }

        public SystemsViewModel()
        {

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
                var sourceItem = dropInfo.Data as MenuItemViewModel;
                var targetItem = dropInfo.TargetItem as MenuItemViewModel;

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
                var sourceItem = dropInfo.Data as MenuItemViewModel;

                if (_hyperspinManager.Systems.IndexOf(sourceItem) == 0) return;

                var targetItem = dropInfo.TargetItem as MenuItemViewModel;

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
        /// Called when [system created]. Add the main menu item to the systems
        /// </summary>
        /// <param name="mainMenu">The main menu.</param>
        private void OnSystemCreated(MainMenu mainMenu)
        {
            if (mainMenu != null)
                _hyperspinManager.Systems.Add(new MenuItemViewModel(mainMenu));
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

            MenuItemViewModel system = SystemItems.CurrentItem as MenuItemViewModel;

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

            UpdateHeader();

            SystemItems.MoveCurrentToFirst();
        }

        private void UpdateHeader()
        {
            //Need to move the systems to viewmodel items to listen  for the change on enabled
            SystemsCount = _hyperspinManager.Systems.Count - 1;
            //var _enabledCount = _hyperspinManager.Systems.Where(x => x.Enabled == 1).Count();
            //            SystemsHeader = $"Systems: {SystemsCount} Enabled: {_enabledCount}";

            SystemsHeader = $"Systems: {SystemsCount}";
        }

        #endregion

    }
}
