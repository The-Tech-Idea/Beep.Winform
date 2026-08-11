using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Helpers;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Models;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Base implementation for list box painters
    /// Provides common functionality for all variants
    /// </summary>
    internal abstract class BaseListBoxPainter : IListBoxPainter
    {
    protected BeepListBox _owner;
    protected IBeepTheme _theme;
    protected BeepListBoxHelper _helper;
    protected BeepListBoxLayoutHelper _layout;
    public BeepControlStyle Style { get; set; } = BeepControlStyle.Minimal;

    public Font TextFont { get; set; }

    // ── Cached GDI resources (Phase 9) ───────────────────────────────────────
    // All fonts here come from the theme manager (shared cache) and MUST NOT be
    // disposed by this painter. Brushes/pens are painter-owned single-color
    // instances reused across rows to avoid per-row allocation.
    private readonly System.Collections.Generic.Dictionary<(float size, FontStyle style), Font> _fontCache = new();
    private readonly System.Collections.Generic.Dictionary<int, SolidBrush> _brushCache = new();
    private readonly System.Collections.Generic.Dictionary<(int argb, float width), Pen> _penCache = new();

    public virtual void Initialize(BeepListBox owner, IBeepTheme theme)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _theme = theme;
        _helper = owner.Helper;
        _layout = owner.LayoutHelper;
        TextFont = owner.ListBoxTextFont ?? owner.Font;
        _fontCache.Clear(); // font families/sizes are relative to the (possibly new) theme font
    }

    /// <summary>
    /// Returns a cached font in the owner's text family at the given point size + style.
    /// Resolved via the theme manager (shared cache — never disposed here). This is the
    /// sanctioned replacement for direct font-manager / raw-font allocations in paint paths.
    /// </summary>
    protected Font GetCachedFont(float size, FontStyle style = FontStyle.Regular)
    {
        size = Math.Max(6f, size);
        var key = (size, style);
        if (_fontCache.TryGetValue(key, out var f) && f != null)
            return f;

        string family = (TextFont ?? _owner?.TextFont ?? SystemFonts.DefaultFont).FontFamily.Name;
        var weight = style.HasFlag(FontStyle.Bold) ? FontWeight.Bold : FontWeight.Normal;
        f = BeepThemesManager.ToFont(family, size, weight, style) ?? SystemFonts.DefaultFont;
        _fontCache[key] = f;
        return f;
    }

    /// <summary>Font one point smaller than the base text font — the trailing-metadata role.</summary>
    private Font GetTrailingMetaFont()
    {
        float baseSize = (TextFont ?? _owner?.TextFont ?? SystemFonts.DefaultFont).Size;
        return GetCachedFont(Math.Max(8f, baseSize - 1f));
    }

    /// <summary>Returns a cached solid brush for the given color (mutated in place, never disposed here).</summary>
    protected SolidBrush GetBrush(Color color)
    {
        int argb = color.ToArgb();
        if (_brushCache.TryGetValue(argb, out var b) && b != null)
            return b;
        b = new SolidBrush(color);
        _brushCache[argb] = b;
        return b;
    }

    /// <summary>Returns a cached pen for the given color/width (never disposed here).</summary>
    protected Pen GetPen(Color color, float width = 1f)
    {
        width = Math.Max(0.5f, width);
        var key = (color.ToArgb(), width);
        if (_penCache.TryGetValue(key, out var p) && p != null)
            return p;
        p = new Pen(color, width);
        _penCache[key] = p;
        return p;
    }
        
        public virtual void Paint(Graphics g, BeepListBox owner, Rectangle drawingRect)
        {
            if (g == null || owner == null || drawingRect.Width <= 0 || drawingRect.Height <= 0)
                return;
            
            _owner = owner;
            _helper = owner.Helper;
            _layout = owner.LayoutHelper;
            // Use the CurrentTheme property instead of MenuStyle string property
            _theme = owner._currentTheme;
            
            // Set high-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Clear the drawing area with background color
            g.FillRectangle(GetBrush(_theme?.BackgroundColor ?? ColorUtils.MapSystemColor(SystemColors.Window)), drawingRect);
            
            // Get layout and items
            var items = _helper.GetVisibleItems();
            var cache = _layout.GetCachedLayout();
            if ((cache == null || cache.Count == 0) && items != null && items.Count > 0)
            {
                _layout.CalculateLayout(_owner);
            }

            // Optionally draw search at top (kept simple)
            int yOffset = drawingRect.Y;
            if (_owner.ShowSearch && SupportsSearch())
            {
                yOffset = DrawSearchArea(g, drawingRect, yOffset);
            }

            // If no items, draw empty state if enabled
            if ((items == null || items.Count == 0) && _owner.ShowEmptyState)
            {
                DrawEmptyState(g, drawingRect, yOffset);
                return;
            }

            DrawItems(g, drawingRect, items, yOffset);
        }

        protected virtual void DrawEmptyState(Graphics g, Rectangle drawingRect, int yOffset)
        {
            int v12 = DpiScalingHelper.ScaleValue(12, _owner);
            int v8 = DpiScalingHelper.ScaleValue(8, _owner);
            int v36 = DpiScalingHelper.ScaleValue(36, _owner);
            int v16 = DpiScalingHelper.ScaleValue(16, _owner);
            var rect = new Rectangle(drawingRect.Left, yOffset + v12, drawingRect.Width, drawingRect.Height - (yOffset - drawingRect.Top) - v12);
            string text = !string.IsNullOrEmpty(_owner.EmptyStateText) ? _owner.EmptyStateText : "No items";

            // Small icon or circle
            Rectangle iconRect = new Rectangle(rect.Left + (rect.Width - v36) / 2, rect.Top + v8, v36, v36);
            using (var brush = new SolidBrush(PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.PrimaryColor ?? Color.Empty, 40)))
            {
                g.FillEllipse(brush, iconRect);
            }

            // Draw text (use TextFont from theme, fallback to a cached derived font)
            var textRect = new Rectangle(rect.Left + v8, iconRect.Bottom + v8, rect.Width - v16, v36);
            var fontToUse = TextFont ?? GetCachedFont(Math.Max(10f, _owner.TextFont.Size - 1f));
            TextRenderer.DrawText(g, text, fontToUse, textRect,
                Theme.ListForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordEllipsis | TextFormatFlags.NoPrefix);
        }
        
        public virtual int GetPreferredItemHeight()
        {
            if (_owner == null)
                return ListBoxTokens.ItemHeightComfortable;

            int tokenHeight = _owner.Density switch
            {
                ListDensityMode.Dense   => ListBoxTokens.ItemHeightDense,
                ListDensityMode.Compact => ListBoxTokens.ItemHeightCompact,
                _                       => ListBoxTokens.ItemHeightComfortable
            };
            return DpiScalingHelper.ScaleValue(tokenHeight, _owner);
        }
        
        public virtual Padding GetPreferredPadding()
        {
            if (_owner == null)
                return new Padding(ListBoxTokens.ItemPaddingH, ListBoxTokens.ItemPaddingV,
                                   ListBoxTokens.ItemPaddingH, ListBoxTokens.ItemPaddingV);

            int h = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingH, _owner);
            int v = DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingV, _owner);
            return new Padding(h, v, h, v);
        }
        
        public virtual bool SupportsSearch()
        {
            return true;
        }
        
        public virtual bool SupportsCheckboxes()
        {
            return true;
        }
        
        #region Abstract Methods
        
        /// <summary>
      
        #endregion
        
        #region Common Drawing Methods
        
        protected virtual int DrawSearchArea(Graphics g, Rectangle drawingRect, int yOffset)
        {
            // The real BeepTextBox control handles search UI rendering.
            // This method only computes the Y offset to leave space for it.
            int searchHeight = DpiScalingHelper.ScaleValue(ListBoxTokens.SearchBarHeight, _owner);
            return yOffset + searchHeight + DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingV, _owner);
        }
        
        protected virtual void DrawItems(Graphics g, Rectangle drawingRect, System.Collections.Generic.List<SimpleItem> items, int yOffset)
        {
            if (items == null || items.Count == 0)
                return;

            var layout = _layout.GetCachedLayout();
            if (layout == null || layout.Count == 0)
                return;

            Point mousePoint = _owner.PointToClient(Control.MousePosition);

            foreach (var info in layout)
            {
                var item = info.Item;
                var rowRect = info.RowRect;
                if (rowRect.IsEmpty) continue;

                if (item is BeepListItem groupHeader && groupHeader.IsGroupHeader)
                {
                    bool isCollapsed = _owner.IsGroupCollapsed(groupHeader.Category ?? groupHeader.Text ?? string.Empty);
                    int count = Math.Max(0, groupHeader.GroupItemCount);
                    DrawGroupHeader(g, _owner, rowRect, groupHeader.Category ?? groupHeader.Text ?? string.Empty, isCollapsed, count);
                    continue;
                }

                // ── Separator row ────────────────────────────────────────────────
                var richItem = item as BeepListItem;
                if (richItem?.IsSeparator == true)
                {
                    DrawSeparatorRow(g, rowRect, richItem.Text);
                    continue;
                }

                bool isDisabled  = richItem?.IsDisabled == true;
                bool isHovered   = !isDisabled && rowRect.Contains(mousePoint);
                bool isSelected  = !isDisabled && _owner.IsItemSelected(item);

                // Save graphics state to apply clipping
                var graphicsState = g.Save();
                try
                {
                    // Set clipping region to prevent item from drawing outside its bounds
                    g.SetClip(rowRect, System.Drawing.Drawing2D.CombineMode.Replace);

                    // ── Left accent bar ──────────────────────────────────────────
                    if (richItem?.ItemAccentColor != Color.Empty && richItem?.ItemAccentColor != null)
                        DrawAccentBar(g, rowRect, richItem.ItemAccentColor);

                    // ── Hierarchy chevron ─────────────────────────────────────────
                    if (info.HasChildren && !info.ChevronRect.IsEmpty)
                        DrawChevron(g, info.ChevronRect, item.IsExpanded);

                    // Let concrete painter draw using the row rect
                    DrawItem(g, rowRect, item, isHovered, isSelected);

                    // ── HC focus ring ────────────────────────────────────────────
                    if (isSelected || _owner.FocusedIndex >= 0)
                    {
                        var visible = _helper?.GetVisibleItems();
                        if (visible != null)
                        {
                            int fi = _owner.FocusedIndex;
                            if (fi >= 0 && fi < visible.Count && visible[fi] == item)
                                _owner.PaintFocusRectIfHC(g, rowRect);
                        }
                    }
                }
                finally
                {
                    // Restore graphics state (removes clipping)
                    g.Restore(graphicsState);
                }
            }

            // The "PgUp / PgDn" hint that used to be drawn here is gone. It was painted at the
            // bottom-right of the drawing rect with no space reserved for it and no background,
            // directly on top of the last visible row - so in every style whose content overflows
            // (about twenty of the forty-three) it sat across the last item's text. A scrollbar
            // already says there is more to see, and it says it without defacing a row.
        }

        protected virtual void DrawItemText(Graphics g, Rectangle textRect, string text, Color textColor, Font font)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (_owner.HighlightSearchMatches &&
                _owner.ShowSearch &&
                !string.IsNullOrWhiteSpace(_owner.SearchText))
            {
                Color hlBack = PathPainterHelpers.WithAlphaIfNotEmpty(Theme.PrimaryColor, 45);
                Color hlFore = _theme?.OnPrimaryColor ?? textColor;
                _owner.DrawHighlightedText(g, text, _owner.SearchText, textRect, font, textColor, hlFore, hlBack);
                return;
            }

            TextFormatFlags flags = TextFormatFlags.Left |
                                    TextFormatFlags.VerticalCenter |
                                    TextFormatFlags.EndEllipsis |
                                    TextFormatFlags.NoPrefix;
            TextRenderer.DrawText(g, text, font, textRect, textColor, flags);
        }
        
        protected virtual void DrawItemImage(Graphics g, Rectangle imageRect, string imagePath)
        {
            if (imageRect.IsEmpty || string.IsNullOrEmpty(imagePath))
                return;
            
            try
            {
                
                StyledImagePainter.Paint(g, imageRect, imagePath, Style);
            }
            catch
            {
                // Fallback: draw a placeholder (translucent gray dot — intentional alpha overlay)
                int inflate = DpiScalingHelper.ScaleValue(4, _owner);
                var smallRect = imageRect;
                smallRect.Inflate(-inflate, -inflate);
                g.FillEllipse(GetBrush(Color.FromArgb(150, Theme.ShadowColor)), smallRect);
            }
        }
        
        protected virtual void DrawCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, bool isHovered)
        {
            // Theme-driven colors with fallback chains (Phase 9: no hardcoded RGB)
            Color bgColor = isHovered
                ? (Theme.ListItemHoverBackColor)
                : (Theme.ListBackColor);
            Color accent = Theme.PrimaryColor;
            Color borderColor = isChecked ? accent : (Theme.BorderColor);

            // High-contrast override
            if (_owner != null && _owner.IsHighContrast)
            {
                bgColor = ColorUtils.MapSystemColor(SystemColors.Window);
                borderColor = ColorUtils.MapSystemColor(SystemColors.WindowText);
                accent = ColorUtils.MapSystemColor(SystemColors.Highlight);
            }

            g.FillRectangle(GetBrush(bgColor), checkboxRect);
            g.DrawRectangle(GetPen(borderColor, 1.5f), checkboxRect.X, checkboxRect.Y, checkboxRect.Width - 1, checkboxRect.Height - 1);

            // Draw checkmark if checked
            if (isChecked)
            {
                int ck3 = DpiScalingHelper.ScaleValue(3, _owner);
                int ck4 = DpiScalingHelper.ScaleValue(4, _owner);
                Point[] checkPoints =
                {
                    new Point(checkboxRect.Left + ck3, checkboxRect.Top + checkboxRect.Height / 2),
                    new Point(checkboxRect.Left + checkboxRect.Width / 2 - 1, checkboxRect.Bottom - ck4),
                    new Point(checkboxRect.Right - ck3, checkboxRect.Top + ck3)
                };
                g.DrawLines(GetPen(accent, 2f), checkPoints);
            }
        }
        
        protected virtual void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            float hoverProgress = isHovered ? 1f : 0f;

            Color backgroundColor = Theme.ListBackColor;

            if (isSelected)
            {
                // The list's own selected slots, not PrimaryColor. A theme sets these separately
                // precisely so a selected row need not be the brand colour.
                backgroundColor = Theme.ListItemSelectedBackColor;
                g.DrawRectangle(GetPen(Theme.ListItemSelectedBorderColor, 1.5f),
                    itemRect.X + 1, itemRect.Y + 1, itemRect.Width - 2, itemRect.Height - 2);
            }
            else if (hoverProgress > 0f)
            {
                Color hoverColor = Theme.ListItemHoverBackColor;
                backgroundColor = BlendColors(backgroundColor, hoverColor, hoverProgress);
            }

            g.FillRectangle(GetBrush(backgroundColor), itemRect);
        }

        /// <summary>
        /// Extended DrawItemBackground with access to item context. Backwards-compatible: default calls DrawItemBackground.
        /// Painters can override DrawItemBackground or override this Ex method for better control.
        /// </summary>
        protected virtual void DrawItemBackgroundEx(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty)
                return;

            // CRITICAL: Always clear the specific item's background first to prevent overlap artifacts
            // This ensures that any previous painting in this rect is completely overwritten
            g.FillRectangle(GetBrush(Theme.ListBackColor), itemRect);

            // Allow painter-specific background customization via DrawItemBackground override.
            // Called BEFORE selection/hover/focus overlays so the painter provides the base
            // canvas and overlays are drawn on top (prevents painters from overwriting focus rings).
            DrawItemBackground(g, itemRect, isHovered, isSelected);

            // Compute hover progress using owner helper if available
            float hoverProgress = 0f;
            try
            {
                hoverProgress = (_owner != null && item != null) ? _owner.GetHoverProgress(item) : 0f;
            }
            catch { hoverProgress = 0f; }

            // If selected, ensure a selection border/overlay is present
            if (isSelected)
            {
                // Use owner-defined selection color or fallback to theme
                var selColor = Theme.PrimaryColor;
                int alpha = Math.Max(0, Math.Min(255, _owner.SelectionOverlayAlpha > 0 ? _owner.SelectionOverlayAlpha : 90));
                g.FillRectangle(GetBrush(Color.FromArgb(alpha, selColor.R, selColor.G, selColor.B)), itemRect);

                // Draw selection border
                var borderColor = Theme.AccentColor;
                int borderThickness = Math.Max(1, _owner.SelectionBorderThickness);
                g.DrawRectangle(GetPen(borderColor, borderThickness), itemRect.X + 1, itemRect.Y + 1, itemRect.Width - 2, itemRect.Height - 2);

                // Focus outline for focused item
                if (_owner.Focused && _owner.SelectedItem == item)
                {
                    var focusColor = Theme.PrimaryColor;
                    int focusThickness = Math.Max(1, _owner.FocusOutlineThickness);
                    g.DrawRectangle(GetPen(focusColor, focusThickness), itemRect.X + 2, itemRect.Y + 2, itemRect.Width - 4, itemRect.Height - 4);
                }
            }
            else if (hoverProgress > 0f)
            {
                // Only apply hover overlay if not selected
                var hoverColor = Theme.ListItemHoverBackColor;
                var overlayColor = BlendColors(Color.Transparent, hoverColor, hoverProgress);
                g.FillRectangle(GetBrush(Color.FromArgb((int)(hoverProgress * 60), overlayColor.R, overlayColor.G, overlayColor.B)), itemRect);
            }

            // ── High-contrast override ───────────────────────────────────────────
            if (_owner.IsHighContrast)
            {
                Color hcBg = _owner.HCItemBackground(isHovered, isSelected);
                if (hcBg != Color.Empty)
                {
                    g.FillRectangle(GetBrush(hcBg), itemRect);
                }
                Color hcBorder = _owner.HCBorderColor;
                if (isSelected && hcBorder != Color.Empty)
                {
                    g.DrawRectangle(GetPen(hcBorder, 2f), itemRect.X + 1, itemRect.Y + 1, itemRect.Width - 2, itemRect.Height - 2);
                }
            }

            // ── Disabled dimming overlay ─────────────────────────────────────────
            if (item is BeepListItem ri && ri.IsDisabled)
            {
                var bg = _theme?.BackgroundColor ?? ColorUtils.MapSystemColor(SystemColors.Window);
                g.FillRectangle(GetBrush(Color.FromArgb(ListBoxTokens.DisabledAlpha, bg)), itemRect);
            }
        }

        /// <summary>
        /// Blends two colors by amount t (0..1)
        /// </summary>
        protected Color BlendColors(Color a, Color b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            int r = (int)(a.R + (b.R - a.R) * t);
            int g = (int)(a.G + (b.G - a.G) * t);
            int bl = (int)(a.B + (b.B - a.B) * t);
            int alpha = (int)(a.A + (b.A - a.A) * t);
            return Color.FromArgb(alpha, r, g, bl);
        }

        protected virtual void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // Draw background - use extended method by default
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            // Calculate layout
            var padding = GetPreferredPadding();
            var contentRect = Rectangle.Inflate(itemRect, -padding.Left, -padding.Top);

            // Draw image if available
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int imgSz = DpiScalingHelper.ScaleValue(_owner.ImageSize, _owner);
                int imgGap = imgSz + DpiScalingHelper.ScaleValue(ListBoxTokens.IconTextGap, _owner);
                int imgY = contentRect.Y + (contentRect.Height - imgSz) / 2;
                var imageRect = new Rectangle(contentRect.X, imgY, imgSz, imgSz);
                DrawItemImage(g, imageRect, item.ImagePath);
                contentRect.X += imgGap;
                contentRect.Width -= imgGap;
            }

            // Draw trailing metadata first (reserving right-side space)
            int trailingReservation = 0;
            if (item is BeepListItem rich && !string.IsNullOrWhiteSpace(rich.TrailingMeta))
            {
                var metricFont = GetTrailingMetaFont();
                var metricSize = TextRenderer.MeasureText(rich.TrailingMeta, metricFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                int metricW = metricSize.Width + DpiScalingHelper.ScaleValue(8, _owner);
                var metricRect = new Rectangle(
                    contentRect.Right - metricW,
                    contentRect.Top,
                    metricW,
                    contentRect.Height);
                TextRenderer.DrawText(
                    g,
                    rich.TrailingMeta,
                    metricFont,
                    metricRect,
                    Color.FromArgb(ListBoxTokens.SubTextAlpha, Theme.ListItemForeColor),
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                trailingReservation = metricW + DpiScalingHelper.ScaleValue(4, _owner);
            }

            // Draw text (use TextFont from theme)
            Color textColor = isSelected ? (Theme.OnPrimaryColor) : (Theme.ListItemForeColor);
            var primaryRect = new Rectangle(contentRect.Left, contentRect.Top, Math.Max(0, contentRect.Width - trailingReservation), contentRect.Height);

            bool hasSubText = item is BeepListItem richItem2 &&
                !string.IsNullOrWhiteSpace(richItem2.SubText) &&
                (richItem2.RowPreset == ListRowPreset.TitleSubtext ||
                 richItem2.RowPreset == ListRowPreset.CheckboxDescription ||
                 richItem2.RowPreset == ListRowPreset.AvatarSecondaryAction ||
                 richItem2.RowPreset == ListRowPreset.Auto);

            if (hasSubText)
            {
                int subGap = DpiScalingHelper.ScaleValue(ListBoxTokens.SubTextGap, _owner);
                int halfHeight = primaryRect.Height / 2;
                var titleRect = new Rectangle(primaryRect.Left, primaryRect.Top, primaryRect.Width, halfHeight);
                DrawItemText(g, titleRect, item.Text, textColor, TextFont ?? _owner.TextFont);
                int subTop = primaryRect.Top + halfHeight + subGap;
                var subRect = new Rectangle(primaryRect.Left, subTop, primaryRect.Width, Math.Max(0, primaryRect.Bottom - subTop));
                DrawSubText(g, subRect, ((BeepListItem)item).SubText, textColor, TextFont ?? _owner.TextFont);
            }
            else
            {
                DrawItemText(g, primaryRect, item.Text, textColor, TextFont ?? _owner.TextFont);
            }
        }

        // ── Sprint 7: IListBoxPainter new contract members ──────────────────────

        /// <inheritdoc/>
        /// Returns a taller height for rich items that have a sub-text line.
        /// <summary>
        /// True when the item carries a second line of text that a painter will draw.
        /// </summary>
        /// <remarks>
        /// One authority so measure and paint cannot key on different properties again.
        /// </remarks>
        /// <summary>
        /// Draws a title with an optional subtitle beneath it, stacked and centred as a block.
        /// </summary>
        /// <remarks>
        /// One implementation because there were four, all with the same defect: the title was
        /// drawn into the FULL row height with <c>VerticalCenter</c> and the subtitle into the
        /// bottom half, so the two overlapped and the subtitle's half-a-row was smaller than its
        /// own font needed - every subtitle came out sliced through horizontally.
        /// <para>
        /// Heights come from the fonts. If the row is too short for both lines the top edge wins,
        /// so the title stays whole rather than both lines being half-cut.
        /// </para>
        /// </remarks>
        protected void DrawTitleAndSubtitle(
            Graphics g, Rectangle bounds, string title, string subtitle,
            Color titleColor, Color subtitleColor, Font titleFont, Font subtitleFont)
        {
            if (g == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            const TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.Top
                                        | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis;

            bool hasSub = !string.IsNullOrWhiteSpace(subtitle) && subtitleFont != null;
            int titleH  = titleFont?.Height ?? 0;
            int subH    = hasSub ? subtitleFont.Height : 0;
            int gap     = hasSub ? DpiScalingHelper.ScaleValue(1, _owner) : 0;
            int top     = bounds.Y + Math.Max(0, (bounds.Height - (titleH + gap + subH)) / 2);

            if (!string.IsNullOrEmpty(title))
                TextRenderer.DrawText(g, title, titleFont,
                    new Rectangle(bounds.X, top, bounds.Width, titleH), titleColor, flags);

            if (hasSub)
                TextRenderer.DrawText(g, subtitle, subtitleFont,
                    new Rectangle(bounds.X, top + titleH + gap, bounds.Width, subH), subtitleColor, flags);
        }

        /// <summary>The theme in force. There is always one.</summary>
        /// <remarks>
        /// Replaces the `_theme?.Slot ?? Color.Something` pattern that appeared 200+ times across
        /// these painters. That form silently substituted a literal for the whole palette whenever
        /// the field was null, and the literal was usually a light-theme grey - so a dark theme
        /// rendered light patches. The slots themselves are real; only the fallback was wrong.
        /// </remarks>
        protected IBeepTheme Theme => _theme ?? BeepThemesManager.CurrentTheme;

        protected static bool HasSecondLine(object item)
        {
            if (item is BeepListItem rich && !string.IsNullOrEmpty(rich.SubText)) return true;
            return item is SimpleItem si && !string.IsNullOrWhiteSpace(si.Description);
        }

        public virtual int GetItemHeight(BeepListBox owner, object item)
        {
            if (item is BeepListItem sep && sep.IsSeparator)
                return DpiScalingHelper.ScaleValue(ListBoxTokens.SeparatorHeight, owner ?? _owner);

            // A second line means EITHER property, because the measure and the paint disagreed
            // about which one carries it: this method asked for BeepListItem.SubText - assigned in
            // exactly two places in the whole repo - while nine painters draw
            // SimpleItem.Description. Any list built the ordinary way therefore measured one line
            // and drew two, and the subtitle was sliced through by the row boundary.
            if (HasSecondLine(item))
            {
                int sub = DpiScalingHelper.ScaleValue(18, owner ?? _owner);
                return GetPreferredItemHeight() + sub;
            }

            return GetPreferredItemHeight();
        }

        /// <inheritdoc/>
        public virtual void DrawGroupHeader(
            Graphics g, BeepListBox owner, Rectangle headerRect,
            string groupKey, bool isCollapsed, int itemCount)
        {
            if (g == null || headerRect.IsEmpty) return;
            Color accent = Theme.PrimaryColor;
            Color foreColor = Theme.ListForeColor;

            // Background + bottom border line (alpha overlays of the theme accent)
            g.FillRectangle(GetBrush(Color.FromArgb(30, accent)), headerRect);
            g.DrawLine(GetPen(Color.FromArgb(40, accent), 1f), headerRect.Left, headerRect.Bottom - 1, headerRect.Right, headerRect.Bottom - 1);

            int pad     = DpiScalingHelper.ScaleValue(8, owner);
            int chevron = DpiScalingHelper.ScaleValue(10, owner);

            // Chevron glyph: ▶ collapsed, ▼ expanded
            string ch = isCollapsed ? "▶" : "▼";
            var chFont = GetCachedFont(7f);
            var chRect = new Rectangle(headerRect.Left + pad, headerRect.Top, chevron, headerRect.Height);
            TextRenderer.DrawText(g, ch, chFont, chRect, foreColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            // Group label + count
            string label = $"{groupKey}  ({itemCount})";
            var lFont = GetCachedFont(Math.Max(8f, owner.Font.Size - 1f), FontStyle.Bold);
            var lRect = new Rectangle(headerRect.Left + pad + chevron + 4, headerRect.Top,
                                      headerRect.Width - pad * 2 - chevron - 4, headerRect.Height);
            TextRenderer.DrawText(g, label, lFont, lRect, foreColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        // ── Sprint 7: shared decoration helpers ─────────────────────────────────

        /// <summary>
        /// Draws a 3 px left-edge accent bar using <paramref name="accentColor"/>.
        /// Painters should call this before rendering item text.
        /// </summary>
        protected void DrawAccentBar(Graphics g, Rectangle rowRect, Color accentColor)
        {
            if (accentColor == Color.Empty || accentColor.A == 0) return;
            int w = DpiScalingHelper.ScaleValue(ListBoxTokens.PinnedAccentBarWidth, _owner);
            g.FillRectangle(GetBrush(accentColor), rowRect.X, rowRect.Y, w, rowRect.Height);
        }

        /// <summary>
        /// Draws an expand/collapse chevron (▶ or ▼) for tree items.
        /// </summary>
        protected virtual void DrawChevron(Graphics g, Rectangle rect, bool isExpanded)
        {
            int cx = rect.X + rect.Width / 2;
            int cy = rect.Y + rect.Height / 2;
            int size = Math.Min(rect.Width, rect.Height) / 2;

            var chevronColor = _helper?.GetTextColor() ?? Theme.SecondaryTextColor;
            var pen = GetPen(Color.FromArgb(ListBoxTokens.SubTextAlpha, chevronColor), 1.5f);

            var oldSmoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            try
            {
                Point[] points;
                if (isExpanded)
                {
                    points = new[]
                    {
                        new Point(cx - size, cy - size / 2),
                        new Point(cx, cy + size / 2),
                        new Point(cx + size, cy - size / 2)
                    };
                }
                else
                {
                    points = new[]
                    {
                        new Point(cx - size / 2, cy - size),
                        new Point(cx + size / 2, cy),
                        new Point(cx - size / 2, cy + size)
                    };
                }
                g.DrawLines(pen, points);
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
            }
        }

        /// <summary>
        /// Draws a badge/count pill in the top-right corner of the item row.
        /// </summary>
        protected void DrawBadgePill(Graphics g, Rectangle rowRect, string badgeText, Color badgeColor)
        {
            if (string.IsNullOrEmpty(badgeText)) return;

            var badgeFont = GetCachedFont(Math.Max(7f, _owner.TextFont.Size - 3f), FontStyle.Bold);
            var textSize = TextRenderer.MeasureText(badgeText, badgeFont);
            int r = DpiScalingHelper.ScaleValue(ListBoxTokens.BadgePillRadius, _owner);
            int pad = DpiScalingHelper.ScaleValue(6, _owner);
            int pillW = Math.Max(r * 2, textSize.Width + pad);
            int pillH = Math.Max(r * 2, textSize.Height + 2);
            int pillX = rowRect.Right  - pillW - DpiScalingHelper.ScaleValue(8, _owner);
            int pillY = rowRect.Top    + (rowRect.Height - pillH) / 2;

            var pillRect = new Rectangle(pillX, pillY, pillW, pillH);

            // Fill pill
            var fill = badgeColor == Color.Empty ? (Theme.PrimaryColor) : badgeColor;
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(pillRect, r))
                g.FillPath(GetBrush(fill), path);

            // Badge text (auto-contrast on the pill fill)
            Color textColor = fill.GetBrightness() > 0.55f ? Theme.ListItemForeColor : Theme.OnPrimaryColor;
            TextRenderer.DrawText(g, badgeText, badgeFont, pillRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        /// <summary>
        /// Draws a secondary sub-text line below the primary text.
        /// </summary>
        protected void DrawSubText(Graphics g, Rectangle subRect, string subText, Color color, Font? ownerFont)
        {
            if (string.IsNullOrEmpty(subText) || subRect.IsEmpty) return;
            float baseSize = (ownerFont ?? _owner.TextFont).Size;
            var subFont = GetCachedFont(Math.Max(7f, baseSize - 1.5f));
            TextRenderer.DrawText(g, subText, subFont, subRect,
                Color.FromArgb(ListBoxTokens.SubTextAlpha, color),
                TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
        }

        /// <summary>
        /// Draws a separator row — a horizontal rule with an optional label centred on it.
        /// </summary>
        protected void DrawSeparatorRow(Graphics g, Rectangle rowRect, string? label = null)
        {
            int midY = rowRect.Top + rowRect.Height / 2;
            var lineColor = Color.FromArgb(60, Theme.ListForeColor);
            var pen = GetPen(lineColor, 1f);

            if (string.IsNullOrEmpty(label))
            {
                g.DrawLine(pen, rowRect.Left + 8, midY, rowRect.Right - 8, midY);
            }
            else
            {
                var lFont = GetCachedFont(Math.Max(7f, _owner.TextFont.Size - 2f));
                var lSize  = TextRenderer.MeasureText(label, lFont);
                int lX     = rowRect.Left + (rowRect.Width - lSize.Width) / 2;
                int gapPad = DpiScalingHelper.ScaleValue(4, _owner);

                g.DrawLine(pen, rowRect.Left + 8, midY, lX - gapPad, midY);
                g.DrawLine(pen, lX + lSize.Width + gapPad, midY, rowRect.Right - 8, midY);
                TextRenderer.DrawText(g, label, lFont,
                    new Rectangle(lX, rowRect.Top, lSize.Width, rowRect.Height),
                    Color.FromArgb(ListBoxTokens.SubTextAlpha, Theme.ListForeColor),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        /// <summary>
        /// Draws a focus ring around the item row (keyboard-navigated item).
        /// Uses HC colours when HC mode is active, otherwise uses <see cref="ListBoxTokens.FocusRingThickness"/>.
        /// </summary>
        protected void DrawFocusRing(Graphics g, Rectangle rowRect)
        {
            Color ringColor;
            int   thickness;

            if (_owner.IsHighContrast)
            {
                ringColor = ColorUtils.MapSystemColor(SystemColors.Highlight);
                thickness = 3;
            }
            else
            {
                ringColor = Theme.PrimaryColor;
                thickness = ListBoxTokens.FocusRingThickness;
            }

            var pen = GetPen(ringColor, thickness);
            g.DrawRectangle(pen,
                rowRect.X + thickness, rowRect.Y + thickness,
                rowRect.Width - thickness * 2 - 1, rowRect.Height - thickness * 2 - 1);
        }

        // ── Rich list shared helpers (Phase 7) ──────────────────────────────────

        /// <summary>Shorthand for DPI scaling.</summary>
        protected int Scale(int value)
            => DpiScalingHelper.ScaleValue(value, _owner);

        /// <summary>Returns 1–2 letter initials from a name string.</summary>
        protected static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return name.Substring(0, Math.Min(2, name.Length)).ToUpper();
        }

        /// <summary>
        /// Draws a circular avatar from ImagePath, or coloured-initials fallback.
        /// </summary>
        protected void DrawCircularAvatar(Graphics g, Rectangle avatarRect, SimpleItem item)
        {
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                float cx = avatarRect.X + avatarRect.Width / 2f;
                float cy = avatarRect.Y + avatarRect.Height / 2f;
                float radius = avatarRect.Width / 2f;
                StyledImagePainter.PaintInCircle(g, cx, cy, radius, item.ImagePath);
            }
            else
            {
                DrawInitialsFallback(g, avatarRect, item.Text ?? item.DisplayField);
            }

            // Subtle translucent-black hairline ring around the avatar (alpha overlay, intentional)
            var pen = GetPen(Color.FromArgb(40, 0, 0, 0), 1.5f);
            g.DrawEllipse(pen, avatarRect);
        }

        /// <summary>
        /// Draws a coloured circle with 1–2 letter initials (reusable fallback).
        /// </summary>
        protected void DrawInitialsFallback(Graphics g, Rectangle rect, string name)
        {
            // Deterministic, but drawn from the THEME's own slots rather than seven hard-coded
            // Material hues. The old palette was labelled "theme-independent", which is exactly
            // the problem: a dark or high-contrast theme still got the same seven light pastels.
            // Still deterministic - the same name always lands on the same slot.
            var t = Theme;
            Color[] palette =
            {
                t.PrimaryColor,
                t.AccentColor,
                t.SuccessColor,
                t.WarningColor,
                t.ErrorColor,
                t.SecondaryColor,
                t.SurfaceColor,
            };
            int idx = Math.Abs((name ?? "").GetHashCode()) % palette.Length;
            g.FillEllipse(GetBrush(palette[idx]), rect);

            string initials = GetInitials(name);
            var font = GetCachedFont(Math.Max(10f, rect.Height * 0.35f), FontStyle.Bold);
            TextRenderer.DrawText(g, initials, font, rect, Theme.OnPrimaryColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        #endregion

    }
}
