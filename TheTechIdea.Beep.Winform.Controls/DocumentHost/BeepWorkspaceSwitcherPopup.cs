// BeepWorkspaceSwitcherPopup.cs
// Workspace picker popup with fuzzy search, thumbnail preview, and CRUD actions.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Floating popup for switching, creating, and deleting named workspaces.
    /// Show via <see cref="BeepDocumentHost.ShowWorkspaceSwitcher"/>.
    /// </summary>
    internal sealed class BeepWorkspaceSwitcherPopup : Form
    {
        // ── Controls ─────────────────────────────────────────────────────────

        private readonly TextBox _search;
        private readonly Panel   _listHost;
        private readonly Button  _btnNew;
        private readonly Button  _btnDelete;

        // ── Data ─────────────────────────────────────────────────────────────

        private readonly WorkspaceManager              _manager;
        private readonly IBeepTheme?                   _theme;
        private List<WorkspaceDefinition>              _filtered = new();
        private int                                    _selectedIndex;

        // ── Results ───────────────────────────────────────────────────────────

        /// <summary>Name of the workspace to activate, or null if cancelled.</summary>
        internal string? SelectedWorkspaceName { get; private set; }

        /// <summary>Name provided for a new workspace to save, or null.</summary>
        internal string? NewWorkspaceName { get; private set; }

        // ── Layout constants ──────────────────────────────────────────────────

        private const int PopupWidth  = 460;
        private const int PopupHeight = 440;
        private const int SearchH     = 36;
        private const int Pad         = 10;
        private const int ItemH       = 54;
        private const int ThumbW      = 64;
        private const int ThumbH      = 44;
        private const int BtnH        = 30;
        private const int MaxVisible  = 5;

        internal BeepWorkspaceSwitcherPopup(
            WorkspaceManager manager,
            IBeepTheme?      theme,
            Point            screenPosition)
        {
            _manager = manager;
            _theme   = theme;

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar   = false;
            TopMost         = true;
            StartPosition   = FormStartPosition.Manual;
            Size            = new Size(PopupWidth, PopupHeight);
            Location        = screenPosition;
            KeyPreview      = true;
            BackColor       = ThemeAwareColor(_theme?.PanelBackColor, SystemColors.Window);

            // Title
            var title = new Label
            {
                Bounds    = new Rectangle(Pad, Pad, PopupWidth - Pad * 2, 20),
                Text      = "Workspace Switcher",
                ForeColor = ThemeAwareColor(_theme?.ForeColor, SystemColors.WindowText),
                Font      = BeepFontManager.GetCachedFont("Segoe UI", 10f, FontStyle.Bold),
                AutoSize  = false,
                BackColor = Color.Transparent,
            };

            // Search box
            _search = new TextBox
            {
                Bounds          = new Rectangle(Pad, Pad + 22, PopupWidth - Pad * 2, SearchH),
                BorderStyle     = BorderStyle.FixedSingle,
                Font            = BeepFontManager.GetCachedFont("Segoe UI", 11f, FontStyle.Regular),
                PlaceholderText = "Search workspaces…",
                BackColor       = ThemeAwareColor(_theme?.BackgroundColor, SystemColors.Window),
                ForeColor       = ThemeAwareColor(_theme?.ForeColor, SystemColors.WindowText),
            };

            int listTop = Pad + 22 + SearchH + 6;
            int listH   = PopupHeight - listTop - Pad - BtnH - 8;

            _listHost = new Panel
            {
                Bounds    = new Rectangle(Pad, listTop, PopupWidth - Pad * 2, listH),
                BackColor = ThemeAwareColor(_theme?.PanelBackColor, SystemColors.Window),
            };

            // Bottom buttons
            int btnY = PopupHeight - Pad - BtnH;
            _btnNew = new Button
            {
                Bounds     = new Rectangle(Pad, btnY, 100, BtnH),
                Text       = "+ New",
                FlatStyle  = FlatStyle.Flat,
                Font       = BeepFontManager.GetCachedFont("Segoe UI", 9.5f, FontStyle.Regular),
                BackColor  = ThemeAwareColor(_theme?.PrimaryColor, SystemColors.Highlight),
                ForeColor  = ThemeAwareColor(_theme?.BackgroundColor, SystemColors.HighlightText),
            };
            _btnNew.FlatAppearance.BorderSize = 0;

            _btnDelete = new Button
            {
                Bounds     = new Rectangle(Pad + 110, btnY, 90, BtnH),
                Text       = "Delete",
                FlatStyle  = FlatStyle.Flat,
                Font       = BeepFontManager.GetCachedFont("Segoe UI", 9.5f, FontStyle.Regular),
                BackColor  = Color.FromArgb(180, 40, 40),
                ForeColor  = Color.White,
            };
            _btnDelete.FlatAppearance.BorderSize = 0;

            Controls.Add(title);
            Controls.Add(_search);
            Controls.Add(_listHost);
            Controls.Add(_btnNew);
            Controls.Add(_btnDelete);

            _search.TextChanged  += (_, _) => RefreshList();
            _listHost.Paint      += OnListPaint;
            _listHost.MouseMove  += OnListMouseMove;
            _listHost.MouseClick += OnListMouseClick;
            _listHost.MouseDoubleClick += (_, _) => CommitSelection();
            _btnNew.Click        += OnNewClicked;
            _btnDelete.Click     += OnDeleteClicked;
            KeyDown              += OnKeyDown;
            _search.KeyDown      += OnKeyDown;

            Deactivate += (_, _) => Close();

            RefreshList();
            _search.Focus();
        }

        // ── List refresh ──────────────────────────────────────────────────────

        private void RefreshList()
        {
            string q    = _search.Text.Trim();
            var    all  = _manager.GetAll().ToList();
            _filtered   = string.IsNullOrEmpty(q)
                ? all
                : all.Where(w => w.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                                 (w.Description ?? string.Empty).Contains(q, StringComparison.OrdinalIgnoreCase))
                     .ToList();
            _selectedIndex = 0;
            UpdateDeleteButton();
            _listHost.Invalidate();
        }

        private void UpdateDeleteButton()
            => _btnDelete.Enabled = _selectedIndex >= 0 && _selectedIndex < _filtered.Count;

        private int VisibleCount => Math.Min(_filtered.Count, MaxVisible);

        // ── Owner-draw list ───────────────────────────────────────────────────

        private void OnListPaint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Color back    = ThemeAwareColor(_theme?.PanelBackColor, SystemColors.Window);
            Color fore    = ThemeAwareColor(_theme?.ForeColor, SystemColors.WindowText);
            Color selBack = ThemeAwareColor(_theme?.PrimaryColor, SystemColors.Highlight);
            Color selFore = ThemeAwareColor(_theme?.BackgroundColor, SystemColors.HighlightText);
            Color dim     = ThemeAwareGrayText(_theme?.PanelBackColor);
            Color sep     = _theme?.BorderColor != Color.Empty && _theme?.BorderColor != null
                ? Color.FromArgb(30, fore) : Color.FromArgb(30, ThemeAwareColor(_theme?.ForeColor, SystemColors.WindowText));
            Color accent  = ThemeAwareColor(_theme?.PrimaryColor, SystemColors.Highlight);

            var nameFont = BeepFontManager.GetCachedFont("Segoe UI", 11f, FontStyle.Regular);
            var descFont = BeepFontManager.GetCachedFont("Segoe UI",  9f, FontStyle.Regular);

            string? activeId = _manager.ActiveWorkspaceId;

            for (int i = 0; i < VisibleCount; i++)
            {
                var ws       = _filtered[i];
                bool sel     = i == _selectedIndex;
                bool isActive = ws.Id == activeId;
                var itemRect = new Rectangle(0, i * ItemH, _listHost.Width, ItemH);

                using var backBr = new SolidBrush(sel ? selBack : back);
                g.FillRectangle(backBr, itemRect);

                // Thumbnail placeholder
                var thumbRect = new Rectangle(8, itemRect.Top + (ItemH - ThumbH) / 2, ThumbW, ThumbH);
                using var thumbBackBr = new SolidBrush(sel
                    ? Color.FromArgb(40, selFore) : Color.FromArgb(20, fore));
                g.FillRectangle(thumbBackBr, thumbRect);
                using var thumbPen = new Pen(sel ? Color.FromArgb(80, selFore) : dim);
                g.DrawRectangle(thumbPen, thumbRect);

                // Active indicator bar
                if (isActive)
                {
                    using var accentBr = new SolidBrush(accent);
                    g.FillRectangle(accentBr, thumbRect.Left, thumbRect.Top, 3, ThumbH);
                }

                // Workspace icon (simplified grid in thumbnail)
                using var iconPen = new Pen(sel ? Color.FromArgb(100, selFore) : Color.FromArgb(80, fore));
                int hm = thumbRect.Left + ThumbW / 2;
                int vm = thumbRect.Top  + ThumbH / 2;
                g.DrawLine(iconPen, thumbRect.Left + 4, vm, thumbRect.Right - 4, vm);
                g.DrawLine(iconPen, hm, thumbRect.Top + 4, hm, thumbRect.Bottom - 4);

                // Text area
                int textX = thumbRect.Right + 10;
                using var nameBr = new SolidBrush(sel ? selFore : fore);
                string nameText = isActive ? ws.Name + "  ✓" : ws.Name;
                g.DrawString(nameText, nameFont, nameBr,
                    new RectangleF(textX, itemRect.Top + 8, itemRect.Width - textX - 8, 20));

                if (!string.IsNullOrEmpty(ws.Description))
                {
                    using var descBr = new SolidBrush(sel ? Color.FromArgb(200, selFore) : dim);
                    g.DrawString(ws.Description, descFont, descBr,
                        new RectangleF(textX, itemRect.Top + 28, itemRect.Width - textX - 8, 16));
                }

                if (i < VisibleCount - 1)
                {
                    using var sepPen = new Pen(sep);
                    g.DrawLine(sepPen, 4, itemRect.Bottom - 1, _listHost.Width - 4, itemRect.Bottom - 1);
                }
            }

            if (_filtered.Count == 0)
            {
                using var emptyBr = new SolidBrush(dim);
                g.DrawString("No workspaces saved yet", nameFont, emptyBr, new PointF(8, 10));
            }
        }

        private void OnListMouseMove(object? sender, MouseEventArgs e)
        {
            int idx = e.Y / ItemH;
            if (idx >= 0 && idx < VisibleCount && idx != _selectedIndex)
            {
                _selectedIndex = idx;
                UpdateDeleteButton();
                _listHost.Invalidate();
            }
        }

        private void OnListMouseClick(object? sender, MouseEventArgs e)
        {
            int idx = e.Y / ItemH;
            if (idx >= 0 && idx < VisibleCount)
            {
                _selectedIndex = idx;
                UpdateDeleteButton();
                _listHost.Invalidate();
            }
        }

        // ── Keyboard ──────────────────────────────────────────────────────────

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    e.Handled = true;
                    Close();
                    break;

                case Keys.Enter:
                    e.Handled = true;
                    CommitSelection();
                    break;

                case Keys.Down:
                    if (_selectedIndex < VisibleCount - 1)
                    {
                        _selectedIndex++;
                        UpdateDeleteButton();
                        _listHost.Invalidate();
                    }
                    e.Handled = true;
                    break;

                case Keys.Up:
                    if (_selectedIndex > 0)
                    {
                        _selectedIndex--;
                        UpdateDeleteButton();
                        _listHost.Invalidate();
                    }
                    e.Handled = true;
                    break;
            }
        }

        private void CommitSelection()
        {
            if (_selectedIndex >= 0 && _selectedIndex < _filtered.Count)
                SelectedWorkspaceName = _filtered[_selectedIndex].Name;
            Close();
        }

        // ── Button handlers ───────────────────────────────────────────────────

        private void OnNewClicked(object? sender, EventArgs e)
        {
            string? name = PromptForName("Save current layout as workspace:", "New Workspace");
            if (!string.IsNullOrWhiteSpace(name))
            {
                NewWorkspaceName = name;
                Close();
            }
        }

        private void OnDeleteClicked(object? sender, EventArgs e)
        {
            if (_selectedIndex < 0 || _selectedIndex >= _filtered.Count) return;
            var ws = _filtered[_selectedIndex];
            var result = MessageBox.Show(
                $"Delete workspace \"{ws.Name}\"?",
                "Delete Workspace",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;

            _manager.Delete(ws.Id);
            RefreshList();
        }

        private static string? PromptForName(string prompt, string defaultValue)
        {
            using var dlg   = new Form
            {
                Text            = "New Workspace",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition   = FormStartPosition.CenterScreen,
                Size            = new Size(340, 130),
                MaximizeBox     = false,
                MinimizeBox     = false,
            };
            var lbl = new Label  { Bounds = new Rectangle(12, 12, 300, 18), Text = prompt };
            var txt = new TextBox{ Bounds = new Rectangle(12, 34, 300, 24), Text = defaultValue };
            var ok  = new Button { Bounds = new Rectangle(158, 66, 70, 28), Text = "OK",     DialogResult = DialogResult.OK };
            var cn  = new Button { Bounds = new Rectangle(242, 66, 70, 28), Text = "Cancel", DialogResult = DialogResult.Cancel };
            dlg.AcceptButton = ok;
            dlg.CancelButton = cn;
            dlg.Controls.AddRange(new Control[] { lbl, txt, ok, cn });

            return dlg.ShowDialog() == DialogResult.OK ? txt.Text.Trim() : null;
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
            return SystemColors.GrayText;
        }

        private static bool IsDarkBackground(Color c) => c.GetBrightness() < 0.5;

        private static Color Sc(Color lightColor)
        {
            return lightColor switch
            {
                var x when x == SystemColors.Window => Color.FromArgb(30, 30, 30),
                var x when x == SystemColors.WindowText => Color.White,
                var x when x == SystemColors.ControlText => Color.White,
                var x when x == SystemColors.GrayText => Color.FromArgb(150, 150, 155),
                var x when x == SystemColors.Highlight => Color.FromArgb(0, 120, 215),
                var x when x == SystemColors.HighlightText => Color.White,
                var x when x == SystemColors.Control => Color.FromArgb(45, 45, 48),
                var x when x == SystemColors.ControlDark => Color.FromArgb(70, 70, 75),
                var x when x == SystemColors.ControlLight => Color.FromArgb(70, 70, 75),
                var x when x == SystemColors.ControlLightLight => Color.FromArgb(60, 60, 65),
                var x when x == SystemColors.ActiveCaption => Color.FromArgb(45, 45, 48),
                var x when x == SystemColors.Info => Color.FromArgb(50, 50, 55),
                var x when x == SystemColors.InfoText => Color.White,
                _ => lightColor
            };
        }
    }
}
