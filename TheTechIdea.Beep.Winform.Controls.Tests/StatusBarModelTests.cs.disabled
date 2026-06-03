// StatusBarModelTests.cs
// Unit tests for StatusBarInfo data contract and BeepDocumentStatusBar helper logic.
// These tests cover the pure data model; no WinForms UI is exercised.
// ─────────────────────────────────────────────────────────────────────────────
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Features;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class StatusBarModelTests
    {
        // ── StatusBarInfo defaults ────────────────────────────────────────────

        [Fact]
        public void DefaultEncoding_IsUtf8()
        {
            var info = new StatusBarInfo();
            Assert.Equal("UTF-8", info.Encoding);
        }

        [Fact]
        public void DefaultLineEnding_IsCRLF()
        {
            var info = new StatusBarInfo();
            Assert.Equal("CRLF", info.LineEnding);
        }

        [Fact]
        public void DefaultLine_IsOne()
        {
            var info = new StatusBarInfo();
            Assert.Equal(1, info.Line);
        }

        [Fact]
        public void DefaultColumn_IsOne()
        {
            var info = new StatusBarInfo();
            Assert.Equal(1, info.Column);
        }

        [Fact]
        public void DefaultZoom_Is100()
        {
            var info = new StatusBarInfo();
            Assert.Equal(100, info.ZoomPercent);
        }

        [Fact]
        public void DefaultNotifications_IsZero()
        {
            var info = new StatusBarInfo();
            Assert.Equal(0, info.Notifications);
        }

        [Fact]
        public void DefaultGitBranch_IsNull()
        {
            var info = new StatusBarInfo();
            Assert.Null(info.GitBranch);
        }

        [Fact]
        public void DefaultDocumentType_IsEmpty()
        {
            var info = new StatusBarInfo();
            Assert.Equal(string.Empty, info.DocumentType);
        }

        [Fact]
        public void DefaultSelectionLen_IsZero()
        {
            var info = new StatusBarInfo();
            Assert.Equal(0, info.SelectionLen);
        }

        // ── StatusBarInfo mutation ────────────────────────────────────────────

        [Fact]
        public void SetLine_RetainedCorrectly()
        {
            var info = new StatusBarInfo { Line = 42 };
            Assert.Equal(42, info.Line);
        }

        [Fact]
        public void SetColumn_RetainedCorrectly()
        {
            var info = new StatusBarInfo { Column = 7 };
            Assert.Equal(7, info.Column);
        }

        [Fact]
        public void SetGitBranch_RetainedCorrectly()
        {
            var info = new StatusBarInfo { GitBranch = "main" };
            Assert.Equal("main", info.GitBranch);
        }

        [Fact]
        public void SetZoom_RetainedCorrectly()
        {
            var info = new StatusBarInfo { ZoomPercent = 150 };
            Assert.Equal(150, info.ZoomPercent);
        }

        [Fact]
        public void SetNotifications_RetainedCorrectly()
        {
            var info = new StatusBarInfo { Notifications = 3 };
            Assert.Equal(3, info.Notifications);
        }

        [Fact]
        public void SetEncoding_RetainedCorrectly()
        {
            var info = new StatusBarInfo { Encoding = "ASCII" };
            Assert.Equal("ASCII", info.Encoding);
        }

        [Fact]
        public void SetLineEnding_RetainedCorrectly()
        {
            var info = new StatusBarInfo { LineEnding = "LF" };
            Assert.Equal("LF", info.LineEnding);
        }

        [Fact]
        public void SetDocumentType_RetainedCorrectly()
        {
            var info = new StatusBarInfo { DocumentType = "C#" };
            Assert.Equal("C#", info.DocumentType);
        }

        [Fact]
        public void SetSelectionLen_RetainedCorrectly()
        {
            var info = new StatusBarInfo { SelectionLen = 25 };
            Assert.Equal(25, info.SelectionLen);
        }

        // ── StatusBarSegmentClickedEventArgs ──────────────────────────────────

        [Fact]
        public void EventArgs_StoresSegmentId()
        {
            var args = new StatusBarSegmentClickedEventArgs("encoding");
            Assert.Equal("encoding", args.SegmentId);
        }

        [Theory]
        [InlineData("left")]
        [InlineData("center")]
        [InlineData("right")]
        [InlineData("goto-line")]
        [InlineData("git-refresh")]
        public void EventArgs_AllKnownSegmentIds_CanBeCreated(string segId)
        {
            var args = new StatusBarSegmentClickedEventArgs(segId);
            Assert.Equal(segId, args.SegmentId);
        }
    }
}
