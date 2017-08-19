using Microsoft.Practices.Unity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.Globalization;
using System.Windows.Threading;

namespace Hypermint.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {        
        private const string ViewNamespace = "Views";
        private const string ViewModelNamespace = "Viewmodels";        

        IUnityContainer _container = new UnityContainer();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);            

            Bootstrapper bootStrap = new Bootstrapper();
            bootStrap.Run();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var friendlyName = viewType.FullName.Remove(viewType.FullName.Length - 4);
                friendlyName = friendlyName.Replace(ViewNamespace, ViewModelNamespace);
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}ViewModel, {1}", friendlyName, viewAssemblyName);
                return Type.GetType(viewModelName);
            });

        }
    }
}
