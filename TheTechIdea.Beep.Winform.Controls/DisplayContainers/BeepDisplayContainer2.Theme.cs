using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        #region Theme Integration

        public override void ApplyTheme()
        {
            // Like BeepMenuBar: Don't call base.ApplyTheme() when transparent
            // base.ApplyTheme() would reset BackColor to theme color, overriding our Transparent setting
            if (!IsTransparentBackground)
            {
                base.ApplyTheme();
            }
            
            // Get control style from FormStyle for modern appearance
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
           
            if (_currentTheme != null)
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
                    // Use style-based colors for more modern look
                    _tabBackColor = BeepStyling.GetBackgroundColor(controlStyle);
                    _tabForeColor = BeepStyling.GetForegroundColor(controlStyle);
                    _activeTabBackColor = ControlPaint.Light(BeepStyling.GetBackgroundColor(controlStyle), 0.1f);
                    _activeTabForeColor = BeepStyling.GetForegroundColor(controlStyle);
                    _hoverTabBackColor = ControlPaint.Light(_tabBackColor, 0.1f);
                    _borderColor = BeepStyling.GetBorderColor(controlStyle);
                    _contentBackColor = BeepStyling.GetBackgroundColor(controlStyle);
                }
                
                // Set background color - preserve transparent if already set
                if (IsTransparentBackground || base.BackColor == Color.Transparent)
                {
                    // Preserve transparent background
                    base.BackColor = Color.Transparent;
                }
                else
                {
                    base.BackColor = _contentBackColor;
                }
            }
            else
            {
                // Fallback colors with modern defaults
                _tabBackColor = Color.FromArgb(248, 248, 248);
                _tabForeColor = Color.FromArgb(64, 64, 64);
                _activeTabBackColor = Color.White;
                _activeTabForeColor = Color.FromArgb(32, 32, 32);
                _hoverTabBackColor = Color.FromArgb(240, 240, 240);
                _borderColor = Color.FromArgb(220, 220, 220);
                _contentBackColor = Color.White;
                
                // Preserve transparent if already set
                if (IsTransparentBackground || base.BackColor == Color.Transparent)
                {
                    base.BackColor = Color.Transparent;
                }
                else
                {
                    base.BackColor = _contentBackColor;
                }
            }
            
            Invalidate();
        }

        #endregion
    }
}

