﻿using System.Windows.Media.Imaging;

namespace Hypermint.Base.Services
{
    public interface ISelectedService
    {
        string CurrentSystem { get; set; }

        bool IsMainMenu();

        BitmapImage SystemImage { get; set; }
    }
}
