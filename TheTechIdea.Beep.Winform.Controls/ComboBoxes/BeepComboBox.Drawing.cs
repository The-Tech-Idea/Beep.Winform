using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepComboBox
    {
        #region Drawing Override
        
        /// <summary>
        /// Main drawing method - delegates to painter based on ComboBoxType
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            if (Width <= 0 || Height <= 0) return;
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
            
            // Ensure DrawingRect is available from BaseControl painter
            if (DrawingRect.IsEmpty || DrawingRect.Width <= 0 || DrawingRect.Height <= 0)
            {
                // Fallback to ClientRectangle if DrawingRect not set yet
                var rect = ClientRectangle;
                rect.Inflate(-2, -2); // Small default padding
                UpdateLayout();
            }
            var rectToDraw = DrawingRect;
           // rectToDraw.Inflate(-2, -2);
            // Let the combo box painter draw everything
            _comboBoxPainter.Paint(g, this, DrawingRect);
            
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
