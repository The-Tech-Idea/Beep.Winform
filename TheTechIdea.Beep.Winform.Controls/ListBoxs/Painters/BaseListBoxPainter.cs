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

    public virtual void Initialize(BeepListBox owner, IBeepTheme theme)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _theme = theme;
        _helper = new BeepListBoxHelper(owner);
        _layout = owner.LayoutHelper;
        TextFont = owner.ListBoxTextFont ?? owner.Font;
    }
        
        public virtual void Paint(Graphics g, BeepListBox owner, Rectangle drawingRect)
        {
            if (g == null || owner == null || drawingRect.Width <= 0 || drawingRect.Height <= 0)
                return;
            
            _owner = owner;
            // Use the CurrentTheme property instead of MenuStyle string property
            _theme = owner._currentTheme;
            
            // Set high-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Clear the drawing area with background color
            using (var clearBrush = new SolidBrush(_theme?.BackgroundColor ?? Color.White))
            {
                g.FillRectangle(clearBrush, drawingRect);
            }
            
            // Get layout and items
            var items = _helper.GetVisibleItems();
            _layout.CalculateLayout(_owner);
            var cache = _layout.GetCachedLayout();

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

            // Draw text (use TextFont from theme, fallback to _owner.TextFont)
            var textRect = new Rectangle(rect.Left + v8, iconRect.Bottom + v8, rect.Width - v16, v36);
            var fontToUse = TextFont ?? BeepFontManager.GetFont(_owner.TextFont.Name, Math.Max(10, _owner.TextFont.Size - 1f), FontStyle.Regular);
            try
            {
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
                using (var brush = new SolidBrush(_theme?.ListForeColor ?? Color.Gray))
                {
                    g.DrawString(text, fontToUse, brush, textRect, sf);
                }
            }
            finally
            {
                if (fontToUse != TextFont) fontToUse?.Dispose();
            }
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
            return yOffset + searchHeight + DpiScalingHelper.ScaleValue(4, _owner);
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

            // Optionally draw 'Page Up / Page Down' hints if the content is taller than the viewport
            try
            {
                var clientArea = _owner.GetClientArea();
                var ownerVirt = new Size();
                ownerVirt = new Size(_owner.Width, _owner.PreferredItemHeight * items.Count);
                if (clientArea.Height > 0 && ownerVirt.Height > clientArea.Height)
                {
                    int v120 = DpiScalingHelper.ScaleValue(120, _owner);
                    int v26 = DpiScalingHelper.ScaleValue(26, _owner);
                    int v110 = DpiScalingHelper.ScaleValue(110, _owner);
                    int v20 = DpiScalingHelper.ScaleValue(20, _owner);
                    var hintFont = TextFont ?? BeepFontManager.GetFont(_owner.TextFont.Name, Math.Max(8, _owner.TextFont.Size - 2), FontStyle.Regular);
                    try
                    {
                        using (var sf = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far })
                        using (var brush = new SolidBrush(PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.ListForeColor ?? Color.Empty, 140)))
                        {
                            var hint = "PgUp / PgDn";
                            var hintRect = new Rectangle(drawingRect.Right - v120, drawingRect.Bottom - v26, v110, v20);
                            g.DrawString(hint, hintFont, brush, hintRect, sf);
                        }
                    }
                    finally
                    {
                        if (hintFont != TextFont) hintFont?.Dispose();
                    }
                }
            }
            catch { }
        }
        
        protected virtual void DrawItemText(Graphics g, Rectangle textRect, string text, Color textColor, Font font)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (_owner.HighlightSearchMatches &&
                _owner.ShowSearch &&
                !string.IsNullOrWhiteSpace(_owner.SearchText))
            {
                Color hlBack = PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.PrimaryColor ?? Color.DodgerBlue, 45);
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
                // Fallback: draw a placeholder
                int inflate = DpiScalingHelper.ScaleValue(4, _owner);
                using (var brush = new SolidBrush(Color.FromArgb(150, Color.Gray)))
                {
                    var smallRect = imageRect;
                    smallRect.Inflate(-inflate, -inflate);
                    g.FillEllipse(brush, smallRect);
                }
            }
        }
        
        protected virtual void DrawCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, bool isHovered)
        {
            // Draw checkbox background
            Color bgColor = isHovered ? Color.FromArgb(240, 240, 240) : Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, checkboxRect);
            }
            
            // Draw checkbox border
            Color borderColor = isChecked ? (_theme?.PrimaryColor ?? Color.Blue) : Color.Gray;
            using (var pen = new Pen(borderColor, 1.5f))
            {
                g.DrawRectangle(pen, checkboxRect.X, checkboxRect.Y, checkboxRect.Width - 1, checkboxRect.Height - 1);
            }
            
            // Draw checkmark if checked
            if (isChecked)
            {
                int ck3 = DpiScalingHelper.ScaleValue(3, _owner);
                int ck4 = DpiScalingHelper.ScaleValue(4, _owner);
                using (var pen = new Pen(_theme?.PrimaryColor ?? Color.Blue, 2f))
                {
                    // Draw checkmark
                    Point[] checkPoints = new Point[]
                    {
                        new Point(checkboxRect.Left + ck3, checkboxRect.Top + checkboxRect.Height / 2),
                        new Point(checkboxRect.Left + checkboxRect.Width / 2 - 1, checkboxRect.Bottom - ck4),
                        new Point(checkboxRect.Right - ck3, checkboxRect.Top + ck3)
                    };
                    g.DrawLines(pen, checkPoints);
                }
            }
        }
        
        protected virtual void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;
            // Compute hover progress-based color blending for smooth transitions
            float hoverProgress = 0f;
            try
            {
                hoverProgress = _owner.GetHoverProgress(_owner.SelectedItem == null ? null : _owner.SelectedItem); // default 0
            }
            catch
            {
                hoverProgress = 0f;
            }

            // If owner can provide hover progress for this item directly, use it
            try
            {
                hoverProgress = Math.Max(hoverProgress, _owner.GetHoverProgress(_owner.SelectedItem == null ? null : _owner.SelectedItem));
            }
            catch { }

            // If the painter supports direct detection using IsHovered flag, we still use it as fallback
            if (isHovered)
            {
                hoverProgress = Math.Max(hoverProgress, 1f);
            }

            // Base background
            Color backgroundColor = _theme?.BackgroundColor ?? Color.White;

            // Selection override
            if (isSelected)
            {
                // Use primary color but with subtle alpha blend based on hover progress for pleasing effect
                Color primary = _theme?.PrimaryColor ?? Color.Empty;
                backgroundColor = PathPainterHelpers.WithAlphaIfNotEmpty(primary, 20);
                using (var pen = new Pen(Color.FromArgb(200, primary), 1.5f))
                {
                    g.DrawRectangle(pen, itemRect.X + 1, itemRect.Y + 1, itemRect.Width - 2, itemRect.Height - 2);
                }
            }
            else if (hoverProgress > 0f)
            {
                // Blend between base background and hover color based on progress
                Color hoverColor = _theme?.ListItemHoverBackColor ?? Color.FromArgb(230, 230, 230);
                backgroundColor = BlendColors(backgroundColor, hoverColor, hoverProgress);
            }

            using (var brush = new SolidBrush(backgroundColor))
            {
                g.FillRectangle(brush, itemRect);
            }
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
            using (var clearBrush = new SolidBrush(_theme?.BackgroundColor ?? Color.White))
            {
                g.FillRectangle(clearBrush, itemRect);
            }

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
                var selColor = (_owner.SelectionBackColor != Color.Empty) ? _owner.SelectionBackColor : (_theme?.PrimaryColor ?? Color.LightBlue);
                int alpha = Math.Max(0, Math.Min(255, _owner.SelectionOverlayAlpha > 0 ? _owner.SelectionOverlayAlpha : 90));
                using (var fillBrush = new SolidBrush(Color.FromArgb(alpha, selColor.R, selColor.G, selColor.B)))
                {
                    g.FillRectangle(fillBrush, itemRect);
                }

                // Draw selection border
                var borderColor = (_owner.SelectionBorderColor != Color.Empty) ? _owner.SelectionBorderColor : (_theme?.AccentColor ?? Color.Empty);
                int borderThickness = Math.Max(1, _owner.SelectionBorderThickness);
                using (var pen = new Pen(borderColor, borderThickness))
                {
                    g.DrawRectangle(pen, itemRect.X + 1, itemRect.Y + 1, itemRect.Width - 2, itemRect.Height - 2);
                }

                // Focus outline for focused item
                if (_owner.Focused && _owner.SelectedItem == item)
                {
                    var focusColor = (_owner.FocusOutlineColor != Color.Empty) ? _owner.FocusOutlineColor : (_theme?.PrimaryColor ?? Color.LightBlue);
                    int focusThickness = Math.Max(1, _owner.FocusOutlineThickness);
                    using (var penFocus = new Pen(focusColor, focusThickness))
                    {
                        // draw a rounded or simple rectangle as focus outline
                        g.DrawRectangle(penFocus, itemRect.X + 2, itemRect.Y + 2, itemRect.Width - 4, itemRect.Height - 4);
                    }
                }
            }
            else if (hoverProgress > 0f)
            {
                // Only apply hover overlay if not selected
                var hoverColor = _theme?.ListItemHoverBackColor ?? Color.FromArgb(230, 230, 230);
                var overlayColor = BlendColors(Color.Transparent, hoverColor, hoverProgress);
                using (var brush = new SolidBrush(Color.FromArgb((int)(hoverProgress * 60), overlayColor.R, overlayColor.G, overlayColor.B)))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }

            // ── High-contrast override ───────────────────────────────────────────
            if (_owner.IsHighContrast)
            {
                Color hcBg = _owner.HCItemBackground(isHovered, isSelected);
                if (hcBg != Color.Empty)
                {
                    using var hcBrush = new SolidBrush(hcBg);
                    g.FillRectangle(hcBrush, itemRect);
                }
                Color hcBorder = _owner.HCBorderColor;
                if (isSelected && hcBorder != Color.Empty)
                {
                    using var hcPen = new Pen(hcBorder, 2f);
                    g.DrawRectangle(hcPen, itemRect.X + 1, itemRect.Y + 1, itemRect.Width - 2, itemRect.Height - 2);
                }
            }

            // ── Disabled dimming overlay ─────────────────────────────────────────
            if (item is BeepListItem ri && ri.IsDisabled)
            {
                var bg = _theme?.BackgroundColor ?? Sc(_owner.BackColor, SystemColors.Window);
                using var dimBrush = new SolidBrush(Color.FromArgb(ListBoxTokens.DisabledAlpha, bg));
                g.FillRectangle(dimBrush, itemRect);
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
                var metricFont = new Font((TextFont ?? _owner.TextFont).FontFamily, Math.Max(8f, (TextFont ?? _owner.TextFont).Size - 1f), FontStyle.Regular);
                try
                {
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
                        Color.FromArgb(ListBoxTokens.SubTextAlpha, _theme?.ListItemForeColor ?? Color.Gray),
                        TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                    trailingReservation = metricW + DpiScalingHelper.ScaleValue(4, _owner);
                }
                finally
                {
                    metricFont.Dispose();
                }
            }

            // Draw text (use TextFont from theme)
            Color textColor = isSelected ? Color.White : (_theme?.ListItemForeColor ?? Color.Black);
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
        public virtual int GetItemHeight(BeepListBox owner, object item)
        {
            if (item is BeepListItem ri && !string.IsNullOrEmpty(ri.SubText))
            {
                // 2-line item: base height + sub-text line (~18 px scaled)
                int sub = DpiScalingHelper.ScaleValue(18, owner ?? _owner);
                return GetPreferredItemHeight() + sub;
            }
            if (item is BeepListItem sep && sep.IsSeparator)
                return DpiScalingHelper.ScaleValue(ListBoxTokens.SeparatorHeight, owner ?? _owner);

            return GetPreferredItemHeight();
        }

        /// <inheritdoc/>
        public virtual void DrawGroupHeader(
            Graphics g, BeepListBox owner, Rectangle headerRect,
            string groupKey, bool isCollapsed, int itemCount)
        {
            if (g == null || headerRect.IsEmpty) return;
            // Background
            using var bg = new SolidBrush(Color.FromArgb(30, _theme?.PrimaryColor ?? Color.Gray));
            g.FillRectangle(bg, headerRect);

            // Bottom border line
            using var pen = new Pen(Color.FromArgb(40, _theme?.PrimaryColor ?? Color.Gray), 1f);
            g.DrawLine(pen, headerRect.Left, headerRect.Bottom - 1, headerRect.Right, headerRect.Bottom - 1);

            int pad     = DpiScalingHelper.ScaleValue(8, owner);
            int chevron = DpiScalingHelper.ScaleValue(10, owner);

            // Chevron: ▶ collapsed, ▼ expanded
            string ch = isCollapsed ? "▶" : "▼";
            using var chFont  = new Font(Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 7f);
            using var chBrush = new SolidBrush(_theme?.ListForeColor ?? Color.Gray);
            var chRect = new Rectangle(headerRect.Left + pad, headerRect.Top, chevron, headerRect.Height);
            g.DrawString(ch, chFont, chBrush,
                new RectangleF(chRect.X, chRect.Y, chRect.Width, chRect.Height),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            // Group label + count
            string label = $"{groupKey}  ({itemCount})";
            using var lFont  = new Font(owner.Font.FontFamily, Math.Max(8f, owner.Font.Size - 1f), FontStyle.Bold);
            using var lBrush = new SolidBrush(_theme?.ListForeColor ?? Color.Gray);
            var lRect = new Rectangle(headerRect.Left + pad + chevron + 4, headerRect.Top,
                                      headerRect.Width - pad * 2 - chevron - 4, headerRect.Height);
            g.DrawString(label, lFont, lBrush, lRect,
                new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center,
                                   Trimming = StringTrimming.EllipsisCharacter });
        }

        // The Font helper below lets DrawGroupHeader compile without a direct field ref
        private static Font? Font => null;  // placeholders resolved via _owner.TextFont above

        // ── Sprint 7: shared decoration helpers ─────────────────────────────────

        /// <summary>
        /// Draws a 3 px left-edge accent bar using <paramref name="accentColor"/>.
        /// Painters should call this before rendering item text.
        /// </summary>
        protected void DrawAccentBar(Graphics g, Rectangle rowRect, Color accentColor)
        {
            if (accentColor == Color.Empty || accentColor.A == 0) return;
            int w = DpiScalingHelper.ScaleValue(ListBoxTokens.PinnedAccentBarWidth, _owner);
            using var brush = new SolidBrush(accentColor);
            g.FillRectangle(brush, rowRect.X, rowRect.Y, w, rowRect.Height);
        }

        /// <summary>
        /// Draws an expand/collapse chevron (▶ or ▼) for tree items.
        /// </summary>
        protected virtual void DrawChevron(Graphics g, Rectangle rect, bool isExpanded)
        {
            int cx = rect.X + rect.Width / 2;
            int cy = rect.Y + rect.Height / 2;
            int size = Math.Min(rect.Width, rect.Height) / 2;

            var chevronColor = _helper?.GetTextColor() ?? Color.Gray;
            using var pen = new Pen(Color.FromArgb(ListBoxTokens.SubTextAlpha, chevronColor), 1.5f);

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

            using var badgeFont = BeepFontManager.GetFont(_owner.TextFont.Name, Math.Max(7f, _owner.TextFont.Size - 3f), FontStyle.Bold);
            var textSize = TextRenderer.MeasureText(badgeText, badgeFont);
            int r = DpiScalingHelper.ScaleValue(ListBoxTokens.BadgePillRadius, _owner);
            int pad = DpiScalingHelper.ScaleValue(6, _owner);
            int pillW = Math.Max(r * 2, textSize.Width + pad);
            int pillH = Math.Max(r * 2, textSize.Height + 2);
            int pillX = rowRect.Right  - pillW - DpiScalingHelper.ScaleValue(8, _owner);
            int pillY = rowRect.Top    + (rowRect.Height - pillH) / 2;

            var pillRect = new Rectangle(pillX, pillY, pillW, pillH);

            // Fill pill
            var fill = badgeColor == Color.Empty ? (_theme?.PrimaryColor ?? Color.DodgerBlue) : badgeColor;
            using var path = GraphicsExtensions.CreateRoundedRectanglePath(pillRect, r);
            using var fillBrush = new SolidBrush(fill);
            g.FillPath(fillBrush, path);

            // Badge text (white/auto-contrast)
            Color textColor = fill.GetBrightness() > 0.55f ? Color.Black : Color.White;
            using var textBrush = new SolidBrush(textColor);
            g.DrawString(badgeText, badgeFont, textBrush, pillRect,
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

        /// <summary>
        /// Draws a secondary sub-text line below the primary text.
        /// </summary>
        protected void DrawSubText(Graphics g, Rectangle subRect, string subText, Color color, Font? ownerFont)
        {
            if (string.IsNullOrEmpty(subText) || subRect.IsEmpty) return;
            using var subFont = new Font(
                (ownerFont ?? _owner.TextFont).FontFamily,
                Math.Max(7f, (ownerFont ?? _owner.TextFont).Size - 1.5f),
                FontStyle.Regular);
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
            var lineColor = Color.FromArgb(60, _theme?.ListForeColor ?? Color.Gray);
            using var pen = new Pen(lineColor, 1f);

            if (string.IsNullOrEmpty(label))
            {
                g.DrawLine(pen, rowRect.Left + 8, midY, rowRect.Right - 8, midY);
            }
            else
            {
                using var lFont = BeepFontManager.GetFont(_owner.TextFont.Name, Math.Max(7f, _owner.TextFont.Size - 2f), FontStyle.Regular);
                var lSize  = TextRenderer.MeasureText(label, lFont);
                int lX     = rowRect.Left + (rowRect.Width - lSize.Width) / 2;
                int gapPad = DpiScalingHelper.ScaleValue(4, _owner);

                g.DrawLine(pen, rowRect.Left + 8, midY, lX - gapPad, midY);
                g.DrawLine(pen, lX + lSize.Width + gapPad, midY, rowRect.Right - 8, midY);
                TextRenderer.DrawText(g, label, lFont,
                    new Rectangle(lX, rowRect.Top, lSize.Width, rowRect.Height),
                    Color.FromArgb(ListBoxTokens.SubTextAlpha, _theme?.ListForeColor ?? Color.Gray),
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
                ringColor = SystemColors.Highlight;
                thickness = 3;
            }
            else
            {
                ringColor = _owner.FocusOutlineColor != Color.Empty
                    ? _owner.FocusOutlineColor
                    : (_theme?.PrimaryColor ?? Color.DodgerBlue);
                thickness = ListBoxTokens.FocusRingThickness;
            }

            using var pen = new Pen(ringColor, thickness);
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

            using var pen = new Pen(Color.FromArgb(40, 0, 0, 0), 1.5f);
            g.DrawEllipse(pen, avatarRect);
        }

        /// <summary>
        /// Draws a coloured circle with 1–2 letter initials (reusable fallback).
        /// </summary>
        protected void DrawInitialsFallback(Graphics g, Rectangle rect, string name)
        {
            Color[] palette =
            {
                Color.FromArgb(239, 83, 80),
                Color.FromArgb(171, 71, 188),
                Color.FromArgb(66, 165, 245),
                Color.FromArgb(38, 166, 154),
                Color.FromArgb(102, 187, 106),
                Color.FromArgb(255, 167, 38),
                Color.FromArgb(141, 110, 99),
            };
            int idx = Math.Abs((name ?? "").GetHashCode()) % palette.Length;
            using var bg = new SolidBrush(palette[idx]);
            g.FillEllipse(bg, rect);

            string initials = GetInitials(name);
            using var font = BeepFontManager.GetFont(
                _owner.TextFont.Name, Math.Max(10f, rect.Height * 0.35f), FontStyle.Bold);
            using var brush = new SolidBrush(Color.White);
            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(initials, font, brush, rect, sf);
        }

        #endregion

        private static Color Sc(Color refColor, Color lightColor)
        {
            bool dark = refColor.GetBrightness() < 0.5;
            if (!dark) return lightColor;
            return lightColor switch
            {
                var x when x == SystemColors.Window => Color.FromArgb(30, 30, 30),
                var x when x == SystemColors.WindowText => Color.White,
                var x when x == SystemColors.ControlText => Color.White,
                var x when x == SystemColors.GrayText => Color.FromArgb(150, 150, 155),
                var x when x == SystemColors.Highlight => Color.FromArgb(0, 120, 215),
                var x when x == SystemColors.HighlightText => Color.White,
                var x when x == SystemColors.Control => Color.FromArgb(45, 45, 48),
                var x when x == SystemColors.ControlDark => Color.FromArgb(70, 70, 75),
                var x when x == SystemColors.ControlLight => Color.FromArgb(70, 70, 75),
                var x when x == SystemColors.ControlLightLight => Color.FromArgb(60, 60, 65),
                var x when x == SystemColors.ActiveCaption => Color.FromArgb(45, 45, 48),
                var x when x == SystemColors.Info => Color.FromArgb(50, 50, 55),
                var x when x == SystemColors.InfoText => Color.White,
                _ => lightColor
            };
        }
    }
}
