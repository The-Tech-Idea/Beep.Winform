using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// BeepFilter - Events partial class
    /// Contains all event definitions and event raising methods
    /// </summary>
    public partial class BeepFilter
    {
        #region Filter Events

        /// <summary>
        /// Raised when a filter is added
        /// </summary>
        [Category("Filter")]
        [Description("Raised when a new filter is added")]
        public event EventHandler? FilterAdded;

        /// <summary>
        /// Raised when a filter is removed
        /// </summary>
        [Category("Filter")]
        [Description("Raised when a filter is removed")]
        public event EventHandler<FilterRemovedEventArgs>? FilterRemoved;

        /// <summary>
        /// Raised when a filter is modified
        /// </summary>
        [Category("Filter")]
        [Description("Raised when a filter is modified")]
        public event EventHandler<FilterChangedEventArgs>? FilterModified;

        /// <summary>
        /// Raised when all filters are cleared
        /// </summary>
        [Category("Filter")]
        [Description("Raised when all filters are cleared")]
        public event EventHandler? FiltersCleared;

        /// <summary>
        /// Raised when the active filter configuration changes
        /// </summary>
        [Category("Filter")]
        [Description("Raised when the active filter configuration changes")]
        public event EventHandler? FilterChanged;

        /// <summary>
        /// Raised when a user requests to edit a filter
        /// </summary>
        [Category("Filter")]
        [Description("Raised when a user requests to edit a filter")]
        public event EventHandler<FilterEditEventArgs>? FilterEditRequested;

        /// <summary>
        /// Raised when filters should be applied
        /// </summary>
        [Category("Filter")]
        [Description("Raised when filters should be applied")]
        public event EventHandler? FilterApplied;

        /// <summary>
        /// Raised when the user cancels the filter dialog interaction.
        /// </summary>
        [Category("Filter")]
        [Description("Raised when user cancels filter dialog interaction")]
        public event EventHandler? FilterCanceled;

        #endregion

        #region Group Events

        /// <summary>
        /// Raised when a filter group is added
        /// </summary>
        [Category("Filter")]
        [Description("Raised when a filter group is added")]
        public event EventHandler? GroupAdded;

        /// <summary>
        /// Raised when a filter group is removed
        /// </summary>
        [Category("Filter")]
        [Description("Raised when a filter group is removed")]
        public event EventHandler<FilterGroupRemovedEventArgs>? GroupRemoved;

        #endregion

        #region Interaction Events

        /// <summary>
        /// Raised when user requests to select a field/column
        /// </summary>
        [Category("Filter")]
        [Description("Raised when user requests to select a field/column")]
        public event EventHandler<FilterInteractionEventArgs>? FieldSelectionRequested;

        /// <summary>
        /// Raised when user requests to select an operator
        /// </summary>
        [Category("Filter")]
        [Description("Raised when user requests to select an operator")]
        public event EventHandler<FilterInteractionEventArgs>? OperatorSelectionRequested;

        /// <summary>
        /// Raised when user requests to input a value
        /// </summary>
        [Category("Filter")]
        [Description("Raised when user requests to input a value")]
        public event EventHandler<FilterInteractionEventArgs>? ValueInputRequested;

        /// <summary>
        /// Raised when user starts dragging a filter
        /// </summary>
        [Category("Filter")]
        [Description("Raised when user starts dragging a filter")]
        public event EventHandler<FilterInteractionEventArgs>? FilterDragStarted;

        /// <summary>
        /// Raised when a section is toggled (collapsed/expanded)
        /// </summary>
        [Category("Filter")]
        [Description("Raised when a section is toggled")]
        public event EventHandler<FilterSectionEventArgs>? SectionToggled;

        /// <summary>
        /// Raised when search input is focused
        /// </summary>
        [Category("Filter")]
        [Description("Raised when search input is focused")]
        public event EventHandler<FilterSearchEventArgs>? SearchFocusRequested;

        #endregion

        #region Configuration Events

        /// <summary>
        /// Raised when user requests to save filter configuration
        /// </summary>
        [Category("Filter")]
        [Description("Raised when user requests to save filter configuration")]
        public event EventHandler? ConfigurationSaveRequested;

        /// <summary>
        /// Raised when user requests to load filter configuration
        /// </summary>
        [Category("Filter")]
        [Description("Raised when user requests to load filter configuration")]
        public event EventHandler? ConfigurationLoadRequested;

        #endregion

        #region UI Events

        /// <summary>
        /// Raised when the filter style changes
        /// </summary>
        [Category("Appearance")]
        [Description("Raised when the filter style changes")]
        public event EventHandler? FilterStyleChanged;

        /// <summary>
        /// Raised when the expanded state changes
        /// </summary>
        [Category("Appearance")]
        [Description("Raised when the expanded state changes")]
        public event EventHandler? ExpandedChanged;

        #endregion

        #region Event Raising Methods

        protected virtual void OnFilterAdded()
        {
            FilterAdded?.Invoke(this, EventArgs.Empty);
            
            if (AutoApply)
                OnFilterApplied();
        }

        protected virtual void OnFilterRemoved(int index)
        {
            FilterRemoved?.Invoke(this, new FilterRemovedEventArgs { Index = index });
            
            if (AutoApply)
                OnFilterApplied();
        }

        protected virtual void OnFilterModified(int index)
        {
            FilterModified?.Invoke(this, new FilterChangedEventArgs { Index = index });
            
            if (AutoApply)
                OnFilterApplied();
        }

        protected virtual void OnFiltersCleared()
        {
            FiltersCleared?.Invoke(this, EventArgs.Empty);
            
            if (AutoApply)
                OnFilterApplied();
        }

        protected virtual void OnFilterChanged()
        {
            FilterChanged?.Invoke(this, EventArgs.Empty);
            
            if (AutoApply)
                OnFilterApplied();
        }

        protected virtual void OnFilterEditRequested(int index)
        {
            FilterEditRequested?.Invoke(this, new FilterEditEventArgs { Index = index });
        }

        protected virtual void OnFilterApplied()
        {
            FilterApplied?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFilterCanceled()
        {
            FilterCanceled?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnGroupAdded()
        {
            GroupAdded?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnGroupRemoved(int index)
        {
            GroupRemoved?.Invoke(this, new FilterGroupRemovedEventArgs { Index = index });
        }

        protected virtual void OnConfigurationSaveRequested()
        {
            ConfigurationSaveRequested?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnConfigurationLoadRequested()
        {
            ConfigurationLoadRequested?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFilterStyleChanged()
        {
            FilterStyleChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnExpandedChanged()
        {
            ExpandedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }

    #region Event Args Classes

    public class FilterRemovedEventArgs : EventArgs
    {
        public int Index { get; set; }
    }

    public class FilterChangedEventArgs : EventArgs
    {
        public int Index { get; set; }
    }

    public class FilterEditEventArgs : EventArgs
    {
        public int Index { get; set; }
    }

    public class FilterGroupRemovedEventArgs : EventArgs
    {
        public int Index { get; set; }
    }

    /// <summary>
    /// Event args for filter interaction events (field/operator/value selection, drag)
    /// </summary>
    public class FilterInteractionEventArgs : EventArgs
    {
        public int Index { get; set; }
        public Rectangle Bounds { get; set; }

        public FilterInteractionEventArgs(int index, Rectangle bounds)
        {
            Index = index;
            Bounds = bounds;
        }
    }

    /// <summary>
    /// Event args for section toggle events
    /// </summary>
    public class FilterSectionEventArgs : EventArgs
    {
        public object SectionId { get; set; }

        public FilterSectionEventArgs(object sectionId)
        {
            SectionId = sectionId;
        }
    }

    /// <summary>
    /// Event args for search focus events
    /// </summary>
    public class FilterSearchEventArgs : EventArgs
    {
        public Rectangle Bounds { get; set; }

        public FilterSearchEventArgs(Rectangle bounds)
        {
            Bounds = bounds;
        }
    }

    #endregion
}
