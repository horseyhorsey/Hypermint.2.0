using Hypermint.Base;
using Hypermint.Base.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Runtime.CompilerServices;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class YoutubeViewModel : ViewModelBase
    {
        #region Services
        private ICollectionView videoList;
        private ISearchYoutube _youtube;
        private IEventAggregator _evtAggr;
        private ISelectedService _selectedService;
        #endregion

        #region Properties
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

        private bool isSearching;
        public bool IsSearching
        {
            get { return isSearching; }
            set { SetProperty(ref isSearching, value); }
        }

        private bool videosVisible;
        public bool VideosVisible
        {
            get { return videosVisible; }
            set { SetProperty(ref videosVisible, value); }
        }

        private string youtubeViewHeader = "YT Search: ";
        public string YoutubeViewHeader
        {
            get { return youtubeViewHeader; }
            set { SetProperty(ref youtubeViewHeader, value); }
        }

        private bool includeSystem = true;
        public bool IncludeSystem
        {
            get { return includeSystem; }
            set { SetProperty(ref includeSystem, value); }
        }

        private bool includeDescription = true;
        public bool IncludeDescription
        {
            get { return includeDescription; }
            set
            {
                SetProperty(ref includeDescription, value);
            }
        }

        private bool includeRomname = false;
        public bool IncludeRomname
        {
            get { return includeRomname; }
            set { SetProperty(ref includeRomname, value); }
        }

        private string searchTermText;
        public string SearchTermText
        {
            get { return searchTermText; }
            set { SetProperty(ref searchTermText, value); }
        }

        private CancellationTokenSource cancelSource;
        #endregion

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case "IncludeDescription":
                case "IncludeRomname":
                case "IncludeSystem":
                    BuildSearchTerm();
                    break;
                default:
                    break;
            }
        }

        public DelegateCommand OpenYtCommand { get; private set; }
        public DelegateCommand AuditViewCommand { get; private set; }
        public DelegateCommand SearchYtCommand { get; private set; }
        public DelegateCommand CancelSearchCommand { get; private set; } 

        public YoutubeViewModel(ISearchYoutube youtube, 
            IEventAggregator ea, ISelectedService selectedService)
        {
            _youtube = youtube;
            _evtAggr = ea;
            _selectedService = selectedService;

            _evtAggr.GetEvent<GetVideosEvent>().Subscribe(async (x) =>
            {

                IsSearching = true;
                VideosVisible = false;

                SearchTermText = BuildSearchTerm();

                await BuildVideoList(x);

            });

            OpenYtCommand = new DelegateCommand(() =>
            {
                var vid = VideoList.CurrentItem as YoutubeVideo;

                if (vid != null)
                {
                    try
                    {
                        Process.Start(vid.VideoUrl);
                    }
                    catch (Exception) { }
                    
                }

            });

            AuditViewCommand = new DelegateCommand(() =>
            {
                _evtAggr.GetEvent<NavigateRequestEvent>().Publish("HsMediaAuditView");
            });

            SearchYtCommand = new DelegateCommand(async () =>
            {
                IsSearching = true;
                VideosVisible = false;

                  await BuildVideoList(new object());

            });

            CancelSearchCommand = new DelegateCommand(() => {

                cancelSource.Cancel();
            });
        }

        private string BuildSearchTerm()
        {
            var searchTerm = "";
            var system = _selectedService.CurrentSystem;
            var rom = _selectedService.CurrentRomname;

            if (IncludeSystem)
            {
                if (isMainMenu(system))
                    searchTerm = rom + " ";
                else
                    searchTerm = system + " ";
            }

            if (IncludeRomname)
                if (!isMainMenu(system))
                    searchTerm += rom + " ";

            if (IncludeDescription)
                if (!isMainMenu(system))
                    searchTerm += _selectedService.CurrentDescription + " ";

            SearchTermText = searchTerm;            

            return searchTerm.TrimEnd(' ').Replace(' ', '+');
        }

        private bool isMainMenu(string system) => system.ToLower().Contains("main menu");

        private async Task BuildVideoList(object x)
        {
            cancelSource = new CancellationTokenSource();

            YoutubeViewHeader = "YT Search: " + SearchTermText;

            await Task.Run(async () =>
            {                
                var links = await _youtube.SearchAsync(SearchTermText);

                var ytVids = new List<YoutubeVideo>();

                try
                {
                    for (int i = 0; i < links.Count; i++)
                    {
                        if (!cancelSource.IsCancellationRequested)
                            ytVids.Add(new YoutubeVideo(links[i]));
                        else
                            break;
                    }
                    
                }
                catch (Exception)
                {
                    
                }
                finally
                {
                    IsSearching = false;
                    VideosVisible = true;

                    if (!cancelSource.IsCancellationRequested)
                        VideoList = new ListCollectionView(ytVids);
                }

            }, cancelSource.Token);

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

            SetVideoTitle();
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

        public void SetVideoTitle()
        {
            
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
