using Hs.Hypermint.Services.Helpers;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Converters;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Models;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;

namespace Hs.Hypermint.ImageEdit.ViewModels
{
    public class CreateImageViewModel : ViewModelBase
    {
        #region Constructors
        public CreateImageViewModel(IEventAggregator eventAggregator,
            ISettingsRepo settings, ISelectedService selected, IImageEditService imageEdit)
        {
            _settings = settings;
            _selected = selected;
            _imageEdit = imageEdit;

            Author = _settings.HypermintSettings.Author;

            CurrentSetting = new ImageEditPreset
            {
                Width = 1920,
                Height = 1080,
                Png = true,
                Description = "desc",
                FlipL = true,
                FlipOn = true,
                ResizeImage = true,
                StretchImage = true,
                TileEnabled = true,
                TileWidth = 200,
                TileHeight = 150
            };

            GeneratePreviewCommand = new DelegateCommand(GeneratePreview);

            MediaExportTypes = new ListCollectionView(new List<string>()
            {
                "Layer 1",
                "Layer 2",
                "Layer 3",
                "Extra Layer 1",
                "BezelBg",
                "Background",
            });

            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<ImageEditSourceEvent>().Subscribe(x =>
            {
                currentImageFileSource = x;

                ImageEditSource = ConvertToImageSource.ImageSourceFromUri(new Uri(currentImageFileSource));

                UpdateImagePreviewHeader();

            });

            //Return the current UI as a preset
            _eventAggregator.GetEvent<ImagePresetRequestEvent>().Subscribe((x) =>
            {
                _eventAggregator.GetEvent<ImagePresetRequestedEvent>().Publish(CurrentSetting);
            });

            _eventAggregator.GetEvent<ImagePresetUpdateUiEvent>().Subscribe(x =>
            {
                SetUIValuesFromPreset((ImageEditPreset)x);
            });

            SaveImageCommand = new DelegateCommand(() =>
            {
                var outputFileName = CreateNewImageFileName();

                CreateImage(outputFileName, CurrentSetting.Png);
            });

        }
        #endregion

        #region Commands
        public DelegateCommand GeneratePreviewCommand { get; private set; }
        public DelegateCommand SaveImageCommand { get; private set; } 
        #endregion

        #region Properties
        private ImageSource imageEditSource;
        public ImageSource ImageEditSource
        {
            get { return imageEditSource; }
            set { SetProperty(ref imageEditSource, value); }
        }

        private ImageEditPreset currentSetting;
        public ImageEditPreset CurrentSetting
        {
            get { return currentSetting; }
            set { SetProperty(ref currentSetting, value); }
        }

        private string imagePreviewHeader = "Image Preview";
        public string ImagePreviewHeader
        {
            get { return imagePreviewHeader; }
            set { SetProperty(ref imagePreviewHeader, value); }
        }

        private ICollectionView mediaExportTypes;
        public ICollectionView MediaExportTypes
        {
            get { return mediaExportTypes; }
            set { SetProperty(ref mediaExportTypes, value); }
        }

        private string author;
        public string Author
        {
            get { return author; }
            set { SetProperty(ref author, value); }
        }

        private string ratio;
        public string Ratio
        {
            get { return ratio; }
            set { SetProperty(ref ratio, value); }
        }

        public string currentImageFileSource { get; set; }
        #endregion

        #region Fields
        private ISettingsRepo _settings;
        private ISelectedService _selected;
        private IImageEditService _imageEdit;
        private IEventAggregator _eventAggregator; 
        #endregion

        #region Support Methods
        private void UpdateImagePreviewHeader()
        {
            ImagePreviewHeader =
            "Image Preview:" + " Source w:" +
            Math.Round(ImageEditSource.Width) +
            " h:" + Math.Round(ImageEditSource.Height);
        }

        private void GeneratePreview()
        {
            try
            {
                var imagePath = Path.GetFullPath(CreateImagePreview(CurrentSetting.Png));

                ImageEditSource =
                    ConvertToImageSource.ImageSourceFromUri(new Uri(imagePath));

                UpdateImagePreviewHeader();
            }
            catch (Exception ex)
            {

            }

        }

        private void SetUIValuesFromPreset(ImageEditPreset setting)
        {
            CurrentSetting = setting;
        }

        private string CreateNewImageFileName()
        {
            var layerName = MediaExportTypes.CurrentItem as string;

            var outPutFileName = "";

            var parent = RlStaticMethods.GetParentMediaType(layerName);

            string rlMediaFilePath =
                Path.Combine(
                _settings.HypermintSettings.RlMediaPath,
                parent, _selected.CurrentSystem,
                _selected.CurrentRomname);

            outPutFileName = RlStaticMethods.CreateFileNameForRlImage(layerName, "", CurrentSetting.Description, "");

            return GetNewFileNameIfExists(layerName, outPutFileName, rlMediaFilePath);
        }

        private string GetNewFileNameIfExists(string layerName, string outPutFileName, string rlMediaFilePath)
        {
            #region FileChecking
            bool flag = true;
            int c = 1;
            while (flag)
            {
                if (!layerName.Contains("HsBackground"))
                {
                    if (File.Exists(rlMediaFilePath + "\\" + outPutFileName + ".png") || File.Exists(rlMediaFilePath + "\\" + outPutFileName + ".jpg"))
                    {
                        if (layerName.Contains("Layer"))
                        {
                            outPutFileName = layerName + " - " + Ratio + " " + CurrentSetting.Description + "(" + c + ") " + "(" + Author + ")";
                        }
                        else
                            outPutFileName = layerName + CurrentSetting.Description + "(" + c + ") " + "(" + Author + ")";

                        if (layerName == "Background" || layerName == "BezelBackground")
                        {
                            outPutFileName = "Background - " + Ratio + " (" + c + ") " + "(" + Author + ")";
                        }

                        c++;
                    }
                    else
                        flag = false;
                }
                else
                    flag = false;
            }
            #endregion
            return rlMediaFilePath + "\\" + outPutFileName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create image 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string CreateImagePreview(bool isPng = false)
        {
            var newExt = ".jpg";
            if (isPng)
                newExt = ".png";

            string newPath = "Preview" + newExt;

            _imageEdit.ConvertImageUsingPreset(CurrentSetting, currentImageFileSource, newPath, isPng);

            return newPath;

        }
        public void CreateImage(string outputFileName, bool isPng)
        {
            var newExt = ".jpg";
            if (isPng)
                newExt = ".png";

            var path = Path.GetDirectoryName(outputFileName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            _imageEdit.ConvertImageUsingPreset(CurrentSetting, currentImageFileSource, outputFileName + newExt, isPng);

        }
    } 
    #endregion

}
