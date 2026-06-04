namespace TheTechIdea.Beep.Winform.Controls.Badges
{
    public static class BeepBadgeManager
    {
        private static readonly Dictionary<Control, List<IBeepBadge>> _badges = new();

        public static void RegisterBadge(Control parent, IBeepBadge badge)
        {
            if (parent is null) throw new ArgumentNullException(nameof(parent));
            if (badge is null) throw new ArgumentNullException(nameof(badge));

            if (!_badges.TryGetValue(parent, out var list))
            {
                list = new List<IBeepBadge>();
                _badges[parent] = list;
                parent.Disposed += OnParentDisposed;
                parent.Paint += OnParentPaint;
            }

            if (!list.Contains(badge))
                list.Add(badge);
        }

        public static void UnregisterBadge(IBeepBadge badge)
        {
            if (badge is null) return;
            foreach (var kv in _badges)
            {
                if (kv.Value.Remove(badge))
                {
                    if (kv.Value.Count == 0)
                    {
                        kv.Key.Disposed -= OnParentDisposed;
                        kv.Key.Paint -= OnParentPaint;
                        _badges.Remove(kv.Key);
                    }
                    return;
                }
            }
        }

        public static void BringBadgesToFront(Control parent)
        {
            if (!_badges.TryGetValue(parent, out var list)) return;
            for (int i = 0; i < list.Count; i++)
            {
                var badge = list[i];
                if (badge is Control c && c.Parent == parent && !c.IsDisposed)
                    c.BringToFront();
            }
        }

        public static IBeepBadge? GetBadge(Control target)
        {
            if (target?.Parent is null) return null;
            if (!_badges.TryGetValue(target.Parent, out var list)) return null;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Target is not null && ReferenceEquals(list[i].Target, target))
                    return list[i];
            }
            return null;
        }

        public static void ClearBadges(Control target)
        {
            if (target?.Parent is null) return;
            if (!_badges.TryGetValue(target.Parent, out var list)) return;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] is BeepFloatingBadge fb && ReferenceEquals(fb.Target, target))
                {
                    list[i].Detach();
                    list.RemoveAt(i);
                }
            }

            if (list.Count == 0)
            {
                target.Parent.Disposed -= OnParentDisposed;
                target.Parent.Paint -= OnParentPaint;
                _badges.Remove(target.Parent);
            }
        }

        private static void OnParentDisposed(object? sender, EventArgs e)
        {
            if (sender is not Control parent) return;
            if (!_badges.TryGetValue(parent, out var list)) return;

            foreach (var badge in list)
                badge.Detach();

            _badges.Remove(parent);
            parent.Disposed -= OnParentDisposed;
            parent.Paint -= OnParentPaint;
        }

        private static void OnParentPaint(object? sender, PaintEventArgs e)
        {
            if (sender is Control parent)
                BringBadgesToFront(parent);
        }
    }
}
