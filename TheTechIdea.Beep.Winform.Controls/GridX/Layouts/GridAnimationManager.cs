using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Manages animations for grid operations
    /// Provides smooth transitions for row insert/delete, cell updates, and other operations
    /// </summary>
    public class GridAnimationManager : IDisposable
    {
        private BeepGridPro _grid;
        private AnimationConfig _config;
        private Dictionary<string, CellAnimation> _cellAnimations = new Dictionary<string, CellAnimation>();
        private Dictionary<int, RowAnimation> _rowAnimations = new Dictionary<int, RowAnimation>();
        private Timer _animationTimer;
        private bool _isInitialized = false;
        
        /// <summary>
        /// Initializes the animation manager
        /// </summary>
        public void Initialize(BeepGridPro grid, AnimationConfig config)
        {
            if (_isInitialized)
                throw new InvalidOperationException("GridAnimationManager is already initialized");
            
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            
            // Initialize animation timer (60 FPS)
            _animationTimer = new Timer { Interval = 16 };
            _animationTimer.Tick += OnAnimationTick;
            
            _isInitialized = true;
        }
        
        /// <summary>
        /// Animates a row insertion
        /// </summary>
        public void AnimateRowInsert(int rowIndex)
        {
            if (!_config.Enabled || !_config.AnimateRowInsert)
                return;
            
            var animation = new RowAnimation
            {
                RowIndex = rowIndex,
                Type = AnimationType.FadeIn,
                StartTime = DateTime.Now,
                Duration = _config.AnimationDuration,
                EasingFunction = _config.EasingFunction,
                CurrentProgress = 0f
            };
            
            _rowAnimations[rowIndex] = animation;
            StartAnimationTimer();
        }
        
        /// <summary>
        /// Animates a row deletion
        /// </summary>
        public void AnimateRowDelete(int rowIndex, Action onComplete = null)
        {
            if (!_config.Enabled || !_config.AnimateRowDelete)
            {
                onComplete?.Invoke();
                return;
            }
            
            var animation = new RowAnimation
            {
                RowIndex = rowIndex,
                Type = AnimationType.FadeOut,
                StartTime = DateTime.Now,
                Duration = _config.AnimationDuration,
                EasingFunction = _config.EasingFunction,
                CurrentProgress = 0f,
                OnComplete = onComplete
            };
            
            _rowAnimations[rowIndex] = animation;
            StartAnimationTimer();
        }
        
        /// <summary>
        /// Animates a cell update with highlight effect
        /// </summary>
        public void AnimateCellUpdate(int rowIndex, string columnName)
        {
            if (!_config.Enabled || !_config.AnimateCellUpdate)
                return;
            
            string key = $"{rowIndex}:{columnName}";
            
            var animation = new CellAnimation
            {
                RowIndex = rowIndex,
                ColumnName = columnName,
                Type = AnimationType.Highlight,
                StartTime = DateTime.Now,
                Duration = _config.CellHighlightDuration,
                CurrentProgress = 0f
            };
            
            _cellAnimations[key] = animation;
            StartAnimationTimer();
        }
        
        /// <summary>
        /// Animation timer tick handler
        /// </summary>
        private void OnAnimationTick(object sender, EventArgs e)
        {
            bool hasActiveAnimations = false;
            
            // Update row animations
            var completedRows = new List<int>();
            foreach (var kvp in _rowAnimations.ToList())
            {
                var animation = kvp.Value;
                var elapsed = (DateTime.Now - animation.StartTime).TotalMilliseconds;
                var progress = Math.Min(1.0, elapsed / animation.Duration);
                
                // Apply easing
                animation.CurrentProgress = EasingFunctions.Apply((float)progress, animation.EasingFunction);
                
                if (progress >= 1.0)
                {
                    completedRows.Add(kvp.Key);
                    animation.OnComplete?.Invoke();
                }
                else
                {
                    hasActiveAnimations = true;
                }
            }
            
            // Remove completed row animations
            foreach (var key in completedRows)
            {
                _rowAnimations.Remove(key);
            }
            
            // Update cell animations
            var completedCells = new List<string>();
            foreach (var kvp in _cellAnimations.ToList())
            {
                var animation = kvp.Value;
                var elapsed = (DateTime.Now - animation.StartTime).TotalMilliseconds;
                var progress = Math.Min(1.0, elapsed / animation.Duration);
                
                // Highlight fades out over time
                animation.CurrentProgress = 1.0f - (float)progress;
                
                if (progress >= 1.0)
                {
                    completedCells.Add(kvp.Key);
                }
                else
                {
                    hasActiveAnimations = true;
                }
            }
            
            // Remove completed cell animations
            foreach (var key in completedCells)
            {
                _cellAnimations.Remove(key);
            }
            
            // Stop timer if no active animations
            if (!hasActiveAnimations && _cellAnimations.Count == 0 && _rowAnimations.Count == 0)
            {
                _animationTimer.Stop();
            }
            
            // Request repaint
            _grid?.Invalidate();
        }
        
        /// <summary>
        /// Gets the opacity for a row (0-1) based on active animations
        /// </summary>
        public float GetRowOpacity(int rowIndex)
        {
            if (_rowAnimations.TryGetValue(rowIndex, out var animation))
            {
                return animation.Type == AnimationType.FadeIn 
                    ? animation.CurrentProgress 
                    : 1 - animation.CurrentProgress;
            }
            return 1.0f;
        }
        
        /// <summary>
        /// Gets the highlight intensity for a cell (0-1) based on active animations
        /// </summary>
        public float GetCellHighlight(int rowIndex, string columnName)
        {
            string key = $"{rowIndex}:{columnName}";
            
            if (_cellAnimations.TryGetValue(key, out var animation))
            {
                return animation.CurrentProgress;
            }
            return 0f;
        }
        
        /// <summary>
        /// Gets the highlight color for cell animations
        /// </summary>
        public Color GetCellHighlightColor()
        {
            return _config.CellHighlightColor ?? Color.FromArgb(255, 255, 200); // Default yellow
        }
        
        /// <summary>
        /// Checks if a row has an active animation
        /// </summary>
        public bool IsRowAnimating(int rowIndex)
        {
            return _rowAnimations.ContainsKey(rowIndex);
        }
        
        /// <summary>
        /// Checks if a cell has an active animation
        /// </summary>
        public bool IsCellAnimating(int rowIndex, string columnName)
        {
            string key = $"{rowIndex}:{columnName}";
            return _cellAnimations.ContainsKey(key);
        }
        
        /// <summary>
        /// Clears all active animations
        /// </summary>
        public void ClearAnimations()
        {
            _rowAnimations.Clear();
            _cellAnimations.Clear();
            _animationTimer?.Stop();
            _grid?.Invalidate();
        }
        
        /// <summary>
        /// Starts the animation timer if not already running
        /// </summary>
        private void StartAnimationTimer()
        {
            if (!_animationTimer.Enabled)
            {
                _animationTimer.Start();
            }
        }
        
        /// <summary>
        /// Disposes the animation manager
        /// </summary>
        public void Dispose()
        {
            if (_animationTimer != null)
            {
                _animationTimer.Stop();
                _animationTimer.Tick -= OnAnimationTick;
                _animationTimer.Dispose();
            }
            
            ClearAnimations();
            _grid = null;
            _config = null;
            _isInitialized = false;
        }
    }
    
    /// <summary>
    /// Represents an animation for a grid row
    /// </summary>
    internal class RowAnimation
    {
        public int RowIndex { get; set; }
        public AnimationType Type { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public AnimationEasing EasingFunction { get; set; }
        public float CurrentProgress { get; set; }
        public Action OnComplete { get; set; }
    }
    
    /// <summary>
    /// Represents an animation for a grid cell
    /// </summary>
    internal class CellAnimation
    {
        public int RowIndex { get; set; }
        public string ColumnName { get; set; }
        public AnimationType Type { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public float CurrentProgress { get; set; }
    }
    
    /// <summary>
    /// Types of animations
    /// </summary>
    internal enum AnimationType
    {
        FadeIn,
        FadeOut,
        SlideIn,
        SlideOut,
        Highlight,
        Scale
    }
}

