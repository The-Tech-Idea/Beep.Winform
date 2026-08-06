using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTestimonial
    {
        private CardScaffold _scaffold;

        private BeepImage _avatarImage;
        private BeepImage _companyLogoImage;
        private BeepLabel _quoteLabel;
        private BeepLabel _nameLabel;
        private BeepLabel _detailsLabel;
        private BeepButton _closeButton;

        private bool _composing;

        /// <summary>
        /// Builds the card from controls. Replaces the <c>DrawContent</c> this card used to run.
        /// </summary>
        /// <remarks>
        /// Each <see cref="TestimonialViewType"/> shows a different subset of the same parts, which is
        /// what the four painted views amounted to. Composed, that is a different call order over one
        /// set of controls rather than four paint routines measuring and centring text by hand.
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
                DetachStarRating();
                _scaffold.Clear();

                switch (_viewType)
                {
                    case TestimonialViewType.Minimal: ComposeMinimal(); break;
                    case TestimonialViewType.Compact: ComposeCompact(); break;
                    case TestimonialViewType.SocialCard: ComposeSocialCard(); break;
                    default: ComposeClassic(); break;
                }

                _scaffold.ResumeLayout(true);
            }
            finally
            {
                _composing = false;
            }
        }

        /// <summary>Avatar, quote, attribution and the star rating.</summary>
        private void ComposeClassic()
        {
            AddAvatar(48);
            AddQuote();
            AddAttribution();
            AddStarRating();
        }

        /// <summary>The quote and attribution, plus the company logo instead of a rating.</summary>
        private void ComposeMinimal()
        {
            AddAvatar(40);
            AddQuote();
            AddAttribution();

            _companyLogoImage = NewIcon(_companyLogoPath, 24);
            if (_companyLogoImage != null)
            {
                _scaffold.AddContent(_companyLogoImage);
                _companyLogoImage.Dock = DockStyle.None;
                _companyLogoImage.Anchor = AnchorStyles.Left;
            }
        }

        /// <summary>Attribution first, then the quote — the tightest of the four.</summary>
        private void ComposeCompact()
        {
            AddAvatar(32);
            AddAttribution();
            AddQuote();
        }

        /// <summary>Attribution at the top with a dismiss button, then the quote.</summary>
        private void ComposeSocialCard()
        {
            AddAvatar(40);
            AddAttribution();
            AddQuote();

            _closeButton = new BeepButton
            {
                Text = "✕",
                Size = new Size(22, 22),
                AutoSize = false,
                IsChild = true,
                IsFrameless = true,
                AccessibleName = "Dismiss",
                Cursor = Cursors.Hand,
            };
            _closeButton.Click += (s, e) => OnCloseClick();

            _scaffold.AddActions(_closeButton);
        }

        private void AddAvatar(int size)
        {
            _avatarImage = NewIcon(_imagePath, size);
            if (_avatarImage == null) return;

            _avatarImage.Cursor = Cursors.Hand;
            _avatarImage.AccessibleName = _name ?? "Avatar";
            _avatarImage.Click += (s, e) => OnImageClick();
            _scaffold.SetIcon(_avatarImage);
        }

        private void AddQuote()
        {
            _quoteLabel = NewLabel(_testimonial, 1.0f, FontStyle.Italic);
            _quoteLabel.Cursor = Cursors.Hand;
            _quoteLabel.Click += (s, e) => OnTestimonialClick();
            _scaffold.AddContent(_quoteLabel, fill: true);
        }

        /// <summary>Name on one line, then position and handle on the next.</summary>
        private void AddAttribution()
        {
            _nameLabel = NewLabel(_name, 1.0f, FontStyle.Bold);
            _scaffold.AddContent(_nameLabel);

            string details = string.Join(" · ", new[] { _position, _username }
                .Where(part => !string.IsNullOrWhiteSpace(part)));

            if (details.Length == 0) return;

            _detailsLabel = NewLabel(details, 0.9f, FontStyle.Regular);
            _scaffold.AddContent(_detailsLabel);
        }

        private void AddStarRating()
        {
            if (starRating == null) return;

            if (int.TryParse(_rating, out int stars))
                starRating.SelectedRating = Math.Max(0, Math.Min(starRating.StarCount, stars));

            starRating.AccessibleName = $"{starRating.SelectedRating} out of {starRating.StarCount} stars";
            _scaffold.AddContent(starRating);
            starRating.Dock = DockStyle.None;
            starRating.Anchor = AnchorStyles.Left;
        }

        /// <summary>
        /// Takes the star rating out of the scaffold before a clear, so it survives recomposition.
        /// </summary>
        /// <remarks>
        /// <see cref="CardScaffold.Clear"/> disposes what it removes — right for controls it built,
        /// wrong for this one, which the card owns and reuses across view changes.
        /// </remarks>
        private void DetachStarRating()
        {
            if (starRating != null && starRating.Parent == _scaffold)
                _scaffold.Controls.Remove(starRating);
        }

        private BeepLabel NewLabel(string text, float scale, FontStyle style) => new()
        {
            Text = text ?? string.Empty,
            AutoSize = false,
            Dock = DockStyle.Fill,
            Height = 22,
            IsChild = true,
            IsFrameless = true,
            WordWrap = true,
            Multiline = true,
            TextAlign = ContentAlignment.MiddleLeft,
            TextFont = new Font(Font.FontFamily, Font.Size * scale, style),
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

        private void Recompose()
        {
            if (_composing || IsDisposed) return;
            Compose();
        }
    }
}
