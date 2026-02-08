using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ColorSystems;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Material You (Material 3) form painter with dynamic color adaptation.
    /// 
    /// Features:
    /// - Material 3 dynamic colors
    /// - Elevation-based surfaces
    /// - Tonal surfaces
    /// - Vertical accent bar (6px)
    /// - Material 3 buttons with state layers (40dp touch targets, 24dp icons)
    /// - Elevation shadows (no visible borders)
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class MaterialYouFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.MaterialYou, owner);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // Material You: Dynamic color-based surface
            Color surfaceColor = metrics.BackgroundColor;
            
            // Try to get Material You palette if theme supports it
            if (owner.UseThemeColors && owner.CurrentTheme != null)
            {
                // Use theme's surface color for Material You dynamic theming
                if (owner.CurrentTheme.SurfaceColor != Color.Empty)
                {
                    surfaceColor = owner.CurrentTheme.SurfaceColor;
                }
            }
            
            using (var brush = new SolidBrush(surfaceColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }

            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;
            
            // Material 3 elevation tint from top (subtle depth)
            FormPainterRenderHelper.PaintGradientBackground(g, owner.ClientRectangle,
                Color.FromArgb(14, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0),
                LinearGradientMode.Vertical);
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            var primary = metrics.BorderColor;

            // Try to get primary color from theme for Material You dynamic theming
            if (owner.UseThemeColors && owner.CurrentTheme != null)
            {
                if (owner.CurrentTheme.PrimaryColor != Color.Empty)
                {
                    primary = owner.CurrentTheme.PrimaryColor;
                }
            }

            // Draw vertical accent bar on the left (Material You style - 6px)
            using var brush = new SolidBrush(primary);
            int accentBarWidth = Math.Max(0, metrics.AccentBarWidth);
            var bar = new Rectangle(captionRect.Left, captionRect.Top, accentBarWidth, captionRect.Height);
            g.FillRectangle(brush, bar);

            // Add subtle state layer effect (Material 3)
            using var stateBrush = new SolidBrush(Color.FromArgb(8, primary));
            var stateRect = new Rectangle(captionRect.Left + accentBarWidth, captionRect.Top, captionRect.Width - accentBarWidth, captionRect.Height);
            g.FillRectangle(stateBrush, stateRect);

            // Paint Material You buttons (Material 3 style)
            PaintMaterialYouButtons(g, owner, captionRect, metrics, primary);
            
            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            // Draw title text with Material 3 typography spacing (16px padding)
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            
            // Paint icon only
            owner._iconRegion?.OnPaint?.Invoke(g, owner.CurrentLayout.IconRect);
        }
        
        /// <summary>
        /// Paint Material You (Material 3) buttons with proper state layers and icons
        /// Following Material 3 guidelines: 40dp touch targets, 24dp icons, rounded ends
        /// </summary>
        private void PaintMaterialYouButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics, Color primaryColor)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            // Material 3: 40dp (40px) touch targets with 24dp (24px) icon areas
            int touchTarget = 40;
            int iconArea = 24;
            int padding = (captionRect.Height - touchTarget) / 2;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Check hover states
            bool closeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("close")) ?? false;
            bool maxHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("maximize")) ?? false;
            bool minHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("minimize")) ?? false;
            bool themeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("theme")) ?? false;
            bool styleHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("Style")) ?? false;
            
            // Close button: Error color with state layer
            PaintMaterialYouButton(g, closeRect, Color.FromArgb(186, 26, 26), padding, touchTarget, iconArea, "close", metrics, closeHovered);
            
            // Maximize button: On-surface variant with state layer
            PaintMaterialYouButton(g, maxRect, Color.FromArgb(73, 69, 79), padding, touchTarget, iconArea, "maximize", metrics, maxHovered);
            
            // Minimize button: On-surface variant with state layer
            PaintMaterialYouButton(g, minRect, Color.FromArgb(73, 69, 79), padding, touchTarget, iconArea, "minimize", metrics, minHovered);
            
            // Theme/Style buttons if shown
            if (owner.ShowStyleButton)
            {
                var styleRect = owner.CurrentLayout.StyleButtonRect;
                PaintMaterialYouButton(g, styleRect, primaryColor, padding, touchTarget, iconArea, "Style", metrics, styleHovered);
            }

            if (owner.ShowThemeButton)
            {
                var themeRect = owner.CurrentLayout.ThemeButtonRect;
                PaintMaterialYouButton(g, themeRect, Color.FromArgb(230, 74, 25), padding, touchTarget, iconArea, "theme", metrics, themeHovered);
            }
        }
        
        private void PaintMaterialYouButton(Graphics g, Rectangle buttonRect, Color baseColor, int padding, int touchTarget, int iconArea, string buttonType, FormPainterMetrics metrics, bool isHovered = false)
        {
            int centerX = buttonRect.X + buttonRect.Width / 2;
            int centerY = buttonRect.Y + buttonRect.Height / 2;
            var touchRect = new Rectangle(centerX - touchTarget / 2, centerY - touchTarget / 2, touchTarget, touchTarget);
            var iconRect = new Rectangle(centerX - iconArea / 2, centerY - iconArea / 2, iconArea, iconArea);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Material 3 state layer (8% opacity -> 20 alpha normally, 12% -> 31 alpha for hover)
            // But let's make it slightly more visible: 20 -> 45
            int alpha = isHovered ? 45 : 20;

            using (var stateBrush = new SolidBrush(Color.FromArgb(alpha, baseColor)))
            {
                g.FillEllipse(stateBrush, touchRect);
            }
            
            // Draw icon with Material 3 style (2dp stroke, round caps)
            int iconSize = 12; // 12px actual icon within 24px icon area
            int iconCenterX = iconRect.X + iconRect.Width / 2;
            int iconCenterY = iconRect.Y + iconRect.Height / 2;
            
            using (var iconPen = new Pen(baseColor, 2f))
            {
                iconPen.StartCap = LineCap.Round;
                iconPen.EndCap = LineCap.Round;
                iconPen.LineJoin = LineJoin.Round;
                
                switch (buttonType)
                {
                    case "close":
                        // Material X with rounded ends
                        g.DrawLine(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2,
                            iconCenterX + iconSize / 2, iconCenterY + iconSize / 2);
                        g.DrawLine(iconPen, iconCenterX + iconSize / 2, iconCenterY - iconSize / 2,
                            iconCenterX - iconSize / 2, iconCenterY + iconSize / 2);
                        break;
                        
                    case "maximize":
                        // Outlined square with 2px rounded corners
                        using (var path = new GraphicsPath())
                        {
                            int halfSize = iconSize / 2;
                            var sqRect = new Rectangle(iconCenterX - halfSize, iconCenterY - halfSize, iconSize, iconSize);
                            int radius = 2;
                            
                            path.AddArc(sqRect.X, sqRect.Y, radius * 2, radius * 2, 180, 90);
                            path.AddArc(sqRect.Right - radius * 2, sqRect.Y, radius * 2, radius * 2, 270, 90);
                            path.AddArc(sqRect.Right - radius * 2, sqRect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                            path.AddArc(sqRect.X, sqRect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                            path.CloseFigure();
                            
                            g.DrawPath(iconPen, path);
                        }
                        break;
                        
                    case "minimize":
                        // Horizontal line with round caps
                        g.DrawLine(iconPen, iconCenterX - iconSize / 2, iconCenterY, iconCenterX + iconSize / 2, iconCenterY);
                        break;
                        
                    case "Style":
                        // Grid icon (Material style)
                        int gridSize = 3;
                        int spacing = 5;
                        for (int i = 0; i < 2; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                int x = iconCenterX - spacing / 2 + i * spacing;
                                int y = iconCenterY - spacing / 2 + j * spacing;
                                g.FillEllipse(new SolidBrush(baseColor), x - gridSize / 2, y - gridSize / 2, gridSize, gridSize);
                            }
                        }
                        break;
                        
                    case "theme":
                        // Contrast icon (Material accessibility)
                        g.DrawEllipse(iconPen, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, iconSize, iconSize);
                        using (var fillBrush = new SolidBrush(baseColor))
                        {
                            g.FillPie(fillBrush, iconCenterX - iconSize / 2, iconCenterY - iconSize / 2, 
                                iconSize, iconSize, 270, 180);
                        }
                        break;
                }
            }
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            // Material 3 uses elevation shadows, no visible borders
            // This method intentionally left minimal for Material You style
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(30, 0, 0, 0),
                Blur = 16,
                OffsetY = 6,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(12);
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.High;
        }

        public bool SupportsAnimations => false;

        public void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            var originalClip = g.Clip;
            var shadow = GetShadowEffect(owner);
            var radius = GetCornerRadius(owner);

            if (!shadow.Inner)
            {
                DrawShadow(g, rect, shadow, radius);
            }

            PaintBackground(g, owner);

            using var path = owner.BorderShape;
            g.Clip = new Region(path);
            g.Clip = originalClip;

            PaintBorders(g, owner);
            if (owner.ShowCaptionBar)
            {
                PaintCaption(g, owner, owner.CurrentLayout.CaptionRect);
            }

            g.Clip = originalClip;
        }

        private void DrawShadow(Graphics g, Rectangle rect, ShadowEffect shadow, CornerRadius radius)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            var shadowRect = new Rectangle(
                rect.X + shadow.OffsetX - shadow.Blur,
                rect.Y + shadow.OffsetY - shadow.Blur,
                rect.Width + shadow.Blur * 2,
                rect.Height + shadow.Blur * 2);
            if (shadowRect.Width <= 0 || shadowRect.Height <= 0) return;
            using var brush = new SolidBrush(shadow.Color);
            using var path = CreateRoundedRectanglePath(shadowRect, radius);
            g.FillPath(brush, path);
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, CornerRadius radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(new Rectangle(rect.X, rect.Y, Math.Max(1, rect.Width), Math.Max(1, rect.Height)));
                return path;
            }
            int maxRadius = Math.Min(rect.Width, rect.Height) / 2;
            int tl = Math.Max(0, Math.Min(radius.TopLeft, maxRadius));
            int tr = Math.Max(0, Math.Min(radius.TopRight, maxRadius));
            int br = Math.Max(0, Math.Min(radius.BottomRight, maxRadius));
            int bl = Math.Max(0, Math.Min(radius.BottomLeft, maxRadius));
            if (tl == 0 && tr == 0 && br == 0 && bl == 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            if (tl > 0) path.AddArc(rect.X, rect.Y, tl * 2, tl * 2, 180, 90); else path.AddLine(rect.X, rect.Y, rect.X, rect.Y);
            if (tr > 0) path.AddArc(rect.Right - tr * 2, rect.Y, tr * 2, tr * 2, 270, 90); else path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);
            if (br > 0) path.AddArc(rect.Right - br * 2, rect.Bottom - br * 2, br * 2, br * 2, 0, 90); else path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);
            if (bl > 0) path.AddArc(rect.X, rect.Bottom - bl * 2, bl * 2, bl * 2, 90, 90); else path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);
            path.CloseFigure();
            return path;
        }

        public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
        {
            var layout = new PainterLayoutInfo();
            
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(owner);
            int captionPadding = DpiScalingHelper.ScaleValue(18, dpiScale);
            var captionHeight = owner.Font.Height + captionPadding;
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            // Material 3: 40dp touch targets
            int buttonWidth = DpiScalingHelper.ScaleValue(40, dpiScale);
            var buttonSize = new Size(buttonWidth, captionHeight);
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
            
            if (owner.ShowCustomActionButton)
            {
                layout.CustomActionButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Search box (between title and buttons)
            int searchBoxWidth = DpiScalingHelper.ScaleValue(200, dpiScale);
            int searchBoxPadding = DpiScalingHelper.ScaleValue(8, dpiScale);
            if (owner.ShowSearchBox)
            {
                layout.SearchBoxRect = new Rectangle(buttonX - searchBoxWidth - searchBoxPadding, buttonY + searchBoxPadding / 2, 
                    searchBoxWidth, captionHeight - searchBoxPadding);
                owner._hits.RegisterHitArea("search", layout.SearchBoxRect, HitAreaType.TextBox);
                buttonX -= searchBoxWidth + searchBoxPadding;
            }
            else
            {
                layout.SearchBoxRect = Rectangle.Empty;
            }
            
            int iconSize = DpiScalingHelper.ScaleValue(24, dpiScale);
            int iconPadding = DpiScalingHelper.ScaleValue(16, dpiScale);
            layout.IconRect = new Rectangle(iconPadding, (captionHeight - iconSize) / 2, iconSize, iconSize);
            owner._hits.RegisterHitArea("icon", layout.IconRect, HitAreaType.Icon);
            
            var titleX = iconPadding + iconSize + iconPadding;
            var titleWidth = buttonX - titleX - iconPadding;
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            owner._hits.RegisterHitArea("title", layout.TitleRect, HitAreaType.Caption);
            
            owner.CurrentLayout = layout;
        }

        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            // Material 3 uses elevation shadows, no visible borders
            // This method intentionally left minimal for Material You style
        }
    }
}
