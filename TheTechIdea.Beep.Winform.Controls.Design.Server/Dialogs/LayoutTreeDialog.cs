// LayoutTreeDialog.cs
// Sprint 19 — Read-only viewer for the BeepDocumentHost layout tree.
//
// Renders the BeepDocumentHost.LayoutRoot (GroupLayoutNode / SplitLayoutNode
// pairs) as an indented text tree. Useful at design time to verify what the
// LayoutPresetPickerDialog or programmatic SplitDocument calls produced.
// ─────────────────────────────────────────────────────────────────────────────
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs
{
    internal sealed class LayoutTreeDialog : Form
    {
        public LayoutTreeDialog(BeepDocumentHost host)
        {
            Text = "Layout Tree Structure";
            Size = new Size(480, 360);
            MinimumSize = new Size(380, 280);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = SystemColors.Window;
            Font = new Font("Segoe UI", 9f);

            var titleLabel = new Label
            {
                Text = $"Layout Tree ({host.Groups.Count} groups)",
                Dock = DockStyle.Top,
                Height = 28,
                Padding = new Padding(8, 4, 0, 0),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            var treeBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 9f),
                BackColor = SystemColors.ControlLightLight,
                Text = FormatLayoutTree(host.LayoutRoot)
            };

            var btnPanel = new Panel { Dock = DockStyle.Bottom, Height = 48 };
            var closeBtn = new Button { Text = "Close", Width = 80, Height = 28, DialogResult = DialogResult.OK };
            closeBtn.Location = new Point(btnPanel.Width - 92, 10);
            btnPanel.Controls.Add(closeBtn);
            AcceptButton = closeBtn;

            Controls.Add(treeBox);
            Controls.Add(btnPanel);
            Controls.Add(titleLabel);
        }

        private static string FormatLayoutTree(ILayoutNode node, int depth = 0)
        {
            var indent = new string(' ', depth * 3);
            if (node is GroupLayoutNode g)
            {
                return $"{indent}[Group] {g.DocumentIds.Count} docs{(g.SelectedDocumentId != null ? $" (active: {g.SelectedDocumentId})" : "")}";
            }
            if (node is SplitLayoutNode s)
            {
                var orient = s.Orientation == Orientation.Horizontal ? "Horizontal" : "Vertical";
                return $"{indent}[Split] {orient} ({s.Ratio:P0})\n{FormatLayoutTree(s.First, depth + 1)}\n{FormatLayoutTree(s.Second, depth + 1)}";
            }
            return $"{indent}[Unknown]";
        }
    }
}
