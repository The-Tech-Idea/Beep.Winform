using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{


        // === DIALOG CARD (visual) ===
        internal sealed class _DialogCard : Panel
        {
            private readonly BeepDialogContent _content;
            private readonly BeepDialogOptions _options;
            private readonly Control _owner;
        // At top of _DialogCard class:
        private BeepButton? _defaultBtn;
        private BeepButton? _cancelBtn;
        private Panel? _validationPanel;
        private BeepLabel? _validationLabel;
        // fields
        private Color _themedBack, _themedBorder;
        private Font _titleFont, _bodyFont, _btnFont;
        private Color _titleColor, _bodyColor, _btnTextColor;


        public event EventHandler<BeepDialogResult>? Closed;

            public _DialogCard(BeepDialogContent content, BeepDialogOptions options, Control owner)
            {
                _content = content;
                _options = options;
                _owner = owner;

                DoubleBuffered = true;
                TabStop = true;

                BuildUI();
            }

            // Theme-aware sizing
            public Size PreferredDialogSize(Size area)
            {
                int w = Math.Min(Math.Max(400, (int)(area.Width * 0.6)), _options.MaxSize.Width);
                int h = Math.Min(Math.Max(220, (int)(area.Height * 0.6)), _options.MaxSize.Height);
                return new Size(w, h);
            }

            public void FocusFirst()
            {
                var first = Controls.OfType<Control>().FirstOrDefault(c => c.TabStop && c.CanSelect);
                (first ?? this).Focus();
            }

            public bool TryTriggerDefault()
            {
                if (_defaultBtn is { Enabled: true, Visible: true })
                {
                    _defaultBtn.PerformClick();
                    return true;
                }
                return false;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                var theme = ((_owner as IBeepModernFormHost)?.CurrentTheme);
                var back = theme?.BackColor ?? Color.White;
                var border = theme?.BorderColor ?? Color.FromArgb(210, 210, 210);

                var rect = ClientRectangle;
                rect.Inflate(-1, -1);

                using var backBr = new SolidBrush(back);
                using var borderPen = new Pen(border, Math.Max(1, _options.BorderThickness));

                if (_options.BorderRadius > 0)
                {
                    using var path = RoundRect(rect, _options.BorderRadius);
                    g.FillPath(backBr, path);
                    g.DrawPath(borderPen, path);
                }
                else
                {
                    g.FillRectangle(backBr, rect);
                    g.DrawRectangle(borderPen, rect);
                }

                // soft shadow glow (cheap)
                using var glowPen = new Pen(Color.FromArgb(35, 0, 0, 0), 16) { LineJoin = System.Drawing.Drawing2D.LineJoin.Round };
                using var path2 = RoundRect(rect, Math.Max(1, _options.BorderRadius));
                g.DrawPath(glowPen, path2);
            }

            private void BuildUI()
            {
                SuspendLayout();
                Controls.Clear();

            var theme = ThemeAccess.GetCurrentTheme(_owner);
            var back = theme?.BackColor ?? Color.White;
                var fore = theme?.ForeColor ?? Color.Black;
            _themedBack = ThemeAccess.PanelBack(theme, Color.White);
            _themedBorder = ThemeAccess.Border(theme, Color.FromArgb(210, 210, 210));
            // Container layout
            var header = new Panel { Height = 48, Dock = DockStyle.Top, Padding = new Padding(16, 12, 16, 8) };
                var body = new Panel { Dock = DockStyle.Fill, Padding = _options.ContentPadding, BackColor = back };
                var footer = new Panel { Height = 56, Dock = DockStyle.Bottom, Padding = new Padding(16), BackColor = back };

            // Title & Close
            var title = CreateLabel(_content.Title, true);
            title.Dock = DockStyle.Fill;

                var close = CreateButton("✕", () => RaiseClose(BeepDialogResult.Cancel));
                close.Width = 36; close.Dock = DockStyle.Right;

                header.Controls.Add(title);
                header.Controls.Add(close);

                // Body
                if (_content.CustomBody != null)
                {
                    _content.CustomBody.Dock = DockStyle.Fill;
                    body.Controls.Add(_content.CustomBody);
                }
                else
                {
                var lbl = CreateLabel(_content.Message, false);
                lbl.Dock = DockStyle.Fill;
                    lbl.Padding = new Padding(0, 4, 0, 0);
                    body.Controls.Add(lbl);
                }

                // Footer buttons (right-aligned)
                var flow = new FlowLayoutPanel { Dock = DockStyle.Right, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
            // When building footer buttons, keep references:
            foreach (var b in _content.Buttons)
            {
                var btn = CreateButton(b.Text, () => RaiseClose(b.Result));
                btn.Margin = new Padding(8, 0, 0, 0);
                if (b.IsDefault) { _defaultBtn = btn; btn.Font = new Font(btn.Font, FontStyle.Bold); }
                if (b.IsCancel) { _cancelBtn = btn; }
                flow.Controls.Add(btn);
            }

            footer.Controls.Add(flow);
            // Inside BuildUI(), after you create 'footer' panel, insert a left-aligned validation area:
            _validationPanel = new Panel { Dock = DockStyle.Left, Width = 320, Padding = new Padding(0, 2, 0, 0) };
            _validationLabel = new BeepLabel { AutoSize = false, Dock = DockStyle.Fill, ForeColor = Color.FromArgb(170, 30, 30) };
            _validationPanel.Controls.Add(_validationLabel);
              footer.Controls.Add(_validationPanel);
                Controls.Add(body);
                Controls.Add(footer);
                Controls.Add(header);

                BackColor = back;
                ForeColor = fore;

                ResumeLayout(true);
            // After adding header/body/footer and ResumeLayout, wire validation:
            if (_content.Validator != null)
            {
                // initial state
                ApplyValidation(_content.Validator());

                // watch changes
                var root = _content.ValidationRoot ?? _content.CustomBody;
                if (root != null) WireValidation(root);
            }
        }
        private void WireValidation(Control root)
        {
            foreach (Control c in root.Controls)
            {
                switch (c)
                {
                    case TextBoxBase tb: tb.TextChanged += (_, __) => Revalidate(); break;
                    case ComboBox cb: cb.SelectedIndexChanged += (_, __) => Revalidate(); break;
                    case CheckBox chk: chk.CheckedChanged += (_, __) => Revalidate(); break;
                    case NumericUpDown nud: nud.ValueChanged += (_, __) => Revalidate(); break;
                    default:
                        // try common pattern
                        c.TextChanged += (_, __) => Revalidate();
                        break;
                }
                if (c.HasChildren) WireValidation(c);
            }
        }

        private void Revalidate()
        {
            if (_content.Validator == null) return;
            ApplyValidation(_content.Validator());
        }

        private void ApplyValidation(ValidationState state)
        {
            if (_defaultBtn != null) _defaultBtn.Enabled = state.IsValid;
            if (_validationPanel != null && _validationLabel != null)
            {
                if (state.IsValid || state.Errors.Count == 0)
                {
                    _validationPanel.Visible = false;
                    _validationLabel.Text = string.Empty;
                }
                else
                {
                    _validationPanel.Visible = true;
                    _validationLabel.Text = "• " + string.Join(Environment.NewLine + "• ", state.Errors);
                }
            }
        }
        private void RaiseClose(BeepDialogResult result) => Closed?.Invoke(this, result);
        // ===== Helpers (keep both overloads) =====
        private BeepLabel CreateLabel(string text, bool big)
        {
            return new BeepLabel
            {
                Text = text,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = big ? _titleFont : _bodyFont,      // uses cached themed fonts
                ForeColor = big ? _titleColor : _bodyColor
            };
        }

        // Back-compat overload for old call sites: CreateLabel(text, themedColor, themedFont)
        private BeepLabel CreateLabel(string text, Color themedText, Font themedFont)
        {
            return new BeepLabel
            {
                Text = text,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = themedFont,
                ForeColor = themedText
            };
        }

        private BeepButton CreateButton(string text, Action onClick)
        {
            var btn = new BeepButton
            {
                Text = text,
                AutoSize = true,
           
                Padding = new Padding(12, 6, 12, 6),
                Font = _btnFont,
                ForeColor = _btnTextColor
            };
            btn.Click += (_, __) => onClick();
            return btn;
        }

        // Back-compat overload: CreateButton(text, onClick, themedFont, themedText)
        private BeepButton CreateButton(string text, Action onClick, Font themedFont, Color themedText)
        {
            var btn = new BeepButton
            {
                Text = text,
                AutoSize = true,
              
                Padding = new Padding(12, 6, 12, 6),
                Font = themedFont,
                ForeColor = themedText
            };
            btn.Click += (_, __) => onClick();
            return btn;
        }

        private static System.Drawing.Drawing2D.GraphicsPath RoundRect(Rectangle rect, int radius)
            {
                var path = new System.Drawing.Drawing2D.GraphicsPath();
                if (radius <= 0) { path.AddRectangle(rect); return path; }
                int d = radius * 2;
                var arc = new Rectangle(rect.Location, new Size(d, d));
                path.AddArc(arc, 180, 90);
                arc.X = rect.Right - d; path.AddArc(arc, 270, 90);
                arc.Y = rect.Bottom - d; path.AddArc(arc, 0, 90);
                arc.X = rect.Left; path.AddArc(arc, 90, 90);
                path.CloseFigure();
                return path;
            }

        public void ApplyThemeFrom(Vis.Modules.BeepTheme? theme)
        {
            theme = ThemeAccess.GetCurrentTheme(_owner);
            _themedBack = ThemeAccess.PanelBack(theme, Color.White);
            _themedBorder = ThemeAccess.Border(theme, Color.FromArgb(210, 210, 210));
            (_titleFont, _titleColor) = ThemeAccess.TitleTypography(theme, new Font("Segoe UI", 11f, FontStyle.Bold), Color.Black);
            (_bodyFont, _bodyColor) = ThemeAccess.BodyTypography(theme, new Font("Segoe UI", 9.5f, FontStyle.Regular), Color.Black);
            (_btnFont, _btnTextColor) = ThemeAccess.ButtonTypography(theme, new Font("Segoe UI", 9.5f, FontStyle.Bold), Color.Black);
            // Walk panels
            foreach (Control c in Controls)
                if (c is Panel p) p.BackColor = _themedBack;

            // Walk labels/buttons to refresh fonts & colors
            foreach (var lbl in Enumerate<BeepLabel>(this))
            {
                bool isHeader = lbl.Parent?.Dock == DockStyle.Top || lbl.Parent?.Parent?.Dock == DockStyle.Top;
                lbl.Font = isHeader ? _titleFont : _bodyFont;
                lbl.ForeColor = isHeader ? _titleColor : _bodyColor;
            }
            foreach (var btn in Enumerate<BeepButton>(this))
            {
                btn.Font = _btnFont;
                btn.ForeColor = _btnTextColor;
            }

            Invalidate();
        }
        private static IEnumerable<T> Enumerate<T>(Control root) where T : Control
        {
            foreach (Control c in root.Controls)
            {
                if (c is T v) yield return v;
                if (c.HasChildren)
                    foreach (var cc in Enumerate<T>(c)) yield return cc;
            }
        }
    }

}

