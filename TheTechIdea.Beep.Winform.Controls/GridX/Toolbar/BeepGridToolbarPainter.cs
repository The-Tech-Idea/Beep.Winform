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
            var back = _grid.ToolbarBackColor;
            var fore = _grid.ToolbarForeColor;
            var placeholder = _grid.ToolbarPlaceholderColor;
            var searchBack = _grid.ToolbarSearchBackColor;
            var searchFocus = _grid.ToolbarSearchFocusBackColor;
            var border = _grid.ToolbarBorderColor;
            var hover = _grid.ToolbarButtonHoverBackColor;
            var pressed = _grid.ToolbarButtonPressedBackColor;
            var separator = _grid.ToolbarSeparatorColor;
            if (ReferenceEquals(theme, _cachedTheme) && state.DpiScale == _cachedDpiScale
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

            _cachedBorderPen = new Pen(ResolveBorderColor(), 1);
            _cachedSearchFocusBorderPen = new Pen(theme?.AccentColor ?? Color.DeepSkyBlue, 1);
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
            TextRenderer.DrawText(g, state.GridTitle, _cachedTitleFont!, state.TitleSectionRect, _grid.ToolbarForeColor, flags);
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
                DrawCenteredTintedIcon(g, state, state.FilterButtonRect,
                    SvgsUIcons.Common.Filter, state.IsFilterActive ? 1f : 0.6f);
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
            DrawCenteredTintedIcon(g, state, state.SearchIconRect,
                SvgsUIcons.Common.Search, 0.7f);
            PaintSearchBox(g, state.SearchBoxRect, state.SearchText, state.SearchHasFocus);
        }

        private void PaintSearchBox(Graphics g, Rectangle bounds, string text, bool hasFocus)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            const int radius = 4;
            // Reuse the path; recreate only on bounds change.
            if (_searchBoxPath == null) _searchBoxPath = new GraphicsPath();
            _searchBoxPath.Reset();
            BuildRoundedRectPath(_searchBoxPath, bounds, radius);
            // Use the focus-aware cached brushes.  Painting a new SolidBrush
            // per paint allocated two GDI handles that the GC would have
            // to finalize — with a repainting toolbar the per-paint cost
            // was visible in profiling.
            var bgBrush = hasFocus ? _cachedSearchFocusBackBrush! : _cachedSearchBackBrush!;
            g.FillPath(bgBrush, _searchBoxPath);
            // The non-focus border matches the bottom border (caller has
            // already cached _cachedBorderPen).  When focused, the accent
            // border is also cached so no GDI handle is allocated per paint.
            var borderPen = hasFocus ? _cachedSearchFocusBorderPen! : _cachedBorderPen!;
            g.DrawPath(borderPen, _searchBoxPath);

            // The on-demand search editor (when shown) is sized to cover
            // the right side of the search box only — it does NOT cover
            // the icon.  Painted text + placeholder therefore use the
            // same padding as the editor's bounds offset.
            int textPad = (int)(_currentSearchIconWidth * _cachedDpiScale);
            const TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            var textRect = new Rectangle(
                bounds.X + textPad, bounds.Y,
                bounds.Width - textPad, bounds.Height);

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
            int top, bottom;
            if (!state.ActionsSectionRect.IsEmpty)
            {
                top = state.ActionsSectionRect.Top + 4;
                bottom = state.ActionsSectionRect.Bottom - 4;
            }
            else
            {
                top = state.SearchSectionRect.Top;
                bottom = state.SearchSectionRect.Bottom;
            }

            // Reuse the visibility checks we did during layout instead of
            // scanning the lists again.  State keeps IsOverflow up to date
            // in CalculateLayout.
            if (state.Separator1X > 0 && HasAnyVisibleButton(state.ActionButtons))
                g.DrawLine(_cachedSeparatorPen, state.Separator1X, top, state.Separator1X, bottom);
            if (state.Separator2X > 0 && HasAnyVisibleButton(state.ExportButtons))
                g.DrawLine(_cachedSeparatorPen, state.Separator2X, top, state.Separator2X, bottom);
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

        private void DrawCenteredTintedIcon(Graphics g, BeepGridToolbarState state, Rectangle bounds,
            string iconPath, float opacity)
        {
            // The "tint" concept was retired: icon recoloring is now done
            // by the ImagePainter's theme (ApplyThemeOnImage) so the
            // painter no longer needs to know the per-button tint at
            // call time.  Kept the helper because the filter / advanced
            // / search icons all share the same "center, opacity" shape.
            int iconSize = ScaledIconSize(state);
            PaintIcon(g, CenterIconInBounds(bounds, iconSize), iconPath, opacity);
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
        /// Paints a toolbar icon using theme-aware coloring.  Allocates
        /// a fresh <see cref="ImagePainter"/> per call.  The toolbar has
        /// at most ~10 icons per paint, so the per-call allocation is
        /// not a hot spot; profile before pooling.
        /// </summary>
        private void PaintIcon(Graphics g, Rectangle bounds, string iconPath, float opacity)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0 || string.IsNullOrEmpty(iconPath)) return;

            using var painter = new ImagePainter(iconPath);
            if (!painter.HasImage) return;

            var theme = Theme;
            if (theme != null)
            {
                painter.CurrentTheme = theme;
                painter.ImageEmbededin = ImageEmbededin.DataGridView;
                painter.ApplyThemeOnImage = true;
            }
            painter.Opacity = opacity;
            painter.DrawImage(g, bounds);
        }

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
