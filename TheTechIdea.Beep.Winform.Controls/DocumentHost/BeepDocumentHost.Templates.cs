// BeepDocumentHost.Templates.cs
// Phase 7 partial — template application, workspace export/import helpers,
// layout history browser, and undo/redo Ctrl+Z/Y keyboard routing.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentHost
    {
        // ── Template picker popup ─────────────────────────────────────────────

        /// <summary>
        /// Shows the interactive <see cref="BeepLayoutTemplatePicker"/> and applies
        /// the chosen template to this host.
        /// </summary>
        internal void ShowTemplatePicker()
        {
            var pos = PointToScreen(new Point(
                Math.Max(0, (Width  - 560) / 2),
                Math.Max(0, (Height - 500) / 2)));

            using var picker = new BeepLayoutTemplatePicker(TemplateLibrary, _currentTheme, pos);
            if (picker.ShowDialog(this) != DialogResult.OK) return;

            var template = picker.SelectedTemplate;
            if (template == null) return;

            ApplyLayoutTemplate(template);
        }

        // ── Apply layout template ─────────────────────────────────────────────

        /// <summary>
        /// Applies the structural shape of <paramref name="template"/> to this host.
        /// Open documents are redistributed across the new groups; excess documents
        /// remain in the primary group.
        /// </summary>
        internal void ApplyLayoutTemplate(BeepLayoutTemplate template)
        {
            ArgumentNullException.ThrowIfNull(template);

            // Push undo state before structural change
            PushUndoState();
            if (_trackLayoutHistory) PushLayoutVersion($"Before template '{template.Name}'");

            // Collect current document IDs in display order
            var allDocIds = _groups
                .SelectMany(g => g.TabStrip.Tabs.Select(t => t.Id))
                .ToList();

            int groupCount = Math.Max(1, template.GroupCount);

            // Merge down to a single group if we currently have multiple
            while (_groups.Count > 1)
                MergeLastGroup();

            // Now create the required number of additional groups via splits
            if (groupCount >= 2)
            {
                // Parse orientation from template shape JSON
                bool horizontal = !template.LayoutShapeJson.Contains("\"Vertical\"");
                for (int i = 1; i < groupCount; i++)
                {
                    if (_groups.Count >= _maxGroups) break;
                    if (horizontal) SplitGroupHorizontal(_primaryGroup);
                    else            SplitGroupVertical(_primaryGroup);
                }
            }

            // Redistribute documents across groups
            if (_groups.Count > 1 && allDocIds.Count > 0)
            {
                // Round-robin distribution: first doc stays in group 0, rest spread evenly
                for (int i = 1; i < allDocIds.Count; i++)
                {
                    int targetGroup = i % _groups.Count;
                    if (targetGroup > 0)
                        MoveDocumentToGroup(allDocIds[i], _groups[targetGroup]);
                }
            }

            RecalculateLayout();
        }

        // ── Workspace export / import UI helpers ──────────────────────────────

        internal void ExportWorkspaceToJsonFile()
        {
            var ws = Workspaces.GetActive();
            if (ws == null)
            {
                MessageBox.Show("No active workspace to export.", "Export Workspace",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dlg = new SaveFileDialog
            {
                Title      = "Export Workspace",
                Filter     = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                FileName   = $"{ws.Name}.json",
                DefaultExt = "json",
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                File.WriteAllText(dlg.FileName, BeepWorkspacePorter.ExportToJson(ws));
                MessageBox.Show($"Workspace exported to:\n{dlg.FileName}", "Export Workspace",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void ExportWorkspacesToZipFile()
        {
            var workspaces = Workspaces.GetAll();
            if (workspaces.Count == 0)
            {
                MessageBox.Show("No workspaces to export.", "Export Workspaces",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dlg = new SaveFileDialog
            {
                Title      = "Export All Workspaces",
                Filter     = "ZIP archives (*.zip)|*.zip|All files (*.*)|*.*",
                FileName   = "workspaces.zip",
                DefaultExt = "zip",
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                BeepWorkspacePorter.ExportToZip(workspaces, dlg.FileName);
                MessageBox.Show($"Exported {workspaces.Count} workspace(s) to:\n{dlg.FileName}",
                    "Export Workspaces", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void ImportWorkspaceFromJsonFile()
        {
            using var dlg = new OpenFileDialog
            {
                Title  = "Import Workspace",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                var json   = File.ReadAllText(dlg.FileName);
                var result = BeepWorkspacePorter.ImportFromJson(json);
                FinalizeImport(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Import failed: {ex.Message}", "Import Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void ImportWorkspacesFromZipFile()
        {
            using var dlg = new OpenFileDialog
            {
                Title  = "Import Workspaces from ZIP",
                Filter = "ZIP archives (*.zip)|*.zip|All files (*.*)|*.*",
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                var result = BeepWorkspacePorter.ImportFromZip(dlg.FileName);
                FinalizeImport(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Import failed: {ex.Message}", "Import Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void ImportWorkspaceFromClipboard()
        {
            var result = BeepWorkspacePorter.PasteFromClipboard();
            FinalizeImport(result);
        }

        private void FinalizeImport(WorkspaceImportResult result)
        {
            if (!result.Success)
            {
                var msg = result.Warnings.Count > 0
                    ? string.Join("\n", result.Warnings)
                    : "No valid workspaces found.";
                MessageBox.Show(msg, "Import Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool overwrite = MessageBox.Show(
                $"Import {result.Workspaces.Count} workspace(s)?\n" +
                "(Yes = overwrite existing names, No = skip duplicates)",
                "Import Workspaces",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

            BeepWorkspacePorter.MergeInto(result, Workspaces, overwrite);

            if (result.Warnings.Count > 0)
                MessageBox.Show(string.Join("\n", result.Warnings), "Import Warnings",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ── Layout history browser ────────────────────────────────────────────

        internal void ShowLayoutHistoryPicker()
        {
            var history = LayoutHistory.GetAll();
            if (history.Count == 0)
            {
                MessageBox.Show("No layout history yet.", "Layout History",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Simple InputBox-style selection via ListBox dialog
            using var dlg   = new Form
            {
                Text            = "Layout History",
                Size            = new System.Drawing.Size(440, 360),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition   = FormStartPosition.CenterParent,
                MaximizeBox     = false,
                MinimizeBox     = false,
            };
            var list = new ListBox
            {
                Dock     = DockStyle.Fill,
                Font     = FontManagement.BeepFontManager.GetCachedFont("Segoe UI", 9.5f, System.Drawing.FontStyle.Regular),
            };
            foreach (var v in history)
                list.Items.Add(v);

            var btnRestore = new Button { Text = "Restore", Dock = DockStyle.Bottom, Height = 32 };
            btnRestore.Click += (_, _) =>
            {
                if (list.SelectedItem is LayoutVersionEntry entry)
                {
                    PushUndoState();
                    RestoreLayout(entry.LayoutJson);
                    dlg.DialogResult = DialogResult.OK;
                    dlg.Close();
                }
            };

            dlg.Controls.Add(list);
            dlg.Controls.Add(btnRestore);
            dlg.ShowDialog(this);
        }

        // ── Private split helpers (used by ApplyLayoutTemplate) ───────────────

        private void SplitGroupHorizontal(BeepDocumentGroup group)
        {
            var firstTab = group.TabStrip.Tabs.FirstOrDefault();
            if (firstTab != null) SplitDocumentHorizontal(firstTab.Id);
        }

        private void SplitGroupVertical(BeepDocumentGroup group)
        {
            var firstTab = group.TabStrip.Tabs.FirstOrDefault();
            if (firstTab != null) SplitDocumentVertical(firstTab.Id);
        }

        private void MergeLastGroup()
        {
            // Move all docs from the last group to the primary group, then collapse
            if (_groups.Count <= 1) return;
            var last = _groups[_groups.Count - 1];
            foreach (var tab in last.TabStrip.Tabs.ToList())
                MoveDocumentToGroup(tab.Id, _primaryGroup);
            CollapseEmptyGroup(last);
        }

        private void MoveDocumentToGroup(string docId, BeepDocumentGroup targetGroup)
        {
            var sourceGroup = _groups.FirstOrDefault(g =>
                g.TabStrip.Tabs.Any(t => t.Id == docId));
            if (sourceGroup == null || sourceGroup == targetGroup) return;

            if (!_panels.TryGetValue(docId, out var panel)) return;

            var tab = sourceGroup.TabStrip.Tabs.FirstOrDefault(t => t.Id == docId);
            if (tab == null) return;

            // Move the panel physically
            sourceGroup.ContentArea.Controls.Remove(panel);
            int tabIdx = sourceGroup.TabStrip.Tabs.ToList().FindIndex(t => t.Id == docId);
            if (tabIdx >= 0) sourceGroup.TabStrip.RemoveTabAt(tabIdx);

            targetGroup.TabStrip.AddTab(docId, tab.Title, tab.IconPath);
            targetGroup.ContentArea.Controls.Add(panel);
            panel.Dock = DockStyle.Fill;
        }
    }
}
