# Helper script to generate HTML documentation pages
# Run: python _gen_docs.py

import os

BASE = r"C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\Help"

HEAD = r"""<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>{title}</title>
<link rel="stylesheet" href="{css_path}sphinx-style.css">
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/themes/prism-tomorrow.min.css">
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet">
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css">
</head>
<body>
<aside class="sidebar" id="sidebar">
<div style="padding:0 1.5rem;margin-bottom:1rem"><h2 style="font-size:1.1rem;margin:0">Beep Controls</h2><span style="font-size:.75rem;color:var(--meta)">{section_name} v1.0.164</span></div>
<nav><ul style="list-style:none;padding:0;font-size:.9rem">
{sidebar_links}
</ul></nav>
</aside>
<main class="content"><div class="content-wrapper">
"""

TAIL = r"""
</div></main>
<script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/components/prism-core.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/plugins/autoloader/prism-autoloader.min.js"></script>
</body></html>"""

# =============================================================================
# DESIGN-TIME PAGES
# =============================================================================


def make_sidebar_dt():
    return [
        "21490",
        "Home",
        "Base Classes",
        "h",
        "basebeepcontroldesigner.html",
        "BaseBeepControlDesigner",
        "basebeepparentcontroldesigner.html",
        "BaseBeepParentControlDesigner",
        "Control Designers",
        "h",
        "beepgridprodesigner.html",
        "BeepGridProDesigner",
        "beepchartdesigner.html",
        "BeepChartDesigner",
        "beepcalendardesigner.html",
        "BeepCalendarDesigner",
        "beepdockdesigner.html",
        "BeepDockDesigner",
        "Docking Designers",
        "h",
        "beepdockingmanagerdesigner.html",
        "BeepDockingManagerDesigner",
        "dockpaneldesigner.html",
        "DockPanelDesigner",
        "beepdockspacedesigner.html",
        "BeepDockspaceDesigner",
        "DocumentHost Designers",
        "h",
        "beepdocumenthostdesigner.html",
        "BeepDocumentHostDesigner",
        "documenthostactionlist.html",
        "DocumentHostActionList",
        "beepdocumentmanagerdesigner.html",
        "BeepDocumentManagerDesigner",
        "Action Lists",
        "h",
        "commonbeepcontrolactionlist.html",
        "CommonBeepControlActionList",
        "Dialogs & Editors",
        "h",
        "beepgridcolumneditordialog.html",
        "BeepGridColumnEditor",
        "pickereditors.html",
        "Picker Editors",
        "Wiring & Helpers",
        "h",
        "beepdockingdesignerwiring.html",
        "BeepDockingDesignerWiring",
        "designtimehelpers.html",
        "Design-Time Helpers",
        "Integrated Designers",
        "h",
        "integrateddesigners.html",
        "BeepBlock/BeepForms Designers",
    ]


def make_sidebar_arch():
    return [
        "21430",
        "Home",
        "Docking",
        "h",
        "docking-overview.html",
        "Docking Architecture",
        "dockpanel-system.html",
        "DockPanel System",
        "docklayoutdefinition.html",
        "DockLayoutDefinition",
        "floatwindow-autohide.html",
        "FloatWindow & AutoHide",
        "docking-painters.html",
        "Docking Painters",
        "docking-dragdrop.html",
        "Docking Drag-Drop",
        "GridX",
        "h",
        "gridx-overview.html",
        "GridX Architecture",
        "gridx-virtualization.html",
        "Virtualization",
        "gridx-selection.html",
        "Selection System",
        "gridx-grouping.html",
        "Grouping Engine",
        "gridx-export.html",
        "Export Engine",
        "gridx-editors.html",
        "Grid Editors",
        "gridx-filtering.html",
        "Grid Filtering",
        "Chart",
        "h",
        "chart-overview.html",
        "Chart Architecture",
        "chart-seriespainters.html",
        "Series Painters",
        "chart-axislegend.html",
        "Axis & Legend",
        "chart-viewportperf.html",
        "Viewport & Performance",
        "Calendar",
        "h",
        "calendar-overview.html",
        "Calendar Architecture",
        "calendar-events.html",
        "Calendar Events",
        "calendar-painting.html",
        "Calendar Painting",
        "calendar-interactions.html",
        "Calendar Interactions",
        "Wizard",
        "h",
        "wizard-overview.html",
        "Wizard Architecture",
        "wizard-forms.html",
        "Wizard Forms",
        "wizard-painters.html",
        "Wizard Painters",
        "Theme System",
        "h",
        "theme-overview.html",
        "Theme Architecture",
        "theme-types.html",
        "Theme Types",
        "theme-tokens.html",
        "Theme Tokens",
        "Menu & Context",
        "h",
        "menubar-internals.html",
        "MenuBar Internals",
        "contextmenu-system.html",
        "ContextMenu System",
        "ListBox",
        "h",
        "listbox-painters.html",
        "ListBox Painters",
        "listbox-internals.html",
        "ListBox Internals",
        "Docks",
        "h",
        "beepdock-architecture.html",
        "BeepDock Architecture",
        "dock-painters.html",
        "Dock Painters",
        "Data & Forms",
        "h",
        "dataconnection.html",
        "DataConnection System",
        "beepforms-contracts.html",
        "BeepForms Contracts",
        "AppBar/Stepper/Marquee",
        "h",
        "appbar-painters.html",
        "AppBar Painters",
        "stepper-painters.html",
        "Stepper Painters",
        "marquee-painters.html",
        "Marquee Painters",
    ]


def build_sidebar(items, prefix=""):
    lines = []
    lines.append(
        '<li style="padding:4px 1.5rem"><a href="{}index.html">&#x2190; Main Docs</a></li>'.format(
            prefix
        )
    )
    for i in range(0, len(items), 2):
        href = items[i]
        label = items[i + 1]
        if label == "h":
            lines.append(
                '<li style="padding:4px 1.5rem;margin-top:1rem;font-weight:700;color:var(--fg)">{}</li>'.format(
                    href
                )
            )
        elif href.startswith("21"):
            # home link
            continue
        else:
            lines.append(
                '<li style="padding:4px 1.5rem"><a href="{}{}">{}</a></li>'.format(
                    prefix, href, label
                )
            )
    return "\n".join(lines)


def toc(items):
    lis = "".join('<li><a href="#{}">{}</a></li>'.format(k, v) for k, v in items)
    return '<div class="toc"><h3>Table of Contents</h3><ul>{}</ul></div>'.format(lis)


def breadcrumb(home_label, parents, current):
    parts = '<a href="../index.html">Home</a>'
    for p in parents:
        parts += '<span>&rsaquo;</span> <a href="{}">{}</a>'.format(p[0], p[1])
    parts += "<span>&rsaquo;</span> <span>{}</span>".format(current)
    return '<nav class="breadcrumb-nav">{}</nav>'.format(parts)


def section(id, heading, body):
    return '<section class="section" id="{}"><h2>{}</h2>{}</section>'.format(
        id, heading, body
    )


def page(title, css_path, section_name, sidebar_links, breadcrumb_html, body):
    h = HEAD.format(
        title=title,
        css_path=css_path,
        section_name=section_name,
        sidebar_links=sidebar_links,
    )
    return h + breadcrumb_html + body + TAIL


# =============================================================================
# PAGE GENERATORS
# =============================================================================


