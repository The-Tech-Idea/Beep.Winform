using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Brutalist shadow painter - hard, offset block shadow for bold geometric 3D layered effect.
    /// </summary>
    public static class BrutalistShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(style))
                return path;

            // Brutalist: Shadow is the same shape as the element, offset to create 3D layered effect
            int offsetX = 3; // Offset to the right
            int offsetY = 3; // Offset down
            
            // Clone the path and offset it
            using (var shadowPath = (GraphicsPath)path.Clone())
            {
                // Create transformation matrix for offset
                using (var matrix = new System.Drawing.Drawing2D.Matrix())
                {
                    matrix.Translate(offsetX, offsetY);
                    shadowPath.Transform(matrix);
                    
                    // Disable anti-aliasing for sharp shadow edges
                    var previousSmoothing = g.SmoothingMode;
                    g.SmoothingMode = SmoothingMode.None;

                    // Draw the shadow shape (solid dark block)
                    var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(180, 0, 0, 0)); // Darker for more contrast
                    g.FillPath(brush, shadowPath);

                    g.SmoothingMode = previousSmoothing;
                }
            }

            return path.CreateInsetPath(0f);
        }
    }
}
