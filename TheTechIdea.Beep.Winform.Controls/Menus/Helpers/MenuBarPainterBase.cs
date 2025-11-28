using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Helpers
{
    /// <summary>
    /// Base class for menu bar painters providing common functionality
    /// </summary>
    public abstract class MenuBarPainterBase : IMenuBarPainter
    {
        #region Fields
        protected BaseControl Owner;
        protected IBeepTheme Theme;
        private bool _disposed = false;
        #endregion

        #region IMenuBarPainter Implementation
        public virtual void Initialize(BaseControl owner, IBeepTheme theme)
        {
            Owner = owner;
            Theme = theme;
            OnInitialized();
        }

        public abstract MenuBarContext AdjustLayout(Rectangle drawingRect, MenuBarContext ctx);
        public abstract void DrawBackground(Graphics g, MenuBarContext ctx);
        public abstract void DrawContent(Graphics g, MenuBarContext ctx);
        public abstract void DrawForegroundAccents(Graphics g, MenuBarContext ctx);
        public abstract void UpdateHitAreas(BaseControl owner, MenuBarContext ctx, Action<string, Rectangle> notifyAreaHit);

        public virtual Size CalculatePreferredSize(MenuBarContext ctx, Size proposedSize)
        {
            if (ctx.MenuItems == null || ctx.MenuItems.Count == 0)
            {
                return new Size(200, ctx.ItemHeight + 8); // Default size with padding
            }

            int totalWidth = 16; // Base padding
            int maxHeight = ctx.ItemHeight + 8; // Item height + padding

            foreach (var item in ctx.MenuItems)
            {
                int itemWidth = MenuBarRenderingHelpers.CalculateMenuItemWidth(item, ctx.ItemHeight, ctx.TextFont);
                totalWidth += itemWidth + ctx.ItemSpacing;
            }

            // Remove last spacing
            if (ctx.MenuItems.Count > 0)
            {
                totalWidth -= ctx.ItemSpacing;
            }

            // Apply size constraints
            int width = Math.Min(totalWidth, proposedSize.Width > 0 ? proposedSize.Width : int.MaxValue);
            int height = proposedSize.Height > 0 ? Math.Min(maxHeight, proposedSize.Height) : maxHeight;

            return new Size(width, height);
        }

        public virtual void ApplyTheme(IBeepTheme theme)
        {
            Theme = theme;
            OnThemeChanged();
        }
        #endregion

        #region Protected Helper Methods
        /// <summary>
        /// Called after initialization is complete
        /// </summary>
        protected virtual void OnInitialized() { }

        /// <summary>
        /// Called when theme changes
        /// </summary>
        protected virtual void OnThemeChanged() { }

        /// <summary>
        /// Adds a hit area to the owner control
        /// </summary>
        protected void AddHitAreaToOwner(string name, Rectangle rect, Action clickAction = null)
        {
            Owner?.AddHitArea(name, rect, null, clickAction);
        }

        /// <summary>
        /// Clears all hit areas from the owner control
        /// </summary>
        protected void ClearOwnerHitAreas()
        {
            Owner?.ClearHitList();
        }

        /// <summary>
        /// Checks if a specific area is currently being hovered
        /// </summary>
        protected bool IsAreaHovered(string areaName)
        {
            return Owner?.HitTestControl != null && 
                   Owner.HitTestControl.Name == areaName && 
                   Owner.HitTestControl.IsHovered;
        }

        /// <summary>
        /// Gets the name of the currently hovered area
        /// </summary>
        protected string GetHoveredAreaName()
        {
            return Owner?.HitTestControl?.Name ?? string.Empty;
        }

        /// <summary>
        /// Creates a rounded rectangle graphics path
        /// </summary>
        protected GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            return MenuBarRenderingHelpers.CreateRoundedPath(rect, radius);
        }

        /// <summary>
        /// Draws a hover effect for the specified rectangle
        /// </summary>
        protected void DrawHoverEffect(Graphics g, Rectangle rect, Color hoverColor, int cornerRadius = 0)
        {
            MenuBarRenderingHelpers.DrawHoverEffect(g, rect, hoverColor, cornerRadius);
        }

        /// <summary>
        /// Draws a selection indicator
        /// </summary>
        protected void DrawSelectionIndicator(Graphics g, Rectangle rect, Color selectionColor, 
            SelectionIndicatorStyle style = SelectionIndicatorStyle.Background)
        {
            MenuBarRenderingHelpers.DrawSelectionIndicator(g, rect, selectionColor, style);
        }

        /// <summary>
        /// Gets theme-appropriate colors with fallbacks
        /// </summary>
        protected Color GetThemeColor(Func<IBeepTheme, Color> colorSelector, Color fallback)
        {
            try
            {
                return Theme != null ? colorSelector(Theme) : fallback;
            }
            catch
            {
                return fallback;
            }
        }

        /// <summary>
        /// Gets background color from theme
        /// </summary>
        protected Color GetBackgroundColor()
        {
            return Color.Transparent;
            return GetThemeColor(t => t.MenuBackColor, Color.FromArgb(250, 250, 250));
        }

        /// <summary>
        /// Gets foreground color from theme
        /// </summary>
        protected Color GetForegroundColor()
        {
            return GetThemeColor(t => t.MenuForeColor, Color.Black);
        }

        /// <summary>
        /// Gets menu item background color from theme
        /// </summary>
        protected Color GetItemBackgroundColor()
        {
            return GetThemeColor(t => t.MenuBackColor, Color.White);
        }

        /// <summary>
        /// Gets menu item foreground color from theme
        /// </summary>
        protected Color GetItemForegroundColor()
        {
            return GetThemeColor(t => t.MenuItemForeColor, Color.Black);
        }

        /// <summary>
        /// Gets hover background color from theme
        /// </summary>
        protected Color GetHoverBackgroundColor()
        {
            return GetThemeColor(t => t.MenuItemHoverBackColor, Color.FromArgb(245, 245, 245));
        }

        /// <summary>
        /// Gets hover foreground color from theme
        /// </summary>
        protected Color GetHoverForegroundColor()
        {
            return GetThemeColor(t => t.MenuItemHoverForeColor, Color.Black);
        }

        /// <summary>
        /// Gets selected background color from theme
        /// </summary>
        protected Color GetSelectedBackgroundColor()
        {
            return GetThemeColor(t => t.MenuItemSelectedBackColor, Color.Empty);
        }

        /// <summary>
        /// Gets selected foreground color from theme
        /// </summary>
        protected Color GetSelectedForegroundColor()
        {
            return GetThemeColor(t => t.MenuItemSelectedForeColor, Color.White);
        }

        /// <summary>
        /// Gets disabled background color from theme
        /// </summary>
        protected Color GetDisabledBackgroundColor()
        {
            return GetThemeColor(t => t.DisabledBackColor, Color.FromArgb(248, 248, 248));
        }

        /// <summary>
        /// Gets disabled foreground color from theme
        /// </summary>
        protected Color GetDisabledForegroundColor()
        {
            return GetThemeColor(t => t.DisabledForeColor, Color.FromArgb(160, 160, 160));
        }

        /// <summary>
        /// Gets border color from theme
        /// </summary>
        protected Color GetBorderColor()
        {
            return GetThemeColor(t => t.MenuBorderColor, Color.FromArgb(200, 200, 200));
        }

        /// <summary>
        /// Gets accent color from theme
        /// </summary>
        protected Color GetAccentColor()
        {
            return GetThemeColor(t => t.AccentColor, Color.Empty);
        }

        /// <summary>
        /// Updates context colors from theme
        /// </summary>
        protected void UpdateContextColors(MenuBarContext ctx)
        {
            if (ctx == null) return;

            ctx.AccentColor = GetAccentColor();
            ctx.ItemBackColor = GetItemBackgroundColor();
            ctx.ItemForeColor = GetItemForegroundColor();
            ctx.ItemBorderColor = GetBorderColor();
            ctx.ItemHoverBackColor = GetHoverBackgroundColor();
            ctx.ItemHoverForeColor = GetHoverForegroundColor();
            ctx.ItemSelectedBackColor = GetSelectedBackgroundColor();
            ctx.ItemSelectedForeColor = GetSelectedForegroundColor();
            ctx.ItemDisabledBackColor = GetDisabledBackgroundColor();
            ctx.ItemDisabledForeColor = GetDisabledForegroundColor();
        }

        /// <summary>
        /// Safely invokes an action on the UI thread
        /// </summary>
        protected void SafeInvalidate()
        {
            if (Owner?.IsDisposed == false)
            {
                if (Owner.InvokeRequired)
                {
                    Owner.BeginInvoke(new Action(() => Owner.Invalidate()));
                }
                else
                {
                    Owner.Invalidate();
                }
            }
        }

        #region DPI Scaling Helpers - Framework handles DPI automatically
        /// <summary>
        /// Returns value unchanged - framework handles DPI scaling
        /// </summary>
        protected int ScaleValue(int value)
        {
            return value;
        }

        /// <summary>
        /// Returns size unchanged - framework handles DPI scaling
        /// </summary>
        protected Size ScaleSize(Size size)
        {
            return size;
        }

        /// <summary>
        /// Returns padding unchanged - framework handles DPI scaling
        /// </summary>
        protected Padding ScalePadding(Padding padding)
        {
            return padding;
        }

        /// <summary>
        /// Returns rectangle unchanged - framework handles DPI scaling
        /// </summary>
        protected Rectangle ScaleRectangle(Rectangle rect)
        {
            return rect;
        }

        /// <summary>
        /// Returns point unchanged - framework handles DPI scaling
        /// </summary>
        protected Point ScalePoint(Point point)
        {
            return point;
        }

        /// <summary>
        /// Returns font unchanged - framework handles DPI scaling
        /// </summary>
        protected Font ScaleFont(Font font)
        {
            return font;
        }

      
        #endregion
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                OnDisposing();
                Owner = null;
                Theme = null;
                _disposed = true;
            }
        }

        /// <summary>
        /// Called during disposal to clean up painter-specific resources
        /// </summary>
        protected virtual void OnDisposing() { }

        ~MenuBarPainterBase()
        {
            Dispose(false);
        }
        #endregion
    }
}