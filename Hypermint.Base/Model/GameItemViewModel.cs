using Frontends.Models.Hyperspin;

namespace Hypermint.Base.Model
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class GameItemViewModel
    {
        public Game game;

        /// <summary>
        /// Data bind wraps the Game model from Frontends.Models.Hyperspin <para/>
        /// *Fody propertychanged not available on .Net std 2.0
        /// </summary>
        /// <param name="game">The game.</param>       
        public GameItemViewModel(Game game)
        {
            this.game = game;
        }

        public int GameEnabled { get { return game.GameEnabled; } set { game.GameEnabled = value; } }
        public string Name { get { return game.Name; } set { game.Name = value; } }
        public string RomName { get { return game.RomName; } set { game.RomName = value; } }
        public string System { get { return game.System; } set { game.System = value; } }
        public string Description { get { return game.Description; } set { game.Description = value; } }
        public string Manufacturer { get { return game.Manufacturer; } set { game.Manufacturer = value; } }
        public int Year { get { return game.Year; } set { game.Year = value; } }
        public string Genre { get { return game.Genre; } set { game.Genre = value; } }
        public string CloneOf { get { return game.CloneOf; } set { game.CloneOf = value; } }
        public string Rating { get { return game.Rating; } set { game.Rating = value; } }
        public string Crc { get { return game.Crc; } set { game.Crc = value; } }

        //Extra props
        public bool RomExists { get { return game.RomExists; } set { game.RomExists = value; } }
        public bool IsFavorite { get { return game.IsFavorite; } set { game.IsFavorite = value; } }
        
    }
}
