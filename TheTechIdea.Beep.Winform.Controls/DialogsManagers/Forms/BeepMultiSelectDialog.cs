using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
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
            Helpers.DialogHelpers.FitFormToContent(this);

            _headerPanel = new BeepPanel
            {
                Dock = DockStyle.Top,
                // Header height = ~2 label rows + padding (skill § default-size composition).
                Height = BeepLayoutMetrics.LabelStandard.Height.ScaleValue(this) * 2
                       + BeepLayoutMetrics.DialogPadding.All.ScaleValue(this)
                       + BeepLayoutMetrics.SmallGap.ScaleValue(this),
                IsFrameless = true,
                ShowTitle = false,
                ShowTitleLine = false,
                UseThemeColors = true,
                Padding = BeepLayoutMetrics.DialogPadding.ScalePadding(this)
            };

            _titleLabel = new BeepLabel
            {
                Location = new Point(
                    BeepLayoutMetrics.DialogPadding.Left.ScaleValue(this),
                    BeepLayoutMetrics.SmallGap.ScaleValue(this)),
                Size = new Size(
                    ClientSize.Width - BeepLayoutMetrics.DialogPadding.Left.ScaleValue(this) * 2,
                    BeepLayoutMetrics.LabelStandard.Height.ScaleValue(this)),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                UseThemeColors = true,
                IsFrameless = true,
                Font = BeepThemesManager.ToFont(BeepThemesManager.CurrentTheme?.TitleStyle) ?? SystemFonts.DefaultFont
            };

            _messageLabel = new BeepLabel
            {
                Location = new Point(
                    BeepLayoutMetrics.DialogPadding.Left.ScaleValue(this),
                    BeepLayoutMetrics.LabelStandard.Height.ScaleValue(this) + BeepLayoutMetrics.SmallGap.ScaleValue(this)),
                Size = new Size(
                    ClientSize.Width - BeepLayoutMetrics.DialogPadding.Left.ScaleValue(this) * 2,
                    BeepLayoutMetrics.LabelStandard.Height.ScaleValue(this)),
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

            int y = BeepLayoutMetrics.SmallGap.ScaleValue(this);
            // Checkbox row = label-standard height + small padding (skill § default-size).
            int checkboxHeight = BeepLayoutMetrics.LabelStandard.Height.ScaleValue(this);
            int spacing = BeepLayoutMetrics.SmallGap.ScaleValue(this);
            int sideMargin = BeepLayoutMetrics.DialogPadding.Left.ScaleValue(this) / 2;

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var cb = new BeepCheckBoxBool
                {
                    Size = new Size(_scrollContainer.ClientSize.Width - (sideMargin * 2), checkboxHeight),
                    Location = new Point(sideMargin, y),
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

            _scrollContainer.AutoScrollMinSize = new Size(0, y + BeepLayoutMetrics.SmallGap.ScaleValue(this));
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
