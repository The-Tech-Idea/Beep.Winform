using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.CommandPalette
{
    public class BeepCommandPaletteDialog : Form
    {
        private readonly BeepTextBox _search = new BeepTextBox { Dock = DockStyle.Top, UseThemeColors = true };
        private readonly BeepListBox _list = new BeepListBox { Dock = DockStyle.Fill, UseThemeColors = true };
        private readonly List<CommandAction> _actions = new();
        private List<CommandAction> _filtered = new();

        public BeepCommandPaletteDialog()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            Width = 520;
            Height = 420;
            Controls.Add(_list);
            Controls.Add(_search);
            _search.TextChanged += (s, e) => Rebind();
            _list.DoubleClick += (s, e) => ExecuteSelected();
        }

        public void SetActions(IEnumerable<CommandAction> actions)
        {
            _actions.Clear();
            _actions.AddRange(actions ?? Enumerable.Empty<CommandAction>());
            Rebind();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter) { ExecuteSelected(); return true; }
            if (keyData == Keys.Escape) { DialogResult = DialogResult.Cancel; Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Rebind()
        {
            string q = _search.Text ?? string.Empty;
            _filtered = _actions.Where(a => string.IsNullOrWhiteSpace(q) || a.Text.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
            _list.ListItems = new BindingList<SimpleItem>(_filtered.Select(a => new SimpleItem { Text = a.Text, Value = a }).ToList());
        }

        private void ExecuteSelected()
        {
            if (_list.SelectedItem?.Value is CommandAction action)
            {
                action.Action?.Invoke();
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
