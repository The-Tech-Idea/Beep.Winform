using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Gradient Card style painter - colorful gradient backgrounds with card-like items
    /// Modern and vibrant design with smooth color transitions
    /// </summary>
    internal class GradientCardListBoxPainter : BaseListBoxPainter
    {
        private readonly int _cornerRadius = 10;
        private readonly int _itemGap = 6;

        /// <summary>
        /// Gradient pairs built from the theme, not from six hard-coded web gradients.
        /// </summary>
        /// <remarks>
        /// These were <c>static readonly</c> literals, so every theme - light, dark or high
        /// contrast - drew the same purple-blue and pink-peach cards. Built per call now because
        /// the theme can change at any time; the pairs still differ from one another, so cards
        /// remain distinguishable.
        /// </remarks>
        private Color[][] GradientPalettes
        {
            get
            {
                var t = Theme;
                return new Color[][]
                {
                    new[] { t.PrimaryColor,   t.AccentColor    },
                    new[] { t.AccentColor,    t.PrimaryColor   },
                    new[] { t.SuccessColor,   t.AccentColor    },
                    new[] { t.WarningColor,   t.ErrorColor     },
                    new[] { t.SecondaryColor, t.PrimaryColor   },
                    new[] { t.SurfaceColor, t.AccentColor    },
                };
            }
        }

        public override int GetPreferredItemHeight()
        {
            return DpiScalingHelper.ScaleValue(ListBoxTokens.ItemHeightComfortable, _owner ?? new System.Windows.Forms.Control());
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            // Use visible-list index so gradients remain deterministic under filtering/grouping/hierarchy.
            // A stable index without a linear search. IndexOf ran over the whole visible list for
            // every item on every paint - O(n squared) purely to choose a gradient. The hash gives
            // the same card the same gradient without walking anything.
            int itemIndex = Math.Abs((item.GuidId ?? item.Text ?? string.Empty).GetHashCode());
            
            // Draw gradient card background
            DrawGradientCard(g, itemRect, itemIndex, isHovered, isSelected);

            // Get layout info
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            Rectangle checkRect = info?.CheckRect ?? Rectangle.Empty;
            Rectangle iconRect = info?.IconRect ?? Rectangle.Empty;
            Rectangle textRect = info?.TextRect ?? itemRect;

            // Checkbox
            if (_owner.ShowCheckBox && SupportsCheckboxes() && !checkRect.IsEmpty)
            {
                bool isChecked = _owner.IsItemSelected(item);
                DrawGradientCheckbox(g, checkRect, isChecked, itemIndex);
            }

            // Icon with circular background
            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath) && !iconRect.IsEmpty)
            {
                DrawCircularIcon(g, iconRect, item.ImagePath, isSelected);
            }

            // Text - always white for contrast on gradient
            Color textColor = Theme.OnPrimaryColor;
            if (!isSelected && !isHovered)
            {
                textColor = Theme.ListItemForeColor;
            }
            
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);

            // Subtext
            if (!string.IsNullOrWhiteSpace(SecondLine(item)))
            {
                var subRect = new Rectangle(textRect.X, textRect.Y + textRect.Height / 2 + Scale(2), 
                    textRect.Width, textRect.Height / 2 - Scale(4));
                var subColor = Color.FromArgb(ListBoxTokens.SubTextAlpha, textColor);
                var subFont = GetCachedFont(_owner.TextFont.Size - 1, FontStyle.Regular);
                DrawItemText(g, subRect, SecondLine(item), subColor, subFont);
            }
        }

        private void DrawGradientCard(Graphics g, Rectangle itemRect, int itemIndex, bool isHovered, bool isSelected)
        {
            var cardRect = Rectangle.Inflate(itemRect, -Scale(3), -Scale(2));
            
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(cardRect, new CornerRadius(Scale(_cornerRadius))))
            {
                if (isSelected)
                {
                    // Use theme primary color gradient for selected
                    var primaryColor = Theme.PrimaryColor;
                    var secondaryColor = _theme?.AccentColor ?? DarkenColor(primaryColor, 0.3f);
                    
                    using (var brush = new LinearGradientBrush(cardRect,
                        primaryColor, secondaryColor, LinearGradientMode.Horizontal))
                    {
                        g.FillPath(brush, path);
                    }

                    // Glow effect
                    DrawCardGlow(g, cardRect, primaryColor);
                }
                else if (isHovered)
                {
                    // Subtle gradient on hover
                    var palette = GetPalette(itemIndex);
                    using (var brush = new LinearGradientBrush(cardRect,
                        Color.FromArgb(ListBoxTokens.ActiveOverlayAlpha, palette[0]),
                        Color.FromArgb(ListBoxTokens.ActiveOverlayAlpha, palette[1]),
                        LinearGradientMode.Horizontal))
                    {
                        g.FillPath(brush, path);
                    }

                    // Hover border
                    g.DrawPath(GetPen(Color.FromArgb(ListBoxTokens.ActiveOverlayAlpha, palette[0]), 1.5f), path);
                }
                else
                {
                    // Default: subtle background
                    g.FillPath(GetBrush(Theme.ListBackColor), path);

                    // Subtle border
                    g.DrawPath(GetPen(Color.FromArgb(40, 0, 0, 0), 0.5f), path);
                }
            }
        }

        private void DrawCardGlow(Graphics g, Rectangle cardRect, Color glowColor)
        {
            // Outer glow effect
            for (int i = 3; i >= 1; i--)
            {
                var glowRect = Rectangle.Inflate(cardRect, i, i);
                using (var path = GraphicsExtensions.CreateRoundedRectanglePath(glowRect, new CornerRadius(Scale(_cornerRadius) + i)))
                {
                    g.DrawPath(GetPen(Color.FromArgb(20 * (4 - i), glowColor), 1f), path);
                }
            }
        }

        private void DrawCircularIcon(Graphics g, Rectangle iconRect, string imagePath, bool isSelected)
        {
            // Draw circular background
            var circleRect = iconRect;
            g.FillEllipse(GetBrush(isSelected
                ? Color.FromArgb(ListBoxTokens.ActiveOverlayAlpha, Theme.OnPrimaryColor)
                : Color.FromArgb(ListBoxTokens.HoverOverlayAlpha, Theme.ListItemForeColor)), circleRect);

            // Draw icon using StyledImagePainter circular rendering
            float cx = circleRect.X + circleRect.Width / 2f;
            float cy = circleRect.Y + circleRect.Height / 2f;
            float radius = (circleRect.Width / 2f) - 4;
            StyledImagePainter.PaintInCircle(g, cx, cy, radius, imagePath);
        }

        private void DrawGradientCheckbox(Graphics g, Rectangle checkRect, bool isChecked, int itemIndex)
        {
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(checkRect, new CornerRadius(Scale(4))))
            {
                if (isChecked)
                {
                    var palette = GetPalette(itemIndex);
                    using (var brush = new LinearGradientBrush(checkRect,
                        palette[0], palette[1], LinearGradientMode.ForwardDiagonal))
                    {
                        g.FillPath(brush, path);
                    }

                    // Checkmark
                    using (var pen = new Pen(Theme.OnPrimaryColor, 2f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        int cp = Scale(4);
                        Point[] checkPoints = new Point[]
                        {
                            new Point(checkRect.Left + cp, checkRect.Top + checkRect.Height / 2),
                            new Point(checkRect.Left + checkRect.Width / 2 - Scale(1), checkRect.Bottom - cp),
                            new Point(checkRect.Right - cp, checkRect.Top + cp)
                        };
                        g.DrawLines(pen, checkPoints);
                    }
                }
                else
                {
                    g.FillPath(GetBrush(Color.FromArgb(ListBoxTokens.HoverOverlayAlpha,
                        Theme.ListItemForeColor)), path);
                    g.DrawPath(GetPen(Color.FromArgb(ListBoxTokens.ActiveOverlayAlpha,
                        Theme.BorderColor), 1f), path);
                }
            }
        }

        private Color[] GetPalette(int index)
        {
            return GradientPalettes[Math.Abs(index) % GradientPalettes.Length];
        }

        private Color DarkenColor(Color c, float amount)
        {
            int r = Math.Max(0, (int)(c.R * (1 - amount)));
            int g = Math.Max(0, (int)(c.G * (1 - amount)));
            int b = Math.Max(0, (int)(c.B * (1 - amount)));
            return Color.FromArgb(c.A, r, g, b);
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // The row surface, under the gradient card. This override was empty, so the row was
            // transparent and the style inherited whatever the control had painted beneath it -
            // the reason it stayed light under a dark theme.
            if (g == null || itemRect.IsEmpty) return;
            g.FillRectangle(GetBrush(Theme.ListBackColor), itemRect);
        }
    }
}

