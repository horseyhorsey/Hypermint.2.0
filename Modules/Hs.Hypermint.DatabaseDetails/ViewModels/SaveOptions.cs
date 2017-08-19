using Prism.Mvvm;

namespace Hs.Hypermint.DatabaseDetails.ViewModels
{
    public class SaveOptions : BindableBase
    {
        #region Properties        

        private bool saveToDatabase = true;
        public bool SaveToDatabase
        {
            get { return saveToDatabase; }
            set { SetProperty(ref saveToDatabase, value); }
        }

        private bool saveGenres;
        public bool SaveGenres
        {
            get { return saveGenres; }
            set { SetProperty(ref saveGenres, value); }
        }

        private bool saveFavoritesText = true;
        public bool SaveFavoritesText
        {
            get { return saveFavoritesText; }
            set { SetProperty(ref saveFavoritesText, value); }
        }

        private bool saveFavoritesXml;
        public bool SaveFavoritesXml
        {
            get { return saveFavoritesXml; }
            set { SetProperty(ref saveFavoritesXml, value); }
        }

        private bool addToGenre;
        public bool AddToGenre
        {
            get { return addToGenre; }
            set { SetProperty(ref addToGenre, value); }
        }

        private string dbName;
        public string DbName
        {
            get { return dbName; }
            set { SetProperty(ref dbName, value); }
        }

        #endregion
    }
}
