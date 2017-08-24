using Hypermint.Base;
using Prism.Events;
using System.Collections.ObjectModel;
using System;
using System.IO;
using Hs.Hypermint.Services.Helpers;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using System.Windows.Input;
using Prism.Commands;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MahApps.Metro.Controls.Dialogs;
using Hs.Hypermint.FilesViewer.Dialog;
using Hypermint.Base.Events;

namespace Hs.Hypermint.FilesViewer.ViewModels
{
    public class RlFilesViewModel : ViewModelBase, IDropTarget
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private ISettingsHypermint _settings;
        private ISelectedService _selectedService;
        private IDialogCoordinator _dialogService;
        private CustomDialog customDialog;
        public ICommand SelectedItemChangedCommand { get; set; }          
        #endregion

        #region Constructors
        public RlFilesViewModel(IEventAggregator ea, ISettingsHypermint settings, ISelectedService selectedService, IDialogCoordinator dialogService)
        {
            _eventAggregator = ea;
            _settings = settings;
            _selectedService = selectedService;
            _dialogService = dialogService;

            RlFiles = new ObservableCollection<RlFileItemViewModel>();
            SelectedNodes = new ObservableCollection<RlFileItemViewModel>();

            _eventAggregator.GetEvent<UpdateFilesEvent>().Subscribe(OnUpdateFiles);

            SelectedItemChangedCommand = new DelegateCommand<RlFileItemViewModel>(FilesSelectionChanged);

        } 
        #endregion

        #region Properties
        private ObservableCollection<RlFileItemViewModel> _rlFiles;
        public ObservableCollection<RlFileItemViewModel> RlFiles
        {
            get { return _rlFiles; }
            set { SetProperty(ref _rlFiles, value); }
        }

        public ObservableCollection<RlFileItemViewModel> SelectedNodes { get; set; }

        private string _filesHeader;
        private bool cancelPending;
        private DroppedFilesViewModel _droppedFilesViewModel;

        public string FilesHeader
        {
            get { return _filesHeader; }
            set { SetProperty(ref _filesHeader, value); }
        }

        public string SelectedFolder { get; private set; }
        public string MediaType { get; private set; } 
        #endregion