def gen_dt_overview():
    b = '<div class="page-header"><h1>Design-Time Infrastructure</h1><p class="page-subtitle">The Visual Studio design-time experience architecture for Beep Controls — smart-tags, designer verbs, property serialization, and layout manipulation at design time</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("designer-hierarchy", "Designer Hierarchy"),
            ("smart-tag", "Smart-Tag System"),
            ("verbs", "Designer Verbs"),
            ("serialization", "Serialization"),
            ("docking-dt", "Docking Design-Time"),
            ("host-dt", "DocumentHost Design-Time"),
            ("extending", "Creating Custom Designers"),
            ("reference", "Class Reference"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p>Every Beep control participates in the Visual Studio design-time experience through a dedicated <strong>Control Designer</strong> class. Designers control what happens when you drag a control from the Toolbox, click the smart-tag glyph, set properties, or right-click for context menu commands.</p>"
        "<p>The design-time infrastructure lives in <strong>TheTechIdea.Beep.Winform.Controls.Design.Server</strong> project and is organized into five layers:</p>"
        "<table><thead><tr><th>Layer</th><th>Role</th><th>Examples</th></tr></thead><tbody>"
        "<tr><td>Base Designer Classes</td><td>Abstract designers with IComponentChangeService + CommonBeepControlActionList</td><td>BaseBeepControlDesigner, BaseBeepParentControlDesigner</td></tr>"
        "<tr><td>Control Designers</td><td>Per-control smart-tag surfaces and designer verbs</td><td>BeepGridProDesigner, BeepChartDesigner</td></tr>"
        "<tr><td>Action Lists</td><td>Smart-tag property grids rendered in the designer panel</td><td>BeepGridProActionList, DocumentHostActionList</td></tr>"
        "<tr><td>Docking/Container</td><td>Complex layout manipulation for panels, dockspaces, MDI</td><td>DockPanelDesigner, BeepDockingManagerDesigner</td></tr>"
        "<tr><td>Infrastructure</td><td>Wiring helpers, dialogs, collection editors, type converters</td><td>BeepDockingDesignerWiring, BeepGridColumnEditorDialog</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "designer-hierarchy",
        "Designer Hierarchy",
        "<p>Beep uses Microsoft.DotNet.DesignTools as the design-time platform. The hierarchy is:</p>"
        '<pre><code class="language-csharp">'
        "// Abstract base for all leaf controls (buttons, labels, grids, charts...)\n"
        "public abstract class BaseBeepControlDesigner : ControlDesigner, IBeepDesignerActionHost\n"
        "{\n"
        "    protected IComponentChangeService ChangeService;\n"
        "    protected BaseControl BeepControl;\n"
        "    public void SetProperty(string name, object value);\n"
        "    public T GetProperty&lt;T&gt;(string name);\n"
        "    public void ApplyTheme();\n"
        "    protected abstract DesignerActionListCollection GetControlSpecificActionLists();\n"
        "}\n"
        "\n"
        "// Abstract base for container/parent controls (panels, tabs, DocumentHost...)\n"
        "public abstract class BaseBeepParentControlDesigner : ControlDesigner, IBeepDesignerActionHost\n"
        "{\n"
        "    // Same pattern, separated for future ParentControlDesigner migration\n"
        "    public void SetProperty(string name, object value);\n"
        "    public T GetProperty&lt;T&gt;(string name);\n"
        "    public void ApplyTheme();\n"
        "}</code></pre>"
        "<p>Each concrete designer overrides <code>GetControlSpecificActionLists()</code> to add its smart-tag. The <code>ActionLists</code> property automatically combines control-specific lists with <code>CommonBeepControlActionList</code> (style/theme/schema for every control).</p>"
        "<h3>IBeepDesignerActionHost Interface</h3>"
        '<pre><code class="language-csharp">public interface IBeepDesignerActionHost\n'
        "{\n"
        "    IComponent Component { get; }\n"
        "    void SetProperty(string propertyName, object value);\n"
        "    T GetProperty&lt;T&gt;(string propertyName);\n"
        "    void ApplyTheme();\n"
        "}</code></pre>",
    )
    b += section(
        "smart-tag",
        "Smart-Tag (Action List) System",
        "<p>Smart-tags are the property panel that opens when clicking the glyph (&#x25BC;) on a selected control. They use <strong>DesignerActionList</strong> subclasses.</p>"
        "<h3>How It Works</h3>"
        "<ol><li>The designer's <code>ActionLists</code> property returns a collection of action lists</li>"
        "<li>Each action list exposes public properties (become editable items) and void methods (become clickable links)</li>"
        "<li>The <code>GetSortedActionItems()</code> override organizes items into named categories</li></ol>"
        "<h3>BeepGridProActionList — 47 Smart-Tag Items</h3>"
        "<p>The largest single-control action list in the library, organized into 7 sections:</p>"
        "<table><thead><tr><th>Section</th><th>Properties</th></tr></thead><tbody>"
        "<tr><td>Quick Configuration</td><td>ConfigureAsDataDisplay(), ConfigureAsDataEntry(), ConfigureAsSimpleList(), ConfigureAsSelectionGrid(), GenerateSampleData(), EditColumns(), BestFitVisibleColumns()</td></tr>"
        "<tr><td>Row Height Presets</td><td>SetStandardRowHeight(), SetCompactRowHeight(), SetComfortableRowHeight()</td></tr>"
        "<tr><td>Features</td><td>EnableAllFeatures(), DisableInteractiveFeatures()</td></tr>"
        "<tr><td>Data</td><td>DataSource, DataMember</td></tr>"
        "<tr><td>Layout</td><td>RowHeight, ColumnHeaderHeight, ShowColumnHeaders, ShowNavigator, AutoSizeColumnsMode, AutoSizeTriggerMode</td></tr>"
        "<tr><td>Appearance</td><td>GridStyle, GridTitle, NavigationStyle, ShowCheckBox, MultiSelect, ReadOnly</td></tr>"
        "<tr><td>Filtering</td><td>ShowTopFilterPanel, TopFilterPanelHeight, SortIconVisibility, FilterIconVisibility</td></tr>"
        "</tbody></table>"
        "<h3>DocumentHostActionList — 40+ Items, 18 Sections</h3>"
        "<p>The richest action list, covering document management, split views, tab configuration, theming, and session state.</p>",
    )
    b += section(
        "verbs",
        "Designer Verbs",
        "<p>Designer verbs appear in the right-click context menu at design time. Override the <code>Verbs</code> property.</p>"
        "<h3>BeepDockingManagerDesigner (15 verbs)</h3>"
        "<table><thead><tr><th>Verb</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Add Panel Left/Right/Top/Bottom/Fill</td><td>Creates a new DockPanel at the specified edge</td></tr>"
        "<tr><td>Move Selected Panel Left/Right/Top/Bottom/Fill</td><td>Repositions currently selected panel</td></tr>"
        "<tr><td>Stack Selected with Previous/Next Panel</td><td>Groups selected panel as a tab with adjacent panel</td></tr>"
        "<tr><td>&amp;Validate Panels</td><td>Verifies all panels have valid Keys</td></tr>"
        "<tr><td>Attach &amp;Host Form</td><td>Auto-detects and assigns the parent Form</td></tr>"
        "</tbody></table>"
        "<h3>DockPanelDesigner (10 verbs)</h3>"
        "<table><thead><tr><th>Verb</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Dock Left/Right/Top/Bottom/Fill</td><td>Move panel to a dock edge</td></tr>"
        "<tr><td>Stack with Previous/Next Panel</td><td>Group as a tab with adjacent panel</td></tr>"
        "<tr><td>Move Earlier/Later in Stack</td><td>Reorder within tab group</td></tr>"
        "<tr><td>&amp;Remove Panel</td><td>Delete the panel</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "serialization",
        "IComponentChangeService &amp; Serialization",
        "<p>All property mutations at design time must route through <strong>IComponentChangeService</strong> for proper .designer.cs code generation.</p>"
        '<pre><code class="language-csharp">public void SetProperty(string propertyName, object value)\n'
        "{\n"
        "    PropertyDescriptor property =\n"
        "        TypeDescriptor.GetProperties(Component)[propertyName];\n"
        "    ChangeService?.OnComponentChanging(Component, property);  // undo\n"
        "    property.SetValue(Component, value);                        // set\n"
        "    ChangeService?.OnComponentChanged(Component, property,      // serialize\n"
        "        null, value);\n"
        "}</code></pre>"
        "<p>This three-step pattern ensures: Undo support (OnComponentChanging), property update, code generation (OnComponentChanged). Without it, changes made in designers would not persist to the .designer.cs file.</p>",
    )
    b += section(
        "docking-dt",
        "Docking Design-Time",
        "<p>The docking design-time system is the most sophisticated in the library, modeled after Krypton's <code>KryptonDockingManager</code>:</p>"
        "<ul><li><strong>BeepDockingManagerDesigner</strong> — Non-visual component designer (tray component). Auto-detects host form on drop.</li>"
        "<li><strong>DockPanelDesigner</strong> — ParentControlDesigner for individual panels. Auto-generates unique Keys. Snap-to-edge when moved.</li>"
        "<li><strong>BeepDockspaceDesigner</strong> — ParentControlDesigner for dockspace controls. Tab drag-drop within header.</li></ul>"
        "<p>All mutations route through <strong>BeepDockingDesignerWiring</strong>, a static helper that provides Panel CRUD, Move, Stack, Resize, and host-layout-refresh operations with proper IComponentChangeService notifications.</p>",
    )
    b += section(
        "host-dt",
        "DocumentHost Design-Time",
        "<p>The DocumentHost design-time experience (DevExpress parity) includes:</p>"
        "<ul><li><strong>BeepDocumentHostDesigner</strong> — Accepts any control via CanParent, routes drops to active document area</li>"
        "<li><strong>DocumentHostActionList</strong> — 40+ smart-tag items across 18 organized sections</li>"
        "<li><strong>BeepDocumentPanelDesigner</strong> — Filters Properties window to 5 meaningful properties</li>"
        "<li>Tab click → Properties window updates for selected document panel</li>"
        "<li>Smart-tag verbs: Add/Clear Documents, Apply Layout Presets, Edit Design-Time Documents, Setup Wizard</li></ul>",
    )
    b += section(
        "extending",
        "Creating Custom Designers",
        "<h3>For a Leaf Control</h3>"
        '<pre><code class="language-csharp">[Designer(typeof(MyControlDesigner))]\n'
        "public class MyControl : BaseControl\n"
        "{\n"
        '    public string CustomProp { get; set; } = "Default";\n'
        "}\n"
        "\n"
        "public class MyControlDesigner : BaseBeepControlDesigner\n"
        "{\n"
        "    public MyControl MyCtrl => (MyControl)Component;\n"
        "\n"
        "    protected override DesignerActionListCollection GetControlSpecificActionLists()\n"
        "    {\n"
        "        return new DesignerActionListCollection\n"
        "        {\n"
        "            new MyActionList(Component)\n"
        "        };\n"
        "    }\n"
        "}\n"
        "\n"
        "internal class MyActionList : DesignerActionList\n"
        "{\n"
        "    private MyControlDesigner Designer =>\n"
        "        (MyControlDesigner)Component.Designer;\n"
        "\n"
        "    public string CustomProp\n"
        "    {\n"
        "        get => Designer.GetProperty&lt;string&gt;(nameof(customProp));\n"
        "        set => Designer.SetProperty(nameof(customProp), value);\n"
        "    }\n"
        "\n"
        "    public override DesignerActionItemCollection GetSortedActionItems()\n"
        "    {\n"
        "        var items = new DesignerActionItemCollection();\n"
        '        items.Add(new DesignerActionHeaderItem("Custom"));\n'
        "        items.Add(new DesignerActionPropertyItem(\n"
        '            nameof(CustomProp), "Custom Property", "Custom"));\n'
        "        return items;\n"
        "    }\n"
        "}</code></pre>",
    )
    b += section(
        "reference",
        "Class Reference",
        "<table><thead><tr><th>Class</th><th>Namespace</th><th>Base Class</th><th>Usage</th></tr></thead><tbody>"
        "<tr><td>BaseBeepControlDesigner</td><td>Designers</td><td>ControlDesigner</td><td>Abstract base for leaf controls</td></tr>"
        "<tr><td>BaseBeepParentControlDesigner</td><td>Designers</td><td>ControlDesigner</td><td>Abstract base for containers</td></tr>"
        "<tr><td>BeepGridProDesigner</td><td>Designers</td><td>BaseBeepControlDesigner</td><td>47 smart-tag items, configure presets</td></tr>"
        "<tr><td>BeepChartDesigner</td><td>Designers</td><td>BaseBeepControlDesigner</td><td>Title, legend, grid smart-tag</td></tr>"
        "<tr><td>BeepCalendarDesigner</td><td>Designers</td><td>BaseBeepControlDesigner</td><td>Week numbers, today button</td></tr>"
        "<tr><td>BeepDockDesigner</td><td>Designers</td><td>BaseBeepControlDesigner</td><td>14 props, 9 style presets, 4 positions</td></tr>"
        "<tr><td>BeepComboBoxDesigner</td><td>Designers</td><td>BaseBeepControlDesigner</td><td>ComboBox-specific smart-tag</td></tr>"
        "<tr><td>BeepMenuBarDesigner</td><td>Designers</td><td>BaseBeepControlDesigner</td><td>Menu bar design-time actions</td></tr>"
        "<tr><td>BeepAccordionMenuDesigner</td><td>Designers</td><td>BaseBeepControlDesigner</td><td>Accordion menu designer</td></tr>"
        "<tr><td>BeepDockingManagerDesigner</td><td>Docking.Designers</td><td>ComponentDesigner</td><td>Tray component, 15 verbs</td></tr>"
        "<tr><td>DockPanelDesigner</td><td>Docking.Designers</td><td>ParentControlDesigner</td><td>10 verbs, auto-key, snap-to-edge</td></tr>"
        "<tr><td>BeepDockspaceDesigner</td><td>Docking.Designers</td><td>ParentControlDesigner</td><td>Tab drag-drop, header routing</td></tr>"
        "<tr><td>DocumentHostActionList</td><td>ActionLists</td><td>DesignerActionList</td><td>40+ items, 18 sections</td></tr>"
        "<tr><td>CommonBeepControlActionList</td><td>ActionLists</td><td>DesignerActionList</td><td>Style/Theme/Schema for all</td></tr>"
        "<tr><td>BeepDockingDesignerWiring</td><td>Docking.Infrastructure</td><td>Static helper</td><td>Panel CRUD, move, resize</td></tr>"
        "</tbody></table>",
    )

    return b


def gen_dt_baseleaf():
    b = '<div class="page-header"><h1>BaseBeepControlDesigner</h1><p class="page-subtitle">The abstract base designer for all leaf (non-container) Beep controls — provides unified smart-tag surface with style/theme/schema and IComponentChangeService-backed property mutation</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("features", "Key Features"),
            ("properties", "Properties"),
            ("methods", "Methods"),
            ("inheritance", "Inheritance Chain"),
            ("smart-tag", "Smart-Tag Integration"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.Designers</code><br>"
        "<strong>Assembly:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.dll</code><br>"
        "<strong>Inheritance:</strong> <code>Microsoft.DotNet.DesignTools.Designers.ControlDesigner</code><br>"
        "<strong>Implements:</strong> <code>IBeepDesignerActionHost</code></p>"
        "<p><code>BaseBeepControlDesigner</code> is the <strong>abstract base class</strong> for all leaf control designers in the Beep library. Every control that is NOT a container (buttons, labels, textboxes, grids, charts, calendars, etc.) has a designer that derives from this class.</p>"
        "<p>It provides three critical services:</p>"
        "<ol><li><strong>IComponentChangeService integration</strong> — Every property mutation routes through OnComponentChanging/OnComponentChanged for undo and .designer.cs serialization</li>"
        "<li><strong>Unified smart-tag</strong> — Combines <code>CommonBeepControlActionList</code> (style/theme/schema) with control-specific action lists</li>"
        "<li><strong>Convenience methods</strong> — <code>SetProperty()</code>, <code>GetProperty&lt;T&gt;()</code>, <code>ApplyTheme()</code>, <code>SetStyle()</code></li></ol>",
    )
    b += section(
        "features",
        "Key Features",
        "<ul><li>Implements <code>IBeepDesignerActionHost</code> for consumption by action lists</li>"
        "<li>Captures <code>IComponentChangeService</code> during <code>Initialize()</code></li>"
        "<li>Exposes <code>BeepControl</code> (casts Component to BaseControl) for derived classes</li>"
        "<li>Auto-combines <code>CommonBeepControlActionList</code> with control-specific lists in <code>ActionLists</code> property</li>"
        "<li>Provides <code>SetStyle(BeepControlStyle)</code> convenience method for style changes</li>"
        "<li>All property setters use <code>TypeDescriptor</code> for proper property descriptor resolution</li>"
        "<li>Abstract: derived designers MUST implement <code>GetControlSpecificActionLists()</code></li></ul>",
    )
    b += section(
        "properties",
        "Properties",
        "<table><thead><tr><th>Member</th><th>Type</th><th>Visibility</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>ChangeService</td><td>IComponentChangeService</td><td>protected</td><td>Captured during Initialize(). Used for undo/code-gen.</td></tr>"
        "<tr><td>BeepControl</td><td>BaseControl</td><td>protected</td><td>Casts Component to BaseControl for convenient access.</td></tr>"
        "<tr><td>ActionLists</td><td>DesignerActionListCollection</td><td>public override</td><td>Returns CommonBeepControlActionList + control-specific lists.</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "methods",
        "Methods",
        "<table><thead><tr><th>Method</th><th>Signature</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Initialize</td><td>void Initialize(IComponent component)</td><td>Captures IComponentChangeService. Calls base.</td></tr>"
        "<tr><td>SetProperty</td><td>void SetProperty(string name, object value)</td><td>Sets property via TypeDescriptor with full change notification.</td></tr>"
        "<tr><td>GetProperty</td><td>T GetProperty&lt;T&gt;(string name)</td><td>Gets property value via TypeDescriptor.</td></tr>"
        "<tr><td>ApplyTheme</td><td>void ApplyTheme()</td><td>Calls BeepControl?.ApplyTheme().</td></tr>"
        '<tr><td>SetStyle</td><td>void SetStyle(BeepControlStyle style)</td><td>Convenience: calls SetProperty("ControlStyle", style).</td></tr>'
        "<tr><td>GetControlSpecificActionLists</td><td>abstract DesignerActionListCollection ...()</td><td>Override to provide control-specific smart-tag items.</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "inheritance",
        "Inheritance Chain",
        '<pre><code class="language-plaintext">Microsoft.DotNet.DesignTools.Designers.ControlDesigner\n'
        "  └── BaseBeepControlDesigner (abstract)\n"
        "       ├── BeepButtonDesigner\n"
        "       ├── BeepLabelDesigner\n"
        "       ├── BeepImageDesigner\n"
        "       ├── BeepCheckBoxDesigner\n"
        "       ├── BeepComboBoxDesigner\n"
        "       ├── BeepGridProDesigner\n"
        "       ├── BeepChartDesigner\n"
        "       ├── BeepCalendarDesigner\n"
        "       ├── BeepDockDesigner\n"
        "       ├── BeepBreadcrumpDesigner\n"
        "       ├── BeepMenuBarDesigner\n"
        "       ├── BeepAccordionMenuDesigner\n"
        "       ├── BeepMultiChipGroupDesigner\n"
        "       ├── BeepDatePickerDesigner\n"
        "       ├── BeepMultiSplitterDesigner\n"
        "       └── ... (+ widget designers)</code></pre>",
    )
    b += section(
        "smart-tag",
        "Smart-Tag Integration",
        "<p>Every leaf control automatically gets <code>CommonBeepControlActionList</code> in its smart-tag panel. This always includes:</p>"
        "<ul><li><strong>ControlStyle</strong> — The visual style preset (Material3, Fluent, Cyberpunk, etc.)</li>"
        "<li><strong>Theme</strong> — The theme name to apply</li>"
        "<li><strong>Schema</strong> — Color schema selection</li></ul>"
        "<p>Control-specific properties and actions are added by overriding <code>GetControlSpecificActionLists()</code> in the derived designer.</p>",
    )
    b += section(
        "examples",
        "Examples",
        "<h3>Setting a Property from an Action List</h3>"
        '<pre><code class="language-csharp">internal class MyActionList : DesignerActionList\n'
        "{\n"
        "    private IBeepDesignerActionHost Host =>\n"
        "        (IBeepDesignerActionHost)Component.Designer;\n"
        "\n"
        "    public int RowHeight\n"
        "    {\n"
        "        get => Host.GetProperty&lt;int&gt;(nameof(RowHeight));\n"
        "        set => Host.SetProperty(nameof(RowHeight), value);\n"
        "    }\n"
        "}</code></pre>"
        "<h3>Implementing a Concrete Leaf Designer</h3>"
        '<pre><code class="language-csharp">[Designer(typeof(MyControlDesigner))]\n'
        "public class MyControl : BaseControl { }\n"
        "\n"
        "public class MyControlDesigner : BaseBeepControlDesigner\n"
        "{\n"
        "    protected override DesignerActionListCollection GetControlSpecificActionLists()\n"
        "    {\n"
        "        return new DesignerActionListCollection\n"
        "        {\n"
        "            new MyControlActionList(Component)\n"
        "        };\n"
        "    }\n"
        "}</code></pre>",
    )
    return b


