using Hs.HyperSpin.Database;
using Hypermint.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Hs.Hypermint.DatabaseDetails.Commands
{
    class GetGamesCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

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
