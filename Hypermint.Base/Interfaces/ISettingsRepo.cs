using Hypermint.Base.Model;

namespace Hypermint.Base.Interfaces
{
    /// <summary>
    /// Stores settings used application wide
    /// </summary>
    public interface ISettingsHypermint
    {
        Setting HypermintSettings { get; set; }         
    }
}
