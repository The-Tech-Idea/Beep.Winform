using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using static System.Windows.Forms.Design.AxImporter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{

    internal sealed class BeepToastHost : Panel
    {
        private readonly Control _owner;
        private readonly Queue<BeepToastOptions> _queue = new();
        private readonly List<BeepToastCard> _active = new();
        private const int MaxVisible = 3;

        public BeepToastHost(Control owner)
        {
            _owner = owner;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
        }

        public void Enqueue(BeepToastOptions o)
        {
            _queue.Enqueue(o);
            TryShowNext();
        }

        private async void TryShowNext()
        {
            if (_active.Count >= MaxVisible) return;
            if (_queue.Count == 0) return;

            var next = _queue.Dequeue();
            var card = new BeepToastCard(_owner, next) { Visible = false };
            Controls.Add(card);
            card.BringToFront();
            _active.Add(card);
            Relayout();

            await card.AnimateInAsync();
            _ = AutoHide(card, next.DurationMs);
        }

        private async Task AutoHide(BeepToastCard card, int ms)
        {
            await Task.Delay(Math.Max(1200, ms));
            await card.AnimateOutAsync();
            Controls.Remove(card);
            card.Dispose();
            _active.Remove(card);
            Relayout();
            TryShowNext();
        }

        public void Relayout()
        {
            var area = ClientRectangle;
            int y = area.Bottom - 16;
            foreach (var card in _active.AsEnumerable().Reverse())
            {
                var w = Math.Min(card.PreferredWidth, Math.Max(240, area.Width / 3));
                var h = card.PreferredHeight;
                var x = area.Right - w - card.Options.Margin.Right;
                y -= h + card.Options.Margin.Bottom;
                card.Bounds = new Rectangle(x, y, w, h);
                y -= card.Options.Margin.Top;
            }
        }

        public void ApplyThemeFrom(object? themeObj)
        {
            var theme = themeObj as BeepTheme ?? ThemeAccess.GetCurrentTheme(_owner);
            foreach (var card in _active) card.ApplyThemeFrom(theme);
            Relayout();
        }

    }
}
