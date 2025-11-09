using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Internal tooltip instance management with proper resource disposal
    /// Handles the lifecycle of a single tooltip: creation, display, updates, and disposal
    /// Implements IDisposable for proper cleanup
    /// </summary>
    internal class ToolTipInstance : IDisposable
    {
        #region Fields

        private readonly ToolTipConfig _config;
        private readonly DateTime _createdAt;
        private CustomToolTip _tooltip;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed;

        #endregion

        #region Constructor

        public ToolTipInstance(ToolTipConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _createdAt = DateTime.UtcNow;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Show the tooltip with animation
        /// </summary>
        public async Task ShowAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ToolTipInstance));
            }

            try
            {
                // Create new tooltip form (inherits from BeepiFormPro)
                _tooltip = new CustomToolTip();
                _tooltip.ApplyConfig(_config);
                
                // Invoke show callback
                try
                {
                    _config.OnShow?.Invoke(_config.Key);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ToolTipInstance] Error in OnShow callback: {ex.Message}");
                }
                
                // Display tooltip with cancellation support
                await _tooltip.ShowAsync(_config.Position, _cancellationTokenSource.Token);

                // Schedule auto-hide if duration is set
                if (_config.Duration > 0 && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    _ = Task.Delay(_config.Duration, _cancellationTokenSource.Token)
                        .ContinueWith(async t =>
                        {
                            if (!t.IsCanceled && !_disposed)
                            {
                                await HideAsync();
                            }
                        }, TaskContinuationOptions.OnlyOnRanToCompletion);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled - suppress
            }
            catch (ObjectDisposedException)
            {
                // Tooltip was disposed during show - suppress
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipInstance] Error showing tooltip: {ex.Message}");
                
                // Cleanup on error
                CleanupTooltip();
                throw;
            }
        }

        /// <summary>
        /// Hide and dispose the tooltip
        /// </summary>
        public async Task HideAsync()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                // Cancel any pending operations
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                }
                
                // Hide tooltip with animation
                if (_tooltip != null && !_tooltip.IsDisposed)
                {
                    await _tooltip.HideAsync();
                }

                // Invoke close callback
                try
                {
                    _config.OnClose?.Invoke(_config.Key);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ToolTipInstance] Error in OnClose callback: {ex.Message}");
                }
            }
            catch (ObjectDisposedException)
            {
                // Tooltip already disposed - suppress
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipInstance] Error hiding tooltip: {ex.Message}");
            }
            finally
            {
                // Ensure cleanup
                CleanupTooltip();
            }
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Update tooltip content dynamically
        /// </summary>
        public void UpdateContent(string text, string title = null)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                if (_tooltip != null && !_tooltip.IsDisposed)
                {
                    _config.Text = text;
                    if (title != null)
                    {
                        _config.Title = title;
                    }
                    _tooltip.ApplyConfig(_config);
                }
            }
            catch (ObjectDisposedException)
            {
                // Tooltip disposed - suppress
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipInstance] Error updating content: {ex.Message}");
            }
        }

        /// <summary>
        /// Update tooltip position (for follow cursor scenarios)
        /// </summary>
        public void UpdatePosition(Point position)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                if (_tooltip != null && !_tooltip.IsDisposed)
                {
                    _config.Position = position;
                    _tooltip.UpdatePosition(position);
                }
            }
            catch (ObjectDisposedException)
            {
                // Tooltip disposed - suppress
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipInstance] Error updating position: {ex.Message}");
            }
        }

        #endregion

        #region State Methods

        /// <summary>
        /// Check if this instance has expired and should be cleaned up
        /// </summary>
        public bool IsExpired(DateTime now)
        {
            if (_disposed)
            {
                return true;
            }

            if (_config.Duration <= 0)
            {
                return false; // Indefinite duration
            }

            // Allow extra buffer time before cleanup (2x duration)
            var expiryThreshold = _config.Duration * 2;
            return (now - _createdAt).TotalMilliseconds > expiryThreshold;
        }

        /// <summary>
        /// Check if the tooltip is currently visible
        /// </summary>
        public bool IsVisible
        {
            get
            {
                try
                {
                    return !_disposed && _tooltip != null && !_tooltip.IsDisposed && _tooltip.Visible;
                }
                catch (ObjectDisposedException)
                {
                    return false;
                }
            }
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Internal cleanup of tooltip form
        /// </summary>
        private void CleanupTooltip()
        {
            if (_tooltip != null)
            {
                try
                {
                    if (!_tooltip.IsDisposed)
                    {
                        _tooltip.Dispose();
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Already disposed - suppress
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ToolTipInstance] Error disposing tooltip: {ex.Message}");
                }
                finally
                {
                    _tooltip = null;
                }
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Clean up resources
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            // Cancel pending operations
            try
            {
                _cancellationTokenSource?.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // Already disposed - suppress
            }

            // Dispose cancellation token source
            try
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipInstance] Error disposing cancellation token: {ex.Message}");
            }

            // Cleanup tooltip
            CleanupTooltip();

            // Suppress finalization
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer for cleanup if Dispose is not called
        /// </summary>
        ~ToolTipInstance()
        {
            Dispose();
        }

        #endregion
    }
}
