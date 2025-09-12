using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{

    internal sealed class BeepToastCard : Panel
    {
        private readonly Control _owner;
        public readonly BeepToastOptions Options;
        public int PreferredWidth => Options.MaxWidth;
        public int PreferredHeight => _body.PreferredSize.Height + 24;
        private float _animT; // 0..1 slide/opacity
        private readonly Panel _body = new();
        private BeepButton _actionBtn;

        public BeepToastCard(Control owner, BeepToastOptions options)
        {
            _owner = owner; Options = options;
            DoubleBuffered = true;

            var theme = ThemeAccess.GetCurrentTheme(owner);

            var title = ThemeAccess.TitleTypography(theme, new Font("Segoe UI", 10.5f, FontStyle.Bold), Color.Black);
            var body = ThemeAccess.BodyTypography(theme, new Font("Segoe UI", 9.0f, FontStyle.Regular), Color.Black);
            var btn = ThemeAccess.ButtonTypography(theme, new Font("Segoe UI", 9.0f, FontStyle.Bold), Color.Black);

            _body = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 12, 14, 12),
                BackColor = GetBackColor(options.Kind),
                ForeColor = GetForeColor(options.Kind)
            };
            Controls.Add(_body);

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var titleLbl = new BeepLabel { Text = options.Title ?? "", AutoSize = true, Font = title.font, ForeColor = title.color };
            var msgLbl = new BeepLabel { Text = options.Message, AutoSize = true, Font = body.font, ForeColor = _body.ForeColor };

            layout.Controls.Add(titleLbl, 0, 0);
            layout.SetColumnSpan(msgLbl, 2);
            layout.Controls.Add(msgLbl, 0, 1);

            if (!string.IsNullOrEmpty(options.ActionText) && options.Action != null)
            {
                var actionBtn = new BeepButton { Text = options.ActionText, AutoSize = true, Padding = new Padding(10, 4, 10, 4), Font = btn.font, ForeColor = btn.color };
                actionBtn.Click += (_, __) => options.Action?.Invoke();
                actionBtn.Margin = new Padding(12, 0, 0, 0);
                layout.Controls.Add(actionBtn, 1, 0);
            }

            _body.Controls.Add(layout);
        }
        //private Color ToastBack(BeepTheme theme, BeepToastKind k)
        //{
        //    // If you’ve added semantic colors to BeepTheme, map them here:
        //    return k switch
        //    {
        //        BeepToastKind.Success => theme.SuccessBackColor.IsEmpty ? Color.FromArgb(0xE7, 0xF6, 0xED) : theme.SuccessBackColor,
        //        BeepToastKind.Warning => theme.WarningBackColor.IsEmpty ? Color.FromArgb(0xFF, 0xF8, 0xE1) : theme.WarningBackColor,
        //        BeepToastKind.Error => theme.ErrorBackColor.IsEmpty ? Color.FromArgb(0xF8, 0xE8, 0xE8) : theme.ErrorBackColor,
        //        _ /*Info*/            => theme.InfoBackColor.IsEmpty ? Color.FromArgb(0xE8, 0xF0, 0xFE) : theme.InfoBackColor
        //    };
        //}
        //private Color ToastFore(BeepTheme theme, BeepToastKind k, Color fallback)
        //{
        //    return k switch
        //    {
        //        BeepToastKind.Success => theme.SuccessTextColor.IsEmpty ? fallback : theme.SuccessTextColor,
        //        BeepToastKind.Warning => theme.WarningTextColor.IsEmpty ? fallback : theme.WarningTextColor,
        //        BeepToastKind.Error => theme.ErrorTextColor.IsEmpty ? fallback : theme.ErrorTextColor,
        //        _ => theme.InfoTextColor.IsEmpty ? fallback : theme.InfoTextColor
        //    };
        //}
        private Color GetBackColor(BeepToastKind k)
        {
            var theme = (_owner as IBeepModernFormHost)?.CurrentTheme;
            // soft defaults if theme doesn’t provide semantic colors
            return k switch
            {
                BeepToastKind.Success => Color.FromArgb(0xE7, 0xF6, 0xED),
                BeepToastKind.Warning => Color.FromArgb(0xFF, 0xF8, 0xE1),
                BeepToastKind.Error => Color.FromArgb(0xF8, 0xE8, 0xE8),
                _ /*Info*/            => Color.FromArgb(0xE8, 0xF0, 0xFE),
            };
        }
        private Color GetForeColor(BeepToastKind k)
        {
            return k switch
            {
                BeepToastKind.Success => Color.FromArgb(0x1E, 0x46, 0x2A),
                BeepToastKind.Warning => Color.FromArgb(0x83, 0x55, 0x00),
                BeepToastKind.Error => Color.FromArgb(0x8A, 0x1C, 0x1C),
                _ => Color.FromArgb(0x0B, 0x3B, 0x8C),
            };
        }

        private BeepLabel CreateBeepLabel(string text, bool bold)
        {
            var lbl = new BeepLabel { Text = text, AutoSize = true };
            if (bold) lbl.Font = new Font(lbl.Font, FontStyle.Bold);
            return lbl;
        }
        private BeepButton CreateBeepButton(string text, Action onClick)
        {
            var btn = new BeepButton { Text = text, AutoSize = true, Padding = new Padding(10, 4, 10, 4) };
            btn.Click += (_, __) => onClick();
            return btn;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // simple drop shadow
            using var pen = new Pen(Color.FromArgb(28, 0, 0, 0), 14) { LineJoin = System.Drawing.Drawing2D.LineJoin.Round };
            var r = _body.Bounds; r.Inflate(-1, -1);
            using var path = RoundRect(r, 12);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawPath(pen, path);
        }

        public async Task AnimateInAsync()
        {
            Visible = true;
            var sw = Stopwatch.StartNew();
            const int ms = 180;
            while (sw.ElapsedMilliseconds < ms)
            {
                _animT = (float)sw.ElapsedMilliseconds / ms;
                Invalidate();
                await Task.Delay(8);
            }
            _animT = 1f;
        }
        public async Task AnimateOutAsync()
        {
            var start = 1f;
            var sw = Stopwatch.StartNew();
            const int ms = 140;
            while (sw.ElapsedMilliseconds < ms)
            {
                _animT = start - (float)sw.ElapsedMilliseconds / ms;
                Invalidate();
                await Task.Delay(8);
            }
            _animT = 0f;
            Visible = false;
        }

        private static System.Drawing.Drawing2D.GraphicsPath RoundRect(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int d = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(d, d));
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - d; path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - d; path.AddArc(arc, 0, 90);
            arc.X = rect.Left; path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
        // Live re-theme (host calls this from ApplyThemeNow)
        public void ApplyThemeFrom(BeepTheme theme)
        {
            var (_, bodyColor) = ThemeAccess.BodyTypography(theme, _body.Font ?? new Font("Segoe UI", 9f), _body.ForeColor);
            _body.BackColor = GetBackColor(Options.Kind);
            _body.ForeColor = GetForeColor(Options.Kind);

            foreach (Control c in _body.Controls)
            {
                if (c is BeepLabel lbl)
                {
                    bool isTitle = lbl.Font.Bold; // crude but fine; or tag your title label
                    var (font, color) = isTitle
                        ? ThemeAccess.TitleTypography(theme, lbl.Font, lbl.ForeColor)
                        : ThemeAccess.BodyTypography(theme, lbl.Font, lbl.ForeColor);
                    lbl.Font = font; lbl.ForeColor = isTitle ? color : _body.ForeColor;
                }
                else if (c is BeepButton btn)
                {
                    var (font, color) = ThemeAccess.ButtonTypography(theme, btn.Font, btn.ForeColor);
                    btn.Font = font; btn.ForeColor = color;
                }
            }
            Invalidate();
        }
    }
}
