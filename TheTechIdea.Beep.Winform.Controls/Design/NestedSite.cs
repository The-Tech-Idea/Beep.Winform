using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Design
{
    public class NestedSite : ISite
    {
        private readonly ISite _parentSite;
        private readonly IComponent _component;

        public NestedSite(ISite parentSite, IComponent component)
        {
            _parentSite = parentSite ?? throw new ArgumentNullException(nameof(parentSite));
            _component = component ?? throw new ArgumentNullException(nameof(component));
        }

        public IComponent Component => _component;

        public IContainer Container => _parentSite.Container;

        public bool DesignMode => _parentSite.DesignMode;

        public string Name
        {
            get => _component.Site?.Name ?? _parentSite.Name;
            set
            {
                if (_component.Site != null)
                {
                    _component.Site.Name = value;
                }
            }
        }

        // Implementation of GetService
        public object? GetService(Type serviceType)
        {
            return _parentSite.GetService(serviceType);
        }
    }
}
