using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class PillBottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "Pill";
        public override void Paint(BottomBarPainterContext context)
        {
            base.CalculateLayout(context);
            using (var b = new SolidBrush(context.BarBackColor == Color.Empty ? Color.FromArgb(250, 250, 250) : context.BarBackColor))
            {
                context.Graphics.FillRectangle(b, context.Bounds);
            }

            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                var r = rects[i];
                if (i == context.SelectedIndex)
                {
                    // Draw expanded rounded pill containing icon and label
                    var pillRect = new Rectangle(r.Left - 6, r.Top + 6, r.Width + 12, r.Height - 12);
                    // if animated indicator is present use it to drive position/width
                    try
                    {
                        if (context.AnimatedIndicatorWidth > 0)
                        {
                            float pulse = 1.0f + 0.02f * context.AnimationPhase; // slight expansion
                            int width = (int)((int)context.AnimatedIndicatorWidth + 12);
                            int w = Math.Max(8, (int)(width * pulse));
                            int x = (int)context.AnimatedIndicatorX - 6 - (w - width) / 2;
                            pillRect = new Rectangle(x, r.Top + 6, w, r.Height - 12);
                        }
                    }
                    catch { }
                    int radius = pillRect.Height / 2;
                    using (var gp = new GraphicsPath())
                    {
                        gp.AddArc(pillRect.Left, pillRect.Top, radius * 2, radius * 2, 180, 90);
                        gp.AddArc(pillRect.Right - radius * 2, pillRect.Top, radius * 2, radius * 2, 270, 90);
                        gp.AddArc(pillRect.Right - radius * 2, pillRect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                        gp.AddArc(pillRect.Left, pillRect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                        gp.CloseFigure();
                        using (var br = new SolidBrush(context.AccentColor))
                        using (var sbr = new SolidBrush(Color.FromArgb(22, context.AccentColor)))
                        {
                            // subtle shadow fill
                            context.Graphics.FillPath(sbr, gp);
                            // pill fill
                            context.Graphics.FillPath(br, gp);
                        }
                        // optional stroke
                        var penBase = context.NavigationBorderColor == Color.Empty ? (context.BarForeColor == Color.Empty ? Color.Black : context.BarForeColor) : context.NavigationBorderColor;
                        var pillStrokeColor = Color.FromArgb(40, penBase.R, penBase.G, penBase.B);
                        using (var pen = new Pen(pillStrokeColor, 1f))
                        {
                            context.Graphics.DrawPath(pen, gp);
                        }
                    }
                }

                var item = context.Items[i];
                // hover effect for unselected items
                if (i == context.HoverIndex && i != context.SelectedIndex)
                {
                    var hoverRect = new Rectangle(r.Left + 6, r.Top + 8, r.Width - 12, r.Height - 16);
                    int radius = hoverRect.Height / 2;
                    using (var gp = new GraphicsPath())
                    {
                        var hoverBrushColor = context.BarHoverBackColor == Color.Empty ? (context.BarForeColor == Color.Empty ? Color.FromArgb(18, Color.Black) : Color.FromArgb(18, context.BarForeColor)) : Color.FromArgb(18, context.BarHoverBackColor);
                        using (var br = new SolidBrush(hoverBrushColor))
                        {
                            gp.AddArc(hoverRect.Left, hoverRect.Top, radius * 2, radius * 2, 180, 90);
                            gp.AddArc(hoverRect.Right - radius * 2, hoverRect.Top, radius * 2, radius * 2, 270, 90);
                            gp.AddArc(hoverRect.Right - radius * 2, hoverRect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                            gp.AddArc(hoverRect.Left, hoverRect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                            gp.CloseFigure();
                            context.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            context.Graphics.FillPath(br, gp);
                            context.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                        }
                    }
                }
                // For the selected pill, we render icon + label inside the pill differently
                if (i == context.SelectedIndex)
                {
                    PaintPillStyleItem(context.Graphics, item, r, context);
                }
                else
                {
                    PaintMenuItem(context.Graphics, item, r, context);
                }
            }
        }

        public override void RegisterHitAreas(BottomBarPainterContext context)
        {
            if (context == null || context.HitTest == null) return;
            var selected = context.SelectedIndex;
            if (selected < 0 || selected >= context.Items.Count) return;
            var r = _layoutHelper.GetItemRect(selected);
            var pillRect = new Rectangle(r.Left - 6, r.Top + 6, r.Width + 12, r.Height - 12);
            // Replace or register the hit area for the selected item so the pill area is clickable
            context.HitTest.AddHitArea($"BottomBarItem_{selected}", pillRect, null, () => context.OnItemClicked?.Invoke(selected, MouseButtons.Left));
        }

        private void PaintPillStyleItem(Graphics g, SimpleItem item, Rectangle rect, BottomBarPainterContext context)
        {
            // Use a left-aligned icon + label inside the pill rectangle (expanded managed earlier)
            var pillRect = new Rectangle(rect.Left - 6, rect.Top + 6, rect.Width + 12, rect.Height - 12);
            // override with animated indicator if present
            try
            {
                if (context.AnimatedIndicatorWidth > 0)
                {
                    pillRect = new Rectangle((int)context.AnimatedIndicatorX - 6, rect.Top + 6, (int)context.AnimatedIndicatorWidth + 12, rect.Height - 12);
                }
            }
            catch { }
            int iconSize = Math.Min(22, rect.Height - 12);
            var iconRect = new Rectangle(pillRect.Left + 12, pillRect.Top + (pillRect.Height - iconSize) / 2, iconSize, iconSize);

            context.ImagePainter.ImagePath = string.IsNullOrEmpty(item.ImagePath) ? context.DefaultImagePath : item.ImagePath;
            context.ImagePainter.ImageEmbededin = ImageEmbededin.Button;
            var prevFill = context.ImagePainter.FillColor;
            context.ImagePainter.FillColor = context.OnAccentColor == Color.Empty ? Color.White : context.OnAccentColor;
            // white icon in pill
            var prevTheme = context.ImagePainter.ApplyThemeOnImage;
            context.ImagePainter.ApplyThemeOnImage = false;
            context.ImagePainter.DrawImage(g, iconRect);
            context.ImagePainter.ApplyThemeOnImage = prevTheme;
            context.ImagePainter.FillColor = prevFill;

            // draw label to the right
            using (var font = new Font("Segoe UI", 9f, FontStyle.Bold))
            using (var brush = new SolidBrush(context.OnAccentColor == Color.Empty ? Color.White : context.OnAccentColor))
            {
                var textRect = new Rectangle(iconRect.Right + 8, pillRect.Top, pillRect.Right - (iconRect.Right + 12), pillRect.Height);
                var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text, font, brush, textRect, sf);
            }

            // draw badge if present
            if (!string.IsNullOrEmpty(item.BadgeText))
            {
                using (var badgeFont = new Font("Segoe UI", 8f, FontStyle.Bold))
                {
                    SizeF textSize = g.MeasureString(item.BadgeText, badgeFont);
                    int padding = 6;
                    int badgeW = Math.Max((int)textSize.Width + padding, 16);
                    int badgeH = Math.Max((int)textSize.Height + 4, 12);
                    int badgeX = iconRect.Right - badgeW / 2;
                    int badgeY = iconRect.Top - badgeH / 2;

                    var badgeRect = new Rectangle(badgeX, badgeY, badgeW, badgeH);
                    using (var brush = new SolidBrush(item.BadgeBackColor))
                    {
                        using (var path = new GraphicsPath())
                        {
                            path.AddEllipse(badgeRect);
                            g.FillPath(brush, path);
                        }
                    }
                    using (var brushFore = new SolidBrush(item.BadgeForeColor))
                    {
                        var sf2 = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString(item.BadgeText, badgeFont, brushFore, badgeRect, sf2);
                    }
                }
            }
        }
    }
}
