// GroupTabPositionDialog.cs
// Sprint 19 — Per-group tab position editor dialog.
//
// Used by the BeepDocumentHost designer to let a developer set a different
// TabStripPosition (Top / Bottom / Left / Right / Hidden) for each split
// group at design time. Returns the set of changed positions through
// ChangedPositions so the caller can apply them inside a single
// DesignerTransaction for clean undo.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs
{
    internal sealed class GroupTabPositionDialog : Form
    {
        public Dictionary<string, TabStripPosition> ChangedPositions { get; } = new();

        private readonly Dictionary<string, ComboBox> _positionCombos = new();

        public GroupTabPositionDialog(BeepDocumentHost host)
        {
            Text = "Set Group Tab Positions";
            Size = new Size(420, 160 + host.Groups.Count * 36);
            MinimumSize = new Size(380, 200);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = SystemColors.Window;
            Font = new Font("Segoe UI", 9f);

            var titleLabel = new Label
            {
                Text = "Per-group tab strip position:",
                Dock = DockStyle.Top,
                Height = 28,
                Padding = new Padding(8, 4, 0, 0),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                Padding = new Padding(12, 8, 12, 0)
            };

            foreach (var grp in host.Groups)
            {
                var row = new Panel { Height = 32, Width = 360 };
                var label = new Label
                {
                    Text = $"Group {grp.GroupId.Substring(0, 8)}{(grp.IsPrimary ? " (Primary)" : "")}:",
                    Location = new Point(0, 6),
                    Width = 180,
                    TextAlign = ContentAlignment.MiddleRight
                };
                var combo = new ComboBox
                {
                    Location = new Point(190, 4),
                    Width = 150,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                combo.Items.AddRange(Enum.GetNames<TabStripPosition>());
                combo.SelectedItem = grp.TabPosition.ToString();
                _positionCombos[grp.GroupId] = combo;

                row.Controls.Add(label);
                row.Controls.Add(combo);
                panel.Controls.Add(row);
            }

            var btnPanel = new Panel { Dock = DockStyle.Bottom, Height = 48 };
            var okBtn = new Button { Text = "OK", Width = 80, Height = 28, DialogResult = DialogResult.OK };
            var cancelBtn = new Button { Text = "Cancel", Width = 80, Height = 28, DialogResult = DialogResult.Cancel };
            okBtn.Location = new Point(btnPanel.Width - 176, 10);
            cancelBtn.Location = new Point(btnPanel.Width - 88, 10);
            okBtn.Click += (s, e) => CollectChanges(host);
            btnPanel.Controls.Add(okBtn);
            btnPanel.Controls.Add(cancelBtn);

            AcceptButton = okBtn;
            CancelButton = cancelBtn;

            Controls.Add(panel);
            Controls.Add(btnPanel);
            Controls.Add(titleLabel);
        }

        private void CollectChanges(BeepDocumentHost host)
        {
            foreach (var grp in host.Groups)
            {
                if (_positionCombos.TryGetValue(grp.GroupId, out var combo)
                    && Enum.TryParse<TabStripPosition>(combo.SelectedItem?.ToString(), out var pos)
                    && pos != grp.TabPosition)
                {
                    ChangedPositions[grp.GroupId] = pos;
                }
            }
        }
    }
}
