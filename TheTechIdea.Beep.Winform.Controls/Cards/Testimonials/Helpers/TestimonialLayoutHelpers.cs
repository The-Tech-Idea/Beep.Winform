using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Testimonials.Helpers
{
    /// <summary>
    /// Centralized layout management for Testimonial controls
    /// Provides responsive layout calculations per view type
    /// </summary>
    public static class TestimonialLayoutHelpers
    {
        #region Layout Calculations

        /// <summary>
        /// Calculate layout for all elements based on view type
        /// </summary>
        public static TestimonialLayout CalculateLayout(
            BeepTestimonial testimonial,
            object viewType, // TestimonialViewType
            Rectangle cardBounds,
            Padding padding)
        {
            string viewTypeStr = viewType?.ToString() ?? "Classic";

            return viewTypeStr switch
            {
                "Classic" => CalculateClassicLayout(cardBounds, padding),
                "Minimal" => CalculateMinimalLayout(cardBounds, padding),
                "Compact" => CalculateCompactLayout(cardBounds, padding),
                "SocialCard" => CalculateSocialCardLayout(cardBounds, padding),
                _ => CalculateClassicLayout(cardBounds, padding)
            };
        }

        /// <summary>
        /// Calculate classic view layout
        /// </summary>
        public static TestimonialLayout CalculateClassicLayout(
            Rectangle cardBounds,
            Padding padding)
        {
            return new TestimonialLayout
            {
                RatingBounds = new Rectangle(cardBounds.Left + padding.Left, cardBounds.Top + padding.Top, 100, 20),
                TestimonialBounds = new Rectangle(cardBounds.Left + padding.Left, cardBounds.Top + padding.Top + 25, cardBounds.Width - padding.Horizontal, 60),
                ImageBounds = new Rectangle(cardBounds.Left + padding.Left, cardBounds.Top + padding.Top + 95, 50, 50),
                NameBounds = new Rectangle(cardBounds.Left + padding.Left + 60, cardBounds.Top + padding.Top + 95, cardBounds.Width - padding.Horizontal - 60, 20),
                PositionBounds = new Rectangle(cardBounds.Left + padding.Left + 60, cardBounds.Top + padding.Top + 115, cardBounds.Width - padding.Horizontal - 60, 20),
                UsernameBounds = new Rectangle(cardBounds.Left + padding.Left + 60, cardBounds.Top + padding.Top + 135, cardBounds.Width - padding.Horizontal - 60, 20),
                CompanyLogoBounds = Rectangle.Empty,
                CloseButtonBounds = Rectangle.Empty
            };
        }

        /// <summary>
        /// Calculate minimal view layout
        /// </summary>
        public static TestimonialLayout CalculateMinimalLayout(
            Rectangle cardBounds,
            Padding padding)
        {
            int centerX = cardBounds.Left + cardBounds.Width / 2;
            int companyLogoWidth = 60;
            int companyLogoHeight = 30;

            return new TestimonialLayout
            {
                CompanyLogoBounds = new Rectangle(centerX - companyLogoWidth / 2, cardBounds.Top + padding.Top, companyLogoWidth, companyLogoHeight),
                TestimonialBounds = new Rectangle(cardBounds.Left + padding.Left, cardBounds.Top + padding.Top + 35, cardBounds.Width - padding.Horizontal, 80),
                NameBounds = new Rectangle(centerX - 50, cardBounds.Top + padding.Top + 125, 100, 20),
                PositionBounds = new Rectangle(centerX - 50, cardBounds.Top + padding.Top + 145, 100, 20),
                UsernameBounds = new Rectangle(centerX - 50, cardBounds.Top + padding.Top + 165, 100, 20),
                ImageBounds = new Rectangle(centerX - 25, cardBounds.Top + padding.Top + 185, 50, 50),
                RatingBounds = Rectangle.Empty,
                CloseButtonBounds = Rectangle.Empty
            };
        }

        /// <summary>
        /// Calculate compact view layout
        /// </summary>
        public static TestimonialLayout CalculateCompactLayout(
            Rectangle cardBounds,
            Padding padding)
        {
            return new TestimonialLayout
            {
                TestimonialBounds = new Rectangle(cardBounds.Left + padding.Left, cardBounds.Top + padding.Top, cardBounds.Width - padding.Horizontal, 40),
                ImageBounds = new Rectangle(cardBounds.Left + padding.Left, cardBounds.Top + padding.Top + 55, 40, 40),
                NameBounds = new Rectangle(cardBounds.Left + padding.Left + 50, cardBounds.Top + padding.Top + 55, cardBounds.Width - padding.Horizontal - 50, 20),
                PositionBounds = new Rectangle(cardBounds.Left + padding.Left + 50, cardBounds.Top + padding.Top + 75, cardBounds.Width - padding.Horizontal - 50, 20),
                UsernameBounds = new Rectangle(cardBounds.Left + padding.Left + 50, cardBounds.Top + padding.Top + 95, cardBounds.Width - padding.Horizontal - 50, 20),
                RatingBounds = Rectangle.Empty,
                CompanyLogoBounds = Rectangle.Empty,
                CloseButtonBounds = Rectangle.Empty
            };
        }

        /// <summary>
        /// Calculate social card view layout
        /// </summary>
        public static TestimonialLayout CalculateSocialCardLayout(
            Rectangle cardBounds,
            Padding padding)
        {
            return new TestimonialLayout
            {
                ImageBounds = new Rectangle(cardBounds.Left + padding.Left, cardBounds.Top + padding.Top, 40, 40),
                NameBounds = new Rectangle(cardBounds.Left + padding.Left + 50, cardBounds.Top + padding.Top, cardBounds.Width - padding.Horizontal - 80, 20),
                UsernameBounds = new Rectangle(cardBounds.Left + padding.Left + 50, cardBounds.Top + padding.Top + 20, cardBounds.Width - padding.Horizontal - 80, 20),
                PositionBounds = new Rectangle(cardBounds.Left + padding.Left + 50, cardBounds.Top + padding.Top + 40, cardBounds.Width - padding.Horizontal - 80, 20),
                TestimonialBounds = new Rectangle(cardBounds.Left + padding.Left, cardBounds.Top + padding.Top + 60, cardBounds.Width - padding.Horizontal, 60),
                CloseButtonBounds = new Rectangle(cardBounds.Right - padding.Right - 24, cardBounds.Top + padding.Top, 24, 24),
                RatingBounds = Rectangle.Empty,
                CompanyLogoBounds = Rectangle.Empty
            };
        }

        /// <summary>
        /// Get optimal card size based on view type
        /// </summary>
        public static Size GetOptimalCardSize(
            object viewType,
            Padding padding)
        {
            string viewTypeStr = viewType?.ToString() ?? "Classic";

            return viewTypeStr switch
            {
                "Classic" => new Size(350, 200),
                "Minimal" => new Size(350, 300),
                "Compact" => new Size(300, 150),
                "SocialCard" => new Size(320, 160),
                _ => new Size(350, 200)
            };
        }

        #endregion
    }

    /// <summary>
    /// Layout structure for testimonial elements
    /// </summary>
    public class TestimonialLayout
    {
        public Rectangle RatingBounds { get; set; }
        public Rectangle TestimonialBounds { get; set; }
        public Rectangle ImageBounds { get; set; }
        public Rectangle NameBounds { get; set; }
        public Rectangle UsernameBounds { get; set; }
        public Rectangle PositionBounds { get; set; }
        public Rectangle CompanyLogoBounds { get; set; }
        public Rectangle CloseButtonBounds { get; set; }
    }
}

