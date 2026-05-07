using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    public partial class uc_NuggetsManage
    {
        private BeepComboBox? _cmbLogFilter;
        private BeepButton? _btnLogClear;
        private BeepButton? _btnLogCopy;
        private BeepButton? _btnLogExport;
        private BeepGridPro? _gridLogs;
        private BeepLabel? _lblLogStatus;
        private DataTable? _dtLogs;

        private void BuildActivityTab(TabPage tab)
        {
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(6) };

            _cmbLogFilter = new BeepComboBox { Location = new System.Drawing.Point(6, 5), Size = new System.Drawing.Size(120, 27) };
            _cmbLogFilter.ListItems = new System.ComponentModel.BindingList<SimpleItem>
            {
                new SimpleItem { Text = "All", Item = "All" },
                new SimpleItem { Text = "Info", Item = "Info" },
                new SimpleItem { Text = "Success", Item = "Success" },
                new SimpleItem { Text = "Warning", Item = "Warning" },
                new SimpleItem { Text = "Error", Item = "Error" }
            };
            _cmbLogFilter.SelectedItemChanged += (_, _) => RefreshLogs();

            _btnLogClear = new BeepButton { Text = "Clear", Location = new System.Drawing.Point(136, 5), Size = new System.Drawing.Size(80, 27) };
            _btnLogClear.Click += (_, _) => { GetService().ClearLogs(); RefreshLogs(); };

            _btnLogCopy = new BeepButton { Text = "Copy", Location = new System.Drawing.Point(224, 5), Size = new System.Drawing.Size(80, 27) };
            _btnLogCopy.Click += (_, _) => CopyLogsToClipboard();

            _btnLogExport = new BeepButton { Text = "Export", Location = new System.Drawing.Point(312, 5), Size = new System.Drawing.Size(80, 27) };
            _btnLogExport.Click += (_, _) => ExportLogs();

            topPanel.Controls.Add(_cmbLogFilter);
            topPanel.Controls.Add(_btnLogClear);
            topPanel.Controls.Add(_btnLogCopy);
            topPanel.Controls.Add(_btnLogExport);

            _dtLogs = new DataTable("ActivityLog");
            _dtLogs.Columns.Add("Time", typeof(string));
            _dtLogs.Columns.Add("Severity", typeof(string));
            _dtLogs.Columns.Add("Message", typeof(string));
            _dtLogs.Columns.Add("Package", typeof(string));

            _gridLogs = new BeepGridPro { Dock = DockStyle.Fill };
            _gridLogs.Columns.Add(new BeepColumnConfig { ColumnName = "Time", ColumnCaption = "Time", Width = 80 });
            _gridLogs.Columns.Add(new BeepColumnConfig { ColumnName = "Severity", ColumnCaption = "Severity", Width = 80 });
            _gridLogs.Columns.Add(new BeepColumnConfig { ColumnName = "Message", ColumnCaption = "Message", Width = 400 });
            _gridLogs.Columns.Add(new BeepColumnConfig { ColumnName = "Package", ColumnCaption = "Package", Width = 150 });
            _gridLogs.DataSource = _dtLogs;

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 30 };
            _lblLogStatus = new BeepLabel { Dock = DockStyle.Fill, Text = "Ready", TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            bottomPanel.Controls.Add(_lblLogStatus);

            tab.Controls.Add(_gridLogs);
            tab.Controls.Add(topPanel);
            tab.Controls.Add(bottomPanel);

            tab.Enter += (_, _) => RefreshLogs();
        }

        private void RefreshLogs()
        {
            if (_dtLogs == null || _gridLogs == null) return;

            _dtLogs.Rows.Clear();
            try
            {
                var filter = _cmbLogFilter?.SelectedItem?.Item?.ToString() ?? "All";
                var logs = GetService().Logs;

                if (filter != "All")
                {
                    logs = logs.Where(l => l.Severity.ToString() == filter).ToList();
                }

                foreach (var log in logs)
                {
                    _dtLogs.Rows.Add(
                        log.Timestamp.ToString("HH:mm:ss"),
                        log.Severity.ToString(),
                        log.Message,
                        log.PackageId ?? string.Empty);
                }

                if (_lblLogStatus != null) _lblLogStatus.Text = $"{logs.Count} entries";
            }
            catch (Exception ex)
            {
                if (_lblLogStatus != null) _lblLogStatus.Text = $"Refresh failed: {ex.Message}";
            }
        }

        private void CopyLogsToClipboard()
        {
            try
            {
                var text = string.Join(Environment.NewLine, GetService().Logs.Select(l => $"[{l.Timestamp:HH:mm:ss}] {l.Severity}: {l.Message}"));
                Clipboard.SetText(text);
                if (_lblLogStatus != null) _lblLogStatus.Text = "Copied to clipboard.";
            }
            catch (Exception ex)
            {
                if (_lblLogStatus != null) _lblLogStatus.Text = $"Copy failed: {ex.Message}";
            }
        }

        private void ExportLogs()
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "Log files (*.log)|*.log|JSON files (*.json)|*.json|All files (*.*)|*.*",
                FileName = $"NuggetsActivity_{DateTime.Now:yyyyMMdd_HHmmss}.log"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                var text = string.Join(Environment.NewLine, GetService().Logs.Select(l => $"[{l.Timestamp:yyyy-MM-dd HH:mm:ss}] {l.Severity}: {l.Message}"));
                System.IO.File.WriteAllText(dialog.FileName, text);
                if (_lblLogStatus != null) _lblLogStatus.Text = $"Exported to {dialog.FileName}";
            }
            catch (Exception ex)
            {
                if (_lblLogStatus != null) _lblLogStatus.Text = $"Export failed: {ex.Message}";
            }
        }
    }
}
