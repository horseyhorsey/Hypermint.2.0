using Frontends.Models.Hyperspin;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Hs.Hypermint.DatabaseDetails.Commands
{
    class GetGamesCommand : ICommand
    {
        public event EventHandler CanExecuteChanged = null;

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            var loadedGames = parameter as ObservableCollection<Game>;
            if (loadedGames != null)
            {
                TaskFactory taskfac = new TaskFactory();
                taskfac.StartNew(() =>
                {
                        //var gamesList = Games.LoadGamesFromDatabase();
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            loadedGames.Clear();
                            //foreach (var game in gamesList)
                              //  loadedGames.Add(game);
                        }));

                        // Thread.Sleep(2000);                    
                });
            }
        }
       
    }
}
