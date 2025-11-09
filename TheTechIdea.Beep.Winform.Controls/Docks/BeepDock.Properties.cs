using System;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Docks.Painters;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDock - Properties
    /// </summary>
    public partial class BeepDock
    {
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
