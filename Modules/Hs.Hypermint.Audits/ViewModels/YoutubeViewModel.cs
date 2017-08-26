﻿using Hypermint.Base;
using Hypermint.Base.Events;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class YoutubeViewModel : ViewModelBase
    {
        #region Fields
        private ICollectionView videoList;
        private ISearchYoutube _youtube;
        private IEventAggregator _evtAggr;
        private ISelectedService _selectedService;
        private ISettingsHypermint _settings;
        private IDialogCoordinator _dialog;
        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken token;
        ProgressDialogController pdc = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="YoutubeViewModel"/> class.
        /// </summary>
        /// <param name="youtube">The youtube.</param>
        /// <param name="ea">The ea.</param>
        /// <param name="selectedService">The selected service.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="dialog">The dialog.</param>
        public YoutubeViewModel(ISearchYoutube youtube,
            IEventAggregator ea, ISelectedService selectedService, ISettingsHypermint settings, IDialogCoordinator dialog)
        {
            _youtube = youtube;
            _evtAggr = ea;
            _selectedService = selectedService;
            _settings = settings;
            _dialog = dialog;            

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

            CancelSearchCommand = new DelegateCommand(() =>
            {

                cancelSource.Cancel();
            });

            DownloadVideoCommand = new DelegateCommand(async () =>
            {
                if (File.Exists(Environment.CurrentDirectory + "\\youtube-dl.exe"))
                {
                    System.Windows.MessageBox.Show("Cannot find youtube-dl.exe in hypermint root");
                    return;
                }
                    
                await DownloadVideoAsync();
            });
        }

        /// <summary>
        /// Downloads the video asynchronous.
        /// </summary>
        /// <returns></returns>
        private async Task DownloadVideoAsync()
        {            
            //Build the video path to Hs
            var sys = _selectedService.CurrentSystem;
            var rom = _selectedService.CurrentRomname;
            var path = Path.Combine(_settings.HypermintSettings.HsPath, "Media", sys, "Video", rom + ".mp4");

            //Get a new incremented filename if already exists.
            var outputpath = VideoHelper.CreateIncrementalFileName(path);;

            token = source.Token;
            pdc = await _dialog.ShowProgressAsync(this, "", "", true, new MetroDialogSettings()
            {
                CancellationToken = token,                
            });
            pdc.SetCancelable(true);
            pdc.Canceled += Pdc_Canceled;

            await _youtube.YtDownload(SelectedVideo.VideoUrl, outputpath, Callback, token);

            await pdc.CloseAsync();
        }

        private void Pdc_Canceled(object sender, EventArgs e)
        {
            //source.Cancel();

            var p = Process.GetProcessesByName("youtube-dl");
            if (p[0] != null)
            {
                p[0].Kill();
            }
        }

        private void Callback(string arg)
        {
            if (pdc != null)
                pdc.SetMessage(arg);
        }

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

        private YoutubeVideo _selectedVideo;
        public YoutubeVideo SelectedVideo
        {
            get { return _selectedVideo; }
            set { SetProperty(ref _selectedVideo, value); }
        }

        private CancellationTokenSource cancelSource;
        #endregion

        #region Commands
        public DelegateCommand OpenYtCommand { get; private set; }
        public DelegateCommand AuditViewCommand { get; private set; }
        public DelegateCommand SearchYtCommand { get; private set; }
        public DelegateCommand CancelSearchCommand { get; private set; }
        public ICommand DownloadVideoCommand { get; set; }

        #endregion

        #region Support Methods
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

            if (!string.IsNullOrWhiteSpace(rom))
                IncludeRomname = true;

            if (IncludeRomname)
            {
                if (!isMainMenu(system))
                    searchTerm += rom + " ";
            }

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

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
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
        #endregion
    }

    #region Support Classes
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
            catch (WebException)
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

    #endregion
}
