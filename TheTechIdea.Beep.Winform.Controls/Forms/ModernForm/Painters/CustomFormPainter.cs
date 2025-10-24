using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Custom form painter that serves as a base/template for user customization.
    /// Users can extend this class to create their own unique form styles with full control
    /// over shapes, colors, effects, and rendering behavior.
    /// ENHANCED: Configurable button shapes with multiple modes (circle, square, rounded, star, hexagon, etc.)
    /// </summary>
    public class CustomFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        // Protected properties allow derived classes to customize behavior
        protected virtual Color DefaultBackgroundColor => Color.FromArgb(240, 240, 240);
        protected virtual Color DefaultBorderColor => Color.FromArgb(100, 100, 100);
        protected virtual Color DefaultCaptionColor => Color.FromArgb(220, 220, 220);
        protected virtual Color DefaultTitleColor => Color.FromArgb(50, 50, 50);
        protected virtual int DefaultBorderWidth => 1;
        protected virtual int DefaultCornerRadius => 4;
        protected virtual bool DefaultUseGradients => false;
        protected virtual bool DefaultUseShadows => true;

        // ENHANCED: Configurable button shape modes
        public enum ButtonShapeMode
        {
            Circle,
            Square,
            RoundedSquare,
            Diamond,
            Hexagon,
            Octagon,
            Star,
            Triangle,
            Chevron
        }

        /// <summary>
        /// Current button shape mode (configurable by user or derived classes)
        /// </summary>
        public ButtonShapeMode CurrentButtonShape { get; set; } = ButtonShapeMode.Circle;

        /// <summary>
        /// Button fill opacity (0-255, default 180)
        /// </summary>
        public int ButtonFillOpacity { get; set; } = 180;

        /// <summary>
        /// Button border thickness (default 2)
        /// </summary>
        public float ButtonBorderThickness { get; set; } = 2f;

        /// <summary>
        /// Enable gradient fill on buttons (default true)
        /// </summary>
        public bool UseButtonGradients { get; set; } = true;

        public virtual FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return new FormPainterMetrics
            {
                BackgroundColor = DefaultBackgroundColor,
                BorderColor = DefaultBorderColor,
                CaptionTextColorActive = DefaultCaptionColor,
                CaptionTextColorInactive = Color.FromArgb(230, 230, 230),
                CaptionColor = DefaultTitleColor,
                BorderWidth = DefaultBorderWidth,
                ResizeBorderWidth = 4,
                CaptionHeight = owner.Font.Height + 16,
                MinimizeButtonColor = DefaultTitleColor,
                MaximizeButtonColor = DefaultTitleColor,
                CloseButtonColor = DefaultTitleColor,
                MinimizeButtonHoverColor = Color.FromArgb(200, 200, 200),
                MaximizeButtonHoverColor = Color.FromArgb(200, 200, 200),
                CloseButtonHoverColor = Color.FromArgb(232, 17, 35),
                AccentBarWidth = 0
            };
        }

        public virtual void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var cornerRadius = GetCornerRadius(owner);

            if (DefaultUseGradients)
            {
                using var brush = new LinearGradientBrush(
                    owner.ClientRectangle,
                    metrics.BackgroundColor,
                    Color.FromArgb(Math.Max(0, metrics.BackgroundColor.R - 10), 
                                   Math.Max(0, metrics.BackgroundColor.G - 10), 
                                   Math.Max(0, metrics.BackgroundColor.B - 10)),
                    LinearGradientMode.Vertical);
                if (cornerRadius.TopLeft > 0 || cornerRadius.TopRight > 0 || cornerRadius.BottomLeft > 0 || cornerRadius.BottomRight > 0)
                {
                    using var path = CreateRoundedPath(owner.ClientRectangle, cornerRadius);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillPath(brush, path);
                }
                else
                {
                    g.FillRectangle(brush, owner.ClientRectangle);
                }
            }
            else
            {
                using var brush = new SolidBrush(metrics.BackgroundColor);
                if (cornerRadius.TopLeft > 0 || cornerRadius.TopRight > 0 || cornerRadius.BottomLeft > 0 || cornerRadius.BottomRight > 0)
                {
                    using var path = CreateRoundedPath(owner.ClientRectangle, cornerRadius);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillPath(brush, path);
                }
                else
                {
                    g.FillRectangle(brush, owner.ClientRectangle);
                }
            }
        }

        public virtual void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            using (var brush = new SolidBrush(metrics.CaptionTextColorActive))
            {
                var cornerRadius = GetCornerRadius(owner);
                if (cornerRadius.TopLeft > 0 || cornerRadius.TopRight > 0)
                {
                    using var path = CreateRoundedPath(
                        new Rectangle(0, 0, owner.ClientSize.Width, captionRect.Height + cornerRadius.TopLeft), 
                        cornerRadius);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillPath(brush, path);
                }
                else
                {
                    g.FillRectangle(brush, captionRect);
                }
            }

            // Draw separator line
            using (var separatorPen = new Pen(metrics.BorderColor, 1f))
            {
                g.DrawLine(separatorPen, 0, captionRect.Bottom - 1, owner.ClientSize.Width, captionRect.Bottom - 1);
            }

            // ENHANCED: Paint configurable shape buttons
            PaintConfigurableButtons(g, owner, captionRect, metrics);

            // Draw title text
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // NOTE: Do NOT call owner.PaintBuiltInCaptionElements(g) - we paint custom configurable buttons
            // Only paint the icon
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }

        /// <summary>
        /// Paint buttons with configurable shapes (ENHANCED UNIQUE SKIN)
        /// Supports 9 different button shapes with customizable fill/border properties
        /// </summary>
        private void PaintConfigurableButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;

            int buttonSize = 18;
            int padding = (captionRect.Height - buttonSize) / 2;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Close button: Red with user-selected shape
            PaintShapedButton(g, closeRect, Color.FromArgb(ButtonFillOpacity, 232, 17, 35), 
                metrics.BorderColor, padding, buttonSize, "close");

            // Maximize button: Green with user-selected shape
            PaintShapedButton(g, maxRect, Color.FromArgb(ButtonFillOpacity, 80, 160, 80), 
                metrics.BorderColor, padding, buttonSize, "maximize");

            // Minimize button: Blue with user-selected shape
            PaintShapedButton(g, minRect, Color.FromArgb(ButtonFillOpacity, 80, 140, 200), 
                metrics.BorderColor, padding, buttonSize, "minimize");

            // Theme/Style buttons
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintShapedButton(g, styleRect, Color.FromArgb(ButtonFillOpacity, 150, 120, 180), 
                    metrics.BorderColor, padding, buttonSize, "Style");
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintShapedButton(g, themeRect, Color.FromArgb(ButtonFillOpacity, 200, 150, 100), 
                    metrics.BorderColor, padding, buttonSize, "theme");
            }
        }

        private void PaintShapedButton(Graphics g, Rectangle buttonRect, Color baseColor, Color borderColor, int padding, int size, string buttonType)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;

            // Create shape path based on current mode
            using (var shapePath = CreateButtonShapePath(centerX, centerY, size))
            {
                // Fill with gradient or solid color
                if (UseButtonGradients)
                {
                    var lightColor = ControlPaint.Light(baseColor);
                    var bounds = shapePath.GetBounds();
                    using (var brush = new LinearGradientBrush(bounds, lightColor, baseColor, LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, shapePath);
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(baseColor))
                    {
                        g.FillPath(brush, shapePath);
                    }
                }

                // Draw border
                using (var pen = new Pen(Color.FromArgb(180, borderColor), ButtonBorderThickness))
                {
                    g.DrawPath(pen, shapePath);
                }
            }

            // Draw icon (standard icons for all shapes)
            using (var iconPen = new Pen(Color.FromArgb(220, Color.White), 1.5f))
            {
                int iconSize = 6;

                switch (buttonType)
                {
                    case "close":
                        g.DrawLine(iconPen, centerX - iconSize / 2, centerY - iconSize / 2,
                            centerX + iconSize / 2, centerY + iconSize / 2);
                        g.DrawLine(iconPen, centerX + iconSize / 2, centerY - iconSize / 2,
                            centerX - iconSize / 2, centerY + iconSize / 2);
                        break;
                    case "maximize":
                        g.DrawRectangle(iconPen, centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize);
                        break;
                    case "minimize":
                        g.DrawLine(iconPen, centerX - iconSize / 2, centerY, centerX + iconSize / 2, centerY);
                        break;
                    case "Style":
                        // Paint palette icon
                        g.DrawEllipse(iconPen, centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize);
                        g.FillEllipse(Brushes.White, centerX - 1, centerY - 1, 2, 2);
                        break;
                    case "theme":
                        // Theme switcher icon
                        g.DrawEllipse(iconPen, centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize);
                        using (var fillBrush = new SolidBrush(Color.FromArgb(180, Color.White)))
                        {
                            g.FillPie(fillBrush, centerX - iconSize / 2, centerY - iconSize / 2, iconSize, iconSize, 0, 180);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Create button shape path based on CurrentButtonShape mode
        /// </summary>
        private GraphicsPath CreateButtonShapePath(int centerX, int centerY, int size)
        {
            var path = new GraphicsPath();
            int radius = size / 2;

            switch (CurrentButtonShape)
            {
                case ButtonShapeMode.Circle:
                    path.AddEllipse(centerX - radius, centerY - radius, size, size);
                    break;

                case ButtonShapeMode.Square:
                    path.AddRectangle(new Rectangle(centerX - radius, centerY - radius, size, size));
                    break;

                case ButtonShapeMode.RoundedSquare:
                    int corner = size / 4;
                    var rect = new Rectangle(centerX - radius, centerY - radius, size, size);
                    path.AddArc(rect.X, rect.Y, corner * 2, corner * 2, 180, 90);
                    path.AddArc(rect.Right - corner * 2, rect.Y, corner * 2, corner * 2, 270, 90);
                    path.AddArc(rect.Right - corner * 2, rect.Bottom - corner * 2, corner * 2, corner * 2, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - corner * 2, corner * 2, corner * 2, 90, 90);
                    path.CloseFigure();
                    break;

                case ButtonShapeMode.Diamond:
                    path.AddPolygon(new Point[]
                    {
                        new Point(centerX, centerY - radius),
                        new Point(centerX + radius, centerY),
                        new Point(centerX, centerY + radius),
                        new Point(centerX - radius, centerY)
                    });
                    break;

                case ButtonShapeMode.Hexagon:
                    var hexPoints = new Point[6];
                    for (int i = 0; i < 6; i++)
                    {
                        double angle = Math.PI / 3 * i - Math.PI / 6;
                        hexPoints[i] = new Point(
                            centerX + (int)(radius * Math.Cos(angle)),
                            centerY + (int)(radius * Math.Sin(angle))
                        );
                    }
                    path.AddPolygon(hexPoints);
                    break;

                case ButtonShapeMode.Octagon:
                    var octPoints = new Point[8];
                    for (int i = 0; i < 8; i++)
                    {
                        double angle = Math.PI / 4 * i - Math.PI / 8;
                        octPoints[i] = new Point(
                            centerX + (int)(radius * Math.Cos(angle)),
                            centerY + (int)(radius * Math.Sin(angle))
                        );
                    }
                    path.AddPolygon(octPoints);
                    break;

                case ButtonShapeMode.Star:
                    var starPoints = new Point[10];
                    int outerRadius = radius;
                    int innerRadius = radius / 2;
                    for (int i = 0; i < 10; i++)
                    {
                        double angle = Math.PI / 5 * i - Math.PI / 2;
                        int r = (i % 2 == 0) ? outerRadius : innerRadius;
                        starPoints[i] = new Point(
                            centerX + (int)(r * Math.Cos(angle)),
                            centerY + (int)(r * Math.Sin(angle))
                        );
                    }
                    path.AddPolygon(starPoints);
                    break;

                case ButtonShapeMode.Triangle:
                    path.AddPolygon(new Point[]
                    {
                        new Point(centerX, centerY - radius),
                        new Point(centerX + radius, centerY + radius),
                        new Point(centerX - radius, centerY + radius)
                    });
                    break;

                case ButtonShapeMode.Chevron:
                    int chevW = size / 3;
                    path.AddPolygon(new Point[]
                    {
                        new Point(centerX - radius, centerY - radius),
                        new Point(centerX + chevW, centerY),
                        new Point(centerX - radius, centerY + radius),
                        new Point(centerX - radius + chevW, centerY + radius),
                        new Point(centerX + radius, centerY),
                        new Point(centerX - radius + chevW, centerY - radius)
                    });
                    break;
            }

            return path;
        }

        public virtual void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var cornerRadius = GetCornerRadius(owner);

            using var pen = new Pen(metrics.BorderColor, Math.Max(1, metrics.BorderWidth))
            {
                Alignment = PenAlignment.Inset
            };

            var rect = new Rectangle(0, 0, owner.ClientSize.Width - 1, owner.ClientSize.Height - 1);
            
            if (cornerRadius.TopLeft > 0 || cornerRadius.TopRight > 0 || cornerRadius.BottomLeft > 0 || cornerRadius.BottomRight > 0)
            {
                using var path = CreateRoundedPath(rect, cornerRadius);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(pen, path);
            }
            else
            {
                g.DrawRectangle(pen, rect);
            }
        }

        public virtual ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            if (!DefaultUseShadows)
            {
                return new ShadowEffect { Color = Color.Transparent, Blur = 0 };
            }

            return new ShadowEffect
            {
                Color = Color.FromArgb(40, 0, 0, 0),
                Blur = 10,
                OffsetY = 4,
                Inner = false
            };
        }

        public virtual CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(DefaultCornerRadius);
        }

        public virtual AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return DefaultCornerRadius > 0 ? AntiAliasMode.High : AntiAliasMode.Low;
        }

        public virtual bool SupportsAnimations => false;

        public virtual void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            var shadow = GetShadowEffect(owner);
            if (!shadow.Inner && shadow.Blur > 0)
            {
                DrawShadow(g, rect, shadow);
            }

            PaintBackground(g, owner);
            PaintBorders(g, owner);
            if (owner.ShowCaptionBar)
            {
                PaintCaption(g, owner, owner.CurrentLayout.CaptionRect);
            }
        }

        protected virtual void DrawShadow(Graphics g, Rectangle rect, ShadowEffect shadow)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            var shadowRect = new Rectangle(
                rect.X + shadow.OffsetX - shadow.Blur,
                rect.Y + shadow.OffsetY - shadow.Blur,
                rect.Width + shadow.Blur * 2,
                rect.Height + shadow.Blur * 2);
            if (shadowRect.Width <= 0 || shadowRect.Height <= 0) return;

            var cornerRadius = GetCornerRadius(null);
            if (cornerRadius.TopLeft > 0 || cornerRadius.TopRight > 0 || cornerRadius.BottomLeft > 0 || cornerRadius.BottomRight > 0)
            {
                using var path = CreateRoundedPath(shadowRect, cornerRadius);
                using var brush = new SolidBrush(shadow.Color);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(brush, path);
            }
            else
            {
                using var brush = new SolidBrush(shadow.Color);
                g.FillRectangle(brush, shadowRect);
            }
        }

        public virtual void CalculateLayoutAndHitAreas(BeepiFormPro owner)
        {
            var layout = new PainterLayoutInfo();
            var metrics = GetMetrics(owner);
            var captionHeight = metrics.CaptionHeight;
            
            // Check if caption bar should be hidden
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);

            var buttonSize = new Size(46, captionHeight);
            var buttonY = 0;
            var buttonX = owner.ClientSize.Width - buttonSize.Width;

            if (owner.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }

            if (owner.ShowMinMaxButtons)
            {
                layout.MaximizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;

                layout.MinimizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }

            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("Style", layout.StyleButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }

            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }

            var iconSize = 18;
            var iconPadding = 12;
            layout.IconRect = new Rectangle(iconPadding, (captionHeight - iconSize) / 2, iconSize, iconSize);
            owner._hits.RegisterHitArea("icon", layout.IconRect, HitAreaType.Icon);

            var titleX = iconPadding + iconSize + iconPadding;
            var titleWidth = buttonX - titleX - iconPadding;
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            owner._hits.RegisterHitArea("title", layout.TitleRect, HitAreaType.Caption);

            owner.CurrentLayout = layout;
        }

        public virtual void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var metrics = GetMetrics(owner);
            var cornerRadius = GetCornerRadius(owner);

            using var pen = new Pen(metrics.BorderColor, Math.Max(1, borderThickness))
            {
                Alignment = PenAlignment.Inset
            };

            var rect = new Rectangle(0, 0, owner.Width - 1, owner.Height - 1);

            if (cornerRadius.TopLeft > 0 || cornerRadius.TopRight > 0 || cornerRadius.BottomLeft > 0 || cornerRadius.BottomRight > 0)
            {
                using var path = CreateRoundedPath(rect, cornerRadius);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(pen, path);
            }
            else
            {
                g.DrawRectangle(pen, rect);
            }
        }

        protected virtual GraphicsPath CreateRoundedPath(Rectangle rect, CornerRadius cornerRadius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0) return path;

            var tl = Math.Min(cornerRadius.TopLeft, Math.Min(rect.Width / 2, rect.Height / 2));
            var tr = Math.Min(cornerRadius.TopRight, Math.Min(rect.Width / 2, rect.Height / 2));
            var br = Math.Min(cornerRadius.BottomRight, Math.Min(rect.Width / 2, rect.Height / 2));
            var bl = Math.Min(cornerRadius.BottomLeft, Math.Min(rect.Width / 2, rect.Height / 2));

            if (tl > 0) path.AddArc(rect.X, rect.Y, tl * 2, tl * 2, 180, 90);
            else path.AddLine(rect.X, rect.Y, rect.X, rect.Y);

            if (tr > 0) path.AddArc(rect.Right - tr * 2, rect.Y, tr * 2, tr * 2, 270, 90);
            else path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);

            if (br > 0) path.AddArc(rect.Right - br * 2, rect.Bottom - br * 2, br * 2, br * 2, 0, 90);
            else path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);

            if (bl > 0) path.AddArc(rect.X, rect.Bottom - bl * 2, bl * 2, bl * 2, 90, 90);
            else path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);

            path.CloseFigure();
            return path;
        }
    }
}
