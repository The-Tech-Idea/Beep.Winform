using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Sprint 6 — Fluent builder for constructing guided tours.
    /// <code>
    ///   BeepTourManager.Instance
    ///       .CreateTour()
    ///       .AddStep(btnSave,    "Save your work",  "Click here to save")
    ///       .AddStep(tbxSearch, "Search",           "Type to filter")
    ///       .Build()
    ///       .Start();
    /// </code>
    /// </summary>
    public class BeepTourBuilder
    {
        private readonly List<BeepTourStep> _steps = new List<BeepTourStep>();

        // ──────────────────────────────────────────────────────────────────────
        // Fluent step registration
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Add a step with the minimum required information.</summary>
        public BeepTourBuilder AddStep(
            Control target, string title, string body,
            ToolTipPlacement placement = ToolTipPlacement.Auto)
        {
            _steps.Add(new BeepTourStep
            {
                TargetControl = target,
                Title         = title,
                Body          = body,
                Placement     = placement
            });
            return this;
        }

        /// <summary>Add a fully configured <see cref="BeepTourStep"/>.</summary>
        public BeepTourBuilder AddStep(BeepTourStep step)
        {
            if (step == null) throw new ArgumentNullException(nameof(step));
            _steps.Add(step);
            return this;
        }

        /// <summary>
        /// Configure the last added step with lifecycle callbacks.
        /// Must be called after at least one <see cref="AddStep"/>.
        /// </summary>
        public BeepTourBuilder WithCallbacks(Action onEnter = null, Action onLeave = null)
        {
            if (_steps.Count == 0)
                throw new InvalidOperationException("No steps have been added yet.");
            var last    = _steps[_steps.Count - 1];
            last.OnEnter = onEnter;
            last.OnLeave = onLeave;
            return this;
        }

        /// <summary>
        /// Attach an image to the last added step.
        /// </summary>
        public BeepTourBuilder WithImage(string imagePath)
        {
            if (_steps.Count == 0)
                throw new InvalidOperationException("No steps have been added yet.");
            _steps[_steps.Count - 1].ImagePath = imagePath;
            return this;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Build
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Validate and load the built steps into <see cref="BeepTourManager"/>.
        /// Returns the manager so you can chain <c>.Start()</c> immediately.
        /// </summary>
        public BeepTourManager Build()
        {
            if (_steps.Count == 0)
                throw new InvalidOperationException("A tour must have at least one step.");

            BeepTourManager.Instance.LoadTour(_steps);
            return BeepTourManager.Instance;
        }
    }
}
