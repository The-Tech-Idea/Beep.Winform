using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Features.Helpers;
using TheTechIdea.Beep.Winform.Controls.ToolTips;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Feature Card")]
    [Category("Beep Controls")]
    [Description("A card control that displays a list of features with a logo and title using BeepListBox.")]
    public class BeepFeatureCard : BaseControl
    {
        #region "Fields"
        private BeepListBox featuresListBox;
        
        // Layout rectangles for hit testing
        private Rectangle logoRect;
        private Rectangle titleRect;
        private Rectangle subtitleRect;
        private Rectangle actionIcon1Rect;
        private Rectangle actionIcon2Rect;
        private Rectangle cardIconRect;
        
        // Hover states
        private string hoveredArea = null;
        
        private string logoPath = "";
        private string titleText = "Sphere UI";
        private string subtitleText = "Charts version";
        private List<SimpleItem> bulletPoints = new List<SimpleItem>
        {
            new SimpleItem { Text = "80+ combined charts in 4 layouts.", ItemType = Vis.Modules.MenuItemType.Main },
            new SimpleItem { Text = "300+ components.", ItemType = Vis.Modules.MenuItemType.Main },
            new SimpleItem { Text = "10+ pages in 2 color schemes: blue & green.", ItemType = Vis.Modules.MenuItemType.Main },
            new SimpleItem { Text = "Autolayout for all elements & components.", ItemType = Vis.Modules.MenuItemType.Main },
            new SimpleItem { Text = "Fully connected library of fonts & color.", ItemType = Vis.Modules.MenuItemType.Main }
        };
        private string bulletIconPath = "bullet.svg";
        private string actionIcon1Path = "action1.svg";
        private string actionIcon2Path = "action2.svg";
        private string cardIconPath = "simpleinfoapps.svg";
        private bool _isApplyingTheme = false;
        private bool _autoGenerateTooltip = false;
        private ListBoxType _listStyle = ListBoxType.Standard;
        #endregion "Fields"

        #region "Properties"
        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the logo image (SVG, PNG, etc.).")]
        public string LogoPath
        {
            get => logoPath;
            set
            {
                logoPath = FeatureCardIconHelpers.ResolveIconPath(value, FeatureCardIconHelpers.GetRecommendedLogoIcon());
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("The title text displayed at the top of the card.")]
        public string TitleText
        {
            get => titleText;
            set
            {
                titleText = value;
                FeatureCardAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateFeatureCardTooltip();
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("The subtitle text displayed below the title.")]
        public string SubtitleText
        {
            get => subtitleText;
            set
            {
                subtitleText = value;
                if (_autoGenerateTooltip)
                    UpdateFeatureCardTooltip();
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("The list of bullet points displayed on the card.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<SimpleItem> BulletPoints
        {
            get => bulletPoints;
            set
            {
                bulletPoints = value;
                UpdateFeaturesList();
                FeatureCardAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateFeatureCardTooltip();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the bullet icon SVG.")]
        public string BulletIconPath
        {
            get => bulletIconPath;
            set
            {
                bulletIconPath = FeatureCardIconHelpers.ResolveIconPath(value, FeatureCardIconHelpers.GetRecommendedBulletIcon());
                UpdateFeaturesList();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the first action icon SVG (top right).")]
        public string ActionIcon1Path
        {
            get => actionIcon1Path;
            set
            {
                actionIcon1Path = FeatureCardIconHelpers.ResolveIconPath(value, FeatureCardIconHelpers.GetRecommendedActionIcon(0));
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the second action icon SVG (top right).")]
        public string ActionIcon2Path
        {
            get => actionIcon2Path;
            set
            {
                actionIcon2Path = FeatureCardIconHelpers.ResolveIconPath(value, FeatureCardIconHelpers.GetRecommendedActionIcon(1));
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the card icon SVG (top right).")]
        public string CardIconPath
        {
            get => cardIconPath;
            set
            {
                cardIconPath = FeatureCardIconHelpers.ResolveIconPath(value, FeatureCardIconHelpers.GetRecommendedCardIcon());
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The list style for the features list box.")]
        [DefaultValue(ListBoxType.Standard)]
        public ListBoxType ListStyle
        {
            get => _listStyle;
            set
            {
                if (_listStyle != value)
                {
                    _listStyle = value;
                    if (featuresListBox != null)
                    {
                        featuresListBox.ListBoxType = value;
                    }
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Automatically generate tooltip text based on current card state.")]
        [DefaultValue(false)]
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
                        UpdateFeatureCardTooltip();
                    }
                }
            }
        }
        #endregion "Properties"

        #region Events
        public event EventHandler ActionIcon1Click;
        public event EventHandler ActionIcon2Click;
        public event EventHandler CardClick;
        public event EventHandler CardIconClick;

        protected virtual void OnActionIcon1Click()
        {
            ActionIcon1Click?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnActionIcon2Click()
        {
            ActionIcon2Click?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCardClick()
        {
            CardClick?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCardIconClick()
        {
            CardIconClick?.Invoke(this, EventArgs.Empty);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            // Check hit areas
            if (HitTest(e.Location, out var hitTest))
            {
                switch (hitTest.Name)
                {
                    case "ActionIcon1":
                        OnActionIcon1Click();
                        break;
                    case "ActionIcon2":
                        OnActionIcon2Click();
                        break;
                    case "CardIcon":
                        OnCardIconClick();
                        break;
                    default:
                        OnCardClick();
                        break;
                }
            }
            else
            {
                OnCardClick();
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
                Cursor = (hoveredArea == "ActionIcon1" || hoveredArea == "ActionIcon2" || hoveredArea == "CardIcon") 
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
                    OnCardClick();
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

        public BeepFeatureCard()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            IsChild = false;
            Padding = new Padding(10);
            BoundProperty = "TitleText";
            Size = new Size(300, 200);
            ApplyThemeToChilds = false;

            // Initialize BeepListBox
            featuresListBox = new BeepListBox
            {
                Theme = Theme,
                IsFrameless = true,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ShowImage = true,
                ShowCheckBox = false,
                ShowHilightBox = true,
                MenuItemHeight = 30,
                ImageSize = 20,
                ListBoxType = _listStyle
            };
            UpdateFeaturesList();
            Controls.Add(featuresListBox);

            FeatureCardAccessibilityHelpers.ApplyAccessibilitySettings(this);
            ApplyTheme();

            if (_autoGenerateTooltip)
            {
                UpdateFeatureCardTooltip();
            }
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            ApplyTheme();
            Invalidate();
        }

        private void UpdateFeaturesList()
        {
            if (featuresListBox == null) return;
            
            foreach (var item in bulletPoints)
            {
                if (string.IsNullOrEmpty(item.ImagePath))
                {
                    item.ImagePath = FeatureCardIconHelpers.ResolveIconPath(bulletIconPath, FeatureCardIconHelpers.GetRecommendedBulletIcon());
                }
                if (item.ItemType != Vis.Modules.MenuItemType.Main)
                {
                    item.ItemType = Vis.Modules.MenuItemType.Main;
                }
            }

            var simpleItems = new BindingList<SimpleItem>(bulletPoints);
            featuresListBox.ListItems = simpleItems;
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            var clientRect = DrawingRect;
            int padding = Padding.Left;
            int logoSize = 24;
            int iconSize = 32;
            int cardIconSize = 64;
            
            // Calculate layout
            logoRect = new Rectangle(clientRect.Left + padding, clientRect.Top + padding, logoSize, logoSize);
            
            // Title and subtitle - measure text to prevent clipping
            Size titleSize;
            Size subtitleSize;
            using (var titleFont = FeatureCardFontHelpers.GetTitleFont(this, ControlStyle))
            {
                titleSize = TextRenderer.MeasureText(g, titleText ?? "", titleFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            }
            
            using (var subtitleFont = FeatureCardFontHelpers.GetSubtitleFont(this, ControlStyle))
            {
                subtitleSize = TextRenderer.MeasureText(g, subtitleText ?? "", subtitleFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            }
            
            // Calculate available width for text (accounting for icons on right)
            int availableTextWidth = clientRect.Width - logoRect.Right - 10 - (iconSize * 3) - 10 - padding;
            titleRect = new Rectangle(logoRect.Right + 10, clientRect.Top + padding, 
                Math.Min(titleSize.Width, Math.Max(0, availableTextWidth)), titleSize.Height);
            subtitleRect = new Rectangle(titleRect.Left, titleRect.Bottom + 2, 
                Math.Min(subtitleSize.Width, Math.Max(0, availableTextWidth)), subtitleSize.Height);
            
            // Icons at top right
            cardIconRect = new Rectangle(clientRect.Right - padding - cardIconSize, clientRect.Top + padding, cardIconSize, cardIconSize);
            actionIcon2Rect = new Rectangle(cardIconRect.Left - 5 - iconSize, clientRect.Top + padding, iconSize, iconSize);
            actionIcon1Rect = new Rectangle(actionIcon2Rect.Left - 5 - iconSize, clientRect.Top + padding, iconSize, iconSize);
            
            // Draw logo
            if (!string.IsNullOrEmpty(logoPath))
            {
                var logoColor = FeatureCardThemeHelpers.GetActionIconColor(_currentTheme, UseThemeColors, null);
                FeatureCardIconHelpers.PaintIcon(g, logoRect, logoPath, _currentTheme, UseThemeColors, logoColor);
            }
            
            // Draw title
            if (!string.IsNullOrEmpty(titleText))
            {
                using (var titleFont = FeatureCardFontHelpers.GetTitleFont(this, ControlStyle))
                using (var titleBrush = new SolidBrush(FeatureCardThemeHelpers.GetTitleColor(_currentTheme, UseThemeColors, null)))
                {
                    TextRenderer.DrawText(g, titleText, titleFont, titleRect, titleBrush.Color, 
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                }
            }
            
            // Draw subtitle
            if (!string.IsNullOrEmpty(subtitleText))
            {
                using (var subtitleFont = FeatureCardFontHelpers.GetSubtitleFont(this, ControlStyle))
                using (var subtitleBrush = new SolidBrush(FeatureCardThemeHelpers.GetSubtitleColor(_currentTheme, UseThemeColors, null)))
                {
                    TextRenderer.DrawText(g, subtitleText, subtitleFont, subtitleRect, subtitleBrush.Color, 
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                }
            }
            
            // Draw action icons
            var actionIconColor = FeatureCardThemeHelpers.GetActionIconColor(_currentTheme, UseThemeColors, null);
            if (hoveredArea == "ActionIcon1")
            {
                actionIconColor = Color.FromArgb(200, actionIconColor);
            }
            if (!string.IsNullOrEmpty(actionIcon1Path))
            {
                FeatureCardIconHelpers.PaintIcon(g, actionIcon1Rect, actionIcon1Path, _currentTheme, UseThemeColors, actionIconColor);
            }
            
            actionIconColor = FeatureCardThemeHelpers.GetActionIconColor(_currentTheme, UseThemeColors, null);
            if (hoveredArea == "ActionIcon2")
            {
                actionIconColor = Color.FromArgb(200, actionIconColor);
            }
            if (!string.IsNullOrEmpty(actionIcon2Path))
            {
                FeatureCardIconHelpers.PaintIcon(g, actionIcon2Rect, actionIcon2Path, _currentTheme, UseThemeColors, actionIconColor);
            }
            
            // Draw card icon
            var cardIconColor = FeatureCardThemeHelpers.GetCardIconColor(_currentTheme, UseThemeColors, null);
            if (hoveredArea == "CardIcon")
            {
                cardIconColor = Color.FromArgb(200, cardIconColor);
            }
            if (!string.IsNullOrEmpty(cardIconPath))
            {
                FeatureCardIconHelpers.PaintIcon(g, cardIconRect, cardIconPath, _currentTheme, UseThemeColors, cardIconColor);
            }
            
            // Position and size BeepListBox
            int listTop = subtitleRect.Bottom + 10;
            int listHeight = featuresListBox.GetMaxHeight();
            featuresListBox.Location = new Point(clientRect.Left + padding, listTop);
            featuresListBox.Size = new Size(Math.Max(0, clientRect.Width - padding * 2), listHeight);
            
            // Register hit areas
            ClearHitList();
            AddHitArea("ActionIcon1", actionIcon1Rect, null, () => OnActionIcon1Click());
            AddHitArea("ActionIcon2", actionIcon2Rect, null, () => OnActionIcon2Click());
            AddHitArea("CardIcon", cardIconRect, null, () => OnCardIconClick());
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
                    FeatureCardThemeHelpers.ApplyThemeColors(this, _currentTheme, UseThemeColors);
                }

                var (backColor, titleColor, subtitleColor, bulletColor, actionIconColor, cardIconColor) = 
                    FeatureCardThemeHelpers.GetThemeColors(_currentTheme, UseThemeColors);

                BackColor = backColor;
                ParentBackColor = backColor;

                // Sync BeepListBox theme
                if (featuresListBox != null)
                {
                    featuresListBox.Theme = Theme;
                    featuresListBox.BackColor = backColor;
                    featuresListBox.TextFont = FeatureCardFontHelpers.GetBulletPointFont(this, ControlStyle);
                    featuresListBox.ForeColor = bulletColor;
                    featuresListBox.ListBoxType = _listStyle;
                    featuresListBox.Invalidate();
                }

                FeatureCardAccessibilityHelpers.ApplyHighContrastAdjustments(this, _currentTheme, UseThemeColors);
            }
            finally
            {
                _isApplyingTheme = false;
            }
            
            Invalidate();
        }

        #region Tooltips
        private void UpdateFeatureCardTooltip()
        {
            if (!EnableTooltip || !_autoGenerateTooltip) return;
            GenerateFeatureCardTooltip();
        }

        private void GenerateFeatureCardTooltip()
        {
            if (!EnableTooltip) return;

            string tooltipText = "";
            string tooltipTitle = !string.IsNullOrEmpty(titleText) ? titleText : "Feature Card";
            
            if (!string.IsNullOrEmpty(titleText))
                tooltipText = titleText;
            if (!string.IsNullOrEmpty(subtitleText))
                tooltipText += (string.IsNullOrEmpty(tooltipText) ? "" : "\n") + subtitleText;
            if (bulletPoints != null && bulletPoints.Count > 0)
            {
                tooltipText += (string.IsNullOrEmpty(tooltipText) ? "" : "\n\n") + $"Features ({bulletPoints.Count}):";
                int maxFeatures = Math.Min(3, bulletPoints.Count);
                for (int i = 0; i < maxFeatures; i++)
                {
                    if (!string.IsNullOrEmpty(bulletPoints[i].Text))
                        tooltipText += $"\n• {bulletPoints[i].Text}";
                }
                if (bulletPoints.Count > maxFeatures)
                    tooltipText += $"\n... and {bulletPoints.Count - maxFeatures} more";
            }

            TooltipText = tooltipText;
            TooltipTitle = tooltipTitle;
            TooltipType = ToolTipType.Info;
            UpdateTooltip();
        }

        public void SetFeatureCardTooltip(string text, string title = null, ToolTipType type = ToolTipType.Info)
        {
            TooltipText = text;
            if (!string.IsNullOrEmpty(title))
                TooltipTitle = title;
            TooltipType = type;
            UpdateTooltip();
        }

        public void ShowFeatureCardNotification(string message, ToolTipType type = ToolTipType.Info)
        {
            ShowInfo(message, 2000);
        }
        #endregion

        public override string ToString()
        {
            return GetType().Name.Replace("Control", "").Replace("Beep", "Beep ");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                featuresListBox?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
