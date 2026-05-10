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

            internal static string NormalizeName(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    return string.Empty;

                return name.Trim().ToLowerInvariant() switch
                {
                    "close" or "system:close" or "region:system:close" => FormHitAreaNames.Close,
                    "maximize" or "system:maximize" or "region:system:maximize" => FormHitAreaNames.Maximize,
                    "minimize" or "system:minimize" or "region:system:minimize" => FormHitAreaNames.Minimize,
                    "theme" or "system:theme" or "region:system:theme" => FormHitAreaNames.Theme,
                    "style" or "system:style" or "region:system:style" => FormHitAreaNames.Style,
                    "customaction" or "custom:action" or "region:custom:action" => FormHitAreaNames.CustomAction,
                    "search" or "system:search" or "region:system:search" => FormHitAreaNames.Search,
                    "profile" or "system:profile" or "region:system:profile" => FormHitAreaNames.Profile,
                    "title" => FormHitAreaNames.Title,
                    "caption" => FormHitAreaNames.Caption,
                    "icon" => FormHitAreaNames.Icon,
                    _ => name.Trim()
                };
            }

            public void Register(string name, Rectangle rect, object data = null)
            {
                name = NormalizeName(name);
                if (rect.Width <= 0 || rect.Height <= 0) return;
                if (!IsAreaEnabled(name)) return;
                _hits.Add(new HitArea { Name = name, Bounds = rect, Data = data });
            }
            public void RegisterHitArea(string name, Rectangle rect, object data = null)
            {
                name = NormalizeName(name);
                if (rect.Width <= 0 || rect.Height <= 0) return;
                if (!IsAreaEnabled(name)) return;
                _hits.Add(new HitArea { Name = name, Bounds = rect, Data = data });
            }
            public bool HitTest(Point p, out HitArea area)
            {
                HitArea best = null;
                long bestArea = long.MaxValue;
             //  //Debug.Print($"HitTest at {p}");
                foreach (var hit in _hits)
                {
                    if (!hit.Bounds.Contains(p))
                    {
                        continue;
                    }

                    long size = (long)hit.Bounds.Width * hit.Bounds.Height;
                    if (size < bestArea)
                    {
                        //Debug.Print($"Hit area '{hit.Name}' with size {size} is a better match than current best area with size {bestArea}");
                        best = hit;
                        bestArea = size;
                    }
                }

                area = best;
                return area != null;
            }
            public HitArea GetHitArea(string name)
            {
                var normalized = NormalizeName(name);
                return _hits.FirstOrDefault(h => h.Name == normalized);
            }

            private bool IsAreaEnabled(string name)
            {
                string key = NormalizeName(name);
                if (string.IsNullOrWhiteSpace(key)) return false;

                switch (key)
                {
                    case FormHitAreaNames.Close:
                        {
                            //Debug.Print($"Checking if close button is enabled: {_owner.ShowCloseButton}");
                            return _owner.ShowCloseButton;
                        }
                       

                    case FormHitAreaNames.Maximize:
                    case FormHitAreaNames.Minimize:
                        return _owner.ShowMinMaxButtons;

                    case FormHitAreaNames.Theme:
                        return _owner.ShowThemeButton;

                    case FormHitAreaNames.Style:
                        return _owner.ShowStyleButton;

                    case FormHitAreaNames.CustomAction:
                        return _owner.ShowCustomActionButton;

                    case FormHitAreaNames.Search:
                        return _owner.ShowSearchBox;

                    case FormHitAreaNames.Profile:
                        return _owner.ShowProfileButton;
                }

                return true;
            }
        }
    }
}
