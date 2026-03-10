using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Docks.Helpers;
using TheTechIdea.Beep.Winform.Controls.Docks.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDock - Public Methods and Theme
    /// </summary>
    public partial class BeepDock
    {
        #region Public Methods
        /// <summary>
        /// Adds an item to the dock
        /// </summary>
        public void AddItem(SimpleItem item)
        {
            if (item != null && !_items.Contains(item))
            {
                _items.Add(item);
            }
        }

        /// <summary>
        /// Removes an item from the dock
        /// </summary>
        public void RemoveItem(SimpleItem item)
        {
            if (item != null && _items.Contains(item))
            {
                if (item == _selectedItem)
                {
                    SelectedItem = null;
                }
                _items.Remove(item);
            }
        }

        /// <summary>
        /// Clears all items from the dock
        /// </summary>
        public void ClearItems()
        {
            SelectedItem = null;
            _items.Clear();
        }

        /// <summary>
        /// Gets the item at the specified screen point
        /// </summary>
        public SimpleItem GetItemAtPoint(Point point)
        {
            int index = DockHitTestHelper.HitTest(point, _itemStates);
            return index >= 0 && index < _itemStates.Count ? _itemStates[index].Item : null;
        }
        #endregion

        #region Theme
        /// <summary>
        /// Applies the current theme to the dock
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            // Apply font theme based on ControlStyle
            Docks.Helpers.DockFontHelpers.ApplyFontTheme(ControlStyle);

            if (_currentTheme != null)
            {
                // Use theme helpers for consistent color retrieval
                // Apply theme colors to config if not set
                if (!_config.BackgroundColor.HasValue)
                {
                    BackColor = Docks.Helpers.DockThemeHelpers.GetDockBackgroundColor(
                        _currentTheme, UseThemeColors, null, _config.BackgroundOpacity);
                }
                else
                {
                    BackColor = Docks.Helpers.DockThemeHelpers.GetDockBackgroundColor(
                        _currentTheme, UseThemeColors, _config.BackgroundColor, _config.BackgroundOpacity);
                }

                ForeColor = Docks.Helpers.DockThemeHelpers.GetDockForegroundColor(
                    _currentTheme, UseThemeColors);
            }

            // Maintain frameless appearance
            IsChild = true;
            // Respect explicit dock background styles when set.
            if (Parent != null && !_config.ShowBackground && !_config.BackgroundColor.HasValue)
                BackColor = Parent.BackColor;
            
            IsFrameless = true;
            ShowAllBorders = false;
            IsBorderAffectedByTheme = false;

            Invalidate();
        }

        internal void ApplyStyleProfile(DockStyleConfig? styleProfile)
        {
            if (styleProfile == null)
            {
                return;
            }

            _config.Style = styleProfile.DockStyle;
            _config.ItemSize = styleProfile.RecommendedItemSize;
            _config.DockHeight = styleProfile.RecommendedDockHeight;
            _config.Spacing = styleProfile.RecommendedSpacing;
            _config.Padding = styleProfile.RecommendedPadding;
            _config.MaxScale = styleProfile.RecommendedMaxScale;
            _config.BackgroundOpacity = styleProfile.RecommendedBackgroundOpacity;
            _config.ShowShadow = styleProfile.ShowShadow;
            _dockPainter = Docks.Painters.DockPainterFactory.GetPainter(_config.Style);
            ControlStyle = styleProfile.ControlStyle;
            UpdateItemBounds();
            Invalidate();
        }

        internal void ApplyColorProfile(DockColorConfig? colorProfile)
        {
            if (colorProfile == null)
            {
                return;
            }

            _config.BackgroundColor = colorProfile.BackgroundColor;
            _config.ForegroundColor = colorProfile.ForegroundColor;
            _config.BorderColor = colorProfile.BorderColor;
            _config.HoverColor = colorProfile.ItemHoverColor;
            _config.SelectedColor = colorProfile.ItemSelectedColor;
            _config.IndicatorColor = colorProfile.IndicatorColor;
            _config.SeparatorColor = colorProfile.SeparatorColor;
            Invalidate();
        }
        #endregion
    }
}
