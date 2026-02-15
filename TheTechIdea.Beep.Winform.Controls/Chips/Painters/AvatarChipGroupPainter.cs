using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Chips;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Painters
{
    /// <summary>
    /// Avatar/User style chips with circular profile images.
    /// Perfect for user mentions, team members, contacts.
    /// </summary>
    internal class AvatarChipGroupPainter : IChipGroupPainter
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private readonly BeepImage _iconRenderer = new BeepImage();
        private readonly StringFormat _centerFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };

        // Avatar color palette
        private static readonly Color AvatarBg = Color.FromArgb(236, 239, 241);         // #ECEFF1
        private static readonly Color AvatarSelected = Color.FromArgb(33, 150, 243);    // #2196F3
        private static readonly Color AvatarHover = Color.FromArgb(227, 242, 253);      // #E3F2FD
        private static readonly Color AvatarBorder = Color.FromArgb(176, 190, 197);     // #B0BEC5
        private static readonly Color TextDark = Color.FromArgb(55, 71, 79);            // #37474F

        // Default avatar colors for users without images
        private static readonly Color[] AvatarColors = new[]
        {
            Color.FromArgb(239, 83, 80),    // Red
            Color.FromArgb(171, 71, 188),   // Purple
            Color.FromArgb(66, 165, 245),   // Blue
            Color.FromArgb(38, 166, 154),   // Teal
            Color.FromArgb(255, 167, 38),   // Orange
            Color.FromArgb(141, 110, 99),   // Brown
        };

        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
        }

        public void UpdateTheme(IBeepTheme theme)
        {
            _theme = theme;
        }

        public Size MeasureChip(SimpleItem item, Graphics g, ChipRenderOptions options)
        {
            float scale = DpiScalingHelper.GetDpiScaleFactor(_owner);
            string text = item?.Text ?? item?.Name ?? item?.DisplayField ?? string.Empty;
            var font = GetFont(options, scale);
            var textSize = TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);

            int extraWidth = 0;
            // Avatar circle always shown
            int avatarSize = GetAvatarSize(options.Size, scale);
            extraWidth += avatarSize + DpiScalingHelper.ScaleValue(8, scale);
            // Close button
            if (options.ShowCloseOnSelected)
                extraWidth += DpiScalingHelper.ScaleValue(16, scale);

            int height = GetChipHeight(options.Size, scale);
            int padding = GetHorizontalPadding(options.Size, scale);

            return new Size(textSize.Width + padding + extraWidth, height);
        }

        public void RenderChip(Graphics g, SimpleItem item, Rectangle bounds, ChipVisualState state, ChipRenderOptions options, out Rectangle closeRect)
        {
            float scale = DpiScalingHelper.GetDpiScaleFactor(_owner);
            closeRect = Rectangle.Empty;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var font = GetFont(options, scale);
            var (bgColor, fgColor, borderColor) = GetColors(state);

            int cornerRadius = DpiScalingHelper.ScaleValue(16, scale);
            using var path = CreateRoundedPath(bounds, cornerRadius);

            // Background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }

            // Border
            using (var pen = new Pen(borderColor, DpiScalingHelper.ScaleValue(1f, scale)))
            {
                g.DrawPath(pen, path);
            }

            var contentRect = Rectangle.Inflate(bounds, -DpiScalingHelper.ScaleValue(4, scale), -DpiScalingHelper.ScaleValue(2, scale));
            int leftOffset = 0;
            int rightOffset = 0;

            // Avatar circle (always on left, slightly protruding)
            int avatarSize = GetAvatarSize(options.Size, scale);
            var avatarRect = new Rectangle(
                bounds.Left + DpiScalingHelper.ScaleValue(2, scale),
                bounds.Top + (bounds.Height - avatarSize) / 2,
                avatarSize, avatarSize);

            DrawAvatar(g, avatarRect, item, state.IsSelected, scale);
            leftOffset += avatarSize + DpiScalingHelper.ScaleValue(6, scale);

            // Selection badge on avatar
            if (state.IsSelected)
            {
                int badgeSize = avatarSize / 3;
                var badgeRect = new Rectangle(
                    avatarRect.Right - badgeSize,
                    avatarRect.Bottom - badgeSize,
                    badgeSize, badgeSize);
                DrawSelectionBadge(g, badgeRect, scale);
            }

            // Close button
            if (options.ShowCloseOnSelected && state.IsSelected)
            {
                int closeSize = Math.Min(contentRect.Height - 6, DpiScalingHelper.ScaleValue(12, scale));
                closeRect = new Rectangle(
                    contentRect.Right - closeSize - 2,
                    contentRect.Top + (contentRect.Height - closeSize) / 2,
                    closeSize, closeSize);

                DrawCloseButton(g, closeRect, fgColor, scale);
                rightOffset += closeSize + DpiScalingHelper.ScaleValue(8, scale);
            }

            // Text (user name)
            var textRect = new Rectangle(
                bounds.Left + leftOffset + DpiScalingHelper.ScaleValue(4, scale),
                contentRect.Top,
                contentRect.Width - leftOffset - rightOffset,
                contentRect.Height);

            using (var textBrush = new SolidBrush(fgColor))
            {
                g.DrawString(item?.Text ?? item?.DisplayField ?? string.Empty, font, textBrush, textRect, _centerFormat);
            }
        }

        public void RenderGroupBackground(Graphics g, Rectangle drawingRect, ChipRenderOptions options)
        {
            // No special group background
        }

        #region Private Helpers

        private void DrawAvatar(Graphics g, Rectangle rect, SimpleItem item, bool isSelected, float scale)
        {
            using var clipPath = new GraphicsPath();
            clipPath.AddEllipse(rect);

            // Check if item has an image
            bool hasImage = !string.IsNullOrEmpty(item?.ImagePath);

            if (hasImage)
            {
                try
                {
                    // Draw circular avatar image
                    StyledImagePainter.Paint(g, clipPath, item.ImagePath);
                }
                catch
                {
                    // Fallback to initials
                    DrawInitialsAvatar(g, rect, item);
                }
            }
            else
            {
                // Draw initials-based avatar
                DrawInitialsAvatar(g, rect, item);
            }

            // Avatar border
            using var borderPen = new Pen(isSelected ? AvatarSelected : Color.White, DpiScalingHelper.ScaleValue(2f, scale));
            g.DrawEllipse(borderPen, rect);
        }

        private void DrawInitialsAvatar(Graphics g, Rectangle rect, SimpleItem item)
        {
            // Get color based on item hash
            int colorIndex = Math.Abs((item?.GuidId ?? item?.Text ?? "").GetHashCode()) % AvatarColors.Length;
            Color avatarColor = AvatarColors[colorIndex];

            // Fill circle
            using (var brush = new SolidBrush(avatarColor))
            {
                g.FillEllipse(brush, rect);
            }

            // Draw initials
            string initials = GetInitials(item?.Text ?? item?.DisplayField ?? "?");
            var font = new Font("Segoe UI", rect.Height * 0.35f, FontStyle.Bold);
            using (var brush = new SolidBrush(Color.White))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(initials, font, brush, rect, format);
            }
            font.Dispose();
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return name.Substring(0, Math.Min(2, name.Length)).ToUpper();
        }

        private void DrawSelectionBadge(Graphics g, Rectangle rect, float scale)
        {
            // Blue circle with white checkmark
            using (var brush = new SolidBrush(AvatarSelected))
            {
                g.FillEllipse(brush, rect);
            }

            // White border
            using (var pen = new Pen(Color.White, DpiScalingHelper.ScaleValue(1f, scale)))
            {
                g.DrawEllipse(pen, rect);
            }

            // Tiny checkmark
            using var checkPen = new Pen(Color.White, DpiScalingHelper.ScaleValue(1.5f, scale)) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            int inset = DpiScalingHelper.ScaleValue(2, scale);
            var points = new Point[]
            {
                new Point(rect.Left + inset, rect.Top + rect.Height / 2),
                new Point(rect.Left + rect.Width / 3, rect.Bottom - inset),
                new Point(rect.Right - inset, rect.Top + inset + 1)
            };
            g.DrawLines(checkPen, points);
        }

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int r = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
            if (r <= 0) { path.AddRectangle(rect); return path; }
            int d = r * 2;
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void DrawCloseButton(Graphics g, Rectangle rect, Color color, float scale)
        {
            using var pen = new Pen(color, DpiScalingHelper.ScaleValue(1.5f, scale)) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawLine(pen, rect.Left + 2, rect.Top + 2, rect.Right - 2, rect.Bottom - 2);
            g.DrawLine(pen, rect.Right - 2, rect.Top + 2, rect.Left + 2, rect.Bottom - 2);
        }

        private int GetAvatarSize(ChipSize size, float scale)
        {
            int val = size switch
            {
                ChipSize.Small => 22,
                ChipSize.Medium => 28,
                ChipSize.Large => 36,
                _ => 28
            };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private int GetChipHeight(ChipSize size, float scale)
        {
            int val = size switch
            {
                ChipSize.Small => 28,
                ChipSize.Medium => 36,
                ChipSize.Large => 44,
                _ => 36
            };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private int GetHorizontalPadding(ChipSize size, float scale)
        {
            int val = size switch
            {
                ChipSize.Small => 12,
                ChipSize.Medium => 16,
                ChipSize.Large => 20,
                _ => 16
            };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private Font GetFont(ChipRenderOptions options, float scale)
        {
            return ChipFontHelpers.GetChipFont(_owner.ControlStyle, options.Size, scale);
        }

        private (Color bg, Color fg, Color border) GetColors(ChipVisualState state)
        {
            if (state.IsSelected)
            {
                return (AvatarHover, TextDark, AvatarSelected);
            }
            else if (state.IsHovered)
            {
                return (AvatarHover, TextDark, AvatarBorder);
            }
            else
            {
                return (AvatarBg, TextDark, AvatarBorder);
            }
        }

        #endregion
    }
}
