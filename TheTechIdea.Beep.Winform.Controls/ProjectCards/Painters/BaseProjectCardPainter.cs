using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;

namespace TheTechIdea.Beep.Winform.Controls.ProjectCards.Painters
{
    public abstract class BaseProjectCardPainter : IProjectCardPainter
    {
        public abstract string Key { get; }
        public abstract void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProjectCard owner, IReadOnlyDictionary<string, object> p);
        public virtual void UpdateHitAreas(BeepProjectCard owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> parameters, Action<string, Rectangle> register)
        {
            // Default: no interactive areas. Concrete painters should override as needed.
        }

        protected static string GetString(IReadOnlyDictionary<string, object> p, string key, string fallback = "")
            => p != null && p.TryGetValue(key, out var v) && v is string s ? s : fallback;

        protected static string[] GetStringArray(IReadOnlyDictionary<string, object> p, string key)
        {
            if (p == null || !p.TryGetValue(key, out var v) || v == null) return Array.Empty<string>();
            if (v is string[] sa) return sa;
            if (v is IEnumerable<string> seq) return seq.ToArray();
            return Array.Empty<string>();
        }

        protected static float GetFloat(IReadOnlyDictionary<string, object> p, string key, float fallback = 0f)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToSingle(v) : fallback;

        protected static Color GetColor(IReadOnlyDictionary<string, object> p, string key, Color fallback)
            => p != null && p.TryGetValue(key, out var v) && v is Color c ? c : fallback;

        protected static void DrawTitle(Graphics g, Rectangle bounds, IBeepTheme theme, string title)
        {
            if (string.IsNullOrEmpty(title)) return;
            using var f = BeepThemesManager.ToFont(theme.CardHeaderStyle);
            TextRenderer.DrawText(g, title, f, bounds, theme.CardTextForeColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
        }

        protected static void DrawSubtitle(Graphics g, Rectangle bounds, IBeepTheme theme, string subtitle)
        {
            if (string.IsNullOrEmpty(subtitle)) return;
            using var fr = BeepThemesManager.ToFont(theme.CardparagraphStyle);
            using var f = new Font(fr.FontFamily, Math.Max(8, fr.Size), FontStyle.Regular);
            TextRenderer.DrawText(g, subtitle, f, bounds, theme.CardTextForeColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
        }

        protected static Rectangle Inset(Rectangle r, int all) => new Rectangle(r.X + all, r.Y + all, Math.Max(0, r.Width - all * 2), Math.Max(0, r.Height - all * 2));
    }
}
