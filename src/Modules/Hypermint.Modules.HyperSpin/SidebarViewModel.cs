using Hypermint.Base.Base;

namespace Hypermint.Modules.HyperSpin
{
    public class SidebarViewModel : ViewModelBase
    {
        private int myVar;

        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }

        public SidebarViewModel()
        {

        }
    }
}
