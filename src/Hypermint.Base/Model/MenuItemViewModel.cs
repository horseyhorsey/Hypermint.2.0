using Frontends.Models.Hyperspin;

namespace Hypermint.Base.Model
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class MenuItemViewModel
    {
        public MainMenu MainMenu { get; }

        public MenuItemViewModel(MainMenu mainMenu)
        {
            MainMenu = mainMenu;
        }
        
        public int Enabled { get { return MainMenu.Enabled; } set { MainMenu.Enabled = value; } }

        [PropertyChanged.DoNotNotify]
        public System.Uri SysIcon { get { return MainMenu.SysIcon; } }

        [PropertyChanged.DoNotNotify]
        public string Name { get { return MainMenu.Name; } set { MainMenu.Name = value; } }
    }
}
