using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Badges;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Base
{
    public partial class BaseControl
    {
        internal ValidationState _validationIcon = ValidationState.None;
        internal BadgeAnchor _validationIconPosition = BadgeAnchor.TopRight;
        internal int _validationIconSize = 16;
        internal bool _showIndicatorLine;
        internal IndicatorLineStyle _indicatorLineStyle = IndicatorLineStyle.Solid;
        internal Color _indicatorLineColor = Color.Red;
        internal int _indicatorLineThickness = 2;

        internal string _customIconPath = string.Empty;
        internal BadgeAnchor _customIconPosition = BadgeAnchor.TopRight;
        internal int _customIconSize = 16;
        internal Color _customIconColor = Color.Black;

        [Browsable(true), Category("Validation")]
        public ValidationState ValidationIcon { get => _validationIcon; set { if (_validationIcon == value) return; _validationIcon = value; UpdateExternalDrawing(); } }

        [Browsable(true), Category("Validation")]
        public BadgeAnchor ValidationIconPosition { get => _validationIconPosition; set { if (_validationIconPosition == value) return; _validationIconPosition = value; UpdateExternalDrawing(); } }

        [Browsable(true), Category("Validation"), DefaultValue(16)]
        public int ValidationIconSize { get => _validationIconSize; set { int c = Math.Max(8, Math.Min(32, value)); if (_validationIconSize == c) return; _validationIconSize = c; UpdateExternalDrawing(); } }

        [Browsable(true), Category("Validation")]
        public bool ShowIndicatorLine { get => _showIndicatorLine; set { if (_showIndicatorLine == value) return; _showIndicatorLine = value; UpdateExternalDrawing(); } }

        [Browsable(true), Category("Validation")]
        public IndicatorLineStyle IndicatorLineStyle { get => _indicatorLineStyle; set { if (_indicatorLineStyle == value) return; _indicatorLineStyle = value; UpdateExternalDrawing(); } }

        [Browsable(true), Category("Validation")]
        public Color IndicatorLineColor { get => _indicatorLineColor; set { if (_indicatorLineColor == value) return; _indicatorLineColor = value; UpdateExternalDrawing(); } }

        [Browsable(true), Category("Validation"), DefaultValue(2)]
        public int IndicatorLineThickness { get => _indicatorLineThickness; set { int c = Math.Max(1, Math.Min(6, value)); if (_indicatorLineThickness == c) return; _indicatorLineThickness = c; UpdateExternalDrawing(); } }

        [Browsable(true), Category("External Icon")]
        public string CustomIconPath { get => _customIconPath; set { _customIconPath = value ?? string.Empty; UpdateExternalDrawing(); } }

        [Browsable(true), Category("External Icon")]
        public BadgeAnchor CustomIconPosition { get => _customIconPosition; set { if (_customIconPosition == value) return; _customIconPosition = value; UpdateExternalDrawing(); } }

        [Browsable(true), Category("External Icon"), DefaultValue(16)]
        public int CustomIconSize { get => _customIconSize; set { int c = Math.Max(8, Math.Min(48, value)); if (_customIconSize == c) return; _customIconSize = c; UpdateExternalDrawing(); } }

        [Browsable(true), Category("External Icon")]
        public Color CustomIconColor { get => _customIconColor; set { if (_customIconColor == value) return; _customIconColor = value; UpdateExternalDrawing(); } }

        public void ShowExternalIcon(string svgPath, BadgeAnchor? position = null, int? size = null, Color? color = null)
        {
            _customIconPath = svgPath;
            if (position.HasValue) _customIconPosition = position.Value;
            if (size.HasValue) _customIconSize = size.Value;
            if (color.HasValue) _customIconColor = color.Value;
            UpdateExternalDrawing();
        }

        public void ClearExternalIcon()
        {
            _customIconPath = string.Empty;
            UpdateExternalDrawing();
        }

        internal static DrawExternalHandler CreateCombinedExternalHandler(BaseControl owner)
        {
            bool hasLabel = owner.LabelTextOn && !string.IsNullOrEmpty(owner.LabelText);
            bool hasHelper = !string.IsNullOrEmpty(owner.HelperText);
            bool hasError = !string.IsNullOrEmpty(owner.ErrorText);
            bool showIcon = owner._validationIcon != ValidationState.None;
            bool showLine = owner._showIndicatorLine;
            bool showCustom = !string.IsNullOrEmpty(owner._customIconPath);

            if (!hasLabel && !hasHelper && !hasError && !showIcon && !showLine && !showCustom)
                return (Graphics g, Rectangle cb) => { };

            string? iconSvg = showIcon ? owner._validationIcon switch
            {
                ValidationState.Error => SvgsUIcons.Common.Error,
                ValidationState.Success => SvgsUIcons.Common.Success,
                ValidationState.Warning => SvgsUIcons.Common.Warning,
                ValidationState.Info => SvgsUIcons.Common.Info,
                _ => null
            } : null;

            return (Graphics g, Rectangle cb) =>
            {
                if (g is null || cb.IsEmpty) return;

                if (hasLabel || hasHelper || hasError)
                {
                    owner.DrawLabelAndHelperToParent(g, cb, owner,
                        owner.GetLabelLocation(), owner.GetImageLocation(), owner.GetMessageImagePath(),
                        owner.GetShowImage(), owner.GetShowHelperText(), owner.GetShowErrorText());
                }

                if (showIcon && iconSvg is not null)
                {
                    var r = ComputeIconRect(cb, owner._validationIconPosition, owner._validationIconSize);
                    try { StyledImagePainter.Paint(g, r, iconSvg); } catch { }
                }

                if (showLine)
                {
                    int lw = cb.Width - 4;
                    if (lw > 0)
                    {
                        using var pen = new Pen(owner._indicatorLineColor, owner._indicatorLineThickness);
                        pen.DashStyle = owner._indicatorLineStyle switch
                        {
                            IndicatorLineStyle.Dashed => System.Drawing.Drawing2D.DashStyle.Dash,
                            IndicatorLineStyle.Dotted => System.Drawing.Drawing2D.DashStyle.Dot,
                            _ => System.Drawing.Drawing2D.DashStyle.Solid
                        };
                        g.DrawLine(pen, cb.Left + 2, cb.Bottom + 3, cb.Left + 2 + lw, cb.Bottom + 3);
                    }
                }

                if (showCustom)
                {
                    var r = ComputeIconRect(cb, owner._customIconPosition, owner._customIconSize);
                    try { StyledImagePainter.Paint(g, r, owner._customIconPath); } catch { }
                }
            };
        }

        private static Rectangle ComputeIconRect(Rectangle cb, BadgeAnchor anchor, int size)
        {
            int off = 4;
            int x = anchor switch
            {
                BadgeAnchor.TopLeft or BadgeAnchor.MiddleLeft => cb.Left + off,
                BadgeAnchor.TopCenter or BadgeAnchor.BottomCenter or BadgeAnchor.MiddleCenter => cb.Left + (cb.Width - size) / 2,
                _ => cb.Right - size - off
            };
            int y = anchor switch
            {
                BadgeAnchor.TopLeft or BadgeAnchor.TopRight or BadgeAnchor.TopCenter => cb.Top + off,
                BadgeAnchor.BottomLeft or BadgeAnchor.BottomRight or BadgeAnchor.BottomCenter => cb.Bottom - size - off,
                _ => cb.Top + (cb.Height - size) / 2
            };
            return new Rectangle(x, y, size, size);
        }
    }
}
