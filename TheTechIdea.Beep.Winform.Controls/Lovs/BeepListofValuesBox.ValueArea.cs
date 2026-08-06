using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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
                    _currentTheme?.SecondaryTextColor ?? SystemColors.GrayText,
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
                _currentTheme?.ForeColor ?? ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        /// <summary>Paints the key pill and returns the width it used, or 0 if it did not fit.</summary>
        private int PaintKeyBadge(Graphics g, Rectangle area, string key)
        {
            var font = _badgeFont ?? _fieldFont ?? Font;
            Size text = TextRenderer.MeasureText(key, font);

            int badgeW = text.Width + 12;
            int badgeH = Math.Min(text.Height + 4, area.Height - 2);
            if (badgeH <= 0 || badgeW > area.Width) return 0;

            var rect = new Rectangle(area.Left, area.Top + (area.Height - badgeH) / 2, badgeW, badgeH);
            Color fill = _currentTheme?.AccentColor ?? SystemColors.Highlight;

            var saved = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = RoundedRect(rect, Math.Min(6, badgeH / 2)))
            using (var brush = new SolidBrush(fill))
                g.FillPath(brush, path);
            g.SmoothingMode = saved;

            TextRenderer.DrawText(g, key, font, rect, ContrastForeColor(fill),
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

        /// <summary>Black or white, whichever reads better on <paramref name="bg"/>.</summary>
        private static Color ContrastForeColor(Color bg)
        {
            // Perceived luminance (ITU-R BT.601)
            double lum = (0.299 * bg.R + 0.587 * bg.G + 0.114 * bg.B) / 255.0;
            return lum > 0.55 ? Color.FromArgb(30, 30, 30) : Color.White;
        }
    }
}
