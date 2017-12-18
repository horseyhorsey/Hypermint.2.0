using Hypermint.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Hypermint.Base.Services
{
    public class SearchYoutubeService : ISearchYoutube
    {
        private Action<string> _outputCallback;

        public async Task<List<string>> SearchAsync(string searchTerm) => await GoogleSearch(searchTerm);

        private async Task<List<string>> GoogleSearch(string searchTerm)
        {
            // ... Target page.
            string page = "https://www.google.co.uk/search?q=sites:youtube.com";            

            //VideoThumb
            // https://img.youtube.com/vi/HoZ_Jq5LN7Q/maxresdefault.jpg

            var videoLinks = new List<string>();

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page + "+" + searchTerm))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();

                var pattern = @"https://www.youtube.com/watch?..................";

                foreach (Match item in Regex.Matches(result, pattern))
                {
                    var newItem = item.Value.Replace("%3F", "?").Replace("%3D", "=");

                    if (!videoLinks.Contains(newItem))
                        videoLinks.Add(newItem);
                }
            }

            return videoLinks;
        }

        CancellationToken _token;
        public async Task YtDownload(string url, string outputPath, Action<string> outputCallback, CancellationToken token)
        {
            _token = token;
            _outputCallback = outputCallback;
            await Task.Run(() =>
             {
                 ProcessStartInfo si = new ProcessStartInfo(Environment.CurrentDirectory + "\\youtube-dl.exe");
                 Process p = new Process();
                 p.EnableRaisingEvents = true;

                 p.StartInfo = si;

                 si.Arguments = " -f mp4 -o " + "\"" + outputPath + "\"" + " " + url;
                 si.RedirectStandardOutput = true;
                 si.UseShellExecute = false;
                 si.CreateNoWindow = true;
                 p.OutputDataReceived += P_OutputDataReceived;
                 p.Exited += P_Exited;
                 p.Start();

                 // To avoid deadlocks, always read the output stream first and then wait.
                 p.BeginOutputReadLine();
                 p.WaitForExit();
                 
             }, _token).ContinueWith(x => x.IsCanceled);
        }

        private void P_Exited(object sender, EventArgs e)
        {
            var cancelled = _token.IsCancellationRequested;
        }

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                _outputCallback.Invoke(e.Data);
        }
    }
}