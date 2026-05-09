// BeepTabInputPolicy.cs
// Centralised policy object that governs what keyboard and pointer interactions
// are allowed for a given tab in a given mode.  Consumers should call
// BeepTabInputPolicy.For(item, mode) instead of scattering if/else guards across
// keyboard, mouse, and command handlers.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// Immutable snapshot of what a user can do with a specific tab in the current mode.
    /// Evaluated once per interaction; cached by the caller if called in a tight loop.
    /// </summary>
    public sealed class BeepTabInputPolicy
    {
        // ── Static factory ────────────────────────────────────────────────────

        /// <summary>
        /// Returns the input policy for <paramref name="item"/> when the tab control
        /// is operating in <paramref name="mode"/>.
        /// </summary>
        public static BeepTabInputPolicy For(BeepTabItem item, BeepTabMode mode)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            bool canNavigate = item.CanSelect && item.IsEnabled && item.IsVisible;
            bool canClose    = item.CanClose
                               && item.IsEnabled
                               && (mode == BeepTabMode.Navigation || !item.IsPinned);
            bool canPin      = mode != BeepTabMode.Navigation && item.IsEnabled;
            bool canReorder  = item.CanReorder && item.IsEnabled && !item.IsPinned;
            bool canActivate = canNavigate;

            return new BeepTabInputPolicy(
                canNavigate: canNavigate,
                canClose:    canClose,
                canPin:      canPin,
                canReorder:  canReorder,
                canActivate: canActivate);
        }

        // ── Constructor ──────────────────────────────────────────────────────

        private BeepTabInputPolicy(
            bool canNavigate,
            bool canClose,
            bool canPin,
            bool canReorder,
            bool canActivate)
        {
            CanNavigate = canNavigate;
            CanClose    = canClose;
            CanPin      = canPin;
            CanReorder  = canReorder;
            CanActivate = canActivate;
        }

        // ── Properties ───────────────────────────────────────────────────────

        /// <summary>Arrow keys and Tab can move keyboard focus to this tab.</summary>
        public bool CanNavigate { get; }

        /// <summary>The close shortcut (Delete / Ctrl+W) or close button may be used.</summary>
        public bool CanClose { get; }

        /// <summary>The pin toggle command is available.</summary>
        public bool CanPin { get; }

        /// <summary>The tab may be dragged to a new position.</summary>
        public bool CanReorder { get; }

        /// <summary>A click or Enter key selects the tab.</summary>
        public bool CanActivate { get; }

        // ── Keyboard shortcut helpers ─────────────────────────────────────────

        /// <summary>
        /// Returns <see langword="true"/> when <paramref name="keyData"/> should be
        /// handled as a close command for this tab.
        /// </summary>
        public bool IsCloseKey(Keys keyData)
        {
            if (!CanClose) return false;

            Keys keyCode = keyData & Keys.KeyCode;
            return keyCode == Keys.Delete
                || keyData == (Keys.Control | Keys.W);
        }

        /// <summary>
        /// Returns <see langword="true"/> when <paramref name="keyData"/> activates
        /// (selects) this tab.
        /// </summary>
        public bool IsActivateKey(Keys keyData)
        {
            if (!CanActivate) return false;

            Keys keyCode = keyData & Keys.KeyCode;
            return keyCode == Keys.Return || keyCode == Keys.Space;
        }

        // ── Well-known policy singletons ─────────────────────────────────────

        /// <summary>Policy that allows nothing — use for placeholder or hidden items.</summary>
        public static readonly BeepTabInputPolicy Blocked = new BeepTabInputPolicy(
            canNavigate: false,
            canClose:    false,
            canPin:      false,
            canReorder:  false,
            canActivate: false);
    }
}
