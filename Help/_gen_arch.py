# Architecture documentation generator
# Run: python _gen_arch.py

import os

BASE = r"C:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\Help"

HEAD = """<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>{title}</title>
<link rel="stylesheet" href="../sphinx-style.css">
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/themes/prism-tomorrow.min.css">
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet">
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css">
</head>
<body>
<aside class="sidebar" id="sidebar">
<div style="padding:0 1.5rem;margin-bottom:1rem"><h2 style="font-size:1.1rem;margin:0">Beep Controls</h2><span style="font-size:.75rem;color:var(--meta)">Architecture v1.0.164</span></div>
<nav><ul style="list-style:none;padding:0;font-size:.9rem">
<li style="padding:4px 1.5rem"><a href="../index.html">&#x2190; Main Docs</a></li>
<li style="padding:4px 1.5rem;margin-top:1rem;font-weight:700;color:var(--fg)">Docking</li>
<li style="padding:4px 1.5rem"><a href="docking-overview.html">Docking Architecture</a></li>
<li style="padding:4px 1.5rem"><a href="dockpanel-system.html">DockPanel System</a></li>
<li style="padding:4px 1.5rem"><a href="docklayoutdefinition.html">DockLayoutDefinition</a></li>
<li style="padding:4px 1.5rem"><a href="floatwindow-autohide.html">FloatWindow &amp; AutoHide</a></li>
<li style="padding:4px 1.5rem"><a href="docking-painters.html">Docking Painters</a></li>
<li style="padding:4px 1.5rem"><a href="docking-dragdrop.html">Docking Drag-Drop</a></li>
<li style="padding:4px 1.5rem;margin-top:1rem;font-weight:700;color:var(--fg)">GridX</li>
<li style="padding:4px 1.5rem"><a href="gridx-overview.html">GridX Architecture</a></li>
<li style="padding:4px 1.5rem"><a href="gridx-virtualization.html">Virtualization</a></li>
<li style="padding:4px 1.5rem"><a href="gridx-selection.html">Selection System</a></li>
<li style="padding:4px 1.5rem"><a href="gridx-grouping.html">Grouping Engine</a></li>
<li style="padding:4px 1.5rem"><a href="gridx-export.html">Export Engine</a></li>
<li style="padding:4px 1.5rem"><a href="gridx-editors.html">Grid Editors</a></li>
<li style="padding:4px 1.5rem"><a href="gridx-filtering.html">Grid Filtering</a></li>
<li style="padding:4px 1.5rem;margin-top:1rem;font-weight:700;color:var(--fg)">Chart</li>
<li style="padding:4px 1.5rem"><a href="chart-overview.html">Chart Architecture</a></li>
<li style="padding:4px 1.5rem"><a href="chart-seriespainters.html">Series Painters</a></li>
<li style="padding:4px 1.5rem"><a href="chart-axislegend.html">Axis &amp; Legend</a></li>
<li style="padding:4px 1.5rem"><a href="chart-viewportperf.html">Viewport &amp; Performance</a></li>
<li style="padding:4px 1.5rem;margin-top:1rem;font-weight:700;color:var(--fg)">Calendar</li>
<li style="padding:4px 1.5rem"><a href="calendar-overview.html">Calendar Architecture</a></li>
<li style="padding:4px 1.5rem"><a href="calendar-events.html">Calendar Events</a></li>
<li style="padding:4px 1.5rem"><a href="calendar-painting.html">Calendar Painting</a></li>
<li style="padding:4px 1.5rem"><a href="calendar-interactions.html">Calendar Interactions</a></li>
<li style="padding:4px 1.5rem;margin-top:1rem;font-weight:700;color:var(--fg)">Wizard</li>
<li style="padding:4px 1.5rem"><a href="wizard-overview.html">Wizard Architecture</a></li>
<li style="padding:4px 1.5rem"><a href="wizard-forms.html">Wizard Forms</a></li>
<li style="padding:4px 1.5rem"><a href="wizard-painters.html">Wizard Painters</a></li>
<li style="padding:4px 1.5rem;margin-top:1rem;font-weight:700;color:var(--fg)">Theme System</li>
<li style="padding:4px 1.5rem"><a href="theme-overview.html">Theme Architecture</a></li>
<li style="padding:4px 1.5rem"><a href="theme-types.html">Theme Types</a></li>
<li style="padding:4px 1.5rem"><a href="theme-tokens.html">Theme Tokens</a></li>
<li style="padding:4px 1.5rem;margin-top:1rem;font-weight:700;color:var(--fg)">Docks &amp; Painters</li>
<li style="padding:4px 1.5rem"><a href="beepdock-architecture.html">BeepDock Architecture</a></li>
<li style="padding:4px 1.5rem"><a href="dock-painters.html">Dock Painters (22)</a></li>
<li style="padding:4px 1.5rem"><a href="listbox-painters.html">ListBox Painters (42)</a></li>
<li style="padding:4px 1.5rem"><a href="appbar-painters.html">AppBar Painters (16)</a></li>
<li style="padding:4px 1.5rem"><a href="stepper-painters.html">Stepper Painters (15)</a></li>
<li style="padding:4px 1.5rem"><a href="marquee-painters.html">Marquee Painters (8)</a></li>
<li style="padding:4px 1.5rem;margin-top:1rem;font-weight:700;color:var(--fg)">Menus &amp; Integrated</li>
<li style="padding:4px 1.5rem"><a href="menubar-internals.html">MenuBar Internals</a></li>
<li style="padding:4px 1.5rem"><a href="contextmenu-system.html">ContextMenu System</a></li>
<li style="padding:4px 1.5rem"><a href="dataconnection.html">DataConnection System</a></li>
<li style="padding:4px 1.5rem"><a href="beepforms-contracts.html">BeepForms Contracts</a></li>
</ul></nav>
</aside>
<main class="content"><div class="content-wrapper">
"""

TAIL = """
</div></main>
<script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/components/prism-core.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/plugins/autoloader/prism-autoloader.min.js"></script>
</body></html>"""


def section(id, heading, body):
    return '<section class="section" id="{}"><h2>{}</h2>{}</section>'.format(
        id, heading, body
    )


def toc(items):
    lis = "".join('<li><a href="#{}">{}</a></li>'.format(k, v) for k, v in items)
    return '<div class="toc"><h3>Table of Contents</h3><ul>{}</ul></div>'.format(lis)


def bc(page_name):
    return '<nav class="breadcrumb-nav"><a href="../index.html">Home</a><span>&rsaquo;</span> <a href="docking-overview.html">Architecture</a><span>&rsaquo;</span> <span>{}</span></nav>'.format(
        page_name
    )


def page(title, body):
    h = HEAD.format(title=title + " - Beep Controls Documentation")
    return h + bc(title) + body + TAIL


# ============ PAGE GENERATORS ============


