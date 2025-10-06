using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models; // BeepImage

namespace TheTechIdea.Beep.Winform.Controls.Toolstrips.Painters
{
    internal abstract class TabsBasePainter : IToolStripPainter
    {
        public abstract string Key { get; }

        // Parameterized metrics with defaults; can be overridden via Parameters
        protected virtual int DefaultItemPaddingH => 16;
        protected virtual int DefaultItemPaddingV => 8;
        protected virtual int DefaultSpacing => 8;
        protected virtual int DefaultUnderlineThickness => 2;
        protected virtual int DefaultCornerRadius => 8;
        protected virtual int DefaultIconSize => 16;
        protected virtual int DefaultIconGap => 8;
        protected virtual string DefaultIconPlacement => "Left"; // or "Top"

        private readonly BeepImage _icon = new BeepImage { IsChild = true, ApplyThemeOnImage = true, ImageEmbededin = ImageEmbededin.TabPage };

        // Cache of last computed item rectangles for use by other painters/decorators
        internal List<Rectangle> LastItemRects { get; private set; } = new();

        public virtual void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepToolStrip owner, IReadOnlyDictionary<string, object> parameters)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var bg = new SolidBrush(owner.BackColor);
            g.FillRectangle(bg, bounds);

            if (owner.Buttons == null || owner.Buttons.Count == 0) return;

            // Resolve params
            int padH = GetInt(parameters, "ItemPaddingH", DefaultItemPaddingH);
            int padV = GetInt(parameters, "ItemPaddingV", DefaultItemPaddingV);
            int spacing = GetInt(parameters, "Spacing", DefaultSpacing);
            int iconSize = GetInt(parameters, "IconSize", DefaultIconSize);
            int iconGap = GetInt(parameters, "IconGap", DefaultIconGap);
            string iconPlacement = GetString(parameters, "IconPlacement", DefaultIconPlacement);

            using var font = BeepThemesManager.ToFont(theme.ComboBoxItemFont);

            // Build rects once to enable decorators to use them
            LastItemRects = BuildItemRects(bounds, owner, font, padH, padV, spacing, iconSize, iconGap, iconPlacement);

