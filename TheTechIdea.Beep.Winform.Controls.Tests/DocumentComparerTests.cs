// DocumentComparerTests.cs
// Unit tests for the Myers O(ND) diff algorithm in DocumentComparer.
// ?????????????????????????????????????????????????????????????????????????????
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Features;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class DocumentComparerTests
    {
        private static int UnchangedCount(DiffResult r)
            => r.Lines.Count(l => l.Kind == DiffLineKind.Unchanged);

        [Fact]
        public void WhenBothInputsEmpty_ResultHasNoLines()
        {
            var result = DocumentComparer.Compare(string.Empty, string.Empty);
            Assert.Empty(result.Lines);
        }

        [Fact]
        public void WhenBothInputsIdentical_AllLinesAreKept()
        {
            var result = DocumentComparer.Compare("a\nb\nc", "a\nb\nc");
            Assert.All(result.Lines, l => Assert.Equal(DiffLineKind.Unchanged, l.Kind));
        }

        [Fact]
        public void WhenBothInputsIdentical_StatsHaveZeroAddedAndRemoved()
        {
            var result = DocumentComparer.Compare("hello\nworld", "hello\nworld");
            Assert.Equal(0, result.Stats.LinesAdded);
            Assert.Equal(0, result.Stats.LinesRemoved);
        }

        [Fact]
        public void WhenBothInputsIdentical_UnchangedCountEqualsLineCount()
        {
            var result = DocumentComparer.Compare("a\nb\nc\nd", "a\nb\nc\nd");
            Assert.Equal(4, UnchangedCount(result));
        }

        [Fact]
        public void WhenLeftIsEmpty_AllLinesAreAdded()
        {
            var result = DocumentComparer.Compare(string.Empty, "x\ny");
            Assert.All(result.Lines, l => Assert.Equal(DiffLineKind.Added, l.Kind));
        }

        [Fact]
        public void WhenRightIsEmpty_AllLinesAreRemoved()
        {
            var result = DocumentComparer.Compare("x\ny", string.Empty);
            Assert.All(result.Lines, l => Assert.Equal(DiffLineKind.Removed, l.Kind));
        }

        [Fact]
        public void WhenLeftIsEmpty_StatsAddedEqualsRightLineCount()
        {
            var result = DocumentComparer.Compare(string.Empty, "a\nb\nc");
            Assert.Equal(3, result.Stats.LinesAdded);
            Assert.Equal(0, result.Stats.LinesRemoved);
        }

        [Fact]
        public void WhenRightIsEmpty_StatsRemovedEqualsLeftLineCount()
        {
            var result = DocumentComparer.Compare("a\nb\nc", string.Empty);
            Assert.Equal(0, result.Stats.LinesAdded);
            Assert.Equal(3, result.Stats.LinesRemoved);
        }

        [Fact]
        public void WhenOnlyFirstLineChanged_ResultContainsRemoveAndAdd()
        {
            var result = DocumentComparer.Compare("old\nb\nc", "new\nb\nc");
            Assert.Contains(result.Lines, l => l.Kind == DiffLineKind.Removed && l.Text == "old");
            Assert.Contains(result.Lines, l => l.Kind == DiffLineKind.Added   && l.Text == "new");
        }

        [Fact]
        public void WhenOnlyLastLineChanged_MiddleLinesAreUnchanged()
        {
            var result = DocumentComparer.Compare("a\nb\nold", "a\nb\nnew");
            var kept   = result.Lines.Where(l => l.Kind == DiffLineKind.Unchanged).ToList();
            Assert.Contains(kept, l => l.Text == "a");
            Assert.Contains(kept, l => l.Text == "b");
        }

        [Fact]
        public void WhenLineInsertedAtStart_InsertedLineShouldBeAdded()
        {
            var result = DocumentComparer.Compare("b\nc", "a\nb\nc");
            Assert.Equal(DiffLineKind.Added, result.Lines.First(l => l.Text == "a").Kind);
        }

        [Fact]
        public void WhenMultipleLinesInserted_AddedCountMatchesInsertionCount()
        {
            var result = DocumentComparer.Compare("a\nd", "a\nb\nc\nd");
            Assert.Equal(2, result.Stats.LinesAdded);
        }

        [Fact]
        public void WhenLineDeletedAtStart_FirstLineShouldBeRemoved()
        {
            var result = DocumentComparer.Compare("a\nb\nc", "b\nc");
            Assert.Equal(DiffLineKind.Removed, result.Lines.First(l => l.Text == "a").Kind);
        }

        [Fact]
        public void WhenMultipleLinesDeleted_RemovedCountMatchesDeletionCount()
        {
            var result = DocumentComparer.Compare("a\nb\nc\nd", "a\nd");
            Assert.Equal(2, result.Stats.LinesRemoved);
        }

        [Fact]
        public void WhenAllLinesDeleted_UnchangedCountIsZero()
        {
            var result = DocumentComparer.Compare("a\nb\nc", string.Empty);
            Assert.Equal(0, UnchangedCount(result));
        }

        [Fact]
        public void AddedLinesHaveNoLeftLineIndex()
        {
            var result = DocumentComparer.Compare(string.Empty, "x");
            Assert.All(result.Lines.Where(l => l.Kind == DiffLineKind.Added),
                       l => Assert.Equal(-1, l.LineA));
        }

        [Fact]
        public void RemovedLinesHaveNoRightLineIndex()
        {
            var result = DocumentComparer.Compare("x", string.Empty);
            Assert.All(result.Lines.Where(l => l.Kind == DiffLineKind.Removed),
                       l => Assert.Equal(-1, l.LineB));
        }

        [Fact]
        public void UnchangedLinesBothLineIndicesAreNonNegative()
        {
            var result = DocumentComparer.Compare("a\nb", "a\nb");
            Assert.All(result.Lines, l =>
            {
                Assert.True(l.LineA >= 0);
                Assert.True(l.LineB >= 0);
            });
        }

        [Fact]
        public void TotalLineCount_EqualsSumOfAllKinds()
        {
            var result = DocumentComparer.Compare("a\nb\nc", "a\nx\ny\nc");
            Assert.Equal(result.Lines.Count, result.Stats.LinesTotal);
        }

        [Fact]
        public void WhenCompleteReplace_NoUnchangedLines()
        {
            var result = DocumentComparer.Compare("a\nb", "c\nd");
            Assert.Equal(0, UnchangedCount(result));
            Assert.True(result.Stats.LinesAdded > 0);
            Assert.True(result.Stats.LinesRemoved > 0);
        }

        [Fact]
        public void IsIdentical_TrueWhenSameText()
        {
            var result = DocumentComparer.Compare("same", "same");
            Assert.True(result.IsIdentical);
        }

        [Fact]
        public void IsIdentical_FalseWhenTextDiffers()
        {
            var result = DocumentComparer.Compare("a", "b");
            Assert.False(result.IsIdentical);
        }

        [Fact]
        public void CaseSensitiveByDefault_DifferentCaseProducesDiff()
        {
            var result = DocumentComparer.Compare("Hello", "hello");
            Assert.False(result.IsIdentical);
        }

        [Fact]
        public void CaseInsensitiveComparison_TreatsAsEqual()
        {
            var result = DocumentComparer.Compare(
                "Hello", "hello",
                StringComparison.OrdinalIgnoreCase);
            Assert.True(result.IsIdentical);
        }

        [Fact]
        public void LargeIdenticalInputs_ProducesOnlyUnchangedLines()
        {
            string text = string.Join("\n", Enumerable.Range(1, 500).Select(i => $"line{i}"));
            var result  = DocumentComparer.Compare(text, text);
            Assert.Equal(0, result.Stats.LinesAdded + result.Stats.LinesRemoved);
        }

        [Fact]
        public void LargeInputWithOneChange_ExactlyOneAddAndOneRemove()
        {
            var linesA = Enumerable.Range(1, 200).Select(i => $"line{i}").ToList();
            var linesB = linesA.ToList();
            linesB[100] = "CHANGED";
            var result = DocumentComparer.Compare(
                string.Join("\n", linesA),
                string.Join("\n", linesB));
            Assert.Equal(1, result.Stats.LinesAdded);
            Assert.Equal(1, result.Stats.LinesRemoved);
        }

        [Fact]
        public void SingleLineFiles_WorkCorrectly()
        {
            var result = DocumentComparer.Compare("only", "only");
            Assert.Single(result.Lines);
            Assert.Equal(DiffLineKind.Unchanged, result.Lines[0].Kind);
        }

        [Fact]
        public void BlankLinesArePreserved()
        {
            var result = DocumentComparer.Compare("a\n\nb", "a\n\nb");
            Assert.Equal(3, UnchangedCount(result));
        }

        [Fact]
        public void LineOrderIsPreservedInOutput()
        {
            var result = DocumentComparer.Compare("a\nb\nc", "a\nb\nc");
            Assert.Equal("a", result.Lines[0].Text);
            Assert.Equal("b", result.Lines[1].Text);
            Assert.Equal("c", result.Lines[2].Text);
        }

        [Fact]
        public void Compare_ReturnsNonNullResult()
        {
            var result = DocumentComparer.Compare("x", "y");
            Assert.NotNull(result);
            Assert.NotNull(result.Lines);
            Assert.NotNull(result.Stats);
        }

        [Fact]
        public void DiffStats_LinesChangedIsMinOfAddedAndRemoved()
        {
            var result = DocumentComparer.Compare("a\nb\nc", "x\ny\nc");
            Assert.Equal(Math.Min(result.Stats.LinesAdded, result.Stats.LinesRemoved),
                         result.Stats.LinesChanged);
        }
    }
}
