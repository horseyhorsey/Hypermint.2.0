using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Hs.Hypermint.Browser.Views
{
    /// <summary>
    /// Interaction logic for WebBrowseView
    /// </summary>
    public partial class WebBrowseView : UserControl
    {
        public WebBrowseView()
        {
            InitializeComponent();
        }

        private void btn_ytb_downloader_Copy_Click(object sender, RoutedEventArgs e)
        {
           // DownloadVideo();
        }
        private void txtUrl_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                browser.Navigate(txtUrl.Text);
        }

        private void browser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            txtUrl.Text = e.Uri.OriginalString;
        }
        private void BrowseBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((browser != null) && (browser.CanGoBack));
        }
        private void BrowseBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            browser.GoBack();
        }
        private void BrowseForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((browser != null) && (browser.CanGoForward));
        }
        private void BrowseForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            browser.GoForward();
        }
        private void GoToPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void GoToPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!browser.IsEnabled)
                browser.IsEnabled = true;

            try
            {
                browser.Navigate(@"https://duckduckgo.com/");
            }
            catch (Exception ex)
            {

                var msg = ex.Message;
            }

        }

        void Browser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            var browser = sender as WebBrowser;

            if (browser == null || browser.Document == null)
                return;

            dynamic document = browser.Document;

            if (document.readyState != "complete")
                return;

            dynamic script = document.createElement("script");
            script.type = @"text/javascript";
            script.text = @"window.onerror = function(msg,url,line){return true;}";
            document.head.appendChild(script);
        }


        /// <summary>
        /// Button that search youtube from selected game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _buttonSearchYoutube(object sender, RoutedEventArgs e)
        {
            if (!browser.IsEnabled)
                browser.IsEnabled = true;

            browser.Navigate(@"https://youtube.com/");

            //if (SelectedGame != null)
            //    browser.Navigate(Videos.YoutubeSearch(SelectedGame, hsdb.SystemName));
        }

        private void btn_ytb_downloader_Click(object sender, RoutedEventArgs e)
        {
            YoutubeSearch();
        }

        private void YoutubeSearch()
        {
            
        }


        /*
        private void DownloadVideo()
        {
            int size = (int)_comboResolution.SelectedItem;
            if (hsdb.IsHyperspin)
                Videos.FileToSaveAs = hsdb.HSPath + "\\Media" + "\\" + hsdb.SystemName + "\\" + "Video" + "\\" + SelectedGame.RomName;
            else { }

            Videos.DownloadVideo(txtUrl.Text, (int)_comboResolution.SelectedItem);

        }
        */

        /*
        private void YoutubeSearch()
        {

            var koof = hsDatagrid.SelectedItem as DatabaseGame;
            if (koof == null)
                return;

            string GameName = koof.Description;

            //if (sender == YoutubeGame)
            string YoutubeSearchLink = "https://www.google.co.uk/search?q=site:youtube.com" + "+" + GameName;
            //if (sender == YoutubeGameSystem)
            YoutubeSearchLink = "https://www.google.co.uk/search?q=site:youtube.com" + "+" + GameName + "+" + hsdb.SystemName;
            //if (sender == YoutubeGameTrailer)
            //    YoutubeSearchLink = "https://www.google.co.uk/search?q=site:youtube.com" + "+" + GameName + "+" + Hlm.sysName + "+" + "Trailer";

            //else
            //{
            //    if (!Directory.Exists(Hlm.HlMPath + "\\Videos\\" + Hlm.sysName + "\\" + selectedRom))
            //        Directory.CreateDirectory(Hlm.HlMPath + "\\Videos\\" + Hlm.sysName + "\\" + selectedRom);
            //    FileToSaveAs = Hlm.HlMPath + "\\Videos\\" + Hlm.sysName + "\\" + selectedRom + "\\" + selectedRom;
            //}

            if (!browser.IsEnabled)
            {
                browser.IsEnabled = true;
                browser.Navigate(Videos.YoutubeSearch(SelectedGame, YoutubeSearchLink));
            }
            else
            {
                browser.Navigate(Videos.YoutubeSearch(SelectedGame, YoutubeSearchLink));
            }

            //HM.FileToSaveAs = FileToSaveAs;

        }
        */
    }
}
