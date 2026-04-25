using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls; // TabStyle enum
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers
{
    /// <summary>
    /// Helper class for professional tab rendering with modern styling using BeepStyling
    /// </summary>
    internal class TabPaintHelper
    {
        private IBeepTheme _theme;
        private BeepControlStyle _controlStyle = BeepControlStyle.Modern;
        private bool _isTransparent = false;
        private TabStyle _tabStyle = TabStyle.Capsule; // default for DC2

        public Control OwnerControl { get; set; }

        public TabPaintHelper(IBeepTheme theme)
        {
            _theme = theme;
            // Default to Modern style for tabs - will be updated by container if needed
            _controlStyle = BeepControlStyle.Modern;
        }
        
        public TabPaintHelper(IBeepTheme theme, BeepControlStyle controlStyle)
        {
            _theme = theme;
            _controlStyle = controlStyle;
        }
        
        public TabPaintHelper(IBeepTheme theme, BeepControlStyle controlStyle, bool isTransparent)
        {
            _theme = theme;
            _controlStyle = controlStyle;
            _isTransparent = isTransparent;
        }
        
        public BeepControlStyle ControlStyle
        {
            get => _controlStyle;
            set => _controlStyle = value;
        }
        public TabStyle TabStyle
        {
            get => _tabStyle;
            set => _tabStyle = value;
        }

        public bool IsTransparent
        {
            get => _isTransparent;
            set => _isTransparent = value;
        }

        /// <summary>
        /// Border radius of the owning container.  Used to shape the outer corners of the
        /// first and last visible tabs so they follow the container's rounded outline.
        /// Set to 0 when the container is not rounded.
        /// </summary>
        public int ContainerBorderRadius { get; set; } = 0;

        public IBeepTheme Theme
        {
            get => _theme;
            set => _theme = value;
        }

        /// <summary>
        /// Draws a professional tab with rounded corners, gradients, and theme colors.
        /// </summary>
        /// <param name="isFirst">True when this is the first visible tab — outer corner follows ContainerBorderRadius.</param>
        /// <param name="isLast">True when this is the last visible tab — outer corner follows ContainerBorderRadius.</param>
        /// <param name="tabPosition">Position of the tab strip so outer-corner direction can be determined.</param>
        public void DrawProfessionalTab(Graphics g, Rectangle bounds, string title, Font font,
            bool isActive, bool isHovered, bool showCloseButton, bool isCloseHovered,
            float animationProgress = 0f,
            bool isFirst = false, bool isLast = false,
            TabPosition tabPosition = TabPosition.Top)
        {
            // Delegate to the extended overload with no icon, no badge, not pinned.
            DrawProfessionalTab(g, bounds, title, font, isActive, isHovered, showCloseButton,
                isCloseHovered, animationProgress, isFirst, isLast, tabPosition,
                iconPath: null, badgeText: null, badgeColor: Color.Empty, isPinned: false);
        }

        /// <summary>
        /// Extended overload that also renders an optional icon (via StyledImagePainter),
        /// a notification badge pill, and supports pinned (compact) tabs.
        /// </summary>
        public void DrawProfessionalTab(Graphics g, Rectangle bounds, string title, Font font,
            bool isActive, bool isHovered, bool showCloseButton, bool isCloseHovered,
            float animationProgress,
            bool isFirst, bool isLast,
            TabPosition tabPosition,
            string iconPath, string badgeText, Color badgeColor, bool isPinned)
        {
            // Validate input parameters
            if (g == null || bounds.Width <= 0 || bounds.Height <= 0 || font == null)
                return;

            // Ensure title is not null
            if (string.IsNullOrEmpty(title))
                title = "Tab";

            // Pinned tabs never show the close button
            if (isPinned) showCloseButton = false;

            bool hasIcon = !string.IsNullOrEmpty(iconPath);

            // Clamp animation progress
            animationProgress = Math.Max(0f, Math.Min(1f, animationProgress));

            try
            {
                // Get theme colors
                var colors = GetTabColors(isActive, isHovered, animationProgress);
                
                // Draw tab background with gradient or style-specific background
                switch (_tabStyle)
                {
                    case TabStyle.Classic:
                        DrawTabBackground(g, bounds, colors, isActive, isHovered, isFirst, isLast, tabPosition);
                        break;
                    case TabStyle.Capsule:
                        DrawCapsuleBackground(g, bounds, colors, isActive, isHovered, isFirst, isLast, tabPosition);
                        break;
                    case TabStyle.Underline:
                        // Underline has no background; we'll draw underline separately later
                        break;
                    case TabStyle.Minimal:
                        // Minimal draws no background
                        break;
                    case TabStyle.Segmented:
                        DrawSegmentBackground(g, bounds, colors, isActive, isHovered, isFirst, isLast, tabPosition);
                        break;
                    default:
                        DrawTabBackground(g, bounds, colors, isActive, isHovered, isFirst, isLast, tabPosition);
                        break;
                }
                
                // Draw tab border
                DrawTabBorder(g, bounds, colors.BorderColor, isActive);

                // ── Phase 6: Hover shadow lift ───────────────────────────────
                // For non-active tabs, draw a subtle drop shadow that fades in
                // with the animation progress for a modern elevation effect.
                if (!isActive && animationProgress > 0.01f)
                {
                    int shadowAlpha = (int)(animationProgress * 22);
                    var shadowRect = new Rectangle(bounds.X + 1, bounds.Bottom - 1,
                        bounds.Width - 2, 2);
                    using (var sb = new SolidBrush(Color.FromArgb(shadowAlpha, Color.Black)))
                        g.FillRectangle(sb, shadowRect);
                }

                // Active indicator line — drawn for styles that don't already rely solely
                // on background fill to indicate selection.
                if (isActive)
                {
                    switch (_tabStyle)
                    {
                        case TabStyle.Classic:
                        case TabStyle.Card:
                        case TabStyle.Segmented:
                        case TabStyle.Button:
                            DrawActiveIndicator(g, bounds, tabPosition);
                            break;
                    }
                }

                // ── Icon ──────────────────────────────────────────────────────
                if (hasIcon)
                {
                    DrawTabIcon(g, bounds, iconPath, colors.TextColor);
                }

                // ── Title text ────────────────────────────────────────────────
                // Pinned tabs are icon-only — skip the title.
                if (!isPinned)
                {
                    var textBounds = TabHeaderMetrics.GetTextBounds(bounds, showCloseButton, hasIcon, OwnerControl);
                    if (textBounds.Width > 10 && textBounds.Height > 10)
                    {
                        DrawTabText(g, textBounds, title, font, colors.TextColor, isActive);
                    }
                }
                
                // ── Close button (fade-in with animation) ────────────────────
                int closeW = TabHeaderMetrics.CloseButtonSlotWidth(OwnerControl);
                int closeSize = TabHeaderMetrics.CloseButtonSize(OwnerControl);
                if (showCloseButton && bounds.Width > closeW + 10 && bounds.Height > closeSize + 4)
                {
                    // Phase 6: Fade-in close button with animation progress
                    // Active tabs always show the close button; hovered tabs fade it in.
                    float closeAlpha = isActive ? 1f : animationProgress;
                    if (closeAlpha > 0.05f)
                    {
                        var closeRect = TabHeaderMetrics.GetCloseButtonBounds(bounds, OwnerControl);
                        Color closeTint = Color.FromArgb((int)(closeAlpha * 160), colors.TextColor);
                        DrawCloseButton(g, closeRect, isCloseHovered, closeTint);
                    }
                }

                // ── Badge pill ────────────────────────────────────────────────
                if (!string.IsNullOrEmpty(badgeText))
                {
                    DrawBadge(g, bounds, badgeText, badgeColor, font);
                }

                // If style is underline or minimal and active, draw underline accent
                if ((_tabStyle == TabStyle.Underline || _tabStyle == TabStyle.Minimal) && isActive)
                {
                    DrawUnderline(g, bounds, colors, tabPosition);
                }
            }
            catch
            {
                // Fallback to simple rectangle drawing
                try
                {
                    using (var brush = new SolidBrush(isActive ? ColorUtils.MapSystemColor(SystemColors.ControlLight) : ColorUtils.MapSystemColor(SystemColors.Control)))
                    {
                        g.FillRectangle(brush, bounds);
                    }
                    using (var pen = new Pen(ColorUtils.MapSystemColor(SystemColors.ControlDark)))
                    {
                        g.DrawRectangle(pen, bounds);
                    }
                    
                    // Simple text rendering as fallback
                    if (!string.IsNullOrEmpty(title) && bounds.Width > 20 && bounds.Height > 10)
                    {
                        var textRect = new Rectangle(bounds.X + 4, bounds.Y + 2, bounds.Width - 8, bounds.Height - 4);
                        TextRenderer.DrawText(g, title, font, textRect, ColorUtils.MapSystemColor(SystemColors.ControlText), 
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | 
                            TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
                    }
                }
                catch
                {
                    // If even the fallback fails, give up gracefully
                }
            }
        }

        // ── Icon rendering via StyledImagePainter ─────────────────────────────
        private void DrawTabIcon(Graphics g, Rectangle tabBounds, string iconPath, Color tintColor)
        {
            var iconRect = TabHeaderMetrics.GetIconBounds(tabBounds, OwnerControl);
            if (iconRect.Width <= 0 || iconRect.Height <= 0) return;

            try
            {
                StyledImagePainter.PaintWithTint(g, iconRect, iconPath, tintColor, 1f, 0);
            }
            catch
            {
                // If StyledImagePainter cannot resolve the path, silently skip.
            }
        }

        // ── Notification badge pill ───────────────────────────────────────────
        private void DrawBadge(Graphics g, Rectangle tabBounds, string badgeText, Color badgeColor, Font baseFont)
        {
            if (string.IsNullOrEmpty(badgeText)) return;

            // Resolve badge colour: provided → theme accent → fallback red
            Color bg = badgeColor;
            if (bg == Color.Empty || bg.A == 0)
                bg = _theme?.ActiveBorderColor ?? Color.FromArgb(220, 60, 60);

            // Measure badge text at a smaller font size
            float badgeFontSize = Math.Max(7f, baseFont.Size * 0.72f);
            var badgeFont = FontListHelper.GetFont(baseFont.FontFamily.Name, badgeFontSize, FontStyle.Bold) ?? baseFont;
            try
            {
                int textWidth = TextRenderer.MeasureText(badgeText, badgeFont,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width;

                var badgeRect = TabHeaderMetrics.GetBadgeBounds(tabBounds, textWidth, OwnerControl);
                if (badgeRect.Width <= 0 || badgeRect.Height <= 0) return;

                int radius = badgeRect.Height / 2;
                using (var path = CreateRoundedPath(badgeRect, radius))
                using (var brush = new SolidBrush(bg))
                {
                    g.FillPath(brush, path);
                }

                // White text on the badge for contrast
                TextRenderer.DrawText(g, badgeText, badgeFont, badgeRect, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
            }
            finally
            {
                if (badgeFont != baseFont) badgeFont?.Dispose();
            }
        }

        private void DrawUnderline(Graphics g, Rectangle bounds, TabColors colors, TabPosition tabPosition = TabPosition.Top)
        {
            // Draw a thin accent line on the content-facing edge of the tab.
            int thickness = Math.Max(2, bounds.Height / 8);
            int hPad = TabHeaderMetrics.HorizontalPadding(OwnerControl);
            int vPad = TabHeaderMetrics.VerticalPadding(OwnerControl);
            Rectangle r;
            switch (tabPosition)
            {
                case TabPosition.Bottom:
                    r = new Rectangle(bounds.X + hPad, bounds.Y + vPad, Math.Max(10, bounds.Width - (hPad * 2)), thickness);
                    break;
                case TabPosition.Left:
                    r = new Rectangle(bounds.Right - thickness - vPad, bounds.Y + hPad, thickness, Math.Max(10, bounds.Height - (hPad * 2)));
                    break;
                case TabPosition.Right:
                    r = new Rectangle(bounds.X + vPad, bounds.Y + hPad, thickness, Math.Max(10, bounds.Height - (hPad * 2)));
                    break;
                default: // Top
                    r = new Rectangle(bounds.X + hPad, bounds.Bottom - thickness - vPad, Math.Max(10, bounds.Width - (hPad * 2)), thickness);
                    break;
            }
            using (var brush = new SolidBrush(IndicatorColor()))
                g.FillRectangle(brush, r);
        }

        /// <summary>
        /// Draws the active-tab accent indicator line — a short colored bar on the
        /// content-facing edge of the tab, styled after modern browser/IDE tab strips.
        /// Thickness: 3 px (DPI-scaled when an <see cref="OwnerControl"/> is set).
        /// Color: <c>theme.ActiveBorderColor</c> → <c>theme.TabSelectedBorderColor</c> → border fallback.
        /// </summary>
        private void DrawActiveIndicator(Graphics g, Rectangle bounds, TabPosition tabPosition)
        {
            int thickness = TabHeaderMetrics.IndicatorThickness(OwnerControl);

            Color color = IndicatorColor();

            // Inset the bar slightly from the tab edges so it sits within the tab bounds.
            int inset = TabHeaderMetrics.IndicatorInset(OwnerControl);

            Rectangle bar;
            switch (tabPosition)
            {
                case TabPosition.Bottom:
                    // Tab strip is below content — indicator is at the top edge of the tab.
                    bar = new Rectangle(
                        bounds.X + inset,
                        bounds.Y,
                        Math.Max(4, bounds.Width - inset * 2),
                        thickness);
                    break;
                case TabPosition.Left:
                    // Tab strip is to the left — indicator is on the right edge of the tab.
                    bar = new Rectangle(
                        bounds.Right - thickness,
                        bounds.Y + inset,
                        thickness,
                        Math.Max(4, bounds.Height - inset * 2));
                    break;
                case TabPosition.Right:
                    // Tab strip is to the right — indicator is on the left edge of the tab.
                    bar = new Rectangle(
                        bounds.X,
                        bounds.Y + inset,
                        thickness,
                        Math.Max(4, bounds.Height - inset * 2));
                    break;
                default: // Top (most common)
                    // Tab strip is above content — indicator is at the bottom edge of the tab.
                    bar = new Rectangle(
                        bounds.X + inset,
                        bounds.Bottom - thickness,
                        Math.Max(4, bounds.Width - inset * 2),
                        thickness);
                    break;
            }

            // Round the indicator bar ends for a pill-shaped look.
            int barRadius = thickness / 2;
            if (barRadius >= 1 && bar.Width > barRadius * 2 && bar.Height > barRadius * 2)
            {
                using (var path = CreateRoundedPath(bar, barRadius))
                using (var brush = new SolidBrush(color))
                    g.FillPath(brush, path);
            }
            else
            {
                using (var brush = new SolidBrush(color))
                    g.FillRectangle(brush, bar);
            }
        }

        /// <summary>
        /// Returns the best available accent colour from the current theme to use as the
        /// active-tab indicator.  Preference order:
        /// <list type="number">
        ///   <item><c>ActiveBorderColor</c></item>
        ///   <item><c>TabSelectedBorderColor</c></item>
        ///   <item><c>TabSelectedForeColor</c></item>
        ///   <item>DodgerBlue (hard fallback)</item>
        /// </list>
        /// </summary>
        private Color IndicatorColor()
        {
            var c = _theme?.ActiveBorderColor ?? Color.Empty;
            if (c == Color.Empty || c.A == 0) c = _theme?.TabSelectedBorderColor ?? Color.Empty;
            if (c == Color.Empty || c.A == 0) c = _theme?.TabSelectedForeColor ?? Color.Empty;
            if (c == Color.Empty || c.A == 0) c = Color.DodgerBlue;
            return c;
        }

        private void DrawCapsuleBackground(Graphics g, Rectangle bounds, TabColors colors, bool isActive, bool isHovered,
            bool isFirst = false, bool isLast = false, TabPosition tabPosition = TabPosition.Top)
        {
            // Normal capsule radius (half-height).
            int radius = Math.Max(6, bounds.Height / 2);

            // BackgroundColor already contains the correct active/inactive/hover color from
            // GetTabColors() — no additional alpha dimming needed here.
            using (var brush = new SolidBrush(colors.BackgroundColor))
            {
                GraphicsPath path = ContainerBorderRadius > 0
                    ? CreateTabCornerPath(bounds, radius, ContainerBorderRadius, isFirst, isLast, tabPosition)
                    : CreateRoundedPath(bounds, radius);
                using (path)
                    g.FillPath(brush, path);
            }

            // Draw a light border on the ACTIVE tab so it reads as a lifted surface
            // standing out from the strip background. Inactive tabs blend in.
            if (isActive)
            {
                GraphicsPath borderPath = ContainerBorderRadius > 0
                    ? CreateTabCornerPath(bounds, radius, ContainerBorderRadius, isFirst, isLast, tabPosition)
                    : CreateRoundedPath(bounds, radius);
                using (borderPath)
                using (var pen = new Pen(colors.BorderColor, 1f) { Alignment = PenAlignment.Inset })
                    g.DrawPath(pen, borderPath);
            }
        }

        private void DrawSegmentBackground(Graphics g, Rectangle bounds, TabColors colors, bool isActive, bool isHovered,
            bool isFirst = false, bool isLast = false, TabPosition tabPosition = TabPosition.Top)
        {
            GraphicsPath path = ContainerBorderRadius > 0
                ? CreateTabCornerPath(bounds, 6, ContainerBorderRadius, isFirst, isLast, tabPosition)
                : CreateRoundedPath(bounds, 6);

            using (path)
            {
                // BackgroundColor already encodes active/inactive/hover from GetTabColors().
                using (var brush = new SolidBrush(colors.BackgroundColor))
                    g.FillPath(brush, path);
                // Draw border only on the active tab for visual prominence.
                if (isActive)
                {
                    using (var pen = new Pen(colors.BorderColor, 1f) { Alignment = PenAlignment.Inset })
                        g.DrawPath(pen, path);
                }
            }
        }

        private TabColors GetTabColors(bool isActive, bool isHovered, float animationProgress)
        {
            Color backColor, textColor, borderColor;

            if (isActive)
            {
                // Prefer semantic active-tab colors; fall back through the less-specific chain.
                backColor = ResolveNonEmpty(_theme?.ActiveTabBackColor, _theme?.TabSelectedBackColor, Color.White);
                textColor = ResolveNonEmpty(_theme?.ActiveTabForeColor, _theme?.TabSelectedForeColor, Color.FromArgb(32, 32, 32));
                borderColor = ResolveNonEmpty(_theme?.TabSelectedBorderColor, _theme?.ActiveBorderColor, Color.FromArgb(120, 120, 120));
            }
            else if (isHovered)
            {
                // Inactive base is the InactiveTabBackColor if available.
                var baseColor = ResolveNonEmpty(_theme?.InactiveTabBackColor, _theme?.TabBackColor, Color.FromArgb(240, 240, 240));
                var hoverColor = ResolveNonEmpty(_theme?.TabHoverBackColor, Color.FromArgb(225, 225, 225));

                backColor = InterpolateColor(baseColor, hoverColor, animationProgress);
                textColor = ResolveNonEmpty(_theme?.TabHoverForeColor, _theme?.InactiveTabForeColor, _theme?.TabForeColor, Color.FromArgb(64, 64, 64));
                borderColor = ResolveNonEmpty(_theme?.TabHoverBorderColor, _theme?.InactiveBorderColor, Color.FromArgb(150, 150, 150));
            }
            else
            {
                backColor = ResolveNonEmpty(_theme?.InactiveTabBackColor, _theme?.TabBackColor, Color.FromArgb(240, 240, 240));
                textColor = ResolveNonEmpty(_theme?.InactiveTabForeColor, _theme?.TabForeColor, Color.FromArgb(96, 96, 96));
                borderColor = ResolveNonEmpty(_theme?.TabBorderColor, _theme?.InactiveBorderColor, Color.FromArgb(200, 200, 200));
            }

            if (_isTransparent)
            {
                borderColor = Color.FromArgb(Math.Min(255, borderColor.A + 50), borderColor);
            }

            return new TabColors
            {
                BackgroundColor = backColor,
                TextColor = textColor,
                BorderColor = borderColor
            };
        }

        /// <summary>Returns the first colour value that is not null, not Empty, and has non-zero alpha.</summary>
        private static Color ResolveNonEmpty(params Color?[] candidates)
        {
            Color last = Color.Empty;
            foreach (var c in candidates)
            {
                if (!c.HasValue) continue;
                last = c.Value;
                if (last != Color.Empty && last.A > 0) return last;
            }
            return last;
        }

        private void DrawTabBackground(Graphics g, Rectangle bounds, TabColors colors, bool isActive, bool isHovered,
            bool isFirst = false, bool isLast = false, TabPosition tabPosition = TabPosition.Top)
        {
            if (_isTransparent) return;

            // Inset the drawing path by the shadow depth so BeepStyling.PaintControl's
            // drop-shadow fills WITHIN the declared bounds rather than bleeding into the
            // adjacent tab.  RecalculateLayout adds shadowDepth*2 to effectiveTabHeight,
            // and CalculateTabContentWidth adds shadowDepth*2 to tab width, so the
            // inset path sits centred inside bounds with exactly shadowDepth of space
            // on every side for the shadow halo.
            int shadowDepth = StyleShadows.HasShadow(_controlStyle)
                ? Math.Max(2, StyleShadows.GetShadowBlur(_controlStyle) / 2)
                : 0;

            Rectangle insetBounds = shadowDepth > 0
                ? new Rectangle(
                    bounds.X      + shadowDepth,
                    bounds.Y      + shadowDepth,
                    Math.Max(1, bounds.Width  - shadowDepth * 2),
                    Math.Max(1, bounds.Height - shadowDepth * 2))
                : bounds;

            var tabPath = BeepStyling.CreateControlStylePath(insetBounds, _controlStyle);
            try
            {
                ControlState state = isActive  ? ControlState.Selected
                                   : isHovered ? ControlState.Hovered
                                               : ControlState.Normal;
                var content = BeepStyling.PaintControl(g, tabPath, _controlStyle, _theme,
                    useThemeColors: true, state: state, IsTransparentBackground: _isTransparent);
                content?.Dispose();
            }
            catch
            {
                using var brush = new SolidBrush(colors.BackgroundColor);
                g.FillRectangle(brush, insetBounds);
            }
            finally
            {
                tabPath?.Dispose();
            }
        }

        private void DrawTabBorder(Graphics g, Rectangle bounds, Color borderColor, bool isActive)
        {
            // BeepStyling.PaintControl already handles borders based on ControlStyle
            // This method is now a no-op since borders are painted by BeepStyling
            // Keeping it for backward compatibility but it does nothing
            
            // If you need custom border logic beyond what BeepStyling provides,
            // you can add it here, but typically BeepStyling handles all border rendering
        }

        private void DrawTabText(Graphics g, Rectangle textBounds, string title, Font font, Color textColor, bool isActive)
        {
            if (g == null || string.IsNullOrEmpty(title) || font == null || textBounds.Width <= 0 || textBounds.Height <= 0)
                return;

            try
            {
                // Determine font style for active tabs
                Font textFont = font;
                if (isActive)
                {
                    try
                    {
                        textFont = FontListHelper.GetFont(font.FontFamily.Name, font.Size, FontStyle.Bold) ?? font;
                    }
                    catch
                    {
                        textFont = font;
                    }
                }
                
                // Left-align matches commercial tab bars (VS Code, DevExpress, Chrome).
                // The textBounds already has close-button and padding margins applied.
                var flags = TextFormatFlags.Left |
                           TextFormatFlags.VerticalCenter |
                           TextFormatFlags.EndEllipsis |
                           TextFormatFlags.SingleLine |
                           TextFormatFlags.NoPadding;

                TextRenderer.DrawText(g, title, textFont, textBounds, textColor, flags);
                
                // Dispose font if we created it
                if (!ReferenceEquals(textFont, font) && textFont != null)
                {
                    textFont.Dispose();
                }
            }
            catch
            {
                // Final fallback to simple text rendering
                try
                {
                    TextRenderer.DrawText(g, title, font, textBounds, textColor, 
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | 
                        TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
                }
                catch
                {
                    // If all else fails, skip drawing this text
                }
            }
        }

        private void DrawCloseButton(Graphics g, Rectangle closeRect, bool isHovered, Color baseColor)
        {
            // Resolve hover background from theme — ButtonErrorBackColor gives a theme-aware
            // destructive colour (typically a muted red/coral) rather than a hardcoded red.
            Color hoverBg = ResolveNonEmpty(
                _theme?.ButtonErrorBackColor ?? Color.Empty,
                Color.FromArgb(196, 64, 64));

            if (isHovered)
            {
                using (var bgBrush = new SolidBrush(Color.FromArgb(220, hoverBg)))
                using (var path = CreateRoundedPath(closeRect, Math.Min(4, closeRect.Width / 4)))
                    g.FillPath(bgBrush, path);
            }
            else
            {
                // Very subtle tint so the hit-target is still discoverable on hover proximity.
                using (var bgBrush = new SolidBrush(Color.FromArgb(25, baseColor)))
                using (var path = CreateRoundedPath(closeRect, Math.Min(4, closeRect.Width / 4)))
                    g.FillPath(bgBrush, path);
            }

            // X glyph – white on hover, muted baseColor otherwise.
            Color xColor = isHovered
                ? ResolveNonEmpty(_theme?.ButtonErrorForeColor ?? Color.Empty, Color.White)
                : Color.FromArgb(160, baseColor);

            using (var pen = new Pen(xColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                float cx = closeRect.X + closeRect.Width / 2f;
                float cy = closeRect.Y + closeRect.Height / 2f;
                float sz = Math.Min(closeRect.Width, closeRect.Height) / 3.5f;

                g.DrawLine(pen, cx - sz, cy - sz, cx + sz, cy + sz);
                g.DrawLine(pen, cx + sz, cy - sz, cx - sz, cy + sz);
            }
        }

        /// <summary>
        /// Creates a rounded path for a tab where outer corners (touching the container's
        /// rounded outline) use <paramref name="containerRadius"/> and inner/inner-edge
        /// corners use the normal <paramref name="tabRadius"/>.
        /// </summary>
        private GraphicsPath CreateTabCornerPath(Rectangle bounds, int tabRadius, int containerRadius,
            bool isFirst, bool isLast, TabPosition tabPosition)
        {
            // Determine which two corners are "outer" (touching the container wall)
            // vs "inner" (touching the tab-strip interior / other tabs).
            bool tl, tr, bl, br; // true = use containerRadius
            switch (tabPosition)
            {
                case TabPosition.Top:
                    tl = isFirst; tr = isLast; bl = false; br = false;
                    break;
                case TabPosition.Bottom:
                    tl = false; tr = false; bl = isFirst; br = isLast;
                    break;
                case TabPosition.Left:
                    tl = isFirst; tr = false; bl = isLast; br = false;
                    break;
                case TabPosition.Right:
                    tl = false; tr = isFirst; bl = false; br = isLast;
                    break;
                default:
                    tl = isFirst; tr = isLast; bl = false; br = false;
                    break;
            }

            // Build path: the four arcs use either containerRadius or tabRadius.
            var path = new GraphicsPath();
            if (bounds.Width <= 0 || bounds.Height <= 0) return path;

            int rTL = Math.Min(tl ? containerRadius : tabRadius, Math.Min(bounds.Width / 2, bounds.Height / 2));
            int rTR = Math.Min(tr ? containerRadius : tabRadius, Math.Min(bounds.Width / 2, bounds.Height / 2));
            int rBL = Math.Min(bl ? containerRadius : tabRadius, Math.Min(bounds.Width / 2, bounds.Height / 2));
            int rBR = Math.Min(br ? containerRadius : tabRadius, Math.Min(bounds.Width / 2, bounds.Height / 2));

            try
            {
                if (rTL > 0) path.AddArc(bounds.X, bounds.Y, rTL * 2, rTL * 2, 180, 90);
                path.AddLine(bounds.X + (rTL > 0 ? rTL : 0), bounds.Y, bounds.Right - (rTR > 0 ? rTR : 0), bounds.Y);
                if (rTR > 0) path.AddArc(bounds.Right - rTR * 2, bounds.Y, rTR * 2, rTR * 2, 270, 90);
                path.AddLine(bounds.Right, bounds.Y + (rTR > 0 ? rTR : 0), bounds.Right, bounds.Bottom - (rBR > 0 ? rBR : 0));
                if (rBR > 0) path.AddArc(bounds.Right - rBR * 2, bounds.Bottom - rBR * 2, rBR * 2, rBR * 2, 0, 90);
                path.AddLine(bounds.Right - (rBR > 0 ? rBR : 0), bounds.Bottom, bounds.X + (rBL > 0 ? rBL : 0), bounds.Bottom);
                if (rBL > 0) path.AddArc(bounds.X, bounds.Bottom - rBL * 2, rBL * 2, rBL * 2, 90, 90);
                path.AddLine(bounds.X, bounds.Bottom - (rBL > 0 ? rBL : 0), bounds.X, bounds.Y + (rTL > 0 ? rTL : 0));
                path.CloseFigure();
            }
            catch
            {
                path.Reset();
                path.AddRectangle(bounds);
            }
            return path;
        }

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return path;
            }
            
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // Ensure radius doesn't exceed rectangle dimensions
            int maxRadius = Math.Min(rect.Width / 2, rect.Height / 2);
            radius = Math.Min(radius, maxRadius);
            
            int diameter = radius * 2;
            
            if (diameter <= 0 || diameter > Math.Min(rect.Width, rect.Height))
            {
                path.AddRectangle(rect);
                return path;
            }

            try
            {
                var arc = new Rectangle(rect.X, rect.Y, diameter, diameter);
                
                path.AddArc(arc, 180, 90);
                arc.X = rect.Right - diameter;
                path.AddArc(arc, 270, 90);
                arc.Y = rect.Bottom - diameter;
                path.AddArc(arc, 0, 90);
                arc.X = rect.Left;
                path.AddArc(arc, 90, 90);
                
                path.CloseFigure();
            }
            catch
            {
                path.Reset();
                if (rect.Width > 0 && rect.Height > 0)
                {
                    path.AddRectangle(rect);
                }
            }

            return path;
        }

        private Color InterpolateColor(Color color1, Color color2, float progress)
        {
            progress = Math.Max(0, Math.Min(1, progress));
            
            return Color.FromArgb(
                (int)(color1.A + (color2.A - color1.A) * progress),
                (int)(color1.R + (color2.R - color1.R) * progress),
                (int)(color1.G + (color2.G - color1.G) * progress),
                (int)(color1.B + (color2.B - color1.B) * progress)
            );
        }

        private struct TabColors
        {
            public Color BackgroundColor;
            public Color TextColor;
            public Color BorderColor;
        }
    }
}