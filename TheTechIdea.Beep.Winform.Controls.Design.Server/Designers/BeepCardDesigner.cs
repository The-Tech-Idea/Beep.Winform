using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepCard control
    /// Provides smart tags for card type presets and common configurations
    /// </summary>
    public class BeepCardDesigner : BaseBeepControlDesigner, IImagePathDesignerHost
    {
        private DesignerVerbCollection? _verbs;

        public BeepCard? Card => Component as BeepCard;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new ImagePathDesignerActionList(this));
            lists.Add(new BeepCardActionList(this));
            return lists;
        }

        public override DesignerVerbCollection Verbs
            => _verbs ??= new DesignerVerbCollection
            {
                new DesignerVerb("Select Image...", OnSelectImage),
                new DesignerVerb("Clear Image", OnClearImage)
            };

        private void OnSelectImage(object sender, EventArgs e)
            => SelectImage();

        private void OnClearImage(object sender, EventArgs e)
            => ClearImage();

        public void SelectImage()
        {
            if (Component == null)
            {
                return;
            }

            var serviceProvider = Component.Site ?? (IServiceProvider)GetService(typeof(IServiceProvider));

            using var dialog = new BeepImagePickerDialog(null, embed: false, serviceProvider, Component.GetType().Assembly);
            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                var newValue = dialog.SelectedResourcePath ?? dialog.SelectedFilePath;
                if (!string.IsNullOrEmpty(newValue))
                {
                    SetImagePath(newValue);
                }
            }
        }

        public void ClearImage()
            => SetImagePath(string.Empty);

        public void EmbedImage()
        {
            if (Component == null) return;

            var serviceProvider = Component.Site ?? (IServiceProvider)GetService(typeof(IServiceProvider));
            var currentPath = GetImagePath();

            using var dialog = new BeepImagePickerDialog(null, embed: true, serviceProvider, Component.GetType().Assembly, currentPath);
            var result = dialog.ShowDialog();

            if (result == DialogResult.OK && !dialog.SelectionResult.IsCancelled)
            {
                var newValue = dialog.SelectedResourcePath ?? dialog.SelectedFilePath;
                if (!string.IsNullOrEmpty(newValue))
                {
                    SetImagePath(newValue);
                }
            }
        }

        public string GetImagePath()
            => GetProperty<string>("ImagePath") ?? string.Empty;

        public void SetImagePath(string value)
            => SetProperty("ImagePath", value ?? string.Empty);
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
        [Description("Secondary text displayed by many card styles")]
        public string SubtitleText
        {
            get => _designer.GetProperty<string>("SubtitleText") ?? string.Empty;
            set => _designer.SetProperty("SubtitleText", value);
        }

        [Category("Card")]
        [Description("Text displayed on a secondary action button when the style supports it")]
        public string SecondaryButtonText
        {
            get => _designer.GetProperty<string>("SecondaryButtonText") ?? string.Empty;
            set => _designer.SetProperty("SecondaryButtonText", value);
        }

        [Category("Card")]
        [Description("Whether to show the secondary action button")]
        public bool ShowSecondaryButton
        {
            get => _designer.GetProperty<bool>("ShowSecondaryButton");
            set => _designer.SetProperty("ShowSecondaryButton", value);
        }

        [Category("Card")]
        [Description("Primary badge text rendered by styles that support badge accents")]
        public string BadgeText1
        {
            get => _designer.GetProperty<string>("BadgeText1") ?? string.Empty;
            set => _designer.SetProperty("BadgeText1", value);
        }

        [Category("Card")]
        [Description("Status or trend text rendered by styles that support status accents")]
        public string StatusText
        {
            get => _designer.GetProperty<string>("StatusText") ?? string.Empty;
            set => _designer.SetProperty("StatusText", value);
        }

        [Category("Card")]
        [Description("Whether to show the style-specific status accent")]
        public bool ShowStatus
        {
            get => _designer.GetProperty<bool>("ShowStatus");
            set => _designer.SetProperty("ShowStatus", value);
        }

        [Category("Card")]
        [Description("Rating value used by styles that render stars or nested depth")]
        public int Rating
        {
            get => _designer.GetProperty<int>("Rating");
            set => _designer.SetProperty("Rating", value);
        }

        [Category("Card")]
        [Description("Whether to render the rating accent")]
        public bool ShowRating
        {
            get => _designer.GetProperty<bool>("ShowRating");
            set => _designer.SetProperty("ShowRating", value);
        }

        [Category("Card")]
        [Description("Accent color used for card highlights and emphasis")]
        public System.Drawing.Color AccentColor
        {
            get => _designer.GetProperty<System.Drawing.Color>("AccentColor");
            set => _designer.SetProperty("AccentColor", value);
        }

        #endregion

        #region Quick Configuration Actions

        private void ResetExtendedCardFields()
        {
            SubtitleText = string.Empty;
            SecondaryButtonText = string.Empty;
            ShowSecondaryButton = false;
            BadgeText1 = string.Empty;
            StatusText = string.Empty;
            ShowStatus = false;
            Rating = 0;
            ShowRating = false;
            ButtonText = string.Empty;
            ShowButton = false;
        }

        private void ApplyCardPreset(CardStyle style, Action configure)
        {
            CardStyle = style;
            ResetExtendedCardFields();
            configure();
        }

        /// <summary>
        /// Configure card as a product card with image, title, price, and button
        /// </summary>
        public void ConfigureAsProductCard()
        {
            ApplyCardPreset(CardStyle.ProductCard, () =>
            {
                HeaderText = "Wireless Headphones Pro";
                ParagraphText = "Premium noise-cancelling headphones with 40hr battery life";
                SubtitleText = "$299.99";
                ButtonText = "Add to Cart";
                ShowButton = true;
                ShowRating = true;
                Rating = 5;
                BadgeText1 = "-20%";
            });
        }

        /// <summary>
        /// Configure card as a feature card with icon, title, and description
        /// </summary>
        public void ConfigureAsFeatureCard()
        {
            ApplyCardPreset(CardStyle.FeatureCard, () =>
            {
                HeaderText = "Advanced Analytics";
                ParagraphText = "Get deep insights into your data with real-time dashboards and custom reports.";
                ShowButton = false;
            });
        }

        /// <summary>
        /// Configure card as a metric card with number, label, and trend indicator
        /// </summary>
        public void ConfigureAsMetricCard()
        {
            ApplyCardPreset(CardStyle.MetricCard, () =>
            {
                HeaderText = "Conversion Rate";
                ParagraphText = "Target: 4.0%";
                SubtitleText = "3.8%";
                StatusText = "+0.5% vs last week";
                ShowStatus = true;
                BadgeText1 = "Growth";
                ShowButton = false;
            });
        }

        /// <summary>
        /// Configure card as a testimonial card with avatar, quote, and author
        /// </summary>
        public void ConfigureAsTestimonialCard()
        {
            ApplyCardPreset(CardStyle.TestimonialCard, () =>
            {
                HeaderText = "Emma Wilson";
                ParagraphText = "\"This product completely transformed our workflow. The team is more productive than ever, and our clients are thrilled with the results!\"";
                SubtitleText = "CEO, TechVision Inc.";
                ShowRating = true;
                Rating = 5;
                ShowButton = false;
            });
        }

        /// <summary>
        /// Configure card as a profile card with avatar, name, and bio
        /// </summary>
        public void ConfigureAsProfileCard()
        {
            ApplyCardPreset(CardStyle.ProfileCard, () =>
            {
                HeaderText = "Alex Morgan";
                ParagraphText = "Senior Full Stack Developer | Cloud Architecture Specialist\nPassionate about building scalable solutions";
                SubtitleText = "@alexmorgan";
                StatusText = "Available for work";
                ShowStatus = true;
                ButtonText = "Follow";
                SecondaryButtonText = "Message";
                ShowButton = true;
                ShowSecondaryButton = true;
                BadgeText1 = "Pro";
            });
        }

        /// <summary>
        /// Configure card as a stat card for dashboard KPIs
        /// </summary>
        public void ConfigureAsStatCard()
        {
            ApplyCardPreset(CardStyle.StatCard, () =>
            {
                HeaderText = "12,458";
                ParagraphText = "Active Users";
                StatusText = "+18.2% from last month";
                ShowStatus = true;
                ShowButton = false;
            });
        }

        /// <summary>
        /// Configure card as a pricing card with features list
        /// </summary>
        public void ConfigureAsPricingCard()
        {
            ApplyCardPreset(CardStyle.PricingCard, () =>
            {
                HeaderText = "Professional Plan";
                ParagraphText = "Perfect for growing teams\n• Unlimited projects\n• Advanced features\n• Priority support\n• Custom integrations";
                SubtitleText = "$49/month";
                StatusText = "Billed monthly";
                ButtonText = "Choose Plan";
                ShowButton = true;
                BadgeText1 = "Most Popular";
            });
        }

        /// <summary>
        /// Configure card as a basic card with title and content
        /// </summary>
        public void ConfigureAsBasicCard()
        {
            ApplyCardPreset(CardStyle.BasicCard, () =>
            {
                HeaderText = "Card Title";
                ParagraphText = "Card content goes here...";
                ShowButton = false;
            });
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
            items.Add(new DesignerActionPropertyItem("SubtitleText", "Subtitle Text", "Card Properties"));
            items.Add(new DesignerActionPropertyItem("ButtonText", "Button Text", "Card Properties"));
            items.Add(new DesignerActionPropertyItem("ShowButton", "Show Button", "Card Properties"));
            items.Add(new DesignerActionPropertyItem("AccentColor", "Accent Color", "Card Properties"));

            items.Add(new DesignerActionHeaderItem("Card Accents"));
            items.Add(new DesignerActionPropertyItem("SecondaryButtonText", "Secondary Button Text", "Card Accents"));
            items.Add(new DesignerActionPropertyItem("ShowSecondaryButton", "Show Secondary Button", "Card Accents"));
            items.Add(new DesignerActionPropertyItem("BadgeText1", "Badge Text", "Card Accents"));
            items.Add(new DesignerActionPropertyItem("StatusText", "Status Text", "Card Accents"));
            items.Add(new DesignerActionPropertyItem("ShowStatus", "Show Status", "Card Accents"));
            items.Add(new DesignerActionPropertyItem("Rating", "Rating", "Card Accents"));
            items.Add(new DesignerActionPropertyItem("ShowRating", "Show Rating", "Card Accents"));

            return items;
        }
    }
}
