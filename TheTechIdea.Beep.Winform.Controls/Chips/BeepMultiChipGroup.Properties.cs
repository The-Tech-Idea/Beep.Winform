using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Chips.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Chips
{
    public partial class BeepMultiChipGroup
    {
        /// <summary>
        /// Switches the painter based on the ChipStyle.
        /// Each ChipStyle has its own distinct painter implementation.
        /// </summary>
        private void SwitchPainter(ChipStyle kind)
        {
            _renderOptions.Style = kind;

            _painter = kind switch
            {
                // Shape-based painters
                ChipStyle.Pill => new PillChipGroupPainter(),
                ChipStyle.Square => new SquareChipGroupPainter(),
                
                // Themed painters
                ChipStyle.Likeable => new LikeableChipGroupPainter(),
                ChipStyle.Ingredient => new IngredientChipGroupPainter(),
                ChipStyle.Avatar => new AvatarChipGroupPainter(),
                
                // Effect-based painters
                ChipStyle.Elevated => new ElevatedChipGroupPainter(),
                ChipStyle.Shaded => new ShadedChipGroupPainter(),
                ChipStyle.Colorful => new ColorfulChipGroupPainter(),
                ChipStyle.Soft => new SoftChipGroupPainter(),
                ChipStyle.Smooth => new SmoothChipGroupPainter(),
                
                // Style-based painters
                ChipStyle.Modern => new ModernChipGroupPainter(),
                ChipStyle.Classic => new OutlinedChipGroupPainter(),
                ChipStyle.Professional => new OutlinedChipGroupPainter(),
                ChipStyle.HighContrast => new HighContrastChipGroupPainter(),
                ChipStyle.Minimalist => new TextChipGroupPainter(),
                
                // Border-based painters
                ChipStyle.Dashed => new DashedChipGroupPainter(),
                ChipStyle.Bold => new BoldChipGroupPainter(),
                
                // Default
                ChipStyle.Default or _ => new DefaultChipGroupPainter(),
            };

            _painter.Initialize(this, GetEffectiveTheme());
            SyncRenderOptions();
            UpdateChipBounds();
            Invalidate();
        }

        private int _focusedIndex = -1;

        /// <summary>
        /// Gets or sets the currently focused chip index (for keyboard navigation)
        /// </summary>
        [Browsable(false)]
        public int FocusedIndex
        {
            get => _focusedIndex;
            set
            {
                if (_focusedIndex != value && value >= -1 && value < _chips.Count)
                {
                    _focusedIndex = value;
                    Invalidate();
                }
            }
        }

        // ── Drag-and-drop reorder ─────────────────────────────────────
        /// <summary>
        /// When true, chips can be reordered by dragging.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Allow chips to be reordered by drag-and-drop.")]
        public bool AllowDragReorder
        {
            get => _allowDragReorder;
            set => _allowDragReorder = value;
        }

        // ── Inline chip editing ───────────────────────────────────────
        /// <summary>
        /// When true, double-clicking a chip allows inline text editing.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Allow inline editing of chip text by double-clicking.")]
        public bool AllowInlineEditing { get; set; } = true;

        // ── Chip input / creation ─────────────────────────────────────
        /// <summary>
        /// When true, shows an input field at the end of the chip group for creating new chips.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Show an input field for creating new chips by typing.")]
        public bool AllowChipCreation
        {
            get => _allowChipCreation;
            set
            {
                if (_allowChipCreation != value)
                {
                    _allowChipCreation = value;
                    if (value)
                        EnsureInputTextBox();
                    else
                        RemoveInputTextBox();
                    UpdateChipBounds();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Placeholder text shown in the chip input field.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("Type to add...")]
        [Description("Placeholder text for the chip input field.")]
        public string InputPlaceholderText
        {
            get => _inputPlaceholderText;
            set { _inputPlaceholderText = value; if (_inputTextBox != null) _inputTextBox.Refresh(); }
        }

        // ── Ripple animation ──────────────────────────────────────────
        /// <summary>
        /// When true, shows a ripple animation on chip click (Material Design style).
        /// </summary>
        [Browsable(true)]
        [Category("Animation")]
        [DefaultValue(true)]
        [Description("Show ripple animation on chip click.")]
        public bool EnableRipple
        {
            get => _enableRipple;
            set
            {
                _enableRipple = value;
                if (!value) _activeRipples.Clear();
            }
        }

        // ── Tooltip ───────────────────────────────────────────────────
        /// <summary>
        /// When true, shows a tooltip with the full chip text when hovering over truncated chips.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Show tooltip on hover for truncated chip text.")]
        public bool ShowTooltip
        {
            get => _showTooltip;
            set
            {
                _showTooltip = value;
                if (!value) HideTooltip();
            }
        }

        // ── Add/remove animation ──────────────────────────────────────
        /// <summary>
        /// When true, animates chips when they are added or removed.
        /// </summary>
        [Browsable(true)]
        [Category("Animation")]
        [DefaultValue(true)]
        [Description("Animate chips when added or removed.")]
        public bool EnableAddRemoveAnimation
        {
            get => _enableAddRemoveAnimation;
            set => _enableAddRemoveAnimation = value;
        }

        // ── Utility row ───────────────────────────────────────────────
        /// <summary>
        /// When true, shows the Select All / Clear All utility row.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Show the Select All / Clear All utility row.")]
        public bool ShowUtilityRow
        {
            get => _showUtilityRow;
            set
            {
                if (_showUtilityRow != value)
                {
                    _showUtilityRow = value;
                    UpdateChipBounds();
                    Invalidate();
                }
            }
        }
    }
}
