using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Glass Acrylic button painter - Frosted glass effect with 6px radius
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// ⭐ Special: Reduced overlay alpha to preserve translucency
    /// </summary>
    public static class GlassAcrylicButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            SpinnerButtonPainterHelpers.ControlState upState = SpinnerButtonPainterHelpers.ControlState.Normal,
            SpinnerButtonPainterHelpers.ControlState downState = SpinnerButtonPainterHelpers.ControlState.Normal)
        {
            // Glass Acrylic: Frosted translucent glass
            int upBaseAlpha = upState == SpinnerButtonPainterHelpers.ControlState.Pressed ? 70 : 50;
            int downBaseAlpha = downState == SpinnerButtonPainterHelpers.ControlState.Pressed ? 70 : 50;
            
            Color upGlassBase = SpinnerButtonPainterHelpers.WithAlpha(Color.White, upBaseAlpha);
            Color downGlassBase = SpinnerButtonPainterHelpers.WithAlpha(Color.White, downBaseAlpha);
            Color glassBorder = SpinnerButtonPainterHelpers.WithAlpha(Color.White, 60);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 6))
            using (var brush = new SolidBrush(upGlassBase))
            using (var pen = new Pen(glassBorder, 1f))
            {
                g.FillPath(brush, path1);
                g.DrawPath(pen, path1);
            }

            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 6))
            using (var brush = new SolidBrush(downGlassBase))
            using (var pen = new Pen(glassBorder, 1f))
            {
                g.FillPath(brush, path2);
                g.DrawPath(pen, path2);
            }

            // State overlays with REDUCED alpha to preserve glass effect
            Color upOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(upState);
            if (upOverlay != Color.Transparent && upState != SpinnerButtonPainterHelpers.ControlState.Disabled)
            {
                Color reducedOverlay = SpinnerButtonPainterHelpers.WithAlpha(upOverlay.R, upOverlay.G, upOverlay.B, upOverlay.A / 2);
                using (var brush = new SolidBrush(reducedOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 6))
                {
                    g.FillPath(brush, path);
                }
            }

            Color downOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(downState);
            if (downOverlay != Color.Transparent && downState != SpinnerButtonPainterHelpers.ControlState.Disabled)
            {
                Color reducedOverlay = SpinnerButtonPainterHelpers.WithAlpha(downOverlay.R, downOverlay.G, downOverlay.B, downOverlay.A / 2);
                using (var brush = new SolidBrush(reducedOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 6))
                {
                    g.FillPath(brush, path);
                }
            }

            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, SpinnerButtonPainterHelpers.ArrowDirection.Up, Color.White);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, SpinnerButtonPainterHelpers.ArrowDirection.Down, Color.White);
        }
    }
}
