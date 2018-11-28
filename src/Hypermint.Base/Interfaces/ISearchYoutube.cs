using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hypermint.Base.Interfaces
{
    public interface ISearchYoutube
    {
        Task<List<string>> SearchAsync(string searchTerm);

        /// <summary>
        /// youtube-dl.exe
        /// </summary>
        /// <returns></returns>
        Task YtDownload(string url, string outputPath, Action<string> outputCallback, CancellationToken token);
    }
}
