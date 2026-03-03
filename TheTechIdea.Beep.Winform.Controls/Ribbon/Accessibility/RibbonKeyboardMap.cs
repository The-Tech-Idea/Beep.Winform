namespace TheTechIdea.Beep.Winform.Controls.Accessibility
{
    public sealed class RibbonKeyboardMap
    {
        private readonly Dictionary<Keys, Action> _map = [];

        public void Register(Keys key, Action action)
        {
            if (action == null)
            {
                return;
            }

            _map[key] = action;
        }

        public void Unregister(Keys key)
        {
            _map.Remove(key);
        }

        public bool TryInvoke(Keys key)
        {
            if (_map.TryGetValue(key, out var action))
            {
                action();
                return true;
            }

            return false;
        }

        public void Clear()
        {
            _map.Clear();
        }
    }
}
