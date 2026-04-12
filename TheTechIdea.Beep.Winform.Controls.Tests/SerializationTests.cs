// SerializationTests.cs
// Unit tests for layout migration, BeepLayoutDiff, BeepLayoutHistory, BeepLayoutUndoRedo.
// ?????????????????????????????????????????????????????????????????????????????
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class SerializationTests
    {
        // ?? LayoutMigrationService — IsCurrentVersion ?????????????????????????

        [Fact]
        public void IsCurrentVersion_ReturnsTrueForSchemaV3()
        {
            string json = """{"schemaVersion":3,"groups":[]}""";
            Assert.True(LayoutMigrationService.IsCurrentVersion(json));
        }

        [Fact]
        public void IsCurrentVersion_ReturnsFalseForSchemaV2()
        {
            string json = """{"schemaVersion":2,"groups":[]}""";
            Assert.False(LayoutMigrationService.IsCurrentVersion(json));
        }

        [Fact]
        public void IsCurrentVersion_ReturnsFalseForEmptyString()
        {
            Assert.False(LayoutMigrationService.IsCurrentVersion(string.Empty));
        }

        [Fact]
        public void IsCurrentVersion_ReturnsFalseForNullString()
        {
            Assert.False(LayoutMigrationService.IsCurrentVersion(null!));
        }

        [Fact]
        public void IsCurrentVersion_ReturnsFalseForInvalidJson()
        {
            Assert.False(LayoutMigrationService.IsCurrentVersion("{not valid json"));
        }

        // ?? LayoutMigrationService — MigrateToLatest ??????????????????????????

        [Fact]
        public void MigrateToLatest_AlreadyV3_ReturnsSchemaVersion3()
        {
            string v3Json  = """{"schemaVersion":3,"groups":[]}""";
            string result  = LayoutMigrationService.MigrateToLatest(v3Json);
            Assert.True(LayoutMigrationService.IsCurrentVersion(result));
        }

        [Fact]
        public void MigrateToLatest_V0Input_ProducesCurrentVersion()
        {
            string v0Json = """{"groups":[]}""";
            string result = LayoutMigrationService.MigrateToLatest(v0Json);
            Assert.True(LayoutMigrationService.IsCurrentVersion(result));
        }

        [Fact]
        public void MigrateToLatest_EmptyJson_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                LayoutMigrationService.MigrateToLatest(string.Empty));
        }

        [Fact]
        public void MigrateToLatest_InvalidJson_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                LayoutMigrationService.MigrateToLatest("{bad json"));
        }

        [Fact]
        public void MigrateToLatest_ReturnsValidJsonString()
        {
            string v0 = """{"groups":[]}""";
            string result = LayoutMigrationService.MigrateToLatest(v0);
            Assert.False(string.IsNullOrWhiteSpace(result));
            // Should be parseable JSON
            Assert.Contains("{", result);
            Assert.Contains("}", result);
        }

        // ?? BeepLayoutDiff ????????????????????????????????????????????????????

        [Fact]
        public void Compare_EmptyInputs_ReturnsEmptyDiff()
        {
            var diff = BeepLayoutDiff.Compare(string.Empty, string.Empty);
            Assert.True(diff.IsIdentical);
        }

        [Fact]
        public void Compare_OneEmptyOnePopulated_ReturnsEmptyDiff()
        {
            var diff = BeepLayoutDiff.Compare(string.Empty, """{"schemaVersion":3}""");
            Assert.True(diff.IsIdentical);
        }

        [Fact]
        public void Compare_IdenticalValidJsonLayouts_ReturnsIdentical()
        {
            string json = """{"schemaVersion":3,"groups":[{"id":"g1","documents":["doc1"]}]}""";
            var diff    = BeepLayoutDiff.Compare(json, json);
            Assert.True(diff.IsIdentical);
        }

        [Fact]
        public void Compare_DifferentStructures_StructureChangedTrue()
        {
            string jsonA = """{"schemaVersion":3,"splitCount":1}""";
            string jsonB = """{"schemaVersion":3,"splitCount":2}""";
            var diff     = BeepLayoutDiff.Compare(jsonA, jsonB);
            Assert.NotNull(diff);
        }

        [Fact]
        public void Summary_ReturnsNonEmptyString_WhenNotIdentical()
        {
            string jsonA = """{"schemaVersion":3,"splitCount":1}""";
            string jsonB = """{"schemaVersion":3,"splitCount":2}""";
            var diff     = BeepLayoutDiff.Compare(jsonA, jsonB);
            Assert.NotNull(diff.Summary());
        }

        [Fact]
        public void Summary_IdenticalLayouts_ReturnsIdenticalMessage()
        {
            string json = """{"schemaVersion":3}""";
            var diff    = BeepLayoutDiff.Compare(json, json);
            Assert.Contains("identical", diff.Summary(), StringComparison.OrdinalIgnoreCase);
        }

        // ?? BeepLayoutHistory ?????????????????????????????????????????????????

        [Fact]
        public void BeepLayoutHistory_StartsEmpty()
        {
            var history = new BeepLayoutHistory();
            Assert.Equal(0, history.Count);
        }

        [Fact]
        public void Push_IncreasesHistoryCount()
        {
            var history = new BeepLayoutHistory();
            history.Push("""{"schemaVersion":3}""");
            Assert.Equal(1, history.Count);
        }

        [Fact]
        public void Push_ReturnsVersionEntry()
        {
            var history = new BeepLayoutHistory();
            var entry   = history.Push("""{"schemaVersion":3}""");
            Assert.NotNull(entry);
        }

        [Fact]
        public void Push_EntryContainsLayoutJson()
        {
            var history = new BeepLayoutHistory();
            string json = """{"schemaVersion":3,"label":"test"}""";
            var entry   = history.Push(json);
            Assert.Equal(json, entry.LayoutJson);
        }

        [Fact]
        public void MaxDepth_LimitsStoredEntries()
        {
            var history       = new BeepLayoutHistory { MaxDepth = 3 };
            string json       = """{"schemaVersion":3}""";
            history.Push(json);
            history.Push(json);
            history.Push(json);
            history.Push(json); // should evict oldest
            Assert.Equal(3, history.Count);
        }

        [Fact]
        public void GetAll_ReturnsAllEntries()
        {
            var history = new BeepLayoutHistory();
            history.Push("""{"schemaVersion":3}""");
            history.Push("""{"schemaVersion":3}""");
            Assert.Equal(2, history.GetAll().Count);
        }

        [Fact]
        public void GetAt_Zero_ReturnsMostRecentEntry()
        {
            var history = new BeepLayoutHistory();
            history.Push("""{"schemaVersion":3,"a":1}""");
            string json2 = """{"schemaVersion":3,"a":2}""";
            history.Push(json2);
            var entry = history.GetAt(0);
            Assert.NotNull(entry);
            Assert.Equal(json2, entry!.LayoutJson);
        }

        [Fact]
        public void GetAt_OutOfRange_ReturnsNull()
        {
            var history = new BeepLayoutHistory();
            Assert.Null(history.GetAt(0));
        }

        [Fact]
        public void Push_WithLabel_LabelIsStored()
        {
            var history = new BeepLayoutHistory();
            var entry   = history.Push("""{"schemaVersion":3}""", label: "my-label");
            Assert.Equal("my-label", entry.Label);
        }

        // ?? BeepLayoutUndoRedo ????????????????????????????????????????????????

        [Fact]
        public void BeepLayoutUndoRedo_StartsWithCanUndoFalse()
        {
            var ur = new BeepLayoutUndoRedo();
            Assert.False(ur.CanUndo);
        }

        [Fact]
        public void BeepLayoutUndoRedo_StartsWithCanRedoFalse()
        {
            var ur = new BeepLayoutUndoRedo();
            Assert.False(ur.CanRedo);
        }

        [Fact]
        public void Push_EnablesUndo()
        {
            var ur  = new BeepLayoutUndoRedo();
            ur.Push("""{"schemaVersion":3}""");
            Assert.True(ur.CanUndo);
        }

        [Fact]
        public void Undo_ReturnsLastPushedState()
        {
            var ur   = new BeepLayoutUndoRedo();
            string a = """{"schemaVersion":3,"v":1}""";
            string b = """{"schemaVersion":3,"v":2}""";
            ur.Push(a);
            ur.Push(b);
            string? undone = ur.Undo("""{"schemaVersion":3,"v":3}""");
            Assert.Equal(b, undone);
        }

        [Fact]
        public void Undo_WhenEmpty_ReturnsNull()
        {
            var ur = new BeepLayoutUndoRedo();
            Assert.Null(ur.Undo("""{"schemaVersion":3}"""));
        }

        [Fact]
        public void Undo_ThenRedo_ReturnsForwardState()
        {
            var ur    = new BeepLayoutUndoRedo();
            string v1 = """{"schemaVersion":3,"v":1}""";
            string v2 = """{"schemaVersion":3,"v":2}""";
            ur.Push(v1);
            ur.Push(v2);
            ur.Undo("""{"schemaVersion":3,"v":3}""");
            string? redone = ur.Redo();
            Assert.NotNull(redone);
        }

        [Fact]
        public void Redo_WhenEmpty_ReturnsNull()
        {
            var ur = new BeepLayoutUndoRedo();
            Assert.Null(ur.Redo());
        }

        [Fact]
        public void Push_AfterUndo_ClearsRedoStack()
        {
            var ur  = new BeepLayoutUndoRedo();
            string j = """{"schemaVersion":3}""";
            ur.Push(j);
            ur.Push(j);
            ur.Undo(j);
            Assert.True(ur.CanRedo);
            ur.Push(j); // clearing redo
            Assert.False(ur.CanRedo);
        }

        [Fact]
        public void UndoHistory_ContainsPushedStates()
        {
            var ur  = new BeepLayoutUndoRedo();
            string j = """{"schemaVersion":3}""";
            ur.Push(j);
            ur.Push(j);
            ur.Push(j);
            Assert.Equal(3, ur.UndoHistory.Count);
        }

        [Fact]
        public void MaxDepth_LimitsUndoStackSize()
        {
            var ur  = new BeepLayoutUndoRedo { MaxDepth = 3 };
            string j = """{"schemaVersion":3}""";
            for (int i = 0; i < 5; i++) ur.Push(j);
            Assert.Equal(3, ur.UndoHistory.Count);
        }

        [Fact]
        public void Clear_ResetsCanUndoAndCanRedo()
        {
            var ur  = new BeepLayoutUndoRedo();
            string j = """{"schemaVersion":3}""";
            ur.Push(j);
            ur.Clear();
            Assert.False(ur.CanUndo);
            Assert.False(ur.CanRedo);
        }
    }
}
