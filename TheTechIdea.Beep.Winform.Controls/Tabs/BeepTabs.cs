using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Tabs.Hosts;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;
using TheTechIdea.Beep.Winform.Controls.Tabs.Painters;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum TabHeaderPosition { Top, Bottom, Left, Right }

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepTabs))]
    [Category("Beep Controls")]
    [DisplayName("Beep Tabs")]
    [Description("A fully custom tab control with themed headers and SVG close buttons.")]
    public partial class BeepTabs : ContainerControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.TabsStrip;
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

        // MinTouchTargetWidth was declared here and offered in the designer. It had exactly one
        // reader — BeepTabLayoutHelper copied it into BeepTabRenderContext.MinTouchTargetWidth,
        // which nothing consumed. A dead chain rather than an unread property, but the value never
        // influenced anything either way.
        //
        // It went with BeepTabHeaderHost.Touch.cs, whose ExpandToMinTouchTarget / TouchHitTestTabIndex
        // / MeetsTouchTarget / ScaleTouchTarget had no callers at all: the live hit test (TryHitTab)
        // uses the painted bounds directly. That API is also unsound for this control — tabs in a run
        // are contiguous, so centring an expansion on each one makes neighbours overlap, and
        // TouchHitTestTabIndex returns the first match, which would make the left edge of a tab
        // select its neighbour.
        //
        // Touch ergonomics for a tab strip is a HeaderHeight concern, not a hit-rect one: the default
        // header is 30px against WCAG 2.5.5's 44dip guidance. Raising HeaderHeight (already a public
        // property) is the mechanism that actually works.

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
        
        private string _themeName = string.Empty;
        protected IBeepTheme _currentTheme = BeepThemesManager.CurrentTheme;

        [Browsable(true)]
        [Category("Appearance")]
        [TypeConverter(typeof(ThemeEnumConverter))]
        public string Theme
        {
            get => _themeName;
            set
            {
                _themeName = value;
                _currentTheme = BeepThemesManager.GetTheme(value) ?? BeepThemesManager.CurrentTheme;
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
                // Morph only when the control is visible: before the handle exists there is
                // nothing to see, and animating construction-time styling replayed a 220ms
                // Classic cross-fade on every form open (the probe caught it mid-blend).
                if (IsHandleCreated)
                {
                    StartStyleTransition(_tabStyle, value);
                }
                _tabStyle = value;
                UpdatePainter();
                RefreshHeaderLayoutState();
            }
        }

        /// <summary>
        /// What the current <see cref="TabMode"/> allows. Every mode-dependent decision in this
        /// control resolves through here, so the contract is stated once instead of being
        /// reconstructed from twenty scattered <c>TabMode == BeepTabMode.Navigation</c> checks.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTabModeCapabilities ModeCapabilities => BeepTabModeCapabilities.For(_tabMode);

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
        private Font? _textFont;
        private List<RectangleF> _cachedHeaderTabRects = new List<RectangleF>();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font? TextFont => _textFont;

        public BeepTabs()
        {
            InitializeControlDefaults();
            InitializeRuntimeAssets();
            WireControlEvents();
            InitializeAccessibilityMetadata();
            // ContainerControl, not BaseControl: global theme changes must be followed by
            // this control itself, or every BeepTabs keeps its construction-time palette.
            BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;
            ApplyTheme();
            UpdatePainter();
        }

        private void OnGlobalThemeChanged(object? sender, EventArgs e)
        {
            if (IsDisposed) return;
            _themeName = BeepThemesManager.CurrentThemeName;
            _currentTheme = BeepThemesManager.CurrentTheme;
            ApplyTheme();
            Invalidate();
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
                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
                _underlineTimer?.Stop();
                _underlineTimer?.Dispose();
                _underlineTimer = null;
                _styleTransitionTimer?.Stop();
                _styleTransitionTimer?.Dispose();
                _styleTransitionTimer = null;
                // Dispose the context menu if it is still open (e.g. control is destroyed
                // while the menu is showing â€” avoids a GDI handle leak).
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