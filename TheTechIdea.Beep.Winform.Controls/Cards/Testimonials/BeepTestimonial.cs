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
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum TestimonialViewType
    {
        Classic,
        Minimal,
        Compact,
        SocialCard
    }

    public partial class BeepTestimonial : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.CardTestimonial;
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
        
        // Hover states

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
            TestimonialAccessibilityHelpers.ApplyAccessibilitySettings(this);
            Compose();

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
                Recompose();
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
                Recompose();
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
                Recompose();
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
                Recompose();
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
                Recompose();
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
                Recompose();
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
                Recompose();
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
                Recompose();
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

        // DrawContent and the four Draw*View routines are removed: the card is composed from controls
        // in BeepTestimonial.Composition.cs. Each view type is a different call order over one set of
        // controls, rather than four paint routines measuring and centring text by hand.
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

        protected virtual void OnTestimonialClick()
        {
            TestimonialClick?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCloseClick()
        {
            Visible = false;
            CloseClick?.Invoke(this, EventArgs.Empty);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        // The hover tracking that lived here compared the mouse against painted rectangles. The avatar,
        // the quote and the dismiss button are controls now, each tracking its own hover and cursor.
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Enter:
                case Keys.Space:
                    OnTestimonialClick();
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
