using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Terminal border painter — pixel-crisp 2px stroke that follows the passed
    /// chamfered GraphicsPath (typically from <see cref="PathPainterHelpers.CreateTerminalPath"/>),
    /// with bright accent marks at each chamfer point for the ASCII bracket aesthetic.
    /// </summary>
    public static class TerminalBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (path == null) return path;

            const float borderWidth = 2f;

            // Terminal green as default; theme override supported
            Color baseColor = BorderPainterHelpers.GetColorFromStyleOrTheme(
                theme, useThemeColors, "Border", Color.FromArgb(0, 255, 0));

            // State-driven color and alpha adjustments
            Color borderColor = state switch
            {
                ControlState.Disabled => BorderPainterHelpers.WithAlpha(baseColor, 90),
                ControlState.Pressed => BorderPainterHelpers.Darken(baseColor, 0.15f),
                ControlState.Hovered => BorderPainterHelpers.Lighten(baseColor, 0.1f),
                ControlState.Selected => BorderPainterHelpers.WithAlpha(baseColor, 240),
                ControlState.Focused => BorderPainterHelpers.WithAlpha(baseColor, 255),
                _ => BorderPainterHelpers.WithAlpha(baseColor, 200)
            };

            // Boost alpha when the isFocused flag is set but state is not explicitly Focused
            if (isFocused && state != ControlState.Focused && state != ControlState.Disabled)
                borderColor = BorderPainterHelpers.WithAlpha(borderColor, 255);

            var oldSmooth = g.SmoothingMode;
            var oldPixel = g.PixelOffsetMode;

            // Pixel-crisp rendering for the retro terminal look
            g.SmoothingMode = SmoothingMode.None;
            g.PixelOffsetMode = PixelOffsetMode.None;

            // Primary border follows the chamfered path (convention-compliant).
            // PaintSimpleBorder insets by half the stroke width so the full 2px
            // stroke sits inside the original path boundary.
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // ASCII bracket corner accents — additive overlay using actual path
            // vertices so the marks stay consistent with the chamfer geometry even
            // when the path has been transformed or scaled.
            PointF[] pts = path.PathPoints;
            if (pts.Length >= 8)
            {
                // Compute accent length proportionally from the path bounds
                // (only used for sizing; actual coordinates come from path points).
                RectangleF bounds = path.GetBounds();
                float accent = Math.Max(3f, Math.Min(8f,
                    Math.Min(bounds.Width, bounds.Height) / 12f));

                Color accentColor = state == ControlState.Disabled
                    ? BorderPainterHelpers.WithAlpha(borderColor, 120)
                    : BorderPainterHelpers.WithAlpha(borderColor, 255);

                using var accentPen = new Pen(accentColor, borderWidth + 1f);
                accentPen.StartCap = LineCap.Square;
                accentPen.EndCap = LineCap.Square;

                // The terminal path is an octagon created by CreateTerminalPath.
                // Point order (assuming 8 distinct vertices):
                // 0 = (L+ch, T)    top edge, near left
                // 1 = (R-ch, T)    top edge, near right
                // 2 = (R, T+ch)    right edge, near top
                // 3 = (R, B-ch)    right edge, near bottom
                // 4 = (R-ch, B)    bottom edge, near right
                // 5 = (L+ch, B)    bottom edge, near left
                // 6 = (L, B-ch)    left edge, near bottom
                // 7 = (L, T+ch)    left edge, near top
                //
                // Accent strokes extend inward (toward shape center) from the
                // chamfer endpoints along the horizontal/vertical edges.

                // Top-left area
                g.DrawLine(accentPen, pts[0].X, pts[0].Y, pts[0].X + accent, pts[0].Y);
                g.DrawLine(accentPen, pts[7].X, pts[7].Y, pts[7].X, pts[7].Y + accent);

                // Top-right area
                g.DrawLine(accentPen, pts[1].X, pts[1].Y, pts[1].X - accent, pts[1].Y);
                g.DrawLine(accentPen, pts[2].X, pts[2].Y, pts[2].X, pts[2].Y + accent);

                // Bottom-right area
                g.DrawLine(accentPen, pts[3].X, pts[3].Y, pts[3].X, pts[3].Y - accent);
                g.DrawLine(accentPen, pts[4].X, pts[4].Y, pts[4].X - accent, pts[4].Y);

                // Bottom-left area
                g.DrawLine(accentPen, pts[5].X, pts[5].Y, pts[5].X + accent, pts[5].Y);
                g.DrawLine(accentPen, pts[6].X, pts[6].Y, pts[6].X, pts[6].Y - accent);
            }

            g.SmoothingMode = oldSmooth;
            g.PixelOffsetMode = oldPixel;

            // Return the content area inside the full 2px stroke
            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}