def gen_dt_baseparent():
    b = '<div class="page-header"><h1>BaseBeepParentControlDesigner</h1><p class="page-subtitle">The abstract base designer for container/parent Beep controls — provides shape-aware design surface interaction with smart-tag support</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("features", "Key Features"),
            ("properties", "Properties & Methods"),
            ("vs-baseleaf", "vs BaseBeepControlDesigner"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.Designers</code><br>"
        "<strong>Assembly:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.dll</code><br>"
        "<strong>Inheritance:</strong> <code>Microsoft.DotNet.DesignTools.Designers.ControlDesigner</code><br>"
        "<strong>Implements:</strong> <code>IBeepDesignerActionHost</code></p>"
        "<p><code>BaseBeepParentControlDesigner</code> is the abstract base class for <strong>container/parent</strong> control designers. It mirrors <code>BaseBeepControlDesigner</code> structurally but exists as a separate class to allow future specialization (e.g., migrating to <code>ParentControlDesigner</code>).</p>"
        "<p>It provides the same <code>IComponentChangeService</code>-backed property mutation pattern, smart-tag integration, and theme-aware design surface.</p>",
    )
    b += section(
        "features",
        "Key Features",
        "<ul><li>Implements <code>IBeepDesignerActionHost</code> (same as BaseBeepControlDesigner)</li>"
        "<li>Captures <code>IComponentChangeService</code> during <code>Initialize()</code></li>"
        "<li>Auto-combines <code>CommonBeepControlActionList</code> with control-specific lists</li>"
        "<li>Read-only property guard in SetProperty()</li>"
        "<li>Separate class tree from leaf designers — enables future <code>ParentControlDesigner</code> migration</li>"
        "<li>Container-appropriate hit testing and shape-aware rendering</li></ul>",
    )
    b += section(
        "properties",
        "Properties & Methods",
        "<table><thead><tr><th>Member</th><th>Type</th><th>Visibility</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>ChangeService</td><td>IComponentChangeService</td><td>protected</td><td>For undo and serialization</td></tr>"
        "<tr><td>BeepControl</td><td>BaseControl</td><td>protected</td><td>Casts Component to BaseControl</td></tr>"
        "<tr><td>ActionLists</td><td>DesignerActionListCollection</td><td>public override</td><td>Common + control-specific lists</td></tr>"
        "<tr><td>SetProperty</td><td>void (string, object)</td><td>public</td><td>TypeDescriptor-based set with IsReadOnly guard</td></tr>"
        "<tr><td>GetProperty</td><td>T GetProperty&lt;T&gt;(string)</td><td>public</td><td>TypeDescriptor-based get</td></tr>"
        "<tr><td>ApplyTheme</td><td>void ApplyTheme()</td><td>public</td><td>Calls BeepControl?.ApplyTheme()</td></tr>"
        "<tr><td>GetControlSpecificActionLists</td><td>abstract ...</td><td>protected</td><td>Override for control-specific smart-tags</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "vs-baseleaf",
        "vs BaseBeepControlDesigner",
        "<table><thead><tr><th>Aspect</th><th>BaseBeepControlDesigner</th><th>BaseBeepParentControlDesigner</th></tr></thead><tbody>"
        "<tr><td>Purpose</td><td>Leaf (non-container) controls</td><td>Container/parent controls</td></tr>"
        "<tr><td>SetStyle() convenience</td><td>Yes</td><td>No</td></tr>"
        "<tr><td>IsReadOnly guard</td><td>No</td><td>Yes</td></tr>"
        "<tr><td>Shape-aware</td><td>No</td><td>Yes</td></tr>"
        "<tr><td>Hit testing</td><td>Standard</td><td>Standard</td></tr>"
        "<tr><td>Derived designers</td><td>BeepDocumentHostDesigner, BeepFormsDesigner</td><td>Grid, Chart, Calendar, Dock, etc.</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">[Designer(typeof(MyContainerDesigner))]\n'
        "public class MyContainer : BaseControl\n"
        "{\n"
        "    public MyContainer() { IsContainerControl = true; }\n"
        "}\n"
        "\n"
        "public class MyContainerDesigner : BaseBeepParentControlDesigner\n"
        "{\n"
        "    protected override DesignerActionListCollection GetControlSpecificActionLists()\n"
        "    {\n"
        "        return new DesignerActionListCollection\n"
        "        {\n"
        "            new MyContainerActionList(Component)\n"
        "        };\n"
        "    }\n"
        "}</code></pre>",
    )
    return b


def gen_dt_gridpro():
    b = '<div class="page-header"><h1>BeepGridProDesigner</h1><p class="page-subtitle">The design-time designer for BeepGridPro — 47 smart-tag properties, quick configuration presets, column editor, sample data generation</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("features", "Key Features"),
            ("smart-tag", "Smart-Tag Sections"),
            ("actions", "Quick Configuration Actions"),
            ("sample-data", "Design-Time Sample Data"),
            ("column-editor", "Column Editor"),
            ("examples", "Code Example"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.Designers</code><br>"
        "<strong>Inheritance:</strong> <code>BaseBeepControlDesigner</code></p>"
        "<p><code>BeepGridProDesigner</code> is the richest single-control designer in the library with <strong>47 smart-tag items</strong> organized across 7 named sections. It provides:</p>"
        "<ul><li>Quick configuration presets (DataDisplay, DataEntry, SimpleList, SelectionGrid)</li>"
        "<li>Design-time sample data generation</li>"
        "<li>Column editor dialog integration</li>"
        "<li>Row height presets (Standard/Compact/Comfortable)</li>"
        "<li>Batch feature toggles (EnableAll/DisableAll)</li>"
        "<li>Column auto-sizing commands</li></ul>",
    )
    b += section(
        "features",
        "Key Features",
        "<ul><li>4 quick-configuration presets that set multiple properties at once</li>"
        "<li>GenerateSampleData() creates a 6-row BindingList for design-time preview</li>"
        "<li>EditColumns() opens the BeepGridColumnEditorDialog modal</li>"
        "<li>3 row-height presets: Standard (24px), Compact (20px), Comfortable (28px)</li>"
        "<li>Column auto-size: BestFitVisibleColumns, BestFitVisibleColumnsAllRows, AutoSizeColumnsNow</li>"
        "<li>Inherits CommonBeepControlActionList for style/theme/schema</li>"
        "<li>All property changes route through IComponentChangeService</li></ul>",
    )
    b += section(
        "smart-tag",
        "Smart-Tag Sections",
        "<table><thead><tr><th>Section</th><th>Items</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Quick Configuration</td><td>7 action methods</td><td>ConfigureAsDataDisplay, ConfigureAsDataEntry, ConfigureAsSimpleList, ConfigureAsSelectionGrid, GenerateSampleData, ClearSamplePreview, EditColumns</td></tr>"
        "<tr><td>Row Height Presets</td><td>3 action methods</td><td>SetStandardRowHeight (24px), SetCompactRowHeight (20px), SetComfortableRowHeight (28px)</td></tr>"
        "<tr><td>Features</td><td>2 action methods</td><td>EnableAllFeatures, DisableInteractiveFeatures</td></tr>"
        "<tr><td>Data Properties</td><td>2 properties</td><td>DataSource (object), DataMember (string)</td></tr>"
        "<tr><td>Layout Properties</td><td>6 properties</td><td>RowHeight, ColumnHeaderHeight, ShowColumnHeaders, ShowNavigator, AutoSizeColumnsMode, AutoSizeTriggerMode</td></tr>"
        "<tr><td>Auto-Size</td><td>3 action methods</td><td>BestFitVisibleColumns, BestFitVisibleColumnsAllRows, AutoSizeColumnsNow</td></tr>"
        "<tr><td>Behavior</td><td>5 properties</td><td>AllowUserToResizeColumns, AllowColumnReorder, MultiSelect, ReadOnly, ShowCheckBox</td></tr>"
        "<tr><td>Appearance</td><td>5 properties</td><td>GridStyle, GridTitle, NavigationStyle, SortIconVisibility, FilterIconVisibility</td></tr>"
        "<tr><td>Focus Styling</td><td>3 properties</td><td>UseDedicatedFocusedRowStyle, ShowFocusedCellFill, ShowFocusedCellBorder, FocusedCellBorderWidth</td></tr>"
        "<tr><td>Filtering UI</td><td>2 properties</td><td>ShowTopFilterPanel, TopFilterPanelHeight</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "actions",
        "Quick Configuration Actions",
        "<h3>ConfigureAsDataDisplay()</h3>"
        "<p>Sets: ReadOnly=true, ShowColumnHeaders=true, ShowNavigator=true, MultiSelect=true, AllowUserToResizeColumns=true, AllowColumnReorder=true</p>"
        "<h3>ConfigureAsDataEntry()</h3>"
        "<p>Sets: ReadOnly=false, ShowColumnHeaders=true, ShowNavigator=true, MultiSelect=false, AllowUserToResizeColumns=true</p>"
        "<h3>ConfigureAsSimpleList()</h3>"
        "<p>Sets: ShowColumnHeaders=false, ShowNavigator=false, AllowUserToResizeColumns=false, AllowColumnReorder=false, MultiSelect=false</p>"
        "<h3>ConfigureAsSelectionGrid()</h3>"
        "<p>Sets: MultiSelect=true, ShowCheckBox=true, ReadOnly=true</p>",
    )
    b += section(
        "sample-data",
        "Design-Time Sample Data",
        "<p>The <code>GenerateSampleData()</code> method creates a <code>BindingList&lt;DesignTimeGridPreviewRow&gt;</code> with 6 sample rows. It uses reflection to access the grid's internal <code>Data</code> and <code>Layout</code> helpers, calls <code>Bind()</code> then <code>Recalculate()</code>. The operation is wrapped in a try/catch to prevent designer crashes.</p>"
        '<pre><code class="language-csharp">public void GenerateSampleData()\n'
        "{\n"
        "    var sampleData = new BindingList&lt;DesignTimeGridPreviewRow&gt;();\n"
        "    for (int i = 0; i &lt; 6; i++)\n"
        "        sampleData.Add(new DesignTimeGridPreviewRow\n"
        "        {\n"
        "            Id = i + 1,\n"
        '            Name = $"Item {i + 1}",\n'
        '            Description = $"Description for item {i + 1}",\n'
        "            Value = (i + 1) * 100m,\n"
        "            IsActive = i % 2 == 0,\n"
        "            CreatedDate = DateTime.Now.AddDays(-i)\n"
        "        });\n"
        "    // Use reflection to bind to grid internals\n"
        "    var dataHelper = grid.GetType()\n"
        '        .GetField("Data", BindingFlags.NonPublic | BindingFlags.Instance)\n'
        "        ?.GetValue(grid);\n"
        "    dataHelper?.GetType()\n"
        '        .GetMethod("Bind")?.Invoke(dataHelper, new[] { sampleData });\n'
        "}</code></pre>",
    )
    b += section(
        "column-editor",
        "Column Editor",
        "<p>The <code>EditColumns()</code> method opens the <strong>BeepGridColumnEditorDialog</strong> — a modal form that provides full column management:</p>"
        "<ul><li>Add/remove/reorder columns</li>"
        "<li>Set column type (Text, Numeric, Date, ComboBox, CheckBox, Masked)</li>"
        "<li>Configure column width, header text, read-only flag</li>"
        "<li>Visual column preview</li></ul>"
        "<p>After the dialog closes, the grid layout is recalculated and the designer surface is invalidated for immediate visual feedback.</p>",
    )
    b += section(
        "examples",
        "Code Example",
        '<pre><code class="language-csharp">// At design time, the smart-tag provides all configuration.\n'
        "// At runtime, bind data programmatically:\n"
        "var grid = new BeepGridPro();\n"
        "grid.DataSource = myDataTable;\n"
        "grid.RowHeight = 28;\n"
        "grid.AllowUserToResizeColumns = true;\n"
        "grid.MultiSelect = true;\n"
        "grid.ShowColumnHeaders = true;\n"
        'grid.GridStyle = "DataTables";</code></pre>',
    )
    return b


