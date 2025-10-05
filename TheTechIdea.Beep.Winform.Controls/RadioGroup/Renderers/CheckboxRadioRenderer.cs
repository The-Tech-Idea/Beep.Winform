using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Checkbox-style renderer. Delegates drawing to FlatRadioRenderer but exposes proper style name and behavior.
    /// </summary>
    public class CheckboxRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private readonly FlatRadioRenderer _inner = new FlatRadioRenderer();

        public string StyleName => "Checkbox";
        public string DisplayName => "Checkbox ProgressBarStyle";
        public bool SupportsMultipleSelection => true;

        public Size MaxImageSize
        {
            get => (_inner as IImageAwareRenderer)?.MaxImageSize ?? new Size(24, 24);
            set
            {
                if (_inner is IImageAwareRenderer aware)
                    aware.MaxImageSize = value;
            }
        }

        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _inner.Initialize(owner, theme);
        }

        public void UpdateTheme(IBeepTheme theme)
        {
            _inner.UpdateTheme(theme);
        }

        public void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            // Force multiple-selection visual path in inner renderer by relying on owner property
            _inner.RenderItem(graphics, item, rectangle, state);
        }

        public Size MeasureItem(SimpleItem item, Graphics graphics) => _inner.MeasureItem(item, graphics);

        public Rectangle GetContentArea(Rectangle itemRectangle) => _inner.GetContentArea(itemRectangle);

        public Rectangle GetSelectorArea(Rectangle itemRectangle) => _inner.GetSelectorArea(itemRectangle);

        public void RenderGroupDecorations(Graphics graphics, Rectangle groupRectangle, List<SimpleItem> items, List<Rectangle> itemRectangles, List<RadioItemState> states)
            => _inner.RenderGroupDecorations(graphics, groupRectangle, items, itemRectangles, states);
    }
}
