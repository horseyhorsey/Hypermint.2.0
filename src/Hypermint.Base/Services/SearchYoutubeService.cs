using Hypermint.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hypermint.Base.Services
{
    public class SearchYoutubeService : ISearchYoutube
    {
        const string YOUTUBELINK = @"https://www.google.com/?q=site+youtube.com+amstrad";


        public async Task<List<string>> SearchAsync(string searchTerm) => await GoogleSearch(searchTerm);

        private  async Task<List<string>>  GoogleSearch(string searchTerm)
        {
            // ... Target page.
            string page = "https://www.google.co.uk/search?q=sites:youtube.com";

            var videoLinks = new List<string>();

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page + "+" + searchTerm))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();

                var pattern = @"https://www.youtube.com/watch?....................";

                foreach (Match item in Regex.Matches(result, pattern))
                {
                    var newItem = item.Value.Replace("%3F", "?").Replace("%3D", "=");

                    if (!videoLinks.Contains(newItem))
                        videoLinks.Add(newItem);
                }
            }

            return videoLinks;
        }

    private static void start_get()
        {
            //Our getVars, to test the get of our php. 
            //We can get a page without any of these vars too though.
            
         

            //Congratulations, with these two functions in basic form, you just learned
            //the two basic forms of web surfing
            //This proves how easy it can be.
        }

        public IEnumerable<string> Search(string searchTerm, string systemName)
        {
            return new string[2];
        }
    }
}