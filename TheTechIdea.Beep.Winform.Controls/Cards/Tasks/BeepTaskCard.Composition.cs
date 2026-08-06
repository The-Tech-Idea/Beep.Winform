using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTaskCard
    {
        private CardScaffold _scaffold;

        private BeepLabel _titleLabel;
        private BeepLabel _subtitleLabel;
        private BeepLabel _metricLabel;
        private BeepProgressBar _progressBar;
        private BeepButton _moreButton;

        private bool _composing;

        /// <summary>
        /// Builds the card from controls. Replaces the <c>DrawContent</c> this card used to run.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <c>MoreIcon</c> is the clearest argument in this folder for the refactor. It had no readers
        /// anywhere in the solution, and an overflow affordance is only useful if it can be clicked,
        /// focused and reached from the keyboard — painted, it is none of those, so it was never going
        /// to work whether or not it rendered. As a <see cref="BeepButton"/> it is all three by
        /// construction, and <c>MoreIconClick</c> is raised by an event rather than by comparing mouse
        /// coordinates against a rectangle.
        /// </para>
        /// <para>
        /// The avatars are the same story with the collection attached: each path in
        /// <see cref="AvatarImagePaths"/> becomes a <c>BeepImage</c> that raises
        /// <c>AvatarClick</c> with its own index, so the index carried by the event args is produced by
        /// the control that was actually clicked instead of by hit-test arithmetic.
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

                _titleLabel = NewLabel(_titleText, 1.15f, FontStyle.Bold);
                _scaffold.AddContent(_titleLabel);

                _subtitleLabel = NewLabel(_subtitleText, 0.95f, FontStyle.Regular);
                _scaffold.AddContent(_subtitleLabel);

                _metricLabel = NewLabel(_metricText, 0.9f, FontStyle.Regular);
                _scaffold.AddContent(_metricLabel);

                ComposeProgress();
                ComposeFooter();

                _scaffold.ResumeLayout(true);
            }
            finally
            {
                _composing = false;
            }
        }

        /// <summary>The progress bar, which is a control this library already has.</summary>
        private void ComposeProgress()
        {
            _progressBar = new BeepProgressBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = (int)Math.Round(_progressValue),
                Height = 8,
                IsChild = true,
                IsFrameless = true,
                AccessibleName = $"{Math.Round(_progressValue)} percent complete",
            };

            _scaffold.AddFullWidth(_progressBar);
        }

        /// <summary>The avatar strip and the overflow button, on one row.</summary>
        private void ComposeFooter()
        {
            var footer = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                RowCount = 1,
                Margin = new Padding(0, 6, 0, 0),
            };
            footer.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            int column = 0;

            for (int i = 0; i < _avatarImagePaths.Count; i++)
            {
                var avatar = NewIcon(_avatarImagePaths[i], 24);
                if (avatar == null) continue;

                int index = i;                       // captured per avatar, so the event reports its own
                avatar.Cursor = Cursors.Hand;
                avatar.Margin = new Padding(column == 0 ? 0 : 4, 0, 0, 0);
                avatar.AccessibleName = $"Assignee {index + 1}";
                avatar.Click += (s, e) => OnAvatarClick(index);

                footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                footer.Controls.Add(avatar, column++, 0);
            }

            // A percent column between the avatars and the overflow button, so the button sits right
            // without a spacer that starves the AutoSize columns beside it.
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            footer.Controls.Add(new Panel { Margin = new Padding(0), Height = 1, Dock = DockStyle.Fill }, column++, 0);

            if (!string.IsNullOrWhiteSpace(_moreIconPath))
            {
                _moreButton = new BeepButton
                {
                    ImagePath = _moreIconPath,
                    Text = string.Empty,
                    HideText = true,
                    MaxImageSize = new Size(16, 16),
                    Size = new Size(24, 24),
                    AutoSize = false,
                    IsChild = true,
                    IsFrameless = true,
                    AccessibleName = "More options",
                    Cursor = Cursors.Hand,
                };
                _moreButton.Click += (s, e) => OnMoreIconClick();

                footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                footer.Controls.Add(_moreButton, column, 0);
            }

            footer.ColumnCount = column + 1;
            _scaffold.AddFullWidth(footer);
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
