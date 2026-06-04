using System;
using System.ComponentModel.Design;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class BeepBlockDesignerTests
    {
        [Fact]
        public void BeepBlock_ActionLists_ExposeBeepBlockActionList()
        {
            using var surface = new DesignSurface();
            var host = (IDesignerHost)surface.GetService(typeof(IDesignerHost));
            using var transaction = host.CreateTransaction();
            var block = (BeepBlock)host.CreateComponent(typeof(BeepBlock), "beepBlock1");
            var designer = (BeepBlockDesigner)host.GetDesigner(block);
            Assert.NotNull(designer);

            var lists = designer.ActionLists;
            Assert.NotNull(lists);
            Assert.Contains(lists, item => item is BeepBlockActionList);

            var actionList = (BeepBlockActionList)lists[0];
            var items = actionList.GetSortedActionItems();
            Assert.NotEmpty(items);

            // The new "Host" group should be present in the smart-tag surface.
            var categories = items.OfType<DesignerActionHeaderItem>().Select(h => h.DisplayName).ToHashSet(StringComparer.Ordinal);
            Assert.Contains("Host", categories);
            transaction.Commit();
        }
    }
}
