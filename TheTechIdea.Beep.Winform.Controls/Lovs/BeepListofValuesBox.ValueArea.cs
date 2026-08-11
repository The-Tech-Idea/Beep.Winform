using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepListofValuesBox
    {
        /// <summary>Widest key the key column sizes itself to, in characters.</summary>
        /// <remarks>
        /// The key column used to be 22 % of the field width, computed independently in
        /// <c>AdjustLayout</c> and again in the paint pass — they agreed only because the two
        /// expressions happened to be identical.
        ///
        /// A percentage was the wrong rule regardless: a key is a short code and does not get longer
        /// because the field got wider, so 22 % of a 460px field reserved 101px for "40" and left a gap
        /// the width of the key between the key and the value it belongs to. One constant now, measured
        /// at the current font, used by both the key box and the value area.
        /// </remarks>
        private const int KeyColumnChars = 6;

        /// <summary>Smallest the key column is allowed to get.</summary>
        private const int KeyColumnMinPx = 44;

        /// <summary>Gap between the key box and the value text.</summary>
        private const int KeyValueGapPx = 8;

        /// <summary>
        /// Why the value area is painted rather than composed from controls.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This control is a deliberate exception to composing from Beep controls, for a reason that is
        /// specific to it: <c>BaseControl</c> paints this field's frame, and it paints
        /// <c>LabelText</c>, <c>HelperText</c> and <c>ErrorText</c> around it. A child control laid over
        /// the content area is drawn <b>on top of all of that</b>.
        /// </para>
        /// <para>
        /// That was not theoretical. With the value as a <c>BeepLabel</c> the label band was covered,
        /// and the key badge rendered as a bare block of accent colour with no text once it stopped
        /// resolving its own theme colours.
        /// </para>
        /// <para>
        /// The key box stays a real <c>BeepTextBox</c> because it must be typed into and focused. The
        /// display value is read-only text, so painting it gives up nothing a control was providing —
        /// and it keeps this control a single control, which matters where LOVs sit in grid cells.
        /// </para>
        /// </remarks>
        private int KeyColumnWidth()
        {
            var font = _fieldFont ?? Font;
            int measured = TextRenderer.MeasureText(new string('0', KeyColumnChars), font).Width;
            return Math.Max(KeyColumnMinPx, measured + 8);
        }

        /// <summary>The area BaseControl leaves for this control's own content.</summary>
        private Rectangle ContentArea()
        {
            Rectangle content = GetAdjustedContentRect();
            if (content.IsEmpty || content.Width <= 0 || content.Height <= 0)
                content = DrawingRect;
            return content;
        }

        /// <summary>Puts the key box at the left of the content area.</summary>
        private void PositionKeyBox()
        {
            if (_keyTextBox == null) return;

            Rectangle content = ContentArea();
            if (content.Width <= 0 || content.Height <= 0) return;

            int keyW = Math.Min(KeyColumnWidth(), Math.Max(20, content.Width - 40));
            int h = Math.Min(Math.Max(1, _keyTextBox.PreferredHeight), content.Height);
            int y = content.Top + (content.Height - h) / 2;

            _keyTextBox.SetBounds(content.Left, y, keyW, h);
        }

        /// <summary>Paints the display value, and the key badge when asked for.</summary>
        private void PaintValueArea(Graphics g)
        {
            Rectangle content = ContentArea();
            if (content.Width <= 0 || content.Height <= 0) return;

            int keyW = Math.Min(KeyColumnWidth(), Math.Max(20, content.Width - 40));
            int x = content.Left + keyW + KeyValueGapPx;
            int w = content.Right - x;
            if (w <= 0) return;

            var area = new Rectangle(x, content.Top, w, content.Height);

            string key = SelectedKey;
            bool hasKey = !string.IsNullOrEmpty(key);
            bool hasValue = !string.IsNullOrEmpty(_selectedDisplayValue);

            if (!hasKey && !hasValue)
            {
                TextRenderer.DrawText(g, "Select a value…", _fieldFont ?? Font, area,
                    T().SecondaryTextColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                return;
            }

            if (_showKeyBadge && hasKey)
            {
                int used = PaintKeyBadge(g, area, key);
                if (used > 0) x += used + 6;
            }

            if (!hasValue || x >= area.Right) return;

            var textArea = new Rectangle(x, area.Top, area.Right - x, area.Height);
            TextRenderer.DrawText(g, _selectedDisplayValue, _fieldFont ?? Font, textArea,
                T().ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        /// <summary>Paints the key pill and returns the width it used, or 0 if it did not fit.</summary>
        private int PaintKeyBadge(Graphics g, Rectangle area, string key)
        {
            var font = _badgeFont ?? _fieldFont ?? Font;
            Size text = TextRenderer.MeasureText(key, font);

            int badgeW = text.Width + DpiScalingHelper.ScaleValue(12, this);
            int badgeH = Math.Min(text.Height + DpiScalingHelper.ScaleValue(4, this), area.Height - 2);
            if (badgeH <= 0 || badgeW > area.Width) return 0;

            var rect = new Rectangle(area.Left, area.Top + (area.Height - badgeH) / 2, badgeW, badgeH);
            var theme  = T();
            Color fill = theme.AccentColor;

            var saved = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = RoundedRect(rect, Math.Min(DpiScalingHelper.ScaleValue(6, this), badgeH / 2)))
            using (var brush = new SolidBrush(fill))
                g.FillPath(brush, path);
            g.SmoothingMode = saved;

            TextRenderer.DrawText(g, key, font, rect, BadgeInk(theme, fill),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            return badgeW;
        }

        private static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            int r = Math.Max(1, radius);
            int d = r * 2;
            var path = new GraphicsPath();
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>The theme in force: the pushed one, else the manager's current.</summary>
        /// <remarks>
        /// There is always a theme. The `_currentTheme?.Slot ?? SystemColors.X` fallbacks this
        /// replaced meant a null theme silently produced Windows' own palette in the middle of a
        /// themed control - a mismatch that reads as a rendering bug rather than as a missing theme.
        /// </remarks>
        private IBeepTheme T() => _currentTheme ?? BeepThemesManager.CurrentTheme;

        /// <summary>
        /// Whichever theme slot reads better on the badge fill.
        /// </summary>
        /// <remarks>
        /// Both candidates are theme slots, so this is a contrast decision rather than a palette.
        /// It used to return literal <c>(30,30,30)</c> or <c>White</c>, which ignored the theme
        /// outright: on a dark theme the key sat in near-black on the accent colour.
        /// <para>
        /// The candidates are <c>ForeColor</c> and <c>PanelBackColor</c> because those are the two
        /// ends of the theme's own contrast range - a theme's text has to be readable on its own
        /// panel, so one of the pair is always far from any fill. Deliberately NOT a <c>*Fore</c>
        /// slot (the theme contrast validator owns and rewrites those), and deliberately not
        /// <c>OnPrimaryColor</c>: that slot means "ink on <c>PrimaryColor</c>", and the badge is
        /// filled with <c>AccentColor</c>. Using it here scored white-on-cyan at ratio 1.25 in
        /// DarkTheme and made it look like the theme was wrong, when the theme was self-consistent
        /// and the candidate was simply the wrong slot for this fill.
        /// </para>
        /// </remarks>
        private static Color BadgeInk(IBeepTheme theme, Color fill)
        {
            Color preferred = theme.ForeColor;
            Color alternate = theme.PanelBackColor;
            return ContrastRatio(fill, preferred) >= ContrastRatio(fill, alternate)
                ? preferred : alternate;
        }

        private static double ContrastRatio(Color a, Color b)
        {
            double la = RelativeLuminance(a), lb = RelativeLuminance(b);
            return (Math.Max(la, lb) + 0.05) / (Math.Min(la, lb) + 0.05);
        }

        private static double RelativeLuminance(Color c)
        {
            static double Channel(int v)
            {
                double s = v / 255.0;
                return s <= 0.03928 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
            }
            return 0.2126 * Channel(c.R) + 0.7152 * Channel(c.G) + 0.0722 * Channel(c.B);
        }
    }
}
