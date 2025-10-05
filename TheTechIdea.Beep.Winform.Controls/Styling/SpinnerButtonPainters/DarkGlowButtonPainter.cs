using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Dark Glow button painter - Neon glow effect with 4px radius
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// ⭐ Special: Glow intensity modulated by state (0.4× to 1.3×)
    /// </summary>
    public static class DarkGlowButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            SpinnerButtonPainterHelpers.ControlState upState = SpinnerButtonPainterHelpers.ControlState.Normal,
            SpinnerButtonPainterHelpers.ControlState downState = SpinnerButtonPainterHelpers.ControlState.Normal)
        {
            // Dark Glow: Cyberpunk neon style
            Color glowColor = useThemeColors ? theme.AccentColor : Color.FromArgb(0, 255, 255);
            Color buttonColor = Color.FromArgb(30, 30, 30);
            Color upButtonColor = SpinnerButtonPainterHelpers.ApplyState(buttonColor, upState);
            Color downButtonColor = SpinnerButtonPainterHelpers.ApplyState(buttonColor, downState);

            // Glow intensity multipliers based on state
            float upGlowMultiplier = upState switch
            {
                SpinnerButtonPainterHelpers.ControlState.Hovered => 1.3f,
                SpinnerButtonPainterHelpers.ControlState.Pressed => 0.7f,
                SpinnerButtonPainterHelpers.ControlState.Selected => 1.2f,
                SpinnerButtonPainterHelpers.ControlState.Disabled => 0.4f,
                SpinnerButtonPainterHelpers.ControlState.Focused => 1.1f,
                _ => 1.0f
            };

            float downGlowMultiplier = downState switch
            {
                SpinnerButtonPainterHelpers.ControlState.Hovered => 1.3f,
                SpinnerButtonPainterHelpers.ControlState.Pressed => 0.7f,
                SpinnerButtonPainterHelpers.ControlState.Selected => 1.2f,
                SpinnerButtonPainterHelpers.ControlState.Disabled => 0.4f,
                SpinnerButtonPainterHelpers.ControlState.Focused => 1.1f,
                _ => 1.0f
            };

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 4))
            {
                // Outer glow with modulated intensity
                int upGlowAlpha = Math.Min(255, (int)(80 * upGlowMultiplier));
                using (var glowPen = new Pen(SpinnerButtonPainterHelpers.WithAlpha(glowColor, upGlowAlpha), 2f))
                {
                    g.DrawPath(glowPen, path1);
                }

                // Fill dark background
                using (var brush = new SolidBrush(upButtonColor))
                {
                    g.FillPath(brush, path1);
                }
            }

            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 4))
            {
                // Outer glow with modulated intensity
                int downGlowAlpha = Math.Min(255, (int)(80 * downGlowMultiplier));
                using (var glowPen = new Pen(SpinnerButtonPainterHelpers.WithAlpha(glowColor, downGlowAlpha), 2f))
                {
                    g.DrawPath(glowPen, path2);
                }

                // Fill dark background
                using (var brush = new SolidBrush(downButtonColor))
                {
                    g.FillPath(brush, path2);
                }
            }

            // State overlays with REDUCED alpha for dark backgrounds
            Color upOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(upState);
            if (upOverlay != Color.Transparent && upState != SpinnerButtonPainterHelpers.ControlState.Disabled)
            {
                Color reducedOverlay = SpinnerButtonPainterHelpers.WithAlpha(upOverlay.R, upOverlay.G, upOverlay.B, upOverlay.A / 3);
                using (var brush = new SolidBrush(reducedOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            Color downOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(downState);
            if (downOverlay != Color.Transparent && downState != SpinnerButtonPainterHelpers.ControlState.Disabled)
            {
                Color reducedOverlay = SpinnerButtonPainterHelpers.WithAlpha(downOverlay.R, downOverlay.G, downOverlay.B, downOverlay.A / 3);
                using (var brush = new SolidBrush(reducedOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, SpinnerButtonPainterHelpers.ArrowDirection.Up, glowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, SpinnerButtonPainterHelpers.ArrowDirection.Down, glowColor);
        }
    }
}
