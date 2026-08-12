using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    /// <summary>
    /// The one place that decides what shape this window is and how that shape gets applied.
    /// </summary>
    /// <remarks>
    /// Two methods used to own the shape independently: <c>UpdateFormRegion</c> set the managed
    /// <see cref="Form.Region"/> from a rounded-rect path, and <c>UpdateWindowRegion</c> set a Win32
    /// region through <c>SetWindowRgn</c>. Same corner radius, two copies of the arithmetic, called
    /// in pairs from six places. That duplication is why the opt-out for a derived silhouette had to
    /// be added twice before it held, and why a derivative could not own its own shape at all.
    ///
    /// The shape is now decided once here and applied through one of two backends:
    ///
    /// - **Dwm** - Windows 11 rounds the window itself. No region is set, so the corners are
    ///   antialiased and the window keeps its native drop shadow. A GDI region has neither: region
    ///   edges are hard-clipped (visibly stair-stepped) and setting one suppresses the shadow. That
    ///   pair - jagged corners, no shadow - is the most obvious visual tell against a commercial
    ///   suite, and it is not something painting can fix from inside the window.
    /// - **Region** - everywhere else, and whenever the radius is one DWM cannot express (DWM
    ///   offers only "round" and "small round", not an arbitrary radius, and nothing per-corner).
    ///
    /// The form is <c>FormBorderStyle.None</c> but adds <c>WS_SIZEBOX</c> (= WS_THICKFRAME) in
    /// CreateParams, which is what makes it eligible for DWM rounding at all; a plain borderless
    /// popup is not.
    /// </remarks>
    public partial class BeepiFormPro
    {
        internal enum WindowShapeBackend
        {
            /// <summary>A derivative owns the silhouette; the base applies nothing.</summary>
            Derived,
            /// <summary>Windows 11 rounds the window: antialiased corners, native shadow, no region.</summary>
            Dwm,
            /// <summary>A GDI region: exact radius, hard edges, no shadow.</summary>
            Region
        }

        private const uint DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private const uint DWMWCP_DEFAULT = 0;
        private const uint DWMWCP_DONOTROUND = 1;
        private const uint DWMWCP_ROUND = 2;        // ~8px
        private const uint DWMWCP_ROUNDSMALL = 3;   // ~4px

        private static bool? _dwmRoundingSupported;

        /// <summary>Windows 11 (build 22000) is where DWMWA_WINDOW_CORNER_PREFERENCE appears.</summary>
        internal static bool DwmRoundingSupported =>
            _dwmRoundingSupported ??= Environment.OSVersion.Platform == PlatformID.Win32NT
                                      && Environment.OSVersion.Version.Build >= 22000;

        /// <summary>The corner radius, from the one source that has it: the active painter.</summary>
        private CornerRadius GetShapeRadius()
        {
            if (ActivePainter == null) ApplyFormStyle();
            return ActivePainter?.GetCornerRadius(this) ?? new CornerRadius(0);
        }

        /// <summary>
        /// Which backend applies the current shape. DWM only when it can express the radius
        /// exactly enough to be an improvement rather than a different wrong shape.
        /// </summary>
        internal WindowShapeBackend ShapeBackend
        {
            get
            {
                if (OwnsWindowShape) return WindowShapeBackend.Derived;
                if (!DwmRoundingSupported || !PreferSystemRoundedCorners) return WindowShapeBackend.Region;

                var r = GetShapeRadius();
                bool uniform = r.TopLeft == r.TopRight && r.TopLeft == r.BottomLeft && r.TopLeft == r.BottomRight;
                if (!uniform) return WindowShapeBackend.Region;

                // DWM has two radii and no third option. Outside the range they approximate, a
                // region is the honest choice - a 24px card rounded to 8px by the system is not
                // "close enough", it is a different design.
                return r.TopLeft == 0 || (r.TopLeft >= 2 && r.TopLeft <= 12)
                    ? WindowShapeBackend.Dwm
                    : WindowShapeBackend.Region;
            }
        }

        private bool _preferSystemRoundedCorners = true;

        /// <summary>
        /// Let Windows 11 round the window (antialiased corners + native drop shadow) when it can
        /// express the painter's radius. Set false to force the GDI region path everywhere.
        /// </summary>
        [System.ComponentModel.Category("Beep Appearance")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("On Windows 11, let the system round the window: antialiased corners and a native drop shadow. Falls back to a region when the radius cannot be expressed.")]
        public bool PreferSystemRoundedCorners
        {
            get => _preferSystemRoundedCorners;
            set
            {
                if (_preferSystemRoundedCorners == value) return;
                _preferSystemRoundedCorners = value;
                if (IsHandleCreated) ApplyWindowShape(force: true);
            }
        }

        // CornerRadius is a CLASS, so a tuple holding one compares by reference and would never
        // match - the cache would silently rebuild the shape every call. Key on the four numbers.
        private (Size size, int tl, int tr, int bl, int br, WindowShapeBackend backend)? _appliedShape;

        /// <summary>
        /// Applies the current shape through the chosen backend. Idempotent: the six call sites
        /// that used to call UpdateWindowRegion + UpdateFormRegion back to back now do the work
        /// once, because a shape that has not changed is not rebuilt.
        /// </summary>
        internal void ApplyWindowShape(bool force = false)
        {
            if (!IsHandleCreated || Width <= 0 || Height <= 0) return;

            var backend = ShapeBackend;
            if (backend == WindowShapeBackend.Derived) return;

            var radius = GetShapeRadius();
            var key = (Size, radius.TopLeft, radius.TopRight, radius.BottomLeft, radius.BottomRight, backend);
            if (!force && _appliedShape.HasValue && _appliedShape.Value.Equals(key)) return;
            _appliedShape = key;

            try
            {
                if (backend == WindowShapeBackend.Dwm)
                {
                    // No region at all - that is the entire point. A region here would clip away
                    // the antialiasing and the shadow the system is about to provide.
                    ReleaseManagedRegion();
                    SetWindowRgn(Handle, IntPtr.Zero, true);

                    uint pref = radius.TopLeft == 0 ? DWMWCP_DONOTROUND
                              : radius.TopLeft <= 5 ? DWMWCP_ROUNDSMALL
                              : DWMWCP_ROUND;
                    DwmSetWindowAttribute(Handle, DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, sizeof(uint));
                }
                else
                {
                    // Region backend: tell DWM to stop rounding, or the system radius and ours
                    // fight and the corner shows both.
                    uint pref = DWMWCP_DONOTROUND;
                    if (DwmRoundingSupported)
                        DwmSetWindowAttribute(Handle, DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, sizeof(uint));

                    UpdateWindowRegion();
                    UpdateFormRegion();
                }
            }
            catch (Exception ex)
            {
                _appliedShape = null;   // do not remember a shape that failed to apply
                BeepLog.FailureOnce("form.shape.apply", this,
                    $"apply the window shape through the {backend} backend", ex);
            }
        }

        // A HasSystemDropShadow() probe lived here and was deleted, not fixed.
        //
        // It compared DWMWA_EXTENDED_FRAME_BOUNDS against the window rect. Measured on Windows
        // build 26200, both backends report the frame EXACTLY equal to the window rect
        // (L0 T0 R0 B0), on-screen and off - so the attribute cannot tell a rounded DWM window
        // from a region-clipped one for a borderless WS_THICKFRAME form. It answered "no shadow"
        // for both, which is not an answer, and a method that cannot distinguish what it is named
        // for is worse than none: the next reader would trust it.
        //
        // What the probe DOES verify is the mechanism: under the Dwm backend neither the managed
        // Region nor SetWindowRgn is set. That a region suppresses the shadow and hard-clips the
        // corners is documented behaviour, not something this repo has measured - see
        // plans/02-geometry-and-invalidate.md, which says so plainly rather than implying the
        // shadow was checked.
    }
}
