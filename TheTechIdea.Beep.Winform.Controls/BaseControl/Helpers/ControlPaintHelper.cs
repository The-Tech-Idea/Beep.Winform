using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{
    internal partial class ControlPaintHelper
    {
        private readonly BaseControl _owner;
        private BaseControl OwnerAdv => _owner as BaseControl;

        public ControlPaintHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            
            UpdateRects();
        }

        #region Basic Appearance Properties
        [Browsable(true)] public bool ShowAllBorders { get; set; } = false;
        [Browsable(true)] public bool ShowTopBorder { get; set; } = false;
        [Browsable(true)] public bool ShowBottomBorder { get; set; } = false;
        [Browsable(true)] public bool ShowLeftBorder { get; set; } = false;
        [Browsable(true)] public bool ShowRightBorder { get; set; } = false;
        [Browsable(true)] public int BorderThickness { get; set; } = 1;
        [Browsable(true)] public int BorderRadius { get; set; } = 8;
        [Browsable(true)] public bool IsRounded { get; set; } = true;
        [Browsable(true)] public DashStyle BorderDashStyle { get; set; } = DashStyle.Solid;
        [Browsable(true)] public Color InactiveBorderColor { get; set; } = Color.Gray;

        [Browsable(true)] public bool ShowShadow { get; set; } = false;
        [Browsable(true)] public Color ShadowColor { get; set; } = Color.Black;
        [Browsable(true)] public float ShadowOpacity { get; set; } = 0.25f;
        [Browsable(true)] public int ShadowOffset { get; set; } = 3;

        [Browsable(true)] public bool UseGradientBackground { get; set; } = false;
        [Browsable(true)] public LinearGradientMode GradientDirection { get; set; } = LinearGradientMode.Horizontal;
        [Browsable(true)] public Color GradientStartColor { get; set; } = Color.LightGray;
        [Browsable(true)] public Color GradientEndColor { get; set; } = Color.Gray;
        #endregion

        #region DrawingRect Offsets (parity with BeepControl)
        // These offsets allow consumers to shrink/shift the inner drawing rectangle
        [Browsable(false)] public int LeftoffsetForDrawingRect { get; set; } = 0;
        [Browsable(false)] public int TopoffsetForDrawingRect { get; set; } = 0;
        [Browsable(false)] public int RightoffsetForDrawingRect { get; set; } = 0;
        [Browsable(false)] public int BottomoffsetForDrawingRect { get; set; } = 0;
        #endregion

        #region Modern Gradient Properties
        [Browsable(true)] public ModernGradientType ModernGradientType { get; set; } = ModernGradientType.None;
        [Browsable(false)] public List<GradientStop> GradientStops { get; set; } = new List<GradientStop>();
        [Browsable(true)] public PointF RadialCenter { get; set; } = new PointF(0.5f, 0.5f);
        [Browsable(true)] public float GradientAngle { get; set; } = 0f;
        [Browsable(true)] public bool UseGlassmorphism { get; set; } = false;
        [Browsable(true)] public float GlassmorphismBlur { get; set; } = 10f;
        [Browsable(true)] public float GlassmorphismOpacity { get; set; } = 0.1f;
        #endregion

        #region Material UI Properties
        [Browsable(true)] public MaterialTextFieldVariant MaterialBorderVariant { get; set; } = MaterialTextFieldVariant.Standard;
        [Browsable(true)] public bool FloatingLabel { get; set; } = true;
        [Browsable(true)] public string LabelText { get; set; } = string.Empty;
        [Browsable(true)] public string HelperText { get; set; } = string.Empty;
        [Browsable(true)] public Color FocusBorderColor { get; set; } = Color.RoyalBlue;
        [Browsable(true)] public Color FilledBackgroundColor { get; set; } = Color.FromArgb(20, 0, 0, 0);
        
        // Validation state for helper text color
        [Browsable(true)] public bool IsValid { get; set; } = true;
        #endregion

        #region React UI Properties
        [Browsable(true)] public ReactUIVariant UIVariant { get; set; } = ReactUIVariant.Default;
        [Browsable(true)] public ReactUISize UISize { get; set; } = ReactUISize.Medium;
        [Browsable(true)] public ReactUIColor UIColor { get; set; } = ReactUIColor.Primary;
        [Browsable(true)] public ReactUIDensity UIDensity { get; set; } = ReactUIDensity.Standard;
        [Browsable(true)] public ReactUIElevation UIElevation { get; set; } = ReactUIElevation.None;
        [Browsable(true)] public ReactUIShape UIShape { get; set; } = ReactUIShape.Rounded;
        [Browsable(true)] public ReactUIAnimation UIAnimation { get; set; } = ReactUIAnimation.None;
        [Browsable(true)] public bool UIFullWidth { get; set; } = false;
        [Browsable(true)] public int UICustomElevation { get; set; } = 0;
        [Browsable(true)] public bool UIDisabled 
        { 
            get => !_owner.Enabled; 
            set => _owner.Enabled = !value; 
        }
        #endregion

        #region Badge Properties
        [Browsable(true)] public string BadgeText { get; set; } = "";
        [Browsable(true)] public Color BadgeBackColor { get; set; } = Color.Red;
        [Browsable(true)] public Color BadgeForeColor { get; set; } = Color.White;
        [Browsable(true)] public Font BadgeFont { get; set; } = new Font("Arial", 8, FontStyle.Bold);
        [Browsable(true)] public BadgeShape BadgeShape { get; set; } = BadgeShape.Circle;
        #endregion

        #region State Colors
        [Browsable(true)] public Color HoverBackColor { get; set; } = Color.LightBlue;
        [Browsable(true)] public Color HoverBorderColor { get; set; } = Color.Blue;
        [Browsable(true)] public Color HoverForeColor { get; set; } = Color.Black;
        [Browsable(true)] public Color PressedBackColor { get; set; } = Color.Gray;
        [Browsable(true)] public Color PressedBorderColor { get; set; } = Color.DarkGray;
        [Browsable(true)] public Color PressedForeColor { get; set; } = Color.White;
        [Browsable(true)] public Color FocusBackColor { get; set; } = Color.LightYellow;
        [Browsable(true)] public Color FocusForeColor { get; set; } = Color.Black;
        [Browsable(true)] public Color DisabledBackColor { get; set; } = Color.LightGray;
        [Browsable(true)] public Color DisabledBorderColor { get; set; } = Color.Gray;
        [Browsable(true)] public Color DisabledForeColor { get; set; } = Color.DarkGray;
        [Browsable(true)] public Color SelectedBackColor { get; set; } = Color.LightGreen;
        [Browsable(true)] public Color SelectedBorderColor { get; set; } = Color.Green;
        [Browsable(true)] public Color SelectedForeColor { get; set; } = Color.Black;
        #endregion

        #region Additional Properties for Full Parity
        [Browsable(true)] public bool CanBeHovered { get; set; } = true;
        [Browsable(true)] public bool CanBePressed { get; set; } = true;
        [Browsable(true)] public bool CanBeFocused { get; set; } = true;
        [Browsable(true)] public bool IsFrameless { get; set; } = false;
        [Browsable(true)] public bool IsBorderAffectedByTheme { get; set; } = true;
        [Browsable(true)] public bool IsShadowAffectedByTheme { get; set; } = true;
        [Browsable(true)] public bool IsRoundedAffectedByTheme { get; set; } = true;
        [Browsable(true)] public BorderStyle BorderStyle { get; set; } = BorderStyle.FixedSingle;
        #endregion

        public Rectangle DrawingRect { get;  set; }
        public Rectangle BorderRectangle { get;  set; }

        public void UpdateRects()
        {
            int shadow = ShowShadow ? ShadowOffset : 0;
            int border = ShowAllBorders ? BorderThickness : 0;
            var padding = _owner.Padding;

            // Include custom offsets like base BeepControl
            int leftPad = padding.Left + LeftoffsetForDrawingRect;
            int topPad = padding.Top + TopoffsetForDrawingRect;
            int rightPad = padding.Right + RightoffsetForDrawingRect;
            int bottomPad = padding.Bottom + BottomoffsetForDrawingRect;

            int w = Math.Max(0, _owner.Width - (shadow * 2 + border * 2 + leftPad + rightPad));
            int h = Math.Max(0, _owner.Height - (shadow * 2 + border * 2 + topPad + bottomPad));

            DrawingRect = new Rectangle(
                shadow + border + leftPad,
                shadow + border + topPad,
                w,
                h);

            // Update border rectangle
            int halfPen = (int)Math.Ceiling(BorderThickness / 2f);
            BorderRectangle = new Rectangle(
                shadow + halfPen,
                shadow + halfPen,
                _owner.Width - (shadow + halfPen) * 2,
                _owner.Height - (shadow + halfPen) * 2
            );
        }

        // Custom border flag will be held on owner (via BaseControl.IsCustomeBorder),
        // we expose an event-like callback for custom drawing if needed.
        public Action<Graphics> CustomBorderDrawer { get; set; }

        public void Draw(Graphics g)
        {
            if (g == null) return;
            //g.SmoothingMode = SmoothingMode.AntiAlias;
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            _owner.UpdateDrawingRect();
            if (UIVariant != ReactUIVariant.Default)
            {
                ApplyReactUIStyles();
            }
            Console.WriteLine($"DrawingRect: {DrawingRect}, BorderRect: {BorderRectangle}");
            DrawBackground(g);

            if (ShowShadow)
            {
                DrawShadow(g);
            }

            // If consumer wants full custom border, let them draw; otherwise default borders
            if (!(OwnerAdv?.IsCustomeBorder ?? false))
            {
                if (!IsFrameless)
                {
                    DrawBorders(g);
                }
            }
            else
            {
                CustomBorderDrawer?.Invoke(g);
            }

            //if (!string.IsNullOrEmpty(BadgeText))
            //{
            //    //DrawBadge(g);
            //    _owner.DrawBadgeExternally(g,new Rectangle() { Height=20,Width=20});
            //}
        }

        private void DrawBackground(Graphics g)
        {
            Color backColor = GetEffectiveBackColor();

            if (UseGradientBackground && ModernGradientType != ModernGradientType.None)
            {
                DrawModernGradient(g, backColor);
            }
            else if (UseGradientBackground)
            {
                DrawLinearGradient(g, GradientStartColor, GradientEndColor);
            }
            else
            {
                // Material UI Filled variant background
                if (MaterialBorderVariant == MaterialTextFieldVariant.Filled)
                {
                    backColor = FilledBackgroundColor;
                }

                using (var brush = new SolidBrush(backColor))
                {
                    FillShape(g, brush, DrawingRect);
                }
            }
        }

        private Color GetEffectiveBackColor()
        {
            var ownerAdv = OwnerAdv;
            if (ownerAdv != null)
            {
                if (!_owner.Enabled) return DisabledBackColor;
                if (ownerAdv.IsPressed) return PressedBackColor;
                if (ownerAdv.IsHovered) return HoverBackColor;
                if (_owner.Focused) return FocusBackColor;
                if (ownerAdv.IsSelected) return SelectedBackColor;
            }
            return _owner.BackColor;
        }

        private void DrawShadow(Graphics g)
        {
            if (ShadowOpacity <= 0) return;

            int shadowDepth = Math.Max(1, ShadowOffset / 2);
            int maxLayers = Math.Min(shadowDepth, 6);

            Rectangle shadowRect = new Rectangle(
                DrawingRect.X + ShadowOffset,
                DrawingRect.Y + ShadowOffset,
                DrawingRect.Width,
                DrawingRect.Height);

            for (int i = 1; i <= maxLayers; i++)
            {
                float layerOpacityFactor = (float)(maxLayers - i + 1) / maxLayers;
                float finalOpacity = ShadowOpacity * layerOpacityFactor * 0.6f;
                int layerAlpha = Math.Max(5, (int)(255 * finalOpacity));

                Color layerShadowColor = Color.FromArgb(layerAlpha, ShadowColor);
                int spread = i - 1;
                Rectangle layerRect = new Rectangle(
                    shadowRect.X - spread,
                    shadowRect.Y - spread,
                    shadowRect.Width + (spread * 2),
                    shadowRect.Height + (spread * 2));

                using (var shadowBrush = new SolidBrush(layerShadowColor))
                {
                    if (IsRounded && BorderRadius > 0)
                    {
                        int shadowRadius = Math.Max(0, BorderRadius + spread);
                        using (var shadowPath = GetRoundedRectPath(layerRect, shadowRadius))
                        {
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }
                    else
                    {
                        g.FillRectangle(shadowBrush, layerRect);
                    }
                }
            }
        }

        private void DrawBorders(Graphics g)
        {
            // Material UI borders take priority
            if (MaterialBorderVariant != MaterialTextFieldVariant.Standard)
            {
                DrawMaterialBorder(g);
                return;
            }

            Color effectiveBorderColor = GetEffectiveBorderColor();

            if (ShowAllBorders && BorderThickness > 0)
            {
                using (var borderPen = new Pen(effectiveBorderColor, BorderThickness))
                {
                    borderPen.DashStyle = BorderDashStyle;
                    borderPen.Alignment = PenAlignment.Inset;

                    if (IsRounded)
                    {
                        using (var path = GetRoundedRectPath(BorderRectangle, BorderRadius))
                        {
                            g.DrawPath(borderPen, path);
                        }
                    }
                    else
                    {
                        g.DrawRectangle(borderPen, BorderRectangle);
                    }
                }
            }
            else
            {
                // Draw individual borders
                using (var borderPen = new Pen(effectiveBorderColor, BorderThickness))
                {
                    borderPen.DashStyle = BorderDashStyle;
                    if (ShowTopBorder)
                        g.DrawLine(borderPen, BorderRectangle.Left, BorderRectangle.Top, BorderRectangle.Right, BorderRectangle.Top);
                    if (ShowBottomBorder)
                        g.DrawLine(borderPen, BorderRectangle.Left, BorderRectangle.Bottom, BorderRectangle.Right, BorderRectangle.Bottom);
                    if (ShowLeftBorder)
                        g.DrawLine(borderPen, BorderRectangle.Left, BorderRectangle.Top, BorderRectangle.Left, BorderRectangle.Bottom);
                    if (ShowRightBorder)
                        g.DrawLine(borderPen, BorderRectangle.Right, BorderRectangle.Top, BorderRectangle.Right, BorderRectangle.Bottom);
                }
            }
        }

        private Color GetEffectiveBorderColor()
        {
            var ownerAdv = OwnerAdv;
            if (ownerAdv != null)
            {
                if (!_owner.Enabled) return DisabledBorderColor;
                if (_owner.Focused) return FocusBorderColor;
                if (ownerAdv.IsHovered) return HoverBorderColor;
                if (ownerAdv.IsPressed) return PressedBorderColor;
                if (ownerAdv.IsSelected) return SelectedBorderColor;
                return ownerAdv.BorderColor;
            }
            return InactiveBorderColor;
        }

        private void DrawMaterialBorder(Graphics g)
        {
            Color borderColor = GetEffectiveBorderColor();
            Rectangle borderRect = BorderRectangle;

            switch (MaterialBorderVariant)
            {
                case MaterialTextFieldVariant.Standard:
                    using (var underlinePen = new Pen(borderColor, 1))
                    {
                        g.DrawLine(underlinePen, borderRect.Left, borderRect.Bottom - 1, borderRect.Right, borderRect.Bottom - 1);
                        if (_owner.Focused)
                        {
                            using (var focusPen = new Pen(FocusBorderColor, 2))
                                g.DrawLine(focusPen, borderRect.Left, borderRect.Bottom, borderRect.Right, borderRect.Bottom);
                        }
                    }
                    break;

                case MaterialTextFieldVariant.Outlined:
                    using (var borderPen = new Pen(borderColor, 1))
                    {
                        if (IsRounded)
                        {
                            using (var path = GetRoundedRectPath(borderRect, BorderRadius))
                                g.DrawPath(borderPen, path);
                        }
                        else
                        {
                            g.DrawRectangle(borderPen, borderRect);
                        }

                        // Draw floating label if needed
                        if (FloatingLabel && !string.IsNullOrEmpty(LabelText))
                        {
                            DrawFloatingLabel(g, borderRect, borderColor);
                        }
                    }
                    break;

                case MaterialTextFieldVariant.Filled:
                    // Background already handled in DrawBackground
                    using (var underlinePen = new Pen(borderColor, 1))
                    {
                        g.DrawLine(underlinePen, borderRect.Left, borderRect.Bottom - 1, borderRect.Right, borderRect.Bottom - 1);
                        if (_owner.Focused)
                        {
                            using (var focusPen = new Pen(FocusBorderColor, 2))
                                g.DrawLine(focusPen, borderRect.Left, borderRect.Bottom, borderRect.Right, borderRect.Bottom);
                        }
                    }
                    break;
            }

            // Draw helper text if provided
            if (!string.IsNullOrEmpty(HelperText))
            {
                DrawHelperText(g, borderRect);
            }
        }

        private void DrawFloatingLabel(Graphics g, Rectangle borderRect, Color borderColor)
        {
            var labelFont = new Font(_owner.Font.FontFamily, _owner.Font.Size * 0.8f);
            var labelSize = TextRenderer.MeasureText(LabelText, labelFont);
            int labelX = borderRect.X + 10;
            var labelGapRect = new Rectangle(labelX - 2, borderRect.Y - labelSize.Height / 2, labelSize.Width + 4, labelSize.Height);

            using (var backBrush = new SolidBrush(_owner.BackColor))
                g.FillRectangle(backBrush, labelGapRect);

            using (var labelBrush = new SolidBrush(_owner.Focused ? FocusBorderColor : borderColor))
                g.DrawString(LabelText, labelFont, labelBrush, labelX, borderRect.Y - labelSize.Height / 2);
        }

        private void DrawHelperText(Graphics g, Rectangle borderRect)
        {
            var helperFont = new Font(_owner.Font.FontFamily, _owner.Font.Size * 0.8f);
            Color helperColor = IsValid ? Color.Gray : Color.Red;
            var helperRect = new Rectangle(borderRect.X, borderRect.Bottom + 2, borderRect.Width, 20);
            TextRenderer.DrawText(g, HelperText, helperFont, helperRect, helperColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        private void DrawBadge(Graphics g)
        {
            const int badgeSize = 22;
            int x = _owner.Width - badgeSize / 2;
            int y = -badgeSize / 2;
            var badgeRect = new Rectangle(x, y, badgeSize, badgeSize);

            // Badge shadow
            if (ShowShadow)
            {
                float badgeShadowOpacity = Math.Min(0.3f, ShadowOpacity * 0.8f);
                int badgeShadowOffset = 1;
                Color badgeShadowColor = Color.FromArgb((int)(255 * badgeShadowOpacity), ShadowColor);

                using (var shadowBrush = new SolidBrush(badgeShadowColor))
                {
                    g.FillEllipse(shadowBrush, badgeRect.X + badgeShadowOffset, badgeRect.Y + badgeShadowOffset, badgeRect.Width, badgeRect.Height);
                }
            }

            // Badge background
            using (var brush = new SolidBrush(BadgeBackColor))
            {
                switch (BadgeShape)
                {
                    case BadgeShape.Circle:
                        g.FillEllipse(brush, badgeRect);
                        break;
                    case BadgeShape.RoundedRectangle:
                        using (var path = GetRoundedRectPath(badgeRect, badgeRect.Height / 4))
                            g.FillPath(brush, path);
                        break;
                    case BadgeShape.Rectangle:
                        g.FillRectangle(brush, badgeRect);
                        break;
                }
            }

            // Badge text
            if (!string.IsNullOrEmpty(BadgeText))
            {
                using (var textBrush = new SolidBrush(BadgeForeColor))
                using (var scaledFont = GetScaledBadgeFont(g, BadgeText, new Size(badgeRect.Width - 4, badgeRect.Height - 4), BadgeFont))
                {
                    var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(BadgeText, scaledFont, textBrush, badgeRect, fmt);
                }
            }
        }

        #region Modern Gradient Methods
        private void DrawModernGradient(Graphics g, Color baseColor)
        {
            switch (ModernGradientType)
            {
                case ModernGradientType.Subtle:
                    DrawSubtleGradient(g, DrawingRect, baseColor);
                    break;
                case ModernGradientType.Linear:
                    DrawLinearGradient(g, GradientStartColor, GradientEndColor);
                    break;
                case ModernGradientType.Radial:
                    DrawRadialGradient(g, DrawingRect, baseColor);
                    break;
                case ModernGradientType.Conic:
                    DrawConicGradient(g, DrawingRect, baseColor);
                    break;
                case ModernGradientType.Mesh:
                    DrawMeshGradient(g, DrawingRect, baseColor);
                    break;
            }

            if (UseGlassmorphism)
            {
                ApplyGlassmorphism(g, DrawingRect);
            }
        }

        private void DrawSubtleGradient(Graphics g, Rectangle rect, Color baseColor)
        {
            Color color1 = baseColor;
            float brightness = baseColor.GetBrightness();
            const float subtleFactor = 0.05f;

            Color color2 = brightness > 0.5f
                ? Color.FromArgb(Math.Max(0, baseColor.R - (int)(255 * subtleFactor)), Math.Max(0, baseColor.G - (int)(255 * subtleFactor)), Math.Max(0, baseColor.B - (int)(255 * subtleFactor)))
                : Color.FromArgb(Math.Min(255, baseColor.R + (int)(255 * subtleFactor)), Math.Min(255, baseColor.G + (int)(255 * subtleFactor)), Math.Min(255, baseColor.B + (int)(255 * subtleFactor)));

            float angleRadians = (float)(GradientAngle * Math.PI / 180f);
            using (var gradientBrush = CreateAngledGradientBrush(rect, color1, color2, angleRadians))
            {
                var blend = new ColorBlend();
                blend.Colors = new Color[] { color1, BlendColors(color1, color2, 0.5f), color2 };
                blend.Positions = new float[] { 0.0f, 0.3f, 1.0f };
                gradientBrush.InterpolationColors = blend;
                FillShape(g, gradientBrush, rect);
            }
        }

        private void DrawLinearGradient(Graphics g, Color startColor, Color endColor)
        {
            using (var gradientBrush = new LinearGradientBrush(DrawingRect, startColor, endColor, GradientDirection))
            {
                if (GradientStops.Count > 0)
                {
                    ApplyGradientStops(gradientBrush);
                }
                FillShape(g, gradientBrush, DrawingRect);
            }
        }

        private void DrawRadialGradient(Graphics g, Rectangle rect, Color baseColor)
        {
            Color centerColor = GradientStartColor != Color.LightGray ? GradientStartColor : baseColor;
            Color edgeColor = GradientEndColor != Color.Gray ? GradientEndColor : ModifyColorBrightness(baseColor, 0.7f);

            var center = new PointF(rect.X + rect.Width * RadialCenter.X, rect.Y + rect.Height * RadialCenter.Y);
            float radius = Math.Max(rect.Width, rect.Height) * 0.7f;

            using (var path = new GraphicsPath())
            {
                path.AddEllipse(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                using (var gradientBrush = new PathGradientBrush(path))
                {
                    gradientBrush.CenterColor = centerColor;
                    gradientBrush.SurroundColors = new Color[] { edgeColor };
                    gradientBrush.CenterPoint = center;
                    FillShape(g, gradientBrush, rect);
                }
            }
        }

        private void DrawConicGradient(Graphics g, Rectangle rect, Color baseColor)
        {
            var center = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
            int segments = 36;

            for (int i = 0; i < segments; i++)
            {
                float startAngle = (i * 360f / segments) + GradientAngle;
                float hue = (startAngle % 360f) / 360f;
                Color segmentColor = ColorFromHSV(hue, 0.5f, baseColor.GetBrightness());

                using (var segmentBrush = new SolidBrush(Color.FromArgb(100, segmentColor)))
                {
                    g.FillPie(segmentBrush, rect, startAngle, 360f / segments);
                }
            }
        }

        private void DrawMeshGradient(Graphics g, Rectangle rect, Color baseColor)
        {
            int gridSize = 3;
            Color[,] colorGrid = new Color[gridSize, gridSize];

            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    float brightness = 0.7f + (0.3f * ((x + y) / (float)(gridSize * 2)));
                    colorGrid[x, y] = ModifyColorBrightness(baseColor, brightness);
                }
            }

            float cellWidth = rect.Width / (float)(gridSize - 1);
            float cellHeight = rect.Height / (float)(gridSize - 1);

            for (int x = 0; x < gridSize - 1; x++)
            {
                for (int y = 0; y < gridSize - 1; y++)
                {
                    var cellRect = new RectangleF(rect.X + x * cellWidth, rect.Y + y * cellHeight, cellWidth * 1.5f, cellHeight * 1.5f);
                    using (var cellBrush = new LinearGradientBrush(cellRect, colorGrid[x, y], colorGrid[x + 1, y + 1], LinearGradientMode.ForwardDiagonal))
                    {
                        g.FillRectangle(cellBrush, cellRect);
                    }
                }
            }
        }

        private void ApplyGlassmorphism(Graphics g, Rectangle rect)
        {
            using (var glassBrush = new SolidBrush(Color.FromArgb((int)(255 * GlassmorphismOpacity), Color.White)))
            {
                var random = new Random(42);
                for (int i = 0; i < rect.Width * rect.Height / 1000; i++)
                {
                    int x = random.Next(rect.X, rect.X + rect.Width);
                    int y = random.Next(rect.Y, rect.Y + rect.Height);
                    using (var noiseBrush = new SolidBrush(Color.FromArgb(random.Next(5, 15), Color.White)))
                    {
                        g.FillRectangle(noiseBrush, x, y, 1, 1);
                    }
                }
                FillShape(g, glassBrush, rect);
            }
        }
        #endregion

        #region React UI Methods
        private void ApplyReactUIStyles()
        {
            // Apply shape based on UIShape
            switch (UIShape)
            {
                case ReactUIShape.Square:
                    IsRounded = false;
                    BorderRadius = 0;
                    break;
                case ReactUIShape.Rounded:
                    IsRounded = true;
                    BorderRadius = GetSizeBasedValue(8, 4, 8, 12, 16);
                    break;
                case ReactUIShape.Circular:
                    IsRounded = true;
                    BorderRadius = Math.Min(_owner.Width, _owner.Height) / 2;
                    break;
                case ReactUIShape.Pill:
                    IsRounded = true;
                    BorderRadius = _owner.Height / 2;
                    break;
            }

            // Apply elevation/shadows
            switch (UIElevation)
            {
                case ReactUIElevation.None:
                    ShowShadow = false;
                    break;
                case ReactUIElevation.Low:
                    ShowShadow = true;
                    ShadowOpacity = 0.2f;
                    ShadowOffset = 2;
                    break;
                case ReactUIElevation.Medium:
                    ShowShadow = true;
                    ShadowOpacity = 0.3f;
                    ShadowOffset = 4;
                    break;
                case ReactUIElevation.High:
                    ShowShadow = true;
                    ShadowOpacity = 0.4f;
                    ShadowOffset = 6;
                    break;
                case ReactUIElevation.Custom:
                    ShowShadow = true;
                    ShadowOpacity = 0.3f;
                    ShadowOffset = UICustomElevation;
                    break;
            }

            ApplyColorScheme();
            ApplyVariantStyling();
        }

        private int GetSizeBasedValue(int xs, int sm, int md, int lg, int xl)
        {
            return UISize switch
            {
                ReactUISize.ExtraSmall => xs,
                ReactUISize.Small => sm,
                ReactUISize.Medium => md,
                ReactUISize.Large => lg,
                ReactUISize.ExtraLarge => xl,
                _ => md
            };
        }

        private void ApplyColorScheme()
        {
            Color primaryColor, secondaryColor, backgroundColor, textColor, borderColor;

            switch (UIColor)
            {
                case ReactUIColor.Primary:
                    primaryColor = Color.FromArgb(25, 118, 210);
                    secondaryColor = Color.FromArgb(66, 165, 245);
                    borderColor = Color.FromArgb(25, 118, 210);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = Color.White;
                    break;
                case ReactUIColor.Secondary:
                    primaryColor = Color.FromArgb(156, 39, 176);
                    secondaryColor = Color.FromArgb(186, 104, 200);
                    borderColor = Color.FromArgb(156, 39, 176);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = Color.White;
                    break;
                case ReactUIColor.Success:
                    primaryColor = Color.FromArgb(46, 125, 50);
                    secondaryColor = Color.FromArgb(76, 175, 80);
                    borderColor = Color.FromArgb(46, 125, 50);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = Color.White;
                    break;
                case ReactUIColor.Error:
                    primaryColor = Color.FromArgb(211, 47, 47);
                    secondaryColor = Color.FromArgb(239, 83, 80);
                    borderColor = Color.FromArgb(211, 47, 47);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = Color.White;
                    break;
                case ReactUIColor.Warning:
                    primaryColor = Color.FromArgb(237, 108, 2);
                    secondaryColor = Color.FromArgb(255, 152, 0);
                    borderColor = Color.FromArgb(237, 108, 2);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = Color.White;
                    break;
                case ReactUIColor.Info:
                    primaryColor = Color.FromArgb(2, 136, 209);
                    secondaryColor = Color.FromArgb(3, 169, 244);
                    borderColor = Color.FromArgb(2, 136, 209);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = Color.White;
                    break;
                default:
                    primaryColor = Color.FromArgb(158, 158, 158);
                    secondaryColor = Color.FromArgb(189, 189, 189);
                    borderColor = Color.FromArgb(158, 158, 158);
                    backgroundColor = Color.FromArgb(255, 255, 255);
                    textColor = Color.Black;
                    break;
            }

            if (UIVariant == ReactUIVariant.Outlined || UIVariant == ReactUIVariant.Text)
            {
                _owner.ForeColor = primaryColor;
                _owner.BackColor = backgroundColor;
                if (OwnerAdv != null) OwnerAdv.BorderColor = primaryColor;
                HoverForeColor = secondaryColor;
                HoverBackColor = Color.FromArgb(10, primaryColor);
                HoverBorderColor = secondaryColor;
            }
            else
            {
                _owner.ForeColor = textColor;
                _owner.BackColor = primaryColor;
                if (OwnerAdv != null) OwnerAdv.BorderColor = primaryColor;
                HoverForeColor = textColor;
                HoverBackColor = secondaryColor;
                HoverBorderColor = secondaryColor;
            }
        }

        private void ApplyVariantStyling()
        {
            switch (UIVariant)
            {
                case ReactUIVariant.Outlined:
                    ShowAllBorders = true;
                    BorderThickness = 1;
                    break;
                case ReactUIVariant.Text:
                    ShowAllBorders = false;
                    break;
                case ReactUIVariant.Contained:
                case ReactUIVariant.Filled:
                    ShowAllBorders = false;
                    break;
                case ReactUIVariant.Ghost:
                    _owner.BackColor = Color.Transparent;
                    ShowAllBorders = false;
                    break;
                default:
                    ShowAllBorders = true;
                    BorderThickness = 1;
                    break;
            }
        }
        #endregion

        #region Helper Methods
        private void FillShape(Graphics g, Brush brush, Rectangle rect)
        {
            if (IsRounded)
            {
                using (var path = GetRoundedRectPath(rect, BorderRadius))
                {
                    g.FillPath(brush, path);
                }
            }
            else
            {
                g.FillRectangle(brush, rect);
            }
        }

        private LinearGradientBrush CreateAngledGradientBrush(Rectangle rect, Color color1, Color color2, float angleRadians)
        {
            float cos = (float)Math.Cos(angleRadians);
            float sin = (float)Math.Sin(angleRadians);

            var start = new PointF(
                rect.X + rect.Width * (0.5f - cos * 0.5f),
                rect.Y + rect.Height * (0.5f - sin * 0.5f));

            var end = new PointF(
                rect.X + rect.Width * (0.5f + cos * 0.5f),
                rect.Y + rect.Height * (0.5f + sin * 0.5f));

            return new LinearGradientBrush(start, end, color1, color2);
        }

        private Color ModifyColorBrightness(Color color, float brightness)
        {
            return Color.FromArgb(color.A, (int)(color.R * brightness), (int)(color.G * brightness), (int)(color.B * brightness));
        }

        private Color BlendColors(Color color1, Color color2, float amount)
        {
            return Color.FromArgb(
                (int)(color1.R + (color2.R - color1.R) * amount),
                (int)(color1.G + (color2.G - color1.G) * amount),
                (int)(color1.B + (color2.B - color1.B) * amount));
        }

        private Color ColorFromHSV(float hue, float saturation, double brightness)
        {
            hue = hue * 360f;
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            double v = brightness;
            double p = brightness * (1 - saturation);
            double q = brightness * (1 - f * saturation);
            double t = brightness * (1 - (1 - f) * saturation);

            switch (hi)
            {
                case 0: return Color.FromArgb(255, (int)(v * 255), (int)(t * 255), (int)(p * 255));
                case 1: return Color.FromArgb(255, (int)(q * 255), (int)(v * 255), (int)(p * 255));
                case 2: return Color.FromArgb(255, (int)(p * 255), (int)(v * 255), (int)(t * 255));
                case 3: return Color.FromArgb(255, (int)(p * 255), (int)(q * 255), (int)(v * 255));
                case 4: return Color.FromArgb(255, (int)(t * 255), (int)(p * 255), (int)(v * 255));
                default: return Color.FromArgb(255, (int)(v * 255), (int)(p * 255), (int)(q * 255));
            }
        }

        private void ApplyGradientStops(LinearGradientBrush brush)
        {
            if (GradientStops.Count < 2) return;

            var sortedStops = GradientStops.OrderBy(s => s.Position).ToList();
            var blend = new ColorBlend();
            blend.Colors = sortedStops.Select(s => s.Color).ToArray();
            blend.Positions = sortedStops.Select(s => s.Position).ToArray();
            brush.InterpolationColors = blend;
        }

        private Font GetScaledBadgeFont(Graphics g, string text, Size maxSize, Font originalFont)
        {
            if (string.IsNullOrEmpty(text) || maxSize.Width <= 0 || maxSize.Height <= 0)
                return new Font(originalFont.FontFamily, 8, FontStyle.Bold);

            if (text.Length == 1)
            {
                float fontSize = Math.Max(6, Math.Min(maxSize.Height * 0.65f, 10));
                return new Font(originalFont.FontFamily, fontSize, FontStyle.Bold);
            }

            for (float size = originalFont.Size; size >= 6; size -= 0.5f)
            {
                using (var testFont = new Font(originalFont.FontFamily, size, FontStyle.Bold))
                {
                    var measuredSize = TextRenderer.MeasureText(g, text, testFont);
                    if (measuredSize.Width <= maxSize.Width && measuredSize.Height <= maxSize.Height)
                    {
                        return new Font(originalFont.FontFamily, size, FontStyle.Bold);
                    }
                }
            }

            return new Font(originalFont.FontFamily, 6, FontStyle.Bold);
        }

        public void AddGradientStop(float position, Color color)
        {
            GradientStops.Add(new GradientStop(position, color));
        }

        public void ClearGradientStops()
        {
            GradientStops.Clear();
        }

        public static GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = Math.Min(Math.Min(radius * 2, rect.Width), rect.Height);
            if (d <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
        #endregion
    }
}
