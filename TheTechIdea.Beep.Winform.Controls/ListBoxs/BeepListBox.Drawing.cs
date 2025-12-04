using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Drawing logic for BeepListBox
    /// </summary>
    public partial class BeepListBox
    {
        #region Drawing Override
        
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
            
            // Ensure painter exists for current type
            if (_listBoxPainter == null)
            {
                _listBoxPainter = CreatePainter(_listBoxType);
                _listBoxPainter.Initialize(this, _currentTheme);
                _listBoxPainter.Style = ControlStyle;
            }
            if (_listBoxPainter.Style != ControlStyle)
            {
                _listBoxPainter.Style = ControlStyle;
            }
            
            // Update layout if needed
            if (_needsLayoutUpdate)
            {
                UpdateLayout();
                _layoutHelper.CalculateLayout();
                _hitHelper.RegisterHitAreas();
                _needsLayoutUpdate = false;
            }
            
            // Let the list box painter draw everything
            _listBoxPainter.Paint(g, this, bounds);
        }
        
        #endregion
        
        #region Painter Factory
        
        /// <summary>
        /// Creates the appropriate painter for the specified ListBoxType
        /// </summary>
        private IListBoxPainter CreatePainter(ListBoxType type)
        {
            IListBoxPainter painter = type switch
            {
                ListBoxType.Standard => new StandardListBoxPainter(),
                ListBoxType.Minimal => new MinimalListBoxPainter(),
                ListBoxType.Outlined => new OutlinedListBoxPainter(),
                ListBoxType.Rounded => new RoundedListBoxPainter(),
                ListBoxType.MaterialOutlined => new MaterialOutlinedListBoxPainter(),
                ListBoxType.Filled => new FilledListBoxPainter(),
                ListBoxType.Borderless => new BorderlessListBoxPainter(),
                ListBoxType.CategoryChips => new CategoryChipsPainter(),
                ListBoxType.SearchableList => new SearchableListPainter(),
                ListBoxType.WithIcons => new WithIconsListBoxPainter(),
                ListBoxType.CheckboxList => new CheckboxListPainter(),
                ListBoxType.SimpleList => new SimpleListPainter(),
                ListBoxType.LanguageSelector => new LanguageSelectorPainter(),
                ListBoxType.CardList => new CardListPainter(),
                ListBoxType.Compact => new CompactListPainter(),
                ListBoxType.Grouped => new GroupedListPainter(),
                ListBoxType.TeamMembers => new TeamMembersPainter(),
                ListBoxType.FilledStyle => new FilledStylePainter(),
                ListBoxType.FilterStatus => new FilterStatusPainter(),
                ListBoxType.OutlinedCheckboxes => new OutlinedCheckboxesPainter(),
                ListBoxType.RaisedCheckboxes => new RaisedCheckboxesPainter(),
                ListBoxType.MultiSelectionTeal => new MultiSelectionTealPainter(),
                ListBoxType.ColoredSelection => new ColoredSelectionPainter(),
                ListBoxType.RadioSelection => new RadioSelectionPainter(),
                ListBoxType.ErrorStates => new ErrorStatesPainter(),
                ListBoxType.Custom => new CustomListPainter(),
                // New modern styles
                ListBoxType.Glassmorphism => new GlassmorphismListBoxPainter(),
                ListBoxType.Neumorphic => new NeumorphicListBoxPainter(),
                ListBoxType.GradientCard => new GradientCardListBoxPainter(),
                ListBoxType.ChipStyle => new ChipStyleListBoxPainter(),
                ListBoxType.AvatarList => new AvatarListBoxPainter(),
                ListBoxType.Timeline => new TimelineListBoxPainter(),
                _ => new StandardListBoxPainter()
            };
            ControlStyle = BeepStyling.GetControlStyle(BeepThemesManager.CurrentStyle);
            painter.Style = ControlStyle;

            // If custom painter and we have a custom renderer, set it
            if (painter is CustomListPainter customPainter && _customItemRenderer != null)
            {
                customPainter.CustomItemRenderer = _customItemRenderer;
            }
            
            return painter;
        }
        
        #endregion
        
        #region Hit Area Registration
        
        /// <summary>
        /// Registers interactive areas for hit testing
        /// </summary>
        // Hit areas are managed by BeepListBoxHitTestHelper via BaseControl._hitTest
        
        #endregion
    }
}
