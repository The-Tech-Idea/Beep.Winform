// BeepDocumentHostProfiler.cs
// Lightweight built-in performance profiler for BeepDocumentHost.
//
// Records:
//   • Last layout pass duration (ms)
//   • Last snapshot/thumbnail capture duration (ms)
//   • Render FPS on the tab strip (frames per 1-second window)
//   • Total managed memory bytes (GC)
//   • Cumulative paint frame count
//
// Thread-safe counters via Interlocked; snapshot produced by GetSnapshot().
// Use the scoped timer structs (MeasureLayout / MeasureSnapshot) for
// automatic start/stop timing with a using-declaration.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Diagnostics;
using System.Threading;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Immutable point-in-time performance snapshot produced by
    /// <see cref="BeepDocumentHostProfiler.GetSnapshot"/>.
    /// </summary>
    public sealed record ProfilerSnapshot(
        int    TabCount,
        double LayoutMs,
        double SnapshotMs,
        double Fps,
        long   ManagedMemoryBytes,
        int    FrameCount);

    /// <summary>
    /// Lightweight built-in profiler attached to a <see cref="BeepDocumentHost"/>.
    /// Access via <see cref="BeepDocumentHost.Profiler"/>.
    /// </summary>
    public sealed class BeepDocumentHostProfiler
    {
        private readonly Stopwatch _uptime = Stopwatch.StartNew();

        private long _layoutTicks;
        private long _snapshotTicks;
        private int  _frameCount;

        // FPS: rolling 1-second window
        private long _fpsWindowStartTicks;
        private int  _fpsWindowFrames;
        private double _lastFps;
        private static readonly long FpsWindowTicks = Stopwatch.Frequency; // 1 s

        /// <summary>
        /// Raised when the host emits a vNext telemetry event.
        /// </summary>
        public event EventHandler<DocumentHostTelemetryEvent>? TelemetryEmitted;

        // ── Record methods ────────────────────────────────────────────────────

        /// <summary>Records the duration of the last completed layout pass.</summary>
        public void RecordLayout(long elapsedTicks)
            => Interlocked.Exchange(ref _layoutTicks, elapsedTicks);

        /// <summary>Records the duration of the last completed thumbnail capture.</summary>
        public void RecordSnapshot(long elapsedTicks)
            => Interlocked.Exchange(ref _snapshotTicks, elapsedTicks);

        /// <summary>
        /// Records a completed paint frame.  Called automatically for each
        /// <c>OnPaint</c> on the tab strip when the profiler is active.
        /// </summary>
        public void RecordFrame()
        {
            Interlocked.Increment(ref _frameCount);

            long now          = _uptime.ElapsedTicks;
            long windowStart  = Interlocked.Read(ref _fpsWindowStartTicks);
            long elapsed      = now - windowStart;

            if (elapsed >= FpsWindowTicks)
            {
                int frames = Interlocked.Exchange(ref _fpsWindowFrames, 0);
                _lastFps   = frames * (double)Stopwatch.Frequency / elapsed;
                Interlocked.Exchange(ref _fpsWindowStartTicks, now);
            }
            else
            {
                Interlocked.Increment(ref _fpsWindowFrames);
            }
        }

        // ── Snapshot ─────────────────────────────────────────────────────────

        /// <summary>
        /// Returns a point-in-time performance snapshot.
        /// </summary>
        /// <param name="tabCount">Current number of open tabs (passed by the host).</param>
        public ProfilerSnapshot GetSnapshot(int tabCount)
        {
            double freq = Stopwatch.Frequency;
            return new ProfilerSnapshot(
                TabCount:           tabCount,
                LayoutMs:           Interlocked.Read(ref _layoutTicks)   * 1000.0 / freq,
                SnapshotMs:         Interlocked.Read(ref _snapshotTicks) * 1000.0 / freq,
                Fps:                _lastFps,
                ManagedMemoryBytes: GC.GetTotalMemory(forceFullCollection: false),
                FrameCount:         Interlocked.CompareExchange(ref _frameCount, 0, 0));
        }

        // ── Scoped timers ─────────────────────────────────────────────────────

        /// <summary>Returns a scope that automatically records a layout timing on dispose.</summary>
        public LayoutTimerScope MeasureLayout()   => new(this);

        /// <summary>Returns a scope that automatically records a snapshot timing on dispose.</summary>
        public SnapshotTimerScope MeasureSnapshot() => new(this);

        /// <summary>
        /// Emits a host telemetry event to profiler subscribers.
        /// </summary>
        public void Emit(DocumentHostTelemetryEvent telemetryEvent)
        {
            if (telemetryEvent == null) return;
            TelemetryEmitted?.Invoke(this, telemetryEvent);
        }

        /// <summary>Disposable scope for timing a layout pass.</summary>
        public readonly struct LayoutTimerScope : IDisposable
        {
            private readonly BeepDocumentHostProfiler _profiler;
            private readonly long _start;

            internal LayoutTimerScope(BeepDocumentHostProfiler profiler)
            {
                _profiler = profiler;
                _start    = Stopwatch.GetTimestamp();
            }

            /// <summary>Stops the timer and records the elapsed ticks.</summary>
            public void Dispose() => _profiler.RecordLayout(Stopwatch.GetTimestamp() - _start);
        }

        /// <summary>Disposable scope for timing a thumbnail capture.</summary>
        public readonly struct SnapshotTimerScope : IDisposable
        {
            private readonly BeepDocumentHostProfiler _profiler;
            private readonly long _start;

            internal SnapshotTimerScope(BeepDocumentHostProfiler profiler)
            {
                _profiler = profiler;
                _start    = Stopwatch.GetTimestamp();
            }

            /// <summary>Stops the timer and records the elapsed ticks.</summary>
            public void Dispose() => _profiler.RecordSnapshot(Stopwatch.GetTimestamp() - _start);
        }
    }
}
