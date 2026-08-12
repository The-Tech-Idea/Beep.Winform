using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.ToolTips;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    /// <summary>
    /// Tooltips for the caption's glyph-only buttons.
    /// </summary>
    /// <remarks>
    /// Seven caption buttons render as a glyph and nothing else. A user who does not recognise the
    /// glyph has no way to learn what it does - no label, no tooltip, and (before stage 03) no
    /// accessible name a screen reader could read out either.
    ///
    /// The text comes from <c>GetCaptionAccessibilityName</c> - the same method the UIA child uses.
    /// That is deliberate: a tooltip and a screen reader that disagree about what a button does is
    /// worse than either alone, and keeping one source makes disagreement impossible rather than
    /// merely unlikely. It also means the maximize button's tooltip already says "restore" when the
    /// window is maximized, because that method varies by WindowState.
    /// </remarks>
    public partial class BeepiFormPro
    {
        private bool _showCaptionToolTips = true;
        private System.Windows.Forms.Timer _captionToolTipTimer;
        private string _captionToolTipKey;
        private string _pendingToolTipArea;

        [Category("Beep Appearance")]
        [DefaultValue(true)]
        [Description("Show a tooltip when hovering a caption button. The text is the button's accessible name, so the tooltip and a screen reader can never disagree.")]
        public bool ShowCaptionToolTips
        {
            get => _showCaptionToolTips;
            set
            {
                if (_showCaptionToolTips == value) return;
                _showCaptionToolTips = value;
                if (!value) HideCaptionToolTip();
            }
        }

        /// <summary>Called by the interaction manager when the hovered caption region changes.</summary>
        internal void OnCaptionHoverChanged(HitArea oldHover, HitArea newHover)
        {
            if (!_showCaptionToolTips || DesignMode) return;

            HideCaptionToolTip();

            if (newHover == null || string.IsNullOrWhiteSpace(newHover.Name)) return;

            string key = BeepiFormProHitAreaManager.NormalizeName(newHover.Name);
            if (key == FormHitAreaNames.Caption ||
                key == FormHitAreaNames.Title ||
                key == FormHitAreaNames.Icon ||
                key == FormHitAreaNames.Search)      // the search box explains itself
                return;

            _pendingToolTipArea = newHover.Name;

            if (_captionToolTipTimer == null)
            {
                _captionToolTipTimer = new System.Windows.Forms.Timer
                {
                    Interval = Math.Max(1, ToolTipManager.Instance.DefaultShowDelay)
                };
                _captionToolTipTimer.Tick += (s, e) =>
                {
                    _captionToolTipTimer.Stop();
                    ShowCaptionToolTip(_pendingToolTipArea);
                };
            }
            _captionToolTipTimer.Stop();
            _captionToolTipTimer.Start();
        }

        private void ShowCaptionToolTip(string areaName)
        {
            if (!_showCaptionToolTips || string.IsNullOrWhiteSpace(areaName) || IsDisposed) return;

            try
            {
                var area = _hits?.GetHitArea(areaName);
                if (area == null || area.Bounds.IsEmpty) return;

                string text = GetCaptionAccessibilityName(areaName);
                if (string.IsNullOrWhiteSpace(text)) return;

                var below = RectangleToScreen(area.Bounds);
                var at = new Point(below.Left + below.Width / 2, below.Bottom + 6);

                // Fire and forget, but never silently: a tooltip that fails to appear is a small
                // problem, and a tooltip failure that is invisible to the developer is a larger one.
                _ = ToolTipManager.Instance.ShowTooltipAsync(text, at)
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                            BeepLog.FailureOnce("form.caption.tooltip.show", this,
                                $"show the caption tooltip for '{areaName}'", t.Exception);
                        else
                            _captionToolTipKey = t.Result;
                    }, System.Threading.Tasks.TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                BeepLog.FailureOnce("form.caption.tooltip", this,
                    $"prepare the caption tooltip for '{areaName}'", ex);
            }
        }

        private void HideCaptionToolTip()
        {
            _captionToolTipTimer?.Stop();
            _pendingToolTipArea = null;

            string key = _captionToolTipKey;
            if (string.IsNullOrEmpty(key)) return;
            _captionToolTipKey = null;

            try
            {
                _ = ToolTipManager.Instance.HideTooltipAsync(key)
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                            BeepLog.FailureOnce("form.caption.tooltip.hide", this,
                                "hide the caption tooltip", t.Exception);
                    }, System.Threading.Tasks.TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                BeepLog.FailureOnce("form.caption.tooltip.hide", this, "hide the caption tooltip", ex);
            }
        }

        /// <summary>The tooltip text a caption region would show - one source with the UIA name.</summary>
        internal string GetCaptionToolTipText(string areaName) => GetCaptionAccessibilityName(areaName);
    }
}
