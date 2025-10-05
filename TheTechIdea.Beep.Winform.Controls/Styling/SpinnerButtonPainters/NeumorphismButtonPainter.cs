using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Neumorphism button painter - Soft embossed 3D effect with 8px radius
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// ⭐ Special: Pressed state inverts the highlight (embossed → debossed)
    /// </summary>
    public static class NeumorphismButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            SpinnerButtonPainterHelpers.ControlState upState = SpinnerButtonPainterHelpers.ControlState.Normal,
            SpinnerButtonPainterHelpers.ControlState downState = SpinnerButtonPainterHelpers.ControlState.Normal)
        {
            // Neumorphism: Soft 3D embossed effect
            Color baseColor = useThemeColors ? theme.BackColor : Color.FromArgb(230, 230, 230);
            Color upBaseColor = SpinnerButtonPainterHelpers.ApplyState(baseColor, upState);
            Color downBaseColor = SpinnerButtonPainterHelpers.ApplyState(baseColor, downState);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 8))
            using (var brush = new SolidBrush(upBaseColor))
            {
                g.FillPath(brush, path1);
            }

            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 8))
            using (var brush = new SolidBrush(downBaseColor))
            {
                g.FillPath(brush, path2);
            }

            // Inner highlight (top half) - INVERTED when pressed
            var upHighlight = new Rectangle(upButtonRect.X + 2, upButtonRect.Y + 2, upButtonRect.Width - 4, upButtonRect.Height / 2);
            var downHighlight = new Rectangle(downButtonRect.X + 2, downButtonRect.Y + 2, downButtonRect.Width - 4, downButtonRect.Height / 2);
            
            Color upHighlightColor = upState == SpinnerButtonPainterHelpers.ControlState.Pressed
                ? SpinnerButtonPainterHelpers.Darken(upBaseColor, 0.1f)  // Darker when pressed (debossed)
                : SpinnerButtonPainterHelpers.Lighten(upBaseColor, 0.1f); // Lighter normally (embossed)
            
            Color downHighlightColor = downState == SpinnerButtonPainterHelpers.ControlState.Pressed
                ? SpinnerButtonPainterHelpers.Darken(downBaseColor, 0.1f)
                : SpinnerButtonPainterHelpers.Lighten(downBaseColor, 0.1f);
            
            using (var brush = new SolidBrush(SpinnerButtonPainterHelpers.WithAlpha(upHighlightColor, 60)))
            {
                g.FillRectangle(brush, upHighlight);
            }

            using (var brush = new SolidBrush(SpinnerButtonPainterHelpers.WithAlpha(downHighlightColor, 60)))
            {
                g.FillRectangle(brush, downHighlight);
            }

            // State overlays (skip for pressed to preserve inverted effect)
            if (upState != SpinnerButtonPainterHelpers.ControlState.Pressed)
            {
                Color upOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(upState);
                if (upOverlay != Color.Transparent)
                {
                    using (var brush = new SolidBrush(upOverlay))
                    using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 8))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }

            if (downState != SpinnerButtonPainterHelpers.ControlState.Pressed)
            {
                Color downOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(downState);
                if (downOverlay != Color.Transparent)
                {
                    using (var brush = new SolidBrush(downOverlay))
                    using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 8))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }

            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, SpinnerButtonPainterHelpers.ArrowDirection.Up, Color.FromArgb(120, 120, 120));
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, SpinnerButtonPainterHelpers.ArrowDirection.Down, Color.FromArgb(120, 120, 120));
        }
    }
}
