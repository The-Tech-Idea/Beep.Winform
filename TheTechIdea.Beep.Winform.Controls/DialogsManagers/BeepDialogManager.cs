using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules.Managers;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    // === THE MANAGER ===
    [DesignerCategory("Code")]
    public partial class BeepDialogManager : Component
    {
        private readonly Control _host;
        private readonly Form? _hostForm; // optional, for KeyPreview/keyboard
        private readonly List<_Shown> _stack = new();
        // cache by host control so repeated calls reuse the same manager
        private static readonly ConditionalWeakTable<Control, BeepDialogManager> _cacheByHost = new();

        public static BeepDialogManager For(Control control)
        {
            if (control is null) throw new ArgumentNullException(nameof(control));

            // prefer the nearest ancestor that can actually host overlays:
            //  - if there’s a Form, use it (covers whole window)
            //  - else use the first ContainerControl up the chain
            Control host = control.FindForm()
                           ?? FindNearestContainer(control)
                           ?? control; // fallback

            return _cacheByHost.GetValue(host, h => new BeepDialogManager(h));
        }

        private static Control? FindNearestContainer(Control start)
        {
            for (Control? p = start; p != null; p = p.Parent)
                if (p is ContainerControl) return p;
            return null;
        }

        public BeepDialogManager(Control host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _hostForm = _host.FindForm();

            if (_hostForm is not null)
            {
                _hostForm.KeyPreview = true;
                _hostForm.KeyDown += Owner_KeyDown;
                _hostForm.Resize += Owner_Resize;
            }
            else
            {
                // Host may be a panel/usercontrol without a parent form yet
                _host.KeyDown += Owner_KeyDown;
                _host.Resize += Owner_Resize;
            }

            // theme changes
            BeepThemesManager.ThemeChanged -= HandleGlobalThemeChanged;
            BeepThemesManager.ThemeChanged += HandleGlobalThemeChanged;
        }


        // Public close method so other features can programmatically close the top dialog.
        public Task ClosesTopAsync(BeepDialogResult result) => InternalCloseTopAsync(result);
        // expose internal close that v1 used privately
        internal async Task InternalCloseTopAsync(BeepDialogResult result) => await CloseTopAsync(result);
        // Quick helpers (you can expand later)
        public Task<BeepDialogResult> ConfirmAsync(string title, string message,
            string okText = "OK", string cancelText = "Cancel",
            BeepDialogOptions? options = null, CancellationToken token = default)
        {
            var dlg = new BeepDialogContent
            {
                Title = title,
                Message = message,
                Buttons = new[]
                {
                    new BeepDialogButton { Text = cancelText, Result = BeepDialogResult.Cancel, IsCancel=true },
                    new BeepDialogButton { Text = okText, Result = BeepDialogResult.OK, IsDefault=true }
                }
            };
            return ShowAsync(dlg, options ?? new BeepDialogOptions(), token);
        }

        public Task<BeepDialogResult> MessageAsync(string title, string message, string okText = "OK",
            BeepDialogOptions? options = null, CancellationToken token = default)
        {
            var dlg = new BeepDialogContent
            {
                Title = title,
                Message = message,
                Buttons = new[] { new BeepDialogButton { Text = okText, Result = BeepDialogResult.OK, IsDefault = true } }
            };
            return ShowAsync(dlg, options ?? new BeepDialogOptions(), token);
        }

        public async Task<BeepDialogResult> ShowAsync(BeepDialogContent content, BeepDialogOptions options, CancellationToken token = default)
        {
            // 1) Overlay
            var overlay = new _OverlayPanel { TabStop = true };
            overlay.BackColor = Color.Transparent; // painted with alpha in OnPaint
            overlay.Opacity = 0f;

            // 2) Container (card)
            var card = new _DialogCard(content, options, _host);
            card.Visible = false;
            card.Closed += (_, result) => _ = CloseTopAsync(result);

            // 3) Attach
            // Wherever you added controls previously:
            _host.Controls.Add(overlay);
            _host.Controls.Add(card);
            overlay.Bounds = GetTargetDisplayRect();
            overlay.BringToFront();
            card.BringToFront();

            var shown = new _Shown(overlay, card, options);
            _stack.Add(shown);

            overlay.Click += (_, __) =>
            {
                if (options.DismissOnOverlayClick) _ = CloseTopAsync(BeepDialogResult.Cancel);
            };

            // 4) Animate in
            await AnimateAsync(shown, true, token);

            // 5) Focus trap
            card.FocusFirst();

            // 6) Wait
            var tcs = new TaskCompletionSource<BeepDialogResult>();
            shown.Tcs = tcs;
            return await tcs.Task;
        }

        private async Task CloseTopAsync(BeepDialogResult result)
        {
            if (_stack.Count == 0) return;
            var top = _stack[^1];
            await AnimateAsync(top, false, CancellationToken.None);
            _stack.RemoveAt(_stack.Count - 1);

            top.Overlay?.Dispose();
            top.Card?.Dispose();

            top.Tcs?.TrySetResult(result);
        }

        private async Task AnimateAsync(_Shown s, bool opening, CancellationToken token)
        {
            var ms = Math.Max(60, s.Options.AnimationMs);
            var sw = Stopwatch.StartNew();

            // Prep
            if (opening)
            {
                s.Card.Visible = true;
            }

            var startRect = opening ? ComputeStartBounds(s.Card, s.Options.Kind) : s.Card.Bounds;
            var endRect = opening ? ComputeEndBounds(s.Card, s.Options.Kind) : ComputeStartBounds(s.Card, s.Options.Kind);
            var startOv = opening ? 0f : 0.45f;
            var endOv = opening ? 0.45f : 0f;

            while (sw.ElapsedMilliseconds < ms)
            {
                token.ThrowIfCancellationRequested();
                var t = (float)sw.ElapsedMilliseconds / ms;
                t = EaseOutCubic(t);

                var r = Lerp(startRect, endRect, t);
                s.Card.Bounds = Snap(r);
                s.Overlay.Opacity = Lerp(startOv, endOv, t);
                s.Overlay.Invalidate();

                await Task.Delay(8, token);
            }
            // Final
            s.Card.Bounds = opening ? ComputeEndBounds(s.Card, s.Options.Kind) : ComputeStartBounds(s.Card, s.Options.Kind);
            s.Overlay.Opacity = endOv;
            s.Overlay.Invalidate();

            if (!opening)
                s.Card.Visible = false;
        }

        // Get bounds where overlays should draw
        private Rectangle GetTargetDisplayRect()
        {
            // If host is BeepiForm, use its adjusted client rect; else use host.ClientRectangle
            if (_host is BeepiForm beepi) return beepi.GetAdjustedClientRectangle();
            return _host.ClientRectangle;
        }
        private static float EaseOutCubic(float x) => 1 - (float)Math.Pow(1 - x, 3);

        private Rectangle ComputeStartBounds(_DialogCard card, BeepDialogKind kind)
        {
            var area = GetTargetDisplayRect();
            var size = card.PreferredDialogSize(area.Size);

            return kind switch
            {
                BeepDialogKind.TopSheet => new Rectangle(area.Left + (area.Width - size.Width) / 2, area.Top - size.Height, size.Width, size.Height),
                BeepDialogKind.LeftDrawer => new Rectangle(area.Left - size.Width, area.Top, size.Width, area.Height),
                BeepDialogKind.RightDrawer => new Rectangle(area.Right, area.Top, size.Width, area.Height),
                _ /* Centered */          => new Rectangle(area.Left + (area.Width - size.Width) / 2, area.Top + (area.Height - size.Height) / 2 + 30, size.Width, size.Height) // slight offset
            };
        }

        private Rectangle ComputeEndBounds(_DialogCard card, BeepDialogKind kind)
        {
            var area = GetTargetDisplayRect();
            var size = card.PreferredDialogSize(area.Size);

            return kind switch
            {
                BeepDialogKind.TopSheet => new Rectangle(area.Left + (area.Width - size.Width) / 2, area.Top, size.Width, Math.Min(size.Height, (int)(area.Height * 0.88))),
                BeepDialogKind.LeftDrawer => new Rectangle(area.Left, area.Top, Math.Min(size.Width, (int)(area.Width * 0.4)), area.Height),
                BeepDialogKind.RightDrawer => new Rectangle(area.Right - Math.Min(size.Width, (int)(area.Width * 0.4)), area.Top, Math.Min(size.Width, (int)(area.Width * 0.4)), area.Height),
                _ /* Centered */          => new Rectangle(area.Left + (area.Width - size.Width) / 2, area.Top + (area.Height - size.Height) / 2, size.Width, size.Height)
            };
        }

        private static Rectangle Lerp(Rectangle a, Rectangle b, float t)
        {
            int LerpI(int x, int y) => x + (int)Math.Round((y - x) * t);
            return new Rectangle(LerpI(a.X, b.X), LerpI(a.Y, b.Y), LerpI(a.Width, b.Width), LerpI(a.Height, b.Height));
        }
        private static float Lerp(float a, float b, float t) => a + (b - a) * t;
        private static Rectangle Snap(Rectangle r) => new Rectangle(r.X, r.Y, Math.Max(1, r.Width), Math.Max(1, r.Height));

        private void Owner_KeyDown(object? sender, KeyEventArgs e)
        {
            if (_stack.Count == 0) return;
            var top = _stack[^1];
            if (e.KeyCode == Keys.Escape && top.Options.EscToClose)
            { e.SuppressKeyPress = true; _ = CloseTopAsync(BeepDialogResult.Cancel); }
            else if (e.KeyCode == Keys.Enter)
            { if (top.Card.TryTriggerDefault()) e.SuppressKeyPress = true; }
        }

        private void Owner_Resize(object? sender, EventArgs e)
        {
            if (_stack.Count == 0) return;
            var area = GetTargetDisplayRect();
            foreach (var s in _stack)
            {
                s.Overlay.Bounds = area;
                if (s.Card.Visible)
                    s.Card.Bounds = ComputeEndBounds(s.Card, s.Options.Kind);
            }
        }

        private sealed class _Shown
        {
            public _Shown(_OverlayPanel overlay, _DialogCard card, BeepDialogOptions options)
            { Overlay = overlay; Card = card; Options = options; }
            public _OverlayPanel Overlay { get; }
            public _DialogCard Card { get; }
            public BeepDialogOptions Options { get; }
            public TaskCompletionSource<BeepDialogResult>? Tcs { get; set; }
        }
    }
}
