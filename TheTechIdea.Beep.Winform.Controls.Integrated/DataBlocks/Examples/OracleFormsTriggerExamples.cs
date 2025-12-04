using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Helpers;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Examples
{
    /// <summary>
    /// Examples demonstrating Oracle Forms-compatible trigger usage in BeepDataBlock
    /// </summary>
    public static class OracleFormsTriggerExamples
    {
        #region Example 1: Basic Trigger Registration
        
        /// <summary>
        /// Example: Register basic triggers for a customer form
        /// </summary>
        public static void Example1_BasicTriggers(BeepDataBlock customerBlock)
        {
            // WHEN-NEW-RECORD-INSTANCE: Set default values
            customerBlock.RegisterTrigger(TriggerType.WhenNewRecordInstance, async (context) =>
            {
                if (context.Block is BeepDataBlock block)
                {
                    block.SetItemValue("CreatedDate", DateTime.Now);
                    block.SetItemValue("Status", "Active");
                    block.SetItemValue("CreditLimit", 5000.00m);
                    
                    MessageBox.Show("New customer record initialized with default values.");
                }
                return true;
            });
            
            // WHEN-VALIDATE-RECORD: Validate before save
            customerBlock.RegisterTrigger(TriggerType.WhenValidateRecord, async (context) =>
            {
                var companyName = context.GetRecordValue<string>("CompanyName");
                
                if (string.IsNullOrEmpty(companyName))
                {
                    context.SetError("Company name is required");
                    return false;
                }
                
                return true;
            });
            
            // POST-QUERY: Calculate computed fields
            customerBlock.RegisterTrigger(TriggerType.PostQuery, async (context) =>
            {
                // Calculate total orders for this customer
                var customerId = context.GetRecordValue<int>("CustomerID");
                var totalOrders = await GetTotalOrders(customerId);
                
                if (context.Block is BeepDataBlock block)
                {
                    block.SetItemValue("TotalOrders", totalOrders);
                }
                return true;
            });
        }
        
        #endregion
        
        #region Example 2: Master-Detail with Triggers
        
        /// <summary>
        /// Example: Setup master-detail blocks with coordinating triggers
        /// </summary>
        public static void Example2_MasterDetailTriggers(
            BeepDataBlock customerBlock, 
            BeepDataBlock ordersBlock)
        {
            // Customer block (master)
            customerBlock.RegisterTrigger(TriggerType.PostRecordNavigate, async (context) =>
            {
                // When master navigates, update summary
                var customerName = context.GetRecordValue<string>("CompanyName");
                var orderCount = context.Block.ChildBlocks
                    .FirstOrDefault(c => c.Name == "ORDERS")
                    ?.Data?.Units?.Count ?? 0;
                    
                MessageBox.Show($"Customer: {customerName} has {orderCount} orders.");
                return true;
            });
            
            // Orders block (detail)
            ordersBlock.RegisterTrigger(TriggerType.WhenNewRecordInstance, async (context) =>
            {
                if (context.Block is BeepDataBlock block)
                {
                    // Auto-populate customer ID from master
                    var masterRecord = context.Block.ParentBlock?.Data?.Units?.Current;
                    if (masterRecord != null)
                    {
                        var customerId = BeepDataBlockTriggerHelper.GetFieldValue(masterRecord, "CustomerID");
                        block.SetItemValue("CustomerID", customerId);
                    }
                    
                    // Set default values
                    block.SetItemValue("OrderDate", DateTime.Now);
                    block.SetItemValue("Status", "Pending");
                }
                
                return true;
            });
            
            // Orders: Calculate order total after query
            ordersBlock.RegisterTrigger(TriggerType.PostQuery, async (context) =>
            {
                // Update master with total order amount
                var orderTotal = ordersBlock.Data?.Units
                    ?.Sum(order => Convert.ToDecimal(
                        BeepDataBlockTriggerHelper.GetFieldValue(order, "OrderTotal") ?? 0)) ?? 0;
                        
                if (context.Block.ParentBlock is BeepDataBlock parentBlock)
                {
                    parentBlock.SetItemValue("TotalOrderAmount", orderTotal);
                }
                
                return true;
            });
        }
        
        #endregion
        
        #region Example 3: Complex Validation
        
        /// <summary>
        /// Example: Complex business rules with triggers
        /// </summary>
        public static void Example3_ComplexValidation(BeepDataBlock ordersBlock)
        {
            // Multi-step validation trigger
            ordersBlock.RegisterTrigger(TriggerType.WhenValidateRecord, async (context) =>
            {
                // Step 1: Validate order total
                var orderTotal = context.GetRecordValue<decimal>("OrderTotal");
                if (orderTotal <= 0)
                {
                    context.SetError("Order total must be greater than zero");
                    return false;
                }
                
                // Step 2: Validate ship date
                var orderDate = context.GetRecordValue<DateTime>("OrderDate");
                var shipDate = context.GetRecordValue<DateTime>("ShipDate");
                
                if (shipDate < orderDate)
                {
                    context.SetError("Ship date cannot be before order date");
                    return false;
                }
                
                // Step 3: Check credit limit
                var customerId = context.GetRecordValue<int>("CustomerID");
                var creditCheck = await ValidateCreditLimit(customerId, orderTotal);
                
                if (!creditCheck.IsValid)
                {
                    context.SetError(creditCheck.Message);
                    return false;
                }
                
                // Step 4: Check inventory
                var productId = context.GetRecordValue<int>("ProductID");
                var quantity = context.GetRecordValue<int>("Quantity");
                var inventoryCheck = await CheckInventory(productId, quantity);
                
                if (!inventoryCheck.IsValid)
                {
                    context.AddWarning(inventoryCheck.Message);
                    // Continue with warning (not error)
                }
                
                return true;
            });
        }
        
        private static async Task<(bool IsValid, string Message)> ValidateCreditLimit(int customerId, decimal orderTotal)
        {
            // Simulate async credit check
            await Task.Delay(10);
            return (true, "Credit OK");
        }
        
        private static async Task<(bool IsValid, string Message)> CheckInventory(int productId, int quantity)
        {
            // Simulate async inventory check
            await Task.Delay(10);
            return (true, "Inventory OK");
        }
        
        #endregion
        
        #region Example 4: Computed Fields
        
        /// <summary>
        /// Example: Automatic field calculation with triggers
        /// </summary>
        public static void Example4_ComputedFields(BeepDataBlock orderItemsBlock)
        {
            // Calculate line total when quantity or price changes
            BeepDataBlockTriggerHelper.RegisterComputedFieldTrigger(
                orderItemsBlock,
                resultField: "LineTotal",
                sourceFields: new[] { "Quantity", "UnitPrice", "Discount" },
                computation: (recordValues) =>
                {
                    var quantity = Convert.ToInt32(recordValues.GetValueOrDefault("Quantity", 0));
                    var unitPrice = Convert.ToDecimal(recordValues.GetValueOrDefault("UnitPrice", 0m));
                    var discount = Convert.ToDecimal(recordValues.GetValueOrDefault("Discount", 0m));
                    
                    return (quantity * unitPrice) * (1 - discount);
                });
        }
        
        #endregion
        
        #region Example 5: Error Handling
        
        /// <summary>
        /// Example: Custom error handling with ON-ERROR trigger
        /// </summary>
        public static void Example5_ErrorHandling(BeepDataBlock block)
        {
            // ON-ERROR trigger for custom error handling
            block.RegisterTrigger(TriggerType.OnError, async (context) =>
            {
                var ex = context.GetParameter<Exception>("Exception");
                var originalTrigger = context.GetParameter<TriggerType>("OriginalTrigger");
                
                // Log error
                Console.WriteLine($"Error in trigger {originalTrigger}: {ex?.Message}");
                
                // Show user-friendly message
                MessageBox.Show(
                    $"An error occurred while processing your request.\n\n" +
                    $"Operation: {originalTrigger}\n" +
                    $"Error: {ex?.Message}",
                    "Application Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                
                // Set system variables
                if (context.SYSTEM != null)
                {
                    context.SYSTEM.SetError("ERR001", ex?.Message ?? "Unknown error");
                }
                
                return true;  // Error handled
            });
        }
        
        #endregion
        
        #region Example 6: Conditional Logic
        
        /// <summary>
        /// Example: Conditional business rules with triggers
        /// </summary>
        public static void Example6_ConditionalLogic(BeepDataBlock customerBlock)
        {
            // PRE-INSERT: Apply different rules based on customer type
            customerBlock.RegisterTrigger(TriggerType.PreInsert, async (context) =>
            {
                var customerType = context.GetRecordValue<string>("CustomerType");
                var taxId = context.GetRecordValue<string>("TaxID");
                var creditLimit = context.GetRecordValue<decimal>("CreditLimit");
                
                // Rule: Corporate customers require Tax ID
                if (customerType == "Corporate" && string.IsNullOrEmpty(taxId))
                {
                    context.SetError("Tax ID is required for corporate customers");
                    return false;
                }
                
                // Rule: Retail customers have lower credit limits
                if (customerType == "Retail" && creditLimit > 10000)
                {
                    context.AddWarning("High credit limit for retail customer");
                    // Continue with warning
                }
                
                // Rule: Wholesale customers get discount
                if (customerType == "Wholesale" && context.Block is BeepDataBlock block)
                {
                    block.SetItemValue("DiscountPercent", 10);
                }
                
                return true;
            });
        }
        
        #endregion
        
        #region Example 7: Audit Trail
        
        /// <summary>
        /// Example: Complete audit trail using helper
        /// </summary>
        public static void Example7_AuditTrail(BeepDataBlock block)
        {
            // Use helper to register standard audit triggers
            BeepDataBlockTriggerHelper.RegisterAuditTriggers(block, Environment.UserName);
            
            // Additional audit: Log all changes
            block.RegisterTrigger(TriggerType.PreUpdate, async (context) =>
            {
                // Log what changed
                var changes = GetChangedFields(context);
                
                Console.WriteLine($"Record updated by {Environment.UserName}:");
                foreach (var change in changes)
                {
                    Console.WriteLine($"  {change.Key}: {change.Value.OldValue} → {change.Value.NewValue}");
                }
                
                return true;
            }, executionOrder: 100);  // Execute after audit trigger (order 10)
        }
        
        private static Dictionary<string, (object OldValue, object NewValue)> GetChangedFields(TriggerContext context)
        {
            // Implementation would track changed fields
            return new Dictionary<string, (object OldValue, object NewValue)>();
        }
        
        #endregion
        
        #region Example 8: Named Triggers
        
        /// <summary>
        /// Example: Named triggers for enable/disable control
        /// </summary>
        public static void Example8_NamedTriggers(BeepDataBlock block)
        {
            // Register named trigger
            block.RegisterTrigger(
                triggerName: "VALIDATE_CREDIT_LIMIT",
                type: TriggerType.WhenValidateRecord,
                handler: async (context) =>
                {
                    var orderTotal = context.GetRecordValue<decimal>("OrderTotal");
                    var customerId = context.GetRecordValue<int>("CustomerID");
                    
                    // Check credit limit
                    var creditLimit = await GetCustomerCreditLimit(customerId);
                    
                    if (orderTotal > creditLimit)
                    {
                        context.SetError($"Order total ({orderTotal:C}) exceeds credit limit ({creditLimit:C})");
                        return false;
                    }
                    
                    return true;
                },
                description: "Validates order total against customer credit limit");
            
            // Later: Disable for admin users
            if (IsAdminUser())
            {
                block.DisableTrigger("VALIDATE_CREDIT_LIMIT");
            }
            
            // Re-enable after admin override
            block.EnableTrigger("VALIDATE_CREDIT_LIMIT");
        }
        
        private static bool IsAdminUser()
        {
            return false;  // Placeholder
        }
        
        private static async Task<decimal> GetCustomerCreditLimit(int customerId)
        {
            await Task.Delay(10);
            return 10000m;  // Placeholder
        }
        
        #endregion
        
        #region Example 9: Trigger Statistics
        
        /// <summary>
        /// Example: Monitor trigger performance
        /// </summary>
        public static void Example9_TriggerStatistics(BeepDataBlock block)
        {
            // Get trigger statistics
            var stats = BeepDataBlockTriggerHelper.GetTriggerStatistics(block);
            
            Console.WriteLine("Trigger Statistics:");
            Console.WriteLine($"  Total Triggers: {stats.TotalTriggers}");
            Console.WriteLine($"  Enabled: {stats.EnabledTriggers}");
            Console.WriteLine($"  Disabled: {stats.DisabledTriggers}");
            Console.WriteLine($"  Total Executions: {stats.TotalExecutions}");
            Console.WriteLine($"  Total Cancellations: {stats.TotalCancellations}");
            Console.WriteLine($"  Average Duration: {stats.AverageExecutionMs:F2}ms");
            
            if (stats.MostExecutedTrigger != null)
            {
                Console.WriteLine($"  Most Executed: {stats.MostExecutedTrigger.TriggerName} " +
                                $"({stats.MostExecutedTrigger.ExecutionCount} times)");
            }
        }
        
        #endregion
        
        #region Example 10: Complete Customer-Orders Form
        
        /// <summary>
        /// Example: Complete setup for a Customer-Orders form (like Oracle Forms)
        /// </summary>
        public static void Example10_CompleteForm(
            BeepDataBlock customerBlock,
            BeepDataBlock ordersBlock,
            BeepDataBlock orderItemsBlock)
        {
            // ========================================
            // CUSTOMER BLOCK (Master)
            // ========================================
            
            // Audit trail
            BeepDataBlockTriggerHelper.RegisterAuditTriggers(customerBlock);
            
            // Default values
            BeepDataBlockTriggerHelper.RegisterDefaultValueTrigger(customerBlock, 
                new Dictionary<string, object>
                {
                    ["Status"] = "Active",
                    ["CreditLimit"] = 5000.00m,
                    ["DiscountPercent"] = 0
                });
            
            // Validation
            customerBlock.RegisterTrigger(TriggerType.WhenValidateRecord, async (context) =>
            {
                var email = context.GetRecordValue<string>("Email");
                if (!IsValidEmail(email))
                {
                    context.SetError("Invalid email format");
                    return false;
                }
                return true;
            });
            
            // ========================================
            // ORDERS BLOCK (Detail)
            // ========================================
            
            // Audit trail
            BeepDataBlockTriggerHelper.RegisterAuditTriggers(ordersBlock);
            
            // Auto-populate from master
            ordersBlock.RegisterTrigger(TriggerType.WhenNewRecordInstance, async (context) =>
            {
                if (context.Block is BeepDataBlock block)
                {
                    var masterRecord = context.Block.ParentBlock?.Data?.Units?.Current;
                    if (masterRecord != null)
                    {
                        var customerId = BeepDataBlockTriggerHelper.GetFieldValue(masterRecord, "CustomerID");
                        var customerName = BeepDataBlockTriggerHelper.GetFieldValue(masterRecord, "CompanyName");
                        
                        block.SetItemValue("CustomerID", customerId);
                        block.SetItemValue("CustomerName", customerName);
                    }
                    
                    block.SetItemValue("OrderDate", DateTime.Now);
                    block.SetItemValue("Status", "Pending");
                }
                
                return true;
            });
            
            // Cross-field validation
            ordersBlock.RegisterTrigger(TriggerType.WhenValidateRecord, async (context) =>
            {
                var orderDate = context.GetRecordValue<DateTime>("OrderDate");
                var shipDate = context.GetRecordValue<DateTime>("ShipDate");
                
                if (shipDate < orderDate)
                {
                    context.SetError("Ship date must be after order date");
                    return false;
                }
                
                return true;
            });
            
            // Update master summary
            ordersBlock.RegisterTrigger(TriggerType.PostQuery, async (context) =>
            {
                if (context.Block is BeepDataBlock block)
                {
                    var orderTotal = block.Data?.Units
                        ?.Sum(o => Convert.ToDecimal(BeepDataBlockTriggerHelper.GetFieldValue(o, "OrderTotal") ?? 0)) ?? 0;
                        
                    if (context.Block.ParentBlock is BeepDataBlock parentBlock)
                    {
                        parentBlock.SetItemValue("TotalOrderAmount", orderTotal);
                    }
                }
                
                return true;
            });
            
            // ========================================
            // ORDER ITEMS BLOCK (Detail-Detail)
            // ========================================
            
            // Auto-populate from master
            orderItemsBlock.RegisterTrigger(TriggerType.WhenNewRecordInstance, async (context) =>
            {
                if (context.Block is BeepDataBlock block)
                {
                    var masterRecord = context.Block.ParentBlock?.Data?.Units?.Current;
                    if (masterRecord != null)
                    {
                        var orderId = BeepDataBlockTriggerHelper.GetFieldValue(masterRecord, "OrderID");
                        block.SetItemValue("OrderID", orderId);
                    }
                    
                    block.SetItemValue("Quantity", 1);
                    block.SetItemValue("Discount", 0);
                }
                
                return true;
            });
            
            // Calculate line total
            BeepDataBlockTriggerHelper.RegisterComputedFieldTrigger(
                orderItemsBlock,
                resultField: "LineTotal",
                sourceFields: new[] { "Quantity", "UnitPrice", "Discount" },
                computation: (values) =>
                {
                    var quantity = Convert.ToInt32(values.GetValueOrDefault("Quantity", 0));
                    var unitPrice = Convert.ToDecimal(values.GetValueOrDefault("UnitPrice", 0m));
                    var discount = Convert.ToDecimal(values.GetValueOrDefault("Discount", 0m));
                    
                    return (quantity * unitPrice) * (1 - discount);
                });
        }
        
        private static bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && email.Contains("@");
        }
        
        private static async Task<int> GetTotalOrders(int customerId)
        {
            await Task.Delay(10);
            return 0;  // Placeholder
        }
        
        #endregion
    }
}

