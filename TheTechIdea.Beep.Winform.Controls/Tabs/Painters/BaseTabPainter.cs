using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Helpers;


namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public abstract class BaseTabPainter : ITabPainter
    {
        public BeepTabs TabControl { get; set; }
        public IBeepTheme Theme { get; set; }
        public Font TextFont { get; set; }

        protected BeepImage _closeIcon;

        public BaseTabPainter(BeepTabs tabControl)
        {
            TabControl = tabControl;
            _closeIcon = new BeepImage
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg",
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                ApplyThemeOnImage = false,
                Size = new Size(DpiScalingHelper.ScaleValue(24, TabControl), DpiScalingHelper.ScaleValue(24, TabControl))
            };
        }

        public virtual void PaintBackground(Graphics g, Rectangle bounds)
        {
             Color backgroundColor = TabControl.Parent?.BackColor ?? TabControl.BackColor;
             g.Clear(backgroundColor);
        }

        public virtual void PaintHeaderBackground(Graphics g, Rectangle headerBounds)
        {
             Color panelColor = TabControl.Parent?.BackColor ?? TabControl.BackColor;
             var brush = PaintersFactory.GetSolidBrush(panelColor);
             g.FillRectangle(brush, headerBounds);
        }

        public abstract void PaintTab(Graphics g, RectangleF tabRect, int index, bool isSelected, bool isHovered, float alpha = 1.0f);

        public virtual SizeF MeasureTab(Graphics g, int index, Font font)
        {
            if (index < 0 || index >= TabControl.TabPages.Count) return SizeF.Empty;

            string text = TabControl.TabPages[index].Text;
            SizeF textSize = TextUtils.MeasureText(g, text, font);
            
            float width = textSize.Width + (GetScaledTextPadding() * 2);
            float height = textSize.Height + (GetScaledTextPadding() * 2);

            if (TabControl.ShowCloseButtons)
            {
                width += GetScaledCloseButtonSize() + (GetScaledCloseButtonPadding() * 2);
            }
            
            return new SizeF(width, height);
        }

        protected int GetScaledCloseButtonSize() => DpiScalingHelper.ScaleValue(24, TabControl);
        protected int GetScaledCloseButtonPadding() => DpiScalingHelper.ScaleValue(8, TabControl);
        protected int GetScaledTextPadding() => DpiScalingHelper.ScaleValue(12, TabControl);

        protected void DrawCloseButton(Graphics g, RectangleF tabRect, bool vertical)
        {
            RectangleF closeRect = GetCloseButtonRect(tabRect, vertical);
            _closeIcon.Size = new Size((int)closeRect.Width, (int)closeRect.Height);
            _closeIcon.DrawingRect = Rectangle.Truncate(closeRect);
            _closeIcon.Draw(g, Rectangle.Truncate(closeRect));
        }

        public RectangleF GetCloseButtonRect(RectangleF tabRect, bool vertical)
        {
            int scaledCloseButtonSize = GetScaledCloseButtonSize();
            int scaledCloseButtonPadding = GetScaledCloseButtonPadding();

            if (vertical)
            {
                return new RectangleF(
                    tabRect.X + (tabRect.Width - scaledCloseButtonSize) / 2,
                    tabRect.Bottom - scaledCloseButtonSize - scaledCloseButtonPadding,
                    scaledCloseButtonSize,
                    scaledCloseButtonSize
                );
            }
            return new RectangleF(
                tabRect.Right - scaledCloseButtonSize - scaledCloseButtonPadding,
                tabRect.Top + (tabRect.Height - scaledCloseButtonSize) / 2,
                scaledCloseButtonSize,
                scaledCloseButtonSize
            );
        }

        protected GraphicsPath GetRoundedRect(RectangleF rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int maxRadius = (int)Math.Min(rect.Width / 2f, rect.Height / 2f);
            int safeRadius = Math.Max(0, Math.Min(radius, maxRadius));
            if (safeRadius < 1)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = safeRadius * 2;
            RectangleF arc = new RectangleF(rect.Location, new SizeF(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
        
        protected void DrawTabText(Graphics g, RectangleF tabRect, string text, bool isSelected, bool vertical, float alpha = 1.0f)
        {
             // Use theme helpers for consistent color retrieval
             Color baseColor = TheTechIdea.Beep.Winform.Controls.Tabs.Helpers.TabThemeHelpers.GetTabTextColor(
                 Theme, 
                 Theme != null, 
                 isSelected);
             Color textColor = Color.FromArgb((int)(alpha * 255), baseColor.R, baseColor.G, baseColor.B);

             // Use TextFont when available, otherwise TabControl.Font
             Font baseFont = TextFont ?? TabControl.Font;
             using (Font font = new Font(baseFont, isSelected ? FontStyle.Bold : FontStyle.Regular))
             {
                var textBrush = PaintersFactory.GetSolidBrush(textColor);
                if (!vertical)
                {
                    SizeF textSize = TextUtils.MeasureText(g, text, font);
                    PointF textPoint = new PointF(tabRect.X + GetScaledTextPadding(), tabRect.Y + (tabRect.Height - textSize.Height) / 2);
                    g.DrawString(text, font, textBrush, textPoint);
                }
                else
                {
                    GraphicsState state = g.Save();
                    g.TranslateTransform(tabRect.X + tabRect.Width / 2, tabRect.Y + tabRect.Height / 2);
                    g.RotateTransform(90);
                    SizeF textSize = TextUtils.MeasureText(g, text, font);
                    PointF textPoint = new PointF(-textSize.Width / 2, -textSize.Height / 2);
                    g.DrawString(text, font, textBrush, textPoint);
                    g.Restore(state);
                }
             }
        }
    }
}
