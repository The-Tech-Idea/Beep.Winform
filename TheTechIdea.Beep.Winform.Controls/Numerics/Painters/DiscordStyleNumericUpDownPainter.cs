using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using TheTechIdea.Beep.Winform.Controls.Numerics.Painters;

/// <summary>
/// Discord ProgressBarStyle painter for NumericUpDown
/// Features: Dark gray backgrounds, Discord blurple accent, gaming-focused design
/// </summary>
public class DiscordStyleNumericUpDownPainter : BaseNumericUpDownPainter
    {
        public override void Paint(Graphics g, INumericUpDownPainterContext context, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var theme = context.Theme;
            
            // Discord dark background
            Color backColor = Color.FromArgb(64, 68, 75); // Discord dark
            Color blurple = Color.FromArgb(88, 101, 242); // Discord blurple
            Color borderColor = context.IsFocused
                ? blurple
                : Color.FromArgb(47, 49, 54); // Darker gray

            // Draw background with Discord's rounded Style
            int radius = context.IsRounded ? Math.Min(context.BorderRadius, 8) : 8;
            using (var bgBrush = new SolidBrush(backColor))
            using (var path = CreateRoundedPath(bounds, radius))
            {
                g.FillPath(bgBrush, path);

                // Subtle inner shadow
                using (var shadowPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                {
                    var shadowRect = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
                    using (var shadowPath = CreateRoundedPath(shadowRect, radius - 1))
                    {
                        g.DrawPath(shadowPen, shadowPath);
                    }
                }

                // Border with blurple glow when focused
                if (context.IsFocused)
                {
                    using (var glowPen = new Pen(Color.FromArgb(80, blurple), 3))
                    {
                        var glowRect = new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width + 2, bounds.Height + 2);
                        using (var glowPath = CreateRoundedPath(glowRect, radius + 1))
                        {
                            g.DrawPath(glowPen, glowPath);
                        }
                    }
                }

                using (var borderPen = new Pen(borderColor, 1))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            // Draw value text if not editing
            if (!context.IsEditing && context.ShowSpinButtons)
            {
                var textRect = GetTextRect(context, bounds);
                string formattedText = FormatValue(context);
                PaintValueText(g, context, textRect, formattedText);
            }

            // Draw buttons
            if (context.ShowSpinButtons)
            {
                int buttonWidth = GetButtonWidth(context, bounds);
                var downRect = new Rectangle(bounds.X + 2, bounds.Y + 2, buttonWidth - 2, bounds.Height - 4);
                var upRect = new Rectangle(bounds.Right - buttonWidth, bounds.Y + 2, buttonWidth - 2, bounds.Height - 4);
                PaintButtons(g, context, upRect, downRect);
            }
        }

        public override void PaintButtons(Graphics g, INumericUpDownPainterContext context, Rectangle upButtonRect, Rectangle downButtonRect)
        {
            Color buttonColor = Color.FromArgb(79, 84, 92); // Discord button gray
            Color buttonHoverColor = Color.FromArgb(88, 101, 242); // Blurple on hover
            Color buttonPressedColor = Color.FromArgb(71, 82, 196); // Darker blurple
            Color iconColor = Color.FromArgb(220, 221, 222); // Discord light gray
            Color borderColor = Color.FromArgb(47, 49, 54);

            // Down button
            DrawDiscordButton(g, downButtonRect, "−", 
                context.DownButtonPressed, context.DownButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);

            // Up button
            DrawDiscordButton(g, upButtonRect, "+", 
                context.UpButtonPressed, context.UpButtonHovered,
                buttonColor, iconColor, buttonHoverColor, buttonPressedColor, borderColor);
        }

        public override void PaintValueText(Graphics g, INumericUpDownPainterContext context, Rectangle textRect, string formattedText)
        {
            Color textColor = Color.FromArgb(220, 221, 222); // Discord text

            using (var textBrush = new SolidBrush(textColor))
            using (var font = new Font("Segoe UI", 10.5f, FontStyle.Regular))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                g.DrawString(formattedText, font, textBrush, textRect, sf);
            }
        }

        private void DrawDiscordButton(Graphics g, Rectangle rect, string text,
            bool pressed, bool hovered, Color backColor, Color foreColor,
            Color hoverBackColor, Color pressedBackColor, Color borderColor)
        {
            Color bgColor = pressed ? pressedBackColor : (hovered ? hoverBackColor : backColor);

            // Discord rounded button
            int radius = 6;
            using (var bgBrush = new SolidBrush(bgColor))
            using (var path = CreateRoundedPath(rect, radius))
            {
                g.FillPath(bgBrush, path);

                // Subtle border separator
                if (!hovered && !pressed)
                {
                    using (var borderPen = new Pen(borderColor, 1))
                    {
                        if (rect.X < 10) // Down button (left)
                        {
                            g.DrawLine(borderPen, rect.Right, rect.Top + 2, rect.Right, rect.Bottom - 2);
                        }
                        else // Up button (right)
                        {
                            g.DrawLine(borderPen, rect.Left, rect.Top + 2, rect.Left, rect.Bottom - 2);
                        }
                    }
                }
            }

            // Button icon
            using (var textBrush = new SolidBrush(foreColor))
            using (var font = new Font("Segoe UI", 11f, FontStyle.Regular))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(text, font, textBrush, rect, sf);
            }
        }

        private string FormatValue(INumericUpDownPainterContext context)
        {
            string valueStr = context.DecimalPlaces > 0
                ? context.Value.ToString($"N{context.DecimalPlaces}")
                : (context.ThousandsSeparator ? context.Value.ToString("N0") : context.Value.ToString());

            return context.DisplayMode switch
            {
                NumericUpDownDisplayMode.Percentage => $"{valueStr}%",
                NumericUpDownDisplayMode.Currency => $"${valueStr}",
                NumericUpDownDisplayMode.CustomUnit => $"{context.Prefix}{valueStr}{context.Suffix} {context.Unit}".Trim(),
                _ => valueStr
            };
        }
    }

