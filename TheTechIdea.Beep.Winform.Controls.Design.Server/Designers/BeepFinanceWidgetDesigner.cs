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

        [Category("Finance")]
        [Description("Subtitle of the finance widget")]
        public string Subtitle
        {
            get => _designer.GetProperty<string>("Subtitle") ?? string.Empty;
            set => _designer.SetProperty("Subtitle", value);
        }

        [Category("Finance")]
        [Description("Primary financial value")]
        public decimal PrimaryValue
        {
            get => _designer.GetProperty<decimal>("PrimaryValue");
            set => _designer.SetProperty("PrimaryValue", value);
        }

        [Category("Finance")]
        [Description("Percentage change or ratio")]
        public decimal Percentage
        {
            get => _designer.GetProperty<decimal>("Percentage");
            set => _designer.SetProperty("Percentage", value);
        }

        [Category("Finance")]
        [Description("Currency code")]
        public string Currency
        {
            get => _designer.GetProperty<string>("Currency") ?? string.Empty;
            set => _designer.SetProperty("Currency", value);
        }

        [Category("Finance")]
        [Description("Whether to show the currency symbol")]
        public bool ShowCurrency
        {
            get => _designer.GetProperty<bool>("ShowCurrency");
            set => _designer.SetProperty("ShowCurrency", value);
        }

        [Category("Finance")]
        [Description("Whether to show percentage values")]
        public bool ShowPercentage
        {
            get => _designer.GetProperty<bool>("ShowPercentage");
            set => _designer.SetProperty("ShowPercentage", value);
        }

        [Category("Finance")]
        [Description("Whether to show trend indicators")]
        public bool ShowTrend
        {
            get => _designer.GetProperty<bool>("ShowTrend");
            set => _designer.SetProperty("ShowTrend", value);
        }

        public void ConfigureAsTransactionList()
        {
            Style = FinanceWidgetStyle.TransactionCard;
            ShowCurrency = true;
            ShowPercentage = false;
            ShowTrend = false;
        }

        public void ConfigureAsPortfolioCard()
        {
            Style = FinanceWidgetStyle.PortfolioCard;
            ShowCurrency = true;
            ShowPercentage = true;
            ShowTrend = true;
        }

        public void ConfigureAsBalanceCard()
        {
            Style = FinanceWidgetStyle.BalanceCard;
            ShowCurrency = true;
            ShowPercentage = false;
            ShowTrend = false;
        }

        public void ConfigureAsBudgetWidget()
        {
            Style = FinanceWidgetStyle.BudgetWidget;
            ShowCurrency = true;
            ShowPercentage = true;
            ShowTrend = true;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsTransactionList", "Transaction List", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsPortfolioCard", "Portfolio Card", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsBalanceCard", "Balance Card", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsBudgetWidget", "Budget Widget", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("Subtitle", "Subtitle", "Properties"));
            items.Add(new DesignerActionPropertyItem("PrimaryValue", "Primary Value", "Properties"));
            items.Add(new DesignerActionPropertyItem("Percentage", "Percentage", "Properties"));
            items.Add(new DesignerActionPropertyItem("Currency", "Currency", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowCurrency", "Show Currency", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowPercentage", "Show Percentage", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowTrend", "Show Trend", "Properties"));
            return items;
        }
    }
}
