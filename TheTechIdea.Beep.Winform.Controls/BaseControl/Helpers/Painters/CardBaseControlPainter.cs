using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// A card-styled painter inspired by the provided UI reference. Renders a rounded card
    /// with optional badge, title, subtitle, and a primary action pill at the bottom-right.
    /// Uses BaseControl.Text as Title, HelperText as Subtitle, and BadgeText as the badge.
    /// Supports optional Leading/Trailing icons placed in the title row.
    /// </summary>
    internal sealed class CardBaseControlPainter : IBaseControlPainter
    {
        // cached layout
        private Rectangle _cardRect;
        private Rectangle _badgeRect;
        private Rectangle _titleRect;
        private Rectangle _subtitleRect;
        private Rectangle _actionRect;
        private Rectangle _contentRect; // body area for user content if needed
        private Rectangle _leadIconRect;
        private Rectangle _trailIconRect;

        public void UpdateLayout(Base.BaseControl owner)
        {
            if (owner == null || owner.Width <= 0 || owner.Height <= 0)
            {
                _cardRect = _badgeRect = _titleRect = _subtitleRect = _actionRect = _contentRect = Rectangle.Empty;
                _leadIconRect = _trailIconRect = Rectangle.Empty;
                return;
            }

            int pad = 12;
            _cardRect = Rectangle.Inflate(new Rectangle(Point.Empty, owner.Size), -2, -2);
            int radius = Math.Max(8, owner.BorderRadius > 0 ? owner.BorderRadius : owner.MaterialBorderRadius);

            // top row: badge (optional) on the right
            int badgeH = 20;
            int badgeW = 0;
            if (!string.IsNullOrEmpty(owner.BadgeText))
            {
                // rough width based on text
                using var g = owner.CreateGraphics();
                var sz = TextRenderer.MeasureText(g, owner.BadgeText, owner.Font);
                badgeW = Math.Min(Math.Max(sz.Width + 16, 40), Math.Max(40, owner.Width / 3));
                _badgeRect = new Rectangle(_cardRect.Right - pad - badgeW, _cardRect.Top + pad, badgeW, badgeH);
            }
            else
            {
                _badgeRect = Rectangle.Empty;
            }

            // title and subtitle
            int topY = _cardRect.Top + pad + 4;
            int rightLimit = _badgeRect.IsEmpty ? _cardRect.Right - pad : _badgeRect.Left - 8;
            _titleRect = new Rectangle(_cardRect.Left + pad, topY, Math.Max(0, rightLimit - (_cardRect.Left + pad)), owner.Font.Height + 8);
            _subtitleRect = new Rectangle(_cardRect.Left + pad, _titleRect.Bottom + 4, Math.Max(0, _cardRect.Right - pad - (_cardRect.Left + pad)), (int)Math.Ceiling(owner.Font.Size + 6));

            // Optional icons in title row
            _leadIconRect = Rectangle.Empty;
            _trailIconRect = Rectangle.Empty;
            int iconPad = Math.Max(0, owner.IconPadding);
            int iconSize = Math.Min(owner.IconSize, Math.Max(12, _titleRect.Height - iconPad));
            bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;

            if (hasLeading)
            {
                _leadIconRect = new Rectangle(_titleRect.Left, _titleRect.Top + (_titleRect.Height - iconSize) / 2, iconSize, iconSize);
                _titleRect = new Rectangle(_leadIconRect.Right + iconPad, _titleRect.Top, Math.Max(0, _titleRect.Width - iconSize - iconPad), _titleRect.Height);
            }

            if (hasTrailing)
            {
                _trailIconRect = new Rectangle(Math.Max(_titleRect.Left, rightLimit - iconSize), _titleRect.Top + (_titleRect.Height - iconSize) / 2, iconSize, iconSize);
                _titleRect = new Rectangle(_titleRect.Left, _titleRect.Top, Math.Max(0, _titleRect.Width - iconSize - iconPad), _titleRect.Height);
            }

            // action pill area at bottom-right
            int actionH = 28;
            int actionW = 80;
            _actionRect = new Rectangle(_cardRect.Right - pad - actionW, _cardRect.Bottom - pad - actionH, actionW, actionH);

            // content area between subtitle and action button
            int contentTop = Math.Max(_subtitleRect.Bottom + 8, _titleRect.Bottom + 8);
            _contentRect = new Rectangle(_cardRect.Left + pad, contentTop, Math.Max(0, _cardRect.Width - pad * 2), Math.Max(0, _actionRect.Top - 8 - contentTop));
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null) return;
            var theme = owner._currentTheme; // internal in same assembly
            bool isSelected = owner.IsSelected;

            // smoothing
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int radius = Math.Max(8, owner.BorderRadius > 0 ? owner.BorderRadius : owner.MaterialBorderRadius);

            // background (card)
            using (var path = ControlPaintHelper.GetRoundedRectPath(_cardRect, radius))
            {
                Color back = owner.BackColor;
                // light elevation tint if selected/hovered
                if (owner.IsHovered)
                {
                    back = Blend(back, theme?.MenuItemHoverBackColor ?? Color.White, 0.12f);
                }
                if (isSelected)
                {
                    back = Blend(back, theme?.MenuItemSelectedBackColor ?? Color.LightGray, 0.18f);
                }

                using var bg = new SolidBrush(back);
                g.FillPath(bg, path);

                // bold outline like the reference
                using var border = new Pen(theme?.BorderColor ?? Color.Black, 2);
                g.DrawPath(border, path);
            }

            // badge
            if (!_badgeRect.IsEmpty)
            {
                using var badgePath = ControlPaintHelper.GetRoundedRectPath(_badgeRect, _badgeRect.Height / 2);
                using var badgeBg = new SolidBrush(theme?.MenuMainItemSelectedBackColor ?? Color.Goldenrod);
                using var badgePen = new Pen(theme?.BorderColor ?? Color.Black, 1);
                g.FillPath(badgeBg, badgePath);
                g.DrawPath(badgePen, badgePath);
                var badgeColor = theme?.MenuMainItemSelectedForeColor ?? Color.White;
                TextRenderer.DrawText(g, owner.BadgeText, owner.Font, _badgeRect, badgeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            // icons in title row
            if (!_leadIconRect.IsEmpty)
            {
                var img = new BeepImage { IsChild = true, BackColor = owner.BackColor, ForeColor = owner.ForeColor, ApplyThemeOnImage = true, PreserveSvgBackgrounds = true, Size = _leadIconRect.Size, ImagePath = string.IsNullOrEmpty(owner.LeadingIconPath) ? owner.LeadingImagePath : owner.LeadingIconPath };
                img.DrawImage(g, _leadIconRect);
            }
            if (!_trailIconRect.IsEmpty)
            {
                var img = new BeepImage { IsChild = true, BackColor = owner.BackColor, ForeColor = owner.ForeColor, ApplyThemeOnImage = true, PreserveSvgBackgrounds = true, Size = _trailIconRect.Size, ImagePath = string.IsNullOrEmpty(owner.TrailingIconPath) ? owner.TrailingImagePath : owner.TrailingIconPath };
                img.DrawImage(g, _trailIconRect);
            }

            // subtitle
            if (!string.IsNullOrWhiteSpace(owner.HelperText))
            {
                using var subFont = new Font(owner.Font.FontFamily, Math.Max(8f, owner.Font.Size - 1f), FontStyle.Regular);
                var subColor = theme?.SecondaryTextColor ?? owner.ForeColor;
                TextRenderer.DrawText(g, owner.HelperText, subFont, _subtitleRect, subColor, TextFormatFlags.Left | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
            }

            // action pill
            using (var actionPath = ControlPaintHelper.GetRoundedRectPath(_actionRect, _actionRect.Height / 2))
            {
                using var fill = new SolidBrush(theme?.MenuItemSelectedBackColor ?? Color.Black);
                using var pen = new Pen(theme?.BorderColor ?? Color.Black, 1);
                g.FillPath(fill, actionPath);
                g.DrawPath(pen, actionPath);
                string actionText = "Select";
                var actionFore = theme?.MenuItemSelectedForeColor ?? Color.White;
                TextRenderer.DrawText(g, actionText, owner.Font, _actionRect, actionFore, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            if (owner == null) return;
            if (!_cardRect.IsEmpty) register?.Invoke("CardBody", _contentRect, null);
            if (!_actionRect.IsEmpty) register?.Invoke("CardPrimaryButton", _actionRect, null);
            if (!_badgeRect.IsEmpty) register?.Invoke("CardBadge", _badgeRect, null);
            if (!_leadIconRect.IsEmpty && owner.LeadingIconClickable) register?.Invoke("CardLeadingIcon", _leadIconRect, owner.TriggerLeadingIconClick);
            if (!_trailIconRect.IsEmpty && owner.TrailingIconClickable) register?.Invoke("CardTrailingIcon", _trailIconRect, owner.TriggerTrailingIconClick);
        }

        private static Color Blend(Color baseColor, Color overlay, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            byte r = (byte)(baseColor.R + (overlay.R - baseColor.R) * amount);
            byte g = (byte)(baseColor.G + (overlay.G - baseColor.G) * amount);
            byte b = (byte)(baseColor.B + (overlay.B - baseColor.B) * amount);
            return Color.FromArgb(r, g, b);
        }
    }
}
