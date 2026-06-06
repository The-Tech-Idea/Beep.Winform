using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public abstract class BeepCellTemplateBase : BaseControl
    {
        public CalendarCellContext CurrentContext { get; private set; }

        protected CalendarEvent Event => CurrentContext?.Event;
        protected DateTime Date => CurrentContext?.Date ?? System.DateTime.Today;
        protected CalendarViewMode ViewMode => CurrentContext?.ViewMode ?? CalendarViewMode.Week4;

        protected string Metadatum(string key, string fallback = "")
        {
            if (Event?.Metadata == null) return fallback;
            return Event.Metadata.TryGetValue(key, out var value) ? value : fallback;
        }

        protected Color ParseColor(string key, Color fallback)
        {
            var hex = Metadatum(key, "");
            if (string.IsNullOrWhiteSpace(hex)) return fallback;
            try { return ColorTranslator.FromHtml(hex); }
            catch { return fallback; }
        }

        protected float ParseFloat(string key, float fallback = 0f)
        {
            var s = Metadatum(key, "");
            if (string.IsNullOrWhiteSpace(s)) return fallback;
            return float.TryParse(s, out var v) ? v : fallback;
        }

        protected int ParseInt(string key, int fallback = 0)
        {
            var s = Metadatum(key, "");
            if (string.IsNullOrWhiteSpace(s)) return fallback;
            return int.TryParse(s, out var v) ? v : fallback;
        }

        public virtual void SetContext(CalendarCellContext ctx)
        {
            CurrentContext = ctx;
            OnContextChanged();
        }

        protected virtual void OnContextChanged()
        {
        }

        protected override bool AllowBaseControlClear => false;

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            if (rectangle.Width <= 0 || rectangle.Height <= 0) return;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            DrawCellContent(graphics, rectangle);
        }

        protected override void DrawContent(Graphics g)
        {
            if (DrawingRect.Width <= 0 || DrawingRect.Height <= 0) return;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            DrawCellContent(g, DrawingRect);
        }

        protected abstract void DrawCellContent(Graphics g, Rectangle rect);
    }
}
