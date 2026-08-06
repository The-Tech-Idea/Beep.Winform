namespace TheTechIdea.Beep.Winform.Controls.Badges.Builtin
{
    public class BeepCounterBadge : BeepFloatingBadge, IBeepTextBadge
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
            Role = BadgeRole.Default;
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

        public int MaxDisplay
        {
            get => _maxDisplay;
            set
            {
                _maxDisplay = Math.Max(1, value);
                _cachedFontText = null;
                ApplyBadgeSize();
            }
        }

        public bool ShowPlus
        {
            get => _showPlus;
            set
            {
                _showPlus = value;
                _cachedFontText = null;
                ApplyBadgeSize();
            }
        }

        public BeepCounterBadge SetText(string text) { DisplayText = text; return this; }
        public BeepCounterBadge At(float fractionX, float fractionY) { Location = BadgeLocations.Relative(fractionX, fractionY); return this; }
        public BeepCounterBadge With(BadgeSide side, BadgeAlignment alignment) { Location = BadgeLocations.Css(side, alignment); return this; }

        /// <summary>The label as drawn, after MaxDisplay clamping.</summary>
        private string EffectiveLabel()
        {
            if (int.TryParse(_displayText, out int count) && count > _maxDisplay)
                return _maxDisplay.ToString() + (_showPlus ? "+" : string.Empty);
            return _displayText;
        }

        protected override int MeasureContentWidth()
        {
            string label = EffectiveLabel();
            if (string.IsNullOrEmpty(label)) return 0;
            using var font = BadgeFontFor(FontSizeFor(label, BadgeDiameter));
            return TextRenderer.MeasureText(label, font).Width;
        }

        /// <summary>
        /// Steps the font down for longer labels.
        /// </summary>
        /// <remarks>
        /// This stepping is why MaxDisplay and ShowPlus exist: the badge could not grow, so "99+" was
        /// rendered at 35% of an 18px badge - about 6pt - rather than in a wider pill. The badge can
        /// grow now, so the stepping is a gentler taper than it was rather than the only defence.
        /// </remarks>
        private static float FontSizeFor(string label, int height) => label.Length switch
        {
            1 => Math.Max(7, height * 0.55f),
            2 => Math.Max(6, height * 0.5f),
            _ => Math.Max(6, height * 0.45f),
        };

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

            string label = EffectiveLabel();

            float fontSize = FontSizeFor(label, contentBounds.Height);

            if (_cachedFont is null || _cachedFontText != label || Math.Abs(_cachedFontSize - fontSize) > 0.1f)
            {
                _cachedFont?.Dispose();
                _cachedFont = BadgeFontFor(fontSize);
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
