using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    /// <summary>
    /// One place the form asks what scale it is drawn at, and one place it reacts to that changing.
    /// </summary>
    /// <remarks>
    /// <para><b>Nothing scaled.</b> <c>FormPainterMetrics</c> has a complete DPI-aware factory -
    /// <c>DefaultFor(style, theme, dpiScaleFactor)</c> scales caption height, button widths, icon
    /// size, spacing, paddings, border widths and corner radii, in one guarded place. It had
    /// **zero callers**. Every real call site used <c>DefaultForCached(style, theme)</c>, the
    /// overload with no DPI in it, so the entire metrics pipeline delivered 96-DPI constants at any
    /// monitor scale. The DPI-aware overloads were dead code that made the pipeline *look* correct
    /// to a reader.</para>
    ///
    /// <para>The six painters that call <c>DpiScalingHelper</c> directly (Linear, MaterialYou,
    /// Modern, NextJS, RadixUI, Shadcn) were therefore not a double-scaling hazard as this stage's
    /// plan assumed - they were the only painters that scaled **at all**. The other 32 ignored DPI
    /// completely. Two skins on the same 150% monitor disagreed about whether DPI exists.</para>
    ///
    /// <para><b>No WM_DPICHANGED.</b> Dragging between monitors left the chrome at the old scale
    /// until something else forced a rebuild.</para>
    ///
    /// <para><see cref="DpiScaleFactor"/> is virtual because <c>Control.DeviceDpi</c> is neither
    /// virtual nor settable (its backing store is a <c>readonly static</c> in this runtime), so
    /// without a seam here no probe can exercise the scaling pipeline at all - the code would be
    /// verifiable only by dragging a window between two physical monitors by hand.</para>
    /// </remarks>
    public partial class BeepiFormPro
    {
        private const int WM_DPICHANGED = 0x02E0;
        private const int WM_GETDPISCALEDSIZE = 0x02E4;

        /// <summary>
        /// The DPI scale this form's chrome is built at: 1.0 at 96 DPI, 1.5 at 144.
        /// </summary>
        /// <remarks>
        /// Every DPI decision in the form goes through here, so there is exactly one answer to
        /// "what scale are we at" and one place to override it.
        /// </remarks>
        protected virtual float DpiScaleFactor => DpiScalingHelper.GetDpiScaleFactor(this);

        /// <summary>
        /// Metrics for an explicit style, scaled for this form's DPI.
        /// </summary>
        /// <remarks>
        /// Every painter had its own GetMetrics calling
        /// <c>DefaultForCached(style, theme)</c> - the overload with no DPI in it - so routing the
        /// FORM's nine call sites through the DPI-aware overload changed nothing that a painter
        /// draws. Thirty-eight near-identical copies of one lookup is also why the DPI hole stayed
        /// invisible: there was no single place to be wrong in.
        /// </remarks>
        internal FormPainterMetrics MetricsFor(FormStyle style) =>
            FormPainterMetrics.DefaultForCached(
                style, UseThemeColors ? _currentTheme : null, DpiScaleFactor, UseThemeColors);

        /// <summary>Metrics for the current style and theme, scaled for this form's DPI.</summary>
        internal FormPainterMetrics CurrentMetrics =>
            FormPainterMetrics.DefaultForCached(
                FormStyle,
                UseThemeColors ? _currentTheme : null,
                DpiScaleFactor,
                UseThemeColors);

        /// <summary>
        /// Rebuilds everything that was measured at the old scale.
        /// </summary>
        private void HandleDpiChanged(Message m)
        {
            try
            {
                // lParam is the rect Windows suggests for the new monitor. Taking it is what keeps
                // the window the same physical size across the move; computing our own would fight
                // the shell's placement.
                if (m.LParam != IntPtr.Zero)
                {
                    var suggested = Marshal.PtrToStructure<RECT>(m.LParam);
                    SetBounds(suggested.left, suggested.top,
                              suggested.right - suggested.left,
                              suggested.bottom - suggested.top,
                              BoundsSpecified.All);
                }

                // Everything below was measured at the old scale.
                FormPainterMetrics.InvalidateCache();
                InvalidateLayout();
                _formpaintermaterics = null;
                ApplyWindowShape(force: true);   // corner radius is a scaled metric
                EnsureLayoutCalculated();
                Invalidate(true);
            }
            catch (Exception ex)
            {
                BeepLog.Failure(this, "rebuild the form chrome for a new monitor DPI", ex);
            }
        }
    }
}
