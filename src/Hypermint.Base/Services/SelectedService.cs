namespace Hypermint.Base.Services
{
    public class SelectedService : ISelectedService
    {
        public string CurrentSystem { get; set; }

        public bool IsMainMenu()
        {
            if (CurrentSystem.Contains("Main Menu"))
                return true;
            else
                return false;
        }
    }
}
