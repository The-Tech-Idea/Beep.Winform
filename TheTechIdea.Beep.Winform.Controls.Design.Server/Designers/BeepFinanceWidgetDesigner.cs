using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepFinanceWidgetDesigner : BaseWidgetDesigner
    {
        public BeepFinanceWidget? FinanceWidget => Component as BeepFinanceWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepFinanceWidgetActionList(this));
            return lists;
        }
    }

    public class BeepFinanceWidgetActionList : DesignerActionList
    {
        private readonly BeepFinanceWidgetDesigner _designer;

        public BeepFinanceWidgetActionList(BeepFinanceWidgetDesigner designer) : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Finance")]
        [Description("Visual style of the finance widget")]
        public FinanceWidgetStyle Style
        {
            get => _designer.GetProperty<FinanceWidgetStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Finance")]
        [Description("Title of the finance widget")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? string.Empty;
            set => _designer.SetProperty("Title", value);
        }

        public void ConfigureAsTransactionList() { Style = FinanceWidgetStyle.TransactionCard; }
        public void ConfigureAsPortfolioCard() { Style = FinanceWidgetStyle.PortfolioCard; }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsTransactionList", "Transaction List", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsPortfolioCard", "Portfolio Card", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            return items;
        }
    }
}
