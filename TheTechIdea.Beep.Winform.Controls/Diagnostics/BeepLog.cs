using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls.Diagnostics
{
    /// <summary>How much attention an entry deserves.</summary>
    public enum BeepLogLevel
    {
        /// <summary>Something happened that a developer tracing behaviour would want to see.</summary>
        Info,

        /// <summary>A degraded path was taken. The operation continued, but not as intended.</summary>
        Warning,

        /// <summary>An operation failed.</summary>
        Error,
    }

    /// <summary>One reported event.</summary>
    public sealed class BeepLogEntry
    {
        public BeepLogEntry(BeepLogLevel level, string source, string action, string message, Exception? exception)
        {
            Level = level;
            Source = source;
            Action = action;
            Message = message;
            Exception = exception;
            TimestampUtc = DateTime.UtcNow;
        }

        public BeepLogLevel Level { get; }

        /// <summary>Which control or component reported it, e.g. <c>BeepCounterBadge</c>.</summary>
        public string Source { get; }

        /// <summary>What was being attempted, e.g. <c>render icon 'cat.svg'</c>.</summary>
        public string Action { get; }

        public string Message { get; }
        public Exception? Exception { get; }
        public DateTime TimestampUtc { get; }

        public override string ToString()
        {
            string text = $"[Beep:{Level}] {Source}: {Action}";
            if (!string.IsNullOrEmpty(Message)) text += $" - {Message}";
            return text;
        }
    }

    /// <summary>
    /// The one place this library reports failures from. Off-switchable, and routable to a host's own
    /// logger.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every <c>catch</c> in this library reports through here.</b> A bare <c>catch { }</c> hides the
    /// bug from the only person who can fix it — that rule produced real defects, including an
    /// unresolvable icon path that rendered a validation badge as a plain coloured shape, which reads
    /// as a *success* badge in the wrong colour.
    /// </para>
    /// <para>
    /// <b>Turning it off is a display decision, not a correctness one.</b> Set
    /// <see cref="IsEnabled"/> to <see langword="false"/> and nothing is emitted, but the surrounding
    /// code still takes its fallback path. Never use the switch to make a failure "go away".
    /// </para>
    /// <para>
    /// <b>Use the <c>…Once</c> overloads inside <c>OnPaint</c> and animation ticks.</b> A pulsing badge
    /// repaints 25 times a second; a message per paint buries the first occurrence it exists to
    /// surface.
    /// </para>
    /// <example>
    /// Routing to a host application's logger:
    /// <code>
    /// BeepLog.Reported += entry =&gt; myLogger.Log(entry.Level.ToString(), entry.ToString(), entry.Exception);
    /// BeepLog.WriteToDebug = false;   // stop duplicating into the debug output
    /// </code>
    /// </example>
    /// </remarks>
    public static class BeepLog
    {
        private static readonly object _gate = new();
        private static readonly HashSet<string> _seen = new(StringComparer.Ordinal);

        /// <summary>
        /// Whether anything is emitted at all. Defaults to on in DEBUG builds, off in release.
        /// </summary>
        /// <remarks>
        /// A release build stays quiet by default because these messages are developer diagnostics, not
        /// user-facing errors. A host that wants them in production sets this to <see langword="true"/>
        /// and subscribes to <see cref="Reported"/>.
        /// </remarks>
        public static bool IsEnabled { get; set; } =
#if DEBUG
            true;
#else
            false;
#endif

        /// <summary>Entries below this level are dropped. Defaults to <see cref="BeepLogLevel.Info"/>.</summary>
        public static BeepLogLevel MinimumLevel { get; set; } = BeepLogLevel.Info;

        /// <summary>Whether to write to <see cref="Debug"/>. On by default.</summary>
        public static bool WriteToDebug { get; set; } = true;

        /// <summary>
        /// Raised for every entry that passes <see cref="IsEnabled"/> and <see cref="MinimumLevel"/>.
        /// </summary>
        /// <remarks>
        /// A subscriber that throws is caught and reported to <see cref="Debug"/> rather than allowed to
        /// propagate — this is called from <c>catch</c> blocks and inside paint handlers, where an
        /// escaping exception would replace the original failure with a worse one.
        /// </remarks>
        public static event Action<BeepLogEntry>? Reported;

        public static void Info(object? source, string action, string? message = null)
            => Write(BeepLogLevel.Info, source, action, message, null);

        public static void Warn(object? source, string action, string? message = null)
            => Write(BeepLogLevel.Warning, source, action, message, null);

        public static void Error(object? source, string action, string? message = null)
            => Write(BeepLogLevel.Error, source, action, message, null);

        /// <summary>Reports a caught exception.</summary>
        public static void Failure(object? source, string action, Exception exception)
            => Write(BeepLogLevel.Error, source, action, Describe(exception), exception);

        /// <summary>Reports a degraded path that succeeded by other means.</summary>
        public static void Fallback(object? source, string action, Exception exception)
            => Write(BeepLogLevel.Warning, source, action, Describe(exception), exception);

        /// <summary>
        /// Reports a caught exception the first time this <paramref name="key"/> is seen.
        /// </summary>
        /// <param name="key">
        /// What makes this failure distinct — usually the thing that failed, such as an icon path.
        /// Two failures sharing a key are reported once.
        /// </param>
        /// <returns><see langword="true"/> if this call emitted, <see langword="false"/> if suppressed.</returns>
        /// <remarks>Use inside <c>OnPaint</c> and timer ticks, where the same failure recurs every frame.</remarks>
        public static bool FailureOnce(string key, object? source, string action, Exception exception)
            => WriteOnce(key, BeepLogLevel.Error, source, action, Describe(exception), exception);

        /// <summary>Reports a degraded path the first time this <paramref name="key"/> is seen.</summary>
        public static bool FallbackOnce(string key, object? source, string action, Exception exception)
            => WriteOnce(key, BeepLogLevel.Warning, source, action, Describe(exception), exception);

        /// <summary>Reports a message the first time this <paramref name="key"/> is seen.</summary>
        public static bool WarnOnce(string key, object? source, string action, string? message = null)
            => WriteOnce(key, BeepLogLevel.Warning, source, action, message, null);

        /// <summary>
        /// Forgets which keys have been reported, so <c>…Once</c> calls emit again.
        /// </summary>
        /// <remarks>Call at the start of a test that asserts on reporting; nothing else should need it.</remarks>
        public static void ResetOnceKeys()
        {
            lock (_gate) _seen.Clear();
        }

        /// <summary>How many distinct keys have been reported. For tests.</summary>
        public static int ReportedKeyCount
        {
            get { lock (_gate) return _seen.Count; }
        }

        private static bool WriteOnce(string key, BeepLogLevel level, object? source, string action, string? message, Exception? exception)
        {
            // The dedupe check runs even when disabled, so enabling mid-run does not replay a backlog
            // of failures that already happened.
            lock (_gate)
            {
                if (!_seen.Add($"{Name(source)}|{key}")) return false;
            }

            Write(level, source, action, message, exception);
            return true;
        }

        private static void Write(BeepLogLevel level, object? source, string action, string? message, Exception? exception)
        {
            if (!IsEnabled || level < MinimumLevel) return;

            var entry = new BeepLogEntry(level, Name(source), action, message ?? string.Empty, exception);

            if (WriteToDebug) Debug.WriteLine(entry.ToString());

            var handler = Reported;
            if (handler is null) return;

            try
            {
                handler(entry);
            }
            catch (Exception ex)
            {
                // The one catch in this library that cannot report through BeepLog, because it IS
                // BeepLog. Writing straight to Debug is the report; rethrowing would let a faulty
                // subscriber replace the original failure with a worse one, from inside a catch block
                // or a paint handler.
                Debug.WriteLine($"[Beep:Error] BeepLog: a Reported subscriber threw - {Describe(ex)}");
            }
        }

        private static string Name(object? source) => source switch
        {
            null => "Beep",
            string s => s,
            Type t => t.Name,
            _ => source.GetType().Name,
        };

        private static string Describe(Exception ex) => $"{ex.GetType().Name}: {ex.Message}";
    }
}
