using System.Collections.Generic;

namespace Hypermint.Shell.Models
{
    public static class MahAppTheme
    {        
        /// <summary>
        /// List for use in the themes combobox
        /// </summary>
        public static List<string> AvailableThemes
        {
            get { return getThemes(); }
        }

        /// <summary>
        /// Get the themes from the Mahapps ThemeManager.Accents
        /// </summary>
        /// <returns></returns>
        private static List<string> getThemes()
        {
            var themes = new List<string>();

            try
            {
                foreach (var item in MahApps.Metro.ThemeManager.Accents)
                {
                    themes.Add(item.Name);
                }
            }
            catch (System.Exception)
            {

                
            }      



            return themes;
        }        
        
    }
}
