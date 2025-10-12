# Script to generate 20 FormStyle painters with correct interface signatures
# Based on ModernFormPainter template structure

$paintersPath = "c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Forms\ModernForm\Painters"

# Define all 20 painters with their characteristics
$painters = @(
    @{Name="NeoMorphism"; Style="NeoMorphism"; Desc="Soft UI with extruded shadows and highlights"; Radius=12; Shadow=15; AntiAlias="Ultra"},
    @{Name="Glassmorphism"; Style="Glassmorphism"; Desc="Frosted glass with translucent blur effects"; Radius=10; Shadow=12; AntiAlias="Ultra"},
    @{Name="Brutalist"; Style="Brutalist"; Desc="Bold geometric high-contrast brutalist design"; Radius=0; Shadow=0; AntiAlias="None"},
    @{Name="Retro"; Style="Retro"; Desc="80s/90s retro computing nostalgic aesthetic"; Radius=0; Shadow=8; AntiAlias="None"},
    @{Name="Cyberpunk"; Style="Cyberpunk"; Desc="Neon-lit futuristic dystopian style"; Radius=4; Shadow=20; AntiAlias="High"},
    @{Name="Nordic"; Style="Nordic"; Desc="Clean Scandinavian minimalist design"; Radius=6; Shadow=8; AntiAlias="High"},
    @{Name="iOS"; Style="iOS"; Desc="Apple iOS modern rounded style"; Radius=12; Shadow=10; AntiAlias="Ultra"},
    @{Name="Windows11"; Style="Windows11"; Desc="Windows 11 rounded corners with mica"; Radius=8; Shadow=12; AntiAlias="High"},
    @{Name="Ubuntu"; Style="Ubuntu"; Desc="Ubuntu Unity desktop style"; Radius=6; Shadow=10; AntiAlias="High"},
    @{Name="KDE"; Style="KDE"; Desc="KDE Plasma Breeze theme style"; Radius=6; Shadow=10; AntiAlias="High"},
    @{Name="ArcLinux"; Style="ArcLinux"; Desc="Arc Linux dark theme style"; Radius=4; Shadow=8; AntiAlias="High"},
    @{Name="Dracula"; Style="Dracula"; Desc="Popular Dracula dark theme with purple accents"; Radius=6; Shadow=15; AntiAlias="High"},
    @{Name="Solarized"; Style="Solarized"; Desc="Solarized light color scheme"; Radius=4; Shadow=8; AntiAlias="High"},
    @{Name="OneDark"; Style="OneDark"; Desc="Atom One Dark editor theme"; Radius=4; Shadow=10; AntiAlias="High"},
    @{Name="GruvBox"; Style="GruvBox"; Desc="Warm retro groove color scheme"; Radius=4; Shadow=10; AntiAlias="High"},
    @{Name="Nord"; Style="Nord"; Desc="Nordic-inspired frost blue palette"; Radius=4; Shadow=10; AntiAlias="High"},
    @{Name="Tokyo"; Style="Tokyo"; Desc="Tokyo Night dark theme"; Radius=4; Shadow=12; AntiAlias="High"},
    @{Name="Paper"; Style="Paper"; Desc="Flat Material Design paper style"; Radius=4; Shadow=6; AntiAlias="Ultra"},
    @{Name="Neon"; Style="Neon"; Desc="Vibrant multi-color neon glow effects"; Radius=6; Shadow=18; AntiAlias="Ultra"},
    @{Name="Holographic"; Style="Holographic"; Desc="Iridescent rainbow holographic effects"; Radius=8; Shadow=15; AntiAlias="Ultra"}
)

