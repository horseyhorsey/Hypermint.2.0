using Frontends.Models.Hyperspin;
using Hypermint.Base.Interfaces;

namespace Hs.Hypermint.Services
{
    public class MultiSystemRepo : IMultiSystemRepo
    {
        public Games MultiSystemList { get; set; }
    }
}
