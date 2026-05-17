// ContextMenuHandle.cs
// Phase 05 — ContextMenu Lifecycle Hardening.
//
// Sealed IDisposable wrapper returned by ContextMenuManager.ShowNonBlocking.
// Disposing the handle closes the popup if it is still open. Double-dispose
// is a no-op. Disposing from a non-UI thread is safe — the underlying
// ContextMenuManager.CloseMenu handles thread marshalling via the menu
// form's BeginInvoke when InvokeRequired.
//
// See .plans/Menus-Phase-05-ContextMenuLifecycle.md.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Threading;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    /// <summary>
    /// Lifetime handle for a non-blocking context menu shown via
    /// <see cref="ContextMenuManager.ShowNonBlocking"/>. Disposing the
    /// handle closes the popup; double-dispose is safe.
    /// </summary>
    public sealed class ContextMenuHandle : IDisposable
    {
        /// <summary>
        /// A pre-disposed handle returned when <c>ShowNonBlocking</c> fails
        /// to bring up a menu (null items, _isClosingAll race, etc.).
        /// </summary>
        public static readonly ContextMenuHandle Empty = new ContextMenuHandle(null);

        private string _menuId;
        private int _disposed;

        internal ContextMenuHandle(string menuId)
        {
            _menuId = menuId;
            if (menuId == null) _disposed = 1; // Empty handle: nothing to dispose.
        }

        /// <summary>
        /// True once <see cref="Dispose"/> has been called, or when this is
        /// the <see cref="Empty"/> sentinel.
        /// </summary>
        public bool IsClosed => Volatile.Read(ref _disposed) != 0;

        /// <summary>
        /// Closes the underlying popup if it is still open. Safe to call
        /// from any thread; safe to call multiple times.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            var id = Interlocked.Exchange(ref _menuId, null);
            if (string.IsNullOrEmpty(id)) return;

            try { ContextMenuManager.CloseMenu(id); }
            catch { /* non-fatal — handle is dead either way */ }
        }
    }
}
