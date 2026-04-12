// TerminalPanel.cs
// Dockable terminal panel for BeepDocumentHost.
// Wraps TerminalHost (child process) + a RichTextBox output view + a TextBox
// input bar.  Multiple instances can be created for a split-terminal layout.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Features
{
    /// <summary>
    /// Self-contained dockable terminal panel.
    /// Add to any <c>Controls</c> collection; call <see cref="Start()"/> to launch the shell.
    /// </summary>
    public sealed class TerminalPanel : UserControl
    {
        // ── Sub-controls ──────────────────────────────────────────────────────

        private readonly RichTextBox _output;
        private readonly TextBox     _input;
        private readonly Panel       _toolbar;
        private readonly Button      _btnClear;
        private readonly Button      _btnSplit;
        private readonly Label       _lblTitle;

        // ── Domain ────────────────────────────────────────────────────────────

        private readonly TerminalHost    _host;
        private          TerminalTheme   _theme;
        private readonly List<string>    _history = new();
        private int _historyIndex = -1;

        // ── Configuration ─────────────────────────────────────────────────────

        public TerminalShell Shell
        {
            get => _host.Shell;
            set => _host.Shell = value;
        }

        public string WorkingDirectory
        {
            get => _host.WorkingDirectory;
            set => _host.WorkingDirectory = value;
        }

        public TerminalTheme Theme
        {
            get => _theme;
            set { _theme = value; ApplyThemeColors(); }
        }

        public string Title
        {
            get => _lblTitle.Text;
            set => _lblTitle.Text = value;
        }

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Raised when the user clicks the Split button.</summary>
        public event EventHandler? SplitRequested;

        // ── Constructor ───────────────────────────────────────────────────────

        public TerminalPanel()
        {
            _theme = TerminalTheme.Dark;
            _host  = new TerminalHost();
            _host.DataReceived  += OnHostDataReceived;
            _host.ProcessExited += OnHostExited;

            // Toolbar
            _btnClear = new Button { Text = "⌫ Clear", Width = 64, FlatStyle = FlatStyle.Flat };
            _btnSplit = new Button { Text = "⊟ Split", Width = 64, FlatStyle = FlatStyle.Flat };
            _lblTitle = new Label  { Text = "Terminal", AutoSize = true,
                                     TextAlign = ContentAlignment.MiddleLeft };
            _btnClear.Click += (_, _) => ClearOutput();
            _btnSplit.Click += (_, _) => SplitRequested?.Invoke(this, EventArgs.Empty);

            _toolbar = new Panel { Dock = DockStyle.Top, Height = 28 };
            _toolbar.Controls.AddRange(new Control[] { _lblTitle, _btnClear, _btnSplit });

            // Output
            _output = new RichTextBox
            {
                Dock          = DockStyle.Fill,
                ReadOnly      = true,
                BorderStyle   = BorderStyle.None,
                ScrollBars    = RichTextBoxScrollBars.Vertical,
                WordWrap      = false,
                DetectUrls    = false,
            };

            // Input
            _input = new TextBox
            {
                Dock         = DockStyle.Bottom,
                Height       = 24,
                BorderStyle  = BorderStyle.FixedSingle,
            };
            _input.KeyDown  += OnInputKeyDown;

            Controls.Add(_output);
            Controls.Add(_input);
            Controls.Add(_toolbar);

            ApplyThemeColors();
            LayoutToolbar();
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────

        /// <summary>Starts the shell process.</summary>
        public void Start()  => _host.Start();

        /// <summary>Stops the shell process.</summary>
        public void Stop()   => _host.Stop();

        /// <summary>Restarts the shell process.</summary>
        public void Restart() { _host.Stop(); _output.Clear(); _host.Start(); }

        // ── I/O ───────────────────────────────────────────────────────────────

        private void OnHostDataReceived(object? sender, string text)
        {
            if (!IsHandleCreated) return;
            if (InvokeRequired)
            {
                Invoke(() => AppendOutput(text));
                return;
            }
            AppendOutput(text);
        }

        private void AppendOutput(string text)
        {
            _output.SuspendLayout();
            _output.AppendText(text);
            _output.ScrollToCaret();
            _output.ResumeLayout();
        }

        private void OnHostExited(object? sender, int code)
        {
            AppendOutput($"\r\n[Process exited with code {code}]\r\n");
        }

        private void OnInputKeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    var line = _input.Text;
                    _input.Clear();
                    if (!string.IsNullOrEmpty(line))
                    {
                        _history.Add(line);
                        _historyIndex = _history.Count;
                    }
                    _host.SendLine(line);
                    AppendOutput("> " + line + "\n");
                    e.Handled = e.SuppressKeyPress = true;
                    break;

                case Keys.Up:
                    if (_historyIndex > 0)
                    {
                        _historyIndex--;
                        _input.Text = _history[_historyIndex];
                        _input.SelectionStart = _input.Text.Length;
                    }
                    e.Handled = e.SuppressKeyPress = true;
                    break;

                case Keys.Down:
                    if (_historyIndex < _history.Count - 1)
                    {
                        _historyIndex++;
                        _input.Text = _history[_historyIndex];
                    }
                    else
                    {
                        _historyIndex = _history.Count;
                        _input.Clear();
                    }
                    e.Handled = e.SuppressKeyPress = true;
                    break;

                case Keys.C when e.Control:
                    _host.Send("\x03");   // Ctrl+C
                    e.Handled = e.SuppressKeyPress = true;
                    break;
            }
        }

        // ── Commands ──────────────────────────────────────────────────────────

        public void ClearOutput()
        {
            _output.Clear();
            _host.ClearScrollback();
        }

        public string GetScrollbackText() => _output.Text;

        // ── Theming ───────────────────────────────────────────────────────────

        private void ApplyThemeColors()
        {
            BackColor         = _theme.Background;
            _output.BackColor = _theme.Background;
            _output.ForeColor = _theme.Foreground;
            _input.BackColor  = Color.FromArgb(
                Math.Min(255, _theme.Background.R + 20),
                Math.Min(255, _theme.Background.G + 20),
                Math.Min(255, _theme.Background.B + 20));
            _input.ForeColor  = _theme.Foreground;
            _toolbar.BackColor = Color.FromArgb(
                Math.Max(0, _theme.Background.R - 10),
                Math.Max(0, _theme.Background.G - 10),
                Math.Max(0, _theme.Background.B - 10));
            _lblTitle.ForeColor = _theme.Foreground;
        }

        private void LayoutToolbar()
        {
            int x = 4;
            _lblTitle.Location = new Point(x, 4);
            x += _lblTitle.Width + 8;
            _btnClear.Location = new Point(x, 2);
            x += _btnClear.Width + 4;
            _btnSplit.Location = new Point(x, 2);
        }

        // ── Dispose ───────────────────────────────────────────────────────────

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _host.Dispose(); _output.Dispose(); _input.Dispose(); }
            base.Dispose(disposing);
        }
    }
}
