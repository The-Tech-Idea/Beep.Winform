using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

public class FullySuppressedObservableCollection<T> : ObservableCollection<T>
{
    private bool _suppressNotification = false;

    public void BeginUpdate()
    {
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            // In design mode, do not suppress notifications
            _suppressNotification = false;
        }
        else
        {
            _suppressNotification = true;
        }
    }

    public void EndUpdate()
    {
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            // In design mode, raise individual add notifications for each item
            foreach (var item in this)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, item));
            }
        }
        else
        {
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (!_suppressNotification)
        {
            base.OnCollectionChanged(e);
        }
    }

    public void AddWithoutReset(T item)
    {
        _suppressNotification = true;
        Add(item);
        _suppressNotification = false;
    }

    public void RemoveWithoutReset(T item)
    {
        _suppressNotification = true;
        Remove(item);
        _suppressNotification = false;
    }
}
