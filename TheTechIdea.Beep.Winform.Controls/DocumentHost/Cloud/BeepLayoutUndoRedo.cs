// BeepLayoutUndoRedo.cs
// Multi-level undo/redo for BeepDocumentHost layout changes.
// Maintains two stacks: past (undo) and future (redo) with a configurable depth limit.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Multi-level undo/redo manager for <see cref="BeepDocumentHost"/> layout changes.
    /// <para>
    /// Usage pattern:
    /// <list type="number">
    ///   <item>Call <see cref="Push"/> <em>before</em> applying a layout change,
    ///         passing the <em>current</em> layout JSON.</item>
    ///   <item>Call <see cref="Undo"/> to revert — the returned JSON is the previous state;
    ///         pass the <em>current</em> state so it can be pushed onto the redo stack.</item>
    ///   <item>Call <see cref="Redo"/> to move forward — returns the next future state.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class BeepLayoutUndoRedo
    {
        private readonly Stack<string> _undoStack = new();
        private readonly Stack<string> _redoStack = new();
        private int _maxDepth = 50;

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Raised whenever the undo/redo stacks change.</summary>
        public event EventHandler? StateChanged;

        // ── Configuration ─────────────────────────────────────────────────────

        /// <summary>
        /// Maximum number of undo levels kept.  Oldest entries are evicted when
        /// this limit is exceeded.  Defaults to 50.
        /// </summary>
        public int MaxDepth
        {
            get => _maxDepth;
            set => _maxDepth = Math.Max(1, Math.Min(500, value));
        }

        // ── State ─────────────────────────────────────────────────────────────

        /// <summary><c>true</c> when there is at least one layout state to undo.</summary>
        public bool CanUndo => _undoStack.Count > 0;

        /// <summary><c>true</c> when there is at least one layout state to redo.</summary>
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>Number of undo levels available.</summary>
        public int UndoCount => _undoStack.Count;

        /// <summary>Number of redo levels available.</summary>
        public int RedoCount => _redoStack.Count;

        // ── Core operations ───────────────────────────────────────────────────

        /// <summary>
        /// Saves <paramref name="currentLayoutJson"/> onto the undo stack.
        /// Call this <em>before</em> applying a new layout change.
        /// Clears the redo stack (any future states are discarded).
        /// </summary>
        public void Push(string currentLayoutJson)
        {
            if (string.IsNullOrWhiteSpace(currentLayoutJson)) return;

            _undoStack.Push(currentLayoutJson);
            _redoStack.Clear();

            // Trim undo stack to MaxDepth
            while (_undoStack.Count > _maxDepth)
            {
                var tmp = _undoStack.ToList();
                tmp.RemoveAt(tmp.Count - 1); // remove oldest
                _undoStack.Clear();
                foreach (var s in Enumerable.Reverse(tmp))
                    _undoStack.Push(s);
            }

            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Reverts one layout change.
        /// <paramref name="currentLayoutJson"/> is pushed onto the redo stack so it
        /// can be re-applied via <see cref="Redo"/>.
        /// </summary>
        /// <returns>The previous layout JSON, or <c>null</c> if there is nothing to undo.</returns>
        public string? Undo(string currentLayoutJson)
        {
            if (!CanUndo) return null;
            if (!string.IsNullOrWhiteSpace(currentLayoutJson))
                _redoStack.Push(currentLayoutJson);

            var previous = _undoStack.Pop();
            StateChanged?.Invoke(this, EventArgs.Empty);
            return previous;
        }

        /// <summary>
        /// Re-applies the last undone layout change.
        /// </summary>
        /// <returns>The next layout JSON, or <c>null</c> if there is nothing to redo.</returns>
        public string? Redo()
        {
            if (!CanRedo) return null;
            var next = _redoStack.Pop();
            StateChanged?.Invoke(this, EventArgs.Empty);
            return next;
        }

        /// <summary>Discards all undo and redo history.</summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        // ── History preview ───────────────────────────────────────────────────

        /// <summary>
        /// Returns a read-only ordered list of layout JSON strings on the undo stack
        /// (index 0 = most recent).  Intended for an undo history viewer.
        /// </summary>
        public IReadOnlyList<string> UndoHistory => _undoStack.ToList();

        /// <summary>
        /// Returns a read-only ordered list of layout JSON strings on the redo stack
        /// (index 0 = next to be redone).
        /// </summary>
        public IReadOnlyList<string> RedoHistory => _redoStack.ToList();
    }
}
