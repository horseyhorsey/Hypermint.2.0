using Hs.Hypermint.Services;
using Hs.HyperSpin.Database;
using Hypermint.Base.Base;
using Hypermint.Base.Helpers;
using Hypermint.Base.Interfaces;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using System.ComponentModel;
using System.Windows.Data;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class RlScanMediaFolderViewModel : ViewModelBase
    {
        const string patterns = @"\(.*\)";

        public RlScanMediaFolderViewModel(string rlMediaFolder, string hsFolder, string mediaFolderName, string systemName, IGameRepo gameRepo,
            IDialogCoordinator dialogService, CustomDialog customDialog)
        {
            rocketMediaScanner = new RocketMediaFolderScanner(rlMediaFolder, hsFolder);

            CurrentMediaFolder = Path.Combine(rlMediaFolder, mediaFolderName, systemName);

            Directories = rocketMediaScanner.GetAllFolders(CurrentMediaFolder);
            Results = rocketMediaScanner.MatchFoldersToGames(Directories, gameRepo);

            //Go over games and create list marking whether a folder is matched
            var games = gameRepo.GamesList;
            var games2 = new ObservableCollection<TempGame>();
            foreach (var game in games)
            {
                games2.Add(new TempGame
                {
                    HasFolder = Results.MatchedFolders.Any(x => x == game.RomName),
                    RomName = game.RomName
                });
            }

            UnmatchedFolders = new ObservableCollection<UnMatchedFolder>();

            Results.UnMatchedFolders.ForEach(folder =>
            {
                UnmatchedFolders.Add(new UnMatchedFolder { FolderName = folder });
            });

            CurrentGames = games2;

            GamesFolders = new ListCollectionView(CurrentGames);

            UnmatchedFoldersView = new ListCollectionView(UnmatchedFolders);            

            CloseCommand = new DelegateCommand(async () =>
            {
                await dialogService.HideMetroDialogAsync(this, customDialog);
            });

            MatchFoldersCommand = new DelegateCommand(async () =>
            {
                await MatchFoldersAsync();
            });

            ClearMatchedCommand = new DelegateCommand(() =>
            {
                var folders = UnmatchedFolders.Where(x => !string.IsNullOrWhiteSpace(x.RecommendedName));
                foreach (var folder in folders)
                {
                    folder.RecommendedName = string.Empty;
                    folder.Rename = false;
                }
            });

            RenameCommand = new DelegateCommand(() =>
            {
                RenameUnmatchedFolders();
            });
        }        

        #region Properties

        public ICollectionView GamesFolders { get; set; }
        public ICollectionView UnmatchedFoldersView { get; set; }

        public ObservableCollection<TempGame> CurrentGames { get; set; }
        public ObservableCollection<UnMatchedFolder> UnmatchedFolders { get; set; }

        private RocketMediaFolderScanner rocketMediaScanner;
        public RocketMediaFolderScanResult Results { get; private set; }

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

        public string CurrentMediaFolder { get; private set; }

        #endregion

        #region Commands
        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand MatchFoldersCommand { get; private set; }
        public DelegateCommand RenameCommand { get; private set; }
        public DelegateCommand ClearMatchedCommand { get; private set; }
        public DelegateCommand ClearSelectedCommand { get; private set; }        
        #endregion

        #region Support Methods

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
                        Directory.Move(Path.Combine(CurrentMediaFolder,folder.FolderName),Path.Combine(CurrentMediaFolder, folder.RecommendedName));

                        foldersToRemove.Add(folder);

                        CurrentGames.Add(new TempGame { HasFolder = true, RomName = folder.RecommendedName });
                    }
                    catch (Exception ex) { }
                    
                }

                //remove folders from unmatched
                foreach (var item in foldersToRemove)
                {
                    UnmatchedFolders.Remove(item);
                }
            }
        }

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

        private void MatchDescriptions()
        {
            //// PIN X CHECK MATCHING
            /*
            var g = MatchYearGreaterThan;
            var tableNameEdit = "";
            var tableDescriptionEdit = "";
            var masterNameEdit = "";
            var masterDescriptionEdit = "";
            var rgx = new Regex(patterns);
            var count = 0;
            var bestTableMatch = new VirtualPin.Database.PinballXTable();

            //clearMatchedDescriptions();

            foreach (VirtualPin.Database.UnMatchedTable table in UnMatchedTables)
            {
                var inputTableName = table.FileName;
                var inputTableDesc = table.Description;

                if (RemoveParenthysis)
                {
                    tableNameEdit = rgx.Replace(inputTableName, string.Empty);
                    tableDescriptionEdit = rgx.Replace(inputTableDesc, string.Empty);
                }

                if (MatchYearGreaterThan)
                {
                    if (table.Year > YearValue || table.Year == 0)
                        flagMatchEnabled = true;
                    else flagMatchEnabled = false;
                }
                else
                {
                    if (table.Year < YearValue || table.Year == 0)
                        flagMatchEnabled = true;
                    else flagMatchEnabled = false;
                }

                if (flagMatchEnabled)
                {
                    if (!table.MatchedName)
                    {
                        foreach (var masterTable in _tableRepo.MasterTableList)
                        {
                            if (RemoveParenthysisMaster)
                            {
                                masterNameEdit = rgx.Replace(masterTable.Name, string.Empty);
                                masterDescriptionEdit = rgx.Replace(masterTable.Description, string.Empty);
                            }

                            if (MatchYearGreaterThan)
                            {
                                flagMatchEnabled = masterTable.Year > YearValue;
                            }
                            else
                            {
                                flagMatchEnabled = masterTable.Year < YearValue;
                            }

                            if (flagMatchEnabled)
                            {
                                string pattern;
                                //if (bw.CancellationPending)
                                //{
                                //    e.Cancel = true;
                                //    return;
                                //}

                                if (!TableMatchDescription)
                                {
                                    pattern = RemoveParenthysis ? tableNameEdit.ToUpper() : table.FileName.ToUpper();
                                }
                                else
                                {
                                    pattern = RemoveParenthysis
                                        ? tableDescriptionEdit.ToUpper()
                                        : table.Description.ToUpper();
                                }

                                var input = !MasterMatchDescription ? masterTable.Name.ToUpper() : masterTable.Description.ToUpper();

                                var d = new Distance();
                                var i = d.LD(input, pattern);

                                if (i <= MatchDistance)
                                {
                                    MatchDistance = i;
                                    bestTableMatch = masterTable;
                                    table.FlagRename = true;
                                    table.MatchedDescription = masterTable.Description;

                                    RenameEnabled = true;
                                }
                            }
                        }
                    }
                }
                count++;
                //li.Add(bestTableMatch);
                bestTableMatch = new VirtualPin.Database.PinballXTable();
                //var percentage = (Int32)Math.Round((double)(count * 100) / ScanCount);
                //bw.ReportProgress(percentage);

            }

            UnMatchedTables.Refresh();

            // Put the list into the background workers Result
            //e.Result = li;
        }
        */
        }

        #endregion

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
    }
}