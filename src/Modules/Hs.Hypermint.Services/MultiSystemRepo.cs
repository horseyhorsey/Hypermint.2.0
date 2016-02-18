using Hypermint.Base.Interfaces;
using Hs.HyperSpin.Database;

namespace Hs.Hypermint.Services
{
    public class MultiSystemRepo : IMultiSystemRepo
    {
        public Games MultiSystemList { get; set; }
    }
}
