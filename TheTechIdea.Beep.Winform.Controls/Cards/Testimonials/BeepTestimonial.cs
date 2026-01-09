using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Testimonials.Helpers;
using TheTechIdea.Beep.Winform.Controls.ToolTips;
using TheTechIdea.Beep.Winform.Controls.Ratings;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum TestimonialViewType
    {
        Classic,
        Minimal,
        Compact,
        SocialCard
    }

    public class BeepTestimonial : BaseControl
    {
        // Keep only BeepStarRating as a child control
        private BeepStarRating starRating;
        
        // Data fields
        private string _imagePath = "";
        private string _companyLogoPath = "";
        private string _testimonial = "This is a testimonial.";
        private string _name = "John Doe";
        private string _username = "@johndoe";
        private string _position = "Developer";
        private string _rating = "5";
        
        // Layout rectangles for hit testing
        private Rectangle imageRect;
        private Rectangle companyLogoRect;
        private Rectangle testimonialRect;
        private Rectangle nameRect;
        private Rectangle usernameRect;
        private Rectangle positionRect;
        private Rectangle closeButtonRect;
        private Rectangle ratingRect;
        
        // Hover states
        private string hoveredArea = null;

        private TestimonialViewType _viewType = TestimonialViewType.Classic;
        private bool _isApplyingTheme = false;
        private bool _autoGenerateTooltip = true;

        public BeepTestimonial()
        {
            Size = new Size(350, 200);
            Padding = new Padding(10);
            
            // Initialize BeepStarRating
            starRating = new BeepStarRating
            {
                StarCount = 5,
                SelectedRating = 5,
                ReadOnly = true,
                IsFrameless = true,
                IsChild = true,
                Size = new Size(100, 20)
            };
            Controls.Add(starRating);

            TestimonialAccessibilityHelpers.ApplyAccessibilitySettings(this);
            ApplyTheme();

            if (_autoGenerateTooltip)
            {
                UpdateTestimonialTooltip();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (DesignMode)
            {
                SetDummyData();
            }
            ApplyTheme();
        }

        public void BeginInit() { }
        public void EndInit()
        {
            if (DesignMode)
            {
                SetDummyData();
            }
            ApplyTheme();
        }

        private void SetDummyData()
        {
            ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.cat.svg";
            CompanyLogoPath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.kitty.svg";
            Testimonial = "This product is amazing!";
            Name = "Nick Parsons";
            Username = "@nickparsons";
            Position = "Director of Marketing";
            Rating = "5";
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Switch between different testimonial views.")]
        public TestimonialViewType ViewType
        {
            get => _viewType;
            set
            {
                _viewType = value;
                Size = TestimonialLayoutHelpers.GetOptimalCardSize(_viewType, Padding);
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string ImagePath
        {
            get => _imagePath;
            set 
            { 
                _imagePath = TestimonialIconHelpers.ResolveIconPath(value, TestimonialIconHelpers.GetRecommendedAvatarIcon());
                TestimonialAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateTestimonialTooltip();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string CompanyLogoPath
        {
            get => _companyLogoPath;
            set 
            { 
                _companyLogoPath = TestimonialIconHelpers.ResolveIconPath(value, TestimonialIconHelpers.GetRecommendedCompanyLogoIcon());
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string Testimonial
        {
            get => _testimonial;
            set
            {
                _testimonial = value ?? "Default Testimonial";
                TestimonialAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateTestimonialTooltip();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value ?? "Anonymous";
                TestimonialAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateTestimonialTooltip();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string Username
        {
            get => _username;
            set
            {
                _username = value ?? "@username";
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string Position
        {
            get => _position;
            set
            {
                _position = value ?? "Unknown Position";
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public string Rating
        {
            get => _rating;
            set
            {
                _rating = value ?? "5";
                if (starRating != null && int.TryParse(_rating, out int rating))
                {
                    starRating.SelectedRating = Math.Max(0, Math.Min(5, rating));
                }
                TestimonialAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateTestimonialTooltip();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Automatically generate tooltip text based on current testimonial state.")]
        [DefaultValue(true)]
        public bool AutoGenerateTooltip
        {
            get => _autoGenerateTooltip;
            set
            {
                if (_autoGenerateTooltip != value)
                {
                    _autoGenerateTooltip = value;
                    if (_autoGenerateTooltip)
                    {
                        UpdateTestimonialTooltip();
                    }
                }
            }
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            var cardBounds = DrawingRect;
            var layout = TestimonialLayoutHelpers.CalculateLayout(this, _viewType, cardBounds, Padding);
            
            ClearHitList();
            
            // Get theme colors
            var (backColor, testimonialTextColor, nameColor, detailsColor, ratingColor) = 
                TestimonialThemeHelpers.GetThemeColors(_currentTheme, UseThemeColors);
            
            // Draw based on view type
            switch (_viewType)
            {
                case TestimonialViewType.Classic:
                    DrawClassicView(g, layout, testimonialTextColor, nameColor, detailsColor, ratingColor);
                    break;
                case TestimonialViewType.Minimal:
                    DrawMinimalView(g, layout, testimonialTextColor, nameColor, detailsColor);
                    break;
                case TestimonialViewType.Compact:
                    DrawCompactView(g, layout, testimonialTextColor, nameColor, detailsColor);
                    break;
                case TestimonialViewType.SocialCard:
                    DrawSocialCardView(g, layout, testimonialTextColor, nameColor, detailsColor);
                    break;
            }
        }

        private void DrawClassicView(Graphics g, TestimonialLayout layout, Color testimonialColor, Color nameColor, Color detailsColor, Color ratingColor)
        {
            // Draw rating (using BeepStarRating control)
            if (starRating != null && !layout.RatingBounds.IsEmpty)
            {
                starRating.Location = new Point(layout.RatingBounds.Left, layout.RatingBounds.Top);
                starRating.Size = layout.RatingBounds.Size;
            }
            
            // Draw testimonial text - measure to prevent clipping
            if (!string.IsNullOrEmpty(_testimonial) && !layout.TestimonialBounds.IsEmpty)
            {
                testimonialRect = layout.TestimonialBounds;
                using (var font = TestimonialFontHelpers.GetTestimonialFont(this, ControlStyle, _viewType))
                {
                    SizeF textSizeF = TextUtils.MeasureText(_testimonial, font, int.MaxValue);
                    var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);
                    testimonialRect.Height = Math.Min(textSize.Height, testimonialRect.Height);
                    using (var brush = new SolidBrush(testimonialColor))
                    {
                        TextRenderer.DrawText(g, _testimonial, font, testimonialRect, brush.Color, 
                            TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                    }
                }
            }
            
            // Draw image
            if (!string.IsNullOrEmpty(_imagePath) && !layout.ImageBounds.IsEmpty)
            {
                imageRect = layout.ImageBounds;
                TestimonialIconHelpers.PaintIcon(g, imageRect, _imagePath, _currentTheme, UseThemeColors, null);
                AddHitArea("Image", imageRect, null, () => OnImageClick());
            }
            
            // Draw name - measure to prevent clipping
            if (!string.IsNullOrEmpty(_name) && !layout.NameBounds.IsEmpty)
            {
                nameRect = layout.NameBounds;
                using (var font = TestimonialFontHelpers.GetNameFont(this, ControlStyle, _viewType))
                {
                    SizeF textSizeF = TextUtils.MeasureText(_name, font, int.MaxValue);
                    var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);
                    nameRect.Width = Math.Min(textSize.Width, nameRect.Width);
                    using (var brush = new SolidBrush(nameColor))
                    {
                        TextRenderer.DrawText(g, _name, font, nameRect, brush.Color, 
                            TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                    }
                }
            }
            
            // Draw position - measure to prevent clipping
            if (!string.IsNullOrEmpty(_position) && !layout.PositionBounds.IsEmpty)
            {
                positionRect = layout.PositionBounds;
                using (var font = TestimonialFontHelpers.GetDetailsFont(this, ControlStyle, _viewType))
                {
                    SizeF textSizeF = TextUtils.MeasureText(_position, font, int.MaxValue);
                    var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);
                    positionRect.Width = Math.Min(textSize.Width, positionRect.Width);
                    using (var brush = new SolidBrush(detailsColor))
                    {
                        TextRenderer.DrawText(g, _position, font, positionRect, brush.Color, 
                            TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                    }
                }
            }
            
            // Draw username - measure to prevent clipping
            if (!string.IsNullOrEmpty(_username) && !layout.UsernameBounds.IsEmpty)
            {
                usernameRect = layout.UsernameBounds;
                using (var font = TestimonialFontHelpers.GetDetailsFont(this, ControlStyle, _viewType))
                {
                    SizeF textSizeF = TextUtils.MeasureText(_username, font, int.MaxValue);
                    var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);
                    usernameRect.Width = Math.Min(textSize.Width, usernameRect.Width);
                    using (var brush = new SolidBrush(detailsColor))
                    {
                        TextRenderer.DrawText(g, _username, font, usernameRect, brush.Color, 
                            TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                    }
                }
            }
        }

        private void DrawMinimalView(Graphics g, TestimonialLayout layout, Color testimonialColor, Color nameColor, Color detailsColor)
        {
            // Draw company logo
            if (!string.IsNullOrEmpty(_companyLogoPath) && !layout.CompanyLogoBounds.IsEmpty)
            {
                companyLogoRect = layout.CompanyLogoBounds;
                TestimonialIconHelpers.PaintIcon(g, companyLogoRect, _companyLogoPath, _currentTheme, UseThemeColors, null);
            }
            
            // Draw testimonial text
            if (!string.IsNullOrEmpty(_testimonial) && !layout.TestimonialBounds.IsEmpty)
            {
                testimonialRect = layout.TestimonialBounds;
                using (var font = TestimonialFontHelpers.GetTestimonialFont(this, ControlStyle, _viewType))
                {
                    SizeF textSizeF = TextUtils.MeasureText(_testimonial, font, testimonialRect.Width);
                    var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);
                    testimonialRect.Height = Math.Min(textSize.Height, testimonialRect.Height);
                    using (var brush = new SolidBrush(testimonialColor))
                    {
                        TextRenderer.DrawText(g, _testimonial, font, testimonialRect, brush.Color, 
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                    }
                }
            }
            
            // Draw name, position, username (centered)
            DrawCenteredText(g, _name, layout.NameBounds, nameColor, TestimonialFontHelpers.GetNameFont(this, ControlStyle, _viewType));
            DrawCenteredText(g, _position, layout.PositionBounds, detailsColor, TestimonialFontHelpers.GetDetailsFont(this, ControlStyle, _viewType));
            DrawCenteredText(g, _username, layout.UsernameBounds, detailsColor, TestimonialFontHelpers.GetDetailsFont(this, ControlStyle, _viewType));
            
            // Draw image (centered)
            if (!string.IsNullOrEmpty(_imagePath) && !layout.ImageBounds.IsEmpty)
            {
                imageRect = layout.ImageBounds;
                TestimonialIconHelpers.PaintIcon(g, imageRect, _imagePath, _currentTheme, UseThemeColors, null);
                AddHitArea("Image", imageRect, null, () => OnImageClick());
            }
        }

        private void DrawCompactView(Graphics g, TestimonialLayout layout, Color testimonialColor, Color nameColor, Color detailsColor)
        {
            // Draw testimonial text
            if (!string.IsNullOrEmpty(_testimonial) && !layout.TestimonialBounds.IsEmpty)
            {
                testimonialRect = layout.TestimonialBounds;
                using (var font = TestimonialFontHelpers.GetTestimonialFont(this, ControlStyle, _viewType))
                {
                    SizeF textSizeF = TextUtils.MeasureText(_testimonial, font, testimonialRect.Width);
                    var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);
                    testimonialRect.Height = Math.Min(textSize.Height, testimonialRect.Height);
                    using (var brush = new SolidBrush(testimonialColor))
                    {
                        TextRenderer.DrawText(g, _testimonial, font, testimonialRect, brush.Color, 
                            TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                    }
                }
            }
            
            // Draw image
            if (!string.IsNullOrEmpty(_imagePath) && !layout.ImageBounds.IsEmpty)
            {
                imageRect = layout.ImageBounds;
                TestimonialIconHelpers.PaintIcon(g, imageRect, _imagePath, _currentTheme, UseThemeColors, null);
                AddHitArea("Image", imageRect, null, () => OnImageClick());
            }
            
            // Draw name, position, username
            DrawText(g, _name, layout.NameBounds, nameColor, TestimonialFontHelpers.GetNameFont(this, ControlStyle, _viewType));
            DrawText(g, _position, layout.PositionBounds, detailsColor, TestimonialFontHelpers.GetDetailsFont(this, ControlStyle, _viewType));
            DrawText(g, _username, layout.UsernameBounds, detailsColor, TestimonialFontHelpers.GetDetailsFont(this, ControlStyle, _viewType));
        }

        private void DrawSocialCardView(Graphics g, TestimonialLayout layout, Color testimonialColor, Color nameColor, Color detailsColor)
        {
            // Draw image
            if (!string.IsNullOrEmpty(_imagePath) && !layout.ImageBounds.IsEmpty)
            {
                imageRect = layout.ImageBounds;
                TestimonialIconHelpers.PaintIcon(g, imageRect, _imagePath, _currentTheme, UseThemeColors, null);
                AddHitArea("Image", imageRect, null, () => OnImageClick());
            }
            
            // Draw name, username, position
            DrawText(g, _name, layout.NameBounds, nameColor, TestimonialFontHelpers.GetNameFont(this, ControlStyle, _viewType));
            DrawText(g, _username, layout.UsernameBounds, detailsColor, TestimonialFontHelpers.GetDetailsFont(this, ControlStyle, _viewType));
            DrawText(g, _position, layout.PositionBounds, detailsColor, TestimonialFontHelpers.GetDetailsFont(this, ControlStyle, _viewType));
            
            // Draw testimonial text
            if (!string.IsNullOrEmpty(_testimonial) && !layout.TestimonialBounds.IsEmpty)
            {
                testimonialRect = layout.TestimonialBounds;
                using (var font = TestimonialFontHelpers.GetTestimonialFont(this, ControlStyle, _viewType))
                {
                    SizeF textSizeF = TextUtils.MeasureText(_testimonial, font, testimonialRect.Width);
                    var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);
                    testimonialRect.Height = Math.Min(textSize.Height, testimonialRect.Height);
                    using (var brush = new SolidBrush(testimonialColor))
                    {
                        TextRenderer.DrawText(g, _testimonial, font, testimonialRect, brush.Color, 
                            TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                    }
                }
            }
            
            // Draw close button
            if (!layout.CloseButtonBounds.IsEmpty)
            {
                closeButtonRect = layout.CloseButtonBounds;
                var closeColor = detailsColor;
                if (hoveredArea == "Close")
                {
                    closeColor = Color.FromArgb(200, closeColor);
                }
                using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
                using (var brush = new SolidBrush(closeColor))
                {
                    TextRenderer.DrawText(g, "✕", font, closeButtonRect, brush.Color, 
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                }
                AddHitArea("Close", closeButtonRect, null, () => OnCloseClick());
            }
        }

        private void DrawText(Graphics g, string text, Rectangle bounds, Color color, Font font)
        {
            if (string.IsNullOrEmpty(text) || bounds.IsEmpty) return;
            
            using (font)
            {
                SizeF textSizeF = TextUtils.MeasureText(text, font, int.MaxValue);
                var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);
                bounds.Width = Math.Min(textSize.Width, bounds.Width);
                using (var brush = new SolidBrush(color))
                {
                    TextRenderer.DrawText(g, text, font, bounds, brush.Color, 
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                }
            }
        }

        private void DrawCenteredText(Graphics g, string text, Rectangle bounds, Color color, Font font)
        {
            if (string.IsNullOrEmpty(text) || bounds.IsEmpty) return;
            
            using (font)
            {
                SizeF textSizeF = TextUtils.MeasureText(text, font, int.MaxValue);
                var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);
                int centerX = bounds.Left + bounds.Width / 2;
                bounds = new Rectangle(centerX - textSize.Width / 2, bounds.Top, textSize.Width, bounds.Height);
                using (var brush = new SolidBrush(color))
                {
                    TextRenderer.DrawText(g, text, font, bounds, brush.Color, 
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoPadding);
                }
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_isApplyingTheme) return;

            _isApplyingTheme = true;
            try
            {
                if (_currentTheme == null) return;

                if (UseThemeColors)
                {
                    TestimonialThemeHelpers.ApplyThemeColors(this, _currentTheme, UseThemeColors);
                }

                var (backColor, testimonialTextColor, nameColor, detailsColor, ratingColor) = 
                    TestimonialThemeHelpers.GetThemeColors(_currentTheme, UseThemeColors);

                BackColor = backColor;

                // Sync BeepStarRating theme
                if (starRating != null)
                {
                    starRating.Theme = Theme;
                    starRating.FilledStarColor = ratingColor;
                    starRating.ApplyTheme();
                }

                TestimonialAccessibilityHelpers.ApplyHighContrastAdjustments(this, _currentTheme, UseThemeColors);
            }
            finally
            {
                _isApplyingTheme = false;
            }
            
            Invalidate();
        }

        #region Tooltips
        private void UpdateTestimonialTooltip()
        {
            if (!EnableTooltip || !_autoGenerateTooltip) return;
            GenerateTestimonialTooltip();
        }

        private void GenerateTestimonialTooltip()
        {
            if (!EnableTooltip) return;

            string tooltipText = "";
            string tooltipTitle = !string.IsNullOrEmpty(_name) ? _name : "Testimonial";
            
            if (!string.IsNullOrEmpty(_testimonial))
                tooltipText = _testimonial;
            if (!string.IsNullOrEmpty(_name))
                tooltipText += (string.IsNullOrEmpty(tooltipText) ? "" : "\n") + _name;
            if (!string.IsNullOrEmpty(_position))
                tooltipText += (string.IsNullOrEmpty(tooltipText) ? "" : "\n") + _position;
            if (!string.IsNullOrEmpty(_rating))
                tooltipText += (string.IsNullOrEmpty(tooltipText) ? "" : "\n") + $"Rating: {_rating}/5";

            TooltipText = tooltipText;
            TooltipTitle = tooltipTitle;
            TooltipType = ToolTipType.Info;
            UpdateTooltip();
        }

        public void SetTestimonialTooltip(string text, string title = null, ToolTipType type = ToolTipType.Info)
        {
            TooltipText = text;
            if (!string.IsNullOrEmpty(title))
                TooltipTitle = title;
            TooltipType = type;
            UpdateTooltip();
        }

        public void ShowTestimonialNotification(string message, ToolTipType type = ToolTipType.Info)
        {
            ShowInfo(message, 2000);
        }
        #endregion

        #region Events
        public event EventHandler ImageClick;
        public event EventHandler CompanyLogoClick;
        public event EventHandler CloseClick;
        public event EventHandler TestimonialClick;

        protected virtual void OnImageClick()
        {
            ImageClick?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCompanyLogoClick()
        {
            CompanyLogoClick?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCloseClick()
        {
            Visible = false;
            CloseClick?.Invoke(this, EventArgs.Empty);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (HitTest(e.Location, out var hitTest))
            {
                switch (hitTest.Name)
                {
                    case "Image":
                        OnImageClick();
                        break;
                    case "CompanyLogo":
                        OnCompanyLogoClick();
                        break;
                    case "Close":
                        OnCloseClick();
                        break;
                    default:
                        TestimonialClick?.Invoke(this, EventArgs.Empty);
                        break;
                }
            }
            else
            {
                TestimonialClick?.Invoke(this, EventArgs.Empty);
            }
        }
       

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            string newHoveredArea = null;
            if (HitTest(e.Location, out var hitTest))
            {
                newHoveredArea = hitTest.Name;
            }
            
            if (newHoveredArea != hoveredArea)
            {
                hoveredArea = newHoveredArea;
                Cursor = (hoveredArea == "Image" || hoveredArea == "CompanyLogo" || hoveredArea == "Close") 
                    ? Cursors.Hand 
                    : Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (hoveredArea != null)
            {
                hoveredArea = null;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Enter:
                case Keys.Space:
                    TestimonialClick?.Invoke(this, EventArgs.Empty);
                    e.Handled = true;
                    break;
            }
            if (e.Handled)
            {
                Invalidate();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                starRating?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
