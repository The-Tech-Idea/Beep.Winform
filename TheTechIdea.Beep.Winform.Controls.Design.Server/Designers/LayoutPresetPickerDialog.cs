// LayoutPresetPickerDialog.cs
// Visual dialog for selecting a starting layout configuration
// (Single, Side-by-Side, Stacked, Three-Way, etc.).
// Invoked from the "Apply Layout Preset…" smart-tag action.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Identifies the built-in layout preset seeds.
    /// </summary>
    public enum LayoutPreset
    {
        /// <summary>Single tab group filling the entire area.</summary>
        Single,
        /// <summary>Two groups side-by-side (horizontal split).</summary>
        SideBySide,
        /// <summary>Two groups stacked (vertical split).</summary>
        Stacked,
        /// <summary>Three groups in an L-pattern (top-split + right column).</summary>
        ThreeWay,
        /// <summary>Four groups in a 2×2 grid.</summary>
        FourUp
    }

    /// <summary>
    /// Modal dialog that lets the developer select a starting layout preset
    /// for a <see cref="TheTechIdea.Beep.Winform.Controls.DocumentHost.BeepDocumentHost"/>
    /// at design time.
    /// </summary>
    public sealed class LayoutPresetPickerDialog : Form
    {
        // ─────────────────────────────────────────────────────────────────────
        // Result
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>The preset chosen by the developer. Valid when <c>DialogResult == OK</c>.</summary>
        public LayoutPreset SelectedPreset { get; private set; } = LayoutPreset.Single;

        // ─────────────────────────────────────────────────────────────────────
        // Internal UI
        // ─────────────────────────────────────────────────────────────────────

        private readonly LayoutPreset[] _presets =
        {
            LayoutPreset.Single,
            LayoutPreset.SideBySide,
            LayoutPreset.Stacked,
            LayoutPreset.ThreeWay,
            LayoutPreset.FourUp
        };

        private readonly string[] _labels =
        {
            "Single",
            "Side-by-Side",
            "Stacked",
            "Three-Way",
            "Four-Up"
        };

        private readonly string[] _descriptions =
        {
            "One tab group filling the entire area",
            "Two groups split left/right",
            "Two groups split top/bottom",
            "Three groups in L-pattern",
            "Four groups in 2 × 2 grid"
        };

        private int _hovered = -1;
        private int _selected = 0;

        private readonly FlowLayoutPanel _tilePanel;
        private readonly Label           _descLabel;
        private readonly Button          _okButton;
        private readonly Button          _cancelButton;
        private readonly PresetTile[]    _tiles;

        public LayoutPresetPickerDialog()
        {
            Text            = "Apply Layout Preset";
            Size            = new Size(580, 380);
            MinimumSize     = new Size(480, 320);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            BackColor       = SystemColors.Window;
            Font            = new Font("Segoe UI", 9f);

            // Title label
            var titleLabel = new Label
            {
                Text      = "Choose a starting layout template:",
                Dock      = DockStyle.Top,
                Height    = 32,
                Padding   = new Padding(8, 8, 0, 0),
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            // Tile panel
            _tilePanel = new FlowLayoutPanel
            {
                Dock          = DockStyle.Fill,
                Padding       = new Padding(12, 8, 12, 0),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = true,
                AutoScroll    = true
            };

            _tiles = new PresetTile[_presets.Length];
            for (int i = 0; i < _presets.Length; i++)
            {
                var tile = new PresetTile(_presets[i], _labels[i])
                {
                    Width  = 96,
                    Height = 90,
                    Margin = new Padding(6),
                    Tag    = i
                };
                int idx = i;
                tile.Click      += (s, e) => SelectTile(idx);
                tile.MouseEnter += (s, e) => { _hovered = idx; UpdateDescLabel(); };
                tile.MouseLeave += (s, e) => { _hovered = -1; UpdateDescLabel(); };
                tile.DoubleClick += (s, e) => { SelectTile(idx); AcceptDialog(); };
                _tiles[i] = tile;
                _tilePanel.Controls.Add(tile);
            }

            // Description label
            _descLabel = new Label
            {
                Dock      = DockStyle.Top,
                Height    = 28,
                Padding   = new Padding(12, 4, 0, 0),
                ForeColor = SystemColors.GrayText,
                Text      = _descriptions[0]
            };

            // Button strip
            var btnPanel = new Panel { Dock = DockStyle.Bottom, Height = 48, Padding = new Padding(0, 8, 12, 0) };
            _okButton = new Button { Text = "Apply", Width = 90, Height = 30, DialogResult = DialogResult.OK };
            _cancelButton = new Button { Text = "Cancel", Width = 90, Height = 30, DialogResult = DialogResult.Cancel };
            _okButton.Click     += (s, e) => { SelectedPreset = _presets[_selected]; Close(); };
            _cancelButton.Click += (s, e) => Close();

            _okButton.Location     = new Point(btnPanel.Width - 200, 8);
            _cancelButton.Location = new Point(btnPanel.Width - 100, 8);
            btnPanel.Anchor        = AnchorStyles.Bottom;
            btnPanel.Controls.Add(_okButton);
            btnPanel.Controls.Add(_cancelButton);
            btnPanel.Resize += (s, e) =>
            {
                _okButton.Location     = new Point(btnPanel.Width - 196, 8);
                _cancelButton.Location = new Point(btnPanel.Width - 98, 8);
            };

            AcceptButton = _okButton;
            CancelButton = _cancelButton;

            Controls.Add(_tilePanel);
            Controls.Add(_descLabel);
            Controls.Add(btnPanel);
            Controls.Add(titleLabel);

            SelectTile(0);
        }

        private void SelectTile(int idx)
        {
            _selected = idx;
            for (int i = 0; i < _tiles.Length; i++)
                _tiles[i].IsSelected = (i == idx);
            UpdateDescLabel();
            _tilePanel.Invalidate(true);
        }

        private void UpdateDescLabel()
        {
            int show = _hovered >= 0 ? _hovered : _selected;
            _descLabel.Text = _descriptions[show];
        }

        private void AcceptDialog()
        {
            SelectedPreset = _presets[_selected];
            DialogResult   = DialogResult.OK;
            Close();
        }

        // ─────────────────────────────────────────────────────────────────────
        // PresetTile — inner owner-drawn tile control
        // ─────────────────────────────────────────────────────────────────────

        private sealed class PresetTile : Control
        {
            public LayoutPreset Preset { get; }
            public string       Label  { get; }
            public bool IsSelected
            {
                get => _selected;
                set { _selected = value; Invalidate(); }
            }
            private bool _selected;
            private bool _hot;

            public PresetTile(LayoutPreset preset, string label)
            {
                Preset    = preset;
                Label     = label;
                DoubleBuffered = true;

                MouseEnter += (s, e) => { _hot = true;  Invalidate(); };
                MouseLeave += (s, e) => { _hot = false; Invalidate(); };
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g   = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Background
                var bg = _selected ? Color.FromArgb(220, 232, 255)
                        : _hot     ? Color.FromArgb(240, 245, 255)
                        :            SystemColors.ControlLight;
                using var bgBrush = new SolidBrush(bg);
                g.FillRoundedRect(bgBrush, new Rectangle(1, 1, Width - 3, Height - 3), 6);

                // Border
                var bc = _selected ? Color.FromArgb(80, 120, 220)
                        : _hot     ? Color.FromArgb(160, 180, 220)
                        :            SystemColors.ControlDark;
                using var pen = new Pen(bc, _selected ? 2f : 1f);
                g.DrawRoundedRect(pen, new Rectangle(1, 1, Width - 3, Height - 3), 6);

                // Layout sketch
                var sketch = new Rectangle(10, 10, Width - 20, Height - 26);
                DrawLayoutSketch(g, sketch, Preset);

                // Label
                var labelRect = new RectangleF(0, Height - 18, Width, 16);
                var sf = new StringFormat { Alignment = StringAlignment.Center };
                using var fgBrush = new SolidBrush(ForeColor);
                g.DrawString(Label, Font, fgBrush, labelRect, sf);
            }

            private static void DrawLayoutSketch(Graphics g, Rectangle r, LayoutPreset preset)
            {
                using var framePen = new Pen(Color.FromArgb(100, 130, 160), 1f);
                using var fillBrush = new SolidBrush(Color.FromArgb(200, 215, 240));
                using var divPen    = new Pen(Color.FromArgb(150, 160, 180), 1f);

                switch (preset)
                {
                    case LayoutPreset.Single:
                        g.FillRectangle(fillBrush, r);
                        g.DrawRectangle(framePen, r);
                        break;

                    case LayoutPreset.SideBySide:
                    {
                        int half = r.Width / 2 - 1;
                        var l    = new Rectangle(r.X,                r.Y, half,        r.Height);
                        var rr   = new Rectangle(r.X + half + 2,    r.Y, r.Width - half - 2, r.Height);
                        g.FillRectangle(fillBrush, l);  g.DrawRectangle(framePen, l);
                        g.FillRectangle(fillBrush, rr); g.DrawRectangle(framePen, rr);
                        break;
                    }
                    case LayoutPreset.Stacked:
                    {
                        int half = r.Height / 2 - 1;
                        var top  = new Rectangle(r.X, r.Y,              r.Width, half);
                        var bot  = new Rectangle(r.X, r.Y + half + 2,  r.Width, r.Height - half - 2);
                        g.FillRectangle(fillBrush, top);  g.DrawRectangle(framePen, top);
                        g.FillRectangle(fillBrush, bot);  g.DrawRectangle(framePen, bot);
                        break;
                    }
                    case LayoutPreset.ThreeWay:
                    {
                        int lw  = r.Width * 6 / 10;
                        int rw  = r.Width - lw - 2;
                        int th  = r.Height / 2 - 1;
                        var l   = new Rectangle(r.X,          r.Y, lw, r.Height);
                        var rt  = new Rectangle(r.X + lw + 2, r.Y, rw, th);
                        var rb  = new Rectangle(r.X + lw + 2, r.Y + th + 2, rw, r.Height - th - 2);
                        g.FillRectangle(fillBrush, l);  g.DrawRectangle(framePen, l);
                        g.FillRectangle(fillBrush, rt); g.DrawRectangle(framePen, rt);
                        g.FillRectangle(fillBrush, rb); g.DrawRectangle(framePen, rb);
                        break;
                    }
                    case LayoutPreset.FourUp:
                    {
                        int hw = r.Width  / 2 - 1;
                        int hh = r.Height / 2 - 1;
                        Rectangle[] rects =
                        {
                            new Rectangle(r.X,       r.Y,       hw,           hh),
                            new Rectangle(r.X+hw+2,  r.Y,       r.Width-hw-2, hh),
                            new Rectangle(r.X,       r.Y+hh+2,  hw,           r.Height-hh-2),
                            new Rectangle(r.X+hw+2,  r.Y+hh+2,  r.Width-hw-2, r.Height-hh-2)
                        };
                        foreach (var rr in rects)
                        {
                            g.FillRectangle(fillBrush, rr);
                            g.DrawRectangle(framePen, rr);
                        }
                        break;
                    }
                }
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Graphics extension helpers (rounded rect — no external dependency)
    // ─────────────────────────────────────────────────────────────────────────

    internal static class GraphicsExtensions
    {
        public static void FillRoundedRect(this Graphics g, Brush b, Rectangle r, int radius)
        {
            using var path = MakeRoundedPath(r, radius);
            g.FillPath(b, path);
        }

        public static void DrawRoundedRect(this Graphics g, Pen p, Rectangle r, int radius)
        {
            using var path = MakeRoundedPath(r, radius);
            g.DrawPath(p, path);
        }

        private static System.Drawing.Drawing2D.GraphicsPath MakeRoundedPath(Rectangle r, int radius)
        {
            int d    = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(r.X,            r.Y,              d, d, 180, 90);
            path.AddArc(r.Right - d,    r.Y,              d, d, 270, 90);
            path.AddArc(r.Right - d,    r.Bottom - d,     d, d,   0, 90);
            path.AddArc(r.X,            r.Bottom - d,     d, d,  90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
