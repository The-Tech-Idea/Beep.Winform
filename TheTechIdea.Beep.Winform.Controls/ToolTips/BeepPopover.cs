using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
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
            _popoverConfig?.OnSecondaryClick?.Invoke(_popoverConfig.Key);
            Close();
        }

        /// <summary>Confirm and raise the primary-click callback then close.</summary>
        public void ConfirmAndClose()
        {
            _popoverConfig?.OnPrimaryClick?.Invoke(_popoverConfig.Key);
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
        // Click-outside dismiss
        // ──────────────────────────────────────────────────────────────────────

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            if (_popoverConfig?.DismissOnClickOutside ?? true)
                DismissAsCancel();
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
                ForeColor   = isSecondary ? Color.FromArgb(180, 180, 180) : Color.White,
                BackColor   = isSecondary
                              ? Color.Transparent
                              : GetPrimaryButtonColor()
            };
            btn.FlatAppearance.BorderSize = isSecondary ? 0 : 0;
            return btn;
        }

        private Color GetPrimaryButtonColor()
        {
            return _popoverConfig?.PrimaryButtonType switch
            {
                ToolTipType.Error   or ToolTipType.Warning => Color.FromArgb(196, 43, 28),
                ToolTipType.Success                         => Color.FromArgb(16, 124, 16),
                _                                           => Color.FromArgb(0, 120, 212)
            };
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
