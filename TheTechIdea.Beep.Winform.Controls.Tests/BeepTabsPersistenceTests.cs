using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class BeepTabsPersistenceTests
    {
        [Fact]
        public void InitializeComponentStyleControlTree_RehydratesPagesChildrenAndMetadata()
        {
            using var form = new Form();
            using var tabs = new BeepTabs
            {
                Name = "beepTabs1",
                Size = new System.Drawing.Size(400, 240)
            };

            var firstChild = new Button { Name = "buttonOnFirstPage", Text = "First" };
            var secondChild = new Label { Name = "labelOnSecondPage", Text = "Second" };

            var firstPage = new BeepTabPage
            {
                Name = "beepTabPage1",
                Text = "General",
                TabIconPath = "icons/general.svg",
                TabSubText = "Profile",
                TabBadgeText = "3",
                TabBadgeKind = BeepTabBadgeKind.Count,
                TabCanClose = false,
                TabIsPinned = true,
                TabGroupKey = "workspace"
            };

            var secondPage = new BeepTabPage
            {
                Name = "beepTabPage2",
                Text = "Details",
                TabIsDirty = true,
                TabReusePreviewSlot = false
            };

            firstPage.Controls.Add(firstChild);
            secondPage.Controls.Add(secondChild);

            tabs.Controls.Add(firstPage);
            tabs.Controls.Add(secondPage);
            form.Controls.Add(tabs);

            Assert.Equal(2, tabs.TabCount);
            Assert.Same(firstPage, tabs.GetPageAt(0));
            Assert.Same(secondPage, tabs.GetPageAt(1));
            Assert.Same(firstPage, tabs.SelectedPage);
            Assert.Equal("General", tabs.GetPageAt(0)?.Text);
            Assert.Equal("Details", tabs.GetPageAt(1)?.Text);

            Assert.Same(firstChild, firstPage.Controls["buttonOnFirstPage"]);
            Assert.Same(secondChild, secondPage.Controls["labelOnSecondPage"]);

            Assert.Equal("icons/general.svg", firstPage.TabIconPath);
            Assert.Equal("Profile", firstPage.TabSubText);
            Assert.Equal("3", firstPage.TabBadgeText);
            Assert.Equal(BeepTabBadgeKind.Count, firstPage.TabBadgeKind);
            Assert.False(firstPage.TabCanClose);
            Assert.True(firstPage.TabIsPinned);
            Assert.Equal("workspace", firstPage.TabGroupKey);
            Assert.True(secondPage.TabIsDirty);
            Assert.False(secondPage.TabReusePreviewSlot);

            tabs.SelectedIndex = 1;

            Assert.Same(secondPage, tabs.SelectedPage);
            Assert.Same(secondChild, secondPage.Controls["labelOnSecondPage"]);
        }

        [Fact]
        public void ActivePageChildrenAndPages_CanBeRemovedThroughNormalControlCollections()
        {
            using var tabs = new BeepTabs { Name = "beepTabs1" };
            var child = new TextBox { Name = "textBoxOnPage" };
            var page = new BeepTabPage { Name = "beepTabPage1", Text = "Editable" };

            page.Controls.Add(child);
            tabs.Controls.Add(page);

            Assert.Equal(1, tabs.TabCount);
            Assert.Same(child, page.Controls["textBoxOnPage"]);

            page.Controls.Remove(child);
            tabs.RemovePage(page);

            Assert.Empty(page.Controls.Cast<Control>());
            Assert.Equal(0, tabs.TabCount);
            Assert.Null(tabs.SelectedPage);
        }

        [Fact]
        public void EmptyTabsControl_DoesNotCreateRuntimePagesByItself()
        {
            using var tabs = new BeepTabs { Name = "beepTabs1" };

            Assert.Equal(0, tabs.TabCount);
            Assert.Empty(tabs.Pages);
            Assert.Null(tabs.SelectedPage);
        }
    }
}