def gen_docking_overview():
    b = '<div class="page-header"><h1>Docking Architecture</h1><p class="page-subtitle">The complete architecture of the Beep docking system — layout tree, panels, groups, positions, floating windows, auto-hide, persistence, and the painter pipeline</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("architecture", "System Architecture"),
            ("core-types", "Core Types"),
            ("layout-tree", "Layout Tree"),
            ("panel-states", "Panel States &amp; Lifecycle"),
            ("painter-pipeline", "Painter Pipeline"),
            ("drag-drop", "Drag-Drop System"),
            ("persistence", "Layout Persistence"),
            ("code-example", "Code Example"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p>The Beep docking system provides a complete Visual-Studio-style docking experience. At its heart is <strong>BeepDockingManager</strong>, a non-visual component that orchestrates a network of <strong>DockPanel</strong> components, <strong>BeepDockspace</strong> controls, <strong>FloatWindow</strong> forms, and <strong>AutoHideSlidePanel</strong> controls.</p>"
        "<p>The system spans ~48 source files organized into subsystems:</p>"
        "<ul><li><strong>Models</strong> — DockPanel, DockGroup, DockLayoutTree, serialization types</li>"
        "<li><strong>Runtime</strong> — FloatWindow, AutoHideSlidePanel, AutoHideStrip, BeepDockspace, splitters</li>"
        "<li><strong>Layout</strong> — DockingLayoutController, LayoutCalculator, LayoutValidator</li>"
        "<li><strong>Drag-Drop</strong> — DockDragController, DockGuideController, DockDragGhost, guide overlay</li>"
        "<li><strong>Painters</strong> — IDockingPainter, caption/splitter/auto-hide renderers, adapter</li>"
        "<li><strong>Animation</strong> — DockAnimator, AnimationTrack, easing functions</li></ul>",
    )
    b += section(
        "architecture",
        "System Architecture",
        '<pre><code class="language-plaintext">BeepDockingManager (IComponent, IDockDragHost)\n'
        "  ├── HostForm (Form)\n"
        "  ├── LayoutTree (DockLayoutTree)\n"
        "  │    └── Root (DockGroup) — tree of nested groups\n"
        '  │         ├── DockGroup (Id="left", Position=Left)\n'
        '  │         │    ├── DockPanel ("solution-explorer")\n'
        '  │         │    └── DockPanel ("properties")\n'
        '  │         ├── DockGroup (Id="main", Position=Fill)\n'
        "  │         │    ├── DockGroup (split child, Horizontal)\n"
        '  │         │    │    ├── DockPanel ("editor1")\n'
        '  │         │    │    └── DockPanel ("editor2")\n'
        '  │         │    └── DockPanel ("output")\n'
        '  │         └── DockGroup (Id="right", Position=Right)\n'
        '  │              └── DockPanel ("toolbox")\n'
        "  ├── LayoutController (DockingLayoutController)\n"
        "  │    ├── LayoutCalculator (ratio-based split math)\n"
        "  │    └── LayoutValidator (post-layout checks)\n"
        "  ├── Dockspaces (BeepDockspace[]) — visual containers\n"
        "  ├── FloatWindows (FloatWindow[]) — floating windows\n"
        "  ├── AutoHideStrips (AutoHideStrip[]) — edge strips\n"
        "  ├── Painter (IDockingPainter) — theme-aware rendering\n"
        "  └── DragController (DockDragController)\n"
        "       ├── DockDragGhost (ghost window during drag)\n"
        "       ├── DockGuideController (diamond overlay)\n"
        "       └── DockTargetResolver (drop target logic)</code></pre>",
    )
    b += section(
        "core-types",
        "Core Types",
        "<table><thead><tr><th>Type</th><th>Base</th><th>Role</th></tr></thead><tbody>"
        "<tr><td>BeepDockingManager</td><td>Component</td><td>Central orchestrator — panel registry, layout, theming, events</td></tr>"
        "<tr><td>DockPanel</td><td>Panel</td><td>Dockable content container — Title, Key, Content, State, Capabilities</td></tr>"
        "<tr><td>DockGroup</td><td>object</td><td>Tree node — contains panels and child groups, split ratios</td></tr>"
        "<tr><td>DockLayoutTree</td><td>object</td><td>In-memory layout — Root group, panel/group registries, ToDefinition()</td></tr>"
        "<tr><td>BeepDockspace</td><td>Panel</td><td>Visual container — tab strip, caption, panel content area</td></tr>"
        "<tr><td>FloatWindow</td><td>Form</td><td>Floating window — borderless, caption drag, edge snap</td></tr>"
        "<tr><td>AutoHideSlidePanel</td><td>Panel</td><td>Slide-out panel — animated reveal from edge strip</td></tr>"
        "<tr><td>AutoHideStrip</td><td>Panel</td><td>Edge strip — tab-like buttons for collapsed panels</td></tr>"
        "<tr><td>DockingLayoutController</td><td>object</td><td>Computes pixel rectangles for every group/dockspace</td></tr>"
        "<tr><td>DockLayoutDefinition</td><td>object</td><td>Serialization format — Groups, Floating, AutoHidden collections</td></tr></tbody></table>",
    )
    b += section(
        "layout-tree",
        "Layout Tree",
        "<p>The layout tree is a hierarchical structure where each <strong>DockGroup</strong> node can contain panels and child groups:</p>"
        "<ul><li><strong>Leaf groups</strong> contain one or more DockPanel (rendered as a BeepDockspace with tabs)</li>"
        "<li><strong>Split groups</strong> contain two or more child groups (rendered side-by-side with splitters)</li>"
        "<li>The <strong>Root</strong> group always has Position=Fill and may contain child groups</li>"
        "<li>Each group has a <strong>SplitRatio</strong> (0.1-0.9, default 0.5) controlling proportional space</li></ul>"
        "<p>The <strong>DockingLayoutController</strong> traverses this tree to compute pixel rectangles, handling min-width/height constraints and splitter positions.</p>",
    )
    b += section(
        "panel-states",
        "Panel States &amp; Lifecycle",
        "<table><thead><tr><th>State</th><th>Description</th><th>Visual</th></tr></thead><tbody>"
        "<tr><td><strong>Docked</strong></td><td>Panel occupies a region of the form (Left/Right/Top/Bottom/Fill)</td><td>Tab in a dockspace with caption bar</td></tr>"
        "<tr><td><strong>Tabbed</strong></td><td>Multiple panels share a dockspace (stacked tabs)</td><td>Tabs with active indicator</td></tr>"
        "<tr><td><strong>Floating</strong></td><td>Panel detached in a separate FloatWindow form</td><td>Borderless window with title bar</td></tr>"
        "<tr><td><strong>Auto-Hidden</strong></td><td>Panel collapsed to an edge strip</td><td>Tab button on edge; hover reveals slide panel</td></tr>"
        "<tr><td><strong>Closed</strong></td><td>Panel in the closed store (can be reopened)</td><td>Not visible</td></tr></tbody></table>"
        "<p>State transitions are managed by BeepDockingManager. Cancelable request events (PageCloseRequest, PageFloatingRequest, etc.) let applications veto transitions.</p>",
    )
    b += section(
        "painter-pipeline",
        "Painter Pipeline",
        "<p>Docking chrome rendering uses a two-layer abstraction:</p>"
        "<ol><li><strong>DockingPainterAdapter</strong> — Bridges BeepTheme tokens to docking-specific colors</li>"
        "<li><strong>DockingRendererSet</strong> — Concrete renderers for each element type</li></ol>"
        "<table><thead><tr><th>Renderer</th><th>Class</th><th>Responsibility</th></tr></thead><tbody>"
        "<tr><td>Caption</td><td>CaptionRenderer</td><td>Draws title bar, tab strip, action buttons (close/float/pin)</td></tr>"
        "<tr><td>Splitter</td><td>SplitterRenderer</td><td>Draws resize handle between docked groups</td></tr>"
        "<tr><td>Auto-Hide</td><td>AutoHideStripRenderer</td><td>Draws edge strip tabs and slide panel chrome</td></tr></tbody></table>",
    )
    b += section(
        "drag-drop",
        "Drag-Drop System",
        "<p>The drag-drop pipeline:</p>"
        "<ol><li><strong>Initiation</strong> — User starts dragging a tab header or caption</li>"
        "<li><strong>Ghost</strong> — DockDragGhost creates a semi-transparent window tracking the mouse</li>"
        "<li><strong>Guide Overlay</strong> — DockingGuideOverlay shows a 5-diamond guide (Left/Right/Top/Bottom/Fill)</li>"
        "<li><strong>Target Resolution</strong> — DockTargetResolver determines valid drop positions</li>"
        "<li><strong>Completion</strong> — Panel is moved to the new position; layout recalculates</li></ol>",
    )
    b += section(
        "persistence",
        "Layout Persistence",
        '<pre><code class="language-csharp">// Save\n'
        "var def = manager.LayoutTree.ToDefinition();\n"
        "string json = JsonConvert.SerializeObject(def);\n"
        "\n"
        "// Restore\n"
        "var def = JsonConvert.DeserializeObject&lt;DockLayoutDefinition&gt;(json);\n"
        "manager.LoadLayout(def);</code></pre>"
        "<p>The <strong>DockLayoutDefinition</strong> contains three serializable collections:</p>"
        "<ul><li><strong>Groups</strong> — List of DockGroupDefinition (position, orientation, split ratio, panel keys)</li>"
        "<li><strong>Floating</strong> — List of FloatingPanelInfo (key, bounds, last dock position)</li>"
        "<li><strong>AutoHidden</strong> — List of AutoHiddenPanelInfo (key, edge)</li></ul>",
    )
    b += section(
        "code-example",
        "Code Example",
        '<pre><code class="language-csharp">var mgr = new BeepDockingManager();\n'
        "mgr.ManageControl(this);\n"
        "\n"
        'mgr.AddPanel("explorer", "Solution Explorer",\n'
        "    DockPosition.Right, new TreeView());\n"
        'mgr.AddPanel("editor", "Main.cs",\n'
        "    DockPosition.Fill, new RichTextBox());\n"
        'mgr.AddPanel("output", "Output",\n'
        "    DockPosition.Bottom, new TextBox { Multiline = true });\n"
        "\n"
        "// Float and re-dock\n"
        'mgr.FloatPanel("output",\n'
        "    new Rectangle(100, 100, 600, 400));\n"
        'mgr.DockFloatingPanel("output",\n'
        "    DockPosition.Right);\n"
        "\n"
        "// Auto-hide\n"
        'mgr.AutoHidePanel("explorer");\n'
        'mgr.RestoreAutoHiddenPanel("explorer");</code></pre>',
    )
    return b


def gen_dockpanel_system():
    b = '<div class="page-header"><h1>DockPanel System</h1><p class="page-subtitle">DockPanel, DockGroup, and DockLayoutTree — the core data model powering the Beep docking system</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("dockpanel", "DockPanel"),
            ("dockgroup", "DockGroup"),
            ("layouttree", "DockLayoutTree"),
            ("relationships", "Relationships"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p>The docking data model consists of three core types that work together to represent the full docking state:</p>"
        "<ul><li><strong>DockPanel</strong> — The atomic unit: a single dockable content window</li>"
        "<li><strong>DockGroup</strong> — A tree node organizing panels and nested groups</li>"
        "<li><strong>DockLayoutTree</strong> — The root container owning the full hierarchy</li></ul>",
    )
    b += section(
        "dockpanel",
        "DockPanel",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Docking.Models</code><br>"
        "<strong>Inheritance:</strong> <code>System.Windows.Forms.Panel</code></p>"
        "<h3>Key Properties</h3>"
        "<table><thead><tr><th>Property</th><th>Type</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Key</td><td>string</td><td>Unique identifier (required)</td></tr>"
        '<tr><td>Title</td><td>string</td><td>Display title in caption/tab. Default "Tool Window"</td></tr>'
        "<tr><td>Content</td><td>Control</td><td>Hosted user control. Set to fill panel</td></tr>"
        "<tr><td>State</td><td>DockPanelState</td><td>Current display state (Docked/Float/AutoHidden/Hidden/Closed)</td></tr>"
        "<tr><td>DockPosition</td><td>DockPosition</td><td>Docked edge (Left/Right/Top/Bottom/Fill)</td></tr>"
        "<tr><td>Group</td><td>DockGroup</td><td>Owning group. Set internally by manager</td></tr>"
        "<tr><td>CanClose / CanFloat / CanAutoHide</td><td>bool</td><td>Capability flags controlling allowed operations</td></tr>"
        "<tr><td>AllowedAreas</td><td>DockAreas</td><td>Bitmask of allowed dock positions</td></tr>"
        "<tr><td>IsDirty</td><td>bool</td><td>Unsaved changes indicator (asterisk in tab)</td></tr>"
        "<tr><td>ShowCaption</td><td>bool</td><td>Draw own caption bar. True when floating</td></tr>"
        "<tr><td>PreferredWidth / MinWidth</td><td>int</td><td>Sizing constraints</td></tr></tbody></table>"
        "<h3>Key Events</h3>"
        "<ul><li><code>Activated / Deactivated</code> — Panel selection changed</li>"
        "<li><code>Closed</code> — Panel moved to closed store</li>"
        "<li><code>PropertyChanged</code> — Panel property changed</li></ul>",
    )
    b += section(
        "dockgroup",
        "DockGroup",
        "<p><strong>Inheritance:</strong> <code>object</code> (not a control — pure data model)</p>"
        "<h3>Key Properties</h3>"
        "<table><thead><tr><th>Property</th><th>Type</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Id</td><td>string</td><td>Unique GUID identifier (default: Guid.NewGuid())</td></tr>"
        "<tr><td>Position</td><td>DockPosition</td><td>Docked edge</td></tr>"
        "<tr><td>SplitOrientation</td><td>SplitOrientation</td><td>Horizontal or Vertical split direction for child groups</td></tr>"
        "<tr><td>SplitRatio</td><td>float</td><td>Proportional split (0.1-0.9, default 0.5)</td></tr>"
        "<tr><td>Parent</td><td>DockGroup</td><td>Parent group (null for root)</td></tr>"
        "<tr><td>Children</td><td>IReadOnlyList&lt;DockGroup&gt;</td><td>Child groups (for split layouts)</td></tr>"
        "<tr><td>Panels</td><td>IReadOnlyList&lt;DockPanel&gt;</td><td>Panels in this group</td></tr>"
        "<tr><td>ActivePanel</td><td>DockPanel</td><td>Currently active panel. Setter fires events</td></tr>"
        "<tr><td>Bounds</td><td>Rectangle</td><td>Cached client rectangle (set internally)</td></tr></tbody></table>"
        "<h3>Key Methods</h3>"
        "<ul><li><code>GetAllPanelsRecursive()</code> — Flat list of all descendant panels</li>"
        "<li><code>FindPanelRecursive(string key)</code> — Deep search by panel key</li>"
        "<li><code>GetPanelIndex(DockPanel)</code> — Position within Panels list</li>"
        "<li><code>ToString()</code> — Diagnostic string showing structure</li></ul>",
    )
    b += section(
        "layouttree",
        "DockLayoutTree",
        "<p><strong>Inheritance:</strong> <code>object</code></p>"
        "<h3>Key Properties</h3>"
        "<table><thead><tr><th>Property</th><th>Type</th><th>Description</th></tr></thead><tbody>"
        '<tr><td>Root</td><td>DockGroup</td><td>Root group (auto-creates on first get). Id="root", Position=Fill</td></tr>'
        "<tr><td>SchemaVersion</td><td>int</td><td>Layout format version. Default 1</td></tr>"
        '<tr><td>Name</td><td>string</td><td>Layout name. Default "Default"</td></tr>'
        "<tr><td>CreatedUtc / ModifiedUtc</td><td>DateTime</td><td>Timestamps</td></tr></tbody></table>"
        "<h3>Key Methods</h3>"
        "<ul><li><code>RegisterPanel / UnregisterPanel</code> — Panel registry management</li>"
        "<li><code>RegisterGroup / UnregisterGroup</code> — Group registry management</li>"
        "<li><code>GetRootGroupsByPosition()</code> — Root-level groups per DockPosition</li>"
        "<li><code>GetDiagnostics()</code> — Summary string of entire layout</li>"
        "<li><code>Clear()</code> — Resets all registries and root</li></ul>",
    )
    b += section(
        "relationships",
        "Relationships",
        '<pre><code class="language-plaintext">DockLayoutTree\n'
        '  └── DockGroup (Root, Id="root", Position=Fill)\n'
        "       ├── DockGroup (Position=Left)\n"
        '       │    ├── DockPanel (Key="explorer", Title="Explorer")\n'
        '       │    └── DockPanel (Key="toolbox", Title="Toolbox")\n'
        "       ├── DockGroup (Position=Fill, SplitOrientation=Horizontal)\n"
        "       │    ├── DockGroup (child, no position)\n"
        '       │    │    └── DockPanel (Key="editor1")\n'
        "       │    └── DockGroup (child, no position)\n"
        '       │         └── DockPanel (Key="editor2")\n'
        "       └── DockGroup (Position=Bottom)\n"
        '            └── DockPanel (Key="output")</code></pre>',
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// Navigate the tree\n'
        "var root = manager.LayoutTree.Root;\n"
        "foreach (var posGroup in root.Children)\n"
        "{\n"
        '    Console.WriteLine($"Group {posGroup.Id}: {posGroup.Position}");\n'
        "    foreach (var panel in posGroup.GetAllPanelsRecursive())\n"
        '        Console.WriteLine($"  Panel: {panel.Key} = {panel.Title}");\n'
        "}\n"
        "\n"
        "// Find specific panel\n"
        'var explorer = manager.LayoutTree.FindPanel("explorer");\n'
        "var ownerGroup = explorer.Group;  // owning DockGroup\n"
        "var siblings = ownerGroup.Panels;  // panels in same group</code></pre>",
    )
    return b


