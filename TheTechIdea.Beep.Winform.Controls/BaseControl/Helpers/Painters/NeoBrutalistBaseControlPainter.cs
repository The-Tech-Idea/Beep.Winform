using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Neo-brutalism painter: bold thick borders, hard shadows, vivid flat fills, simple blocks.
    /// Uses BaseControl.Text as title, HelperText as subtitle, BadgeText as a sticker.
    /// Supports optional Leading/Trailing icons in the title row.
    /// </summary>
    internal sealed class NeoBrutalistBaseControlPainter : IBaseControlPainter
    {
        private Rectangle _cardRect;
        private Rectangle _shadowRect;
        private Rectangle _stickerRect;
        private Rectangle _titleRect;
        private Rectangle _subtitleRect;
        private Rectangle _ctaRect;
        private Rectangle _bodyRect;
        private Rectangle _leadIconRect;
        private Rectangle _trailIconRect;

        public void UpdateLayout(Base.BaseControl owner)
        {
            if (owner == null || owner.Width <= 0 || owner.Height <= 0)
            {
                _cardRect = _shadowRect = _stickerRect = _titleRect = _subtitleRect = _ctaRect = _bodyRect = Rectangle.Empty;
                _leadIconRect = _trailIconRect = Rectangle.Empty;
                return;
            }

            int pad = 12;
            int border = 3; // thick brutalist border
            int shadowOffset = 6;

            _cardRect = new Rectangle(0 + border, 0 + border, Math.Max(0, owner.Width - border * 2 - shadowOffset), Math.Max(0, owner.Height - border * 2 - shadowOffset));
            _shadowRect = new Rectangle(_cardRect.X + shadowOffset, _cardRect.Y + shadowOffset, _cardRect.Width, _cardRect.Height);

            // sticker (badge) top-left, rectangular with thick border
            if (!string.IsNullOrEmpty(owner.BadgeText))
            {
                using var g = owner.CreateGraphics();
                var sz = TextRenderer.MeasureText(g, owner.BadgeText, owner.Font);
                int w = Math.Min(Math.Max(sz.Width + 14, 48), Math.Max(48, owner.Width / 2));
                int h = Math.Max(22, owner.Font.Height + 6);
                _stickerRect = new Rectangle(_cardRect.Left + pad, _cardRect.Top + pad, w, h);
            }
            else
            {
                _stickerRect = Rectangle.Empty;
            }

            int top = _cardRect.Top + pad + (_stickerRect.IsEmpty ? 0 : _stickerRect.Height + 8);
            _titleRect = new Rectangle(_cardRect.Left + pad, top, Math.Max(0, _cardRect.Width - pad * 2), owner.Font.Height + 8);
            _subtitleRect = new Rectangle(_cardRect.Left + pad, _titleRect.Bottom + 4, Math.Max(0, _cardRect.Width - pad * 2), owner.Font.Height + 6);

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
                _trailIconRect = new Rectangle(Math.Max(_titleRect.Left, _cardRect.Right - pad - iconSize), _titleRect.Top + (_titleRect.Height - iconSize) / 2, iconSize, iconSize);
                _titleRect = new Rectangle(_titleRect.Left, _titleRect.Top, Math.Max(0, _titleRect.Width - iconSize - iconPad), _titleRect.Height);
            }

            int ctaH = 32; int ctaW = 96;
            _ctaRect = new Rectangle(_cardRect.Right - pad - ctaW, _cardRect.Bottom - pad - ctaH, ctaW, ctaH);

            int bodyTop = _subtitleRect.Bottom + 8;
            _bodyRect = new Rectangle(_cardRect.Left + pad, bodyTop, Math.Max(0, _cardRect.Width - pad * 2), Math.Max(0, _ctaRect.Top - 8 - bodyTop));
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null) return;
            var theme = owner._currentTheme; // internal access

            g.SmoothingMode = SmoothingMode.None; // crisp brutalist look

            // derive colors
            Color accent = theme?.MenuMainItemSelectedBackColor ?? Color.Gold;
            Color surface = theme?.BackColor ?? owner.BackColor;
            if (surface == Color.Empty || surface.A == 0) surface = Color.White;
            Color cardFill = accent; // bright card color
            Color borderColor = Color.Black;
            Color shadowColor = Color.Black;
            Color titleColor = Color.Black;
            Color subtitleColor = theme?.SecondaryTextColor ?? Color.Black;

            // hard offset shadow rectangle
            using (var sb = new SolidBrush(shadowColor))
            {
                g.FillRectangle(sb, _shadowRect);
            }

            // card block
            using (var b = new SolidBrush(cardFill))
            using (var pen = new Pen(borderColor, 3))
            {
                g.FillRectangle(b, _cardRect);
                g.DrawRectangle(pen, _cardRect);
            }

            // sticker (badge) — white fill with thick black outline
            if (!_stickerRect.IsEmpty)
            {
                using var sbFill = new SolidBrush(Color.White);
                using var sbPen = new Pen(Color.Black, 2);
                g.FillRectangle(sbFill, _stickerRect);
                g.DrawRectangle(sbPen, _stickerRect);
                TextRenderer.DrawText(g, owner.BadgeText, owner.Font, _stickerRect, Color.Black,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }

            // icons
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

            // subtitle (one line, black/secondary)
            if (!string.IsNullOrWhiteSpace(owner.HelperText))
            {
                using var subFont = new Font(owner.Font.FontFamily, owner.Font.Size, FontStyle.Regular);
                TextRenderer.DrawText(g, owner.HelperText, subFont, _subtitleRect, subtitleColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }

            // CTA button: black fill, white text, thick outline, slight inner offset to look like a sticker
            using (var ctaFill = new SolidBrush(Color.Black))
            using (var ctaPen = new Pen(Color.Black, 2))
            {
                g.FillRectangle(ctaFill, _ctaRect);
                g.DrawRectangle(ctaPen, _ctaRect);
                TextRenderer.DrawText(g, "Select", owner.Font, _ctaRect, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }

            // optional dotted grid accent inside body area (very subtle)
            using (var gridPen = new Pen(Color.FromArgb(40, Color.Black), 1))
            {
                int step = 8;
                for (int x = _bodyRect.Left; x < _bodyRect.Right; x += step)
                {
                    g.DrawLine(gridPen, x, _bodyRect.Top, x, _bodyRect.Top + 2);
                }
            }
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            if (owner == null) return;
            if (!_bodyRect.IsEmpty) register?.Invoke("NeoBody", _bodyRect, null);
            if (!_ctaRect.IsEmpty) register?.Invoke("NeoPrimary", _ctaRect, null);
            if (!_stickerRect.IsEmpty) register?.Invoke("NeoSticker", _stickerRect, null);
            if (!_leadIconRect.IsEmpty && owner.LeadingIconClickable) register?.Invoke("NeoLeadingIcon", _leadIconRect, owner.TriggerLeadingIconClick);
            if (!_trailIconRect.IsEmpty && owner.TrailingIconClickable) register?.Invoke("NeoTrailingIcon", _trailIconRect, owner.TriggerTrailingIconClick);
        }
    }
}
