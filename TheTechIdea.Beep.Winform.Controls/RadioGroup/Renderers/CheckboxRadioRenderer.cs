using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Checkbox-style renderer. Delegates to <see cref="FlatRadioRenderer"/> for visuals
    /// but exposes its own style identity. Forces multi-selection visual semantics.
    /// </summary>
    public sealed class CheckboxRadioRenderer : BaseRadioRenderer
    {
        private readonly FlatRadioRenderer _inner = new FlatRadioRenderer();

        public override string StyleName => "Checkbox";
        public override string DisplayName => "Checkbox";
        public override bool SupportsMultipleSelection => true;

        public override bool AllowMultipleSelection
        {
            get => _inner.AllowMultipleSelection;
            set => _inner.AllowMultipleSelection = value;
        }

        public override BeepControlStyle ControlStyle
        {
            get => _inner.ControlStyle;
            set => _inner.ControlStyle = value;
        }

        public override bool UseThemeColors
        {
            get => _inner.UseThemeColors;
            set => _inner.UseThemeColors = value;
        }

        public override Size MaxImageSize
        {
            get => _inner.MaxImageSize;
            set => _inner.MaxImageSize = value;
        }

        public override void Initialize(BaseControl owner, IBeepTheme theme) => _inner.Initialize(owner, theme);
        public override void UpdateTheme(IBeepTheme theme) => _inner.UpdateTheme(theme);

        public override void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
            => _inner.RenderItem(graphics, item, rectangle, state);

        public override Size MeasureItem(SimpleItem item, Graphics graphics) => _inner.MeasureItem(item, graphics);

        public override Rectangle GetContentArea(Rectangle itemRectangle) => _inner.GetContentArea(itemRectangle);

        public override Rectangle GetSelectorArea(Rectangle itemRectangle) => _inner.GetSelectorArea(itemRectangle);
    }
}
