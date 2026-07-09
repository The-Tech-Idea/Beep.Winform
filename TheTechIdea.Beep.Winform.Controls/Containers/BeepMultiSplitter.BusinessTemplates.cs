using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Ready-made business-document / screen templates for <see cref="BeepMultiSplitter"/>.
    /// Each template lays out the named zones of a common business surface as a resizable grid.
    /// Use <c>ApplyBusinessTemplate(t, true)</c> to scaffold the zones as labelled tiles.
    /// </summary>
    public enum BusinessTemplate
    {
        None,

        // ── Billing / Finance ─────────────────────────────────────────────
        Invoice, TaxInvoice, ProformaInvoice, Quote, Estimate, PurchaseOrder,
        SalesReceipt, CreditNote, DebitNote, DeliveryNote, PackingSlip,
        AccountStatement, BankStatement, ExpenseReport, Payslip, Budget, FinancialReport,

        // ── Commerce / Retail ─────────────────────────────────────────────
        ProductDetail, ProductCatalog, ProductComparison, Storefront, Wishlist,
        ShoppingCart, Checkout, OrderConfirmation, PointOfSale, SalesOrderForm, InventoryList,

        // ── CRM / Sales ───────────────────────────────────────────────────
        CrmContact, CustomerProfile, CompanyProfile, LeadDetail, DealPipeline, ContactList,

        // ── Support / Operations ──────────────────────────────────────────
        SupportTicket, TicketQueue, KnowledgeBaseArticle, WorkOrder, ServiceRequest,

        // ── HR / People ───────────────────────────────────────────────────
        EmployeeProfile, ApplicantProfile, JobPosting, Timesheet, LeaveRequest, Onboarding, OrgChart,

        // ── Analytics / Dashboards ────────────────────────────────────────
        AnalyticsReport, SalesDashboard, FinanceDashboard, KpiOverview, ReportViewer,

        // ── Project / Task ────────────────────────────────────────────────
        KanbanBoard, TaskDetail, ProjectOverview, GanttView,

        // ── Admin / Settings ──────────────────────────────────────────────
        SettingsPage, UserManagement, RolesPermissions, AuditLog, IntegrationsPage,

        // ── Content / Docs ────────────────────────────────────────────────
        DocumentEditor, ArticlePage, EmailComposer, FormBuilder,

        // ── Logistics / Scheduling ────────────────────────────────────────
        ShippingLabel, DeliveryTracking, BookingAppointment, CalendarSchedule,

        // ── Industry ──────────────────────────────────────────────────────
        RestaurantMenu, TableReservation, PatientChart, PropertyListing, PropertyDetail, CoursePage, Gradebook,

        // ── Logistics / Trade ─────────────────────────────────────────────
        FreightManifest, BillOfLading, CustomsDeclaration, WarehousePicking, DispatchBoard,

        // ── Insurance / Banking ───────────────────────────────────────────
        InsuranceClaim, PolicyDetail, LoanApplication, PortfolioOverview, TransactionDetail,

        // ── Legal ─────────────────────────────────────────────────────────
        LegalContract, CaseFile, MatterOverview,

        // ── Hospitality ───────────────────────────────────────────────────
        HotelFolio, RoomBooking, GuestProfile, HousekeepingBoard,

        // ── Manufacturing ─────────────────────────────────────────────────
        BillOfMaterials, ProductionOrder, QualityInspection,

        // ── Healthcare ────────────────────────────────────────────────────
        PrescriptionForm, LabReport,

        // ── Automotive / Field service ────────────────────────────────────
        RepairEstimate, VehicleInspection,

        // ── Nonprofit / Events / Government ───────────────────────────────
        DonationReceipt, EventProgram, PermitApplication
    }

    public partial class BeepMultiSplitter
    {
        private BusinessTemplate _businessTemplate = BusinessTemplate.None;

        /// <summary>
        /// Gets or sets a ready-made business template. Setting a value other than
        /// <see cref="BusinessTemplate.None"/> reconfigures the grid to that template's zones.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("Apply a ready-made business template (invoice, product, POS, CRM, report, ...).")]
        [DefaultValue(BusinessTemplate.None)]
        public BusinessTemplate BusinessTemplate
        {
            get => _businessTemplate;
            set
            {
                _businessTemplate = value;
                if (value != BusinessTemplate.None)
                    ApplyBusinessTemplate(value, addPlaceholders: false);
            }
        }

        /// <summary>
        /// Applies a business template's grid. When <paramref name="addPlaceholders"/> is true,
        /// each named zone is scaffolded as a labelled BeepLabel tile (with correct spans).
        /// </summary>
        public void ApplyBusinessTemplate(BusinessTemplate template, bool addPlaceholders = false)
        {
            if (template == BusinessTemplate.None) return;
            var tlp = _tableLayoutPanel;
            if (tlp == null) return;

            var spec = GetBusinessTemplate(template);
            if (spec.cols == null) return;

            tlp.SuspendLayout();
            try
            {
                if (addPlaceholders)
                {
                    for (int i = tlp.Controls.Count - 1; i >= 0; i--)
                    {
                        var c = tlp.Controls[i];
                        tlp.Controls.Remove(c);
                        c.Dispose();
                    }
                }

                tlp.ColumnStyles.Clear();
                tlp.RowStyles.Clear();
                tlp.ColumnCount = spec.cols.Length;
                tlp.RowCount = spec.rows.Length;
                foreach (var c in spec.cols) tlp.ColumnStyles.Add(new ColumnStyle(c.t, c.v));
                foreach (var r in spec.rows) tlp.RowStyles.Add(new RowStyle(r.t, r.v));

                if (addPlaceholders) PlaceRegions(spec.regions);
            }
            finally
            {
                tlp.ResumeLayout(true);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  Template registry (mostly one line each via the archetype builders)
        // ════════════════════════════════════════════════════════════════════
        private ((SizeType t, float v)[] cols, (SizeType t, float v)[] rows, Region[] regions) GetBusinessTemplate(BusinessTemplate template)
        {
            switch (template)
            {
                // Billing / Finance ------------------------------------------------
                case BusinessTemplate.Invoice:         return Doc("Invoice Header", "Bill To", "Ship To", "Line Items", "Notes", "Totals", "Footer / Terms");
                case BusinessTemplate.TaxInvoice:      return Doc("Tax Invoice", "Seller (Tax ID)", "Buyer (Tax ID)", "Line Items", "Tax Summary", "Totals", "Footer");
                case BusinessTemplate.ProformaInvoice: return Doc("Proforma Invoice", "Seller", "Buyer", "Items", "Notes", "Totals", "Terms");
                case BusinessTemplate.Quote:           return Doc("Quote Header", "Prepared For", "Valid Until", "Items", "Notes", "Totals", "Terms");
                case BusinessTemplate.Estimate:        return Doc("Estimate Header", "Client", "Project", "Line Items", "Notes", "Estimated Total", "Signature");
                case BusinessTemplate.PurchaseOrder:   return Doc("PO Header", "Supplier", "Ship To", "Line Items", "Notes", "Totals", "Approval");
                case BusinessTemplate.CreditNote:      return Doc("Credit Note", "Customer", "Original Invoice", "Items", "Reason", "Credit Total", "Footer");
                case BusinessTemplate.DebitNote:       return Doc("Debit Note", "Supplier", "Reference", "Items", "Reason", "Debit Total", "Footer");
                case BusinessTemplate.DeliveryNote:    return Doc("Delivery Note", "Ship From", "Ship To", "Items Delivered", "Notes", "Received By", "Footer");
                case BusinessTemplate.PackingSlip:     return Doc("Packing Slip", "Ship From", "Ship To", "Packed Items", "Notes", "Carrier", "Barcode");
                case BusinessTemplate.Payslip:         return Doc("Payslip Header", "Employee", "Pay Period", "Earnings & Deductions", "Notes", "Net Pay", "Footer");
                case BusinessTemplate.SalesReceipt:    return Stack("Store Header", "Items", "Totals", "Thank You");
                case BusinessTemplate.AccountStatement:return Stack("Statement Header", "Account Summary", "Transactions", "Footer");
                case BusinessTemplate.BankStatement:   return Stack("Bank Statement Header", "Account Summary", "Transactions", "Footer");
                case BusinessTemplate.ExpenseReport:   return Stack("Report Header", "Employee & Period", "Expense Lines", "Totals");
                case BusinessTemplate.Budget:          return Dash("Budget KPIs", "Planned vs Actual", "By Category", "Budget Lines");
                case BusinessTemplate.FinancialReport: return Dash("Financial KPIs", "Revenue", "Expenses", "Ledger");

                // Commerce / Retail ------------------------------------------------
                case BusinessTemplate.ProductDetail:     return Gallery("Gallery", "Product Info", "Specifications");
                case BusinessTemplate.ProductCatalog:    return Catalog("Toolbar", "Filters", "Product Grid");
                case BusinessTemplate.ProductComparison: return Columns("Compare Products", "Product A", "Product B", "Product C");
                case BusinessTemplate.Storefront:        return Shell2("Header", "Categories", "Product Grid");
                case BusinessTemplate.Wishlist:          return StackTop("Wishlist Header", "Saved Items");
                case BusinessTemplate.ShoppingCart:      return Split("Cart Items", "Order Summary", 65);
                case BusinessTemplate.Checkout:          return Split("Shipping & Payment", "Order Summary", 60);
                case BusinessTemplate.OrderConfirmation: return Stack("Confirmation", "Order Details", "Summary", "Next Steps");
                case BusinessTemplate.PointOfSale:       return Pos("Product Grid", "Cart Items", "Payment");
                case BusinessTemplate.SalesOrderForm:    return Stack3("Customer & Order", "Order Lines", "Summary");
                case BusinessTemplate.InventoryList:     return Catalog("Toolbar", "Filters", "Inventory Table");

                // CRM / Sales ------------------------------------------------------
                case BusinessTemplate.CrmContact:     return Shell3("Contact Header", "Profile", "Details", "Activity");
                case BusinessTemplate.CustomerProfile:return Shell3("Customer Header", "Summary", "Orders & Details", "Activity");
                case BusinessTemplate.CompanyProfile: return Shell3("Company Header", "Overview", "People & Details", "Notes");
                case BusinessTemplate.LeadDetail:     return Shell3("Lead Header", "Summary", "Details", "Timeline");
                case BusinessTemplate.DealPipeline:   return Columns("Pipeline", "New", "Qualified", "Proposal", "Won");
                case BusinessTemplate.ContactList:    return MD("Contacts", "Contact Detail", 320);

                // Support / Operations --------------------------------------------
                case BusinessTemplate.SupportTicket:        return ContentAside("Ticket Header", "Conversation", "Properties");
                case BusinessTemplate.TicketQueue:          return MD("Ticket Queue", "Ticket Detail", 360);
                case BusinessTemplate.KnowledgeBaseArticle: return Shell3("", "Table of Contents", "Article", "Related");
                case BusinessTemplate.WorkOrder:            return Doc("Work Order", "Customer", "Asset", "Tasks", "Notes", "Costs", "Sign-off");
                case BusinessTemplate.ServiceRequest:       return Stack3("Request Header", "Description", "Status & Actions");

                // HR / People ------------------------------------------------------
                case BusinessTemplate.EmployeeProfile: return Shell2("Header", "Summary", "Details");
                case BusinessTemplate.ApplicantProfile:return Shell3("Applicant Header", "Profile", "Resume", "Evaluation");
                case BusinessTemplate.JobPosting:      return Split("Job Description", "Apply / Summary", 65);
                case BusinessTemplate.Timesheet:       return Stack3("Employee / Period", "Time Entries", "Totals");
                case BusinessTemplate.LeaveRequest:    return Stack("Request Header", "Leave Details", "Balance", "Approval");
                case BusinessTemplate.Onboarding:      return Shell2("Header", "Checklist", "Content");
                case BusinessTemplate.OrgChart:        return StackTop("Toolbar", "Org Chart Canvas");

                // Analytics / Dashboards ------------------------------------------
                case BusinessTemplate.AnalyticsReport: return Dash("KPIs", "Chart", "Chart", "Data Table");
                case BusinessTemplate.SalesDashboard:  return Dash("Sales KPIs", "Revenue Trend", "Top Products", "Recent Orders");
                case BusinessTemplate.FinanceDashboard:return Dash("Finance KPIs", "Cash Flow", "Expenses", "Ledger");
                case BusinessTemplate.KpiOverview:     return Kpi("KPI 1", "KPI 2", "KPI 3", "KPI 4", "Content");
                case BusinessTemplate.ReportViewer:    return Shell2("Report Toolbar", "Parameters", "Report Canvas");

                // Project / Task ---------------------------------------------------
                case BusinessTemplate.KanbanBoard:    return Columns("Board Toolbar", "To Do", "In Progress", "Review", "Done");
                case BusinessTemplate.TaskDetail:     return ContentAside("Task Header", "Description", "Properties");
                case BusinessTemplate.ProjectOverview:return Dash("Project KPIs", "Timeline", "Team", "Tasks");
                case BusinessTemplate.GanttView:      return Split("Task List", "Timeline", 30);

                // Admin / Settings -------------------------------------------------
                case BusinessTemplate.SettingsPage:     return Shell2("Header", "Settings Nav", "Settings Panel");
                case BusinessTemplate.UserManagement:   return Catalog("Toolbar", "Filters", "Users Table");
                case BusinessTemplate.RolesPermissions: return Split("Roles", "Permissions", 35);
                case BusinessTemplate.AuditLog:         return StackTop("Toolbar", "Audit Log Table");
                case BusinessTemplate.IntegrationsPage: return Catalog("Toolbar", "Categories", "Integrations Grid");

                // Content / Docs ---------------------------------------------------
                case BusinessTemplate.DocumentEditor: return Shell3("Toolbar", "Outline", "Editor", "Properties");
                case BusinessTemplate.ArticlePage:    return Shell3("", "Table of Contents", "Article", "Related");
                case BusinessTemplate.EmailComposer:  return Stack3("Recipients & Subject", "Message Body", "Toolbar");
                case BusinessTemplate.FormBuilder:    return Shell3("Toolbar", "Fields Palette", "Canvas", "Properties");

                // Logistics / Scheduling ------------------------------------------
                case BusinessTemplate.ShippingLabel:     return Stack("From", "To", "Package & Barcode", "Carrier");
                case BusinessTemplate.DeliveryTracking:  return Split("Map", "Delivery Details", 60);
                case BusinessTemplate.BookingAppointment:return Split("Calendar", "Booking Details", 65);
                case BusinessTemplate.CalendarSchedule:  return Shell2("Toolbar", "Mini Calendar", "Schedule");

                // Industry ---------------------------------------------------------
                case BusinessTemplate.RestaurantMenu:  return Catalog("Toolbar", "Categories", "Menu Items");
                case BusinessTemplate.TableReservation:return Split("Floor Plan", "Reservation", 65);
                case BusinessTemplate.PatientChart:    return Shell3("Patient Header", "Summary", "Chart", "Vitals");
                case BusinessTemplate.PropertyListing: return Catalog("Toolbar", "Filters", "Listings");
                case BusinessTemplate.PropertyDetail:  return Gallery("Gallery", "Property Info", "Details & Map");
                case BusinessTemplate.CoursePage:      return Shell2("Course Header", "Modules", "Lesson Content");
                case BusinessTemplate.Gradebook:       return Catalog("Toolbar", "Roster", "Grade Grid");

                // Logistics / Trade -----------------------------------------------
                case BusinessTemplate.FreightManifest:    return Doc("Manifest Header", "Carrier", "Consignee", "Shipment Lines", "Notes", "Totals", "Signatures");
                case BusinessTemplate.BillOfLading:       return Doc("Bill of Lading", "Shipper", "Consignee", "Freight Description", "Special Instructions", "Charges", "Signatures");
                case BusinessTemplate.CustomsDeclaration: return Doc("Customs Declaration", "Exporter", "Importer", "Goods", "Declaration", "Duties & Taxes", "Signature");
                case BusinessTemplate.WarehousePicking:   return Catalog("Toolbar", "Zones", "Pick List");
                case BusinessTemplate.DispatchBoard:      return Columns("Dispatch", "Pending", "In Transit", "Delivered");

                // Insurance / Banking ---------------------------------------------
                case BusinessTemplate.InsuranceClaim:   return Doc("Claim Header", "Policy Holder", "Incident", "Claim Items", "Notes", "Assessed Amount", "Adjuster Sign-off");
                case BusinessTemplate.PolicyDetail:     return Shell3("Policy Header", "Summary", "Coverage & Details", "Documents");
                case BusinessTemplate.LoanApplication:  return Stack("Application Header", "Applicant & Loan Details", "Documents", "Decision");
                case BusinessTemplate.PortfolioOverview:return Dash("Portfolio KPIs", "Allocation", "Performance", "Holdings");
                case BusinessTemplate.TransactionDetail:return ContentAside("Transaction Header", "Details", "Related");

                // Legal ------------------------------------------------------------
                case BusinessTemplate.LegalContract:  return Stack("Contract Header", "Clauses / Body", "Signatures", "Footer");
                case BusinessTemplate.CaseFile:       return Shell3("Case Header", "Parties", "Documents & Notes", "Timeline");
                case BusinessTemplate.MatterOverview: return Dash("Matter KPIs", "Timeline", "Tasks", "Documents");

                // Hospitality ------------------------------------------------------
                case BusinessTemplate.HotelFolio:       return Doc("Folio Header", "Guest", "Stay", "Charges", "Notes", "Balance Due", "Footer");
                case BusinessTemplate.RoomBooking:      return Split("Room Availability", "Booking Details", 62);
                case BusinessTemplate.GuestProfile:     return Shell3("Guest Header", "Profile", "Stay History", "Preferences");
                case BusinessTemplate.HousekeepingBoard:return Columns("Housekeeping", "Dirty", "Cleaning", "Inspected", "Ready");

                // Manufacturing ----------------------------------------------------
                case BusinessTemplate.BillOfMaterials:   return Stack3("BOM Header", "Components", "Summary");
                case BusinessTemplate.ProductionOrder:   return Doc("Production Order", "Product", "Work Center", "Operations", "Notes", "Quantities", "Sign-off");
                case BusinessTemplate.QualityInspection: return Stack("Inspection Header", "Checklist", "Results", "Sign-off");

                // Healthcare -------------------------------------------------------
                case BusinessTemplate.PrescriptionForm: return Doc("Prescription", "Patient", "Prescriber", "Medications", "Instructions", "Refills", "Signature");
                case BusinessTemplate.LabReport:        return Stack("Lab Report Header", "Patient & Order", "Results", "Comments");

                // Automotive / Field service --------------------------------------
                case BusinessTemplate.RepairEstimate:    return Doc("Repair Estimate", "Customer", "Vehicle", "Parts & Labor", "Notes", "Total", "Authorization");
                case BusinessTemplate.VehicleInspection: return Stack("Inspection Header", "Checklist", "Findings", "Sign-off");

                // Nonprofit / Events / Government ---------------------------------
                case BusinessTemplate.DonationReceipt:  return Stack("Receipt Header", "Donor", "Donation Details", "Acknowledgement");
                case BusinessTemplate.EventProgram:     return Stack("Event Header", "Schedule", "Speakers", "Footer");
                case BusinessTemplate.PermitApplication:return Stack("Application Header", "Applicant & Details", "Attachments", "Decision");

                default: return (null, null, null);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  Archetype builders (DPI-scaled fixed dimensions)
        // ════════════════════════════════════════════════════════════════════
        private int Sc(int px) => DpiScalingHelper.ScaleValue(px, this);
        private static Region Rg(string n, int c, int r, int cs = 1, int rs = 1) => new Region(n, c, r, cs, rs);

        /// <summary>Document layout: 2 columns, [header · two blocks · table · notes/totals · footer].</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) Doc(string title, string left, string right, string table, string notes, string totals, string footer)
            => (C(Pct(50), Pct(50)), C(Abs(Sc(90)), Abs(Sc(110)), Pct(100), Abs(Sc(120)), Abs(Sc(64))),
                R(Rg(title, 0, 0, 2), Rg(left, 0, 1), Rg(right, 1, 1), Rg(table, 0, 2, 2), Rg(notes, 0, 3), Rg(totals, 1, 3), Rg(footer, 0, 4, 2)));

        /// <summary>Single column: [header · body · totals · footer], header/totals/footer fixed.</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) Stack(string header, string body, string totals, string footer)
            => (C(Pct(100)), C(Abs(Sc(90)), Pct(100), Abs(Sc(120)), Abs(Sc(64))),
                R(Rg(header, 0, 0), Rg(body, 0, 1), Rg(totals, 0, 2), Rg(footer, 0, 3)));

        /// <summary>Single column: [header · body · footer], header + footer fixed.</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) Stack3(string header, string body, string footer)
            => (C(Pct(100)), C(Abs(Sc(100)), Pct(100), Abs(Sc(120))),
                R(Rg(header, 0, 0), Rg(body, 0, 1), Rg(footer, 0, 2)));

        /// <summary>Single column: fixed header + flexible body.</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) StackTop(string header, string body)
            => (C(Pct(100)), C(Abs(Sc(48)), Pct(100)), R(Rg(header, 0, 0), Rg(body, 0, 1)));

        /// <summary>App shell: full-width header, fixed sidebar + flexible content.</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) Shell2(string header, string sidebar, string content)
            => (C(Abs(Sc(240)), Pct(100)), C(Abs(Sc(90)), Pct(100)),
                R(Rg(header, 0, 0, 2), Rg(sidebar, 0, 1), Rg(content, 1, 1)));

        /// <summary>App shell: full-width header, fixed left + flexible center + fixed right.</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) Shell3(string header, string left, string center, string right)
            => (C(Abs(Sc(240)), Pct(100), Abs(Sc(300))), C(Abs(Sc(90)), Pct(100)),
                R(Rg(header, 0, 0, 3), Rg(left, 0, 1), Rg(center, 1, 1), Rg(right, 2, 1)));

        /// <summary>Header + flexible content + fixed right properties pane.</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) ContentAside(string header, string content, string aside)
            => (C(Pct(100), Abs(Sc(300))), C(Abs(Sc(90)), Pct(100)),
                R(Rg(header, 0, 0, 2), Rg(content, 0, 1), Rg(aside, 1, 1)));

        /// <summary>Two columns at a left/right percentage split.</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) Split(string left, string right, float leftPct)
            => (C(Pct(leftPct), Pct(100 - leftPct)), C(Pct(100)), R(Rg(left, 0, 0), Rg(right, 1, 0)));

        /// <summary>Master-detail: fixed-width list + flexible detail.</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) MD(string list, string detail, int listPx)
            => (C(Abs(Sc(listPx)), Pct(100)), C(Pct(100)), R(Rg(list, 0, 0), Rg(detail, 1, 0)));

        /// <summary>Catalog: full-width toolbar, fixed filters sidebar + flexible grid.</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) Catalog(string toolbar, string filters, string grid)
            => (C(Abs(Sc(240)), Pct(100)), C(Abs(Sc(48)), Pct(100)),
                R(Rg(toolbar, 0, 0, 2), Rg(filters, 0, 1), Rg(grid, 1, 1)));

        /// <summary>Dashboard: full-width KPI row, two charts, full-width table.</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) Dash(string kpis, string chartL, string chartR, string table)
            => (C(Pct(50), Pct(50)), C(Abs(Sc(120)), Pct(100), Pct(100)),
                R(Rg(kpis, 0, 0, 2), Rg(chartL, 0, 1), Rg(chartR, 1, 1), Rg(table, 0, 2, 2)));

        /// <summary>KPI tiles across the top + full-width content below.</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) Kpi(string k1, string k2, string k3, string k4, string content)
            => (C(Pct(25), Pct(25), Pct(25), Pct(25)), C(Abs(Sc(120)), Pct(100)),
                R(Rg(k1, 0, 0), Rg(k2, 1, 0), Rg(k3, 2, 0), Rg(k4, 3, 0), Rg(content, 0, 1, 4)));

        /// <summary>POS: flexible product grid + fixed cart column with a payment row.</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) Pos(string grid, string cart, string payment)
            => (C(Pct(62), Pct(38)), C(Pct(100), Abs(Sc(150))),
                R(Rg(grid, 0, 0, 1, 2), Rg(cart, 1, 0), Rg(payment, 1, 1)));

        /// <summary>Full-width header/toolbar row + N equal columns (kanban / comparison).</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) Columns(string header, params string[] cols)
        {
            var colSpecs = new (SizeType t, float v)[cols.Length];
            for (int i = 0; i < cols.Length; i++) colSpecs[i] = Pct(100f / cols.Length);
            var regions = new Region[cols.Length + 1];
            regions[0] = Rg(header, 0, 0, cols.Length);
            for (int i = 0; i < cols.Length; i++) regions[i + 1] = Rg(cols[i], i, 1);
            return (colSpecs, C(Abs(Sc(48)), Pct(100)), regions);
        }

        /// <summary>Detail with a media pane: gallery (left, full height) + info + extra.</summary>
        private ((SizeType, float)[], (SizeType, float)[], Region[]) Gallery(string gallery, string info, string extra)
            => (C(Pct(45), Pct(55)), C(Pct(100), Abs(Sc(180))),
                R(Rg(gallery, 0, 0, 1, 2), Rg(info, 1, 0), Rg(extra, 1, 1)));
    }
}
