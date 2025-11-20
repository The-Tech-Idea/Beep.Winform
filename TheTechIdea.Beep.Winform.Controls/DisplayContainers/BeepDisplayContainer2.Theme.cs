using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        #region Theme Integration

        /// <summary>
        /// Applies the current theme to the container and tabs.
        /// Follows BaseControl pattern for consistent theme application.
        /// </summary>
        public override void ApplyTheme()
        {
            // Call base.ApplyTheme() first for proper theme initialization (like BaseControl)
            // This sets up _currentTheme and applies basic theme properties
            base.ApplyTheme();
            
            // Update tab painter with current theme and style
            var controlStyle = ControlStyle;
            if(_paintHelper==null)
            {
                _paintHelper = new TabPaintHelper(_currentTheme, controlStyle, IsTransparentBackground);
            }
            else
            {
                _paintHelper.ControlStyle = controlStyle;
                _paintHelper.IsTransparent = IsTransparentBackground;
            }
            // Ensure paint helper uses the selected tab style
            _paintHelper.TabStyle = this.TabStyle;
           
            if (_currentTheme != null)
            {
                // Apply theme colors to tabs (follows BaseControl pattern)
                ApplyThemeColorsToTabs();
                
                // Apply theme font if UseThemeFont is enabled (like BaseControl)
                if (UseThemeFont && _currentTheme.LabelFont != null)
                {
                    try
                    {
                        TextFont = FontListHelper.CreateFontFromTypography(_currentTheme.LabelFont);
                        
                        // Update layout helper with new font for proper tab sizing
                        if (_layoutHelper != null)
                        {
                            _layoutHelper.UpdateStyle(ControlStyle, Font);
                        }
                        
                        // Recalculate layout with new font metrics
                        RecalculateLayout();
                    }
                    catch
                    {
                        // Keep existing font on error
                    }
                }
                
                // Set background color based on theme and transparency setting
                if (IsTransparentBackground)
                {
                    base.BackColor = Color.Transparent;
                }
                else if (IsChild && Parent != null)
                {
                    // Follow parent background when IsChild is true (like BaseControl)
                    base.BackColor = Parent.BackColor;
                }
                else
                {
                    base.BackColor = _contentBackColor;
                }
            }
            else
            {
                // Fallback colors with modern defaults
                ApplyFallbackColors();
            }
            
            // Invalidate to trigger repaint with new theme
            Invalidate();
        }
        
        /// <summary>
        /// Applies theme colors to tab elements
        /// </summary>
        private void ApplyThemeColorsToTabs()
        {
            // Use theme colors if available, otherwise use style defaults
            if (UseThemeColors)
            {
                _tabBackColor = _currentTheme.PanelBackColor;
                _tabForeColor = _currentTheme.ForeColor;
                _activeTabBackColor = _currentTheme.ButtonBackColor;
                _activeTabForeColor = _currentTheme.ButtonForeColor;
                _hoverTabBackColor = ControlPaint.Light(_currentTheme.ButtonBackColor, 0.3f);
                _borderColor = _currentTheme.BorderColor;
                _contentBackColor = _currentTheme.BackColor;
            }
            else
            {
                // Use style-based colors for more modern look (consistent with BeepStyling)
                _tabBackColor = BeepStyling.GetBackgroundColor(ControlStyle);
                _tabForeColor = BeepStyling.GetForegroundColor(ControlStyle);
                _activeTabBackColor = ControlPaint.Light(BeepStyling.GetBackgroundColor(ControlStyle), 0.1f);
                _activeTabForeColor = BeepStyling.GetForegroundColor(ControlStyle);
                _hoverTabBackColor = ControlPaint.Light(_tabBackColor, 0.1f);
                _borderColor = BeepStyling.GetBorderColor(ControlStyle);
                _contentBackColor = BeepStyling.GetBackgroundColor(ControlStyle);
            }
        }
        
        /// <summary>
        /// Applies fallback colors when no theme is available
        /// </summary>
        private void ApplyFallbackColors()
        {
            _tabBackColor = Color.FromArgb(248, 248, 248);
            _tabForeColor = Color.FromArgb(64, 64, 64);
            _activeTabBackColor = Color.White;
            _activeTabForeColor = Color.FromArgb(32, 32, 32);
            _hoverTabBackColor = Color.FromArgb(240, 240, 240);
            _borderColor = Color.FromArgb(220, 220, 220);
            _contentBackColor = Color.White;
            
            // Set background based on transparency
            if (IsTransparentBackground)
            {
                base.BackColor = Color.Transparent;
            }
            else
            {
                base.BackColor = _contentBackColor;
            }
        }

        #endregion
    }
}

