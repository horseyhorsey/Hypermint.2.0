namespace Hypermint.Base.Interfaces
{
    public interface ISystemGen
    {
        void CreateSystem(string systemName);

        /// <summary>
        /// Create settings file if doesn't exist
        /// Create or copy an xml if doesn't exist
        /// Create directorys for media
        /// </summary>
        /// <param name="database"></param>
        /// <param name="systemName"></param>
        void CreateSystemFromHsDb(string database, string systemName = null);


    }
}
