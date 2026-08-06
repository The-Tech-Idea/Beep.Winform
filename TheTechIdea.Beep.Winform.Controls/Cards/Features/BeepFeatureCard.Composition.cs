using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepFeatureCard
    {
        private CardScaffold _scaffold;

        private BeepImage _logoImage;
        private BeepImage _cardIconImage;
        private BeepLabel _titleLabel;
        private BeepLabel _subtitleLabel;
        private BeepButton _action1Button;
        private BeepButton _action2Button;

        private bool _composing;

        /// <summary>
        /// Builds the card from controls. Replaces the <c>DrawContent</c> this card used to run.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Four icon paths — <c>CardIconPath</c>, <c>BulletIconPath</c>, <c>ActionIcon1Path</c> and
        /// <c>ActionIcon2Path</c> — had no readers anywhere in the solution. On a control whose whole
        /// description is an <i>icon-based</i> feature highlight, every icon a caller set was dropped.
        /// Each one now has a control to be assigned to, which is what stops the wiring being something
        /// somebody has to remember.
        /// </para>
        /// <para>
        /// The two actions are <c>BeepButton</c>s, not images. Painted icons had no focus, no keyboard
        /// route and no hit-testing beyond a rectangle this card compared mouse coordinates against by
        /// hand; a button has all three because it is a button.
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
                DetachListBox();
                _scaffold.Clear();

                _cardIconImage = NewIcon(cardIconPath, 32);
                if (_cardIconImage != null)
                {
                    _cardIconImage.Cursor = Cursors.Hand;
                    _cardIconImage.Click += (s, e) => OnCardIconClick();
                    _scaffold.SetIcon(_cardIconImage);
                }

                _logoImage = NewIcon(logoPath, 24);
                if (_logoImage != null)
                {
                    _scaffold.AddContent(_logoImage);
                    // Left-aligned: a logo stretched across the card's width is not a logo.
                    _logoImage.Dock = DockStyle.None;
                    _logoImage.Anchor = AnchorStyles.Left;
                }

                _titleLabel = NewLabel(titleText, 1.15f, FontStyle.Bold);
                _scaffold.AddContent(_titleLabel);

                _subtitleLabel = NewLabel(subtitleText, 0.95f, FontStyle.Regular);
                _scaffold.AddContent(_subtitleLabel);

                ComposeBullets();
                ComposeActions();

                _scaffold.ResumeLayout(true);
            }
            finally
            {
                _composing = false;
            }
        }

        /// <summary>Puts the bullet list in the row that absorbs the card's leftover height.</summary>
        /// <remarks>
        /// The bullets stay a <c>BeepListBox</c> rather than becoming one composed row per bullet. The
        /// list box already renders an icon and a label per item and already honours
        /// <see cref="ListStyle"/>; composing rows by hand would wire <c>BulletIconPath</c> and silently
        /// lose the other presentation modes — the exact failure stage 04 named. Using the control that
        /// exists <i>is</i> the composition rule, not an exception to it.
        /// </remarks>
        private void ComposeBullets()
        {
            if (featuresListBox == null) return;

            UpdateFeaturesList();
            _scaffold.AddContent(featuresListBox, fill: true);
        }

        private void ComposeActions()
        {
            _action1Button = NewActionButton(actionIcon1Path, "Action 1", () => OnActionIcon1Click());
            _action2Button = NewActionButton(actionIcon2Path, "Action 2", () => OnActionIcon2Click());

            if (_action1Button == null && _action2Button == null) return;

            if (_action1Button != null && _action2Button != null)
                _scaffold.AddActions(_action1Button, _action2Button);
            else
                _scaffold.AddActions(_action1Button ?? _action2Button);
        }

        /// <summary>
        /// Takes the list box out of the scaffold before a clear, so it survives recomposition.
        /// </summary>
        /// <remarks>
        /// <see cref="CardScaffold.Clear"/> disposes what it removes — right for controls it built, wrong
        /// for this one, which the card owns and reuses. Without this a second recompose would bind a
        /// disposed list box.
        /// </remarks>
        private void DetachListBox()
        {
            if (featuresListBox != null && featuresListBox.Parent == _scaffold)
                _scaffold.Controls.Remove(featuresListBox);
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

        private BeepButton NewActionButton(string path, string name, Action onClick)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            var button = new BeepButton
            {
                ImagePath = path,
                Text = string.Empty,
                HideText = true,
                MaxImageSize = new Size(20, 20),
                Size = new Size(32, 32),
                AutoSize = false,
                IsChild = true,
                IsFrameless = true,
                AccessibleName = name,
                Cursor = Cursors.Hand,
            };

            button.Click += (s, e) => onClick();
            return button;
        }

        private void Recompose()
        {
            if (_composing || IsDisposed) return;
            Compose();
        }
    }
}