def gen_dt_dockingmgr():
    b = '<div class="page-header"><h1>BeepDockingManagerDesigner</h1><p class="page-subtitle">The non-visual component designer for BeepDockingManager — lives in the component tray, provides 15 designer verbs and smart-tag for panel management at design time</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("features", "Key Features"),
            ("verbs", "Designer Verbs"),
            ("smart-tag", "Smart-Tag"),
            ("design-time-behavior", "Design-Time Behavior"),
            ("wiring", "Wiring Architecture"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Designers</code><br>"
        "<strong>Inheritance:</strong> <code>ComponentDesigner</code> (NOT a ControlDesigner — this is a tray component)<br>"
        "<strong>Visibility:</strong> <code>internal sealed</code></p>"
        "<p><code>BeepDockingManagerDesigner</code> is the design-time experience for <strong>BeepDockingManager</strong>. Since the manager is a non-visual component, it appears in the component tray below the form designer rather than on the form surface. This follows the same pattern as Krypton's KryptonDockingManager.</p>"
        "<p>It provides <strong>15 designer verbs</strong> for adding, moving, stacking, and validating dock panels entirely from the Visual Studio designer.</p>",
    )
    b += section(
        "features",
        "Key Features",
        "<ul><li><strong>Tray component</strong> — Sits in the component tray, not on the form surface</li>"
        "<li><strong>15 designer verbs</strong> — Context menu commands for panel management</li>"
        "<li><strong>Smart-tag</strong> — BeepDockingManagerActionList for property configuration</li>"
        "<li><strong>Auto host-form detection</strong> — On InitializeNewComponent, finds parent Form and assigns HostForm</li>"
        "<li><strong>Panel validation</strong> — ValidatePanels() checks all associated panels for missing Keys</li>"
        "<li><strong>IComponentChangeService integration</strong> — All mutations route through wiring for proper serialization</li>"
        "<li><strong>Krypton parity</strong> — Follows KryptonDockingManager design-time pattern</li></ul>",
    )
    b += section(
        "verbs",
        "Designer Verbs (15)",
        "<table><thead><tr><th>Category</th><th>Verb</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Add Panel</td><td>Add Panel Left</td><td>Creates a new DockPanel docked to the left edge</td></tr>"
        "<tr><td></td><td>Add Panel Right</td><td>Creates a new DockPanel docked to the right edge</td></tr>"
        "<tr><td></td><td>Add Panel Top</td><td>Creates a new DockPanel docked to the top edge</td></tr>"
        "<tr><td></td><td>Add Panel Bottom</td><td>Creates a new DockPanel docked to the bottom edge</td></tr>"
        "<tr><td></td><td>Add Panel Fill</td><td>Creates a new DockPanel filling remaining space</td></tr>"
        "<tr><td>Move Panel</td><td>Move Selected Panel Left</td><td>Moves currently selected panel to left edge</td></tr>"
        "<tr><td></td><td>Move Selected Panel Right</td><td>Moves currently selected panel to right edge</td></tr>"
        "<tr><td></td><td>Move Selected Panel Top</td><td>Moves currently selected panel to top edge</td></tr>"
        "<tr><td></td><td>Move Selected Panel Bottom</td><td>Moves currently selected panel to bottom edge</td></tr>"
        "<tr><td></td><td>Move Selected Panel Fill</td><td>Moves currently selected panel to fill</td></tr>"
        "<tr><td>Stack</td><td>Stack Selected with Previous Panel</td><td>Groups selected panel into tab stack with previous</td></tr>"
        "<tr><td></td><td>Stack Selected with Next Panel</td><td>Groups selected panel into tab stack with next</td></tr>"
        "<tr><td>Management</td><td>&amp;Validate Panels</td><td>Checks all panels for missing/duplicate Keys</td></tr>"
        "<tr><td></td><td>Attach &amp;Host Form</td><td>Auto-detects and assigns Manager.HostForm</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "smart-tag",
        "Smart-Tag",
        "<p>The <code>BeepDockingManagerActionList</code> exposes these properties in the smart-tag panel:</p>"
        "<table><thead><tr><th>Property</th><th>Type</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>HostForm</td><td>Form</td><td>The hosting form. Setting this calls ManageControl().</td></tr>"
        "<tr><td>Theme</td><td>string</td><td>Theme name for docking chrome.</td></tr>"
        "<tr><td>UseThemeColors</td><td>bool</td><td>When true, reads from Beep theme.</td></tr>"
        "<tr><td>Style</td><td>BeepControlStyle</td><td>Control style for docking surfaces.</td></tr>"
        "<tr><td>PanelCount</td><td>int (read-only)</td><td>Number of currently managed panels.</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "design-time-behavior",
        "Design-Time Behavior",
        "<h3>InitializeNewComponent</h3>"
        "<p>When first dropped on a form, the designer detects the parent form via the designer host and assigns it as <code>Manager.HostForm</code>. This mimics the Krypton behavior where the manager automatically binds to its hosting form.</p>"
        "<h3>Panel Operations</h3>"
        "<p>All panel Add/Move/Stack/Remove operations delegate to <code>BeepDockingDesignerWiring</code> which handles:</p>"
        "<ol><li>Creating new <code>DockPanel</code> components via <code>IDesignerHost.CreateComponent()</code></li>"
        "<li>Setting panel properties (Key, Title, DockPosition)</li>"
        "<li>Notifying <code>IComponentChangeService</code> for serialization</li>"
        "<li>Refreshing the host layout</li></ol>",
    )
    b += section(
        "wiring",
        "Wiring Architecture",
        '<pre><code class="language-plaintext">BeepDockingManagerDesigner (ComponentDesigner)\n'
        "  │\n"
        "  ├── AddPanel(position)  ──┐\n"
        "  ├── RemovePanel(panel)  ──┤\n"
        "  ├── MoveSelectedPanel() ──┤\n"
        "  ├── StackSelectedWith() ──┤\n"
        "  └── ValidatePanels()   ──┤\n"
        "                              │\n"
        "                    ┌─────────▼────────────┐\n"
        "                    │ BeepDockingDesignerWiring │\n"
        "                    │ (static helper class)      │\n"
        "                    │                            │\n"
        "                    │ CreateComponent(DockPanel) │\n"
        "                    │ IComponentChangeService    │\n"
        "                    │ Designer.cs code-gen       │\n"
        "                    └────────────────────────────┘</code></pre>",
    )
    b += section(
        "examples",
        "Examples",
        "<h3>Using the Docking Manager at Design Time</h3>"
        '<pre><code class="language-csharp">// 1. Drop BeepDockingManager from toolbox onto form\n'
        "//    → Appears in component tray\n"
        "//    → Auto-detects form as HostForm\n"
        '// 2. Right-click → "Add Panel Left"\n'
        "//    → Creates DockPanel with unique Key\n"
        "//    → Appears docked to left edge\n"
        '// 3. Right-click panel → "Dock Right"\n'
        "//    → Panel repositions to right edge\n"
        '// 4. Right-click manager → "Validate Panels"\n'
        "//    → Checks all panels for valid configuration</code></pre>",
    )
    return b


