using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.StatusCards.Painters
{
    public abstract class BaseStatCardPainter : IStatCardPainter
    {
        public abstract string Key { get; }
        public abstract void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepStatCard owner, IReadOnlyDictionary<string, object> parameters);

        protected static string GetString(IReadOnlyDictionary<string, object> p, string key, string fallback = "")
            => p != null && p.TryGetValue(key, out var v) && v is string s ? s : fallback;

        protected static float[] GetFloatArray(IReadOnlyDictionary<string, object> p, string key)
        {
            if (p == null || !p.TryGetValue(key, out var v) || v == null) return Array.Empty<float>();
            if (v is float[] fa) return fa;
            if (v is IEnumerable<float> fseq) return fseq.ToArray();
            if (v is IEnumerable<double> dseq) return dseq.Select(x => (float)x).ToArray();
            if (v is double[] da) return da.Select(x => (float)x).ToArray();
            return Array.Empty<float>();
        }

        protected static string[] GetStringArray(IReadOnlyDictionary<string, object> p, string key)
        {
            if (p == null || !p.TryGetValue(key, out var v) || v == null) return Array.Empty<string>();
            if (v is string[] sa) return sa;
            if (v is IEnumerable<string> seq) return seq.ToArray();
            return Array.Empty<string>();
        }

        protected static Color GetColor(IReadOnlyDictionary<string, object> p, string key, Color fallback)
            => p != null && p.TryGetValue(key, out var v) && v is Color c ? c : fallback;

        protected static float GetFloat(IReadOnlyDictionary<string, object> p, string key, float fallback = 0f)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToSingle(v) : fallback;

        protected static void DrawHeader(Graphics g, Rectangle bounds, IBeepTheme theme, string header)
        {
            if (string.IsNullOrEmpty(header)) return;
            using var font = TheTechIdea.Beep.Vis.Modules.Managers.BeepThemesManager.ToFont(theme.CardHeaderStyle);
            using var brush = new SolidBrush(theme.CardTextForeColor.IsEmpty ? Color.Black : theme.CardTextForeColor);
            var textRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, (int)(font.Size * 1.6f));
            TextRenderer.DrawText(g, header, font, textRect, brush.Color, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }

        protected static void DrawValue(Graphics g, Rectangle bounds, IBeepTheme theme, string valueText, float scale = 1.6f)
        {
            if (string.IsNullOrEmpty(valueText)) return;
            using var fontRaw = TheTechIdea.Beep.Vis.Modules.Managers.BeepThemesManager.ToFont(theme.CardparagraphStyle);
            using var font = new Font(fontRaw.FontFamily, Math.Max(8, fontRaw.Size * scale), FontStyle.Bold);
            using var brush = new SolidBrush(theme.CardTextForeColor.IsEmpty ? Color.Black : theme.CardTextForeColor);
            TextRenderer.DrawText(g, valueText, font, bounds, brush.Color, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        protected static Rectangle Inset(Rectangle r, int all) => new Rectangle(r.X + all, r.Y + all, Math.Max(0, r.Width - all * 2), Math.Max(0, r.Height - all * 2));
        protected static Rectangle Below(Rectangle r, int top, int height) => new Rectangle(r.X, r.Y + top, r.Width, height);
    }
}
