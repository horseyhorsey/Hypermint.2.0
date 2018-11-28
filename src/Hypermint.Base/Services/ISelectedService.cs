using Frontends.Models.Hyperspin;
using Hypermint.Base.Model;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hypermint.Base.Services
{
    public interface ISelectedService
    {
        #region Properties
        string CurrentSystem { get; set; }
        string CurrentMainMenu { get; set; }
        string CurrentRomname { get; set; }
        string CurrentDescription { get; set; }
        bool IsMultiSystem { get; set; }
        BitmapImage SystemImage { get; set; }
        ImageSource GameImage { get; set; }
        List<GameItemViewModel> SelectedGames { get; set; }
        #endregion

        #region Methods
        bool IsMainMenu();
        #endregion
    }
}
