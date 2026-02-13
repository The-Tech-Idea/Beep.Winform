using System.Drawing;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Modern shadow effect configuration for forms
    /// </summary>
   
    public class ShadowEffect
    {
        public Color Color { get; set; } = Color.FromArgb(30, 0, 0, 0);
        public int Blur { get; set; } = 10;
        public int Spread { get; set; } = 0;
        public int OffsetX { get; set; } = 0;
        public int OffsetY { get; set; } = 2;
        public bool Inner { get; set; } = false;
    }

    /// <summary>
       public class CornerRadius
    {
        public int TopLeft { get; set; } = 8;
        public int TopRight { get; set; } = 8;
        public int BottomLeft { get; set; } = 8;
        public int BottomRight { get; set; } = 8;

        public CornerRadius() { }
        public CornerRadius(int radius) => SetAll(radius);
        public CornerRadius(int topLeft, int topRight, int bottomLeft, int bottomRight)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
        }
        public void SetAll(int radius) => TopLeft = TopRight = BottomLeft = BottomRight = radius;

        /// <summary>
        /// Gets a value indicating whether all corner radii are the same.
        /// </summary>
        [Browsable(false)]
        public bool AllCornersEqual => TopLeft == TopRight && TopLeft == BottomLeft && TopLeft == BottomRight;
    }
    /// <summary>
    /// Anti-aliasing quality modes
    /// </summary>
    public enum AntiAliasMode
    {
        None,
        Low,
        High,
        Ultra
    }

    /// <summary>
    /// Optional contract for painters to provide sizing and layout metrics for the caption area.
    /// </summary>
    public interface IFormPainterMetricsProvider
    {
        FormPainterMetrics GetMetrics(BeepiFormPro owner);
    }

    /// <summary>
    /// Interface for form painters that handle custom rendering of BeepiFormPro forms.
    /// Provides methods for painting background, caption bar, and borders with modern effects.
    /// Now also handles layout calculations and hit area registration for its specific Style.
    /// </summary>
    public interface IFormPainter
    {
        /// <summary>
        /// Calculates layout positions specific to this painter's Style and registers hit areas.
        /// </summary>
        /// <param name="owner">The form instance being laid out.</param>
        void CalculateLayoutAndHitAreas(BeepiFormPro owner);

        /// <summary>
        /// Paints the background of the form with modern effects.
        /// </summary>
        /// <param name="g">The graphics context to paint on.</param>
        /// <param name="owner">The form instance being painted.</param>
        void PaintBackground(Graphics g, BeepiFormPro owner);

        /// <summary>
        /// Paints the caption bar area of the form.
        /// </summary>
        /// <param name="g">The graphics context to paint on.</param>
        /// <param name="owner">The form instance being painted.</param>
        /// <param name="captionRect">The rectangle defining the caption bar area.</param>
        void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect);

        /// <summary>
        /// Paints the borders of the form.
        /// </summary>
        /// <param name="g">The graphics context to paint on.</param>
        /// <param name="owner">The form instance being painted.</param>
        void PaintBorders(Graphics g, BeepiFormPro owner);

        /// <summary>
        /// Gets the shadow effect for this painter.
        /// </summary>
        ShadowEffect GetShadowEffect(BeepiFormPro owner);

        /// <summary>
        /// Gets the corner radius for this painter.
        /// </summary>
        CornerRadius GetCornerRadius(BeepiFormPro owner);

        /// <summary>
        /// Gets the anti-aliasing mode for this painter.
        /// </summary>
        AntiAliasMode GetAntiAliasMode(BeepiFormPro owner);

        /// <summary>
        /// Whether this painter supports animations.
        /// </summary>
        bool SupportsAnimations { get; }

        /// <summary>
        /// Paints with enhanced effects (shadows, rounded corners, anti-aliasing).
        /// </summary>
        void PaintWithEffects(Graphics g, BeepiFormPro owner, Rectangle rect);
    }

    /// <summary>
    /// Optional interface for painters that want to customize non-client border painting
    /// (when the form reserves a non-client frame and paints it via WM_NCPAINT).
    /// Implement this to render per-Style borders in the window's non-client area.
    /// </summary>
    public interface IFormNonClientPainter
    {
        /// <param name="g">Graphics for the entire window surface (non-client coordinates).</param>
        /// <param name="owner">The form.</param>
        /// <param name="borderThickness">Effective non-client border thickness in device pixels.</param>
        /// <remarks>The painter is responsible for determining the window shape/path appropriate for its Style.</remarks>
        void PaintNonClientBorder(Graphics g, BeepiFormPro owner, int borderThickness);
    }

    /// <summary>
    /// Single authoritative layout class for BeepiFormPro.
    /// Painters create an instance in CalculateLayoutAndHitAreas and assign it
    /// to owner.CurrentLayout. All form code reads from CurrentLayout.
    /// </summary>
    public class PainterLayoutInfo
    {
        // Main layout areas
        public Rectangle CaptionRect { get; set; }
        public Rectangle ContentRect { get; set; }
        public Rectangle BottomRect { get; set; }
        public Rectangle LeftRect { get; set; }
        public Rectangle RightRect { get; set; }

        // Caption zones (for flexible alignment within the caption bar)
        public Rectangle LeftZoneRect { get; set; }
        public Rectangle CenterZoneRect { get; set; }
        public Rectangle RightZoneRect { get; set; }

        // Caption element rects
        public Rectangle IconRect { get; set; }
        public Rectangle TitleRect { get; set; }

        // System button rects
        public Rectangle MinimizeButtonRect { get; set; }
        public Rectangle MaximizeButtonRect { get; set; }
        public Rectangle CloseButtonRect { get; set; }

        // Toolbar / action button rects
        public Rectangle ThemeButtonRect { get; set; }
        public Rectangle StyleButtonRect { get; set; }
        public Rectangle CustomActionButtonRect { get; set; }
        public Rectangle ProfileButtonRect { get; set; }
        public Rectangle MailButtonRect { get; set; }
        public Rectangle SearchBoxRect { get; set; }
    }
}
