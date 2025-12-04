using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Integrated.Helpers;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Examples
{
    /// <summary>
    /// Examples demonstrating Oracle Forms-compatible validation in BeepDataBlock
    /// </summary>
    public static class OracleFormsValidationExamples
    {
        #region Example 1: Basic Validation Rules
        
        /// <summary>
        /// Example: Register basic validation rules
        /// </summary>
        public static void Example1_BasicValidation(BeepDataBlock customerBlock)
        {
            // Required field
            customerBlock.RegisterValidationRule("CustomerName", new ValidationRule
            {
                RuleName = "CustomerName_Required",
                IsRequired = true,
                ErrorMessage = "Customer name is required"
            });
            
            // Email format
            customerBlock.RegisterValidationRule("Email", ValidationRuleHelpers.EmailRule("Email"));
            
            // Phone format
            customerBlock.RegisterValidationRule("Phone", ValidationRuleHelpers.PhoneRule("Phone"));
            
            // Credit limit range
            customerBlock.RegisterValidationRule("CreditLimit", new ValidationRule
            {
                RuleName = "CreditLimit_Range",
                ValidationType = ValidationType.Range,
                MinValue = 0,
                MaxValue = 100000,
                ErrorMessage = "Credit limit must be between $0 and $100,000"
            });
        }
        
        #endregion
        
        #region Example 2: Fluent Validation API
        
        /// <summary>
        /// Example: Use fluent API for validation
        /// </summary>
        public static void Example2_FluentValidation(BeepDataBlock customerBlock)
        {
            // Customer name: Required, 2-100 characters
            customerBlock.ForField("CustomerName")
                .Required("Customer name is required")
                .MinLength(2, "Customer name must be at least 2 characters")
                .MaxLength(100, "Customer name cannot exceed 100 characters")
                .Register();
            
            // Email: Required, valid format
            customerBlock.ForField("Email")
                .Required()
                .Pattern(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", "Invalid email format")
                .Register();
            
            // Discount: 0-50%
            customerBlock.ForField("DiscountPercent")
                .Range(0, 50, "Discount must be between 0% and 50%")
                .Register();
            
            // Status: Must be Active, Inactive, or Suspended
            customerBlock.ForField("Status")
                .MustBe("Active", "Inactive", "Suspended")
                .Register();
        }
        
        #endregion
        
        #region Example 3: Cross-Field Validation
        
        /// <summary>
        /// Example: Validate fields based on other field values
        /// </summary>
        public static void Example3_CrossFieldValidation(BeepDataBlock orderBlock)
        {
            // Discount amount cannot exceed total amount
            orderBlock.RegisterValidationRule("DiscountAmount", new ValidationRule
            {
                RuleName = "DiscountAmount_LessThanTotal",
                ValidationType = ValidationType.CrossField,
                DependentFields = new List<string> { "TotalAmount" },
                ValidationFunction = (value, context) =>
                {
                    if (value == null) return true;
                    
                    var discount = Convert.ToDecimal(value);
                    var total = Convert.ToDecimal(context.RecordValues.GetValueOrDefault("TotalAmount", 0m));
                    
                    return discount <= total;
                },
                ErrorMessage = "Discount amount cannot exceed total amount"
            });
            
            // End date must be after start date
            orderBlock.RegisterValidationRule("EndDate", new ValidationRule
            {
                RuleName = "EndDate_AfterStartDate",
                ValidationType = ValidationType.CrossField,
                DependentFields = new List<string> { "StartDate" },
                ValidationFunction = (value, context) =>
                {
                    if (value == null) return true;
                    
                    var endDate = Convert.ToDateTime(value);
                    var startDate = Convert.ToDateTime(context.RecordValues.GetValueOrDefault("StartDate", DateTime.MinValue));
                    
                    return endDate >= startDate;
                },
                ErrorMessage = "End date must be on or after start date"
            });
        }
        
        #endregion
        
        #region Example 4: Conditional Validation
        
        /// <summary>
        /// Example: Validate only when certain conditions are met
        /// </summary>
        public static void Example4_ConditionalValidation(BeepDataBlock customerBlock)
        {
            // Tax ID required only for corporate customers
            customerBlock.RegisterValidationRule("TaxID", ValidationRuleHelpers.ConditionalRequiredRule(
                "TaxID",
                "CustomerType = 'Corporate'",
                context =>
                {
                    var customerType = context.RecordValues.GetValueOrDefault("CustomerType")?.ToString();
                    return customerType == "Corporate";
                }
            ));
            
            // Company name required only for corporate customers
            customerBlock.RegisterValidationRule("CompanyName", ValidationRuleHelpers.ConditionalRequiredRule(
                "CompanyName",
                "CustomerType = 'Corporate'",
                context =>
                {
                    var customerType = context.RecordValues.GetValueOrDefault("CustomerType")?.ToString();
                    return customerType == "Corporate";
                }
            ));
            
            // First/Last name required only for individual customers
            customerBlock.RegisterValidationRule("FirstName", ValidationRuleHelpers.ConditionalRequiredRule(
                "FirstName",
                "CustomerType = 'Individual'",
                context =>
                {
                    var customerType = context.RecordValues.GetValueOrDefault("CustomerType")?.ToString();
                    return customerType == "Individual";
                }
            ));
        }
        
        #endregion
        
        #region Example 5: Business Rules
        
        /// <summary>
        /// Example: Complex business rules
        /// </summary>
        public static void Example5_BusinessRules(BeepDataBlock orderBlock)
        {
            // Order total must match sum of line items
            orderBlock.RegisterRecordValidationRule(new ValidationRule
            {
                RuleName = "OrderTotal_MatchesLineItems",
                ValidationType = ValidationType.BusinessRule,
                ValidationFunction = (value, context) =>
                {
                    var orderTotal = Convert.ToDecimal(context.RecordValues.GetValueOrDefault("OrderTotal", 0m));
                    var lineItemsTotal = Convert.ToDecimal(context.RecordValues.GetValueOrDefault("LineItemsTotal", 0m));
                    
                    // Allow 0.01 difference for rounding
                    return Math.Abs(orderTotal - lineItemsTotal) <= 0.01m;
                },
                ErrorMessage = "Order total must match sum of line items"
            });
            
            // Cannot order more than available stock
            orderBlock.RegisterValidationRule("Quantity", new ValidationRule
            {
                RuleName = "Quantity_InStock",
                ValidationType = ValidationType.BusinessRule,
                ValidationFunction = (value, context) =>
                {
                    if (value == null) return true;
                    
                    var quantity = Convert.ToInt32(value);
                    var stockQty = Convert.ToInt32(context.RecordValues.GetValueOrDefault("StockQuantity", 0));
                    
                    return quantity <= stockQty;
                },
                ErrorMessage = "Quantity cannot exceed available stock"
            });
        }
        
        #endregion
        
        #region Example 6: Validation with Triggers
        
        /// <summary>
        /// Example: Integrate validation with triggers
        /// </summary>
        public static void Example6_ValidationWithTriggers(BeepDataBlock customerBlock)
        {
            // Register validation rules
            customerBlock.ForField("Email")
                .Required()
                .Pattern(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", "Invalid email")
                .Register();
            
            customerBlock.ForField("CreditLimit")
                .Range(0, 100000)
                .Register();
            
            // WHEN-VALIDATE-ITEM trigger: Validate on field change
            customerBlock.RegisterTrigger(TriggerType.WhenValidateItem, async context =>
            {
                if (!string.IsNullOrEmpty(context.FieldName))
                {
                    var errors = await customerBlock.ValidateField(context.FieldName, context.NewValue);
                    
                    if (errors.Flag != Errors.Ok)
                    {
                        context.Cancel = true;
                        context.ErrorMessage = errors.Message;
                        return false;
                    }
                }
                
                return true;
            });
            
            // WHEN-VALIDATE-RECORD trigger: Validate entire record before save
            customerBlock.RegisterTrigger(TriggerType.WhenValidateRecord, async context =>
            {
                var errors = await customerBlock.ValidateCurrentRecord();
                
                if (errors.Flag != Errors.Ok)
                {
                    context.Cancel = true;
                    context.ErrorMessage = errors.Message;
                    
                    System.Windows.Forms.MessageBox.Show(errors.Message, "Validation Error",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    
                    return false;
                }
                
                return true;
            });
        }
        
        #endregion
        
        #region Example 7: Pre-Built Validation Rules
        
        /// <summary>
        /// Example: Use pre-built validation rules
        /// </summary>
        public static void Example7_PreBuiltRules(BeepDataBlock customerBlock)
        {
            // Email
            customerBlock.RegisterValidationRule("Email", ValidationRuleHelpers.EmailRule("Email"));
            
            // Phone
            customerBlock.RegisterValidationRule("Phone", ValidationRuleHelpers.PhoneRule("Phone"));
            
            // Website URL
            customerBlock.RegisterValidationRule("Website", ValidationRuleHelpers.URLRule("Website"));
            
            // ZIP code
            customerBlock.RegisterValidationRule("ZipCode", ValidationRuleHelpers.USZipCodeRule("ZipCode"));
            
            // Credit card
            customerBlock.RegisterValidationRule("CreditCard", ValidationRuleHelpers.CreditCardRule("CreditCard"));
            
            // SSN
            customerBlock.RegisterValidationRule("SSN", ValidationRuleHelpers.SSNRule("SSN"));
            
            // Discount percentage (0-100)
            customerBlock.RegisterValidationRule("DiscountPercent", 
                ValidationRuleHelpers.PercentageRule("DiscountPercent"));
            
            // Birth date (must be in past)
            customerBlock.RegisterValidationRule("BirthDate", 
                ValidationRuleHelpers.PastDateRule("BirthDate"));
        }
        
        #endregion
        
        #region Example 8: Validation with LOVs
        
        /// <summary>
        /// Example: Combine validation with LOVs
        /// </summary>
        public static void Example8_ValidationWithLOVs(BeepDataBlock orderBlock)
        {
            // Register LOV for CustomerID
            orderBlock.RegisterLOV("CustomerID", new BeepDataBlockLOV
            {
                LOVName = "CUSTOMERS_LOV",
                Title = "Select Customer",
                DataSourceName = "MainDB",
                EntityName = "Customers",
                DisplayField = "CompanyName",
                ReturnField = "CustomerID",
                ValidationType = LOVValidationType.ListOnly  // Must select from LOV
            });
            
            // Add validation rule to enforce LOV selection
            orderBlock.RegisterValidationRule("CustomerID", new ValidationRule
            {
                RuleName = "CustomerID_ValidLOV",
                ValidationType = ValidationType.Lookup,
                ValidationFunction = (value, context) =>
                {
                    if (value == null) return false;
                    
                    // Validate against LOV (synchronous check)
                    // In real scenario, you'd check cached LOV data
                    return value != null;
                },
                ErrorMessage = "Please select a valid customer from the list (F9)"
            });
        }
        
        #endregion
        
        #region Example 9: Complete Validation Setup
        
        /// <summary>
        /// Example: Complete validation setup for a form
        /// </summary>
        public static void Example9_CompleteValidationSetup(BeepDataBlock customerBlock)
        {
            // ========================================
            // REQUIRED FIELDS
            // ========================================
            
            customerBlock.ForField("CustomerName")
                .Required()
                .MinLength(2)
                .MaxLength(100)
                .Register();
            
            customerBlock.ForField("Email")
                .Required()
                .Pattern(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", "Invalid email")
                .Register();
            
            customerBlock.ForField("Phone")
                .Required()
                .Pattern(@"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$", "Invalid phone")
                .Register();
            
            // ========================================
            // RANGE VALIDATIONS
            // ========================================
            
            customerBlock.ForField("CreditLimit")
                .Range(0, 100000, "Credit limit must be between $0 and $100,000")
                .Register();
            
            customerBlock.ForField("DiscountPercent")
                .Range(0, 50, "Discount must be between 0% and 50%")
                .Register();
            
            // ========================================
            // CONDITIONAL VALIDATIONS
            // ========================================
            
            // Tax ID required for corporate customers
            customerBlock.RegisterValidationRule("TaxID", 
                ValidationRuleHelpers.ConditionalRequiredRule(
                    "TaxID",
                    "CustomerType = 'Corporate'",
                    context => context.RecordValues.GetValueOrDefault("CustomerType")?.ToString() == "Corporate"
                ));
            
            // ========================================
            // BUSINESS RULES
            // ========================================
            
            // Email must be unique
            customerBlock.RegisterValidationRule("Email", 
                ValidationRuleHelpers.UniqueValueRule("Email", 
                    email => CheckEmailUnique(email?.ToString())));
            
            // ========================================
            // TRIGGER INTEGRATION
            // ========================================
            
            // Validate on item change
            customerBlock.RegisterTrigger(TriggerType.WhenValidateItem, async context =>
            {
                if (!string.IsNullOrEmpty(context.FieldName))
                {
                    var errors = await customerBlock.ValidateField(context.FieldName, context.NewValue);
                    
                    if (errors.Flag != Errors.Ok)
                    {
                        context.Cancel = true;
                        context.ErrorMessage = errors.Message;
                        return false;
                    }
                }
                
                return true;
            });
            
            // Validate before commit
            customerBlock.RegisterTrigger(TriggerType.PreFormCommit, async context =>
            {
                var errors = await customerBlock.ValidateCurrentRecord();
                
                if (errors.Flag != Errors.Ok)
                {
                    context.Cancel = true;
                    context.ErrorMessage = errors.Message;
                    
                    System.Windows.Forms.MessageBox.Show(errors.Message, "Validation Error",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    
                    return false;
                }
                
                return true;
            });
        }
        
        private static bool CheckEmailUnique(string email)
        {
            // Placeholder - would check database
            return true;
        }
        
        #endregion
        
        #region Example 10: Advanced Business Rules
        
        /// <summary>
        /// Example: Complex business rules with multiple conditions
        /// </summary>
        public static void Example10_AdvancedBusinessRules(BeepDataBlock orderBlock)
        {
            // Order total validation
            orderBlock.RegisterRecordValidationRule(new ValidationRule
            {
                RuleName = "OrderTotal_Validation",
                ValidationType = ValidationType.BusinessRule,
                ValidationFunction = (value, context) =>
                {
                    var subtotal = Convert.ToDecimal(context.RecordValues.GetValueOrDefault("Subtotal", 0m));
                    var tax = Convert.ToDecimal(context.RecordValues.GetValueOrDefault("Tax", 0m));
                    var shipping = Convert.ToDecimal(context.RecordValues.GetValueOrDefault("Shipping", 0m));
                    var discount = Convert.ToDecimal(context.RecordValues.GetValueOrDefault("Discount", 0m));
                    var total = Convert.ToDecimal(context.RecordValues.GetValueOrDefault("Total", 0m));
                    
                    var expectedTotal = subtotal + tax + shipping - discount;
                    
                    // Allow 0.01 difference for rounding
                    return Math.Abs(total - expectedTotal) <= 0.01m;
                },
                ErrorMessage = "Order total calculation is incorrect"
            });
            
            // Quantity validation with stock check
            orderBlock.RegisterValidationRule("Quantity", new ValidationRule
            {
                RuleName = "Quantity_StockCheck",
                ValidationType = ValidationType.BusinessRule,
                ValidationFunction = (value, context) =>
                {
                    if (value == null) return true;
                    
                    var quantity = Convert.ToInt32(value);
                    var productId = context.RecordValues.GetValueOrDefault("ProductID");
                    
                    if (productId == null) return true;
                    
                    // Check stock (placeholder - would query database)
                    var stockQty = GetStockQuantity(productId);
                    
                    return quantity <= stockQty;
                },
                ErrorMessage = "Quantity exceeds available stock"
            });
            
            // Discount validation based on customer type
            orderBlock.RegisterValidationRule("DiscountPercent", new ValidationRule
            {
                RuleName = "Discount_CustomerTypeLimit",
                ValidationType = ValidationType.BusinessRule,
                ValidationFunction = (value, context) =>
                {
                    if (value == null) return true;
                    
                    var discount = Convert.ToDecimal(value);
                    var customerId = context.RecordValues.GetValueOrDefault("CustomerID");
                    
                    if (customerId == null) return true;
                    
                    // Get customer type (placeholder - would query database)
                    var customerType = GetCustomerType(customerId);
                    
                    // Retail: max 10%, Wholesale: max 30%, Corporate: max 50%
                    var maxDiscount = customerType switch
                    {
                        "Retail" => 10m,
                        "Wholesale" => 30m,
                        "Corporate" => 50m,
                        _ => 0m
                    };
                    
                    return discount <= maxDiscount;
                },
                ErrorMessage = "Discount exceeds maximum allowed for customer type"
            });
        }
        
        private static int GetStockQuantity(object productId)
        {
            // Placeholder
            return 100;
        }
        
        private static string GetCustomerType(object customerId)
        {
            // Placeholder
            return "Retail";
        }
        
        #endregion
    }
}

