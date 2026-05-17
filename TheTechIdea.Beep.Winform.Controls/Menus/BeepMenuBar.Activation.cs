// BeepMenuBar.Activation.cs
// Phase 04A — Commercial-Grade Menubar UX.
//
// Owns the menubar's keyboard-activation state machine and the related
// "draw mnemonics?" gate. Three discrete states mirror WPF Menu's
// activation model:
//
//   Inactive          — keyboard ignores Alt-letter/arrow/Esc;
//                       mnemonic underlines are hidden.
//   ActiveNoPopup     — Alt was pressed (or arrow keys traversed in);
//                       mnemonic underlines are shown; arrow keys move
//                       top-level highlight; Down/Enter opens popup.
//   ActiveWithPopup   — a popup is currently up under the menubar's
//                       control. Hover-swap (Phase 04B) is allowed only
//                       in this state.
//
// Transitions are funneled through SetActivation so a single hook
// guarantees state-side-effects stay paired (mnemonic redraw, focus
// restore, etc.).
//
// See .plans/Menus-Phase-04-CommercialMenuBarUX.md.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Discrete activation states for <see cref="BeepMenuBar"/>'s keyboard
    /// state machine. Public so unit tests / accessibility code can read
    /// the current state, but the only setter lives on the menubar.
    /// </summary>
    public enum MenubarActivation
    {
        /// <summary>Keyboard ignores menubar hot keys; mnemonics hidden.</summary>
        Inactive,

        /// <summary>Menubar accepts arrow keys + Esc; mnemonics visible; no popup up.</summary>
        ActiveNoPopup,

        /// <summary>A popup launched from this menubar is currently visible.</summary>
        ActiveWithPopup
    }

    public partial class BeepMenuBar
    {
        // ─────────────────────────────────────────────────────────────────
        // State
        // ─────────────────────────────────────────────────────────────────

        private MenubarActivation _activation = MenubarActivation.Inactive;

        /// <summary>
        /// True when the menubar is currently drawing mnemonic underlines.
        /// Driven by <see cref="SetActivation"/> so this flag stays in
        /// lock-step with the public state.
        /// </summary>
        private bool _drawMnemonics;

        /// <summary>
        /// Control that owned focus when the menubar entered keyboard
        /// activation. Restored when activation returns to
        /// <see cref="MenubarActivation.Inactive"/> so keyboard users
        /// land back where they started.
        /// </summary>
        private Control _focusToRestore;

        // ─────────────────────────────────────────────────────────────────
        // Public surface
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Current keyboard activation state. Designer-hidden because the
        /// menubar drives it internally; exposed so unit tests +
        /// accessibility helpers can read it.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MenubarActivation Activation => _activation;

        /// <summary>
        /// True while mnemonic underlines are rendered. Designer-hidden;
        /// derived from <see cref="Activation"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DrawMnemonics => _drawMnemonics;

        /// <summary>
        /// Raised every time <see cref="Activation"/> changes. Subscribers
        /// receive the new state.
        /// </summary>
        public event EventHandler<MenubarActivation> ActivationChanged;

        // ─────────────────────────────────────────────────────────────────
        // Transitions
        //
        // SetActivation is the single chokepoint so the side-effects
        // (mnemonic toggle, focus restore, invalidate, event fire) stay
        // paired. Keyboard.cs and Popup.cs both go through here.
        // ─────────────────────────────────────────────────────────────────

        internal void SetActivation(MenubarActivation next)
        {
            if (_activation == next) return;

            var previous = _activation;
            _activation  = next;

            switch (next)
            {
                case MenubarActivation.Inactive:
                    _drawMnemonics = false;
                    RestoreFocusOnDeactivation();
                    break;

                case MenubarActivation.ActiveNoPopup:
                    _drawMnemonics = true;
                    if (previous == MenubarActivation.Inactive)
                    {
                        CaptureFocusForActivation();
                    }
                    break;

                case MenubarActivation.ActiveWithPopup:
                    // ActiveWithPopup keeps the mnemonic underlines drawn
                    // — VS / Office both leave them up so the user can see
                    // the next Alt-letter target without a flicker.
                    _drawMnemonics = true;
                    if (previous == MenubarActivation.Inactive)
                    {
                        CaptureFocusForActivation();
                    }
                    break;
            }

            Debug.WriteLine($"BeepMenuBar.Activation: {previous} -> {next}");

            // Phase 07-E — UIA announcement so screen readers track focus
            // when the user enters keyboard mode (or arrow-traverses the
            // bar). The bar itself has no input focus (CanBeFocused =
            // false), so this AccessibleEvents.Focus on the highlighted
            // item is what NVDA / Narrator latches onto.
            if (next != MenubarActivation.Inactive && _selectedIndex >= 0)
            {
                RaiseItemAccessibleEvent(AccessibleEvents.Focus, _selectedIndex);
            }

            Invalidate();
            try { ActivationChanged?.Invoke(this, next); }
            catch { /* subscriber errors must not break the menubar */ }
        }

        /// <summary>
        /// Convenience: deactivate from any state. Used by Esc, focus
        /// loss, and the Phase 01 dismissal handler.
        /// </summary>
        internal void DeactivateKeyboard()
        {
            if (_activation == MenubarActivation.Inactive) return;
            SetActivation(MenubarActivation.Inactive);
        }

        // ─────────────────────────────────────────────────────────────────
        // Focus capture/restore
        //
        // The menubar itself cannot take focus (CanBeFocused = false in
        // the constructor); we just remember who DID have focus when
        // keyboard activation started so we can hand it back on Esc.
        // ─────────────────────────────────────────────────────────────────

        private void CaptureFocusForActivation()
        {
            try
            {
                var form = FindForm();
                if (form == null) return;
                var active = form.ActiveControl;
                if (active != null && !ReferenceEquals(active, this))
                {
                    _focusToRestore = active;
                }
            }
            catch { /* defensive — never block activation */ }
        }

        private void RestoreFocusOnDeactivation()
        {
            var target = _focusToRestore;
            _focusToRestore = null;
            if (target == null || target.IsDisposed || !target.Visible) return;

            try
            {
                if (target.CanFocus) target.Focus();
            }
            catch { /* defensive */ }
        }
    }
}
