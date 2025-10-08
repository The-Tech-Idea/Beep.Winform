using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
        internal sealed class BeepiFormProLayoutManager
        {
            private readonly BeepiFormPro _owner;
            public Rectangle CaptionRect { get; private set; }
            public Rectangle ContentRect { get; private set; }
            public Rectangle BottomRect { get; private set; }
            public Rectangle LeftRect { get; private set; }
            public Rectangle RightRect { get; private set; }

            // System button rects
            public Rectangle IconRect { get; private set; }
            public Rectangle TitleRect { get; private set; }
            public Rectangle CustomActionButtonRect { get; private set; }
            public Rectangle MinimizeButtonRect { get; private set; }
            public Rectangle MaximizeButtonRect { get; private set; }
            public Rectangle CloseButtonRect { get; private set; }

            public BeepiFormProLayoutManager(BeepiFormPro owner) { _owner = owner; }

            public void Calculate()
            {
                var r = _owner.ClientRectangle;
                // DPI-aware caption height: base 32px, scaled by font + DPI
                int captionH = Math.Max(_owner.ScaleDpi(32), (int)(_owner.Font.Height * 2.5f));
                int bottomH = 0; // start at 0; regions can use Bottom dock
                CaptionRect = new Rectangle(r.Left, r.Top, r.Width, captionH);
                BottomRect = new Rectangle(r.Left, r.Bottom - bottomH, r.Width, bottomH);
                LeftRect = new Rectangle(r.Left, CaptionRect.Bottom, 0, r.Height - captionH - bottomH);
                RightRect = new Rectangle(r.Right, CaptionRect.Bottom, 0, r.Height - captionH - bottomH);
                ContentRect = Rectangle.FromLTRB(r.Left, CaptionRect.Bottom, r.Right, r.Bottom - bottomH);

                // System buttons (right-aligned, DPI-scaled 32px each)
                int buttonWidth = _owner.ScaleDpi(32);
                int buttonY = 0;
                int buttonHeight = captionH;

                CloseButtonRect = new Rectangle(r.Width - buttonWidth, buttonY, buttonWidth, buttonHeight);
                MaximizeButtonRect = new Rectangle(CloseButtonRect.Left - buttonWidth, buttonY, buttonWidth, buttonHeight);
                MinimizeButtonRect = new Rectangle(MaximizeButtonRect.Left - buttonWidth, buttonY, buttonWidth, buttonHeight);

                // Custom action button (left of minimize, same size)
                CustomActionButtonRect = new Rectangle(MinimizeButtonRect.Left - buttonWidth - _owner.ScaleDpi(4), buttonY, buttonWidth, buttonHeight);

                // Icon (left side, square based on caption height, DPI-scaled)
                int iconSize = Math.Min(captionH - _owner.ScaleDpi(8), _owner.ScaleDpi(24));
                int iconY = (captionH - iconSize) / 2;
                IconRect = new Rectangle(_owner.ScaleDpi(8), iconY, iconSize, iconSize);

                // Title (between icon and custom action button)
                int titleX = IconRect.Right + _owner.ScaleDpi(8);
                int titleWidth = CustomActionButtonRect.Left - titleX - _owner.ScaleDpi(8);
                TitleRect = new Rectangle(titleX, 0, titleWidth, captionH);
            }
        }

        internal sealed class BeepiFormProHitAreaManager
        {
            private readonly BeepiFormPro _owner;
            private readonly List<HitArea> _hits = new();
            public IReadOnlyList<HitArea> Areas => _hits;
            public BeepiFormProHitAreaManager(BeepiFormPro owner) { _owner = owner; }
            public void Clear() => _hits.Clear();
            public void Register(string name, Rectangle rect, object data = null)
            {
                if (rect.Width <= 0 || rect.Height <= 0) return;
                _hits.Add(new HitArea { Name = name, Bounds = rect, Data = data });
            }
            public bool HitTest(Point p, out HitArea area)
            {
                area = _hits.FirstOrDefault(h => h.Bounds.Contains(p));
                return area != null;
            }
        }

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
                if (!Equals(area, _hover))
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
                if (_pressed != null && released != null && ReferenceEquals(_pressed, released))
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

        protected virtual void OnRegionClicked(HitArea area)
        {
            // Extension point: override to route actions
        }
    }
}