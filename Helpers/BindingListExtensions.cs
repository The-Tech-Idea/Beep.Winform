using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class BindingListExtensions
    {
        public static void AddRange(this BindingList<SimpleItem> bindingList, IEnumerable<SimpleItem> items)
        {
            if (bindingList == null)
                throw new ArgumentNullException(nameof(bindingList));
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (!bindingList.AllowNew && bindingList.AllowEdit)
                throw new InvalidOperationException("BindingList does not allow adding new items.");

            bool oldRaiseListChangedEvents = bindingList.RaiseListChangedEvents;
            bindingList.RaiseListChangedEvents = false; // Suspend notifications

            try
            {
                foreach (var item in items)
                {
                    if (item != null)
                    {
                        bindingList.Add(item);
                    }
                }
            }
            finally
            {
                bindingList.RaiseListChangedEvents = oldRaiseListChangedEvents; // Restore notifications
                if (bindingList.RaiseListChangedEvents)
                {
                    bindingList.ResetBindings(); // Notify listeners of the change
                }
            }
        }
    }
}
