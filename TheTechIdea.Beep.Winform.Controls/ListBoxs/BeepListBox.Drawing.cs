using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Drawing logic for BeepListBox
    /// </summary>
    public partial class BeepListBox
    {
        #region Drawing Override
        
        /// <summary>
        /// DrawContent override - called by BaseControl
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            Paint(g, DrawingRect);
        }

        /// <summary>
        /// Draw override - called by BeepGridPro and containers
        /// </summary>
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            Paint(graphics, rectangle);
        }

        /// <summary>
        /// Main paint function - centralized painting logic
        /// Called from both DrawContent and Draw
        /// </summary>
        private void Paint(Graphics g, Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            
            // Ensure painter exists for current type
            if (_listBoxPainter == null)
            {
                _listBoxPainter = CreatePainter(_listBoxType);
                _listBoxPainter.Initialize(this, _currentTheme);
                _listBoxPainter.Style = ControlStyle;
            }
            if (_listBoxPainter.Style != ControlStyle)
            {
                _listBoxPainter.Style = ControlStyle;
            }
            
            // Update layout if needed
            if (_needsLayoutUpdate)
            {
                UpdateLayout();
                _layoutHelper.CalculateLayout();
                _hitHelper.RegisterHitAreas();
                _needsLayoutUpdate = false;
            }

            // ── Loading / skeleton state ─────────────────────────────────────────
            if (_isLoading)
            {
                DrawSkeletonRows(g, bounds);
                return;
            }

            // ── Empty state ──────────────────────────────────────────────────────
            if (ShowEmptyState && (_listItems == null || _listItems.Count == 0))
            {
                DrawEmptyState(g, bounds);
                return;
            }

            // ── Empty-search state ───────────────────────────────────────────────
            if (ShowSearch && !string.IsNullOrWhiteSpace(SearchText))
            {
                var visible = Helper?.GetVisibleItems() ?? new System.Collections.Generic.List<SimpleItem>();
                if (visible.Count == 0)
                {
                    DrawEmptySearchState(g, bounds);
                    return;
                }
            }

            // Let the list box painter draw everything
            _listBoxPainter.Paint(g, this, bounds);

            // Draw drag-to-reorder insertion indicator if a drag is active
            DrawDragIndicator(g);
        }
        
        #endregion
        
        #region Painter Factory
        
        /// <summary>
        /// Creates the appropriate painter for the specified ListBoxType
        /// </summary>
        private IListBoxPainter CreatePainter(ListBoxType type)
        {
            var metadata = ListBoxVariantMetadataCatalog.Resolve(type);
            IListBoxPainter painter = type switch
            {
                ListBoxType.Standard => new StandardListBoxPainter(),
                ListBoxType.Minimal => new MinimalListBoxPainter(),
                ListBoxType.Outlined => new OutlinedListBoxPainter(),
                ListBoxType.Rounded => new RoundedListBoxPainter(),
                ListBoxType.MaterialOutlined => new MaterialOutlinedListBoxPainter(),
                ListBoxType.Filled => new FilledListBoxPainter(),
                ListBoxType.Borderless => new BorderlessListBoxPainter(),
                ListBoxType.CategoryChips => new CategoryChipsPainter(),
                ListBoxType.SearchableList => new SearchableListPainter(),
                ListBoxType.WithIcons => new WithIconsListBoxPainter(),
                ListBoxType.CheckboxList => new CheckboxListPainter(),
                ListBoxType.SimpleList => new SimpleListPainter(),
                ListBoxType.LanguageSelector => new LanguageSelectorPainter(),
                ListBoxType.CardList => new CardListPainter(),
                ListBoxType.Compact => new CompactListPainter(),
                ListBoxType.Grouped => new GroupedListPainter(),
                ListBoxType.TeamMembers => new TeamMembersPainter(),
                ListBoxType.FilledStyle => new FilledStylePainter(),
                ListBoxType.FilterStatus => new FilterStatusPainter(),
                ListBoxType.OutlinedCheckboxes => new OutlinedCheckboxesPainter(),
                ListBoxType.RaisedCheckboxes => new RaisedCheckboxesPainter(),
                ListBoxType.MultiSelectionTeal => new MultiSelectionTealPainter(),
                ListBoxType.ColoredSelection => new ColoredSelectionPainter(),
                ListBoxType.RadioSelection => new RadioSelectionPainter(),
                ListBoxType.ErrorStates => new ErrorStatesPainter(),
                ListBoxType.Custom => new CustomListPainter(),
                // New modern styles
                ListBoxType.Glassmorphism => new GlassmorphismListBoxPainter(),
                ListBoxType.Neumorphic => new NeumorphicListBoxPainter(),
                ListBoxType.GradientCard => new GradientCardListBoxPainter(),
                ListBoxType.ChipStyle => new ChipStyleListBoxPainter(),
                ListBoxType.AvatarList => new AvatarListBoxPainter(),
                ListBoxType.Timeline => new TimelineListBoxPainter(),
                // Sprint 4 — new painters
                ListBoxType.InfiniteScroll => new InfiniteScrollListBoxPainter(),
                ListBoxType.CommandList    => new CommandListBoxPainter(),
                ListBoxType.NavigationRail => new NavigationRailListBoxPainter(),
                ListBoxType.ChatList        => new ChatListBoxPainter(),
                ListBoxType.ContactList     => new ContactListBoxPainter(),
                ListBoxType.ThreeLineList   => new ThreeLineListBoxPainter(),
                ListBoxType.NotificationList => new NotificationListBoxPainter(),
                ListBoxType.ProfileCard     => new ProfileCardListBoxPainter(),
                // UI framework styles
                ListBoxType.RekaUI          => new RekaUIListBoxPainter(),
                ListBoxType.ChakraUI        => new ChakraUIListBoxPainter(),
                ListBoxType.HeroUI          => new HeroUIListBoxPainter(),
                _ => new StandardListBoxPainter()
            };
            ControlStyle = BeepStyling.GetControlStyle(BeepThemesManager.CurrentStyle);
            painter.Style = ControlStyle;

            // Honor distinct variant defaults without consolidating enum values.
            if (Density == ListDensityMode.Comfortable && metadata.DensityDefault != ListDensityMode.Comfortable)
            {
                Density = metadata.DensityDefault;
            }

            // If custom painter and we have a custom renderer, set it
            if (painter is CustomListPainter customPainter && _customItemRenderer != null)
            {
                customPainter.CustomItemRenderer = _customItemRenderer;
            }
            
            return painter;
        }
        
        #endregion
        
        #region Hit Area Registration
        
        /// <summary>
        /// Registers interactive areas for hit testing
        /// </summary>
        // Hit areas are managed by BeepListBoxHitTestHelper via BaseControl._hitTest
        
        #endregion

        // ════════════════════════════════════════════════════════════════════════════
        //  Sprint 4 — Skeleton / empty-state / search-highlight drawing helpers
        // ════════════════════════════════════════════════════════════════════════════

        #region Skeleton rows

        private void DrawSkeletonRows(Graphics g, Rectangle bounds)
        {
            int rowH   = Helpers.DpiScalingHelper.ScaleValue(ListBoxTokens.ItemHeightComfortable, this);
            int padH   = Helpers.DpiScalingHelper.ScaleValue(ListBoxTokens.ItemPaddingH, this);
            int padV   = Helpers.DpiScalingHelper.ScaleValue(8, this);
            int radius = Helpers.DpiScalingHelper.ScaleValue(6, this);

            int rows  = SkeletonRowCount;
            int phase = (int)(_skeletonPhase * bounds.Width);   // shimmer X offset

            Color baseColor = _currentTheme != null
                ? Color.FromArgb(ListBoxTokens.SkeletonAlpha, _currentTheme.ForeColor)
                : Color.FromArgb(ListBoxTokens.SkeletonAlpha, Color.Gray);

            for (int i = 0; i < rows; i++)
            {
                var rowRect = new Rectangle(bounds.Left, bounds.Top + i * rowH, bounds.Width, rowH);
                if (rowRect.Bottom > bounds.Bottom) break;

                // Avatar circle placeholder
                int avatarSize = Helpers.DpiScalingHelper.ScaleValue(ListBoxTokens.AvatarSize, this);
                var avatarRect = new Rectangle(
                    rowRect.Left + padH,
                    rowRect.Top  + (rowH - avatarSize) / 2,
                    avatarSize, avatarSize);

                using var avatarBrush = new SolidBrush(baseColor);
                g.FillEllipse(avatarBrush, avatarRect);

                // Title bar placeholder
                int textX = avatarRect.Right + padH;
                int titleH = Helpers.DpiScalingHelper.ScaleValue(12, this);
                int titleW = (int)(bounds.Width * (0.35 + 0.25 * ((i % 3) / 2.0)));
                var titleRect = new Rectangle(textX, rowRect.Top + padV, titleW, titleH);

                using var titleBrush = new SolidBrush(baseColor);
                DrawRoundRect(g, titleBrush, titleRect, radius);

                // Sub-text bar placeholder
                int subH = Helpers.DpiScalingHelper.ScaleValue(10, this);
                int subW = (int)(titleW * 0.65);
                var subRect = new Rectangle(textX, titleRect.Bottom + Helpers.DpiScalingHelper.ScaleValue(4, this), subW, subH);
                DrawRoundRect(g, titleBrush, subRect, radius);

                // Shimmer overlay
                if (bounds.Width > 0)
                {
                    int shimW = bounds.Width / 3;
                    int shimX = rowRect.Left + (phase + i * 40) % (bounds.Width + shimW) - shimW;
                    var shimRect = new Rectangle(shimX, rowRect.Top, shimW, rowH);
                    using var shimBrush = new LinearGradientBrush(
                        shimRect,
                        Color.Transparent,
                        Color.FromArgb(60, Color.White),
                        LinearGradientMode.Horizontal);
                    g.FillRectangle(shimBrush, shimRect);
                }
            }
        }

        private static void DrawRoundRect(Graphics g, Brush brush, Rectangle rect, int radius)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            radius = Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2));
            using var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            g.FillPath(brush, path);
        }

        #endregion

        #region Empty state

        private void DrawEmptyState(Graphics g, Rectangle bounds)
        {
            string headline = EmptyStateText ?? "Nothing here yet";
            string subText  = "Add items to see them here";

            int iconSize = Helpers.DpiScalingHelper.ScaleValue(ListBoxTokens.EmptyStateIconSize, this);

            // Centre the content block
            using var headlineFont = new Font(_textFont.FontFamily, ListBoxTokens.EmptyStateHeadlinePt, System.Drawing.FontStyle.Bold);
            using var subFont      = new Font(_textFont.FontFamily, ListBoxTokens.EmptyStateSubTextPt);

            var headlineSize = g.MeasureString(headline, headlineFont);
            var subTextSize  = g.MeasureString(subText,  subFont);

            int totalH = iconSize + 12 + (int)headlineSize.Height + 4 + (int)subTextSize.Height;
            int startY = bounds.Top + (bounds.Height - totalH) / 2;

            Color headlineColor = _currentTheme?.ForeColor ?? Color.Gray;
            Color subColor      = Color.FromArgb(ListBoxTokens.SubTextAlpha, headlineColor);

            // Icon placeholder — simple inbox tray shape
            int iconX = bounds.Left + (bounds.Width - iconSize) / 2;
            var iconRect = new Rectangle(iconX, startY, iconSize, iconSize);
            using var iconPen  = new Pen(Color.FromArgb(90, headlineColor), 2f);
            g.DrawRectangle(iconPen,
                iconRect.X + iconSize / 4,
                iconRect.Y + iconSize / 4,
                iconSize / 2,
                iconSize / 2);
            g.DrawLine(iconPen, iconRect.Left  + iconSize / 4, iconRect.Top + iconSize * 3 / 4, iconRect.Left, iconRect.Bottom);
            g.DrawLine(iconPen, iconRect.Right - iconSize / 4, iconRect.Top + iconSize * 3 / 4, iconRect.Right, iconRect.Bottom);
            g.DrawLine(iconPen, iconRect.Left, iconRect.Bottom, iconRect.Right, iconRect.Bottom);

            int textY = iconRect.Bottom + 12;

            // Headline
            using var hBrush = new SolidBrush(headlineColor);
            var hRect = new RectangleF(bounds.Left, textY, bounds.Width, headlineSize.Height + 4);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(headline, headlineFont, hBrush, hRect, sf);

            // Sub-text
            using var sBrush = new SolidBrush(subColor);
            var sRect = new RectangleF(bounds.Left, hRect.Bottom + 4, bounds.Width, subTextSize.Height + 4);
            g.DrawString(subText, subFont, sBrush, sRect, sf);
        }

        private void DrawEmptySearchState(Graphics g, Rectangle bounds)
        {
            string headline = $"No results for \"{SearchText}\"";
            string subText  = "Try a different search term";

            int iconSize = Helpers.DpiScalingHelper.ScaleValue(ListBoxTokens.EmptyStateIconSize, this);

            using var headlineFont = new Font(_textFont.FontFamily, ListBoxTokens.EmptyStateHeadlinePt, System.Drawing.FontStyle.Bold);
            using var subFont      = new Font(_textFont.FontFamily, ListBoxTokens.EmptyStateSubTextPt);

            var headlineSize = g.MeasureString(headline, headlineFont);
            var subTextSize  = g.MeasureString(subText,  subFont);

            int totalH = iconSize + 12 + (int)headlineSize.Height + 4 + (int)subTextSize.Height;
            int startY = bounds.Top + (bounds.Height - totalH) / 2;

            Color headlineColor = _currentTheme?.ForeColor ?? Color.Gray;
            Color subColor      = Color.FromArgb(ListBoxTokens.SubTextAlpha, headlineColor);

            // Search icon — magnifying glass
            int iconX = bounds.Left + (bounds.Width - iconSize) / 2;
            int circleSize = iconSize / 2;
            int circleX = iconX + (iconSize - circleSize) / 2;
            int circleY = startY + iconSize / 6;
            using var iconPen = new Pen(Color.FromArgb(90, headlineColor), 2f);
            g.DrawEllipse(iconPen, circleX, circleY, circleSize, circleSize);
            int handleLen = iconSize / 4;
            int hx = circleX + circleSize / 2 + handleLen / 3;
            int hy = circleY + circleSize + handleLen / 3;
            g.DrawLine(iconPen, circleX + circleSize * 3 / 4, circleY + circleSize * 3 / 4, hx, hy);

            int textY = startY + iconSize + 12;

            using var hBrush = new SolidBrush(headlineColor);
            var hRect = new RectangleF(bounds.Left, textY, bounds.Width, headlineSize.Height + 4);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(headline, headlineFont, hBrush, hRect, sf);

            using var sBrush = new SolidBrush(subColor);
            var sRect = new RectangleF(bounds.Left, hRect.Bottom + 4, bounds.Width, subTextSize.Height + 4);
            g.DrawString(subText, subFont, sBrush, sRect, sf);
        }

        #endregion

        #region Search-match highlight utility

        /// <summary>
        /// Draws text with matched portions highlighted (like VS Code search).
        /// Painters call this in place of a plain DrawString when SearchText is active.
        /// </summary>
        internal void DrawHighlightedText(
            Graphics g,
            string text,
            string query,
            Rectangle rect,
            Font font,
            Color normalColor,
            Color highlightForeColor,
            Color highlightBackColor)
        {
            if (string.IsNullOrEmpty(query) || string.IsNullOrEmpty(text))
            {
                using var b = new SolidBrush(normalColor);
                g.DrawString(text ?? "", font, b, rect);
                return;
            }

            string lower      = text.ToLowerInvariant();
            string queryLower = query.ToLowerInvariant();

            int pos = 0;
            float x = rect.Left;
            float y = rect.Top + (rect.Height - font.Height) / 2f;

            using var normalBrush = new SolidBrush(normalColor);
            using var hlBrush     = new SolidBrush(highlightForeColor);
            using var hlBackBrush = new SolidBrush(highlightBackColor);

            while (pos < text.Length)
            {
                int matchIdx = lower.IndexOf(queryLower, pos, StringComparison.Ordinal);
                if (matchIdx < 0)
                {
                    string rest = text.Substring(pos);
                    g.DrawString(rest, font, normalBrush, x, y);
                    break;
                }

                // Draw text before match
                if (matchIdx > pos)
                {
                    string before = text.Substring(pos, matchIdx - pos);
                    g.DrawString(before, font, normalBrush, x, y);
                    x += g.MeasureString(before, font).Width;
                }

                // Draw match with background pill
                string match = text.Substring(matchIdx, queryLower.Length);
                float matchW = g.MeasureString(match, font).Width;
                var hlRect = new RectangleF(x, y, matchW, font.Height);
                int r = Helpers.DpiScalingHelper.ScaleValue(3, this);
                using (var path = RoundedRectPath(hlRect, r))
                    g.FillPath(hlBackBrush, path);
                g.DrawString(match, font, hlBrush, x, y);
                x += matchW;

                pos = matchIdx + queryLower.Length;
            }
        }

        private static System.Drawing.Drawing2D.GraphicsPath RoundedRectPath(RectangleF rect, int r)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
            path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
            path.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion
    }
}
