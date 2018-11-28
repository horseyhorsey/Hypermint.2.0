using System.ComponentModel;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using Prism.Events;
using System.Windows.Data;

namespace Hs.Hypermint.Audits.ViewModels
{
    public class RlMediaAuditViewModel : HyperMintModelBase, IGamesList
    {
        public RlMediaAuditViewModel(IEventAggregator evtAggregator, ISelectedService selectedService,
                    IGameLaunch gameLaunch, ISettingsHypermint settingsRepo, IHyperspinManager hyperspinManager) : 
            base(evtAggregator, selectedService, gameLaunch, settingsRepo)
        {

            GamesList = new ListCollectionView(hyperspinManager.CurrentSystemsGames);
            GamesList.CurrentChanged += GamesList_CurrentChanged;
        }

        public ICollectionView GamesList { get; set; }


        private void GamesList_CurrentChanged(object sender, System.EventArgs e)
        {

        }
    }
}
