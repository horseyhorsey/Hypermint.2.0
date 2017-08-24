namespace Hypermint.Base.Model
{
    /// <summary>
    /// Base class for a WPF Tree View control. Has properties for DisplayName, IsExpanded and IsSelected.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public abstract class TreeItemViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeItemViewModel"/> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        public TreeItemViewModel(string displayName)
        {
            DisplayName = displayName;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// Gets or Sets the IsSelected
        /// </summary>
        public bool IsSelected { get; set; }

        #endregion
    }
}
