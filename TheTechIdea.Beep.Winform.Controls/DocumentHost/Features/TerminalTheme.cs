// TerminalTheme.cs
// Theme tokens for TerminalPanel — background, foreground, cursor colour,
// and ANSI colour palette (8 standard + 8 bright).
// ─────────────────────────────────────────────────────────────────────────────
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Features
{
    /// <summary>
    /// Colour scheme for a <see cref="TerminalPanel"/>.
    /// Provides a <see cref="Dark"/> and a <see cref="Light"/> built-in theme
    /// plus a fully customisable instance for designer use.
    /// </summary>
    public sealed class TerminalTheme
    {
        public Color Background  { get; set; } = Color.FromArgb(30, 30, 30);
        public Color Foreground  { get; set; } = Color.FromArgb(212, 212, 212);
        public Color Cursor      { get; set; } = Color.FromArgb(0, 175, 255);
        public Color SelectionBg { get; set; } = Color.FromArgb(80, 80, 200);
        public Color SelectionFg { get; set; } = Color.White;

        // ANSI 16-colour palette (0–7 standard, 8–15 bright)
        public Color[] AnsiColors { get; set; } = DefaultAnsiColors();

        // ── Built-in themes ───────────────────────────────────────────────────

        /// <summary>Classic dark terminal (VS Code defaults).</summary>
        public static TerminalTheme Dark => new();

        /// <summary>Light terminal theme for high-contrast environments.</summary>
        public static TerminalTheme Light => new()
        {
            Background  = Color.White,
            Foreground  = Color.Black,
            Cursor      = Color.Blue,
            SelectionBg = Color.LightBlue,
            SelectionFg = Color.Black,
        };

        // ── Helpers ───────────────────────────────────────────────────────────

        private static Color[] DefaultAnsiColors() =>
        [
            // Standard
            Color.Black, Color.DarkRed, Color.DarkGreen, Color.DarkGoldenrod,
            Color.DarkBlue, Color.DarkMagenta, Color.DarkCyan, Color.LightGray,
            // Bright
            Color.DimGray, Color.Red, Color.LimeGreen, Color.Yellow,
            Color.CornflowerBlue, Color.Fuchsia, Color.Cyan, Color.White,
        ];
    }
}
