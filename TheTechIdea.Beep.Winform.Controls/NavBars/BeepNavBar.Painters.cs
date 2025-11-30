using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.NavBars.Painters;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.NavBars
{
    /// <summary>
    /// Partial class for BeepNavBar painter integration
    /// </summary>
    public partial class BeepNavBar
    {
        #region Painter Fields
        private BeepControlStyle _style = BeepControlStyle.Material3;
        private INavBarPainter _currentPainter;
        private int _hoveredItemIndex = -1;
        private Dictionary<string, (Rectangle rect, Action action)> _hitAreas = 
            new Dictionary<string, (Rectangle, Action)>();
        #endregion

        #region Painter Properties
        [Category("Appearance")]
        [Description("The visual Style painter for the navigation bar")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle Style
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;
                    _layoutDirty = true;
                    if (!IsDesignModeSafe)
                    {
                        InitializePainter();
                    }
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public INavBarPainter CurrentPainter => _currentPainter;

        [Browsable(false)]
        public int HoveredItemIndex => _hoveredItemIndex;
        #endregion

        #region Painter Initialization
        private void InitializePainter()
        {
            _currentPainter = _style switch
            {
                BeepControlStyle.Material3 => new Material3NavBarPainter(),
                BeepControlStyle.iOS15 => new iOS15NavBarPainter(),
                BeepControlStyle.AntDesign => new AntDesignNavBarPainter(),
                BeepControlStyle.Fluent2 => new Fluent2NavBarPainter(),
                BeepControlStyle.MaterialYou => new MaterialYouNavBarPainter(),
                BeepControlStyle.Windows11Mica => new Windows11MicaNavBarPainter(),
                BeepControlStyle.MacOSBigSur => new MacOSBigSurNavBarPainter(),
                BeepControlStyle.ChakraUI => new ChakraUINavBarPainter(),
                BeepControlStyle.TailwindCard => new TailwindCardNavBarPainter(),
                BeepControlStyle.NotionMinimal => new NotionMinimalNavBarPainter(),
                BeepControlStyle.Minimal => new MinimalNavBarPainter(),
                BeepControlStyle.VercelClean => new VercelCleanNavBarPainter(),
                BeepControlStyle.StripeDashboard => new StripeDashboardNavBarPainter(),
                BeepControlStyle.DarkGlow => new DarkGlowNavBarPainter(),
                BeepControlStyle.DiscordStyle => new DiscordStyleNavBarPainter(),
                BeepControlStyle.GradientModern => new GradientModernNavBarPainter(),
                _ => new Material3NavBarPainter()
            };

            RefreshHitAreas();
        }
        #endregion

        #region Hit Area Management
        private void RefreshHitAreas()
        {
            // Skip during design-time to prevent flickering
            if (IsDesignModeSafe)
                return;
                
            _hitAreas.Clear();

            if (_currentPainter != null && DrawingRect.Width > 0 && DrawingRect.Height > 0)
            {
                // Create a temporary adapter to pass to painters
                var adapter = new BeepNavBarAdapter(this);
                _currentPainter.UpdateHitAreas(adapter, DrawingRect, (name, rect, action) =>
                {
                    _hitAreas[name] = (rect, action);
                });
                _layoutDirty = false;
            }
        }

        private void UpdateHoverState(Point mouseLocation)
        {
            int previousHover = _hoveredItemIndex;
            _hoveredItemIndex = -1;

            foreach (var kvp in _hitAreas)
            {
                if (kvp.Value.rect.Contains(mouseLocation))
                {
                    if (kvp.Key.StartsWith("NavItem_") && 
                        int.TryParse(kvp.Key.Substring(8), out int index))
                    {
                        _hoveredItemIndex = index;
                        Cursor = System.Windows.Forms.Cursors.Hand;
                        break;
                    }
                }
            }

            if (_hoveredItemIndex < 0)
            {
                Cursor = System.Windows.Forms.Cursors.Default;
            }

            if (previousHover != _hoveredItemIndex)
            {
                Invalidate();
            }
        }

        private bool HandleHitAreaClick(Point mouseLocation)
        {
            foreach (var kvp in _hitAreas)
            {
                if (kvp.Value.rect.Contains(mouseLocation))
                {
                    kvp.Value.action?.Invoke();
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Mouse Events
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            // Skip during design-time
            if (IsDesignModeSafe)
                return;
                
            UpdateHoverState(e.Location);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            
            // Skip during design-time
            if (IsDesignModeSafe)
                return;
                
            if (_hoveredItemIndex != -1)
            {
                _hoveredItemIndex = -1;
                Cursor = System.Windows.Forms.Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            // Skip during design-time
            if (IsDesignModeSafe)
                return;
                
            HandleHitAreaClick(e.Location);
        }
        #endregion

        #region Adapter Class
        /// <summary>
        /// Adapter class to expose BeepNavBar properties to painters
        /// </summary>
        private class BeepNavBarAdapter : INavBarPainterContext
        {
            private readonly BeepNavBar _navBar;

            public BeepNavBarAdapter(BeepNavBar navBar)
            {
                _navBar = navBar;
            }

            public BindingList<SimpleItem> Items => _navBar.Items;
            public SimpleItem SelectedItem => _navBar.SelectedItem;
            public int HoveredItemIndex => _navBar.HoveredItemIndex;
            public Color AccentColor => _navBar.AccentColor;
            public bool UseThemeColors => _navBar.UseThemeColors;
            public bool EnableShadow => _navBar.EnableShadow;
            public int CornerRadius => _navBar.CornerRadius;
            public int ItemWidth => _navBar.NavItemWidth;
            public int ItemHeight => _navBar.NavItemHeight;
            public NavBarOrientation Orientation => _navBar.Orientation;
            public IBeepTheme Theme => _navBar._currentTheme;

            public void SelectItemByIndex(int index)
            {
                _navBar.SelectNavItemByIndex(index);
            }
        }
        #endregion
    }
}
