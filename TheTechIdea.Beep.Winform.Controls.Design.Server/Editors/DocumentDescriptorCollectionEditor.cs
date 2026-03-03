// DocumentDescriptorCollectionEditor.cs
// Sprint 17.2 — Full custom grid editor for BeepDocumentHost.DesignTimeDocuments.
// Features: DataGridView with icon picker, colour picker, up/down reorder,
//           Add/Remove, and a live mini tab-strip preview.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    // ─────────────────────────────────────────────────────────────────────────
    // CollectionEditor entry point (used by Properties grid via [Editor] attr)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Design-time collection editor for <see cref="DocumentDescriptor"/>.
    /// Launches a fully custom form with DataGridView, icon picker, colour
    /// picker, up/down reorder, and a live mini tab-strip preview.
    /// </summary>
    public class DocumentDescriptorCollectionEditor : UITypeEditor
    {
        public DocumentDescriptorCollectionEditor() { }
        public DocumentDescriptorCollectionEditor(Type type) { }  // kept for [Editor] attribute compatibility

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            => UITypeEditorEditStyle.Modal;

        /// <summary>
        /// Shows the custom <see cref="DocumentDescriptorEditorForm"/> instead of
        /// the standard collection editor grid.
        /// </summary>
        public override object? EditValue(
            ITypeDescriptorContext? context,
            IServiceProvider? provider,
            object? value)
        {
            var wfSvc = provider?.GetService(typeof(System.Windows.Forms.Design.IWindowsFormsEditorService))
                        as System.Windows.Forms.Design.IWindowsFormsEditorService;

            var working = ExtractDescriptors(value);

            using var form = new DocumentDescriptorEditorForm(working);

            if (wfSvc != null)
                wfSvc.ShowDialog(form);
            else
                form.ShowDialog();

            if (form.DialogResult == DialogResult.OK)
                return PushBack(value, form.Result);

            return value;
        }

        // ── helpers ──────────────────────────────────────────────────────────

        internal static List<DocumentDescriptor> ExtractDescriptors(object? value)
        {
            if (value is IEnumerable<DocumentDescriptor> typed)
                return typed.Select(Clone).ToList();
            if (value is System.Collections.IEnumerable raw)
                return raw.OfType<DocumentDescriptor>().Select(Clone).ToList();
            return new List<DocumentDescriptor>();
        }

        internal static object PushBackPublic(object? original, IReadOnlyList<DocumentDescriptor> edited)
            => DocumentDescriptorCollectionEditorHelper.PushBack(original, edited);

        private static object PushBack(object? original, IReadOnlyList<DocumentDescriptor> edited)
            => DocumentDescriptorCollectionEditorHelper.PushBack(original, edited);

        internal static DocumentDescriptor Clone(DocumentDescriptor src) => new DocumentDescriptor
        {
            Id             = src.Id,
            Title          = src.Title,
            IconPath       = src.IconPath,
            IsPinned       = src.IsPinned,
            CanClose       = src.CanClose,
            AccentColor    = src.AccentColor,
            InitialContent = src.InitialContent,
            TooltipText    = src.TooltipText,
            IsModified     = src.IsModified,
        };
    }

    // ─────────────────────────────────────────────────────────────────────────
    // UITypeEditor wrapper — used from smart-tag via ActionList
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// <see cref="System.Drawing.Design.UITypeEditor"/> that launches the
    /// <see cref="DocumentDescriptorEditorForm"/> from the Properties grid or
    /// smart-tag.
    /// </summary>
    public class DesignTimeDocumentsEditor : UITypeEditor
    {
        public DesignTimeDocumentsEditor() { }
        public DesignTimeDocumentsEditor(Type type) { }  // kept for [Editor] attribute compatibility

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            => UITypeEditorEditStyle.Modal;

        public override object? EditValue(
            ITypeDescriptorContext? context,
            IServiceProvider? provider,
            object? value)
        {
            var wfSvc = provider?.GetService(typeof(System.Windows.Forms.Design.IWindowsFormsEditorService))
                        as System.Windows.Forms.Design.IWindowsFormsEditorService;

            var working = DocumentDescriptorCollectionEditor.ExtractDescriptors(value);

            using var form = new DocumentDescriptorEditorForm(working);

            if (wfSvc != null)
                wfSvc.ShowDialog(form);
            else
                form.ShowDialog();

            if (form.DialogResult == DialogResult.OK)
                return DocumentDescriptorCollectionEditor.PushBackPublic(value, form.Result);

            return value;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Custom editor form
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Fully custom form for editing a collection of <see cref="DocumentDescriptor"/>
    /// items at design time.
    /// </summary>
    internal sealed class DocumentDescriptorEditorForm : Form
    {
        // ── public result ─────────────────────────────────────────────────────

        /// <summary>The edited list. Valid when <c>DialogResult == OK</c>.</summary>
        public IReadOnlyList<DocumentDescriptor> Result => _items.AsReadOnly();

        // ── internal state ────────────────────────────────────────────────────

        private readonly List<DocumentDescriptor> _items;

        // ── UI controls ───────────────────────────────────────────────────────

        private readonly DataGridView _grid;
        private readonly Button       _btnAdd;
        private readonly Button       _btnRemove;
        private readonly Button       _btnUp;
        private readonly Button       _btnDown;
        private readonly Button       _btnOk;
        private readonly Button       _btnCancel;
        private readonly Panel        _previewPanel;
        private readonly Label        _previewLabel;

        // ── column indices ────────────────────────────────────────────────────

        private const int ColId       = 0;
        private const int ColTitle    = 1;
        private const int ColIcon     = 2;   // text + "…" button
        private const int ColPinned   = 3;
        private const int ColClose    = 4;
        private const int ColContent  = 5;
        private const int ColAccent   = 6;   // color swatch button

        // ─────────────────────────────────────────────────────────────────────

        public DocumentDescriptorEditorForm(IEnumerable<DocumentDescriptor> items)
        {
            _items = items.ToList();

            Text            = "Edit Design-Time Documents";
            Size            = new Size(860, 600);
            MinimumSize     = new Size(680, 480);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox     = true;
            Font            = new Font("Segoe UI", 9f);
            BackColor       = SystemColors.Window;

            // ── tool strip ────────────────────────────────────────────────────
            var toolPanel = new Panel
            {
                Dock   = DockStyle.Top,
                Height = 34,
                BackColor = SystemColors.ControlLight,
                Padding = new Padding(4, 4, 4, 0)
            };

            _btnAdd    = SmallButton("+ Add",    "Adds a new document descriptor.");
            _btnRemove = SmallButton("− Remove", "Removes the selected descriptor.");
            _btnUp     = SmallButton("▲ Up",     "Moves the selected row up.");
            _btnDown   = SmallButton("▼ Down",   "Moves the selected row down.");

            _btnAdd.Click    += BtnAdd_Click;
            _btnRemove.Click += BtnRemove_Click;
            _btnUp.Click     += BtnUp_Click;
            _btnDown.Click   += BtnDown_Click;

            int x = 4;
            foreach (var b in new[] { _btnAdd, _btnRemove, _btnUp, _btnDown })
            {
                b.Location = new Point(x, 4);
                toolPanel.Controls.Add(b);
                x += b.Width + 4;
            }

            // ── grid ──────────────────────────────────────────────────────────
            _grid = new DataGridView
            {
                Dock             = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode    = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect      = false,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = true,
                BorderStyle      = BorderStyle.None,
                BackgroundColor  = SystemColors.Window,
                GridColor        = SystemColors.ControlLight,
                ColumnHeadersDefaultCellStyle = { BackColor = SystemColors.ControlLight, Font = new Font("Segoe UI", 9f) }
            };
            BuildColumns();
            _grid.CellClick           += Grid_CellClick;
            _grid.SelectionChanged    += Grid_SelectionChanged;
            _grid.CellEndEdit         += Grid_CellEndEdit;
            _grid.RowEnter            += (_, __) => RefreshPreview();

            // ── preview panel ─────────────────────────────────────────────────
            _previewLabel = new Label
            {
                Text     = "Preview:",
                Dock     = DockStyle.Top,
                Height   = 18,
                Padding  = new Padding(4, 2, 0, 0),
                Font     = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = SystemColors.GrayText,
                BackColor = SystemColors.ControlLight
            };

            _previewPanel = new Panel
            {
                Height    = 32,
                Dock      = DockStyle.Bottom,
                BackColor = Color.FromArgb(40, 40, 40)
            };
            _previewPanel.Paint += PreviewPanel_Paint;

            var bottomBar = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = SystemColors.Control };
            _btnOk     = new Button { Text = "OK",     DialogResult = DialogResult.OK,     Size = new Size(80, 28), Anchor = AnchorStyles.Right | AnchorStyles.Bottom };
            _btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel,  Size = new Size(80, 28), Anchor = AnchorStyles.Right | AnchorStyles.Bottom };
            _btnOk.Location     = new Point(bottomBar.Width - 176, 11);
            _btnCancel.Location = new Point(bottomBar.Width - 92,  11);
            _btnOk.Anchor     = AnchorStyles.Right | AnchorStyles.Top;
            _btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            bottomBar.Controls.Add(_btnOk);
            bottomBar.Controls.Add(_btnCancel);
            bottomBar.Resize += (_, __) =>
            {
                _btnOk.Location     = new Point(bottomBar.Width - 176, 11);
                _btnCancel.Location = new Point(bottomBar.Width - 92,  11);
            };

            AcceptButton = _btnOk;
            CancelButton = _btnCancel;

            // ── layout ────────────────────────────────────────────────────────
            var center = new Panel { Dock = DockStyle.Fill };
            center.Controls.Add(_grid);

            Controls.Add(center);
            Controls.Add(_previewLabel);
            Controls.Add(_previewPanel);
            Controls.Add(bottomBar);
            Controls.Add(toolPanel);

            LoadGrid();
        }

        // ── column building ───────────────────────────────────────────────────

        private void BuildColumns()
        {
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId", HeaderText = "Id", DataPropertyName = "Id",
                Width = 120, MinimumWidth = 60,
                ToolTipText = "Unique identifier for this document."
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTitle", HeaderText = "Title", DataPropertyName = "Title",
                Width = 140, MinimumWidth = 80, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ToolTipText = "Text displayed on the tab."
            });

            // IconPath — text column + "…" button column
            var colIcon = new DataGridViewButtonColumn
            {
                Name = "colIcon", HeaderText = "Icon Path",
                Width = 160, MinimumWidth = 80,
                UseColumnTextForButtonValue = false,
                ToolTipText = "Click '…' to open the icon picker."
            };
            _grid.Columns.Add(colIcon);

            _grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colPinned", HeaderText = "Pinned",
                Width = 54, MinimumWidth = 50,
                ToolTipText = "When checked, the tab is pinned (icon-only)."
            });

            _grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colClose", HeaderText = "Can Close",
                Width = 68, MinimumWidth = 50,
                ToolTipText = "When unchecked, the close button is hidden."
            });

            _grid.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "colContent", HeaderText = "Content",
                Width = 100, MinimumWidth = 70,
                DataSource = Enum.GetValues(typeof(DocumentInitialContent)),
                ToolTipText = "Placeholder content to show at design/startup time."
            });

            var colAccent = new DataGridViewButtonColumn
            {
                Name = "colAccent", HeaderText = "Accent",
                Width = 62, MinimumWidth = 54,
                UseColumnTextForButtonValue = false,
                ToolTipText = "Click to choose an accent colour for this tab."
            };
            _grid.Columns.Add(colAccent);
        }

        // ── grid data ─────────────────────────────────────────────────────────

        private void LoadGrid()
        {
            _grid.Rows.Clear();
            foreach (var d in _items)
                AddRow(d);
            if (_grid.Rows.Count > 0)
                _grid.CurrentCell = _grid.Rows[0].Cells[ColTitle];
        }

        private void AddRow(DocumentDescriptor d)
        {
            int idx = _grid.Rows.Add();
            var row = _grid.Rows[idx];

            row.Tag = d;
            RefreshRow(row, d);
        }

        private void RefreshRow(DataGridViewRow row, DocumentDescriptor d)
        {
            // Id column (text)
            row.Cells[ColId].Value    = d.Id;
            row.Cells[ColTitle].Value = d.Title;

            // Icon button — show truncated path + "…" suffix
            var iconCell = (DataGridViewButtonCell)row.Cells[ColIcon];
            string iconLabel = string.IsNullOrEmpty(d.IconPath)
                ? "(none)  …"
                : System.IO.Path.GetFileName(d.IconPath) + "  …";
            iconCell.Value = iconLabel;
            iconCell.ToolTipText = d.IconPath ?? "(no icon)";

            row.Cells[ColPinned].Value  = d.IsPinned;
            row.Cells[ColClose].Value   = d.CanClose;
            row.Cells[ColContent].Value = d.InitialContent;

            // Accent button — paint background in AccentColor
            var accentCell = (DataGridViewButtonCell)row.Cells[ColAccent];
            accentCell.Value = d.AccentColor == Color.Empty ? "●" : "■";
            accentCell.Style.ForeColor = d.AccentColor == Color.Empty
                ? SystemColors.GrayText : d.AccentColor;
            accentCell.ToolTipText = d.AccentColor == Color.Empty
                ? "Default (theme PrimaryColor)" : ColorTranslator.ToHtml(d.AccentColor);
        }

        // ── grid events ───────────────────────────────────────────────────────

        private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _items.Count) return;

            var d = _items[e.RowIndex];

            if (e.ColumnIndex == ColIcon)
            {
                PickIcon(d, e.RowIndex);
            }
            else if (e.ColumnIndex == ColAccent)
            {
                PickAccentColor(d, e.RowIndex);
            }
        }

        private void Grid_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _items.Count) return;

            var d   = _items[e.RowIndex];
            var row = _grid.Rows[e.RowIndex];
            var val = row.Cells[e.ColumnIndex].Value;

            switch (e.ColumnIndex)
            {
                case ColId:      d.Id       = val?.ToString() ?? d.Id; break;
                case ColTitle:   d.Title    = val?.ToString() ?? d.Title; break;
                case ColPinned:  d.IsPinned = val is true; break;
                case ColClose:   d.CanClose = val is true; break;
                case ColContent:
                    if (val is DocumentInitialContent ic) d.InitialContent = ic;
                    break;
            }

            RefreshPreview();
        }

        private void Grid_SelectionChanged(object? sender, EventArgs e)
        {
            bool hasRow = CurrentRowIndex >= 0;
            _btnRemove.Enabled = hasRow;
            _btnUp.Enabled     = hasRow && CurrentRowIndex > 0;
            _btnDown.Enabled   = hasRow && CurrentRowIndex < _items.Count - 1;
        }

        // ── icon picker ───────────────────────────────────────────────────────

        private void PickIcon(DocumentDescriptor d, int rowIndex)
        {
            using var dlg = new IconPickerDialog(d.IconPath);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                d.IconPath = dlg.SelectedIconPath ?? string.Empty;
                RefreshRow(_grid.Rows[rowIndex], d);
                RefreshPreview();
            }
        }

        // ── accent colour picker ──────────────────────────────────────────────

        private void PickAccentColor(DocumentDescriptor d, int rowIndex)
        {
            using var dlg = new ColorDialog
            {
                Color          = d.AccentColor == Color.Empty ? SystemColors.Highlight : d.AccentColor,
                FullOpen       = true,
                AllowFullOpen  = true,
            };
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                d.AccentColor = dlg.Color;
                RefreshRow(_grid.Rows[rowIndex], d);
                RefreshPreview();
            }
        }

        // ── tool-strip button handlers ────────────────────────────────────────

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            int seed = _items.Count + 1;
            var d = new DocumentDescriptor
            {
                Id       = "doc-" + Guid.NewGuid().ToString("N")[..8],
                Title    = $"Document {seed}",
                CanClose = true
            };
            _items.Add(d);
            AddRow(d);
            _grid.ClearSelection();
            _grid.Rows[_grid.Rows.Count - 1].Selected = true;
            _grid.CurrentCell = _grid.Rows[_grid.Rows.Count - 1].Cells[ColTitle];
            RefreshPreview();
        }

        private void BtnRemove_Click(object? sender, EventArgs e)
        {
            int idx = CurrentRowIndex;
            if (idx < 0) return;
            _items.RemoveAt(idx);
            _grid.Rows.RemoveAt(idx);
            RefreshPreview();
            Grid_SelectionChanged(null, EventArgs.Empty);
        }

        private void BtnUp_Click(object? sender, EventArgs e)
        {
            int idx = CurrentRowIndex;
            if (idx <= 0) return;
            SwapItems(idx, idx - 1);
        }

        private void BtnDown_Click(object? sender, EventArgs e)
        {
            int idx = CurrentRowIndex;
            if (idx < 0 || idx >= _items.Count - 1) return;
            SwapItems(idx, idx + 1);
        }

        private void SwapItems(int a, int b)
        {
            (_items[a], _items[b]) = (_items[b], _items[a]);
            RefreshRow(_grid.Rows[a], _items[a]);
            RefreshRow(_grid.Rows[b], _items[b]);
            _grid.ClearSelection();
            _grid.Rows[b].Selected = true;
            _grid.CurrentCell = _grid.Rows[b].Cells[ColTitle];
            RefreshPreview();
        }

        private int CurrentRowIndex =>
            _grid.SelectedRows.Count > 0 ? _grid.SelectedRows[0].Index : -1;

        // ── mini tab preview ──────────────────────────────────────────────────

        private void RefreshPreview() => _previewPanel.Invalidate();

        private void PreviewPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g   = e.Graphics;
            var rc  = _previewPanel.ClientRectangle;

            using var bg = new SolidBrush(Color.FromArgb(45, 45, 48));
            g.FillRectangle(bg, rc);

            if (_items.Count == 0) return;

            float x     = 4f;
            float tabH  = rc.Height - 2f;
            float maxW  = (rc.Width - 4f) / Math.Max(1, _items.Count);
            float tabW  = Math.Min(130f, maxW);

            using var regularFont = new Font("Segoe UI", 8f);
            using var fgBrush     = new SolidBrush(Color.FromArgb(220, 220, 220));
            using var activeBrush = new SolidBrush(Color.White);
            using var tabBg       = new SolidBrush(Color.FromArgb(62, 62, 64));
            using var activeBg    = new SolidBrush(Color.FromArgb(28, 151, 234));

            int selIdx = CurrentRowIndex;

            for (int i = 0; i < _items.Count && x + tabW <= rc.Width + 2; i++)
            {
                var d      = _items[i];
                bool active = i == selIdx;
                var tabRc   = new RectangleF(x, 1, tabW - 2, tabH - 1);

                g.FillRectangle(active ? activeBg : tabBg, tabRc);

                // Accent indicator
                if (d.AccentColor != Color.Empty)
                {
                    using var accentBrush = new SolidBrush(d.AccentColor);
                    g.FillRectangle(accentBrush, tabRc.X, tabRc.Bottom - 3, tabRc.Width, 3);
                }

                string label = d.IsPinned ? "📌"
                             : (d.Title.Length > 14 ? d.Title[..13] + "…" : d.Title);

                var sf = new StringFormat
                {
                    Alignment     = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming      = StringTrimming.EllipsisCharacter,
                    FormatFlags   = StringFormatFlags.NoWrap
                };
                g.DrawString(label, regularFont, active ? activeBrush : fgBrush, tabRc, sf);

                x += tabW;
            }
        }

        // ── helpers ───────────────────────────────────────────────────────────

        private static Button SmallButton(string text, string tooltip)
        {
            var btn = new Button
            {
                Text      = text,
                Size      = new Size(76, 24),
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 8.5f)
            };
            btn.FlatAppearance.BorderColor = SystemColors.ControlDark;
            new ToolTip().SetToolTip(btn, tooltip);
            return btn;
        }

    }

    // ─────────────────────────────────────────────────────────────────────────
    // Shim — expose PushBack publicly so DesignTimeDocumentsEditor can use it
    // ─────────────────────────────────────────────────────────────────────────

    public static class DocumentDescriptorCollectionEditorHelper
    {
        public static object PushBack(object? original, IReadOnlyList<DocumentDescriptor> edited)
        {
            if (original is System.Collections.ObjectModel.Collection<DocumentDescriptor> col)
            {
                col.Clear();
                foreach (var d in edited) col.Add(d);
                return col;
            }
            if (original is List<DocumentDescriptor> lst)
            {
                lst.Clear();
                lst.AddRange(edited);
                return lst;
            }
            var newCol = new System.Collections.ObjectModel.Collection<DocumentDescriptor>();
            foreach (var d in edited) newCol.Add(d);
            return newCol;
        }
    }
}
