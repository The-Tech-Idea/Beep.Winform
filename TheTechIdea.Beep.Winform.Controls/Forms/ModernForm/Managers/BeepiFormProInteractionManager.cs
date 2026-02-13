using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

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
                    var oldHover = _hover;
                    _hover = area;

                    // Invalidate only the changed regions
                    if (oldHover != null)
                    {
                        _owner.Invalidate(oldHover.Bounds);
                    }
                    if (_hover != null)
                    {
                        _owner.Invalidate(_hover.Bounds);
                    }

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
                Debug.Print($"MouseDown at {p}, hit area: {_pressed?.Name ?? "none"}");
                // Invalidate only the pressed region
                if (_pressed != null)
                {
                    _owner.Invalidate(_pressed.Bounds);
                }
            }
            public void OnMouseUp(Point p)
            {
                _hits.HitTest(p, out var released);
                // Compare by name to be resilient to hit-list re-registration between down/up
                if (_pressed != null && released != null && string.Equals(_pressed.Name, released.Name))
                {
                    // Raise event hook (future extension) or command
                    _owner.OnRegionClicked(released);
                }

                // Invalidate only the released region
                if (released != null)
                {
                    _owner.Invalidate(released.Bounds);
                }

                _pressed = null;
            }
            public bool IsHovered(HitArea a) => a != null && _hover != null && string.Equals(_hover.Name, a.Name);
            public bool IsPressed(HitArea a) => a != null && _pressed != null && string.Equals(_pressed.Name, a.Name);

            internal void OnMouseHover(Point pos)
            {
                // Treat hover as move to update hover state consistently
                OnMouseMove(pos);
            }
        }
    }
}
