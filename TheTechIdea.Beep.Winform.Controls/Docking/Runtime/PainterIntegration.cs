using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// Bridges between the docking engine and painter system.
    /// Coordinates rendering of panels, tabs, splitters, and chrome.
    /// </summary>
    public class PainterIntegration : IDisposable
    {
        private IDockingPainter _painter;
        private WindowChrome _chrome;
        private Dictionary<IntPtr, PanelRenderContext> _renderContexts = new();
        private Dictionary<string, Rectangle> _cachedTabBounds = new();
        private bool _isRenderingEnabled = true;
        private bool _disposed = false;

        /// <summary>
        /// Rendering context for a panel.
        /// </summary>
        public class PanelRenderContext
        {
            public string PanelKey { get; set; }
            public IntPtr WindowHandle { get; set; }
            public DockGroup Group { get; set; }
            public Rectangle TotalBounds { get; set; }
            public Rectangle TabStripBounds { get; set; }
            public Rectangle ContentBounds { get; set; }
            public DateTime LastPaintedAt { get; set; }
            public int PaintCount { get; set; }
            public bool IsDirty { get; set; }
        }

        /// <summary>
        /// Initializes painter integration.
        /// </summary>
        public PainterIntegration(IDockingPainter painter, WindowChrome chrome)
        {
            _painter = painter ?? throw new ArgumentNullException(nameof(painter));
            _chrome = chrome ?? throw new ArgumentNullException(nameof(chrome));
        }

        /// <summary>
        /// Registers a panel for rendering.
        /// </summary>
        public void RegisterPanelForRendering(
            string panelKey,
            IntPtr windowHandle,
            DockGroup group,
            Rectangle totalBounds)
        {
            var context = new PanelRenderContext
            {
                PanelKey = panelKey,
                WindowHandle = windowHandle,
                Group = group,
                TotalBounds = totalBounds,
                TabStripBounds = CalculateTabStripBounds(totalBounds),
                ContentBounds = CalculateContentBounds(totalBounds),
                LastPaintedAt = DateTime.MinValue,
                PaintCount = 0,
                IsDirty = true
            };

            _renderContexts[windowHandle] = context;
            Debug.WriteLine($"[PainterIntegration] Registered panel '{panelKey}' for rendering");
        }

        /// <summary>
        /// Unregisters a panel from rendering.
        /// </summary>
        public void UnregisterPanelFromRendering(IntPtr windowHandle)
        {
            if (_renderContexts.Remove(windowHandle))
            {
                Debug.WriteLine($"[PainterIntegration] Unregistered panel rendering for window 0x{windowHandle:X8}");
            }
        }

        /// <summary>
        /// Invalidates a panel for repainting.
        /// </summary>
        public void InvalidatePanel(IntPtr windowHandle)
        {
            if (_renderContexts.TryGetValue(windowHandle, out var context))
            {
                context.IsDirty = true;
                Debug.WriteLine($"[PainterIntegration] Invalidated panel '{context.PanelKey}'");
            }
        }

        /// <summary>
        /// Renders a panel using the painter.
        /// </summary>
        public void RenderPanel(IntPtr windowHandle, Graphics graphics)
        {
            if (!_isRenderingEnabled || !_renderContexts.TryGetValue(windowHandle, out var context))
                return;

            try
            {
                // Paint the background
                graphics.Clear(SystemColors.Control);

                // Paint tab strip if group has multiple panels
                if (context.Group?.Panels.Count > 1)
                {
                    PaintTabStrip(graphics, context);
                }

                // Paint chrome (borders, sizing grips, etc.)
                PaintChrome(graphics, context);

                context.LastPaintedAt = DateTime.UtcNow;
                context.PaintCount++;
                context.IsDirty = false;

                Debug.WriteLine(
                    $"[PainterIntegration] Rendered panel '{context.PanelKey}' " +
                    $"(count: {context.PaintCount})"
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PainterIntegration] Render error for panel '{context.PanelKey}': {ex.Message}");
            }
        }

        /// <summary>
        /// Paints the tab strip for a group.
        /// </summary>
        private void PaintTabStrip(Graphics graphics, PanelRenderContext context)
        {
            if (context.Group == null)
                return;

            var tabStripRect = context.TabStripBounds;

            // Let the chrome handle tab rendering
            _chrome?.DrawTabStrip(graphics, tabStripRect, context.Group);

            // Cache tab bounds for hit-testing
            CacheTabBounds(context);
        }

        /// <summary>
        /// Paints the window chrome (title bar, borders, etc.).
        /// </summary>
        private void PaintChrome(Graphics graphics, PanelRenderContext context)
        {
            _chrome?.DrawChrome(graphics, context.TotalBounds);
        }

        /// <summary>
        /// Caches tab bounds for hit-testing during paint.
        /// </summary>
        private void CacheTabBounds(PanelRenderContext context)
        {
            if (context.Group?.Panels == null)
                return;

            int tabX = context.TabStripBounds.X;
            const int TAB_HEIGHT = 24;
            const int TAB_PADDING = 2;

            foreach (var panel in context.Group.Panels)
            {
                // Simplified tab width calculation
                int tabWidth = 100;

                var tabBounds = new Rectangle(tabX, context.TabStripBounds.Y, tabWidth, TAB_HEIGHT);
                _cachedTabBounds[panel.Key] = tabBounds;

                tabX += tabWidth + TAB_PADDING;
            }
        }

        /// <summary>
        /// Calculates the tab strip bounds for a panel window.
        /// </summary>
        private Rectangle CalculateTabStripBounds(Rectangle totalBounds)
        {
            const int TAB_STRIP_HEIGHT = 24;
            return new Rectangle(totalBounds.X, totalBounds.Y, totalBounds.Width, TAB_STRIP_HEIGHT);
        }

        /// <summary>
        /// Calculates the content area bounds for a panel window.
        /// </summary>
        private Rectangle CalculateContentBounds(Rectangle totalBounds)
        {
            const int TAB_STRIP_HEIGHT = 24;
            return new Rectangle(
                totalBounds.X,
                totalBounds.Y + TAB_STRIP_HEIGHT,
                totalBounds.Width,
                totalBounds.Height - TAB_STRIP_HEIGHT
            );
        }

        /// <summary>
        /// Updates render context bounds (called on resize).
        /// </summary>
        public void UpdatePanelBounds(IntPtr windowHandle, Rectangle newBounds)
        {
            if (_renderContexts.TryGetValue(windowHandle, out var context))
            {
                context.TotalBounds = newBounds;
                context.TabStripBounds = CalculateTabStripBounds(newBounds);
                context.ContentBounds = CalculateContentBounds(newBounds);
                context.IsDirty = true;

                Debug.WriteLine(
                    $"[PainterIntegration] Updated bounds for panel '{context.PanelKey}': {newBounds}"
                );
            }
        }

        /// <summary>
        /// Gets a cached tab bounds.
        /// </summary>
        public Rectangle? GetTabBounds(string panelKey)
        {
            if (_cachedTabBounds.TryGetValue(panelKey, out var bounds))
                return bounds;

            return null;
        }

        /// <summary>
        /// Gets all cached tab bounds.
        /// </summary>
        public IReadOnlyDictionary<string, Rectangle> GetAllTabBounds() => _cachedTabBounds;

        /// <summary>
        /// Enables or disables rendering.
        /// </summary>
        public void SetRenderingEnabled(bool enabled)
        {
            _isRenderingEnabled = enabled;
            Debug.WriteLine($"[PainterIntegration] Rendering {(enabled ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// Gets a render context for a panel.
        /// </summary>
        public PanelRenderContext GetRenderContext(IntPtr windowHandle)
        {
            _renderContexts.TryGetValue(windowHandle, out var context);
            return context;
        }

        /// <summary>
        /// Gets diagnostic information.
        /// </summary>
        public PainterIntegrationDiagnostics GetDiagnostics()
        {
            var panelStats = new List<PanelRenderStat>();
            foreach (var kvp in _renderContexts)
            {
                var context = kvp.Value;
                panelStats.Add(new PanelRenderStat
                {
                    PanelKey = context.PanelKey,
                    WindowHandle = context.WindowHandle,
                    TotalBounds = context.TotalBounds,
                    TabStripBounds = context.TabStripBounds,
                    ContentBounds = context.ContentBounds,
                    PaintCount = context.PaintCount,
                    LastPaintedAtUtc = context.LastPaintedAt,
                    IsDirty = context.IsDirty
                });
            }

            return new PainterIntegrationDiagnostics
            {
                IsRenderingEnabled = _isRenderingEnabled,
                PanelsRegistered = _renderContexts.Count,
                CachedTabBounds = _cachedTabBounds.Count,
                PainterName = _painter?.GetType().Name,
                PanelStats = panelStats
            };
        }

        /// <summary>
        /// Diagnostics result.
        /// </summary>
        public class PainterIntegrationDiagnostics
        {
            public bool IsRenderingEnabled { get; set; }
            public int PanelsRegistered { get; set; }
            public int CachedTabBounds { get; set; }
            public string PainterName { get; set; }
            public List<PanelRenderStat> PanelStats { get; set; }
        }

        /// <summary>
        /// Stat for a rendered panel.
        /// </summary>
        public class PanelRenderStat
        {
            public string PanelKey { get; set; }
            public IntPtr WindowHandle { get; set; }
            public Rectangle TotalBounds { get; set; }
            public Rectangle TabStripBounds { get; set; }
            public Rectangle ContentBounds { get; set; }
            public int PaintCount { get; set; }
            public DateTime LastPaintedAtUtc { get; set; }
            public bool IsDirty { get; set; }
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _renderContexts.Clear();
            _cachedTabBounds.Clear();
            _disposed = true;
            Debug.WriteLine("[PainterIntegration] Disposed");
        }
    }
}