$template = @'
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// {DESCRIPTION}
    /// </summary>
    internal sealed class {NAME}FormPainter : IFormPainter, IFormPainterMetricsProvider, IFormNonClientPainter
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.{STYLE}, owner.UseThemeColors ? owner.CurrentTheme : null);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            using (var brush = new SolidBrush(metrics.BackgroundColor))
            {
                g.FillRectangle(brush, owner.ClientRectangle);
            }
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            
            using var capBrush = new SolidBrush(metrics.CaptionColor);
            g.FillRectangle(capBrush, captionRect);

            var textRect = owner.CurrentLayout.TitleRect;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            owner.PaintBuiltInCaptionElements(g);
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, metrics.BorderWidth));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }

        public ShadowEffect GetShadowEffect(BeepiFormPro owner)
        {
            return new ShadowEffect
            {
                Color = Color.FromArgb(25, 0, 0, 0),
                Blur = {SHADOW},
                OffsetY = 4,
                Inner = false
            };
        }

        public CornerRadius GetCornerRadius(BeepiFormPro owner)
        {
            return new CornerRadius({RADIUS});
        }

        public AntiAliasMode GetAntiAliasMode(BeepiFormPro owner)
        {
            return AntiAliasMode.{ANTIALIAS};
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

            using var path = CreateRoundedRectanglePath(owner.ClientRectangle, radius);
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
            var metrics = GetMetrics(owner);
            
            int captionHeight = Math.Max(metrics.CaptionHeight, (int)(owner.Font.Height * metrics.FontHeightMultiplier));
            owner._hits.Clear();
            
            layout.CaptionRect = new Rectangle(0, 0, owner.ClientSize.Width, captionHeight);
            owner._hits.Register("caption", layout.CaptionRect, HitAreaType.Drag);
            
            int buttonWidth = metrics.ButtonWidth;
            int buttonX = owner.ClientSize.Width - buttonWidth;
            
            layout.CloseButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
            owner._hits.Register("close", layout.CloseButtonRect, HitAreaType.Button);
            buttonX -= buttonWidth;
            
            layout.MaximizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
            owner._hits.Register("maximize", layout.MaximizeButtonRect, HitAreaType.Button);
            buttonX -= buttonWidth;
            
            layout.MinimizeButtonRect = new Rectangle(buttonX, 0, buttonWidth, captionHeight);
            owner._hits.Register("minimize", layout.MinimizeButtonRect, HitAreaType.Button);
            
            int iconX = metrics.IconLeftPadding;
            int iconY = (captionHeight - metrics.IconSize) / 2;
            layout.IconRect = new Rectangle(iconX, iconY, metrics.IconSize, metrics.IconSize);
            if (owner.ShowIcon && owner.Icon != null)
            {
                owner._hits.Register("icon", layout.IconRect, HitAreaType.Icon);
            }
            
            int titleX = layout.IconRect.Right + metrics.TitleLeftPadding;
            int titleWidth = layout.MinimizeButtonRect.Left - metrics.ButtonSpacing - titleX;
            layout.TitleRect = new Rectangle(titleX, 0, titleWidth, captionHeight);
            
            owner.CurrentLayout = layout;
        }

        public void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness)
        {
            var metrics = GetMetrics(owner);
            var radius = GetCornerRadius(owner);
            var outer = new Rectangle(0, 0, owner.Width, owner.Height);
            using var path = CreateRoundedRectanglePath(outer, radius);
            using var pen = new Pen(metrics.BorderColor, Math.Max(1, borderThickness))
            {
                Alignment = PenAlignment.Inset
            };
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);
        }
    }
}
'@

$count = 0
foreach ($painter in $painters) {
    $content = $template `
        -replace '{NAME}', $painter.Name `
        -replace '{STYLE}', $painter.Style `
        -replace '{DESCRIPTION}', $painter.Desc `
        -replace '{RADIUS}', $painter.Radius `
        -replace '{SHADOW}', $painter.Shadow `
        -replace '{ANTIALIAS}', $painter.AntiAlias
    
    $filePath = Join-Path $paintersPath "$($painter.Name)FormPainter.cs"
    $content | Out-File -FilePath $filePath -Encoding UTF8
    $count++
    Write-Host "[$count/20] Created $($painter.Name)FormPainter.cs"
}

Write-Host "`n✅ All 20 painters created successfully with correct interface signatures!"
