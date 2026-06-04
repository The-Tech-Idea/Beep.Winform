using System;
using System.ComponentModel.Design;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class BeepDataConnectionDesignerTests
    {
        [Fact]
        public void BeepDataConnection_ActionLists_ExposeBeepDataConnectionActionList()
        {
            using var surface = new DesignSurface();
            var host = (IDesignerHost)surface.GetService(typeof(IDesignerHost));
            using var transaction = host.CreateTransaction();
            var connection = (BeepDataConnection)host.CreateComponent(typeof(BeepDataConnection), "beepDataConnection1");
            var designer = (BeepDataConnectionDesigner)host.GetDesigner(connection);
            Assert.NotNull(designer);

            var lists = designer.ActionLists;
            Assert.NotNull(lists);
            Assert.Contains(lists, item => item is BeepDataConnectionActionList);

            var actionList = (BeepDataConnectionActionList)lists[0];
            var items = actionList.GetSortedActionItems();
            Assert.NotEmpty(items);
            transaction.Commit();
        }

        [Fact]
        public void BeepDataConnection_CanInstantiateWithoutCrash()
        {
            using var surface = new DesignSurface();
            var host = (IDesignerHost)surface.GetService(typeof(IDesignerHost));
            using var transaction = host.CreateTransaction();
            var connection = (BeepDataConnection)host.CreateComponent(typeof(BeepDataConnection), "beepDataConnection1");
            Assert.NotNull(connection);
            Assert.NotNull(connection.Site);
            transaction.Commit();
        }
    }
}
