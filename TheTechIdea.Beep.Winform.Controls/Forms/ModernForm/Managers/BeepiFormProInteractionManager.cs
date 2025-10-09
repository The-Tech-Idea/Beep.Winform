using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
        internal sealed class BeepiFormProInteractionManager
        {
            private readonly BeepiFormPro _owner;
            private readonly BeepiFormProHitAreaManager _hits;
            private HitArea _hover;
            private HitArea _pressed;
            public BeepiFormProInteractionManager(BeepiFormPro owner, BeepiFormProHitAreaManager hits)
            {
                _owner = owner; _hits = hits;
            }
            public void OnMouseMove(Point p)
            {
                _hits.HitTest(p, out var area);
                if (!System.Object.Equals(area, _hover))
                {
                    _hover = area;
                    _owner.Invalidate();

                    // Raise hover event
                    if (_hover != null)
                    {
                        var regionData = _hover.Data as FormRegion;
                        _owner.RegionHover?.Invoke(_owner, new RegionEventArgs(_hover.Name, regionData, _hover.Bounds));
                    }
                }
            }
            public void OnMouseDown(Point p)
            {
                _hits.HitTest(p, out _pressed);
                _owner.Invalidate();
            }
            public void OnMouseUp(Point p)
            {
                _hits.HitTest(p, out var released);
                if (_pressed != null && released != null && System.Object.ReferenceEquals(_pressed, released))
                {
                    // Raise event hook (future extension) or command
                    _owner.OnRegionClicked(released);
                }
                _pressed = null;
                _owner.Invalidate();
            }
            public bool IsHovered(HitArea a) => _hover == a;
            public bool IsPressed(HitArea a) => _pressed == a;
        }
    }
}
