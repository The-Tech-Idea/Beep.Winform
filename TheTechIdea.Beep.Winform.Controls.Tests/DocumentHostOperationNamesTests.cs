using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class DocumentHostOperationNamesTests
    {
        [Fact]
        public void OperationNames_AllDeclaredConstants_AreAcceptedByGuard()
        {
            foreach (string op in DocumentHostOperationNames.GetAll())
            {
                Exception? ex = Record.Exception(() => DocumentHostOperationNames.EnsureKnown(op));
                Assert.Null(ex);
            }
        }

        [Fact]
        public void OperationNames_AllDeclaredConstants_HaveUniqueValues()
        {
            var all = DocumentHostOperationNames.GetAll();
            Assert.NotEmpty(all);

            var uniqueValues = new HashSet<string>(all, StringComparer.Ordinal);
            Assert.Equal(all.Count, uniqueValues.Count);
        }

        [Fact]
        public void OperationNames_GetAll_ReturnsDeterministicSortedNonEmptyNames()
        {
            var all = DocumentHostOperationNames.GetAll();
            Assert.NotEmpty(all);

            string previous = all[0];
            Assert.False(string.IsNullOrWhiteSpace(previous));

            for (int i = 1; i < all.Count; i++)
            {
                string current = all[i];
                Assert.False(string.IsNullOrWhiteSpace(current));
                Assert.True(string.CompareOrdinal(previous, current) < 0);
                previous = current;
            }
        }

        [Fact]
        public void OperationNames_GetAll_IsReadOnlyCollection()
        {
            var all = DocumentHostOperationNames.GetAll();
            var list = Assert.IsAssignableFrom<IList<string>>(all);
            Assert.True(list.IsReadOnly);
            Assert.Throws<NotSupportedException>(() => list.Add("new-op"));
        }

        [Fact]
        public void OperationNames_EnsureKnown_Whitespace_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => DocumentHostOperationNames.EnsureKnown(" "));
            Assert.Throws<ArgumentException>(() => DocumentHostOperationNames.EnsureKnown(string.Empty));
        }

        [Fact]
        public void OperationNames_EnsureKnown_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DocumentHostOperationNames.EnsureKnown(null!));
        }
    }
}
