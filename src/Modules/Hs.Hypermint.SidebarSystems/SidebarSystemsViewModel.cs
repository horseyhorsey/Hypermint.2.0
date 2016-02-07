﻿using Hypermint.Base.Base;
using System.Collections.ObjectModel;
using Hypermint.Base.Interfaces;

namespace Hs.Hypermint.SidebarSystems
{
    public class SidebarSystemsViewModel : ViewModelBase
    {
        #region Properties
        private string _systemWheelImage;
        public string SystemWheelImage
        {
            get { return _systemWheelImage; }
            set { SetProperty(ref _systemWheelImage, value); }
        }

        private string _systemTitleCount;
        public string SystemTitleCount
        {
            get { return "Systems: "; }
            set { SetProperty(ref _systemTitleCount, value); }
        }

        public ObservableCollection<string> MainMenuDatabases { get; set; }

        IMainMenuRepo _mainMenuRepo;
        
        #endregion

        public SidebarSystemsViewModel( IMainMenuRepo main)
        {
            _mainMenuRepo = main;
            

            MainMenuDatabases = new ObservableCollection<string>();
            foreach (var item in _mainMenuRepo.GetMainMenuDatabases(@"I:\HyperSpin\Databases\Main Menu\"))
            {
                MainMenuDatabases.Add(item);
            }
        }


    }
}
