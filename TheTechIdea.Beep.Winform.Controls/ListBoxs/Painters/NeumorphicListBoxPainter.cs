using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Neumorphic (Soft UI) style painter - soft shadows and embossed/extruded look
    /// Modern design trend with subtle 3D effects using shadows
    /// </summary>
    internal class NeumorphicListBoxPainter : BaseListBoxPainter
    {
        private readonly int _cornerRadius = 12;
        private readonly int _shadowBlur = 6;
        private Color _baseColor = Color.FromArgb(235, 235, 240);

        public override int GetPreferredItemHeight()
        {
            return Math.Max(_owner.Font.Height + 24, 48);
        }

        public override void Paint(Graphics g, BeepListBox owner, Rectangle drawingRect)
        {
            // Set base color from theme
            _baseColor = _theme?.BackgroundColor ?? Color.FromArgb(235, 235, 240);
            
            // Ensure base color is light enough for neumorphism
            if (GetLuminance(_baseColor) < 0.5f)
            {
                _baseColor = Color.FromArgb(235, 235, 240);
            }

            base.Paint(g, owner, drawingRect);
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // Draw neumorphic background
            DrawNeumorphicBackground(g, itemRect, isHovered, isSelected);

            // Get layout info
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            Rectangle checkRect = info?.CheckRect ?? Rectangle.Empty;
            Rectangle iconRect = info?.IconRect ?? Rectangle.Empty;
            Rectangle textRect = info?.TextRect ?? itemRect;

            // Checkbox with neumorphic style
            if (_owner.ShowCheckBox && SupportsCheckboxes() && !checkRect.IsEmpty)
            {
                bool isChecked = _owner.IsItemSelected(item);
                DrawNeumorphicCheckbox(g, checkRect, isChecked, isHovered);
            }

            // Icon
            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath) && !iconRect.IsEmpty)
            {
                DrawItemImage(g, iconRect, item.ImagePath);
            }

            // Text
            Color textColor = _theme?.ListItemForeColor ?? Color.FromArgb(60, 60, 60);
            if (isSelected)
            {
                textColor = _theme?.PrimaryColor ?? Color.FromArgb(0, 100, 180);
            }
            
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);

            // Subtext
            if (!string.IsNullOrEmpty(item.SubText))
            {
                var subRect = new Rectangle(textRect.X, textRect.Y + textRect.Height / 2 + 2, 
                    textRect.Width, textRect.Height / 2 - 4);
                var subColor = Color.FromArgb(140, textColor);
                using (var subFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size - 1, FontStyle.Regular))
                {
                    DrawItemText(g, subRect, item.SubText, subColor, subFont);
                }
            }
        }

        private void DrawNeumorphicBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            var neuRect = Rectangle.Inflate(itemRect, -4, -2);
            
            // Calculate shadow colors
            Color lightShadow = LightenColor(_baseColor, 0.15f);
            Color darkShadow = DarkenColor(_baseColor, 0.15f);

            if (isSelected)
            {
                // Inset/pressed effect for selected items
                DrawInsetNeumorphic(g, neuRect, lightShadow, darkShadow);
                
                // Accent indicator
                var accentColor = _theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215);
                using (var accentBrush = new SolidBrush(accentColor))
                {
                    var accentRect = new Rectangle(neuRect.Left, neuRect.Top + 4, 4, neuRect.Height - 8);
                    using (var accentPath = GraphicsExtensions.CreateRoundedRectanglePath(accentRect, 2))
                    {
                        g.FillPath(accentBrush, accentPath);
                    }
                }
            }
            else if (isHovered)
            {
                // Slightly raised effect for hover
                DrawRaisedNeumorphic(g, neuRect, lightShadow, darkShadow, 0.7f);
            }
            else
            {
                // Subtle raised effect for normal state
                DrawRaisedNeumorphic(g, neuRect, lightShadow, darkShadow, 0.3f);
            }
        }

        private void DrawRaisedNeumorphic(Graphics g, Rectangle rect, Color lightShadow, Color darkShadow, float intensity)
        {
            int shadowOffset = (int)(_shadowBlur * intensity);
            
            // Draw dark shadow (bottom-right)
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(
                new Rectangle(rect.X + shadowOffset, rect.Y + shadowOffset, rect.Width, rect.Height), _cornerRadius))
            using (var brush = new SolidBrush(Color.FromArgb((int)(40 * intensity), darkShadow)))
            {
                g.FillPath(brush, path);
            }

            // Draw light shadow (top-left)
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(
                new Rectangle(rect.X - shadowOffset / 2, rect.Y - shadowOffset / 2, rect.Width, rect.Height), _cornerRadius))
            using (var brush = new SolidBrush(Color.FromArgb((int)(60 * intensity), lightShadow)))
            {
                g.FillPath(brush, path);
            }

            // Draw main surface
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(rect, _cornerRadius))
            using (var brush = new SolidBrush(_baseColor))
            {
                g.FillPath(brush, path);
            }
        }

        private void DrawInsetNeumorphic(Graphics g, Rectangle rect, Color lightShadow, Color darkShadow)
        {
            // Draw main surface first
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(rect, _cornerRadius))
            using (var brush = new SolidBrush(DarkenColor(_baseColor, 0.05f)))
            {
                g.FillPath(brush, path);
            }

            // Inner shadow (dark on top-left for inset effect)
            var innerRect = Rectangle.Inflate(rect, -2, -2);
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(innerRect, _cornerRadius - 2))
            {
                // Top-left inner shadow
                using (var brush = new LinearGradientBrush(innerRect,
                    Color.FromArgb(40, darkShadow),
                    Color.FromArgb(0, darkShadow),
                    LinearGradientMode.ForwardDiagonal))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private void DrawNeumorphicCheckbox(Graphics g, Rectangle checkRect, bool isChecked, bool isHovered)
        {
            Color lightShadow = LightenColor(_baseColor, 0.15f);
            Color darkShadow = DarkenColor(_baseColor, 0.15f);

            if (isChecked)
            {
                // Inset checkbox
                using (var path = GraphicsExtensions.CreateRoundedRectanglePath(checkRect, 4))
                {
                    // Background
                    var accentColor = _theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215);
                    using (var brush = new SolidBrush(accentColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Checkmark
                    using (var pen = new Pen(Color.White, 2f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        Point[] checkPoints = new Point[]
                        {
                            new Point(checkRect.Left + 4, checkRect.Top + checkRect.Height / 2),
                            new Point(checkRect.Left + checkRect.Width / 2 - 1, checkRect.Bottom - 4),
                            new Point(checkRect.Right - 4, checkRect.Top + 4)
                        };
                        g.DrawLines(pen, checkPoints);
                    }
                }
            }
            else
            {
                // Raised checkbox
                DrawRaisedNeumorphic(g, checkRect, lightShadow, darkShadow, isHovered ? 0.6f : 0.4f);
            }
        }

        private float GetLuminance(Color c)
        {
            return (0.299f * c.R + 0.587f * c.G + 0.114f * c.B) / 255f;
        }

        private Color LightenColor(Color c, float amount)
        {
            int r = Math.Min(255, (int)(c.R + (255 - c.R) * amount));
            int g = Math.Min(255, (int)(c.G + (255 - c.G) * amount));
            int b = Math.Min(255, (int)(c.B + (255 - c.B) * amount));
            return Color.FromArgb(c.A, r, g, b);
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
            // Background is handled in DrawNeumorphicBackground
        }
    }
}

