using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Toolbar
{
    /// <summary>
    /// Paints the unified grid toolbar.  Everything is owner-drawn
    /// using <see cref="Graphics"/> primitives; the only child control
    /// is the on-demand search text editor managed by
    /// <see cref="Filtering.FilterEditorHelper"/>.
    /// </summary>
    public class BeepGridToolbarPainter : IDisposable
    {
        private readonly BeepGridPro _grid;

        // Cached brushes/pens/paths.  Recreated only when the theme
        // resolution, DPI scale, host font, or any of the toolbar
        // appearance colors change, not on every paint.
        private IBeepTheme? _cachedTheme;
        private float _cachedDpiScale;
        private int _currentSearchIconWidth = 24;
        private Brush? _cachedBackBrush;
        private Brush? _cachedBadgeBrush;
        private Brush? _cachedHoverBrush;
        private Brush? _cachedPressedBrush;
        private Brush? _cachedSearchBackBrush;
        private Brush? _cachedSearchFocusBackBrush;
        private Pen? _cachedBorderPen;
        private Pen? _cachedSeparatorPen;
        private Pen? _cachedChevronPen;
        private Pen? _cachedSearchFocusBorderPen;
        private Brush? _cachedSearchForeBrush;
        private Brush? _cachedSearchPlaceholderBrush;
        private Brush? _cachedLabelForeBrush;
        private Brush? _cachedWhiteBrush;
        private Font? _cachedLabelFont;
        private Font? _cachedTitleFont;
        private Font? _cachedBadgeFont;
        private GraphicsPath? _searchBoxPath;

        // Snapshot of the appearance colors used to build the cache.
        // Compared on each paint so a runtime color change invalidates
        // the cache without forcing the host to call InvalidateCache().
        private Color _cachedBackColor;
        private Color _cachedForeColor;
        private Color _cachedPlaceholderColor;
        private Color _cachedSearchBackColor;
        private Color _cachedSearchFocusBackColor;
        private Color _cachedBorderColor;
        private Color _cachedButtonHoverColor;
        private Color _cachedButtonPressedColor;
        private Color _cachedSeparatorColor;

        public BeepGridToolbarPainter(BeepGridPro grid) { _grid = grid; }

        private IBeepTheme? Theme => _cachedTheme ??= _grid.Theme != null
            ? BeepThemesManager.GetTheme(_grid.Theme)
            : BeepThemesManager.GetDefaultTheme();

        public void Paint(Graphics g, Rectangle bounds, BeepGridToolbarState state)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            EnsurePaintCache(state);

            // Background
            g.FillRectangle(_cachedBackBrush!, bounds);
            PaintGridTitle(g, state);
            PaintActionButtons(g, state);
            PaintFilterSection(g, state);
            PaintSearchSection(g, state);
            PaintExportButtons(g, state);
            PaintOverflowButton(g, state);
            PaintSeparators(g, state);
            PaintBottomBorder(g, bounds);
        }

        /// <summary>
        /// Ensures the painter's brush/pen/font cache is up to date and
        /// copies the resolved fonts onto <paramref name="state"/> so a
        /// subsequent <see cref="BeepGridToolbarState.CalculateLayout"/>
        /// uses the same font instance the painter will draw with.
        /// Hosts that run layout from their own paint pipeline
        /// (instead of calling <see cref="Paint"/> directly) should
        /// call this before <c>state.CalculateLayout(...)</c>.
        /// </summary>
        public void PrepareLayout(BeepGridToolbarState state)
        {
            EnsurePaintCache(state);
            // Sync the font references after the cache is built so a
            // later CalculateLayout measures with the same instance the
            // painter will draw with.  Done here (not inside
            // EnsurePaintCache) so the painter never touches the state
            // unless the host asked for layout preparation.
            state.LabelFont = _cachedLabelFont!;
            state.TitleFont = _cachedTitleFont!;
        }

        // ─── Cache management ────────────────────────────────────────

        private void EnsurePaintCache(BeepGridToolbarState state)
        {
            // Recreate the cache only when something that affects the
            // cache has changed.  This is the hot path; we do not want
            // a per-paint allocation.  The check covers:
            //   - theme resolution
            //   - DPI scale
            //   - host font
            //   - any of the toolbar appearance colors (so a runtime
            //     color change from the property grid is reflected)
            var theme = Theme;
            // Theme-aware resolution: each grid property defaults to Color.Empty
            // (sentinel = "use theme").  When the property value is Color.Empty,
            // resolve the color from the active IBeepTheme.  Fall back to
            // ThemeFallback (the same Bootstrap-light palette the grid shipped
            // with) so existing hosts see no visual change.
            var back       = ResolveToolbarColor(_grid.ToolbarBackColor,              theme?.BackgroundColor ?? ThemeFallback.Back);
            var fore       = ResolveToolbarColor(_grid.ToolbarForeColor,              theme?.ForeColor ?? ThemeFallback.Fore);
            var placeholder = ResolveToolbarColor(_grid.ToolbarPlaceholderColor,       ThemeFallback.Placeholder);
            var searchBack  = ResolveToolbarColor(_grid.ToolbarSearchBackColor,        theme?.BackgroundColor ?? ThemeFallback.SearchBack);
            var searchFocus = ResolveToolbarColor(_grid.ToolbarSearchFocusBackColor,   theme?.SelectedRowBackColor ?? ThemeFallback.SearchFocusBack);
            var border      = ResolveToolbarColor(_grid.ToolbarBorderColor,            theme?.BorderColor ?? ThemeFallback.Border);
            var hover       = ResolveToolbarColor(_grid.ToolbarButtonHoverBackColor,   ThemeFallback.Hover);
            var pressed     = ResolveToolbarColor(_grid.ToolbarButtonPressedBackColor, ThemeFallback.Pressed);
            var separator   = ResolveToolbarColor(_grid.ToolbarSeparatorColor,         theme?.BorderColor ?? ThemeFallback.Separator);

            // The cache is only reusable when every brush in it actually exists. Assigning
            // _cachedBackBrush here (before the check) and then letting DisposeCache null it was
            // hiding a much worse problem — see the rebuild below.
            bool cacheIntact = _cachedBackBrush != null && _cachedHoverBrush != null
                               && _cachedPressedBrush != null && _cachedSearchBackBrush != null
                               && _cachedSearchFocusBackBrush != null && _cachedBorderPen != null;

            if (cacheIntact
                && ReferenceEquals(theme, _cachedTheme) && state.DpiScale == _cachedDpiScale
                && ReferenceEquals(_grid.Font, _cachedLabelFont)
                && back == _cachedBackColor
                && fore == _cachedForeColor
                && placeholder == _cachedPlaceholderColor
                && searchBack == _cachedSearchBackColor
                && searchFocus == _cachedSearchFocusBackColor
                && border == _cachedBorderColor
                && hover == _cachedButtonHoverColor
                && pressed == _cachedButtonPressedColor
                && separator == _cachedSeparatorColor)
            {
                return;
            }

            DisposeCache();
            _cachedTheme = theme;
            _cachedDpiScale = state.DpiScale;
            _currentSearchIconWidth = state.SearchIconWidth;
            _cachedLabelFont = _grid.Font ?? SystemFonts.DefaultFont;
            _cachedTitleFont = new Font(_cachedLabelFont.FontFamily, _cachedLabelFont.Size + 1f, FontStyle.Bold);
            _cachedBadgeFont = new Font(_cachedLabelFont.FontFamily, 7f);

            // Snapshot the appearance colors so the next pass can detect
            // a runtime change without forcing the host to call
            // InvalidatePaintCache().
            _cachedBackColor = back;
            _cachedForeColor = fore;
            _cachedPlaceholderColor = placeholder;
            _cachedSearchBackColor = searchBack;
            _cachedSearchFocusBackColor = searchFocus;
            _cachedBorderColor = border;
            _cachedButtonHoverColor = hover;
            _cachedButtonPressedColor = pressed;
            _cachedSeparatorColor = separator;

            // These five were disposed and nulled by DisposeCache but never rebuilt, so from the
            // first cache rebuild onwards _cachedSearchBackBrush was null and PaintSearchBox threw
            // on FillPath. The toolbar's paint call is wrapped in a silent catch, so the exception
            // aborted the rest of the toolbar: no search box, no export buttons, no overflow
            // chevron, no separators, no bottom border — only the title and the filter gear ever
            // reached the screen.
            _cachedBackBrush = new SolidBrush(back);
            _cachedHoverBrush = new SolidBrush(hover);
            _cachedPressedBrush = new SolidBrush(pressed);
            _cachedSearchBackBrush = new SolidBrush(searchBack);
            _cachedSearchFocusBackBrush = new SolidBrush(searchFocus);

            _cachedSearchForeBrush = new SolidBrush(fore);
            _cachedSearchPlaceholderBrush = new SolidBrush(placeholder);
            _cachedLabelForeBrush = new SolidBrush(fore);
            _cachedWhiteBrush = new SolidBrush(Color.White);
            _cachedBadgeBrush = new SolidBrush(theme?.AccentColor ?? Color.DeepSkyBlue);

            _cachedBorderPen = new Pen(ResolveBorderColor(), SearchBoxBorderWidth);
            _cachedSearchFocusBorderPen = new Pen(theme?.AccentColor ?? Color.DeepSkyBlue, SearchBoxBorderWidth);
            _cachedSeparatorPen = separator == Color.Empty
                ? null
                : new Pen(separator, 1);
            _cachedChevronPen = new Pen(fore, 1.5f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
            };
        }

        private void DisposeCache()
        {
            _cachedBackBrush?.Dispose();
            _cachedHoverBrush?.Dispose();
            _cachedPressedBrush?.Dispose();
            _cachedSearchBackBrush?.Dispose();
            _cachedSearchFocusBackBrush?.Dispose();
            _cachedSearchForeBrush?.Dispose();
            _cachedSearchPlaceholderBrush?.Dispose();
            _cachedLabelForeBrush?.Dispose();
            _cachedWhiteBrush?.Dispose();
            _cachedBadgeBrush?.Dispose();
            _cachedBorderPen?.Dispose();
            _cachedSeparatorPen?.Dispose();
            _cachedChevronPen?.Dispose();
            _cachedSearchFocusBorderPen?.Dispose();
            _cachedTitleFont?.Dispose();
            _cachedBadgeFont?.Dispose();
            _searchBoxPath?.Dispose();
            _searchBoxPath = null;
            _cachedBackBrush = _cachedHoverBrush = _cachedPressedBrush = _cachedSearchBackBrush =
                _cachedSearchFocusBackBrush = _cachedSearchForeBrush = _cachedSearchPlaceholderBrush =
                _cachedLabelForeBrush = _cachedWhiteBrush = _cachedBadgeBrush = null;
            _cachedBorderPen = _cachedSeparatorPen = _cachedChevronPen = _cachedSearchFocusBorderPen = null;
            _cachedTitleFont = _cachedBadgeFont = null;
        }

                /// <summary>
        /// Returns <paramref name="propertyValue"/> when it is not
        /// <see cref="Color.Empty"/>, otherwise the <paramref name="fallback"/>.
        /// The grid properties default to <c>Color.Empty</c> (sentinel = "use
        /// theme"); the fallback is the theme-resolved color from
        /// <see cref="EnsurePaintCache"/>'s theme resolution step.
        /// </summary>
        private static Color ResolveToolbarColor(Color propertyValue, Color fallback)
            => propertyValue != Color.Empty ? propertyValue : fallback;

        /// <summary>Bootstrap-light fallback palette used when no theme is active.
        /// These are the same colours the grid shipped with before the theme-aware
        /// refactor so existing hosts see no visual change.</summary>
        private static class ThemeFallback
        {
            public static readonly Color Back         = Color.FromArgb(248, 249, 250);
            public static readonly Color Fore         = Color.FromArgb(33, 37, 41);
            public static readonly Color Placeholder  = Color.FromArgb(150, 150, 150);
            public static readonly Color SearchBack   = Color.White;
            public static readonly Color SearchFocusBack = Color.FromArgb(240, 245, 255);
            public static readonly Color Border       = Color.FromArgb(200, 200, 200);
            public static readonly Color Hover        = Color.FromArgb(230, 235, 240);
            public static readonly Color Pressed      = Color.FromArgb(210, 220, 230);
            public static readonly Color Separator    = Color.FromArgb(220, 220, 220);
        }

        private Color ResolveBorderColor()
        {
            var theme = Theme;
            if (theme?.GridLineColor is { } lineColor && !lineColor.IsEmpty)
                return lineColor;
            var back = theme?.GridBackColor ?? Color.White;
            return back.GetBrightness() < 0.5 ? Color.FromArgb(60, 70, 85) : Color.FromArgb(180, 180, 180);
        }

        // ─── Section painters ───────────────────────────────────────

        private void PaintGridTitle(Graphics g, BeepGridToolbarState state)
        {
            if (!state.ShowGridTitle || string.IsNullOrEmpty(state.GridTitle) || state.TitleSectionRect.IsEmpty) return;
            const TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

            // Guard the title against a theme whose toolbar foreground and background are too close
            // to tell apart; the theme's own colour is used whenever it is legible.
            var titleColor = TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.EnsureReadable(
                _grid.ToolbarForeColor, _grid.ToolbarBackColor);
            TextRenderer.DrawText(g, state.GridTitle, _cachedTitleFont!, state.TitleSectionRect, titleColor, flags);
        }

        private void PaintActionButtons(Graphics g, BeepGridToolbarState state)
            => PaintButtonList(g, state, state.ActionButtons, labeled: true);

        private void PaintExportButtons(Graphics g, BeepGridToolbarState state)
            => PaintButtonList(g, state, state.ExportButtons, labeled: false);

        private void PaintButtonList(Graphics g, BeepGridToolbarState state,
            List<ToolbarButtonItem> buttons, bool labeled)
        {
            foreach (var btn in buttons)
            {
                if (!btn.IsVisible || btn.IsOverflow || btn.Bounds.IsEmpty) continue;
                if (labeled) DrawLabeledIconButton(g, state, btn);
                else DrawIconOnlyButton(g, state, btn);
            }
        }

        private void PaintFilterSection(Graphics g, BeepGridToolbarState state)
        {
            // The standalone Filter button is opt-in (ShowFilterButton).
            // When it is hidden (the default), the active-filter colour
            // is still visible on the Advanced button via the badge.
            if (!state.FilterButtonRect.IsEmpty)
            {
                // An active filter reads as the theme's accent, not merely as a more opaque grey —
                // it is state the user needs to spot at a glance.
                DrawCenteredTintedIcon(g, state, state.FilterButtonRect,
                    SvgsUIcons.Common.Filter,
                    state.IsFilterActive ? 1f : 0.6f,
                    state.IsFilterActive ? (Theme?.AccentColor ?? Color.DodgerBlue) : null);
            }
            DrawCenteredTintedIcon(g, state, state.AdvancedButtonRect,
                SvgsUIcons.Common.Settings, 0.6f);

            if (state.IsFilterActive)
            {
                // The clear-filter chip is painted the same way as an
                // icon-only toolbar button, but the hover/pressed key
                // is fixed (KeyClearFilter) and the icon is the Close
                // glyph.  Inlined here so the helper DrawIconOnlyButton
                // is the single source of truth for the "icon-only
                // button" pattern.
                DrawHoverPressState(g, state, state.ClearFilterRect,
                    isHovered: state.HoveredButtonKey == BeepGridToolbarState.KeyClearFilter,
                    isPressed: state.PressedButtonKey == BeepGridToolbarState.KeyClearFilter);
                int iconSize = ScaledIconSize(state);
                PaintIcon(g, CenterIconInBounds(state.ClearFilterRect, iconSize),
                    SvgsUIcons.Common.Close, 0.8f);
            }
            if (state.ActiveFilterCount > 0) PaintBadge(g, state.BadgeRect, state.ActiveFilterCount);
        }

        private void PaintSearchSection(Graphics g, BeepGridToolbarState state)
        {
            // Box first, icon second: the search icon now sits INSIDE the box, so painting it
            // before the box fill would bury it under the background.
            PaintSearchBox(g, state.SearchBoxRect, state.SearchText, state.SearchHasFocus);
            DrawCenteredTintedIcon(g, state, state.SearchIconRect,
                SvgsUIcons.Common.Search, 0.7f);
        }

        /// <summary>Corner radius of the painted search box, in logical pixels.</summary>
        internal const int SearchBoxRadius = 4;

        /// <summary>
        /// The text area inside a painted search box: past the icon on the left, and clear of the
        /// border and corner arc on the right.
        /// </summary>
        /// <remarks>
        /// One definition, used by the painter for the placeholder and the typed value, and by
        /// <c>FilterEditorHelper</c> to size the real editor. They previously computed it
        /// separately and disagreed at the right edge - the painter ran to the box's edge while the
        /// editor stopped short of the arc - so text shifted by a few pixels at the moment the
        /// editor opened or closed.
        /// </remarks>
        internal static Rectangle SearchTextArea(Rectangle box, int iconInset, float dpiScale)
        {
            int left = (int)(iconInset * dpiScale);
            int edge = Math.Max(1, (int)Math.Ceiling(SearchBoxBorderWidth * dpiScale));
            int rightInset = Math.Max(edge, (int)Math.Ceiling(SearchBoxRadius * dpiScale));

            int x = box.X + left;
            return new Rectangle(
                x,
                box.Y + edge,
                Math.Max(0, box.Right - rightInset - x),
                Math.Max(0, box.Height - edge * 2));
        }

        /// <summary>Stroke width of the painted search box border, in logical pixels.</summary>
        internal const int SearchBoxBorderWidth = 1;

        private void PaintSearchBox(Graphics g, Rectangle bounds, string text, bool hasFocus)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            const int radius = SearchBoxRadius;
            // Reuse the path; recreate only on bounds change.
            if (_searchBoxPath == null) _searchBoxPath = new GraphicsPath();
            _searchBoxPath.Reset();
            BuildRoundedRectPath(_searchBoxPath, bounds, radius);
            // Use the focus-aware cached brushes.  Painting a new SolidBrush
            // per paint allocated two GDI handles that the GC would have
            // to finalize — with a repainting toolbar the per-paint cost
            // was visible in profiling.
            // While the real editor is up, fill the box with the editor's own background so the
            // painted box and the control sitting inside it are the same colour. BeepTextBox owns
            // its BackColor (ApplyTheme rewrites it), so matching it here is the only way to get
            // one seamless box without fighting the theme -- see FilterEditorHelper.
            var editorBack = _grid.FilterEditor?.SearchEditorBackColor;
            if (editorBack is Color liveEditorBack)
            {
                using var editorBrush = new SolidBrush(liveEditorBack);
                g.FillPath(editorBrush, _searchBoxPath);
            }
            else
            {
                var bgBrush = hasFocus ? _cachedSearchFocusBackBrush! : _cachedSearchBackBrush!;
                g.FillPath(bgBrush, _searchBoxPath);
            }
            // The non-focus border matches the bottom border (caller has
            // already cached _cachedBorderPen).  When focused, the accent
            // border is also cached so no GDI handle is allocated per paint.
            var borderPen = hasFocus ? _cachedSearchFocusBorderPen! : _cachedBorderPen!;
            g.DrawPath(borderPen, _searchBoxPath);

            // The on-demand search editor (when shown) is sized to cover
            // the right side of the search box only — it does NOT cover
            // the icon.  Painted text + placeholder therefore use the
            // same padding as the editor's bounds offset.
            const TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

            // Shared with the editor, so the placeholder, the painted value and the live editor all
            // occupy the same rectangle. Computing it here separately is what let painted text run
            // under the corner arc while edited text stopped short of it.
            var textRect = SearchTextArea(bounds, _currentSearchIconWidth, _cachedDpiScale);

            // While the real editor is up it owns every pixel of the text area, so the painter must
            // not draw the text or placeholder underneath it. This is the same rule commercial grids
            // follow for in-place editors: the view stops rendering the value once the editor is
            // activated, otherwise painted text and the editor's own text are stacked and every
            // repaint of the toolbar re-draws the string behind a live caret.
            if (_grid.FilterEditor?.IsSearchEditorVisible == true) return;

            if (!string.IsNullOrEmpty(text))
                TextRenderer.DrawText(g, text, _grid.Font, textRect, _grid.ToolbarForeColor, flags);
            else
                TextRenderer.DrawText(g, _grid.SearchPlaceholder, _grid.Font, textRect, _grid.ToolbarPlaceholderColor, flags);
        }



        private void PaintOverflowButton(Graphics g, BeepGridToolbarState state)
        {
            if (state.OverflowButtonRect.IsEmpty || !state.HasOverflowItems) return;
            DrawHoverPressState(g, state, state.OverflowButtonRect,
                isHovered: state.HoveredButtonKey == BeepGridToolbarState.KeyOverflow,
                isPressed: state.PressedButtonKey == BeepGridToolbarState.KeyOverflow);
            // Chevron-down: two lines meeting at the centre.
            var rect = state.OverflowButtonRect;
            int iconSize = ScaledIconSize(state);
            var center = new Point(rect.X + rect.Width / 2, rect.Y + (rect.Height + iconSize) / 2);
            int halfWidth = Math.Max(3, (int)(3 * state.DpiScale));
            int halfHeight = Math.Max(2, (int)(2 * state.DpiScale));
            g.DrawLine(_cachedChevronPen!, center.X - halfWidth, center.Y - halfHeight, center.X, center.Y + halfHeight);
            g.DrawLine(_cachedChevronPen!, center.X + halfWidth, center.Y - halfHeight, center.X, center.Y + halfHeight);
        }

        private void PaintSeparators(Graphics g, BeepGridToolbarState state)
        {
            if (_cachedSeparatorPen == null) return;

            // Extent comes from the advanced button, which is the one element always placed. It
            // used to come from the actions section, or the search section when there were no
            // actions - and the search section can now be empty on a narrow toolbar, which left
            // top and bottom both at zero and drew the separators as nothing at the top-left
            // corner.
            var band = state.AdvancedButtonRect;
            if (band.IsEmpty) return;

            int top = band.Top + 4;
            int bottom = band.Bottom - 4;

            // Each separator is gated on what it actually divides, not on an unrelated list.
            //
            // Separator1 sits between the middle of the toolbar and the filter cluster, and was
            // gated on the *action* buttons being visible. Those are hidden by default, so the
            // line between the search box and the filter buttons was never drawn on a default
            // toolbar - the one place a separator is most obviously wanted.
            bool anythingToTheLeft = !state.SearchSectionRect.IsEmpty
                                     || !state.ActionsSectionRect.IsEmpty
                                     || !state.TitleSectionRect.IsEmpty;

            if (state.Separator1X > 0 && anythingToTheLeft)
                g.DrawLine(_cachedSeparatorPen, state.Separator1X, top, state.Separator1X, bottom);

            // Separator2 divides the filter cluster from the exports, so it belongs to the exports
            // being present - which is what it already asked, and is correct.
            if (state.Separator2X > 0 && !state.ExportSectionRect.IsEmpty)
                g.DrawLine(_cachedSeparatorPen, state.Separator2X, top, state.Separator2X, bottom);

            // Separator3 precedes the overflow chevron, which only exists when something overflowed.
            if (state.Separator3X > 0 && state.HasOverflowItems)
                g.DrawLine(_cachedSeparatorPen, state.Separator3X, top, state.Separator3X, bottom);
        }

        private static bool HasAnyVisibleButton(List<ToolbarButtonItem> buttons)
        {
            foreach (var btn in buttons)
                if (btn.IsVisible && !btn.IsOverflow) return true;
            return false;
        }

        private void PaintBadge(Graphics g, Rectangle bounds, int count)
        {
            g.FillEllipse(_cachedBadgeBrush!, bounds);
            var text = count > 9 ? "9+" : count.ToString();
            const TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
            TextRenderer.DrawText(g, text, _cachedBadgeFont!, bounds, Color.White, flags);
        }

        private void PaintBottomBorder(Graphics g, Rectangle bounds)
        {
            g.DrawLine(_cachedBorderPen!, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
        }

        // ─── Unified button painters ────────────────────────────────

        private void DrawLabeledIconButton(Graphics g, BeepGridToolbarState state, ToolbarButtonItem btn)
        {
            bool isHovered = state.HoveredButtonKey == btn.Key;
            bool isPressed = state.PressedButtonKey == btn.Key;
            DrawHoverPressState(g, state, btn.Bounds, isHovered, isPressed);

            int iconSize = ScaledIconSize(state);
            int iconX = btn.Bounds.X + (int)(4 * state.DpiScale);
            int iconY = btn.Bounds.Y + (btn.Bounds.Height - iconSize) / 2;
            var iconRect = new Rectangle(iconX, iconY, iconSize, iconSize);
            PaintIcon(g, iconRect, ResolveIconPath(btn.IconPath), 0.8f);

            if (string.IsNullOrEmpty(btn.Label)) return;
            var labelRect = new Rectangle(
                iconRect.Right + (int)(2 * state.DpiScale), btn.Bounds.Y,
                btn.Bounds.Right - iconRect.Right - (int)(4 * state.DpiScale), btn.Bounds.Height);
            const TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            TextRenderer.DrawText(g, btn.Label, _cachedLabelFont!, labelRect, _grid.ToolbarForeColor, flags);
        }

        private void DrawIconOnlyButton(Graphics g, BeepGridToolbarState state, ToolbarButtonItem btn)
        {
            DrawHoverPressState(g, state, btn.Bounds,
                isHovered: state.HoveredButtonKey == btn.Key,
                isPressed: state.PressedButtonKey == btn.Key);
            int iconSize = ScaledIconSize(state);
            PaintIcon(g, CenterIconInBounds(btn.Bounds, iconSize), ResolveIconPath(btn.IconPath), 0.8f);
        }

        /// <summary>
        /// Centres an icon in <paramref name="bounds"/> and paints it. Pass
        /// <paramref name="tintOverride"/> to colour a state (an active filter, for instance);
        /// otherwise the icon takes the toolbar's resolved foreground.
        /// </summary>
        private void DrawCenteredTintedIcon(Graphics g, BeepGridToolbarState state, Rectangle bounds,
            string iconPath, float opacity, Color? tintOverride = null)
        {
            int iconSize = ScaledIconSize(state);
            PaintIcon(g, CenterIconInBounds(bounds, iconSize), iconPath, opacity, tintOverride);
        }

        // ─── Visual primitives ───────────────────────────────────────

        /// <summary>Paints the hover/pressed background state.  No icon.</summary>
        private void DrawHoverPressState(Graphics g, BeepGridToolbarState state, Rectangle bounds,
            bool isHovered, bool isPressed)
        {
            if (!isHovered && !isPressed) return;
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            var brush = isPressed ? _cachedPressedBrush! : _cachedHoverBrush!;
            using var path = CreateRoundedPath(bounds, 4);
            g.FillPath(brush, path);
        }

        private int ScaledIconSize(BeepGridToolbarState state) => (int)(16 * state.DpiScale);

        private static Rectangle CenterIconInBounds(Rectangle bounds, int iconSize)
        {
            int iconX = bounds.X + (bounds.Width - iconSize) / 2;
            int iconY = bounds.Y + (bounds.Height - iconSize) / 2;
            return new Rectangle(iconX, iconY, iconSize, iconSize);
        }

        private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0) return path;
            BuildRoundedRectPath(path, rect, radius);
            return path;
        }

        private static void BuildRoundedRectPath(GraphicsPath path, Rectangle rect, int radius)
        {
            int r = Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2));
            int d = r * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddLine(rect.X + r, rect.Y, rect.Right - r, rect.Y);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddLine(rect.Right, rect.Y + r, rect.Right, rect.Bottom - r);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddLine(rect.Right - r, rect.Bottom, rect.X + r, rect.Bottom);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.AddLine(rect.X, rect.Bottom - r, rect.X, rect.Y + r);
            path.CloseFigure();
        }

        // ─── Icon painting ──────────────────────────────────────────

        /// <summary>
        /// Paints a toolbar icon, recoloured to an explicit theme colour.
        /// <para>
        /// This previously built a fresh <see cref="ImagePainter"/> per icon per paint and left the
        /// colour to <c>ApplyThemeOnImage</c> + <c>ImageEmbededin.DataGridView</c>. That produced a
        /// fixed dark grey no matter the theme, so the icons were nearly invisible on dark toolbars
        /// (MaterialYou paints a #101010 toolbar; Cyberpunk asks for cyan and got grey).
        /// </para>
        /// <para>
        /// <see cref="StyledImagePainter.PaintSvgRecolored"/> takes the colour as an argument and
        /// caches the rendered result by path+colour+opacity+size — so a theme change just produces
        /// a different cache key rather than needing invalidation, and repeat paints (the toolbar
        /// repaints on every hover change) become a blit instead of an SVG re-render. It recolours
        /// rather than tints: <c>PaintWithTint</c> multiplies, which cannot lighten a near-black
        /// source glyph, so tinting these icons only ever made them darker.
        /// </para>
        /// </summary>
        private void PaintIcon(Graphics g, Rectangle bounds, string iconPath, float opacity,
            Color? tintOverride = null)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0 || string.IsNullOrEmpty(iconPath)) return;
            StyledImagePainter.PaintSvgRecolored(g, bounds, iconPath, tintOverride ?? ResolveIconColor(), opacity);
        }

        /// <summary>
        /// The colour toolbar icons are drawn in: the toolbar's own foreground, guarded so a theme
        /// pairing it too closely with the toolbar background cannot render the icons invisible.
        /// </summary>
        private Color ResolveIconColor()
            => TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.EnsureReadable(
                ResolveToolbarColor(_grid.ToolbarForeColor, Theme?.ForeColor ?? ThemeFallback.Fore),
                ResolveToolbarColor(_grid.ToolbarBackColor, Theme?.BackgroundColor ?? ThemeFallback.Back));

        // ─── Icon resolution ─────────────────────────────────────────

        // Static icon lookup: keeps the switch out of the hot path and
        // makes it easy to register additional short keys from the host.
        private static readonly Dictionary<string, string> s_iconMap = new()
        {
            ["plus"] = SvgsUIcons.Common.Add,
            ["edit"] = SvgsUIcons.Common.Edit,
            ["trash"] = SvgsUIcons.Common.Delete,
            ["file_upload"] = SvgsUIcons.Common.Upload,
            ["download"] = SvgsUIcons.Common.Download,
            ["print"] = SvgsUIcons.Devices.Printer,
        };

        private static string ResolveIconPath(string? iconKey)
            => string.IsNullOrEmpty(iconKey)
                ? string.Empty
                : s_iconMap.TryGetValue(iconKey, out var path) ? path : iconKey;

        /// <summary>
        /// Public icon-path resolver.  Exposed so the input helper can
        /// build context-menu items with real SVG paths instead of
        /// passing the icon key through (which the menu cannot resolve).
        /// </summary>
        public static string ResolveIconPathPublic(string? iconKey) => ResolveIconPath(iconKey);

        /// <summary>
        /// Disposes the cached brushes / pens / fonts and resets all
        /// snapshot fields so the next paint will rebuild the cache.
        /// Hosts can call this when they change a toolbar color outside
        /// of the property setter (e.g. from a theme engine that does
        /// not raise property change notifications) and want the change
        /// to take effect immediately.
        /// </summary>
        public void InvalidatePaintCache()
        {
            DisposeCache();
            _cachedTheme = null;
            _cachedLabelFont = null;
            _cachedBackColor = _cachedForeColor = _cachedPlaceholderColor = _cachedSearchBackColor =
                _cachedSearchFocusBackColor = _cachedBorderColor = _cachedButtonHoverColor =
                _cachedButtonPressedColor = _cachedSeparatorColor = Color.Empty;
        }

        /// <summary>
        /// Disposes the painter's cache.  Call this from the owning
        /// grid's <c>Dispose</c> path so the GDI brushes, pens, and
        /// fonts the painter allocated are released before the grid
        /// handle is destroyed.  Safe to call multiple times.
        /// </summary>
        public void Dispose()
        {
            DisposeCache();
            _cachedTheme = null;
            _cachedLabelFont = null;
        }
    }
}
