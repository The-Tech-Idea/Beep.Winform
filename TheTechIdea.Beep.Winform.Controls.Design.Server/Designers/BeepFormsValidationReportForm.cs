using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public enum BeepFormsValidationSeverity
    {
        Info,
        Warning,
        Error
    }

    public sealed class BeepFormsValidationIssue
    {
        public BeepFormsValidationSeverity Severity { get; init; }
        public string BlockName { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public override string ToString() => $"[{Severity}] {(string.IsNullOrEmpty(BlockName) ? "Form" : BlockName)}: {Message}";
    }

    public static class BeepFormsDefinitionValidator
    {
        public static IReadOnlyList<BeepFormsValidationIssue> Validate(BeepFormsDefinition? definition)
        {
            var issues = new List<BeepFormsValidationIssue>();
            if (definition == null)
            {
                issues.Add(new BeepFormsValidationIssue
                {
                    Severity = BeepFormsValidationSeverity.Warning,
                    Message = "Form has no definition assigned."
                });
                return issues;
            }

            if (string.IsNullOrWhiteSpace(definition.FormName))
            {
                issues.Add(new BeepFormsValidationIssue
                {
                    Severity = BeepFormsValidationSeverity.Warning,
                    Message = "Form Name is empty."
                });
            }
            if (string.IsNullOrWhiteSpace(definition.Title))
            {
                issues.Add(new BeepFormsValidationIssue
                {
                    Severity = BeepFormsValidationSeverity.Info,
                    Message = "Form Title is empty; the form will use Form Name as the title."
                });
            }

            var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var block in definition.Blocks)
            {
                if (block == null)
                {
                    continue;
                }
                if (string.IsNullOrWhiteSpace(block.BlockName))
                {
                    issues.Add(new BeepFormsValidationIssue
                    {
                        Severity = BeepFormsValidationSeverity.Error,
                        Message = "Block has an empty BlockName."
                    });
                    continue;
                }
                if (!seenNames.Add(block.BlockName))
                {
                    issues.Add(new BeepFormsValidationIssue
                    {
                        Severity = BeepFormsValidationSeverity.Error,
                        BlockName = block.BlockName,
                        Message = "Duplicate block name."
                    });
                }
                if (block.Entity == null || string.IsNullOrWhiteSpace(block.Entity.EntityName))
                {
                    issues.Add(new BeepFormsValidationIssue
                    {
                        Severity = BeepFormsValidationSeverity.Warning,
                        BlockName = block.BlockName,
                        Message = "Block has no entity assigned; data binding will be inert."
                    });
                }
                if (block.Fields == null || block.Fields.Count == 0)
                {
                    issues.Add(new BeepFormsValidationIssue
                    {
                        Severity = BeepFormsValidationSeverity.Info,
                        BlockName = block.BlockName,
                        Message = "Block has no field definitions."
                    });
                }
            }

            return issues;
        }
    }

    public sealed class BeepFormsValidationReportForm : Form
    {
        private readonly ListView _listView;
        private readonly Label _summaryLabel;
        private readonly Button _closeButton;

        public BeepFormsValidationReportForm(BeepFormsDefinition? definition)
        {
            var issues = BeepFormsDefinitionValidator.Validate(definition);

            Text = "Form Validation Report";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = false;
            MaximizeBox = true;
            ShowInTaskbar = false;
            Size = new Size(720, 460);
            MinimumSize = new Size(560, 320);

            int errors = issues.Count(i => i.Severity == BeepFormsValidationSeverity.Error);
            int warnings = issues.Count(i => i.Severity == BeepFormsValidationSeverity.Warning);
            int infos = issues.Count(i => i.Severity == BeepFormsValidationSeverity.Info);

            _summaryLabel = new Label
            {
                Text = $"{issues.Count} issue(s): {errors} error(s), {warnings} warning(s), {infos} info.",
                Dock = DockStyle.Top,
                Height = 32,
                Padding = new Padding(12, 0, 12, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont, FontStyle.Bold),
                ForeColor = errors > 0 ? Color.Firebrick : (warnings > 0 ? Color.DarkOrange : Color.DarkGreen)
            };

            _listView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Dock = DockStyle.Fill,
                MultiSelect = false
            };
            _listView.Columns.Add("Severity", 90);
            _listView.Columns.Add("Block", 180);
            _listView.Columns.Add("Message", 420);

            foreach (var issue in issues)
            {
                var item = new ListViewItem(issue.Severity.ToString());
                item.SubItems.Add(issue.BlockName);
                item.SubItems.Add(issue.Message);
                item.ForeColor = issue.Severity switch
                {
                    BeepFormsValidationSeverity.Error => Color.Firebrick,
                    BeepFormsValidationSeverity.Warning => Color.DarkOrange,
                    _ => SystemColors.ControlText
                };
                _listView.Items.Add(item);
            }

            _closeButton = new Button { Text = "Close", DialogResult = DialogResult.OK, Width = 90, Height = 30 };
            var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 40, Padding = new Padding(0, 6, 12, 6) };
            _closeButton.Location = new Point(buttonPanel.Width - _closeButton.Width - 6, 6);
            buttonPanel.Resize += (_, _) => _closeButton.Location = new Point(buttonPanel.Width - _closeButton.Width - 6, 6);
            buttonPanel.Controls.Add(_closeButton);

            Controls.Add(_listView);
            Controls.Add(_summaryLabel);
            Controls.Add(buttonPanel);
            AcceptButton = _closeButton;
            CancelButton = _closeButton;
        }
    }
}
