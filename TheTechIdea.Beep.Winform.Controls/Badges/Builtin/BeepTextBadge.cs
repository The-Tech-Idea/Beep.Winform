namespace TheTechIdea.Beep.Winform.Controls.Badges.Builtin
{
    public class BeepTextBadge : BeepFloatingBadge, IBeepTextBadge
    {
        private string _displayText = string.Empty;
        private Font? _cachedFont;
        private string? _cachedFontText;
        private int _cachedFontHeight;
        private StringFormat? _cachedTextFormat;

        public BeepTextBadge() : this("") { }

        public BeepTextBadge(string text)
        {
            _displayText = text;
            Shape = BadgeShape.Pill;
            Role = BadgeRole.Accent;
            BadgeDiameter = 18;
            Anchor = BadgeAnchor.TopRight;
        }

        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value ?? string.Empty;
                _cachedFontText = null;
                ApplyBadgeSize();
            }
        }

        public BeepTextBadge SetText(string text) { DisplayText = text; return this; }
        public BeepTextBadge At(float fractionX, float fractionY) { Location = BadgeLocations.Relative(fractionX, fractionY); return this; }

        /// <summary>The width this badge's text needs, so a pill can be as wide as its word.</summary>
        protected override int MeasureContentWidth()
        {
            if (string.IsNullOrEmpty(_displayText)) return 0;
            using var font = BadgeFontFor(Math.Max(6, BadgeDiameter * 0.5f));
            return TextRenderer.MeasureText(_displayText, font).Width;
        }

        private StringFormat GetOrCreateTextFormat()
        {
            if (_cachedTextFormat is null)
                _cachedTextFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            return _cachedTextFormat;
        }

        protected override void DrawBadgeContent(Graphics g, Rectangle contentBounds)
        {
            if (string.IsNullOrEmpty(_displayText))
                return;

            float fontSize = Math.Max(6, contentBounds.Height * 0.5f);
            int ch = contentBounds.Height;

            if (_cachedFont is null || _cachedFontText != _displayText || _cachedFontHeight != ch)
            {
                _cachedFont?.Dispose();
                _cachedFont = BadgeFontFor(fontSize);
                _cachedFontText = _displayText;
                _cachedFontHeight = ch;
            }

            using var textBrush = new SolidBrush(BadgeForeColor);
            g.DrawString(_displayText, _cachedFont, textBrush, contentBounds, GetOrCreateTextFormat());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cachedFont?.Dispose();
                _cachedFont = null;
                _cachedTextFormat?.Dispose();
                _cachedTextFormat = null;
            }
            base.Dispose(disposing);
        }
    }
}
