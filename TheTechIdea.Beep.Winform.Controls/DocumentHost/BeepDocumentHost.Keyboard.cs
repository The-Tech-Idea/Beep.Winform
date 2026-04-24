// BeepDocumentHost.Keyboard.cs
// Host-level keyboard shortcuts and Ctrl+K chord system for BeepDocumentHost.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentHost
    {
        // ── Chord state ───────────────────────────────────────────────────────

        private bool   _chordPending;
        private readonly System.Windows.Forms.Timer _chordTimer = new() { Interval = 2000 };
        private Label? _chordHintLabel;

        // ── Chord map: second key → command id ───────────────────────────────

        private static readonly Dictionary<Keys, string> ChordMap = new()
        {
            [Keys.OemBackslash] = "split.horizontal",
            [Keys.Oem5]         = "split.horizontal",
            [Keys.OemPipe]      = "split.vertical",
            [Keys.W]            = "document.close.all",
            [Keys.B]            = "breadcrumb.toggle",
            [Keys.T]            = "tabs.cycle.position",
            [Keys.S]            = "tabs.cycle.style",
            [Keys.H]            = "shortcut.help",
            [Keys.F]            = "document.float",
            [Keys.P]            = "document.pin",
            [Keys.Left]         = "split.focus.left",
            [Keys.Right]        = "split.focus.right",
            [Keys.Up]           = "split.focus.up",
            [Keys.Down]         = "split.focus.down",
        };

        // ── Chord helpers ─────────────────────────────────────────────────────

        private void BeginChord()
        {
            _chordPending = true;
            _chordTimer.Stop();
            _chordTimer.Tick -= OnChordTimeout;
            _chordTimer.Tick += OnChordTimeout;
            _chordTimer.Start();
            ShowChordHint("Ctrl+K — waiting for second key  (Esc or wait to cancel)");
        }

        private void CancelChord()
        {
            _chordPending = false;
            _chordTimer.Stop();
            HideChordHint();
        }

        private void OnChordTimeout(object? sender, EventArgs e) => CancelChord();

        private void ShowChordHint(string text)
        {
            if (_chordHintLabel == null)
            {
                _chordHintLabel = new Label
                {
                    AutoSize    = false,
                    TextAlign   = ContentAlignment.MiddleCenter,
                    BorderStyle = BorderStyle.None,
                    Font        = BeepFontManager.GetCachedFont("Segoe UI", 9.5f, FontStyle.Regular),
                    Visible     = false,
                };
                Controls.Add(_chordHintLabel);
                _chordHintLabel.BringToFront();
            }

            _chordHintLabel.BackColor = KeyboardThemeHelpers.ThemeAwareInfo(_currentTheme?.PanelBackColor);
            _chordHintLabel.ForeColor = KeyboardThemeHelpers.ThemeAwareInfoText(_currentTheme?.PanelBackColor);
            _chordHintLabel.Text      = text;
            _chordHintLabel.SetBounds(0, Height - 26, Width, 26);
            _chordHintLabel.Visible   = true;
            _chordHintLabel.BringToFront();
        }

        private void HideChordHint()
        {
            if (_chordHintLabel != null)
                _chordHintLabel.Visible = false;
        }

        // ── Command dispatch ──────────────────────────────────────────────────

        private void ExecuteCommandById(string id)
        {
            // Try registry first
            if (_commandRegistry != null)
            {
                var cmd = _commandRegistry.FindById(id);
                if (cmd != null && (cmd.CanExecute?.Invoke() ?? true))
                {
                    _commandRegistry.RecordUsage(id);
                    cmd.Execute();
                    return;
                }
            }

            // Built-in fallbacks for commands that are wired after registry creation
            switch (id)
            {
                case "view.fullscreen":                ToggleFullscreen();                           break;
                case "shortcut.help":                  ShowShortcutHelp();                           break;
                case "navigate.workspace.switcher":    ShowWorkspaceSwitcher();                      break;
                case "split.focus.left":               FocusSplitGroup(-1);                          break;
                case "split.focus.right":              FocusSplitGroup(+1);                          break;
                case "split.focus.up":                 FocusSplitGroup(-1);                          break;
                case "split.focus.down":               FocusSplitGroup(+1);                          break;
            }
        }

        // ── Built-in action implementations ──────────────────────────────────

        private void ToggleFullscreen()
        {
            var form = FindForm();
            if (form == null) return;

            if (form.FormBorderStyle != FormBorderStyle.None)
            {
                form.Tag             = form.FormBorderStyle;
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState     = FormWindowState.Maximized;
            }
            else
            {
                form.FormBorderStyle = form.Tag is FormBorderStyle fbs
                    ? fbs : FormBorderStyle.Sizable;
                form.WindowState = FormWindowState.Normal;
            }
        }

        // ── Command palette ───────────────────────────────────────────────────

        /// <summary>Shows the command palette or Go-to-File popup.</summary>
        internal void ShowCommandPalette(CommandPaletteMode mode)
        {
            var pos  = PointToScreen(new Point(Math.Max(0, (Width - 520) / 2),
                                               Math.Max(0, Height / 4)));
            var tabs = GetAllTabs();

            using var popup = new BeepCommandPalettePopup(
                EnsureCommandRegistry(), tabs, _currentTheme, mode, pos);
            popup.ShowDialog(this);

            if (mode == CommandPaletteMode.Commands && popup.SelectedCommand != null)
            {
                if (popup.SelectedCommand.CanExecute?.Invoke() ?? true)
                    popup.SelectedCommand.Execute();
            }
            else if (mode == CommandPaletteMode.GoToFile && popup.SelectedDocumentId != null)
            {
                if (popup.OpenInSplit)
                    SplitDocumentHorizontal(popup.SelectedDocumentId);
                else
                    SetActiveDocument(popup.SelectedDocumentId);
            }
        }

        // ── Workspace switcher popup ──────────────────────────────────────────

        /// <summary>Shows the workspace switcher popup.</summary>
        internal void ShowWorkspaceSwitcher()
        {
            var pos = PointToScreen(new Point(Math.Max(0, (Width - 480) / 2),
                                              Math.Max(0, Height / 4)));
            using var popup = new BeepWorkspaceSwitcherPopup(Workspaces, _currentTheme, pos);
            popup.ShowDialog(this);

            if (!string.IsNullOrEmpty(popup.SelectedWorkspaceName))
                SwitchWorkspace(popup.SelectedWorkspaceName);
            else if (!string.IsNullOrEmpty(popup.NewWorkspaceName))
                SaveWorkspace(popup.NewWorkspaceName);
        }

        // ── Shortcut help popup ───────────────────────────────────────────────

        /// <summary>Shows the keyboard shortcut help dialog.</summary>
        internal void ShowShortcutHelp()
        {
            var pos = PointToScreen(new Point(Math.Max(0, (Width  - 560) / 2),
                                              Math.Max(0, (Height - 500) / 2)));
            using var dlg = new BeepDocumentShortcutHelp(EnsureCommandRegistry(), _currentTheme, pos);
            dlg.ShowDialog(this);
        }

        // ── All-tabs helper ───────────────────────────────────────────────────

        private IReadOnlyList<BeepDocumentTab> GetAllTabs()
        {
            var tabs = new List<BeepDocumentTab>();
            foreach (var group in _groups)
                tabs.AddRange(group.TabStrip.Tabs);
            return tabs;
        }

        // ── Split focus cycling ───────────────────────────────────────────────

        private void FocusSplitGroup(int direction)
        {
            if (_groups.Count <= 1) return;
            int idx  = _groups.IndexOf(_activeGroup);
            if (idx < 0) return;
            int next = (idx + direction + _groups.Count) % _groups.Count;
            _activeGroup = _groups[next];
            _activeGroup.TabStrip.Focus();
        }

        // ── Theme-aware color fallbacks ──────────────────────────────────────

        private static class KeyboardThemeHelpers
        {
            internal static Color ThemeAwareInfo(Color? refColor)
            {
                if (refColor.HasValue && IsDarkBackground(refColor.Value))
                    return Color.FromArgb(50, 50, 55);
                return SystemColors.Info;
            }

            internal static Color ThemeAwareInfoText(Color? refColor)
            {
                if (refColor.HasValue && IsDarkBackground(refColor.Value))
                    return Color.White;
                return SystemColors.InfoText;
            }

            private static bool IsDarkBackground(Color c) => c.GetBrightness() < 0.5;
        }
    }
}
