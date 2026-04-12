// BeepDocumentTabStrip.Painting.cs
// All rendering logic for BeepDocumentTabStrip — four visual styles (Chrome, VSCode,
// Underline, Pill), the sliding indicator, scroll buttons, add button and close glyph.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentTabStrip
    {
        // ─────────────────────────────────────────────────────────────────────
        // OnPaint
        // ─────────────────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode      = SmoothingMode.AntiAlias;
            g.TextRenderingHint  = TextRenderingHint.ClearTypeGridFit;

            // Strip background
            using (var bg = new SolidBrush(_currentTheme?.PanelBackColor ?? BackColor))
                g.FillRectangle(bg, ClientRectangle);

            // Empty-state hint
            if (_tabs.Count == 0)
            {
                using var hintBr  = new SolidBrush(Color.FromArgb(100, _currentTheme?.SecondaryTextColor ?? ForeColor));
                using var hintFmt = new StringFormat
                {
                    Alignment     = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags   = StringFormatFlags.NoWrap
                };
                g.DrawString("No open documents", Font, hintBr, (RectangleF)ClientRectangle, hintFmt);
                if (_showAddButton && !_addButtonRect.IsEmpty)
                    DrawAddButton(g);
                return;
            }

            // Separator line — right edge in vertical mode, bottom edge otherwise
            using (var sep = new Pen(_currentTheme?.BorderColor ?? SystemColors.ControlDark, 1))
            {
                if (IsVertical)
                    g.DrawLine(sep, Width - 1, 0, Width - 1, Height);
                else
                    g.DrawLine(sep, 0, Height - 1, Width, Height - 1);
            }

            // Scroll buttons — arrows adapt to vertical vs horizontal mode
            if (_showScrollButtons)
            {
                DrawScrollButton(g, _scrollLeftRect,  isLeft: true);
                DrawScrollButton(g, _scrollRightRect, isLeft: false);
                if (!IsVertical)
                    DrawOverflowButton(g);
            }

            // Group headers (7.3) — painted before tabs so they act as separators
            if (_tabGroups.Count > 0 && !IsVertical)
                DrawGroupHeaders(g);

            // Clip to scrollable tab area to prevent drawing over scroll/add buttons
            if (!_tabArea.IsEmpty)
            {
                var clip = g.Clip;
                g.SetClip(_tabArea, CombineMode.Intersect);
                for (int i = 0; i < _tabs.Count; i++)
                {
                    var t = _tabs[i];
                    // Skip tabs whose group is collapsed
                    if (!string.IsNullOrEmpty(t.Group))
                    {
                        var grp = _tabGroups.Find(tg => tg.Id == t.Group);
                        if (grp != null && grp.IsCollapsed) continue;
                    }
                    // Sprint 18.2: in ActiveOnly mode only the active tab is painted
                    if (_responsiveMode == TabResponsiveMode.ActiveOnly && !t.IsActive) continue;
                    // 5.1: Skip unpinned tabs scrolled entirely outside the visible tab area
                    if (!t.IsPinned && !t.TabRect.IntersectsWith(_tabArea)) continue;
                    DrawTab(g, t, i);
                }

                // Sliding indicator (drawn above tab shapes, below text)
                DrawIndicator(g);

                g.Clip = clip;
            }
            else
            {
                for (int i = 0; i < _tabs.Count; i++)
                {
                    var t = _tabs[i];
                    if (!string.IsNullOrEmpty(t.Group))
                    {
                        var grp = _tabGroups.Find(tg => tg.Id == t.Group);
                        if (grp != null && grp.IsCollapsed) continue;
                    }
                    // Sprint 18.2: in ActiveOnly mode only the active tab is painted
                    if (_responsiveMode == TabResponsiveMode.ActiveOnly && !t.IsActive) continue;
                    DrawTab(g, t, i);
                }
                DrawIndicator(g);
            }

            // Keyboard focus rectangle over the active tab
            DrawFocusIndicator(g);

            // Add button
            if (_showAddButton && !_addButtonRect.IsEmpty)
                DrawAddButton(g);

            // Chrome: bridge the active tab's bottom edge into the content area
            // (overdraw the separator line so the active tab appears connected)
            if (!IsVertical
                && _tabStyle == DocumentTabStyle.Chrome
                && _activeTabIndex >= 0 && _activeTabIndex < _tabs.Count)
            {
                var at = _tabs[_activeTabIndex];
                if (!at.TabRect.IsEmpty)
                {
                    Color contentBg = _currentTheme?.BackgroundColor ?? BackColor;
                    using var bridgeBr = new SolidBrush(contentBg);
                    int bLeft  = Math.Max(at.TabRect.Left  + 1, _tabArea.Left);
                    int bRight = Math.Min(at.TabRect.Right - 1, _tabArea.Right);
                    if (bRight > bLeft)
                        g.FillRectangle(bridgeBr, bLeft, Height - 1, bRight - bLeft, 1);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // DrawTab — style dispatcher
        // ─────────────────────────────────────────────────────────────────────

        private void DrawTab(Graphics g, BeepDocumentTab tab, int index)
        {
            if (tab.TabRect.IsEmpty) return;

            if (tab.IsPinned)
                DrawTabPinned(g, tab, index);
            else
            {
                switch (_tabStyle)
                {
                    case DocumentTabStyle.VSCode:      DrawTabVSCode(g, tab, index);      break;
                    case DocumentTabStyle.Underline:   DrawTabUnderline(g, tab, index);   break;
                    case DocumentTabStyle.Pill:        DrawTabPill(g, tab, index);        break;
                    case DocumentTabStyle.Flat:        DrawTabFlat(g, tab, index);        break;
                    case DocumentTabStyle.Rounded:     DrawTabRounded(g, tab, index);     break;
                    case DocumentTabStyle.Trapezoid:   DrawTabTrapezoid(g, tab, index);   break;
                    case DocumentTabStyle.Office:      DrawTabOffice(g, tab, index);      break;
                    case DocumentTabStyle.Fluent:      DrawTabFluent(g, tab, index);      break;
                    default:                           DrawTabChrome(g, tab, index);      break;
                }
            }

            // Tab colour overlay (2.2) — rendered last so it sits on top of the tab background
            if (_tabColorMode != TabColorMode.None && tab.TabColor != Color.Empty)
                DrawTabColorOverlay(g, tab);
        }

        // ── Tab colour overlay (2.2) ───────────────────────────────────────

        private void DrawTabColorOverlay(Graphics g, BeepDocumentTab tab)
        {
            Color c = tab.TabColor;
            if (c == Color.Empty) return;

            int barH = S(3);

            switch (_tabColorMode)
            {
                case TabColorMode.AccentBar:
                    using (var br = new SolidBrush(c))
                        g.FillRectangle(br, new Rectangle(
                            tab.TabRect.Left + 2, tab.TabRect.Top,
                            tab.TabRect.Width - 4, barH));
                    break;

                case TabColorMode.FullBackground:
                    using (var br = new SolidBrush(Color.FromArgb(48, c)))
                        g.FillRectangle(br, tab.TabRect);
                    break;

                case TabColorMode.BottomBorder:
                    using (var br = new SolidBrush(c))
                        g.FillRectangle(br, new Rectangle(
                            tab.TabRect.Left + 2, tab.TabRect.Bottom - barH,
                            tab.TabRect.Width - 4, barH));
                    break;
            }
        }

        // ── Pinned (icon-only) style ────────────────────────────────────────

        private void DrawTabPinned(Graphics g, BeepDocumentTab tab, int index)
        {
            bool active  = tab.IsActive;
            bool hovered = index == _hoverTabIndex;

            Color fill = active  ? _currentTheme.BackgroundColor
                       : hovered ? Blend(_currentTheme.PanelBackColor, _currentTheme.BorderColor, 0.3f)
                       :           _currentTheme.PanelBackColor;

            // Background (use chrome-style rounded path when applicable)
            if (_tabStyle == DocumentTabStyle.Chrome)
            {
                using var path = BuildChromeTabPath(tab.TabRect, TabRadius);
                using var br   = new SolidBrush(fill);
                g.FillPath(br, path);
                using var pen  = new Pen(_currentTheme.BorderColor, 1f);
                g.DrawPath(pen, path);
            }
            else
            {
                using var br  = new SolidBrush(fill);
                g.FillRectangle(br, tab.TabRect);
                using var pen = new Pen(_currentTheme.BorderColor, 1f);
                g.DrawLine(pen, tab.TabRect.Right - 1, tab.TabRect.Top,
                                tab.TabRect.Right - 1, tab.TabRect.Bottom);
            }

            // Top accent on active
            if (active) DrawAccentBar(g, tab);

            // Icon (centred)
            if (!tab.IconRect.IsEmpty)
            {
                // TODO: render via StyledImagePainter when an icon path is set
            }
            else
            {
                // Draw a pin glyph (small filled circle) when no icon
                int r = S(5);
                int cx = tab.TabRect.Left + tab.TabRect.Width  / 2;
                int cy = tab.TabRect.Top  + tab.TabRect.Height / 2 - S(1);
                Color pinColor = active
                    ? _currentTheme.PrimaryColor
                    : Blend(_currentTheme.SecondaryTextColor, _currentTheme.PanelBackColor, 0.4f);
                using var br = new SolidBrush(pinColor);
                g.FillEllipse(br, cx - r, cy - r, r * 2, r * 2);

                // Small pin stem
                using var pen = new Pen(pinColor, S(2));
                g.DrawLine(pen, cx, cy + r, cx, cy + r + S(4));
            }

            // Dirty dot at bottom-right corner of pin tab
            if (tab.IsModified)
            {
                int ds = S(5);
                using var dotBr = new SolidBrush(_currentTheme.WarningColor);
                g.FillEllipse(dotBr,
                    tab.TabRect.Right - ds - S(3),
                    tab.TabRect.Bottom - ds - S(3),
                    ds, ds);
            }
        }

        // ── Chrome style ─────────────────────────────────────────────────────

        private void DrawTabChrome(Graphics g, BeepDocumentTab tab, int index)
        {
            bool active  = tab.IsActive;
            bool hovered = index == _hoverTabIndex;

            Color fill = active  ? _currentTheme.BackgroundColor
                       : hovered ? Blend(_currentTheme.PanelBackColor, Color.White, 0.08f)   // +8 % white (Figma 2026)
                       :           _currentTheme.PanelBackColor;

            using var path = BuildChromeTabPath(tab.TabRect, TabRadius);
            using (var br = new SolidBrush(fill))
                g.FillPath(br, path);
            using (var pen = new Pen(_currentTheme.BorderColor, 1f))
                g.DrawPath(pen, path);

            // Top accent bar on active tab
            if (active)
                DrawAccentBar(g, tab);

            DrawTabContent(g, tab, index);
        }

        // ── VSCode style ──────────────────────────────────────────────────────

        private void DrawTabVSCode(Graphics g, BeepDocumentTab tab, int index)
        {
            bool active  = tab.IsActive;
            bool hovered = index == _hoverTabIndex;

            Color fill   = active  ? _currentTheme.BackgroundColor
                         : hovered ? Blend(_currentTheme.PanelBackColor, _currentTheme.BorderColor, 0.2f)
                         :           _currentTheme.PanelBackColor;

            using (var br = new SolidBrush(fill))
                g.FillRectangle(br, tab.TabRect);

            // Top accent stripe for active
            if (active)
                DrawAccentBar(g, tab);

            // Vertical separator lines
            using (var pen = new Pen(_currentTheme.BorderColor, 1f))
            {
                g.DrawLine(pen, tab.TabRect.Right - 1, tab.TabRect.Top + 4,
                                tab.TabRect.Right - 1, tab.TabRect.Bottom - 4);
            }

            DrawTabContent(g, tab, index);
        }

        // ── Underline style ───────────────────────────────────────────────────

        private void DrawTabUnderline(Graphics g, BeepDocumentTab tab, int index)
        {
            bool active  = tab.IsActive;
            bool hovered = index == _hoverTabIndex;

            if (hovered && !active)
            {
                using var br = new SolidBrush(Blend(_currentTheme.PanelBackColor, _currentTheme.BorderColor, 0.15f));
                g.FillRectangle(br, tab.TabRect);
            }

            DrawTabContent(g, tab, index);
            // The animated indicator handles the underline for the active tab
        }

        // ── Flat style ───────────────────────────────────────────────────────

        private void DrawTabFlat(Graphics g, BeepDocumentTab tab, int index)
        {
            bool active  = tab.IsActive;
            bool hovered = index == _hoverTabIndex;

            Color fill = active  ? _currentTheme.BackgroundColor
                       : hovered ? Blend(_currentTheme.PanelBackColor, _currentTheme.BorderColor, 0.2f)
                       :           _currentTheme.PanelBackColor;

            using (var br = new SolidBrush(fill))
                g.FillRectangle(br, tab.TabRect);

            // Top accent bar only on active tab (4 px, full width)
            if (active)
            {
                Color accent = tab.AccentColor == Color.Empty ? _currentTheme.PrimaryColor : tab.AccentColor;
                using var pen = new Pen(accent, 3f);
                g.DrawLine(pen, tab.TabRect.Left, tab.TabRect.Top + 1,
                                tab.TabRect.Right, tab.TabRect.Top + 1);
            }

            DrawTabContent(g, tab, index);
        }

        // ── Rounded style ─────────────────────────────────────────────────────

        private void DrawTabRounded(Graphics g, BeepDocumentTab tab, int index)
        {
            bool active  = tab.IsActive;
            bool hovered = index == _hoverTabIndex;

            var r = Rectangle.Inflate(tab.TabRect, -2, -2);

            Color fill = active  ? _currentTheme.BackgroundColor
                       : hovered ? Blend(_currentTheme.PanelBackColor, _currentTheme.BorderColor, 0.25f)
                       :           _currentTheme.PanelBackColor;

            int radius = Math.Min(r.Height / 2, S(10));
            using var path = BuildRoundedRect(r, radius);
            using (var br  = new SolidBrush(fill))
                g.FillPath(br, path);

            if (active)
            {
                Color accent = tab.AccentColor == Color.Empty ? _currentTheme.PrimaryColor : tab.AccentColor;
                using var pen = new Pen(accent, 2f);
                g.DrawPath(pen, path);
            }

            DrawTabContent(g, tab, index);
        }

        // ── Trapezoid style ───────────────────────────────────────────────────

        private void DrawTabTrapezoid(Graphics g, BeepDocumentTab tab, int index)
        {
            bool active  = tab.IsActive;
            bool hovered = index == _hoverTabIndex;

            int slant = S(8);   // horizontal offset of the angled side
            var r     = tab.TabRect;

            // Build trapezoid: bottom wider than top
            System.Drawing.Drawing2D.GraphicsPath? path = null;
            try
            {
                path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddPolygon(new PointF[]
                {
                    new(r.Left,          r.Bottom),
                    new(r.Left  + slant, r.Top),
                    new(r.Right - slant, r.Top),
                    new(r.Right,         r.Bottom)
                });

                Color fill = active  ? _currentTheme.BackgroundColor
                           : hovered ? Blend(_currentTheme.PanelBackColor, _currentTheme.BorderColor, 0.2f)
                           :           _currentTheme.PanelBackColor;

                using (var br = new SolidBrush(fill))
                    g.FillPath(br, path);

                using (var pen = new Pen(_currentTheme.BorderColor, 1f))
                    g.DrawPath(pen, path);

                if (active) DrawAccentBar(g, tab);
            }
            finally { path?.Dispose(); }

            DrawTabContent(g, tab, index);
        }

        // ── Office style ──────────────────────────────────────────────────────

        private void DrawTabOffice(Graphics g, BeepDocumentTab tab, int index)
        {
            bool active  = tab.IsActive;
            bool hovered = index == _hoverTabIndex;

            Color fill = active  ? _currentTheme.BackgroundColor
                       : hovered ? Blend(_currentTheme.PanelBackColor, _currentTheme.BorderColor, 0.18f)
                       :           _currentTheme.PanelBackColor;

            using (var br = new SolidBrush(fill))
                g.FillRectangle(br, tab.TabRect);

            // Office: thick 3 px bottom accent on the active tab (below content)
            if (active)
            {
                Color accent = tab.AccentColor == Color.Empty ? _currentTheme.PrimaryColor : tab.AccentColor;
                using var pen = new Pen(accent, 3f);
                g.DrawLine(pen, tab.TabRect.Left, tab.TabRect.Bottom - 2,
                                tab.TabRect.Right, tab.TabRect.Bottom - 2);
            }

            // Subtle right-side separator for inactive tabs
            if (!active)
            {
                using var sep = new Pen(Color.FromArgb(40, _currentTheme.BorderColor), 1f);
                g.DrawLine(sep, tab.TabRect.Right - 1, tab.TabRect.Top + 4,
                                tab.TabRect.Right - 1, tab.TabRect.Bottom - 4);
            }

            DrawTabContent(g, tab, index);
        }

        // ── Fluent UI 2 / Windows 11 style ───────────────────────────────────
        // Translucent acrylic-style fill for inactive tabs; 4 px bottom accent on active.

        private void DrawTabFluent(Graphics g, BeepDocumentTab tab, int index)
        {
            bool active  = tab.IsActive;
            bool hovered = index == _hoverTabIndex;

            // Active tab: full BackgroundColor fill (opaque, like Fluent active pane)
            // Inactive tab: semi-transparent overlay — simulates acrylic / mica backdrop
            Color fill = active  ? _currentTheme.BackgroundColor
                       : hovered ? Color.FromArgb(32, _currentTheme.ForeColor)
                       :           Color.FromArgb(16, _currentTheme.ForeColor);

            using (var br = new SolidBrush(fill))
                g.FillRectangle(br, tab.TabRect);

            // 4 px bottom accent bar on the active tab (Fluent: bottom-aligned indicator)
            if (active)
            {
                Color accent = tab.AccentColor == Color.Empty ? _currentTheme.PrimaryColor : tab.AccentColor;
                using var pen = new Pen(accent, 4f);
                g.DrawLine(pen,
                    tab.TabRect.Left  + 1, tab.TabRect.Bottom - 2,
                    tab.TabRect.Right - 1, tab.TabRect.Bottom - 2);
            }

            // Thin top line on every tab for structural clarity
            if (active || hovered)
            {
                using var topPen = new Pen(Color.FromArgb(60, _currentTheme.BorderColor), 1f);
                g.DrawLine(topPen, tab.TabRect.Left, tab.TabRect.Top,
                                   tab.TabRect.Right, tab.TabRect.Top);
            }

            DrawTabContent(g, tab, index);
        }

        // ── Pill style ────────────────────────────────────────────────────────

        private void DrawTabPill(Graphics g, BeepDocumentTab tab, int index)
        {
            bool active  = tab.IsActive;
            bool hovered = index == _hoverTabIndex;

            if (active || hovered)
            {
                var pillRect = Rectangle.Inflate(
                    new Rectangle(tab.TabRect.Left,
                                  tab.TabRect.Top + PillPadV,
                                  tab.TabRect.Width,
                                  tab.TabRect.Height - PillPadV * 2),
                    -4, 0);

                Color pillColor = active
                    ? Blend(_currentTheme.PrimaryColor, _currentTheme.BackgroundColor, 0.85f)
                    : Blend(_currentTheme.PanelBackColor, _currentTheme.BorderColor, 0.25f);

                using var path = BuildRoundedRect(pillRect, PillRadius);
                using var br   = new SolidBrush(pillColor);
                g.FillPath(br, path);
            }

            DrawTabContent(g, tab, index);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Shared tab content (icon + title + dirty dot + close)
        // ─────────────────────────────────────────────────────────────────────

        private void DrawTabContent(Graphics g, BeepDocumentTab tab, int index,
                                        Color? textColorOverride = null)
        {
            // Sprint 18.2: Responsive mode gates icon and title rendering
            bool drawIcon  = _responsiveMode != TabResponsiveMode.Compact;           // Compact = title-only
            bool drawTitle = _responsiveMode == TabResponsiveMode.Normal              // Normal always shows title
                          || _responsiveMode == TabResponsiveMode.Compact             // Compact shows title
                          || (_responsiveMode == TabResponsiveMode.IconOnly  && (tab.IsActive || string.IsNullOrEmpty(tab.IconPath)))
                          || (_responsiveMode == TabResponsiveMode.ActiveOnly && tab.IsActive);

            // Icon (from ImageList, IconPath, or nothing)
            if (drawIcon && !tab.IconRect.IsEmpty)
                DrawTabIcon(g, tab);

            // Title
            if (drawTitle && !tab.TitleRect.IsEmpty)
            {
                // Base text colour; inactive tabs get 70 % opacity per DocTokens (visual hierarchy)
                Color baseColor = textColorOverride
                    ?? (tab.IsActive ? _currentTheme.ForeColor : _currentTheme.SecondaryTextColor);
                Color textColor = tab.IsActive
                    ? baseColor
                    : Color.FromArgb(DocTokens.InactiveTabTextAlpha, baseColor);

                using var fmt  = new StringFormat
                {
                    Alignment     = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming      = StringTrimming.EllipsisCharacter,
                    FormatFlags   = StringFormatFlags.NoWrap
                };

                // Bold font for active tab
                var font = tab.IsActive
                    ? new Font(Font, System.Drawing.FontStyle.Bold)
                    : Font;

                using var textBr = new SolidBrush(textColor);
                g.DrawString(tab.Title, font, textBr, tab.TitleRect, fmt);

                if (tab.IsActive) font.Dispose();
            }

            // Dirty / modified dot
            if (!tab.DirtyRect.IsEmpty)
            {
                using var dotBr = new SolidBrush(_currentTheme.WarningColor);
                g.FillEllipse(dotBr, tab.DirtyRect);
            }

            // Close button — visibility gated by CloseMode and hover/active state
            bool showClose = !tab.CloseRect.IsEmpty && tab.CanClose
                && _closeMode switch
                {
                    TabCloseMode.Always     => true,
                    TabCloseMode.OnHover    => index == _hoverTabIndex || tab.IsActive,
                    TabCloseMode.ActiveOnly => tab.IsActive,
                    _                      => false   // Never
                };

            if (showClose)
                DrawCloseButton(g, tab, index);

            // Notification badge (7.4)
            if (!string.IsNullOrEmpty(tab.BadgeText))
                DrawBadge(g, tab);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Tab icon rendering (4.4)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Renders the icon for a tab.  Priority order:
        ///   1. <c>BeepDocumentTab.ImageList</c> + <c>ImageIndex</c>
        ///   2. <c>BeepDocumentTab.IconPath</c> (file loaded and cached)
        ///   3. Nothing (icon area left empty).
        /// Inactive tabs are painted with a greyscale colour matrix so they
        /// appear visually de-emphasised, matching VS Code behaviour.
        /// </summary>
        private void DrawTabIcon(Graphics g, BeepDocumentTab tab)
        {
            if (tab.IconRect.IsEmpty) return;

            System.Drawing.Image? img = null;
            bool ownImage = false;

            try
            {
                // ── Source 1: ImageList ───────────────────────────────────────
                if (tab.ImageList != null
                    && tab.ImageIndex >= 0
                    && tab.ImageIndex < tab.ImageList.Images.Count)
                {
                    img = tab.ImageList.Images[tab.ImageIndex];
                }

                // ── Source 2: IconPath ────────────────────────────────────────
                if (img == null && !string.IsNullOrEmpty(tab.IconPath))
                {
                    img      = TryLoadTabIcon(tab.IconPath);
                    ownImage = img != null;
                }

                if (img == null) return;

                // ── Render (greyscale for inactive tabs) ──────────────────────
                if (tab.IsActive)
                {
                    g.DrawImage(img, tab.IconRect);
                }
                else
                {
                    using var ia = new System.Drawing.Imaging.ImageAttributes();
                    ia.SetColorMatrix(_greyscaleMatrix,
                        System.Drawing.Imaging.ColorMatrixFlag.Default,
                        System.Drawing.Imaging.ColorAdjustType.Bitmap);

                    g.DrawImage(img,
                        new Rectangle(tab.IconRect.X, tab.IconRect.Y,
                                      tab.IconRect.Width, tab.IconRect.Height),
                        0, 0, img.Width, img.Height,
                        GraphicsUnit.Pixel, ia);
                }
            }
            finally
            {
                if (ownImage) img?.Dispose();
            }
        }

        // Shared greyscale colour matrix (constructed once)
        private static readonly System.Drawing.Imaging.ColorMatrix _greyscaleMatrix =
            new(new float[][]
            {
                new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                new float[] { 0,      0,      0,      0.55f, 0 },   // 55 % opacity
                new float[] { 0,      0,      0,      0,     1 }
            });

        // Simple file-based icon cache (clears when the strip is disposed)
        private readonly System.Collections.Generic.Dictionary<string, System.Drawing.Image>
            _iconCache = new(StringComparer.OrdinalIgnoreCase);

        private System.Drawing.Image? TryLoadTabIcon(string path)
        {
            if (_iconCache.TryGetValue(path, out var cached)) return cached;
            try
            {
                if (!System.IO.File.Exists(path)) return null;
                var img = System.Drawing.Image.FromFile(path);
                _iconCache[path] = img;
                return img;
            }
            catch { return null; }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Accent bar (used by Chrome and VSCode styles)
        // ─────────────────────────────────────────────────────────────────────

        private void DrawAccentBar(Graphics g, BeepDocumentTab tab)
        {
            Color accent = tab.AccentColor == Color.Empty ? _currentTheme.PrimaryColor : tab.AccentColor;
            // Token-driven accent line thickness at the very top of the tab
            int inset = TabRadius;
            using var pen = new Pen(accent, DocTokens.IndicatorThickness)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                EndCap   = System.Drawing.Drawing2D.LineCap.Round
            };
            g.DrawLine(pen,
                tab.TabRect.Left  + inset, tab.TabRect.Top + DocTokens.IndicatorThickness / 2,
                tab.TabRect.Right - inset, tab.TabRect.Top + DocTokens.IndicatorThickness / 2);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Sliding active indicator
        // ─────────────────────────────────────────────────────────────────────

        private void DrawIndicator(Graphics g)
        {
            if (_indicatorCurrent.IsEmpty || _activeTabIndex < 0) return;
            // Styles that draw their own active indicator skip the sliding underline
            if (_tabStyle == DocumentTabStyle.Chrome  ||
                _tabStyle == DocumentTabStyle.VSCode  ||
                _tabStyle == DocumentTabStyle.Fluent) return;

            Color color = _currentTheme.PrimaryColor;
            using var path = BuildRoundedRect(_indicatorCurrent, IndicatorHeight / 2);
            using var br   = new SolidBrush(color);
            g.FillPath(br, path);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Close glyph
        // ─────────────────────────────────────────────────────────────────────

        private void DrawCloseButton(Graphics g, BeepDocumentTab tab, int index)
        {
            bool hovered = index == _hoverTabIndex && _hoverClose;
            if (hovered)
            {
                using var hBr = new SolidBrush(_currentTheme.ErrorColor);
                g.FillEllipse(hBr, tab.CloseRect);
            }

            int m = Math.Max(3, tab.CloseRect.Width / 4);
            var cr = tab.CloseRect;
            using var xPen = new Pen(
                hovered ? Color.White : _currentTheme.SecondaryTextColor, 1.5f);
            g.DrawLine(xPen, cr.Left + m, cr.Top + m, cr.Right - m, cr.Bottom - m);
            g.DrawLine(xPen, cr.Right - m, cr.Top + m, cr.Left + m, cr.Bottom - m);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Notification badge pill (7.4)
        // ─────────────────────────────────────────────────────────────────────

        private void DrawBadge(Graphics g, BeepDocumentTab tab)
        {
            if (string.IsNullOrEmpty(tab.BadgeText)) return;

            using Font badgeFont = new Font(Font.FontFamily, Math.Max(6f, Font.Size - 2f),
                                            System.Drawing.FontStyle.Bold, Font.Unit);
            SizeF textSize  = g.MeasureString(tab.BadgeText, badgeFont);
            int   padH      = S(4);
            int   padV      = S(2);
            int   pillW     = Math.Max((int)textSize.Width + padH * 2, S(14));
            int   pillH     = (int)textSize.Height + padV * 2;

            // Apply pulse scale animation (clamped to avoid degenerate rect)
            float phase = _badgePulseScale.TryGetValue(tab.Id, out float p) ? p : -1f;
            float scale = phase >= 0f
                ? (1f + 0.25f * (float)Math.Sin(phase * Math.PI))
                : 1f;
            int   scaledW = (int)(pillW * scale);
            int   scaledH = (int)(pillH * scale);

            // Position: top-right corner of the tab rect
            int bx = tab.TabRect.Right  - scaledW - S(2);
            int by = tab.TabRect.Top    + S(1);
            var pillRect = new Rectangle(bx, by, scaledW, scaledH);

            // Fill
            Color fillCol = tab.BadgeColor == Color.Empty ? _currentTheme.ErrorColor : tab.BadgeColor;
            int   radius  = pillH / 2;

            using (var fillBr = new SolidBrush(fillCol))
            using (var path   = BadgePillPath(pillRect, radius))
                g.FillPath(fillBr, path);

            // Text
            using var textBr = new SolidBrush(Color.White);
            var textRect     = new RectangleF(pillRect.X, pillRect.Y, pillRect.Width, pillRect.Height);
            using var fmt    = new StringFormat
            {
                Alignment     = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(tab.BadgeText, badgeFont, textBr, textRect, fmt);
        }

        private static System.Drawing.Drawing2D.GraphicsPath BadgePillPath(Rectangle r, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int d    = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            if (d < 2) { path.AddRectangle(r); return path; }
            path.AddArc(r.X,           r.Y,            d, d, 180, 90);
            path.AddArc(r.Right - d,   r.Y,            d, d, 270, 90);
            path.AddArc(r.Right - d,   r.Bottom - d,   d, d,   0, 90);
            path.AddArc(r.X,           r.Bottom - d,   d, d,  90, 90);
            path.CloseFigure();
            return path;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Add button  (+)
        // ─────────────────────────────────────────────────────────────────────

        private void DrawAddButton(Graphics g)
        {
            Color fill = _hoverAdd
                ? Blend(_currentTheme.PanelBackColor, _currentTheme.PrimaryColor, 0.25f)
                : _currentTheme.PanelBackColor;

            using (var br = new SolidBrush(fill))
                g.FillRectangle(br, _addButtonRect);

            int cx  = _addButtonRect.Left + _addButtonRect.Width / 2;
            int cy  = _addButtonRect.Top  + _addButtonRect.Height / 2;
            int arm = S(6);

            using var pen = new Pen(_currentTheme.SecondaryTextColor, 1.5f);
            g.DrawLine(pen, cx - arm, cy,       cx + arm, cy);
            g.DrawLine(pen, cx,       cy - arm, cx,       cy + arm);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Overflow button  (▾)
        // ─────────────────────────────────────────────────────────────────────

        private void DrawOverflowButton(Graphics g)
        {
            if (_overflowButtonRect.IsEmpty) return;

            Color fill = _hoverOverflow
                ? Blend(_currentTheme.PanelBackColor, _currentTheme.PrimaryColor, 0.25f)
                : _currentTheme.PanelBackColor;

            using (var br = new SolidBrush(fill))
                g.FillRectangle(br, _overflowButtonRect);

            // Chevron ▾ glyph
            int cx  = _overflowButtonRect.Left + _overflowButtonRect.Width  / 2;
            int cy  = _overflowButtonRect.Top  + _overflowButtonRect.Height / 2;
            int arm = S(5);
            int half = arm;

            var pts = new Point[]
            {
                new Point(cx - half, cy - arm / 2),
                new Point(cx,        cy + arm / 2),
                new Point(cx + half, cy - arm / 2)
            };
            using var pen = new Pen(_currentTheme.SecondaryTextColor, 1.5f);
            pen.EndCap = pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            g.DrawLines(pen, pts);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Scroll buttons  (◄ ► or ▲ ▼ in vertical mode)
        // ─────────────────────────────────────────────────────────────────────

        private void DrawScrollButton(Graphics g, Rectangle rect, bool isLeft)
        {
            bool hovered = isLeft ? _hoverScrollLeft : _hoverScrollRight;
            Color fill = hovered
                ? Blend(_currentTheme.PanelBackColor, _currentTheme.BorderColor, 0.4f)
                : _currentTheme.PanelBackColor;

            using (var br = new SolidBrush(fill))
                g.FillRectangle(br, rect);

            // Arrow glyph
            int cx  = rect.Left + rect.Width  / 2;
            int cy  = rect.Top  + rect.Height / 2;
            int arm = S(5);

            using var pen = new Pen(_currentTheme.SecondaryTextColor, 1.5f);
            if (IsVertical)
            {
                // isLeft = "up" button, !isLeft = "down" button
                if (isLeft)
                {
                    g.DrawLine(pen, cx - arm, cy + arm, cx,       cy - arm);
                    g.DrawLine(pen, cx,       cy - arm, cx + arm, cy + arm);
                }
                else
                {
                    g.DrawLine(pen, cx - arm, cy - arm, cx,       cy + arm);
                    g.DrawLine(pen, cx,       cy + arm, cx + arm, cy - arm);
                }
            }
            else
            {
                if (isLeft)
                {
                    g.DrawLine(pen, cx + arm, cy - arm, cx - arm, cy);
                    g.DrawLine(pen, cx - arm, cy,       cx + arm, cy + arm);
                }
                else
                {
                    g.DrawLine(pen, cx - arm, cy - arm, cx + arm, cy);
                    g.DrawLine(pen, cx + arm, cy,       cx - arm, cy + arm);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Group headers (7.3) — separator bars drawn between tab groups
        // ─────────────────────────────────────────────────────────────────────

        private void DrawGroupHeaders(Graphics g)
        {
            const int LogicalHeaderH = 16;
            int hdrH = S(LogicalHeaderH);
            int tabH = TabHeight;

            // Work out the set of group IDs that have at least one visible tab
            var visibleGroupIds = new System.Collections.Generic.HashSet<string>();
            foreach (var t in _tabs)
                if (!string.IsNullOrEmpty(t.Group))
                    visibleGroupIds.Add(t.Group);

            foreach (var grp in _tabGroups)
            {
                if (!visibleGroupIds.Contains(grp.Id)) continue;

                // Find the first tab in this group (by current position)
                BeepDocumentTab? firstTab = null;
                foreach (var t in _tabs)
                    if (t.Group == grp.Id)
                    {
                        if (firstTab == null || t.TabRect.Left < firstTab.TabRect.Left)
                            firstTab = t;
                    }
                if (firstTab == null) continue;

                // The header sits in the tab row, immediately to the left of the first tab
                // In single-row mode it's a vertical bar + label.  In multi-row it would
                // span the row above — for now, single-row only.
                int x = firstTab.TabRect.Left;
                int y = firstTab.TabRect.Top;

                // Vertical colour bar (4 px wide)
                Color barColor = grp.GroupColor.IsEmpty ? (_currentTheme?.BorderColor ?? Color.Gray) : grp.GroupColor;
                using (var barBr = new SolidBrush(barColor))
                    g.FillRectangle(barBr, x, y, S(4), tabH);

                // Group name label (if space allows)
                if (!string.IsNullOrEmpty(grp.GroupName))
                {
                    using var lblFmt  = new StringFormat
                    {
                        Trimming      = StringTrimming.EllipsisCharacter,
                        FormatFlags   = StringFormatFlags.NoWrap,
                        LineAlignment = StringAlignment.Center
                    };
                    using var lblBr   = new SolidBrush(Color.FromArgb(
                        160, _currentTheme?.SecondaryTextColor ?? ForeColor));
                    using var lblFont = new Font(Font.FontFamily, Font.Size - 1f, FontStyle.Regular);
                    var lblRect = new Rectangle(x + S(6), y, S(80), tabH);
                    g.DrawString(grp.GroupName, lblFont, lblBr, (RectangleF)lblRect, lblFmt);
                }

                // Store header rect for mouse hit-test (used for collapse toggle)
                grp.HeaderRect = new Rectangle(x, y, S(4) + S(80), tabH);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Path helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Chrome-style tab path: rounded top corners, flat bottom (merges with content area).</summary>
        private static GraphicsPath BuildChromeTabPath(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            int d = radius * 2;
            p.AddArc(r.Left,         r.Top, d, d, 180, 90);
            p.AddArc(r.Right - d,    r.Top, d, d, 270, 90);
            p.AddLine(r.Right,  r.Top + radius, r.Right,  r.Bottom);
            p.AddLine(r.Right,  r.Bottom,       r.Left,   r.Bottom);
            p.AddLine(r.Left,   r.Bottom,       r.Left,   r.Top + radius);
            p.CloseFigure();
            return p;
        }

        /// <summary>Generic fully-rounded rectangle.</summary>
        private static GraphicsPath BuildRoundedRect(Rectangle r, int radius)
        {
            var p = new GraphicsPath();
            int d = Math.Max(1, radius * 2);
            p.AddArc(r.Left,          r.Top,           d, d, 180, 90);
            p.AddArc(r.Right - d,     r.Top,           d, d, 270, 90);
            p.AddArc(r.Right - d,     r.Bottom - d,    d, d,   0, 90);
            p.AddArc(r.Left,          r.Bottom - d,    d, d,  90, 90);
            p.CloseFigure();
            return p;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Focus indicator
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Draws a dotted focus rectangle around the keyboard-active tab when the
        /// control has keyboard focus.  Skipped when the tab rect is empty or the
        /// control is not focused (mouse-only interaction path).
        /// </summary>
        private void DrawFocusIndicator(Graphics g)
        {
            if (!Focused) return;
            if (_activeTabIndex < 0 || _activeTabIndex >= _tabs.Count) return;

            var tabRect = _tabs[_activeTabIndex].TabRect;
            if (tabRect.IsEmpty) return;

            // Deflate 2 px so the focus ring sits just inside the tab border
            var focusRect = Rectangle.Inflate(tabRect, -2, -2);
            if (focusRect.Width < 4 || focusRect.Height < 4) return;

            if (SystemInformation.HighContrast)
            {
                // High-contrast: solid 3 px rounded rect in system Highlight colour
                using var pen = new Pen(SystemColors.Highlight, 3f);
                int r = Math.Min(TabRadius, focusRect.Width / 2);
                using var path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(focusRect.Left,                       focusRect.Top,                        r * 2, r * 2, 180, 90);
                path.AddArc(focusRect.Right - r * 2,              focusRect.Top,                        r * 2, r * 2, 270, 90);
                path.AddArc(focusRect.Right - r * 2,              focusRect.Bottom - r * 2,             r * 2, r * 2,   0, 90);
                path.AddArc(focusRect.Left,                       focusRect.Bottom - r * 2,             r * 2, r * 2,  90, 90);
                path.CloseFigure();
                g.DrawPath(pen, path);
            }
            else
            {
                // Normal mode: standard dotted focus rectangle
                ControlPaint.DrawFocusRectangle(g, focusRect);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Colour helper
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Linear blend between two colours (t=0 → c1, t=1 → c2).</summary>
        internal static Color Blend(Color c1, Color c2, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            return Color.FromArgb(
                (int)(c1.A + (c2.A - c1.A) * t),
                (int)(c1.R + (c2.R - c1.R) * t),
                (int)(c1.G + (c2.G - c1.G) * t),
                (int)(c1.B + (c2.B - c1.B) * t));
        }
    }
}
