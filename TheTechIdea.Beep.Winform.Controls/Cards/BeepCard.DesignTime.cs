using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepCard
    {
        #region Design-Time Data

        // Apply design-time dummy data based on card Style - inspired by modern web frameworks
        private void ApplyDesignTimeData()
        {
            if (!DesignMode) return;

            ResetDesignTimeData();

            switch (_style)
            {
                #region Profile & User Cards
                
                case CardStyle.ProfileCard:
                    headerText = "Alex Morgan";
                    paragraphText = "Senior Full Stack Developer | Cloud Architecture Specialist\nPassionate about building scalable solutions";
                    _subtitleText = "@alexmorgan";
                    _statusText = "Available for work";
                    buttonText = "Follow";
                    secondaryButtonText = "Message";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.Cat;
                    _badgeText1 = "Pro";
                    _badge1BackColor = Color.FromArgb(255, 193, 7); // Amber
                    _badge1ForeColor = Color.Black;
                    break;

                case CardStyle.CompactProfile:
                    headerText = "Jordan Chen";
                    _subtitleText = "UI/UX Designer • 12K followers";
                    _statusText = "Active now";
                    imagePath = Svgs.Person;
                    _showStatus = true;
                    _statusColor = Color.FromArgb(76, 175, 80); // Green
                    break;

                case CardStyle.UserCard:
                    headerText = "Taylor Swift";
                    paragraphText = "Product Manager at TechCorp";
                    _subtitleText = "San Francisco, CA";
                    buttonText = "View Profile";
                    showButton = true;
                    imagePath = Svgs.User;
                    _badgeText1 = "2.5K Followers";
                    _badge1BackColor = Color.FromArgb(33, 150, 243); // Blue
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.TeamMemberCard:
                    headerText = "Morgan Lee";
                    paragraphText = "Builds secure delivery platforms and mentors the infrastructure guild.";
                    _subtitleText = "Lead DevOps Engineer";
                    _statusText = "24 Projects | 8 Years | 12 Certs";
                    showButton = true;
                    imagePath = Svgs.PersonEdit;
                    break;

                #endregion

                #region Content & Blog Cards

                case CardStyle.ContentCard:
                    headerText = "10 Best Practices for Modern UI Design";
                    paragraphText = "Discover the latest trends and techniques that top designers use to create stunning user interfaces.";
                    _subtitleText = "Design • 5 min read";
                    _badgeText1 = "NEW";
                    _badge1BackColor = Color.FromArgb(76, 175, 80); // Green
                    _badge1ForeColor = Color.White;
                    buttonText = "Read Article";
                    showButton = true;
                    imagePath = Svgs.Beep;
                    break;

                case CardStyle.BlogCard:
                    headerText = "Building Scalable Microservices with .NET";
                    paragraphText = "Learn how to architect and deploy microservices that can handle millions of requests per day.";
                    _subtitleText = "By John Smith • Dec 15, 2024";
                    _badgeText1 = "Development";
                    _badge1BackColor = Color.FromArgb(33, 150, 243); // Blue
                    _badge1ForeColor = Color.White;
                    buttonText = "Continue Reading";
                    showButton = true;
                    imagePath = Svgs.Beep;
                    break;

                case CardStyle.NewsCard:
                    headerText = "Microsoft Announces New AI Features";
                    paragraphText = "Tech giant unveils groundbreaking AI capabilities in latest product update";
                    _subtitleText = "TechNews • 1 hour ago";
                    _badgeText1 = "Breaking";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    buttonText = "Read More";
                    showButton = true;
                    imagePath = Svgs.InfoAlert;
                    break;

                case CardStyle.MediaCard:
                    headerText = "Stunning Landscape Photography";
                    paragraphText = "Explore breathtaking views from around the world";
                    _subtitleText = "Photography Collection";
                    buttonText = string.Empty;
                    showButton = false;
                    _badgeText1 = "Gallery";
                    _badge1BackColor = Color.FromArgb(33, 150, 243); // Blue
                    _badge1ForeColor = Color.White;
                    imagePath = Svgs.Cat;
                    break;

                #endregion

                #region Feature & Service Cards

                case CardStyle.FeatureCard:
                    headerText = "Advanced Analytics";
                    paragraphText = "Get deep insights into your data with our powerful analytics engine. Real-time dashboards and custom reports.";
                    _badgeText1 = "PRO";
                    _badge1BackColor = Color.FromArgb(255, 152, 0); // Orange
                    _badge1ForeColor = Color.White;
                    buttonText = "Learn More";
                    showButton = true;
                    imagePath = Svgs.TrendUp;
                    break;

                case CardStyle.ServiceCard:
                    headerText = "Cloud Hosting";
                    paragraphText = "Deploy your applications with confidence. 99.9% uptime guarantee.";
                    _subtitleText = "Starting at $9.99/month";
                    buttonText = "Get Started";
                    showButton = true;
                    imagePath = Svgs.DataSources;
                    _badgeText1 = "Popular";
                    _badge1BackColor = Color.FromArgb(156, 39, 176); // Purple
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.IconCard:
                    headerText = "Secure & Encrypted";
                    paragraphText = "Your data is protected with enterprise-grade security";
                    buttonText = "View Security";
                    showButton = true;
                    imagePath = Svgs.Keys;
                    break;

                case CardStyle.BenefitCard:
                    headerText = "Why Choose Us?";
                    paragraphText = "✓ 24/7 Support\n✓ Easy Integration\n✓ Scalable Solutions\n✓ Cost Effective";
                    buttonText = "Compare Plans";
                    showButton = true;
                    imagePath = Svgs.CheckCircle;
                    break;

                #endregion

                #region E-commerce & Product Cards

                case CardStyle.ProductCard:
                    headerText = "Wireless Headphones Pro";
                    paragraphText = "Premium noise-cancelling headphones with 40hr battery life";
                    _subtitleText = "$299.99";
                    _priceText = "$299.99";
                    buttonText = "Add to Cart";
                    showButton = true;
                    imagePath = Svgs.Cat;
                    _rating = 5;
                    _showRating = true;
                    _badgeText1 = "-20%";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.PricingCard:
                    headerText = "Professional Plan";
                    paragraphText = "Perfect for growing teams\n• Unlimited projects\n• Advanced features\n• Priority support\n• Custom integrations";
                    _subtitleText = "$49/month";
                    _priceText = "$49";
                    _statusText = "Billed monthly";
                    buttonText = "Choose Plan";
                    showButton = true;
                    _badgeText1 = "Most Popular";
                    _badge1BackColor = Color.FromArgb(255, 193, 7); // Amber
                    _badge1ForeColor = Color.Black;
                    break;

                case CardStyle.OfferCard:
                    headerText = "Black Friday Sale!";
                    paragraphText = "Save up to 70% on select items. Limited time offer!";
                    _subtitleText = "$29.99 | $99.99";
                    _statusText = "Ends in 2 days";
                    _showStatus = true;
                    buttonText = "Shop Now";
                    showButton = true;
                    _badgeText1 = "-70%";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    imagePath = Svgs.Star;
                    break;

                case CardStyle.CartItemCard:
                    headerText = "Premium T-Shirt";
                    paragraphText = "$29.99 each";
                    _subtitleText = "Size: L • Color: Navy Blue";
                    _statusText = "$59.98 total";
                    _badgeText1 = "2";
                    buttonText = "Update";
                    secondaryButtonText = "Remove";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.Cat;
                    break;

                #endregion

                #region Social & Interaction Cards

                case CardStyle.TestimonialCard:
                    headerText = "Emma Wilson";
                    paragraphText = "\"This product completely transformed our workflow. The team is more productive than ever, and our clients are thrilled with the results!\"";
                    _subtitleText = "CEO, TechVision Inc.";
                    _rating = 5;
                    _showRating = true;
                    imagePath = Svgs.Person;
                    break;

                case CardStyle.ReviewCard:
                    headerText = "David Martinez";
                    paragraphText = "I've been using this for 6 months and it's been fantastic. Highly recommend to anyone looking for a reliable solution.";
                    _subtitleText = "Verified Purchase • 6 months ago";
                    _rating = 5;
                    _showRating = true;
                    _statusText = "24 people found this helpful";
                    buttonText = "Helpful";
                    showButton = true;
                    imagePath = Svgs.Person;
                    _badgeText1 = "Verified";
                    _badge1BackColor = Color.FromArgb(76, 175, 80); // Green
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.CommentCard:
                    headerText = "Michael Chen";
                    paragraphText = "Great article! I especially liked the section about performance optimization. Would love to see more content like this.";
                    _statusText = "2 hours ago";
                    buttonText = string.Empty;
                    secondaryButtonText = string.Empty;
                    showButton = false;
                    showSecondaryButton = false;
                    imagePath = Svgs.Comment;
                    _badgeText1 = "15";
                    break;

                case CardStyle.SocialMediaCard:
                    headerText = "Sarah Johnson";
                    paragraphText = "Just shipped a major update! 🚀 Check out the new features we've been working on. Your feedback would mean a lot! #development #coding";
                    _subtitleText = "@sarahdesigns";
                    _statusText = "3 hours ago";
                    buttonText = string.Empty;
                    secondaryButtonText = string.Empty;
                    showButton = false;
                    showSecondaryButton = false;
                    imagePath = Svgs.Person;
                    break;

                #endregion

                #region Dashboard & Analytics Cards

                case CardStyle.StatCard:
                    headerText = "12,458";
                    paragraphText = "Active Users";
                    _statusText = "+18.2% from last month";
                    imagePath = Svgs.TrendUp;
                    _showStatus = true;
                    _statusColor = Color.FromArgb(76, 175, 80);
                    break;

                case CardStyle.ChartCard:
                    headerText = "Revenue Overview";
                    paragraphText = "Revenue vs target";
                    _subtitleText = "$124,500";
                    _statusText = "+12.4%";
                    _badgeText1 = "Q4";
                    break;

                case CardStyle.MetricCard:
                    headerText = "Conversion Rate";
                    paragraphText = "Target: 4.0%";
                    _subtitleText = "3.8%";
                    _statusText = "+0.5% vs last week";
                    _badgeText1 = "Growth";
                    _badge1BackColor = Color.FromArgb(255, 152, 0); // Orange
                    _badge1ForeColor = Color.White;
                    imagePath = Svgs.Sum;
                    break;

                case CardStyle.ActivityCard:
                    headerText = "New Order Placed";
                    paragraphText = "Order #4567 by John Smith\nTotal: $156.99";
                    _statusText = "5 minutes ago";
                    imagePath = Svgs.InfoAlert;
                    _badgeText1 = "NEW";
                    _badge1BackColor = Color.FromArgb(33, 150, 243); // Blue
                    _badge1ForeColor = Color.White;
                    _showStatus = true;
                    _statusColor = Color.FromArgb(33, 150, 243);
                    break;

                #endregion

                #region Communication & Messaging Cards

                case CardStyle.NotificationCard:
                    headerText = "System Update Available";
                    paragraphText = "A new version is ready to install. Update now to get the latest features and security improvements.";
                    _statusText = "Just now";
                    _subtitleText = "Recommended update";
                    _showStatus = true;
                    imagePath = Svgs.InfoInfo;
                    break;

                case CardStyle.MessageCard:
                    headerText = "Lisa Anderson";
                    paragraphText = "Hey! Did you get a chance to review the proposal? Let me know if you have any questions.";
                    _statusText = "10:32 AM";
                    imagePath = Svgs.Mail;
                    break;

                case CardStyle.AlertCard:
                    headerText = "Action Required";
                    paragraphText = "Your payment method will expire soon. Please update your billing information to avoid service interruption.";
                    _subtitleText = "Expires in 7 days";
                    buttonText = "Update Payment";
                    showButton = true;
                    imagePath = Svgs.ExclamationTriangle;
                    _badgeText1 = "URGENT";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.AnnouncementCard:
                    headerText = "New Feature Released!";
                    paragraphText = "We've added dark mode support and improved performance. Check out what's new in the latest update.";
                    _subtitleText = "December 15, 2024";
                    buttonText = "Learn More";
                    showButton = true;
                    imagePath = Svgs.Beep;
                    _badgeText1 = "v2.5";
                    _badge1BackColor = Color.FromArgb(156, 39, 176); // Purple
                    _badge1ForeColor = Color.White;
                    break;

                #endregion

                #region Event & Calendar Cards

                case CardStyle.EventCard:
                    headerText = "Annual Developer Conference 2025";
                    paragraphText = "Join industry leaders for three days of workshops, keynotes, and networking.";
                    _subtitleText = "San Francisco Convention Center";
                    _statusText = "JAN\n15";
                    _badgeText1 = "Early Bird";
                    _badge1BackColor = Color.FromArgb(156, 39, 176); // Purple
                    _badge1ForeColor = Color.White;
                    buttonText = "Register Now";
                    showButton = true;
                    imagePath = string.Empty;
                    break;

                case CardStyle.CalendarEventCard:
                    headerText = "Team Meeting";
                    paragraphText = "Q4 Planning & Strategy Review";
                    _subtitleText = "DEC\n15";
                    _statusText = "Today at 2:00 PM";
                    buttonText = "Join Meeting";
                    showButton = true;
                    imagePath = string.Empty;
                    _badgeText1 = "Room A";
                    _badge1BackColor = Color.FromArgb(33, 150, 243); // Blue
                    _badge1ForeColor = Color.White;
                    _showStatus = true;
                    _statusColor = Color.FromArgb(103, 58, 183); // Deep Purple
                    break;

                case CardStyle.ScheduleCard:
                    headerText = "Doctor's Appointment";
                    paragraphText = "Dr. Sarah Williams • Annual Checkup";
                    _subtitleText = "Medical Center • Building A";
                    _statusText = "10:30 AM - 11:30 AM";
                    buttonText = string.Empty;
                    secondaryButtonText = string.Empty;
                    showButton = false;
                    showSecondaryButton = false;
                    imagePath = string.Empty;
                    _badgeText1 = "Confirmed";
                    _badge1BackColor = Color.FromArgb(76, 175, 80); // Green
                    _badge1ForeColor = Color.White;
                    _statusColor = Color.FromArgb(33, 150, 243); // Blue
                    break;

                case CardStyle.TaskCard:
                    headerText = "Complete Project Documentation";
                    paragraphText = "Write comprehensive docs for the new API endpoints";
                    _statusText = "Due: Dec 20, 2024";
                    buttonText = string.Empty;
                    showButton = false;
                    imagePath = string.Empty;
                    _badgeText1 = "High Priority";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    _showStatus = true;
                    _statusColor = Color.FromArgb(255, 152, 0); // Orange
                    break;

                #endregion

                #region List & Data Cards

                case CardStyle.ListCard:
                    headerText = "Project Milestones";
                    paragraphText = "✓ Requirements gathering\n✓ UI/UX design\n○ Development\n○ Testing\n○ Deployment";
                    _statusText = "Phase 2 of 5";
                    _statusColor = Color.FromArgb(33, 150, 243); // Blue
                    _showStatus = true;
                    buttonText = "View Timeline";
                    showButton = true;
                    imagePath = Svgs.Bullet;
                    break;

                case CardStyle.DataCard:
                    headerText = "Server Status";
                    paragraphText = "CPU: 45%\nMemory: 62%\nDisk: 78%\nNetwork: 12 MB/s";
                    _subtitleText = "CPU\nMemory\nDisk\nNetwork";
                    buttonText = "Details";
                    showButton = true;
                    imagePath = Svgs.DataView;
                    _badgeText1 = "Healthy";
                    _badge1BackColor = Color.FromArgb(76, 175, 80); // Green
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.FormCard:
                    headerText = "Contact Information";
                    paragraphText = "Update your profile details";
                    buttonText = "Save Changes";
                    secondaryButtonText = "Cancel";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.Edit;
                    break;

                case CardStyle.SettingsCard:
                    headerText = "Email Notifications";
                    paragraphText = "Receive updates about your account activity";
                    _statusText = "Enabled";
                    buttonText = string.Empty;
                    showButton = false;
                    imagePath = Svgs.Settings;
                    _showRating = true;
                    break;

                #endregion

                #region Specialized Cards

                case CardStyle.DialogCard:
                    headerText = "Delete Account?";
                    paragraphText = "This action cannot be undone. All your data will be permanently deleted from our servers.";
                    buttonText = "Delete";
                    secondaryButtonText = "Cancel";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.ExclamationTriangle;
                    _badgeText1 = "WARNING";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.BasicCard:
                    headerText = "Simple Card Example";
                    paragraphText = "This is a basic card with minimal styling. Perfect for displaying straightforward information.";
                    buttonText = "Action";
                    showButton = true;
                    imagePath = Svgs.Beep;
                    break;

                case CardStyle.HoverCard:
                    headerText = "Interactive Experience";
                    paragraphText = "Hover over this card to see the smooth transition effects";
                    buttonText = string.Empty;
                    showButton = true;
                    imagePath = Svgs.Cool;
                    _badgeText1 = "Explore";
                    break;

                case CardStyle.InteractiveCard:
                    headerText = "Multi-Action Card";
                    paragraphText = "This card supports multiple actions and interactions";
                    buttonText = "Primary Action";
                    secondaryButtonText = "Secondary";
                    showButton = true;
                    showSecondaryButton = true;
                    imagePath = Svgs.More;
                    break;

                case CardStyle.ImageCard:
                    headerText = "Beautiful Landscapes";
                    paragraphText = "Explore stunning photography from around the world";
                    buttonText = "View Gallery";
                    showButton = true;
                    imagePath = Svgs.Cat;
                    break;

                case CardStyle.VideoCard:
                    headerText = "Product Demo Video";
                    paragraphText = "1.2M views • 3 days ago";
                    _subtitleText = "Beep Labs";
                    _statusText = "5:42";
                    buttonText = string.Empty;
                    showButton = false;
                    _showStatus = true;
                    imagePath = Svgs.Cat;
                    _badgeText1 = "HD";
                    _badge1BackColor = Color.FromArgb(244, 67, 54); // Red
                    _badge1ForeColor = Color.White;
                    break;

                case CardStyle.DownloadCard:
                    headerText = "Q4_2024_Report.pdf";
                    paragraphText = "Annual financial report and analysis";
                    _subtitleText = "2.4 MB • PDF Document";
                    buttonText = "Download";
                    showButton = true;
                    imagePath = Svgs.File;
                    break;

                case CardStyle.ContactCard:
                    headerText = "Customer Support";
                    paragraphText = "📧 support@example.com\n📞 +1 (555) 123-4567\n📍 123 Main St, San Francisco";
                    _subtitleText = "Available 24/7";
                    buttonText = "Contact Us";
                    showButton = true;
                    imagePath = Svgs.AddressBook;
                    break;

                #endregion

                default:
                    headerText = "Modern Card Design";
                    paragraphText = "This card showcases modern UI design principles with clean typography and thoughtful spacing.";
                    buttonText = "Learn More";
                    showButton = true;
                    imagePath = Svgs.Beep;
                    break;
            }

            Invalidate();
        }

        #endregion

        // Returns a sample image path for design-time previews when ImagePath is empty.
        private string GetDesignTimeSampleImage(CardStyle style)
        {
            // Use Svgs static class for all image references
            switch (style)
            {
                // Profile & User
                case CardStyle.ProfileCard:
                case CardStyle.UserCard:
                    return Svgs.Cat;
                case CardStyle.CompactProfile:
                    return Svgs.Person;
                case CardStyle.TeamMemberCard:
                    return Svgs.PersonEdit;

                // Content & Blog
                case CardStyle.ContentCard:
                case CardStyle.BlogCard:
                case CardStyle.MediaCard:
                    return Svgs.Beep;
                case CardStyle.NewsCard:
                    return Svgs.InfoAlert;

                // Feature & Service
                case CardStyle.FeatureCard:
                    return Svgs.TrendUp;
                case CardStyle.ServiceCard:
                    return Svgs.DataSources;
                case CardStyle.IconCard:
                    return Svgs.Keys;
                case CardStyle.BenefitCard:
                    return Svgs.CheckCircle;

                // E-commerce & Product
                case CardStyle.ProductCard:
                case CardStyle.CartItemCard:
                    return Svgs.Cat;
                case CardStyle.OfferCard:
                    return Svgs.Star;

                // Social & Interaction
                case CardStyle.TestimonialCard:
                case CardStyle.SocialMediaCard:
                    return Svgs.Person;
                case CardStyle.ReviewCard:
                    return Svgs.ThumbUp;
                case CardStyle.CommentCard:
                    return Svgs.Comment;

                // Dashboard & Analytics
                case CardStyle.StatCard:
                    return Svgs.TrendUp;
                case CardStyle.ChartCard:
                    return string.Empty;
                case CardStyle.MetricCard:
                    return Svgs.Sum;
                case CardStyle.ActivityCard:
                    return Svgs.InfoAlert;

                // Communication & Messaging
                case CardStyle.NotificationCard:
                    return Svgs.InfoInfo;
                case CardStyle.MessageCard:
                    return Svgs.Mail;
                case CardStyle.AlertCard:
                case CardStyle.DialogCard:
                    return Svgs.ExclamationTriangle;
                case CardStyle.AnnouncementCard:
                    return Svgs.Beep;

                // Event & Calendar
                case CardStyle.EventCard:
                    return string.Empty;
                case CardStyle.CalendarEventCard:
                    return string.Empty;
                case CardStyle.ScheduleCard:
                    return string.Empty;
                case CardStyle.TaskCard:
                    return string.Empty;

                // List & Data
                case CardStyle.ListCard:
                    return Svgs.Bullet;
                case CardStyle.DataCard:
                    return Svgs.DataView;
                case CardStyle.FormCard:
                    return Svgs.Edit;
                case CardStyle.SettingsCard:
                    return Svgs.Settings;

                // Specialized
                case CardStyle.BasicCard:
                case CardStyle.HoverCard:
                    return Svgs.Beep;
                case CardStyle.InteractiveCard:
                    return Svgs.More;
                case CardStyle.ImageCard:
                case CardStyle.VideoCard:
                    return Svgs.Cat;
                case CardStyle.DownloadCard:
                    return Svgs.File;
                case CardStyle.ContactCard:
                    return Svgs.AddressBook;

                default:
                    return Svgs.Beep;
            }
        }

        private void ResetDesignTimeData()
        {
            headerText = "Card Title";
            paragraphText = "Card Description";
            buttonText = "Action";
            secondaryButtonText = "More";
            showButton = false;
            showSecondaryButton = false;
            imagePath = string.Empty;
            _subtitleText = string.Empty;
            _statusText = string.Empty;
            _showStatus = false;
            _statusColor = Color.Green;
            _rating = 0;
            _showRating = false;
            _badgeText1 = string.Empty;
            _badge1BackColor = Color.FromArgb(255, 235, 59);
            _badge1ForeColor = Color.Black;
            _badgeText2 = string.Empty;
            _badge2BackColor = Color.FromArgb(33, 150, 243);
            _badge2ForeColor = Color.White;
            _priceText = string.Empty;
        }

        private string PathCombineGfx(string folder, string file)
        {
            try
            {
                var baseDir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location) ?? string.Empty, "GFX");
                var candidate = System.IO.Path.Combine(baseDir, folder, file);
                if (System.IO.File.Exists(candidate)) return candidate;
            }
            catch { }
            // fallback to logical name (ImagePainter/ImageListHelper may resolve)
            return file;
        }

    }
}
