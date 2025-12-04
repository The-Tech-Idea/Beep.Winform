using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;
using TheTechIdea.Beep.Report;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepSwitch - Properties and Events
    /// </summary>
    public partial class BeepSwitch
    {
        #region Events

        /// <summary>
        /// Occurs when the Checked property value changes.
        /// </summary>
        public event EventHandler? CheckedChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether the switch is On (Checked) or Off (Unchecked).
        /// </summary>
        [Category("Behavior")]
        [Description("Indicates whether the switch is On (Checked) or Off (Unchecked).")]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    AnimateToggle(value);
                    OnCheckedChanged();
                }
            }
        }

        /// <summary>
        /// Orientation of the switch (Horizontal or Vertical).
        /// </summary>
        [Category("Appearance")]
        [Description("Orientation of the switch (Horizontal or Vertical).")]
        [DefaultValue(SwitchOrientation.Horizontal)]
        public SwitchOrientation Orientation
        {
            get => _orientation;
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Label text for the On state.
        /// </summary>
        [Category("Appearance")]
        [Description("Label for the On state.")]
        public string OnLabel
        {
            get => _onLabel;
            set
            {
                _onLabel = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Label text for the Off state.
        /// </summary>
        [Category("Appearance")]
        [Description("Label for the Off state.")]
        public string OffLabel
        {
            get => _offLabel;
            set
            {
                _offLabel = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Background image path for the On state (legacy - use OnIconName for icon library).
        /// The image is loaded via a BeepImage instance and drawn clipped inside the capsule.
        /// </summary>
        [Category("Appearance")]
        [Description("Background image for the On state. This image is drawn and clipped inside the capsule track.")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string? OnImagePath
        {
            get => _onImagePath;
            set
            {
                _onImagePath = value;
                _onBeepImage.ImagePath = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Background image path for the Off state (legacy - use OffIconName for icon library).
        /// The image is loaded via a BeepImage instance and drawn clipped inside the capsule.
        /// </summary>
        [Category("Appearance")]
        [Description("Background image for the Off state. This image is drawn and clipped inside the capsule track.")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string? OffImagePath
        {
            get => _offImagePath;
            set
            {
                _offImagePath = value;
                _offBeepImage.ImagePath = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Icon name from icon library for On state (e.g., 'check', 'power', 'lightbulb').
        /// Uses IconsManagement.SvgsUI to resolve icon paths.
        /// </summary>
        [Category("Appearance")]
        [Description("Icon name from icon library for On state (e.g., 'check', 'power', 'lightbulb')")]
        public string OnIconName
        {
            get => _onIconName;
            set
            {
                _onIconName = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Icon name from icon library for Off state.
        /// Uses IconsManagement.SvgsUI to resolve icon paths.
        /// </summary>
        [Category("Appearance")]
        [Description("Icon name from icon library for Off state")]
        public string OffIconName
        {
            get => _offIconName;
            set
            {
                _offIconName = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Enable drag-to-toggle functionality.
        /// When true, user can drag the thumb to toggle the switch.
        /// </summary>
        [Category("Behavior")]
        [Description("Enable drag-to-toggle functionality")]
        [DefaultValue(true)]
        public bool DragToToggleEnabled
        {
            get => _dragToToggleEnabled;
            set => _dragToToggleEnabled = value;
        }

        // Data binding properties (optional)
        [Browsable(true)]
        [Category("Data")]
        [Description("The property in the control to bind to the data source.")]
        public new string BoundProperty { get; set; } = "Checked";

        [Browsable(true)]
        [Category("Data")]
        [Description("The property in the data source to bind to.")]
        public new string DataSourceProperty { get; set; }

        [Browsable(true)]
        [Category("Data")]
        [Description("The linked property name.")]
        public new string LinkedProperty { get; set; }

        #endregion

        #region Helper Methods for Icons

        /// <summary>
        /// Convenience method: Use checkmark icons (check ON, close OFF).
        /// </summary>
        public void UseCheckmarkIcons()
        {
            OnIconName = "check";
            OffIconName = "close";
        }

        /// <summary>
        /// Convenience method: Use power icons.
        /// </summary>
        public void UsePowerIcons()
        {
            OnIconName = "power";
            OffIconName = "power_off";
        }

        /// <summary>
        /// Convenience method: Use light icons.
        /// </summary>
        public void UseLightIcons()
        {
            OnIconName = "lightbulb";
            OffIconName = "lightbulb_outline";
        }

        #endregion

        #region Event Raisers

        protected virtual void OnCheckedChanged()
        {
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}

