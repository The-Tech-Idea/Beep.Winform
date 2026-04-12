// GitStatusParserTests.cs
// Unit tests for GitFileStatus, GitFileInfo, and BeepGitStatusProvider public API.
// ?????????????????????????????????????????????????????????????????????????????
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Features;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class GitStatusParserTests
    {
        // ?? GitFileStatus enum coverage ???????????????????????????????????????

        [Fact]
        public void GitFileStatus_ContainsUnknown()
            => Assert.Contains(GitFileStatus.Unknown, Enum.GetValues<GitFileStatus>());

        [Fact]
        public void GitFileStatus_ContainsModified()
            => Assert.Contains(GitFileStatus.Modified, Enum.GetValues<GitFileStatus>());

        [Fact]
        public void GitFileStatus_ContainsAdded()
            => Assert.Contains(GitFileStatus.Added, Enum.GetValues<GitFileStatus>());

        [Fact]
        public void GitFileStatus_ContainsDeleted()
            => Assert.Contains(GitFileStatus.Deleted, Enum.GetValues<GitFileStatus>());

        [Fact]
        public void GitFileStatus_ContainsRenamed()
            => Assert.Contains(GitFileStatus.Renamed, Enum.GetValues<GitFileStatus>());

        [Fact]
        public void GitFileStatus_ContainsUntracked()
            => Assert.Contains(GitFileStatus.Untracked, Enum.GetValues<GitFileStatus>());

        [Fact]
        public void GitFileStatus_ContainsConflicted()
            => Assert.Contains(GitFileStatus.Conflicted, Enum.GetValues<GitFileStatus>());

        [Fact]
        public void GitFileStatus_HasAtLeastSevenValues()
            => Assert.True(Enum.GetValues<GitFileStatus>().Length >= 7);

        // ?? GitFileInfo init properties ???????????????????????????????????????

        [Fact]
        public void GitFileInfo_FilePath_StoredCorrectly()
        {
            var info = new GitFileInfo { FilePath = "src/Foo.cs" };
            Assert.Equal("src/Foo.cs", info.FilePath);
        }

        [Fact]
        public void GitFileInfo_Status_StoredCorrectly()
        {
            var info = new GitFileInfo { Status = GitFileStatus.Modified };
            Assert.Equal(GitFileStatus.Modified, info.Status);
        }

        [Fact]
        public void GitFileInfo_BadgeText_StoredCorrectly()
        {
            var info = new GitFileInfo { BadgeText = "M" };
            Assert.Equal("M", info.BadgeText);
        }

        [Fact]
        public void GitFileInfo_BadgeColor_StoredCorrectly()
        {
            var info = new GitFileInfo { BadgeColor = Color.Orange };
            Assert.Equal(Color.Orange, info.BadgeColor);
        }

        [Fact]
        public void GitFileInfo_ToolTip_StoredCorrectly()
        {
            var info = new GitFileInfo { ToolTip = "Modified file" };
            Assert.Equal("Modified file", info.ToolTip);
        }

        [Fact]
        public void GitFileInfo_DefaultStatus_IsUnknown()
        {
            var info = new GitFileInfo();
            Assert.Equal(GitFileStatus.Unknown, info.Status);
        }

        [Fact]
        public void GitFileInfo_DefaultFilePath_IsEmpty()
        {
            var info = new GitFileInfo();
            Assert.Equal(string.Empty, info.FilePath);
        }

        [Fact]
        public void GitFileInfo_DefaultBadgeText_IsEmpty()
        {
            var info = new GitFileInfo();
            Assert.Equal(string.Empty, info.BadgeText);
        }

        [Fact]
        public void GitFileInfo_DefaultBadgeColor_IsGray()
        {
            var info = new GitFileInfo();
            Assert.Equal(Color.Gray, info.BadgeColor);
        }

        // ?? BeepGitStatusProvider public API ??????????????????????????????????

        [Fact]
        public void BeepGitStatusProvider_CanBeConstructed()
        {
            using var provider = new BeepGitStatusProvider();
            Assert.NotNull(provider);
        }

        [Fact]
        public void GetStatus_UnknownPath_ReturnsUnknownStatus()
        {
            using var provider = new BeepGitStatusProvider();
            var info           = provider.GetStatus("nonexistent/path/to/file.cs");
            Assert.Equal(GitFileStatus.Unknown, info.Status);
        }

        [Fact]
        public void GetStatus_EmptyPath_ReturnsUnknownStatus()
        {
            using var provider = new BeepGitStatusProvider();
            var info           = provider.GetStatus(string.Empty);
            Assert.Equal(GitFileStatus.Unknown, info.Status);
        }

        [Fact]
        public void GetStatus_NullPath_ReturnsUnknownStatus()
        {
            using var provider = new BeepGitStatusProvider();
            var info           = provider.GetStatus(null!);
            Assert.Equal(GitFileStatus.Unknown, info.Status);
        }

        [Fact]
        public void DefaultGitExecutable_IsGit()
        {
            using var provider = new BeepGitStatusProvider();
            Assert.Equal("git", provider.GitExecutable);
        }

        [Fact]
        public void DefaultRefreshIntervalMs_IsPositive()
        {
            using var provider = new BeepGitStatusProvider();
            Assert.True(provider.RefreshIntervalMs > 0);
        }

        [Fact]
        public void RepositoryRoot_DefaultIsNull()
        {
            using var provider = new BeepGitStatusProvider();
            Assert.Null(provider.RepositoryRoot);
        }

        [Fact]
        public void SetRepositoryRoot_RetainedCorrectly()
        {
            using var provider = new BeepGitStatusProvider();
            provider.RepositoryRoot = @"C:\MyRepo";
            Assert.Equal(@"C:\MyRepo", provider.RepositoryRoot);
        }

        [Fact]
        public void SetGitExecutable_RetainedCorrectly()
        {
            using var provider = new BeepGitStatusProvider();
            provider.GitExecutable = "git.exe";
            Assert.Equal("git.exe", provider.GitExecutable);
        }
    }
}
