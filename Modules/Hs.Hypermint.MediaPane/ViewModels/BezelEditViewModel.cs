using Hs.Hypermint.Services.Helpers;
using Hypermint.Base;
using Hypermint.Base.Base;
using Prism.Commands;
using Prism.Events;
using System;

namespace Hs.Hypermint.MediaPane.ViewModels
{
    public class BezelEditViewModel : ViewModelBase
    {        
        #region Constructors
        public BezelEditViewModel(IEventAggregator ea)
        {
            _eventAgg = ea;

            _eventAgg.GetEvent<SetBezelImagesEvent>().Subscribe(imageFile =>
            {
                BezelImage = imageFile;
                BezelHeader = BezelImage;

                if (string.IsNullOrEmpty(imageFile))
                {
                    BezelImage = null;
                    return;
                }

                try
                {
                    var bezelValues =
                        RlStaticMethods.LoadBezelIniValues(PngToIni(BezelImage));

                    LeftClickX = Convert.ToDouble(bezelValues[0]);
                    LeftClickY = Convert.ToDouble(bezelValues[1]);
                    RightClickX = Convert.ToDouble(bezelValues[2]);
                    RightClickY = Convert.ToDouble(bezelValues[3]);
                }
                catch (Exception)
                {

                }


            });

            PreviewMouseDownCommand = new DelegateCommand(() =>
            {

            });

            SaveBezelIniCommand = new DelegateCommand(() =>
            {

                RlStaticMethods.SaveBezelIni(
                new double[]
                {
                    LeftClickX, LeftClickY, RightClickX, RightClickY
                }, PngToIni(BezelImage));

            });
        }
        #endregion

        #region Properties
        private string bezelHeader;
        public string BezelHeader
        {
            get { return bezelHeader; }
            set { SetProperty(ref bezelHeader, value); }
        }
        public string BezelImage
        {
            get { return bezelImage; }
            set { SetProperty(ref bezelImage, value); }
        }
        private double leftClickX = 0;
        public double LeftClickX
        {
            get { return leftClickX; }
            set { SetProperty(ref leftClickX, value); }
        }
        private double leftClickY = 0;
        public double LeftClickY
        {
            get { return leftClickY; }
            set { SetProperty(ref leftClickY, value); }
        }
        private double rightClickX = 0;
        public double RightClickX
        {
            get { return rightClickX; }
            set { SetProperty(ref rightClickX, value); }
        }
        private double rightClickY = 0;
        public double RightClickY
        {
            get { return rightClickY; }
            set { SetProperty(ref rightClickY, value); }
        }
        #endregion

        #region Fields
        private string bezelImage;

        private IEventAggregator _eventAgg;
        #endregion

        #region Commands
        public DelegateCommand PreviewMouseDownCommand { get; private set; }
        public DelegateCommand SaveBezelIniCommand { get; private set; }
        #endregion

        #region Support Methods
        private string PngToIni(string file) => BezelImage.Replace(".png", ".ini"); 
        #endregion
    }
}
