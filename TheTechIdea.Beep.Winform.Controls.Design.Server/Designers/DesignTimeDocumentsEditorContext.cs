// DesignTimeDocumentsEditorContext.cs
// Phase 03 — split out from BeepDocumentHostDesigner.cs (workspace rule:
// one file, one class).
//
// Minimal ITypeDescriptorContext implementation required to invoke a
// CollectionEditor from the designer verb / smart-tag action *outside* of
// the Properties grid. Without it the editor would have no IComponent
// change service to round-trip changes through, so Ctrl+Z would not
// reverse the edit.
//
// Reused by BeepDocumentManagerDesigner.OpenDesignTimeDocumentsEditor as
// well, so it lives at namespace scope rather than nested inside one
// designer.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class DesignTimeDocumentsEditorContext
        : ITypeDescriptorContext, IServiceProvider
    {
        private readonly object             _instance;
        private readonly PropertyDescriptor _property;
        private readonly IServiceProvider   _services;

        public DesignTimeDocumentsEditorContext(
            object instance, PropertyDescriptor property, IServiceProvider services)
        {
            _instance = instance;
            _property = property;
            _services = services;
        }

        // ITypeDescriptorContext
        public IContainer?        Container          => (_services.GetService(typeof(IDesignerHost)) as IDesignerHost)?.Container;
        public object             Instance           => _instance;
        public PropertyDescriptor PropertyDescriptor => _property;

        public bool OnComponentChanging()
        {
            var svc = _services.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            try { svc?.OnComponentChanging(_instance, _property); return true; }
            catch { return false; }
        }

        public void OnComponentChanged()
        {
            var svc = _services.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            svc?.OnComponentChanged(_instance, _property, null, _property.GetValue(_instance));
        }

        // IServiceProvider
        public object? GetService(Type serviceType) => _services.GetService(serviceType);
    }
}
