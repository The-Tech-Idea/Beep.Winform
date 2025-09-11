using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Converters;

namespace TheTechIdea.Beep.Winform.Controls.MDI
{
    // Designer-friendly collection with change notification
    [Editor(typeof(BeepMdiDocumentCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [Serializable]
    public class BeepMdiDocumentCollection : Collection<BeepMdiDocument>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void InsertItem(int index, BeepMdiDocument item)
        {
            base.InsertItem(index, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }
        protected override void RemoveItem(int index)
        {
            var old = this[index];
            base.RemoveItem(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, old, index));
        }
        protected override void SetItem(int index, BeepMdiDocument item)
        {
            var old = this[index];
            base.SetItem(index, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, old, index));
        }
        protected override void ClearItems()
        {
            base.ClearItems();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public void Move(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || newIndex < 0 || oldIndex >= Count || newIndex >= Count || oldIndex == newIndex) return;
            var item = this[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
        }
    }
}
