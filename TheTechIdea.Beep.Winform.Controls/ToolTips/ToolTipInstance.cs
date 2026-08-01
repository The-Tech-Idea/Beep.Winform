using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;

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
        private Helpers.ToolTipAutoUpdate _autoUpdate;
        private Helpers.ToolTipEscapeFilter _escapeFilter;
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
                
                // Apply theme colors if available (from BeepThemesManager or BaseControl)
                if (_config.UseBeepThemeColors)
                {
                    var theme = BeepThemesManager.CurrentTheme ?? BeepThemesManager.DefaultTheme;
                    if (theme != null)
                    {
                        _tooltip.ApplyTheme(theme, _config.UseBeepThemeColors);
                    }
                }
                
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

                // Resolve preview content. Fire-and-forget on purpose: the tooltip is already
                // visible showing its skeleton, and awaiting here would delay the show by however
                // long the caller's delegate takes.
                _ = ResolvePreviewAsync();

                // Escape must dismiss while focus is still on the trigger (WCAG 1.4.13
                // "dismissible"). The tooltip's own ProcessCmdKey only fires when the tooltip has
                // focus, which a hover-triggered tooltip never does.
                if (_config.KeyboardTriggerable)
                {
                    _escapeFilter = Helpers.ToolTipEscapeFilter.Install(() =>
                    {
                        if (!_disposed) _ = HideAsync();
                    });
                }

                // Follow the anchor for as long as we are visible. Without this the tooltip is
                // positioned once and then stays put while its anchor scrolls or the window moves.
                if (_config.AutoUpdate && _config.AnchorControl != null && !_config.AnchorControl.IsDisposed)
                {
                    _autoUpdate = new Helpers.ToolTipAutoUpdate(
                        _config.AnchorControl,
                        rect =>
                        {
                            if (_disposed || _tooltip == null || _tooltip.IsDisposed) return;
                            _config.AnchorRect = rect;
                            _tooltip.UpdatePosition(rect);
                        },
                        () => { if (!_disposed) _ = HideAsync(); });
                }

                // Schedule auto-hide if duration is set. A pinned tooltip is exempt: pinning means
                // "keep this until I dismiss it", so a timer closing it would defeat the feature.
                if (_config.Duration > 0 && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    _ = Task.Delay(_config.Duration, _cancellationTokenSource.Token)
                        .ContinueWith(t =>
                        {
                            if (!t.IsCanceled && !_disposed && !_config.IsPinned)
                            {
                                _ = HideAsync();
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
        /// Resolves the preview image exactly once — from <see cref="ToolTipConfig.LoadPreviewAsync"/>
        /// if supplied, otherwise by loading <see cref="ToolTipConfig.PreviewImagePath"/> off the UI
        /// thread — then re-measures and repositions the tooltip around it.
        /// <para>
        /// <c>LoadPreviewAsync</c> was declared, documented as showing "a skeleton placeholder until
        /// the task completes", and invoked by nothing.
        /// </para>
        /// </summary>
        private async Task ResolvePreviewAsync()
        {
            if (_config.ResolvedPreviewImage != null) return;

            Image image = null;
            try
            {
                if (_config.LoadPreviewAsync != null)
                {
                    image = await _config.LoadPreviewAsync().ConfigureAwait(true);
                }
                else if (!string.IsNullOrEmpty(_config.PreviewImagePath))
                {
                    string path = _config.PreviewImagePath;
                    image = await Task.Run(() =>
                    {
                        try { return File.Exists(path) ? Image.FromFile(path) : null; }
                        catch { return null; }
                    }).ConfigureAwait(true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipInstance] Preview load failed: {ex.Message}");
                return;
            }

            if (image == null) return;

            // The tooltip may have been hidden while the delegate was running. Dropping the image
            // on the floor here would leak it, and touching a disposed window would throw.
            if (_disposed || _tooltip == null || _tooltip.IsDisposed)
            {
                image.Dispose();
                return;
            }

            _config.ResolvedPreviewImage = image;

            try
            {
                if (_tooltip.InvokeRequired)
                    _tooltip.BeginInvoke(new Action(() => _tooltip.RefreshContentSize()));
                else
                    _tooltip.RefreshContentSize();
            }
            catch (ObjectDisposedException)
            {
                // Hidden between the check above and the invoke — nothing to update.
            }
        }

        /// <summary>
        /// True when the pointer is currently over the tooltip window itself.
        /// <para>
        /// This is what makes a tooltip "hoverable" per WCAG 1.4.13: the manager checks it before
        /// completing a pending hide, so a user can move onto the tooltip to read it, scroll it, or
        /// click a link inside it.
        /// </para>
        /// </summary>
        internal bool IsPointerOver()
        {
            try
            {
                var tip = _tooltip;
                if (_disposed || tip == null || tip.IsDisposed || !tip.Visible) return false;
                return tip.Bounds.Contains(Cursor.Position);
            }
            catch (ObjectDisposedException)
            {
                return false;
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

        #region Properties

        /// <summary>
        /// Get the tooltip configuration
        /// </summary>
        public ToolTipConfig Config => _config;

        /// <summary>
        /// C8: Get the underlying CustomToolTip form. Used by ToolTipManager
        /// to push theme updates to on-screen tooltips. May return null if
        /// the instance has not been shown yet or has been disposed.
        /// </summary>
        public CustomToolTip ToolTip => _tooltip;

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
            // Unsubscribe from the anchor first. These handlers are attached to controls that
            // outlive the tooltip, so leaving them attached would keep the anchor's whole parent
            // chain wired to a dead tooltip.
            try { _autoUpdate?.Dispose(); } catch { }
            _autoUpdate = null;

            // Message filters are process-wide; leaving one installed outlives the tooltip and
            // keeps its closure alive.
            try { _escapeFilter?.Dispose(); } catch { }
            _escapeFilter = null;

            // We resolved this image, so we own it.
            try { _config.ResolvedPreviewImage?.Dispose(); } catch { }
            _config.ResolvedPreviewImage = null;

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
