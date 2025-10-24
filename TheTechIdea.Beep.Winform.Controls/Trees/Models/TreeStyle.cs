using System;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Models
{
    /// <summary>
    /// Defines different visual styles for BeepTree rendering.
    /// Each Style represents a distinct UI design pattern from popular frameworks and applications.
    /// </summary>
    public enum TreeStyle
    {
        /// <summary>
        /// Standard Windows Explorer-Style tree with simple lines and folders.
        /// Classic hierarchical view with minimal styling.
        /// </summary>
        Standard = 0,

        /// <summary>
        /// VMware vSphere-Style infrastructure tree with colored tags and status indicators.
        /// Dark theme with hierarchical datacenter/host/VM structure.
        /// </summary>
        InfrastructureTree = 1,

        /// <summary>
        /// Jira/Atlassian Portfolio-Style tree with progress bars and effort indicators.
        /// Grouped themes/epics/features with state tracking.
        /// </summary>
        PortfolioTree = 2,

        /// <summary>
        /// Modern file manager tree (Google Drive, OneDrive Style).
        /// Clean minimal design with folder icons and nested structure.
        /// </summary>
        FileManagerTree = 3,

        /// <summary>
        /// Activity log/timeline tree with status icons and timestamps.
        /// Shows expandable activity groups with detailed status information.
        /// </summary>
        ActivityLogTree = 4,

        /// <summary>
        /// Component/sidebar navigation tree (Figma, VS Code Style).
        /// Grouped sections with drag handles and visibility toggles.
        /// </summary>
        ComponentTree = 5,

        /// <summary>
        /// File browser tree with type-specific icons and deep nesting.
        /// Optimized for folder/file hierarchies with visual type indicators.
        /// <summary>
        /// Deep file browser tree with type-specific icons and folder hierarchy.
        /// Optimized for large nested directory structures.
        /// </summary>
        FileBrowserTree = 6,

        /// <summary>
        /// Multi-level document tree with various content type icons.
        /// Supports mixed content types (folders, files, documents, media).
        /// </summary>
        DocumentTree = 7,

        /// <summary>
        /// Material Design 3 tree with elevation and material colors.
        /// Modern Google Material Design principles with rounded corners.
        /// </summary>
        Material3 = 10,

        /// <summary>
        /// Fluent Design 2 tree (Microsoft 365 Style).
        /// Acrylic backgrounds, subtle shadows, and modern Microsoft design language.
        /// </summary>
        Fluent2 = 11,

        /// <summary>
        /// iOS 15/iPadOS Style tree with SF Symbols and rounded rectangles.
        /// Apple design language with subtle animations and clean aesthetics.
        /// </summary>
        iOS15 = 12,

        /// <summary>
        /// macOS Big Sur Style tree with sidebar design.
        /// Native macOS appearance with transparency and native controls.
        /// </summary>
        MacOSBigSur = 13,

        /// <summary>
        /// Notion-Style minimal tree with clean indentation.
        /// Database/page hierarchy with simple but effective visual design.
        /// </summary>
        NotionMinimal = 14,

        /// <summary>
        /// Vercel dashboard clean tree Style.
        /// Minimalist design with subtle hover effects and clear hierarchy.
        /// </summary>
        VercelClean = 15,

        /// <summary>
        /// Discord channel/server tree Style.
        /// Dark theme optimized for chat/channel hierarchies with icons.
        /// </summary>
        Discord = 16,

        /// <summary>
        /// Ant Design tree component Style.
        /// Enterprise-grade Chinese design system with balanced aesthetics.
        /// </summary>
        AntDesign = 17,

        /// <summary>
        /// Chakra UI tree component Style.
        /// Modern accessible React component library design.
        /// </summary>
        ChakraUI = 18,

        /// <summary>
        /// Bootstrap tree view Style.
        /// Classic Bootstrap framework aesthetics with card-based nodes.
        /// </summary>
        Bootstrap = 19,

        /// <summary>
        /// Tailwind CSS card-based tree.
        /// Utility-first design with card containers for each node.
        /// </summary>
        TailwindCard = 20,

        /// <summary>
        /// DevExpress tree view Style.
        /// Professional enterprise control library appearance.
        /// </summary>
        DevExpress = 21,

        /// <summary>
        /// Syncfusion tree view Style.
        /// Modern enterprise component suite design.
        /// </summary>
        Syncfusion = 22,

        /// <summary>
        /// Telerik UI tree view Style.
        /// Professional controls with rich features and polished appearance.
        /// </summary>
        Telerik = 23,

        /// <summary>
        /// Pill/rail Style tree (sidebar navigation).
        /// Rounded pill-shaped selection indicators for navigation.
        /// </summary>
        PillRail = 24,

        /// <summary>
        /// Stripe Dashboard tree Style.
        /// Clean fintech dashboard design with clear data hierarchy.
        /// </summary>
        StripeDashboard = 25,

        /// <summary>
        /// Figma Layers panel card Style.
        /// Design tool hierarchy with visual layer representation.
        /// </summary>
        FigmaCard = 26
    }
}
