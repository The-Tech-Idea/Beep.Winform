using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Integrated.Helpers;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;
using TheTechIdea.Beep.Editor.UOWManager.Models;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Examples
{
    /// <summary>
    /// Examples demonstrating Oracle Forms-compatible item properties in BeepDataBlock
    /// </summary>
    public static class OracleFormsItemPropertiesExamples
    {
        #region Example 1: Basic Item Properties
        
        /// <summary>
        /// Example: Set basic item properties (REQUIRED, ENABLED, VISIBLE)
        /// </summary>
        public static void Example1_BasicProperties(BeepDataBlock customerBlock)
        {
            // Make CustomerName required
            customerBlock.SetItemProperty("CustomerName", nameof(BeepDataBlockItem.Required), true);
            
            // Disable CustomerID (auto-generated)
            customerBlock.SetItemProperty("CustomerID", nameof(BeepDataBlockItem.Enabled), false);
            
            // Hide InternalNotes field
            customerBlock.SetItemProperty("InternalNotes", nameof(BeepDataBlockItem.Visible), false);
            
            // Set hint text
            customerBlock.SetItemProperty("Email", nameof(BeepDataBlockItem.HintText), 
                "Enter a valid email address");
        }
        
        #endregion
        
        #region Example 2: Using Property Helpers
        
        /// <summary>
        /// Example: Use helper methods for common operations
        /// </summary>
        public static void Example2_UsingHelpers(BeepDataBlock customerBlock)
        {
            // Make required using helper
            BeepDataBlockPropertyHelper.MakeRequired(customerBlock, "CustomerName");
            BeepDataBlockPropertyHelper.MakeRequired(customerBlock, "Email");
            
            // Disable items
            BeepDataBlockPropertyHelper.DisableItem(customerBlock, "CustomerID");
            BeepDataBlockPropertyHelper.DisableItem(customerBlock, "CreatedDate");
            
            // Hide items
            BeepDataBlockPropertyHelper.HideItem(customerBlock, "InternalNotes");
            
            // Set default values
            BeepDataBlockPropertyHelper.SetDefaultValue(customerBlock, "Status", "Active");
            BeepDataBlockPropertyHelper.SetDefaultValue(customerBlock, "CreatedDate", DateTime.Now);
            
            // Set hint text
            BeepDataBlockPropertyHelper.SetHintText(customerBlock, "Phone", 
                "Format: (555) 123-4567");
        }
        
        #endregion
        
        #region Example 3: Batch Operations
        
        /// <summary>
        /// Example: Apply properties to multiple items at once
        /// </summary>
        public static void Example3_BatchOperations(BeepDataBlock block)
        {
            // Make multiple fields required
            BeepDataBlockPropertyHelper.MakeRequiredBatch(block,
                "CustomerName", "Email", "Phone", "Address");
            
            // Disable audit fields (read-only)
            BeepDataBlockPropertyHelper.DisableItemsBatch(block,
                "CreatedBy", "CreatedDate", "ModifiedBy", "ModifiedDate");
            
            // Hide internal fields
            BeepDataBlockPropertyHelper.HideItemsBatch(block,
                "InternalNotes", "InternalCode", "SystemField");
        }
        
        #endregion
        
        #region Example 4: Mode-Based Properties
        
        /// <summary>
        /// Example: Configure items based on mode (Query vs CRUD)
        /// </summary>
        public static void Example4_ModeBasedProperties(BeepDataBlock block)
        {
            // Primary key: Allowed in Query, Insert only
            block.SetItemProperty("CustomerID", nameof(BeepDataBlockItem.QueryAllowed), true);
            block.SetItemProperty("CustomerID", nameof(BeepDataBlockItem.InsertAllowed), true);
            block.SetItemProperty("CustomerID", nameof(BeepDataBlockItem.UpdateAllowed), false);
            
            // Or use helper:
            BeepDataBlockPropertyHelper.ConfigurePrimaryKey(block, "CustomerID");
            
            // Foreign key: Always queryable and editable
            BeepDataBlockPropertyHelper.ConfigureForeignKey(block, "CountryID");
            
            // Audit fields: Query-only
            BeepDataBlockPropertyHelper.SetQueryOnlyItems(block,
                "CreatedBy", "CreatedDate", "ModifiedBy", "ModifiedDate");
            
            // Order number: Insert-only
            BeepDataBlockPropertyHelper.SetInsertOnlyItems(block, "OrderNumber");
        }
        
        #endregion
        
        #region Example 5: Property Templates
        
        /// <summary>
        /// Example: Use property templates for common patterns
        /// </summary>
        public static void Example5_PropertyTemplates(BeepDataBlock block)
        {
            // Configure primary key
            BeepDataBlockPropertyHelper.ConfigurePrimaryKey(block, "CustomerID");
            
            // Configure audit fields
            BeepDataBlockPropertyHelper.ConfigureAuditFields(block,
                "CreatedBy", "CreatedDate", "ModifiedBy", "ModifiedDate");
            
            // Configure computed field
            BeepDataBlockPropertyHelper.ConfigureComputedField(block, "FullName");
            BeepDataBlockPropertyHelper.ConfigureComputedField(block, "TotalAmount");
        }
        
        #endregion
        
        #region Example 6: Dynamic Properties with Triggers
        
        /// <summary>
        /// Example: Change properties dynamically based on conditions
        /// </summary>
        public static void Example6_DynamicProperties(BeepDataBlock customerBlock)
        {
            // Register trigger to change properties based on customer type
            customerBlock.RegisterTrigger(TriggerType.WhenNewRecordInstance, async context =>
            {
                // Get customer type
                var customerType = customerBlock.GetItemValue("CustomerType")?.ToString();
                
                if (customerType == "Corporate")
                {
                    // Corporate customers: Company name required, individual name optional
                    BeepDataBlockPropertyHelper.MakeRequired(customerBlock, "CompanyName");
                    BeepDataBlockPropertyHelper.MakeOptional(customerBlock, "FirstName");
                    BeepDataBlockPropertyHelper.MakeOptional(customerBlock, "LastName");
                    
                    // Show tax ID field
                    BeepDataBlockPropertyHelper.ShowItem(customerBlock, "TaxID");
                }
                else if (customerType == "Individual")
                {
                    // Individual customers: Individual name required, company optional
                    BeepDataBlockPropertyHelper.MakeRequired(customerBlock, "FirstName");
                    BeepDataBlockPropertyHelper.MakeRequired(customerBlock, "LastName");
                    BeepDataBlockPropertyHelper.MakeOptional(customerBlock, "CompanyName");
                    
                    // Hide tax ID field
                    BeepDataBlockPropertyHelper.HideItem(customerBlock, "TaxID");
                }
                
                await Task.CompletedTask;
                return true;
            });
        }
        
        #endregion
        
        #region Example 7: Property Validation
        
        /// <summary>
        /// Example: Validate required fields before commit
        /// </summary>
        public static void Example7_RequiredValidation(BeepDataBlock block)
        {
            // Register PRE-FORM-COMMIT trigger to validate required fields
            block.RegisterTrigger(TriggerType.PreFormCommit, async context =>
            {
                // Validate required fields
                if (!block.ValidateRequiredFields(out var errors))
                {
                    // Show errors
                    context.ErrorMessage = "The following required fields are missing:\n" +
                        string.Join("\n", errors.Select(e => $"  • {e}"));
                    
                    System.Windows.Forms.MessageBox.Show(context.ErrorMessage, "Required Fields Missing",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    
                    context.Cancel = true;
                    
                    await Task.CompletedTask;
                    return false;  // Cancel operation
                }
                
                await Task.CompletedTask;
                return true;  // Continue operation
            });
        }
        
        #endregion
        
        #region Example 8: Block Properties
        
        /// <summary>
        /// Example: Set block-level properties
        /// </summary>
        public static void Example8_BlockProperties(BeepDataBlock customerBlock)
        {
            // Set WHERE clause for block query
            customerBlock.WhereClause = "IsActive = 1 AND Region = 'US'";
            
            // Set ORDER BY clause
            customerBlock.OrderByClause = "CustomerName ASC";
            
            // Set data source
            customerBlock.QueryDataSourceName = "MainDB";
            
            // Control block behavior
            customerBlock.InsertAllowed = true;
            customerBlock.UpdateAllowed = true;
            customerBlock.DeleteAllowed = false;  // Read-only delete
            customerBlock.QueryAllowed = true;
            
            // Set max records
            customerBlock.BlockProperties.MaxRecords = 500;
        }
        
        #endregion
        
        #region Example 9: Complete Form Configuration
        
        /// <summary>
        /// Example: Complete form with all property types
        /// </summary>
        public static void Example9_CompleteFormConfiguration(BeepDataBlock customerBlock)
        {
            // ========================================
            // PRIMARY KEY
            // ========================================
            BeepDataBlockPropertyHelper.ConfigurePrimaryKey(customerBlock, "CustomerID");
            BeepDataBlockPropertyHelper.SetDefaultValue(customerBlock, "CustomerID", 
                Guid.NewGuid().ToString());
            
            // ========================================
            // REQUIRED FIELDS
            // ========================================
            BeepDataBlockPropertyHelper.MakeRequiredBatch(customerBlock,
                "CustomerName", "Email", "Phone", "Country");
            
            // ========================================
            // AUDIT FIELDS
            // ========================================
            BeepDataBlockPropertyHelper.ConfigureAuditFields(customerBlock,
                "CreatedBy", "CreatedDate", "ModifiedBy", "ModifiedDate");
            
            // ========================================
            // COMPUTED FIELDS
            // ========================================
            BeepDataBlockPropertyHelper.ConfigureComputedField(customerBlock, "FullName");
            BeepDataBlockPropertyHelper.ConfigureComputedField(customerBlock, "TotalOrders");
            BeepDataBlockPropertyHelper.ConfigureComputedField(customerBlock, "LifetimeValue");
            
            // ========================================
            // FOREIGN KEYS WITH LOVs
            // ========================================
            BeepDataBlockPropertyHelper.ConfigureForeignKey(customerBlock, "CountryID");
            customerBlock.SetItemProperty("CountryID", nameof(BeepDataBlockItem.LOVName), "COUNTRIES_LOV");
            
            BeepDataBlockPropertyHelper.ConfigureForeignKey(customerBlock, "SalesRepID");
            customerBlock.SetItemProperty("SalesRepID", nameof(BeepDataBlockItem.LOVName), "SALES_REPS_LOV");
            
            // ========================================
            // HINT TEXT
            // ========================================
            BeepDataBlockPropertyHelper.SetHintText(customerBlock, "Email", 
                "Format: user@domain.com");
            BeepDataBlockPropertyHelper.SetHintText(customerBlock, "Phone", 
                "Format: (555) 123-4567");
            BeepDataBlockPropertyHelper.SetHintText(customerBlock, "CreditLimit", 
                "Maximum credit allowed for this customer");
            
            // ========================================
            // DEFAULT VALUES
            // ========================================
            BeepDataBlockPropertyHelper.SetDefaultValue(customerBlock, "Status", "Active");
            BeepDataBlockPropertyHelper.SetDefaultValue(customerBlock, "CustomerType", "Individual");
            BeepDataBlockPropertyHelper.SetDefaultValue(customerBlock, "CreditLimit", 5000m);
            BeepDataBlockPropertyHelper.SetDefaultValue(customerBlock, "DiscountPercent", 0);
            
            // ========================================
            // BLOCK PROPERTIES
            // ========================================
            customerBlock.WhereClause = "IsActive = 1";
            customerBlock.OrderByClause = "CustomerName ASC";
            customerBlock.InsertAllowed = true;
            customerBlock.UpdateAllowed = true;
            customerBlock.DeleteAllowed = false;
            customerBlock.QueryAllowed = true;
            
            // Apply all properties to UI
            customerBlock.ApplyAllItemProperties();
        }
        
        #endregion
        
        #region Example 10: Property-Driven Validation
        
        /// <summary>
        /// Example: Use item properties for comprehensive validation
        /// </summary>
        public static void Example10_PropertyDrivenValidation(BeepDataBlock block)
        {
            // Configure required fields
            BeepDataBlockPropertyHelper.MakeRequiredBatch(block,
                "CustomerName", "Email", "Phone");
            
            // Configure max lengths
            block.SetItemProperty("CustomerName", nameof(BeepDataBlockItem.MaxLength), 100);
            block.SetItemProperty("Email", nameof(BeepDataBlockItem.MaxLength), 100);
            block.SetItemProperty("Phone", nameof(BeepDataBlockItem.MaxLength), 20);
            
            // Register WHEN-VALIDATE-RECORD trigger
            block.RegisterTrigger(TriggerType.WhenValidateRecord, async context =>
            {
                // Validate required fields
                if (!BeepDataBlockPropertyHelper.AreRequiredFieldsFilled(block, out var missingFields))
                {
                    context.Cancel = true;
                    context.ErrorMessage = $"Missing required fields: {string.Join(", ", missingFields)}";
                    
                    await Task.CompletedTask;
                    return false;  // Cancel operation
                }
                
                // Validate max lengths
                foreach (var item in block.GetAllItems().Values.Where(i => i.MaxLength > 0))
                {
                    var value = item.Component?.GetValue()?.ToString();
                    if (!string.IsNullOrEmpty(value) && value.Length > item.MaxLength)
                    {
                        context.Cancel = true;
                        context.ErrorMessage = $"{item.ItemName} exceeds maximum length of {item.MaxLength}";
                        
                        await Task.CompletedTask;
                        return false;  // Cancel operation
                    }
                }
                
                await Task.CompletedTask;
                return true;  // Continue operation
            });
        }
        
        #endregion
    }
}

