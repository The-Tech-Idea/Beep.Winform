// BeepDocumentDragData.cs
// Cross-host drag-to-transfer infrastructure (Feature 7.2).
//
// All BeepDocumentHost instances register themselves with BeepDocumentDragManager on
// construction so that, when a tab is dragged out of one host, the manager can detect
// whether the drop point is over another host and route the panel there instead of
// creating a floating window.
//
// No OLE / cross-process drag is involved — this is a purely in-process position check.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    // ─────────────────────────────────────────────────────────────────────────
    // Event args
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Raised on <see cref="BeepDocumentHost.DocumentDetaching"/> just before a document
    /// is transferred to another host.  Set <c>Cancel = true</c> to prevent the transfer
    /// and fall through to the normal float behaviour.
    /// </summary>
    public sealed class DocumentTransferEventArgs : EventArgs
    {
        /// <summary>The document being transferred.</summary>
        public string DocumentId { get; }

        /// <summary>The host that will receive the document.</summary>
        public BeepDocumentHost TargetHost { get; }

        /// <summary>Set to true inside a handler to cancel the transfer.</summary>
        public bool Cancel { get; set; }

        public DocumentTransferEventArgs(string documentId, BeepDocumentHost targetHost)
        {
            DocumentId = documentId;
            TargetHost = targetHost;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Manager
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Static registry of all live <see cref="BeepDocumentHost"/> instances used to
    /// support cross-host document dragging.
    /// </summary>
    public static class BeepDocumentDragManager
    {
        private static readonly List<WeakReference<BeepDocumentHost>> _hosts
            = new List<WeakReference<BeepDocumentHost>>();

        // Active drag session state
        private static BeepDocumentHost? _dragSource;
        private static string?           _dragDocumentId;

        // ── Registration ─────────────────────────────────────────────────────

        /// <summary>
        /// Called from <see cref="BeepDocumentHost"/> constructor to register the instance.
        /// </summary>
        internal static void Register(BeepDocumentHost host)
        {
            lock (_hosts) _hosts.Add(new WeakReference<BeepDocumentHost>(host));
        }

        /// <summary>
        /// Called from <see cref="BeepDocumentHost.Dispose"/> to unregister the instance.
        /// </summary>
        internal static void Unregister(BeepDocumentHost host)
        {
            lock (_hosts)
                _hosts.RemoveAll(wr =>
                    !wr.TryGetTarget(out var t) || ReferenceEquals(t, host));
        }

        // ── Hit-test ─────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the first registered <see cref="BeepDocumentHost"/> whose screen
        /// bounds contain <paramref name="screenPt"/>, or null if none is found.
        /// </summary>
        /// <param name="screenPt">Screen coordinate to test.</param>
        /// <param name="exclude">Host to exclude from the search (the drag source).</param>
        public static BeepDocumentHost? FindHostAtPoint(Point screenPt,
                                                         BeepDocumentHost? exclude = null)
        {
            lock (_hosts)
            {
                for (int i = _hosts.Count - 1; i >= 0; i--)
                {
                    if (!_hosts[i].TryGetTarget(out var host))
                    {
                        _hosts.RemoveAt(i);
                        continue;
                    }

                    if (ReferenceEquals(host, exclude)) continue;
                    if (!host.IsHandleCreated || !host.Visible) continue;

                    Rectangle screen = host.RectangleToScreen(host.ClientRectangle);
                    if (screen.Contains(screenPt))
                        return host;
                }
            }

            return null;
        }

        // ── Cross-host drag broadcast (Feature 4.5) ──────────────────────────

        /// <summary>
        /// Called by the source host's float-window-moved handler.
        /// Notifies all other registered hosts of the current cursor position so they
        /// can show or hide their own dock overlays.
        /// </summary>
        /// <param name="screenPt">Current screen cursor position.</param>
        /// <param name="source">The host that owns the floating document.</param>
        /// <param name="documentId">The document being dragged.</param>
        internal static void BroadcastFloatWindowMoved(
            Point screenPt, BeepDocumentHost source, string documentId)
        {
            _dragSource     = source;
            _dragDocumentId = documentId;

            List<BeepDocumentHost> snapshot = GetLiveHosts();

            foreach (var host in snapshot)
            {
                if (ReferenceEquals(host, source)) continue;
                host.HandleExternalDragPosition(screenPt, source, documentId);
            }
        }

        /// <summary>
        /// Called when the float window is released.  Asks the host under the cursor
        /// (if any, other than the source) to accept the drop at its current dock zone.
        /// Returns true if another host accepted the drop, false if the source host
        /// should handle it locally.
        /// </summary>
        internal static bool BroadcastFloatWindowDropped(
            Point screenPt, BeepDocumentHost source, string documentId)
        {
            List<BeepDocumentHost> snapshot = GetLiveHosts();

            // Hide overlays on all non-source hosts regardless
            foreach (var host in snapshot)
            {
                if (!ReferenceEquals(host, source))
                    host.HideExternalDragOverlay();
            }

            // Find which non-source host (if any) the cursor is over
            foreach (var host in snapshot)
            {
                if (ReferenceEquals(host, source)) continue;
                if (!host.IsHandleCreated || !host.Visible) continue;

                Rectangle bounds = host.RectangleToScreen(host.ClientRectangle);
                if (!bounds.Contains(screenPt)) continue;

                // Ask the target host to accept the drop
                bool accepted = host.AcceptExternalDrop(source, documentId, screenPt);
                if (accepted)
                {
                    _dragSource     = null;
                    _dragDocumentId = null;
                    return true;
                }
            }

            _dragSource     = null;
            _dragDocumentId = null;
            return false;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static List<BeepDocumentHost> GetLiveHosts()
        {
            var result = new List<BeepDocumentHost>();
            lock (_hosts)
            {
                for (int i = _hosts.Count - 1; i >= 0; i--)
                {
                    if (!_hosts[i].TryGetTarget(out var host))
                        _hosts.RemoveAt(i);
                    else
                        result.Add(host);
                }
            }
            return result;
        }
    }
}
