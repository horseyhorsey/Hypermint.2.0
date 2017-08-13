using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hypermint.Base.Interfaces
{
    public interface ISearchYoutube
    {
        Task<List<string>> SearchAsync(string searchTerm);

        IEnumerable<string> Search(string searchTerm, string systemName);

        List<string> GetYoutubeMp4s(string youtubeUrl);        
    }
}
