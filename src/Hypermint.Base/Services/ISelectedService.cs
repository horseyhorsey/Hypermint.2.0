using Hs.HyperSpin.Database;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hypermint.Base.Services
{
    public interface ISelectedService
    {
        string CurrentSystem { get; set; }

        string CurrentMainMenu { get; set;}

        bool IsMainMenu();

        BitmapImage SystemImage { get; set; }
        ImageSource GameImage { get; set; }

        List<Game> SelectedGames { get; set; }
    }
}