def gen_dt_dockpanel():
    b = '<div class="page-header"><h1>DockPanelDesigner</h1><p class="page-subtitle">The ParentControlDesigner for DockPanel — provides 10 designer verbs, auto-key generation, intelligent move snapping, and stack reordering at design time</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("features", "Key Features"),
            ("verbs", "Designer Verbs"),
            ("behaviors", "Design-Time Behaviors"),
            ("smart-tag", "Smart-Tag"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Designers</code><br>"
        "<strong>Inheritance:</strong> <code>ParentControlDesigner</code> (allows child controls to be dropped onto the panel)<br>"
        "<strong>Visibility:</strong> <code>internal sealed</code></p>"
        "<p><code>DockPanelDesigner</code> provides the design-time experience for individual <strong>DockPanel</strong> controls. It extends <code>ParentControlDesigner</code> so users can drag child controls onto the panel from the toolbox. It provides 10 designer verbs for docking/manageCommands and intelligent snapping when the panel is moved on the form.</p>",
    )
    b += section(
        "features",
        "Key Features",
        '<ul><li><strong>Auto-key generation</strong> — On InitializeNewComponent, a unique Key is derived from the component\'s site name (e.g., "dockPanel1")</li>'
        "<li><strong>Intelligent move snapping</strong> — When dragged, panel snaps to nearest dock edge or stacks with overlapped panels</li>"
        "<li><strong>Float detection</strong> — Dragging outside the host form (>40px boundary) floats the panel</li>"
        "<li><strong>10 designer verbs</strong> — Dock Left/Right/Top/Bottom/Fill, Stack with Prev/Next, Move Earlier/Later in Stack, Remove</li>"
        "<li><strong>Smart-tag</strong> — DockPanelActionList with position, sizing, and behavior properties</li>"
        "<li><strong>Resize forwarding</strong> — Panel resize delegates to BeepDockingDesignerWiring for proportional recalculation</li></ul>",
    )
    b += section(
        "verbs",
        "Designer Verbs (10)",
        "<table><thead><tr><th>Verb</th><th>Method</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Dock &amp;Left</td><td>ApplyDockPosition(Left)</td><td>Move panel to left edge</td></tr>"
        "<tr><td>Dock &amp;Right</td><td>ApplyDockPosition(Right)</td><td>Move panel to right edge</td></tr>"
        "<tr><td>Dock &amp;Top</td><td>ApplyDockPosition(Top)</td><td>Move panel to top edge</td></tr>"
        "<tr><td>Dock &amp;Bottom</td><td>ApplyDockPosition(Bottom)</td><td>Move panel to bottom edge</td></tr>"
        "<tr><td>Dock &amp;Fill</td><td>ApplyDockPosition(Fill)</td><td>Expand to fill remaining space</td></tr>"
        "<tr><td>Stack with Previous Panel</td><td>StackWithPreviousPanel()</td><td>Group with previous sibling as tabs</td></tr>"
        "<tr><td>Stack with Next Panel</td><td>StackWithNextPanel()</td><td>Group with next sibling as tabs</td></tr>"
        "<tr><td>Move Earlier in Stack</td><td>MoveEarlierInStack()</td><td>Reorder to earlier tab position</td></tr>"
        "<tr><td>Move Later in Stack</td><td>MoveLaterInStack()</td><td>Reorder to later tab position</td></tr>"
        "<tr><td>&amp;Remove Panel</td><td>RemovePanel()</td><td>Permanently delete the panel</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "behaviors",
        "Design-Time Behaviors",
        "<h3>Auto-Key on InitializeNewComponent</h3>"
        '<p>When a new DockPanel is created (either from the manager\'s "Add Panel" verb or by drag-dropping), the designer auto-generates a unique Key from the designer host name:</p>'
        '<pre><code class="language-csharp">public override void InitializeNewComponent(IDictionary defaultValues)\n'
        "{\n"
        "    base.InitializeNewComponent(defaultValues);\n"
        '    string key = Component.Site?.Name ?? "dockPanel";\n'
        '    SetPanelProperty("Key", key);\n'
        '    SetPanelProperty("Title", key);\n'
        "}</code></pre>"
        "<h3>Move Snapping</h3>"
        "<p>When the user drags a DockPanel in the designer, the <code>OnDesignerComponentChanged</code> handler detects Bounds/Location/Size changes. For move changes:</p>"
        "<ol><li><strong>FindPanelUnderCenter()</strong> — Checks if another panel is at the same parent level under the moved panel's center. If yes, stacks them.</li>"
        "<li><strong>ShouldFloatPanel()</strong> — Checks if the panel center is >40px outside the host form bounds. If yes, floats the panel.</li>"
        "<li><strong>ResolveNearestDockPosition()</strong> — Calculates the nearest dock edge (Left/Right/Top/Bottom) based on distance from center to host form bounds.</li></ol>"
        "<h3>Resize Forwarding</h3>"
        "<p>Resize changes are forwarded to <code>BeepDockingDesignerWiring.ResizePanel()</code> for proportional recalculation of adjacent panels.</p>"
        "<h3>Selection Tracking</h3>"
        "<p>When a DockPanel becomes the primary selection in the designer, <code>BeepDockingDesignerWiring.ActivateDesignPanel()</code> is called to update the docking manager's active panel state.</p>",
    )
    b += section(
        "smart-tag",
        "DockPanelActionList Smart-Tag",
        "<table><thead><tr><th>Property</th><th>Type</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Key</td><td>string</td><td>Unique panel identifier</td></tr>"
        "<tr><td>Title</td><td>string</td><td>Panel display title (caption)</td></tr>"
        "<tr><td>DockPosition</td><td>DockPosition</td><td>Docked edge</td></tr>"
        "<tr><td>PreferredWidth</td><td>int</td><td>Preferred panel width</td></tr>"
        "<tr><td>PreferredHeight</td><td>int</td><td>Preferred panel height</td></tr>"
        "<tr><td>MinWidth / MinHeight</td><td>int</td><td>Minimum resize dimensions</td></tr>"
        "<tr><td>CanClose / CanFloat / CanAutoHide</td><td>bool</td><td>Capability flags</td></tr>"
        "<tr><td>ShowCaption</td><td>bool</td><td>Show/hide panel caption bar</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "examples",
        "Examples",
        "<h3>Snap-to-Edge Logic</h3>"
        '<pre><code class="language-csharp">private DockPosition ResolveNearestDockPosition()\n'
        "{\n"
        "    var panel = (DockPanel)Component;\n"
        "    var hostBounds = panel.Manager?.HostForm?.Bounds\n"
        "        ?? panel.Parent?.Bounds;\n"
        "    var center = new Point(\n"
        "        panel.Bounds.Left + panel.Bounds.Width / 2,\n"
        "        panel.Bounds.Top + panel.Bounds.Height / 2);\n"
        "\n"
        "    // Calculate distances to each edge\n"
        "    int distLeft = center.X - hostBounds.Left;\n"
        "    int distRight = hostBounds.Right - center.X;\n"
        "    int distTop = center.Y - hostBounds.Top;\n"
        "    int distBottom = hostBounds.Bottom - center.Y;\n"
        "\n"
        "    // Return nearest edge\n"
        "    int min = Math.Min(Math.Min(distLeft, distRight),\n"
        "                       Math.Min(distTop, distBottom));\n"
        "    if (min == distLeft) return DockPosition.Left;\n"
        "    if (min == distRight) return DockPosition.Right;\n"
        "    if (min == distTop) return DockPosition.Top;\n"
        "    return DockPosition.Bottom;\n"
        "}</code></pre>",
    )
    return b


def gen_dt_dockspace():
    b = '<div class="page-header"><h1>BeepDockspaceDesigner</h1><p class="page-subtitle">The ParentControlDesigner for BeepDockspace — enables tab drag-drop reordering, header click routing, and intelligent move snapping at design time</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("features", "Key Features"),
            ("design-time-behavior", "Design-Time Behavior"),
            ("window-messages", "Window Message Handling"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Designers</code><br>"
        "<strong>Inheritance:</strong> <code>ParentControlDesigner</code><br>"
        "<strong>Visibility:</strong> <code>internal sealed</code></p>"
        "<p><code>BeepDockspaceDesigner</code> manages the design-time experience for <strong>BeepDockspace</strong> controls — the visual containers that hold docked panels and render tab strips with captions. At design time, it enables tab reordering via drag-drop within the header area and intelligent repositioning when the entire dockspace is moved.</p>",
    )
    b += section(
        "features",
        "Key Features",
        "<ul><li><strong>Tab drag-drop</strong> — Users can drag tab headers within the dockspace's header bar to reorder panels at design time</li>"
        "<li><strong>Header click routing</strong> — Clicks on the header area at design time are properly routed for tab selection</li>"
        "<li><strong>Move snapping</strong> — When the dockspace is dragged, it recalcalculates the nearest DockPosition from its new center</li>"
        "<li><strong>Resize forwarding</strong> — Resize operations delegate to BeepDockingDesignerWiring for proportional recalculation</li>"
        "<li><strong>Selection rules</strong> — Visible and AllSizeable for proper designer interaction</li></ul>",
    )
    b += section(
        "design-time-behavior",
        "Design-Time Behavior",
        "<h3>GetHitTest</h3>"
        "<p>The <code>GetHitTest()</code> override returns true only when the dockspace is the primary selection AND the click point is within the header area. This allows tab header interactions at design time while letting non-header areas pass through to the contained panels.</p>"
        "<h3>Move Snapping</h3>"
        "<p>When the user moves a dockspace in the designer, <code>HandleDockspaceMove()</code> calculates the nearest DockPosition based on the center point of the dockspace relative to the host form bounds. This is similar to DockPanel's move snapping but operates at the dockspace level.</p>"
        "<h3>ComponentChanged Handling</h3>"
        "<p>Subscribes to <code>ComponentChanged</code> and <code>SelectionChanged</code> events. On move changes, resolves the nearest dock position. On resize changes, forwards to <code>BeepDockingDesignerWiring.ResizeDockspace()</code>.</p>",
    )
    b += section(
        "window-messages",
        "Window Message Handling",
        "<p>The designer overrides <code>WndProc()</code> to intercept three key Windows messages for tab drag-drop:</p>"
        "<table><thead><tr><th>Message</th><th>Behavior</th></tr></thead><tbody>"
        "<tr><td><code>WM_LBUTTONDOWN</code></td><td>Detects if the click is on a tab header. If yes, initiates drag tracking by recording the source panel.</td></tr>"
        "<tr><td><code>WM_MOUSEMOVE</code></td><td>While dragging, tracks mouse position for potential drop target.</td></tr>"
        "<tr><td><code>WM_LBUTTONUP</code></td><td>Completes the drag: calls <code>BeepDockingDesignerWiring.DropPanelAt()</code> to place the panel at its new position.</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// At design time, the user can:\n'
        "// 1. Click a tab header to select the corresponding DockPanel\n"
        "// 2. Drag a tab header to reorder panels within the dockspace\n"
        "// 3. Move the entire dockspace — it snaps to nearest edge\n"
        "// 4. Resize the dockspace — adjacent panels recalculate\n"
        "\n"
        "// At runtime, the dockspace is managed by BeepDockingManager:\n"
        "dockspace.ActivatePanel(myPanel);\n"
        'dockspace.SelectPage("my-panel-key");\n'
        "var activePanel = dockspace.ActivePanel;</code></pre>",
    )
    return b


def gen_dt_documenthost_actionlist():
    b = '<div class="page-header"><h1>DocumentHostActionList</h1><p class="page-subtitle">The richest smart-tag in the Beep library — 40+ properties and 20+ action methods across 18 organized sections for comprehensive design-time document management</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("features", "Key Features"),
            ("sections", "Smart-Tag Sections"),
            ("actions", "Document Actions"),
            ("style-presets", "Style Presets"),
            ("setup-wizard", "Setup Wizard"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists</code><br>"
        "<strong>Inheritance:</strong> <code>DesignerActionList</code></p>"
        "<p><code>DocumentHostActionList</code> is the most comprehensive smart-tag in the Beep controls library. It provides the entire design-time surface for <strong>BeepDocumentHost</strong>, the MDI tabbed document container. With 40+ properties and 20+ action methods across 18 organized sections, it rivals the design-time experience of commercial controls like DevExpress DocumentManager.</p>",
    )
    b += section(
        "features",
        "Key Features",
        "<ul><li><strong>18 organized smart-tag sections</strong> — Status, Documents, Split View, Nested Splits, Tabs, Tab Sizing, Interaction, Style Presets, Appearance, Preview, History, Cross-Host Drag, Policies, Animation, Quick Actions, Session</li>"
        "<li><strong>8 document CRUD actions</strong> — Add, Close Active, Close All, Reopen Last Closed, Float, Pin/Unpin, Split</li>"
        "<li><strong>6 style preset shortcuts</strong> — Chrome, VSCode, Flat, Office, Pill, Underline</li>"
        "<li><strong>LayoutTreeInfo</strong> — Read-only formatted layout tree text for debugging</li>"
        "<li><strong>GroupTabPositions</strong> — Parsable string format for per-group tab position configuration</li>"
        "<li><strong>Setup Wizard</strong> — Quick setup wizard registered as a designer verb</li></ul>",
    )
    b += section(
        "sections",
        "Smart-Tag Sections (18)",
        "<table><thead><tr><th>Section</th><th>Properties</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Status</td><td>Status (read-only) + SetupWizard verb</td><td>Host state + quick setup launcher</td></tr>"
        "<tr><td>Documents</td><td>ActiveDocumentTitle + 8 action methods</td><td>Document CRUD operations</td></tr>"
        "<tr><td>Split View</td><td>MaxGroups, SplitHorizontal, SplitRatio + 4 actions</td><td>Split window management</td></tr>"
        "<tr><td>Nested Splits</td><td>GroupTabPositions, LayoutTreeInfo</td><td>Per-group configuration</td></tr>"
        "<tr><td>Tabs</td><td>TabStyle, TabPosition, CloseMode, ShowAddButton, TabColorMode, KeyboardShortcutsEnabled</td><td>Tab appearance and behavior</td></tr>"
        "<tr><td>Tab Sizing</td><td>TabSizeMode, FixedTabWidth</td><td>Tab width behavior</td></tr>"
        "<tr><td>Interaction</td><td>TabTooltipMode, AllowDragFloat</td><td>User interaction settings</td></tr>"
        "<tr><td>Style Presets</td><td>6 preset methods</td><td>Quick style application</td></tr>"
        "<tr><td>Appearance</td><td>ControlStyle, ThemeName + ChooseTheme()</td><td>Visual configuration</td></tr>"
        "<tr><td>Preview</td><td>TabPreviewEnabled</td><td>Tab hover preview</td></tr>"
        "<tr><td>History</td><td>MaxRecentHistory, MaxClosedHistory</td><td>Session history limits</td></tr>"
        "<tr><td>Cross-Host Drag</td><td>AllowDragBetweenHosts</td><td>Multi-host drag support</td></tr>"
        "<tr><td>Policies</td><td>AllowFloat, AllowSplit, AllowPin, AllowAutoHide, MaxSplitDepth</td><td>Feature gate flags</td></tr>"
        "<tr><td>Animation</td><td>AutoHideHoverDelay</td><td>Auto-hide timing</td></tr>"
        "<tr><td>Session</td><td>AutoSaveLayout, SessionFile, EnableRoutedCommands, EnableTransactionalDocking, EnableHostTelemetry</td><td>Persistence and telemetry</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "actions",
        "Document Actions",
        "<table><thead><tr><th>Method</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>AddNewDocument()</td><td>Adds a placeholder document at design time</td></tr>"
        "<tr><td>CloseActiveDocument()</td><td>Closes the currently active document tab</td></tr>"
        "<tr><td>CloseAllDocuments()</td><td>Closes all open documents</td></tr>"
        "<tr><td>ReopenLastClosed()</td><td>Reopens the most recently closed document</td></tr>"
        "<tr><td>FloatActiveDocument()</td><td>Detaches active document to its own floating window</td></tr>"
        "<tr><td>PinActiveDocument() / UnpinActiveDocument()</td><td>Toggles pinned state</td></tr>"
        "<tr><td>SplitActiveHorizontal() / SplitActiveVertical()</td><td>Creates new split document groups</td></tr>"
        "<tr><td>MergeAllGroups()</td><td>Collapses all split groups into a single tab group</td></tr>"
        "<tr><td>SaveLayoutSnapshot()</td><td>Copies layout JSON to clipboard</td></tr>"
        "<tr><td>ClearAllDocuments()</td><td>Removes all documents, resets host</td></tr>"
        "<tr><td>SetupWizard()</td><td>Opens quick DocumentHost setup wizard (designer verb)</td></tr>"
        "<tr><td>ChooseTheme()</td><td>Opens ThemePickerDialog</td></tr>"
        "<tr><td>OpenLayoutAssistant()</td><td>Opens guided layout assistant dialog</td></tr>"
        "<tr><td>EditDesignTimeDocuments()</td><td>Opens document editor for design-time docs</td></tr>"
        "<tr><td>ApplyLayoutPreset()</td><td>Opens layout preset picker dialog</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "style-presets",
        "Style Presets",
        '<pre><code class="language-csharp">// Six one-click style presets:\n'
        "public void UseChromStyle()     // Google Chrome style tabs\n"
        "public void UseVSCodeStyle()    // Visual Studio Code style\n"
        "public void UseFlatStyle()      // Clean flat design\n"
        "public void UseOfficeStyle()    // Microsoft Office style\n"
        "public void UsePillStyle()      // Rounded pill tabs\n"
        "public void UseUnderlineStyle()  // Underline indicator</code></pre>",
    )
    b += section(
        "setup-wizard",
        "Setup Wizard",
        "<p>The <strong>DocumentSetupWizardDialog</strong> is registered as a designer verb (star icon in the smart-tag). It provides a step-by-step wizard for configuring a new DocumentHost:</p>"
        "<ol><li>Select tab style (Chrome, VSCode, Flat, Office, Pill, Underline)</li>"
        "<li>Choose tab position (Top, Bottom, Left, Right)</li>"
        "<li>Configure initial documents</li>"
        "<li>Set split view preferences</li>"
        "<li>Configure keyboard shortcuts and interaction policies</li></ol>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// At design time, configure via smart-tag.\n'
        "// At runtime, equivalent API:\n"
        "var host = new BeepDocumentHost();\n"
        "\n"
        "// Tab configuration\n"
        "host.TabStyle = TabStyle.Chrome;\n"
        "host.TabPosition = TabPosition.Top;\n"
        "host.CloseMode = CloseMode.All;\n"
        "host.ShowAddButton = true;\n"
        "\n"
        "// Document management\n"
        'host.AddDocument("Main.cs", new RichTextBox());\n'
        'host.AddDocument("App.config", new TextBox());\n'
        "host.CloseAllDocuments();\n"
        "\n"
        "// Split views\n"
        "host.SplitActiveHorizontal();\n"
        "host.MaxGroups = 4;\n"
        "\n"
        "// Layout persistence\n"
        "host.AutoSaveLayout = true;\n"
        'host.SessionFile = "layout.json";\n'
        "var json = host.SaveLayout();\n"
        "host.LoadLayout(json);</code></pre>",
    )
    return b


