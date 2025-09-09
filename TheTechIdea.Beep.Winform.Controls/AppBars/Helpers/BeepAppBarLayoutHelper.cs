using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.Helpers
{
    /// <summary>
    /// Handles layout calculations and DPI-aware positioning for BeepAppBar components
    /// </summary>
    internal class BeepAppBarLayoutHelper
    {
        private readonly IBeepAppBarHost _host;
        private readonly BeepAppBarStateStore _stateStore;

        public BeepAppBarLayoutHelper(IBeepAppBarHost host, BeepAppBarStateStore stateStore)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
        }

        #region "Layout Calculation"
        
        /// <summary>
        /// Ensures layout is calculated and cached
        /// </summary>
        public void EnsureLayout()
        {
            if (_stateStore.LayoutDirty || _stateStore.CachedLogoRect.IsEmpty)
            {
                CalculateAndCacheLayout();
                _stateStore.LayoutDirty = false;
            }
        }

        /// <summary>
        /// Forces layout recalculation
        /// </summary>
        public void InvalidateLayout()
        {
            _stateStore.InvalidateLayout();
        }

        /// <summary>
        /// Main layout calculation method
        /// </summary>
        private void CalculateAndCacheLayout()
        {
            CalculateLayout(
                out Rectangle logoRect, 
                out Rectangle titleRect, 
                out Rectangle searchRect,
                out Rectangle notificationRect, 
                out Rectangle profileRect, 
                out Rectangle themeRect,
                out Rectangle minimizeRect, 
                out Rectangle maximizeRect, 
                out Rectangle closeRect
            );

            // Cache the calculated rectangles
            _stateStore.CachedLogoRect = logoRect;
            _stateStore.CachedTitleRect = titleRect;
            _stateStore.CachedSearchRect = searchRect;
            _stateStore.CachedNotificationRect = notificationRect;
            _stateStore.CachedProfileRect = profileRect;
            _stateStore.CachedThemeRect = themeRect;
            _stateStore.CachedMinimizeRect = minimizeRect;
            _stateStore.CachedMaximizeRect = maximizeRect;
            _stateStore.CachedCloseRect = closeRect;
        }

        /// <summary>
        /// Calculates component positions with DPI-awareness
        /// </summary>
        private void CalculateLayout(
            out Rectangle logoRect, out Rectangle titleRect, out Rectangle searchRect,
            out Rectangle notificationRect, out Rectangle profileRect, out Rectangle themeRect,
            out Rectangle minimizeRect, out Rectangle maximizeRect, out Rectangle closeRect)
        {
            // Use DPI-scaled values consistently
            int padding = _host.ScaleValue(5);
            int spacing = _host.ScaleValue(10);
            int searchHeight = _host.ScaleValue(24);
            int scaledWindowIconsHeight = _host.ScaleValue(40);
            int imageOffset = 2;

            // Initialize rectangles
            logoRect = Rectangle.Empty;
            titleRect = Rectangle.Empty;
            searchRect = Rectangle.Empty;
            notificationRect = Rectangle.Empty;
            profileRect = Rectangle.Empty;
            themeRect = Rectangle.Empty;
            minimizeRect = Rectangle.Empty;
            maximizeRect = Rectangle.Empty;
            closeRect = Rectangle.Empty;

            var drawingRect = _host.DrawingRect;
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
                return;

            // Calculate available areas in DrawingRect
            int leftEdge = drawingRect.Left + padding;
            int rightEdge = drawingRect.Right - padding;
            int centerY = drawingRect.Top + drawingRect.Height / 2;

            // Position window control buttons (right-aligned)
            if (_host.ShowCloseIcon)
            {
                closeRect = new Rectangle(
                    rightEdge - scaledWindowIconsHeight,
                    centerY - scaledWindowIconsHeight / 2,
                    scaledWindowIconsHeight,
                    scaledWindowIconsHeight
                );
                rightEdge = closeRect.Left - spacing;
            }

            if (_host.ShowMaximizeIcon)
            {
                maximizeRect = new Rectangle(
                    rightEdge - scaledWindowIconsHeight,
                    centerY - scaledWindowIconsHeight / 2,
                    scaledWindowIconsHeight,
                    scaledWindowIconsHeight
                );
                rightEdge = maximizeRect.Left - spacing;
            }

            if (_host.ShowMinimizeIcon)
            {
                minimizeRect = new Rectangle(
                    rightEdge - scaledWindowIconsHeight,
                    centerY - scaledWindowIconsHeight / 2,
                    scaledWindowIconsHeight,
                    scaledWindowIconsHeight
                );
                rightEdge = minimizeRect.Left - spacing;
            }

            // Position search box with DPI-scaled width
            if (_host.ShowSearchBox)
            {
                int scaledSearchWidth = _host.ScaleValue(_host.SearchBoxWidth);
                searchRect = new Rectangle(
                    rightEdge - scaledSearchWidth,
                    centerY - searchHeight / 2,
                    scaledSearchWidth,
                    searchHeight
                );
                rightEdge = searchRect.Left - spacing;
            }

            // Position notification, profile, and theme icons
            if (_host.ShowNotificationIcon)
            {
                notificationRect = new Rectangle(
                    rightEdge - scaledWindowIconsHeight,
                    centerY - scaledWindowIconsHeight / 2,
                    scaledWindowIconsHeight,
                    scaledWindowIconsHeight
                );
                rightEdge = notificationRect.Left - spacing;
            }

            if (_host.ShowProfileIcon)
            {
                profileRect = new Rectangle(
                    rightEdge - scaledWindowIconsHeight,
                    centerY - scaledWindowIconsHeight / 2,
                    scaledWindowIconsHeight,
                    scaledWindowIconsHeight
                );
                rightEdge = profileRect.Left - spacing;
            }

            if (_host.ShowThemeIcon)
            {
                themeRect = new Rectangle(
                    rightEdge - scaledWindowIconsHeight,
                    centerY - scaledWindowIconsHeight / 2,
                    scaledWindowIconsHeight,
                    scaledWindowIconsHeight
                );
                rightEdge = themeRect.Left - spacing;
            }

            // Position logo with DPI-scaled size
            if (_host.ShowLogo && !string.IsNullOrEmpty(_host.LogoImage))
            {
                Size logoSize = _host.ScaleSize(_host.LogoSize);
                logoRect = new Rectangle(
                    leftEdge,
                    centerY - logoSize.Height / 2,
                    logoSize.Width,
                    logoSize.Height
                );
                leftEdge = logoRect.Right + spacing;
            }

            // Position title (fill remaining space)
            if (_host.ShowTitle)
            {
                int titleHeight = _host.ScaleValue(24);
                titleRect = new Rectangle(
                    leftEdge,
                    centerY - titleHeight / 2,
                    Math.Max(0, rightEdge - leftEdge - spacing),
                    titleHeight
                );
            }
        }
        #endregion

        #region "Hit Testing"

        /// <summary>
        /// Determines which component is at the given point
        /// </summary>
        public string GetComponentAtPoint(Point point)
        {
            EnsureLayout();

            if (_host.ShowLogo && !string.IsNullOrEmpty(_host.LogoImage) && _stateStore.CachedLogoRect.Contains(point))
                return "Logo";

            if (_host.ShowTitle && _stateStore.CachedTitleRect.Contains(point))
                return "Title";

            if (_host.ShowSearchBox && !_stateStore.SearchBoxAddedToControls && _stateStore.CachedSearchRect.Contains(point))
                return "Search";

            if (_host.ShowNotificationIcon && _stateStore.CachedNotificationRect.Contains(point))
                return "Notification";

            if (_host.ShowProfileIcon && _stateStore.CachedProfileRect.Contains(point))
                return "Profile";

            if (_host.ShowThemeIcon && _stateStore.CachedThemeRect.Contains(point))
                return "Theme";

            if (_host.ShowMinimizeIcon && _stateStore.CachedMinimizeRect.Contains(point))
                return "Minimize";

            if (_host.ShowMaximizeIcon && _stateStore.CachedMaximizeRect.Contains(point))
                return "Maximize";

            if (_host.ShowCloseIcon && _stateStore.CachedCloseRect.Contains(point))
                return "Close";

            return null;
        }

        /// <summary>
        /// Checks if point is in an interactive area (not draggable)
        /// </summary>
        public bool IsInteractiveArea(Point point)
        {
            EnsureLayout();

            return (_host.ShowSearchBox && !_stateStore.SearchBoxAddedToControls && _stateStore.CachedSearchRect.Contains(point)) ||
                   (_host.ShowNotificationIcon && _stateStore.CachedNotificationRect.Contains(point)) ||
                   (_host.ShowProfileIcon && _stateStore.CachedProfileRect.Contains(point)) ||
                   (_host.ShowThemeIcon && _stateStore.CachedThemeRect.Contains(point)) ||
                   (_host.ShowMinimizeIcon && _stateStore.CachedMinimizeRect.Contains(point)) ||
                   (_host.ShowMaximizeIcon && _stateStore.CachedMaximizeRect.Contains(point)) ||
                   (_host.ShowCloseIcon && _stateStore.CachedCloseRect.Contains(point));
        }
        #endregion

        #region "Public Rectangle Access"
        
        public Rectangle GetLogoRect() { EnsureLayout(); return _stateStore.CachedLogoRect; }
        public Rectangle GetTitleRect() { EnsureLayout(); return _stateStore.CachedTitleRect; }
        public Rectangle GetSearchRect() { EnsureLayout(); return _stateStore.CachedSearchRect; }
        public Rectangle GetNotificationRect() { EnsureLayout(); return _stateStore.CachedNotificationRect; }
        public Rectangle GetProfileRect() { EnsureLayout(); return _stateStore.CachedProfileRect; }
        public Rectangle GetThemeRect() { EnsureLayout(); return _stateStore.CachedThemeRect; }
        public Rectangle GetMinimizeRect() { EnsureLayout(); return _stateStore.CachedMinimizeRect; }
        public Rectangle GetMaximizeRect() { EnsureLayout(); return _stateStore.CachedMaximizeRect; }
        public Rectangle GetCloseRect() { EnsureLayout(); return _stateStore.CachedCloseRect; }

        #endregion
    }
}