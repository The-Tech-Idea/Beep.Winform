using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.AppBars.Helpers
{
    /// <summary>
    /// Manages state and caching for BeepAppBar to avoid redundant calculations
    /// </summary>
    internal class BeepAppBarStateStore
    {
        #region "Layout Cache"
        private Rectangle _cachedLogoRect;
        private Rectangle _cachedTitleRect;
        private Rectangle _cachedSearchRect;
        private Rectangle _cachedNotificationRect;
        private Rectangle _cachedProfileRect;
        private Rectangle _cachedThemeRect;
        private Rectangle _cachedMinimizeRect;
        private Rectangle _cachedMaximizeRect;
        private Rectangle _cachedCloseRect;
        private bool _layoutDirty = true;
        #endregion

        #region "State Tracking"
        private string _hoveredComponentName;
        private bool _searchBoxAddedToControls = false;
        private bool _isDragging = false;
        private string _currentMenuName;
        #endregion

        #region "Layout Cache Properties"
        public Rectangle CachedLogoRect 
        { 
            get => _cachedLogoRect; 
            set => _cachedLogoRect = value; 
        }
        
        public Rectangle CachedTitleRect 
        { 
            get => _cachedTitleRect; 
            set => _cachedTitleRect = value; 
        }
        
        public Rectangle CachedSearchRect 
        { 
            get => _cachedSearchRect; 
            set => _cachedSearchRect = value; 
        }
        
        public Rectangle CachedNotificationRect 
        { 
            get => _cachedNotificationRect; 
            set => _cachedNotificationRect = value; 
        }
        
        public Rectangle CachedProfileRect 
        { 
            get => _cachedProfileRect; 
            set => _cachedProfileRect = value; 
        }
        
        public Rectangle CachedThemeRect 
        { 
            get => _cachedThemeRect; 
            set => _cachedThemeRect = value; 
        }
        
        public Rectangle CachedMinimizeRect 
        { 
            get => _cachedMinimizeRect; 
            set => _cachedMinimizeRect = value; 
        }
        
        public Rectangle CachedMaximizeRect 
        { 
            get => _cachedMaximizeRect; 
            set => _cachedMaximizeRect = value; 
        }
        
        public Rectangle CachedCloseRect 
        { 
            get => _cachedCloseRect; 
            set => _cachedCloseRect = value; 
        }

        public bool LayoutDirty 
        { 
            get => _layoutDirty; 
            set => _layoutDirty = value; 
        }
        #endregion

        #region "State Properties"
        public string HoveredComponentName 
        { 
            get => _hoveredComponentName; 
            set => _hoveredComponentName = value; 
        }

        public bool SearchBoxAddedToControls 
        { 
            get => _searchBoxAddedToControls; 
            set => _searchBoxAddedToControls = value; 
        }

        public bool IsDragging 
        { 
            get => _isDragging; 
            set => _isDragging = value; 
        }

        public string CurrentMenuName 
        { 
            get => _currentMenuName; 
            set => _currentMenuName = value; 
        }
        #endregion

        #region "Methods"
        public void InvalidateLayout()
        {
            _layoutDirty = true;
        }

        public void ClearLayoutCache()
        {
            _cachedLogoRect = Rectangle.Empty;
            _cachedTitleRect = Rectangle.Empty;
            _cachedSearchRect = Rectangle.Empty;
            _cachedNotificationRect = Rectangle.Empty;
            _cachedProfileRect = Rectangle.Empty;
            _cachedThemeRect = Rectangle.Empty;
            _cachedMinimizeRect = Rectangle.Empty;
            _cachedMaximizeRect = Rectangle.Empty;
            _cachedCloseRect = Rectangle.Empty;
            _layoutDirty = true;
        }

        public bool IsComponentHovered(string componentName)
        {
            return _hoveredComponentName == componentName;
        }

        public void SetHoveredComponent(string componentName)
        {
            if (_hoveredComponentName != componentName)
            {
                _hoveredComponentName = componentName;
            }
        }

        public void ClearHover()
        {
            _hoveredComponentName = null;
        }
        #endregion
    }
}