def gen_dt_common_actionlist():
    b = '<div class="page-header"><h1>CommonBeepControlActionList</h1><p class="page-subtitle">The universal smart-tag applied to every Beep control — provides style, theme, and schema configuration for all controls at design time</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("features", "Key Features"),
            ("properties", "Smart-Tag Properties"),
            ("how-it-works", "How It Works"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists</code><br>"
        "<strong>Inheritance:</strong> <code>DesignerActionList</code></p>"
        "<p><code>CommonBeepControlActionList</code> is the <strong>universal smart-tag</strong> applied to every single Beep control. Both <code>BaseBeepControlDesigner</code> and <code>BaseBeepParentControlDesigner</code> automatically add this action list to the smart-tag panel via their <code>ActionLists</code> property.</p>"
        "<p>It provides the three foundational design-time properties shared by all Beep controls: <strong>Style</strong> (visual preset), <strong>Theme</strong> (theme name), and <strong>Schema</strong> (color scheme).</p>",
    )
    b += section(
        "features",
        "Key Features",
        "<ul><li>Automatically included in every Beep control's smart-tag panel</li>"
        "<li>Three properties: ControlStyle, Theme, Schema</li>"
        "<li>Uses IBeepDesignerActionHost for property access (works with both designer base classes)</li>"
        "<li>Theme property uses ThemeEnumConverter for dropdown selection</li>"
        "<li>Style property uses BeepControlStyle enum for visual preset selection</li></ul>",
    )
    b += section(
        "properties",
        "Smart-Tag Properties",
        "<table><thead><tr><th>Property</th><th>Type</th><th>Description</th><th>Example Values</th></tr></thead><tbody>"
        "<tr><td>ControlStyle</td><td>BeepControlStyle</td><td>Visual style preset for the control</td><td>Material3, Fluent, Cyberpunk, Card, Glass, Minimal</td></tr>"
        "<tr><td>Theme</td><td>string</td><td>Theme name to apply</td><td>DarkTheme, LightTheme, UbuntuTheme, GNOMETheme</td></tr>"
        "<tr><td>Schema</td><td>string</td><td>Color schema selection</td><td>Default, HighContrast, Custom</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "how-it-works",
        "How It Works",
        '<pre><code class="language-csharp">// In BaseBeepControlDesigner:\n'
        "public override DesignerActionListCollection ActionLists\n"
        "{\n"
        "    get\n"
        "    {\n"
        "        var lists = new DesignerActionListCollection\n"
        "        {\n"
        "            new CommonBeepControlActionList(Component)  // Always included\n"
        "        };\n"
        "        lists.AddRange(GetControlSpecificActionLists());  // Control-specific\n"
        "        return lists;\n"
        "    }\n"
        "}\n"
        "\n"
        "// Smart-tag shows:\n"
        "// ┌─────────────────────┐\n"
        "// │ Appearance          │\n"
        "// │   ControlStyle  [v] │\n"
        "// │   Theme          [v]│\n"
        "// │   Schema         [v]│\n"
        "// ├─────────────────────┤\n"
        "// │ [Control-Specific]  │  ← from GetControlSpecificActionLists()\n"
        "// └─────────────────────┘</code></pre>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// The CommonBeepControlActionList ensures every\n'
        "// control in the designer has consistent styling:\n"
        "\n"
        "// BeepButton smart-tag:\n"
        "//   [Common] ControlStyle, Theme, Schema\n"
        "//   [Button] Text, IconPath, ButtonKind, etc.\n"
        "\n"
        "// BeepGridPro smart-tag:\n"
        "//   [Common] ControlStyle, Theme, Schema\n"
        "//   [Grid]   DataSource, RowHeight, GridStyle, etc.\n"
        "\n"
        "// BeepChart smart-tag:\n"
        "//   [Common] ControlStyle, Theme, Schema\n"
        "//   [Chart]  Title, ShowLegend, ShowGrid</code></pre>",
    )
    return b


def gen_dt_wiring():
    b = '<div class="page-header"><h1>BeepDockingDesignerWiring</h1><p class="page-subtitle">The static helper class that routes all docking panel CRUD operations through IComponentChangeService for proper .designer.cs serialization</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("features", "Key Features"),
            ("methods", "Methods"),
            ("architecture", "Architecture"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Infrastructure</code><br>"
        "<strong>Visibility:</strong> <code>internal static</code></p>"
        "<p><code>BeepDockingDesignerWiring</code> is the central nervous system of the docking design-time infrastructure. It is a <strong>static helper class</strong> that provides all panel and dockspace operations with proper <code>IComponentChangeService</code> integration.</p>"
        "<p>Every designer in the docking system (BeepDockingManagerDesigner, DockPanelDesigner, BeepDockspaceDesigner) delegates its panel mutations to this class.</p>",
    )
    b += section(
        "features",
        "Key Features",
        "<ul><li>Centralized panel CRUD with IComponentChangeService integration</li>"
        "<li>Uses IDesignerHost.CreateComponent() for new panel creation (required for serialization)</li>"
        "<li>Panel move, stack, reorder, and resize operations</li>"
        "<li>Dockspace resize and position updates</li>"
        "<li>Tab drag-drop routing (DropPanelAt)</li>"
        "<li>Host form layout refresh after mutations</li>"
        "<li>Handles panel activation tracking at design time</li></ul>",
    )
    b += section(
        "methods",
        "Methods",
        "<table><thead><tr><th>Method</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>AddPanel(BeepDockingManager, DockPosition)</td><td>Creates a new DockPanel component at the given position</td></tr>"
        "<tr><td>RemovePanel(BeepDockingManager, DockPanel)</td><td>Permanently removes a panel from the manager</td></tr>"
        "<tr><td>MovePanel(BeepDockingManager, DockPanel, DockPosition)</td><td>Moves a panel to a different dock position</td></tr>"
        "<tr><td>StackPanel(BeepDockingManager, DockPanel, DockPanel)</td><td>Stacks a panel into the same group as target</td></tr>"
        "<tr><td>ResizePanel(BeepDockingManager, DockPanel, int, int)</td><td>Updates panel preferred size</td></tr>"
        "<tr><td>ResizeDockspace(BeepDockingManager, BeepDockspace, int, int)</td><td>Updates dockspace size</td></tr>"
        "<tr><td>DropPanelAt(BeepDockingManager, DockPanel, BeepDockspace, int)</td><td>Places a panel at a specific index in a dockspace (tab drag-drop)</td></tr>"
        "<tr><td>ActivateDesignPanel(BeepDockingManager, DockPanel)</td><td>Marks a panel as active at design time</td></tr>"
        "<tr><td>RefreshHostLayout(BeepDockingManager)</td><td>Forces the host form to reapply the docking layout</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "architecture",
        "Architecture",
        '<pre><code class="language-plaintext">BeepDockingManagerDesigner ──┐\n'
        "DockPanelDesigner ─────────┤\n"
        "BeepDockspaceDesigner ─────┤\n"
        "                             │\n"
        "              ┌──────────────▼──────────────┐\n"
        "              │ BeepDockingDesignerWiring     │\n"
        "              │ (static helper)               │\n"
        "              │                                │\n"
        "              │ IDesignerHost.CreateComponent  │\n"
        "              │ IComponentChangeService        │\n"
        "              │ TypeDescriptor.GetProperties   │\n"
        "              │ Panel.Key validation            │\n"
        "              │ Layout recalculation             │\n"
        "              └────────────────────────────────┘</code></pre>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// Example: How DockPanelDesigner delegates to Wiring\n'
        "private void ApplyDockPosition(DockPosition position)\n"
        "{\n"
        "    var panel = (DockPanel)Component;\n"
        "    var manager = panel.Manager;\n"
        "    BeepDockingDesignerWiring.MovePanel(manager, panel, position);\n"
        "}\n"
        "\n"
        "// Wiring handles:\n"
        "// 1. IComponentChangeService.OnComponentChanging(panel)\n"
        "// 2. panel.DockPosition = newPosition\n"
        "// 3. manager.RecalculateLayout()\n"
        "// 4. IComponentChangeService.OnComponentChanged(panel)\n"
        "// 5. RefreshHostLayout()</code></pre>",
    )
    return b


