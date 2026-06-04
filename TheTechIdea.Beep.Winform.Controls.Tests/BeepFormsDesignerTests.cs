using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class BeepFormsDesignerTests
    {
        [Fact]
        public void BeepForms_DerivesFromPanel_SupportsTransparentBackground()
        {
            Assert.True(typeof(Panel).IsAssignableFrom(typeof(BeepForms)),
                "BeepForms must derive from Panel so the WinForms out-of-process designer accepts transparency.");
        }

        [Fact]
        public void BeepForms_CanInstantiateWithoutCrash()
        {
            using var surface = new DesignSurface();
            var host = (IDesignerHost)surface.GetService(typeof(IDesignerHost));
            using var transaction = host.CreateTransaction();
            var forms = (BeepForms)host.CreateComponent(typeof(BeepForms), "beepForms1");
            Assert.NotNull(forms);
            Assert.NotNull(forms.Site);
            transaction.Commit();
        }

        [Fact]
        public void BeepForms_ActionLists_ExposeFormsHostActionList()
        {
            using var surface = new DesignSurface();
            var host = (IDesignerHost)surface.GetService(typeof(IDesignerHost));
            using var transaction = host.CreateTransaction();
            var forms = (BeepForms)host.CreateComponent(typeof(BeepForms), "beepForms1");
            var designer = (BeepFormsHostDesigner)host.GetDesigner(forms);
            Assert.NotNull(designer);

            var lists = designer.ActionLists;
            Assert.NotNull(lists);
            Assert.Contains(lists, item => item is BeepFormsHostActionList);

            var formsList = (BeepFormsHostActionList)lists[0];
            var items = formsList.GetSortedActionItems();
            Assert.NotEmpty(items);
            transaction.Commit();
        }

        [Fact]
        public void BeepForms_DataConnectionProperty_IsBrowsable()
        {
            var property = TypeDescriptor.GetProperties(typeof(BeepForms))["DataConnection"];
            Assert.NotNull(property);
            Assert.True(property.IsBrowsable, "BeepForms.DataConnection should be browsable in the property grid.");
            Assert.NotNull(property.Converter);
        }
    }
}
