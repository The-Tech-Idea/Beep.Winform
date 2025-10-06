using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Internal tooltip instance management
    /// Handles the lifecycle of a single tooltip: creation, display, updates, and disposal
    /// Based on React component lifecycle and modern UI framework patterns
    /// </summary>
    internal class ToolTipInstance
    {
        #region Fields

        private readonly ToolTipConfig _config;
        private readonly DateTime _createdAt;
        private CustomToolTip _tooltip;
        private CancellationTokenSource _cancellationTokenSource;

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
            try
            {
                _tooltip = new CustomToolTip();
                _tooltip.ApplyConfig(_config);
                
                // Invoke show callback
                _config.OnShow?.Invoke(_config.Key);
                
                // Display tooltip
                await _tooltip.ShowAsync(_config.Position, _cancellationTokenSource.Token);

                // Schedule auto-hide if duration is set
                if (_config.Duration > 0)
                {
                    _ = Task.Delay(_config.Duration, _cancellationTokenSource.Token)
                        .ContinueWith(async t =>
                        {
                            if (!t.IsCanceled)
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipInstance] Error showing tooltip: {ex.Message}");
            }
        }

        /// <summary>
        /// Hide and dispose the tooltip
        /// </summary>
        public async Task HideAsync()
        {
            try
            {
                // Cancel any pending operations
                _cancellationTokenSource?.Cancel();
                
                if (_tooltip != null)
                {
                    await _tooltip.HideAsync();
                    _tooltip.Dispose();
                    _tooltip = null;
                }

                // Invoke close callback
                _config.OnClose?.Invoke(_config.Key);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ToolTipInstance] Error hiding tooltip: {ex.Message}");
            }
            finally
            {
                // Ensure cleanup
                _tooltip?.Dispose();
                _tooltip = null;
            }
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Update tooltip content dynamically
        /// </summary>
        public void UpdateContent(string text, string title = null)
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

        /// <summary>
        /// Update tooltip position (for follow cursor scenarios)
        /// </summary>
        public void UpdatePosition(Point position)
        {
            if (_tooltip != null && !_tooltip.IsDisposed)
            {
                _config.Position = position;
                _tooltip.UpdatePosition(position);
            }
        }

        #endregion

        #region State Methods

        /// <summary>
        /// Check if this instance has expired and should be cleaned up
        /// </summary>
        public bool IsExpired(DateTime now)
        {
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
        public bool IsVisible => _tooltip != null && !_tooltip.IsDisposed && _tooltip.Visible;

        #endregion

        #region Disposal

        /// <summary>
        /// Clean up resources
        /// </summary>
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _tooltip?.Dispose();
        }

        #endregion
    }
}
