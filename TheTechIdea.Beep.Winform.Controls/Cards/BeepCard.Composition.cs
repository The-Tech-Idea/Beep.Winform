using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Ratings;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepCard
    {
        private CardScaffold _scaffold;

        private BeepImage _avatarImage;
        private BeepImage _heroImage;
        private BeepLabel _headerLabel;
        private BeepLabel _subtitleLabel;
        private BeepLabel _paragraphLabel;
        private BeepLabel _emphasisLabel;
        private BeepStarRating _ratingControl;
        private BeepButton _primaryButton;
        private BeepButton _secondaryButton;
        private BeepCheckBoxBool _selectionCheckBox;
        private BeepButton _contextMenuButton;
        private BeepButton _collapseButton;

        private bool _composing;

        /// <summary>
        /// Builds the card from controls, in the order its <see cref="CardStyle"/> calls for.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The 56 styles were 56 painters. They are now one method reading a table: every style is a
        /// selection of the same parts in an order, so a new style is an entry in
        /// <see cref="CardStyleLayouts"/> rather than a class.
        /// </para>
        /// <para>
        /// This is also where the card's `AccessibleName` stops being the literal string <c>"Card"</c>
        /// for all 56 styles. Each label carries the text it displays, so the accessible name is the
        /// content and cannot fall out of step with it — there is no second string to maintain.
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
                ClearComposedReferences();

                ComposeChrome();

                foreach (var part in CardStyleLayouts.For(_style))
                    AddPart(part);

                _scaffold.ResumeLayout(true);
            }
            finally
            {
                _composing = false;
            }
        }

        /// <summary>
        /// The card's own affordances: select, overflow menu, collapse.
        /// </summary>
        /// <remarks>
        /// These three were painted by a <c>DrawAuxiliaryIcons</c> pass and hit-tested against
        /// rectangles. <c>ShowSelectionCheckbox</c>, <c>ContextMenuIcon</c> and <c>IsCollapsible</c> are
        /// public properties, so deleting the paint pass without composing them would have left three
        /// live properties doing nothing — the defect this refactor exists to remove, reintroduced.
        /// </remarks>
        private void ComposeChrome()
        {
            var chrome = new List<Control>();

            if (_showSelectionCheckbox)
            {
                _selectionCheckBox = new BeepCheckBoxBool
                {
                    Checked = _isSelected,
                    Size = new Size(18, 18),
                    IsChild = true,
                    IsFrameless = true,
                    AccessibleName = "Select card",
                };
                _selectionCheckBox.CheckedChanged += (s, e) => IsSelected = _selectionCheckBox.Checked;
                chrome.Add(_selectionCheckBox);
            }

            if (!string.IsNullOrWhiteSpace(_contextMenuIcon))
            {
                _contextMenuButton = NewIconButton(_contextMenuIcon, "More options");
                _contextMenuButton.Click += (s, e) => ContextMenuRequested?.Invoke(this, EventArgs.Empty);
                chrome.Add(_contextMenuButton);
            }

            if (_isCollapsible)
            {
                _collapseButton = new BeepButton
                {
                    Text = _isExpanded ? "⌃" : "⌄",
                    Size = new Size(22, 22),
                    AutoSize = false,
                    IsChild = true,
                    IsFrameless = true,
                    AccessibleName = _isExpanded ? "Collapse card" : "Expand card",
                    Cursor = Cursors.Hand,
                };
                _collapseButton.Click += (s, e) => ToggleExpandedState();
                chrome.Add(_collapseButton);
            }

            if (chrome.Count == 0) return;

            var strip = NewStrip(chrome);
            strip.Anchor = AnchorStyles.Right;
            _scaffold.AddFullWidth(strip);
        }

        private BeepButton NewIconButton(string path, string name) => new()
        {
            ImagePath = path,
            Text = string.Empty,
            HideText = true,
            MaxImageSize = new Size(16, 16),
            Size = new Size(22, 22),
            AutoSize = false,
            IsChild = true,
            IsFrameless = true,
            AccessibleName = name,
            Cursor = Cursors.Hand,
        };

        private void AddPart(CardPart part)
        {
            switch (part)
            {
                case CardPart.Avatar: AddAvatar(); break;
                case CardPart.Hero: AddHero(); break;
                case CardPart.Header: AddHeader(); break;
                case CardPart.Subtitle: AddSubtitle(); break;
                case CardPart.Paragraph: AddParagraph(); break;
                case CardPart.Emphasis: AddEmphasis(); break;
                case CardPart.Badges: AddBadges(); break;
                case CardPart.Tags: AddTags(); break;
                case CardPart.Rating: AddRating(); break;
                case CardPart.Status: AddStatus(); break;
                case CardPart.Actions: AddActions(primary: true, secondary: true); break;
                case CardPart.PrimaryAction: AddActions(primary: true, secondary: false); break;
                case CardPart.SecondaryAction: AddActions(primary: false, secondary: true); break;
            }
        }

        private void AddAvatar()
        {
            _avatarImage = NewIcon(imagePath, 40);
            if (_avatarImage == null) return;

            _avatarImage.Cursor = Cursors.Hand;
            _avatarImage.AccessibleName = headerText ?? "Image";
            _avatarImage.Click += (s, e) => RaiseCardEvent(ImageClicked, nameof(ImagePath), imagePath);
            _scaffold.SetIcon(_avatarImage);
        }

        private void AddHero()
        {
            _heroImage = NewIcon(imagePath, 0);
            if (_heroImage == null) return;

            _heroImage.Cursor = Cursors.Hand;
            _heroImage.AccessibleName = headerText ?? "Image";
            _heroImage.Height = 120;
            _heroImage.Click += (s, e) => RaiseCardEvent(ImageClicked, nameof(ImagePath), imagePath);
            _scaffold.AddFullWidth(_heroImage);
        }

        private void AddHeader()
        {
            if (string.IsNullOrWhiteSpace(headerText)) return;

            _headerLabel = NewLabel(headerText, 1.15f, FontStyle.Bold, headerAlignment);
            _headerLabel.Cursor = Cursors.Hand;
            _headerLabel.Click += (s, e) => RaiseCardEvent(HeaderClicked, nameof(HeaderText), headerText);
            _scaffold.AddContent(_headerLabel);
        }

        private void AddSubtitle()
        {
            if (string.IsNullOrWhiteSpace(_subtitleText)) return;

            _subtitleLabel = NewLabel(_subtitleText, 0.95f, FontStyle.Regular, textAlignment);
            _scaffold.AddContent(_subtitleLabel);
        }

        private void AddParagraph()
        {
            if (string.IsNullOrWhiteSpace(paragraphText)) return;

            _paragraphLabel = NewLabel(paragraphText, 1.0f, FontStyle.Regular, textAlignment);
            _paragraphLabel.Cursor = Cursors.Hand;
            _paragraphLabel.Click += (s, e) => RaiseCardEvent(ParagraphClicked, nameof(ParagraphText), paragraphText);

            // The paragraph is the row that absorbs leftover height: it is the part with a variable
            // amount to say, so it is the part that should grow rather than leave a gap under the card.
            _scaffold.AddContent(_paragraphLabel, fill: true);
        }

        /// <summary>The one number the card exists to show, at display size.</summary>
        private void AddEmphasis()
        {
            if (string.IsNullOrWhiteSpace(_priceText)) return;

            _emphasisLabel = NewLabel(_priceText, 1.8f, FontStyle.Bold, textAlignment);
            _emphasisLabel.Height = 34;
            _scaffold.AddContent(_emphasisLabel);
        }

        /// <summary>
        /// The badges, on one row.
        /// </summary>
        /// <remarks>
        /// Badges are the exception this library allows to "nothing assigns colours": a badge's colour
        /// is set by the caller through <c>Badge1BackColor</c> and carries meaning — PRO against FREE,
        /// a discount against a category. Honouring the property is the point of it. Every other part
        /// of this card takes its colours from the theme by not being told.
        /// </remarks>
        private void AddBadges()
        {
            var badges = new List<Control>();

            if (!string.IsNullOrWhiteSpace(_badgeText1))
                badges.Add(NewBadge(_badgeText1, _badge1BackColor, _badge1ForeColor));

            if (!string.IsNullOrWhiteSpace(_badgeText2))
                badges.Add(NewBadge(_badgeText2, _badge2BackColor, _badge2ForeColor));

            if (badges.Count == 0) return;

            _scaffold.AddContent(NewStrip(badges));
        }

        private void AddTags()
        {
            var tags = (_tags ?? new List<string>())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => (Control)NewLabel(t, 0.85f, FontStyle.Regular, ContentAlignment.MiddleLeft))
                .ToList();

            if (tags.Count == 0) return;

            foreach (var tag in tags)
            {
                tag.Margin = new Padding(0, 0, 8, 0);
                tag.Dock = DockStyle.None;
                tag.AutoSize = true;
            }

            _scaffold.AddContent(NewStrip(tags));
        }

        private void AddRating()
        {
            if (!_showRating) return;

            _ratingControl = new BeepStarRating
            {
                StarCount = 5,
                SelectedRating = Math.Max(0, Math.Min(5, _rating)),
                ReadOnly = true,
                IsChild = true,
                IsFrameless = true,
                Size = new Size(100, 20),
                AccessibleName = $"{Math.Max(0, Math.Min(5, _rating))} out of 5 stars",
                Cursor = Cursors.Hand,
            };
            _ratingControl.Click += (s, e) => RaiseCardEvent(RatingClicked, nameof(Rating), _rating.ToString());

            _scaffold.AddContent(_ratingControl);
            _ratingControl.Dock = DockStyle.None;
            _ratingControl.Anchor = AnchorStyles.Left;
        }

        /// <summary>
        /// The status line: a dot in the status colour, then its text.
        /// </summary>
        /// <remarks>
        /// The dot's colour is <c>StatusColor</c>, which the caller sets to mean something — available,
        /// busy, offline. That is the same meaning-carrying exception the badges use.
        /// </remarks>
        private void AddStatus()
        {
            if (!_showStatus || string.IsNullOrWhiteSpace(_statusText)) return;

            var dot = new Panel
            {
                Size = new Size(8, 8),
                BackColor = _statusColor,
                Margin = new Padding(0, 6, 6, 0),
            };

            var text = NewLabel(_statusText, 0.9f, FontStyle.Regular, ContentAlignment.MiddleLeft);
            text.Dock = DockStyle.None;
            text.AutoSize = true;

            _scaffold.AddContent(NewStrip(new List<Control> { dot, text }));
        }

        private void AddActions(bool primary, bool secondary)
        {
            var actions = new List<Control>();

            if (primary && showButton && !string.IsNullOrWhiteSpace(buttonText))
            {
                _primaryButton = NewButton(buttonText);
                _primaryButton.Click += (s, e) => RaiseCardEvent(ButtonClicked, nameof(ButtonText), buttonText);
                actions.Add(_primaryButton);
            }

            if (secondary && showSecondaryButton && !string.IsNullOrWhiteSpace(secondaryButtonText))
            {
                _secondaryButton = NewButton(secondaryButtonText);
                _secondaryButton.Click += (s, e) => RaiseCardEvent(ButtonClicked, nameof(SecondaryButtonText), secondaryButtonText);
                actions.Add(_secondaryButton);
            }

            if (actions.Count == 0) return;

            _scaffold.AddActions(actions.ToArray());
        }

        /// <summary>A left-aligned row of controls that hug their content.</summary>
        private static TableLayoutPanel NewStrip(List<Control> items)
        {
            var strip = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                RowCount = 1,
                ColumnCount = items.Count,
                Margin = new Padding(0, 0, 0, 2),
                Anchor = AnchorStyles.Left,
            };
            strip.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            for (int i = 0; i < items.Count; i++)
            {
                strip.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                strip.Controls.Add(items[i], i, 0);
            }

            return strip;
        }

        private BeepLabel NewBadge(string text, Color back, Color fore) => new()
        {
            Text = text,
            AutoSize = true,
            IsChild = false,       // a badge is a mark ON the card, not part of its surface
            IsFrameless = true,
            BackColor = back,
            ForeColor = fore,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(6, 2, 6, 2),
            Margin = new Padding(0, 0, 6, 0),
            AccessibleName = text,
            TextFont = new Font(Font.FontFamily, Font.Size * 0.85f, FontStyle.Bold),
        };

        private BeepButton NewButton(string text) => new()
        {
            Text = text,
            AutoSize = false,
            Size = new Size(Math.Max(80, TextRenderer.MeasureText(text, Font).Width + 24), 28),
            IsChild = true,
            AccessibleName = text,
            Cursor = Cursors.Hand,
        };

        private BeepLabel NewLabel(string text, float scale, FontStyle style, ContentAlignment align) => new()
        {
            Text = text ?? string.Empty,
            AutoSize = false,
            Dock = DockStyle.Fill,
            Height = 22,
            IsChild = true,
            IsFrameless = true,
            WordWrap = true,
            Multiline = true,
            TextAlign = TranslateAlignment(align),
            TextFont = new Font(Font.FontFamily, Font.Size * scale, style),
            AccessibleName = text ?? string.Empty,
        };

        /// <summary>
        /// Keeps the horizontal intent of an alignment property and drops the vertical part.
        /// </summary>
        /// <remarks>
        /// The alignment properties were written for painted text placed inside a computed rectangle,
        /// where "top" meant something. A label occupies a row the layout engine sized to it, so only
        /// left/centre/right still has meaning — and reading the property is what keeps a caller's
        /// centred header centred.
        /// </remarks>
        private static ContentAlignment TranslateAlignment(ContentAlignment align) => align switch
        {
            ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter
                => ContentAlignment.MiddleCenter,
            ContentAlignment.TopRight or ContentAlignment.MiddleRight or ContentAlignment.BottomRight
                => ContentAlignment.MiddleRight,
            _ => ContentAlignment.MiddleLeft,
        };

        /// <summary>An icon, or null when there is no path — an empty BeepImage still occupies a cell.</summary>
        private BeepImage NewIcon(string path, int size)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            var image = new BeepImage
            {
                ImagePath = path,
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                IsChild = true,
                Margin = new Padding(0),
            };

            if (size > 0) image.Size = new Size(size, size);
            return image;
        }

        private void RaiseCardEvent(EventHandler<BeepEventDataArgs> handler, string name, string value)
        {
            handler?.Invoke(this, new BeepEventDataArgs(name, this));
        }

        private void ClearComposedReferences()
        {
            _avatarImage = null;
            _heroImage = null;
            _headerLabel = null;
            _subtitleLabel = null;
            _paragraphLabel = null;
            _emphasisLabel = null;
            _ratingControl = null;
            _primaryButton = null;
            _secondaryButton = null;
            _selectionCheckBox = null;
            _contextMenuButton = null;
            _collapseButton = null;
        }

        /// <summary>DPI-scales a design-time constant. Kept from the paint pass; the layout still needs it.</summary>
        private int Scale(int value) => DpiScalingHelper.ScaleValue(value, this);

        private void Recompose()
        {
            if (_composing || IsDisposed) return;
            Compose();
        }
    }
}
