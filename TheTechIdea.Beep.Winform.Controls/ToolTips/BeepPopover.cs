using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Sprint 3 — Persistent popover panel that stays open until explicitly dismissed.
    /// Supports Click / Focus / Hover / Manual trigger modes, optional action buttons,
    /// dismiss-on-click-outside, and dismiss-on-Escape.
    ///
    /// Usage:
    /// <code>
    ///   var cfg = new PopoverConfig { Text = "Are you sure?", PrimaryButtonText = "Yes",
    ///                                  SecondaryButtonText = "Cancel", TriggerMode = ToolTipTriggerMode.Click };
    ///   await ToolTipManager.Instance.ShowPopoverAsync(myButton, cfg);
    /// </code>
    /// </summary>
    public class BeepPopover : CustomToolTip
    {
        // ──────────────────────────────────────────────────────────────────────
        // Fields
        // ──────────────────────────────────────────────────────────────────────
        private PopoverConfig    _popoverConfig;
        private Button           _primaryBtn;
        private Button           _secondaryBtn;
        private bool             _isMounted;
        private OutsideClickMessageFilter _outsideClickFilter;   // C5

        // ──────────────────────────────────────────────────────────────────────
        // Constructor
        // ──────────────────────────────────────────────────────────────────────

        public BeepPopover() : base()
        {
            // Popovers are not auto-dismissed by duration
            // Don't remove owner-form registration
        }

        // ──────────────────────────────────────────────────────────────────────
        // Public API
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Apply a <see cref="PopoverConfig"/> and mount action buttons if required.</summary>
        public void ApplyPopoverConfig(PopoverConfig cfg)
        {
            _popoverConfig = cfg ?? throw new ArgumentNullException(nameof(cfg));

            // Disable auto-hide timer — popovers persist until dismissed
            if (cfg.Duration > 0) cfg.Duration = 0;

            // Keep open when mouse moves onto the popover itself
            cfg.PersistOnHover = true;

            Configure(cfg);
            MountActionButtons();
        }

        /// <summary>Dismiss the popover and raise the secondary-click callback.</summary>
        public void DismissAsCancel()
        {
            _popoverConfig?.OnSecondaryClick?.Invoke();
            Close();
        }

        /// <summary>Confirm and raise the primary-click callback then close.</summary>
        public void ConfirmAndClose()
        {
            _popoverConfig?.OnPrimaryClick?.Invoke();
            Close();
        }

        // ──────────────────────────────────────────────────────────────────────
        // Keyboard: Escape dismisses
        // ──────────────────────────────────────────────────────────────────────

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape && (_popoverConfig?.DismissOnEscape ?? true))
            {
                DismissAsCancel();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Click-outside dismiss (C5)
        // ──────────────────────────────────────────────────────────────────────

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // Install a global message filter that fires DismissAsCancel
            // when the user clicks anywhere outside the popover's bounds.
            // OnDeactivate used to do this but it fired for clicks on the
            // popover's own buttons (since the click moved focus), causing
            // the action handlers to race with the dismiss handler.
            if (_popoverConfig?.DismissOnClickOutside ?? true)
            {
                _outsideClickFilter = new OutsideClickMessageFilter(this, DismissAsCancel);
                Application.AddMessageFilter(_outsideClickFilter);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            RemoveOutsideClickFilter();
            base.OnClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            // Also unhook here, not only in OnClosing. A Form disposed directly — by its parent,
            // or by an explicit Dispose() — never raises Closing, which left the filter installed
            // in Application's process-wide list forever.
            if (disposing) RemoveOutsideClickFilter();
            base.Dispose(disposing);
        }

        private void RemoveOutsideClickFilter()
        {
            if (_outsideClickFilter == null) return;
            Application.RemoveMessageFilter(_outsideClickFilter);
            _outsideClickFilter = null;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            // C5: keep no-op base. Dismissal is driven by OutsideClickMessageFilter.
            base.OnDeactivate(e);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Action buttons
        // ──────────────────────────────────────────────────────────────────────

        private void MountActionButtons()
        {
            // Remove previously mounted buttons
            _primaryBtn?.Dispose();
            _secondaryBtn?.Dispose();
            _primaryBtn   = null;
            _secondaryBtn = null;

            bool hasPrimary   = !string.IsNullOrEmpty(_popoverConfig.PrimaryButtonText);
            bool hasSecondary = !string.IsNullOrEmpty(_popoverConfig.SecondaryButtonText);

            if (!hasPrimary && !hasSecondary) return;

            int btnH       = 28;
            int btnPadding = 8;

            if (hasSecondary)
            {
                _secondaryBtn = CreateButton(
                    _popoverConfig.SecondaryButtonText,
                    isSecondary: true);
                _secondaryBtn.Click += (s, e) => DismissAsCancel();
                Controls.Add(_secondaryBtn);
            }

            if (hasPrimary)
            {
                _primaryBtn = CreateButton(
                    _popoverConfig.PrimaryButtonText,
                    isSecondary: false);
                _primaryBtn.Click += (s, e) => ConfirmAndClose();
                Controls.Add(_primaryBtn);
            }

            // Position buttons on next layout
            // C6: unsubscribe first to prevent duplicate subscription on re-ApplyPopoverConfig
            Layout -= PositionButtons;
            Layout += PositionButtons;
        }

        private void PositionButtons(object sender, LayoutEventArgs e)
        {
            int margin = 8;
            int btnH   = 28;
            int x      = Width - margin;

            if (_primaryBtn != null)
            {
                _primaryBtn.Size     = new Size(80, btnH);
                x                   -= 80;
                _primaryBtn.Location = new Point(x, Height - btnH - margin);
                x                   -= margin + 4;
            }

            if (_secondaryBtn != null)
            {
                int w = TextRenderer.MeasureText(_secondaryBtn.Text,
                        _secondaryBtn.Font).Width + 16;
                _secondaryBtn.Size     = new Size(w, btnH);
                x                     -= w;
                _secondaryBtn.Location = new Point(x, Height - btnH - margin);
            }
        }

        private Button CreateButton(string text, bool isSecondary)
        {
            var btn = new Button
            {
                Text        = text,
                FlatStyle   = FlatStyle.Flat,
                Cursor      = Cursors.Hand,
                Font        = new Font("Segoe UI", 9f),
                ForeColor   = isSecondary
                              ? Color.FromArgb(200, Helpers.ToolTipThemeHelpers.GetToolTipForeColor(BeepThemesManager.CurrentTheme, ToolTipType.Default))
                              : GetPrimaryButtonForeColor(),
                BackColor   = isSecondary
                              ? Color.Transparent
                              : GetPrimaryButtonColor()
            };
            btn.FlatAppearance.BorderSize = isSecondary ? 0 : 0;
            return btn;
        }

        private Color GetPrimaryButtonColor()
        {
            var t = BeepThemesManager.CurrentTheme;
            return _popoverConfig?.PrimaryButtonType switch
            {
                ToolTipType.Error   => t.ErrorColor,
                ToolTipType.Warning => t.WarningColor,
                ToolTipType.Success => t.SuccessColor,
                _                   => t.PrimaryColor
            };
        }

        private Color GetPrimaryButtonForeColor()
        {
            Color fill = GetPrimaryButtonColor();
            return fill.GetBrightness() > 0.55f ? Color.Black : Color.White;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Forward Configure() — CustomToolTip exposes this via Methods partial
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Calls the underlying CustomToolTip ApplyConfig method.
        /// Wrapped here so subclasses don't need to cast.
        /// </summary>
        private void Configure(ToolTipConfig cfg) => ApplyConfig(cfg);
    }
}
