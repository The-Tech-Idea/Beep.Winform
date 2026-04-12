// BeepGitStatusProvider.cs
// Lightweight Git status integration for BeepDocumentHost.
// Reads `git status --porcelain` via a background process and maps
// per-file status codes to badge text + colour shown on document tabs.
// No libgit2 dependency — shells out to the installed Git executable.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Features
{
    /// <summary>Git working-tree status for a single file.</summary>
    public enum GitFileStatus
    {
        Unknown,
        Unmodified,
        Modified,     // M
        Staged,       // index modified
        Untracked,    // ?
        Conflicted,   // U / AA / DD
        Deleted,      // D
        Renamed,      // R
        Added         // A
    }

    /// <summary>
    /// Per-file Git status with badge display properties.
    /// </summary>
    public sealed class GitFileInfo
    {
        public string         FilePath   { get; init; } = string.Empty;
        public GitFileStatus  Status     { get; init; } = GitFileStatus.Unknown;
        public string         BadgeText  { get; init; } = string.Empty;
        public Color          BadgeColor { get; init; } = Color.Gray;
        public string         ToolTip    { get; init; } = string.Empty;
    }

    /// <summary>
    /// Provides Git status information for files open in the document host.
    /// Refresh is driven by a timer or explicit call; results are cached.
    /// </summary>
    public sealed class BeepGitStatusProvider : IDisposable
    {
        // ── Configuration ─────────────────────────────────────────────────────

        /// <summary>Path to the Git executable (defaults to PATH lookup).</summary>
        public string GitExecutable { get; set; } = "git";

        /// <summary>If set, Git is run relative to this directory; otherwise the
        /// working directory of the host process is used.</summary>
        public string? RepositoryRoot { get; set; }

        /// <summary>Auto-refresh interval in milliseconds (0 = disabled).</summary>
        public int RefreshIntervalMs { get; set; } = 5_000;

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Raised on the UI thread after each successful refresh.</summary>
        public event EventHandler? StatusRefreshed;

        // ── State ─────────────────────────────────────────────────────────────

        private readonly Dictionary<string, GitFileInfo> _cache
            = new(StringComparer.OrdinalIgnoreCase);
        private readonly System.Windows.Forms.Timer _timer;
        private bool _disposed;

        // ── Constructor ───────────────────────────────────────────────────────

        public BeepGitStatusProvider()
        {
            _timer = new System.Windows.Forms.Timer { Interval = 5_000 };
            _timer.Tick += async (_, _) => await RefreshAsync();
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Starts automatic background refresh.</summary>
        public void Start()
        {
            _timer.Interval = Math.Max(500, RefreshIntervalMs);
            _timer.Start();
        }

        /// <summary>Stops automatic refresh.</summary>
        public void Stop() => _timer.Stop();

        /// <summary>Returns the cached status for a given absolute file path.</summary>
        public GitFileInfo GetStatus(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return Unknown(filePath);
            if (_cache.TryGetValue(filePath, out var info)) return info;
            return Unknown(filePath);
        }

        /// <summary>Runs `git status --porcelain` and rebuilds the cache.</summary>
        public async Task RefreshAsync(CancellationToken ct = default)
        {
            try
            {
                string output = await RunGitAsync("status --porcelain -u", ct);
                var newCache  = ParsePorcelain(output);
                lock (_cache)
                {
                    _cache.Clear();
                    foreach (var (k, v) in newCache)
                        _cache[k] = v;
                }
                StatusRefreshed?.Invoke(this, EventArgs.Empty);
            }
            catch (OperationCanceledException) { }
            catch { /* Git not available or not a repo — swallow */ }
        }

        // ── Git process ───────────────────────────────────────────────────────

        private async Task<string> RunGitAsync(string args, CancellationToken ct)
        {
            var psi = new ProcessStartInfo
            {
                FileName               = GitExecutable,
                Arguments              = args,
                WorkingDirectory       = RepositoryRoot ?? Directory.GetCurrentDirectory(),
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true,
            };

            using var proc = new Process { StartInfo = psi, EnableRaisingEvents = true };
            var tcs = new TaskCompletionSource<string>();

            proc.Exited += (_, _) =>
            {
                if (!tcs.Task.IsCompleted)
                    tcs.TrySetResult(proc.StandardOutput.ReadToEnd());
            };

            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            using var reg = ct.Register(() => { try { proc.Kill(); } catch { } });
            await Task.Run(() => proc.WaitForExit(10_000), ct);

            return proc.StandardOutput.ReadToEnd();
        }

        // ── Porcelain parser ──────────────────────────────────────────────────

        private Dictionary<string, GitFileInfo> ParsePorcelain(string porcelain)
        {
            var result = new Dictionary<string, GitFileInfo>(StringComparer.OrdinalIgnoreCase);
            string root = RepositoryRoot ?? Directory.GetCurrentDirectory();

            foreach (var rawLine in porcelain.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                if (rawLine.Length < 4) continue;
                char x    = rawLine[0];   // index status
                char y    = rawLine[1];   // worktree status
                string rel = rawLine[3..].Trim();
                string abs = Path.GetFullPath(Path.Combine(root, rel));

                var (status, badge, color, tip) = ClassifyStatus(x, y);
                result[abs] = new GitFileInfo
                {
                    FilePath  = abs,
                    Status    = status,
                    BadgeText = badge,
                    BadgeColor= color,
                    ToolTip   = tip,
                };
            }

            return result;
        }

        private static (GitFileStatus, string, Color, string) ClassifyStatus(char x, char y)
        {
            if (x == '?' && y == '?') return (GitFileStatus.Untracked,   "U",  Color.Gray,   "Untracked");
            if (x == 'A')             return (GitFileStatus.Added,        "A",  Color.LimeGreen,"Staged: Added");
            if (x == 'M' || y == 'M') return (GitFileStatus.Modified,    "M",  Color.Orange, "Modified");
            if (x == 'D' || y == 'D') return (GitFileStatus.Deleted,     "D",  Color.Red,    "Deleted");
            if (x == 'R')             return (GitFileStatus.Renamed,      "R",  Color.Teal,   "Renamed");
            if (x == 'U' || y == 'U') return (GitFileStatus.Conflicted,  "C",  Color.Red,    "Conflict");
            if (x == ' ' && y == ' ') return (GitFileStatus.Unmodified,  "",   Color.Transparent,"Clean");
            return (GitFileStatus.Unknown, "?", Color.Gray, "Unknown");
        }

        private static GitFileInfo Unknown(string path) =>
            new() { FilePath = path, Status = GitFileStatus.Unknown,
                    BadgeText = "", BadgeColor = Color.Transparent };

        // ── Dispose ───────────────────────────────────────────────────────────

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
