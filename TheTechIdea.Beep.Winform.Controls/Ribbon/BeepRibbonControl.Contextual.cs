using TheTechIdea.Beep.Winform.Controls.Accessibility;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        public int AddContextualGroup(string name, Color color)
        {
            var grp = new ContextualGroup { Name = name, Color = color, Visible = false };
            _contextGroups.Add(grp);
            return _contextGroups.Count - 1;
        }

        public RibbonTab AddContextualTab(int groupId, string title)
        {
            if (groupId < 0 || groupId >= _contextGroups.Count) throw new ArgumentOutOfRangeException(nameof(groupId));
            var grp = _contextGroups[groupId];
            var tab = _tabStrip.AddTab(title);
            tab.ContentPanel = new Panel { BackColor = _theme.TabActiveBack, Dock = DockStyle.Fill, Visible = false };
            _ribbonContentHost.Controls.Add(tab.ContentPanel);
            grp.Pages.Add(tab);
            _pageToGroup[tab] = grp;
            if (grp.Visible) tab.ContentPanel.Visible = true;
            _contextHeader.Invalidate();
            return tab;
        }

        public void SetContextualGroupVisible(int groupId, bool visible)
        {
            if (groupId < 0 || groupId >= _contextGroups.Count) return;
            var grp = _contextGroups[groupId];
            if (grp.Visible == visible) return;
            grp.Visible = visible;
            foreach (var tab in grp.Pages)
                if (tab.ContentPanel != null) tab.ContentPanel.Visible = visible;
            _contextHeader.Invalidate();
        }

        public void RemoveContextualGroup(int groupId)
        {
            if (groupId < 0 || groupId >= _contextGroups.Count) return;
            var grp = _contextGroups[groupId];
            foreach (var tab in grp.Pages)
            {
                _pageToGroup.Remove(tab);
                if (tab.ContentPanel != null) _ribbonContentHost.Controls.Remove(tab.ContentPanel);
                _tabStrip.RemoveTab(tab);
            }
            _contextGroups.RemoveAt(groupId);
            _contextHeader.Invalidate();
        }
    }
}
