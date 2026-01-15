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
                
            int currentIndex = items.FindIndex(i => i.ItemName == CurrentItemName);
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
                
            int currentIndex = items.FindIndex(i => i.ItemName == CurrentItemName);
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
                
            if (!UIComponents.ContainsKey(itemName))
                return false;
                
            var item = UIComponents[itemName];
            
            if (item is Control control && control.CanFocus)
            {
                // Set focus
                control.Focus();
                
                // Update current item
                CurrentItemName = itemName;
                CurrentItem = item;
                
                // Fire WHEN-ITEM-FOCUS trigger (async fire-and-forget)
                var postContext = new TriggerContext
                {
                    Block = this,
                    TriggerType = TriggerType.WhenItemFocus,
                   FieldName = itemName
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
            
            // Attach keyboard handlers to all components
            foreach (var item in UIComponents.Values)
            {
                if (item is Control control)
                {
                    control.KeyDown += Control_KeyDown;
                    control.Enter += Control_Enter;
                    control.Leave += Control_Leave;
                }
            }
        }
        
        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                if (e.Shift)
                {
                    PreviousItem();
                }
                else
                {
                    NextItem();
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
                    CurrentItemName = item.ComponentName;
                    CurrentItem = item;
                    
                    // Fire WHEN-ITEM-FOCUS trigger
                    var context = new TriggerContext
                    {
                        Block = this,
                        TriggerType = TriggerType.WhenItemFocus,
                       FieldName = item.ComponentName
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
                    // Fire WHEN-ITEM-BLUR trigger
                    var context = new TriggerContext
                    {
                        Block = this,
                        TriggerType = TriggerType.WhenItemBlur,
                       FieldName = item.ComponentName
                    };
                    
                    _ = ExecuteTriggers(TriggerType.WhenItemBlur, context);
                }
            }
        }
        
        #endregion
    }
}

