using Hypermint.Base.Constants;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypermint.Base.Helpers
{
    public static class RegionHelper
    {
        /// <summary>
        /// Gets the current view name (first instance) from the region managers, Regions.ActiveViews
        /// </summary>        
        public static string GetCurrentViewName(IRegionManager rm)
        {
            var views = rm.Regions[RegionNames.ContentRegion].ActiveViews.ToList();

            var activeViewName = views[0].ToString();

            return activeViewName;
        }
    }
}
