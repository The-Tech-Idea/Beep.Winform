using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices.JavaScript;
using System.Windows;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.StatusCards.Helpers;
using TheTechIdea.Beep.Winform.Controls.ToolTips;

namespace TheTechIdea.Beep.Winform.Controls.StatusCards
{
    public partial class BeepStatCard : BaseControl
    {
        #region Backing fields
        private string headerText = "Total Revenue";
        private string percentageText = "+12.5%";
        private string valueText = "$1,250.00";
        private string trendText = "Trending up this month";
        private string infoText = "Visitors for the last 6 months";
        private bool isTrendingUp = true;
        private string trendUpSvgPath = "trendup.svg";
        private string trendDownSvgPath = "trenddown.svg";
        private string cardiconSvgPath = "simpleinfoapps.svg";
        #endregion
        private bool _useThemeColors = true;
        private bool _isApplyingTheme = false; // Prevent re-entrancy during theme application
        private bool _autoGenerateTooltip = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use theme colors instead of custom accent color.")]
        [DefaultValue(true)]
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set
            {
                _useThemeColors = value;
                Invalidate();
            }
        }
        private BeepControlStyle _style = BeepControlStyle.Material3;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual Style/painter to use for rendering the sidebar.")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle Style
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;
                
                    Invalidate();
                }
            }
        }
        public BeepStatCard()
        {
            IsChild = false;
            Padding = new Padding(10);
            BoundProperty = "ValueText";
            Size = new System.Drawing.Size(300, 150);
            ApplyThemeToChilds = false;

            // Apply accessibility settings
            StatCardAccessibilityHelpers.ApplyAccessibilitySettings(this);

            ApplyTheme();

            // Update tooltip if auto-generate is enabled
            if (_autoGenerateTooltip)
            {
                UpdateStatCardTooltip();
            }

            // default demo parameters for design-time view
            Parameters[ParamHeader] = headerText;
            Parameters[ParamValue] = valueText;
            Parameters[ParamDelta] = percentageText;
            Parameters[ParamInfo] = infoText;
            Parameters[ParamSpark] = new float[] { 2, 3, 2.5f, 3.4f, 3.0f, 3.8f, 4.2f };
        }

        #region Properties
        [Category("Appearance"), Description("The header text displayed at the top of the card.")]
        public string HeaderText 
        { 
            get => headerText; 
            set 
            { 
                headerText = value;
                // Update accessibility attributes
                StatCardAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateStatCardTooltip();
                Invalidate(); 
            } 
        }

        [Category("Appearance"), Description("The percentage change text displayed at the top right.")]
        public string PercentageText 
        { 
            get => percentageText; 
            set 
            { 
                percentageText = value;
                // Update accessibility attributes
                StatCardAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateStatCardTooltip();
                Invalidate(); 
            } 
        }

        [Category("Appearance"), Description("The main value displayed in the card.")]
        public string ValueText 
        { 
            get => valueText; 
            set 
            { 
                valueText = value;
                // Update accessibility attributes
                StatCardAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateStatCardTooltip();
                Invalidate(); 
            } 
        }

        [Category("Appearance"), Description("The trend text displayed below the main value.")]
        public string TrendText 
        { 
            get => trendText; 
            set 
            { 
                trendText = value;
                if (_autoGenerateTooltip)
                    UpdateStatCardTooltip();
                Invalidate(); 
            } 
        }

        [Category("Appearance"), Description("Additional information displayed at the bottom of the card.")]
        public string InfoText 
        { 
            get => infoText; 
            set 
            { 
                infoText = value;
                // Update accessibility attributes
                StatCardAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateStatCardTooltip();
                Invalidate(); 
            } 
        }

        [Category("Appearance"), Description("Indicates whether the trend is up (true) or down (false).")]
        public bool IsTrendingUp { get => isTrendingUp; set { isTrendingUp = value; Invalidate(); } }

        [Browsable(true), Category("Appearance"), Description("Path to the SVG file for the upward trend icon."), Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string TrendUpSvgPath 
        { 
            get => trendUpSvgPath; 
            set 
            { 
                trendUpSvgPath = StatCardIconHelpers.ResolveIconPath(value, StatCardIconHelpers.GetRecommendedTrendUpIcon());
                Invalidate(); 
            } 
        }

        [Browsable(true), Category("Appearance"), Description("Path to the SVG file for the downward trend icon."), Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string TrendDownSvgPath 
        { 
            get => trendDownSvgPath; 
            set 
            { 
                trendDownSvgPath = StatCardIconHelpers.ResolveIconPath(value, StatCardIconHelpers.GetRecommendedTrendDownIcon());
                Invalidate(); 
            } 
        }

        [Browsable(true), Category("Appearance"), Description("Path to the SVG file for the Card Icon icon."), Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Icon 
        { 
            get => cardiconSvgPath; 
            set 
            { 
                cardiconSvgPath = StatCardIconHelpers.ResolveIconPath(value, StatCardIconHelpers.GetRecommendedCardIcon(headerText));
                Invalidate(); 
            } 
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Automatically generate tooltip text based on current card state.")]
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
                        UpdateStatCardTooltip();
                    }
                }
            }
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_isApplyingTheme) return;

            _isApplyingTheme = true;
            try
            {
                if (_currentTheme == null) return;

                // Use theme helpers for centralized color management
                if (UseThemeColors)
                {
                    StatCardThemeHelpers.ApplyThemeColors(this, _currentTheme, UseThemeColors);
                }
                else
                {
                    // Apply default colors
                    BackColor = StatCardThemeHelpers.GetCardBackColor(_currentTheme, UseThemeColors, null);
                    ParentBackColor = StatCardThemeHelpers.GetCardBackColor(_currentTheme, UseThemeColors, null);
                }

                // Apply high contrast adjustments if needed
                StatCardAccessibilityHelpers.ApplyHighContrastAdjustments(this, _currentTheme, UseThemeColors);
            }
            finally
            {
                _isApplyingTheme = false;
            }

            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            // Let BaseControl paint background via PaintInnerShape already called in OnPaint pipeline.
            var painter = GetActivePainter();
            if (painter == null) return;

            // Determine drawing area: use BaseControl.DrawingRect and honour Padding
            var rect = DrawingRect;
            rect = new Rectangle(rect.X + Padding.Left, rect.Y + Padding.Top, Math.Max(0, rect.Width - Padding.Horizontal), Math.Max(0, rect.Height - Padding.Vertical));
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Push some implicit common parameters if not present
            if (!Parameters.ContainsKey(ParamHeader) && !string.IsNullOrEmpty(HeaderText)) Parameters[ParamHeader] = HeaderText;
            if (!Parameters.ContainsKey(ParamValue) && !string.IsNullOrEmpty(ValueText)) Parameters[ParamValue] = ValueText;
            if (!Parameters.ContainsKey(ParamDelta) && !string.IsNullOrEmpty(PercentageText)) Parameters[ParamDelta] = PercentageText;
            if (!Parameters.ContainsKey(ParamInfo) && !string.IsNullOrEmpty(InfoText)) Parameters[ParamInfo] = InfoText;
            if (UseThemeColors && _currentTheme != null)
            {
                BackColor = _currentTheme.CardBackColor;
                g.Clear(BackColor);
            }
            else
            {
                // Paint background based on selected Style
                BeepStyling.PaintStyleBackground(g, DrawingRect, Style);
            }
            painter.Paint(g, rect, _currentTheme, this, Parameters);
        }

        #region Tooltips
        /// <summary>
        /// Update tooltip based on current state
        /// Called automatically when AutoGenerateTooltip is enabled
        /// </summary>
        private void UpdateStatCardTooltip()
        {
            if (!EnableTooltip)
                return;

            if (_autoGenerateTooltip)
            {
                GenerateStatCardTooltip();
            }
        }

        /// <summary>
        /// Generate tooltip text based on current card state
        /// </summary>
        private void GenerateStatCardTooltip()
        {
            if (!EnableTooltip)
                return;

            string tooltipText = "";
            string tooltipTitle = !string.IsNullOrEmpty(headerText) ? headerText : "Stat Card";
            ToolTipType tooltipType = ToolTipType.Info;

            if (!string.IsNullOrEmpty(headerText))
            {
                tooltipText = headerText;
            }

            if (!string.IsNullOrEmpty(valueText))
            {
                if (!string.IsNullOrEmpty(tooltipText))
                    tooltipText += "\n";
                tooltipText += $"Value: {valueText}";
            }

            if (!string.IsNullOrEmpty(percentageText))
            {
                if (!string.IsNullOrEmpty(tooltipText))
                    tooltipText += "\n";
                tooltipText += percentageText;
            }

            if (!string.IsNullOrEmpty(infoText))
            {
                if (!string.IsNullOrEmpty(tooltipText))
                    tooltipText += "\n";
                tooltipText += infoText;
            }

            // Determine tooltip type based on trend
            if (isTrendingUp)
            {
                tooltipType = ToolTipType.Success;
            }
            else
            {
                tooltipType = ToolTipType.Warning;
            }

            TooltipText = tooltipText;
            TooltipTitle = tooltipTitle;
            TooltipType = tooltipType;
            UpdateTooltip();
        }

        /// <summary>
        /// Set stat card tooltip with custom text, title, and type
        /// </summary>
        public void SetStatCardTooltip(string text, string title = null, ToolTipType type = ToolTipType.Info)
        {
            TooltipText = text;
            if (!string.IsNullOrEmpty(title))
            {
                TooltipTitle = title;
            }
            TooltipType = type;
            UpdateTooltip();
        }

        /// <summary>
        /// Show stat card notification
        /// </summary>
        public void ShowStatCardNotification(string message, ToolTipType type = ToolTipType.Info)
        {
            ShowInfo(message, 2000);
        }
        #endregion

        #region Events
        /// <summary>
        /// Event raised when the card is clicked
        /// </summary>
        public event EventHandler CardClick;

        /// <summary>
        /// Raises the CardClick event
        /// </summary>
        protected virtual void OnCardClick()
        {
            CardClick?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            OnCardClick();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Keyboard navigation
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

        public override string ToString()
        {
            return GetType().Name.Replace("Control", string.Empty).Replace("Beep", "Beep ");
        }
    }
}
