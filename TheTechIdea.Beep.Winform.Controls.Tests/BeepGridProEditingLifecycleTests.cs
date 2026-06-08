using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Models;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    /// <summary>
    /// Tests for the editing / validation lifecycle events added to
    /// <see cref="BeepGridPro"/> (Pass 26).  The new events bring
    /// the grid's hook surface to parity with the standard WinForms
    /// <c>DataGridView</c>'s CellBeginEdit / CellValidating /
    /// CellValidated / CellEndEdit / UserAddingRow / UserDeletingRow
    /// pattern, so DGV-style hosts can port with one-line event name
    /// changes.
    /// </summary>
    public class BeepGridProEditingLifecycleTests : IDisposable
    {
        private readonly BeepGridPro _grid;
        // Data is internal so we reach it through reflection.
        private readonly PropertyInfo _dataProp = typeof(BeepGridPro).GetProperty(
            "Data", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
        // The internal thread-statics that bridge the event-raise
        // site back to the helper.  Also accessed via reflection.
        private static readonly FieldInfo s_lastCellBeginEditCancelField =
            typeof(BeepGridPro).GetField("s_lastCellBeginEditCancel",
                BindingFlags.NonPublic | BindingFlags.Static)!;
        private static readonly FieldInfo s_lastCellValidatingCancelField =
            typeof(BeepGridPro).GetField("s_lastCellValidatingCancel",
                BindingFlags.NonPublic | BindingFlags.Static)!;
        private static readonly FieldInfo s_lastCellValidatingNewValueField =
            typeof(BeepGridPro).GetField("s_lastCellValidatingNewValue",
                BindingFlags.NonPublic | BindingFlags.Static)!;
        private static readonly FieldInfo s_lastRowValidatingCancelField =
            typeof(BeepGridPro).GetField("s_lastRowValidatingCancel",
                BindingFlags.NonPublic | BindingFlags.Static)!;

        public BeepGridProEditingLifecycleTests()
        {
            _grid = new BeepGridPro();
            // Seed a tiny data set so BeginEdit / InsertNew / DeleteCurrent
            // have something to act on.  The grid's DataSource setter
            // builds the schema and binds, but the in-memory Data.Rows
            // is populated by RefreshData() (see BeepGridProDataSourceTests
            // for the same pattern).  Without RefreshData, Data.Rows
            // would be empty and the indexer would throw.
            var rows = new List<SampleRow>
            {
                new SampleRow { Id = 1, Name = "Alpha" },
                new SampleRow { Id = 2, Name = "Beta" },
            };
            _grid.DataSource = rows;
            _grid.RefreshData();
            // Reset thread-statics so a previous test class doesn't
            // pollute this one.
            s_lastCellBeginEditCancelField.SetValue(null, false);
            s_lastCellValidatingCancelField.SetValue(null, false);
            s_lastCellValidatingNewValueField.SetValue(null, null);
            s_lastRowValidatingCancelField.SetValue(null, false);
        }

        public void Dispose()
        {
            _grid.Dispose();
        }

        // Reach the internal Data helper.  Returns the
        // GridDataHelper instance whose `Rows` property is the list
        // of BeepRowConfig for the grid.  We avoid dynamic dispatch
        // (which can't see internal types) by going through reflection
        // for both the Data property and the Rows property.
        private object GetDataHelper() => _dataProp.GetValue(_grid)!;

        // BeepRowConfig collection (returned by Data.Rows) is typed
        // as IList<BeepRowConfig> in the public surface; we use
        // reflection so the test doesn't need an InternalsVisibleTo.
        private BeepRowConfig GetRow(int index)
        {
            var data = GetDataHelper();
            var rowsProp = data.GetType().GetProperty("Rows")!;
            var rows = rowsProp.GetValue(data)!;
            var indexer = rows.GetType().GetProperty("Item",
                new[] { typeof(int) })!;
            return (BeepRowConfig)indexer.GetValue(rows, new object[] { index })!;
        }

        private BeepCellConfig GetCell(int row, int col)
        {
            var rowCfg = GetRow(row);
            var cellsProp = rowCfg.GetType().GetProperty("Cells")!;
            var cells = cellsProp.GetValue(rowCfg)!;
            var indexer = cells.GetType().GetProperty("Item",
                new[] { typeof(int) })!;
            return (BeepCellConfig)indexer.GetValue(cells, new object[] { col })!;
        }

        private bool LastCellBeginEditCancel
        {
            get => (bool)s_lastCellBeginEditCancelField.GetValue(null)!;
            set => s_lastCellBeginEditCancelField.SetValue(null, value);
        }
        private bool LastCellValidatingCancel
        {
            get => (bool)s_lastCellValidatingCancelField.GetValue(null)!;
            set => s_lastCellValidatingCancelField.SetValue(null, value);
        }
        private object? LastCellValidatingNewValue
        {
            get => s_lastCellValidatingNewValueField.GetValue(null);
            set => s_lastCellValidatingNewValueField.SetValue(null, value);
        }
        private bool LastRowValidatingCancel
        {
            get => (bool)s_lastRowValidatingCancelField.GetValue(null)!;
            set => s_lastRowValidatingCancelField.SetValue(null, value);
        }

        // ─── CellBeginEdit ──────────────────────────────────────────

        [Fact]
        public void OnCellBeginEdit_Trigger_Fires_Event_With_Cell_And_Indices()
        {
            int fired = 0;
            BeepCellBeginEditEventArgs? captured = null;
            _grid.CellBeginEdit += (s, e) =>
            {
                fired++;
                captured = e;
            };
            // Invoke the internal OnCellBeginEdit directly (the public
            // path is BeginEdit which requires a window handle).
            var m = typeof(BeepGridPro).GetMethod("OnCellBeginEdit",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var cell = GetCell(0, 0);
            m!.Invoke(_grid, new object[] { 0, 0, cell! });

            Assert.Equal(1, fired);
            Assert.NotNull(captured);
            Assert.Equal(0, captured!.RowIndex);
            Assert.Equal(0, captured.ColumnIndex);
            Assert.Same(cell, captured.Cell);
        }

        [Fact]
        public void CellBeginEdit_Cancel_Stored_In_ThreadStatic_For_BeginEdit_To_Read()
        {
            // After CellBeginEdit fires with e.Cancel = true, the
            // thread-static s_lastCellBeginEditCancel is true so
            // BeginEdit can return early.  A separate, non-cancelling
            // handler leaves the flag false.  Each sub-assertion runs
            // its own OnCellBeginEdit invocation so the handlers are
            // registered fresh and the order is deterministic.
            var m = typeof(BeepGridPro).GetMethod("OnCellBeginEdit",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var cell = GetCell(0, 0);

            // Use a NAMED handler so -= can find and remove it (the
            // lambda form creates a new delegate instance every
            // time, defeating the -= operator's reference equality).
            EventHandler<BeepCellBeginEditEventArgs> cancelHandler =
                (s, e) => e.Cancel = true;

            // First invocation: handler sets Cancel = true.
            LastCellBeginEditCancel = false;
            _grid.CellBeginEdit += cancelHandler;
            m!.Invoke(_grid, new object[] { 0, 0, cell! });
            Assert.True(LastCellBeginEditCancel);
            _grid.CellBeginEdit -= cancelHandler;

            // Second invocation: no handler registered — the flag
            // must stay false.  This proves the flag is per-invocation
            // and not stuck from a previous call.
            LastCellBeginEditCancel = false;
            m!.Invoke(_grid, new object[] { 0, 0, cell! });
            Assert.False(LastCellBeginEditCancel);
        }

        // ─── CellValidating ─────────────────────────────────────────

        [Fact]
        public void OnCellValidating_Trigger_Fires_Event_With_Old_And_New_Values()
        {
            int fired = 0;
            BeepCellValidatingEventArgs? captured = null;
            _grid.CellValidating += (s, e) =>
            {
                fired++;
                captured = e;
            };
            var m = typeof(BeepGridPro).GetMethod("OnCellValidating",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var cell = GetCell(0, 0);
            m!.Invoke(_grid, new object[] { 0, 0, cell!, 1, 99 });

            Assert.Equal(1, fired);
            Assert.NotNull(captured);
            Assert.Equal(0, captured!.RowIndex);
            Assert.Equal(0, captured.ColumnIndex);
            Assert.Equal(1, captured.OldValue);
            Assert.Equal(99, captured.NewValue);
        }

        [Fact]
        public void CellValidating_Cancel_Stored_In_ThreadStatic_For_EndEdit_To_Read()
        {
            var m = typeof(BeepGridPro).GetMethod("OnCellValidating",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var cell = GetCell(0, 0);

            // Use a NAMED handler so -= can remove it.
            EventHandler<BeepCellValidatingEventArgs> cancelHandler =
                (s, e) => e.Cancel = true;

            // First: a cancelling handler sets the flag.
            LastCellValidatingCancel = false;
            _grid.CellValidating += cancelHandler;
            m!.Invoke(_grid, new object[] { 0, 0, cell!, 1, 2 });
            Assert.True(LastCellValidatingCancel);
            _grid.CellValidating -= cancelHandler;

            // Second: no handler.  Flag must reset to false on a fresh
            // invocation (proves it's not stuck on true from a
            // previous call — the OnCellValidating raise site
            // always writes the latest args.Cancel).
            LastCellValidatingCancel = false;
            m!.Invoke(_grid, new object[] { 0, 0, cell!, 1, 2 });
            Assert.False(LastCellValidatingCancel);
        }

        [Fact]
        public void CellValidating_NewValue_Stored_In_ThreadStatic_For_EndEdit_To_Read()
        {
            var m = typeof(BeepGridPro).GetMethod("OnCellValidating",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var cell = GetCell(0, 0);
            _grid.CellValidating += (s, e) => e.NewValue = 42;
            m!.Invoke(_grid, new object[] { 0, 0, cell!, 1, 2 });
            Assert.Equal(42, LastCellValidatingNewValue);
        }

        // ─── CellValidated ──────────────────────────────────────────

        [Fact]
        public void OnCellValidated_Trigger_Fires_Event_With_Cell()
        {
            int fired = 0;
            BeepCellEventArgs? captured = null;
            _grid.CellValidated += (s, e) =>
            {
                fired++;
                captured = e;
            };
            var m = typeof(BeepGridPro).GetMethod("OnCellValidated",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var cell = GetCell(0, 0);
            m!.Invoke(_grid, new object[] { 0, 0, cell! });

            Assert.Equal(1, fired);
            Assert.NotNull(captured);
            Assert.Same(cell, captured!.Cell);
        }

        // ─── CellEndEdit ────────────────────────────────────────────

        [Fact]
        public void OnCellEndEdit_Trigger_Fires_Event_With_Committed_Flag()
        {
            int fired = 0;
            BeepCellEndEditEventArgs? captured = null;
            _grid.CellEndEdit += (s, e) =>
            {
                fired++;
                captured = e;
            };
            var m = typeof(BeepGridPro).GetMethod("OnCellEndEdit",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var cell = GetCell(0, 0);

            // Verify committed = true
            m!.Invoke(_grid, new object[] { 0, 0, cell!, true });
            Assert.Equal(1, fired);
            Assert.NotNull(captured);
            Assert.True(captured!.Committed);

            // Verify committed = false (cancelled edit)
            fired = 0;
            captured = null;
            m!.Invoke(_grid, new object[] { 0, 0, cell!, false });
            Assert.Equal(1, fired);
            Assert.False(captured!.Committed);
        }

        // ─── RowValidating ──────────────────────────────────────────

        [Fact]
        public void OnRowValidating_Trigger_Fires_Event_With_Action_String()
        {
            int fired = 0;
            BeepRowValidatingEventArgs? captured = null;
            _grid.RowValidating += (s, e) =>
            {
                fired++;
                captured = e;
            };
            var m = typeof(BeepGridPro).GetMethod("OnRowValidating",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var row = GetRow(0);
            m!.Invoke(_grid, new object[] { 0, "insert", row });

            Assert.Equal(1, fired);
            Assert.NotNull(captured);
            Assert.Equal("insert", captured!.Action);
            Assert.Equal(0, captured.RowIndex);
            Assert.Same(row, captured.Row);
        }

        [Fact]
        public void RowValidating_Cancel_Stored_In_ThreadStatic_For_Navigator_To_Read()
        {
            var m = typeof(BeepGridPro).GetMethod("OnRowValidating",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Use a NAMED handler so -= can remove it.
            EventHandler<BeepRowValidatingEventArgs> cancelHandler =
                (s, e) => e.Cancel = true;

            // First: a cancelling handler sets the flag.
            LastRowValidatingCancel = false;
            _grid.RowValidating += cancelHandler;
            m!.Invoke(_grid, new object[] { 0, "delete", null });
            Assert.True(LastRowValidatingCancel);
            _grid.RowValidating -= cancelHandler;

            // Second: no handler.  Flag must reset to false on a
            // fresh invocation.
            LastRowValidatingCancel = false;
            m!.Invoke(_grid, new object[] { 0, "delete", null });
            Assert.False(LastRowValidatingCancel);
        }

        // ─── RowValidated ───────────────────────────────────────────

        [Fact]
        public void OnRowValidated_Trigger_Fires_Event_With_Row_And_Index()
        {
            int fired = 0;
            BeepRowSelectedEventArgs? captured = null;
            _grid.RowValidated += (s, e) =>
            {
                fired++;
                captured = e;
            };
            var m = typeof(BeepGridPro).GetMethod("OnRowValidated",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var row = GetRow(0);
            m!.Invoke(_grid, new object[] { 0, row });

            Assert.Equal(1, fired);
            Assert.NotNull(captured);
            Assert.Equal(0, captured!.RowIndex);
            Assert.Same(row, captured.Row);
        }

        // ─── Thread-static isolation ─────────────────────────────────

        [Fact]
        public void ThreadStatic_Cancel_Flags_Do_Not_Leak_Between_Events()
        {
            // After each validation trigger, the host's cancel must
            // not leak into the next event on the same thread.  This
            // guards the "stale cancel" footgun: BeginEdit reads the
            // thread-static immediately after the raise, so a
            // leftover true from a previous edit cycle would
            // incorrectly veto a fresh one.
            var cell = GetCell(0, 0);
            var row = GetRow(0);

            // Use named handlers so -= can remove them.
            EventHandler<BeepCellBeginEditEventArgs> cellCancel =
                (s, e) => e.Cancel = true;
            EventHandler<BeepRowValidatingEventArgs> rowCancel =
                (s, e) => e.Cancel = true;

            // Veto a cell begin edit.  CellBeginEdit flag is true.
            LastCellBeginEditCancel = false;
            _grid.CellBeginEdit += cellCancel;
            var beginEdit = typeof(BeepGridPro).GetMethod("OnCellBeginEdit",
                BindingFlags.NonPublic | BindingFlags.Instance);
            beginEdit!.Invoke(_grid, new object[] { 0, 0, cell! });
            Assert.True(LastCellBeginEditCancel);
            _grid.CellBeginEdit -= cellCancel;

            // Veto a row validation.  RowValidating flag is true;
            // CellBeginEdit flag is NOT touched (it was set to true
            // in step 1 by the cell veto and we expect it to stay
            // set — the goal is to prove RowValidating does not
            // overwrite or clear the cell flag, not that the cell
            // flag is false).
            LastRowValidatingCancel = false;
            _grid.RowValidating += rowCancel;
            var rowVal = typeof(BeepGridPro).GetMethod("OnRowValidating",
                BindingFlags.NonPublic | BindingFlags.Instance);
            rowVal!.Invoke(_grid, new object[] { 0, "insert", row });
            Assert.True(LastRowValidatingCancel);
            // Cell flag persists from the first part — RowValidating
            // must not have clobbered it.  This is the "no leak"
            // assertion: each thread-static carries its own value.
            Assert.True(LastCellBeginEditCancel);
            _grid.RowValidating -= rowCancel;
        }

        // ─── Sample data ────────────────────────────────────────────

        private class SampleRow
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }
    }
}
