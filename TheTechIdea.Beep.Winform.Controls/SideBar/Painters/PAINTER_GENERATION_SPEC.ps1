# BeepSideBar Painter Generation Script
# This script creates all 12 remaining distinct painters for BeepSideBar

$painters = @(
    @{
        Name = "AntDesignSideBarPainter"
        DisplayName = "AntDesign"
        Description = "Ant Design style - Clean blue accent, card-based layout"
        PrimaryColor = "Color.FromArgb(24, 144, 255)"
        BackColor = "Color.FromArgb(250, 250, 250)"
        BorderRadius = "6"
        HasBorder = $true
        BorderColor = "Color.FromArgb(217, 217, 217)"
    },
    @{
        Name = "MaterialYouSideBarPainter"
        DisplayName = "MaterialYou"
        Description = "Material You - Dynamic theming, rounded corners, vibrant colors"
        PrimaryColor = "Color.FromArgb(103, 80, 164)"
        BackColor = "Color.FromArgb(252, 249, 255)"
        BorderRadius = "16"
        HasGradient = $true
    },
    @{
        Name = "Windows11MicaSideBarPainter"
        DisplayName = "Windows11Mica"
        Description = "Windows 11 Mica - Translucent material, soft shadows"
        PrimaryColor = "Color.FromArgb(0, 103, 192)"
        BackColor = "Color.FromArgb(243, 243, 243)"
        BorderRadius = "4"
        HasAcrylic = $true
    },
    @{
        Name = "MacOSBigSurSideBarPainter"
        DisplayName = "MacOSBigSur"
        Description = "macOS Big Sur - Translucent sidebar, SF symbols"
        PrimaryColor = "Color.FromArgb(0, 122, 255)"
        BackColor = "Color.FromArgb(246, 246, 246)"
        BorderRadius = "8"
        HasVibrancy = $true
    },
    @{
        Name = "ChakraUISideBarPainter"
        DisplayName = "ChakraUI"
        Description = "Chakra UI - Accessible design, clear focus states"
        PrimaryColor = "Color.FromArgb(49, 130, 206)"
        BackColor = "Color.White"
        BorderRadius = "6"
        HasShadow = $true
    },
    @{
        Name = "TailwindCardSideBarPainter"
        DisplayName = "TailwindCard"
        Description = "Tailwind CSS - Utility-first, card-based design"
        PrimaryColor = "Color.FromArgb(59, 130, 246)"
        BackColor = "Color.FromArgb(249, 250, 251)"
        BorderRadius = "8"
        HasBorder = $true
        BorderColor = "Color.FromArgb(229, 231, 235)"
    },
    @{
        Name = "NotionMinimalSideBarPainter"
        DisplayName = "NotionMinimal"
        Description = "Notion-inspired - Clean, minimal with hover effects"
        PrimaryColor = "Color.FromArgb(55, 53, 47)"
        BackColor = "Color.FromArgb(255, 255, 255)"
        BorderRadius = "3"
        IsFlat = $true
    },
    @{
        Name = "VercelCleanSideBarPainter"
        DisplayName = "VercelClean"
        Description = "Vercel - Ultra-minimal black and white"
        PrimaryColor = "Color.Black"
        BackColor = "Color.White"
        BorderRadius = "0"
        IsFlat = $true
    },
    @{
        Name = "StripeDashboardSideBarPainter"
        DisplayName = "StripeDashboard"
        Description = "Stripe-inspired - Professional purple theme"
        PrimaryColor = "Color.FromArgb(99, 91, 255)"
        BackColor = "Color.FromArgb(248, 250, 252)"
        BorderRadius = "6"
        HasShadow = $true
    },
    @{
        Name = "DarkGlowSideBarPainter"
        DisplayName = "DarkGlow"
        Description = "Dark theme with neon glow effects"
        PrimaryColor = "Color.FromArgb(0, 255, 255)"
        BackColor = "Color.FromArgb(18, 18, 18)"
        BorderRadius = "8"
        HasGlow = $true
    },
    @{
        Name = "DiscordStyleSideBarPainter"
        DisplayName = "DiscordStyle"
        Description = "Discord-inspired - Blurple accent, dark theme"
        PrimaryColor = "Color.FromArgb(88, 101, 242)"
        BackColor = "Color.FromArgb(47, 49, 54)"
        BorderRadius = "4"
        IsDark = $true
    },
    @{
        Name = "GradientModernSideBarPainter"
        DisplayName = "GradientModern"
        Description = "Modern gradients - Purple to pink vibrant theme"
        PrimaryColor = "Color.FromArgb(168, 85, 247)"
        BackColor = "Color.FromArgb(250, 250, 250)"
        BorderRadius = "12"
        HasGradient = $true
        SecondaryColor = "Color.FromArgb(236, 72, 153)"
    }
)

Write-Host "BeepSideBar Painter Generation Guide"
Write-Host "======================================"
Write-Host ""
Write-Host "Total painters to create: $($painters.Count)"
Write-Host ""

foreach ($painter in $painters) {
    Write-Host "Painter: $($painter.Name)" -ForegroundColor Cyan
    Write-Host "  Display: $($painter.DisplayName)"
    Write-Host "  Style: $($painter.Description)"
    Write-Host "  Primary: $($painter.PrimaryColor)"
    Write-Host "  Background: $($painter.BackColor)"
    Write-Host "  Radius: $($painter.BorderRadius)"
    Write-Host ""
}

Write-Host "Use these specifications to create 12 unique painter files following the pattern of Material3SideBarPainter.cs"
