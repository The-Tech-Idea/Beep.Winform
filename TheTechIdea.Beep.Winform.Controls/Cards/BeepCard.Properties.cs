using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepCard
    {
        #region Enhanced Properties
        [Category("Appearance")]
        [Description("Visual Style of the card layout and design.")]
        public CardStyle CardStyle
        {
            get => _style;
            set 
            { 
                _style = value;
                // Update accessible description when style changes
                AccessibleDescription = $"Card: {value}";
                ApplyDesignTimeData(); // Refresh dummy data when Style changes
                Recompose(); 
            }
        }

        [Category("Appearance")]
        [Description("Accent color used for highlights and accents.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Recompose(); }
        }

        [Category("Behavior")]
        [Description("Indicates whether the card is selected.")]
        [DefaultValue(false)]
        public new bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                SelectionChanged?.Invoke(this, EventArgs.Empty);
                Recompose();
            }
        }

        [Category("Behavior")]
        [Description("Shows a selectable checkbox indicator in the top-left corner.")]
        [DefaultValue(false)]
        public bool ShowSelectionCheckbox
        {
            get => _showSelectionCheckbox;
            set
            {
                if (_showSelectionCheckbox == value) return;
                _showSelectionCheckbox = value;
                Recompose();
            }
        }

        [Category("Behavior")]
        [Description("Shows loading skeleton placeholders and shimmer animation.")]
        [DefaultValue(false)]
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading == value) return;
                _isLoading = value;
                // CRITICAL: Check timer exists (design-time safety)
                if (_loadingTimer != null)
                {
                    if (_isLoading)
                    {
                        _loadingShimmerPhase = 0f;
                        _loadingTimer.Start();
                    }
                    else
                    {
                        _loadingTimer.Stop();
                    }
                }

                Recompose();
            }
        }

        [Category("Appearance")]
        [Description("Height of the accent strip at top of the card.")]
        [DefaultValue(0)]
        public int AccentBarHeight
        {
            get => _accentBarHeight;
            set
            {
                int normalized = Math.Max(0, value);
                if (_accentBarHeight == normalized) return;
                _accentBarHeight = normalized;
                Recompose();
            }
        }

        [Category("Appearance")]
        [Description("Icon path used for the overflow context menu button.")]
        public string ContextMenuIcon
        {
            get => _contextMenuIcon;
            set
            {
                _contextMenuIcon = string.IsNullOrWhiteSpace(value) ? SvgsUI.DotsVertical : value;
                Recompose();
            }
        }

        [Category("Behavior")]
        [Description("Enables collapse/expand affordance in card footer.")]
        [DefaultValue(false)]
        public bool IsCollapsible
        {
            get => _isCollapsible;
            set
            {
                if (_isCollapsible == value) return;
                _isCollapsible = value;
                if (!_isCollapsible && !_isExpanded)
                {
                    _isExpanded = true;
                    StartExpandCollapseAnimation(true);
                }

                Recompose();
            }
        }

        [Category("Behavior")]
        [Description("Current expanded/collapsed state when collapse is enabled.")]
        [DefaultValue(true)]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value) return;

                if (!value && _isExpanded)
                {
                    _rememberedExpandedHeight = Math.Max(Height, _rememberedExpandedHeight);
                }

                _isExpanded = value;
                if (_isCollapsible)
                {
                    StartExpandCollapseAnimation(_isExpanded);
                }
                else
                {
                    Recompose();
                }
            }
        }

        [Category("Content")]
        [Description("Subtitle text displayed below the header.")]
        public string SubtitleText
        {
            get => _subtitleText;
            set { _subtitleText = value; Recompose(); }
        }

        [Category("Content")]
        [Description("The one number the card exists to show - a price, a KPI, a tier.")]
        public string PriceText
        {
            get => _priceText;
            set { _priceText = value; Recompose(); }
        }

        [Category("Content")]
        [Description("Status text (e.g., 'Available for work').")]
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; Recompose(); }
        }

        [Category("Appearance")]
        [Description("Color of the status indicator.")]
        public Color StatusColor
        {
            get => _statusColor;
            set { _statusColor = value; Recompose(); }
        }

        [Category("Appearance")]
        [Description("Whether to show the status indicator.")]
        public bool ShowStatus
        {
            get => _showStatus;
            set { _showStatus = value; Recompose(); }
        }

        [Category("Content")]
        [Description("Rating value (0-5 stars) for testimonial cards.")]
        public int Rating
        {
            get => _rating;
            set { _rating = Math.Max(0, Math.Min(5, value)); Recompose(); }
        }

        [Category("Appearance")]
        [Description("Whether to show the rating stars.")]
        public bool ShowRating
        {
            get => _showRating;
            set { _showRating = value; Recompose(); }
        }

        [Category("Appearance")]
        [Description("Primary badge text, e.g., PRO or FREE.")]
        public string BadgeText1
        {
            get => _badgeText1;
            set { _badgeText1 = value; Recompose(); }
        }

        [Category("Appearance")]
        public Color Badge1BackColor
        {
            get => _badge1BackColor;
            set { _badge1BackColor = value; Recompose(); }
        }

        [Category("Appearance")]
        public Color Badge1ForeColor
        {
            get => _badge1ForeColor;
            set { _badge1ForeColor = value; Recompose(); }
        }

        [Category("Appearance")]
        [Description("Secondary badge text (optional).")]
        public string BadgeText2
        {
            get => _badgeText2;
            set { _badgeText2 = value; Recompose(); }
        }

        [Category("Appearance")]
        public Color Badge2BackColor
        {
            get => _badge2BackColor;
            set { _badge2BackColor = value; Recompose(); }
        }

        [Category("Appearance")]
        public Color Badge2ForeColor
        {
            get => _badge2ForeColor;
            set { _badge2ForeColor = value; Recompose(); }
        }

        [Category("Appearance")]
        [Description("Tags/chips rendered on the card.")]
        public List<string> Tags
        {
            get => _tags;
            set { _tags = value ?? new List<string>(); Recompose(); }
        }

        [Category("Appearance")]
        [Description("Text for secondary button.")]
        public string SecondaryButtonText
        {
            get => secondaryButtonText;
            set { secondaryButtonText = value; Recompose(); }
        }

        [Category("Appearance")]
        [Description("Whether to show the secondary button.")]
        public bool ShowSecondaryButton
        {
            get => showSecondaryButton;
            set { showSecondaryButton = value; Recompose(); }
        }

        [Category("Appearance")]
        [Description("Text displayed as the header of the card.")]
        public string HeaderText
        {
            get => headerText;
            set { headerText = value; Recompose(); }
        }

        [Category("Appearance")]
        [Description("Text displayed as the paragraph of the card.")]
        public string ParagraphText
        {
            get => paragraphText;
            set { paragraphText = value; Recompose(); }
        }

        [Category("Appearance")]
        [Description("Text displayed on the primary action button.")]
        public string ButtonText
        {
            get => buttonText;
            set { buttonText = value; Recompose(); }
        }

        [Category("Appearance")]
        [Description("Determines whether the primary action button is visible.")]
        public bool ShowButton
        {
            get => showButton;
            set { showButton = value; Recompose(); }
        }

        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        public string ImagePath
        {
            get => imagePath;
            set
            {
                imagePath = value;
                Recompose();
            }
        }

        [Category("Appearance")]
        [Description("The alignment of the header text.")]
        public ContentAlignment HeaderAlignment
        {
            get => headerAlignment;
            set { headerAlignment = value; Recompose(); }
        }

        [Category("Appearance")]
        [Description("The alignment of the image (Style-specific usage).")]
        public ContentAlignment ImageAlignment
        {
            get => imageAlignment;
            set { imageAlignment = value; Recompose(); }
        }

        [Category("Appearance")]
        [Description("The alignment of the paragraph text.")]
        public ContentAlignment TextAlignment
        {
            get => textAlignment;
            set { textAlignment = value; Recompose(); }
        }

        /// <summary>
        /// Accessible name for screen readers
        /// </summary>
        [Browsable(true)]
        [Category("Accessibility")]
        [Description("Name of the control for accessibility and screen readers.")]
        public new string AccessibleName
        {
            get => base.AccessibleName;
            set
            {
                base.AccessibleName = value;
                Recompose();
            }
        }

        /// <summary>
        /// Accessible description for screen readers
        /// </summary>
        [Browsable(true)]
        [Category("Accessibility")]
        [Description("Description of the control for accessibility and screen readers.")]
        public new string AccessibleDescription
        {
            get => base.AccessibleDescription;
            set
            {
                base.AccessibleDescription = value;
                Recompose();
            }
        }
        #endregion
    }
}
