using System.Collections.Generic;
using System.Diagnostics;
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
                if (!IsAreaEnabled(name)) return;
                _hits.Add(new HitArea { Name = name, Bounds = rect, Data = data });
            }
            public void RegisterHitArea(string name, Rectangle rect, object data = null)
            {
                if (rect.Width <= 0 || rect.Height <= 0) return;
                if (!IsAreaEnabled(name)) return;
                _hits.Add(new HitArea { Name = name, Bounds = rect, Data = data });
            }
            public bool HitTest(Point p, out HitArea area)
            {
                HitArea best = null;
                long bestArea = long.MaxValue;
             //  Debug.Print($"HitTest at {p}");
                foreach (var hit in _hits)
                {
                    if (!hit.Bounds.Contains(p))
                    {
                        continue;
                    }

                    long size = (long)hit.Bounds.Width * hit.Bounds.Height;
                    if (size < bestArea)
                    {
                        Debug.Print($"Hit area '{hit.Name}' with size {size} is a better match than current best area with size {bestArea}");
                        best = hit;
                        bestArea = size;
                    }
                }

                area = best;
                return area != null;
            }
            public HitArea GetHitArea(string name)
            {
                return _hits.FirstOrDefault(h => h.Name == name);
            }

            private bool IsAreaEnabled(string name)
            {
                if (string.IsNullOrWhiteSpace(name)) return false;

                string key = name.Trim();
                switch (key)
                {
                    case "close":
                    case "system:close":
                    case "region:system:close":
                        {
                            Debug.Print($"Checking if close button is enabled: {_owner.ShowCloseButton}");
                            return _owner.ShowCloseButton;
                        }
                       

                    case "maximize":
                    case "minimize":
                    case "system:maximize":
                    case "system:minimize":
                    case "region:system:maximize":
                    case "region:system:minimize":
                        return _owner.ShowMinMaxButtons;

                    case "theme":
                    case "system:theme":
                    case "region:system:theme":
                        return _owner.ShowThemeButton;

                    case "Style":
                    case "style":
                    case "system:Style":
                    case "region:system:Style":
                        return _owner.ShowStyleButton;

                    case "customAction":
                    case "custom:action":
                    case "region:custom:action":
                        return _owner.ShowCustomActionButton;

                    case "search":
                    case "system:search":
                    case "region:system:search":
                        return _owner.ShowSearchBox;

                    case "profile":
                    case "system:profile":
                    case "region:system:profile":
                        return _owner.ShowProfileButton;
                }

                return true;
            }
        }
    }
}
