using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Data-dense list variant — synced with <see cref="Popup.DenseAvatarPopupContent"/>.
    /// Popup draws circular avatars (28px), initial-letter fallback, status dots, avatar ring
    /// for selected, compact 38px rows. Field echoes: circular avatar clip for leading image,
    /// status dot indicator, dense font, compact padding, thin crisp focus border.
    /// </summary>
    internal sealed class DenseListComboBoxPainter : DesignSystemComboBoxFieldPainterBase
    {
        protected override int CornerRadius => 4;
        protected override bool ShowButtonSeparator => true;

        public override int GetPreferredButtonWidth() => 28;
        public override Padding GetPreferredPadding() => new Padding(6, 3, 4, 3);

        protected override void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            // Dense: minimal hover feedback — just a faint tint
            var istate = GetInteractionState();
            if (istate == ComboBoxInteractionState.Hover)
            {
                using var brush = new SolidBrush(Color.FromArgb(6, _theme?.ForeColor ?? Color.Black));
                g.FillRectangle(brush, textAreaRect);
            }
        }

        /// <summary>
        /// Override leading image to draw circular avatar matching popup AvatarRow style.
        /// Uses circular clip + initial-letter fallback with hash-based avatar colors.
        /// </summary>
        protected override void DrawLeadingImage(Graphics g, Rectangle imageRect)
        {
            if (imageRect.IsEmpty) return;

            string imagePath = _owner?.LeadingImagePath;
            if (string.IsNullOrEmpty(imagePath))
                imagePath = _owner?.LeadingIconPath;

            // Make avatar circular (square → circle) matching popup 28px circular avatars
            int avatarSize = Math.Min(imageRect.Width, imageRect.Height);
            var avatarRect = new Rectangle(
                imageRect.X + (imageRect.Width - avatarSize) / 2,
                imageRect.Y + (imageRect.Height - avatarSize) / 2,
                avatarSize, avatarSize);

            if (!string.IsNullOrEmpty(imagePath))
            {
                // Circular clip for image — matches popup AvatarRow circular avatar
                var state = g.Save();
                using var clipPath = new GraphicsPath();
                clipPath.AddEllipse(avatarRect);
                g.SetClip(clipPath, CombineMode.Intersect);
                StyledImagePainter.Paint(g, avatarRect, imagePath, BeepControlStyle.Minimal);
                g.Restore(state);
            }
            else
            {
                // Initial-letter fallback — matches popup AvatarRow GetAvatarColor logic
                string displayText = _helper?.GetDisplayText() ?? "";
                Color avatarColor = GetAvatarColor(displayText);
                using var avatarBrush = new SolidBrush(avatarColor);
                g.FillEllipse(avatarBrush, avatarRect);

                string initial = !string.IsNullOrEmpty(displayText) ? displayText[0].ToString().ToUpper() : "?";
                using var initialFont = new Font(SystemFonts.DefaultFont.FontFamily, Math.Max(7, avatarSize * 0.4f), FontStyle.Bold);
                TextRenderer.DrawText(g, initial, initialFont, avatarRect, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            // Status dot indicator — matches popup AvatarRow status dot (10px colored circle)
            int dotSize = Math.Max(6, ScaleX(7));
            int dotX = avatarRect.Right - dotSize + 1;
            int dotY = avatarRect.Bottom - dotSize + 1;
            Color dotColor = Color.FromArgb(76, 175, 80); // default: green "online"
            using var dotBrush = new SolidBrush(dotColor);
            g.FillEllipse(dotBrush, dotX, dotY, dotSize, dotSize);
            // White border around dot for visibility
            using var dotBorderPen = new Pen(Color.White, 1.2f);
            g.DrawEllipse(dotBorderPen, dotX, dotY, dotSize, dotSize);
        }

        protected override void DrawText(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;

            string displayText = _helper.GetDisplayText();
            if (string.IsNullOrEmpty(displayText)) return;

            Color textColor;
            if (_helper.IsShowingPlaceholder())
            {
                textColor = Color.FromArgb(140, _theme?.SecondaryColor ?? Color.Gray);
            }
            else
            {
                textColor = _helper.GetTextColor();
            }

            // Dense font (-1pt) matching popup AvatarRow compact text rendering
            Font baseFont = _owner.TextFont ?? BeepThemesManager.ToFont(_theme?.LabelFont) ?? SystemFonts.DefaultFont;
            float denseFontSize = Math.Max(7f, baseFont.Size - 1f);
            using var denseFont = new Font(baseFont.FontFamily, denseFontSize, baseFont.Style);

            int horizontalInset = ScaleX(_tokens?.TextInset ?? 4);
            var textBounds = new Rectangle(
                textAreaRect.X + horizontalInset,
                textAreaRect.Y,
                Math.Max(1, textAreaRect.Width - horizontalInset * 2),
                textAreaRect.Height);

            TextRenderer.DrawText(g, displayText, denseFont, textBounds, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        protected override void DrawDecorations(Graphics g, Rectangle drawingRect)
        {
            if (drawingRect.IsEmpty) return;
            var istate = GetInteractionState();

            // Focus: thin crisp border (1.5px) — matches popup compact row density
            if (istate == ComboBoxInteractionState.Focused || istate == ComboBoxInteractionState.Open)
            {
                Color focusColor = _theme?.PrimaryColor ?? Color.FromArgb(25, 118, 210);
                int radius = ScaleX(CornerRadius);
                using var path = GetRoundedRectPath(drawingRect, radius);
                using var pen = new Pen(focusColor, 1.5f);
                g.DrawPath(pen, path);
            }
        }

        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            // Compact separator — matches popup's tight row density
            if (ShowButtonSeparator)
            {
                Color sepColor = Color.FromArgb(80, _theme?.BorderColor ?? Color.Gray);
                int sepTop = buttonRect.Top + ScaleY(4);
                int sepBot = buttonRect.Bottom - ScaleY(4);
                if (sepBot > sepTop)
                    g.DrawLine(PaintersFactory.GetPen(sepColor, 1f), buttonRect.Left, sepTop, buttonRect.Left, sepBot);
            }

            // Compact chevron — smaller (12px), matching popup's dense 38px row height
            Color arrowColor = GetArrowColor();
            int iconSize = Math.Min(ScaleX(12), Math.Min(buttonRect.Width, buttonRect.Height) - ScaleX(6));
            if (iconSize > 3)
            {
                var iconRect = new Rectangle(
                    buttonRect.X + (buttonRect.Width - iconSize) / 2,
                    buttonRect.Y + (buttonRect.Height - iconSize) / 2,
                    iconSize, iconSize);
                DrawSvgIcon(g, iconRect, SvgsUI.ChevronDown, arrowColor, 0.85f, _owner?.ChevronAngle ?? 0f);
            }
        }

        /// <summary>
        /// Hash-based avatar color — mirrors popup AvatarRow.GetAvatarColor for visual consistency.
        /// </summary>
        private static Color GetAvatarColor(string text)
        {
            if (string.IsNullOrEmpty(text)) return Color.FromArgb(158, 158, 158);
            int hash = 0;
            foreach (char c in text) hash = (hash * 31 + c) & 0x7FFFFFFF;
            var colors = new[]
            {
                Color.FromArgb(183, 28, 28),
                Color.FromArgb(25, 118, 210),
                Color.FromArgb(56, 142, 60),
                Color.FromArgb(255, 143, 0),
                Color.FromArgb(106, 27, 154),
                Color.FromArgb(0, 151, 167),
                Color.FromArgb(216, 67, 21)
            };
            return colors[hash % colors.Length];
        }
    }
}
