// BeepDocumentHost.Preview.cs
// Tab preview / peek thumbnail support (Feature 7.1).
//
// When TabPreviewEnabled is true, the host captures a scaled bitmap snapshot of each
// document panel using Control.DrawToBitmap() and stores it in a per-document cache.
// The cache is fed to BeepDocumentTabStrip.ThumbnailProvider so the Rich Tooltip popup
// can display a live preview when the user hovers a tab.
//
// Capture strategy:
//   • Eager: snapshot is taken each time a document becomes active (SetActiveDocument).
//   • On-demand: ThumbnailProvider captures a fresh bitmap the first time it is called
//     for a document that has no cached entry yet.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentHost
    {
        // ─────────────────────────────────────────────────────────────────────
        // State
        // ─────────────────────────────────────────────────────────────────────

        private bool _tabPreviewEnabled;

        /// <summary>documentId → cached thumbnail bitmap.</summary>
        private readonly Dictionary<string, Bitmap> _previewCache
            = new Dictionary<string, Bitmap>(StringComparer.Ordinal);

        // 5.4 — Bounded LRU: front = most-recently-used
        private const int MaxPreviewCacheEntries = 50;
        private readonly LinkedList<string> _previewLruOrder = new();

        // ─────────────────────────────────────────────────────────────────────
        // Public property
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// When true, the tab strip shows a live preview thumbnail of the document panel
        /// inside the rich tooltip popup (requires <c>TooltipMode = Rich</c> on the tab strip).
        /// </summary>
        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.Description(
            "Show a panel thumbnail inside the tab rich-tooltip. " +
            "Requires TooltipMode = Rich on the underlying tab strip.")]
        public bool TabPreviewEnabled
        {
            get => _tabPreviewEnabled;
            set
            {
                _tabPreviewEnabled = value;
                // Wire or unwire the thumbnail provider on the tab strip
                _tabStrip.ThumbnailProvider = value ? GetOrCaptureThumbnail : (Func<string, Bitmap?>?)null;
            }
        }

        /// <summary>
        /// Size at which panel content is captured for the thumbnail.
        /// Defaults to 200 × 120 logical pixels (auto-scaled by DPI).
        /// </summary>
        public Size PreviewCaptureSize { get; set; } = new Size(200, 120);

        // ─────────────────────────────────────────────────────────────────────
        // Thumbnail provider
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the cached (or freshly captured) thumbnail for a document.
        /// Called by <see cref="BeepDocumentTabStrip.ThumbnailProvider"/> on hover.
        /// </summary>
        private Bitmap? GetOrCaptureThumbnail(string documentId)
        {
            if (_previewCache.TryGetValue(documentId, out var cached))
            {
                // 5.4: Promote to MRU front on cache hit
                var node = _previewLruOrder.Find(documentId);
                if (node != null) { _previewLruOrder.Remove(node); _previewLruOrder.AddFirst(documentId); }
                return cached;
            }

            // Lazy capture on first request
            return CaptureSnapshot(documentId);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Capture helpers — called from SetActiveDocument (eager) and on demand
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Captures a scaled bitmap snapshot of the document panel and stores it
        /// in the preview cache.  Safe to call even when <c>TabPreviewEnabled</c> is false;
        /// the cache will simply not be connected to the tab strip.
        /// </summary>
        internal Bitmap? CaptureSnapshot(string documentId)
        {
            if (!_panels.TryGetValue(documentId, out var panel)) return null;
            if (panel.Width <= 0 || panel.Height <= 0)             return null;

            try
            {
                // Capture full-size first
                using var fullBitmap = new Bitmap(panel.Width, panel.Height);
                panel.DrawToBitmap(fullBitmap, new Rectangle(0, 0, panel.Width, panel.Height));

                // Scale to thumbnail size
                int tw = PreviewCaptureSize.Width;
                int th = PreviewCaptureSize.Height;

                var thumb = new Bitmap(tw, th);
                using (var gThumb = Graphics.FromImage(thumb))
                {
                    gThumb.InterpolationMode =
                        System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    gThumb.DrawImage(fullBitmap, 0, 0, tw, th);
                }

                // Replace cache entry
                if (_previewCache.TryGetValue(documentId, out var old))
                    old.Dispose();

                _previewCache[documentId] = thumb;

                // 5.4: Update LRU order and evict least-recently-used entries over the cap
                var existingNode = _previewLruOrder.Find(documentId);
                if (existingNode != null) _previewLruOrder.Remove(existingNode);
                _previewLruOrder.AddFirst(documentId);

                while (_previewLruOrder.Count > MaxPreviewCacheEntries)
                {
                    var evictId = _previewLruOrder.Last!.Value;
                    _previewLruOrder.RemoveLast();
                    if (_previewCache.TryGetValue(evictId, out var evicted))
                    {
                        _previewCache.Remove(evictId);
                        evicted.Dispose();
                    }
                }

                return thumb;
            }
            catch
            {
                // DrawToBitmap can fail if the control is not fully initialised;
                // swallow the exception and return null (no preview).
                return null;
            }
        }

        /// <summary>
        /// Invalidates the cached thumbnail for a document so it is recaptured
        /// on the next hover.  Call this after significant content changes.
        /// </summary>
        public void InvalidatePreview(string documentId)
        {
            if (_previewCache.TryGetValue(documentId, out var bmp))
            {
                bmp.Dispose();
                _previewCache.Remove(documentId);
            }
            // 5.4: keep LRU list in sync
            var node = _previewLruOrder.Find(documentId);
            if (node != null) _previewLruOrder.Remove(node);
        }

        /// <summary>Clears all cached thumbnails.</summary>
        public void ClearPreviewCache()
        {
            foreach (var bmp in _previewCache.Values)
                bmp.Dispose();
            _previewCache.Clear();
            _previewLruOrder.Clear();   // 5.4: keep LRU list in sync
        }

        // ─────────────────────────────────────────────────────────────────────
        // Disposal
        // ─────────────────────────────────────────────────────────────────────

        private void DisposePreviewCache()
            => ClearPreviewCache();
    }
}
