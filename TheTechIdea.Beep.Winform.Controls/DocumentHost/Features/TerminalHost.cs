// TerminalHost.cs
// Manages a child shell process (cmd / PowerShell / bash) for TerminalPanel.
// Writes to stdin, reads stdout+stderr into a ring buffer, raises DataReceived
// on the calling thread via a SynchronizationContext capture.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Features
{
    /// <summary>
    /// Supported shell types for <see cref="TerminalHost"/>.
    /// </summary>
    public enum TerminalShell { Cmd, PowerShell, Bash, Custom }

    /// <summary>
    /// Manages one shell child process and exposes async I/O.
    /// </summary>
    public sealed class TerminalHost : IDisposable
    {
        // ── Configuration ─────────────────────────────────────────────────────

        public TerminalShell Shell         { get; set; } = TerminalShell.PowerShell;
        public string        CustomShell   { get; set; } = string.Empty;
        public string        WorkingDirectory { get; set; } = System.IO.Directory.GetCurrentDirectory();
        public int           ScrollbackLines  { get; set; } = 10_000;

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Raised (on the captured SynchronizationContext) when output arrives.</summary>
        public event EventHandler<string>? DataReceived;

        /// <summary>Raised when the shell process exits.</summary>
        public event EventHandler<int>? ProcessExited;

        // ── State ─────────────────────────────────────────────────────────────

        private Process?                 _process;
        private readonly StringBuilder   _scrollback = new();
        private readonly SynchronizationContext? _syncCtx;
        private bool _disposed;

        // ── Constructor ───────────────────────────────────────────────────────

        public TerminalHost()
            => _syncCtx = SynchronizationContext.Current;

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Starts the shell process.</summary>
        public void Start()
        {
            Stop();
            var exe  = ResolveShellExe();
            var args = ResolveShellArgs();

            var psi = new ProcessStartInfo
            {
                FileName               = exe,
                Arguments              = args,
                WorkingDirectory       = WorkingDirectory,
                RedirectStandardInput  = true,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding  = Encoding.UTF8,
            };

            _process = new Process { StartInfo = psi, EnableRaisingEvents = true };
            _process.OutputDataReceived += OnOutput;
            _process.ErrorDataReceived  += OnOutput;
            _process.Exited             += OnExited;
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        /// <summary>Sends a line of text to the shell's stdin.</summary>
        public void SendLine(string line)
        {
            if (_process == null || _process.HasExited) return;
            try { _process.StandardInput.WriteLine(line); }
            catch { /* process may have just exited */ }
        }

        /// <summary>Sends raw text (no newline) to stdin.</summary>
        public void Send(string text)
        {
            if (_process == null || _process.HasExited) return;
            try { _process.StandardInput.Write(text); }
            catch { }
        }

        /// <summary>Terminates the running shell.</summary>
        public void Stop()
        {
            if (_process == null) return;
            try
            {
                if (!_process.HasExited) _process.Kill(entireProcessTree: true);
            }
            catch { }
            _process.Dispose();
            _process = null;
        }

        /// <summary>Returns all scrollback text accumulated so far.</summary>
        public string GetScrollback()
        {
            lock (_scrollback) return _scrollback.ToString();
        }

        /// <summary>Clears the scrollback buffer.</summary>
        public void ClearScrollback()
        {
            lock (_scrollback) _scrollback.Clear();
        }

        /// <summary><c>true</c> if the shell process is running.</summary>
        public bool IsRunning => _process is { HasExited: false };

        // ── Callbacks ─────────────────────────────────────────────────────────

        private void OnOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;
            string text = e.Data + "\n";

            lock (_scrollback)
            {
                _scrollback.Append(text);
                // Trim to scrollback limit (rough line count)
                if (_scrollback.Length > ScrollbackLines * 80)
                    _scrollback.Remove(0, _scrollback.Length - ScrollbackLines * 80);
            }

            if (_syncCtx != null)
                _syncCtx.Post(_ => DataReceived?.Invoke(this, text), null);
            else
                DataReceived?.Invoke(this, text);
        }

        private void OnExited(object? sender, EventArgs e)
        {
            int code = -1;
            try { code = _process?.ExitCode ?? -1; } catch { }

            if (_syncCtx != null)
                _syncCtx.Post(_ => ProcessExited?.Invoke(this, code), null);
            else
                ProcessExited?.Invoke(this, code);
        }

        // ── Shell resolution ──────────────────────────────────────────────────

        private string ResolveShellExe() => Shell switch
        {
            TerminalShell.Cmd        => "cmd.exe",
            TerminalShell.PowerShell => "pwsh.exe",
            TerminalShell.Bash       => "bash",
            TerminalShell.Custom     => CustomShell,
            _                        => "cmd.exe"
        };

        private string ResolveShellArgs() => Shell switch
        {
            TerminalShell.Cmd        => "/Q /K",
            TerminalShell.PowerShell => "-NoLogo",
            _                        => string.Empty
        };

        // ── Dispose ───────────────────────────────────────────────────────────

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Stop();
        }
    }
}
