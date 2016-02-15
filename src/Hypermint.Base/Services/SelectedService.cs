using System;
using System.Windows.Media.Imaging;

namespace Hypermint.Base.Services
{
    public class SelectedService : ISelectedService
    {
        public string CurrentSystem { get; set; }

        public BitmapImage SystemImage { get; set; }

        public bool IsMainMenu()
        {
            if (CurrentSystem.Contains("Main Menu"))
                return true;
            else
                return false;
        }
    }
}
