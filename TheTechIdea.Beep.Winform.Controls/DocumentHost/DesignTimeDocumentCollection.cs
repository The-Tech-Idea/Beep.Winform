using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Owner-aware collection used by <see cref="BeepDocumentHost.DesignTimeDocuments"/>.
    /// Mutations immediately refresh the design surface when the host is sited in the
    /// WinForms designer, which keeps property-grid editing and smart-tag editing aligned.
    /// </summary>
    internal sealed class DesignTimeDocumentCollection : Collection<DocumentDescriptor>
    {
        private readonly Action _notifyOwner;

        public DesignTimeDocumentCollection(BeepDocumentHost owner)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            _notifyOwner = owner.OnDesignTimeDocumentsChanged;
        }

        public DesignTimeDocumentCollection(Action notifyOwner)
        {
            _notifyOwner = notifyOwner ?? throw new ArgumentNullException(nameof(notifyOwner));
        }

        protected override void InsertItem(int index, DocumentDescriptor item)
        {
            base.InsertItem(index, item);
            Attach(item);
            NotifyOwner();
        }

        protected override void SetItem(int index, DocumentDescriptor item)
        {
            Detach(this[index]);
            base.SetItem(index, item);
            Attach(item);
            NotifyOwner();
        }

        protected override void RemoveItem(int index)
        {
            Detach(this[index]);
            base.RemoveItem(index);
            NotifyOwner();
        }

        protected override void ClearItems()
        {
            foreach (var descriptor in this)
            {
                Detach(descriptor);
            }

            base.ClearItems();
            NotifyOwner();
        }

        private void Attach(DocumentDescriptor? descriptor)
        {
            if (descriptor == null)
            {
                return;
            }

            descriptor.PropertyChanged -= OnDescriptorPropertyChanged;
            descriptor.PropertyChanged += OnDescriptorPropertyChanged;
        }

        private void Detach(DocumentDescriptor? descriptor)
        {
            if (descriptor == null)
            {
                return;
            }

            descriptor.PropertyChanged -= OnDescriptorPropertyChanged;
        }

        private void OnDescriptorPropertyChanged(object? sender, PropertyChangedEventArgs e)
            => NotifyOwner();

        private void NotifyOwner()
            => _notifyOwner();
    }
}
