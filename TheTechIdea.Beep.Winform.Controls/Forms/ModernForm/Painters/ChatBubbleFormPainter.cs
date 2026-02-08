using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;


namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// A painter inspired by chat UI bubbles: soft gradients, speech bubble tails, and message-like appearance (synced with ChatBubbleTheme).
    /// 
    /// ChatBubble Color Palette (synced with ChatBubbleTheme):
    /// - Background: #F0F0F0 (240, 240, 240) - Light gray base (chat canvas)
    /// - Foreground: #000000 (0, 0, 0) - Black text
    /// - Border: #CCCCCC (204, 204, 204) - Light gray border
    /// - Hover: #E0E0E0 (224, 224, 224) - Light gray hover
    /// - Selected: #4A90E2 (74, 144, 226) - Blue selected (chat bubble)
    /// 
    /// Features:
    /// - Rounded bubble shapes
    /// - Message-style shadows
    /// - Chat application aesthetics
    /// - Faint diagonal stripes pattern
    /// - Compositing mode management to prevent overlay accumulation
    /// </summary>
    internal sealed class ChatBubbleFormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.ChatBubble, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            
            // CRITICAL: Set compositing mode to SourceCopy to ensure we fully replace pixels
            // This prevents semi-transparent overlays from accumulating on repaint
            var previousCompositing = g.CompositingMode;
            g.CompositingMode = CompositingMode.SourceCopy;
            
            // Soft chat canvas base
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }

            // Restore compositing mode for semi-transparent overlays
            g.CompositingMode = CompositingMode.SourceOver;

            // Faint diagonal stripes pattern (very light)
            var r = owner.ClientRectangle;
            using var pen = new Pen(Color.FromArgb(10, 0, 0, 0), 1f);
            for (int y = r.Top - r.Width; y < r.Bottom; y += 24)
            {
                g.DrawLine(pen, r.Left, y, r.Left + r.Width, y + r.Width);
            }
            
            // Restore original compositing mode
            g.CompositingMode = previousCompositing;
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Create speech bubble shape with tail
            using var path = CreateSpeechBubblePath(captionRect, 8);

            // Bubble gradient (light blue to white)
            using var bubbleBrush = new LinearGradientBrush(
                captionRect,
                Color.FromArgb(255, 220, 248, 255), // Light blue
                Color.FromArgb(255, 255, 255, 255), // White
                LinearGradientMode.Vertical);
            g.FillPath(bubbleBrush, path);

            // Bubble border
            using var borderPen = new Pen(Color.FromArgb(200, 100, 149, 237), 2f);
            g.DrawPath(borderPen, path);

            // Message typing dots (positioned left of title to avoid overlap)
            DrawMessageDots(g, new Rectangle(captionRect.Left + 8, captionRect.Top, owner.CurrentLayout.TitleRect.Left - (captionRect.Left + 16), captionRect.Height));

            // Paint search box if visible (using FormRegion for consistency)
            if (owner.ShowSearchBox && owner.CurrentLayout.SearchBoxRect.Width > 0)
            {
                owner.SearchBox?.OnPaint?.Invoke(g, owner.CurrentLayout.SearchBoxRect);
            }

            // Title as "message sender"
            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect,
                  metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Built-in caption elements
            // Paint custom ChatBubble buttons (Rounded, soft, message-like)
            PaintChatBubbleButtons(g, owner, captionRect, metrics);
        }

        /// <summary>
        /// Paint ChatBubble buttons (Rounded, soft, message-like appearance)
        /// </summary>
        private void PaintChatBubbleButtons(Graphics g, BeepiFormPro owner, Rectangle captionRect, FormPainterMetrics metrics)
        {
            var closeRect = owner.CurrentLayout.CloseButtonRect;
            var maxRect = owner.CurrentLayout.MaximizeButtonRect;
            var minRect = owner.CurrentLayout.MinimizeButtonRect;
            
            int buttonSize = 22;
            int padding = (captionRect.Height - buttonSize) / 2;
            
            // Check hover states
            bool closeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("close")) ?? false;
            bool maxHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("maximize")) ?? false;
            bool minHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("minimize")) ?? false;
            bool themeHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("theme")) ?? false;
            bool styleHovered = owner._interact?.IsHovered(owner._hits?.GetHitArea("Style")) ?? false;
            
            // Shared styling for chat buttons
            // Close: Red bubble
            PaintChatBubbleButton(g, closeRect, Color.FromArgb(255, 90, 90), "close", closeHovered);
            
            // Maximize: Gray/Blue bubble
            PaintChatBubbleButton(g, maxRect, Color.FromArgb(100, 149, 237), "maximize", maxHovered);
            
            // Minimize: Gray/Blue bubble
            PaintChatBubbleButton(g, minRect, Color.FromArgb(100, 149, 237), "minimize", minHovered);
            
            // Theme/Style buttons
            if (owner.ShowStyleButton)
            {
                PaintChatBubbleButton(g, owner.CurrentLayout.StyleButtonRect, Color.FromArgb(147, 112, 219), "Style", styleHovered);
            }
            
            if (owner.ShowThemeButton)
            {
                PaintChatBubbleButton(g, owner.CurrentLayout.ThemeButtonRect, Color.FromArgb(255, 165, 0), "theme", themeHovered);
            }
        }
        
        private void PaintChatBubbleButton(Graphics g, Rectangle rect, Color baseColor, string type, bool isHovered)
        {
            int cx = rect.X + rect.Width / 2;
            int cy = rect.Y + rect.Height / 2;
            int size = 18;
            
            // Hover effect: Brighten color and slight scale
            if (isHovered)
            {
                baseColor = ControlPaint.Light(baseColor, 0.2f);
                size += 2;
            }
            
            int radius = size / 2;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw bubble shape (rounded rect or circle)
            // Using circle for buttons to look like user avatars/status icons
            using (var brush = new SolidBrush(baseColor))
            {
                g.FillEllipse(brush, cx - radius, cy - radius, size, size);
            }
            
            // Draw Icon (White)
            using (var iconPen = new Pen(Color.White, 1.5f))
            {
                int iconSize = 8;
                
                switch (type)
                {
                    case "close":
                        g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy + iconSize/2);
                        g.DrawLine(iconPen, cx + iconSize/2, cy - iconSize/2, cx - iconSize/2, cy + iconSize/2);
                        break;
                    case "maximize":
                        g.DrawRectangle(iconPen, cx - iconSize/2, cy - iconSize/2, iconSize, iconSize);
                        break;
                    case "minimize":
                        g.DrawLine(iconPen, cx - iconSize/2, cy + iconSize/2, cx + iconSize/2, cy + iconSize/2);
                        break;
                    case "Style": 
                         g.DrawEllipse(iconPen, cx - iconSize/2, cy - iconSize/2, iconSize, iconSize);
                         g.FillEllipse(Brushes.White, cx, cy, 2, 2);
                        break;
                    case "theme": 
                         g.DrawRectangle(iconPen, cx - iconSize/2, cy - iconSize/2, iconSize, iconSize - 2);
                         g.DrawLine(iconPen, cx - iconSize/2, cy - iconSize/2, cx + iconSize/2, cy - iconSize/2);
                        break;
                }
            }
            
            // Hover: Add tiny speech tail to button?
            if (isHovered)
            {
                using (var brush = new SolidBrush(baseColor))
                {
                    // Tiny tail pointing down-right
                    var points = new Point[] 
                    {
                        new Point(cx + radius - 3, cy + radius - 3),
                        new Point(cx + radius + 2, cy + radius + 2),
                        new Point(cx + radius - 1, cy + radius - 5)
                    };
                    g.FillPolygon(brush, points);
                }
            }
        }
        

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = owner.BorderShape;
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, metrics.BorderWidth));
            pen.Alignment = PenAlignment.Center;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        private static void AddRoundedRect(GraphicsPath path, Rectangle rect, int radius)
        {
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return;
            }
            int d = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(d, d));

            // top-left
            path.AddArc(arc, 180, 90);

            // top-right
            arc.X = rect.Right - d;
            path.AddArc(arc, 270, 90);

            // bottom-right
            arc.Y = rect.Bottom - d;
            path.AddArc(arc, 0, 90);

            // bottom-left
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(20, 100, 149, 237),
                Blur = 12,
                OffsetY = 6,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius(16);
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.Ultra;
        }

        public bool SupportsAnimations => false;

        public void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            // Orchestrated: shadow, base background, clipped background effects (none), borders, caption
            var originalClip = g.Clip;
            var shadow = GetShadowEffect(owner);
            var radius = GetCornerRadius(owner);

            // 1) Shadow
            if (!shadow.Inner)
            {
                DrawShadow(g, rect, shadow, radius);
            }

            // 2) Base background
            PaintBackground(g, owner);

            // 3) Background effects with clipping (none; caption owns bubble)
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            // Always paint over entire form background since this runs in OnPaintBackground
            g.Clip = new Region(path);

            PaintBackgroundEffects(g, owner, rect);

            // 4) Reset clip
            g.Clip = originalClip;

            // 5) Borders and caption
            PaintBorders(g, owner);
            if (owner.ShowCaptionBar)
            {
                PaintCaption(g, owner, owner.CurrentLayout.CaptionRect);
            }

            g.Clip = originalClip;
        }

        private void PaintBackgroundEffects(Graphics g, BeepiFormPro owner, Rectangle rect)
        {
            // No additional form-level effects; caption provides bubble look
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

        // Painter-owned non-client border rendering for ChatBubble Style
        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            var outerRect = new Rectangle(0, 0, owner.Width, owner.Height);
            using var path = CreateRoundedRectanglePath(outerRect, radius);
            using var pen = new Pen(Color.FromArgb(180, metrics.BorderColor), Math.Max(1, borderThickness))
            {
                Alignment = PenAlignment.Inset,
                LineJoin = LineJoin.Round
            };
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);

            // Tiny top-left tail hint: a small triangular notch inside the border for character
            int t = Math.Max(0, borderThickness - 1);
            var tri = new[]
            {
                new Point(12, t),
                new Point(16, t),
                new Point(14, t + 4)
            };
            using var tailBrush = new SolidBrush(Color.FromArgb(160, metrics.BorderColor));
            g.FillPolygon(tailBrush, tri);
        }

        /// <summary>
        /// Creates a speech bubble path with rounded corners and a tail
        /// </summary>
        private GraphicsPath CreateSpeechBubblePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
            // Main bubble rectangle with rounded corners
            var bubbleRect = Rectangle.Inflate(rect, -8, -4);
            AddRoundedRect(path, bubbleRect, radius);
            
            // Add speech bubble tail
            int tailWidth = 8;
            int tailHeight = 6;
            var tailPoints = new Point[]
            {
                new Point(bubbleRect.Left + 20, bubbleRect.Bottom),
                new Point(bubbleRect.Left + 20 + tailWidth/2, bubbleRect.Bottom + tailHeight),
                new Point(bubbleRect.Left + 20 + tailWidth, bubbleRect.Bottom)
            };
            path.AddPolygon(tailPoints);
            
            return path;
        }

        /// <summary>
        /// Draws typing indicator dots like in chat apps
        /// </summary>
        private void DrawMessageDots(Graphics g, Rectangle rect)
        {
            using var dotBrush = new SolidBrush(Color.FromArgb(150, 100, 149, 237));
            int dotSize = 3;
            int spacing = 6;
            int startX = Math.Max(rect.Left + 8, rect.Right - 40);
            int centerY = rect.Top + rect.Height / 2;
            
            // Draw three dots
            for (int i = 0; i < 3; i++)
            {
                int x = startX + i * spacing;
                g.FillEllipse(dotBrush, x, centerY - dotSize/2, dotSize, dotSize);
            }
        }

        public void CalculateLayoutAndHitAreas(BeepiFormPro owner)
        {
            var layout = new PainterLayoutInfo();
            
            // If caption bar is hidden, skip button layout
            if (!owner.ShowCaptionBar)
            {
                layout.CaptionRect = Rectangle.Empty;
                layout.ContentRect = new Rectangle(0, 0, owner.ClientSize.Width, owner.ClientSize.Height);
                owner.CurrentLayout = layout;
                return;
            }
            
            // Calculate caption height based on font and padding (Chat bubble uses rounded padding)
            var captionHeight = owner.Font.Height + 20; // 10px padding top and bottom for chat bubble Style
            
            // Set caption rectangle
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            
            // Set content rectangle (below caption)
            layout.ContentRect = new Rectangle(0, captionHeight, owner.ClientSize.Width, owner.ClientSize.Height - captionHeight);
            
            // Calculate button positions (right-aligned in caption, chat bubble Style)
            var buttonSize = new Size(32, captionHeight);
            var buttonY = 0;
            var buttonX = owner.ClientSize.Width - buttonSize.Width;
            
            // Close button
            if (owner.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("close", layout.CloseButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Maximize/Minimize buttons
            if (owner.ShowMinMaxButtons)
            {
                layout.MaximizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
                
                layout.MinimizeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Style button (if shown)
            if (owner.ShowStyleButton)
            {
                layout.StyleButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("Style", layout.StyleButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Theme button (if shown)
            if (owner.ShowThemeButton)
            {
                layout.ThemeButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("theme", layout.ThemeButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Custom action button (only if ShowCustomActionButton is true)
            if (owner.ShowCustomActionButton)
            {
                layout.CustomActionButtonRect = new Rectangle(buttonX, buttonY, buttonSize.Width, buttonSize.Height);
                owner._hits.RegisterHitArea("customAction", layout.CustomActionButtonRect, HitAreaType.Button);
                buttonX -= buttonSize.Width;
            }
            
            // Search box (between title and buttons)
            int searchBoxWidth = 200;
            int searchBoxPadding = 8;
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
            
            // Icon and title areas (left side of caption, chat bubble Style)
            var iconSize = 16;
            var iconPadding = 12; // Rounded padding for chat bubble
            layout.IconRect = new Rectangle(iconPadding, (captionHeight - iconSize) / 2, iconSize, iconSize);
            owner._hits.RegisterHitArea("icon", layout.IconRect, HitAreaType.Icon);
            
            var titleX = iconPadding + iconSize + iconPadding;
            var titleWidth = buttonX - titleX - iconPadding;
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            owner._hits.RegisterHitArea("title", layout.TitleRect, HitAreaType.Caption);
            
            owner.CurrentLayout = layout;
        }
    }
}
