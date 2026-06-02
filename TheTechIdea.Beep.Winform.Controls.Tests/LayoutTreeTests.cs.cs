// LayoutTreeTests.cs
// Unit tests for LayoutNodeExtensions, LayoutTreeValidator, LayoutTreeRepairer,
// and SplitLayoutNode/GroupLayoutNode.
// ?????????????????????????????????????????????????????????????????????????????
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class LayoutTreeTests
    {
        // ?? GroupLayoutNode construction ??????????????????????????????????????

        [Fact]
        public void GroupLayoutNode_NodeIdMatchesConstructorArg()
        {
            var leaf = new GroupLayoutNode("g1");
            Assert.Equal("g1", leaf.NodeId);
        }

        [Fact]
        public void GroupLayoutNode_DefaultConstructor_AssignsNonEmptyNodeId()
        {
            var leaf = new GroupLayoutNode();
            Assert.False(string.IsNullOrWhiteSpace(leaf.NodeId));
        }

        [Fact]
        public void GroupLayoutNode_EmptyStringId_NodeIdIsEmpty()
        {
            var leaf = new GroupLayoutNode(string.Empty);
            Assert.Equal(string.Empty, leaf.NodeId);
        }

        [Fact]
        public void GroupLayoutNode_DocumentIds_InitiallyEmpty()
        {
            var leaf = new GroupLayoutNode("g");
            Assert.Empty(leaf.DocumentIds);
        }

        [Fact]
        public void GroupLayoutNode_IsEmpty_TrueWhenNoDocuments()
        {
            var leaf = new GroupLayoutNode("g");
            Assert.True(leaf.IsEmpty);
        }

        [Fact]
        public void GroupLayoutNode_IsEmpty_FalseAfterAddingDocument()
        {
            var leaf = new GroupLayoutNode("g");
            leaf.DocumentIds.Add("doc1");
            Assert.False(leaf.IsEmpty);
        }

        // ?? SplitLayoutNode construction ??????????????????????????????????????

        [Fact]
        public void SplitLayoutNode_ChildrenAssignedCorrectly()
        {
            var left  = new GroupLayoutNode("a");
            var right = new GroupLayoutNode("b");
            var split = new SplitLayoutNode(left, right);
            Assert.Same(left,  split.First);
            Assert.Same(right, split.Second);
        }

        [Fact]
        public void SplitLayoutNode_DefaultRatioIsHalf()
        {
            var split = new SplitLayoutNode(
                new GroupLayoutNode("a"),
                new GroupLayoutNode("b"));
            Assert.Equal(0.5f, split.Ratio, precision: 2);
        }

        [Fact]
        public void SplitLayoutNode_RatioBelowMinIsClamped()
        {
            var split = new SplitLayoutNode(
                new GroupLayoutNode("a"),
                new GroupLayoutNode("b"),
                ratio: 0.05f);
            Assert.Equal(0.1f, split.Ratio, precision: 2);
        }

        [Fact]
        public void SplitLayoutNode_RatioAboveMaxIsClamped()
        {
            var split = new SplitLayoutNode(
                new GroupLayoutNode("a"),
                new GroupLayoutNode("b"),
                ratio: 1.5f);
            Assert.Equal(0.9f, split.Ratio, precision: 2);
        }

        [Fact]
        public void SplitLayoutNode_NullFirstChild_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SplitLayoutNode(null!, new GroupLayoutNode("b")));
        }

        [Fact]
        public void SplitLayoutNode_NullSecondChild_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SplitLayoutNode(new GroupLayoutNode("a"), null!));
        }

        // ?? LayoutNodeExtensions — AllNodes ???????????????????????????????????

        [Fact]
        public void AllNodes_OnLeaf_ReturnsSingleNode()
        {
            var leaf = new GroupLayoutNode("g1");
            var all  = leaf.AllNodes().ToList();
            Assert.Single(all);
        }

        [Fact]
        public void AllNodes_OnSplit_ReturnsBothChildrenAndSplit()
        {
            var left  = new GroupLayoutNode("a");
            var right = new GroupLayoutNode("b");
            var split = new SplitLayoutNode(left, right);
            var all   = split.AllNodes().ToList();
            Assert.Equal(3, all.Count);
        }

        [Fact]
        public void AllLeaves_ReturnsOnlyGroupNodes()
        {
            var left  = new GroupLayoutNode("a");
            var right = new GroupLayoutNode("b");
            var split = new SplitLayoutNode(left, right);
            var leaves = split.AllLeaves().ToList();
            Assert.Equal(2, leaves.Count);
            Assert.All(leaves, l => Assert.IsType<GroupLayoutNode>(l));
        }

        [Fact]
        public void AllLeaves_OnLeaf_ReturnsSelf()
        {
            var leaf   = new GroupLayoutNode("g");
            var leaves = leaf.AllLeaves().ToList();
            Assert.Single(leaves);
        }

        // ?? LayoutNodeExtensions — Depth ??????????????????????????????????????

        [Fact]
        public void Depth_OnLeaf_IsOne()
        {
            var leaf = new GroupLayoutNode("g");
            Assert.Equal(1, leaf.Depth());
        }

        [Fact]
        public void Depth_OnOneLevelSplit_IsTwo()
        {
            var split = new SplitLayoutNode(
                new GroupLayoutNode("a"),
                new GroupLayoutNode("b"));
            Assert.Equal(2, split.Depth());
        }

        [Fact]
        public void Depth_NestedSplit_IsThree()
        {
            var inner = new SplitLayoutNode(
                new GroupLayoutNode("c"),
                new GroupLayoutNode("d"));
            var outer = new SplitLayoutNode(
                new GroupLayoutNode("a"),
                inner);
            Assert.Equal(3, outer.Depth());
        }

        // ?? LayoutNodeExtensions — FindNode ???????????????????????????????????

        [Fact]
        public void FindNode_ReturnsCorrectLeaf()
        {
            var target = new GroupLayoutNode("target");
            var split  = new SplitLayoutNode(target, new GroupLayoutNode("other"));
            var found  = split.FindNode(target.NodeId);
            Assert.Same(target, found);
        }

        [Fact]
        public void FindNode_ReturnsNullWhenNotFound()
        {
            var leaf  = new GroupLayoutNode("g");
            var found = leaf.FindNode("missing");
            Assert.Null(found);
        }

        [Fact]
        public void FindNode_FindsSplitNode()
        {
            var left  = new GroupLayoutNode("a");
            var right = new GroupLayoutNode("b");
            var split = new SplitLayoutNode(left, right);
            var found = split.FindNode(split.NodeId);
            Assert.Same(split, found);
        }

        // ?? LayoutNodeExtensions — DeepClone ?????????????????????????????????

        [Fact]
        public void DeepClone_LeafProducesDistinctObject()
        {
            var leaf  = new GroupLayoutNode("g");
            var clone = leaf.DeepClone();
            Assert.NotSame(leaf, clone);
        }

        [Fact]
        public void DeepClone_SplitProducesDistinctTree()
        {
            var split = new SplitLayoutNode(
                new GroupLayoutNode("a"),
                new GroupLayoutNode("b"));
            var clone = split.DeepClone();
            Assert.NotSame(split, clone);
            var cloneSplit = Assert.IsType<SplitLayoutNode>(clone);
            Assert.NotSame(split.First,  cloneSplit.First);
            Assert.NotSame(split.Second, cloneSplit.Second);
        }

        // ?? LayoutTreeValidator ???????????????????????????????????????????????

        [Fact]
        public void ValidateNullRoot_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => LayoutTreeValidator.Validate(null!));
        }

        [Fact]
        public void ValidateSingleLeaf_IsValid()
        {
            var leaf   = new GroupLayoutNode("g1");
            var report = LayoutTreeValidator.Validate(leaf);
            Assert.True(report.IsValid);
        }

        [Fact]
        public void ValidateValidSplit_IsValid()
        {
            var split = new SplitLayoutNode(
                new GroupLayoutNode("a"),
                new GroupLayoutNode("b"),
                ratio: 0.5f);
            var report = LayoutTreeValidator.Validate(split);
            Assert.True(report.IsValid);
        }

        [Fact]
        public void ValidateDuplicateNodeIds_ReportsError()
        {
            var dupId = "dup";
            var left  = new GroupLayoutNode(dupId);
            var right = new GroupLayoutNode(dupId);
            var split = new SplitLayoutNode(left, right);
            // split.NodeId is a fresh GUID; only the two leaves share the same id
            var report = LayoutTreeValidator.Validate(split);
            Assert.False(report.IsValid);
        }

        [Fact]
        public void ValidateEmptyNodeId_ReportsError()
        {
            var leaf   = new GroupLayoutNode(string.Empty);
            var report = LayoutTreeValidator.Validate(leaf);
            Assert.False(report.IsValid);
        }

        [Fact]
        public void ValidateReport_IsValidReturnsTrue_WhenNoErrors()
        {
            var report = LayoutTreeValidator.Validate(new GroupLayoutNode("ok"));
            Assert.Equal(0, report.Errors.Count);
        }

        [Fact]
        public void ValidateReport_ToStringContainsValid_WhenClean()
        {
            var report = LayoutTreeValidator.Validate(new GroupLayoutNode("ok"));
            Assert.Contains("Valid", report.ToString());
        }

        // ?? LayoutTreeRepairer ????????????????????????????????????????????????

        [Fact]
        public void RepairNullRoot_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => LayoutTreeRepairer.Repair(null!));
        }

        [Fact]
        public void RepairValidTree_ReturnsSameStructure()
        {
            var leaf           = new GroupLayoutNode("g");
            var (result, rep)  = LayoutTreeRepairer.Repair(leaf);
            Assert.Equal("g", result.NodeId);
        }

        [Fact]
        public void RepairEmptyNodeId_AssignsNewId()
        {
            var leaf          = new GroupLayoutNode(string.Empty);
            var (result, rep) = LayoutTreeRepairer.Repair(leaf);
            Assert.False(string.IsNullOrEmpty(result.NodeId));
        }

        [Fact]
        public void RepairDuplicateDocIds_RemovesDuplicate()
        {
            var left  = new GroupLayoutNode("a");
            left.DocumentIds.AddRange(new[] { "doc1", "doc2" });
            var right = new GroupLayoutNode("b");
            right.DocumentIds.AddRange(new[] { "doc2", "doc3" }); // doc2 is dup
            var split = new SplitLayoutNode(left, right);
            var (result, _) = LayoutTreeRepairer.Repair(split);
            var allDocs = result.AllLeaves()
                .SelectMany(l => l.DocumentIds)
                .ToList();
            Assert.Equal(allDocs.Count, allDocs.Distinct().Count());
        }

        [Fact]
        public void RepairReport_DescribesNoActionsOnValidTree()
        {
            var leaf          = new GroupLayoutNode("g");
            var (_, report)   = LayoutTreeRepairer.Repair(leaf);
            Assert.False(report.WasRepaired);
        }

        [Fact]
        public void RepairReport_ReflectsRepairOnInvalidTree()
        {
            var leaf          = new GroupLayoutNode(string.Empty);
            var (_, report)   = LayoutTreeRepairer.Repair(leaf);
            Assert.True(report.WasRepaired);
        }

        // ?? SplitLayoutNode — ComputeChildBounds ??????????????????????????????

        [Fact]
        public void ComputeChildBoundsHorizontal_LeftPlusRightEqualsWidth()
        {
            var split = new SplitLayoutNode(
                new GroupLayoutNode("a"),
                new GroupLayoutNode("b"),
                Orientation.Horizontal, 0.5f)
            {
                Bounds = new Rectangle(0, 0, 800, 600)
            };
            var (fb, sb, secb) = split.ComputeChildBounds(splitterThickness: 4);
            Assert.Equal(800, fb.Width + sb.Width + secb.Width);
        }

        [Fact]
        public void ComputeChildBoundsVertical_TopPlusBottomEqualsHeight()
        {
            var split = new SplitLayoutNode(
                new GroupLayoutNode("a"),
                new GroupLayoutNode("b"),
                Orientation.Vertical, 0.5f)
            {
                Bounds = new Rectangle(0, 0, 800, 600)
            };
            var (fb, sb, secb) = split.ComputeChildBounds(splitterThickness: 4);
            Assert.Equal(600, fb.Height + sb.Height + secb.Height);
        }

        [Fact]
        public void ComputeChildBounds_SplitterBoundsWidthEqualsThickness_WhenHorizontal()
        {
            var split = new SplitLayoutNode(
                new GroupLayoutNode("a"),
                new GroupLayoutNode("b"),
                Orientation.Horizontal, 0.5f)
            {
                Bounds = new Rectangle(0, 0, 800, 600)
            };
            var (_, splitterBounds, _) = split.ComputeChildBounds(splitterThickness: 5);
            Assert.Equal(5, splitterBounds.Width);
        }
    }
}
