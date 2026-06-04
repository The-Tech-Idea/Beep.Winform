namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{
    public enum IconPlacement { TopRight, TopLeft, MiddleRight, MiddleLeft, BottomRight }

    public static class BaseControlDrawingHandlers
    {
        public static DrawExternalHandler SvgIcon(string svgPath, IconPlacement placement = IconPlacement.TopRight, int size = 14, int offset = 4)
        {
            return (Graphics parentGraphics, Rectangle childBounds) =>
            {
                if (parentGraphics is null || childBounds.IsEmpty) return;
                int x = placement switch
                {
                    IconPlacement.TopLeft or IconPlacement.MiddleLeft => childBounds.Left + offset,
                    _ => childBounds.Right - size - offset
                };
                int y = placement switch
                {
                    IconPlacement.TopRight or IconPlacement.TopLeft => childBounds.Top + offset,
                    IconPlacement.BottomRight => childBounds.Bottom - size - offset,
                    _ => childBounds.Top + (childBounds.Height - size) / 2
                };
                try { Styling.ImagePainters.StyledImagePainter.Paint(parentGraphics, new Rectangle(x, y, size, size), svgPath); }
                catch { }
            };
        }

        public static DrawExternalHandler Underline(Color color, int thickness = 2, int offsetY = 3)
        {
            return (Graphics parentGraphics, Rectangle childBounds) =>
            {
                if (parentGraphics is null || childBounds.IsEmpty) return;
                int y = childBounds.Bottom + offsetY;
                using var pen = new Pen(color, thickness);
                parentGraphics.DrawLine(pen, childBounds.Left + 2, y, childBounds.Right - 2, y);
            };
        }

        public static DrawExternalHandler CounterBadge(int count, Color backColor, Color foreColor, int size = 20)
        {
            return (Graphics parentGraphics, Rectangle childBounds) =>
            {
                if (parentGraphics is null || childBounds.IsEmpty) return;
                string text = count > 99 ? "99+" : count.ToString();
                int x = childBounds.Right - size / 2;
                int y = childBounds.Top - size / 2;
                var badgeRect = new Rectangle(x, y, size, size);

                parentGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var backBrush = new SolidBrush(backColor);
                parentGraphics.FillEllipse(backBrush, badgeRect);

                using var font = new Font("Segoe UI", text.Length == 1 ? 10f : 8f, FontStyle.Bold);
                using var textBrush = new SolidBrush(foreColor);
                using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                parentGraphics.DrawString(text, font, textBrush, badgeRect, fmt);
            };
        }

        public static DrawExternalHandler FocusRing(Color color, int thickness = 2, int padding = 2)
        {
            return (Graphics parentGraphics, Rectangle childBounds) =>
            {
                if (parentGraphics is null || childBounds.IsEmpty) return;
                var ringRect = new Rectangle(childBounds.X - padding, childBounds.Y - padding, childBounds.Width + padding * 2, childBounds.Height + padding * 2);
                parentGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                int radius = Math.Min(4, ringRect.Height / 6);
                using var path = GraphicsExtensions.GetRoundedRectPath(ringRect, radius);
                using var pen = new Pen(color, thickness);
                parentGraphics.DrawPath(pen, path);
            };
        }

        // Pre-built convenience methods using SvgIcon
        public static DrawExternalHandler LoadingSpinner(int size = 20, int offset = 4) => SvgIcon(SvgsUIcons.Activity.Loading, IconPlacement.MiddleRight, size, offset);
        public static DrawExternalHandler RequiredIndicator(int size = 8, int offset = 4) => SvgIcon(SvgsUIcons.Common.Star, IconPlacement.TopRight, size, offset);
        public static DrawExternalHandler LockOverlay(int size = 14, int offset = 4) => SvgIcon(SvgsUIcons.Common.Lock, IconPlacement.MiddleRight, size, offset);
        public static DrawExternalHandler SearchIcon(int size = 14, int offset = 4) => SvgIcon(SvgsUIcons.Common.Search, IconPlacement.MiddleLeft, size, offset);
        public static DrawExternalHandler ChevronDown(int size = 12, int offset = 4) => SvgIcon(SvgsUIcons.Common.ChevronDown, IconPlacement.MiddleRight, size, offset);
        public static DrawExternalHandler ClearButton(int size = 14, int offset = 4) => SvgIcon(SvgsUIcons.Common.Close, IconPlacement.MiddleRight, size, offset);
        public static DrawExternalHandler CalendarIcon(int size = 14, int offset = 4) => SvgIcon(SvgsUIcons.Common.Calendar, IconPlacement.MiddleRight, size, offset);
        public static DrawExternalHandler FilterIcon(int size = 14, int offset = 4) => SvgIcon(SvgsUIcons.Common.Filter, IconPlacement.MiddleRight, size, offset);
        public static DrawExternalHandler EllipsisButton(int size = 18, int offset = 4) => SvgIcon(SvgsUIcons.Common.Settings, IconPlacement.MiddleRight, size, offset);
    }
}
