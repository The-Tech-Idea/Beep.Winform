using System;
using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// DatePicker - Date selection interface painter with hit areas and hover accents
    /// </summary>
    internal sealed class DatePickerPainter : WidgetPainterBase
    {
        private Rectangle _headerRectCache;
        private Rectangle _iconRectCache;
        private Rectangle _footerRectCache;

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);
            
            // Selected date display
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                40
            );
            
            // Calendar icon
            ctx.IconRect = new Rectangle(
                ctx.HeaderRect.Right - 32,
                ctx.HeaderRect.Y + 4,
                24, 24
            );
            
            // Date format/info area
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - ctx.HeaderRect.Bottom - pad * 2
            );

            // Cache for hit areas
            _headerRectCache = ctx.HeaderRect;
            _iconRectCache = ctx.IconRect;
            _footerRectCache = ctx.FooterRect;
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
            
            // Draw border like an input field
            using var borderPen = new Pen(Theme?.TextBoxBorderColor ?? Theme?.BorderColor ?? Color.FromArgb(150, Color.Black), 1);
            g.DrawPath(borderPen, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var selectedDate = ctx.CustomData.ContainsKey("SelectedDate") ? 
                Convert.ToDateTime(ctx.CustomData["SelectedDate"]) : DateTime.Today;

            // Draw selected date
            using var dateFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 14f, FontStyle.Regular);
            using var dateBrush = new SolidBrush(Theme?.ForeColor ?? Color.FromArgb(200, Color.Black));
            string dateText = selectedDate.ToString("dddd, MMMM dd, yyyy");
            
            var textRect = new Rectangle(ctx.HeaderRect.X + 6, ctx.HeaderRect.Y, ctx.HeaderRect.Width - 40 - 6, ctx.HeaderRect.Height);
            var dateFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(dateText, dateFont, dateBrush, textRect, dateFormat);
            
            // Draw calendar icon
            DrawCalendarIcon(g, ctx.IconRect, ctx.AccentColor);
            
            // Draw format/info
            using var infoFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Regular);
            using var infoBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
            string infoText = ctx.CustomData.ContainsKey("FooterText") ? ctx.CustomData["FooterText"].ToString() : "Click to select a different date";
            g.DrawString(infoText, infoFont, infoBrush, ctx.FooterRect);

            // Hover overlays
            if (IsAreaHovered("DatePicker_Header"))
            {
                using var hover = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                g.FillRectangle(hover, _headerRectCache);
            }
            if (IsAreaHovered("DatePicker_Icon"))
            {
                using var pen = new Pen(Theme?.PrimaryColor ?? Color.Blue, 1.5f);
                g.DrawRectangle(pen, _iconRectCache);
            }
        }

        private void DrawCalendarIcon(Graphics g, Rectangle rect, Color accentColor)
        {
            // Draw calendar base
            using var calendarBrush = new SolidBrush(Color.FromArgb(200, accentColor));
            var calendarRect = new Rectangle(rect.X, rect.Y + 4, rect.Width, rect.Height - 4);
            using var calendarPath = CreateRoundedPath(calendarRect, 3);
            g.FillPath(calendarBrush, calendarPath);
            
            // Draw calendar header
            using var headerBrush = new SolidBrush(accentColor);
            var headerRect = new Rectangle(rect.X + 1, rect.Y + 5, rect.Width - 2, 6);
            g.FillRectangle(headerBrush, headerRect);
            
            // Draw calendar rings
            using var ringBrush = new SolidBrush(Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));
            g.FillRectangle(ringBrush, rect.X + 4, rect.Y, 2, 8);
            g.FillRectangle(ringBrush, rect.X + rect.Width - 6, rect.Y, 2, 8);
            
            // Draw calendar grid
            using var gridPen = new Pen(Theme?.BackColor ?? Color.White, 1);
            for (int i = 1; i < 3; i++)
            {
                int y = calendarRect.Y + 8 + i * 4;
                g.DrawLine(gridPen, calendarRect.X + 2, y, calendarRect.Right - 2, y);
            }
            for (int i = 1; i < 4; i++)
            {
                int x = calendarRect.X + 2 + i * 4;
                g.DrawLine(gridPen, x, calendarRect.Y + 8, x, calendarRect.Bottom - 2);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: caret indicator at far right of header
            var caretRect = new Rectangle(_headerRectCache.Right - 16, _headerRectCache.Y + (_headerRectCache.Height - 10) / 2, 12, 10);
            using var pen = new Pen(Color.FromArgb(160, Theme?.ForeColor ?? Color.Black), 1.5f);
            g.DrawLine(pen, caretRect.Left, caretRect.Top, caretRect.Left + caretRect.Width / 2, caretRect.Bottom);
            g.DrawLine(pen, caretRect.Right, caretRect.Top, caretRect.Left + caretRect.Width / 2, caretRect.Bottom);
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            
            if (!_headerRectCache.IsEmpty)
            {
                owner.AddHitArea("DatePicker_Header", _headerRectCache, null, () =>
                {
                    ctx.CustomData["OpenDatePicker"] = true;
                    notifyAreaHit?.Invoke("DatePicker_Header", _headerRectCache);
                    Owner?.Invalidate();
                });
            }
            if (!_iconRectCache.IsEmpty)
            {
                owner.AddHitArea("DatePicker_Icon", _iconRectCache, null, () =>
                {
                    ctx.CustomData["OpenDatePickerFromIcon"] = true;
                    notifyAreaHit?.Invoke("DatePicker_Icon", _iconRectCache);
                    Owner?.Invalidate();
                });
            }
            if (!_footerRectCache.IsEmpty)
            {
                owner.AddHitArea("DatePicker_Footer", _footerRectCache, null, () =>
                {
                    ctx.CustomData["FooterClicked"] = true;
                    notifyAreaHit?.Invoke("DatePicker_Footer", _footerRectCache);
                    Owner?.Invalidate();
                });
            }
        }
    }
}