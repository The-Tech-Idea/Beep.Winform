using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            Color checkMarkColor = CheckBoxThemeHelpers.GetCheckMarkColor(
                options.Theme,
                options.UseThemeColors);

            using (var pen = new Pen(checkMarkColor, options.CheckMarkThickness))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                PointF[] checkMarkPoints = new PointF[]
                {
                    new PointF(bounds.X + bounds.Width * 0.25f, bounds.Y + bounds.Height * 0.5f),
                    new PointF(bounds.X + bounds.Width * 0.5f, bounds.Y + bounds.Height * 0.75f),
                    new PointF(bounds.X + bounds.Width * 0.75f, bounds.Y + bounds.Height * 0.25f)
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
            Color indeterminateColor = CheckBoxThemeHelpers.GetIndeterminateMarkColor(
                options.Theme,
                options.UseThemeColors);

            using (var brush = new SolidBrush(indeterminateColor))
            {
                Rectangle indeterminateRect = new Rectangle(
                    bounds.X + bounds.Width / 4,
                    bounds.Y + bounds.Height / 4,
                    bounds.Width / 2,
                    bounds.Height / 2);

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
                float scale = DpiScalingHelper.GetDpiScaleFactor(g);
                font = CheckBoxFontHelpers.GetCheckBoxFont(options.ControlStyle, scale);
            }

            Color textColor = CheckBoxThemeHelpers.GetForegroundColor(
                options.Theme,
                options.UseThemeColors);

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = options.TextAlignment == TextAlignment.Left ? StringAlignment.Far : StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                g.DrawString(text, font, brush, bounds, format);
            }
        }

        #endregion
    }
}
