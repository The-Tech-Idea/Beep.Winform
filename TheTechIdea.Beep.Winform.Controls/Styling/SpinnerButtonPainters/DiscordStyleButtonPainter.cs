using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Discord ProgressBarStyle button painter - Discord blurple #5865F2 with 4px radius
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class DiscordStyleButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // Discord: Signature blurple brand color
            Color discordBlurple = Color.FromArgb(88, 101, 242);
            Color upColor = SpinnerButtonPainterHelpers.ApplyState(discordBlurple, upState);
            Color downColor = SpinnerButtonPainterHelpers.ApplyState(discordBlurple, downState);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var brush = new SolidBrush(upColor))
            using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 4))
            {
                g.FillPath(brush, path);
            }

            using (var brush = new SolidBrush(downColor))
            using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 4))
            {
                g.FillPath(brush, path);
            }

            // State overlays
            Color upOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(upState);
            if (upOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(upOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            Color downOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(downState);
            if (downOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(downOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, Color.White);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, Color.White);
        }
    }
}
