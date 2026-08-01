using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    public partial class BeepMultiSelectDialog : BeepiFormPro
    {
        private readonly List<SimpleItem> _items = new();
        private readonly List<BeepCheckBoxBool> _checkBoxes = new();

        public List<SimpleItem> SelectedItems => _checkBoxes
            .Where(cb => cb.CurrentValue)
            .Select(cb => cb.Tag as SimpleItem)
            .Where(item => item != null)
            .ToList()!;

        public BeepMultiSelectDialog()
        {
            InitializeComponent();
            ComposeLayout();
            Helpers.DialogHelpers.FitFormToContent(this);
        }

        /// <summary>Places the controls into the shell's header / body / footer regions.</summary>
        private void ComposeLayout()
        {
            _headerPanel.Controls.Add(_dialogIcon, 0, 0);
            _headerPanel.Controls.Add(_titleLabel, 1, 0);

            _bodyPanel.Controls.Add(_messageLabel, 0, 0);
            _bodyPanel.Controls.Add(_listPanel, 0, 1);

            _shell.SetHeader(_headerPanel);
            _shell.SetBody(_bodyPanel);
            _shell.AddAction(_cancelButton);
            _shell.AddAction(_okButton);
            _shell.AddAction(_selectAllButton);

            _okButton.DialogResult = DialogResult.OK;
            _cancelButton.DialogResult = DialogResult.Cancel;
            AcceptButton = _okButton;
            CancelButton = _cancelButton;
        }

        public string Title { get => _titleLabel.Text; set => Helpers.DialogHelpers.SetTitle(this, _titleLabel, value); }
        public string Message { get => _messageLabel.Text; set => _messageLabel.Text = value ?? string.Empty; }

        public void SetItems(string title, string message, List<SimpleItem> items,
                             IEnumerable<string>? preSelected = null)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            Title = title;
            Message = message;

            _items.Clear();
            _items.AddRange(items);

            var selectedSet = preSelected != null
                ? new HashSet<string>(preSelected, StringComparer.OrdinalIgnoreCase)
                : null;

            _listPanel.SuspendLayout();
            foreach (var old in _checkBoxes) old.Dispose();
            _listPanel.Controls.Clear();
            _checkBoxes.Clear();

            foreach (var item in _items)
            {
                var cb = new BeepCheckBoxBool
                {
                    AutoSize = true,
                    UseThemeColors = true,
                    Theme = Theme,
                    Text = item.Text,
                    CurrentValue = selectedSet?.Contains(item.Text) ?? false,
                    Tag = item,
                    Margin = new Padding(0, 0, 0, 4)
                };

                _checkBoxes.Add(cb);
                _listPanel.Controls.Add(cb);
            }

            _listPanel.ResumeLayout(true);
            UpdateSelectAllCaption();
        }

        private void SelectAllButton_Click(object? sender, EventArgs e)
        {
            bool selectAll = !_checkBoxes.All(cb => cb.CurrentValue);
            foreach (var cb in _checkBoxes) cb.CurrentValue = selectAll;
            UpdateSelectAllCaption();
        }

        /// <summary>Keeps the button's caption describing what pressing it will do.</summary>
        private void UpdateSelectAllCaption()
        {
            bool allChecked = _checkBoxes.Count > 0 && _checkBoxes.All(cb => cb.CurrentValue);
            _selectAllButton.Text = allChecked ? "Deselect All" : "Select All";
            _selectAllButton.Enabled = _checkBoxes.Count > 0;
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // ProcessCmdKey override removed: AcceptButton and CancelButton route Enter and Escape.

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (_checkBoxes.Count > 0) _checkBoxes[0].Focus();
        }

        public override void ApplyTheme()
        {
            if (_titleLabel == null) return;
            // The header/body/footer panels are layout, not themed surfaces — the shell's rows carry
            // no chrome of their own, so only the Beep controls inside them take the theme.
            _dialogIcon.Theme = Theme; _titleLabel.Theme = Theme; _messageLabel.Theme = Theme;
            _selectAllButton.Theme = Theme; _okButton.Theme = Theme; _cancelButton.Theme = Theme;
            foreach (var cb in _checkBoxes) cb.Theme = Theme;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }
    }
}