        #region File Dropping
        public void DragOver(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(item =>
            {
                var extension = Path.GetExtension(item);
                return extension != null;
            }) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public async void Drop(IDropInfo dropInfo)
        {
            if (!Directory.Exists(_settings.HypermintSettings.RlMediaPath)) return;
            if (string.IsNullOrWhiteSpace(MediaType)) return;
            if (MediaType == "RomName" || MediaType == "Description") return;

            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(item =>
            {
                var extension = Path.GetExtension(item);
                return extension != null && extension.Equals(".*");
            }) ? DragDropEffects.Copy : DragDropEffects.None;

            await ShowFilesDialogsAsync(dragFileList);
            
            //#TODO Update files after dropping
            //this. UpdateMediaFiles(SelectedFolder);
            //if (Files != null)
            //{
            //    Files.CurrentChanged += Files_CurrentChanged;

            //    if (Files.CurrentItem == null)
            //        _eventAggregator.GetEvent<PreviewGeneratedEvent>().Publish("");
            //    else
            //    {
            //        _eventAggregator.GetEvent<RlAuditUpdateEvent>().Publish(new string[] { _romName, MediaType });

            //        Files_CurrentChanged(null, null);
            //    }
            //}

        }

        private async Task ShowFilesDialogsAsync(IEnumerable<string> dragFileList)
        {
            var _currentPath = RlStaticMethods.GetSelectedPath(_settings.HypermintSettings.RlMediaPath, MediaType, _selectedService.CurrentSystem, _selectedService.CurrentRomname);

            foreach (var file in dragFileList)
            {
                _droppedFilesViewModel = new DroppedFilesViewModel(MediaType, file, _dialogService, customDialog);

                await DroppedFileCustomDialogAsync(file);

                await customDialog.WaitUntilUnloadedAsync();

                if (cancelPending)
                    break;
            }            
        }

        private async Task DroppedFileCustomDialogAsync(string file)
        {
            var settings = new MetroDialogSettings();
            settings.ColorScheme = MetroDialogColorScheme.Theme;

            var title = string.Format("RL Drop: {0} : {1} {2}", MediaType, 
                _selectedService.CurrentSystem, _selectedService.CurrentRomname);

            customDialog = new CustomDialog() { Title = title };            
            customDialog.Content = new DroppedFilesView { DataContext = _droppedFilesViewModel };

            _droppedFilesViewModel.SetCustomDialog(customDialog);
            _droppedFilesViewModel.SelectedFolder = SelectedFolder;

            await _dialogService.ShowMetroDialogAsync(this, customDialog, settings);
        }

        #endregion

        #region Support Methods

        /// <summary>
        /// Files selection changed.
        /// </summary>
        /// <param name="fileItemVm">The file item vm.</param>
        private void FilesSelectionChanged(RlFileItemViewModel fileItemVm)
        {
            
            ShowMediaPaneSelectedFile(fileItemVm);
        }

        private void SetSelectedClearFiles(string[] HeaderAndGame)
        {
            UpdateHeader(HeaderAndGame[0], HeaderAndGame[1]);

            SelectedFolder = "";

            try
            {
                this.SelectedNodes.Clear();
                this.RlFiles = new ObservableCollection<RlFileItemViewModel>();
            }
            catch (Exception ex)
            {                
            }
        }       

        private void ShowMediaPaneSelectedFile()
        {
            //Clear the pane
            _eventAggregator.GetEvent<SetMediaFileRlEvent>().Publish("");

            // Display media file
            //try
            //{
            //    var fileToDisplay = RlFiles;

            //    _eventAggregator
            //            .GetEvent<SetMediaFileRlEvent>()
            //            .Publish("");

            //    if (fileToDisplay != null)
            //    {
            //        //If the final contains the name bezel then display the view to edit it.
            //        if (fileToDisplay.Name.ToLower().Contains("bezel") && fileToDisplay.Extension.ToLower() == ".png")
            //        {
            //            if (RlStaticMethods.GetMediaFormatFromFile(fileToDisplay.FullPath) == "image")
            //            {
            //                _eventAggregator.GetEvent<NavigateMediaPaneRequestEvent>().Publish("BezelEditView");
            //            }

            //            _eventAggregator.GetEvent<SetBezelImagesEvent>()
            //            .Publish(fileToDisplay.FullPath);
            //        }
            //        else
            //        {
            //            _eventAggregator.GetEvent<NavigateMediaPaneRequestEvent>().Publish("MediaPaneView");

            //            _eventAggregator
            //            .GetEvent<SetMediaFileRlEvent>()
            //            .Publish(fileToDisplay.FullPath);
            //        }

            //    }
            //}
            //catch (NullReferenceException ex)
            //{
            //    _eventAggregator
            //        .GetEvent<ErrorMessageEvent>().Publish(ex.Message);
            //}

        }

        private void ShowMediaPaneSelectedFile(RlFileItemViewModel fileItemVm)
        {
            _eventAggregator.GetEvent<SetMediaFileRlEvent>().Publish("");

            if (!fileItemVm.IsDirectory)
            {
                SelectedFolder = Path.GetDirectoryName(fileItemVm.FullPath);
                //Get file extension
                var ext = Path.GetExtension(fileItemVm.FullPath);

                //Show the bezel editor if its a bezel file
                if (fileItemVm.DisplayName.ToLower().Contains("bezel") && ext.ToLower() == ".png")
                {
                    if (RlStaticMethods.GetMediaFormatFromFile(fileItemVm.FullPath) == "image")
                    {
                        _eventAggregator.GetEvent<NavigateMediaPaneRequestEvent>().Publish("BezelEditView");
                    }

                    _eventAggregator.GetEvent<SetBezelImagesEvent>().Publish(fileItemVm.FullPath);
                }
                else
                {
                    _eventAggregator.GetEvent<NavigateMediaPaneRequestEvent>().Publish("MediaPaneView");

                    _eventAggregator.GetEvent<SetMediaFileRlEvent>().Publish(fileItemVm.FullPath);
                }
            }
            else
                SelectedFolder = fileItemVm.FullPath;
        }

        /// <summary>
        /// Called when [update files].
        /// </summary>
        /// <param name="HeaderAndGame">The header and game.</param>
        private void OnUpdateFiles(string[] HeaderAndGame)
        {
            SetSelectedClearFiles(HeaderAndGame);
            var sys = _selectedService.CurrentSystem;
            _selectedService.CurrentRomname = HeaderAndGame[1];
            MediaType = HeaderAndGame[0];
            try
            {
                UpdateRlFolders(sys,HeaderAndGame[1], HeaderAndGame[0]);

            }
            catch (Exception ex)
            {
                _eventAggregator.GetEvent<ErrorMessageEvent>().Publish(ex.Message);
            }

            ShowMediaPaneSelectedFile();
        }

        /// <summary>
        /// Updates the files header
        /// </summary>
        /// <param name="romname">The romname.</param>
        /// <param name="mediatype">The mediatype.</param>
        private void UpdateHeader(string romname, string mediatype) => FilesHeader = $"{mediatype} Files for: {romname}";

        private void UpdateRlFolders(string systemName, string romname, string mediatype)
        {
            SelectedFolder = RlStaticMethods.GetSelectedPath(_settings.HypermintSettings.RlMediaPath, mediatype, systemName, romname);

            if (!Directory.Exists(SelectedFolder)) return;

            var rlRootFolderItem = new RlFileItemViewModel(SelectedFolder, true);            

            var dirs = Directory.EnumerateDirectories(SelectedFolder,"*",SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {
                var rlFolderItem = new RlFileItemViewModel(dir, true);

                var files = Directory.EnumerateFiles(dir);
                foreach (var file in files)
                {
                    rlFolderItem.Children.Add(new RlFileItemViewModel(file));
                }

                rlRootFolderItem.Children.Add(rlFolderItem);
            }

            var rootFiles = Directory.EnumerateFiles(SelectedFolder);
            foreach (var file in rootFiles)
            {
                rlRootFolderItem.Children.Add(new RlFileItemViewModel(file));
            }

            RlFiles.Add(rlRootFolderItem);

        }
        #endregion
    }
}
