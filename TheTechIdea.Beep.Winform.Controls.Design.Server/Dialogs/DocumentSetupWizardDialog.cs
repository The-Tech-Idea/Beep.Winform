// DocumentSetupWizardDialog.cs
// Phase 07 — commercial-grade design-time setup experience.
//
// One dialog, one click, fully-wired document area. Modeled on commercial MDI
// platform setup wizards (DevExpress XtraTabbedMdiManager, Telerik
// RadDocking) where dropping a single component opens a guided setup with
// visual mode tiles, optional starter templates, a live preview, and a
// single "Apply" that auto-creates and wires every companion component.
//
// Used by:
//   • BeepDocumentManagerDesigner.InitializeNewComponent (first drop)
//   • BeepDocumentManagerDesigner.ShowSetupWizard         (re-run any time)
//   • BeepDocumentHostDesigner.InitializeNewComponent     (host-only flavour)
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs
{
    /// <summary>Top-level document presentation modes the wizard exposes.</summary>
    public enum DocumentSetupMode
    {
        /// <summary>BeepTabbedView + BeepDocumentHost — IDE / editor style with tabs, splits, docking.</summary>
        TabbedDocuments,
        /// <summary>BeepTabbedView + BeepDocumentHost configured Chrome-style with new-tab button.</summary>
        BrowserTabs,
        /// <summary>BeepNativeMdiView — classic WinForms MDI children, no tabs/splits/docking.</summary>
        NativeMdi
    }

    /// <summary>Result of the setup wizard, consumed by the designer.</summary>
    public sealed class DocumentSetupResult
    {
        public DocumentSetupMode Mode { get; set; } = DocumentSetupMode.TabbedDocuments;
        public bool AddSampleDocuments { get; set; }
        public int SampleDocumentCount { get; set; } = 3;
        public bool ApplyTemplate { get; set; }
        public string TemplateId { get; set; } = "Empty";
        public bool ConfigureLater { get; set; }

        /// <summary>
        /// When the form has more than one BeepDocumentHost, the user picks one
        /// here. Null = let the designer auto-resolve (single host) or warn the
        /// user to wire it manually (zero hosts).
        /// </summary>
        public object? SelectedHostComponent { get; set; }

        /// <summary>
        /// When true, the designer should persist the user's "Don't show the
        /// setup wizard automatically" preference so future drops skip the
        /// wizard.
        /// </summary>
        public bool DoNotShowAgain { get; set; }
    }

    /// <summary>
    /// Tagged option used by the host picker combo. Designer passes one entry
    /// per BeepDocumentHost found on the design surface.
    /// </summary>
    public sealed class HostPickerOption
    {
        public string DisplayName { get; }
        public object HostComponent { get; }
        public HostPickerOption(string displayName, object hostComponent)
        {
            DisplayName   = displayName;
            HostComponent = hostComponent;
        }
        public override string ToString() => DisplayName;
    }

    /// <summary>
    /// Modal setup wizard. Re-entrant: pass the existing mode to highlight the
    /// current selection so the user can switch modes without surprises.
    /// </summary>
    public sealed class DocumentSetupWizardDialog : Form
    {
        public DocumentSetupResult Result { get; private set; } = new DocumentSetupResult();

        // ── UI state ──────────────────────────────────────────────────────
        private ModeTile? _selectedTile;
        private readonly List<ModeTile> _tiles = new List<ModeTile>();
        private readonly CheckBox _addSamplesCheck;
        private readonly NumericUpDown _sampleCount;
        private readonly CheckBox _applyTemplateCheck;
        private readonly ComboBox _templateCombo;
        private readonly PreviewPanel _preview;
        private readonly AnnouncingLabel _statusLabel;
        private readonly CheckBox _dontShowAgainCheck;
        private readonly ComboBox? _hostPicker;
        private readonly Label? _hostPickerLabel;
        private readonly int _existingDocumentCount;

        // ── Palette (Phase 07 polish) ────────────────────────────────────
        // Sampled once at construction time from BeepThemesManager so the
        // wizard inherits whichever Beep theme the user has selected. Falls
        // back to SystemColors only when the theme system is unavailable or
        // when OS High Contrast is on. No mid-dialog hot-swap — matches
        // commercial wizards (DevExpress Template Gallery, VS New Project).
        private readonly WizardPalette _palette = new WizardPalette();

        public DocumentSetupWizardDialog(DocumentSetupMode initialMode = DocumentSetupMode.TabbedDocuments)
            : this(initialMode, existingDocumentCount: 0, hostOptions: null) { }

        /// <summary>
        /// Full constructor. Pass the current document count so the wizard can
        /// disable "Add N sample tabs" when the host already has documents,
        /// and pass the available BeepDocumentHost options so the user can
        /// pick which one to wire when there is ambiguity.
        /// </summary>
        public DocumentSetupWizardDialog(
            DocumentSetupMode initialMode,
            int existingDocumentCount,
            IReadOnlyList<HostPickerOption>? hostOptions)
        {
            _existingDocumentCount = Math.Max(0, existingDocumentCount);
            bool needsHostPicker = hostOptions != null && hostOptions.Count > 1;

            Text            = "Beep Document Area — Setup";
            Size            = new Size(820, needsHostPicker ? 660 : 620);
            MinimumSize     = new Size(780, 580);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox     = false;
            MaximizeBox     = false;
            BackColor       = _palette.FormBack;
            Font            = new Font("Segoe UI", 9f);

            // Phase 07 polish — accessibility:
            // expose a clear screen-reader name + role for the modal itself so
            // assistive tech announces "Beep Document Area Setup — dialog".
            AccessibleName        = "Beep Document Area Setup Wizard";
            AccessibleDescription = "Choose how this document area displays documents, optionally seed sample tabs, and pick a layout template.";
            AccessibleRole        = AccessibleRole.Dialog;

            // ── Header ────────────────────────────────────────────────────
            var header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = _palette.FormBack };
            var headerTitle = new Label
            {
                Text     = "How should this document area display documents?",
                Dock     = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding  = new Padding(20, 0, 20, 0),
                Font     = new Font("Segoe UI Semibold", 13f),
                ForeColor = _palette.HeadingFore,
                AccessibleName = "Setup wizard heading",
                AccessibleRole = AccessibleRole.StaticText
            };
            var headerSep = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = _palette.Separator };
            header.Controls.Add(headerTitle);
            header.Controls.Add(headerSep);

            // ── Tiles row ─────────────────────────────────────────────────
            var tilesContainer = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 200,
                BackColor = _palette.FormBack,
                Padding   = new Padding(20, 16, 20, 8),
                AccessibleName = "Display mode tiles",
                AccessibleRole = AccessibleRole.Grouping
            };

            _tiles.Add(CreateTile(
                DocumentSetupMode.TabbedDocuments,
                "Tabbed Documents",
                "IDE / editor style. Tabs, side-by-side splits,\nfloating windows, pinned tabs, themes.",
                isRecommended: true,
                iconPainter: PaintTabbedIcon));

            _tiles.Add(CreateTile(
                DocumentSetupMode.BrowserTabs,
                "Browser Tabs",
                "Chrome-style tabs with a new-tab button and\nalways-visible close buttons.",
                isRecommended: false,
                iconPainter: PaintBrowserIcon));

            _tiles.Add(CreateTile(
                DocumentSetupMode.NativeMdi,
                "Native MDI",
                "Classic WinForms MDI children. No tabs,\nsplits, or docking. For legacy MDI apps.",
                isRecommended: false,
                iconPainter: PaintMdiIcon));

            // place tiles in a horizontal flow with even spacing
            int tileWidth  = 240;
            int tileHeight = 170;
            int gap        = 16;
            int totalW     = tileWidth * 3 + gap * 2;
            int startX     = (tilesContainer.ClientSize.Width - totalW) / 2;
            for (int i = 0; i < _tiles.Count; i++)
            {
                var t = _tiles[i];
                t.Size = new Size(tileWidth, tileHeight);
                t.Location = new Point(20 + i * (tileWidth + gap), 12);
                t.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                tilesContainer.Controls.Add(t);
            }
            tilesContainer.Resize += (s, e) =>
            {
                int sx = Math.Max(20, (tilesContainer.ClientSize.Width - (tileWidth * 3 + gap * 2)) / 2);
                for (int i = 0; i < _tiles.Count; i++)
                {
                    _tiles[i].Location = new Point(sx + i * (tileWidth + gap), 12);
                }
            };

            // ── Options strip (starter content) ───────────────────────────
            var options = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 68,
                BackColor = _palette.FormSurface,
                Padding   = new Padding(20, 4, 20, 4),
                AccessibleName = "Starter content options",
                AccessibleRole = AccessibleRole.Grouping
            };

            var optionsHeader = new Label
            {
                Text       = "Starter content (optional)",
                Location   = new Point(20, 4),
                AutoSize   = true,
                Font       = new Font("Segoe UI Semibold", 9.5f),
                ForeColor  = _palette.HeadingFore
            };

            // Sample-docs checkbox is suppressed when the host already has documents
            // so the wizard doesn't pile extra tabs on top of existing user data.
            bool hasExistingDocs = _existingDocumentCount > 0;

            _addSamplesCheck = new CheckBox
            {
                Text     = "Add",
                Location = new Point(22, 32),
                AutoSize = true,
                Checked  = !hasExistingDocs,
                Enabled  = !hasExistingDocs,
                Font     = new Font("Segoe UI", 9f),
                ForeColor = _palette.BodyFore,
                AccessibleName = "Add sample document tabs"
            };

            _sampleCount = new NumericUpDown
            {
                Minimum   = 1,
                Maximum   = 12,
                Value     = 3,
                Location  = new Point(72, 30),
                Width     = 50,
                Font      = new Font("Segoe UI", 9f),
                AccessibleName = "Number of sample documents to add"
            };

            var sampleSuffix = new Label
            {
                Text     = hasExistingDocs
                    ? $"sample document tabs   (skipped — {_existingDocumentCount} doc{(_existingDocumentCount == 1 ? "" : "s")} already present)"
                    : "sample document tabs to get started",
                Location = new Point(128, 32),
                AutoSize = true,
                Font     = new Font("Segoe UI", 9f),
                ForeColor = hasExistingDocs ? _palette.WarningFore : _palette.MutedFore
            };
            _sampleCount.Enabled = !hasExistingDocs;

            _applyTemplateCheck = new CheckBox
            {
                Text     = "Apply layout template:",
                Location = new Point(360, 32),
                AutoSize = true,
                Checked  = false,
                Font     = new Font("Segoe UI", 9f),
                ForeColor = _palette.BodyFore,
                AccessibleName = "Apply a starter layout template"
            };

            _templateCombo = new ComboBox
            {
                Location      = new Point(495, 29),
                Width         = 180,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9f),
                AccessibleName = "Layout template selector"
            };
            _templateCombo.Items.AddRange(new object[]
            {
                "Empty",
                "Visual Studio (3 panes)",
                "Two-pane editor",
                "Browser (single tab)"
            });
            _templateCombo.SelectedIndex = 0;
            _templateCombo.Enabled = false;
            _applyTemplateCheck.CheckedChanged += (s, e) => _templateCombo.Enabled = _applyTemplateCheck.Checked;

            options.Controls.Add(optionsHeader);
            options.Controls.Add(_addSamplesCheck);
            options.Controls.Add(_sampleCount);
            options.Controls.Add(sampleSuffix);
            options.Controls.Add(_applyTemplateCheck);
            options.Controls.Add(_templateCombo);

            // ── Host picker strip (only shown when there are 2+ hosts on the form) ──
            Panel? hostPickerStrip = null;
            if (needsHostPicker)
            {
                hostPickerStrip = new Panel
                {
                    Dock      = DockStyle.Top,
                    Height    = 44,
                    BackColor = _palette.WarningBack,
                    Padding   = new Padding(20, 8, 20, 0),
                    AccessibleName = "Host picker",
                    AccessibleDescription = "Select which BeepDocumentHost control the wizard should wire."
                };
                _hostPickerLabel = new Label
                {
                    Text      = "Wire to host:",
                    Location  = new Point(20, 12),
                    AutoSize  = true,
                    Font      = new Font("Segoe UI Semibold", 9f),
                    ForeColor = _palette.WarningFore
                };
                _hostPicker = new ComboBox
                {
                    Location      = new Point(110, 9),
                    Width         = 260,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font          = new Font("Segoe UI", 9f),
                    AccessibleName = "Wire to host selector"
                };
                foreach (var opt in hostOptions!)
                {
                    _hostPicker.Items.Add(opt);
                }
                _hostPicker.SelectedIndex = 0;

                var hostPickerHint = new Label
                {
                    Text      = "Multiple BeepDocumentHost controls were found — pick one.",
                    Location  = new Point(380, 12),
                    AutoSize  = true,
                    Font      = new Font("Segoe UI", 8.5f),
                    ForeColor = _palette.WarningFore
                };
                hostPickerStrip.Controls.Add(_hostPickerLabel);
                hostPickerStrip.Controls.Add(_hostPicker);
                hostPickerStrip.Controls.Add(hostPickerHint);
            }

            // ── Preview ────────────────────────────────────────────────────
            var previewLabel = new Label
            {
                Text      = "Preview",
                Dock      = DockStyle.Top,
                Height    = 22,
                Padding   = new Padding(20, 4, 20, 0),
                Font      = new Font("Segoe UI Semibold", 9.5f),
                ForeColor = _palette.HeadingFore
            };

            var previewContainer = new Panel
            {
                Dock      = DockStyle.Fill,
                Padding   = new Padding(20, 0, 20, 8),
                BackColor = _palette.FormSurface
            };

            _preview = new PreviewPanel(_palette)
            {
                Dock = DockStyle.Fill,
                AccessibleName = "Live preview of the selected display mode",
                AccessibleRole = AccessibleRole.Graphic
            };
            previewContainer.Controls.Add(_preview);

            // ── Footer (buttons + status) ──────────────────────────────────
            var footer = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 68,
                BackColor = _palette.FooterBack
            };
            var footerSep = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = _palette.Separator };

            _statusLabel = new AnnouncingLabel
            {
                Text      = "",
                Location  = new Point(20, 8),
                Width     = 420,
                Height    = 18,
                TextAlign = ContentAlignment.MiddleLeft,
                Font      = new Font("Segoe UI", 9f),
                ForeColor = _palette.MutedFore,
                AccessibleName = "Setup wizard status",
                AccessibleRole = AccessibleRole.StaticText
            };

            _dontShowAgainCheck = new CheckBox
            {
                Text     = "Don't show this wizard automatically next time",
                Location = new Point(20, 28),
                AutoSize = true,
                Font     = new Font("Segoe UI", 8.75f),
                ForeColor = _palette.MutedFore,
                AccessibleName = "Suppress automatic wizard on next drop"
            };

            var applyButton = new Button
            {
                Text         = "Apply && Close",
                DialogResult = DialogResult.OK,
                Width        = 130,
                Height       = 32,
                Location     = new Point(footer.ClientSize.Width - 280, 18),
                Anchor       = AnchorStyles.Right | AnchorStyles.Top,
                BackColor    = _palette.Accent,
                ForeColor    = _palette.AccentFore,
                FlatStyle    = FlatStyle.Flat,
                Font         = new Font("Segoe UI Semibold", 9.5f),
                AccessibleName = "Apply setup and close wizard"
            };
            applyButton.FlatAppearance.BorderSize = 0;
            applyButton.Click += (s, e) => CaptureResult(configureLater: false);

            var laterButton = new Button
            {
                Text         = "Configure Later",
                DialogResult = DialogResult.Cancel,
                Width        = 130,
                Height       = 32,
                Location     = new Point(footer.ClientSize.Width - 140, 18),
                Anchor       = AnchorStyles.Right | AnchorStyles.Top,
                BackColor    = _palette.FormSurface,
                ForeColor    = _palette.BodyFore,
                FlatStyle    = FlatStyle.Flat,
                Font         = new Font("Segoe UI", 9f),
                AccessibleName = "Skip setup and configure later"
            };
            laterButton.FlatAppearance.BorderColor = _palette.Separator;
            laterButton.Click += (s, e) => CaptureResult(configureLater: true);

            footer.Resize += (s, e) =>
            {
                applyButton.Location = new Point(footer.ClientSize.Width - 280, 18);
                laterButton.Location = new Point(footer.ClientSize.Width - 140, 18);
            };

            footer.Controls.Add(_statusLabel);
            footer.Controls.Add(_dontShowAgainCheck);
            footer.Controls.Add(applyButton);
            footer.Controls.Add(laterButton);
            footer.Controls.Add(footerSep);

            AcceptButton = applyButton;
            CancelButton = laterButton;

            // ── Dock order matters: last-added Top is closest to the top edge.
            //    Visual layout (top → bottom):
            //      header → tilesContainer → [hostPickerStrip]? → options → previewLabel → previewContainer (Fill) → footer
            Controls.Add(previewContainer);
            Controls.Add(previewLabel);
            Controls.Add(options);
            if (hostPickerStrip != null) Controls.Add(hostPickerStrip);
            Controls.Add(tilesContainer);
            Controls.Add(header);
            Controls.Add(footer);

            // Pick initial tile
            SelectTile(_tiles.FirstOrDefault(t => t.Mode == initialMode) ?? _tiles[0]);
        }

        // ── Result capture ────────────────────────────────────────────────

        private void CaptureResult(bool configureLater)
        {
            object? selectedHost = null;
            if (_hostPicker?.SelectedItem is HostPickerOption opt)
                selectedHost = opt.HostComponent;

            Result = new DocumentSetupResult
            {
                Mode                  = _selectedTile?.Mode ?? DocumentSetupMode.TabbedDocuments,
                AddSampleDocuments    = !configureLater && _addSamplesCheck.Checked && _addSamplesCheck.Enabled,
                SampleDocumentCount   = (int)_sampleCount.Value,
                ApplyTemplate         = !configureLater && _applyTemplateCheck.Checked,
                TemplateId            = _templateCombo.SelectedItem?.ToString() ?? "Empty",
                ConfigureLater        = configureLater,
                SelectedHostComponent = selectedHost,
                DoNotShowAgain        = _dontShowAgainCheck.Checked
            };
        }

        // ── Tile factory ──────────────────────────────────────────────────

        private ModeTile CreateTile(DocumentSetupMode mode, string title, string description, bool isRecommended, Action<Graphics, Rectangle> iconPainter)
        {
            var tile = new ModeTile
            {
                Mode            = mode,
                TileTitle       = title,
                Description     = description,
                IsRecommended   = isRecommended,
                IconPainter     = iconPainter,
                AccentColor     = _palette.Accent,
                AccentFore      = _palette.AccentFore,
                BorderColor     = _palette.TileBorder,
                HoverColor      = _palette.TileHover,
                SelectedColor   = _palette.TileSelected,
                BackgroundColor = _palette.TileBack,
                TitleTextColor  = _palette.TileTitleFore,
                MutedTextColor  = _palette.TileDescFore,
                // Phase 07 polish — accessibility:
                // expose a meaningful name + role per tile and append
                // "(recommended)" so screen readers announce the suggested
                // choice without us needing an explicit live region.
                AccessibleName        = title + (isRecommended ? " (recommended)" : string.Empty),
                AccessibleDescription = description.Replace("\n", " "),
                AccessibleRole        = AccessibleRole.RadioButton
            };
            tile.Selected += (s, e) => SelectTile(tile);
            return tile;
        }

        private void SelectTile(ModeTile tile)
        {
            foreach (var t in _tiles)
            {
                bool wasSelected = t.IsSelected;
                t.IsSelected = ReferenceEquals(t, tile);

                // Phase 08 polish — accessibility (P8-032):
                // raise UIA SelectionItemPattern.IsSelectedProperty so that
                // screen readers announce "<tile name> selected" instead of
                // having to re-read the whole tile. We also nudge StateChange
                // so legacy AT bridges (Narrator's pre-UIA path, JAWS) pick
                // up the change.
                if (wasSelected != t.IsSelected)
                {
                    t.RaiseSelectionAccessibilityEvent();
                }
            }
            _selectedTile = tile;

            _preview.RenderMode = tile.Mode;
            _preview.Invalidate();

            // Status text doubles as a live-region announcement: it is
            // already AccessibleRole.StaticText with an AccessibleName, and
            // re-assigning .Text fires a UIA TextChanged event that screen
            // readers pick up. We prefix with the selected mode so the
            // announcement is self-contained without focus context.
            _statusLabel.Text = tile.Mode switch
            {
                DocumentSetupMode.TabbedDocuments => "Tabbed Documents selected. Creates BeepTabbedView + BeepDocumentHost on the form.",
                DocumentSetupMode.BrowserTabs     => "Browser Tabs selected. Creates BeepTabbedView + BeepDocumentHost (Chrome style, +button, always-close).",
                DocumentSetupMode.NativeMdi       => "Native MDI selected. Creates BeepNativeMdiView and sets Form.IsMdiContainer = true.",
                _ => string.Empty
            };
            _statusLabel.RaiseAnnouncement();

            // Sample-document option doesn't fit Native MDI as cleanly
            _addSamplesCheck.Enabled = tile.Mode != DocumentSetupMode.NativeMdi;
            if (!_addSamplesCheck.Enabled) _addSamplesCheck.Checked = false;
        }

        // ── Tile icon painters ────────────────────────────────────────────

        private static void PaintTabbedIcon(Graphics g, Rectangle r)
        {
            // Three tabs above a content area
            using var border = new Pen(Color.FromArgb(120, 120, 120), 1.2f);
            using var fill   = new SolidBrush(Color.FromArgb(70, 130, 180));
            using var bg     = new SolidBrush(Color.FromArgb(245, 247, 250));
            using var active = new SolidBrush(Color.White);

            int top = r.Y + 6;
            int tabH = 12;
            int tabW = (r.Width - 12) / 3;
            for (int i = 0; i < 3; i++)
            {
                var tabR = new Rectangle(r.X + 6 + i * tabW, top, tabW - 2, tabH);
                g.FillRectangle(i == 1 ? active : bg, tabR);
                g.DrawRectangle(border, tabR);
            }
            var content = new Rectangle(r.X + 6, top + tabH, r.Width - 12, r.Height - tabH - 12);
            g.FillRectangle(active, content);
            g.DrawRectangle(border, content);
            g.FillRectangle(fill, content.X + 6, content.Y + 6, content.Width - 12, 4);
            g.FillRectangle(fill, content.X + 6, content.Y + 14, (content.Width - 12) / 2, 3);
        }

        private static void PaintBrowserIcon(Graphics g, Rectangle r)
        {
            using var border  = new Pen(Color.FromArgb(120, 120, 120), 1.2f);
            using var active  = new SolidBrush(Color.White);
            using var bg      = new SolidBrush(Color.FromArgb(225, 228, 232));
            using var addBg   = new SolidBrush(Color.FromArgb(245, 247, 250));
            using var fg      = new SolidBrush(Color.FromArgb(80, 80, 80));

            int top  = r.Y + 6;
            int tabH = 12;
            int tabW = (r.Width - 28) / 3;
            for (int i = 0; i < 3; i++)
            {
                using var path = new GraphicsPath();
                int x = r.X + 6 + i * tabW;
                path.AddArc(x, top, 4, 4, 180, 90);
                path.AddArc(x + tabW - 6, top, 4, 4, 270, 90);
                path.AddLine(x + tabW - 2, top + tabH, x, top + tabH);
                path.CloseFigure();
                g.FillPath(i == 0 ? active : bg, path);
                g.DrawPath(border, path);
                // x close marker
                g.DrawLine(new Pen(Color.FromArgb(120, 120, 120)), x + tabW - 8, top + 4, x + tabW - 4, top + 8);
                g.DrawLine(new Pen(Color.FromArgb(120, 120, 120)), x + tabW - 4, top + 4, x + tabW - 8, top + 8);
            }
            // + button
            var plusRect = new Rectangle(r.X + 6 + 3 * tabW, top + 1, 18, tabH - 1);
            g.FillRectangle(addBg, plusRect);
            g.DrawRectangle(border, plusRect);
            int cx = plusRect.X + plusRect.Width / 2;
            int cy = plusRect.Y + plusRect.Height / 2;
            g.DrawLine(new Pen(Color.FromArgb(80, 80, 80), 1.2f), cx - 4, cy, cx + 4, cy);
            g.DrawLine(new Pen(Color.FromArgb(80, 80, 80), 1.2f), cx, cy - 4, cx, cy + 4);

            var content = new Rectangle(r.X + 6, top + tabH, r.Width - 12, r.Height - tabH - 12);
            g.FillRectangle(active, content);
            g.DrawRectangle(border, content);
            g.FillRectangle(fg, content.X + 6, content.Y + 6, content.Width - 12, 3);
            g.FillRectangle(fg, content.X + 6, content.Y + 13, (content.Width - 12) / 2, 3);
        }

        private static void PaintMdiIcon(Graphics g, Rectangle r)
        {
            // Two overlapping child-window frames
            using var border  = new Pen(Color.FromArgb(120, 120, 120), 1.2f);
            using var fillA   = new SolidBrush(Color.FromArgb(245, 247, 250));
            using var fillB   = new SolidBrush(Color.White);
            using var titleA  = new SolidBrush(Color.FromArgb(180, 180, 180));
            using var titleB  = new SolidBrush(Color.FromArgb(70, 130, 180));

            int marginX = 12, marginY = 12;
            var winA = new Rectangle(r.X + marginX, r.Y + marginY, r.Width - 60, r.Height - 36);
            var winB = new Rectangle(r.X + 36, r.Y + 30, r.Width - 50, r.Height - 36);

            g.FillRectangle(fillA, winA); g.DrawRectangle(border, winA);
            g.FillRectangle(titleA, winA.X, winA.Y, winA.Width, 8);

            g.FillRectangle(fillB, winB); g.DrawRectangle(border, winB);
            g.FillRectangle(titleB, winB.X, winB.Y, winB.Width, 8);
        }

        // ──────────────────────────────────────────────────────────────────
        // Mode tile control
        // ──────────────────────────────────────────────────────────────────

        private sealed class ModeTile : Control
        {
            public DocumentSetupMode Mode { get; set; }
            public string TileTitle   { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public bool IsRecommended { get; set; }
            public bool IsSelected    { get; set; }
            public Action<Graphics, Rectangle>? IconPainter { get; set; }

            public Color AccentColor     { get; set; }
            public Color AccentFore      { get; set; } = Color.White;
            public Color BorderColor     { get; set; }
            public Color HoverColor      { get; set; }
            public Color SelectedColor   { get; set; }
            public Color BackgroundColor { get; set; }
            public Color TitleTextColor  { get; set; } = Color.FromArgb(32, 33, 36);
            public Color MutedTextColor  { get; set; }

            private bool _hover;

            public event EventHandler? Selected;

            public ModeTile()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint
                       | ControlStyles.UserPaint
                       | ControlStyles.OptimizedDoubleBuffer
                       | ControlStyles.ResizeRedraw
                       | ControlStyles.Selectable, true);
                Cursor    = Cursors.Hand;
                TabStop   = true;
            }

            /// <summary>
            /// Public shim around the protected
            /// <see cref="Control.AccessibilityNotifyClients(AccessibleEvents, int)"/>
            /// so the parent dialog can fire UIA SelectionAdd + StateChange
            /// events when the tile's IsSelected flips. Lets screen readers
            /// announce the new mode without us needing a custom
            /// <see cref="AccessibleObject"/> with the SelectionItem pattern.
            /// </summary>
            public void RaiseSelectionAccessibilityEvent()
            {
                AccessibilityNotifyClients(AccessibleEvents.SelectionAdd, -1);
                AccessibilityNotifyClients(AccessibleEvents.StateChange,  -1);
            }

            protected override void OnMouseEnter(EventArgs e) { _hover = true;  Invalidate(); base.OnMouseEnter(e); }
            protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }
            protected override void OnClick(EventArgs e)
            {
                Focus();
                Selected?.Invoke(this, EventArgs.Empty);
                base.OnClick(e);
            }
            protected override void OnKeyDown(KeyEventArgs e)
            {
                if (e.KeyCode is Keys.Enter or Keys.Space)
                {
                    Selected?.Invoke(this, EventArgs.Empty);
                    e.Handled = true;
                }
                base.OnKeyDown(e);
            }
            protected override void OnEnter(EventArgs e) { Invalidate(); base.OnEnter(e); }
            protected override void OnLeave(EventArgs e) { Invalidate(); base.OnLeave(e); }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode     = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                Color bg = IsSelected ? SelectedColor : (_hover ? HoverColor : BackgroundColor);
                Color border = IsSelected ? AccentColor : BorderColor;

                using (var bgBrush = new SolidBrush(bg))
                using (var path    = RoundedRect(rect, 6))
                {
                    g.FillPath(bgBrush, path);
                    using var pen = new Pen(border, IsSelected ? 2f : 1f);
                    g.DrawPath(pen, path);
                }

                // Icon area (top 55%)
                int iconAreaH = (int)(Height * 0.55);
                int iconSize  = Math.Min(iconAreaH - 16, Width - 32);
                var iconRect  = new Rectangle((Width - iconSize) / 2, 12, iconSize, iconAreaH - 20);
                IconPainter?.Invoke(g, iconRect);

                // Title
                using var titleFont = new Font("Segoe UI Semibold", 10.5f);
                using var descFont  = new Font("Segoe UI", 8.75f);

                var titleRect = new Rectangle(12, iconAreaH, Width - 24, 20);
                TextRenderer.DrawText(g, TileTitle, titleFont, titleRect,
                    TitleTextColor,
                    TextFormatFlags.Left | TextFormatFlags.NoPrefix);

                var descRect = new Rectangle(12, iconAreaH + 22, Width - 24, Height - iconAreaH - 30);
                TextRenderer.DrawText(g, Description, descFont, descRect,
                    MutedTextColor,
                    TextFormatFlags.Left | TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);

                // Recommended badge
                if (IsRecommended)
                {
                    using var badgeBrush = new SolidBrush(AccentColor);
                    using var badgeFont  = new Font("Segoe UI Semibold", 7.5f);
                    string badge = "RECOMMENDED";
                    var sz = TextRenderer.MeasureText(badge, badgeFont);
                    var badgeRect = new Rectangle(Width - sz.Width - 18, 8, sz.Width + 10, sz.Height + 4);
                    using var badgePath = RoundedRect(badgeRect, 3);
                    g.FillPath(badgeBrush, badgePath);
                    TextRenderer.DrawText(g, badge, badgeFont, badgeRect, AccentFore,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                // Focus ring
                if (Focused)
                {
                    using var focusPen = new Pen(Color.FromArgb(160, AccentColor), 1f) { DashStyle = DashStyle.Dot };
                    var focusRect = new Rectangle(3, 3, Width - 7, Height - 7);
                    using var focusPath = RoundedRect(focusRect, 5);
                    g.DrawPath(focusPen, focusPath);
                }
            }

            private static GraphicsPath RoundedRect(Rectangle r, int radius)
            {
                var path = new GraphicsPath();
                int d = radius * 2;
                path.AddArc(r.X,              r.Y,              d, d, 180, 90);
                path.AddArc(r.Right - d,      r.Y,              d, d, 270, 90);
                path.AddArc(r.Right - d,      r.Bottom - d,     d, d,   0, 90);
                path.AddArc(r.X,              r.Bottom - d,     d, d,  90, 90);
                path.CloseFigure();
                return path;
            }
        }

        // ──────────────────────────────────────────────────────────────────
        // Preview panel
        // ──────────────────────────────────────────────────────────────────

        private sealed class PreviewPanel : Panel
        {
            public DocumentSetupMode RenderMode { get; set; } = DocumentSetupMode.TabbedDocuments;

            private readonly WizardPalette _palette;

            public PreviewPanel(WizardPalette palette)
            {
                _palette = palette ?? throw new ArgumentNullException(nameof(palette));
                SetStyle(ControlStyles.AllPaintingInWmPaint
                       | ControlStyles.UserPaint
                       | ControlStyles.OptimizedDoubleBuffer
                       | ControlStyles.ResizeRedraw, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var outer = new Rectangle(0, 0, Width - 1, Height - 1);
                using var bg     = new SolidBrush(_palette.PreviewBack);
                using var border = new Pen(_palette.PreviewBorder);
                g.FillRectangle(bg, outer);
                g.DrawRectangle(border, outer);

                switch (RenderMode)
                {
                    case DocumentSetupMode.TabbedDocuments: PaintTabbedPreview(g, outer); break;
                    case DocumentSetupMode.BrowserTabs:     PaintBrowserPreview(g, outer); break;
                    case DocumentSetupMode.NativeMdi:       PaintMdiPreview(g, outer); break;
                }
            }

            private void PaintTabbedPreview(Graphics g, Rectangle r)
            {
                using var stripBg  = new SolidBrush(_palette.PreviewStrip);
                using var active   = new SolidBrush(_palette.PreviewActiveTab);
                using var bg       = new SolidBrush(_palette.PreviewIdleTab);
                using var border   = new Pen(_palette.PreviewBorder);

                int stripH = 28;
                g.FillRectangle(stripBg, r.X + 1, r.Y + 1, r.Width - 2, stripH);

                string[] tabs = { "Welcome", "Document 1", "Document 2" };
                int x = r.X + 8;
                int activeIdx = 1;
                for (int i = 0; i < tabs.Length; i++)
                {
                    int tw = TextRenderer.MeasureText(tabs[i], SystemFonts.MessageBoxFont!).Width + 24;
                    var tr = new Rectangle(x, r.Y + 6, tw, stripH - 6);
                    g.FillRectangle(i == activeIdx ? active : bg, tr);
                    g.DrawRectangle(border, tr);
                    TextRenderer.DrawText(g, tabs[i], SystemFonts.MessageBoxFont, tr,
                        i == activeIdx ? _palette.PreviewText : _palette.PreviewDimText,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    x += tw + 2;
                }

                // Content area split into two panes (illustrates split capability)
                int contentTop = r.Y + 1 + stripH;
                var content = new Rectangle(r.X + 1, contentTop, r.Width - 2, r.Bottom - contentTop - 1);
                g.FillRectangle(active, content);
                int splitX = content.X + content.Width * 6 / 10;
                g.DrawLine(border, splitX, content.Y, splitX, content.Bottom);
                TextRenderer.DrawText(g, "Document 1 (active pane)",  SystemFonts.MessageBoxFont,
                    new Rectangle(content.X + 12, content.Y + 12, splitX - content.X - 16, 18),
                    _palette.PreviewText, TextFormatFlags.Left);
                TextRenderer.DrawText(g, "Document 2 (split pane)",   SystemFonts.MessageBoxFont,
                    new Rectangle(splitX + 12,   content.Y + 12, content.Right - splitX - 16, 18),
                    _palette.PreviewDimText, TextFormatFlags.Left);
            }

            private void PaintBrowserPreview(Graphics g, Rectangle r)
            {
                using var stripBg = new SolidBrush(_palette.PreviewStrip);
                using var active  = new SolidBrush(_palette.PreviewActiveTab);
                using var bg      = new SolidBrush(_palette.PreviewIdleTab);
                using var border  = new Pen(_palette.PreviewBorder);
                using var dimPen  = new Pen(_palette.PreviewDimText);

                int stripH = 30;
                g.FillRectangle(stripBg, r.X + 1, r.Y + 1, r.Width - 2, stripH);

                string[] tabs = { "Tab 1", "Tab 2", "Tab 3", "Tab 4" };
                int x = r.X + 6;
                int activeIdx = 0;
                for (int i = 0; i < tabs.Length; i++)
                {
                    int tw = 110;
                    using var path = new GraphicsPath();
                    path.AddArc(x,           r.Y + 4, 6, 6, 180, 90);
                    path.AddArc(x + tw - 6,  r.Y + 4, 6, 6, 270, 90);
                    path.AddLine(x + tw,     r.Y + 4 + stripH - 4, x, r.Y + 4 + stripH - 4);
                    path.CloseFigure();
                    g.FillPath(i == activeIdx ? active : bg, path);
                    g.DrawPath(border, path);
                    TextRenderer.DrawText(g, tabs[i], SystemFonts.MessageBoxFont,
                        new Rectangle(x + 10, r.Y + 6, tw - 24, stripH - 8),
                        i == activeIdx ? _palette.PreviewText : _palette.PreviewDimText,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                    g.DrawLine(dimPen, x + tw - 14, r.Y + 12, x + tw - 8,  r.Y + 18);
                    g.DrawLine(dimPen, x + tw - 8,  r.Y + 12, x + tw - 14, r.Y + 18);
                    x += tw + 2;
                }

                var addR = new Rectangle(x + 4, r.Y + 8, 22, stripH - 12);
                g.FillRectangle(active, addR);
                g.DrawRectangle(border, addR);
                using var plusPen = new Pen(_palette.PreviewText, 1.5f);
                int cx = addR.X + addR.Width / 2;
                int cy = addR.Y + addR.Height / 2;
                g.DrawLine(plusPen, cx - 5, cy, cx + 5, cy);
                g.DrawLine(plusPen, cx, cy - 5, cx, cy + 5);

                int contentTop = r.Y + 1 + stripH;
                var content = new Rectangle(r.X + 1, contentTop, r.Width - 2, r.Bottom - contentTop - 1);
                g.FillRectangle(active, content);
                TextRenderer.DrawText(g, "Tab 1 — page content", SystemFonts.MessageBoxFont,
                    new Rectangle(content.X + 12, content.Y + 12, content.Width - 24, 18),
                    _palette.PreviewText, TextFormatFlags.Left);
            }

            private void PaintMdiPreview(Graphics g, Rectangle r)
            {
                using var fill   = new SolidBrush(_palette.PreviewMdiSurface);
                using var winBg  = new SolidBrush(_palette.PreviewActiveTab);
                using var titleA = new SolidBrush(_palette.PreviewDimText);
                using var titleB = new SolidBrush(_palette.Accent);
                using var border = new Pen(_palette.PreviewBorder);

                g.FillRectangle(fill, r.X + 1, r.Y + 1, r.Width - 2, r.Height - 2);

                // Child A
                var a = new Rectangle(r.X + 14, r.Y + 14, r.Width / 2, r.Height / 2);
                g.FillRectangle(winBg, a); g.DrawRectangle(border, a);
                g.FillRectangle(titleA, a.X, a.Y, a.Width, 14);
                TextRenderer.DrawText(g, "Child Form A", SystemFonts.MessageBoxFont,
                    new Rectangle(a.X + 4, a.Y, a.Width - 8, 14), _palette.AccentFore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                // Child B (active)
                var b = new Rectangle(r.X + r.Width / 3, r.Y + r.Height / 3, r.Width / 2, r.Height / 2);
                g.FillRectangle(winBg, b); g.DrawRectangle(border, b);
                g.FillRectangle(titleB, b.X, b.Y, b.Width, 14);
                TextRenderer.DrawText(g, "Child Form B (active)", SystemFonts.MessageBoxFont,
                    new Rectangle(b.X + 4, b.Y, b.Width - 8, 14), _palette.AccentFore,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }

        // ──────────────────────────────────────────────────────────────────
        // Status label that doubles as a live region
        // ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Plain <see cref="Label"/> with a public hook over the protected
        /// <see cref="Control.AccessibilityNotifyClients(AccessibleEvents, int)"/>.
        /// Used as the wizard's status / live-region surface so screen
        /// readers announce mode changes (e.g. "Tabbed Documents selected.
        /// Creates …") without us shipping a custom UIA peer.
        /// </summary>
        private sealed class AnnouncingLabel : Label
        {
            public void RaiseAnnouncement()
            {
                AccessibilityNotifyClients(AccessibleEvents.NameChange,  -1);
                AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
            }
        }
    }
}
