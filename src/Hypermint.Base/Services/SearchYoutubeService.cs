using Hypermint.Base.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeInfo.Models;

namespace Hypermint.Base.Services
{
    public class SearchYoutubeService : ISearchYoutube
    {        
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
        
        public List<string> GetYoutubeMp4s(string youtubeUrl)
        {
            var info = new YoutubeInfo.YoutubeInfo();

            var mp4s = new List<string>();

            foreach (var mp4 in info.GetMp4Videos(youtubeUrl))
            {                
                mp4s.Add(mp4.DownloadUrl);
            }

            return mp4s;
            
        }

        public IEnumerable<string> Search(string searchTerm, string systemName)
        {
            return new string[2];
        }
    }
}