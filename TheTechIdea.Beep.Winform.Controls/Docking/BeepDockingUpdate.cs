using System;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Suspends layout recalculation on <see cref="BeepDockingManager"/> for the duration
    /// of a <c>using</c> block, then applies a single recalculation on disposal.
    /// Mirrors Krypton's <c>DockingMultiUpdate</c>.
    /// </summary>
    /// <example>
    /// <code>
    /// using (new BeepDockingUpdate(manager))
    /// {
    ///     manager.HidePanel("Panel1");
    ///     manager.FloatPanel("Panel2");
    /// }
    /// // Single RecalculateLayout() fires here.
    /// </code>
    /// </example>
    public sealed class BeepDockingUpdate : IDisposable
    {
        private readonly BeepDockingManager _manager;
        private bool _disposed;

        /// <summary>
        /// Initialises a new batch-update scope, suspending layout on <paramref name="manager"/>.
        /// </summary>
        /// <param name="manager">The manager to suspend.</param>
        public BeepDockingUpdate(BeepDockingManager manager)
        {
            ArgumentNullException.ThrowIfNull(manager);
            _manager = manager;
            _manager.BeginUpdate();
        }

        /// <summary>
        /// Ends the batch-update scope, resuming and applying a single layout pass.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _manager.EndUpdate();
            }
        }
    }
}
