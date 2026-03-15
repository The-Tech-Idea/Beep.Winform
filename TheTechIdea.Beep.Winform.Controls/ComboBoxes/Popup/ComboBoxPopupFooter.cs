using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Buttons;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal class ComboBoxPopupFooter : Control
    {
        private readonly FlowLayoutPanel _layout;
        private readonly Label _countLabel;
        private ComboBoxPopupHostProfile _profile = ComboBoxPopupHostProfile.OutlineDefault();
        private ComboBoxThemeTokens _themeTokens = ComboBoxThemeTokens.Fallback();

        public event EventHandler ApplyClicked;
        public event EventHandler CancelClicked;
        public event EventHandler SelectAllClicked;
        public event EventHandler ClearAllClicked;
        public event EventHandler PrimaryActionClicked;

        private BeepButton _primaryActionButton;
        private bool _usePrimaryActionFooter;
        private string _primaryActionText = string.Empty;

        public ComboBoxPopupFooter()
        {
            Height = 40;
            Dock = DockStyle.Bottom;
            DoubleBuffered = true;

            _countLabel = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 4, 0),
                Visible = false
            };

            _layout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(6, 4, 6, 4)
            };
            Controls.Add(_layout);
            Controls.Add(_countLabel);
        }

        public void Setup(bool showApplyCancel, bool showSelectAll, bool usePrimaryActionFooter = false, string primaryActionText = null)
        {
            _layout.Controls.Clear();
            _primaryActionButton = null;
            _usePrimaryActionFooter = usePrimaryActionFooter;
            _primaryActionText = primaryActionText ?? string.Empty;

            if (_usePrimaryActionFooter)
            {
                string ctaText = ResolvePrimaryActionText(0);
                _primaryActionButton = AddButton(ctaText, (s, e) => PrimaryActionClicked?.Invoke(this, EventArgs.Empty));
            }
            else
            {
                if (showApplyCancel)
                {
                    AddButton("Apply", (s, e) => ApplyClicked?.Invoke(this, EventArgs.Empty));
                    AddButton("Cancel", (s, e) => CancelClicked?.Invoke(this, EventArgs.Empty));
                }

                if (showSelectAll)
                {
                    AddButton("Clear all", (s, e) => ClearAllClicked?.Invoke(this, EventArgs.Empty));
                    AddButton("Select all", (s, e) => SelectAllClicked?.Invoke(this, EventArgs.Empty));
                }
            }

            Visible = _layout.Controls.Count > 0;
            _countLabel.Visible = false;
        }

        public void ApplyProfile(ComboBoxPopupHostProfile profile)
        {
            _profile = profile ?? ComboBoxPopupHostProfile.OutlineDefault();
            Height = _profile.FooterHeight;
            _layout.Padding = new Padding(6, 4, 6, 4);
            _layout.FlowDirection = _profile.FooterLeftAligned ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
        }

        public void ApplyThemeTokens(ComboBoxThemeTokens tokens)
        {
            _themeTokens = tokens ?? ComboBoxThemeTokens.Fallback();
            BackColor = _themeTokens.PopupBackColor;
            _layout.BackColor = _themeTokens.PopupBackColor;
            _countLabel.BackColor = _themeTokens.PopupBackColor;
            _countLabel.ForeColor = _themeTokens.PopupSubTextColor;
        }

        public void UpdateSelectedCount(int count)
        {
            if (_usePrimaryActionFooter && _primaryActionButton != null)
            {
                _primaryActionButton.Text = ResolvePrimaryActionText(count);
                _countLabel.Visible = false;
                return;
            }

            if (count > 0)
            {
                _countLabel.Text = $"{count} selected";
                _countLabel.Visible = true;
            }
            else
            {
                _countLabel.Visible = false;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var pen = new Pen(_themeTokens.PopupSeparatorColor);
            e.Graphics.DrawLine(pen, 0, 0, Width - 1, 0);
        }

        private BeepButton AddButton(string text, EventHandler onClick)
        {
            var button = new BeepButton
            {
                Text = text,
                AutoSize = true,
                MinimumSize = new Size(78, Math.Max(26, _profile.FooterHeight - 12)),
                Margin = new Padding(4, 0, 0, 0),
                UseThemeColors = true,
                IsChild = true
            };
            button.Click += onClick;
            _layout.Controls.Add(button);
            return button;
        }

        private string ResolvePrimaryActionText(int count)
        {
            if (string.IsNullOrWhiteSpace(_primaryActionText))
            {
                return count > 0 ? $"Apply ({count})" : "Apply";
            }

            if (_primaryActionText.Contains("{count}"))
            {
                return _primaryActionText.Replace("{count}", count.ToString());
            }

            return _primaryActionText;
        }
    }
}
