using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    public class BeepMultiSelectDialog : BeepiFormPro
    {
        private readonly BeepPanel _headerPanel;
        private readonly BeepLabel _titleLabel;
        private readonly BeepLabel _messageLabel;
        private readonly BeepPanel _listPanel;
        private readonly BeepPanel _buttonPanel;
        private readonly BeepButton _selectAllButton;
        private readonly BeepButton _okButton;
        private readonly BeepButton _cancelButton;
        private readonly BeepPanel _scrollContainer;

        private readonly List<SimpleItem> _items = new();
        private readonly List<BeepCheckBoxBool> _checkBoxes = new();

        public List<SimpleItem> SelectedItems => _checkBoxes
            .Where(cb => cb.CurrentValue)
            .Select(cb => cb.Tag as SimpleItem)
            .Where(item => item != null)
            .ToList()!;

        public BeepMultiSelectDialog()
        {
            ShowCaptionBar = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(480, 380);
            MinimumSize = new Size(360, 260);

            _headerPanel = new BeepPanel
            {
                Dock = DockStyle.Top,
                Height = 70,
                IsFrameless = true,
                ShowTitle = false,
                ShowTitleLine = false,
                UseThemeColors = true,
                Padding = new Padding(16, 12, 16, 8)
            };

            _titleLabel = new BeepLabel
            {
                Location = new Point(16, 10),
                Size = new Size(440, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                UseThemeColors = true,
                IsFrameless = true,
                Font = new Font(Font.FontFamily, 12f, FontStyle.Bold)
            };

            _messageLabel = new BeepLabel
            {
                Location = new Point(16, 36),
                Size = new Size(440, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                UseThemeColors = true,
                IsFrameless = true
            };

            _headerPanel.Controls.Add(_titleLabel);
            _headerPanel.Controls.Add(_messageLabel);

            _listPanel = new BeepPanel
            {
                Dock = DockStyle.Fill,
                IsFrameless = true,
                ShowTitle = false,
                ShowTitleLine = false,
                UseThemeColors = true,
                Padding = new Padding(8, 4, 8, 4)
            };

            _scrollContainer = new BeepPanel
            {
                Dock = DockStyle.Fill,
                IsFrameless = true,
                ShowTitle = false,
                ShowTitleLine = false,
                UseThemeColors = true,
                AutoScroll = true
            };

            _listPanel.Controls.Add(_scrollContainer);

            _buttonPanel = new BeepPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                IsFrameless = true,
                ShowTitle = false,
                ShowTitleLine = false,
                UseThemeColors = true,
                Padding = new Padding(8, 8, 8, 8)
            };

            _selectAllButton = new BeepButton
            {
                Size = new Size(100, 32),
                Location = new Point(10, 9),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                UseThemeColors = true,
                Text = "Select All"
            };

            _okButton = new BeepButton
            {
                Size = new Size(100, 32),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                UseThemeColors = true,
                Text = "OK"
            };

            _cancelButton = new BeepButton
            {
                Size = new Size(100, 32),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                UseThemeColors = true,
                Text = "Cancel"
            };

            _selectAllButton.Click += (s, e) => ToggleSelectAll();
            _okButton.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };
            _cancelButton.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            _buttonPanel.Controls.Add(_selectAllButton);
            _buttonPanel.Controls.Add(_okButton);
            _buttonPanel.Controls.Add(_cancelButton);

            Controls.Add(_listPanel);
            Controls.Add(_buttonPanel);
            Controls.Add(_headerPanel);

            ApplyTheme();
        }

        public void SetItems(string title, string message, List<SimpleItem> items, IEnumerable<string>? preSelected = null)
        {
            _titleLabel.Text = title;
            _messageLabel.Text = message;
            _items.Clear();
            _items.AddRange(items);

            var selectedSet = preSelected != null
                ? new HashSet<string>(preSelected, StringComparer.OrdinalIgnoreCase)
                : null;

            _scrollContainer.Controls.Clear();
            _checkBoxes.Clear();

            int y = 4;
            int checkboxHeight = 28;
            int spacing = 2;

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var cb = new BeepCheckBoxBool
                {
                    Size = new Size(_scrollContainer.ClientSize.Width - 16, checkboxHeight),
                    Location = new Point(8, y),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    UseThemeColors = true,
                    Text = item.Text,
                    CurrentValue = selectedSet?.Contains(item.Text) ?? false,
                    Tag = item
                };

                _checkBoxes.Add(cb);
                _scrollContainer.Controls.Add(cb);
                y += checkboxHeight + spacing;
            }

            _scrollContainer.AutoScrollMinSize = new Size(0, y + 8);
        }

        private void ToggleSelectAll()
        {
            bool allChecked = _checkBoxes.All(cb => cb.CurrentValue);
            bool newState = !allChecked;
            foreach (var cb in _checkBoxes)
            {
                cb.CurrentValue = newState;
            }
            _selectAllButton.Text = newState ? "Deselect All" : "Select All";
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                _okButton.PerformClick();
                return true;
            }
            if (keyData == Keys.Escape)
            {
                _cancelButton.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public override void ApplyTheme()
        {
            if (_headerPanel == null) return;

            _headerPanel.Theme = Theme;
            _listPanel.Theme = Theme;
            _buttonPanel.Theme = Theme;
            _scrollContainer.Theme = Theme;
            _titleLabel.Theme = Theme;
            _messageLabel.Theme = Theme;
            _selectAllButton.Theme = Theme;
            _okButton.Theme = Theme;
            _cancelButton.Theme = Theme;

            foreach (var cb in _checkBoxes)
            {
                cb.Theme = Theme;
            }

            if (_currentTheme != null)
            {
                _okButton.BackColor = _currentTheme.AccentColor != Color.Empty
                    ? _currentTheme.AccentColor
                    : Color.FromArgb(59, 130, 246);
                _okButton.ForeColor = ColorUtils.GetContrastColor(_okButton.BackColor);
            }

            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PositionButtons();
            ResizeCheckBoxes();
        }

        private void PositionButtons()
        {
            int panelW = _buttonPanel.ClientSize.Width;
            int btnW = 100, btnH = 32, margin = 10;
            int y = (_buttonPanel.ClientSize.Height - btnH) / 2;

            _selectAllButton.SetBounds(margin, y, btnW, btnH);
            _cancelButton.SetBounds(panelW - margin - btnW, y, btnW, btnH);
            _okButton.SetBounds(panelW - margin - btnW * 2 - 8, y, btnW, btnH);
        }

        private void ResizeCheckBoxes()
        {
            int width = _scrollContainer.ClientSize.Width - 16;
            foreach (var cb in _checkBoxes)
            {
                cb.Width = width;
            }
        }
    }
}
