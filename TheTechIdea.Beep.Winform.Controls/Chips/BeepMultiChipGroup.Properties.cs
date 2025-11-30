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

            _painter.Initialize(this, _currentTheme);
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
    }
}
