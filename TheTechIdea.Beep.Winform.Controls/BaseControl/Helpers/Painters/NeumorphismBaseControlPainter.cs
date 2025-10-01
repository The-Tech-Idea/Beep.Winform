using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Neumorphism painter: soft 3D look using light and dark shadows. Outer container only.
    /// </summary>
    internal sealed class NeumorphismBaseControlPainter : IBaseControlPainter
    {
        private Rectangle _outerRect;
        private Rectangle _drawingRect;
        private const int RADIUS = 10;
        private const int PADDING = 12;

        // Reserved label/helper space
        private int _reserveTop;
        private int _reserveBottom;

        public Rectangle DrawingRect => _drawingRect;
        public Rectangle BorderRect => _outerRect;
        public Rectangle ContentRect => _drawingRect;

        public void UpdateLayout(Base.BaseControl owner)
        {
            _reserveTop = 0;
            _reserveBottom = 0;

            if (owner == null || owner.Width <= 0 || owner.Height <= 0)
            {
                _outerRect = Rectangle.Empty;
                _drawingRect = Rectangle.Empty;
                return;
            }

            // Measure label/helper
            try
            {
                using var g = owner.CreateGraphics();
                if (!string.IsNullOrEmpty(owner.LabelText))
                {
                    float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    _reserveTop = h + 4;
                }

                string supporting = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
                if (!string.IsNullOrEmpty(supporting))
                {
                    float supSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    _reserveBottom = h + 6;
                }
            }
            catch { }

            _outerRect = new Rectangle(2, 2 + _reserveTop, Math.Max(0, owner.Width - 4), Math.Max(0, owner.Height - 4 - _reserveTop - _reserveBottom));
            _drawingRect = Rectangle.Inflate(_outerRect, -PADDING, -PADDING);
            if (_drawingRect.Width <= 0 || _drawingRect.Height <= 0) _drawingRect = Rectangle.Empty;
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null || _outerRect.IsEmpty) return;

            var theme = owner._currentTheme;
            Color baseColor = owner.BackColor != Color.Empty && owner.BackColor != SystemColors.Control
                ? owner.BackColor
                : (theme?.BackColor ?? Color.FromArgb(243, 243, 243));

            var oldSmoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            try
            {
                using (var path = CreateRoundedPath(_outerRect, RADIUS))
                using (var bg = new SolidBrush(baseColor))
                {
                    g.FillPath(bg, path);
                }

                // Soft dual shadows
                DrawNeumorphicShadows(g, _outerRect, RADIUS, baseColor);

                // Optional border (very subtle)
                Color border = ControlPaint.Dark(baseColor, 0.02f);
                using (var path = CreateRoundedPath(_outerRect, RADIUS))
                using (var pen = new Pen(border, 1))
                {
                    g.DrawPath(pen, path);
                }

                // Label/helper
                DrawLabelAndHelper(g, owner);
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
            }
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            if (owner == null || register == null) return;
            if (!_outerRect.IsEmpty)
                register("Neumorphism_Main", _outerRect, owner.TriggerClick);
        }

        public Size GetPreferredSize(Base.BaseControl owner, Size proposedSize)
        {
            int minW = 140, minH = 44;

            int extraTop = 0, extraBottom = 0;
            try
            {
                using var g = owner.CreateGraphics();
                if (!string.IsNullOrEmpty(owner.LabelText))
                {
                    float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    extraTop = h + 4;
                }
                string supporting = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
                if (!string.IsNullOrEmpty(supporting))
                {
                    float supSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    extraBottom = h + 6;
                }
            }
            catch { }

            return new Size(Math.Max(minW, proposedSize.Width), Math.Max(minH + extraTop + extraBottom, proposedSize.Height));
        }

        private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            int d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void DrawNeumorphicShadows(Graphics g, Rectangle rect, int radius, Color baseColor)
        {
            // Light source top-left: draw dark shadow bottom-right and light highlight top-left
            using var dark = new SolidBrush(Color.FromArgb(35, 0, 0, 0));
            using var light = new SolidBrush(Color.FromArgb(110, 255, 255, 255));

            Rectangle br = Rectangle.Inflate(new Rectangle(rect.X + 3, rect.Y + 3, rect.Width, rect.Height), 6, 6);
            Rectangle tl = Rectangle.Inflate(new Rectangle(rect.X - 3, rect.Y - 3, rect.Width, rect.Height), 6, 6);

            using (var pathBR = CreateRoundedPath(br, radius + 6))
                g.FillPath(dark, pathBR);

            using (var pathTL = CreateRoundedPath(tl, radius + 6))
                g.FillPath(light, pathTL);
        }

        private static void DrawLabelAndHelper(Graphics g, Base.BaseControl owner)
        {
            var painter = owner._painter as NeumorphismBaseControlPainter;
            var rectRef = painter?._outerRect ?? owner.ClientRectangle;

            if (!string.IsNullOrEmpty(owner.LabelText))
            {
                float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                int labelHeight = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                var labelRect = new Rectangle(rectRef.Left + 6, Math.Max(0, rectRef.Top - labelHeight - 2), Math.Max(10, rectRef.Width - 12), labelHeight);
                Color labelColor = string.IsNullOrEmpty(owner.ErrorText) ? owner.ForeColor : owner.ErrorColor;
                TextRenderer.DrawText(g, owner.LabelText, lf, labelRect, labelColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }

            string supporting = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
            if (!string.IsNullOrEmpty(supporting))
            {
                float supSize = Math.Max(8f, owner.Font.Size - 1f);
                using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                int supportHeight = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                var supportRect = new Rectangle(rectRef.Left + 6, rectRef.Bottom + 2, Math.Max(10, rectRef.Width - 12), supportHeight);
                Color supportColor = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorColor : owner.ForeColor;
                TextRenderer.DrawText(g, supporting, sf, supportRect, supportColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
        }
    }
}
