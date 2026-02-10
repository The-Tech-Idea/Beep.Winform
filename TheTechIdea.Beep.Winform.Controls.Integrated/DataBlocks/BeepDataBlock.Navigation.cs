using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial class - Navigation & Focus Management
    /// Provides Oracle Forms-compatible navigation system
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Navigation State
        
        private string _currentItemName;
        private IBeepUIComponent _currentItem;
        private bool _defaultKeyTriggersRegistered;
        
        /// <summary>
        /// Current item name (Oracle Forms: :SYSTEM.CURSOR_ITEM)
        /// </summary>
        public string CurrentItemName
        {
            get => _currentItemName;
            private set
            {
                if (_currentItemName != value)
                {
                    _currentItemName = value;
                    SYSTEM.TRIGGER_ITEM = value;
                    SYSTEM.TRIGGER_FIELD = value;
                }
            }
        }
        
        /// <summary>
        /// Current item component
        /// </summary>
        public IBeepUIComponent CurrentItem
        {
            get => _currentItem;
            private set => _currentItem = value;
        }
        
        #endregion
        
        #region Item Navigation
        
        /// <summary>
        /// Navigate to next item
        /// Oracle Forms equivalent: NEXT_ITEM built-in
        /// </summary>
        public bool NextItem()
        {
            var items = GetNavigableItems();
            
            if (items.Count == 0)
                return false;
                
            int currentIndex = items.FindIndex(i =>
                string.Equals(i.ItemName, CurrentItemName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(i.BoundProperty, CurrentItemName, StringComparison.OrdinalIgnoreCase));
            int nextIndex = (currentIndex + 1) % items.Count;
            
            return GoToItem(items[nextIndex].ItemName);
        }
        
        /// <summary>
        /// Navigate to previous item
        /// Oracle Forms equivalent: PREVIOUS_ITEM built-in
        /// </summary>
        public bool PreviousItem()
        {
            var items = GetNavigableItems();
            
            if (items.Count == 0)
                return false;
                
            int currentIndex = items.FindIndex(i =>
                string.Equals(i.ItemName, CurrentItemName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(i.BoundProperty, CurrentItemName, StringComparison.OrdinalIgnoreCase));
            int prevIndex = currentIndex <= 0 ? items.Count - 1 : currentIndex - 1;
            
            return GoToItem(items[prevIndex].ItemName);
        }
        
        /// <summary>
        /// Navigate to first item
        /// Oracle Forms equivalent: FIRST_ITEM built-in
        /// </summary>
        public bool FirstItem()
        {
            var items = GetNavigableItems();
            
            if (items.Count == 0)
                return false;
                
            return GoToItem(items[0].ItemName);
        }
        
        /// <summary>
        /// Navigate to last item
        /// Oracle Forms equivalent: LAST_ITEM built-in
        /// </summary>
        public bool LastItem()
        {
            var items = GetNavigableItems();
            
            if (items.Count == 0)
                return false;
                
            return GoToItem(items[items.Count - 1].ItemName);
        }
        
        /// <summary>
        /// Navigate to specific item
        /// Oracle Forms equivalent: GO_ITEM built-in
        /// </summary>
        public bool GoToItem(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return false;

            if (!TryResolveItem(itemName, out var blockItem, out var resolvedItemName))
                return false;

            var item = blockItem.Component;
            if (item == null && !TryGetComponentByIdentifier(resolvedItemName, out item))
                return false;
             
            if (item is Control control && control.CanFocus)
            {
                // Set focus
                control.Focus();
                 
                // Update current item
                CurrentItemName = resolvedItemName;
                CurrentItem = item;
                blockItem.Component = item;
                blockItem.BoundProperty = item.BoundProperty;

                var triggerField = blockItem.BoundProperty ?? item.BoundProperty ?? resolvedItemName;
                if (SYSTEM != null)
                {
                    SYSTEM.TRIGGER_ITEM = item.ComponentName ?? resolvedItemName;
                    SYSTEM.TRIGGER_FIELD = triggerField;
                }
                 
                // Fire WHEN-ITEM-FOCUS trigger (async fire-and-forget)
                var postContext = new TriggerContext
                {
                    Block = this,
                    TriggerType = TriggerType.WhenItemFocus,
                    FieldName = triggerField
                };
                 
                _ = ExecuteTriggers(TriggerType.WhenItemFocus, postContext);
                
                return true;
            }
            
            return false;
        }
        
        #endregion
        
        #region Helper Methods
        
        private List<BeepDataBlockItem> GetNavigableItems()
        {
            return _items.Values
                .Where(i => i.Enabled && i.Visible)
                .OrderBy(i => i.TabIndex)
                .ToList();
        }
        
        #endregion
        
        #region Keyboard Navigation
        
        /// <summary>
        /// Setup keyboard navigation for the block
        /// </summary>
        public void SetupKeyboardNavigation()
        {
            if (!_defaultKeyTriggersRegistered)
            {
                // Register KEY-NEXT-ITEM trigger (Tab/Enter)
                RegisterTrigger(TriggerType.KeyNextItem, async context =>
                {
                    NextItem();
                    await Task.CompletedTask;
                    return true;
                });

                // Register KEY-PREV-ITEM trigger (Shift+Tab)
                RegisterTrigger(TriggerType.KeyPrevItem, async context =>
                {
                    PreviousItem();
                    await Task.CompletedTask;
                    return true;
                });

                _defaultKeyTriggersRegistered = true;
            }
            
            // Attach keyboard handlers to all components
            foreach (var item in UIComponents.Values)
            {
                if (item is Control control)
                {
                    control.KeyDown -= Control_KeyDown;
                    control.Enter -= Control_Enter;
                    control.Leave -= Control_Leave;
                    control.KeyDown += Control_KeyDown;
                    control.Enter += Control_Enter;
                    control.Leave += Control_Leave;
                }
            }
        }

        private void TearDownKeyboardNavigation()
        {
            foreach (var item in UIComponents.Values)
            {
                if (item is Control control)
                {
                    control.KeyDown -= Control_KeyDown;
                    control.Enter -= Control_Enter;
                    control.Leave -= Control_Leave;
                }
            }
        }
        
        private async void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                var component = sender as IBeepUIComponent;
                component ??= UIComponents.Values.FirstOrDefault(i => ReferenceEquals(i, sender));

                if (e.Shift)
                {
                    if (component != null)
                    {
                        await FireKeyPrevItem(component);
                    }
                    else
                    {
                        PreviousItem();
                    }
                }
                else
                {
                    if (component != null)
                    {
                        await FireKeyNextItem(component);
                    }
                    else
                    {
                        NextItem();
                    }
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
        
        private void Control_Enter(object sender, EventArgs e)
        {
            if (sender is Control control)
            {
                // Find item name from control
                var item = UIComponents.Values.FirstOrDefault(i => i == sender);
                if (item != null)
                {
                    var lookupKey = item.BoundProperty ?? item.ComponentName ?? item.GuidID;
                    if (!TryResolveItem(lookupKey, out var blockItem, out var resolvedItemName))
                    {
                        RegisterItem(lookupKey, item);
                        blockItem = GetItem(lookupKey);
                        resolvedItemName = lookupKey;
                    }

                    CurrentItemName = resolvedItemName;
                    CurrentItem = item;

                    var triggerField = blockItem?.BoundProperty ?? item.BoundProperty ?? resolvedItemName;
                    if (SYSTEM != null)
                    {
                        SYSTEM.TRIGGER_ITEM = item.ComponentName ?? resolvedItemName;
                        SYSTEM.TRIGGER_FIELD = triggerField;
                    }
                     
                    // Fire WHEN-ITEM-FOCUS trigger
                    var context = new TriggerContext
                    {
                        Block = this,
                        TriggerType = TriggerType.WhenItemFocus,
                        FieldName = triggerField
                    };
                     
                    _ = ExecuteTriggers(TriggerType.WhenItemFocus, context);
                }
            }
        }
        
        private void Control_Leave(object sender, EventArgs e)
        {
            if (sender is Control control)
            {
                // Find item name from control
                var item = UIComponents.Values.FirstOrDefault(i => i == sender);
                if (item != null)
                {
                    var lookupKey = item.BoundProperty ?? item.ComponentName ?? item.GuidID;
                    TryResolveItem(lookupKey, out var blockItem, out var resolvedItemName);

                    // Fire WHEN-ITEM-BLUR trigger
                    var context = new TriggerContext
                    {
                        Block = this,
                        TriggerType = TriggerType.WhenItemBlur,
                        FieldName = blockItem?.BoundProperty ?? item.BoundProperty ?? resolvedItemName ?? lookupKey
                    };
                     
                    _ = ExecuteTriggers(TriggerType.WhenItemBlur, context);
                }
            }
        }
        
        #endregion
    }
}

