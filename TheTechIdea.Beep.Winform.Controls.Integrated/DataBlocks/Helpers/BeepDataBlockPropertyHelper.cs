using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Helpers
{
    /// <summary>
    /// Helper class for BeepDataBlock item and block properties
    /// Provides utility methods for property management
    /// </summary>
    public static class BeepDataBlockPropertyHelper
    {
        #region Quick Property Sets
        
        /// <summary>
        /// Make an item required
        /// </summary>
        public static void MakeRequired(BeepDataBlock block, string itemName)
        {
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.Required), true);
        }
        
        /// <summary>
        /// Make an item optional
        /// </summary>
        public static void MakeOptional(BeepDataBlock block, string itemName)
        {
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.Required), false);
        }
        
        /// <summary>
        /// Disable an item
        /// </summary>
        public static void DisableItem(BeepDataBlock block, string itemName)
        {
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.Enabled), false);
        }
        
        /// <summary>
        /// Enable an item
        /// </summary>
        public static void EnableItem(BeepDataBlock block, string itemName)
        {
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.Enabled), true);
        }
        
        /// <summary>
        /// Hide an item
        /// </summary>
        public static void HideItem(BeepDataBlock block, string itemName)
        {
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.Visible), false);
        }
        
        /// <summary>
        /// Show an item
        /// </summary>
        public static void ShowItem(BeepDataBlock block, string itemName)
        {
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.Visible), true);
        }
        
        /// <summary>
        /// Set default value for an item
        /// </summary>
        public static void SetDefaultValue(BeepDataBlock block, string itemName, object defaultValue)
        {
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.DefaultValue), defaultValue);
        }
        
        /// <summary>
        /// Set hint text for an item
        /// </summary>
        public static void SetHintText(BeepDataBlock block, string itemName, string hintText)
        {
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.HintText), hintText);
        }
        
        #endregion
        
        #region Batch Property Operations
        
        /// <summary>
        /// Make multiple items required
        /// </summary>
        public static void MakeRequiredBatch(BeepDataBlock block, params string[] itemNames)
        {
            foreach (var itemName in itemNames)
            {
                MakeRequired(block, itemName);
            }
        }
        
        /// <summary>
        /// Disable multiple items
        /// </summary>
        public static void DisableItemsBatch(BeepDataBlock block, params string[] itemNames)
        {
            foreach (var itemName in itemNames)
            {
                DisableItem(block, itemName);
            }
        }
        
        /// <summary>
        /// Hide multiple items
        /// </summary>
        public static void HideItemsBatch(BeepDataBlock block, params string[] itemNames)
        {
            foreach (var itemName in itemNames)
            {
                HideItem(block, itemName);
            }
        }
        
        /// <summary>
        /// Set query-only items (disable in insert/update mode)
        /// </summary>
        public static void SetQueryOnlyItems(BeepDataBlock block, params string[] itemNames)
        {
            foreach (var itemName in itemNames)
            {
                block.SetItemProperty(itemName, nameof(BeepDataBlockItem.InsertAllowed), false);
                block.SetItemProperty(itemName, nameof(BeepDataBlockItem.UpdateAllowed), false);
            }
        }
        
        /// <summary>
        /// Set insert-only items (disable in update mode)
        /// </summary>
        public static void SetInsertOnlyItems(BeepDataBlock block, params string[] itemNames)
        {
            foreach (var itemName in itemNames)
            {
                block.SetItemProperty(itemName, nameof(BeepDataBlockItem.UpdateAllowed), false);
            }
        }
        
        #endregion
        
        #region Property Validation
        
        /// <summary>
        /// Check if all required fields are filled
        /// </summary>
        public static bool AreRequiredFieldsFilled(BeepDataBlock block, out List<string> missingFields)
        {
            missingFields = new List<string>();
            
            var items = block.GetAllItems();
            
            foreach (var item in items.Values.Where(i => i.Required))
            {
                var value = item.Component?.GetValue();
                
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    missingFields.Add(item.ItemName);
                }
            }
            
            return missingFields.Count == 0;
        }
        
        /// <summary>
        /// Get all items with errors
        /// </summary>
        public static List<BeepDataBlockItem> GetItemsWithErrors(BeepDataBlock block)
        {
            return block.GetAllItems()
                .Values
                .Where(i => i.HasError)
                .ToList();
        }
        
        /// <summary>
        /// Clear all item errors
        /// </summary>
        public static void ClearAllItemErrors(BeepDataBlock block)
        {
            foreach (var item in block.GetAllItems().Values)
            {
                item.HasError = false;
                item.ErrorMessage = null;
                
                if (item.Component is BaseControl beepControl)
                {
                    beepControl.HasError = false;
                    beepControl.ErrorText = "";
                }
            }
        }
        
        #endregion
        
        #region Property Templates
        
        /// <summary>
        /// Apply standard primary key properties
        /// </summary>
        public static void ConfigurePrimaryKey(BeepDataBlock block, string itemName)
        {
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.Required), true);
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.InsertAllowed), true);
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.UpdateAllowed), false);
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.QueryAllowed), true);
        }
        
        /// <summary>
        /// Apply standard foreign key properties
        /// </summary>
        public static void ConfigureForeignKey(BeepDataBlock block, string itemName)
        {
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.Required), true);
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.QueryAllowed), true);
        }
        
        /// <summary>
        /// Apply standard audit field properties (CreatedBy, ModifiedBy, etc.)
        /// </summary>
        public static void ConfigureAuditFields(BeepDataBlock block, params string[] fieldNames)
        {
            foreach (var fieldName in fieldNames)
            {
                block.SetItemProperty(fieldName, nameof(BeepDataBlockItem.Enabled), false);
                block.SetItemProperty(fieldName, nameof(BeepDataBlockItem.InsertAllowed), false);
                block.SetItemProperty(fieldName, nameof(BeepDataBlockItem.UpdateAllowed), false);
                block.SetItemProperty(fieldName, nameof(BeepDataBlockItem.QueryAllowed), true);
            }
        }
        
        /// <summary>
        /// Apply standard computed field properties
        /// </summary>
        public static void ConfigureComputedField(BeepDataBlock block, string itemName)
        {
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.Enabled), false);
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.InsertAllowed), false);
            block.SetItemProperty(itemName, nameof(BeepDataBlockItem.UpdateAllowed), false);
        }
        
        #endregion
        
        #region Property Query
        
        /// <summary>
        /// Get all required items
        /// </summary>
        public static List<BeepDataBlockItem> GetRequiredItems(BeepDataBlock block)
        {
            return block.GetAllItems()
                .Values
                .Where(i => i.Required)
                .ToList();
        }
        
        /// <summary>
        /// Get all visible items
        /// </summary>
        public static List<BeepDataBlockItem> GetVisibleItems(BeepDataBlock block)
        {
            return block.GetAllItems()
                .Values
                .Where(i => i.Visible)
                .ToList();
        }
        
        /// <summary>
        /// Get all enabled items
        /// </summary>
        public static List<BeepDataBlockItem> GetEnabledItems(BeepDataBlock block)
        {
            return block.GetAllItems()
                .Values
                .Where(i => i.Enabled)
                .ToList();
        }
        
        /// <summary>
        /// Get all items with LOVs
        /// </summary>
        public static List<BeepDataBlockItem> GetItemsWithLOVs(BeepDataBlock block)
        {
            return block.GetAllItems()
                .Values
                .Where(i => !string.IsNullOrEmpty(i.LOVName))
                .ToList();
        }
        
        #endregion
    }
}

