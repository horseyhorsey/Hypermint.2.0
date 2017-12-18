using Hypermint.Base;
using Hypermint.Base.Helpers;
using Hypermint.Base.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Frontends.Models.Hyperspin;
using Frontends.Models.RocketLauncher;
using Microsoft.Practices.ObjectBuilder2;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class RlScanMediaFolderViewModel : ViewModelBase
    {
        const string patterns = @"\(.*\)";

        #region Fields
        private IEnumerable<Game> _games;
        private IRlScan _rlScan;        
        #endregion

        public RlScanMediaFolderViewModel(string rlMediaFolder, string hsFolder, string mediaFolderName, string systemName,
            IDialogCoordinator dialogService, CustomDialog customDialog, IEnumerable<Game> gamesList, IRlScan rlScan)
        {
            _games = gamesList;
            _rlScan = rlScan;

            CurrentMediaFolder = Path.Combine(rlMediaFolder, mediaFolderName, systemName);

            //Collection views
            UnmatchedFolders = new ObservableCollection<UnMatchedFolder>();
            CurrentGames = new ObservableCollection<TempGame>();
            GamesFolders = new ListCollectionView(CurrentGames);
            UnmatchedFoldersView = new ListCollectionView(UnmatchedFolders);

            ClearMatchedCommand = new DelegateCommand(() =>
            {
                var folders = UnmatchedFolders.Where(x => !string.IsNullOrWhiteSpace(x.RecommendedName));
                foreach (var folder in folders)
                {
                    folder.RecommendedName = string.Empty;
                    folder.Rename = false;
                }
            });
            CloseCommand = new DelegateCommand(async () =>
            {
                await dialogService.HideMetroDialogAsync(this, customDialog);
            });
            MatchFoldersCommand = new DelegateCommand(async () =>
            {
                await MatchFoldersAsync();
            });
            RenameCommand = new DelegateCommand(() =>
            {
                RenameUnmatchedFolders();
            });
            ScanForMappedFoldersCommand = new DelegateCommand(async () => await LoadAndScanFolders());
        }

        #region Commands
        public ICommand CloseCommand { get; private set; }
        public ICommand MatchFoldersCommand { get; private set; }
        public ICommand RenameCommand { get; private set; }
        public ICommand ClearMatchedCommand { get; private set; }
        public ICommand ClearSelectedCommand { get; private set; }
        public ICommand ScanForMappedFoldersCommand { get; private set; }
        #endregion

        #region Properties

        public ICollectionView GamesFolders { get; set; }
        public ICollectionView UnmatchedFoldersView { get; set; }

        public ObservableCollection<TempGame> CurrentGames { get; set; }
        public ObservableCollection<UnMatchedFolder> UnmatchedFolders { get; set; }        
        public  MediaScanResult Results { get; private set; }

        public string[] Directories { get; set; }

        private int _matchDistance = 2;
        public int MatchDistance
        {
            get { return _matchDistance; }
            set { SetProperty(ref _matchDistance, value); }
        }

        private bool _removeParenthsys = true;
        public bool RemoveParenthsys
        {
            get { return _removeParenthsys; }
            set { SetProperty(ref _removeParenthsys, value); }
        }

        private bool _showMissing = true;
        public bool ShowMissing
        {
            get { return _showMissing; }
            set
            {
                SetProperty(ref _showMissing, value);

                UpdateFolderFilter();
            }
        }

        private bool _showAvailable = true;
        public bool ShowAvailable
        {
            get { return _showAvailable; }
            set
            {
                SetProperty(ref _showAvailable, value);

                UpdateFolderFilter();
            }
        }

        private bool _showMatched = true;
        public bool ShowMatched
        {
            get { return _showMatched; }
            set
            {
                SetProperty(ref _showMatched, value);

                UpdateMatchedFolderFilter();
            }
        }

        private bool _showUnmatched = true;
        public bool ShowUnmatched
        {
            get { return _showUnmatched; }
            set
            {
                SetProperty(ref _showUnmatched, value);

                UpdateMatchedFolderFilter();
            }
        }

        private bool _isBusy = true;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public string CurrentMediaFolder { get; private set; }

        #endregion

        public async Task LoadAndScanFolders()
        {
            IsBusy = false;

            try
            {
                CurrentGames.Clear();

                await Task.Run(() =>
                 {
                     Directories = _rlScan.GetAllFolders(CurrentMediaFolder);
                     Results = _rlScan.MatchFoldersToGames(Directories, _games);
                 });

                Results.UnMatchedFolders.ForEach(folder =>
                {
                    UnmatchedFolders.Add(new UnMatchedFolder { FolderName = folder });
                });

                var games = _games.Select(x => new TempGame { RomName = x.RomName, HasFolder = Results.MatchedFolders.Any(y => y == x.RomName) });
                foreach (var game in games)
                {
                    CurrentGames.Add(game);
                }
            }
            catch { }
            finally { IsBusy = true; }
        }

        #region Support Methods

        /// <summary>
        /// Matches the folders asynchronous.
        /// </summary>
        /// <returns></returns>
        private async Task MatchFoldersAsync()
        {
            await Task.Run(() =>
            {
                var matchDistance = 0;
                var rgx = new Regex(patterns);

                foreach (var unmatchedFolder in UnmatchedFolders)
                {
                    //Reset the match distance
                    matchDistance = MatchDistance;
                    //Get the unmatched folders name and remove parentheses if necessary
                    var folderToMatch = unmatchedFolder.FolderName;
                    if (RemoveParenthsys)
                    {
                        folderToMatch = rgx.Replace(folderToMatch, string.Empty);
                    }

                    //Get best match from games list where not already matched to a folder.
                    foreach (var item in CurrentGames.Where(x => !x.HasFolder))
                    {
                        var bestMatch = "";
                        var gameFolder = item.RomName;
                        if (RemoveParenthsys)
                        {
                            gameFolder = rgx.Replace(gameFolder, string.Empty);
                        }

                        var d = new Distance();
                        var i = d.LD(gameFolder, folderToMatch);

                        if (i <= matchDistance)
                        {
                            matchDistance = i;
                            bestMatch = item.RomName;
                            unmatchedFolder.Rename = true;
                            unmatchedFolder.RecommendedName = item.RomName;
                        }
                    }

                    //See if this matched folder already exists and mark it.
                    if (!string.IsNullOrWhiteSpace(unmatchedFolder.RecommendedName))
                    {
                        var game = CurrentGames.FirstOrDefault(x => x.RomName == unmatchedFolder.RecommendedName);
                        if (game != null && game.HasFolder)
                            unmatchedFolder.FolderExists = true;
                    }
                }
            });

        }

        private void RenameUnmatchedFolders()
        {
            var result = System.Windows.MessageBox.Show("Sure you want to rename all folders checked?", "Rename folders", System.Windows.MessageBoxButton.YesNo);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                var foldersToRemove = new List<UnMatchedFolder>();

                //Add folders to Matched (CurrentGames)
                foreach (var folder in UnmatchedFolders.Where(x => x.Rename))
                {
                    try
                    {
                        Directory.Move(Path.Combine(CurrentMediaFolder, folder.FolderName), Path.Combine(CurrentMediaFolder, folder.RecommendedName));

                        foldersToRemove.Add(folder);

                        CurrentGames.Add(new TempGame { HasFolder = true, RomName = folder.RecommendedName });
                    }
                    catch (Exception) { }

                }

                //remove folders from unmatched
                foreach (var item in foldersToRemove)
                {
                    UnmatchedFolders.Remove(item);
                }
            }
        }

        private void UpdateMatchedFolderFilter()
        {
            UnmatchedFoldersView.Filter = (x) =>
            {
                if (x != null)
                {
                    var folder = x as UnMatchedFolder;

                    //Hide all
                    if (!ShowMatched && !ShowUnmatched) return false;
                    else if (!ShowMatched && ShowUnmatched)
                    {
                        return string.IsNullOrEmpty(folder.RecommendedName);
                    }
                    else if (ShowMatched && !ShowUnmatched)
                    {
                        return !string.IsNullOrEmpty(folder.RecommendedName);
                    }
                    else return true;
                }

                return false;
            };
        }

        private void UpdateFolderFilter()
        {
            GamesFolders.Filter = (x) =>
            {
                if (x != null)
                {
                    var folder = x as TempGame;

                    //Hide all
                    if (!ShowMissing && !ShowAvailable) return false;
                    else if (!ShowMissing && ShowAvailable)
                    {
                        return folder.HasFolder;
                    }
                    else if (ShowMissing && !ShowAvailable)
                    {
                        return !folder.HasFolder;
                    }
                    else return true;
                }

                return false;
            };
        }

        #endregion

        #region Support Classes
        public class TempGame
        {
            public string RomName { get; set; }

            public bool HasFolder { get; set; }
        }

        public class UnMatchedFolder : ViewModelBase
        {
            private bool _rename;
            public bool Rename
            {
                get { return _rename; }
                set { SetProperty(ref _rename, value); }
            }

            private bool _folderExists;
            public bool FolderExists
            {
                get { return _folderExists; }
                set { SetProperty(ref _folderExists, value); }
            }

            private string _folderName;
            public string FolderName
            {
                get { return _folderName; }
                set { SetProperty(ref _folderName, value); }
            }

            private string _recommendedName;
            public string RecommendedName
            {
                get { return _recommendedName; }
                set { SetProperty(ref _recommendedName, value); }
            }
        }
        #endregion
    }
}