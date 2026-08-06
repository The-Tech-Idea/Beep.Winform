using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMetricTile
    {
        private CardScaffold _scaffold;
        private BeepLabel _titleLabel;
        private BeepLabel _valueLabel;
        private BeepLabel _deltaLabel;
        private BeepImage _icon;
        private Image _fadedSilhouette;

        private bool _composing;

        /// <summary>
        /// Builds the tile from controls. Replaces the <c>DrawContent</c> this tile used to run.
        /// </summary>
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

                _icon = NewIcon(_iconImagepath, 20);
                if (_icon != null) _scaffold.SetIcon(_icon);

                _titleLabel = NewLabel(_titleText, 1.0f, FontStyle.Regular);
                _scaffold.AddContent(_titleLabel);

                // The metric. Largest type on the tile - the number is what a tile is read for.
                _valueLabel = NewLabel(_metricValue, 2.2f, FontStyle.Bold);
                _valueLabel.Height = 44;
                _scaffold.AddContent(_valueLabel);

                _deltaLabel = NewLabel(_deltaValue, 0.9f, FontStyle.Regular);
                _scaffold.AddContent(_deltaLabel);

                ApplySilhouette();

                _scaffold.ResumeLayout(true);
            }
            finally
            {
                _composing = false;
            }
        }

        /// <summary>
        /// Puts the silhouette behind the tile's text as the scaffold's background image.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the one part of a card that composition does not hand you: a
        /// <see cref="TableLayoutPanel"/> puts controls in cells, it does not layer them. A
        /// <c>BeepImage</c> in a spanning cell would sit <i>beside</i> the text in z-order, and the
        /// labels — which are opaque, because <c>IsChild</c> gives a control its parent's colour
        /// rather than transparency over what the parent painted — would hide it.
        /// </para>
        /// <para>
        /// The panel's own <c>BackgroundImage</c> is the layer that works: it is behind every child,
        /// and the labels are marked transparent so the glyph reads through them rather than being
        /// punched out in label-shaped rectangles.
        /// </para>
        /// <para>
        /// The fade is baked into a bitmap once per image rather than drawn per paint. That is asset
        /// preparation, not rendering: the tile still draws nothing.
        /// </para>
        /// </remarks>
        private void ApplySilhouette()
        {
            _fadedSilhouette?.Dispose();
            _fadedSilhouette = null;

            _scaffold.BackgroundImage = null;

            if (_backgroundSilhouette == null) return;

            _fadedSilhouette = Fade(_backgroundSilhouette, 0.20f);
            _scaffold.BackgroundImage = _fadedSilhouette;
            _scaffold.BackgroundImageLayout = ImageLayout.Center;
        }

        /// <summary>Returns a copy of <paramref name="source"/> at <paramref name="alpha"/> opacity.</summary>
        private static Image Fade(Image source, float alpha)
        {
            var faded = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);

            var matrix = new ColorMatrix { Matrix33 = alpha };
            using var attributes = new ImageAttributes();
            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            using var g = Graphics.FromImage(faded);
            g.DrawImage(source,
                new Rectangle(0, 0, source.Width, source.Height),
                0, 0, source.Width, source.Height,
                GraphicsUnit.Pixel, attributes);

            return faded;
        }

        private BeepLabel NewLabel(string text, float scale, FontStyle style) => new()
        {
            Text = text ?? string.Empty,
            AutoSize = false,
            Dock = DockStyle.Fill,
            Height = 22,
            IsChild = true,
            IsFrameless = true,
            IsTransparentBackground = true,   // so the silhouette reads through the text rows
            WordWrap = true,
            Multiline = true,
            TextAlign = ContentAlignment.MiddleLeft,
            TextFont = new Font(Font.FontFamily, Font.Size * scale, style),
            AccessibleName = text ?? string.Empty,
        };

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

        private void Recompose()
        {
            if (_composing || IsDisposed) return;
            Compose();
        }
    }
}
