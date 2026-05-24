using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Interop
{
    /// <summary>
    /// Efficiently batches multiple SetWindowPos operations into a single atomic update.
    /// Uses BeginDeferWindowPos/DeferWindowPos/EndDeferWindowPos to minimize redraws and flicker.
    /// </summary>
    internal sealed class WindowBatchUpdater : IDisposable
    {
        private IntPtr _hWinPosInfo = IntPtr.Zero;
        private List<WindowPositionChange> _changes;
        private bool _disposed = false;

        /// <summary>
        /// Represents a single window position change to be deferred.
        /// </summary>
        private struct WindowPositionChange
        {
            public IntPtr Hwnd;
            public Rectangle Bounds;
            public uint Flags;
        }

        /// <summary>
        /// Creates a new batch updater initialized for the given expected number of windows.
        /// </summary>
        /// <param name="estimatedWindowCount">
        /// Expected number of SetWindowPos calls. If exceeded, performance may degrade slightly.
        /// </param>
        public WindowBatchUpdater(int estimatedWindowCount = 10)
        {
            if (estimatedWindowCount <= 0)
                throw new ArgumentException("Estimated window count must be greater than 0", nameof(estimatedWindowCount));

            _hWinPosInfo = MdiNativeApi.BeginDeferWindowPos(estimatedWindowCount);
            if (_hWinPosInfo == IntPtr.Zero)
                throw new InvalidOperationException(
                    $"Failed to begin deferred window position update: {MdiNativeApi.GetLastErrorMessage()}");

            _changes = new List<WindowPositionChange>(estimatedWindowCount);
        }

        /// <summary>
        /// Queues a window position update (size and/or position).
        /// The actual update is deferred until EndUpdate() is called.
        /// </summary>
        /// <param name="hWnd">Handle to the window to update.</param>
        /// <param name="bounds">Target rectangle for the window.</param>
        /// <param name="flags">SWP_* flags controlling the update behavior.</param>
        public void DeferUpdate(IntPtr hWnd, Rectangle bounds, uint flags = 0)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(WindowBatchUpdater));

            if (hWnd == IntPtr.Zero)
                throw new ArgumentException("Window handle cannot be null", nameof(hWnd));

            // Queue the change for tracking/debugging
            _changes.Add(new WindowPositionChange { Hwnd = hWnd, Bounds = bounds, Flags = flags });

            // Defer to Win32 batch
            _hWinPosInfo = MdiNativeApi.DeferWindowPos(
                _hWinPosInfo,
                hWnd,
                IntPtr.Zero,  // No z-order change
                bounds.X,
                bounds.Y,
                bounds.Width,
                bounds.Height,
                flags);

            if (_hWinPosInfo == IntPtr.Zero)
                throw new InvalidOperationException(
                    $"Failed to defer window position for hwnd {hWnd}: {MdiNativeApi.GetLastErrorMessage()}");
        }

        /// <summary>
        /// Queues a move-only update (no resize).
        /// Convenience overload that sets SWP_NOSIZE automatically.
        /// </summary>
        public void DeferMove(IntPtr hWnd, int x, int y)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(WindowBatchUpdater));

            _hWinPosInfo = MdiNativeApi.DeferWindowPos(
                _hWinPosInfo,
                hWnd,
                IntPtr.Zero,
                x,
                y,
                0,
                0,
                MdiConstants.SWP_NOSIZE);

            if (_hWinPosInfo == IntPtr.Zero)
                throw new InvalidOperationException(
                    $"Failed to defer window move for hwnd {hWnd}: {MdiNativeApi.GetLastErrorMessage()}");
        }

        /// <summary>
        /// Queues a resize-only update (no move).
        /// Convenience overload that sets SWP_NOMOVE automatically.
        /// </summary>
        public void DeferResize(IntPtr hWnd, int width, int height)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(WindowBatchUpdater));

            _hWinPosInfo = MdiNativeApi.DeferWindowPos(
                _hWinPosInfo,
                hWnd,
                IntPtr.Zero,
                0,
                0,
                width,
                height,
                MdiConstants.SWP_NOMOVE);

            if (_hWinPosInfo == IntPtr.Zero)
                throw new InvalidOperationException(
                    $"Failed to defer window resize for hwnd {hWnd}: {MdiNativeApi.GetLastErrorMessage()}");
        }

        /// <summary>
        /// Gets the number of deferred updates queued so far.
        /// </summary>
        public int ChangeCount => _changes.Count;

        /// <summary>
        /// Applies all deferred window position changes atomically.
        /// This is the expensive operation; batch as many changes as possible before calling.
        /// </summary>
        /// <returns>True if the batch update succeeded; false if the operation failed.</returns>
        public bool EndUpdate()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(WindowBatchUpdater));

            if (_hWinPosInfo == IntPtr.Zero)
                return true;  // Nothing to update

            bool result = MdiNativeApi.EndDeferWindowPos(_hWinPosInfo);
            _hWinPosInfo = IntPtr.Zero;

            if (!result)
            {
                // Log error but don't throw; partial updates may have succeeded
                string errorMsg = MdiNativeApi.GetLastErrorMessage();
                System.Diagnostics.Debug.WriteLine($"WindowBatchUpdater.EndUpdate failed: {errorMsg}");
            }

            return result;
        }

        /// <summary>
        /// Gets a diagnostic string describing all queued changes.
        /// Useful for debugging layout issues.
        /// </summary>
        public string GetDiagnostics()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"WindowBatchUpdater: {_changes.Count} changes queued");

            for (int i = 0; i < _changes.Count; i++)
            {
                var change = _changes[i];
                sb.AppendLine($"  [{i}] hwnd=0x{change.Hwnd:X8}, bounds={change.Bounds}, flags=0x{change.Flags:X4}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Disposes the batch updater, cancelling any pending updates.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            // If there are pending changes and EndUpdate wasn't called, we're discarding them
            _hWinPosInfo = IntPtr.Zero;
            _changes.Clear();
            _disposed = true;
        }
    }
}
