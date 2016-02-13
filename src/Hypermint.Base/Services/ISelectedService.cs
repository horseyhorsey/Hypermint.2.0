namespace Hypermint.Base.Services
{
    public interface ISelectedService
    {
        string CurrentSystem { get; set; }

        bool IsMainMenu();
    }
}
