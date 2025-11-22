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
        /// Applies theme colors to tab elements
        /// </summary>
        private void ApplyThemeColorsToTabs()
        {
            // Update paint helper theme
            if (_paintHelper != null)
            {
                _paintHelper.Theme = _currentTheme;
            }

            // Use theme colors if available, otherwise use style defaults
            if (UseThemeColors && _currentTheme != null)
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
            
            Invalidate();
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

