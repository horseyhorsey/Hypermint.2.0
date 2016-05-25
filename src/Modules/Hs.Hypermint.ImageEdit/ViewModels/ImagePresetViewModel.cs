using System;
using Hypermint.Base.Base;
using Prism.Commands;

namespace Hs.Hypermint.ImageEdit.ViewModels
{
    public class ImagePresetViewModel : ViewModelBase
    {
        public DelegateCommand SavePresetCommand { get; private set; } 

        public ImagePresetViewModel()
        {
            SavePresetCommand = new DelegateCommand(SavePreset);
        }

        private void SavePreset()
        {
            throw new NotImplementedException();
        }
    }
}
