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
            ApplyTheme();

            // default demo parameters for design-time view
            Parameters[ParamHeader] = headerText;
            Parameters[ParamValue] = valueText;
            Parameters[ParamDelta] = percentageText;
            Parameters[ParamInfo] = infoText;
            Parameters[ParamSpark] = new float[] { 2, 3, 2.5f, 3.4f, 3.0f, 3.8f, 4.2f };
        }

        #region Properties
        [Category("Appearance"), Description("The header text displayed at the top of the card.")]
        public string HeaderText { get => headerText; set { headerText = value; Invalidate(); } }

        [Category("Appearance"), Description("The percentage change text displayed at the top right.")]
        public string PercentageText { get => percentageText; set { percentageText = value; Invalidate(); } }

        [Category("Appearance"), Description("The main value displayed in the card.")]
        public string ValueText { get => valueText; set { valueText = value; Invalidate(); } }

        [Category("Appearance"), Description("The trend text displayed below the main value.")]
        public string TrendText { get => trendText; set { trendText = value; Invalidate(); } }

        [Category("Appearance"), Description("Additional information displayed at the bottom of the card.")]
        public string InfoText { get => infoText; set { infoText = value; Invalidate(); } }

        [Category("Appearance"), Description("Indicates whether the trend is up (true) or down (false).")]
        public bool IsTrendingUp { get => isTrendingUp; set { isTrendingUp = value; Invalidate(); } }

        [Browsable(true), Category("Appearance"), Description("Path to the SVG file for the upward trend icon."), Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string TrendUpSvgPath { get => trendUpSvgPath; set { trendUpSvgPath = value; Invalidate(); } }

        [Browsable(true), Category("Appearance"), Description("Path to the SVG file for the downward trend icon."), Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string TrendDownSvgPath { get => trendDownSvgPath; set { trendDownSvgPath = value; Invalidate(); } }

        [Browsable(true), Category("Appearance"), Description("Path to the SVG file for the Card Icon icon."), Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Icon { get => cardiconSvgPath; set { cardiconSvgPath = value; Invalidate(); } }
        #endregion

        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;
            BackColor = _currentTheme.CardBackColor;
            ParentBackColor = _currentTheme.CardBackColor;
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

        public override string ToString()
        {
            return GetType().Name.Replace("Control", string.Empty).Replace("Beep", "Beep ");
        }
    }
}
