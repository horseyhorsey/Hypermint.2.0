using Prism.Logging;
using Prism.Mvvm;
using System.Runtime.CompilerServices;

namespace Hypermint.Base
{
    public class ViewModelBase : BindableBase { }

    /// <summary>
    /// Base viewModel with logging. See <see cref="Base.Log.HypermintLogger"/>
    /// </summary>
    public class HypermintViewModelBase : ViewModelBase
    {
        protected readonly ILoggerFacade _loggerFacade;

        public HypermintViewModelBase(ILoggerFacade loggerFacade)
        {
            _loggerFacade = loggerFacade;
        }

        public virtual void Log(string message, Category cat = Category.Debug, Priority priority = Priority.None, [CallerMemberName] string caller = "")
        {
            _loggerFacade.Log($"{caller} | {message}", cat, priority);
        }
    }
}
