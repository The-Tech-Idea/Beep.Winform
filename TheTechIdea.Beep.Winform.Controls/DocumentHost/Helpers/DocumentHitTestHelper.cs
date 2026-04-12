using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Helpers
{
    public class HitTestResult
    {
        public string AreaName { get; set; }
        public int TabIndex { get; set; } = -1;
        public bool IsCloseButton { get; set; }
        public bool IsAddButton { get; set; }
        public bool IsScrollLeft { get; set; }
        public bool IsScrollRight { get; set; }
        public bool IsOverflowButton { get; set; }
        public bool IsSplitter { get; set; }
        public bool IsGroupHeader { get; set; }
        public bool Hit => AreaName != null;
    }

    public class DocumentHitTestHelper
    {
        private readonly Dictionary<string, Rectangle> _hitAreas = new Dictionary<string, Rectangle>();
        private readonly Dictionary<string, object> _hitAreaData = new Dictionary<string, object>();

        public void RegisterHitArea(string name, Rectangle rect, object data = null)
        {
            _hitAreas[name] = rect;
            if (data != null) _hitAreaData[name] = data;
        }

        public void UpdateHitArea(string name, Rectangle rect)
        {
            if (_hitAreas.ContainsKey(name))
                _hitAreas[name] = rect;
        }

        public Rectangle? GetHitArea(string name)
        {
            return _hitAreas.TryGetValue(name, out var rect) ? rect : (Rectangle?)null;
        }

        public object GetHitAreaData(string name)
        {
            return _hitAreaData.TryGetValue(name, out var data) ? data : null;
        }

        public HitTestResult HitTest(Point pt)
        {
            var result = new HitTestResult();

            foreach (var kvp in _hitAreas)
            {
                if (kvp.Value.Contains(pt))
                {
                    result.AreaName = kvp.Key;

                    if (kvp.Key.StartsWith("close_") && int.TryParse(kvp.Key.Substring(6), out int closeIdx))
                    {
                        result.IsCloseButton = true;
                        result.TabIndex = closeIdx;
                    }
                    else if (kvp.Key.StartsWith("tab_") && int.TryParse(kvp.Key.Substring(4), out int tabIdx))
                    {
                        result.TabIndex = tabIdx;
                    }
                    else if (kvp.Key == "addButton")
                    {
                        result.IsAddButton = true;
                    }
                    else if (kvp.Key == "scrollLeft")
                    {
                        result.IsScrollLeft = true;
                    }
                    else if (kvp.Key == "scrollRight")
                    {
                        result.IsScrollRight = true;
                    }
                    else if (kvp.Key == "overflowButton")
                    {
                        result.IsOverflowButton = true;
                    }
                    else if (kvp.Key.StartsWith("splitter_"))
                    {
                        result.IsSplitter = true;
                    }
                    else if (kvp.Key.StartsWith("groupHeader_"))
                    {
                        result.IsGroupHeader = true;
                    }

                    return result;
                }
            }

            return result;
        }

        public void ClearHitAreas()
        {
            _hitAreas.Clear();
            _hitAreaData.Clear();
        }
    }
}
