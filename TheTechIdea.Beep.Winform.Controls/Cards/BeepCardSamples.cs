using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Cards
{
    /// <summary>
    /// Sample implementations demonstrating the various BeepCard styles
    /// based on the provided reference images and the refactored painter architecture.
    /// </summary>
    public static class BeepCardSamples
    {
        /// <summary>
        /// Creates a profile card like "Corey Tawney" from the samples
        /// Uses ProfileCardPainter.cs
        /// </summary>
        public static BeepCard CreateProfileCard()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.ProfileCard,
                HeaderText = "Corey Tawney",
                StatusText = "Available for work",
                ShowStatus = true,
                StatusColor = Color.Green,
                BadgeText1 = "PRO",
                Badge1BackColor = Color.FromArgb(33, 150, 243), // Blue
                Badge1ForeColor = Color.White,
                ImagePath = "", // Set to actual image path
                ButtonText = "Portfolio",
                ShowButton = true,
                Size = new Size(300, 400),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a content card like the course cards from samples
        /// Uses ContentCardPainter.cs
        /// </summary>
        public static BeepCard CreateContentCard()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.ContentCard,
                HeaderText = "Film Coverage — A Step-By-Step Guide To Shot Listing Efficiently",
                ParagraphText = "by Cameron P. West • Apr 10, 2020",
                Tags = new List<string> { "Production", "Film" },
                BadgeText1 = "PREMIUM",
                Badge1BackColor = Color.FromArgb(255, 193, 7), // Amber
                Badge1ForeColor = Color.Black,
                ImagePath = "", // Set to actual banner image
                ButtonText = "Learn More",
                ShowButton = true,
                Size = new Size(400, 300),
                AccentColor = Color.FromArgb(255, 193, 7)
            };
        }

        /// <summary>
        /// Creates a list card like the director listings from samples
        /// Uses ListCardPainter.cs
        /// </summary>
        public static BeepCard CreateListCard()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.ListCard,
                HeaderText = "James T. Graham",
                SubtitleText = "Producer, Director",
                ParagraphText = "Robin Sharma, Fred Quinn and 12 more",
                Rating = 5,
                ShowRating = true,
                BadgeText1 = "PRO",
                Badge1BackColor = Color.FromArgb(33, 150, 243),
                Badge1ForeColor = Color.White,
                ImagePath = "", // Set to actual avatar
                Size = new Size(450, 120),
                AccentColor = Color.FromArgb(255, 193, 7)
            };
        }

        /// <summary>
        /// Creates a testimonial card like the quote cards from samples
        /// Uses TestimonialCardPainter.cs
        /// </summary>
        public static BeepCard CreateTestimonialCard()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.TestimonialCard,
                HeaderText = "Emma",
                ParagraphText = "\"Emma is an amazing producer.\" She put together one of the best sets that I've ever been on!",
                Rating = 5,
                ShowRating = true,
                ImagePath = "", // Set to actual avatar
                Size = new Size(350, 250),
                AccentColor = Color.FromArgb(255, 193, 7)
            };
        }

        /// <summary>
        /// Creates a feature card for app features or services
        /// Uses FeatureCardPainter.cs
        /// </summary>
        public static BeepCard CreateFeatureCard()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.FeatureCard,
                HeaderText = "Unlock magic features",
                ParagraphText = "Discover the amazing features we designed to empower your customization experience",
                ImagePath = "", // Set to feature icon
                Size = new Size(280, 320),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a dialog card for confirmations or modals
        /// Uses DialogCardPainter.cs
        /// </summary>
        public static BeepCard CreateDialogCard()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.DialogCard,
                HeaderText = "Delete content",
                ParagraphText = "Are you sure to remove this content? You can access this file for 7 days in your trash.",
                ButtonText = "Confirm",
                SecondaryButtonText = "Cancel",
                ShowButton = true,
                ShowSecondaryButton = true,
                ImagePath = "", // Optional warning icon
                Size = new Size(400, 200),
                AccentColor = Color.FromArgb(244, 67, 54) // Red for warnings
            };
        }

        /// <summary>
        /// Creates a group/community card like the NYC Coders example
        /// Uses ContentCardPainter.cs with group-specific content
        /// </summary>
        public static BeepCard CreateGroupCard()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.ContentCard,
                HeaderText = "NYC Coders",
                ParagraphText = "We are a community of developers prepping for coding interviews, participating in hackathons...",
                Tags = new List<string> { "Coding", "Community", "NYC" },
                ImagePath = "", // Group logo/banner
                ButtonText = "Join",
                SecondaryButtonText = "View",
                ShowButton = true,
                ShowSecondaryButton = true,
                Size = new Size(350, 280),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a compact profile for list views
        /// Uses CompactProfileCardPainter.cs
        /// </summary>
        public static BeepCard CreateCompactProfile()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.CompactProfile,
                HeaderText = "Richard Wyatt",
                SubtitleText = "Director, Producer",
                StatusText = "Online",
                ShowStatus = true,
                StatusColor = Color.Green,
                BadgeText1 = "MENTOR",
                Badge1BackColor = Color.FromArgb(255, 152, 0), // Orange
                Badge1ForeColor = Color.White,
                ImagePath = "", // Avatar image
                Size = new Size(400, 80),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Creates a basic general-purpose card
        /// Uses BasicCardPainter.cs
        /// </summary>
        public static BeepCard CreateBasicCard()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.BasicCard,
                HeaderText = "Basic Card Title",
                ParagraphText = "This is a simple, clean card layout suitable for general content and basic information display.",
                ButtonText = "Action",
                ShowButton = true,
                Size = new Size(300, 200),
                AccentColor = Color.FromArgb(96, 125, 139) // Blue Grey
            };
        }

        /// <summary>
        /// Creates a product card for e-commerce
        /// Uses ProductCardPainter.cs
        /// </summary>
        public static BeepCard CreateProductCard()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.ProductCard,
                HeaderText = "Wireless Bluetooth Headphones",
                ParagraphText = "Sony",
                SubtitleText = "$149.99", // Price in SubtitleText
                Rating = 4,
                ShowRating = true,
                BadgeText1 = "SALE",
                Badge1BackColor = Color.FromArgb(244, 67, 54), // Red
                Badge1ForeColor = Color.White,
                ImagePath = "", // Product image
                ButtonText = "Add to Cart",
                ShowButton = true,
                Size = new Size(280, 350),
                AccentColor = Color.FromArgb(255, 193, 7) // Amber for rating stars
            };
        }

        /// <summary>
        /// Creates a compact product card for product lists
        /// Uses ProductCompactCardPainter.cs
        /// </summary>
        public static BeepCard CreateProductCompactCard()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.ProductCompactCard,
                HeaderText = "MacBook Pro 14-inch",
                ParagraphText = "Apple",
                SubtitleText = "$1,999.00", // Price
                Rating = 5,
                ShowRating = true,
                BadgeText1 = "NEW",
                Badge1BackColor = Color.FromArgb(76, 175, 80), // Green
                Badge1ForeColor = Color.White,
                ImagePath = "", // Product thumbnail
                Size = new Size(400, 100),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a statistics/KPI card
        /// Uses StatCardPainter.cs
        /// </summary>
        public static BeepCard CreateStatCard()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.StatCard,
                HeaderText = "Total Sales", // Label
                SubtitleText = "$127,340", // Main value
                StatusText = "+12.5%", // Trend indicator
                ShowStatus = true,
                StatusColor = Color.FromArgb(76, 175, 80), // Green accent line
                ImagePath = "", // Optional icon
                Size = new Size(250, 150),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates an event/appointment card
        /// Uses EventCardPainter.cs
        /// </summary>
        public static BeepCard CreateEventCard()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.EventCard,
                HeaderText = "Design System Workshop",
                SubtitleText = "2:00 PM - 4:00 PM • Conference Room A",
                ParagraphText = "Learn to build and maintain consistent design systems for modern web applications.",
                StatusText = "MAR\n15", // Date block
                Tags = new List<string> { "Design", "Workshop", "UX" },
                ButtonText = "RSVP",
                ShowButton = true,
                Size = new Size(380, 160),
                AccentColor = Color.FromArgb(156, 39, 176) // Purple
            };
        }

        /// <summary>
        /// Creates a social media post card
        /// Uses SocialMediaCardPainter.cs
        /// </summary>
        public static BeepCard CreateSocialMediaCard()
        {
            return new BeepCard
            {
                CardStyle = CardStyle.SocialMediaCard,
                HeaderText = "Sarah Johnson",
                SubtitleText = "@sarahdesigns", // Handle
                ParagraphText = "Just finished an amazing design sprint with the team! 🎨 The new user dashboard is looking incredible. Can't wait to share it with everyone soon.",
                StatusText = "2h ago", // Timestamp
                Tags = new List<string> { "#design", "#teamwork", "#ux" },
                ButtonText = "Like",
                SecondaryButtonText = "Share",
                ShowButton = true,
                ShowSecondaryButton = true,
                ImagePath = "", // User avatar
                Size = new Size(400, 200),
                AccentColor = Color.FromArgb(29, 161, 242) // Twitter blue
            };
        }

        /// <summary>
        /// Demonstrates how to handle card events using BaseControl's hit area system
        /// </summary>
        public static void SetupCardEvents(BeepCard card)
        {
            // Use BaseControl's built-in event system instead of custom CardAreaClickedEventArgs
            card.HeaderClicked += (sender, e) => 
            {
                MessageBox.Show($"Header clicked on {card.HeaderText}");
            };

            card.ButtonClicked += (sender, e) => 
            {
                if (e.EventName == "ButtonClicked")
                    MessageBox.Show($"Primary button clicked: {card.ButtonText}");
                else if (e.EventName == "SecondaryButtonClicked")
                    MessageBox.Show($"Secondary button clicked: {card.SecondaryButtonText}");
            };

            card.ImageClicked += (sender, e) => 
            {
                MessageBox.Show("Image clicked - could open full-size view");
            };

            // Use BaseControl's HitDetected event for custom hit areas registered by painters
            card.HitDetected += (sender, e) => 
            {
                MessageBox.Show($"Custom area clicked: {e.HitTest.Name}");
            };
        }

        /// <summary>
        /// Creates a comprehensive demonstration form with all card styles
        /// </summary>
        public static Form CreateDemoForm()
        {
            var form = new Form
            {
                Text = "BeepCard ProgressBarStyle Demo - All 13 Styles",
                Size = new Size(1400, 900),
                StartPosition = FormStartPosition.CenterScreen
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10)
            };

            // Add all card styles including new ones
            var cards = new[]
            {
                CreateProfileCard(),
                CreateCompactProfile(),
                CreateContentCard(),
                CreateFeatureCard(),
                CreateListCard(),
                CreateTestimonialCard(),
                CreateDialogCard(),
                CreateBasicCard(),
                CreateProductCard(),
                CreateProductCompactCard(),
                CreateStatCard(),
                CreateEventCard(),
                CreateSocialMediaCard(),
                CreateGroupCard()
            };

            foreach (var card in cards)
            {
                card.Margin = new Padding(10);
                SetupCardEvents(card);
                panel.Controls.Add(card);
            }

            form.Controls.Add(panel);
            return form;
        }

        /// <summary>
        /// Creates a specialized form showing e-commerce cards
        /// </summary>
        public static Form CreateECommerceDemo()
        {
            var form = new Form
            {
                Text = "E-Commerce Card Demo",
                Size = new Size(1000, 600),
                StartPosition = FormStartPosition.CenterScreen
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10)
            };

            // Multiple product cards
            var products = new[]
            {
                ("Wireless Headphones", "Sony", "$149.99", 4, "SALE"),
                ("Gaming Mouse", "Logitech", "$79.99", 5, "NEW"),
                ("Mechanical Keyboard", "Corsair", "$129.99", 4, ""),
                ("Webcam HD", "Microsoft", "$89.99", 3, "DEAL"),
            };

            foreach (var (name, brand, price, rating, badge) in products)
            {
                var card = new BeepCard
                {
                    CardStyle = CardStyle.ProductCard,
                    HeaderText = name,
                    ParagraphText = brand,
                    SubtitleText = price,
                    Rating = rating,
                    ShowRating = true,
                    BadgeText1 = badge,
                    Badge1BackColor = badge == "SALE" ? Color.Red : badge == "NEW" ? Color.Green : Color.Orange,
                    Badge1ForeColor = Color.White,
                    ButtonText = "Add to Cart",
                    ShowButton = true,
                    Size = new Size(280, 350),
                    AccentColor = Color.FromArgb(255, 193, 7),
                    Margin = new Padding(10)
                };
                
                SetupCardEvents(card);
                panel.Controls.Add(card);
            }

            form.Controls.Add(panel);
            return form;
        }

        /// <summary>
        /// Creates a dashboard-style form with stat cards
        /// </summary>
        public static Form CreateDashboardDemo()
        {
            var form = new Form
            {
                Text = "Dashboard Statistics Demo",
                Size = new Size(1200, 400),
                StartPosition = FormStartPosition.CenterScreen
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(20)
            };

            // Various KPI cards
            var stats = new[]
            {
                ("Total Revenue", "$847,391", "+8.2%", Color.Green),
                ("Active Users", "23,456", "+15.3%", Color.Blue),
                ("Conversion Rate", "3.24%", "-2.1%", Color.Red),
                ("Avg Order Value", "$89.50", "+5.7%", Color.Purple),
            };

            foreach (var (label, value, trend, color) in stats)
            {
                var card = new BeepCard
                {
                    CardStyle = CardStyle.StatCard,
                    HeaderText = label,
                    SubtitleText = value,
                    StatusText = trend,
                    ShowStatus = true,
                    StatusColor = color,
                    Size = new Size(250, 150),
                    AccentColor = color,
                    Margin = new Padding(10)
                };
                
                SetupCardEvents(card);
                panel.Controls.Add(card);
            }

            form.Controls.Add(panel);
            return form;
        }
    }
}