def gen_docklayout_definition():
    b = '<div class="page-header"><h1>DockLayoutDefinition</h1><p class="page-subtitle">Serialization format for Beep docking layouts — groups, floating panels, auto-hidden panels, and their properties preserved across save/restore</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("classes", "Serialization Classes"),
            ("format", "JSON Format"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Docking.Models</code></p>"
        "<p><code>DockLayoutDefinition</code> is the root serializable object for persisting docking layouts. It contains three collections that capture the complete state:</p>",
    )
    b += section(
        "classes",
        "Serialization Classes",
        "<h3>DockLayoutDefinition (root)</h3>"
        "<table><thead><tr><th>Property</th><th>Type</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>SchemaVersion</td><td>int</td><td>Layout format version. Default 1</td></tr>"
        "<tr><td>Groups</td><td>List&lt;DockGroupDefinition&gt;</td><td>Top-level docked groups (content-serialized)</td></tr>"
        "<tr><td>Floating</td><td>List&lt;FloatingPanelInfo&gt;</td><td>Floating panel positions</td></tr>"
        "<tr><td>AutoHidden</td><td>List&lt;AutoHiddenPanelInfo&gt;</td><td>Auto-hidden panel edges</td></tr>"
        "<tr><td>IsEmpty</td><td>bool</td><td>True when all collections are null/empty</td></tr></tbody></table>"
        "<h3>DockGroupDefinition</h3>"
        "<table><thead><tr><th>Property</th><th>Type</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Position</td><td>DockPosition</td><td>Docked edge. Default Left</td></tr>"
        "<tr><td>SplitOrientation</td><td>SplitOrientation</td><td>Horizontal or Vertical. Default Horizontal</td></tr>"
        "<tr><td>SplitRatio</td><td>float</td><td>Proportional split. Default 0.5</td></tr>"
        "<tr><td>PanelKeys</td><td>List&lt;string&gt;</td><td>Panel keys in this group (content-serialized)</td></tr>"
        "<tr><td>ActivePanelKey</td><td>string</td><td>Key of active panel. Default null</td></tr>"
        "<tr><td>Children</td><td>List&lt;DockGroupDefinition&gt;</td><td>Nested split children (content-serialized)</td></tr></tbody></table>"
        "<h3>FloatingPanelInfo</h3>"
        "<table><thead><tr><th>Property</th><th>Type</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Key</td><td>string</td><td>Panel identifier</td></tr>"
        "<tr><td>Bounds</td><td>Rectangle</td><td>Screen position and size</td></tr>"
        "<tr><td>LastDockPosition</td><td>DockPosition</td><td>Position before floating. Default Left</td></tr></tbody></table>"
        "<h3>AutoHiddenPanelInfo</h3>"
        "<table><thead><tr><th>Property</th><th>Type</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>Key</td><td>string</td><td>Panel identifier</td></tr>"
        "<tr><td>Edge</td><td>DockPosition</td><td>Auto-hide edge. Default Left</td></tr></tbody></table>",
    )
    b += section(
        "format",
        "JSON Format",
        '<pre><code class="language-json">{\n'
        '  "SchemaVersion": 1,\n'
        '  "Groups": [\n'
        "    {\n"
        '      "Position": "Left",\n'
        '      "SplitOrientation": "Horizontal",\n'
        '      "SplitRatio": 0.3,\n'
        '      "PanelKeys": ["explorer", "toolbox"],\n'
        '      "ActivePanelKey": "explorer",\n'
        '      "Children": []\n'
        "    },\n"
        "    {\n"
        '      "Position": "Fill",\n'
        '      "SplitOrientation": "Vertical",\n'
        '      "SplitRatio": 0.6,\n'
        '      "PanelKeys": [],\n'
        '      "ActivePanelKey": null,\n'
        '      "Children": [\n'
        "        {\n"
        '          "PanelKeys": ["editor1"],\n'
        '          "ActivePanelKey": "editor1"\n'
        "        },\n"
        "        {\n"
        '          "PanelKeys": ["editor2"],\n'
        '          "ActivePanelKey": "editor2"\n'
        "        }\n"
        "      ]\n"
        "    }\n"
        "  ],\n"
        '  "Floating": [\n'
        "    {\n"
        '      "Key": "output",\n'
        '      "Bounds": "100,100,600,400",\n'
        '      "LastDockPosition": "Bottom"\n'
        "    }\n"
        "  ],\n"
        '  "AutoHidden": [\n'
        "    {\n"
        '      "Key": "properties",\n'
        '      "Edge": "Right"\n'
        "    }\n"
        "  ]\n"
        "}</code></pre>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// Save to file\n'
        "var def = manager.LayoutTree.ToDefinition();\n"
        "string json = JsonConvert.SerializeObject(def, Formatting.Indented);\n"
        'File.WriteAllText("layout.json", json);\n'
        "\n"
        "// Restore from file\n"
        'string json = File.ReadAllText("layout.json");\n'
        "var def = JsonConvert.DeserializeObject&lt;DockLayoutDefinition&gt;(json);\n"
        "manager.MaterializeFromDefinition(def);\n"
        "\n"
        "// Check if layout is empty\n"
        "if (def.IsEmpty)\n"
        '    Console.WriteLine("No layout to restore");</code></pre>',
    )
    return b


