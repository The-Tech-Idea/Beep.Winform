// ThemePickerDialog.cs
// Sprint 17.4 — Visual theme picker invoked from the "Choose Theme…" smart-tag.
// Displays a scrollable tile grid where each tile shows the theme name, a
// primary-colour swatch, and dark/light badge.  Selecting a tile and clicking
// OK returns the theme name string which the smart-tag writes to ThemeName.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Modal form that lets the developer choose a Beep theme at design time.
    /// Opens from the "Choose Theme…" smart-tag action on BeepDocumentHost.
    /// </summary>
    public sealed class ThemePickerDialog : Form
    {
        // ── lightweight snapshot of what we need from IBeepTheme ─────────────

        private sealed record ThemeInfo(
            string Name,
            Color  Primary,
            Color  Back,
            Color  Fore,
            bool   IsDark);

        // ── result ────────────────────────────────────────────────────────────

        /// <summary>Name of the selected theme. Valid when DialogResult == OK.</summary>
        public string SelectedThemeName { get; private set; } = string.Empty;

        // ── UI ────────────────────────────────────────────────────────────────

        private readonly TextBox         _searchBox;
        private readonly FlowLayoutPanel _tilePanel;
        private readonly Label           _infoLabel;
        private readonly Button          _okButton;
        private readonly Button          _cancelButton;

        private List<ThemeInfo>  _allThemes = new();
        private ThemeTile?       _selectedTile;
        private string           _currentName;

        // ─────────────────────────────────────────────────────────────────────

        public ThemePickerDialog(string currentThemeName = "")
        {
            _currentName = currentThemeName;
            SelectedThemeName = currentThemeName;

            Text            = "Choose Theme";
            Size            = new Size(720, 520);
            MinimumSize     = new Size(520, 380);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox     = false;
            MinimizeBox     = false;
            BackColor       = SystemColors.Window;
            Font            = new Font("Segoe UI", 9f);

            // Title row
            var titleLabel = new Label
            {
                Text    = "Select a theme:",
                Dock    = DockStyle.Top,
                Height  = 32,
                Padding = new Padding(8, 8, 0, 0),
                Font    = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            // Search box
            _searchBox = new TextBox
            {
                Dock        = DockStyle.Top,
                Height      = 26,
                BorderStyle = BorderStyle.FixedSingle,
                Font        = new Font("Segoe UI", 9f),
            };
            _searchBox.TextChanged += SearchBox_TextChanged;

            var searchWrap = new Panel
            {
                Dock    = DockStyle.Top,
                Height  = 32,
                Padding = new Padding(8, 3, 8, 3)
            };
            _searchBox.Dock = DockStyle.Fill;
            searchWrap.Controls.Add(_searchBox);

            // Tile panel
            _tilePanel = new FlowLayoutPanel
            {
                Dock          = DockStyle.Fill,
                AutoScroll    = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = true,
                Padding       = new Padding(8, 4, 8, 4)
            };

            // Info label
            _infoLabel = new Label
            {
                Dock      = DockStyle.Top,
                Height    = 22,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(8, 0, 0, 0),
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = SystemColors.GrayText
            };

            // Button bar
            var btnBar = new FlowLayoutPanel
            {
                Dock          = DockStyle.Bottom,
                Height        = 44,
                BackColor     = SystemColors.Control,
                FlowDirection = FlowDirection.RightToLeft,
                Padding       = new Padding(8, 7, 8, 7),
                WrapContents  = false
            };

            _cancelButton = new Button
            {
                Text         = "Cancel",
                DialogResult = DialogResult.Cancel,
                Size         = new Size(80, 28)
            };
            _okButton = new Button
            {
                Text         = "OK",
                DialogResult = DialogResult.OK,
                Size         = new Size(80, 28),
                Enabled      = false
            };
            btnBar.Controls.Add(_cancelButton);
            btnBar.Controls.Add(_okButton);

            AcceptButton = _okButton;
            CancelButton = _cancelButton;

            Controls.Add(_tilePanel);
            Controls.Add(_infoLabel);
            Controls.Add(searchWrap);
            Controls.Add(titleLabel);
            Controls.Add(btnBar);

            _okButton.Click += (_, __) => DialogResult = DialogResult.OK;

            LoadThemes();
        }

        // ── theme loading ─────────────────────────────────────────────────────

        private void LoadThemes()
        {
            try
            {
                _allThemes = BeepThemesManager.GetThemes()
                    .Select(t => new ThemeInfo(
                        t.ThemeName,
                        SafeColor(t, "PrimaryColor", Color.SteelBlue),
                        SafeColor(t, "BackColor",    Color.White),
                        SafeColor(t, "ForeColor",    Color.Black),
                        SafeBool(t,  "IsDarkTheme")))
                    .ToList();
            }
            catch
            {
                _allThemes = new List<ThemeInfo>();
            }

            // Merge with fallback list so we always have something to show
            var known = new HashSet<string>(_allThemes.Select(t => t.Name), StringComparer.OrdinalIgnoreCase);
            foreach (var fb in FallbackThemes())
            {
                if (!known.Contains(fb.Name))
                    _allThemes.Add(fb);
            }

            PopulateTiles(_allThemes);
        }

        // Safe reflection helpers for IBeepTheme (avoids hard coupling to every property)
        private static Color SafeColor(object theme, string prop, Color fallback)
        {
            try
            {
                var pi = theme.GetType().GetProperty(prop);
                if (pi?.GetValue(theme) is Color c && c != Color.Empty) return c;
            }
            catch { }
            return fallback;
        }

        private static bool SafeBool(object theme, string prop)
        {
            try
            {
                var pi = theme.GetType().GetProperty(prop);
                if (pi?.GetValue(theme) is bool b) return b;
            }
            catch { }
            return false;
        }

        private void PopulateTiles(IEnumerable<ThemeInfo> themes)
        {
            _tilePanel.SuspendLayout();
            _tilePanel.Controls.Clear();

            foreach (var theme in themes.OrderBy(t => t.Name))
            {
                var tile = new ThemeTile(theme.Name, theme.Primary, theme.Back, theme.Fore, theme.IsDark)
                {
                    IsSelected = string.Equals(theme.Name, _currentName,
                                               StringComparison.OrdinalIgnoreCase)
                };
                tile.TileClicked       += Tile_Clicked;
                tile.TileDoubleClicked += Tile_DoubleClicked;
                _tilePanel.Controls.Add(tile);

                if (tile.IsSelected)
                    _selectedTile = tile;
            }

            _tilePanel.ResumeLayout(true);
            _infoLabel.Text = $"{_tilePanel.Controls.Count} theme(s)";
        }

        private void SearchBox_TextChanged(object? sender, EventArgs e)
        {
            var q = _searchBox.Text.Trim();
            var filtered = string.IsNullOrEmpty(q)
                ? _allThemes
                : _allThemes.Where(t => t.Name.Contains(q, StringComparison.OrdinalIgnoreCase));
            PopulateTiles(filtered);
        }

        private void Tile_Clicked(ThemeTile tile)
        {
            if (_selectedTile != null && _selectedTile != tile)
            {
                _selectedTile.IsSelected = false;
                _selectedTile.Invalidate();
            }
            _selectedTile = tile;
            tile.IsSelected = true;
            tile.Invalidate();
            SelectedThemeName = tile.ThemeName;
            _okButton.Enabled = true;
        }

        private void Tile_DoubleClicked(ThemeTile tile)
        {
            Tile_Clicked(tile);
            DialogResult = DialogResult.OK;
        }

        // ── fallback theme seeds (shown even when BeepThemesManager can't load) ──

        private static List<ThemeInfo> FallbackThemes() => new()
        {
            new("DefaultTheme",         Color.FromArgb(0, 120, 215),   Color.White,              Color.Black,              false),
            new("DarkTheme",            Color.FromArgb(0,  99, 177),   Color.FromArgb(30,30,30), Color.White,              true),
            new("LightTheme",           Color.FromArgb(16, 110, 190),  Color.White,              Color.Black,              false),
            new("OceanTheme",           Color.FromArgb(0, 150, 180),   Color.FromArgb(10,40,60), Color.White,              true),
            new("ForestTheme",          Color.FromArgb(40, 140, 80),   Color.FromArgb(20,40,20), Color.White,              true),
            new("NeonTheme",            Color.FromArgb(255, 0, 255),   Color.Black,              Color.FromArgb(220,220,220), true),
            new("MaterialDesignTheme",  Color.FromArgb(33, 150, 243),  Color.White,              Color.Black,              false),
            new("DraculaTheme",         Color.FromArgb(189, 147, 249), Color.FromArgb(40,42,54), Color.FromArgb(248,248,242), true),
            new("NordTheme",            Color.FromArgb(94, 129, 172),  Color.FromArgb(46,52,64), Color.FromArgb(236,239,244), true),
            new("TokyoTheme",           Color.FromArgb(122, 162, 247), Color.FromArgb(26,27,38), Color.FromArgb(192,202,245), true),
            new("FluentTheme",          Color.FromArgb(0, 120, 212),   Color.White,              Color.Black,              false),
            new("iOSTheme",             Color.FromArgb(0, 122, 255),   Color.White,              Color.Black,              false),
            new("MacOSTheme",           Color.FromArgb(0, 122, 255),   Color.FromArgb(246,246,246), Color.Black,           false),
            new("NeumorphismTheme",     Color.FromArgb(110, 115, 200), Color.FromArgb(227,230,234), Color.FromArgb(60,65,120), false),
            new("CyberpunkNeonTheme",   Color.FromArgb(0, 255, 159),   Color.FromArgb(10,10,25), Color.FromArgb(0,255,159), true),
        };
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ThemeTile — one visual tile inside the picker grid
    // ─────────────────────────────────────────────────────────────────────────

    internal sealed class ThemeTile : Control
    {
        // ── events ────────────────────────────────────────────────────────────
        public event Action<ThemeTile>? TileClicked;
        public event Action<ThemeTile>? TileDoubleClicked;

        // ── data ──────────────────────────────────────────────────────────────
        public string ThemeName    { get; }
        public bool   IsSelected   { get; set; }

        private readonly Color  _primary;
        private readonly Color  _back;
        private readonly Color  _fore;
        private readonly bool   _isDark;
        private bool            _hovered;

        private const int TileW = 150;
        private const int TileH = 80;

        // ─────────────────────────────────────────────────────────────────────

        public ThemeTile(string themeName, Color primary, Color back, Color fore, bool isDark)
        {
            ThemeName = themeName;
            _primary  = primary;
            _back     = back;
            _fore     = fore;
            _isDark   = isDark;

            Size           = new Size(TileW, TileH);
            Cursor         = Cursors.Hand;
            Margin         = new Padding(4);
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g  = e.Graphics;
            var rc = ClientRectangle;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Background swatch (upper 60%)
            var swatchRc = new Rectangle(0, 0, rc.Width, rc.Height - 22);
            using (var b = new SolidBrush(_back))
                g.FillRectangle(b, swatchRc);

            // Primary accent stripe (top 4 px)
            using (var b = new SolidBrush(_primary))
                g.FillRectangle(b, 0, 0, rc.Width, 6);

            // Small colour dots
            int dotY = swatchRc.Height / 2 - 6;
            DrawDot(g, _primary,    12, dotY);
            DrawDot(g, _back,       36, dotY);
            DrawDot(g, _fore,       60, dotY);

            // Label background
            var nameRc = new Rectangle(0, swatchRc.Bottom, rc.Width, 22);
            using (var b = new SolidBrush(SystemColors.ControlLight))
                g.FillRectangle(b, nameRc);

            // Theme name text
            string shortName = ThemeName.EndsWith("Theme", StringComparison.OrdinalIgnoreCase)
                ? ThemeName[..^5]
                : ThemeName;

            using var fnt = new Font("Segoe UI", 7.5f);
            using var fg  = new SolidBrush(SystemColors.ControlText);
            var sf = new StringFormat
            {
                Alignment     = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming      = StringTrimming.EllipsisCharacter,
                FormatFlags   = StringFormatFlags.NoWrap
            };
            g.DrawString(shortName, fnt, fg, (RectangleF)nameRc, sf);

            // Dark/Light badge
            string badge = _isDark ? "◗ Dark" : "◑ Light";
            using var badgeFont = new Font("Segoe UI", 6.5f);
            var badgeSize = g.MeasureString(badge, badgeFont);
            var badgeRc   = new RectangleF(rc.Width - badgeSize.Width - 4, swatchRc.Height - badgeSize.Height - 3, badgeSize.Width + 2, badgeSize.Height);
            using var badgeBg = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
            g.FillRectangle(badgeBg, badgeRc);
            g.DrawString(badge, badgeFont, Brushes.White, badgeRc);

            // Selection / hover border
            Color borderColor = IsSelected
                ? SystemColors.Highlight
                : _hovered ? SystemColors.ControlDark : SystemColors.ControlLight;
            int bw = IsSelected ? 2 : 1;
            using var pen = new Pen(borderColor, bw);
            g.DrawRectangle(pen, 0, 0, rc.Width - 1, rc.Height - 1);
        }

        private static void DrawDot(Graphics g, Color c, int x, int y)
        {
            using var b = new SolidBrush(c);
            using var p = new Pen(Color.FromArgb(80, 0, 0, 0), 1);
            g.FillEllipse(b, x, y, 14, 14);
            g.DrawEllipse(p, x, y, 14, 14);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hovered = false;
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left)
                TileClicked?.Invoke(this);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button == MouseButtons.Left)
                TileDoubleClicked?.Invoke(this);
        }
    }
}
