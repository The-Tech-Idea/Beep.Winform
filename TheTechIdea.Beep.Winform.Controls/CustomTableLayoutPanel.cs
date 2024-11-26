using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Converters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class CustomTableLayoutPanel : TableLayoutPanel
    {
        private BeepTheme _theme = BeepThemesManager.DefaultTheme;
        private EnumBeepThemes _themeEnum = EnumBeepThemes.DefaultTheme;
        // Theme-related properties
        [Browsable(true)]
        [TypeConverter(typeof(ThemeConverter))]
        public EnumBeepThemes Theme
        {
            get => _themeEnum;
            set
            {
                _themeEnum = value;
                _theme = BeepThemesManager.GetTheme(value);
               
                Invalidate();
            }
        }
        public CustomTableLayoutPanel()
        {
            
            this.DoubleBuffered = true;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            // Custom background painting here
            if (_theme.GradientStartColor != Color.Empty && _theme.GradientEndColor != Color.Empty)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                    _theme.GradientStartColor, _theme.GradientEndColor, _theme.GradientDirection))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            }
            else
            {
                using (SolidBrush solidBrush = new SolidBrush(_theme.PanelBackColor))
                {
                    e.Graphics.FillRectangle(solidBrush, this.ClientRectangle);
                }
            }
        }
    }

}
