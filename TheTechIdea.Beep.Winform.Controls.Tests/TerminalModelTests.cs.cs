// TerminalModelTests.cs
// Unit tests for TerminalTheme and TerminalHost (non-UI, non-process-launch aspects).
// ─────────────────────────────────────────────────────────────────────────────
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Features;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class TerminalModelTests
    {
        // ── TerminalTheme — Dark preset ───────────────────────────────────────

        [Fact]
        public void DarkTheme_BackgroundIsDark()
        {
            var theme = TerminalTheme.Dark;
            // Luminance < 0.5 ≈ dark
            Assert.True(theme.Background.GetBrightness() < 0.5f);
        }

        [Fact]
        public void DarkTheme_ForegroundIsLight()
        {
            var theme = TerminalTheme.Dark;
            Assert.True(theme.Foreground.GetBrightness() > 0.5f);
        }

        [Fact]
        public void DarkTheme_AnsiColorsHas16Entries()
        {
            var theme = TerminalTheme.Dark;
            Assert.Equal(16, theme.AnsiColors.Length);
        }

        [Fact]
        public void DarkTheme_NoAnsiColorIsTransparent()
        {
            var theme = TerminalTheme.Dark;
            Assert.All(theme.AnsiColors, c => Assert.NotEqual(Color.Transparent, c));
        }

        // ── TerminalTheme — Light preset ──────────────────────────────────────

        [Fact]
        public void LightTheme_BackgroundIsLight()
        {
            var theme = TerminalTheme.Light;
            Assert.True(theme.Background.GetBrightness() > 0.5f);
        }

        [Fact]
        public void LightTheme_ForegroundIsDark()
        {
            var theme = TerminalTheme.Light;
            Assert.True(theme.Foreground.GetBrightness() < 0.5f);
        }

        [Fact]
        public void LightTheme_AnsiColorsHas16Entries()
        {
            var theme = TerminalTheme.Light;
            Assert.Equal(16, theme.AnsiColors.Length);
        }

        [Fact]
        public void DarkAndLightTheme_HaveDifferentBackgrounds()
        {
            var dark  = TerminalTheme.Dark;
            var light = TerminalTheme.Light;
            Assert.NotEqual(dark.Background, light.Background);
        }

        // ── TerminalTheme — Colour properties ─────────────────────────────────

        // [Fact]
        // public void DarkTheme_CursorColorIsNotEmpty()
        // {
        //     var theme = TerminalTheme.Dark;
        //     Assert.NotEqual(Color.Empty, theme.CursorColor);
        // }

        // [Fact]
        // public void DarkTheme_SelectionColorIsNotEmpty()
        // {
        //     var theme = TerminalTheme.Dark;
        //     Assert.NotEqual(Color.Empty, theme.SelectionColor);
        // }

        // ── TerminalHost — configuration ──────────────────────────────────────

        [Fact]
        public void DefaultShell_IsPowerShell()
        {
            var host = new TerminalHost();
            Assert.Equal(TerminalShell.PowerShell, host.Shell);
        }

        [Fact]
        public void SetShell_RetainedCorrectly()
        {
            var host = new TerminalHost { Shell = TerminalShell.Cmd };
            Assert.Equal(TerminalShell.Cmd, host.Shell);
        }

        [Fact]
        public void DefaultScrollbackLines_IsPositive()
        {
            var host = new TerminalHost();
            Assert.True(host.ScrollbackLines > 0);
        }

        [Fact]
        public void SetScrollbackLines_RetainedCorrectly()
        {
            var host = new TerminalHost { ScrollbackLines = 2000 };
            Assert.Equal(2000, host.ScrollbackLines);
        }

        [Fact]
        public void DefaultWorkingDirectory_IsNotNull()
        {
            var host = new TerminalHost();
            Assert.NotNull(host.WorkingDirectory);
        }

        [Fact]
        public void SetWorkingDirectory_RetainedCorrectly()
        {
            var host = new TerminalHost { WorkingDirectory = @"C:\Temp" };
            Assert.Equal(@"C:\Temp", host.WorkingDirectory);
        }

        // ── TerminalHost — scrollback management (no process) ─────────────────

        [Fact]
        public void GetScrollback_WhenNothingWritten_ReturnsEmpty()
        {
            var host = new TerminalHost();
            Assert.Equal(string.Empty, host.GetScrollback());
        }

        [Fact]
        public void ClearScrollback_LeavesScrollbackEmpty()
        {
            var host = new TerminalHost();
            host.ClearScrollback();
            Assert.Equal(string.Empty, host.GetScrollback());
        }

        // ── TerminalShell enum ────────────────────────────────────────────────

        [Fact]
        public void TerminalShell_EnumContainsExpectedValues()
        {
            var values = Enum.GetValues<TerminalShell>();
            Assert.Contains(TerminalShell.Cmd,        values);
            Assert.Contains(TerminalShell.PowerShell, values);
            Assert.Contains(TerminalShell.Bash,       values);
            Assert.Contains(TerminalShell.Custom,     values);
        }
    }
}
