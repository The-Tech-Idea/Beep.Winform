using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Metrices.Helpers;
using TheTechIdea.Beep.Winform.Controls.ToolTips;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Dashboard Metric Tile")]
    [Description("A dashboard tile showing a title, icon, large metric, delta text, and a central silhouette.")]
    public class BeepMetricTile : BaseControl
    {
        // Layout rectangles for hit testing
        private Rectangle titleRect;
        private Rectangle iconRect;
        private Rectangle metricRect;
        private Rectangle deltaRect;
        private Rectangle silhouetteRect;
        
        // Hover states
        private string hoveredArea = null;

        private string _titleText = "Views";
        private string _metricValue = "31";
        private string _deltaValue = "+3 last day";
        private Image _iconImage;              // For the top-right icon
        private Image _backgroundSilhouette;   // For the semi-transparent center image

        private string _iconImagepath;              // For the top-right icon
        private string _backgroundSilhouettepath;   // For the semi-transparent center image
        private bool _isApplyingTheme = false; // Prevent re-entrancy during theme application
        private bool _autoGenerateTooltip = true;

        [Category("Appearance")]
        [Description("Title displayed in the top-left corner (e.g., 'Views').")]
        public string TitleText
        {
            get => _titleText;
            set 
            { 
                _titleText = value;
                MetricTileAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateMetricTileTooltip();
                Invalidate(); 
            }
        }

        [Category("Appearance")]
        [Description("Main numeric or text metric (e.g., '31').")]
        public string MetricValue
        {
            get => _metricValue;
            set 
            { 
                _metricValue = value;
                MetricTileAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateMetricTileTooltip();
                Invalidate(); 
            }
        }

        [Category("Appearance")]
        [Description("Delta or additional info (e.g., '+3 last day').")]
        public string DeltaValue
        {
            get => _deltaValue;
            set 
            { 
                _deltaValue = value;
                MetricTileAccessibilityHelpers.ApplyAccessibilitySettings(this);
                if (_autoGenerateTooltip)
                    UpdateMetricTileTooltip();
                Invalidate(); 
            }
        }

        [Category("Appearance")]
        [Description("Icon shown in the top-right corner (e.g., a home or stats icon).")]
        public string IconImage
        {
            get => _iconImagepath;
            set 
            { 
                _iconImagepath = MetricTileIconHelpers.ResolveIconPath(value, MetricTileIconHelpers.GetRecommendedMetricIcon(_titleText));
                _iconImage = (Image)ImageListHelper.GetImageFromName(_iconImagepath);
                Invalidate(); 
            }
        }

        [Category("Appearance")]
        [Description("A silhouette or watermark image placed in the center with partial transparency.")]
        public string BackgroundSilhouette
        {
            get => _backgroundSilhouettepath;
            set 
            { 
                _backgroundSilhouettepath = MetricTileIconHelpers.ResolveIconPath(value, MetricTileIconHelpers.GetRecommendedSilhouetteIcon(_titleText));
                _backgroundSilhouette = (Image)ImageListHelper.GetImageFromName(_backgroundSilhouettepath);
                Invalidate(); 
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Automatically generate tooltip text based on current tile state.")]
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
                        UpdateMetricTileTooltip();
                    }
                }
            }
        }

        public BeepMetricTile()
        {
            Size = new Size(150, 150);
            BorderRadius = 8;
            ShowShadow = true;
            UseGradientBackground = true;
            GradientDirection = LinearGradientMode.Vertical;
            GradientStartColor = Color.FromArgb(255, 235, 228, 255);
            GradientEndColor = Color.FromArgb(255, 215, 233, 255);
            ForeColor = Color.Black;

            MetricTileAccessibilityHelpers.ApplyAccessibilitySettings(this);
            ApplyTheme();

            if (_autoGenerateTooltip)
            {
                UpdateMetricTileTooltip();
            }
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var drawingRect = DrawingRect;
            var cardTheme = _currentTheme;

            ClearHitList();

            // 1. Draw the background silhouette (center, semi-transparent)
            if (_backgroundSilhouette != null)
            {
                silhouetteRect = MetricTileLayoutHelpers.CalculateSilhouetteBounds(drawingRect, 0.6f);
                MetricTileIconHelpers.PaintSilhouette(g, silhouetteRect, _backgroundSilhouette, 0.20f);
            }

            // 2. Draw the title (top-left) - measure text to prevent clipping
            Size titleSize;
            using (var titleFont = MetricTileFontHelpers.GetTitleFont(this, ControlStyle))
            {
                titleSize = TextRenderer.MeasureText(g, _titleText ?? "", titleFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            }
            titleRect = MetricTileLayoutHelpers.CalculateTitleBounds(drawingRect, Padding);
            titleRect.Width = Math.Min(titleSize.Width, drawingRect.Width - titleRect.Left - Padding.Right - 30); // Account for icon
            
            if (!string.IsNullOrEmpty(_titleText))
            {
                using (var titleFont = MetricTileFontHelpers.GetTitleFont(this, ControlStyle))
                using (var titleBrush = new SolidBrush(MetricTileThemeHelpers.GetTitleColor(cardTheme, UseThemeColors, null)))
                {
                    TextRenderer.DrawText(g, _titleText, titleFont, titleRect, titleBrush.Color, 
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                }
            }

            // 3. Draw the icon (top-right)
            int iconSize = 24;
            iconRect = MetricTileLayoutHelpers.CalculateIconBounds(drawingRect, new Size(iconSize, iconSize), Padding);
            
            if (!string.IsNullOrEmpty(_iconImagepath))
            {
                var iconColor = MetricTileThemeHelpers.GetIconColor(cardTheme, UseThemeColors, null);
                if (hoveredArea == "Icon")
                {
                    iconColor = Color.FromArgb(200, iconColor);
                }
                MetricTileIconHelpers.PaintIcon(g, iconRect, _iconImagepath, cardTheme, UseThemeColors, iconColor);
            }
            else if (_iconImage != null)
            {
                // Draw Image directly
                g.DrawImage(_iconImage, iconRect);
            }
            
            AddHitArea("Icon", iconRect, null, () => OnIconClick());

            // 4. Draw the metric value (bottom-left) - measure text to prevent clipping
            Size metricSize;
            using (var metricFont = MetricTileFontHelpers.GetMetricValueFont(this, ControlStyle))
            {
                metricSize = TextRenderer.MeasureText(g, _metricValue ?? "", metricFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            }
            metricRect = MetricTileLayoutHelpers.CalculateMetricValueBounds(drawingRect, titleRect.Size, Padding);
            metricRect.Width = Math.Min(metricSize.Width, drawingRect.Width - metricRect.Left - Padding.Right);
            
            if (!string.IsNullOrEmpty(_metricValue))
            {
                using (var metricFont = MetricTileFontHelpers.GetMetricValueFont(this, ControlStyle))
                using (var metricBrush = new SolidBrush(MetricTileThemeHelpers.GetMetricValueColor(cardTheme, UseThemeColors, null)))
                {
                    TextRenderer.DrawText(g, _metricValue, metricFont, metricRect, metricBrush.Color, 
                        TextFormatFlags.Left | TextFormatFlags.Bottom | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                }
            }
            
            AddHitArea("Metric", metricRect, null, () => OnTileClick());

            // 5. Draw the delta value (right of metric) - measure text to prevent clipping
            Size deltaSize;
            using (var deltaFont = MetricTileFontHelpers.GetDeltaFont(this, ControlStyle))
            {
                deltaSize = TextRenderer.MeasureText(g, _deltaValue ?? "", deltaFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            }
            deltaRect = MetricTileLayoutHelpers.CalculateDeltaBounds(drawingRect, metricRect.Size, Padding);
            deltaRect.Width = Math.Min(deltaSize.Width, drawingRect.Width - deltaRect.Left - Padding.Right);
            
            if (!string.IsNullOrEmpty(_deltaValue))
            {
                using (var deltaFont = MetricTileFontHelpers.GetDeltaFont(this, ControlStyle))
                using (var deltaBrush = new SolidBrush(MetricTileThemeHelpers.GetDeltaColor(cardTheme, UseThemeColors, null)))
                {
                    TextRenderer.DrawText(g, _deltaValue, deltaFont, deltaRect, deltaBrush.Color, 
                        TextFormatFlags.Left | TextFormatFlags.Bottom | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                }
            }
            
            AddHitArea("Delta", deltaRect, null, () => OnTileClick());
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
                    MetricTileThemeHelpers.ApplyThemeColors(this, _currentTheme, UseThemeColors);
                }
                else
                {
                    BackColor = MetricTileThemeHelpers.GetTileBackColor(_currentTheme, UseThemeColors, null);
                    var (startColor, endColor) = MetricTileThemeHelpers.GetGradientColors(_currentTheme, UseThemeColors, null, null);
                    GradientStartColor = startColor;
                    GradientEndColor = endColor;
                }

                MetricTileAccessibilityHelpers.ApplyHighContrastAdjustments(this, _currentTheme, UseThemeColors);
            }
            finally
            {
                _isApplyingTheme = false;
            }

            Invalidate();
        }

        #region Tooltips
        private void UpdateMetricTileTooltip()
        {
            if (!EnableTooltip || !_autoGenerateTooltip) return;
            GenerateMetricTileTooltip();
        }

        private void GenerateMetricTileTooltip()
        {
            if (!EnableTooltip) return;

            string tooltipText = "";
            string tooltipTitle = !string.IsNullOrEmpty(_titleText) ? _titleText : "Metric Tile";
            ToolTipType tooltipType = ToolTipType.Info;

            if (!string.IsNullOrEmpty(_titleText))
                tooltipText = _titleText;

            if (!string.IsNullOrEmpty(_metricValue))
                tooltipText += (string.IsNullOrEmpty(tooltipText) ? "" : "\n") + $"Value: {_metricValue}";

            if (!string.IsNullOrEmpty(_deltaValue))
            {
                tooltipText += (string.IsNullOrEmpty(tooltipText) ? "" : "\n") + _deltaValue;
                if (_deltaValue.StartsWith("+", StringComparison.OrdinalIgnoreCase))
                    tooltipType = ToolTipType.Success;
                else if (_deltaValue.StartsWith("-", StringComparison.OrdinalIgnoreCase))
                    tooltipType = ToolTipType.Warning;
            }

            TooltipText = tooltipText;
            TooltipTitle = tooltipTitle;
            TooltipType = tooltipType;
            UpdateTooltip();
        }

        public void SetMetricTileTooltip(string text, string title = null, ToolTipType type = ToolTipType.Info)
        {
            TooltipText = text;
            if (!string.IsNullOrEmpty(title))
                TooltipTitle = title;
            TooltipType = type;
            UpdateTooltip();
        }

        public void ShowMetricTileNotification(string message, ToolTipType type = ToolTipType.Info)
        {
            ShowInfo(message, 2000);
        }
        #endregion

        #region Events
        public event EventHandler TileClick;
        public event EventHandler IconClick;

        protected virtual void OnTileClick()
        {
            TileClick?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnIconClick()
        {
            IconClick?.Invoke(this, EventArgs.Empty);
        }
           protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (HitTest(e.Location, out var hitTest))
            {
                switch (hitTest.Name)
                {
                    case "Icon":
                        OnIconClick();
                        break;
                    case "Metric":
                    case "Delta":
                    default:
                        OnTileClick();
                        break;
                }
            }
            else
            {
                OnTileClick();
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
                Cursor = (hoveredArea == "Icon" || hoveredArea == "Metric" || hoveredArea == "Delta") 
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
                    OnTileClick();
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
    }
}
