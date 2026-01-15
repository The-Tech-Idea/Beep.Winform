using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.DataBase;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// BeepFilter - Properties partial class
    /// Contains all public properties and their change handlers
    /// </summary>
    public partial class BeepFilter
    {
        #region Filter Style Properties

        /// <summary>
        /// Gets or sets the filter interaction pattern
        /// </summary>
        [Category("Filter")]
        [Description("Filter interaction pattern (TagPills, GroupedRows, QueryBuilder, etc.)")]
        [DefaultValue(FilterStyle.TagPills)]
        public FilterStyle FilterStyle
        {
            get => _filterStyle;
            set
            {
                if (_filterStyle != value)
                {
                    _filterStyle = value;
                    UpdatePainter();
                    RecalculateLayout();
                    Invalidate();
                    OnFilterStyleChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the display mode
        /// </summary>
        [Category("Filter")]
        [Description("How the filter is displayed (AlwaysVisible, Collapsible, Modal, etc.)")]
        [DefaultValue(FilterDisplayMode.AlwaysVisible)]
        public FilterDisplayMode DisplayMode
        {
            get => _displayMode;
            set
            {
                if (_displayMode != value)
                {
                    _displayMode = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter position
        /// </summary>
        [Category("Filter")]
        [Description("Position relative to parent control (Top, Bottom, Left, Right, etc.)")]
        [DefaultValue(FilterPosition.Top)]
        public FilterPosition Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    RecalculateLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the filter panel is expanded
        /// </summary>
        [Category("Filter")]
        [Description("Whether the filter panel is expanded (for Collapsible mode)")]
        [DefaultValue(true)]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    RecalculateLayout();
                    Invalidate();
                    OnExpandedChanged();
                }
            }
        }

        #endregion

        #region Data Properties

        /// <summary>
        /// Gets or sets the active filter configuration
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FilterConfiguration ActiveFilter
        {
            get => _activeFilter;
            set
            {
                if (_activeFilter != value)
                {
                    _activeFilter = value ?? new FilterConfiguration();
                    _filterCount = _activeFilter.Criteria.Count;
                    RecalculateLayout();
                    Invalidate();
                    OnFilterChanged();
                }
            }
        }

        /// <summary>
        /// Gets the number of active filters
        /// </summary>
        [Browsable(false)]
        public int FilterCount => _filterCount;

        /// <summary>
        /// Gets whether any filters are active
        /// </summary>
        [Browsable(false)]
        public bool HasFilters => _filterCount > 0;

        #endregion

        #region Appearance Properties

        /// <summary>
        /// Gets the current painter being used
        /// </summary>
        [Browsable(false)]
        public IFilterPainter? ActivePainter => _activePainter;

        /// <summary>
        /// Gets the current layout information
        /// </summary>
        [Browsable(false)]
        public FilterLayoutInfo CurrentLayout => _currentLayout;

        /// <summary>
        /// Gets or sets the control style for visual appearance
        /// Inherited from BaseControl - affects colors, borders, shadows via BeepStyling
        /// </summary>
        [Category("Appearance")]
        [Description("Visual style from BeepControlStyle enum (Material, Fluent, Minimal, etc.)")]
        public new BeepControlStyle ControlStyle
        {
            get => base.ControlStyle;
            set
            {
                if (base.ControlStyle != value)
                {
                    base.ControlStyle = value;
                    Invalidate();
                }
            }
        }

        #endregion

        #region Data Source Configuration

        /// <summary>
        /// Gets or sets the data source for the filter
        /// Can be any enumerable collection (grid data, list, etc.)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? DataSource { get; set; }

        /// <summary>
        /// Gets or sets the entity structure describing the data
        /// Contains field names, types, and metadata for building filters
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EntityStructure? EntityStructure { get; set; }

        /// <summary>
        /// Gets the columns available for filtering (derived from EntityStructure)
        /// Used by painters to show column dropdowns
        /// </summary>
        [Browsable(false)]
        public List<FilterColumnInfo> AvailableColumns
        {
            get
            {
                if (EntityStructure == null || EntityStructure.Fields == null)
                    return new List<FilterColumnInfo>();

                return EntityStructure.Fields
                    .Select(f => new FilterColumnInfo
                    {
                        ColumnName = f.FieldName,
                        DisplayName = f.Originalfieldname ?? f.FieldName,
                        DataType = Type.GetType(f.Fieldtype) ?? typeof(string),
                        IsFilterable = true
                    })
                    .ToList();
            }
        }

        #endregion

        #region Behavior Properties

        /// <summary>
        /// Gets or sets whether filters are applied automatically on change
        /// </summary>
        [Category("Behavior")]
        [Description("Whether filters are applied automatically when changed")]
        [DefaultValue(false)]
        public bool AutoApply { get; set; } = false;

        /// <summary>
        /// Gets or sets whether to show the filter count badge
        /// </summary>
        [Category("Behavior")]
        [Description("Whether to show the active filter count badge")]
        [DefaultValue(true)]
        public bool ShowCountBadge { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to show action buttons (Add, Clear, etc.)
        /// </summary>
        [Category("Behavior")]
        [Description("Whether to show action buttons")]
        [DefaultValue(true)]
        public bool ShowActionButtons { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to enable drag and drop reordering
        /// </summary>
        [Category("Behavior")]
        [Description("Whether to enable drag and drop reordering of filters")]
        [DefaultValue(false)]
        public bool EnableDragDrop { get; set; } = false;

        #endregion

        #region Phase 1 Enhancement Properties

        /// <summary>
        /// Gets or sets whether keyboard shortcuts are enabled
        /// </summary>
        [Category("Filter")]
        [Description("Enable keyboard shortcuts (Ctrl+F, Ctrl+N, etc.)")]
        [DefaultValue(true)]
        public bool KeyboardShortcutsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets whether autocomplete suggestions are enabled
        /// </summary>
        [Category("Filter")]
        [Description("Enable smart autocomplete for filter values")]
        [DefaultValue(true)]
        public bool AutocompleteEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets whether filter validation is enabled
        /// </summary>
        [Category("Filter")]
        [Description("Enable validation of filter criteria")]
        [DefaultValue(true)]
        public bool ValidationEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to show filter count badge
        /// </summary>
        [Category("Filter")]
        [Description("Show badge with active filter count")]
        [DefaultValue(true)]
        public bool ShowFilterCountBadge { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to show validation indicators
        /// </summary>
        [Category("Filter")]
        [Description("Show visual indicators for validation errors/warnings")]
        [DefaultValue(true)]
        public bool ShowValidationIndicators { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to show column type icons
        /// </summary>
        [Category("Filter")]
        [Description("Show icons indicating column data types")]
        [DefaultValue(true)]
        public bool ShowColumnTypeIcons { get; set; } = true;

        /// <summary>
        /// Gets or sets the data source for autocomplete suggestions
        /// </summary>
        [Browsable(false)]
        public object AutocompleteDataSource
        {
            get => _autocompleteDataSource;
            set
            {
                _autocompleteDataSource = value;
                _suggestionProvider?.SetDataSource(value);
            }
        }

        #endregion
    }

    /// <summary>
    /// Column information for filter dropdowns
    /// </summary>
    public class FilterColumnInfo
    {
        public string ColumnName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public Type? DataType { get; set; }
        public bool IsFilterable { get; set; } = true;
    }
}
