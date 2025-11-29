using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        #region Theme Integration
        
        /// <summary>
        /// Applies theme colors to tab elements
        /// </summary>
        private void ApplyThemeColorsToTabs()
        {
            // Update paint helper theme
            if (_paintHelper != null)
            {
                _paintHelper.Theme = _currentTheme;
                _paintHelper.ControlStyle = ControlStyle;
            }

            // Use theme colors if available, otherwise use style defaults
            if (UseThemeColors && _currentTheme != null)
            {
                // Tab area colors from theme
                _tabBackColor = _currentTheme.TabBackColor != Color.Empty 
                    ? _currentTheme.TabBackColor 
                    : _currentTheme.PanelBackColor;
                _tabForeColor = _currentTheme.TabForeColor != Color.Empty 
                    ? _currentTheme.TabForeColor 
                    : _currentTheme.ForeColor;
                _activeTabBackColor = _currentTheme.TabSelectedBackColor != Color.Empty 
                    ? _currentTheme.TabSelectedBackColor 
                    : _currentTheme.ButtonBackColor;
                _activeTabForeColor = _currentTheme.TabSelectedForeColor != Color.Empty 
                    ? _currentTheme.TabSelectedForeColor 
                    : _currentTheme.ButtonForeColor;
                _hoverTabBackColor = _currentTheme.TabHoverBackColor != Color.Empty 
                    ? _currentTheme.TabHoverBackColor 
                    : ControlPaint.Light(_currentTheme.ButtonBackColor, 0.2f);
                _borderColor = _currentTheme.TabBorderColor != Color.Empty 
                    ? _currentTheme.TabBorderColor 
                    : _currentTheme.BorderColor;
                _contentBackColor = _currentTheme.BackColor;
            }
            else
            {
                // Use style-based colors for more modern look (consistent with BeepStyling)
                _tabBackColor = StyleColors.GetBackground(ControlStyle);
                _tabForeColor = StyleColors.GetForeground(ControlStyle);
                _activeTabBackColor = StyleColors.GetSelection(ControlStyle);
                _activeTabForeColor = StyleColors.GetForeground(ControlStyle);
                _hoverTabBackColor = StyleColors.GetHover(ControlStyle);
                _borderColor = StyleColors.GetBorder(ControlStyle);
                _contentBackColor = StyleColors.GetSurface(ControlStyle);
            }
            
            // Update BackColor property to match content background
            if (!IsTransparentBackground)
            {
                base.BackColor = _contentBackColor;
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
        
        /// <summary>
        /// Gets the effective content background color, respecting transparency and theme
        /// </summary>
        private Color GetEffectiveContentBackColor()
        {
            if (IsTransparentBackground)
                return Color.Transparent;
                
            if (UseThemeColors && _currentTheme != null)
                return _currentTheme.BackColor;
                
            return _contentBackColor;
        }

        #endregion
    }
}

