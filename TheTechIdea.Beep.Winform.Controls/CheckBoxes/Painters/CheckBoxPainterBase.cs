using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Painters
{
    /// <summary>
    /// Base class for checkbox painters providing common functionality
    /// </summary>
    public abstract class CheckBoxPainterBase : ICheckBoxPainter
    {
        #region Abstract Methods

        public abstract void PaintCheckBox(Graphics g, Rectangle bounds, CheckBoxItemState state, CheckBoxRenderOptions options);
        public abstract void PaintCheckMark(Graphics g, Rectangle bounds, CheckBoxRenderOptions options);
        public abstract void PaintIndeterminateMark(Graphics g, Rectangle bounds, CheckBoxRenderOptions options);
        public abstract void PaintText(Graphics g, Rectangle bounds, string text, CheckBoxRenderOptions options);

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets colors for a checkbox using theme helpers
        /// </summary>
        protected (Color background, Color border, Color checkMark, Color foreground) GetCheckBoxColors(
            CheckBoxItemState state,
            CheckBoxRenderOptions options)
        {
            return CheckBoxThemeHelpers.GetCheckBoxColors(
                options.Theme,
                options.UseThemeColors,
                state.IsChecked,
                state.IsIndeterminate);
        }

        protected (Color background, Color border) ApplyInteractionStateColors(
            CheckBoxItemState state,
            Color background,
            Color border)
        {
            if (state.IsDisabled)
            {
                return (ControlPaint.Light(background, 0.18f), ControlPaint.Light(border, 0.30f));
            }

            if (state.IsHovered)
            {
                return (ControlPaint.Light(background, 0.08f), ControlPaint.Light(border, 0.10f));
            }

            return (background, border);
        }

        protected Rectangle GetFocusRingBounds(Rectangle bounds)
        {
            var ringRect = Rectangle.Inflate(bounds, 3, 3);
            ringRect.Width = Math.Max(1, ringRect.Width);
            ringRect.Height = Math.Max(1, ringRect.Height);
            return ringRect;
        }

        protected Rectangle GetGlyphBounds(Rectangle bounds, CheckBoxRenderOptions options)
        {
            int checkSize = options.CheckBoxSize > 0 ? options.CheckBoxSize : Math.Min(bounds.Width, bounds.Height);
            return CheckBoxIconHelpers.CalculateCheckBoxIconBounds(bounds, checkSize, options.GlyphSizeRatio);
        }

        protected Rectangle GetIndeterminateBarBounds(Rectangle glyphBounds)
        {
            int barHeight = Math.Max(2, glyphBounds.Height / 6);
            int y = glyphBounds.Y + (glyphBounds.Height - barHeight) / 2;
            return new Rectangle(glyphBounds.X, y, glyphBounds.Width, barHeight);
        }

        protected void PaintFocusRing(Graphics g, Rectangle bounds, CheckBoxRenderOptions options)
        {
            if (bounds.Width <= 2 || bounds.Height <= 2)
            {
                return;
            }

            Color focusColor = options.Theme?.PrimaryColor != Color.Empty
                ? options.Theme.PrimaryColor
                : Color.FromArgb(46, 110, 240);
            Rectangle ringRect = GetFocusRingBounds(bounds);

            using var ringPen = new Pen(Color.FromArgb(150, focusColor), 2f);
            ringPen.DashStyle = DashStyle.Solid;
            using var ringPath = CreateRoundedPath(ringRect, Math.Max(2, options.BorderRadius + 2));
            g.DrawPath(ringPen, ringPath);
        }

        /// <summary>
        /// Creates a rounded rectangle path
        /// </summary>
        protected GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            if (diameter > bounds.Width || diameter > bounds.Height)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Paints a check mark using standard drawing
        /// </summary>
        protected void PaintStandardCheckMark(
            Graphics g,
            Rectangle bounds,
            CheckBoxRenderOptions options)
        {
            Color checkMarkColor = CheckBoxIconHelpers.GetIconColor(
                options.Theme,
                options.UseThemeColors,
                isChecked: true);

            string iconPath = string.IsNullOrWhiteSpace(options.CheckIconPath)
                ? CheckBoxIconHelpers.GetCheckIconPath()
                : options.CheckIconPath;
            Rectangle iconBounds = GetGlyphBounds(bounds, options);

            if (!string.IsNullOrWhiteSpace(iconPath))
            {
                CheckBoxIconHelpers.PaintIcon(
                    g,
                    iconBounds,
                    iconPath,
                    checkMarkColor,
                    options.Theme,
                    options.UseThemeColors,
                    isChecked: true,
                    controlStyle: options.ControlStyle);
                return;
            }

            using (var pen = new Pen(checkMarkColor, options.CheckMarkThickness))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                PointF[] checkMarkPoints = new PointF[]
                {
                    new PointF(iconBounds.X + iconBounds.Width * 0.15f, iconBounds.Y + iconBounds.Height * 0.55f),
                    new PointF(iconBounds.X + iconBounds.Width * 0.42f, iconBounds.Y + iconBounds.Height * 0.82f),
                    new PointF(iconBounds.X + iconBounds.Width * 0.86f, iconBounds.Y + iconBounds.Height * 0.20f)
                };

                g.DrawLines(pen, checkMarkPoints);
            }
        }

        /// <summary>
        /// Paints an indeterminate mark (dash/minus)
        /// </summary>
        protected void PaintStandardIndeterminateMark(
            Graphics g,
            Rectangle bounds,
            CheckBoxRenderOptions options)
        {
            Color indeterminateColor = CheckBoxIconHelpers.GetIconColor(
                options.Theme,
                options.UseThemeColors,
                isIndeterminate: true);

            string iconPath = string.IsNullOrWhiteSpace(options.IndeterminateIconPath)
                ? CheckBoxIconHelpers.GetIndeterminateIconPath()
                : options.IndeterminateIconPath;
            Rectangle iconBounds = GetGlyphBounds(bounds, options);

            if (!string.IsNullOrWhiteSpace(iconPath))
            {
                CheckBoxIconHelpers.PaintIcon(
                    g,
                    iconBounds,
                    iconPath,
                    indeterminateColor,
                    options.Theme,
                    options.UseThemeColors,
                    isIndeterminate: true,
                    controlStyle: options.ControlStyle);
                return;
            }

            using (var brush = new SolidBrush(indeterminateColor))
            {
                Rectangle indeterminateRect = GetIndeterminateBarBounds(iconBounds);

                g.FillRectangle(brush, indeterminateRect);
            }
        }

        /// <summary>
        /// Paints text using font helpers
        /// </summary>
        protected void PaintStandardText(
            Graphics g,
            Rectangle bounds,
            string text,
            CheckBoxRenderOptions options)
        {
            if (string.IsNullOrEmpty(text) || bounds.IsEmpty)
                return;

            Font font = options.TextFont;
            if (font == null)
            {
                // Resolve typography from theme via BeepThemesManager helper path.
                font = CheckBoxFontHelpers.GetCheckBoxFont(options.Theme, options.ControlStyle);
            }

            Color textColor = CheckBoxThemeHelpers.GetForegroundColor(
                options.Theme,
                options.UseThemeColors);

            TextFormatFlags flags = TextFormatFlags.VerticalCenter |
                                    TextFormatFlags.EndEllipsis |
                                    TextFormatFlags.NoPrefix;
            flags |= options.TextAlignment switch
            {
                TextAlignment.Left => TextFormatFlags.Right,
                TextAlignment.Above => TextFormatFlags.HorizontalCenter,
                TextAlignment.Below => TextFormatFlags.HorizontalCenter,
                _ => TextFormatFlags.Left
            };

            if (bounds.Width > 0 && bounds.Height > 0)
            {
                TextRenderer.DrawText(g, text, font, bounds, textColor, flags);
            }
        }

        #endregion
    }
}
