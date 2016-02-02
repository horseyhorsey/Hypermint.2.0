
using Hypermint.Base.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Hypermint.Modules.HyperSpin
{
    public class SelectedFolderBrowserViewModel : ViewModelBase
    {
        //private readonly ILotteryRepo _lotteryRepo;
       // public ObservableCollection<Lottery> Hypermint.Modules.HyperSpin { get; set; }
        //public IRegionManager _regionManager;

        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                SetProperty(ref isBusy, value, "IsBusy");
            }
        }


        public SelectedFolderBrowserViewModel()//ILotteryRepo lottoRepo)
        {
            //_regionManager = regionManager;
            ///this._lotteryRepo = lottoRepo;


           // LoadLotteryDraws();

        }



        private void LoadLotteryDraws()
        {
            IsBusy = true;
            //Hypermint.Modules.HyperSpin = CollectionExtensions.ToObservableCollection(_lotteryRepo.GetLotteryDraws());
            IsBusy = false;
        }
    }

    /// <summary>
    /// Extension method to convert to observable collections
    /// </summary>
    public static class CollectionExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
        {
            if (enumerableList != null)
            {
                //create an emtpy observable collection object
                var observableCollection = new ObservableCollection<T>();

                //loop through all the records and add to observable collection object
                foreach (var item in enumerableList)
                    observableCollection.Add(item);

                //return the populated observable collection
                return observableCollection;
            }
            return null;
        }
    }
}
