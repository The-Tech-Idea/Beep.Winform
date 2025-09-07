using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{
    internal partial class ControlPaintHelper
    {
        private readonly BaseControl _owner;
        private BaseControl OwnerAdv => _owner as BaseControl;

        public ControlPaintHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            UpdateRects();
        }

        public Rectangle DrawingRect { get; set; }
        public Rectangle BorderRectangle { get; set; }
    private bool _rectsDirty = true;

        public void UpdateRects()
        {
            _rectsDirty = false;
            int shadow = _owner.ShowShadow ? _owner.ShadowOffset : 0;
            int border = 0;
            if (_owner.ShowAllBorders || _owner.MaterialBorderVariant == MaterialTextFieldVariant.Outlined)
            {
                border = _owner.BorderThickness;
            }
            var padding = _owner.Padding;

            // Include custom offsets like base BeepControl
            int leftPad = padding.Left + _owner.LeftoffsetForDrawingRect;
            int topPad = padding.Top + _owner.TopoffsetForDrawingRect;
            int rightPad = padding.Right + _owner.RightoffsetForDrawingRect;
            int bottomPad = padding.Bottom + _owner.BottomoffsetForDrawingRect;
            
            // If Material style is enabled and helper exists, use it
            if (_owner.EnableMaterialStyle && _owner._materialHelper != null)
            {
                try
                {
                    _owner._materialHelper.UpdateLayout();
                    var materialContentRect = _owner._materialHelper.GetContentRect();
                    
                    if (materialContentRect.Width > 0 && materialContentRect.Height > 0)
                    {
                        DrawingRect = materialContentRect;
                        return;
                    }
                }
                catch
                {
                    // Fall through to standard layout
                }
            }
            
            // Standard layout (like BeepControl)
            int w = Math.Max(0, _owner.Width - (shadow * 2 + border * 2 + leftPad + rightPad));
            int h = Math.Max(0, _owner.Height - (shadow * 2 + border * 2 + topPad + bottomPad));

            DrawingRect = new Rectangle(
                shadow + border + leftPad,
                shadow + border + topPad,
                w,
                h);

            // Update border rectangle
            int halfPen = (int)Math.Ceiling(_owner.BorderThickness / 2f);
            BorderRectangle = new Rectangle(
                shadow + halfPen,
                shadow + halfPen,
                Math.Max(0, _owner.Width - (shadow + halfPen) * 2),
                Math.Max(0, _owner.Height - (shadow + halfPen) * 2)
            );
        }

        public void InvalidateRects()
        {
            _rectsDirty = true;
        }

        public void EnsureUpdated()
        {
            if (_rectsDirty) UpdateRects();
        }

        // Custom border flag will be held on owner (via BaseControl.IsCustomeBorder),
        // we expose an event-like callback for custom drawing if needed.
        public Action<Graphics> CustomBorderDrawer { get; set; }

        public void Draw(Graphics g)
        {
            if (g == null) return;
            EnsureUpdated();
          

            // Paint outer padding area first to match parent (prevents white gutters)
            try
            {
                var parentBack = (_owner.Parent as Control)?.BackColor ?? _owner.BackColor;
                if (parentBack.A > 0) // not fully transparent
                {
                    using var padBrush = new SolidBrush(parentBack);
                    g.FillRectangle(padBrush, new Rectangle(0, 0, _owner.Width, _owner.Height));
                    g.FillPath(padBrush, _owner.InnerShape);
                }
            }
            catch { }
           
           
            DrawBackground(g);

            if (_owner.ShowShadow)
            {
                DrawShadow(g);
            }

            // If consumer wants full custom border, let them draw; otherwise default borders
            if (!(OwnerAdv?.IsCustomeBorder ?? false))
            {
                if (!_owner.IsFrameless)
                {
                    DrawBorders(g);
                }
            }
            else
            {
                CustomBorderDrawer?.Invoke(g);
            }

            //if (!string.IsNullOrEmpty(_owner.BadgeText))
            //{
            //    //DrawBadge(g);
            //    _owner.DrawBadgeExternally(g,new Rectangle() { Height=20,Width=20});
            //}
        }

        private void DrawBackground(Graphics g)
        {
            // Skip background drawing when Material style is enabled as it's handled in PaintInnerShape
            if (_owner.EnableMaterialStyle)
            {
                return;
            }

            Color backColor = GetEffectiveBackColor();

            if (_owner.UseGradientBackground && _owner.ModernGradientType != ModernGradientType.None)
            {
                DrawModernGradient(g, backColor);
            }
            else if (_owner.UseGradientBackground)
            {
                DrawLinearGradient(g, _owner.GradientStartColor, _owner.GradientEndColor);
            }
            else
            {
                // Material UI Filled variant background
                if (_owner.MaterialBorderVariant == MaterialTextFieldVariant.Filled)
                {
                    backColor = _owner.FilledBackgroundColor;
                }

                using (var brush = new SolidBrush(backColor))
                {
                    FillShape(g, brush, DrawingRect);
                }
            }
        }

        private Color GetEffectiveBackColor()
        {
            var ownerAdv = OwnerAdv;
            if (ownerAdv != null)
            {
                if (!_owner.Enabled) return _owner.DisabledBackColor;
                if (ownerAdv.IsPressed) return _owner.PressedBackColor;
                if (ownerAdv.IsHovered) return _owner.HoverBackColor;
                if (_owner.Focused) return _owner.FocusBackColor;
                if (ownerAdv.IsSelected) return _owner.SelectedBackColor;
            }
            return _owner.BackColor;
        }

        private void DrawShadow(Graphics g)
        {
            if (_owner.ShadowOpacity <= 0) return;

            int shadowDepth = Math.Max(1, _owner.ShadowOffset / 2);
            int maxLayers = Math.Min(shadowDepth, 6);

            Rectangle shadowRect = new Rectangle(
                DrawingRect.X + _owner.ShadowOffset,
                DrawingRect.Y + _owner.ShadowOffset,
                DrawingRect.Width,
                DrawingRect.Height);

            for (int i = 1; i <= maxLayers; i++)
            {
                float layerOpacityFactor = (float)(maxLayers - i + 1) / maxLayers;
                float finalOpacity = _owner.ShadowOpacity * layerOpacityFactor * 0.6f;
                int layerAlpha = Math.Max(5, (int)(255 * finalOpacity));

                Color layerShadowColor = Color.FromArgb(layerAlpha, _owner.ShadowColor);
                int spread = i - 1;
                Rectangle layerRect = new Rectangle(
                    shadowRect.X - spread,
                    shadowRect.Y - spread,
                    shadowRect.Width + (spread * 2),
                    shadowRect.Height + (spread * 2));

                using (var shadowBrush = new SolidBrush(layerShadowColor))
                {
                    if (_owner.IsRounded && _owner.BorderRadius > 0)
                    {
                        int shadowRadius = Math.Max(0, _owner.BorderRadius + spread);
                        using (var shadowPath = GetRoundedRectPath(layerRect, shadowRadius))
                        {
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }
                    else
                    {
                        g.FillRectangle(shadowBrush, layerRect);
                    }
                }
            }
        }

        private void DrawBorders(Graphics g)
        {
            // Material UI borders take priority
            if (_owner.MaterialBorderVariant != MaterialTextFieldVariant.Standard)
            {
                DrawMaterialBorder(g);
                return;
            }

            Color effectiveBorderColor = GetEffectiveBorderColor();

            if (_owner.ShowAllBorders && _owner.BorderThickness > 0)
            {
                using (var borderPen = new Pen(effectiveBorderColor, _owner.BorderThickness))
                {
                    borderPen.DashStyle = _owner.BorderDashStyle;
                    borderPen.Alignment = PenAlignment.Inset;

                    if (_owner.IsRounded)
                    {
                        using (var path = GetRoundedRectPath(BorderRectangle, _owner.BorderRadius))
                        {
                            g.DrawPath(borderPen, path);
                        }
                    }
                    else
                    {
                        g.DrawRectangle(borderPen, BorderRectangle);
                    }
                }
            }
            else
            {
                // Draw individual borders
                using (var borderPen = new Pen(effectiveBorderColor, _owner.BorderThickness))
                {
                    borderPen.DashStyle = _owner.BorderDashStyle;
                    if (_owner.ShowTopBorder)
                        g.DrawLine(borderPen, BorderRectangle.Left, BorderRectangle.Top, BorderRectangle.Right, BorderRectangle.Top);
                    if (_owner.ShowBottomBorder)
                        g.DrawLine(borderPen, BorderRectangle.Left, BorderRectangle.Bottom, BorderRectangle.Right, BorderRectangle.Bottom);
                    if (_owner.ShowLeftBorder)
                        g.DrawLine(borderPen, BorderRectangle.Left, BorderRectangle.Top, BorderRectangle.Left, BorderRectangle.Bottom);
                    if (_owner.ShowRightBorder)
                        g.DrawLine(borderPen, BorderRectangle.Right, BorderRectangle.Top, BorderRectangle.Right, BorderRectangle.Bottom);
                }
            }
        }

        private Color GetEffectiveBorderColor()
        {
            var ownerAdv = OwnerAdv;
            if (ownerAdv != null)
            {
                if (!_owner.Enabled) return _owner.DisabledBorderColor;
                if (_owner.Focused) return _owner.FocusBorderColor;
                if (ownerAdv.IsHovered) return _owner.HoverBorderColor;
                if (ownerAdv.IsPressed) return _owner.PressedBorderColor;
                if (ownerAdv.IsSelected) return _owner.SelectedBorderColor;
                return ownerAdv.BorderColor;
            }
            return _owner.InactiveBorderColor;
        }

        private void DrawMaterialBorder(Graphics g)
        {
            Color borderColor = GetEffectiveBorderColor();
            Rectangle borderRect = BorderRectangle;

            switch (_owner.MaterialBorderVariant)
            {
                case MaterialTextFieldVariant.Standard:
                    using (var underlinePen = new Pen(borderColor, 1))
                    {
                        g.DrawLine(underlinePen, borderRect.Left, borderRect.Bottom - 1, borderRect.Right, borderRect.Bottom - 1);
                        if (_owner.Focused)
                        {
                            using (var focusPen = new Pen(_owner.FocusBorderColor, 2))
                                g.DrawLine(focusPen, borderRect.Left, borderRect.Bottom, borderRect.Right, borderRect.Bottom);
                        }
                    }
                    break;

                case MaterialTextFieldVariant.Outlined:
                    using (var borderPen = new Pen(borderColor, 1))
                    {
                        if (_owner.IsRounded)
                        {
                            using (var path = GetRoundedRectPath(borderRect, _owner.BorderRadius))
                                g.DrawPath(borderPen, path);
                        }
                        else
                        {
                            g.DrawRectangle(borderPen, borderRect);
                        }

                        // Draw floating label if needed
                        if (_owner.FloatingLabel && !string.IsNullOrEmpty(_owner.LabelText))
                        {
                            DrawFloatingLabel(g, borderRect, borderColor);
                        }
                    }
                    break;

                case MaterialTextFieldVariant.Filled:
                    // Background already handled in DrawBackground
                    using (var underlinePen = new Pen(borderColor, 1))
                    {
                        g.DrawLine(underlinePen, borderRect.Left, borderRect.Bottom - 1, borderRect.Right, borderRect.Bottom - 1);
                        if (_owner.Focused)
                        {
                            using (var focusPen = new Pen(_owner.FocusBorderColor, 2))
                                g.DrawLine(focusPen, borderRect.Left, borderRect.Bottom, borderRect.Right, borderRect.Bottom);
                        }
                    }
                    break;
            }

            // Draw helper text if provided
            if (!string.IsNullOrEmpty(_owner.HelperText))
            {
                DrawHelperText(g, borderRect);
            }
        }

        private void DrawFloatingLabel(Graphics g, Rectangle borderRect, Color borderColor)
        {
            var labelFont = new Font(_owner.Font.FontFamily, _owner.Font.Size * 0.8f);
            var labelSize = TextRenderer.MeasureText(_owner.LabelText, labelFont);
            int labelX = borderRect.X + 10;
            var labelGapRect = new Rectangle(labelX - 2, borderRect.Y - labelSize.Height / 2, labelSize.Width + 4, labelSize.Height);

            using (var backBrush = new SolidBrush(_owner.BackColor))
                g.FillRectangle(backBrush, labelGapRect);

            using (var labelBrush = new SolidBrush(_owner.Focused ? _owner.FocusBorderColor : borderColor))
                g.DrawString(_owner.LabelText, labelFont, labelBrush, labelX, borderRect.Y - labelSize.Height / 2);
        }

        private void DrawHelperText(Graphics g, Rectangle borderRect)
        {
            var helperFont = new Font(_owner.Font.FontFamily, _owner.Font.Size * 0.8f);
            Color helperColor = _owner.IsValid ? Color.Gray : Color.Red;
            var helperRect = new Rectangle(borderRect.X, borderRect.Bottom + 2, borderRect.Width, 20);
            TextRenderer.DrawText(g, _owner.HelperText, helperFont, helperRect, helperColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        private void DrawBadge(Graphics g)
        {
            const int badgeSize = 22;
            int x = _owner.Width - badgeSize / 2;
            int y = -badgeSize / 2;
            var badgeRect = new Rectangle(x, y, badgeSize, badgeSize);

            // Badge shadow
            if (_owner.ShowShadow)
            {
                float badgeShadowOpacity = Math.Min(0.3f, _owner.ShadowOpacity * 0.8f);
                int badgeShadowOffset = 1;
                Color badgeShadowColor = Color.FromArgb((int)(255 * badgeShadowOpacity), _owner.ShadowColor);

                using (var shadowBrush = new SolidBrush(badgeShadowColor))
                {
                    g.FillEllipse(shadowBrush, badgeRect.X + badgeShadowOffset, badgeRect.Y + badgeShadowOffset, badgeRect.Width, badgeRect.Height);
                }
            }

            // Badge background
            using (var brush = new SolidBrush(_owner.BadgeBackColor))
            {
                switch (_owner.BadgeShape)
                {
                    case BadgeShape.Circle:
                        g.FillEllipse(brush, badgeRect);
                        break;
                    case BadgeShape.RoundedRectangle:
                        using (var path = GetRoundedRectPath(badgeRect, badgeRect.Height / 4))
                            g.FillPath(brush, path);
                        break;
                    case BadgeShape.Rectangle:
                        g.FillRectangle(brush, badgeRect);
                        break;
                }
            }

            // Badge text
            if (!string.IsNullOrEmpty(_owner.BadgeText))
            {
                using (var textBrush = new SolidBrush(_owner.BadgeForeColor))
                using (var scaledFont = _owner.DisableDpiAndScaling ? _owner.BadgeFont : GetScaledBadgeFont(g, _owner.BadgeText, new Size(badgeRect.Width - 4, badgeRect.Height - 4), _owner.BadgeFont))
                {
                    var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(_owner.BadgeText, scaledFont, textBrush, badgeRect, fmt);
                }
            }
        }

        #region Modern Gradient Methods
        private void DrawModernGradient(Graphics g, Color baseColor)
        {
            switch (_owner.ModernGradientType)
            {
                case ModernGradientType.Subtle:
                    DrawSubtleGradient(g, DrawingRect, baseColor);
                    break;
                case ModernGradientType.Linear:
                    DrawLinearGradient(g, _owner.GradientStartColor, _owner.GradientEndColor);
                    break;
                case ModernGradientType.Radial:
                    DrawRadialGradient(g, DrawingRect, baseColor);
                    break;
                case ModernGradientType.Conic:
                    DrawConicGradient(g, DrawingRect, baseColor);
                    break;
                case ModernGradientType.Mesh:
                    DrawMeshGradient(g, DrawingRect, baseColor);
                    break;
            }

            if (_owner.UseGlassmorphism)
            {
                ApplyGlassmorphism(g, DrawingRect);
            }
        }

        private void DrawSubtleGradient(Graphics g, Rectangle rect, Color baseColor)
        {
            Color color1 = baseColor;
            float brightness = baseColor.GetBrightness();
            const float subtleFactor = 0.05f;

            Color color2 = brightness > 0.5f
                ? Color.FromArgb(Math.Max(0, baseColor.R - (int)(255 * subtleFactor)), Math.Max(0, baseColor.G - (int)(255 * subtleFactor)), Math.Max(0, baseColor.B - (int)(255 * subtleFactor)))
                : Color.FromArgb(Math.Min(255, baseColor.R + (int)(255 * subtleFactor)), Math.Min(255, baseColor.G + (int)(255 * subtleFactor)), Math.Min(255, baseColor.B + (int)(255 * subtleFactor)));

            float angleRadians = (float)(_owner.GradientAngle * Math.PI / 180f);
            using (var gradientBrush = CreateAngledGradientBrush(rect, color1, color2, angleRadians))
            {
                var blend = new ColorBlend();
                blend.Colors = new Color[] { color1, BlendColors(color1, color2, 0.5f), color2 };
                blend.Positions = new float[] { 0.0f, 0.3f, 1.0f };
                gradientBrush.InterpolationColors = blend;
                FillShape(g, gradientBrush, rect);
            }
        }

        private void DrawLinearGradient(Graphics g, Color startColor, Color endColor)
        {
            using (var gradientBrush = new LinearGradientBrush(DrawingRect, startColor, endColor, _owner.GradientDirection))
            {
                if (_owner.GradientStops.Count > 0)
                {
                    ApplyGradientStops(gradientBrush);
                }
                FillShape(g, gradientBrush, DrawingRect);
            }
        }

        private void DrawRadialGradient(Graphics g, Rectangle rect, Color baseColor)
        {
            Color centerColor = _owner.GradientStartColor != Color.LightGray ? _owner.GradientStartColor : baseColor;
            Color edgeColor = _owner.GradientEndColor != Color.Gray ? _owner.GradientEndColor : ModifyColorBrightness(baseColor, 0.7f);

            var center = new PointF(rect.X + rect.Width * _owner.RadialCenter.X, rect.Y + rect.Height * _owner.RadialCenter.Y);
            float radius = Math.Max(rect.Width, rect.Height) * 0.7f;

            using (var path = new GraphicsPath())
            {
                path.AddEllipse(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                using (var gradientBrush = new PathGradientBrush(path))
                {
                    gradientBrush.CenterColor = centerColor;
                    gradientBrush.SurroundColors = new Color[] { edgeColor };
                    gradientBrush.CenterPoint = center;
                    FillShape(g, gradientBrush, rect);
                }
            }
        }

        private void DrawConicGradient(Graphics g, Rectangle rect, Color baseColor)
        {
            var center = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
            int segments = 36;

            for (int i = 0; i < segments; i++)
            {
                float startAngle = (i * 360f / segments) + _owner.GradientAngle;
                float hue = (startAngle % 360f) / 360f;
                Color segmentColor = ColorFromHSV(hue, 0.5f, baseColor.GetBrightness());

                using (var segmentBrush = new SolidBrush(Color.FromArgb(100, segmentColor)))
                {
                    g.FillPie(segmentBrush, rect, startAngle, 360f / segments);
                }
            }
        }

        private void DrawMeshGradient(Graphics g, Rectangle rect, Color baseColor)
        {
            int gridSize = 3;
            Color[,] colorGrid = new Color[gridSize, gridSize];

            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    float brightness = 0.7f + (0.3f * ((x + y) / (float)(gridSize * 2)));
                    colorGrid[x, y] = ModifyColorBrightness(baseColor, brightness);
                }
            }

            float cellWidth = rect.Width / (float)(gridSize - 1);
            float cellHeight = rect.Height / (float)(gridSize - 1);

            for (int x = 0; x < gridSize - 1; x++)
            {
                for (int y = 0; y < gridSize - 1; y++)
                {
                    var cellRect = new RectangleF(rect.X + x * cellWidth, rect.Y + y * cellHeight, cellWidth * 1.5f, cellHeight * 1.5f);
                    using (var cellBrush = new LinearGradientBrush(cellRect, colorGrid[x, y], colorGrid[x + 1, y + 1], LinearGradientMode.ForwardDiagonal))
                    {
                        g.FillRectangle(cellBrush, cellRect);
                    }
                }
            }
        }

        private void ApplyGlassmorphism(Graphics g, Rectangle rect)
        {
            using (var glassBrush = new SolidBrush(Color.FromArgb((int)(255 * _owner.GlassmorphismOpacity), Color.White)))
            {
                var random = new Random(42);
                for (int i = 0; i < rect.Width * rect.Height / 1000; i++)
                {
                    int x = random.Next(rect.X, rect.X + rect.Width);
                    int y = random.Next(rect.Y, rect.Y + rect.Height);
                    using (var noiseBrush = new SolidBrush(Color.FromArgb(random.Next(5, 15), Color.White)))
                    {
                        g.FillRectangle(noiseBrush, x, y, 1, 1);
                    }
                }
                FillShape(g, glassBrush, rect);
            }
        }
        #endregion

    
        #region Helper Methods
        private void FillShape(Graphics g, Brush brush, Rectangle rect)
        {
            if (_owner.IsRounded)
            {
                using (var path = GetRoundedRectPath(rect, _owner.BorderRadius))
                {
                    g.FillPath(brush, path);
                }
            }
            else
            {
                g.FillRectangle(brush, rect);
            }
        }

        private LinearGradientBrush CreateAngledGradientBrush(Rectangle rect, Color color1, Color color2, float angleRadians)
        {
            float cos = (float)Math.Cos(angleRadians);
            float sin = (float)Math.Sin(angleRadians);

            var start = new PointF(
                rect.X + rect.Width * (0.5f - cos * 0.5f),
                rect.Y + rect.Height * (0.5f - sin * 0.5f));

            var end = new PointF(
                rect.X + rect.Width * (0.5f + cos * 0.5f),
                rect.Y + rect.Height * (0.5f + sin * 0.5f));

            return new LinearGradientBrush(start, end, color1, color2);
        }

        private Color ModifyColorBrightness(Color color, float brightness)
        {
            return Color.FromArgb(color.A, (int)(color.R * brightness), (int)(color.G * brightness), (int)(color.B * brightness));
        }

        private Color BlendColors(Color color1, Color color2, float amount)
        {
            return Color.FromArgb(
                (int)(color1.R + (color2.R - color1.R) * amount),
                (int)(color1.G + (color2.G - color1.G) * amount),
                (int)(color1.B + (color2.B - color1.B) * amount));
        }

        private Color ColorFromHSV(float hue, float saturation, double brightness)
        {
            hue = hue * 360f;
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            double v = brightness;
            double p = brightness * (1 - saturation);
            double q = brightness * (1 - f * saturation);
            double t = brightness * (1 - (1 - f) * saturation);

            switch (hi)
            {
                case 0: return Color.FromArgb(255, (int)(v * 255), (int)(t * 255), (int)(p * 255));
                case 1: return Color.FromArgb(255, (int)(q * 255), (int)(v * 255), (int)(p * 255));
                case 2: return Color.FromArgb(255, (int)(p * 255), (int)(v * 255), (int)(t * 255));
                case 3: return Color.FromArgb(255, (int)(p * 255), (int)(q * 255), (int)(v * 255));
                case 4: return Color.FromArgb(255, (int)(t * 255), (int)(p * 255), (int)(v * 255));
                default: return Color.FromArgb(255, (int)(v * 255), (int)(p * 255), (int)(q * 255));
            }
        }

        private void ApplyGradientStops(LinearGradientBrush brush)
        {
            if (_owner.GradientStops.Count < 2) return;

            var sortedStops = _owner.GradientStops.OrderBy(s => s.Position).ToList();
            var blend = new ColorBlend();
            blend.Colors = sortedStops.Select(s => s.Color).ToArray();
            blend.Positions = sortedStops.Select(s => s.Position).ToArray();
            brush.InterpolationColors = blend;
        }

        private Font GetScaledBadgeFont(Graphics g, string text, Size maxSize, Font originalFont)
        {
            if (string.IsNullOrEmpty(text) || maxSize.Width <= 0 || maxSize.Height <= 0)
                return new Font(originalFont.FontFamily, 8, FontStyle.Bold);

            if (text.Length == 1)
            {
                float fontSize = Math.Max(6, Math.Min(maxSize.Height * 0.65f, 10));
                return new Font(originalFont.FontFamily, fontSize, FontStyle.Bold);
            }

            for (float size = originalFont.Size; size >= 6; size -= 0.5f)
            {
                using (var testFont = new Font(originalFont.FontFamily, size, FontStyle.Bold))
                {
                    var measuredSize = TextRenderer.MeasureText(g, text, testFont);
                    if (measuredSize.Width <= maxSize.Width && measuredSize.Height <= maxSize.Height)
                    {
                        return new Font(originalFont.FontFamily, size, FontStyle.Bold);
                    }
                }
            }

            return new Font(originalFont.FontFamily, 6, FontStyle.Bold);
        }

        public void AddGradientStop(float position, Color color)
        {
            _owner.GradientStops.Add(new GradientStop(position, color));
        }

        public void ClearGradientStops()
        {
            _owner.GradientStops.Clear();
        }

        public static GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = Math.Min(Math.Min(radius * 2, rect.Width), rect.Height);
            if (d <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
        #endregion
    }
}
