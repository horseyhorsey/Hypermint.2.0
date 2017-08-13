using Hypermint.Base.Base;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hs.Hypermint.IntroVideos.ViewModels
{
    public class AviSynthOverlayViewModel : ViewModelBase
    {
        #region Properties
        private int wheelPosX = 5;
        public int WheelPosX
        {
            get { return wheelPosX; }
            set { SetProperty(ref wheelPosX, value); }
        }

        private int wheelPosY = 420;
        public int WheelPosY
        {
            get { return wheelPosY; }
            set { SetProperty(ref wheelPosY, value); }
        }

        private int resizeWidth = 200;
        public int ResizeWidth
        {
            get { return resizeWidth; }
            set { SetProperty(ref resizeWidth, value); }
        }

        private int resizeHeight = 50;
        public int ResizeHeight
        {
            get { return resizeHeight; }
            set { SetProperty(ref resizeHeight, value); }
        }

        private bool overlayOn = false;
        public bool OverlayOn
        {
            get { return overlayOn; }
            set { SetProperty(ref overlayOn, value); }
        }

        private bool resizeOn = false;
        public bool ResizeOn
        {
            get { return resizeOn; }
            set { SetProperty(ref resizeOn, value); }
        }

        #endregion

        public AviSynthOverlayViewModel()
        {

        }
    }
}
