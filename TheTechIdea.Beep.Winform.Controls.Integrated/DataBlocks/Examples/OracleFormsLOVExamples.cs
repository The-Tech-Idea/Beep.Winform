using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Examples
{
    /// <summary>
    /// Examples demonstrating Oracle Forms-compatible LOV usage in BeepDataBlock
    /// </summary>
    public static class OracleFormsLOVExamples
    {
        #region Example 1: Basic LOV Registration
        
        /// <summary>
        /// Example: Register a simple LOV for customer selection
        /// </summary>
        public static void Example1_BasicLOV(BeepDataBlock ordersBlock)
        {
            // Register LOV for CustomerID field
            ordersBlock.RegisterLOV("CustomerID", new BeepDataBlockLOV
            {
                LOVName = "CUSTOMERS_LOV",
                Title = "Select Customer",
                DataSourceName = "MainDB",
                EntityName = "Customers",
                DisplayField = "CompanyName",
                ReturnField = "CustomerID",
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "CustomerID", DisplayName = "ID", Width = 80 },
                    new LOVColumn { FieldName = "CompanyName", DisplayName = "Company", Width = 200 },
                    new LOVColumn { FieldName = "ContactName", DisplayName = "Contact", Width = 150 }
                },
                ValidationType = LOVValidationType.ListOnly  // Must select from list
            });
            
            // User can now:
            // 1. Press F9 on CustomerID field → LOV popup appears
            // 2. Double-click CustomerID field → LOV popup appears
            // 3. Select customer → CustomerID populated
        }
        
        #endregion
        
        #region Example 2: LOV with Auto-Populate
        
        /// <summary>
        /// Example: LOV that auto-populates related fields
        /// </summary>
        public static void Example2_LOVWithAutoPopulate(BeepDataBlock ordersBlock)
        {
            ordersBlock.RegisterLOV("CustomerID", new BeepDataBlockLOV
            {
                LOVName = "CUSTOMERS_LOV",
                Title = "Select Customer",
                DataSourceName = "MainDB",
                EntityName = "Customers",
                DisplayField = "CompanyName",
                ReturnField = "CustomerID",
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "CustomerID", DisplayName = "ID", Width = 80 },
                    new LOVColumn { FieldName = "CompanyName", DisplayName = "Company", Width = 200 },
                    new LOVColumn { FieldName = "ContactName", DisplayName = "Contact", Width = 150 },
                    new LOVColumn { FieldName = "Phone", DisplayName = "Phone", Width = 120 },
                    new LOVColumn { FieldName = "Address", DisplayName = "Address", Width = 200 }
                },
                ValidationType = LOVValidationType.ListOnly,
                AutoPopulateRelatedFields = true,
                RelatedFieldMappings = new Dictionary<string, string>
                {
                    ["CompanyName"] = "CustomerName",      // LOV field → Block field
                    ["ContactName"] = "CustomerContact",
                    ["Phone"] = "CustomerPhone",
                    ["Address"] = "CustomerAddress"
                }
            });
            
            // When user selects customer:
            // → CustomerID populated (return field)
            // → CustomerName populated (from CompanyName)
            // → CustomerContact populated (from ContactName)
            // → CustomerPhone populated (from Phone)
            // → CustomerAddress populated (from Address)
            // All automatically!
        }
        
        #endregion
        
        #region Example 3: LOV with Filters
        
        /// <summary>
        /// Example: LOV with pre-defined filters (active customers only)
        /// </summary>
        public static void Example3_LOVWithFilters(BeepDataBlock ordersBlock)
        {
            ordersBlock.RegisterLOV("CustomerID", new BeepDataBlockLOV
            {
                LOVName = "ACTIVE_CUSTOMERS_LOV",
                Title = "Select Active Customer",
                DataSourceName = "MainDB",
                EntityName = "Customers",
                DisplayField = "CompanyName",
                ReturnField = "CustomerID",
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "CustomerID", DisplayName = "ID", Width = 80 },
                    new LOVColumn { FieldName = "CompanyName", DisplayName = "Company", Width = 200 },
                    new LOVColumn { FieldName = "Status", DisplayName = "Status", Width = 100 }
                },
                Filters = new List<AppFilter>
                {
                    new AppFilter { FieldName = "IsActive", Operator = "=", FilterValue = "true" },
                    new AppFilter { FieldName = "Status", Operator = "=", FilterValue = "Active" }
                },
                OrderByClause = "CompanyName ASC"
            });
            
            // LOV will only show active customers, sorted by company name
        }
        
        #endregion
        
        #region Example 4: LOV with Events
        
        /// <summary>
        /// Example: LOV with event handlers
        /// </summary>
        public static void Example4_LOVWithEvents(BeepDataBlock ordersBlock)
        {
            var lov = new BeepDataBlockLOV
            {
                LOVName = "CUSTOMERS_LOV",
                Title = "Select Customer",
                DataSourceName = "MainDB",
                EntityName = "Customers",
                DisplayField = "CompanyName",
                ReturnField = "CustomerID",
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "CustomerID", DisplayName = "ID", Width = 80 },
                    new LOVColumn { FieldName = "CompanyName", DisplayName = "Company", Width = 200 }
                }
            };
            
            // BEFORE-DISPLAY event: Can cancel LOV display
            lov.BeforeDisplay += (sender, args) =>
            {
                // Check if user has permission
                if (!HasPermission("VIEW_CUSTOMERS"))
                {
                    MessageBox.Show("You don't have permission to view customers", "Access Denied",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    args.Cancel = true;
                }
            };
            
            // AFTER-SELECTION event: Custom logic after selection
            lov.AfterSelection += (sender, args) =>
            {
                var customerId = args.SelectedValues.FirstOrDefault();
                MessageBox.Show($"Customer {customerId} selected", "Selection Confirmed",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                // Log selection
                LogCustomerSelection(customerId);
            };
            
            // ON-CANCEL event: Handle cancellation
            lov.OnCancel += (sender, args) =>
            {
                Console.WriteLine("LOV selection cancelled by user");
            };
            
            ordersBlock.RegisterLOV("CustomerID", lov);
        }
        
        private static bool HasPermission(string permission) => true;
        private static void LogCustomerSelection(object customerId) { }
        
        #endregion
        
        #region Example 5: Multi-Select LOV
        
        /// <summary>
        /// Example: LOV with multi-select capability
        /// </summary>
        public static void Example5_MultiSelectLOV(BeepDataBlock reportBlock)
        {
            reportBlock.RegisterLOV("SelectedCustomers", new BeepDataBlockLOV
            {
                LOVName = "CUSTOMERS_MULTISELECT_LOV",
                Title = "Select Customers for Report",
                DataSourceName = "MainDB",
                EntityName = "Customers",
                DisplayField = "CompanyName",
                ReturnField = "CustomerID",
                AllowMultiSelect = true,  // Enable multi-select
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "CustomerID", DisplayName = "ID", Width = 80 },
                    new LOVColumn { FieldName = "CompanyName", DisplayName = "Company", Width = 200 },
                    new LOVColumn { FieldName = "City", DisplayName = "City", Width = 120 }
                }
            });
            
            // User can select multiple customers
            // SelectedCustomers field will contain comma-separated IDs
        }
        
        #endregion
        
        #region Example 6: LOV with Cache
        
        /// <summary>
        /// Example: LOV with caching for performance
        /// </summary>
        public static void Example6_LOVWithCache(BeepDataBlock block)
        {
            block.RegisterLOV("ProductID", new BeepDataBlockLOV
            {
                LOVName = "PRODUCTS_LOV",
                Title = "Select Product",
                DataSourceName = "MainDB",
                EntityName = "Products",
                DisplayField = "ProductName",
                ReturnField = "ProductID",
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "ProductID", DisplayName = "ID", Width = 80 },
                    new LOVColumn { FieldName = "ProductName", DisplayName = "Product", Width = 200 },
                    new LOVColumn { FieldName = "UnitPrice", DisplayName = "Price", Width = 100, Format = "C2" }
                },
                UseCache = true,  // Enable caching
                CacheDurationMinutes = 30,  // Cache for 30 minutes
                AutoRefresh = false  // Don't refresh on every open
            });
            
            // First time: Queries database
            // Subsequent times (within 30 min): Uses cache
            // Performance improvement for large LOVs!
            
            // To manually refresh cache:
            // block.GetLOV("ProductID").ClearCache();
            // Or clear all:
            // block.ClearAllLOVCaches();
        }
        
        #endregion
        
        #region Example 7: LOV Validation
        
        /// <summary>
        /// Example: LOV validation types
        /// </summary>
        public static void Example7_LOVValidation(BeepDataBlock block)
        {
            // List Only: User MUST select from LOV
            block.RegisterLOV("StatusCode", new BeepDataBlockLOV
            {
                LOVName = "STATUS_CODES_LOV",
                Title = "Select Status",
                DataSourceName = "MainDB",
                EntityName = "StatusCodes",
                DisplayField = "StatusName",
                ReturnField = "StatusCode",
                ValidationType = LOVValidationType.ListOnly,  // Strict validation
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "StatusCode", DisplayName = "Code", Width = 80 },
                    new LOVColumn { FieldName = "StatusName", DisplayName = "Status", Width = 150 }
                }
            });
            
            // Unrestricted: User can type any value
            block.RegisterLOV("Notes", new BeepDataBlockLOV
            {
                LOVName = "COMMON_NOTES_LOV",
                Title = "Select or Enter Notes",
                DataSourceName = "MainDB",
                EntityName = "CommonNotes",
                DisplayField = "NoteText",
                ReturnField = "NoteText",
                ValidationType = LOVValidationType.Unrestricted,  // No validation
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "NoteText", DisplayName = "Note", Width = 300 }
                }
            });
        }
        
        #endregion
        
        #region Example 8: LOV with Search Modes
        
        /// <summary>
        /// Example: Different LOV search modes
        /// </summary>
        public static void Example8_LOVSearchModes(BeepDataBlock block)
        {
            // Contains search (default)
            block.RegisterLOV("CustomerID", new BeepDataBlockLOV
            {
                LOVName = "CUSTOMERS_CONTAINS_LOV",
                Title = "Find Customer (Contains)",
                DataSourceName = "MainDB",
                EntityName = "Customers",
                DisplayField = "CompanyName",
                ReturnField = "CustomerID",
                SearchMode = LOVSearchMode.Contains,  // Searches anywhere in string
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "CompanyName", DisplayName = "Company", Width = 200 }
                }
            });
            
            // StartsWith search
            block.RegisterLOV("ProductCode", new BeepDataBlockLOV
            {
                LOVName = "PRODUCTS_STARTSWITH_LOV",
                Title = "Find Product (Starts With)",
                DataSourceName = "MainDB",
                EntityName = "Products",
                DisplayField = "ProductCode",
                ReturnField = "ProductID",
                SearchMode = LOVSearchMode.StartsWith,  // Searches at start of string
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "ProductCode", DisplayName = "Code", Width = 100 },
                    new LOVColumn { FieldName = "ProductName", DisplayName = "Product", Width = 200 }
                }
            });
        }
        
        #endregion
        
        #region Example 9: Complete Customer-Orders with LOVs
        
        /// <summary>
        /// Example: Complete form with LOVs for all lookup fields
        /// </summary>
        public static void Example9_CompleteFormWithLOVs(
            BeepDataBlock customerBlock,
            BeepDataBlock ordersBlock,
            BeepDataBlock orderItemsBlock)
        {
            // ========================================
            // ORDERS BLOCK: Customer LOV
            // ========================================
            
            ordersBlock.RegisterLOV("CustomerID", new BeepDataBlockLOV
            {
                LOVName = "CUSTOMERS_LOV",
                Title = "Select Customer",
                DataSourceName = "MainDB",
                EntityName = "Customers",
                DisplayField = "CompanyName",
                ReturnField = "CustomerID",
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "CustomerID", DisplayName = "ID", Width = 80 },
                    new LOVColumn { FieldName = "CompanyName", DisplayName = "Company", Width = 200 },
                    new LOVColumn { FieldName = "ContactName", DisplayName = "Contact", Width = 150 },
                    new LOVColumn { FieldName = "Phone", DisplayName = "Phone", Width = 120 }
                },
                Filters = new List<AppFilter>
                {
                    new AppFilter { FieldName = "IsActive", Operator = "=", FilterValue = "true" }
                },
                OrderByClause = "CompanyName ASC",
                AutoPopulateRelatedFields = true,
                RelatedFieldMappings = new Dictionary<string, string>
                {
                    ["CompanyName"] = "CustomerName",
                    ["Phone"] = "CustomerPhone"
                }
            });
            
            // ========================================
            // ORDER ITEMS BLOCK: Product LOV
            // ========================================
            
            orderItemsBlock.RegisterLOV("ProductID", new BeepDataBlockLOV
            {
                LOVName = "PRODUCTS_LOV",
                Title = "Select Product",
                DataSourceName = "MainDB",
                EntityName = "Products",
                DisplayField = "ProductName",
                ReturnField = "ProductID",
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "ProductID", DisplayName = "ID", Width = 80 },
                    new LOVColumn { FieldName = "ProductCode", DisplayName = "Code", Width = 100 },
                    new LOVColumn { FieldName = "ProductName", DisplayName = "Product", Width = 200 },
                    new LOVColumn { FieldName = "UnitPrice", DisplayName = "Price", Width = 100, Format = "C2" },
                    new LOVColumn { FieldName = "StockQuantity", DisplayName = "In Stock", Width = 100 }
                },
                Filters = new List<AppFilter>
                {
                    new AppFilter { FieldName = "IsActive", Operator = "=", FilterValue = "true" },
                    new AppFilter { FieldName = "StockQuantity", Operator = ">", FilterValue = "0" }
                },
                OrderByClause = "ProductName ASC",
                AutoPopulateRelatedFields = true,
                RelatedFieldMappings = new Dictionary<string, string>
                {
                    ["ProductName"] = "ProductDescription",
                    ["UnitPrice"] = "UnitPrice"
                }
            });
            
            // ========================================
            // ORDER ITEMS BLOCK: Discount Code LOV
            // ========================================
            
            orderItemsBlock.RegisterLOV("DiscountCode", new BeepDataBlockLOV
            {
                LOVName = "DISCOUNT_CODES_LOV",
                Title = "Select Discount Code",
                DataSourceName = "MainDB",
                EntityName = "DiscountCodes",
                DisplayField = "CodeName",
                ReturnField = "DiscountCode",
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "DiscountCode", DisplayName = "Code", Width = 100 },
                    new LOVColumn { FieldName = "CodeName", DisplayName = "Name", Width = 150 },
                    new LOVColumn { FieldName = "DiscountPercent", DisplayName = "Discount %", Width = 100 }
                },
                ValidationType = LOVValidationType.Unrestricted,  // Can type custom code
                AutoPopulateRelatedFields = true,
                RelatedFieldMappings = new Dictionary<string, string>
                {
                    ["DiscountPercent"] = "DiscountPercent"
                }
            });
        }
        
        #endregion
        
        #region Example 10: LOV with Triggers
        
        /// <summary>
        /// Example: Combine LOV with triggers for complete automation
        /// </summary>
        public static void Example10_LOVWithTriggers(BeepDataBlock orderItemsBlock)
        {
            // Register Product LOV
            orderItemsBlock.RegisterLOV("ProductID", new BeepDataBlockLOV
            {
                LOVName = "PRODUCTS_LOV",
                Title = "Select Product",
                DataSourceName = "MainDB",
                EntityName = "Products",
                DisplayField = "ProductName",
                ReturnField = "ProductID",
                Columns = new List<LOVColumn>
                {
                    new LOVColumn { FieldName = "ProductID", DisplayName = "ID", Width = 80 },
                    new LOVColumn { FieldName = "ProductName", DisplayName = "Product", Width = 200 },
                    new LOVColumn { FieldName = "UnitPrice", DisplayName = "Price", Width = 100, Format = "C2" }
                },
                AutoPopulateRelatedFields = true,
                RelatedFieldMappings = new Dictionary<string, string>
                {
                    ["ProductName"] = "ProductDescription",
                    ["UnitPrice"] = "UnitPrice"
                }
            });
            
            // POST-TEXT-ITEM trigger: After product selected, calculate line total
            orderItemsBlock.RegisterTrigger(TriggerType.PostTextItem, async (context) =>
            {
                if (context.FieldName == "ProductID" && context.Block is BeepDataBlock block)
                {
                    // Product selected → Calculate line total
                    var quantity = context.GetRecordValue<int>("Quantity", 1);
                    var unitPrice = context.GetRecordValue<decimal>("UnitPrice", 0m);
                    var discount = context.GetRecordValue<decimal>("Discount", 0m);
                    
                    var lineTotal = (quantity * unitPrice) * (1 - discount);
                    block.SetItemValue("LineTotal", lineTotal);
                    
                    // Check inventory
                    var productId = context.NewValue;
                    var stockQty = await GetStockQuantity(productId);
                    
                    if (quantity > stockQty)
                    {
                        context.AddWarning($"Requested quantity ({quantity}) exceeds stock ({stockQty})");
                    }
                }
                
                return true;
            });
        }
        
        private static async Task<int> GetStockQuantity(object productId)
        {
            await Task.Delay(10);
            return 100;  // Placeholder
        }
        
        #endregion
    }
}

