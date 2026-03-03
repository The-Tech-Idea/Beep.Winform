using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Base class for widget painters providing common functionality.
    /// Integrates with BaseControl's hit area system, supplies DPI scaling helpers,
    /// font lifecycle hooks (via BeepThemesManager.ToFont), and Material-3-style
    /// thin scrollbar rendering.
    /// </summary>
    internal abstract class WidgetPainterBase : IWidgetPainter
    {
        protected BaseControl? Owner;
        protected IBeepTheme? Theme;

        // ----------------------------------------------------------------
        // Initialisation
        // ----------------------------------------------------------------
        public virtual void Initialize(BaseControl owner, IBeepTheme theme)
        {
            Owner = owner;
            Theme = theme;
            RebuildFonts();
        }

        // ----------------------------------------------------------------
        // Abstract draw pipeline
        // ----------------------------------------------------------------
        public abstract WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx);
        public abstract void DrawBackground(Graphics g, WidgetContext ctx);
        public abstract void DrawContent(Graphics g, WidgetContext ctx);
        public abstract void DrawForegroundAccents(Graphics g, WidgetContext ctx);

        // Painters directly register hit areas on the owner (like BeepAppBar)
        public virtual void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit) { }

        // ----------------------------------------------------------------
        // DPI helpers  (use for sizes / spacings / rect dimensions ONLY)
        // For fonts use BeepThemesManager.ToFont(style, applyDpiScaling:true)
        // ----------------------------------------------------------------
        /// <summary>Converts a logical 96-DPI pixel value to physical pixels at the owner's current DPI.</summary>
        protected int Dp(int logicalPx)
        {
            float dpi = Owner?.DeviceDpi ?? 96f;
            return (int)Math.Round(logicalPx * dpi / 96f);
        }

        /// <summary>Converts a logical 96-DPI float value to physical pixels (float) at the owner's current DPI.</summary>
        protected float Dpf(float logicalPx)
        {
            float dpi = Owner?.DeviceDpi ?? 96f;
            return logicalPx * dpi / 96f;
        }

        // ----------------------------------------------------------------
        // Font lifecycle  – subclasses override and call
        // BeepThemesManager.ToFont(style, applyDpiScaling:true)
        // ----------------------------------------------------------------
        /// <summary>
        /// Called by Initialize and OnThemeChanged. Override to create / re-create
        /// cached fonts using BeepThemesManager.ToFont(style, applyDpiScaling: true).
        /// Always dispose old fonts before allocating new ones.
        /// </summary>
        protected virtual void RebuildFonts() { }

        /// <summary>Called when the active theme changes. Re-creates cached fonts.</summary>
        public virtual void OnThemeChanged(IBeepTheme theme)
        {
            Theme = theme;
            RebuildFonts();
        }

        // ----------------------------------------------------------------
        // Scroll helpers
        // ----------------------------------------------------------------
        private const int _scrollBarWidth   = 6;  // 6dp – Material 3 thin track
        private const int _scrollThumbMinH  = 24; // 24dp – minimum thumb height

        /// <summary>Returns true when virtual content height exceeds the visible area.</summary>
        protected bool NeedsVerticalScroll(WidgetContext ctx)
            => ctx.TotalContentHeight > ctx.ContentRect.Height;

        /// <summary>Returns true when virtual content width exceeds the visible area.</summary>
        protected bool NeedsHorizontalScroll(WidgetContext ctx)
            => ctx.TotalContentWidth > ctx.ContentRect.Width;

        /// <summary>
        /// Clamps ScrollOffsetY / ScrollOffsetX to valid range.
        /// Call at the end of AdjustLayout after TotalContentHeight/Width are known.
        /// </summary>
        protected void ClampScrollOffset(WidgetContext ctx)
        {
            int maxY = Math.Max(0, ctx.TotalContentHeight - ctx.ContentRect.Height);
            ctx.ScrollOffsetY = Math.Max(0, Math.Min(ctx.ScrollOffsetY, maxY));
            int maxX = Math.Max(0, ctx.TotalContentWidth - ctx.ContentRect.Width);
            ctx.ScrollOffsetX = Math.Max(0, Math.Min(ctx.ScrollOffsetX, maxX));
        }

        /// <summary>
        /// Draws a thin Material-3-style vertical scrollbar on the right edge of
        /// <paramref name="scrollRect"/>. Call from DrawForegroundAccents.
        /// Does nothing when content fits in the visible area.
        /// </summary>
        protected void DrawVerticalScrollbar(Graphics g, Rectangle scrollRect, WidgetContext ctx, bool hovered = false)
        {
            if (!NeedsVerticalScroll(ctx)) return;

            int tw = Dp(_scrollBarWidth);
            int margin = Dp(2);
            int trackX = scrollRect.Right - tw - margin;
            var trackRect = new Rectangle(trackX, scrollRect.Y + Dp(4), tw, scrollRect.Height - Dp(8));

            // Track
            using var trackBrush = new SolidBrush(Color.FromArgb(20, Color.Black));
            using var trackPath  = CreateRoundedPath(trackRect, Dp(3));
            g.FillPath(trackBrush, trackPath);

            // Thumb  (proportional to visible:total ratio)
            float ratio   = (float)scrollRect.Height / Math.Max(1, ctx.TotalContentHeight);
            int   thumbH  = Math.Max(Dp(_scrollThumbMinH), (int)(trackRect.Height * ratio));
            int   maxY    = trackRect.Bottom - thumbH;
            int   maxScrl = Math.Max(1, ctx.TotalContentHeight - scrollRect.Height);
            int   thumbY  = trackRect.Y + (maxScrl == 0 ? 0 :
                            (int)((float)ctx.ScrollOffsetY / maxScrl * (maxY - trackRect.Y)));

            var thumbRect = new Rectangle(trackRect.X, thumbY, trackRect.Width, thumbH);
            int alpha = hovered ? 140 : 80;
            using var thumbBrush = new SolidBrush(Color.FromArgb(alpha, Color.Black));
            using var thumbPath  = CreateRoundedPath(thumbRect, Dp(3));
            g.FillPath(thumbBrush, thumbPath);
        }

        /// <summary>
        /// Draws a thin Material-3-style horizontal scrollbar on the bottom edge of
        /// <paramref name="scrollRect"/>. Call from DrawForegroundAccents.
        /// Does nothing when content fits in the visible area.
        /// </summary>
        protected void DrawHorizontalScrollbar(Graphics g, Rectangle scrollRect, WidgetContext ctx, bool hovered = false)
        {
            if (!NeedsHorizontalScroll(ctx)) return;

            int th = Dp(_scrollBarWidth);
            int margin = Dp(2);
            int trackY = scrollRect.Bottom - th - margin;
            var trackRect = new Rectangle(scrollRect.X + Dp(4), trackY, scrollRect.Width - Dp(8), th);

            using var trackBrush = new SolidBrush(Color.FromArgb(20, Color.Black));
            using var trackPath  = CreateRoundedPath(trackRect, Dp(3));
            g.FillPath(trackBrush, trackPath);

            float ratio   = (float)scrollRect.Width / Math.Max(1, ctx.TotalContentWidth);
            int   thumbW  = Math.Max(Dp(_scrollThumbMinH), (int)(trackRect.Width * ratio));
            int   maxX    = trackRect.Right - thumbW;
            int   maxScrl = Math.Max(1, ctx.TotalContentWidth - scrollRect.Width);
            int   thumbX  = trackRect.X + (maxScrl == 0 ? 0 :
                            (int)((float)ctx.ScrollOffsetX / maxScrl * (maxX - trackRect.X)));

            var thumbRect = new Rectangle(thumbX, trackRect.Y, thumbW, trackRect.Height);
            int alpha = hovered ? 140 : 80;
            using var thumbBrush = new SolidBrush(Color.FromArgb(alpha, Color.Black));
            using var thumbPath  = CreateRoundedPath(thumbRect, Dp(3));
            g.FillPath(thumbBrush, thumbPath);
        }

        // ----------------------------------------------------------------
        // Hit-area helpers
        // ----------------------------------------------------------------
        protected void AddHitAreaToOwner(string name, Rectangle rect, Action? clickAction = null)
            => Owner?.AddHitArea(name, rect, null, clickAction);

        protected void ClearOwnerHitAreas() => Owner?.ClearHitList();

        protected bool IsAreaHovered(string areaName)
            => Owner?.HitTestControl != null && Owner.HitTestControl.Name == areaName && Owner.HitTestControl.IsHovered;

        protected string? GetHoveredAreaName() => Owner?.HitTestControl?.Name;

        // ----------------------------------------------------------------
        // Rendering helpers (delegates to WidgetRenderingHelpers)
        // ----------------------------------------------------------------
        protected GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
            => WidgetRenderingHelpers.CreateRoundedPath(rect, radius);

        protected void DrawSoftShadow(Graphics g, Rectangle rect, int radius, int layers = 4, int offset = 2)
            => WidgetRenderingHelpers.DrawSoftShadow(g, rect, radius, layers, offset);

        protected void DrawTrendArrow(Graphics g, Rectangle rect, string direction, Color color)
            => WidgetRenderingHelpers.DrawTrendArrow(g, rect, direction, color);

        protected void DrawProgressBar(Graphics g, Rectangle rect, double percentage, Color fillColor, Color backgroundColor)
            => WidgetRenderingHelpers.DrawProgressBar(g, rect, percentage, fillColor, backgroundColor);

        protected void DrawValue(Graphics g, Rectangle rect, string value, string units, Font font, Color color, StringAlignment alignment = StringAlignment.Center)
            => WidgetRenderingHelpers.DrawValue(g, rect, value, units, font, color, alignment);
    }
}