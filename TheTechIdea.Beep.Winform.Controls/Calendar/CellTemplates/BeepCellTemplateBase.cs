using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.CellTemplates
{
    public abstract class BeepCellTemplateBase : BaseControl
    {
        // Shared cell scaffold. Every template draws the same frame - accent bar, padding,
        // title line, detail rows, time line - so the pad, accent width, line height, fonts and
        // ink colours live here once instead of being re-declared and re-allocated per template.
        protected const int CellPad = 6;
        protected const int CellAccentWidth = 4;
        protected const int CellLineHeight = 16;

        protected internal override Padding StylePadding => new Padding(0);
        public CalendarCellContext? CurrentContext { get; private set; }

        private Font _titleFont;
        private Font _detailFont;

        /// <summary>Title font (9pt). Cached; owned by the cell - do not dispose.</summary>
        protected Font TitleFont => _titleFont ??= CreateCellFont(9f);

        /// <summary>Detail font (7.5pt). Cached; owned by the cell - do not dispose.</summary>
        protected Font DetailFont => _detailFont ??= CreateCellFont(7.5f);

        /// <summary>Primary ink colour from the current theme.</summary>
        protected Color TextColor => _currentTheme?.PrimaryTextColor ?? Color.Black;

        /// <summary>Secondary ink colour (theme ink at reduced alpha).</summary>
        protected Color SecondaryTextColor => Color.FromArgb(180, TextColor);

        protected CalendarEvent Event => CurrentContext?.Event;
        protected DateTime Date => CurrentContext?.Date ?? System.DateTime.Today;
        protected CalendarViewMode ViewMode => CurrentContext?.ViewMode ?? CalendarViewMode.Week4;

        /// <summary>The content rectangle inside the cell, past the accent bar and padding.</summary>
        protected Rectangle CellContentRect(Rectangle rect) => new Rectangle(
            rect.X + CellPad + CellAccentWidth,
            rect.Y + CellPad,
            rect.Width - CellPad * 2 - CellAccentWidth,
            rect.Height - CellPad * 2);

        private Font CreateCellFont(float size) =>
            new Font(Font?.FontFamily ?? FontFamily.GenericSansSerif, size, FontStyle.Regular);

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
            catch (FormatException) { return fallback; }
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

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _titleFont?.Dispose();
            _titleFont = null;
            _detailFont?.Dispose();
            _detailFont = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _titleFont?.Dispose();
                _titleFont = null;
                _detailFont?.Dispose();
                _detailFont = null;
            }
            base.Dispose(disposing);
        }
    }
}