def gen_floatwindow_autohide():
    b = '<div class="page-header"><h1>FloatWindow &amp; AutoHideSlidePanel</h1><p class="page-subtitle">Runtime components for floating windows and auto-hide slide panels — borderless forms, edge snapping, animated reveal, and separator resizing</p></div>\n'
    b += toc(
        [
            ("floatwindow", "FloatWindow"),
            ("autohide", "AutoHideSlidePanel"),
            ("autohidestrip", "AutoHideStrip"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "floatwindow",
        "FloatWindow",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Docking.Runtime</code><br>"
        "<strong>Inheritance:</strong> <code>System.Windows.Forms.Form</code></p>"
        "<p><code>FloatWindow</code> is a borderless themed form that hosts a floating <strong>DockPanel</strong>:</p>"
        "<ul><li><strong>Caption drag</strong> — Uses <code>WM_NCLBUTTONDOWN</code> with <code>ReleaseCapture</code> + <code>SendMessage</code> for native smooth movement</li>"
        "<li><strong>Resize</strong> — Handles <code>WM_NCHITTEST</code> to provide border resize cursors</li>"
        "<li><strong>Edge snapping</strong> — Snaps to edges of the owner form</li>"
        "<li><strong>Close/double-click</strong> — Close button or double-click caption triggers re-dock via <code>PanelRedocked</code> event</li>"
        "<li><strong>ExtractHostedPanel()</strong> — Removes the hosted panel without disposing it for re-docking</li></ul>"
        "<h3>Key Events</h3>"
        "<ul><li><code>PanelRedocked</code> — Raised when user re-docks the panel</li>"
        "<li><code>MoveOperationEnded</code> — Raised after native move/resize completes</li></ul>",
    )
    b += section(
        "autohide",
        "AutoHideSlidePanel",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Docking.Runtime</code><br>"
        "<strong>Inheritance:</strong> <code>System.Windows.Forms.Panel</code></p>"
        "<p>Reveals auto-hidden panel content with animation:</p>"
        "<ul><li><strong>Show(DockPanel)</strong> — Starts slide-in animation (~100 ms, 10 ms ticks)</li>"
        "<li><strong>Hide()</strong> — Starts slide-out animation</li>"
        "<li><strong>Slide direction</strong> — Determined by <code>Edge</code> property (Left/Right/Top/Bottom)</li>"
        "<li><strong>Separator grip</strong> — 5px resizable grip on the inner edge, minimum 80px extent</li>"
        "<li><strong>SeparatorResize event</strong> — Raised on drag completion with new dimensions</li></ul>"
        "<h3>Animation Timing</h3>"
        '<pre><code class="language-csharp">// Slide animation: ~100ms total, 10ms ticks\n'
        "int duration = 100;\n"
        "int steps = duration / 10;  // 10 steps\n"
        "// Each tick moves the panel proportionally\n"
        "// Uses simple linear interpolation</code></pre>",
    )
    b += section(
        "autohidestrip",
        "AutoHideStrip",
        "<p><strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Docking.Runtime</code><br>"
        "<strong>Inheritance:</strong> <code>System.Windows.Forms.Panel</code></p>"
        "<p>A slim strip along a form edge showing tab-like buttons for auto-hidden panels:</p>"
        "<ul><li>Each auto-hidden panel gets a button showing its title</li>"
        "<li>Hovering a button triggers the <code>AutoHideSlidePanel</code> to slide out</li>"
        "<li>Clicking activates the panel</li>"
        "<li>Pin button in the slide panel header restores the panel to docked state</li></ul>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// Float a panel\n'
        'manager.FloatPanel("properties",\n'
        "    new Rectangle(100, 100, 300, 500));\n"
        "// Panel appears in its own FloatWindow\n"
        "\n"
        "// Auto-hide a panel\n"
        'manager.AutoHidePanel("toolbox");\n'
        "// Panel collapses to edge strip\n"
        "// Hover to reveal slide panel\n"
        "\n"
        "// Restore auto-hidden panel\n"
        'manager.RestoreAutoHiddenPanel("toolbox");\n'
        "// Panel returns to previous docked position\n"
        "\n"
        "// Cancel float before it happens\n"
        "manager.PageFloatingRequest += (s, e) =>\n"
        "{\n"
        '    if (e.PanelKey == "locked-panel")\n'
        "        e.Cancel = true;\n"
        "};</code></pre>",
    )
    return b


def gen_docking_painters():
    b = '<div class="page-header"><h1>Docking Painters</h1><p class="page-subtitle">The theme-aware painting pipeline for docking chrome — captions, tab strips, splitters, auto-hide strips rendered through IDockingPainter and specialized renderers</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("interfaces", "Painter Interfaces"),
            ("factory", "DockingPainterFactory"),
            ("renderers", "Specialized Renderers"),
            ("adapter", "DockingPainterAdapter"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p>The docking painting pipeline follows a bridge/adapter pattern to decouple Beep theme tokens from docking-specific rendering:</p>"
        "<ul><li><strong>IDockingPainter</strong> — Top-level interface for the painting contract</li>"
        "<li><strong>DockingPainterAdapter</strong> — Bridges BeepTheme tokens to color values</li>"
        "<li><strong>DockingRendererSet</strong> — Set of specialized renderers for each UI element</li>"
        "<li><strong>DockingPainterFactory</strong> — Creates painters based on style/theme</li></ul>",
    )
    b += section(
        "interfaces",
        "Painter Interfaces",
        "<h3>IDockingPainter</h3>"
        '<pre><code class="language-csharp">internal interface IDockingPainter\n'
        "{\n"
        "    void Initialize(IBeepTheme theme);\n"
        "    void PaintDockspace(Graphics g, BeepDockspace dockspace, Rectangle bounds);\n"
        "    void PaintCaption(Graphics g, Rectangle bounds, string title,\n"
        "        bool isActive, DockPanelState state);\n"
        "    void PaintTab(Graphics g, Rectangle bounds, string title,\n"
        "        bool isActive, bool isHovered, bool isDirty, string iconPath);\n"
        "    void PaintSplitter(Graphics g, Rectangle bounds,\n"
        "        SplitOrientation orientation);\n"
        "    void PaintAutoHideStrip(Graphics g, AutoHideStrip strip,\n"
        "        Rectangle bounds);\n"
        "}</code></pre>"
        "<h3>IDockingElementRenderer</h3>"
        '<pre><code class="language-csharp">internal interface IDockingElementRenderer\n'
        "{\n"
        "    void Render(Graphics g, DockingPainterContext ctx);\n"
        "}</code></pre>"
        "<p>Specialized implementations: <code>CaptionRenderer</code>, <code>SplitterRenderer</code>, <code>AutoHideStripRenderer</code>.</p>",
    )
    b += section(
        "factory",
        "DockingPainterFactory",
        '<pre><code class="language-csharp">internal static class DockingPainterFactory\n'
        "{\n"
        "    public static IDockingPainter Create(\n"
        "        BeepControlStyle style, IBeepTheme theme);\n"
        "}</code></pre>"
        "<p>The factory resolves the style to a concrete implementation. <code>NullDockingPainter</code> is the no-op default.</p>",
    )
    b += section(
        "renderers",
        "Specialized Renderers",
        "<table><thead><tr><th>Class</th><th>Location</th><th>Renders</th></tr></thead><tbody>"
        "<tr><td>CaptionRenderer</td><td>Painters/Caption/</td><td>Title bar, tab strip, action buttons (close, float, pin, dropdown)</td></tr>"
        "<tr><td>SplitterRenderer</td><td>Painters/Splitter/</td><td>Resize grips between docked groups</td></tr>"
        "<tr><td>AutoHideStripRenderer</td><td>Painters/AutoHide/</td><td>Edge strip tabs and hover effects</td></tr></tbody></table>",
    )
    b += section(
        "adapter",
        "DockingPainterAdapter",
        '<pre><code class="language-csharp">internal class DockingPainterAdapter : IDockingPainter\n'
        "{\n"
        "    private IBeepTheme _theme;\n"
        "    private DockingRendererSet _renderers;\n"
        "\n"
        "    // Bridges theme tokens:\n"
        "    //   _theme.TabActiveBackColor  → caption active background\n"
        "    //   _theme.TabInactiveBackColor → caption inactive background\n"
        "    //   _theme.TabBorderColor       → splitter color\n"
        "    //   _theme.PanelBackColor       → auto-hide strip background\n"
        "}\n"
        "\n"
        "internal class DockingRendererSet\n"
        "{\n"
        "    public CaptionRenderer Caption { get; }\n"
        "    public SplitterRenderer Splitter { get; }\n"
        "    public AutoHideStripRenderer AutoHide { get; }\n"
        "}</code></pre>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// Apply theme globally\n'
        "manager.ApplyTheme(myTheme);\n"
        "// → DockingPainterFactory.Create(style, theme)\n"
        "// → Adapter bridges theme tokens to renderers\n"
        "// → All dockspace surfaces repaint\n"
        "\n"
        "// Apply explicit colors (bypass theme system)\n"
        "manager.ApplyTheme(\n"
        "    Color.FromArgb(30, 30, 30),  // background\n"
        "    Color.White,                  // foreground\n"
        "    Color.FromArgb(60, 60, 60),  // border\n"
        "    Color.FromArgb(80, 80, 80),  // hover\n"
        "    Color.DodgerBlue              // accent\n"
        ");</code></pre>",
    )
    return b


def gen_docking_dragdrop():
    b = '<div class="page-header"><h1>Docking Drag-Drop System</h1><p class="page-subtitle">The drag-and-drop pipeline for Beep docking — ghost windows, diamond guide overlay, target resolution, and drag session management</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("pipeline", "Drag Pipeline"),
            ("ghost", "DockDragGhost"),
            ("guides", "DockingGuideOverlay"),
            ("controller", "DockDragController"),
            ("target", "DockTargetResolver"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p>The drag-drop subsystem provides the Visual-Studio-like docking experience when users drag panels or tab headers. It consists of 7 classes in the <strong>Runtime/DragDrop/</strong> directory:</p>",
    )
    b += section(
        "pipeline",
        "Drag Pipeline",
        "<ol><li><strong>Initiation</strong> — User starts dragging a tab header or panel caption</li>"
        "<li><strong>DockDragSession</strong> created — tracks source panel, start position, current state</li>"
        "<li><strong>DockDragGhost</strong> shown — semi-transparent window following the mouse</li>"
        "<li><strong>DockingGuideOverlay</strong> shown — 5-diamond guide overlay on the host form</li>"
        "<li><strong>DockTargetResolver</strong> evaluates valid drop positions</li>"
        "<li><strong>DockGuideController</strong> manages guide diamond highlighting</li>"
        "<li><strong>Completion</strong> — Panel moved to resolved position; layout recalculated</li></ol>",
    )
    b += section(
        "ghost",
        "DockDragGhost",
        "<p>A <strong>Form</strong> subclass rendered as a semi-transparent rectangle during drag. Size matches the source panel/tab. Color is theme-aware.</p>",
    )
    b += section(
        "guides",
        "DockingGuideOverlay",
        "<p>A transparent always-on-top <strong>Form</strong> centered on the host form showing 5 directional diamonds:</p>"
        "<ul><li><strong>Left/Right/Top/Bottom</strong> — Dock to that edge</li>"
        "<li><strong>Center (Fill)</strong> — Dock to fill remaining space</li>"
        "<li>Hovered diamond is highlighted in blue</li>"
        "<li>Directional SVG icons from <code>SvgsUIcons</code> rendered inside each diamond</li>"
        "<li><strong>ActiveTarget</strong> property exposes the currently hovered <code>DockPosition?</code></li></ul>",
    )
    b += section(
        "controller",
        "DockDragController",
        "<p>Orchestrates the complete drag operation:</p>"
        "<ul><li>Starts/stops the drag session</li>"
        "<li>Updates ghost position on mouse move</li>"
        "<li>Shows/hides the guide overlay</li>"
        "<li>Resolves drop targets</li>"
        "<li>Commits the panel repositioning</li>"
        "<li>Fires <code>DoDragDropEnd</code> / <code>DoDragDropQuit</code> events on the manager</li></ul>",
    )
    b += section(
        "target",
        "DockTargetResolver",
        "<p>Determines valid drop positions based on:</p>"
        "<ul><li>Current mouse position relative to host form and dockspaces</li>"
        "<li>Panel's <code>AllowedAreas</code> (bitmask gate)</li>"
        "<li>Panel's capability flags (<code>CanFloat</code>, <code>CanAutoHide</code>)</li>"
        "<li>Current layout constraints (max groups, max split depth)</li>"
        "<li>Tab strip hit testing for in-place reordering</li></ul>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// Subscribe to drag events\n'
        "manager.DoDragDropEnd += (s, e) =>\n"
        '    Console.WriteLine("Drag completed");\n'
        "\n"
        "manager.DoDragDropQuit += (s, e) =>\n"
        '    Console.WriteLine("Drag cancelled (Escape)");\n'
        "\n"
        "// During drag, guide overlay shows 5 diamonds:\n"
        "//   ┌───┐\n"
        "//   │ &#x25B2; │   &larr; Top\n"
        "//   ├───┤\n"
        "//   │&#x25C0; &#x25C9; &#x25B6;│  &#x25C0;Left  &#x25C9;Fill  &#x25B6;Right\n"
        "//   ├───┤\n"
        "//   │ &#x25BC; │   &rarr; Bottom\n"
        "//   └───┘</code></pre>",
    )
    return b


def gen_gridx_overview():
    b = '<div class="page-header"><h1>GridX Architecture</h1><p class="page-subtitle">BeepGridPro internal architecture — helper decomposition, virtualization, selection strategies, grouping, filtering, and export pipeline</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("helpers", "Helper Decomposition"),
            ("data-flow", "Data Flow"),
            ("painting", "Painting Pipeline"),
            ("features", "Advanced Features"),
            ("code", "Code Example"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><code>BeepGridPro</code> is the flagship data grid control, organized across multiple partial files. Internally, it decomposes functionality into 18 specialized helper classes, each handling a specific concern:</p>",
    )
    b += section(
        "helpers",
        "Helper Decomposition (18 Classes)",
        "<table><thead><tr><th>Helper</th><th>Class</th><th>Responsibility</th></tr></thead><tbody>"
        "<tr><td>Layout</td><td>GridLayoutHelper</td><td>Column/row sizing, header layout, scrollbar positioning</td></tr>"
        "<tr><td>Data</td><td>GridDataHelper</td><td>Data binding, value access, conversion</td></tr>"
        "<tr><td>Render</td><td>GridRenderHelper</td><td>Cell painting, grid lines, header rendering</td></tr>"
        "<tr><td>Selection</td><td>GridSelectionHelper</td><td>Multi-select, cell/row/column selection state</td></tr>"
        "<tr><td>Input</td><td>GridInputHelper</td><td>Mouse clicks, hit testing, context menu</td></tr>"
        "<tr><td>Scroll</td><td>GridScrollHelper</td><td>Scroll position tracking, wheel handling</td></tr>"
        "<tr><td>ScrollBars</td><td>GridScrollBarsHelper</td><td>Scrollbar visibility and sizing</td></tr>"
        "<tr><td>SortFilter</td><td>GridSortFilterHelper</td><td>Column sorting, filtering state</td></tr>"
        "<tr><td>Edit</td><td>GridEditHelper</td><td>Cell editing mode, editor lifecycle</td></tr>"
        "<tr><td>Theme</td><td>GridThemeHelper</td><td>Theme token application</td></tr>"
        "<tr><td>Navigator</td><td>GridNavigatorHelper</td><td>Keyboard navigation, focus tracking</td></tr>"
        "<tr><td>NavigatorPainter</td><td>GridNavigationPainterHelper</td><td>Navigation bar painting</td></tr>"
        "<tr><td>Sizing</td><td>GridSizingHelper</td><td>Column resize, auto-size, DPI scaling</td></tr>"
        "<tr><td>Dialog</td><td>GridDialogHelper</td><td>Filtering dialogs, inline criterion menus</td></tr>"
        "<tr><td>Clipboard</td><td>GridClipboardHelper</td><td>Copy/paste, CSV format</td></tr>"
        "<tr><td>ColumnReorder</td><td>GridColumnReorderHelper</td><td>Drag-to-reorder columns</td></tr>"
        "<tr><td>KeyboardNavigator</td><td>GridKeyboardNavigator</td><td>Arrow keys, Tab, Home/End navigation</td></tr>"
        "<tr><td>FocusManager</td><td>GridFocusManager</td><td>Focus tracking, accessibility</td></tr></tbody></table>",
    )
    b += section(
        "data-flow",
        "Data Flow",
        '<pre><code class="language-plaintext">DataSource (object)\n'
        "  └── GridDataHelper (Bind, GetValue, SetValue)\n"
        "       ├── GridColumnConfigCollection (column definitions)\n"
        "       ├── GridSortFilterHelper (sort/filter state)\n"
        "       └── GridRenderHelper (paint cells)\n"
        "\n"
        "Optional Virtualization:\n"
        "  GridVirtualDataSource (IVirtualDataSource)\n"
        "    └── GridRowVirtualizer (window management)\n"
        "         └── GridDataHelper (fetch visible window)\n"
        "\n"
        "Optional Grouping:\n"
        "  GridGroupEngine\n"
        "    └── IGridGrouper (sort + group)\n"
        "         └── GridGroupSummaryRow (aggregate row)</code></pre>",
    )
    b += section(
        "painting",
        "Painting Pipeline",
        "<p>Grid painting is style-aware with 6 style families, each providing Header, Navigation, and Filter panel painters:</p>"
        "<table><thead><tr><th>Style Family</th><th>Header Painter</th><th>Navigation Painter</th><th>Filter Panel Painter</th></tr></thead><tbody>"
        "<tr><td>AG Grid</td><td>AGGridHeaderPainter</td><td>AGGridNavigationPainter</td><td>AGGridFilterPanelPainter</td></tr>"
        "<tr><td>Ant Design</td><td>AntDesignHeaderPainter</td><td>AntDesignNavigationPainter</td><td>AntDesignFilterPanelPainter</td></tr>"
        "<tr><td>Bootstrap</td><td>BootstrapHeaderPainter</td><td>BootstrapNavigationPainter</td><td>BootstrapFilterPanelPainter</td></tr>"
        "<tr><td>Card</td><td>CardHeaderPainter</td><td>CardNavigationPainter</td><td>CardFilterPanelPainter</td></tr>"
        "<tr><td>Compact</td><td>CompactHeaderPainter</td><td>CompactNavigationPainter</td><td>CompactFilterPanelPainter</td></tr>"
        "<tr><td>DataTables</td><td>DataTablesHeaderPainter</td><td>DataTablesNavigationPainter</td><td>DataTablesFilterPanelPainter</td></tr></tbody></table>",
    )
    b += section(
        "features",
        "Advanced Features",
        "<ul><li><strong>Virtualization</strong> — Large datasets (millions of rows) via IVirtualDataSource</li>"
        "<li><strong>Selection</strong> — Cell, row, column, multi-range via ISelectionStrategy</li>"
        "<li><strong>Grouping</strong> — Multi-level grouping with aggregate summary rows</li>"
        "<li><strong>Filtering</strong> — Quick filter bar, advanced filter dialog, per-column filters</li>"
        "<li><strong>Export</strong> — CSV, JSON, HTML built-in; Excel/PDF stubs (plugin-extensible)</li>"
        "<li><strong>Editors</strong> — 7 cell editor types (Text, Numeric, Date, Combo, CheckBox, Masked, Generic)</li>"
        "<li><strong>Accessibility</strong> — UIA provider pattern, screen reader support, keyboard navigation</li></ul>",
    )
    b += section(
        "code",
        "Code Example",
        '<pre><code class="language-csharp">var grid = new BeepGridPro();\n'
        "grid.DataSource = myDataTable;\n"
        "grid.RowHeight = 28;\n"
        'grid.GridStyle = "Bootstrap";\n'
        "grid.MultiSelect = true;\n"
        "grid.AllowColumnReorder = true;\n"
        "\n"
        "// Virtual mode\n"
        "var source = GridVirtualDataSource.FromList(myMillionRows);\n"
        "grid.RowVirtualizer.DataSource = source;\n"
        "\n"
        "// Grouping\n"
        'grid.GroupEngine.AddDescriptor(new GroupDescriptor("Category"));\n'
        'grid.GroupEngine.AddDescriptor(new GroupDescriptor("Region"));\n'
        "\n"
        "// Export\n"
        'grid.ExportEngine.Export(ExportFormat.Csv, "data.csv");</code></pre>',
    )
    return b


def gen_gridx_virtualization():
    b = '<div class="page-header"><h1>GridX Virtualization</h1><p class="page-subtitle">Virtual data source and row/column virtualization for handling millions of rows with minimal memory and instant scrolling</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("datasource", "IVirtualDataSource"),
            ("rowvirtualizer", "GridRowVirtualizer"),
            ("preloading", "Preloading Strategy"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p>The grid virtualization system enables <strong>efficient rendering of arbitrarily large datasets</strong> by only materializing rows that are visible in the viewport, plus a configurable overscan buffer:</p>"
        "<ul><li><strong>IVirtualDataSource</strong> — Contract for providing row data on demand</li>"
        "<li><strong>GridVirtualDataSource</strong> — Built-in implementation with preloading and total-count tracking</li>"
        "<li><strong>GridRowVirtualizer</strong> — Manages the visible window and requests data from the source</li>"
        "<li><strong>GridColumnVirtualizer</strong> — Column-level virtualization for wide datasets</li></ul>",
    )
    b += section(
        "datasource",
        "IVirtualDataSource",
        '<pre><code class="language-csharp">public interface IVirtualDataSource\n'
        "{\n"
        "    long TotalRowCount { get; }\n"
        "    IEnumerable&lt;VirtualRowData&gt; GetRows(long startIndex, int count);\n"
        "    void PreloadWindow(long startIndex, int visibleCount, int overscan);\n"
        "    void SetTotalRowCount(long count);\n"
        "    event EventHandler? TotalRowCountChanged;\n"
        "}\n"
        "\n"
        "// Built-in implementations:\n"
        "var source = GridVirtualDataSource.FromList(myList);\n"
        "var source = GridVirtualDataSource.FromDataTable(myTable);\n"
        "var source = GridVirtualDataSource.FromAsync(fetchCallback);</code></pre>",
    )
    b += section(
        "rowvirtualizer",
        "GridRowVirtualizer",
        '<pre><code class="language-csharp">var virtualizer = new GridRowVirtualizer(grid);\n'
        "virtualizer.DataSource = myVirtualSource;\n"
        "\n"
        "// Internal window management:\n"
        "// visibleWindow: first visible row + visible row count\n"
        "// overscanWindow: visibleWindow + padding above/below\n"
        "// rowCache: Dictionary&lt;long, VirtualRowData&gt; for current window</code></pre>",
    )
    b += section(
        "preloading",
        "Preloading Strategy",
        "<ul><li><strong>Visible window</strong> — Rows currently visible in the grid viewport</li>"
        "<li><strong>Overscan buffer</strong> — Extra rows above and below the visible window (default: 2x visible count)</li>"
        "<li><strong>Preloading</strong> — DataSource.PreloadWindow() is called on scroll to hint future data needs</li>"
        "<li><strong>Cache invalidation</strong> — Cache is invalidated when scrolling beyond buffer or when TotalRowCount changes</li></ul>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// Virtual grid with 10 million rows\n'
        "var source = new GridVirtualDataSource();\n"
        "source.SetTotalRowCount(10_000_000);\n"
        "grid.RowVirtualizer.DataSource = source;\n"
        "\n"
        "// Custom async data source\n"
        "public class DatabaseDataSource : IVirtualDataSource\n"
        "{\n"
        "    public long TotalRowCount => db.Count&lt;Customer&gt;();\n"
        "\n"
        "    public IEnumerable&lt;VirtualRowData&gt; GetRows(\n"
        "        long startIndex, int count)\n"
        "    {\n"
        "        return db.Customers.Skip((int)startIndex).Take(count)\n"
        "            .Select(c => VirtualRowData.FromObject(c));\n"
        "    }\n"
        "}</code></pre>",
    )
    return b


def gen_calendar_overview():
    b = '<div class="page-header"><h1>Calendar Architecture</h1><p class="page-subtitle">BeepCalendar internal architecture — 90+ partial file decomposition covering event operations, painting, interactions, commands, and layout theming</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("partial-design", "Partial Class Design"),
            ("event-ops", "Event Operations"),
            ("painting", "Painting Pipeline"),
            ("interactions", "Interaction System"),
            ("file-map", "File Map"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><code>BeepCalendar</code> is the most decomposed control in the library, split across <strong>90+ partial files</strong> organized into 8 subsystems. This design keeps each file focused and manageable despite the control's complexity (~15,000+ lines total).</p>",
    )
    b += section(
        "partial-design",
        "Partial Class Design",
        "<table><thead><tr><th>Subsystem</th><th>Files</th><th>Responsibility</th></tr></thead><tbody>"
        "<tr><td>Core</td><td>6 files</td><td>Constructor, public API, lifecycle, style, appearance</td></tr>"
        "<tr><td>Commands</td><td>5 files</td><td>Public commands, execution core, helpers, duplicate detection</td></tr>"
        "<tr><td>Event Operations</td><td>13 files</td><td>CRUD, history (undo/redo), navigation, queries, editor</td></tr>"
        "<tr><td>Interactions</td><td>16 files</td><td>Pointer events, timing, proposals, hit testing, commitment</td></tr>"
        "<tr><td>Painting</td><td>11 files</td><td>Pipeline, views (Month/Week/Day/List), design-time, sidebar</td></tr>"
        "<tr><td>Layout Theme</td><td>9 files</td><td>Theme application, controls, categories, responsive labels</td></tr>"
        "<tr><td>Visual Updates</td><td>3 files</td><td>Flush, scope, invalidation strategy</td></tr>"
        "<tr><td>Types & Fields</td><td>5 files</td><td>Enums, EventArgs, command types, event model</td></tr></tbody></table>",
    )
    b += section(
        "event-ops",
        "Event Operations",
        '<pre><code class="language-plaintext">EventOperations/\n'
        "  ├── BeepCalendar.EventOperations.cs           — Main partial\n"
        "  ├── BeepCalendar.EventOperations.Public.cs     — Public API\n"
        "  ├── BeepCalendar.EventOperations.Navigation.cs  — Today/Next/Prev\n"
        "  ├── BeepCalendar.EventOperations.Queries.cs    — Filter/Search\n"
        "  ├── BeepCalendar.EventOperations.Crud.cs        — CRUD coordinator\n"
        "  │    ├── Crud.Add.cs      — Add event logic\n"
        "  │    ├── Crud.Remove.cs   — Remove event logic\n"
        "  │    └── Crud.Upsert.cs   — Upsert logic\n"
        "  ├── BeepCalendar.EventOperations.History.cs     — Undo/redo coordinator\n"
        "  │    ├── History.Internal.cs\n"
        "  │    └── History.Apply.cs  — Apply/Apply.Upsert/Apply.Remove\n"
        "  └── BeepCalendar.EventOperations.Editor.cs     — Event editor integration\n"
        "       └── Editor.Commit.cs</code></pre>",
    )
    b += section(
        "painting",
        "Painting Pipeline",
        '<pre><code class="language-plaintext">Painting/\n'
        "  ├── BeepCalendar.Painting.cs                    — Main partial\n"
        "  ├── BeepCalendar.Painting.Pipeline.cs           — Paint orchestration\n"
        "  │    ├── Pipeline.Views.cs     — View dispatch\n"
        "  │    ├── Pipeline.Telemetry.cs — Performance metrics\n"
        "  │    ├── Pipeline.Legacy.cs    — Backward compat\n"
        "  │    └── Pipeline.HeaderFormatting.cs\n"
        "  ├── BeepCalendar.Painting.Views.cs              — View-level paint\n"
        "  │    ├── Painting.MonthView.cs + MonthView.Events.cs + MonthView.Headers.cs\n"
        "  │    ├── Painting.WeekView.cs + WeekView.Events.cs\n"
        "  │    ├── Painting.DayView.cs\n"
        "  │    └── Painting.ListView.cs\n"
        "  ├── BeepCalendar.Painting.DesignTime.cs         — Designer preview\n"
        "  ├── BeepCalendar.Painting.Helpers.cs            — Paint utilities\n"
        "  └── BeepCalendar.Painting.Sidebar.cs            — Sidebar calendar</code></pre>",
    )
    b += section(
        "interactions",
        "Interaction System",
        "<p>The interaction system handles all pointer/touch/mouse events with hit testing that adapts to the current view mode:</p>"
        "<ul><li><strong>Pointer</strong> — Mouse down/up/move/cancel with view-specific hit testing</li>"
        "<li><strong>Timing</strong> — Start/end time selection with snap-to-grid (configurable interval)</li>"
        "<li><strong>Proposals</strong> — Visual preview of event being created (ghost rectangle)</li>"
        "<li><strong>Hit Testing</strong> — MonthView, WeekView, DayView, TimelineView, TimedView, ListView, AgendaView</li>"
        "<li><strong>Commitment</strong> — New event creation, copy-on-drag, modification tracking</li></ul>",
    )
    b += section(
        "file-map",
        "File Map",
        "<p>Notable internal helpers:</p>"
        "<ul><li><strong>CalendarEventEditor</strong> — Public sealed class implementing <code>ICalendarEventEditor</code>. Opens a dialog form with a UserControl for editing event details (title, start/end, all-day, location, status, organizer, description).</li>"
        "<li><strong>CalendarLayoutManager</strong> — Calculates cell positions and dimensions for each view mode</li>"
        "<li><strong>CalendarViewStateHelper</strong> — Tracks view state across mode switches</li>"
        "<li><strong>CalendarPerformanceMetrics</strong> — Measures rendering performance</li>"
        "<li><strong>CalendarEventService</strong> — Conflict detection and resolution</li></ul>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">var cal = new BeepCalendar();\n'
        "\n"
        "// View modes\n"
        "cal.ViewMode = CalendarViewMode.Month;  // Month/Week/Day/Agenda/Timeline\n"
        "cal.CurrentDate = DateTime.Today;\n"
        "cal.ShowSidebar = true;\n"
        "\n"
        "// Events\n"
        "cal.Events.Add(new CalendarEvent\n"
        "{\n"
        '    Title = "Team Standup",\n'
        "    Start = DateTime.Today.AddHours(9),\n"
        "    End = DateTime.Today.AddHours(9.5),\n"
        '    Category = categories["Meeting"]\n'
        "});\n"
        "\n"
        "// Undo/redo\n"
        "if (cal.CanUndo) cal.Undo();\n"
        "if (cal.CanRedo) cal.Redo();\n"
        "\n"
        "// Conflict detection\n"
        "cal.ConflictDetected += (s, e) =>\n"
        '    Console.WriteLine($"Conflict: {e.Conflict.Description}");</code></pre>',
    )
    return b


def gen_wizard_overview():
    b = '<div class="page-header"><h1>Wizard Architecture</h1><p class="page-subtitle">The wizard system architecture — WizardManager, WizardInstance, form types, painters, step validation, and animation engine</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("manager", "WizardManager"),
            ("instance", "WizardInstance"),
            ("forms", "Wizard Forms"),
            ("painters", "Wizard Painters"),
            ("config", "WizardConfig"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p>The wizard system provides a complete multi-step wizard framework with 4 form styles, 4 painter styles, step validation, and animation:</p>"
        "<ul><li><strong>WizardManager</strong> — Static facade. CreateWizard(), ShowWizard(), wizard registry</li>"
        "<li><strong>WizardInstance</strong> — Stateful instance. Navigate, validate, complete, cancel</li>"
        "<li><strong>WizardForms</strong> — 4 form types: VerticalStepper, HorizontalStepper, Minimal, Cards</li>"
        "<li><strong>WizardPainters</strong> — Corresponding painters for each form style</li></ul>",
    )
    b += section(
        "manager",
        "WizardManager",
        '<pre><code class="language-csharp">public static class WizardManager\n'
        "{\n"
        "    public static WizardStyle DefaultStyle { get; set; } = WizardStyle.HorizontalStepper;\n"
        "    public static bool EnableAnimations { get; set; } = true;\n"
        "\n"
        "    public static WizardInstance CreateWizard(WizardConfig config);\n"
        "    public static DialogResult ShowWizard(WizardConfig config);\n"
        "    public static DialogResult ShowWizard(WizardConfig config, IWin32Window owner);\n"
        "    public static WizardInstance GetWizard(string key);\n"
        "    public static void CloseWizard(string key);\n"
        "    public static void CloseAllWizards();\n"
        "}</code></pre>",
    )
    b += section(
        "instance",
        "WizardInstance",
        '<pre><code class="language-csharp">public class WizardInstance\n'
        "{\n"
        "    public WizardConfig Config { get; }\n"
        "    public WizardContext Context { get; }\n"
        "    public int CurrentStepIndex { get; }\n"
        "    public WizardStep CurrentStep { get; }\n"
        "    public bool IsFirstStep { get; }\n"
        "    public bool IsLastStep { get; }\n"
        "\n"
        "    public async Task&lt;bool&gt; NavigateNextAsync();\n"
        "    public async Task&lt;bool&gt; NavigateBackAsync();\n"
        "    public async Task&lt;bool&gt; CompleteAsync();\n"
        "    public void Cancel();\n"
        "    public Task&lt;DialogResult&gt; ShowDialogAsync();\n"
        "\n"
        "    public event EventHandler&lt;StepChangingEventArgs&gt; StepChanging;\n"
        "    public event EventHandler&lt;StepChangedEventArgs&gt; StepChanged;\n"
        "    public event EventHandler&lt;WizardCompletedEventArgs&gt; Completed;\n"
        "    public event EventHandler&lt;WizardCancelledEventArgs&gt; Cancelled;\n"
        "}</code></pre>",
    )
    b += section(
        "forms",
        "Wizard Forms",
        "<table><thead><tr><th>Form</th><th>Style</th><th>Description</th></tr></thead><tbody>"
        "<tr><td>VerticalStepperWizardForm</td><td>Vertical stepper</td><td>Left sidebar with vertical step indicators</td></tr>"
        "<tr><td>HorizontalStepperWizardForm</td><td>Horizontal stepper</td><td>Top horizontal step bar with progress</td></tr>"
        "<tr><td>MinimalWizardForm</td><td>Minimal</td><td>Clean simple navigation without step indicators</td></tr>"
        "<tr><td>CardsWizardForm</td><td>Cards</td><td>Card-based wizard with flip animation</td></tr></tbody></table>",
    )
    b += section(
        "painters",
        "Wizard Painters",
        "<ul><li><strong>VerticalStepperPainter</strong> — Paints left-side stepper with numbered circles and connector lines</li>"
        "<li><strong>HorizontalStepperPainter</strong> — Paints top bar with connected step nodes</li>"
        "<li><strong>MinimalPainter</strong> — Paints simple navigation bar</li>"
        "<li><strong>CardsPainter</strong> — Paints card transitions</li></ul>",
    )
    b += section(
        "config",
        "WizardConfig & Models",
        '<pre><code class="language-csharp">public class WizardConfig\n'
        "{\n"
        "    public string Key { get; set; }\n"
        "    public string Title { get; set; }\n"
        "    public WizardStyle Style { get; set; }\n"
        "    public List&lt;WizardStep&gt; Steps { get; set; }\n"
        "    public bool ShowCancelButton { get; set; }\n"
        "    public bool AllowBackNavigation { get; set; }\n"
        "    public IWizardFormHost FormHost { get; set; }\n"
        "}\n"
        "\n"
        "public class WizardStep\n"
        "{\n"
        "    public string Title { get; set; }\n"
        "    public string Description { get; set; }\n"
        "    public Control Content { get; set; }\n"
        "    public Func&lt;bool&gt; ValidateFn { get; set; }\n"
        "    public Action SaveFn { get; set; }\n"
        "    public string IconPath { get; set; }\n"
        "}</code></pre>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">var config = new WizardConfig\n'
        "{\n"
        '    Key = "setup",\n'
        '    Title = "Application Setup",\n'
        "    Style = WizardStyle.VerticalStepper,\n"
        "    Steps = new List&lt;WizardStep&gt;\n"
        "    {\n"
        "        new WizardStep\n"
        "        {\n"
        '            Title = "Welcome",\n'
        '            Description = "Get started with setup",\n'
        "            Content = new WelcomeControl()\n"
        "        },\n"
        "        new WizardStep\n"
        "        {\n"
        '            Title = "Configuration",\n'
        '            Description = "Configure settings",\n'
        "            Content = new ConfigControl(),\n"
        "            ValidateFn = () => ValidateConfig()\n"
        "        }\n"
        "    }\n"
        "};\n"
        "\n"
        "var result = WizardManager.ShowWizard(config);\n"
        "if (result == DialogResult.OK)\n"
        "    ApplyConfiguration(config.Context);</code></pre>",
    )
    return b


def gen_theme_overview():
    b = '<div class="page-header"><h1>Theme System Architecture</h1><p class="page-subtitle">IBeepTheme contract, BeepTheme partial class decomposition, theme types (Ubuntu/GNOME/Cyberpunk/Candy/Zen), and the token resolution pipeline</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("ibeep-theme", "IBeepTheme Contract"),
            ("beep-theme", "BeepTheme Partial Class"),
            ("theme-types", "Theme Types"),
            ("token-resolution", "Token Resolution"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p>The Beep theme system powers the visual appearance of every control. It consists of:</p>"
        "<ul><li><strong>IBeepTheme</strong> — Interface defining the theme contract (in TheTechIdea.Beep.Vis.Modules2.0)</li>"
        "<li><strong>BeepTheme</strong> — Central partial class with 35+ partial files, each handling a control type</li>"
        "<li><strong>Theme Types</strong> — Concrete implementations: Ubuntu, GNOME, Cyberpunk, Candy, Zen</li>"
        "<li><strong>BeepThemesManager</strong> — Singleton manager for theme registration and switching</li></ul>",
    )
    b += section(
        "ibeep-theme",
        "IBeepTheme Contract",
        '<pre><code class="language-csharp">public interface IBeepTheme\n'
        "{\n"
        "    string ThemeName { get; }\n"
        "    Color ButtonBackColor { get; }\n"
        "    Color ButtonForeColor { get; }\n"
        "    Color ButtonBorderColor { get; }\n"
        "    Color ButtonHoverColor { get; }\n"
        "    // ... 200+ color/style properties for every control type\n"
        "    Font DefaultFont { get; }\n"
        "    Font TitleFont { get; }\n"
        "    int BorderRadius { get; }\n"
        "    float Opacity { get; }\n"
        "}</code></pre>",
    )
    b += section(
        "beep-theme",
        "BeepTheme Partial Class (35 Files)",
        "<p>Each partial file handles a specific control type:</p>"
        "<table><thead><tr><th>File</th><th>Control Type</th><th>Properties</th></tr></thead><tbody>"
        "<tr><td>BeepTheme.cs / Core.cs / Utility.cs</td><td>Base</td><td>Name, fonts, borders, opacity</td></tr>"
        "<tr><td>BeepTheme.Buttons.cs</td><td>Button</td><td>BackColor, ForeColor, Border, Hover, Pressed, Disabled</td></tr>"
        "<tr><td>BeepTheme.TextBox.cs</td><td>TextBox</td><td>BackColor, ForeColor, Border, Focused, Placeholder</td></tr>"
        "<tr><td>BeepTheme.ComboBox.cs</td><td>ComboBox</td><td>Field, dropdown, popup colors</td></tr>"
        "<tr><td>BeepTheme.CheckBox.cs</td><td>CheckBox</td><td>Check mark, box border, fill colors</td></tr>"
        "<tr><td>BeepTheme.RadioButton.cs</td><td>RadioButton</td><td>Circle border, dot colors</td></tr>"
        "<tr><td>BeepTheme.Grid.cs</td><td>Grid</td><td>Header, cell, alternating row, selection colors</td></tr>"
        "<tr><td>BeepTheme.Chart.cs</td><td>Chart</td><td>Series palette, axis, legend colors</td></tr>"
        "<tr><td>BeepTheme.Calendar.cs</td><td>Calendar</td><td>Cell, header, today, event colors</td></tr>"
        "<tr><td>BeepTheme.Tab.cs</td><td>Tabs</td><td>Active, inactive, hover, border colors</td></tr>"
        "<tr><td>BeepTheme.Stepper.cs</td><td>Stepper</td><td>Step node, connector, label colors</td></tr>"
        "<tr><td>BeepTheme.Tree.cs</td><td>TreeView</td><td>Node, expander, selection colors</td></tr>"
        "<tr><td>BeepTheme.Menu.cs</td><td>Menu</td><td>Bar, item, hover, separator colors</td></tr>"
        "<tr><td>BeepTheme.Navigation.cs</td><td>Navigation</td><td>Sidebar, nav item colors</td></tr>"
        "</tbody></table>"
        "<p>Plus 22 more: Dialog, Login, Dashboard, StatusBar, ToolTip, FontAndTypography, Gradient, ColorPalette, Company, BlockquoteCode, Badge, Link, SideMenu, StarRating, StatsCard, TaskCard, Testimony, ScrollList, Card, ScrollBar, Switch, ProgressBar, Miscellaneous.</p>",
    )
    b += section(
        "theme-types",
        "Theme Types",
        "<table><thead><tr><th>Theme</th><th>Project</th><th>Files</th><th>Style</th></tr></thead><tbody>"
        "<tr><td>UbuntuTheme</td><td>Controls</td><td>26 partial files</td><td>Ubuntu Linux aesthetic — orange accents, dark/light variants</td></tr>"
        "<tr><td>GNOMETheme</td><td>Controls</td><td>30 partial files</td><td>GNOME desktop — Adwaita colors, rounded corners</td></tr>"
        "<tr><td>CyberpunkTheme</td><td>Controls</td><td>30 partial files</td><td>Neon cyberpunk — bright magenta/cyan, dark backgrounds</td></tr>"
        "<tr><td>CandyTheme</td><td>Vis.Modules2.0</td><td>1 file</td><td>Soft candy colors — pastels, rounded shapes</td></tr>"
        "<tr><td>ZenTheme</td><td>Vis.Modules2.0</td><td>1 file</td><td>Minimal zen — earth tones, clean typography</td></tr>"
        "<tr><td>CyberpunkNeonTheme</td><td>Vis.Modules2.0</td><td>5 partial files</td><td>Neon variant — brighter, more saturated</td></tr>"
        "</tbody></table>",
    )
    b += section(
        "token-resolution",
        "Token Resolution",
        '<pre><code class="language-csharp">// Token resolution pipeline:\n'
        "// 1. Control checks its own theme property override\n"
        "// 2. Falls back to BeepThemesManager.CurrentTheme\n"
        "// 3. Falls back to default theme values\n"
        "\n"
        "public Color GetButtonBackColor(BaseControl control)\n"
        "{\n"
        "    if (control.ThemeOverrides?.ButtonBackColor != null)\n"
        "        return control.ThemeOverrides.ButtonBackColor;\n"
        "\n"
        "    return BeepThemesManager.CurrentTheme\n"
        "        ?.ButtonBackColor ?? DefaultTheme.ButtonBackColor;\n"
        "}</code></pre>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">// Global theme switch\n'
        'BeepThemesManager.Instance.SetTheme("DarkTheme");\n'
        "// All controls update automatically\n"
        "\n"
        "// Per-control override\n"
        "myButton.ThemeOverrides = new ThemeOverrides\n"
        "{\n"
        "    ButtonBackColor = Color.Red,\n"
        "    BorderRadius = 12\n"
        "};\n"
        "\n"
        "// Custom theme\n"
        "var myTheme = new BeepTheme\n"
        "{\n"
        '    ThemeName = "MyCustomTheme",\n'
        "    ButtonBackColor = Color.Navy,\n"
        "    ButtonForeColor = Color.White,\n"
        "    // ... configure all properties\n"
        "};\n"
        "BeepThemesManager.Instance.RegisterTheme(myTheme);</code></pre>",
    )
    return b


def gen_beepdock_arch():
    b = '<div class="page-header"><h1>BeepDock Architecture</h1><p class="page-subtitle">The macOS-style application dock — 22 theme-aware painters, easing animations, hit testing, keyboard navigation, and drag-drop reordering</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("painters", "22 Dock Painters"),
            ("features", "Key Features"),
            ("interaction", "Interaction System"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p><code>BeepDock</code> provides a macOS-style application dock for WinForms. It supports <strong>22 visual styles</strong> through a painter system, each with unique rendering for icons, backgrounds, indicators, badges, and tooltips.</p>"
        "<p>Organized across 12 partial files: Properties, Events, Methods, Drawing, Animation, Mouse, DragDrop, Keyboard, Items, Notifications, InteractionState.</p>",
    )
    b += section(
        "painters",
        "22 Dock Painters",
        "<table><thead><tr><th>Painter</th><th>Style</th><th>Key Visual</th></tr></thead><tbody>"
        "<tr><td>ClassicTaskbarDockPainter</td><td>Windows Classic</td><td>Flat toolbar with labels</td></tr>"
        "<tr><td>Windows11DockPainter</td><td>Windows 11</td><td>Centered icons, rounded corners, acrylic</td></tr>"
        "<tr><td>AppleDockPainter</td><td>macOS</td><td>Glass shelf, magnification, reflection</td></tr>"
        "<tr><td>iOSDockPainter</td><td>iOS</td><td>Frosted glass, rounded icon containers</td></tr>"
        "<tr><td>Material3DockPainter</td><td>Material Design 3</td><td>Pill-shaped container, dynamic color</td></tr>"
        "<tr><td>GNOMEDockPainter</td><td>GNOME</td><td>Dark strip, subtle indicators</td></tr>"
        "<tr><td>PlankDockPainter</td><td>Plank</td><td>Flat bar, simple active dots</td></tr>"
        "<tr><td>NeumorphicDockPainter</td><td>Neumorphic</td><td>Soft shadows, embossed effect</td></tr>"
        "<tr><td>GlassmorphismDockPainter</td><td>Glassmorphism</td><td>Blurred glass, border glow</td></tr>"
        "<tr><td>CyberpunkDockPainter</td><td>Cyberpunk</td><td>Neon borders, glitch effects</td></tr>"
        "<tr><td>DraculaDockPainter</td><td>Dracula</td><td>Purple/cyan dark theme</td></tr>"
        "<tr><td>NordDockPainter</td><td>Nord</td><td>Arctic blue-gray palette</td></tr>"
        "<tr><td>NeonDockPainter</td><td>Neon</td><td>Bright glowing indicators</td></tr>"
        "<tr><td>MinimalDockPainter</td><td>Minimal</td><td>Clean, no chrome</td></tr>"
        "<tr><td>FloatingDockPainter</td><td>Floating</td><td>Detached pill, drop shadow</td></tr>"
        "<tr><td>BubbleDockPainter</td><td>Bubble</td><td>Round bubble containers</td></tr>"
        "<tr><td>ArcDockPainter</td><td>Arc</td><td>Curved arc shelf</td></tr>"
        "<tr><td>PlasmaPanelPainter</td><td>Plasma</td><td>KDE-style panel</td></tr>"
        "<tr><td>TerminalDockPainter</td><td>Terminal</td><td>Hacker terminal aesthetic</td></tr></tbody></table>"
        "<p>All painters implement <code>IDockPainter</code> and extend <code>DockPainterBase</code>. The <code>DockPainterFactory</code> resolves <code>DockStyle</code> to the appropriate painter.</p>",
    )
    b += section(
        "features",
        "Key Features",
        "<ul><li><strong>22 visual styles</strong> — From macOS to Cyberpunk</li>"
        "<li><strong>Icon magnification</strong> — Apple-dock-style hover magnification with configurable max scale</li>"
        "<li><strong>Easing animations</strong> — Configurable easing curves via DockEasingHelper</li>"
        "<li><strong>Drag-drop reordering</strong> — Items can be reordered by dragging</li>"
        "<li><strong>Badge support</strong> — Notification badges on dock items</li>"
        "<li><strong>Tooltips</strong> — Hover tooltips with configurable styles</li>"
        "<li><strong>Keyboard navigation</strong> — Arrow keys, Enter to activate</li>"
        "<li><strong>4 positions</strong> — Bottom, Top, Left, Right with auto-orientation</li></ul>",
    )
    b += section(
        "interaction",
        "Interaction System",
        "<ul><li><strong>DockHitTestHelper</strong> — Resolves which item is under the cursor</li>"
        "<li><strong>DockAnimationHelper</strong> — Manages hover scale animation with easing</li>"
        "<li><strong>DockLayoutHelper</strong> — Calculates item positions with magnification spread</li>"
        "<li><strong>DockThemeHelpers</strong> — Resolves theme tokens for the active painter</li>"
        "<li><strong>DockIconHelpers</strong> — Icon resolution and sizing</li></ul>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">var dock = new BeepDock();\n'
        "\n"
        "// Style\n"
        "dock.DockStyleType = DockStyle.AppleDock;\n"
        "dock.PositionAtBottom();\n"
        "dock.ItemSize = 56;\n"
        "dock.MaxScale = 1.5f;  // Magnification\n"
        "\n"
        "// Items\n"
        'dock.AddItem("Finder", Icons.Finder, () => OpenFinder());\n'
        'dock.AddItem("Editor", Icons.Editor, () => OpenEditor());\n'
        'dock.AddItem("Terminal", Icons.Terminal, () => OpenTerminal());\n'
        "\n"
        "// Badges\n"
        'dock.SetBadge("Editor", "3");  // Show "3" badge\n'
        "\n"
        "// Animation\n"
        "dock.AnimationStyle = DockAnimationStyle.EaseInOut;\n"
        "dock.ShowTooltips = true;</code></pre>",
    )
    return b


def gen_listbox_painters():
    b = '<div class="page-header"><h1>ListBox Painters</h1><p class="page-subtitle">42 theme-aware list box painters — covering Standard, Material, Glassmorphism, Chat, Timeline, Notification, and many more styles</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("architecture", "Architecture"),
            ("category-list", "Painter Categories"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p>The BeepListBox supports <strong>42 visual painter styles</strong>, making it one of the most versatile controls in the library. Each painter handles background, item rendering, selection indication, hover effects, and accessibility contrast modes.</p>"
        "<p>Painters implement <code>IListBoxPainter</code> or extend <code>BaseListBoxPainter</code>. Selection resolves via <code>ListBoxPainterFactory</code>.</p>",
    )
    b += section(
        "architecture",
        "Architecture",
        '<pre><code class="language-plaintext">IListBoxPainter\n'
        "  └── BaseListBoxPainter (abstract)\n"
        "       ├── StandardListBoxPainter\n"
        "       ├── FilledListBoxPainter\n"
        "       ├── OutlinedListBoxPainter\n"
        "       ├── CardListPainter\n"
        "       ├── CustomListPainter\n"
        "       └── ... (42 total)\n"
        "\n"
        "ListBoxPainterFactory.Get(ListBoxType)\n"
        "  → IListBoxPainter</code></pre>",
    )
    b += section(
        "category-list",
        "Painter Categories (42 Total)",
        "<h3>Standard Styles (10)</h3>"
        "<p>StandardListBoxPainter, SimpleListPainter, OutlinedListBoxPainter, FilledListBoxPainter, FilledStylePainter, MinimalListBoxPainter, CompactListPainter, RoundedListBoxPainter, BorderlessListBoxPainter, MaterialOutlinedListBoxPainter</p>"
        "<h3>Card & Content Styles (8)</h3>"
        "<p>CardListPainter, GradientCardListBoxPainter, GlassmorphismListBoxPainter, HeroUIListBoxPainter, NeumorphicListBoxPainter, RekaUIListBoxPainter, ChakraUIListBoxPainter, CommandListBoxPainter</p>"
        "<h3>Specialized Item Styles (10)</h3>"
        "<p>AvatarListBoxPainter, ChatListBoxPainter, NotificationListBoxPainter, TimelineListBoxPainter, ThreeLineListBoxPainter, ProfileCardListBoxPainter, ContactListBoxPainter, TeamMembersPainter, WithIconsListBoxPainter, ChipStyleListBoxPainter</p>"
        "<h3>Behavior Styles (6)</h3>"
        "<p>SearchableListPainter, GroupedListPainter, CheckboxListPainter, RadioSelectionPainter, MultiSelectionTealPainter, InfiniteScrollListBoxPainter</p>"
        "<h3>Selection & State Styles (5)</h3>"
        "<p>ColoredSelectionPainter, LanguageSelectorPainter, CategoryChipsPainter, FilterStatusPainter, ErrorStatesPainter</p>"
        "<h3>Checkbox Variants (3)</h3>"
        "<p>CheckboxListPainter, RaisedCheckboxesPainter, OutlinedCheckboxesPainter</p>"
        "<h3>Navigation</h3>"
        "<p>NavigationRailListBoxPainter</p>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">var listBox = new BeepListBox();\n'
        "\n"
        "// Set visual style\n"
        "listBox.ItemStyle = ListBoxType.CardList;\n"
        "// → CardListPainter renders each item as a card\n"
        "\n"
        "listBox.ItemStyle = ListBoxType.ChatList;\n"
        "// → ChatListBoxPainter adds avatar + message preview\n"
        "\n"
        "listBox.ItemStyle = ListBoxType.GroupedList;\n"
        "// → GroupedListPainter renders section headers\n"
        "\n"
        "// Add items\n"
        "listBox.Items.Add(new BeepListItem\n"
        "{\n"
        '    Text = "John Doe",\n'
        '    SubText = "Online",\n'
        '    ImagePath = "avatars/john.png"\n'
        "});</code></pre>",
    )
    return b


def gen_stepper_painters():
    b = '<div class="page-header"><h1>Stepper Painters</h1><p class="page-subtitle">15 stepper painter styles — from Breadcrumb to Timeline, ProgressBar to Dots, with IStepperPainter contract</p></div>\n'
    b += toc(
        [
            ("overview", "Overview"),
            ("painters", "Painter List (15)"),
            ("examples", "Examples"),
        ]
    )
    b += section(
        "overview",
        "Overview",
        "<p>Stepper painters control the visual rendering of step indicators in wizard-like flows. 15 painters cover diverse visual styles:</p>"
        "<table><thead><tr><th>Painter</th><th>Visual Style</th></tr></thead><tbody>"
        "<tr><td>ChevronBreadcrumbStepperPainter</td><td>Chevron-separated breadcrumb</td></tr>"
        "<tr><td>SegmentedTabStepperPainter</td><td>Segmented tab control</td></tr>"
        "<tr><td>CircularNodeStepperPainter</td><td>Numbered circles with lines</td></tr>"
        "<tr><td>DotsStepperPainter</td><td>Dot indicators (mobile-style)</td></tr>"
        "<tr><td>BadgeStatusStepperPainter</td><td>Badge-style status indicators</td></tr>"
        "<tr><td>ProgressBarStepperPainter</td><td>Progress bar + step labels</td></tr>"
        "<tr><td>GradientMaterialStepperPainter</td><td>Material gradient steps</td></tr>"
        "<tr><td>VerticalTimelineStepperPainter</td><td>Vertical timeline</td></tr>"
        "<tr><td>AlternatingTimelineStepperPainter</td><td>Alternating left/right timeline</td></tr>"
        "<tr><td>IconTimelineStepperPainter</td><td>Timeline with icons</td></tr>"
        "<tr><td>CompactInlineStepperPainter</td><td>Compact inline text</td></tr>"
        "<tr><td>SquareDashedStepperPainter</td><td>Square steps with dashed lines</td></tr>"
        "<tr><td>NoOpStepperPainter</td><td>No visible stepper</td></tr></tbody></table>"
        "<p>All painters implement <code>IStepperPainter</code>. Selection via <code>StepperPainterRegistry</code> with <code>StepperPainterNameConverter</code> for string-based lookup.</p>",
    )
    b += section(
        "examples",
        "Examples",
        '<pre><code class="language-csharp">var stepper = new BeepSteppperBar();\n'
        'stepper.PainterName = "CircularNode";\n'
        "// Renders: (1)─(2)─(3)─(4)\n"
        "stepper.GoToStep(2);\n"
        "// Step 1: completed (green), Step 2: active (blue)\n"
        "\n"
        'stepper.PainterName = "ChevronBreadcrumb";\n'
        "// Renders: Home > Products > Details > Confirm</code></pre>",
    )
    return b


# ============ PAGE REGISTRY ============

ARCH_PAGES = {
    "docking-overview.html": gen_docking_overview,
    "dockpanel-system.html": gen_dockpanel_system,
    "docklayoutdefinition.html": gen_docklayout_definition,
    "floatwindow-autohide.html": gen_floatwindow_autohide,
    "docking-painters.html": gen_docking_painters,
    "docking-dragdrop.html": gen_docking_dragdrop,
    "gridx-overview.html": gen_gridx_overview,
    "gridx-virtualization.html": gen_gridx_virtualization,
    "calendar-overview.html": gen_calendar_overview,
    "wizard-overview.html": gen_wizard_overview,
    "theme-overview.html": gen_theme_overview,
    "beepdock-architecture.html": gen_beepdock_arch,
    "listbox-painters.html": gen_listbox_painters,
    "stepper-painters.html": gen_stepper_painters,
}

# ============ BUILD ============


def build():
    arch_dir = os.path.join(BASE, "architecture")
    os.makedirs(arch_dir, exist_ok=True)

    for filename, gen_func in ARCH_PAGES.items():
        body = gen_func()
        name = filename.replace(".html", "")
        html = page(name.replace("-", " ").title(), body)
        path = os.path.join(arch_dir, filename)
        with open(path, "w", encoding="utf-8") as f:
            f.write(html)
        print("Wrote:", path)

    print("\nDone! {} architecture pages created.".format(len(ARCH_PAGES)))


if __name__ == "__main__":
    build()
