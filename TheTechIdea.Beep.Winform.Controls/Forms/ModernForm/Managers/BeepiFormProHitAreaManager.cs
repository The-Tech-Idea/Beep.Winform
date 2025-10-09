using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
        internal sealed class BeepiFormProHitAreaManager
        {
            private readonly BeepiFormPro _owner;
            private readonly List<HitArea> _hits = new();
            public System.Collections.Generic.IReadOnlyList<HitArea> Areas => _hits;
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
    }
}
