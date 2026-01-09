using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepCard control
    /// Provides smart tags for card type presets and common configurations
    /// </summary>
    public class BeepCardDesigner : BaseBeepControlDesigner
    {
        public BeepCard? Card => Component as BeepCard;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepCardActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Action list for BeepCard smart tags
    /// Provides quick card type presets and common property access
    /// </summary>
    public class BeepCardActionList : DesignerActionList
    {
        private readonly BeepCardDesigner _designer;

        public BeepCardActionList(BeepCardDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepCard? Card => Component as BeepCard;

        #region Properties (for smart tags)

        [Category("Card")]
        [Description("The style/type of card")]
        public CardStyle CardStyle
        {
            get => _designer.GetProperty<CardStyle>("CardStyle");
            set => _designer.SetProperty("CardStyle", value);
        }

        [Category("Card")]
        [Description("Header text displayed on the card")]
        public string HeaderText
        {
            get => _designer.GetProperty<string>("HeaderText") ?? string.Empty;
            set => _designer.SetProperty("HeaderText", value);
        }

        [Category("Card")]
        [Description("Paragraph/description text displayed on the card")]
        public string ParagraphText
        {
            get => _designer.GetProperty<string>("ParagraphText") ?? string.Empty;
            set => _designer.SetProperty("ParagraphText", value);
        }

        [Category("Card")]
        [Description("Button text displayed on the card")]
        public string ButtonText
        {
            get => _designer.GetProperty<string>("ButtonText") ?? string.Empty;
            set => _designer.SetProperty("ButtonText", value);
        }

        [Category("Card")]
        [Description("Whether to show the primary button")]
        public bool ShowButton
        {
            get => _designer.GetProperty<bool>("ShowButton");
            set => _designer.SetProperty("ShowButton", value);
        }

        [Category("Card")]
        [Description("Image path for the card")]
        public string ImagePath
        {
            get => _designer.GetProperty<string>("ImagePath") ?? string.Empty;
            set => _designer.SetProperty("ImagePath", value);
        }

        #endregion

        #region Quick Configuration Actions

        /// <summary>
        /// Configure card as a product card with image, title, price, and button
        /// </summary>
        public void ConfigureAsProductCard()
        {
            CardStyle = CardStyle.ProductCard;
            HeaderText = "Product Name";
            ParagraphText = "Product description goes here...";
            ButtonText = "Add to Cart";
            ShowButton = true;
        }

        /// <summary>
        /// Configure card as a feature card with icon, title, and description
        /// </summary>
        public void ConfigureAsFeatureCard()
        {
            CardStyle = CardStyle.FeatureCard;
            HeaderText = "Feature Title";
            ParagraphText = "Feature description highlighting key benefits...";
            ShowButton = false;
        }

        /// <summary>
        /// Configure card as a metric card with number, label, and trend indicator
        /// </summary>
        public void ConfigureAsMetricCard()
        {
            CardStyle = CardStyle.MetricCard;
            HeaderText = "1,234";
            ParagraphText = "Total Users";
            ShowButton = false;
        }

        /// <summary>
        /// Configure card as a testimonial card with avatar, quote, and author
        /// </summary>
        public void ConfigureAsTestimonialCard()
        {
            CardStyle = CardStyle.TestimonialCard;
            HeaderText = "John Doe";
            ParagraphText = "\"This is an amazing product! Highly recommended.\"";
            ShowButton = false;
        }

        /// <summary>
        /// Configure card as a profile card with avatar, name, and bio
        /// </summary>
        public void ConfigureAsProfileCard()
        {
            CardStyle = CardStyle.ProfileCard;
            HeaderText = "User Name";
            ParagraphText = "User bio and description...";
            ButtonText = "Follow";
            ShowButton = true;
        }

        /// <summary>
        /// Configure card as a stat card for dashboard KPIs
        /// </summary>
        public void ConfigureAsStatCard()
        {
            CardStyle = CardStyle.StatCard;
            HeaderText = "1,234";
            ParagraphText = "Total Sales";
            ShowButton = false;
        }

        /// <summary>
        /// Configure card as a pricing card with features list
        /// </summary>
        public void ConfigureAsPricingCard()
        {
            CardStyle = CardStyle.PricingCard;
            HeaderText = "Premium Plan";
            ParagraphText = "$99/month\n• Feature 1\n• Feature 2\n• Feature 3";
            ButtonText = "Subscribe";
            ShowButton = true;
        }

        /// <summary>
        /// Configure card as a basic card with title and content
        /// </summary>
        public void ConfigureAsBasicCard()
        {
            CardStyle = CardStyle.BasicCard;
            HeaderText = "Card Title";
            ParagraphText = "Card content goes here...";
            ShowButton = false;
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Quick card type presets
            items.Add(new DesignerActionHeaderItem("Card Type Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsProductCard", "Product Card", "Card Type Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsFeatureCard", "Feature Card", "Card Type Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsMetricCard", "Metric Card", "Card Type Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsTestimonialCard", "Testimonial Card", "Card Type Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsProfileCard", "Profile Card", "Card Type Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsStatCard", "Stat Card", "Card Type Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsPricingCard", "Pricing Card", "Card Type Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsBasicCard", "Basic Card", "Card Type Presets", true));

            // Card properties
            items.Add(new DesignerActionHeaderItem("Card Properties"));
            items.Add(new DesignerActionPropertyItem("CardStyle", "Card Style", "Card Properties"));
            items.Add(new DesignerActionPropertyItem("HeaderText", "Header Text", "Card Properties"));
            items.Add(new DesignerActionPropertyItem("ParagraphText", "Paragraph Text", "Card Properties"));
            items.Add(new DesignerActionPropertyItem("ButtonText", "Button Text", "Card Properties"));
            items.Add(new DesignerActionPropertyItem("ShowButton", "Show Button", "Card Properties"));
            items.Add(new DesignerActionPropertyItem("ImagePath", "Image Path", "Card Properties"));

            return items;
        }
    }
}
