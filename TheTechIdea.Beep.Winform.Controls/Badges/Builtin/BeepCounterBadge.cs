namespace TheTechIdea.Beep.Winform.Controls.Badges.Builtin
{
    public class BeepCounterBadge : BeepFloatingBadge
    {
        private string _displayText = string.Empty;
        private int _maxDisplay = 99;
        private bool _showPlus = true;
        private Font? _cachedFont;
        private string? _cachedFontText;
        private float _cachedFontSize;
        private StringFormat? _cachedTextFormat;

        public BeepCounterBadge() : this("0") { }

        public BeepCounterBadge(string text)
        {
            _displayText = text;
            Shape = BadgeShape.Circle;
            BadgeBackColor = Color.Red;
            BadgeForeColor = Color.White;
            Anchor = BadgeAnchor.TopRight;
        }

        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value ?? string.Empty;
                _cachedFontText = null;
                Invalidate();
            }
        }

        public int MaxDisplay
        {
            get => _maxDisplay;
            set
            {
                _maxDisplay = Math.Max(1, value);
                _cachedFontText = null;
                Invalidate();
            }
        }

        public bool ShowPlus
        {
            get => _showPlus;
            set
            {
                _showPlus = value;
                _cachedFontText = null;
                Invalidate();
            }
        }

        public BeepCounterBadge SetText(string text) { DisplayText = text; return this; }
        public BeepCounterBadge At(float fractionX, float fractionY) { Location = BadgeLocations.Relative(fractionX, fractionY); return this; }
        public BeepCounterBadge With(BadgeSide side, BadgeAlignment alignment) { Location = BadgeLocations.Css(side, alignment); return this; }

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

            string label = _displayText;
            if (int.TryParse(_displayText, out int count) && count > _maxDisplay)
            {
                label = _maxDisplay.ToString();
                if (_showPlus) label += "+";
            }

            float fontSize;
            if (label.Length == 1)
                fontSize = Math.Max(7, contentBounds.Height * 0.55f);
            else if (label.Length == 2)
                fontSize = Math.Max(6, contentBounds.Height * 0.45f);
            else
                fontSize = Math.Max(5, contentBounds.Height * 0.35f);

            if (_cachedFont is null || _cachedFontText != label || Math.Abs(_cachedFontSize - fontSize) > 0.1f)
            {
                _cachedFont?.Dispose();
                _cachedFont = new Font("Segoe UI", fontSize, FontStyle.Bold);
                _cachedFontText = label;
                _cachedFontSize = fontSize;
            }

            using var textBrush = new SolidBrush(BadgeForeColor);
            g.DrawString(label, _cachedFont, textBrush, contentBounds, GetOrCreateTextFormat());
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
