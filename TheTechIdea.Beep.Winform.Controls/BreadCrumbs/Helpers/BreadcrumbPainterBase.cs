using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers
{
    internal abstract class BreadcrumbPainterBase : IBreadcrumbPainter
    {
        protected BaseControl Owner;
        protected IBeepTheme Theme;
        protected Font TextFont;
        protected bool ShowIcons;

        // small cache for measured text
        private readonly Dictionary<(string, int), Size> _textMeasureCache = new();

        public virtual void Initialize(BaseControl owner, IBeepTheme theme, Font textFont, bool showIcons)
        {
            Owner = owner;
            Theme = theme;
            TextFont = textFont;
            ShowIcons = showIcons;
            _textMeasureCache.Clear();
        }

        protected Size MeasureText(Graphics g, string text)
        {
            text ??= string.Empty;
            int key = TextFont?.GetHashCode() ?? 0;
            var cacheKey = (text, key);
            if (_textMeasureCache.TryGetValue(cacheKey, out var size))
                return size;
            size = TextRenderer.MeasureText(text, TextFont ?? SystemFonts.DefaultFont);
            _textMeasureCache[cacheKey] = size;
            return size;
        }

        public abstract Rectangle CalculateItemRect(Graphics g, SimpleItem item, int x, int y, int height, bool isHovered);
        public abstract void DrawItem(Graphics g, BeepButton button, SimpleItem item, Rectangle rect, bool isHovered, bool isSelected, bool isLast);

        public virtual int DrawSeparator(Graphics g, BeepLabel label, int x, int y, int height, string separatorText, Font textFont, Color separatorColor, int itemSpacing)
        {
            label.Text = separatorText;
            label.ForeColor = separatorColor;
            label.BackColor = Color.Transparent;
            var sepSize = TextRenderer.MeasureText(separatorText ?? string.Empty, textFont ?? SystemFonts.DefaultFont);
            var sepRect = new Rectangle(x + itemSpacing, y, sepSize.Width, height);
            label.Draw(g, sepRect);
            return sepRect.Width + itemSpacing;
        }
    }
}
