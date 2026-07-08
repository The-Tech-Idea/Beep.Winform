// BeepMenuBar.Properties.cs
// Phase 02 — Partial-Class Split.
//
// Owns the public `[Browsable]` surface of BeepMenuBar plus the backing
// fields they read/write. Layout/Drawing/Input partials share these
// fields via the partial-class merge.
//
// See .plans/Menus-Phase-02-PartialClassSplit.md.
// ─────────────────────────────────────────────────────────────────────────────
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMenuBar
    {
        // ─────────────────────────────────────────────────────────────────
        // Backing fields shared across partials
        // ─────────────────────────────────────────────────────────────────

        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        private BindingList<SimpleItem> currentMenu = new BindingList<SimpleItem>();

        private int _selectedIndex = -1;

        // DPI-aware default values (scaled at use-site through ScaleUi).
        private int _menuItemWidth = 60;
        private int _imagesize = 20;
        private int _menuItemHeight = 32;
        private bool _menuItemHeightLocked = false;
        private bool _heightManuallySet = false;

        private Font _textFont = SystemFonts.DefaultFont; // Overridden by ApplyTheme()
        private bool _explicitTextFont = false;

        private SimpleItem _selectedItem;

        // ─────────────────────────────────────────────────────────────────
        // DPI helpers — kept private; all sizing flows through these so the
        // partials never see raw pixel literals.
        // ─────────────────────────────────────────────────────────────────

        private int ScaledMenuItemHeight => DpiScalingHelper.ScaleValue(MenuItemHeight, this);
        private int ScaledImageSize      => DpiScalingHelper.ScaleValue(_imagesize, this);
        private int ScaledMenuItemWidth  => DpiScalingHelper.ScaleValue(_menuItemWidth, this);
        private int ScaleUi(int value)   => DpiScalingHelper.ScaleValue(value, this);

        // ─────────────────────────────────────────────────────────────────
        // Public properties
        // ─────────────────────────────────────────────────────────────────

        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                _textFont = value;
                UseThemeFont = false;
                _explicitTextFont = true;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> MenuItems
        {
            get => items;
            set
            {
                items = value;
                RefreshHitAreas();
                Invalidate();
            }
        }

        public event EventHandler<SelectedItemChangedEventArgs> MenuItemSelected;
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        protected virtual void OnMenuItemSelected(SimpleItem selectedItem)
        {
            MenuItemSelected?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }

        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }

        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            private set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnSelectedItemChanged(_selectedItem);
                }
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0)
                {
                    _selectedIndex = value;
                    if (currentMenu.Count > 0)
                    {
                        SelectedItem = currentMenu[value];
                    }
                }
            }
        }

        public int MenuItemHeight
        {
            get => _menuItemHeight;
            set
            {
                // Always honour an explicit set by the developer; unlock if locked.
                if (_menuItemHeightLocked && _menuItemHeight != value)
                {
                    _menuItemHeightLocked = false;
                }
                if (_menuItemHeight != value)
                {
                    _menuItemHeight = value;
                    if (!_heightManuallySet)
                    {
                        int verticalBuffer = ScaleUi(12);
                        int newHeight = ScaledMenuItemHeight + verticalBuffer;
                        if (base.Height != newHeight)
                        {
                            base.Height = newHeight;
                        }
                    }
                    RefreshHitAreas();
                    Invalidate();
                }
            }
        }

        [Browsable(true), Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MenuItemWidth
        {
            get => _menuItemWidth;
            set
            {
                if (value > 0)
                {
                    _menuItemWidth = value;
                    RefreshHitAreas();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ImageSize
        {
            get => _imagesize;
            set
            {
                if (value > 0)
                {
                    // Cap image size so it fits inside the menu item with padding.
                    int maxSize = Math.Max(16, MenuItemHeight - 4);
                    _imagesize = Math.Min(value, maxSize);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the menu bar. When set manually by
        /// the developer, this value is preserved and will not be
        /// overridden by automatic layout calculations.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("Height of the menu bar. When set manually, this value is preserved.")]
        public new int Height
        {
            get => base.Height;
            set
            {
                if (base.Height != value)
                {
                    base.Height = value;
                    _heightManuallySet = true;
                }
            }
        }
    }
}
