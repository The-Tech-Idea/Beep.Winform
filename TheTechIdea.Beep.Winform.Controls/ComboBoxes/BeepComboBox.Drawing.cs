using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepComboBox
    {
        #region Drawing
        
        /// <summary>
        /// DrawContent override - called by BaseControl
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            Paint(g, DrawingRect);
        }

        /// <summary>
        /// Draw override - called by BeepGridPro and containers
        /// </summary>
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            Paint(graphics, rectangle);
        }

        /// <summary>
        /// Main paint function - centralized painting logic
        /// Called from both DrawContent and Draw
        /// </summary>
        private void Paint(Graphics g, Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            UpdateLayout();
            
            // Ensure painter exists for current type
            if (_comboBoxPainter == null)
            {
                _comboBoxPainter = CreatePainter(_comboBoxType);
                _comboBoxPainter.Initialize(this, _currentTheme);
            }
            
            // Update layout if needed
            if (_needsLayoutUpdate)
            {
                UpdateLayout();
                _needsLayoutUpdate = false;
            }
            
            // Let the combo box painter draw everything
            _comboBoxPainter.Paint(g, this, bounds);
            
            // Register hit areas for interaction
            RegisterHitAreas();
        }
        
        #endregion
        
        #region Painter Factory
        
        /// <summary>
        /// Creates the appropriate painter for the specified ComboBoxType
        /// </summary>
        private IComboBoxPainter CreatePainter(ComboBoxType type)
        {
            return type switch
            {
                ComboBoxType.Standard => new StandardComboBoxPainter(),
                ComboBoxType.Minimal => new MinimalComboBoxPainter(),
                ComboBoxType.Outlined => new OutlinedComboBoxPainter(),
                ComboBoxType.Rounded => new RoundedComboBoxPainter(),
                ComboBoxType.MaterialOutlined => new MaterialOutlinedComboBoxPainter(),
                ComboBoxType.Filled => new FilledComboBoxPainter(),
                ComboBoxType.Borderless => new BorderlessComboBoxPainter(),
                ComboBoxType.BlueDropdown => new BlueDropdownPainter(),
                ComboBoxType.GreenDropdown => new GreenDropdownPainter(),
                ComboBoxType.Inverted => new InvertedComboBoxPainter(),
                ComboBoxType.Error => new ErrorComboBoxPainter(),
                ComboBoxType.MultiSelectChips => new MultiSelectChipsPainter(),
                ComboBoxType.SearchableDropdown => new SearchableDropdownPainter(),
                ComboBoxType.WithIcons => new WithIconsComboBoxPainter(),
                ComboBoxType.Menu => new MenuComboBoxPainter(),
                ComboBoxType.CountrySelector => new CountrySelectorPainter(),
                ComboBoxType.SmoothBorder => new SmoothBorderPainter(),
                ComboBoxType.DarkBorder => new DarkBorderPainter(),
                ComboBoxType.PillCorners => new PillCornersComboBoxPainter(),
                _ => new StandardComboBoxPainter()
            };
        }
        
        #endregion
        
        #region Hit Area Registration
        
        /// <summary>
        /// Registers interactive hit areas using BaseControl's hit test system
        /// </summary>
        private void RegisterHitAreas()
        {
            // Clear previous hit areas
            ClearHitList();
            
            // Register dropdown button hit area
            if (!_dropdownButtonRect.IsEmpty)
            {
                AddHitArea("DropdownButton", _dropdownButtonRect, null, TogglePopup);
            }
            
            // Register text area hit area (for editable mode or focus)
            if (!_textAreaRect.IsEmpty)
            {
                AddHitArea("TextArea", _textAreaRect, null, () =>
                {
                    if (IsEditable)
                    {
                        StartEditing();
                    }
                    else
                    {
                        Focus();
                    }
                });
            }
        }
        
        #endregion
    }
}
