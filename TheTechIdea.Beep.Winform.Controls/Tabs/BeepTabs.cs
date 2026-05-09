using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Tabs.Hosts;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;
using TheTechIdea.Beep.Winform.Controls.Tabs.Painters;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Helpers;
 
namespace TheTechIdea.Beep.Winform.Controls
{
    public enum TabHeaderPosition { Top, Bottom, Left, Right }

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepTabs))]
    [Category("Beep Controls")]
    [DisplayName("Beep Tabs")]
    [Description("A fully custom tab control with themed headers and SVG close buttons.")]
    public partial class BeepTabs : BaseControl
    {
        public new event EventHandler? SelectedIndexChanged;

        /// <summary>
        /// Raised after a closed tab is restored via <see cref="TryReopenLastClosedTab"/>
        /// (Ctrl+Shift+T). The handler can use the record to refresh the tab's content.
        /// </summary>
        public event EventHandler<BeepTabReopenEventArgs>? TabReopenRequested;

        /// <summary>
        /// Raised before a dirty (unsaved) tab is closed in Documents or Workspace mode.
        /// Set <see cref="System.ComponentModel.CancelEventArgs.Cancel"/> to <see langword="true"/>
        /// to keep the tab open (e.g. show a save dialog first).
        /// </summary>
        public event EventHandler<BeepTabCloseRequestedEventArgs>? TabCloseRequested;

        // New: toggle showing close buttons on tab headers
        private bool _showCloseButtons = true;
        private ITabPainter _painter;
        public IBeepTheme CurrentTheme => _currentTheme;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int TabCount => GetHostedSourceItemCount();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int SelectedIndex
        {
            get => GetHostedSourceSelectedIndex();
            set => TrySelectHostedSourceItem(value);
        }

        private int _minTouchTargetWidth = 44;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Minimum touch target width for tabs in pixels (minimum 32).")]
        [DefaultValue(44)]
        public int MinTouchTargetWidth
        {
            get => _minTouchTargetWidth;
            set
            {
                int normalizedValue = Math.Max(32, value);
                if (_minTouchTargetWidth == normalizedValue) return;
                _minTouchTargetWidth = normalizedValue;
                RefreshHeaderLayoutState(updateItemSize: false);
            }
        }

        private TabLabelVisibility _tabTextVisibility = TabLabelVisibility.Always;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Controls when tab text labels are visible.")]
        [DefaultValue(TabLabelVisibility.Always)]
        public TabLabelVisibility TabTextVisibility
        {
            get => _tabTextVisibility;
            set
            {
                if (_tabTextVisibility == value) return;
                _tabTextVisibility = value;
                RefreshHeaderLayoutState();
            }
        }

        private bool _isPopupOpen;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPopupOpen => _isPopupOpen;

        public event EventHandler? PopupOpened;
        public event EventHandler? PopupClosed;

        public void CloseChildPopup()
        {
            if (!_isPopupOpen) return;
            _isPopupOpen = false;
            PopupClosed?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        protected void OnPopupOpened()
        {
            _isPopupOpen = true;
            PopupOpened?.Invoke(this, EventArgs.Empty);
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("If false, the close button is hidden and tabs cannot be closed from the header.")]
        [DefaultValue(true)]
        public bool ShowCloseButtons
        {
            get => _showCloseButtons;
            set
            {
                if (_showCloseButtons == value) return;
                _showCloseButtons = value;
                RefreshHeaderLayoutState();
            }
        }

     
        public event EventHandler<TabRemovedEventArgs> TabRemoved;
        
        protected IBeepTheme _currentTheme = BeepThemesManager.GetDefaultTheme();
        private string _theme;
        [Browsable(true)]
        [TypeConverter(typeof(ThemeEnumConverter))]
        public string Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                _currentTheme = BeepThemesManager.GetTheme(value);
                ApplyTheme();
            }
        }

        private int _headerHeight = 30;
        private TabStyle _tabStyle = TabStyle.Classic;
        private BeepTabMode _tabMode = BeepTabMode.Navigation;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Tab visual style: Classic, Underline, Capsule, Minimal, Segmented.")]
        [DefaultValue(TabStyle.Classic)]
        public TabStyle TabStyle
        {
            get => _tabStyle;
            set
            {
                if (value == _tabStyle) return;
                // Start transition from current style to new
                StartStyleTransition(_tabStyle, value);
                _tabStyle = value;
                UpdatePainter();
                RefreshHeaderLayoutState();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Controls whether the tab surface behaves like navigation tabs, document tabs, or a workspace.")]
        [DefaultValue(BeepTabMode.Navigation)]
        public BeepTabMode TabMode
        {
            get => _tabMode;
            set
            {
                if (_tabMode == value)
                {
                    return;
                }

                _tabMode = value;
                ResetWorkspaceMruCycle();
                ClearClosedTabHistory();
                UpdateLayout();
                UpdateItemSize();
                Invalidate();
            }
        }


        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(30)]
        [Description("The size of the custom header area. For horizontal headers, this is the height; for vertical, the width.")]
        public int HeaderHeight
        {
            get => _headerHeight;
            set
            {
                int normalizedValue = Math.Max(10, value);
                if (_headerHeight == normalizedValue) return;
                _headerHeight = normalizedValue;
                RefreshHeaderLayoutState();
            }
        }

        private TabHeaderPosition _headerPosition = TabHeaderPosition.Top;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(TabHeaderPosition.Top)]
        [Description("The position of the tab header (Top, Bottom, Left, or Right).")]
        public TabHeaderPosition HeaderPosition
        {
            get => _headerPosition;
            set
            {
                if (_headerPosition == value) return;
                _headerPosition = value;
                RefreshHeaderLayoutState();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTabItem? SelectedTabItem
        {
            get => GetHostedSourceSelectedItemSnapshot();
            set
            {
                if (value == null)
                {
                    return;
                }

                if (TrySelectHostedSourceItem(value.Index))
                {
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectTabByIndex
        {
            set
            {
                if (TrySelectHostedSourceItem(value))
                {
                    Invalidate();
                }
            }
        }

        // Replace hardcoded constants with DPI-aware properties
        private int GetScaledCloseButtonSize() => DpiScalingHelper.ScaleValue(24, this);
        private int GetScaledCloseButtonPadding() => DpiScalingHelper.ScaleValue(8, this);
        private int GetScaledTextPadding() => DpiScalingHelper.ScaleValue(12, this);
        private int GetScaledMinTabWidth() => DpiScalingHelper.ScaleValue(60, this);
        private int GetScaledMaxTabWidth() => DpiScalingHelper.ScaleValue(250, this);
        private int GetScaledMinTabHeight() => DpiScalingHelper.ScaleValue(60, this);
        private int GetScaledMaxTabHeight() => DpiScalingHelper.ScaleValue(250, this);
        private int GetScaledHeaderHeight() => DpiScalingHelper.ScaleValue(_headerHeight, this);

        // Keep original constants for reference
        private const int CloseButtonSize = 16;
        private const int CloseButtonPadding = 8;
        private const int TextPadding = 12;
        private const int MinTabWidth = 60;
        private const int MaxTabWidth = 250;
        private const int MinTabHeight = 60;
        private const int MaxTabHeight = 250;
        private Size _itemSize = new Size(120, 30);

        private BeepImage closeIcon;
        private BeepTabContentHost? _contentHost;

        protected override bool UseBaseMouseInputRouting => false;

        public BeepTabs()
        {
            InitializeControlDefaults();
            InitializeRuntimeAssets();
            WireControlEvents();
            InitializeAccessibilityMetadata();
            ApplyTheme();
            UpdatePainter();
        }

        /// <summary>
        /// Apply TabStyle preset to this tabs control
        /// </summary>
        public void SetTabStylePreset(TheTechIdea.Beep.Winform.Controls.TabStyle style)
        {
            TheTechIdea.Beep.Winform.Controls.Styling.TabStylePresets.ApplyPreset(this, style);
        }

        public int LastTabSelected { get; private set; }

        internal Size ItemSize
        {
            get => _itemSize;
            set => _itemSize = value;
        }

        internal int GetHostedItemCount()
        {
            return GetHostedSourceItemCount();
        }

        internal string GetTabTitle(int index)
        {
            return GetHostedSourceItemTitle(index);
        }

        internal BeepTabHeaderLayoutSnapshot CreateRuntimeLayoutSnapshot()
        {
            return BeepTabLayoutHelper.CreateSnapshot(this, GetHostedSourceItemsSnapshot());
        }

        public new Rectangle GetTabRect(int index)
        {
            if (index < 0 || !IsHandleCreated)
            {
                return Rectangle.Empty;
            }

            using Graphics graphics = CreateGraphics();
            var headerRects = GetCurrentHeaderTabRects(graphics);
            if (index >= headerRects.Count)
            {
                return Rectangle.Empty;
            }

            return Rectangle.Ceiling(headerRects[index]);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _underlineTimer?.Stop();
                _underlineTimer?.Dispose();
                _underlineTimer = null;
                _styleTransitionTimer?.Stop();
                _styleTransitionTimer?.Dispose();
                _styleTransitionTimer = null;
                // Dispose the context menu if it is still open (e.g. control is destroyed
                // while the menu is showing — avoids a GDI handle leak).
                DisposeHeaderTabContextMenu();
                closeIcon?.Dispose();
                closeIcon = null;
            }
            base.Dispose(disposing);
        }

    }

    public class TabRemovedEventArgs : EventArgs
    {
        public string? TabText { get; set; }
    }
}