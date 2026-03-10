using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Docks.Painters;
using TheTechIdea.Beep.Winform.Controls.Docks.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDock - Properties
    /// </summary>
    public partial class BeepDock
    {
        private DockStyleConfig _styleProfile = new DockStyleConfig();
        private DockColorConfig _colorProfile = new DockColorConfig();

        #region Properties
        /// <summary>
        /// Gets the dock configuration
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockConfig Config => _config;

        /// <summary>
        /// Gets or sets the visual style of the dock
        /// </summary>
        [Category("Beep Dock")]
        [Description("Visual style of the dock")]
        [DefaultValue(Docks.DockStyle.AppleDock)]
        public Docks.DockStyle DockStyleType
        {
            get => _config.Style;
            set
            {
                if (_config.Style != value)
                {
                    _config.Style = value;
                    _dockPainter = DockPainterFactory.GetPainter(value);
                    _config.ItemSize = Docks.Helpers.DockStyleHelpers.GetRecommendedItemSize(value);
                    _config.DockHeight = Docks.Helpers.DockStyleHelpers.GetRecommendedDockHeight(value);
                    _config.Spacing = Docks.Helpers.DockStyleHelpers.GetRecommendedSpacing(value);
                    _config.Padding = Docks.Helpers.DockStyleHelpers.GetRecommendedPadding(value);
                    _config.MaxScale = Docks.Helpers.DockStyleHelpers.GetRecommendedMaxScale(value);
                    _config.ShowShadow = Docks.Helpers.DockStyleHelpers.ShouldShowShadow(value);
                    _config.BackgroundOpacity = Docks.Helpers.DockStyleHelpers.GetRecommendedBackgroundOpacity(value);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of dock items
        /// </summary>
        [Category("Beep Dock")]
        [Description("Size of dock items in pixels")]
        [DefaultValue(56)]
        public int ItemSize
        {
            get => _config.ItemSize;
            set
            {
                if (_config.ItemSize != value)
                {
                    _config.ItemSize = value;
                    UpdateItemBounds();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the dock container
        /// </summary>
        [Category("Beep Dock")]
        [Description("Height of the dock container")]
        [DefaultValue(72)]
        public int DockHeight
        {
            get => _config.DockHeight;
            set
            {
                if (_config.DockHeight != value)
                {
                    _config.DockHeight = value;
                    UpdateItemBounds();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the spacing between dock items
        /// </summary>
        [Category("Beep Dock")]
        [Description("Spacing between dock items")]
        [DefaultValue(8)]
        public int ItemSpacing
        {
            get => _config.Spacing;
            set
            {
                if (_config.Spacing != value)
                {
                    _config.Spacing = value;
                    UpdateItemBounds();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum scale factor for hovered items
        /// </summary>
        [Category("Beep Dock")]
        [Description("Maximum scale factor for hovered items")]
        [DefaultValue(1.5f)]
        public float MaxScale
        {
            get => _config.MaxScale;
            set
            {
                if (_config.MaxScale != value)
                {
                    _config.MaxScale = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the position of the dock
        /// </summary>
        [Category("Beep Dock")]
        [Description("Position of the dock")]
        [DefaultValue(Docks.DockPosition.Bottom)]
        public Docks.DockPosition DockPositionType
        {
            get => _config.Position;
            set
            {
                if (_config.Position != value)
                {
                    _config.Position = value;
                    UpdateItemBounds();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the dock
        /// </summary>
        [Category("Beep Dock")]
        [Description("Orientation of the dock (horizontal or vertical)")]
        [DefaultValue(Docks.DockOrientation.Horizontal)]
        public Docks.DockOrientation DockOrientationType
        {
            get => _config.Orientation;
            set
            {
                if (_config.Orientation != value)
                {
                    _config.Orientation = value;
                    UpdateItemBounds();
                    Invalidate();
                }
            }
        }

        [Category("Beep Dock")]
        [Description("Alignment of dock items within the control.")]
        [DefaultValue(Docks.DockAlignment.Center)]
        public Docks.DockAlignment DockAlignmentType
        {
            get => _config.Alignment;
            set
            {
                if (_config.Alignment != value)
                {
                    _config.Alignment = value;
                    UpdateItemBounds();
                    Invalidate();
                }
            }
        }

        [Category("Beep Dock")]
        [Description("Animation style used for item interactions.")]
        [DefaultValue(Docks.DockAnimationStyle.Spring)]
        public Docks.DockAnimationStyle AnimationStyle
        {
            get => _config.AnimationStyle;
            set
            {
                if (_config.AnimationStyle != value)
                {
                    _config.AnimationStyle = value;
                    Invalidate();
                }
            }
        }

        [Category("Beep Dock")]
        [Description("Controls dock icon rendering mode.")]
        [DefaultValue(Docks.DockIconMode.IconOnly)]
        public Docks.DockIconMode IconMode
        {
            get => _config.IconMode;
            set
            {
                if (_config.IconMode != value)
                {
                    _config.IconMode = value;
                    Invalidate();
                }
            }
        }

        [Category("Beep Dock")]
        [Description("Indicator visual style for selected/running items.")]
        [DefaultValue(Docks.DockIndicatorStyle.Dot)]
        public Docks.DockIndicatorStyle IndicatorStyle
        {
            get => _config.IndicatorStyle;
            set
            {
                if (_config.IndicatorStyle != value)
                {
                    _config.IndicatorStyle = value;
                    Invalidate();
                }
            }
        }

        [Category("Beep Dock")]
        [Description("Show per-item tooltips on hover.")]
        [DefaultValue(true)]
        public bool ShowTooltips
        {
            get => _config.ShowTooltips;
            set
            {
                if (_config.ShowTooltips != value)
                {
                    _config.ShowTooltips = value;
                    if (!value)
                    {
                        HideDockTooltip();
                    }
                    Invalidate();
                }
            }
        }

        [Category("Beep Dock")]
        [Description("Show notification badges on items.")]
        [DefaultValue(false)]
        public bool ShowBadges
        {
            get => _config.ShowBadges;
            set
            {
                if (_config.ShowBadges != value)
                {
                    _config.ShowBadges = value;
                    Invalidate();
                }
            }
        }

        [Category("Beep Dock")]
        [Description("Show dock border.")]
        [DefaultValue(true)]
        public bool ShowBorder
        {
            get => _config.ShowBorder;
            set
            {
                if (_config.ShowBorder != value)
                {
                    _config.ShowBorder = value;
                    Invalidate();
                }
            }
        }

        [Category("Beep Dock")]
        [Description("Show dock shadow.")]
        [DefaultValue(true)]
        public bool ShowShadow
        {
            get => _config.ShowShadow;
            set
            {
                if (_config.ShowShadow != value)
                {
                    _config.ShowShadow = value;
                    Invalidate();
                }
            }
        }

        [Category("Beep Dock")]
        [Description("Show separators between dock items.")]
        [DefaultValue(Docks.DockSeparatorStyle.None)]
        public Docks.DockSeparatorStyle SeparatorStyle
        {
            get => _config.SeparatorStyle;
            set
            {
                if (_config.SeparatorStyle != value)
                {
                    _config.SeparatorStyle = value;
                    Invalidate();
                }
            }
        }

        [Category("Beep Dock")]
        [Description("Color for separators between items.")]
        public Color SeparatorColor
        {
            get => _config.SeparatorColor;
            set
            {
                if (_config.SeparatorColor != value)
                {
                    _config.SeparatorColor = value;
                    Invalidate();
                }
            }
        }

        [Category("Beep Dock")]
        [Description("Enable item dragging.")]
        [DefaultValue(false)]
        public bool EnableDrag
        {
            get => _config.EnableDrag;
            set => _config.EnableDrag = value;
        }

        [Category("Beep Dock")]
        [Description("Enable item reordering.")]
        [DefaultValue(false)]
        public bool EnableReorder
        {
            get => _config.EnableReorder;
            set => _config.EnableReorder = value;
        }

        [Category("Beep Dock")]
        [Description("Automatically hide dock when idle.")]
        [DefaultValue(false)]
        public bool AutoHide
        {
            get => _config.AutoHide;
            set => _config.AutoHide = value;
        }

        [Category("Beep Dock")]
        [Description("Enable overflow affordance when item count exceeds available space.")]
        [DefaultValue(true)]
        public bool EnableOverflow
        {
            get => _config.EnableOverflow;
            set
            {
                if (_config.EnableOverflow != value)
                {
                    _config.EnableOverflow = value;
                    UpdateItemBounds();
                    Invalidate();
                }
            }
        }

        [Category("Beep Dock")]
        [Description("Style profile used to apply recommended dock metrics and style behavior.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DockStyleConfig StyleProfile
        {
            get => _styleProfile;
            set
            {
                if (value != null)
                {
                    _styleProfile = value;
                    ApplyStyleProfile(_styleProfile);
                }
            }
        }

        [Category("Beep Dock")]
        [Description("Color profile used for dock background, indicator, and separators.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DockColorConfig ColorProfile
        {
            get => _colorProfile;
            set
            {
                if (value != null)
                {
                    _colorProfile = value;
                    ApplyColorProfile(_colorProfile);
                }
            }
        }

        /// <summary>
        /// Gets or sets the collection of dock items
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("Collection of dock items")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new BindingList<SimpleItem> Items
        {
            get => _items;
            set
            {
                if (value != null && value != _items)
                {
                    _items.ListChanged -= Items_ListChanged;
                    _items.Clear();
                    foreach (var item in value)
                        _items.Add(item);
                    _items.ListChanged += Items_ListChanged;
                    InitializeItems();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected item
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SimpleItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    _selectedIndex = _selectedItem != null ? _items.IndexOf(_selectedItem) : -1;
                    UpdateSelectionStates();
                    OnSelectedItemChanged(_selectedItem);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected item index
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0 && value < _items.Count)
                {
                    _selectedIndex = value;
                    SelectedItem = _items[_selectedIndex];
                }
                else
                {
                    _selectedIndex = -1;
                    SelectedItem = null;
                }
            }
        }
        #endregion
    }
}
