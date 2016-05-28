using Hypermint.Base.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class YoutubeViewModel : ViewModelBase
    {
        private ICollectionView videoList;
        private ISearchYoutube _youtube;
        private IEventAggregator _evtAggr;
        private ISelectedService _selectedService;

        private Uri video;
        public Uri Video
        {
            get { return video; }
            set { SetProperty(ref video, value); }
        }

        public ICollectionView VideoList
        {
            get { return videoList; }
            set { SetProperty(ref videoList, value); }
        }

        public YoutubeViewModel(ISearchYoutube youtube, 
            IEventAggregator ea, ISelectedService selectedService)
        {
            _youtube = youtube;
            _evtAggr = ea;
            _selectedService = selectedService;

            _evtAggr.GetEvent<GetVideosEvent>().Subscribe(BuildVideoList);
            
            //var tempList = new List<string>();
            //for (int i = 0; i < 4; i++)
            //{
            //    tempList.Add("chine" + i);
            //}

            //VideoList = new ListCollectionView(tempList);
        }

        private void BuildVideoList(object x)
        {
            Task.Run(async () =>
            {
                var searchTerm = _selectedService.CurrentSystem + "+" + _selectedService.CurrentRomname;

                var links = await _youtube.SearchAsync(searchTerm);                

                var ytVids = new List<YoutubeVideo>();

                for (int i = 0; i < links.Count; i++)
                {
                    ytVids.Add(new YoutubeVideo(links[i]));                    

                }

                VideoList = new ListCollectionView(ytVids);

            });

        }
    }

    public class YoutubeVideo
    {
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public string VideoId { get; set; }
        public List<string> Mp4s { get; set; }
        public string VideoThumb { get; set; }

        public YoutubeVideo(string videoUrl)
        {
            VideoUrl = videoUrl;

            SetVideoIdWatchUrl();

            SetThumbUrl();
        }

        public void SetVideoIdWatchUrl()
        {
            var regex = new Regex(@"=");

            var splitEquals = regex.Split(VideoUrl);

            VideoId = splitEquals[1];
        }

        public void SetThumbUrl()
        {
            var url = @"https://img.youtube.com/vi/" + VideoId;

            var maxres = url + "/maxresdefault.jpg";

            if (UrlCheck.URLExists(maxres))
                VideoThumb = maxres;
            else
            {
                var stdRes = url + "/0.jpg";
                if (UrlCheck.URLExists(stdRes))
                    VideoThumb = stdRes;
            }

        }

//        var client = new WebClient();

//                  

//            try
//            {
//                var maxres = url + "/maxresdefault.jpg";
//        client.DownloadFile(maxres, "ytThumb.jpg");
//            }
//            catch (Exception ex)
//            {                          
//            }

//            try
//            {
//                var stdRes = url + "/0.jpg";
//client.DownloadFile(stdRes, "ytThumb.jpg");

//            }
//            catch (Exception)
//            {
                
//            }

//            try
//            {
//                if (File.Exists("ytThumb.jpg"))
                   

//            }
//            catch (Exception ex)
//            {

                
//            }
        
    }

    public class UrlCheck
    {
        static public bool URLExists(string url)
        {
            bool result = false;

            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Timeout = 1200; // miliseconds
            webRequest.Method = "HEAD";

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                result = true;
            }
            catch (WebException webException)
            {
                
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return result;
        }
    }
}
