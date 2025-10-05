using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Base class for NumericUpDown painters providing common functionality
    /// </summary>
    public abstract class BaseNumericUpDownPainter : INumericUpDownPainter
    {
        public abstract void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds);
        public abstract void PaintButtons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect);
        public abstract void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText);

        public virtual void UpdateHitAreas(INumericUpDownPainterContext context, Rectangle bounds,
            Action<string, Rectangle, Action> registerHitArea)
        {
            if (!context.ShowSpinButtons) return;

            int buttonWidth = GetButtonWidth(context, bounds);
            int buttonHeight = bounds.Height;

            // Down button (left)
            var downRect = new Rectangle(bounds.X, bounds.Y, buttonWidth, buttonHeight);
            registerHitArea("DownButton", downRect, () => context.DecrementValue());

            // Up button (right)
            var upRect = new Rectangle(bounds.Right - buttonWidth, bounds.Y, buttonWidth, buttonHeight);
            registerHitArea("UpButton", upRect, () => context.IncreaseValue());
        }

        #region Helper Methods

        protected int GetButtonWidth(INumericUpDownPainterContext context, Rectangle bounds)
        {
            return context.ButtonSize switch
            {
                NumericSpinButtonSize.Small => Math.Min(20, bounds.Width / 6),
                NumericSpinButtonSize.Standard => Math.Min(24, bounds.Width / 5),
                NumericSpinButtonSize.Large => Math.Min(28, bounds.Width / 4),
                NumericSpinButtonSize.ExtraLarge => Math.Min(32, bounds.Width / 3),
                _ => Math.Min(24, bounds.Width / 5)
            };
        }

        protected Rectangle GetTextRect(INumericUpDownPainterContext context, Rectangle bounds)
        {
            int padding = 4;
            int buttonWidth = context.ShowSpinButtons ? GetButtonWidth(context, bounds) : 0;
            int textBoxLeft = context.ShowSpinButtons ? padding + buttonWidth : padding;
            int textWidth = bounds.Width - 2 * padding;

            if (context.ShowSpinButtons)
            {
                textWidth -= 2 * buttonWidth;
            }

            return new Rectangle(
                bounds.X + textBoxLeft,
                bounds.Y + padding,
                textWidth,
                bounds.Height - 2 * padding);
        }

        protected GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected void DrawButtonBase(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor, int cornerRadius = 0)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            using (var bgBrush = new SolidBrush(bgColor))
            {
                if (cornerRadius > 0)
                {
                    using (var path = CreateRoundedPath(rect, cornerRadius))
                    {
                        g.FillPath(bgBrush, path);
                    }
                }
                else
                {
                    g.FillRectangle(bgBrush, rect);
                }
            }

            using (var textBrush = new SolidBrush(foreColor))
            using (var font = new Font("Segoe UI", 10f, FontStyle.Bold))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(text, font, textBrush, rect, sf);
            }
        }

        #endregion
    }
}
