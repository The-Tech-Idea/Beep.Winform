using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls.Badges
{
    public partial class BeepFloatingBadge
    {
        /// <summary>Space either side of measured content, before the shape's own rounding.</summary>
        private const int ContentPaddingX = 6;

        private string? _failedIconPath;
        private string? _resolvedIconPath;
        private bool _resolvedIconOk;

        /// <summary>
        /// Draws an icon, or a visible "missing glyph" mark if it cannot be rendered.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The two icon-bearing badges each wrapped their painter call in a bare <c>catch { }</c>. An
        /// unresolvable SVG path left a coloured shape with no glyph and no indication anything was
        /// wrong — which for a validation badge is worse than cosmetic, since an error badge that
        /// silently loses its glyph looks like a success badge in the wrong colour, and communicating
        /// state at a glance is the control's entire job.
        /// </para>
        /// <para>
        /// The catch stays, because this runs inside <c>OnPaint</c>: an exception escaping a paint
        /// handler leaves the region invalid, so the next <c>WM_PAINT</c> throws again and the failure
        /// loops. What changes is that the failure is <b>reported once per path</b> rather than
        /// discarded, and that it <b>renders as broken</b> instead of as a deliberate blank.
        /// </para>
        /// </remarks>
        protected void DrawIconOrFallback(Graphics g, Rectangle iconRect, string iconPath)
        {
            if (string.IsNullOrEmpty(iconPath)) return;

            // Catching is not enough on its own: StyledImagePainter.Paint writes a Debug line and
            // RETURNS when it cannot resolve an image. It never throws, so the bare `catch { }` this
            // replaced could not have caught anything - the icon simply vanished and the badge rendered
            // as a plain coloured shape. Resolvability has to be asked about, not inferred from a throw.
            if (!IconResolves(iconPath))
            {
                ReportIconFailure(iconPath, "no image could be resolved for that path");
                DrawMissingGlyph(g, iconRect);
                return;
            }

            try
            {
                StyledImagePainter.Paint(g, iconRect, iconPath);
                if (_failedIconPath == iconPath) _failedIconPath = null;
            }
            catch (Exception ex)
            {
                // Kept for the case where resolution succeeds and rendering still fails. Letting it
                // escape would be worse than swallowing: a throwing paint handler leaves the region
                // invalid, so the next WM_PAINT throws again and the failure loops.
                ReportIconFailure(iconPath, $"{ex.GetType().Name}: {ex.Message}");
                DrawMissingGlyph(g, iconRect);
            }
        }

        /// <summary>Whether an icon path resolves to an image, cached per path.</summary>
        private bool IconResolves(string iconPath)
        {
            if (_resolvedIconPath == iconPath) return _resolvedIconOk;

            bool ok;
            try
            {
                using var probe = new ImagePainter(iconPath);
                ok = probe.HasImage;
            }
            catch (Exception ex)
            {
                // The probe itself failing is a different fact from "the path does not resolve", and
                // the caller cannot tell them apart from the return value alone.
                BeepLog.FailureOnce(iconPath, this, $"probe icon '{iconPath}'", ex);
                ok = false;
            }

            _resolvedIconPath = iconPath;
            _resolvedIconOk = ok;
            return ok;
        }

        /// <summary>Reports once per path, not once per paint - a pulsing badge repaints every 40ms.</summary>
        private void ReportIconFailure(string iconPath, string reason)
        {
            if (_failedIconPath == iconPath) return;

            _failedIconPath = iconPath;
            BeepLog.Error(this, $"render icon '{iconPath}'", reason);
        }

        /// <summary>A cross, so a badge that lost its icon does not look like one that never had one.</summary>
        private void DrawMissingGlyph(Graphics g, Rectangle bounds)
        {
            using var pen = new Pen(BadgeForeColor, Math.Max(1f, bounds.Width / 10f));
            int inset = bounds.Width / 4;
            var box = Rectangle.Inflate(bounds, -inset, -inset);

            g.DrawLine(pen, box.Left, box.Top, box.Right, box.Bottom);
            g.DrawLine(pen, box.Right, box.Top, box.Left, box.Bottom);
        }

        private Func<Rectangle, GraphicsPath>? _customShapeProvider;

        /// <summary>
        /// Builds the badge's outline when <see cref="Shape"/> is <see cref="BadgeShape.Custom"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <c>BadgeShape.Custom</c> had no hook: <c>GetShapePath</c> handled Circle, RoundedSquare,
        /// Pill and Diamond, and everything else fell to a <c>default:</c> that returned a rectangle.
        /// Measured, a Custom badge rendered <b>pixel-identical</b> to a Rectangle one — the enum member
        /// promising "I supply my own shape" silently gave you the default.
        /// </para>
        /// <para>
        /// This mirrors <see cref="BadgeLocation.BoundsProvider"/>, which is the same idea for position
        /// and already worked. That asymmetry was the argument for adding the hook rather than deleting
        /// the member: <c>BadgeAnchor.Custom</c> was real and <c>BadgeShape.Custom</c> was not.
        /// </para>
        /// <para>Left null, Custom falls back to a rectangle rather than throwing.</para>
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<Rectangle, GraphicsPath>? CustomShapeProvider
        {
            get => _customShapeProvider;
            set
            {
                _customShapeProvider = value;
                if (_shape == BadgeShape.Custom) Invalidate();
            }
        }

        /// <summary>
        /// Width the badge's content needs, or 0 when it has none.
        /// </summary>
        /// <remarks>
        /// Overridden by the badges that draw text. The base class deliberately does not measure:
        /// only the subclass knows what it is about to draw, and a dot has nothing to measure.
        /// </remarks>
        protected virtual int MeasureContentWidth() => 0;

        /// <summary>Whether this shape is square by definition.</summary>
        /// <remarks>
        /// A circle with a long label should grow as a circle or the caller should have picked a pill —
        /// silently turning one shape into another because the text got longer is worse than clipping.
        /// </remarks>
        private bool IsSquareShape => _shape is BadgeShape.Circle or BadgeShape.Diamond;

        /// <summary>
        /// Sizes the badge: <see cref="BadgeDiameter"/> is the height, and the width grows to fit the
        /// content for shapes that are allowed to be oblong.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This class used to force <c>Size = new Size(diameter, diameter)</c>, so a
        /// <c>BeepTextBadge("NEW")</c> with <c>Shape = Pill</c> measured 18x18 — a pill exactly as wide
        /// as it is tall is a circle, and the shape existed to hold a word it could not hold.
        /// </para>
        /// <para>
        /// <c>MaximumSize</c> capped both axes at 48px, so setting <c>Width</c> by hand did not help
        /// either: WinForms clamped it straight back. The cap now applies to height, where a limit on a
        /// decoration makes sense, and not to width, where it made "IN PROGRESS" impossible.
        /// </para>
        /// </remarks>
        protected void ApplyBadgeSize()
        {
            int height = _badgeDiameter;
            int width = height;

            if (!IsSquareShape)
            {
                int content = MeasureContentWidth();
                if (content > 0) width = Math.Max(height, content + ContentPaddingX * 2);
            }

            if (Width == width && Height == height) return;

            Size = new Size(width, height);
            Reposition();
            Invalidate();
        }

        /// <summary>
        /// Centres the badge on the anchored edge or corner so it overhangs by half its size.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This reads the anchor rather than inferring it from where the computed bounds landed. The
        /// inference was wrong for the middle anchors: a <c>MiddleLeft</c> badge sits above the
        /// target's vertical centre, which the old code read as "top", so it pulled the badge up to the
        /// target's top edge. Two <c>if</c> blocks with empty bodies and comments saying middle anchors
        /// are not shifted were the previous attempt at this, and they did nothing.
        /// </para>
        /// <para>
        /// A middle anchor now overhangs only on the axis it names, and stays centred on the other.
        /// </para>
        /// </remarks>
        private Rectangle ApplyCornerOverlap(Rectangle bounds, Rectangle target)
        {
            int halfW = bounds.Width / 2;
            int halfH = bounds.Height / 2;

            int centreX = target.Left + (target.Width - bounds.Width) / 2;
            int centreY = target.Top + (target.Height - bounds.Height) / 2;

            int left = Location.Anchor switch
            {
                BadgeAnchor.TopLeft or BadgeAnchor.BottomLeft or BadgeAnchor.MiddleLeft => target.Left - halfW,
                BadgeAnchor.TopRight or BadgeAnchor.BottomRight or BadgeAnchor.MiddleRight => target.Right - halfW,
                BadgeAnchor.TopCenter or BadgeAnchor.BottomCenter or BadgeAnchor.MiddleCenter => centreX,
                _ => bounds.X,
            };

            int top = Location.Anchor switch
            {
                BadgeAnchor.TopLeft or BadgeAnchor.TopRight or BadgeAnchor.TopCenter => target.Top - halfH,
                BadgeAnchor.BottomLeft or BadgeAnchor.BottomRight or BadgeAnchor.BottomCenter => target.Bottom - halfH,
                BadgeAnchor.MiddleLeft or BadgeAnchor.MiddleRight or BadgeAnchor.MiddleCenter => centreY,
                _ => bounds.Y,
            };

            // Custom anchors - RelativePosition and BoundsProvider - place the badge themselves. Corner
            // overlap has no corner to apply to, so it leaves them where they asked to be.
            if (Location.Anchor == BadgeAnchor.Custom || Location.BoundsProvider is not null)
                return bounds;

            return new Rectangle(left + Location.Offset.X, top + Location.Offset.Y, bounds.Width, bounds.Height);
        }
    }
}
