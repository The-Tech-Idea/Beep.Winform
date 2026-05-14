// BeepDocumentQuickSwitch.cs
// Keyboard-driven document picker popup for BeepDocumentHost.
// Shown via Ctrl+P (or BeepDocumentHost.ShowQuickSwitch()).
// Provides type-to-filter + arrow-key navigation over open documents.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ImageManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Lightweight floating popup that lets the user quickly switch to any
    /// open document by typing a substring of its title or id.
    /// </summary>
    internal sealed class BeepDocumentQuickSwitch : Form
    {
        // ── Controls ─────────────────────────────────────────────────────────

        private readonly TextBox  _search;
        private readonly ListBox  _list;
        private readonly Panel    _frame;
        private readonly Panel    _preview;
        private readonly PictureBox _previewIcon;
        private readonly Label    _previewTitle;
        private readonly Label    _previewPath;
        private readonly Label    _previewMeta;

        // ── Data ─────────────────────────────────────────────────────────────

        private readonly IReadOnlyList<BeepDocumentTab> _allTabs;
        private readonly IBeepTheme?                    _currentTheme;
        private readonly int                            _initialIndex;   // -1 = use preselectId
        private List<BeepDocumentTab>                   _filtered = new();

        // ── Result ───────────────────────────────────────────────────────────

        /// <summary>The document id chosen by the user, or <c>null</c> if cancelled.</summary>
        internal string? SelectedDocumentId { get; private set; }

        // ── Layout constants ──────────────────────────────────────────────────

        private const int PopupWidth  = 640;
        private const int PopupHeight = 340;
        private const int SearchH     = 36;
        private const int Pad         = 8;
        private const int ItemH       = 30;
        private const int PreviewW    = 220;

        // ════════════════════════════════════════════════════════════════════
        // Constructor
        // ════════════════════════════════════════════════════════════════════

        internal BeepDocumentQuickSwitch(
            IReadOnlyList<BeepDocumentTab> tabs,
            string?                        activeDocumentId,
            IBeepTheme?                    theme,
            Point                          screenPosition,
            int                            initialIndex = -1)
        {
            _allTabs      = tabs;
            _currentTheme = theme;
            _initialIndex = initialIndex;

            // ── Form setup ───────────────────────────────────────────────────
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar   = false;
            TopMost         = true;
            StartPosition   = FormStartPosition.Manual;
            Size            = new Size(PopupWidth, PopupHeight);
            Location        = screenPosition;
            KeyPreview      = true;

            BackColor = ThemeAwareColor(_currentTheme?.PanelBackColor, SystemColors.Window);

            // ── Outer frame panel ────────────────────────────────────────────
            _frame = new Panel
            {
                Dock        = DockStyle.Fill,
                Padding     = new Padding(Pad),
                BackColor   = ThemeAwareColor(_currentTheme?.PanelBackColor, SystemColors.Window)
            };

            // ── Search box ───────────────────────────────────────────────────
            _search = new TextBox
            {
                Bounds         = new Rectangle(Pad, Pad, PopupWidth - Pad * 2, SearchH),
                Anchor         = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle    = BorderStyle.FixedSingle,
                Font           = BeepFontManager.GetCachedFont("Segoe UI", 11f, FontStyle.Regular),
                PlaceholderText= "Type to filter…",
                BackColor      = ThemeAwareColor(_currentTheme?.BackgroundColor, SystemColors.Window),
                ForeColor      = ThemeAwareColor(_currentTheme?.ForeColor, SystemColors.WindowText),
            };

            // ── Results list ─────────────────────────────────────────────────
            _list = new ListBox
            {
                Bounds         = new Rectangle(Pad, Pad + SearchH + 4, PopupWidth - PreviewW - Pad * 3,
                                               PopupHeight - SearchH - Pad * 3 - 4),
                Anchor         = AnchorStyles.Top | AnchorStyles.Bottom
                                | AnchorStyles.Left,
                DrawMode       = DrawMode.OwnerDrawFixed,
                ItemHeight     = ItemH,
                BorderStyle    = BorderStyle.None,
                BackColor      = ThemeAwareColor(_currentTheme?.PanelBackColor, SystemColors.Window),
                ForeColor      = ThemeAwareColor(_currentTheme?.ForeColor, SystemColors.WindowText),
                ScrollAlwaysVisible = false,
                IntegralHeight = false,
            };

            // ── Preview panel ────────────────────────────────────────────────
            _preview = new Panel
            {
                Bounds      = new Rectangle(_list.Right + Pad, _list.Top, PreviewW, _list.Height),
                Anchor      = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor   = ThemeAwareColor(_currentTheme?.BackgroundColor, SystemColors.Window),
                Padding     = new Padding(14)
            };

            _previewIcon = new PictureBox
            {
                Bounds      = new Rectangle(14, 14, 48, 48),
                BackColor   = Color.Transparent,
                SizeMode    = PictureBoxSizeMode.Zoom
            };

            _previewTitle = new Label
            {
                Bounds      = new Rectangle(14, 74, PreviewW - 28, 48),
                AutoSize    = false,
                BackColor   = Color.Transparent,
                ForeColor   = ThemeAwareColor(_currentTheme?.ForeColor, SystemColors.WindowText),
                Font        = BeepFontManager.GetCachedFont("Segoe UI", 11f, FontStyle.Bold),
                TextAlign   = ContentAlignment.TopLeft
            };

            _previewPath = new Label
            {
                Bounds      = new Rectangle(14, 132, PreviewW - 28, 84),
                AutoSize    = false,
                BackColor   = Color.Transparent,
                ForeColor   = ThemeAwareGrayText(_currentTheme?.PanelBackColor),
                Font        = BeepFontManager.GetCachedFont("Segoe UI", 8.75f, FontStyle.Regular),
                TextAlign   = ContentAlignment.TopLeft
            };

            _previewMeta = new Label
            {
                Bounds      = new Rectangle(14, 226, PreviewW - 28, 78),
                AutoSize    = false,
                BackColor   = Color.Transparent,
                ForeColor   = ThemeAwareColor(_currentTheme?.ForeColor, SystemColors.WindowText),
                Font        = BeepFontManager.GetCachedFont("Segoe UI", 8.75f, FontStyle.Regular),
                TextAlign   = ContentAlignment.TopLeft
            };

            _preview.Controls.Add(_previewIcon);
            _preview.Controls.Add(_previewTitle);
            _preview.Controls.Add(_previewPath);
            _preview.Controls.Add(_previewMeta);

            _frame.Controls.Add(_search);
            _frame.Controls.Add(_list);
            _frame.Controls.Add(_preview);
            Controls.Add(_frame);

            // ── Wire events ──────────────────────────────────────────────────
            _search.TextChanged  += OnSearchChanged;
            _list.DrawItem       += OnDrawItem;
            _list.SelectedIndexChanged += OnSelectedIndexChanged;
            _list.MouseDoubleClick += (s, e) => CommitSelection();

            KeyDown  += OnKeyDown;
            _search.KeyDown += OnKeyDown;
            _list.KeyDown   += OnKeyDown;

            Deactivate += (s, e) => { SelectedDocumentId = null; Close(); };

            // ── Populate ─────────────────────────────────────────────────────
            PopulateList(string.Empty, activeDocumentId);
            _search.Focus();
        }

        // ── Search filter ────────────────────────────────────────────────────

        private void OnSearchChanged(object? sender, EventArgs e)
        {
            PopulateList(_search.Text, null);
        }

        private void PopulateList(string filter, string? preselectId)
        {
            _filtered = string.IsNullOrWhiteSpace(filter)
                ? _allTabs.ToList()
                : _allTabs.Where(t =>
                      t.Title.Contains(filter, StringComparison.OrdinalIgnoreCase)
                   || t.Id.Contains(filter, StringComparison.OrdinalIgnoreCase))
                  .ToList();

            _list.BeginUpdate();
            _list.Items.Clear();
            foreach (var t in _filtered)
                _list.Items.Add(t.Title);
            _list.EndUpdate();

            // Pre-select — honour initialIndex (MRU mode first), then preselectId, then index 0
            if (_filtered.Count > 0)
            {
                int sel = 0;
                if (_initialIndex >= 0 && string.IsNullOrWhiteSpace(filter))
                {
                    sel = Math.Clamp(_initialIndex, 0, _filtered.Count - 1);
                }
                else if (preselectId != null)
                {
                    int found = _filtered.FindIndex(t => t.Id == preselectId);
                    if (found >= 0) sel = found;
                }
                _list.SelectedIndex = sel;
            }
            else
            {
                UpdatePreview(null);
            }
        }

        private void OnSelectedIndexChanged(object? sender, EventArgs e)
        {
            var tab = _list.SelectedIndex >= 0 && _list.SelectedIndex < _filtered.Count
                ? _filtered[_list.SelectedIndex]
                : null;
            UpdatePreview(tab);
        }

        private void UpdatePreview(BeepDocumentTab? tab)
        {
            var oldImage = _previewIcon.Image;
            _previewIcon.Image = null;
            if (oldImage != null)
                oldImage.Dispose();

            if (tab == null)
            {
                _previewTitle.Text = "No document selected";
                _previewPath.Text = string.Empty;
                _previewMeta.Text = string.Empty;
                return;
            }

            _previewTitle.Text = string.IsNullOrWhiteSpace(tab.Title) ? "Untitled document" : tab.Title;
            _previewPath.Text = $"Path\r\n{GetPreviewPath(tab)}";
            _previewMeta.Text = BuildMetaText(tab);
            _previewIcon.Image = ResolvePreviewImage(tab);
        }

        private static string GetPreviewPath(BeepDocumentTab tab)
        {
            if (!string.IsNullOrWhiteSpace(tab.TooltipText))
                return tab.TooltipText!;
            return tab.Id;
        }

        private static string BuildMetaText(BeepDocumentTab tab)
        {
            var parts = new List<string>();
            if (tab.IsActive) parts.Add("Active");
            if (tab.IsModified) parts.Add("Modified");
            if (tab.IsPinned) parts.Add("Pinned");
            if (!tab.CanClose) parts.Add("Locked");
            if (!string.IsNullOrWhiteSpace(tab.DocumentCategory)) parts.Add(tab.DocumentCategory!);
            if (!string.IsNullOrWhiteSpace(tab.Group)) parts.Add($"Group: {tab.Group}");
            return string.Join("\r\n", parts);
        }

        private static Image? ResolvePreviewImage(BeepDocumentTab tab)
        {
            if (tab.ImageList != null && tab.ImageIndex >= 0 && tab.ImageIndex < tab.ImageList.Images.Count)
                return new Bitmap(tab.ImageList.Images[tab.ImageIndex]);

            if (!string.IsNullOrWhiteSpace(tab.IconPath))
            {
                var resolved = ImageListHelper.GetImageFromName(tab.IconPath!);
                if (resolved is Icon icon)
                    return icon.ToBitmap();
                if (resolved is Image image)
                    return new Bitmap(image);
            }

            return CreateFallbackImage(tab);
        }

        private static Image CreateFallbackImage(BeepDocumentTab tab)
        {
            var bmp = new Bitmap(48, 48);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            Color back = tab.AccentColor != Color.Empty ? tab.AccentColor : Color.SteelBlue;
            using var backBr = new SolidBrush(back);
            g.FillEllipse(backBr, 0, 0, 47, 47);

            string glyph = string.IsNullOrWhiteSpace(tab.Title)
                ? "D"
                : char.ToUpperInvariant(tab.Title.Trim()[0]).ToString();
            using var font = BeepFontManager.GetCachedFont("Segoe UI", 18f, FontStyle.Bold);
            using var textBr = new SolidBrush(Color.White);
            using var fmt = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.DrawString(glyph, font, textBr, new RectangleF(0, 0, 48, 48), fmt);
            return bmp;
        }

        // ── Keyboard handling ────────────────────────────────────────────────

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    SelectedDocumentId = null;
                    e.Handled = true;
                    Close();
                    break;

                case Keys.Enter:
                    CommitSelection();
                    e.Handled = true;
                    break;

                case Keys.Down:
                    if (_list.SelectedIndex < _list.Items.Count - 1)
                        _list.SelectedIndex++;
                    e.Handled = true;
                    break;

                case Keys.Up:
                    if (_list.SelectedIndex > 0)
                        _list.SelectedIndex--;
                    e.Handled = true;
                    break;

                case Keys.Tab:
                    // Tab with no filter → cycle downward
                    if (_list.Items.Count > 0)
                    {
                        int n = e.Shift
                            ? (_list.SelectedIndex - 1 + _list.Items.Count) % _list.Items.Count
                            : (_list.SelectedIndex + 1) % _list.Items.Count;
                        _list.SelectedIndex = n;
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        // ── Ctrl+Tab / Ctrl+Shift+Tab cycling while the popup is visible ────────
        // Intercept before WinForms dialog-key processing so holding Ctrl and
        // pressing Tab repeatedly steps through the MRU list without closing the popup.

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_list.Items.Count > 0)
            {
                if (keyData == (Keys.Control | Keys.Tab))
                {
                    _list.SelectedIndex = (_list.SelectedIndex + 1) % _list.Items.Count;
                    return true;
                }
                if (keyData == (Keys.Control | Keys.Shift | Keys.Tab))
                {
                    _list.SelectedIndex = (_list.SelectedIndex - 1 + _list.Items.Count) % _list.Items.Count;
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CommitSelection()
        {
            int idx = _list.SelectedIndex;
            if (idx >= 0 && idx < _filtered.Count)
                SelectedDocumentId = _filtered[idx].Id;
            Close();
        }

        // ── Owner-draw list items ────────────────────────────────────────────

        private void OnDrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _filtered.Count) return;

            var tab     = _filtered[e.Index];
            bool sel    = (e.State & DrawItemState.Selected) != 0;

            Color backCol = sel
                ? ThemeAwareColor(_currentTheme?.PrimaryColor, SystemColors.Highlight)
                : ThemeAwareColor(_currentTheme?.PanelBackColor, SystemColors.Window);
            Color foreCol = sel
                ? ThemeAwareColor(_currentTheme?.BackgroundColor, SystemColors.HighlightText)
                : ThemeAwareColor(_currentTheme?.ForeColor, SystemColors.WindowText);
            Color dimCol  = sel
                ? Color.FromArgb(200, foreCol)
                : ThemeAwareGrayText(_currentTheme?.PanelBackColor);

            using var backBr = new SolidBrush(backCol);
            e.Graphics.FillRectangle(backBr, e.Bounds);

            // Active indicator bar
            if (tab.IsActive)
            {
                using var bar = new SolidBrush(_currentTheme?.PrimaryColor ?? Color.DodgerBlue);
                e.Graphics.FillRectangle(bar,
                    e.Bounds.Left, e.Bounds.Top + 4, 3, e.Bounds.Height - 8);
            }

            // Icon area placeholder (8 px left margin)
            int x = e.Bounds.Left + 12;

            // Modified dot
            if (tab.IsModified)
            {
                using var dot = new SolidBrush(Color.OrangeRed);
                e.Graphics.FillEllipse(dot,
                    x, e.Bounds.Top + (e.Bounds.Height - 8) / 2, 8, 8);
                x += 12;
            }

            // Title
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            var titleRect = new Rectangle(
                x, e.Bounds.Top, e.Bounds.Width - x - 60, e.Bounds.Height);
            using var titleFmt = new StringFormat
            {
                Alignment     = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter,
                FormatFlags   = StringFormatFlags.NoWrap
            };
            using var titleBr = new SolidBrush(foreCol);
            e.Graphics.DrawString(tab.Title, Font, titleBr, titleRect, titleFmt);

            // Pinned badge
            if (tab.IsPinned)
            {
                using var pinBr = new SolidBrush(dimCol);
                using var fmt   = new StringFormat
                    { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString("📌", Font, pinBr,
                    new Rectangle(e.Bounds.Right - 28, e.Bounds.Top, 24, e.Bounds.Height), fmt);
            }
        }

        // ── Paint border ────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Color borderCol = ThemeAwareColor(_currentTheme?.BorderColor, SystemColors.ControlDark);
            using var pen = new Pen(borderCol, 1);
            e.Graphics.DrawRectangle(pen,
                0, 0, ClientSize.Width - 1, ClientSize.Height - 1);

            using var dividerPen = new Pen(Color.FromArgb(60, borderCol), 1);
            e.Graphics.DrawLine(dividerPen, _preview.Left - (Pad / 2), _preview.Top,
                _preview.Left - (Pad / 2), _preview.Bottom);
        }

        // ── WndProc: resizable via owner-drawn border ─────────────────────────

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                // Prevent the shadow / rounded corners that look wrong for a picker
                cp.ClassStyle |= 0x20000; // CS_DROPSHADOW
                return cp;
            }
        }

        // ── Theme-aware color fallbacks ──────────────────────────────────────

        private static Color ThemeAwareColor(Color? themeColor, Color lightColor)
        {
            if (themeColor.HasValue && themeColor.Value != Color.Empty)
                return themeColor.Value;
            return Sc(lightColor);
        }

        private static Color ThemeAwareGrayText(Color? refColor)
        {
            if (refColor.HasValue && IsDarkBackground(refColor.Value))
                return Color.FromArgb(150, 150, 155);
            return TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.GrayText);
        }

        private static bool IsDarkBackground(Color c) => c.GetBrightness() < 0.5;

        private static Color Sc(Color lightColor)
        {
            return TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(lightColor);
        }
    }
}