def gen_dt_helpers():
    b = '<div class="page-header"><h1>Design-Time Helpers</h1><p class="page-subtitle">Supporting infrastructure classes for the Design.Server project — service management, theme preview, validation, project integration, and resource embedding</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("classes", "Helper Classes"),
            ("design-time-services", "DesignTimeBeepServiceManager"),
            ("theme-preview", "ThemePreviewHelper"),
            ("validation", "ControlValidationHelper"),
            ("project", "Project Helpers"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers</code><br>"
        "<strong>Assembly:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.dll</code></p>"
        "<p>The Helpers namespace contains supporting infrastructure for the design-time experience:</p>",
    )
    b += section(
        "classes",
        "Helper Classes",
        "<table><thead><tr><th>Class</th><th>Type</th><th>Purpose</th></tr></thead><tbody>"
        "<tr><td>DesignTimeBeepServiceManager</td><td>Static</td><td>Manages design-time service resolution and DI container</td></tr>"
        "<tr><td>ThemePreviewHelper</td><td>Static</td><td>Generates theme preview thumbnails for theme picker</td></tr>"
        "<tr><td>ControlValidationHelper</td><td>Static</td><td>Validates control configurations at design time</td></tr>"
        "<tr><td>ControlPresetHelper</td><td>Static</td><td>Applies preset configurations to controls</td></tr>"
        "<tr><td>DesignTimeDataHelper</td><td>Static</td><td>Generates sample data for design-time previews</td></tr>"
        "<tr><td>ProjectHelper</td><td>Static</td><td>Integration with Visual Studio project system</td></tr>"
        "<tr><td>ProjectFileHelper</td><td>Static</td><td>File operations within the VS project</td></tr>"
        "<tr><td>ResxResourceHelper</td><td>Static</td><td>Manages embedded resources in .resx files</td></tr>"
        "<tr><td>ResourceValidationHelper</td><td>Static</td><td>Validates resource references</td></tr>"
        "<tr><td>ProjectResourceEmbedder</td><td>Static</td><td>Embeds files as project resources</td></tr>"
        "<tr><td>FileOperationHelper</td><td>Static</td><td>Safe file operations with VS integration</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "design-time-services",
        "DesignTimeBeepServiceManager",
        "<p>Manages the design-time dependency injection container. When controls need services at design time (e.g., data access for sample data), this class provides mock implementations that work in the Visual Studio process.</p>"
        '<pre><code class="language-csharp">// At design time, controls use mock services\n'
        "var dataService = DesignTimeBeepServiceManager.GetService&lt;IDataService&gt;();\n"
        "// Returns a design-time mock that generates sample data\n"
        "// instead of connecting to a real database</code></pre>",
    )
    b += section(
        "theme-preview",
        "ThemePreviewHelper",
        "<p>Generates small preview thumbnails for the theme picker dialog. Each thumbnail shows a mini rendering of key controls (button, textbox, grid) with the theme applied.</p>",
    )
    b += section(
        "validation",
        "ControlValidationHelper",
        "<p>Validates control configurations at design time, providing warnings and errors in the Visual Studio Error List:</p>"
        "<ul><li>Missing required properties</li><li>Invalid property combinations</li><li>Resource path validation</li><li>Data binding configuration issues</li></ul>",
    )
    b += section(
        "project",
        "Project Helpers",
        "<h3>ProjectHelper</h3>"
        "<p>Provides integration with the Visual Studio project system: resolves project paths, manages assembly references, handles NuGet package detection.</p>"
        "<h3>ResxResourceHelper &amp; ProjectResourceEmbedder</h3>"
        "<p>When a control uses embedded resources (images, icons, SVG files), these helpers manage adding the resources to the project's .resx file and setting appropriate build actions.</p>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// Theme preview generation\n'
        'var preview = ThemePreviewHelper.GeneratePreview("DarkTheme", 120, 80);\n'
        "// Returns a 120x80 Bitmap showing theme preview\n"
        "\n"
        "// Control validation\n"
        "var issues = ControlValidationHelper.Validate(new BeepGridPro());\n"
        "foreach (var issue in issues)\n"
        '    Console.WriteLine($"[{issue.Severity}] {issue.Message}");\n'
        "\n"
        "// Project resource embedding\n"
        'ProjectResourceEmbedder.EmbedResource(project, "icon.svg",\n'
        "    ResourceTypes.SvgImage);</code></pre>",
    )
    return b


def gen_dt_integrated_designers():
    b = '<div class="page-header"><h1>Integrated Control Designers</h1><p class="page-subtitle">Design-time support for BeepBlock, BeepForms, and BeepForms sub-controls — configuration wizards, field editors, and policy management</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("beepblock", "BeepBlockDesigner"),
            ("beepforms", "BeepFormsDesigner"),
            ("beepformshost", "BeepFormsHostDesigner"),
            ("shelf-designers", "Shelf Designers"),
            ("editors", "Integrated Editors"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p>The integrated controls in <strong>TheTechIdea.Beep.Winform.Controls.Integrated</strong> have dedicated designers for their unique design-time needs:</p>"
        "<ul><li><strong>BeepBlock</strong> — Data block configuration wizard, field definition editor</li>"
        "<li><strong>BeepForms</strong> — Form shell configuration</li>"
        "<li><strong>BeepForms sub-controls</strong> — Header, CommandBar, QueryShelf, PersistenceShelf, Toolbar, StatusStrip</li></ul>",
    )
    b += section(
        "beepblock",
        "BeepBlockDesigner",
        "<p>Provides design-time configuration of data blocks:</p>"
        "<ul><li><strong>Setup Wizard</strong> — BeepBlockSetupWizardForm for step-by-step block configuration</li>"
        "<li><strong>Field Definition Editor</strong> — BeepBlockFieldEditorForm for managing field definitions</li>"
        "<li><strong>Policy Editor</strong> — BeepFieldControlTypePolicyEditorForm for field-to-control type mapping</li>"
        "<li><strong>Layout Mode</strong> — BeepBlockFieldControlsLayoutModeHelper for grid/list/card layout selection</li></ul>",
    )
    b += section(
        "beepforms",
        "BeepFormsDesigner",
        "<p>Provides design-time configuration of the BeepForms coordinator:</p>"
        "<ul><li>Form shell style selection</li>"
        "<li>Sub-control positioning (header, toolbar, status strip)</li>"
        "<li>Data connection binding</li></ul>",
    )
    b += section(
        "beepformshost",
        "BeepFormsHostDesigner",
        "<p>Manages the BeepForms host control:</p>"
        "<ul><li>Bootstrapper configuration</li>"
        "<li>Command routing setup</li>"
        "<li>Notification service wiring</li></ul>",
    )
    b += section(
        "shelf-designers",
        "Shelf Designers",
        "<table><thead><tr><th>Designer</th><th>Control</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>BeepFormsHeaderDesigner</td><td>BeepFormsHeader</td><td>Form header with title, icon, navigation</td></tr>"
        "<tr><td>BeepFormsCommandBarDesigner</td><td>BeepFormsCommandBar</td><td>Command bar button configuration</td></tr>"
        "<tr><td>BeepFormsQueryShelfDesigner</td><td>BeepFormsQueryShelf</td><td>Query shelf with caption mode</td></tr>"
        "<tr><td>BeepFormsPersistenceShelfDesigner</td><td>BeepFormsPersistenceShelf</td><td>Persistence shelf buttons</td></tr>"
        "<tr><td>BeepFormsToolbarDesigner</td><td>BeepFormsToolbar</td><td>Toolbar configuration</td></tr>"
        "<tr><td>BeepFormsStatusStripDesigner</td><td>BeepFormsStatusStrip</td><td>Status strip with message severity</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "editors",
        "Integrated Editors",
        "<h3>IntegratedFormsDefinitionEditors</h3>"
        "<p>Custom type editors for BeepForms and BeepBlock definition objects:</p>"
        "<ul><li><strong>BeepFormsDefinition</strong> — Form configuration object editor</li>"
        "<li><strong>BeepBlockDefinition</strong> — Block configuration object editor</li>"
        "<li><strong>BeepBlockEntityDefinition</strong> — Entity definition editor</li></ul>"
        "<h3>Converters</h3>"
        "<ul><li><strong>DataBlockConnectionNameConverter</strong> — Connection name in Properties window</li>"
        "<li><strong>DataBlockEntityConverter</strong> — Entity name picker</li>"
        "<li><strong>DataBlockConverter</strong> — Block reference converter</li>"
        "<li><strong>DataConnectionConverter</strong> — Connection picker</li></ul>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// At design time:\n'
        "// 1. Drop BeepBlock on form\n"
        '// 2. Smart-tag → "Setup Wizard" → configure data source, entity, layout\n'
        '// 3. Smart-tag → "Edit Fields" → add/remove/reorder field definitions\n'
        "// 4. Fields auto-generate as labeled input controls\n"
        "\n"
        "// At runtime:\n"
        "var block = new BeepBlock();\n"
        "block.Definition = new BeepBlockDefinition\n"
        "{\n"
        '    EntityName = "Customer",\n'
        "    LayoutMode = BlockLayoutMode.Grid,\n"
        "    Fields = new List&lt;BeepFieldDefinition&gt;\n"
        "    {\n"
        '        new BeepFieldDefinition { FieldName = "Id", ControlType = "Numeric" },\n'
        '        new BeepFieldDefinition { FieldName = "Name", ControlType = "Text" },\n'
        '        new BeepFieldDefinition { FieldName = "Status", ControlType = "Combo" }\n'
        "    }\n"
        "};</code></pre>",
    )
    return b


def gen_dt_chart():
    b = '<div class="page-header"><h1>BeepChartDesigner</h1><p class="page-subtitle">Design-time designer for BeepChart — exposes Title, ShowLegend, and ShowGrid properties via smart-tag</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("smart-tag", "Smart-Tag"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.Designers</code><br>"
        "<strong>Inheritance:</strong> <code>BaseBeepControlDesigner</code></p>"
        "<p>A minimalist designer that adds chart-specific properties to the smart-tag. Inherits CommonBeepControlActionList for style/theme/schema.</p>",
    )
    b += section(
        "smart-tag",
        "BeepChartActionList Smart-Tag",
        "<table><thead><tr><th>Section</th><th>Property</th><th>Type</th></tr></thead><tbody>"
        "<tr><td>Appearance</td><td>Title</td><td>string</td></tr>"
        "<tr><td>Behavior</td><td>ShowLegend</td><td>bool</td></tr>"
        "<tr><td></td><td>ShowGrid</td><td>bool</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">var chart = new BeepChart();\n'
        'chart.Title = "Sales Overview";\n'
        "chart.ShowLegend = true;\n"
        "chart.ShowGrid = false;</code></pre>",
    )
    return b


def gen_dt_calendar():
    b = '<div class="page-header"><h1>BeepCalendarDesigner</h1><p class="page-subtitle">Design-time designer for BeepCalendar — simplest designer, exposing ShowWeekNumbers and ShowTodayButton</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("smart-tag", "Smart-Tag"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.Designers</code><br>"
        "<strong>Inheritance:</strong> <code>BaseBeepControlDesigner</code></p>"
        "<p>The simplest designer in the library. Adds two calendar-specific properties to the smart-tag panel.</p>",
    )
    b += section(
        "smart-tag",
        "BeepCalendarActionList Smart-Tag",
        "<table><thead><tr><th>Property</th><th>Type</th><th>Default</th></tr></thead><tbody>"
        "<tr><td>ShowWeekNumbers</td><td>bool</td><td>false</td></tr>"
        "<tr><td>ShowTodayButton</td><td>bool</td><td>true</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">var cal = new BeepCalendar();\n'
        "cal.ShowWeekNumbers = true;\n"
        "cal.ShowTodayButton = false;</code></pre>",
    )
    return b


def gen_dt_dock():
    b = '<div class="page-header"><h1>BeepDockDesigner</h1><p class="page-subtitle">Design-time designer for BeepDock — 14 smart-tag properties, 9 style presets, 4 position presets, 3 size presets</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("smart-tag", "Smart-Tag Properties"),
            ("presets", "Style & Position Presets"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.Designers</code><br>"
        "<strong>Inheritance:</strong> <code>BaseBeepControlDesigner</code></p>"
        "<p>One of the richest single-control designers, with 14 properties and 16 quick-configuration actions for the BeepDock control.</p>",
    )
    b += section(
        "smart-tag",
        "Smart-Tag Properties (14)",
        "<table><thead><tr><th>Property</th><th>Type</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>DockStyleType</td><td>DockStyle</td><td>Visual style (Apple, Windows11, Material3, etc.)</td></tr>"
        "<tr><td>ItemSize</td><td>int</td><td>Default item icon size</td></tr>"
        "<tr><td>DockHeight</td><td>int</td><td>Height of the dock bar</td></tr>"
        "<tr><td>ItemSpacing</td><td>int</td><td>Space between items</td></tr>"
        "<tr><td>MaxScale</td><td>float</td><td>Maximum magnification on hover</td></tr>"
        "<tr><td>DockPositionType</td><td>DockPosition</td><td>Dock bar position</td></tr>"
        "<tr><td>DockOrientationType</td><td>DockOrientation</td><td>Orientation (horizontal/vertical)</td></tr>"
        "<tr><td>DockAlignmentType</td><td>DockAlignment</td><td>Alignment within position</td></tr>"
        "<tr><td>IndicatorStyle</td><td>DockIndicatorStyle</td><td>Active item indicator style</td></tr>"
        "<tr><td>AnimationStyle</td><td>DockAnimationStyle</td><td>Hover animation effect</td></tr>"
        "<tr><td>IconMode</td><td>DockIconMode</td><td>Icon display mode</td></tr>"
        "<tr><td>ShowTooltips</td><td>bool</td><td>Show tooltip on hover</td></tr>"
        "<tr><td>ShowBadges</td><td>bool</td><td>Show notification badges</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "presets",
        "Style & Position Presets",
        "<h3>9 Style Presets</h3>"
        "<table><thead><tr><th>Preset</th><th>DockStyle</th><th>Look</th></tr></thead><tbody>"
        "<tr><td>ConfigureAsAppleDock()</td><td>AppleDock</td><td>macOS dock with magnification</td></tr>"
        "<tr><td>ConfigureAsWindows11Dock()</td><td>Windows11Dock</td><td>Win11 taskbar look</td></tr>"
        "<tr><td>ConfigureAsMaterial3Dock()</td><td>Material3Dock</td><td>Google Material Design 3</td></tr>"
        "<tr><td>ConfigureAsGlassmorphismDock()</td><td>GlassmorphismDock</td><td>Frosted glass effect</td></tr>"
        "<tr><td>ConfigureAsiOSDock()</td><td>iOSDock</td><td>iOS-style dock</td></tr>"
        "<tr><td>ConfigureAsMinimalDock()</td><td>MinimalDock</td><td>Clean minimal style</td></tr>"
        "<tr><td>ConfigureAsCyberpunkDock()</td><td>CyberpunkDock</td><td>Neon cyberpunk</td></tr>"
        "<tr><td>ConfigureAsDraculaDock()</td><td>DraculaDock</td><td>Dracula theme</td></tr>"
        "</tbody></table>"
        "<h3>4 Position Presets</h3>"
        "<p>PositionAtBottom(), PositionAtTop(), PositionOnLeft(), PositionOnRight()</p>"
        "<h3>3 Size Presets</h3>"
        "<p>SetStandardItemSize (56px), SetCompactItemSize (48px), SetLargeItemSize (64px)</p>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">var dock = new BeepDock();\n'
        "dock.DockStyleType = DockStyle.Windows11Dock;\n"
        "dock.ItemSize = 56;\n"
        "dock.PositionAtBottom();\n"
        "dock.ShowTooltips = true;\n"
        "// Or use preset:\n"
        "// dock.ConfigureAsMaterial3Dock();\n"
        "// dock.PositionOnLeft();</code></pre>",
    )
    return b


