using Hs.Hypermint.ImageEdit.Preset;
using Hs.Hypermint.ImageEdit.Repo;
using Hs.Hypermint.ImageEdit.Service;
using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Converters;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;

namespace Hs.Hypermint.ImageEdit.ViewModels
{
    public class CreateImageViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;

        public DelegateCommand GeneratePreviewCommand { get; private set; }
        public DelegateCommand SaveImageCommand { get; private set; } 

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

        private ISettingsRepo _settings;
        private ISelectedService _selected;

        public CreateImageViewModel(IEventAggregator eventAggregator,
            ISettingsRepo settings, ISelectedService selected)
        {
            _settings = settings;
            _selected = selected;

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

            MediaExportTypes = new ListCollectionView(Enum.GetNames(typeof(ImageExportType)));

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

                CreateImage(outputFileName,CurrentSetting.Png);

            });

        }

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

            var newCellFolder = "";
            var outPutFileName = "";

            switch (layerName)
            {
                case "Background":
                    newCellFolder = "Backgrounds";
                    outPutFileName = "Background - " + Ratio + " (" + Author + ")";
                    break;
                case "HsBackground":
                    newCellFolder = "Backgrounds";
                    outPutFileName = "RomName";
                    break;
                case "BezelBackground":
                    newCellFolder = "Bezels";
                    outPutFileName = "Background - " + Ratio + " (" + Author + ")";
                    break;
                case "Layer1":
                    newCellFolder = "Fade";
                    outPutFileName = "Layer 1 - " + Ratio + " " + CurrentSetting.Description + " (" + Author + ")";
                    break;
                case "Layer2":
                    newCellFolder = "Fade";
                    outPutFileName = "Layer 2 - " + Ratio + " " + CurrentSetting.Description + " (" + Author + ")";
                    break;
                case "Layer3":
                    newCellFolder = "Fade";
                    outPutFileName = "Layer 3 - " + Ratio + " " + CurrentSetting.Description + " (" + Author + ")";
                    break;
                case "LayerExtra":
                    newCellFolder = "Fade";
                    outPutFileName = "Extra Layer 1 - " + Ratio + " " + CurrentSetting.Description + " (" + Author + ")";
                    break;
                default:
                    break;
            }

            string rlMediaFilePath = 
                Path.Combine(
                _settings.HypermintSettings.RlMediaPath,
                newCellFolder, _selected.CurrentSystem,
                _selected.CurrentRomname);

            return GetNewFileNameIfExists(layerName, outPutFileName, rlMediaFilePath);
        }

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

            IImageEditService imgService = new ImageEditRepo();

            imgService.ConvertImageUsingPreset(CurrentSetting, currentImageFileSource, newPath, isPng);

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

            IImageEditService imgService = new ImageEditRepo();

            imgService.ConvertImageUsingPreset(CurrentSetting, currentImageFileSource, outputFileName + newExt, isPng);

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
    }

        public enum ImageExportType
        {
            Layer1,
            Layer2,
            Layer3,
            LayerExtra,
            Background,
            BezelBackground,
            HsBackground
        }
}
