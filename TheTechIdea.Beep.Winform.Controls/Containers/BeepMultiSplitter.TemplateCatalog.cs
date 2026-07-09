using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>Groups the <see cref="BusinessTemplate"/> values into browsable categories.</summary>
    public enum BusinessTemplateCategory
    {
        Billing,
        Commerce,
        CRM,
        Support,
        HR,
        Analytics,
        Project,
        Admin,
        Content,
        Scheduling,
        Logistics,
        InsuranceBanking,
        Legal,
        Hospitality,
        Manufacturing,
        Healthcare,
        Automotive,
        RealEstate,
        Education,
        Nonprofit
    }

    public partial class BeepMultiSplitter
    {
        /// <summary>Returns the business templates that belong to the given category.</summary>
        public static BusinessTemplate[] GetTemplates(BusinessTemplateCategory category) => category switch
        {
            BusinessTemplateCategory.Billing => new[]
            {
                BusinessTemplate.Invoice, BusinessTemplate.TaxInvoice, BusinessTemplate.ProformaInvoice,
                BusinessTemplate.Quote, BusinessTemplate.Estimate, BusinessTemplate.PurchaseOrder,
                BusinessTemplate.SalesReceipt, BusinessTemplate.CreditNote, BusinessTemplate.DebitNote,
                BusinessTemplate.DeliveryNote, BusinessTemplate.PackingSlip, BusinessTemplate.AccountStatement,
                BusinessTemplate.BankStatement, BusinessTemplate.ExpenseReport, BusinessTemplate.Payslip,
                BusinessTemplate.Budget, BusinessTemplate.FinancialReport
            },
            BusinessTemplateCategory.Commerce => new[]
            {
                BusinessTemplate.ProductDetail, BusinessTemplate.ProductCatalog, BusinessTemplate.ProductComparison,
                BusinessTemplate.Storefront, BusinessTemplate.Wishlist, BusinessTemplate.ShoppingCart,
                BusinessTemplate.Checkout, BusinessTemplate.OrderConfirmation, BusinessTemplate.PointOfSale,
                BusinessTemplate.SalesOrderForm, BusinessTemplate.InventoryList
            },
            BusinessTemplateCategory.CRM => new[]
            {
                BusinessTemplate.CrmContact, BusinessTemplate.CustomerProfile, BusinessTemplate.CompanyProfile,
                BusinessTemplate.LeadDetail, BusinessTemplate.DealPipeline, BusinessTemplate.ContactList
            },
            BusinessTemplateCategory.Support => new[]
            {
                BusinessTemplate.SupportTicket, BusinessTemplate.TicketQueue, BusinessTemplate.KnowledgeBaseArticle,
                BusinessTemplate.WorkOrder, BusinessTemplate.ServiceRequest
            },
            BusinessTemplateCategory.HR => new[]
            {
                BusinessTemplate.EmployeeProfile, BusinessTemplate.ApplicantProfile, BusinessTemplate.JobPosting,
                BusinessTemplate.Timesheet, BusinessTemplate.LeaveRequest, BusinessTemplate.Onboarding,
                BusinessTemplate.OrgChart
            },
            BusinessTemplateCategory.Analytics => new[]
            {
                BusinessTemplate.AnalyticsReport, BusinessTemplate.SalesDashboard, BusinessTemplate.FinanceDashboard,
                BusinessTemplate.KpiOverview, BusinessTemplate.ReportViewer
            },
            BusinessTemplateCategory.Project => new[]
            {
                BusinessTemplate.KanbanBoard, BusinessTemplate.TaskDetail, BusinessTemplate.ProjectOverview,
                BusinessTemplate.GanttView
            },
            BusinessTemplateCategory.Admin => new[]
            {
                BusinessTemplate.SettingsPage, BusinessTemplate.UserManagement, BusinessTemplate.RolesPermissions,
                BusinessTemplate.AuditLog, BusinessTemplate.IntegrationsPage
            },
            BusinessTemplateCategory.Content => new[]
            {
                BusinessTemplate.DocumentEditor, BusinessTemplate.ArticlePage, BusinessTemplate.EmailComposer,
                BusinessTemplate.FormBuilder
            },
            BusinessTemplateCategory.Scheduling => new[]
            {
                BusinessTemplate.BookingAppointment, BusinessTemplate.CalendarSchedule
            },
            BusinessTemplateCategory.Logistics => new[]
            {
                BusinessTemplate.ShippingLabel, BusinessTemplate.DeliveryTracking, BusinessTemplate.FreightManifest,
                BusinessTemplate.BillOfLading, BusinessTemplate.CustomsDeclaration, BusinessTemplate.WarehousePicking,
                BusinessTemplate.DispatchBoard
            },
            BusinessTemplateCategory.InsuranceBanking => new[]
            {
                BusinessTemplate.InsuranceClaim, BusinessTemplate.PolicyDetail, BusinessTemplate.LoanApplication,
                BusinessTemplate.PortfolioOverview, BusinessTemplate.TransactionDetail
            },
            BusinessTemplateCategory.Legal => new[]
            {
                BusinessTemplate.LegalContract, BusinessTemplate.CaseFile, BusinessTemplate.MatterOverview
            },
            BusinessTemplateCategory.Hospitality => new[]
            {
                BusinessTemplate.RestaurantMenu, BusinessTemplate.TableReservation, BusinessTemplate.HotelFolio,
                BusinessTemplate.RoomBooking, BusinessTemplate.GuestProfile, BusinessTemplate.HousekeepingBoard
            },
            BusinessTemplateCategory.Manufacturing => new[]
            {
                BusinessTemplate.BillOfMaterials, BusinessTemplate.ProductionOrder, BusinessTemplate.QualityInspection
            },
            BusinessTemplateCategory.Healthcare => new[]
            {
                BusinessTemplate.PatientChart, BusinessTemplate.PrescriptionForm, BusinessTemplate.LabReport
            },
            BusinessTemplateCategory.Automotive => new[]
            {
                BusinessTemplate.RepairEstimate, BusinessTemplate.VehicleInspection
            },
            BusinessTemplateCategory.RealEstate => new[]
            {
                BusinessTemplate.PropertyListing, BusinessTemplate.PropertyDetail
            },
            BusinessTemplateCategory.Education => new[]
            {
                BusinessTemplate.CoursePage, BusinessTemplate.Gradebook
            },
            BusinessTemplateCategory.Nonprofit => new[]
            {
                BusinessTemplate.DonationReceipt, BusinessTemplate.EventProgram, BusinessTemplate.PermitApplication
            },
            _ => new[] { BusinessTemplate.None }
        };

        /// <summary>Returns the category a template belongs to (defaults to Billing).</summary>
        public static BusinessTemplateCategory GetTemplateCategory(BusinessTemplate template)
        {
            foreach (BusinessTemplateCategory cat in System.Enum.GetValues(typeof(BusinessTemplateCategory)))
            {
                foreach (var t in GetTemplates(cat))
                    if (t == template) return cat;
            }
            return BusinessTemplateCategory.Billing;
        }
    }
}
