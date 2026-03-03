using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Sprint 6 — Singleton manager for guided-tour / onboarding flows.
    ///
    /// Usage (fluent builder):
    /// <code>
    ///   await BeepTourManager.Instance
    ///       .CreateTour()
    ///       .AddStep(btnSave, "Save", "Click to save your file")
    ///       .Build()
    ///       .StartAsync();
    /// </code>
    ///
    /// Manual navigation:
    /// <code>
    ///   BeepTourManager.Instance.Next();
    ///   BeepTourManager.Instance.Previous();
    ///   BeepTourManager.Instance.Skip();
    /// </code>
    /// </summary>
    public sealed class BeepTourManager : IDisposable
    {
        // ──────────────────────────────────────────────────────────────────────
        // Singleton
        // ──────────────────────────────────────────────────────────────────────

        private static readonly Lazy<BeepTourManager> _instance =
            new Lazy<BeepTourManager>(() => new BeepTourManager());

        public static BeepTourManager Instance => _instance.Value;

        private BeepTourManager() { }

        // ──────────────────────────────────────────────────────────────────────
        // Events
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Fired when the tour begins.</summary>
        public event EventHandler TourStarted;

        /// <summary>Fired when moving to a new step. Args: (currentIndex, totalSteps).</summary>
        public event EventHandler<(int current, int total)> StepChanged;

        /// <summary>Fired when all steps have been completed.</summary>
        public event EventHandler TourCompleted;

        /// <summary>Fired when the user clicks Skip or the tour is cancelled.</summary>
        public event EventHandler TourSkipped;

        // ──────────────────────────────────────────────────────────────────────
        // State
        // ──────────────────────────────────────────────────────────────────────

        private List<BeepTourStep> _steps       = new List<BeepTourStep>();
        private int                _currentIndex = -1;
        private CustomToolTip      _activeTooltip;
        private bool               _disposed;

        // ──────────────────────────────────────────────────────────────────────
        // Public read-only state
        // ──────────────────────────────────────────────────────────────────────

        public bool  IsRunning    => _currentIndex >= 0;
        public int   CurrentStep  => _currentIndex + 1;           // 1-based
        public int   TotalSteps   => _steps?.Count ?? 0;

        // ──────────────────────────────────────────────────────────────────────
        // Setup
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Fluent factory: returns a builder bound to this manager.</summary>
        public BeepTourBuilder CreateTour() => new BeepTourBuilder();

        /// <summary>Load tour steps (called by <see cref="BeepTourBuilder.Build"/>).</summary>
        public void LoadTour(IEnumerable<BeepTourStep> steps)
        {
            if (IsRunning)
                EndTourInternal(skipped: true);

            _steps = new List<BeepTourStep>(steps);
            _currentIndex = -1;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Navigation
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Start the tour from the first step.</summary>
        public async Task StartAsync()
        {
            if (_steps == null || _steps.Count == 0)
                throw new InvalidOperationException("Call LoadTour() or Build() before StartAsync().");

            _currentIndex = 0;
            TourStarted?.Invoke(this, EventArgs.Empty);
            await ShowCurrentStepAsync();
        }

        /// <summary>Advance to the next step; completes the tour if on the last step.</summary>
        public async Task NextAsync()
        {
            if (!IsRunning) return;

            _steps[_currentIndex].OnLeave?.Invoke();
            HideCurrentTooltip();

            _currentIndex++;

            if (_currentIndex >= _steps.Count)
            {
                EndTourInternal(skipped: false);
                return;
            }

            await ShowCurrentStepAsync();
        }

        /// <summary>Go back to the previous step (no-op on first step).</summary>
        public async Task PreviousAsync()
        {
            if (!IsRunning || _currentIndex <= 0) return;

            _steps[_currentIndex].OnLeave?.Invoke();
            HideCurrentTooltip();

            _currentIndex--;
            await ShowCurrentStepAsync();
        }

        /// <summary>Cancel the tour without completing it.</summary>
        public void Skip()
        {
            if (!IsRunning) return;
            EndTourInternal(skipped: true);
        }

        /// <summary>Synchronous wrapper – fires-and-forgets on the current synchronisation context.</summary>
        public void Next()     => _ = NextAsync();
        public void Previous() => _ = PreviousAsync();
        public void Start()    => _ = StartAsync();

        // ──────────────────────────────────────────────────────────────────────
        // Internal
        // ──────────────────────────────────────────────────────────────────────

        private async Task ShowCurrentStepAsync()
        {
            var step = _steps[_currentIndex];

            // Pre-enter callback
            step.OnEnter?.Invoke();

            // Build config
            var cfg = new ToolTipConfig
            {
                Title                 = step.Title,
                Text                  = step.Body,
                ImagePath             = step.ImagePath,
                Placement             = step.Placement,
                LayoutVariant         = ToolTipLayoutVariant.Tour,
                CurrentStep           = _currentIndex + 1,
                TotalSteps            = _steps.Count,
                ShowNavigationButtons = true,
                Duration              = 0,       // No auto-hide
                ShowArrow             = true,
                Animation             = ToolTipAnimation.Scale
            };

            // Resolve screen position from target control
            if (step.TargetControl != null)
            {
                cfg.Position = ResolveControlScreenPosition(step.TargetControl, cfg);
            }

            // Show via ToolTipManager (re-use its lifecycle)
            await ToolTipManager.Instance.ShowTooltipAsync(cfg);

            StepChanged?.Invoke(this, (_currentIndex + 1, _steps.Count));
        }

        private static Point ResolveControlScreenPosition(Control ctrl, ToolTipConfig cfg)
        {
            if (ctrl == null) return cfg.Position;
            var scrBounds = ctrl.RectangleToScreen(ctrl.ClientRectangle);
            var resolved  = ToolTips.Helpers.ToolTipPositionResolver.Resolve(
                scrBounds, new Size(310, 200), cfg.Placement);
            cfg.Placement = resolved.ActualPlacement;
            return resolved.Location;
        }

        private void HideCurrentTooltip()
        {
            // ToolTipManager tracks tooltips by key — dismiss any current tour tooltip
            // We use a well-known key prefix
            _ = ToolTipManager.Instance.HideTooltipAsync(TourKey());
        }

        private string TourKey() => $"__tour_{_currentIndex}";

        private void EndTourInternal(bool skipped)
        {
            HideCurrentTooltip();
            int savedIndex = _currentIndex;
            _currentIndex = -1;

            if (skipped)
                TourSkipped?.Invoke(this, EventArgs.Empty);
            else
                TourCompleted?.Invoke(this, EventArgs.Empty);
        }

        // ──────────────────────────────────────────────────────────────────────
        // IDisposable
        // ──────────────────────────────────────────────────────────────────────

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (IsRunning) EndTourInternal(skipped: true);
            _activeTooltip?.Dispose();
        }
    }
}
