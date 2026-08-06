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
            int index = _dockPainter.HitTest(point, _itemStates);
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

            // The painters are shared singletons, so the control's theme preference reaches them
            // through the config. This is the only place that writes it.
            _config.UseThemeColors = UseThemeColors;
            SyncDpiScale();

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

        /// <summary>
        /// Publishes the control's DPI to the painters and the layout helper.
        /// </summary>
        /// <remarks>
        /// <c>Control.DeviceDpi</c> is the authoritative source and is updated on WM_DPICHANGED;
        /// painters get no Control reference, so it travels on the config. Layout and painting both
        /// read the scaled dimensions, so they cannot drift apart.
        /// </remarks>
        private void SyncDpiScale()
        {
            var scale = TheTechIdea.Beep.Winform.Controls.Helpers.DpiScalingHelper.GetDpiScaleFactor(this);
            if (scale <= 0f)
            {
                scale = 1.0f;
            }

            if (Math.Abs(_config.DpiScale - scale) > 0.001f)
            {
                _config.DpiScale = scale;
                UpdateItemBounds();
                UpdateDockSize();
            }
        }

        /// <summary>
        /// Refreshes the designer-facing profiles from the live config.
        /// </summary>
        /// <remarks>
        /// The profiles were built once in the constructor and never refreshed, so after any style
        /// change they reported the constructor's AppleDock values - and being
        /// <c>DesignerSerializationVisibility.Content</c>, that is what the designer would serialize.
        /// They are projections of <c>_config</c>, so every path that writes <c>_config</c> calls this.
        /// </remarks>
        private void SyncProfiles()
        {
            _styleProfile.DockStyle = _config.Style;
            _styleProfile.ControlStyle = Docks.Helpers.DockStyleHelpers.GetControlStyleForDock(_config.Style);
            _styleProfile.RecommendedItemSize = _config.ItemSize;
            _styleProfile.RecommendedDockHeight = _config.DockHeight;
            _styleProfile.RecommendedSpacing = _config.Spacing;
            _styleProfile.RecommendedPadding = _config.Padding;
            _styleProfile.RecommendedMaxScale = _config.MaxScale;
            _styleProfile.RecommendedBackgroundOpacity = _config.BackgroundOpacity;
            _styleProfile.ShowShadow = _config.ShowShadow;

            _colorProfile.IndicatorColor = _config.IndicatorColor;
            _colorProfile.SeparatorColor = _config.SeparatorColor;
            if (_config.BackgroundColor.HasValue) _colorProfile.BackgroundColor = _config.BackgroundColor.Value;
            if (_config.ForegroundColor.HasValue) _colorProfile.ForegroundColor = _config.ForegroundColor.Value;
            if (_config.BorderColor.HasValue) _colorProfile.BorderColor = _config.BorderColor.Value;
            if (_config.HoverColor.HasValue) _colorProfile.ItemHoverColor = _config.HoverColor.Value;
            if (_config.SelectedColor.HasValue) _colorProfile.ItemSelectedColor = _config.SelectedColor.Value;
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
            SyncProfiles();
            UpdateItemBounds();
            Invalidate();
        }

        internal void ApplyColorProfile(DockColorConfig? colorProfile)
        {
            if (colorProfile == null)
            {
                return;
            }

            // Only colours the profile actually carries. Assigning all five unconditionally, as this
            // did, meant that merely touching ColorProfile in the designer filled in every nullable
            // colour on the config - and once they are non-null the painters' style defaults can
            // never apply again. That silently undid stage 01's fix for any dock whose profile had
            // been serialized.
            //
            // "Carries" is decided by comparing against a pristine profile, which is the best a model
            // of non-nullable Colors allows: a user who explicitly picks exactly the default colour
            // is indistinguishable from one who never touched it. Making DockColorConfig's properties
            // nullable would remove the ambiguity, at the cost of the designer's expandable editor
            // showing blanks.
            var pristine = new DockColorConfig();

            if (colorProfile.BackgroundColor != pristine.BackgroundColor)
                _config.BackgroundColor = colorProfile.BackgroundColor;
            if (colorProfile.ForegroundColor != pristine.ForegroundColor)
                _config.ForegroundColor = colorProfile.ForegroundColor;
            if (colorProfile.BorderColor != pristine.BorderColor)
                _config.BorderColor = colorProfile.BorderColor;
            if (colorProfile.ItemHoverColor != pristine.ItemHoverColor)
                _config.HoverColor = colorProfile.ItemHoverColor;
            if (colorProfile.ItemSelectedColor != pristine.ItemSelectedColor)
                _config.SelectedColor = colorProfile.ItemSelectedColor;

            _config.IndicatorColor = colorProfile.IndicatorColor;
            _config.SeparatorColor = colorProfile.SeparatorColor;
            Invalidate();
        }
        #endregion
    }
}
