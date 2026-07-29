using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Partial class implementing <see cref="ISupportInitialize"/> for BeepGridPro.
    /// Designer-generated code wraps batched property assignment in
    /// <c>((ISupportInitialize)grid).BeginInit()</c> / <c>EndInit()</c>; implementing the
    /// interface keeps those casts valid and lets the grid defer layout work until the
    /// whole batch has been applied.
    /// </summary>
    public partial class BeepGridPro : ISupportInitialize
    {
        #region ISupportInitialize

        private int _initCount;

        /// <summary>
        /// True while a BeginInit/EndInit batch is in progress. Layout recalculation is
        /// deferred until the outermost EndInit completes.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsInitializing => _initCount > 0;

        /// <summary>
        /// Signals the start of a batch initialization. Nested calls are counted so only
        /// the outermost EndInit performs the deferred work.
        /// </summary>
        public void BeginInit()
        {
            _initCount++;
        }

        /// <summary>
        /// Signals the end of a batch initialization. The outermost call recalculates
        /// layout, refreshes the scrollbars and repaints once for the whole batch.
        /// Unbalanced calls are ignored.
        /// </summary>
        public void EndInit()
        {
            if (_initCount == 0) return;   // unbalanced EndInit - nothing to close
            if (--_initCount > 0) return;  // still inside an outer batch

            if (IsDisposed || Disposing) return;

            SafeRecalculate();
            ScrollBars?.UpdateBars();
            SafeInvalidate();
        }

        #endregion
    }
}