def gen_dt_column_editor():
    b = '<div class="page-header"><h1>BeepGridColumnEditorDialog</h1><p class="page-subtitle">Modal design-time dialog for managing BeepGridPro columns — add, remove, reorder, and configure column types with real-time preview</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("features", "Key Features"),
            ("ui-sections", "UI Sections"),
            ("column-types", "Column Types"),
            ("collection-editor", "BeepGridColumnCollectionEditor"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Design.Server.Editors</code><br>"
        "<strong>Inheritance:</strong> <code>Form</code></p>"
        "<p>Opened from the BeepGridPro smart-tag via the <code>EditColumns()</code> command. Provides full visual management of grid columns at design time.</p>",
    )
    b += section(
        "features",
        "Key Features",
        "<ul><li>Visual column list with drag-to-reorder</li>"
        "<li>Add/remove/duplicate columns</li>"
        "<li>Set column HeaderText, Width, ReadOnly</li>"
        "<li>Assign column type (Text, Numeric, Date, ComboBox, CheckBox, Masked)</li>"
        "<li>Real-time preview of column layout</li>"
        "<li>Collection editor integration for complex types</li></ul>",
    )
    b += section(
        "column-types",
        "Column Types",
        "<table><thead><tr><th>Type</th><th>Editor</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Text</td><td>BeepGridTextEditor</td><td>Free-form text input</td></tr>"
        "<tr><td>Numeric</td><td>BeepGridNumericEditor</td><td>Numeric-only input</td></tr>"
        "<tr><td>Date</td><td>BeepGridDateDropDownEditor</td><td>Date picker dropdown</td></tr>"
        "<tr><td>ComboBox</td><td>BeepGridComboBoxEditor</td><td>Dropdown list selection</td></tr>"
        "<tr><td>CheckBox</td><td>BeepGridCheckBoxEditor</td><td>Boolean toggle</td></tr>"
        "<tr><td>Masked</td><td>BeepGridMaskedEditor</td><td>Input mask (phone, SSN, etc.)</td></tr>"
        "<tr><td>Generic</td><td>BeepGridGenericEditor</td><td>Fallback for unknown types</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "collection-editor",
        "BeepGridColumnCollectionEditor",
        "<p>The <strong>BeepGridColumnCollectionEditor</strong> is a <code>CollectionEditor</code> subclass that provides the standard .NET collection editing UI for grid columns. Features:</p>"
        "<ul><li>Add/remove column items in the collection</li>"
        "<li>Property grid for editing individual column properties</li>"
        "<li>Type filtering for column-specific subtypes</li></ul>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// Open the column editor from code:\n'
        "var editor = new BeepGridColumnEditorDialog(grid.Columns);\n"
        "if (editor.ShowDialog() == DialogResult.OK)\n"
        "{\n"
        "    grid.Columns = editor.Result;\n"
        "    grid.Refresh();\n"
        "}</code></pre>",
    )
    return b


def gen_dt_picker_editors():
    b = '<div class="page-header"><h1>Picker Editors</h1><p class="page-subtitle">Design-time editors for selecting themes, painters, styles, icons, and colors — UI-friendly pickers for the Properties window</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("theme-picker", "ThemePickerDialog"),
            ("icon-picker", "IconPickerDialog"),
            ("color-editor", "ColorPaletteEditor"),
            ("painter-editor", "PainterSelectorEditor"),
            ("style-editor", "StyleSelectorEditor"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p>Picker editors provide <strong>UI-friendly selection interfaces</strong> for complex property types in the Visual Studio Properties window:</p>",
    )
    b += section(
        "theme-picker",
        "ThemePickerDialog",
        "<p>Displays a grid of theme previews with thumbnails (generated by ThemePreviewHelper). Users can browse, preview, and select themes visually rather than typing theme names.</p>"
        "<ul><li>Thumbnail grid with theme names</li><li>Search/filter by theme name</li><li>Preview on hover</li><li>Categories: Light, Dark, High Contrast, Custom</li></ul>",
    )
    b += section(
        "icon-picker",
        "IconPickerDialog",
        "<p>Visual icon browser for selecting icons from the Beep icon catalog (IconsManagement/IconCatalog.cs).</p>"
        "<ul><li>Categorized icon grid (Actions, Navigation, Data, Media, etc.)</li><li>Search by icon name</li><li>Preview at multiple sizes</li><li>Integrates with SvgResourcePathHelper for resource resolution</li></ul>",
    )
    b += section(
        "color-editor",
        "ColorPaletteEditor",
        "<p>Color palette picker with Material Design / Fluent color sets:</p>"
        "<ul><li>Palette presets (Material, Fluent, Tailwind, Custom)</li><li>Shade/variant selection (50-900 scale)</li><li>Custom color creation via color wheel</li><li>Recent colors list</li></ul>",
    )
    b += section(
        "painter-editor",
        "PainterSelectorEditor",
        "<p>Picker for selecting painter implementations (e.g., for ListBox, Grid, Chart). Shows available painters with previews.</p>"
        "<ul><li>Filters to available painters for the control type</li><li>Preview of each painter style</li><li>Category organization</li></ul>",
    )
    b += section(
        "style-editor",
        "StyleSelectorEditor",
        "<p>Picker for BeepControlStyle values. Shows visual examples of each style variant.</p>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// Properties window integration:\n'
        "[Editor(typeof(ThemePickerDialog), typeof(UITypeEditor))]\n"
        "public string Theme { get; set; }\n"
        "\n"
        "[Editor(typeof(IconPickerDialog), typeof(UITypeEditor))]\n"
        "public string IconPath { get; set; }\n"
        "\n"
        "[Editor(typeof(ColorPaletteEditor), typeof(UITypeEditor))]\n"
        "public Color AccentColor { get; set; }</code></pre>",
    )
    return b


# =============================================================================
# PAGE REGISTRY
# =============================================================================

DESIGN_TIME_PAGES = {
    "overview.html": (
        gen_dt_overview,
        "Design-Time Overview",
        "overview.html",
        "Design-Time Infrastructure",
    ),
    "basebeepcontroldesigner.html": (
        gen_dt_baseleaf,
        "BaseBeepControlDesigner",
        "basebeepcontroldesigner.html",
        "Design-Time Infrastructure",
    ),
    "basebeepparentcontroldesigner.html": (
        gen_dt_baseparent,
        "BaseBeepParentControlDesigner",
        "basebeepparentcontroldesigner.html",
        "Design-Time Infrastructure",
    ),
    "beepgridprodesigner.html": (
        gen_dt_gridpro,
        "BeepGridProDesigner",
        "beepgridprodesigner.html",
        "Design-Time Infrastructure",
    ),
    "beepchartdesigner.html": (
        gen_dt_chart,
        "BeepChartDesigner",
        "beepchartdesigner.html",
        "Design-Time Infrastructure",
    ),
    "beepcalendardesigner.html": (
        gen_dt_calendar,
        "BeepCalendarDesigner",
        "beepcalendardesigner.html",
        "Design-Time Infrastructure",
    ),
    "beepdockdesigner.html": (
        gen_dt_dock,
        "BeepDockDesigner",
        "beepdockdesigner.html",
        "Design-Time Infrastructure",
    ),
    "beepdockingmanagerdesigner.html": (
        gen_dt_dockingmgr,
        "BeepDockingManagerDesigner",
        "beepdockingmanagerdesigner.html",
        "Design-Time Infrastructure",
    ),
    "dockpaneldesigner.html": (
        gen_dt_dockpanel,
        "DockPanelDesigner",
        "dockpaneldesigner.html",
        "Design-Time Infrastructure",
    ),
    "beepdockspacedesigner.html": (
        gen_dt_dockspace,
        "BeepDockspaceDesigner",
        "beepdockspacedesigner.html",
        "Design-Time Infrastructure",
    ),
    "beepdocumenthostdesigner.html": (
        gen_dt_documenthost_actionlist,
        "BeepDocumentHostDesigner / DocumentHostActionList",
        "beepdocumenthostdesigner.html",
        "Design-Time Infrastructure",
    ),
    "documenthostactionlist.html": (
        gen_dt_documenthost_actionlist,
        "DocumentHostActionList",
        "documenthostactionlist.html",
        "Design-Time Infrastructure",
    ),
    "commonbeepcontrolactionlist.html": (
        gen_dt_common_actionlist,
        "CommonBeepControlActionList",
        "commonbeepcontrolactionlist.html",
        "Design-Time Infrastructure",
    ),
    "beepdockingdesignerwiring.html": (
        gen_dt_wiring,
        "BeepDockingDesignerWiring",
        "beepdockingdesignerwiring.html",
        "Design-Time Infrastructure",
    ),
    "designtimehelpers.html": (
        gen_dt_helpers,
        "Design-Time Helpers",
        "designtimehelpers.html",
        "Design-Time Infrastructure",
    ),
    "integrateddesigners.html": (
        gen_dt_integrated_designers,
        "Integrated Control Designers",
        "integrateddesigners.html",
        "Design-Time Infrastructure",
    ),
    "beepgridcolumneditordialog.html": (
        gen_dt_column_editor,
        "BeepGridColumnEditorDialog",
        "beepgridcolumneditordialog.html",
        "Design-Time Infrastructure",
    ),
    "pickereditors.html": (
        gen_dt_picker_editors,
        "Picker Editors",
        "pickereditors.html",
        "Design-Time Infrastructure",
    ),
}

# =============================================================================
# BUILD
# =============================================================================


def build():
    sidebar_dt = build_sidebar(make_sidebar_dt(), prefix="")
    sidebar_arch = build_sidebar(make_sidebar_arch(), prefix="")

    dt_dir = os.path.join(BASE, "design-time")
    arch_dir = os.path.join(BASE, "architecture")
    os.makedirs(dt_dir, exist_ok=True)
    os.makedirs(arch_dir, exist_ok=True)

    for filename, (
        gen_func,
        title,
        current_page,
        section_name,
    ) in DESIGN_TIME_PAGES.items():
        body = gen_func()
        bc = '<nav class="breadcrumb-nav"><a href="../index.html">Home</a><span>&rsaquo;</span> <a href="overview.html">Design-Time Infrastructure</a><span>&rsaquo;</span> <span>{}</span></nav>'.format(
            current_page.replace(".html", "").replace("overview", "Overview")
        )
        html = HEAD.format(
            title=title + " - Beep Controls Documentation",
            css_path="../",
            section_name=section_name,
            sidebar_links=sidebar_dt,
        )
        html += bc + body + TAIL
        path = os.path.join(dt_dir, filename)
        with open(path, "w", encoding="utf-8") as f:
            f.write(html)
        print("Wrote:", path)

    print("\nDone! {} design-time pages created.".format(len(DESIGN_TIME_PAGES)))


if __name__ == "__main__":
    build()
