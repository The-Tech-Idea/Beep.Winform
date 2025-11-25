using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    public partial class BeepSideBar
    {
        #region Painter Fields
        private ISideBarPainter _currentPainter;
        private BeepControlStyle _style = BeepControlStyle.Material3;
        private SideBarPainterContextAdapter _painterContext;
        #endregion

        #region Painter Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual Style/painter to use for rendering the sidebar.")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle Style
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;
                    InitializePainter();
                    Invalidate();
                }
            }
        }
        #endregion

        #region Painter Initialization
        private void InitializePainter()
        {
            _currentPainter = _style switch
            {
                BeepControlStyle.Material3 => new Painters.Material3SideBarPainter(),
                BeepControlStyle.iOS15 => new Painters.iOS15SideBarPainter(),
                BeepControlStyle.Fluent2 => new Painters.Fluent2SideBarPainter(),

                // Consolidate Minimal into NotionMinimal to reduce duplication
                BeepControlStyle.Minimal => new Painters.NotionMinimalSideBarPainter(),
                BeepControlStyle.AntDesign => new Painters.AntDesignSideBarPainter(),
                BeepControlStyle.MaterialYou => new Painters.MaterialYouSideBarPainter(),
                BeepControlStyle.Windows11Mica => new Painters.Windows11MicaSideBarPainter(),
                BeepControlStyle.MacOSBigSur => new Painters.MacOSBigSurSideBarPainter(),
                BeepControlStyle.ChakraUI => new Painters.ChakraUISideBarPainter(),
                BeepControlStyle.TailwindCard => new Painters.TailwindCardSideBarPainter(),
                BeepControlStyle.NotionMinimal => new Painters.NotionMinimalSideBarPainter(),
                BeepControlStyle.VercelClean => new Painters.VercelCleanSideBarPainter(),
                BeepControlStyle.StripeDashboard => new Painters.StripeDashboardSideBarPainter(),
                BeepControlStyle.DarkGlow => new Painters.DarkGlowSideBarPainter(),
                BeepControlStyle.DiscordStyle => new Painters.DiscordStyleSideBarPainter(),
                BeepControlStyle.GradientModern => new Painters.GradientModernSideBarPainter(),

                // Newly added painters
                BeepControlStyle.FinSet => new Painters.FinSetSideBarPainter(),
                BeepControlStyle.PillRail => new Painters.PillRailSideBarPainter(),

                _ => new Painters.Material3SideBarPainter(),
            };

            _painterContext = new SideBarPainterContextAdapter(this);
        }
        #endregion

        #region Painter Context Adapter
        /// <summary>
        /// Adapter class that exposes BeepSideBar's internal state to painters
        /// </summary>
        private class SideBarPainterContextAdapter : ISideBarPainterContext
        {
            private readonly BeepSideBar _sideBar;

            public SideBarPainterContextAdapter(BeepSideBar sideBar)
            {
                _sideBar = sideBar ?? throw new ArgumentNullException(nameof(sideBar));
            }

            public Graphics Graphics => _sideBar._currentGraphics;
            public Rectangle Bounds => _sideBar.ClientRectangle;
            public Rectangle DrawingRect => _sideBar.DrawingRect;
            public string ThemeName => _sideBar.Theme;
            public IBeepTheme Theme => _sideBar._currentTheme;
            public bool UseThemeColors => _sideBar.UseThemeColors;
            public Color AccentColor => _sideBar.AccentColor;
            public Color BackColor => _sideBar.BackColor;
            public BindingList<SimpleItem> Items => _sideBar._items;
            public SimpleItem SelectedItem => _sideBar._selectedItem;
            public SimpleItem HoveredItem => _sideBar._hoveredItem;
            public Dictionary<SimpleItem, bool> ExpandedState => _sideBar._expandedState;
            public bool IsCollapsed => _sideBar._isCollapsed;
            public bool IsAnimating => _sideBar._isAnimating;
            public int ItemHeight => _sideBar._itemHeight;
            public int ChildItemHeight => _sideBar._childItemHeight;
            public int ExpandedWidth => _sideBar._expandedWidth;
            public int CollapsedWidth => _sideBar._collapsedWidth;
            public int IndentationWidth => _sideBar._indentationWidth;
            public int ChromeCornerRadius => _sideBar.ChromeCornerRadius;
            public bool EnableRailShadow => _sideBar.EnableRailShadow;
            public bool IsEnabled => _sideBar.Enabled;
            public bool ShowToggleButton => _sideBar._showToggleButton;
            public bool UseExpandCollapseIcon => _sideBar.UseExpandCollapseIcon;
            public string ExpandIconPath => _sideBar.ExpandIconPath;
            public string CollapseIconPath => _sideBar.CollapseIconPath;
            public string DefaultImagePath => _sideBar.DefaultItemImagePath;
            public TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle ControlStyle => _sideBar.Style;
        }
        #endregion
    }
}