            for (int i = 0; i < owner.Buttons.Count && i < LastItemRects.Count; i++)
            {
                var item = owner.Buttons[i];
                var text = string.IsNullOrEmpty(item.Text) ? item.Name : item.Text;
                DrawTab(g, LastItemRects[i], theme, owner, item, i, text, font, padH, padV, spacing, iconSize, iconGap, iconPlacement);
            }
        }

        protected List<Rectangle> BuildItemRects(Rectangle bounds, BeepToolStrip owner, Font font, int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement)
        {
            var rects = new List<Rectangle>();
            if (owner.Buttons == null || owner.Buttons.Count == 0) return rects;

            if (owner.Orientation == ToolStripOrientation.Vertical)
            {
                int y = bounds.Top + spacing;
                int x = bounds.Left + spacing;
                int maxWidth = bounds.Width - spacing * 2;
                for (int i = 0; i < owner.Buttons.Count; i++)
                {
                    var item = owner.Buttons[i];
                    string text = string.IsNullOrEmpty(item.Text) ? item.Name : item.Text;
                    var textSize = TextRenderer.MeasureText(text, font);
                    int w = maxWidth;
                    int h = (int)textSize.Height + padV * 2;
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        if (iconPlacement.Equals("Left", StringComparison.OrdinalIgnoreCase)) h = Math.Max(h, iconSize + padV * 2);
                        else h = iconSize + iconGap + (int)textSize.Height + padV * 2;
                    }
                    rects.Add(new Rectangle(x, y, w, h));
                    y += h + spacing;
                }
            }
            else
            {
                int x = bounds.Left + spacing;
                for (int i = 0; i < owner.Buttons.Count; i++)
                {
                    var item = owner.Buttons[i];
                    string text = string.IsNullOrEmpty(item.Text) ? item.Name : item.Text;
                    var textSize = TextRenderer.MeasureText(text, font);
                    int w = (int)textSize.Width + padH * 2;
                    int h = (int)textSize.Height + padV * 2;
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        if (iconPlacement.Equals("Left", StringComparison.OrdinalIgnoreCase)) { w += iconSize + iconGap; h = Math.Max(h, iconSize + padV * 2); }
                        else { h = iconSize + iconGap + (int)textSize.Height + padV * 2; w = Math.Max(w, iconSize + padH * 2); }
                    }
                    rects.Add(new Rectangle(x, bounds.Top + (bounds.Height - h) / 2, w, h));
                    x += w + spacing;
                }
            }
            return rects;
        }

        protected (Rectangle iconRect, Rectangle textRect) GetIconTextRects(Rectangle rect, SimpleItem item, string iconPlacement, int iconSize, int iconGap, int padH, int padV)
        {
            Rectangle iconRect = Rectangle.Empty;
            Rectangle textRect = rect;
            if (item != null && !string.IsNullOrEmpty(item.ImagePath))
            {
                if (iconPlacement.Equals("Left", StringComparison.OrdinalIgnoreCase))
                {
                    iconRect = new Rectangle(rect.Left + padH, rect.Top + (rect.Height - iconSize) / 2, iconSize, iconSize);
                    textRect = new Rectangle(iconRect.Right + iconGap, rect.Top, rect.Right - (iconRect.Right + iconGap) - padH, rect.Height);
                }
                else
                {
                    iconRect = new Rectangle(rect.Left + (rect.Width - iconSize) / 2, rect.Top + padV, iconSize, iconSize);
                    textRect = new Rectangle(rect.Left + padH, iconRect.Bottom + iconGap, rect.Width - padH * 2, rect.Bottom - (iconRect.Bottom + iconGap) - padV);
                }
            }
            return (iconRect, textRect);
        }

        protected void DrawIconAndText(Graphics g, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, bool selected, string text, Font font, Rectangle iconRect, Rectangle textRect)
        {
            if (!iconRect.IsEmpty)
            {
                DrawIconIfAny(g, iconRect, theme, owner, item, selected, iconRect.Width, out _);
            }
            var tc = selected ? theme.MenuItemSelectedForeColor : theme.MenuItemForeColor;
            TextRenderer.DrawText(g, text, font, textRect, tc, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        protected abstract void DrawTab(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, int index, string text, System.Drawing.Font font,
            int padH, int padV, int spacing, int iconSize, int iconGap, string iconPlacement);

        public virtual void UpdateHitAreas(BeepToolStrip owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> parameters, System.Action<string, Rectangle> register)
        {
            if (owner.Buttons == null || owner.Buttons.Count == 0) return;
            int padH = GetInt(parameters, "ItemPaddingH", DefaultItemPaddingH);
            int padV = GetInt(parameters, "ItemPaddingV", DefaultItemPaddingV);
            int spacing = GetInt(parameters, "Spacing", DefaultSpacing);
            int iconSize = GetInt(parameters, "IconSize", DefaultIconSize);
            int iconGap = GetInt(parameters, "IconGap", DefaultIconGap);
            string iconPlacement = GetString(parameters, "IconPlacement", DefaultIconPlacement);
            using var font = BeepThemesManager.ToFont(theme.ComboBoxItemFont);

            var rects = BuildItemRects(bounds, owner, font, padH, padV, spacing, iconSize, iconGap, iconPlacement);
            for (int i = 0; i < rects.Count; i++) register($"Item:{i}", rects[i]);
        }

        protected GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            int r = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(rect.Left, rect.Top, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Top, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected void DrawIconIfAny(Graphics g, Rectangle rect, IBeepTheme theme, BeepToolStrip owner, SimpleItem item, bool selected, int iconSize, out Rectangle iconRect)
        {
            iconRect = Rectangle.Empty;
            if (item == null || string.IsNullOrEmpty(item.ImagePath)) return;
            _icon.BackColor = owner.BackColor;
            _icon.ForeColor = selected ? theme.MenuMainItemSelectedForeColor : theme.MenuMainItemForeColor;
            _icon.ImagePath = item.ImagePath;
            _icon.Size = new Size(iconSize, iconSize);
            _icon.DrawImage(g, rect);
            iconRect = rect;
        }

        protected int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
        protected string GetString(IReadOnlyDictionary<string, object> p, string key, string fallback)
            => p != null && p.TryGetValue(key, out var v) && v is string s ? s : fallback;
        protected int GetParam(IReadOnlyDictionary<string, object> p, string key, int fallback) => GetInt(p, key, fallback);
        protected int GetCorner(IReadOnlyDictionary<string, object> p, string key, int fallback) => GetInt(p, key, fallback);
        protected int GetUnderline(IReadOnlyDictionary<string, object> p, string key, int fallback) => GetInt(p, key, fallback);
    }
}
