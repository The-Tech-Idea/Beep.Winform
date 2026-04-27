using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        #region Painting
        
        /// <summary>
        /// Handles tab transition animation rendering — draws a sliding accent bar
        /// that moves from the previously-active tab to the currently-active one.
        /// This replaces the old unreliable DrawToBitmap cross-fade approach.
        /// Only draws when a transition is in progress (<c>_indicatorProgress &lt; 1f</c>).
        /// </summary>
        private void HandleTabTransition(Graphics g)
        {
            if (_indicatorProgress >= 1f) return;
            if (_indicatorFrom.IsEmpty || _indicatorTo.IsEmpty) return;

            DrawSlidingIndicator(g);
        }

        /// <summary>
        /// Draws a pill-shaped accent bar interpolated between <c>_indicatorFrom</c> and
        /// <c>_indicatorTo</c> using the current <c>_indicatorProgress</c> (0 → 1, eased).
        /// The bar is drawn slightly translucent so it layers naturally over the tab
        /// backgrounds without obscuring text.
        /// </summary>
        private void DrawSlidingIndicator(Graphics g)
        {
            // Use a smooth ease-out curve: square the complement.
            float t = 1f - (1f - _indicatorProgress) * (1f - _indicatorProgress);

            // Lerp the bounding rectangle.
            int x = (int)(_indicatorFrom.X + (_indicatorTo.X - _indicatorFrom.X) * t);
            int y = (int)(_indicatorFrom.Y + (_indicatorTo.Y - _indicatorFrom.Y) * t);
            int w = (int)(_indicatorFrom.Width  + (_indicatorTo.Width  - _indicatorFrom.Width)  * t);
            int h = (int)(_indicatorFrom.Height + (_indicatorTo.Height - _indicatorFrom.Height) * t);

            if (w <= 0 || h <= 0) return;
            var bar = new Rectangle(x, y, w, h);

            // Resolve indicator colour (same helper used by DrawActiveIndicator in TabPaintHelper).
            Color baseColor = _currentTheme?.ActiveBorderColor ?? Color.Empty;
            if (baseColor == Color.Empty || baseColor.A == 0)
                baseColor = _currentTheme?.TabSelectedBorderColor ?? Color.DodgerBlue;

            // Fade: full opacity early in the animation, fade out as it settles.
            int alpha = (int)(Math.Min(1f, (1f - _indicatorProgress) * 3f) * 200);
            if (alpha <= 4) return;
            Color indicatorColor = Color.FromArgb(alpha, baseColor);

            // Thickness and edge position match DrawActiveIndicator in TabPaintHelper.
            int thickness = Math.Max(2, DpiScalingHelper.ScaleValue(3, this));
            int inset      = Math.Max(0, DpiScalingHelper.ScaleValue(4, this));

            Rectangle indicatorBar;
            switch (_tabPosition)
            {
                case TabPosition.Bottom:
                    indicatorBar = new Rectangle(bar.X + inset, bar.Y, Math.Max(4, bar.Width - inset * 2), thickness);
                    break;
                case TabPosition.Left:
                    indicatorBar = new Rectangle(bar.Right - thickness, bar.Y + inset, thickness, Math.Max(4, bar.Height - inset * 2));
                    break;
                case TabPosition.Right:
                    indicatorBar = new Rectangle(bar.X, bar.Y + inset, thickness, Math.Max(4, bar.Height - inset * 2));
                    break;
                default: // Top
                    indicatorBar = new Rectangle(bar.X + inset, bar.Bottom - thickness, Math.Max(4, bar.Width - inset * 2), thickness);
                    break;
            }

            if (indicatorBar.Width <= 0 || indicatorBar.Height <= 0) return;

            int r = thickness / 2;
            if (r >= 1 && indicatorBar.Width > r * 2 && indicatorBar.Height > r * 2)
            {
                using (var path = CreatePartialRoundedPath(indicatorBar, r, true, true, true, true))
                using (var brush = new SolidBrush(indicatorColor))
                    g.FillPath(brush, path);
            }
            else
            {
                using (var brush = new SolidBrush(indicatorColor))
                    g.FillRectangle(brush, indicatorBar);
            }
        }

        /// <summary>
        /// Draws the content area background with proper theme colors.
        /// Only rounds the corners that lie on the outer boundary of the container
        /// (not the edge shared with the tab strip) so the two sections tile seamlessly.
        /// </summary>
        private void DrawContentAreaBackground(Graphics g)
        {
            if (g == null || _contentArea.IsEmpty) return;
            if (IsTransparentBackground) return;

            Color bgColor = GetEffectiveContentBackColor();
            int r = (IsRounded && BorderRadius > 0) ? BorderRadius : 0;

            // Round only the corners on the container's outer boundary.
            bool tl, tr, bl, br;
            switch (_tabPosition)
            {
                case TabPosition.Top:    tl = false; tr = false; bl = true;  br = true;  break;
                case TabPosition.Bottom: tl = true;  tr = true;  bl = false; br = false; break;
                case TabPosition.Left:   tl = false; tr = true;  bl = false; br = true;  break;
                case TabPosition.Right:  tl = true;  tr = false; bl = true;  br = false; break;
                default:                 tl = true;  tr = true;  bl = true;  br = true;  break;
            }

            using (var path = CreatePartialRoundedPath(_contentArea, r, tl, tr, bl, br))
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillPath(brush, path);
            }
        }
        
        /// <summary>
        /// Draws the tab strip background.
        /// Rounds only the outer-boundary corners and extends 1 px into the content area
        /// to seal the seam between the two sections.
        /// </summary>
        private void DrawTabAreaBackground(Graphics g)
        {
            if (g == null || _tabArea.IsEmpty) return;
            if (IsTransparentBackground) return;

            Color tabStripColor = _tabBackColor;
            int r = (IsRounded && BorderRadius > 0) ? BorderRadius : 0;

            // Round only the corners on the container's outer boundary.
            bool tl, tr, bl, br;
            switch (_tabPosition)
            {
                case TabPosition.Top:    tl = true;  tr = true;  bl = false; br = false; break;
                case TabPosition.Bottom: tl = false; tr = false; bl = true;  br = true;  break;
                case TabPosition.Left:   tl = true;  tr = false; bl = true;  br = false; break;
                case TabPosition.Right:  tl = false; tr = true;  bl = false; br = true;  break;
                default:                 tl = true;  tr = true;  bl = true;  br = true;  break;
            }

            // Extend 1 px into the content area to seal the boundary seam.
            var area = _tabArea;
            switch (_tabPosition)
            {
                case TabPosition.Top:    area = new Rectangle(area.X, area.Y, area.Width, area.Height + 1); break;
                case TabPosition.Bottom: area = new Rectangle(area.X, area.Y - 1, area.Width, area.Height + 1); break;
                case TabPosition.Left:   area = new Rectangle(area.X, area.Y, area.Width + 1, area.Height); break;
                case TabPosition.Right:  area = new Rectangle(area.X - 1, area.Y, area.Width + 1, area.Height); break;
            }

            using (var path = CreatePartialRoundedPath(area, r, tl, tr, bl, br))
            {
                // ---- Enhancement 6: optional gradient fill ----
                if (_useTabStripGradient && area.Width > 1 && area.Height > 1)
                {
                    Color endColor = _tabStripGradientEndColor != Color.Empty
                        ? _tabStripGradientEndColor
                        : ShiftLuminance(tabStripColor, -0.08f);

                    bool horizontal = _tabPosition == TabPosition.Top || _tabPosition == TabPosition.Bottom;
                    System.Drawing.Drawing2D.LinearGradientMode gradMode = horizontal
                        ? System.Drawing.Drawing2D.LinearGradientMode.Horizontal
                        : System.Drawing.Drawing2D.LinearGradientMode.Vertical;

                    try
                    {
                        using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                            area, tabStripColor, endColor, gradMode))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                    catch
                    {
                        // Fallback to solid fill if gradient fails (e.g., zero-size area).
                        using (var brush = new SolidBrush(tabStripColor))
                            g.FillPath(brush, path);
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(tabStripColor))
                        g.FillPath(brush, path);
                }
            }
        }

        /// <summary>
        /// Draws tabs directly in OnPaint with proper styling
        /// </summary>
        private void DrawTabsDirectlyInOnPaint(Graphics g)
        {
            if (g == null || _tabArea.IsEmpty) return;
            
            // Ensure paint helper exists and is configured
            EnsurePaintHelper();

            // Clip all tab-area drawing so individual tabs cannot bleed into the content area.
            var savedClip = g.Clip?.Clone() as Region;
            try
            {
                g.SetClip(_tabArea, System.Drawing.Drawing2D.CombineMode.Intersect);

                // Draw tab strip background first
                DrawTabAreaBackground(g);
                
                // Draw each visible tab — skip the tab being dragged (it is drawn as a ghost below).
                foreach (var tab in _tabs.Where(t => t.IsVisible && !t.Bounds.IsEmpty))
                {
                    if (_isDragging && tab == _dragTab) continue;
                    DrawTab(g, tab);
                }

                // ---- Enhancement 5: drag-to-reorder visuals ----
                if (_isDragging)
                {
                    DrawDropIndicator(g);
                    if (_dragTab != null)
                        DrawDragGhost(g, _dragTab);
                }

                // Draw scroll buttons if needed
                if (_needsScrolling)
                {
                    bool horiz = _tabPosition == TabPosition.Top || _tabPosition == TabPosition.Bottom;
                    DrawModernButton(g, _scrollLeftButton,
                        horiz ? ArrowDirection.Left : ArrowDirection.Up,
                        ControlStyle,
                        isHovered:  _hoveredScrollButton == 1,
                        isPressed:  _pressedScrollButton == 1,
                        isDisabled: _scrollOffset <= 0);
                    DrawModernButton(g, _scrollRightButton,
                        horiz ? ArrowDirection.Right : ArrowDirection.Down,
                        ControlStyle,
                        isHovered:  _hoveredScrollButton == 2,
                        isPressed:  _pressedScrollButton == 2,
                        isDisabled: !CanScrollRight());
                    
                    // Enhancement 5: Overflow Dropdown chevron button
                    DrawModernButton(g, _overflowButton,
                        ArrowDirection.Down,
                        ControlStyle,
                        isHovered:  _hoveredScrollButton == 4,
                        isPressed:  _pressedScrollButton == 4,
                        isDisabled: false);

                    DrawModernButton(g, _newTabButton, null, ControlStyle,
                        isPlusButton: true,
                        isHovered:   _hoveredScrollButton == 3,
                        isPressed:   _pressedScrollButton == 3);
                }
                else if (!_newTabButton.IsEmpty)
                {
                    DrawModernButton(g, _newTabButton, null, ControlStyle,
                        isPlusButton: true,
                        isHovered:   _hoveredScrollButton == 3,
                        isPressed:   _pressedScrollButton == 3);
                }
            }
            finally
            {
                if (savedClip != null) { g.Clip = savedClip; savedClip.Dispose(); }
                else g.ResetClip();
            }
            
            // Draw separator line between tabs and content (outside clip — sits on the boundary)
            DrawTabContentSeparator(g);
        }
        
        /// <summary>
        /// Ensures the paint helper is initialized and configured
        /// </summary>
        private void EnsurePaintHelper()
        {
            if (_paintHelper == null)
            {
                _paintHelper = new TabPaintHelper(_currentTheme, ControlStyle, IsTransparentBackground);
            }

            // Always update to current settings
            _paintHelper.ControlStyle            = ControlStyle;
            _paintHelper.IsTransparent           = IsTransparentBackground;
            _paintHelper.TabStyle                = TabStyle;
            _paintHelper.Theme                   = _currentTheme;
            _paintHelper.OwnerControl            = this;
            // Keep tab corner radius in sync with the container's overall shape.
            _paintHelper.ContainerBorderRadius   = (IsRounded && BorderRadius > 0) ? BorderRadius : 0;
        }
      
        /// <summary>
        /// DrawContent is called by BaseControl.OnPaint - this is where we draw our tabs.
        /// Pushes a rounded-rect clip that matches the control's overall shape so no
        /// child painting (tab corners, backgrounds) can bleed outside the rounded border.
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            if (_batchMode) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Push a clip path that matches the container's rounded shape.
            Region? previousClip = null;
            if (IsRounded && BorderRadius > 0 && Width > 0 && Height > 0)
            {
                try
                {
                    using (var clipPath = CreatePartialRoundedPath(
                        new Rectangle(0, 0, Width, Height), BorderRadius, true, true, true, true))
                    {
                        previousClip = g.Clip?.Clone() as Region;
                        var newClip = new Region(clipPath);
                        if (previousClip != null && !previousClip.IsInfinite(g))
                            newClip.Intersect(previousClip);
                        g.Clip = newClip;
                    }
                }
                catch { /* non-fatal — continue without clip */ }
            }

            try
            {
                try { DrawContentAreaBackground(g); } catch { }

                if (_displayMode == ContainerDisplayMode.Tabbed && !_tabArea.IsEmpty && _tabs != null && _tabs.Count > 0)
                {
                    try { DrawTabsDirectlyInOnPaint(g); } catch { }

                    // ── Tooltip hover card (drawn outside tab clip) ─────────
                    if (_showTooltip && _tooltipTab != null && !_tooltipTab.Bounds.IsEmpty)
                        try { DrawTabTooltip(g, _tooltipTab); } catch { }
                }
                if (_displayMode == ContainerDisplayMode.Tabbed
                         && (_tabs == null || _tabs.Count == 0)
                         && _showEmptyState)
                {
                    try { DrawEmptyState(g); } catch { }
                }

                try { HandleTabTransition(g); } catch { }
            }
            finally
            {
                if (previousClip != null)
                {
                    try { g.Clip = previousClip; }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Draws a centred empty-state placeholder in the content area when there are no tabs.
        /// The placeholder consists of:
        /// <list type="bullet">
        ///   <item>A simple inline-drawn "folder + tab" icon (no external image dependency).</item>
        ///   <item>The <see cref="_emptyStateText"/> message below the icon.</item>
        /// </list>
        /// </summary>
        private void DrawEmptyState(Graphics g)
        {
            var area = _contentArea.IsEmpty ? ClientRectangle : _contentArea;
            if (area.Width < 40 || area.Height < 40) return;

            // ---------- choose colours ----------
            // Muted: blend 40 % of the foreground into the background.
            var fg      = _currentTheme?.DisabledForeColor ?? Color.Gray;
            var bg      = GetEffectiveContentBackColor();
            Color iconColor = Color.FromArgb(
                (fg.R * 4 + bg.R * 6) / 10,
                (fg.G * 4 + bg.G * 6) / 10,
                (fg.B * 4 + bg.B * 6) / 10);
            Color textColor = iconColor;

            // ---------- icon size & position ----------
            int iconSize  = Math.Min(DpiScalingHelper.ScaleValue(48, this), Math.Min(area.Width, area.Height) / 3);
            iconSize      = Math.Max(16, iconSize);
            int iconX     = area.X + (area.Width  - iconSize) / 2;
            int iconY     = area.Y + (area.Height - iconSize) / 2 - DpiScalingHelper.ScaleValue(12, this);

            // Draw a simple "open folder with a tiny tab on top" icon using lines.
            try
            {
                float penW = Math.Max(1.5f, DpiScalingHelper.ScaleValue(2f, this));
                using (var pen = new Pen(iconColor, penW)
                       { LineJoin = LineJoin.Round, StartCap = LineCap.Round, EndCap = LineCap.Round })
                using (var fillBrush = new SolidBrush(Color.FromArgb(30, iconColor)))
                {
                    // Folder body — partial-rounded path (only bottom corners rounded so it
                    // looks like an open folder with a flat top edge adjacent to the tab flap).
                    var folder = new Rectangle(iconX, iconY + iconSize / 4, iconSize, iconSize * 3 / 4);
                    int cr = Math.Max(2, (int)(penW * 2));
                    using (var folderPath = CreatePartialRoundedPath(folder, cr, false, false, true, true))
                    {
                        g.FillPath(fillBrush, folderPath);
                        g.DrawPath(pen, folderPath);
                    }

                    // Folder tab (top-left flap)
                    int tabW = iconSize / 3;
                    int tabH = iconSize / 6;
                    var tabPts = new PointF[]
                    {
                        new PointF(folder.X,          folder.Y),
                        new PointF(folder.X + tabW,   folder.Y),
                        new PointF(folder.X + tabW + tabH / 2, folder.Y - tabH),
                        new PointF(folder.X + tabW * 2 - tabH / 2, folder.Y - tabH),
                        new PointF(folder.X + tabW * 2, folder.Y),
                    };
                    g.DrawLines(pen, tabPts);

                    // Three subtle horizontal lines inside the folder ("content lines")
                    int lineMargin = iconSize / 6;
                    int lineSpacing = (folder.Height - lineMargin * 2) / 4;
                    for (int i = 1; i <= 3; i++)
                    {
                        int ly = folder.Y + lineMargin + i * lineSpacing;
                        float lineLen = folder.Width * (i == 2 ? 0.55f : 0.75f);
                        using (var linePen = new Pen(Color.FromArgb(80, iconColor), penW * 0.6f))
                        {
                            g.DrawLine(linePen,
                                folder.X + lineMargin, ly,
                                folder.X + lineMargin + (int)lineLen, ly);
                        }
                    }
                }
            }
            catch { /* icon drawing is non-fatal */ }

            // ---------- text ----------
            if (!string.IsNullOrWhiteSpace(_emptyStateText))
            {
                int textY    = iconY + iconSize + DpiScalingHelper.ScaleValue(10, this);
                int textH    = area.Bottom - textY - DpiScalingHelper.ScaleValue(4, this);
                var textRect = new Rectangle(area.X + DpiScalingHelper.ScaleValue(8, this),
                                             textY,
                                             area.Width - DpiScalingHelper.ScaleValue(16, this),
                                             Math.Max(1, textH));
                if (textRect.Width > 0 && textRect.Height > 0)
                {
                    var fmt = new StringFormat
                    {
                        Alignment     = StringAlignment.Center,
                        LineAlignment = StringAlignment.Near,
                        Trimming      = StringTrimming.EllipsisCharacter,
                        FormatFlags   = StringFormatFlags.LineLimit
                    };
                    using (var brush = new SolidBrush(textColor))
                    {
                        var emptyStateFont = FontListHelper.GetFont(TextFont.FontFamily.Name, Math.Max(6f, TextFont.Size * 0.9f), FontStyle.Regular);
                        try
                        {
                            g.DrawString(_emptyStateText, emptyStateFont ?? TextFont, brush, textRect, fmt);
                        }
                        finally
                        {
                            if (emptyStateFont != null && !ReferenceEquals(emptyStateFont, TextFont))
                            {
                                emptyStateFont.Dispose();
                            }
                        }
                    }
                }
            }

            // ---------- action button ----------
            if (ShowEmptyStateActionButton && !string.IsNullOrWhiteSpace(EmptyStateActionText))
            {
                var btnRect = GetEmptyStateButtonRect();
                if (!btnRect.IsEmpty && btnRect.Width > 0 && btnRect.Height > 0)
                {
                    var btnRadius = Math.Min(btnRect.Height / 2, DpiScalingHelper.ScaleValue(18, this));
                    
                    // Use accent color logic, slightly muted
                    Color btnBgColor = _currentTheme?.AccentColor ?? Color.RoyalBlue;
                    Color btnHoverColor = Color.FromArgb(
                        Math.Min(255, btnBgColor.R + 20),
                        Math.Min(255, btnBgColor.G + 20),
                        Math.Min(255, btnBgColor.B + 20));
                    
                    if (_emptyStateButtonHovered)
                    {
                        btnBgColor = btnHoverColor;
                    }

                    try
                    {
                        using (var path = GraphicsExtensions.GetRoundedRectPath(btnRect, btnRadius))
                        {
                            if (path != null)
                            {
                                using (var brush = new SolidBrush(btnBgColor))
                                {
                                    g.FillPath(brush, path);
                                }
                            }
                        }
                        
                        var fmt = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter
                        };
                        
                        using (var textBrush = new SolidBrush(Color.White))
                        {
                            g.DrawString(EmptyStateActionText, TextFont, textBrush, btnRect, fmt);
                        }
                    }
                    catch { /* Ignore empty path or parameter errors and just let it skip drawing */ }
                }
            }
        }

        private void DrawTabTransition(Graphics g)
        {
            if (_previousTab?.Addin == null || _activeTab?.Addin == null) return;

            var previousControl = _previousTab.Addin as Control;
            var activeControl = _activeTab.Addin as Control;

            if (previousControl == null || activeControl == null) return;

            // Calculate opacity values for cross-fade
            // transitionProgress: 0 = showing old, 1 = showing new
            float transitionProgress = _animationHelper.TransitionProgress;
            float oldOpacity = 1.0f - transitionProgress;
            float newOpacity = transitionProgress;

            // Draw previous control fading out (controls are hidden during transition, so we draw them)
            if (oldOpacity > 0.01f)
            {
                // Temporarily make control visible for DrawToBitmap, then hide again
                bool wasVisible = previousControl.Visible;
                previousControl.Visible = true;
                DrawControlWithOpacity(g, previousControl, _contentArea, oldOpacity);
                previousControl.Visible = wasVisible;
            }

            // Draw active control fading in
            if (newOpacity > 0.01f)
            {
                // Temporarily make control visible for DrawToBitmap, then hide again
                bool wasVisible = activeControl.Visible;
                activeControl.Visible = true;
                DrawControlWithOpacity(g, activeControl, _contentArea, newOpacity);
                activeControl.Visible = wasVisible;
            }
        }

        private void DrawControlWithOpacity(Graphics g, Control control, Rectangle bounds, float opacity)
        {
            if (control == null || !control.Visible || opacity <= 0) return;

            try
            {
                // Create a bitmap to capture the control's appearance
                using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (var bitmapGraphics = Graphics.FromImage(bitmap))
                    {
                        bitmapGraphics.SmoothingMode = g.SmoothingMode;
                        bitmapGraphics.InterpolationMode = g.InterpolationMode;
                        bitmapGraphics.PixelOffsetMode = g.PixelOffsetMode;
                        bitmapGraphics.TextRenderingHint = g.TextRenderingHint;
                        
                        // Capture control rendering using DrawToBitmap
                        try
                        {
                            control.DrawToBitmap(bitmap, new Rectangle(0, 0, bounds.Width, bounds.Height));
                        }
                        catch
                        {
                            // If DrawToBitmap fails, fall back to simple fill
                            bitmapGraphics.Clear(control.BackColor);
                        }
                    }

                    // Apply opacity and draw
                    using (var imageAttributes = new System.Drawing.Imaging.ImageAttributes())
                    {
                        float[][] colorMatrixElements = {
                            new float[] {1, 0, 0, 0, 0},
                            new float[] {0, 1, 0, 0, 0},
                            new float[] {0, 0, 1, 0, 0},
                            new float[] {0, 0, 0, opacity, 0},
                            new float[] {0, 0, 0, 0, 1}
                        };

                        var colorMatrix = new System.Drawing.Imaging.ColorMatrix(colorMatrixElements);
                        imageAttributes.SetColorMatrix(colorMatrix, 
                            System.Drawing.Imaging.ColorMatrixFlag.Default, 
                            System.Drawing.Imaging.ColorAdjustType.Bitmap);

                        g.DrawImage(bitmap, bounds, 0, 0, bounds.Width, bounds.Height, 
                            GraphicsUnit.Pixel, imageAttributes);
                    }
                }
            }
            catch
            {
                // If rendering fails, just ensure the control is visible at the right opacity
                // Fall back to simple approach
            }
        }
        
        private void DrawTabContentSeparator(Graphics g)
        {
            if (_tabArea.IsEmpty || _contentArea.IsEmpty) return;
            
            // Use theme border color
            Color separatorColor = _borderColor;
            float penWidth = Math.Max(1f, DpiScalingHelper.ScaleValue(1f, this));
            
            using (var pen = new Pen(separatorColor, penWidth))
            {
                switch (_tabPosition)
                {
                    case TabPosition.Top:
                        g.DrawLine(pen, _tabArea.Left, _tabArea.Bottom - 1, _tabArea.Right, _tabArea.Bottom - 1);
                        break;
                    case TabPosition.Bottom:
                        g.DrawLine(pen, _tabArea.Left, _tabArea.Top, _tabArea.Right, _tabArea.Top);
                        break;
                    case TabPosition.Left:
                        g.DrawLine(pen, _tabArea.Right - 1, _tabArea.Top, _tabArea.Right - 1, _tabArea.Bottom);
                        break;
                    case TabPosition.Right:
                        g.DrawLine(pen, _tabArea.Left, _tabArea.Top, _tabArea.Left, _tabArea.Bottom);
                        break;
                }
            }
        }

        private void DrawTab(Graphics g, AddinTab tab)
        {
            if (tab == null || tab.Bounds.IsEmpty || string.IsNullOrEmpty(tab.Title)) return;

            EnsurePaintHelper();

            var isActive  = tab == _activeTab;
            var isHovered = tab == _hoveredTab;

            // Determine first/last visible tab so the paint helper can shape the outer
            // corners to match the container's border radius.
            var visible = _tabs.Where(t => t.IsVisible && !t.Bounds.IsEmpty).ToList();
            bool isFirst = visible.Count > 0 && visible[0] == tab;
            bool isLast  = visible.Count > 0 && visible[visible.Count - 1] == tab;

            _paintHelper.DrawProfessionalTab(
                g, tab.Bounds, tab.Title,
                TextFont,
                isActive, isHovered,
                _showCloseButtons && !tab.IsPinned,
                tab.IsCloseHovered, tab.AnimationProgress,
                isFirst, isLast, _tabPosition,
                tab.IconPath, tab.BadgeText, tab.BadgeColor, tab.IsPinned);
        }

        // ── Tooltip hover card ──────────────────────────────────────────────────
        private void DrawTabTooltip(Graphics g, AddinTab tab)
        {
            if (tab == null || string.IsNullOrEmpty(tab.TooltipText)) return;

            var titleFont = TextFont;
            var descFont  = FontListHelper.GetFont(titleFont.FontFamily.Name, Math.Max(7f, titleFont.Size * 0.85f), FontStyle.Regular) ?? titleFont;

            try
            {
                string title = tab.Title ?? "Tab";
                string desc  = tab.TooltipText;

                // Measure
                var titleSize = TextRenderer.MeasureText(title, titleFont, new Size(300, int.MaxValue),
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
                var descSize  = TextRenderer.MeasureText(desc, descFont, new Size(300, int.MaxValue),
                    TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);

                int pad    = DpiScalingHelper.ScaleValue(10, this);
                int gap    = DpiScalingHelper.ScaleValue(4, this);
                int cardW  = Math.Max(titleSize.Width, descSize.Width) + pad * 2;
                int cardH  = titleSize.Height + gap + descSize.Height + pad * 2;

                // Position: centered horizontally under the tab
                int tabCenterX = tab.Bounds.X + tab.Bounds.Width / 2;
                int cardX = tabCenterX - cardW / 2;
                // Clamp to control bounds
                if (cardX < 4) cardX = 4;
                if (cardX + cardW > ClientRectangle.Right - 4)
                    cardX = ClientRectangle.Right - cardW - 4;

                // Vertical: below the tab (or above if near bottom)
                int pointerH = DpiScalingHelper.ScaleValue(6, this);
                int cardY = tab.Bounds.Bottom + DpiScalingHelper.ScaleValue(4, this) + pointerH;
                bool above = false;
                if (cardY + cardH > ClientRectangle.Bottom)
                {
                    cardY = tab.Bounds.Top - cardH - DpiScalingHelper.ScaleValue(4, this) - pointerH;
                    above = true;
                }

                var cardRect = new Rectangle(cardX, cardY, cardW, cardH);

                // Shadow
                var shadowRect = Rectangle.Inflate(cardRect, 2, 2);
                shadowRect.Offset(1, 1);
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, Color.Black)))
                    g.FillRectangle(shadowBrush, shadowRect);

                // Card background with rounded corners
                Color cardBg  = _currentTheme?.BackColor ?? ColorUtils.MapSystemColor(SystemColors.Info);
                Color cardFg  = _currentTheme?.ForeColor ?? ColorUtils.MapSystemColor(SystemColors.InfoText);
                Color borderC = _currentTheme?.BorderColor ?? ColorUtils.MapSystemColor(SystemColors.ControlDark);
                int radius = Math.Min(6, cardH / 2);

                // Build path with pointer triangle
                using (var path = CreateTooltipPath(cardRect, pointerH, above, radius, tabCenterX, cardX))
                using (var bgBrush = new SolidBrush(cardBg))
                using (var borderPen = new Pen(borderC))
                {
                    g.FillPath(bgBrush, path);
                    g.DrawPath(borderPen, path);
                }

                // Title (bold)
                var titleRect = new Rectangle(cardRect.X + pad, cardRect.Y + pad, cardRect.Width - pad * 2, titleSize.Height);
                var boldFont = FontListHelper.GetFont(titleFont.FontFamily.Name, titleFont.Size, FontStyle.Bold) ?? titleFont;
                try
                {
                    TextRenderer.DrawText(g, title, boldFont, titleRect, cardFg,
                        TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
                }
                finally
                {
                    if (boldFont != titleFont) boldFont?.Dispose();
                }

                // Description
                var descRect = new Rectangle(cardRect.X + pad, titleRect.Bottom + gap, cardRect.Width - pad * 2, descSize.Height);
                TextRenderer.DrawText(g, desc, descFont, descRect, Color.FromArgb(180, cardFg),
                    TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
            }
            catch { /* tooltip drawing is non-fatal */ }
            finally
            {
                if (descFont != null && descFont != titleFont) descFont.Dispose();
            }
        }

        /// <summary>
        /// Creates a rounded rectangle path with a small pointer triangle aimed at the tab.
        /// </summary>
        private GraphicsPath CreateTooltipPath(Rectangle cardRect, int pointerHeight, bool pointerAbove, int radius, int targetX, int cardX)
        {
            var path = new GraphicsPath();
            if (cardRect.Width <= 0 || cardRect.Height <= 0) return path;

            int r = Math.Min(radius, Math.Min(cardRect.Width / 2, cardRect.Height / 2));
            int pointerW = DpiScalingHelper.ScaleValue(10, this); // width of pointer base

            // Clamp pointer position to card bounds (leave margin for corners)
            int pointerX = targetX - cardX;
            int minPtrX = r + pointerW / 2 + 2;
            int maxPtrX = cardRect.Width - r - pointerW / 2 - 2;
            pointerX = Math.Max(minPtrX, Math.Min(maxPtrX, pointerX));

            int halfW = pointerW / 2;

            if (pointerAbove)
            {
                // Pointer at top edge, pointing down toward tab
                int tipY = cardRect.Y;
                int baseY = cardRect.Y + pointerHeight;

                // Start at left of pointer base, go clockwise
                path.AddLine(cardRect.X, baseY, cardRect.X + pointerX - halfW, baseY);
                path.AddLine(cardRect.X + pointerX - halfW, baseY, cardRect.X + pointerX, tipY);
                path.AddLine(cardRect.X + pointerX, tipY, cardRect.X + pointerX + halfW, baseY);
                path.AddLine(cardRect.X + pointerX + halfW, baseY, cardRect.Right - r, baseY);

                // Top-right corner
                if (r > 0) path.AddArc(cardRect.Right - r * 2, baseY, r * 2, r * 2, 270, 90);
                path.AddLine(cardRect.Right, baseY + (r > 0 ? r : 0), cardRect.Right, cardRect.Bottom - r);

                // Bottom-right corner
                if (r > 0) path.AddArc(cardRect.Right - r * 2, cardRect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                path.AddLine(cardRect.Right - (r > 0 ? r : 0), cardRect.Bottom, cardRect.X + r, cardRect.Bottom);

                // Bottom-left corner
                if (r > 0) path.AddArc(cardRect.X, cardRect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                path.AddLine(cardRect.X, cardRect.Bottom - (r > 0 ? r : 0), cardRect.X, baseY);

                // Left edge back to pointer base
                path.AddLine(cardRect.X, baseY, cardRect.X, baseY);
            }
            else
            {
                // Pointer at bottom edge, pointing up toward tab
                int tipY = cardRect.Bottom;
                int baseY = cardRect.Bottom - pointerHeight;

                // Start at top-left, go clockwise
                path.AddLine(cardRect.X + r, cardRect.Y, cardRect.Right - r, cardRect.Y);

                // Top-right corner
                if (r > 0) path.AddArc(cardRect.Right - r * 2, cardRect.Y, r * 2, r * 2, 270, 90);
                path.AddLine(cardRect.Right, cardRect.Y + (r > 0 ? r : 0), cardRect.Right, baseY);

                // Bottom-right to pointer base
                if (r > 0) path.AddArc(cardRect.Right - r * 2, baseY - r * 2, r * 2, r * 2, 0, 90);
                path.AddLine(cardRect.Right - (r > 0 ? r : 0), baseY, cardRect.X + pointerX + halfW, baseY);

                // Pointer triangle
                path.AddLine(cardRect.X + pointerX + halfW, baseY, cardRect.X + pointerX, tipY);
                path.AddLine(cardRect.X + pointerX, tipY, cardRect.X + pointerX - halfW, baseY);

                // Pointer base to bottom-left
                path.AddLine(cardRect.X + pointerX - halfW, baseY, cardRect.X + r, baseY);

                // Bottom-left corner
                if (r > 0) path.AddArc(cardRect.X, baseY - r * 2, r * 2, r * 2, 90, 90);
                path.AddLine(cardRect.X, baseY - (r > 0 ? r : 0), cardRect.X, cardRect.Y + r);

                // Top-left corner
                if (r > 0) path.AddArc(cardRect.X, cardRect.Y, r * 2, r * 2, 180, 90);
                path.AddLine(cardRect.X + (r > 0 ? r : 0), cardRect.Y, cardRect.X + r, cardRect.Y);
            }

            path.CloseFigure();
            return path;
        }

        private void DrawScrollButtons(Graphics g)
        {
            if (!_needsScrolling) return;

            // Get control style for modern button rendering
            var controlStyle = ControlStyle;
            
            // Draw scroll buttons with modern styling
            DrawModernButton(g, _scrollLeftButton, _tabPosition == TabPosition.Top || _tabPosition == TabPosition.Bottom ? ArrowDirection.Left : ArrowDirection.Up, controlStyle);
            DrawModernButton(g, _scrollRightButton, _tabPosition == TabPosition.Top || _tabPosition == TabPosition.Bottom ? ArrowDirection.Right : ArrowDirection.Down, controlStyle);
            DrawModernButton(g, _newTabButton, null, controlStyle, isPlusButton: true);
        }
        
        /// <summary>
        /// Draws one of the scroll or utility buttons (left/right scroll, new-tab +) with full
        /// hover, pressed, and disabled visual feedback.
        /// </summary>
        private void DrawModernButton(
            Graphics g, Rectangle bounds, ArrowDirection? arrowDirection,
            BeepControlStyle style,
            bool isPlusButton = false,
            bool isHovered  = false,
            bool isPressed  = false,
            bool isDisabled = false)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // ------------------------------------------------------------------
            // Choose background colours based on interaction state.
            // ------------------------------------------------------------------
            Color bgTop, bgBottom, borderCol;
            float iconAlpha;

            if (isDisabled)
            {
                // Greyed-out; same hue as tab strip but half-opacity.
                bgTop      = bgBottom = _tabBackColor;
                borderCol  = Color.FromArgb(80, _borderColor);
                iconAlpha  = 0.35f;
            }
            else if (isPressed)
            {
                // Pressed: noticeably darker background, no top highlight.
                bgTop      = ShiftLuminance(_tabBackColor, -0.18f);
                bgBottom   = ShiftLuminance(_tabBackColor, -0.10f);
                borderCol  = ShiftLuminance(_borderColor, -0.15f);
                iconAlpha  = 1f;
            }
            else if (isHovered)
            {
                // Hovered: lighter background, accent border.
                bgTop      = ShiftLuminance(_tabBackColor, 0.25f);
                bgBottom   = ShiftLuminance(_tabBackColor, 0.12f);
                borderCol  = _currentTheme?.ActiveBorderColor ?? _borderColor;
                iconAlpha  = 1f;
            }
            else
            {
                // Normal resting state.
                bgTop      = ShiftLuminance(_tabBackColor, 0.10f);
                bgBottom   = ShiftLuminance(_tabBackColor, -0.05f);
                borderCol  = _borderColor;
                iconAlpha  = 0.75f;
            }

            // Pressed: visually "push" the icon in by 1 px.
            var iconBounds = isPressed
                ? new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 1, bounds.Height - 1)
                : bounds;

            // Give hover state a subtle 1-px glow ring (inflate outward) — only in non-pressed.
            var pathBounds = (isHovered && !isPressed)
                ? Rectangle.Inflate(bounds, 0, 0)   // keep flush; border colour change is enough
                : bounds;

            var buttonPath = BeepStyling.CreateControlStylePath(pathBounds, style);
            if (buttonPath == null) return;

            try
            {
                // Background gradient.
                if (pathBounds.Width > 0 && pathBounds.Height > 0)
                {
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        pathBounds, bgTop, bgBottom,
                        System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, buttonPath);
                    }
                }

                // Border — thicker on hover for visibility.
                float borderWidth = Math.Max(1f, DpiScalingHelper.ScaleValue(isHovered ? 1.5f : 1f, this));
                using (var pen = new Pen(borderCol, borderWidth))
                    g.DrawPath(pen, buttonPath);

                // Icon colour — apply disabled alpha via a semi-transparent solid brush / pen.
                var iconColor = isDisabled
                    ? Color.FromArgb((int)(255 * iconAlpha), _tabForeColor)
                    : _tabForeColor;

                float iconPenWidth = Math.Max(1.5f, DpiScalingHelper.ScaleValue(2f, this));
                using (var pen = new Pen(iconColor, iconPenWidth))
                {
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap   = System.Drawing.Drawing2D.LineCap.Round;

                    if (isPlusButton)
                    {
                        var center = new Point(iconBounds.X + iconBounds.Width / 2, iconBounds.Y + iconBounds.Height / 2);
                        var sz = Math.Min(iconBounds.Width, iconBounds.Height) / 4;
                        g.DrawLine(pen, center.X - sz, center.Y, center.X + sz, center.Y);
                        g.DrawLine(pen, center.X, center.Y - sz, center.X, center.Y + sz);
                    }
                    else if (arrowDirection.HasValue)
                    {
                        DrawArrowColored(g, iconBounds, arrowDirection.Value, iconColor);
                    }
                }
            }
            finally
            {
                buttonPath.Dispose();
            }
        }

        /// <summary>
        /// Creates a rounded rectangle path where each corner can be individually rounded or square.
        /// Used to fill section backgrounds (tab strip, content area) that share a straight interior edge.
        /// </summary>
        private GraphicsPath CreatePartialRoundedPath(Rectangle rect, int radius,
            bool roundTopLeft, bool roundTopRight, bool roundBottomLeft, bool roundBottomRight)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0) return path;

            int r = (radius > 0) ? Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2)) : 0;
            int d = r * 2;

            try
            {
                // Top-left → top-right (top edge)
                if (roundTopLeft && r > 0)
                    path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                else
                    path.AddLine(rect.X, rect.Y, rect.X, rect.Y); // point

                path.AddLine(rect.X + (roundTopLeft ? r : 0), rect.Y,
                             rect.Right - (roundTopRight ? r : 0), rect.Y);

                // Top-right corner
                if (roundTopRight && r > 0)
                    path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);

                // Right edge
                path.AddLine(rect.Right, rect.Y + (roundTopRight ? r : 0),
                             rect.Right, rect.Bottom - (roundBottomRight ? r : 0));

                // Bottom-right corner
                if (roundBottomRight && r > 0)
                    path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);

                // Bottom edge
                path.AddLine(rect.Right - (roundBottomRight ? r : 0), rect.Bottom,
                             rect.X + (roundBottomLeft ? r : 0), rect.Bottom);

                // Bottom-left corner
                if (roundBottomLeft && r > 0)
                    path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);

                // Left edge back to start
                path.AddLine(rect.X, rect.Bottom - (roundBottomLeft ? r : 0),
                             rect.X, rect.Y + (roundTopLeft ? r : 0));

                path.CloseFigure();
            }
            catch
            {
                path.Reset();
                path.AddRectangle(rect);
            }

            return path;
        }

        private void DrawArrow(Graphics g, Rectangle bounds, ArrowDirection direction)
        {
            DrawArrowColored(g, bounds, direction, _tabForeColor);
        }

        /// <summary>Draws a solid filled arrow in the specified <paramref name="color"/>.</summary>
        private void DrawArrowColored(Graphics g, Rectangle bounds, ArrowDirection direction, Color color)
        {
            var center = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
            var size   = Math.Min(bounds.Width, bounds.Height) / 3;
            var points = new PointF[3];

            switch (direction)
            {
                case ArrowDirection.Left:
                    points[0] = new PointF(center.X - size / 2, center.Y);
                    points[1] = new PointF(center.X + size / 2, center.Y - size);
                    points[2] = new PointF(center.X + size / 2, center.Y + size);
                    break;
                case ArrowDirection.Right:
                    points[0] = new PointF(center.X + size / 2, center.Y);
                    points[1] = new PointF(center.X - size / 2, center.Y - size);
                    points[2] = new PointF(center.X - size / 2, center.Y + size);
                    break;
                case ArrowDirection.Up:
                    points[0] = new PointF(center.X, center.Y - size / 2);
                    points[1] = new PointF(center.X - size, center.Y + size / 2);
                    points[2] = new PointF(center.X + size, center.Y + size / 2);
                    break;
                case ArrowDirection.Down:
                    points[0] = new PointF(center.X, center.Y + size / 2);
                    points[1] = new PointF(center.X - size, center.Y - size / 2);
                    points[2] = new PointF(center.X + size, center.Y - size / 2);
                    break;
            }

            using (var brush = new SolidBrush(color))
            {
                g.FillPolygon(brush, points);
            }
        }

        // =====================================================================
        #region Drag-to-Reorder Painting  (P4 Enhancement 5)
        // =====================================================================

        /// <summary>
        /// Paints a semi-transparent "ghost" copy of the tab being dragged,
        /// centred on the current cursor position.
        /// </summary>
        private void DrawDragGhost(Graphics g, AddinTab tab)
        {
            if (tab.Bounds.IsEmpty || _dragGhostLoc.IsEmpty) return;

            // Offset ghost so it follows the cursor centred on the tab.
            int offsetX = _dragGhostLoc.X - tab.Bounds.Left - tab.Bounds.Width  / 2;
            int offsetY = _dragGhostLoc.Y - tab.Bounds.Top  - tab.Bounds.Height / 2;
            Rectangle ghost = new Rectangle(
                tab.Bounds.X + offsetX,
                tab.Bounds.Y + offsetY,
                tab.Bounds.Width,
                tab.Bounds.Height);

            // Save graphics state and apply 60 % alpha fade via a colour matrix.
            var state = g.Save();
            try
            {
                float[][] matrix = {
                    new float[] {1, 0, 0, 0,    0},
                    new float[] {0, 1, 0, 0,    0},
                    new float[] {0, 0, 1, 0,    0},
                    new float[] {0, 0, 0, 0.55f,0},
                    new float[] {0, 0, 0, 0,    1},
                };
                var cm = new System.Drawing.Imaging.ColorMatrix(matrix);
                var ia = new System.Drawing.Imaging.ImageAttributes();
                ia.SetColorMatrix(cm);

                // Render the ghost tab into a temporary bitmap then blit with fade.
                using (var bmp = new System.Drawing.Bitmap(tab.Bounds.Width, tab.Bounds.Height))
                using (var bg = System.Drawing.Graphics.FromImage(bmp))
                {
                    bg.SmoothingMode = g.SmoothingMode;
                    bg.TextRenderingHint = g.TextRenderingHint;

                    // Shift the tab's own bounds to (0,0) in the temp bitmap.
                    var savedBounds = tab.Bounds;
                    tab.Bounds = new Rectangle(0, 0, savedBounds.Width, savedBounds.Height);
                    DrawTab(bg, tab);
                    tab.Bounds = savedBounds;

                    g.DrawImage(bmp,
                        new Rectangle(ghost.X, ghost.Y, ghost.Width, ghost.Height),
                        0, 0, bmp.Width, bmp.Height,
                        GraphicsUnit.Pixel, ia);
                }

                // Draw a thin accent border around the ghost to make it obvious.
                Color borderCol = _currentTheme?.ActiveBorderColor ?? ColorUtils.MapSystemColor(SystemColors.Highlight);
                using (var pen = new Pen(Color.FromArgb(160, borderCol), 1.5f))
                {
                    int r = DpiScalingHelper.ScaleValue(4, this);
                    using (var path = CreatePartialRoundedPath(ghost, r, true, true, true, true))
                        g.DrawPath(pen, path);
                }
            }
            finally
            {
                g.Restore(state);
            }
        }

        /// <summary>
        /// Paints a 2 px insertion bar showing where the dragged tab will be dropped.
        /// Pulses in opacity and thickness for visual emphasis.
        /// </summary>
        private void DrawDropIndicator(Graphics g)
        {
            if (_dropInsertIndex < 0 || _tabs == null) return;
            if (_tabs.Count == 0) return;

            bool vertical = _tabPosition == TabPosition.Left || _tabPosition == TabPosition.Right;
            Color indColor = _currentTheme?.ActiveBorderColor ?? ColorUtils.MapSystemColor(SystemColors.Highlight);

            // Pulse: sine wave modulates alpha (140..255) and thickness (2..3.5)
            float pulse = _dropIndicatorPulse > 0f
                ? (MathF.Sin(_dropIndicatorPulse) * 0.5f + 0.5f)
                : 1f;
            int alpha = (int)(140 + 115 * pulse);
            float penWidth = 2f + 1.5f * pulse;

            int lineX, lineY, lineW, lineH;

            if (_dropInsertIndex >= _tabs.Count)
            {
                // Append after the last visible tab.
                var last = _tabs.LastOrDefault(t => !t.Bounds.IsEmpty);
                if (last == null) return;
                if (vertical)
                {
                    lineX = last.Bounds.X;
                    lineY = last.Bounds.Bottom - 1;
                    lineW = last.Bounds.Width;
                    lineH = 2;
                }
                else
                {
                    lineX = last.Bounds.Right - 1;
                    lineY = last.Bounds.Y;
                    lineW = 2;
                    lineH = last.Bounds.Height;
                }
            }
            else
            {
                var target = _tabs[_dropInsertIndex];
                if (target.Bounds.IsEmpty) return;
                if (vertical)
                {
                    lineX = target.Bounds.X;
                    lineY = target.Bounds.Top;
                    lineW = target.Bounds.Width;
                    lineH = 2;
                }
                else
                {
                    lineX = target.Bounds.Left;
                    lineY = target.Bounds.Y;
                    lineW = 2;
                    lineH = target.Bounds.Height;
                }
            }

            var pulseColor = Color.FromArgb(alpha, indColor);
            using (var pen = new Pen(pulseColor, penWidth))
            {
                // Draw a small triangle "cap" + a vertical/horizontal line.
                if (vertical)
                {
                    g.DrawLine(pen, lineX, lineY, lineX + lineW, lineY);
                    // Small triangle caps on the left edge.
                    PointF[] tri = {
                        new PointF(lineX - 5, lineY - 4),
                        new PointF(lineX,     lineY),
                        new PointF(lineX - 5, lineY + 4)
                    };
                    using (var b = new SolidBrush(pulseColor)) g.FillPolygon(b, tri);
                }
                else
                {
                    g.DrawLine(pen, lineX, lineY, lineX, lineY + lineH);
                    // Small triangle caps on the top edge.
                    PointF[] tri = {
                        new PointF(lineX - 4, lineY - 5),
                        new PointF(lineX,     lineY),
                        new PointF(lineX + 4, lineY - 5)
                    };
                    using (var b = new SolidBrush(pulseColor)) g.FillPolygon(b, tri);
                }
            }
        }

        #endregion

        private static Color ShiftLuminance(Color color, float amount)
        {
            float h, s, l;
            ColorToHsl(color, out h, out s, out l);
            l = Math.Max(0, Math.Min(1, l + amount));
            return ColorFromHsl(h, s, l);
        }

        private static void ColorToHsl(Color color, out float h, out float s, out float l)
        {
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;
            float min = Math.Min(r, Math.Min(g, b));
            float max = Math.Max(r, Math.Max(g, b));
            float delta = max - min;
            l = (max + min) / 2.0f;
            if (delta == 0) { h = 0; s = 0; }
            else
            {
                s = l < 0.5f ? delta / (max + min) : delta / (2.0f - max - min);
                if (r == max) h = (g - b) / delta;
                else if (g == max) h = 2.0f + (b - r) / delta;
                else h = 4.0f + (r - g) / delta;
                h /= 6.0f;
                if (h < 0) h += 1.0f;
            }
        }

        private static Color ColorFromHsl(float h, float s, float l)
        {
            float r, g, b;
            if (s == 0) { r = g = b = l; }
            else
            {
                float q = l < 0.5f ? l * (1.0f + s) : l + s - l * s;
                float p = 2.0f * l - q;
                r = HueToRgb(p, q, h + 1.0f / 3.0f);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1.0f / 3.0f);
            }
            return Color.FromArgb(255, Math.Max(0, Math.Min(255, (int)(r * 255))), Math.Max(0, Math.Min(255, (int)(g * 255))), Math.Max(0, Math.Min(255, (int)(b * 255))));
        }

        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0) t += 1.0f;
            if (t > 1) t -= 1.0f;
            if (t < 1.0f / 6.0f) return p + (q - p) * 6.0f * t;
            if (t < 1.0f / 2.0f) return q;
            if (t < 2.0f / 3.0f) return p + (q - p) * (2.0f / 3.0f - t) * 6.0f;
            return p;
        }

        #endregion
    }
}

