using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System.ComponentModel;
using System.Windows.Input;
using Hs.Hypermint.Services.Helpers;
using System.IO;
using System;
using System.Windows.Data;
using Frontends.Models.RocketLauncher;
using Hypermint.Base;

namespace Hs.Hypermint.FilesViewer.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class DroppedFilesViewModel : ViewModelBase
    {
        #region Fields
        private bool cancelPending = false;
        private CustomDialog _customDialog; 
        #endregion

        #region Commands
        public ICommand CloseAllPendingFileDropCommand { get; set; }
        public ICommand SaveNewFileCommand { get; private set; }
        public ICommand CloseDialogCommand { get; set; }
        #endregion

        public DroppedFilesViewModel(string mediaType,string file, IDialogCoordinator dialogService, CustomDialog customDialog)
        {
            MediaType = mediaType;
            _customDialog = customDialog;

            CloseAllPendingFileDropCommand = new DelegateCommand(async () =>
            {
                await CloseDialog(dialogService, _customDialog);
            });

            CloseDialogCommand = new DelegateCommand(async () =>
            {
                cancelPending = true;

                await dialogService.HideMetroDialogAsync(this, _customDialog);

                cancelPending = false;                

            });

            SaveNewFileCommand = new DelegateCommand(async () =>
            {
                try
                {                    
                    ProcessFile(DroppedFileName, SelectedFolder, FileNameToSave);
                }
                catch (Exception) { }

                await dialogService.HideMetroDialogAsync(this, _customDialog);
            });

            CardPositionsArray = new ListCollectionView(Enum.GetNames(typeof(CardPosition)));
            CardPositionsArray.MoveCurrentToFirst();
            CardPositionsArray.CurrentChanged += CardPositionsArray_CurrentChanged;

            Load(file);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
                        
        }

        #region Properties
        public ICollectionView CardPositionsArray { get; set; }
        public string Author { get; set; }
        public bool CardsEnabled { get; set; }
        public bool ConvertEnabled { get; set; }
        public string Description { get; set; }
        public string DroppedFileName { get; set; }
        public bool FileNameOptionsOff { get; set; }
        public bool FileNameOptionsOn { get; set; }
        public string FileNameToSave { get; set; }
        public bool FormatJpg { get; set; }
        public bool ImageConvertEnabled { get; set; }
        public bool IsFadeOptions { get; set; }
        public string MediaType { get; set; }
        public string OutputFileName { get; set; }
        public string Ratio { get; set; }
        public string SelectedFolder { get; internal set; }
        #endregion

        #region Public Methods
        public void SetCustomDialog(CustomDialog customDialog) => _customDialog = customDialog;
        #endregion

        #region Support Methods

        private void CardPositionsArray_CurrentChanged(object sender, EventArgs e)
        {
            if (MediaType != "Cards") return;

            if (CardPositionsArray != null)
                FileNameToSave =
                    RlStaticMethods
                    .CreateCardFileName(Description, Author,
                    (string)CardPositionsArray.CurrentItem);
        }

        private string ChangeImageExtension(string ext)
        {
            if (ImageConvertEnabled)
            {
                if (FormatJpg)
                    ext = ".jpg";
                else
                    ext = ".png";
            }

            return ext;
        }

        /// <summary>
        /// Closes the dialog window
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="customDialog">The custom dialog.</param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task CloseDialog(IDialogCoordinator dialogService, CustomDialog customDialog)
        {
            cancelPending = true;

            await dialogService.HideMetroDialogAsync(this, customDialog);

            cancelPending = false;
        }

        private void Load(string file)
        {
            if (RlStaticMethods.GetMediaFormatFromFile(file) == "image")
                ConvertEnabled = true;
            else
                ConvertEnabled = false;

            FileNameToSave = Path.GetFileNameWithoutExtension(file);

            var parentType = RlStaticMethods.GetParentMediaType(MediaType);

            CardsEnabled = false;

            if (parentType == "Fade" || parentType == "Bezels" || parentType == "Backgrounds")
            {
                IsFadeOptions = true;
                FileNameOptionsOn = true;
                FileNameOptionsOff = false;

                if (MediaType != "Cards")
                {
                    FileNameToSave =
                        RlStaticMethods.CreateFileNameForRlImage(MediaType, Ratio, Description, Author);
                }
                else
                {
                    CardsEnabled = true;

                    FileNameToSave =
                        RlStaticMethods
                        .CreateCardFileName(Description, Author,
                        (string)CardPositionsArray.CurrentItem);
                }
            }
            else
            {
                FileNameOptionsOn = false;
                FileNameOptionsOff = true;

                IsFadeOptions = false;
            }

            DroppedFileName = file;
        }

        public void ProcessFile(string file, string pathToCopy, string newFileName)
        {
            int i = 1;
            string path;

            var originalExt = Path.GetExtension(file);

            if (!Directory.Exists(pathToCopy))
                Directory.CreateDirectory(pathToCopy);

            var ext = ChangeImageExtension(originalExt);

            path = pathToCopy + "\\" + newFileName + ext;

            while (File.Exists(path))
            {
                path = pathToCopy + "\\" + newFileName + " (" + i + ")" + ext;
                i++;
            }

            //TODO
            //if (ImageConvertEnabled && ext != originalExt)
            //    _imageEdit.ConvertImageFormat(file, path, FormatJpg);
            //else
            File.Copy(file, path);
        }
        #endregion

    }
}
