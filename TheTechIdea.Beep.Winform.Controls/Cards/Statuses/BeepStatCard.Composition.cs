using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.StatusCards
{
    public partial class BeepStatCard
    {
        private CardScaffold _scaffold;

        private BeepLabel _headerLabel;
        private BeepLabel _valueLabel;
        private BeepLabel _percentageLabel;
        private BeepLabel _trendLabel;
        private BeepLabel _infoLabel;
        private BeepImage _cardIcon;
        private BeepImage _trendIcon;

        private bool _composing;

        /// <summary>
        /// Builds the card from controls. Replaces the painter this card used to select.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Every part is a Beep control, so the theme, DPI scaling and accessible naming that the
        /// painters did by hand now happen because the controls do them. Nothing here assigns a
        /// colour or scales a value.
        /// </para>
        /// <para>
        /// The trend row is the reason this card gains behaviour rather than only losing code:
        /// <c>TrendText</c>, <c>TrendUpSvgPath</c> and <c>TrendDownSvgPath</c> had <b>no readers
        /// anywhere in the solution</b> — a stat card whose entire point is a number and its direction
        /// of travel could not show the direction. They have a row to occupy now.
        /// </para>
        /// </remarks>
        private void Compose()
        {
            if (_composing) return;
            _composing = true;

            try
            {
                if (_scaffold == null)
                {
                    _scaffold = new CardScaffold();
                    Controls.Add(_scaffold);
                }

                _scaffold.SuspendLayout();
                _scaffold.Clear();

                _cardIcon = NewIcon(cardiconSvgPath, 24);
                if (_cardIcon != null) _scaffold.SetIcon(_cardIcon);

                _headerLabel = NewLabel(headerText);
                _scaffold.AddContent(_headerLabel);

                // The KPI. Largest type on the card - a stat card whose number does not dominate has
                // lost its point.
                _valueLabel = NewLabel(valueText);
                _valueLabel.TextFont = new Font(Font.FontFamily, Font.Size * 1.8f, FontStyle.Bold);
                _scaffold.AddContent(_valueLabel);

                if (!string.IsNullOrWhiteSpace(percentageText))
                {
                    _percentageLabel = NewLabel(percentageText);
                    _scaffold.AddContent(_percentageLabel);
                }

                ComposeTrendRow();

                if (!string.IsNullOrWhiteSpace(infoText))
                {
                    _infoLabel = NewLabel(infoText);
                    _scaffold.AddContent(_infoLabel);
                }

                _scaffold.ResumeLayout(true);
            }
            finally
            {
                _composing = false;
            }
        }

        /// <summary>The direction of travel: a glyph chosen by <see cref="IsTrendingUp"/>, and its text.</summary>
        private void ComposeTrendRow()
        {
            string glyph = isTrendingUp ? trendUpSvgPath : trendDownSvgPath;
            bool hasText = !string.IsNullOrWhiteSpace(trendText);
            bool hasGlyph = !string.IsNullOrWhiteSpace(glyph);

            if (!hasText && !hasGlyph) return;

            var row = new TableLayoutPanel
            {
                ColumnCount = hasGlyph ? 2 : 1,
                RowCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0),
            };
            row.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            int column = 0;
            if (hasGlyph)
            {
                row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                _trendIcon = NewIcon(glyph, 14);
                _trendIcon.Margin = new Padding(0, 0, 4, 0);
                _trendIcon.Anchor = AnchorStyles.None;
                row.Controls.Add(_trendIcon, column++, 0);
            }

            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            _trendLabel = NewLabel(hasText ? trendText : string.Empty);
            row.Controls.Add(_trendLabel, column, 0);

            _scaffold.AddContent(row);
        }

        /// <summary>A label that reads as part of the card rather than a box on it.</summary>
        private BeepLabel NewLabel(string text) => new()
        {
            Text = text ?? string.Empty,
            AutoSize = false,
            Dock = DockStyle.Fill,
            Height = 20,
            IsChild = true,          // takes the card's surface; the framework's idiom for chrome
            IsFrameless = true,
            WordWrap = true,
            Multiline = true,        // WordWrap decides where lines break; Multiline renders them
            TextAlign = ContentAlignment.MiddleLeft,
            AccessibleName = text ?? string.Empty,
        };

        /// <summary>An icon, or null when there is no path — an empty BeepImage still occupies a cell.</summary>
        private BeepImage NewIcon(string path, int size)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            return new BeepImage
            {
                ImagePath = path,
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                Size = new Size(size, size),
                IsChild = true,
                Margin = new Padding(0),
            };
        }

        /// <summary>Recomposes after a property that changes the card's content or arrangement.</summary>
        private void Recompose()
        {
            if (_composing || IsDisposed) return;
            Compose();
        }
    }
}
