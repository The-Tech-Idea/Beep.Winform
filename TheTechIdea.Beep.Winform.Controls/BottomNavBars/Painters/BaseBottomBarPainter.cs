using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.BottomNavBars.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal abstract class BaseBottomBarPainter : IBottomBarPainter
    {
        protected BeepBottomBarLayoutHelper _layoutHelper;

        public virtual string Name => "BaseBottomBarPainter";

        public virtual void Dispose() { }

        public virtual void CalculateLayout(BottomBarPainterContext context)
        {
            // Use the layout helper provided in context if present; otherwise create a new one
            _layoutHelper = context.LayoutHelper ?? new BeepBottomBarLayoutHelper();
            _layoutHelper.EnsureLayout(context.Bounds, context.Items, context.CTAIndex, context.SelectedIndex);
        }

        public virtual void RegisterHitAreas(BottomBarPainterContext context)
        {
            // Default: do not register hit areas directly; the control's hit helper will register them.
        }

        public virtual void Paint(BottomBarPainterContext context)
        {
            if (context.Graphics == null) return;
            // Draw background
            using (var b = new SolidBrush(context.BarBackColor == Color.Empty ? Color.White : context.BarBackColor))
            {
                context.Graphics.FillRectangle(b, context.Bounds);
            }

            // Draw selection indicator (use animated values if present in context)
            var indicatorRect = _layoutHelper.GetIndicatorRect();
            RectangleF indicator = indicatorRect;
            try
            {
                if (context.AnimatedIndicatorWidth > 0f)
                {
                    indicator = new RectangleF(context.AnimatedIndicatorX, indicatorRect.Top, context.AnimatedIndicatorWidth, indicatorRect.Height);
                }
            }
            catch { }
            using (var brush = new SolidBrush(Color.FromArgb(40, context.AccentColor)))
            using (var gp = new System.Drawing.Drawing2D.GraphicsPath())
            {
                int radius = (int)Math.Round(indicator.Height / 2f);
                var rect = Rectangle.Round(indicator);
                // Draw rounded pill-like indicator
                gp.AddArc(rect.Left, rect.Top, radius * 2, radius * 2, 180, 90);
                gp.AddArc(rect.Right - radius * 2, rect.Top, radius * 2, radius * 2, 270, 90);
                gp.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                gp.AddArc(rect.Left, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                gp.CloseFigure();
                context.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                context.Graphics.FillPath(brush, gp);
                context.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }

            // Draw items
            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                var item = context.Items[i];
                PaintMenuItem(context.Graphics, item, rects[i], context);
            }
        }

        protected virtual void PaintMenuItem(Graphics g, SimpleItem item, Rectangle rect, BottomBarPainterContext context)
        {
            // draw icon
            var iconRect = new Rectangle(rect.Left + (rect.Width - 24) / 2, rect.Top + 4, 24, 24);
            context.ImagePainter.ImagePath = string.IsNullOrEmpty(item?.ImagePath) ? context.DefaultImagePath : item.ImagePath;
            context.ImagePainter.ImageEmbededin = ImageEmbededin.Button;
            context.ImagePainter.DrawImage(g, iconRect);

            // draw badge if present
            if (!string.IsNullOrEmpty(item.BadgeText))
            {
                var badgeText = item.BadgeText;
                using (var badgeFont = new Font("Segoe UI", 8f, FontStyle.Bold))
                {
                    SizeF textSize = g.MeasureString(badgeText, badgeFont);
                    int padding = 6;
                    int badgeW = Math.Max((int)textSize.Width + padding, 16);
                    int badgeH = Math.Max((int)textSize.Height + 4, 12);
                    int badgeX = iconRect.Right - badgeW / 2;
                    int badgeY = iconRect.Top - badgeH / 2;

                    var badgeRect = new Rectangle(badgeX, badgeY, badgeW, badgeH);
                    using (var brush = new SolidBrush(item.BadgeBackColor))
                    {
                        if (item.BadgeShape == BadgeShape.Circle)
                        {
                            int size = Math.Max(badgeW, badgeH);
                            var circRect = new Rectangle(iconRect.Right - size / 2, iconRect.Top - size / 2, size, size);
                            g.FillEllipse(brush, circRect);
                        }
                        else if (item.BadgeShape == BadgeShape.RoundedRectangle)
                        {
                            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(badgeRect, badgeH / 2))
                            {
                                g.FillPath(brush, path);
                            }
                        }
                        else
                        {
                            g.FillRectangle(brush, badgeRect);
                        }
                    }
                    using (var brushFore = new SolidBrush(item.BadgeForeColor))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString(badgeText, badgeFont, brushFore, badgeRect, sf);
                    }
                }
            }

            // draw label
            bool isSelected = context.SelectedIndex >= 0 && context.SelectedIndex < context.Items.Count && context.Items[context.SelectedIndex] == item;
            bool isHovered = context.HoverIndex >= 0 && context.HoverIndex < context.Items.Count && context.Items[context.HoverIndex] == item;
            using (var font = new Font("Segoe UI", 9f))
            using (var brush = new SolidBrush(isSelected ? context.AccentColor : (isHovered ? context.BarHoverForeColor : context.BarForeColor)))
            {
                var textRect = new Rectangle(rect.Left + 2, iconRect.Bottom + 2, rect.Width - 4, rect.Height - iconRect.Height - 6);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text, font, brush, textRect, sf);
            }
        }
    }
}
