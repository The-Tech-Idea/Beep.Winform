using System.Threading;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes
{
    /// <summary>
    /// Lightweight, zero-cost-in-release diagnostics for <see cref="BeepCheckBox{T}"/>.
    /// </summary>
    /// <remarks>
    /// Diagnostics hooks (BCHK-P5-006):
    ///   All counters are compiled only when the DEBUG symbol is defined.
    ///   In Release builds this entire class exists but every static member is a no-op property
    ///   returning 0, so call sites can remain unconditional.
    ///   Use <see cref="Reset"/> at test start and compare counters after exercising the control
    ///   to detect unexpected invalidation or toggle regressions.
    ///
    /// Typical usage in a diagnostic host or unit test:
    /// <code>
    ///   BeepCheckBoxDiagnostics.Reset();
    ///   checkbox.State = CheckBoxState.Checked;
    ///   checkbox.State = CheckBoxState.Checked; // no-op
    ///   Assert.AreEqual(1, BeepCheckBoxDiagnostics.InvalidationCount);
    ///   Assert.AreEqual(1, BeepCheckBoxDiagnostics.ToggleCount);
    /// </code>
    /// </remarks>
    public static class BeepCheckBoxDiagnostics
    {
#if DEBUG
        private static int _invalidationCount;
        private static int _toggleCount;
        private static int _cacheRebuildCount;
        private static int _painterFetchCount;

        /// <summary>Total number of times <c>RequestVisualRefresh</c> was called.</summary>
        public static int InvalidationCount => _invalidationCount;

        /// <summary>Total number of times <c>ToggleState</c> was called.</summary>
        public static int ToggleCount => _toggleCount;

        /// <summary>Total number of times the GDI brush/pen/path caches were cleared.</summary>
        public static int CacheRebuildCount => _cacheRebuildCount;

        /// <summary>Total number of times <c>CheckBoxPainterFactory.GetPainter</c> was called.</summary>
        public static int PainterFetchCount => _painterFetchCount;

        internal static void RecordInvalidation()  => Interlocked.Increment(ref _invalidationCount);
        internal static void RecordToggle()        => Interlocked.Increment(ref _toggleCount);
        internal static void RecordCacheRebuild()  => Interlocked.Increment(ref _cacheRebuildCount);
        internal static void RecordPainterFetch()  => Interlocked.Increment(ref _painterFetchCount);

        /// <summary>Reset all counters to zero.</summary>
        public static void Reset()
        {
            _invalidationCount = 0;
            _toggleCount       = 0;
            _cacheRebuildCount = 0;
            _painterFetchCount = 0;
        }
#else
        /// <summary>Always 0 in Release builds.</summary>
        public static int InvalidationCount => 0;
        /// <summary>Always 0 in Release builds.</summary>
        public static int ToggleCount       => 0;
        /// <summary>Always 0 in Release builds.</summary>
        public static int CacheRebuildCount => 0;
        /// <summary>Always 0 in Release builds.</summary>
        public static int PainterFetchCount => 0;

        internal static void RecordInvalidation()  { }
        internal static void RecordToggle()        { }
        internal static void RecordCacheRebuild()  { }
        internal static void RecordPainterFetch()  { }

        /// <summary>No-op in Release builds.</summary>
        public static void Reset() { }
#endif
    }
}
