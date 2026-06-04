using TheTechIdea.Beep.Winform.Controls.Badges.Builtin;

namespace TheTechIdea.Beep.Winform.Controls.Badges
{
    public static class BeepBadgeFactory
    {
        private static readonly Dictionary<string, Func<IBeepBadge>> _factories = new(StringComparer.OrdinalIgnoreCase);

        static BeepBadgeFactory()
        {
            Register<BeepCounterBadge>("Counter");
            Register<BeepDotBadge>("Dot");
            Register<BeepIconBadge>("Icon");
            Register<BeepTextBadge>("Text");
            Register<BeepValidationBadge>("Validation");
            Register<BeepNotificationBadge>("Notification");
        }

        public static void Register<TBadge>(string name) where TBadge : IBeepBadge, new()
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            _factories[name] = () => new TBadge();
        }

        public static void Register<TBadge>(string name, Func<TBadge> factory) where TBadge : IBeepBadge
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));
            _factories[name] = () => factory();
        }

        public static IBeepBadge? Create(string name)
        {
            if (_factories.TryGetValue(name, out var factory))
                return factory();
            return null;
        }

        public static T? Create<T>(string name) where T : class, IBeepBadge
        {
            return Create(name) as T;
        }

        public static IEnumerable<string> RegisteredTypes => _factories.Keys.ToArray();
    }
}
