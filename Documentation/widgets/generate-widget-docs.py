"""
generate-widget-docs.py
Generates the complete Beep Widget System HTML documentation.
Run from the Documentation/widgets/ directory or anywhere (paths are absolute).
"""
import os

DOCS_DIR = os.path.dirname(os.path.abspath(__file__))

# ── Shared sidebar HTML ──────────────────────────────────────────────────────
def sidebar(active_href: str) -> str:
    items = [
        ("index.html",                   "Widget Overview"),
        ("beep-metric-widget.html",      "BeepMetricWidget"),
        ("beep-chart-widget.html",       "BeepChartWidget"),
        ("beep-list-widget.html",        "BeepListWidget"),
        ("beep-dashboard-widget.html",   "BeepDashboardWidget"),
        ("beep-control-widget.html",     "BeepControlWidget"),
        ("beep-notification-widget.html","BeepNotificationWidget"),
        ("beep-navigation-widget.html",  "BeepNavigationWidget"),
        ("beep-media-widget.html",       "BeepMediaWidget"),
        ("beep-finance-widget.html",     "BeepFinanceWidget"),
        ("beep-form-widget.html",        "BeepFormWidget"),
        ("beep-social-widget.html",      "BeepSocialWidget"),
        ("beep-map-widget.html",         "BeepMapWidget"),
        ("beep-calendar-widget.html",    "BeepCalendarWidget"),
    ]
    lis = ""
    for href, label in items:
        cls = ' class="active"' if href == active_href else ""
        lis += f'                            <li><a href="{href}"{cls}>{label}</a></li>\n'

    return f"""        <aside class="sidebar" id="sidebar">
            <div class="logo">
                <img src="../assets/beep-logo.svg" alt="Beep Controls Logo">
                <div class="logo-text">
                    <h2>Beep Controls</h2>
                    <span class="version">v1.0.164</span>
                </div>
            </div>
            <div class="search-container">
                <input type="text" class="search-input" placeholder="Search documentation..." onkeyup="searchDocs(this.value)">
            </div>
            <nav>
                <ul class="nav-menu">
                    <li><a href="../index.html"><i class="bi bi-house"></i> Home</a></li>
                    <li><a href="../controls/beep-button.html"><i class="bi bi-grid"></i> Controls</a></li>
                    <li class="has-submenu open">
                        <a href="#"><i class="bi bi-window-stack"></i> Widgets</a>
                        <ul class="submenu">
{lis}                        </ul>
                    </li>
                    <li><a href="../api/beep-control-base.html"><i class="bi bi-code-square"></i> API Reference</a></li>
                </ul>
            </nav>
        </aside>"""


# ── Page wrapper ─────────────────────────────────────────────────────────────
def page(title: str, href: str, breadcrumb_label: str, body: str) -> str:
    return f"""<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>{title} - Beep Controls Documentation</title>
    <link rel="stylesheet" href="../sphinx-style.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/themes/prism-tomorrow.min.css">
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css">
</head>
<body>
    <button class="mobile-menu-toggle" onclick="toggleSidebar()"><i class="bi bi-list"></i></button>
    <button class="theme-toggle" onclick="toggleTheme()" title="Toggle theme"><i class="bi bi-sun-fill" id="theme-icon"></i></button>
    <div class="container">
{sidebar(href)}
        <main class="content">
            <div class="content-wrapper">
                <nav class="breadcrumb-nav">
                    <a href="../index.html">Home</a>
                    <span>›</span>
                    <a href="index.html">Widgets</a>
                    <span>›</span>
                    <span>{breadcrumb_label}</span>
                </nav>
{body}
            </div>
        </main>
    </div>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/prism.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/components/prism-csharp.min.js"></script>
    <script>
        const savedTheme = localStorage.getItem('theme');
        if (savedTheme === 'dark') {{
            document.body.setAttribute('data-theme', 'dark');
            document.getElementById('theme-icon').className = 'bi bi-moon-fill';
        }}
        function toggleTheme() {{
            const body = document.body;
            const icon = document.getElementById('theme-icon');
            if (body.getAttribute('data-theme') === 'dark') {{
                body.removeAttribute('data-theme');
                icon.className = 'bi bi-sun-fill';
                localStorage.setItem('theme', 'light');
            }} else {{
                body.setAttribute('data-theme', 'dark');
                icon.className = 'bi bi-moon-fill';
                localStorage.setItem('theme', 'dark');
            }}
        }}
        function toggleSidebar() {{ document.getElementById('sidebar').classList.toggle('open'); }}
        function searchDocs(q) {{
            document.querySelectorAll('.nav-menu a').forEach(a => {{
                a.closest('li').style.display = a.textContent.toLowerCase().includes(q.toLowerCase()) || q==='' ? '' : 'none';
            }});
        }}
    </script>
</body>
</html>"""


# ── Styles table helper ───────────────────────────────────────────────────────
def styles_table(styles: list[tuple]) -> str:
    rows = "\n".join(
        f"                            <tr><td><code>{s}</code></td><td>{d}</td></tr>"
        for s, d in styles
    )
    return f"""                    <table class="props-table">
                        <thead><tr><th>Style Value</th><th>Description</th></tr></thead>
                        <tbody>
{rows}
                        </tbody>
                    </table>"""


# ── Properties table helper ───────────────────────────────────────────────────
def props_table(props: list[tuple]) -> str:
    rows = "\n".join(
        f"                            <tr><td><code>{p}</code></td><td>{t}</td><td>{d}</td><td>{desc}</td></tr>"
        for p, t, d, desc in props
    )
    return f"""                    <table class="props-table">
                        <thead><tr><th>Property</th><th>Type</th><th>Default</th><th>Description</th></tr></thead>
                        <tbody>
{rows}
                        </tbody>
                    </table>"""


# ── Events table helper ───────────────────────────────────────────────────────
def events_table(events: list[tuple]) -> str:
    rows = "\n".join(
        f"                            <tr><td><code>{e}</code></td><td>{a}</td><td>{d}</td></tr>"
        for e, a, d in events
    )
    return f"""                    <table class="props-table">
                        <thead><tr><th>Event</th><th>Args Type</th><th>Description</th></tr></thead>
                        <tbody>
{rows}
                        </tbody>
                    </table>"""


# ── Feature cards helper ──────────────────────────────────────────────────────
def feature_cards(feats: list[tuple]) -> str:
    cards = "\n".join(
        f"""                        <div class="feature-card"><h3>{ico} {t}</h3><p>{d}</p></div>"""
        for ico, t, d in feats
    )
    return f'                    <div class="feature-grid">\n{cards}\n                    </div>'


# ═══════════════════════════════════════════════════════════════════════════════
# INDEX PAGE
# ═══════════════════════════════════════════════════════════════════════════════
def gen_index():
    widgets = [
        ("📊", "BeepMetricWidget",      "beep-metric-widget.html",      "6 styles",  "KPI and metric display panels with optional trend indicators and progress bars."),
        ("📈", "BeepChartWidget",       "beep-chart-widget.html",       "7 styles",  "Bar, line, pie, gauge, sparkline, heatmap and combination charts."),
        ("📋", "BeepListWidget",        "beep-list-widget.html",        "6 styles",  "Activity feeds, data tables, rankings, status lists, profile lists and to-do lists."),
        ("🗂️", "BeepDashboardWidget",   "beep-dashboard-widget.html",   "6 styles",  "Composite dashboard panels combining multiple KPIs, charts and timelines."),
        ("🎛️", "BeepControlWidget",     "beep-control-widget.html",     "10 styles", "Interactive input controls: toggles, sliders, dropdowns, date pickers, search boxes, colour pickers and more."),
        ("🔔", "BeepNotificationWidget","beep-notification-widget.html","10 styles", "Toast notifications, alert banners, validation messages, progress alerts and success banners."),
        ("🧭", "BeepNavigationWidget",  "beep-navigation-widget.html",  "10 styles", "Breadcrumbs, step indicators, tabs, pagination, menu bars, sidebars and wizard steps."),
        ("🖼️", "BeepMediaWidget",       "beep-media-widget.html",       "10 styles", "Image cards, avatar groups, icon cards, galleries, profile cards and photo grids."),
        ("💰", "BeepFinanceWidget",     "beep-finance-widget.html",     "10 styles", "Portfolio cards, crypto widgets, transaction cards, balance cards and budget trackers."),
        ("📝", "BeepFormWidget",        "beep-form-widget.html",        "10 styles", "Field groups, validation panels, form sections, input cards and multi-step form steps."),
        ("👥", "BeepSocialWidget",      "beep-social-widget.html",      "10 styles", "Profile cards, team grids, chat widgets, comment threads and social activity feeds."),
        ("🗺️", "BeepMapWidget",         "beep-map-widget.html",         "10 styles", "Location cards, route display, geographic heatmaps, address cards and place cards."),
        ("📅", "BeepCalendarWidget",    "beep-calendar-widget.html",    "10 styles", "Calendar views, time slots, event cards, week views, event lists and availability grids."),
    ]
    cards = "\n".join(f"""                    <a href="{href}" class="related-card" style="text-decoration:none;">
                        <h4>{ico} {name} <span class="badge badge-stable" style="font-size:11px;margin-left:6px;">{styles}</span></h4>
                        <p>{desc}</p>
                    </a>""" for ico, name, href, styles, desc in widgets)

    body = f"""
                <div class="page-header">
                    <div class="header-content">
                        <h1>Beep Widget System</h1>
                        <p class="subtitle">Self-contained, fully themed display/input widgets built on BaseControl. Drop any widget on a form, set a style, and it paints itself.</p>
                        <div class="header-badges">
                            <span class="badge badge-stable">Stable</span>
                            <span class="badge badge-version">v1.0.164</span>
                            <span class="badge badge-namespace">TheTechIdea.Beep.Winform.Controls.Widgets</span>
                        </div>
                    </div>
                </div>

                <section id="overview" class="section">
                    <h2>What are Beep Widgets?</h2>
                    <p>Beep Widgets are self-contained, paint-based WinForms controls that deliver rich visual output without child controls. Each widget:</p>
                    <ul>
                        <li>Inherits from <strong>BaseControl</strong> — full theme support, Border, Padding, and Hover states come for free.</li>
                        <li>Has a <strong>Style enum</strong> property that switches the entire visual rendering mode (e.g. <code>MetricWidgetStyle.GaugeMetric</code>).</li>
                        <li>Uses an <strong>IWidgetPainter</strong> / <strong>WidgetContext</strong> pipeline — background, content, and foreground accents are drawn in three separate passes.</li>
                        <li>Fires strongly-typed <strong>BeepEventDataArgs</strong> events for every meaningful user interaction.</li>
                        <li>Re-applies the currently active <strong>BeepTheme</strong> automatically when the application theme changes.</li>
                    </ul>
                    <div class="info-box">
                        <strong>Namespace:</strong> <code>TheTechIdea.Beep.Winform.Controls.Widgets</code><br>
                        <strong>Base class:</strong> <code>BaseControl</code> (→ Control)<br>
                        <strong>Toolbox category:</strong> <em>Beep Widgets</em>
                    </div>
                </section>

                <section id="architecture" class="section">
                    <h2>Architecture</h2>
                    <p>The widget rendering pipeline consists of three layers:</p>
                    <pre><code class="language-csharp">// IWidgetPainter interface
void Initialize(BaseControl owner, IBeepTheme theme);
WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx);
void DrawBackground(Graphics g, WidgetContext ctx);
void DrawContent(Graphics g, WidgetContext ctx);
void DrawForegroundAccents(Graphics g, WidgetContext ctx);
void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action&lt;string, Rectangle&gt; notifyAreaHit);</code></pre>
                    <p>The <code>WidgetContext</code> object carries all layout rectangles, colours, and data needed for a single paint pass. The widget control populates the context from its properties; the current painter reads it.</p>
                    <h3>Painter Selection</h3>
                    <p>When the <code>Style</code> property changes, the widget calls <code>InitializePainter()</code> which instantiates the matching concrete painter class and calls <code>Initialize()</code> followed by <code>Invalidate()</code>.</p>
                    <pre><code class="language-csharp">// Example: changing a metric widget style at runtime
metricWidget.Style = MetricWidgetStyle.GaugeMetric;
// The widget automatically swaps in GaugeMetricPainter and repaints.</code></pre>
                </section>

                <section id="quick-start" class="section">
                    <h2>Quick Start</h2>
                    <pre><code class="language-csharp">// 1. Drop any widget on a form, or create programmatically:
var metric = new BeepMetricWidget
{{
    Style      = MetricWidgetStyle.ValueWithTrend,
    Title      = "Monthly Revenue",
    Value      = "$127,450",
    TrendValue = "+8.3%",
    ShowTrend  = true,
    Size       = new Size(220, 120)
}};
panel.Controls.Add(metric);

// 2. Handle events:
metric.ValueClicked += (s, e) => ShowRevenueDetail();

// 3. Theme changes are picked up automatically via ApplyTheme().</code></pre>

                    <h3>Using a Chart Widget</h3>
                    <pre><code class="language-csharp">var chart = new BeepChartWidget
{{
    Style      = ChartWidgetStyle.LineChart,
    Title      = "Sales Trend",
    Values     = new List&lt;double&gt; {{ 120, 135, 98, 160, 175, 210 }},
    Labels     = new List&lt;string&gt; {{ "Jan","Feb","Mar","Apr","May","Jun" }},
    ShowLegend = true
}};
chart.DataPointClicked += (s, e) => Console.WriteLine(e.Data);</code></pre>
                </section>

                <section id="available-widgets" class="section">
                    <h2>Available Widgets (13)</h2>
                    <div class="related-grid">
{cards}
                    </div>
                </section>

                <section id="best-practices" class="section">
                    <h2>Best Practices</h2>
                    <ul>
                        <li><strong>Set Size before adding to a container</strong> — each widget has a sensible default size but you should adjust it to fit your layout.</li>
                        <li><strong>Use Style enums, not magic numbers</strong> — always assign the strongly-typed enum value.</li>
                        <li><strong>Avoid overlapping widgets</strong> — widgets are GDI+ paint-based and do not clip to parent containers automatically.</li>
                        <li><strong>Let the theme engine drive colours</strong> — avoid hard-coding color properties; most colour properties default to the current theme values.</li>
                        <li><strong>Reuse widget instances</strong> — updating data properties and calling <code>Invalidate()</code> is cheaper than creating a new widget.</li>
                        <li><strong>Handle BeepEventDataArgs.Data</strong> — the <code>Data</code> property contains context-specific clicked-item data (index, value, label, etc.).</li>
                    </ul>
                </section>
"""
    return page("Beep Widget System", "index.html", "Widget Overview", body)


# ═══════════════════════════════════════════════════════════════════════════════
# INDIVIDUAL WIDGET PAGES
# ═══════════════════════════════════════════════════════════════════════════════

def widget_page(
    name: str, href: str, description: str, default_size: str,
    icon: str, category: str,
    styles: list[tuple],
    props: list[tuple],
    events: list[tuple],
    features: list[tuple],
    code_basic: str,
    code_events: str,
    related: list[tuple],
    extra_sections: str = ""
) -> str:

    related_html = "\n".join(
        f'                        <a href="{h}" class="related-card"><h4>{t}</h4><p>{d}</p></a>'
        for t, h, d in related
    )

    body = f"""
                <div class="page-header">
                    <div class="header-content">
                        <h1>{name}</h1>
                        <p class="subtitle">{description}</p>
                        <div class="header-badges">
                            <span class="badge badge-stable">Stable</span>
                            <span class="badge badge-version">v1.0.164</span>
                            <span class="badge badge-namespace">Beep Widgets / {category}</span>
                        </div>
                    </div>
                </div>

                <section id="overview" class="section">
                    <h2>Overview</h2>
                    <p><strong>{name}</strong> is a paint-based WinForms widget that inherits from <strong>BaseControl</strong>. It uses an IWidgetPainter pipeline to render rich visuals without child controls. The <code>Style</code> property selects one of {len(styles)} built-in render modes; changing it at runtime immediately swaps the painter and triggers a repaint.</p>
                    <div class="info-box">
                        <strong>Toolbox icon:</strong> {icon}<br>
                        <strong>Category:</strong> Beep Widgets → {category}<br>
                        <strong>Default size:</strong> {default_size}<br>
                        <strong>Namespace:</strong> TheTechIdea.Beep.Winform.Controls.Widgets
                    </div>
                </section>

                <section id="features" class="section">
                    <h2>Key Features</h2>
{feature_cards(features)}
                </section>

                <section id="styles" class="section">
                    <h2>Style Variants ({len(styles)})</h2>
{styles_table(styles)}
                </section>

                <section id="properties" class="section">
                    <h2>Properties</h2>
{props_table(props)}
                    <p class="note">Inherits all <strong>BaseControl</strong> properties (Border, Padding, Theme colors, etc.).</p>
                </section>

                <section id="events" class="section">
                    <h2>Events</h2>
{events_table(events)}
                </section>

                <section id="basic-usage" class="section">
                    <h2>Basic Usage</h2>
                    <pre><code class="language-csharp">{code_basic}</code></pre>
                </section>

                <section id="event-handling" class="section">
                    <h2>Event Handling</h2>
                    <pre><code class="language-csharp">{code_events}</code></pre>
                </section>
{extra_sections}
                <section id="best-practices" class="section">
                    <h2>Best Practices</h2>
                    <ul>
                        <li>Set <code>Style</code> before adding the widget to the form to avoid double repaints.</li>
                        <li>Adjust <code>Size</code> to fit your layout — defaults are minimum comfortable sizes.</li>
                        <li>Read <code>BeepEventDataArgs.Data</code> in event handlers for context-specific click data.</li>
                        <li>Call <code>Invalidate()</code> after updating data properties to trigger a repaint.</li>
                        <li>Let theme colors flow from <code>BeepStyling.CurrentTheme</code> — avoid overriding color properties unless customization is explicitly desired.</li>
                    </ul>
                </section>

                <section id="related" class="section">
                    <h2>Related Widgets</h2>
                    <div class="related-grid">
{related_html}
                    </div>
                </section>
"""
    return page(name, href, name, body)


# ═══════════════════════════════════════════════════════════════════════════════
# PER-WIDGET DATA
# ═══════════════════════════════════════════════════════════════════════════════

PAGES = {}

# ── BeepMetricWidget ──────────────────────────────────────────────────────────
PAGES["beep-metric-widget.html"] = widget_page(
    name="BeepMetricWidget", href="beep-metric-widget.html",
    description="KPI and metric display widget with six visual styles including trend indicators, progress bars, circular gauges, comparison panels, and card layouts.",
    default_size="200 × 120", icon="📊", category="Data / KPI",
    styles=[
        ("SimpleValue",      "Plain number + label – the most compact display."),
        ("ValueWithTrend",   "Number plus an arrow trend indicator and percentage change."),
        ("ProgressMetric",   "Number with a horizontal progress bar showing completion."),
        ("GaugeMetric",      "Circular arc gauge coloured by value range."),
        ("ComparisonMetric", "Two values rendered side-by-side for direct comparison."),
        ("CardMetric",       "Card layout with an icon, title, and large value."),
    ],
    props=[
        ("Style",           "MetricWidgetStyle", "SimpleValue",  "Selects the active painter / render mode."),
        ("Title",           "string",            "&quot;Metric Title&quot;", "Header label shown above or beside the value."),
        ("Value",           "string",            "&quot;1,234&quot;",        "Primary displayed value (formatted string)."),
        ("Units",           "string",            "&quot;&quot;",             "Optional unit suffix (e.g. &quot;ms&quot;, &quot;%&quot;, &quot;USD&quot;)."),
        ("TrendValue",      "string",            "&quot;+12.5%&quot;",       "Trend label (e.g. &quot;+8.3%&quot;, &quot;-2.1%&quot;)."),
        ("TrendDirection",  "string",            "&quot;up&quot;",           "&quot;up&quot;, &quot;down&quot;, or &quot;neutral&quot; — controls arrow direction and colour."),
        ("TrendPercentage", "double",            "12.5",                    "Numeric percentage used by progress/gauge painters."),
        ("ShowTrend",       "bool",              "false",                   "Show or hide the trend indicator area."),
        ("ShowIcon",        "bool",              "false",                   "Show or hide the icon area (CardMetric style)."),
        ("IconPath",        "string",            "&quot;&quot;",             "Path to icon image rendered in the icon area."),
        ("AccentColor",     "Color",             "(theme primary)",         "Dominant accent colour for bars, arcs, and arrows."),
        ("SuccessColor",    "Color",             "Color(76,175,80)",        "Colour for positive/upward trends."),
        ("WarningColor",    "Color",             "Color(255,193,7)",        "Colour for neutral/warning indicators."),
        ("ErrorColor",      "Color",             "Color(244,67,54)",        "Colour for negative/downward trends."),
    ],
    events=[
        ("ValueClicked",  "BeepEventDataArgs", "Fired when the user clicks the value area."),
        ("TrendClicked",  "BeepEventDataArgs", "Fired when the user clicks the trend indicator."),
        ("IconClicked",   "BeepEventDataArgs", "Fired when the user clicks the icon area."),
    ],
    features=[
        ("📊", "Six Visual Modes",    "From a single plain number to a full card layout with icon — one property switch."),
        ("📈", "Trend Indicators",    "Arrow icons with positive/negative colours and percentage labels."),
        ("⭕", "Circular Gauge",      "Smooth arc gauge drawn entirely in GDI+ — no external charting library needed."),
        ("🎨", "Theme-Aware Colors",  "AccentColor, SuccessColor, and ErrorColor default to the active BeepTheme."),
        ("🔔", "Click Events",        "Separate events for value area, trend area, and icon clicks."),
        ("📐", "Compact Default",     "200×120 default — fits in tight dashboard grids."),
    ],
    code_basic="""var metric = new BeepMetricWidget
{
    Style         = MetricWidgetStyle.ValueWithTrend,
    Title         = "Monthly Revenue",
    Value         = "$127,450",
    TrendValue    = "+8.3%",
    TrendDirection = "up",
    ShowTrend     = true,
    Size          = new Size(220, 120)
};
this.Controls.Add(metric);""",
    code_events="""metric.ValueClicked += (s, e) =>
{
    // e.Data contains the current Value string
    ShowRevenueBreakdown(metric.Value);
};

metric.TrendClicked += (s, e) =>
{
    ShowTrendHistory();
};""",
    related=[
        ("BeepChartWidget",     "beep-chart-widget.html",     "Pair with a chart for full KPI+trend display."),
        ("BeepDashboardWidget", "beep-dashboard-widget.html", "Composite panel showing multiple metrics."),
        ("BeepListWidget",      "beep-list-widget.html",      "Show the data series behind the metric."),
    ],
)

# ── BeepChartWidget ───────────────────────────────────────────────────────────
PAGES["beep-chart-widget.html"] = widget_page(
    name="BeepChartWidget", href="beep-chart-widget.html",
    description="Chart and data-visualization widget with seven styles: bar, line, pie/donut, gauge, sparkline, heatmap, and combination charts — all rendered in pure GDI+.",
    default_size="300 × 200", icon="📈", category="Data / Charts",
    styles=[
        ("BarChart",         "Vertical or horizontal bar chart with optional gridlines."),
        ("LineChart",        "Smooth line or filled area chart for trend data."),
        ("PieChart",         "Pie or donut chart with optional legend."),
        ("GaugeChart",       "Speedometer-style gauge for a single KPI value."),
        ("Sparkline",        "Miniature inline trend line — ideal in tight layouts."),
        ("HeatmapChart",     "Calendar or grid heatmap for frequency/intensity data."),
        ("CombinationChart", "Bar + line combination for comparing two series."),
    ],
    props=[
        ("Style",      "ChartWidgetStyle",  "BarChart",        "Selects the chart type and painter."),
        ("Title",      "string",            "&quot;Chart Title&quot;", "Optional header label drawn above the chart area."),
        ("Values",     "List&lt;double&gt;","[10,25,30…]",    "Primary data series."),
        ("Labels",     "List&lt;string&gt;","[Jan,Feb…]",     "Category labels aligned with Values."),
        ("Colors",     "List&lt;Color&gt;", "(theme palette)", "Per-series or per-segment colour list."),
        ("ShowLegend", "bool",              "true",            "Show/hide the legend panel."),
        ("ShowGrid",   "bool",              "true",            "Show/hide background gridlines."),
        ("MinValue",   "double",            "0",               "Y-axis minimum (bar and line charts)."),
        ("MaxValue",   "double",            "100",             "Y-axis maximum (bar and line charts)."),
        ("AccentColor","Color",             "(theme primary)", "Dominant accent used for single-series charts."),
    ],
    events=[
        ("ChartClicked",     "BeepEventDataArgs", "Fired when the chart background area is clicked."),
        ("DataPointClicked", "BeepEventDataArgs", "Fired when a bar, segment, or data point is clicked. Data contains index."),
        ("LegendClicked",    "BeepEventDataArgs", "Fired when a legend item is clicked. Data contains series label."),
    ],
    features=[
        ("📊", "Seven Chart Types",   "Bar, line, pie, gauge, sparkline, heatmap, combination — all via one Style enum."),
        ("🎨", "Theme Palette",       "Colors defaults to the active theme's ChartDefaultSeriesColors list."),
        ("🖱️", "Hit-test Events",    "DataPointClicked carries the clicked series index for drill-down navigation."),
        ("📐", "Gridlines & Legend",  "ShowGrid and ShowLegend toggles with no extra configuration needed."),
        ("⚡", "Pure GDI+",           "No third-party charting library — lightweight and always theme-consistent."),
        ("🔢", "Axis Control",        "MinValue / MaxValue let you fix axis extents for meaningful comparisons."),
    ],
    code_basic="""var chart = new BeepChartWidget
{
    Style      = ChartWidgetStyle.LineChart,
    Title      = "Weekly Sales",
    Values     = new List<double> { 120, 135, 98, 160, 175, 210, 195 },
    Labels     = new List<string> { "Mon","Tue","Wed","Thu","Fri","Sat","Sun" },
    ShowLegend = false,
    ShowGrid   = true,
    Size       = new Size(360, 220)
};
this.Controls.Add(chart);""",
    code_events="""chart.DataPointClicked += (s, e) =>
{
    int idx = (int)e.Data;  // index into Values/Labels
    MessageBox.Show($"Clicked: {chart.Labels[idx]} = {chart.Values[idx]}");
};""",
    related=[
        ("BeepMetricWidget",    "beep-metric-widget.html",    "Pair with a metric card for the headline KPI."),
        ("BeepDashboardWidget", "beep-dashboard-widget.html", "ChartGrid dashboard shows multiple small charts."),
        ("BeepListWidget",      "beep-list-widget.html",      "Show the raw data table alongside the chart."),
    ],
)

# ── BeepListWidget ────────────────────────────────────────────────────────────
PAGES["beep-list-widget.html"] = widget_page(
    name="BeepListWidget", href="beep-list-widget.html",
    description="List and table widget with six display styles: activity feed, data table, ranking list, status list, profile list, and to-do/task list.",
    default_size="300 × 250", icon="📋", category="Data / Lists",
    styles=[
        ("ActivityFeed", "Timeline-style activity log with timestamps and icons."),
        ("DataTable",    "Structured multi-column data table with sortable headers."),
        ("RankingList",  "Numbered ranking list with value badges."),
        ("StatusList",   "Items each showing a colour-coded status indicator."),
        ("ProfileList",  "User/contact listing with avatar placeholder and role subtitle."),
        ("TaskList",     "Checklist with checkbox toggle and completion strikethrough."),
    ],
    props=[
        ("Style",            "ListWidgetStyle",     "ActivityFeed",  "Selects the list painter."),
        ("Title",            "string",              "&quot;List Title&quot;", "Header text drawn above the list."),
        ("Items",            "List&lt;ListItem&gt;","(3 sample rows)","Data collection. Each ListItem has Id, Title, Subtitle, Status, Timestamp, IsCompleted."),
        ("Columns",          "List&lt;string&gt;",  "[Name,Value,Status]","Column headers shown in DataTable style."),
        ("ShowHeader",       "bool",                "true",          "Show/hide the list header row."),
        ("AllowSelection",   "bool",                "true",          "Enable row click selection."),
        ("SelectedIndex",    "int",                 "-1",            "Currently selected row index."),
        ("MaxVisibleItems",  "int",                 "10",            "Number of rows visible before internal scrolling."),
        ("AccentColor",      "Color",               "(theme primary)","Accent used for status dots, badges, and selected rows."),
    ],
    events=[
        ("ItemClicked",   "BeepEventDataArgs", "Fired on any list item click. Data contains the clicked ListItem."),
        ("ItemSelected",  "BeepEventDataArgs", "Fired when selection changes. Data contains the newly selected ListItem."),
        ("HeaderClicked", "BeepEventDataArgs", "Fired when a column header is clicked (DataTable style). Data contains column name."),
    ],
    features=[
        ("📋", "Six List Styles",  "Activity feeds to ranked tables — flip a property to change the entire layout."),
        ("✅", "Task Style",       "TaskList style renders checkboxes and strikethrough text for to-do items."),
        ("🔵", "Status Indicators","StatusList style renders per-item colour dots linked to the item's Status string."),
        ("👤", "Profile Rows",     "ProfileList renders avatars and role subtitles for contact/team lists."),
        ("🖱️", "Row Selection",   "AllowSelection enables highlighted row selection with ItemSelected events."),
        ("🔢", "Scrollable",      "MaxVisibleItems clips the visible area; internal painted scroll handles overflow."),
    ],
    code_basic="""var list = new BeepListWidget
{
    Style = ListWidgetStyle.StatusList,
    Title = "Service Status",
    Items = new List<ListItem>
    {
        new ListItem { Title = "API Gateway",   Status = "Online",  Subtitle = "99.9% uptime" },
        new ListItem { Title = "Database",      Status = "Online",  Subtitle = "12ms avg" },
        new ListItem { Title = "Cache Layer",   Status = "Warning", Subtitle = "High memory" },
        new ListItem { Title = "Mail Service",  Status = "Offline", Subtitle = "Investigating" },
    },
    Size = new Size(320, 220)
};
this.Controls.Add(list);""",
    code_events="""list.ItemClicked += (s, e) =>
{
    var item = (ListItem)e.Data;
    ShowServiceDetail(item.Title);
};

list.ItemSelected += (s, e) =>
{
    var item = (ListItem)e.Data;
    statusBar.Text = $"Selected: {item.Title} — {item.Status}";
};""",
    related=[
        ("BeepMetricWidget",  "beep-metric-widget.html",  "Show aggregated KPIs from the same data."),
        ("BeepChartWidget",   "beep-chart-widget.html",   "Visualise the list data as a chart."),
        ("BeepNotificationWidget","beep-notification-widget.html","Alert users when list item status changes."),
    ],
)

# ── BeepDashboardWidget ───────────────────────────────────────────────────────
PAGES["beep-dashboard-widget.html"] = widget_page(
    name="BeepDashboardWidget", href="beep-dashboard-widget.html",
    description="Composite dashboard widget that arranges multiple KPIs, charts, and timelines in a single panel with six layout styles.",
    default_size="400 × 300", icon="🗂️", category="Dashboard",
    styles=[
        ("MultiMetric",    "Grid of metric cards (Revenue, Users, Orders, Growth)."),
        ("ChartGrid",      "Grid of small charts each representing a different metric."),
        ("TimelineView",   "Chronological event/activity timeline."),
        ("ComparisonGrid", "Side-by-side metric comparisons with delta indicators."),
        ("StatusOverview", "System-health style dashboard with service status indicators."),
        ("AnalyticsPanel", "Complex analytics layout with chart + metric summary."),
    ],
    props=[
        ("Style",             "DashboardWidgetStyle","MultiMetric",     "Selects the dashboard layout painter."),
        ("Title",             "string",              "&quot;Dashboard&quot;","Header text for the panel."),
        ("Metrics",           "List&lt;DashboardMetric&gt;","(4 sample)","Data items. Each DashboardMetric has Title, Value, Trend, Color."),
        ("Columns",           "int",                 "2",               "Number of columns in metric/chart grid layouts."),
        ("Rows",              "int",                 "2",               "Number of rows in metric/chart grid layouts."),
        ("ShowTitle",         "bool",                "true",            "Show/hide the panel title bar."),
        ("GradientDirection", "LinearGradientMode",  "Vertical",        "Direction of the panel background gradient."),
        ("AccentColor",       "Color",               "(theme primary)", "Primary accent colour used across metric cards."),
        ("CardBackColor",     "Color",               "(theme card)",    "Background colour for each inner metric card."),
    ],
    events=[
        ("MetricClicked", "BeepEventDataArgs", "Fired when a metric card is clicked. Data contains the DashboardMetric."),
        ("PanelClicked",  "BeepEventDataArgs", "Fired when the dashboard panel background is clicked."),
    ],
    features=[
        ("🗂️", "Six Layouts",       "Grid, timeline, comparison, and analytics — all from one widget."),
        ("📊", "DashboardMetric",   "Simple DashboardMetric model (Title, Value, Trend, Color) populates all styles."),
        ("🎨", "Gradient Panels",   "Built-in LinearGradient support for the panel background."),
        ("🖱️", "Card Click Events","MetricClicked fires with the specific DashboardMetric the user clicked."),
        ("📐", "Configurable Grid", "Columns and Rows properties control the metric/chart grid density."),
        ("🔄", "Live Update",       "Update Metrics collection and call Invalidate() to refresh in place."),
    ],
    code_basic="""var dash = new BeepDashboardWidget
{
    Style   = DashboardWidgetStyle.MultiMetric,
    Title   = "Q4 KPIs",
    Columns = 2,
    Metrics = new List<DashboardMetric>
    {
        new DashboardMetric { Title = "Revenue", Value = "$127K",  Trend = "+12%", Color = Color.Green  },
        new DashboardMetric { Title = "Users",   Value = "23,456", Trend = "+8%",  Color = Color.Blue   },
        new DashboardMetric { Title = "Orders",  Value = "1,234",  Trend = "-2%",  Color = Color.Red    },
        new DashboardMetric { Title = "Growth",  Value = "15.7%",  Trend = "+5%",  Color = Color.Orange },
    },
    Size = new Size(420, 260)
};
this.Controls.Add(dash);""",
    code_events="""dash.MetricClicked += (s, e) =>
{
    var m = (DashboardMetric)e.Data;
    DrillDown(m.Title);
};""",
    related=[
        ("BeepMetricWidget", "beep-metric-widget.html", "Individual KPI card for single-metric focus."),
        ("BeepChartWidget",  "beep-chart-widget.html",  "Add a standalone chart alongside the dashboard."),
        ("BeepListWidget",   "beep-list-widget.html",   "Detail list to pair with the summary dashboard."),
    ],
)

# ── BeepControlWidget ─────────────────────────────────────────────────────────
PAGES["beep-control-widget.html"] = widget_page(
    name="BeepControlWidget", href="beep-control-widget.html",
    description="Interactive input-control widget with ten styles: toggle switches, sliders, dropdowns, date pickers, search boxes, button groups, checkbox groups, range selectors, colour pickers, and number spinners.",
    default_size="240 × 80", icon="🎛️", category="Input Controls",
    styles=[
        ("ToggleSwitch",   "Animated on/off toggle with label."),
        ("Slider",         "Horizontal range slider with min/max labels."),
        ("DropdownFilter", "Styled dropdown for list filtering."),
        ("DatePicker",     "Date-selection control with calendar pop-out."),
        ("SearchBox",      "Search input with magnifier icon and suggestion support."),
        ("ButtonGroup",    "Radio-style grouped button selection."),
        ("CheckboxGroup",  "Multiple checkbox list with individual labels."),
        ("RangeSelector",  "Dual-handle range selector for min/max input."),
        ("ColorPicker",    "Colour swatch picker with custom color option."),
        ("NumberSpinner",  "Numeric up/down spinner with typed input."),
    ],
    props=[
        ("Style",           "ControlWidgetStyle", "ToggleSwitch",  "Selects the input control painter."),
        ("Title",           "string",             "&quot;Control Title&quot;","Label drawn above or beside the control."),
        ("IsEnabled",       "bool",               "true",          "Enable/disable the interactive control."),
        ("ShowLabel",       "bool",               "true",          "Show/hide the title label."),
        ("ToggleValue",     "ToggleValue",         "(IsOn=false)",  "Toggle state for ToggleSwitch style."),
        ("SliderValue",     "SliderValue",         "(Value=50)",    "Slider position for Slider style."),
        ("RangeValue",      "RangeValue",          "(Min=0,Max=100)","Range handles for RangeSelector style."),
        ("DateValue",       "DateValue",           "(DateTime.Now)","Selected date for DatePicker style."),
        ("NumberValue",     "NumberValue",         "(Value=0)",     "Current number for NumberSpinner style."),
        ("Options",         "List&lt;string&gt;",  "[Option 1…]",   "List of options for Dropdown and ButtonGroup."),
        ("SelectedOption",  "string",              "&quot;&quot;",  "Currently selected option string."),
        ("SearchText",      "string",              "&quot;&quot;",  "Current search query text."),
        ("CheckboxOptions", "List&lt;CheckboxOption&gt;","(3 items)","Options list for CheckboxGroup."),
    ],
    events=[
        ("ValueChanged",  "BeepEventDataArgs", "Fired when the control value changes. Data contains new value."),
        ("ControlClicked","BeepEventDataArgs", "Fired on any click within the control area."),
        ("OptionSelected","BeepEventDataArgs", "Fired when an option is selected (Dropdown, ButtonGroup, Checkbox). Data contains selected option."),
    ],
    features=[
        ("🎛️", "Ten Input Styles",  "From a simple toggle to a full range-selector — one control, ten behaviours."),
        ("✅", "Strongly-Typed Values","ToggleValue, SliderValue, RangeValue, DateValue, NumberValue models carry all state."),
        ("🔔", "ValueChanged Event","Universal ValueChanged event fires regardless of style; Data carries new value."),
        ("🎨", "Theme Integration", "Button, checkbox, and input colours all default to active theme tokens."),
        ("🚫", "IsEnabled",         "Disabled state can be toggled at runtime; painters render the greyed-out style."),
        ("🖱️", "OptionSelected",   "Dropdown and ButtonGroup fire OptionSelected with the chosen option string."),
    ],
    code_basic="""// Toggle switch
var toggle = new BeepControlWidget
{
    Style      = ControlWidgetStyle.ToggleSwitch,
    Title      = "Dark Mode",
    ToggleValue = { IsOn = false },
    Size       = new Size(200, 60)
};

// Dropdown filter
var dropdown = new BeepControlWidget
{
    Style          = ControlWidgetStyle.DropdownFilter,
    Title          = "Filter by Status",
    Options        = new List<string> { "All", "Active", "Inactive", "Pending" },
    SelectedOption = "All",
    Size           = new Size(220, 70)
};
this.Controls.Add(toggle);
this.Controls.Add(dropdown);""",
    code_events="""toggle.ValueChanged += (s, e) =>
{
    bool isOn = (bool)e.Data;
    ApplyDarkMode(isOn);
};

dropdown.OptionSelected += (s, e) =>
{
    string selected = (string)e.Data;
    FilterGrid(selected);
};""",
    related=[
        ("BeepFormWidget",         "beep-form-widget.html",         "Group multiple control widgets in a form layout."),
        ("BeepNotificationWidget", "beep-notification-widget.html", "Show validation feedback after control input."),
        ("BeepNavigationWidget",   "beep-navigation-widget.html",   "Pair with navigation for multi-step forms."),
    ],
)

# ── BeepNotificationWidget ────────────────────────────────────────────────────
PAGES["beep-notification-widget.html"] = widget_page(
    name="BeepNotificationWidget", href="beep-notification-widget.html",
    description="Alert and notification widget with ten styles: toast, banner, progress alert, status card, message centre, system alert, validation message, info panel, warning badge, and success banner.",
    default_size="350 × 80", icon="🔔", category="Alerts / Notifications",
    styles=[
        ("ToastNotification", "Floating toast message with icon and optional dismiss button."),
        ("AlertBanner",       "Full-width coloured banner for important messages."),
        ("ProgressAlert",     "Banner with integrated progress bar and status text."),
        ("StatusCard",        "Card with large status icon, title, and detail message."),
        ("MessageCenter",     "Compact message centre showing notification count and preview."),
        ("SystemAlert",       "High-priority system status alert with severity colour."),
        ("ValidationMessage", "Form validation feedback with field-level error/warning/success."),
        ("InfoPanel",         "Informational panel with expandable detail area."),
        ("WarningBadge",      "Small badge/chip suitable for inline warning indicators."),
        ("SuccessBanner",     "Compact success confirmation strip."),
    ],
    props=[
        ("Style",           "NotificationWidgetStyle","ToastNotification","Selects the notification painter."),
        ("NotificationType","NotificationType",        "Info",             "Info, Success, Warning, Error, or Progress — drives icon and colour."),
        ("Title",           "string",                 "&quot;Notification&quot;","Heading text."),
        ("Message",         "string",                 "&quot;This is a notification message&quot;","Body/detail text."),
        ("ActionText",      "string",                 "&quot;Action&quot;","Label on the optional action button."),
        ("IsDismissible",   "bool",                   "true",             "Show/hide the dismiss (×) button."),
        ("ShowIcon",        "bool",                   "true",             "Show/hide the type icon."),
        ("ShowAction",      "bool",                   "false",            "Show/hide the action button."),
        ("Progress",        "int",                    "0",                "0–100 progress value used by ProgressAlert style."),
        ("Timestamp",       "DateTime",               "DateTime.Now",     "Timestamp rendered in MessageCenter and ActivityStream styles."),
    ],
    events=[
        ("NotificationClicked","BeepEventDataArgs","Fired when the notification body is clicked."),
        ("ActionClicked",      "BeepEventDataArgs","Fired when the action button is clicked."),
        ("DismissClicked",     "BeepEventDataArgs","Fired when the dismiss (×) button is clicked."),
    ],
    features=[
        ("🔔", "Ten Notification Styles","Toast, banner, badge, validation — every common alert pattern covered."),
        ("🎨", "Type-Driven Colors",    "NotificationType drives accent colour automatically (green=success, red=error…)."),
        ("📊", "Progress Integration",  "ProgressAlert style shows live progress for background operation feedback."),
        ("✅", "Validation Feedback",   "ValidationMessage style targets form field validation use-cases."),
        ("🚫", "Dismissible",          "IsDismissible toggles the × button; DismissClicked fires when pressed."),
        ("⚡", "Action Button",        "ShowAction + ActionText expose a one-click CTA directly in the notification."),
    ],
    code_basic="""// Inline error for a form field
var validation = new BeepNotificationWidget
{
    Style            = NotificationWidgetStyle.ValidationMessage,
    NotificationType = NotificationType.Error,
    Title            = "Email Invalid",
    Message          = "Please enter a valid email address.",
    IsDismissible    = false,
    ShowIcon         = true,
    Size             = new Size(380, 50)
};
emailPanel.Controls.Add(validation);

// Progress alert during a long operation
var progress = new BeepNotificationWidget
{
    Style            = NotificationWidgetStyle.ProgressAlert,
    NotificationType = NotificationType.Progress,
    Title            = "Importing data...",
    Progress         = 0,
    IsDismissible    = false
};
statusPanel.Controls.Add(progress);
// Update in background worker:
progress.Progress = 45;
progress.Invalidate();""",
    code_events="""notification.ActionClicked += (s, e) => OpenNotificationCenter();
notification.DismissClicked += (s, e) =>
{
    notification.Visible = false;
};""",
    related=[
        ("BeepControlWidget",   "beep-control-widget.html",   "Pair validation with input controls."),
        ("BeepFormWidget",      "beep-form-widget.html",       "Attach validation messages to form sections."),
        ("BeepDashboardWidget", "beep-dashboard-widget.html",  "StatusOverview dashboard for system health."),
    ],
)

# ── BeepNavigationWidget ──────────────────────────────────────────────────────
PAGES["beep-navigation-widget.html"] = widget_page(
    name="BeepNavigationWidget", href="beep-navigation-widget.html",
    description="Navigation widget with ten styles covering breadcrumbs, step indicators, tabs, pagination, menu bars, sidebar navigation, wizard steps, process flows, tree navigation, and quick actions.",
    default_size="400 × 50", icon="🧭", category="Navigation",
    styles=[
        ("Breadcrumb",     "Horizontal breadcrumb trail showing the current page path."),
        ("StepIndicator",  "Numbered step progress bar with completed/active/upcoming states."),
        ("TabContainer",   "Clickable tab bar for switching content panels."),
        ("Pagination",     "Page number bar with prev/next buttons."),
        ("MenuBar",        "Horizontal menu bar with labelled items."),
        ("SidebarNav",     "Vertical sidebar navigation list with optional icons."),
        ("WizardSteps",    "Wizard-style step navigation with state indicators."),
        ("ProcessFlow",    "Left-to-right process milestone flow diagram."),
        ("TreeNavigation", "Collapsible tree-style hierarchical navigation."),
        ("QuickActions",   "Row of icon + label quick-action buttons."),
    ],
    props=[
        ("Style",        "NavigationWidgetStyle","Breadcrumb","Selects the navigation painter."),
        ("Items",        "List&lt;NavigationItem&gt;","(sample items)","Navigation items. Each has Id, Label, Icon, IsActive, IsEnabled, ParentId."),
        ("CurrentIndex", "int",                  "0",          "Index of the active/selected item."),
        ("ShowIcons",    "bool",                 "true",       "Show/hide item icons."),
        ("IsHorizontal", "bool",                 "true",       "Horizontal or vertical layout (Sidebar uses vertical)."),
        ("Title",        "string",               "&quot;Navigation&quot;","Header label for sidebar and tree styles."),
        ("AccentColor",  "Color",                "(theme primary)","Active item highlight colour."),
    ],
    events=[
        ("ItemClicked",       "BeepEventDataArgs","Fired when any navigation item is clicked. Data contains NavigationItem."),
        ("NavigationChanged", "BeepEventDataArgs","Fired when CurrentIndex changes. Data contains new index."),
    ],
    features=[
        ("🧭", "Ten Navigation Styles","Every navigation pattern from breadcrumb to tree — one widget."),
        ("✅", "Step Progress",       "StepIndicator and WizardSteps track completed/active/pending visually."),
        ("🌳", "Tree Support",        "TreeNavigation supports hierarchical items via NavigationItem.ParentId."),
        ("⚡", "QuickActions",        "Row of icon+label buttons for toolbar-style quick access."),
        ("🖱️", "NavigationChanged",  "Fires whenever the active item changes — no plumbing needed."),
        ("🔢", "Pagination",          "Pagination style includes prev/next buttons and numbered page buttons."),
    ],
    code_basic="""// Wizard step indicator
var wizard = new BeepNavigationWidget
{
    Style        = NavigationWidgetStyle.WizardSteps,
    CurrentIndex = 1,  // currently on step 2 (0-based)
    Items = new List<NavigationItem>
    {
        new NavigationItem { Label = "Account",  IsActive = false },
        new NavigationItem { Label = "Profile",  IsActive = true  },
        new NavigationItem { Label = "Payment",  IsActive = false },
        new NavigationItem { Label = "Confirm",  IsActive = false },
    },
    Size = new Size(480, 60)
};
this.Controls.Add(wizard);""",
    code_events="""wizard.NavigationChanged += (s, e) =>
{
    int step = (int)e.Data;
    ShowStepPanel(step);
};""",
    related=[
        ("BeepFormWidget",   "beep-form-widget.html",   "Pair WizardSteps with FormStep for multi-step forms."),
        ("BeepControlWidget","beep-control-widget.html","QuickActions complement control panels."),
        ("BeepListWidget",   "beep-list-widget.html",   "TreeNavigation pairs well with a detail list."),
    ],
)

# ── BeepMediaWidget ───────────────────────────────────────────────────────────
PAGES["beep-media-widget.html"] = widget_page(
    name="BeepMediaWidget", href="beep-media-widget.html",
    description="Media display widget with ten styles: image cards, avatar groups, icon cards, galleries, profile cards, image overlays, photo grids, media viewers, avatar lists, and icon grids.",
    default_size="280 × 200", icon="🖼️", category="Media / Images",
    styles=[
        ("ImageCard",    "Card with a background image and an overlay text strip."),
        ("AvatarGroup",  "Stacked circular avatar thumbnails for group display."),
        ("IconCard",     "Large centred icon with a title and description below."),
        ("MediaGallery", "Horizontally scrollable image carousel."),
        ("ProfileCard",  "User profile card with photo, name, role, and stats."),
        ("ImageOverlay", "Full-bleed image with a semi-transparent text overlay."),
        ("PhotoGrid",    "Masonry-style grid of photo thumbnails."),
        ("MediaViewer",  "Single large media item with title and metadata."),
        ("AvatarList",   "Vertical list of avatars with name and subtitle labels."),
        ("IconGrid",     "Grid of icons with labels — ideal for feature showcases."),
    ],
    props=[
        ("Style",        "MediaWidgetStyle","ImageCard",       "Selects the media painter."),
        ("Title",        "string",          "&quot;Media Title&quot;","Primary caption or heading."),
        ("Subtitle",     "string",          "&quot;Subtitle&quot;",   "Secondary caption."),
        ("ImagePath",    "string",          "&quot;&quot;",            "File path to the primary display image."),
        ("Image",        "Image",           "null",            "Directly assigned Image object (takes priority over ImagePath)."),
        ("MediaItems",   "List&lt;MediaItem&gt;","(empty)",    "Collection for gallery, grid, and group styles."),
        ("ShowOverlay",  "bool",            "true",            "Show/hide the text overlay on ImageCard/ImageOverlay."),
        ("OverlayText",  "string",          "&quot;Overlay Text&quot;","Text rendered in the overlay strip."),
        ("AccentColor",  "Color",           "(theme primary)",  "Accent for icon colours and selection borders."),
    ],
    events=[
        ("MediaClicked",  "BeepEventDataArgs","Fired when anywhere on the widget is clicked."),
        ("ImageClicked",  "BeepEventDataArgs","Fired when a specific image/thumbnail is clicked. Data contains MediaItem or index."),
        ("AvatarClicked", "BeepEventDataArgs","Fired when an avatar is clicked in AvatarGroup or AvatarList styles."),
        ("OverlayClicked","BeepEventDataArgs","Fired when the overlay text area is clicked."),
    ],
    features=[
        ("🖼️", "Ten Media Styles",  "Images, galleries, avatars, icons — one widget covers every media type."),
        ("🎭", "Overlay Support",   "Built-in text overlay with ShowOverlay toggle for ImageCard/ImageOverlay."),
        ("👤", "Avatar Clusters",   "AvatarGroup renders stacked circular thumbnails with overflow count badge."),
        ("🎨", "Theme Consistent",  "Border, accent, and hover colours derive automatically from the active theme."),
        ("🖱️", "Per-Item Events",  "ImageClicked and AvatarClicked carry individual item data for drill-down."),
        ("📐", "Gallery & Grid",    "MediaGallery and PhotoGrid handle multi-image layouts internally."),
    ],
    code_basic="""// Profile card
var profile = new BeepMediaWidget
{
    Style    = MediaWidgetStyle.ProfileCard,
    Title    = "Sarah Connor",
    Subtitle = "Lead Designer",
    ImagePath = "avatars/sarah.png",
    Size     = new Size(240, 180)
};

// Icon grid
var icons = new BeepMediaWidget
{
    Style      = MediaWidgetStyle.IconGrid,
    Title      = "Quick Access",
    MediaItems = new List<MediaItem>
    {
        new MediaItem { Title = "Reports",    IconPath = "icons/report.svg"    },
        new MediaItem { Title = "Settings",   IconPath = "icons/settings.svg"  },
        new MediaItem { Title = "Team",       IconPath = "icons/team.svg"      },
        new MediaItem { Title = "Analytics",  IconPath = "icons/analytics.svg" },
    },
    Size = new Size(320, 160)
};""",
    code_events="""icons.ImageClicked += (s, e) =>
{
    var item = (MediaItem)e.Data;
    NavigateTo(item.Title);
};""",
    related=[
        ("BeepSocialWidget",  "beep-social-widget.html",  "Full ProfileCard social widget with stats."),
        ("BeepListWidget",    "beep-list-widget.html",    "ProfileList renders rows of avatars with details."),
        ("BeepFormWidget",    "beep-form-widget.html",    "Pair with InputCard for photo-upload forms."),
    ],
)

# ── BeepFinanceWidget ─────────────────────────────────────────────────────────
PAGES["beep-finance-widget.html"] = widget_page(
    name="BeepFinanceWidget", href="beep-finance-widget.html",
    description="Financial data widget with ten styles covering portfolio cards, crypto stats, transaction history, balance cards, financial charts, payment methods, investment tracking, expense categories, revenue cards, and budget progress.",
    default_size="300 × 180", icon="💰", category="Finance",
    styles=[
        ("PortfolioCard",  "Investment portfolio with asset allocation and overall return."),
        ("CryptoWidget",   "Cryptocurrency price and 24h change display."),
        ("TransactionCard","Financial transaction list with credit/debit indicators."),
        ("BalanceCard",    "Account balance with income and expense summary."),
        ("FinancialChart", "Specialised financial candlestick or line chart."),
        ("PaymentCard",    "Payment method card (masked card number, expiry)."),
        ("InvestmentCard", "Single investment holding with gain/loss indicator."),
        ("ExpenseCard",    "Expense category breakdown with pie/progress sub-display."),
        ("RevenueCard",    "Revenue tracking with period-over-period comparison."),
        ("BudgetWidget",   "Budget progress bars with spent vs allocated amounts."),
    ],
    props=[
        ("Style",           "FinanceWidgetStyle","PortfolioCard","Selects the finance painter."),
        ("Title",           "string",            "&quot;Finance Widget&quot;","Card heading."),
        ("Subtitle",        "string",            "&quot;Financial Data&quot;","Secondary heading or account name."),
        ("PrimaryValue",    "decimal",           "0",            "Main financial figure (balance, price, amount)."),
        ("SecondaryValue",  "decimal",           "0",            "Supplementary figure (e.g. invested amount)."),
        ("Percentage",      "decimal",           "0",            "Percentage change or allocation."),
        ("Currency",        "string",            "&quot;USD&quot;","ISO currency code."),
        ("CurrencySymbol",  "string",            "&quot;$&quot;", "Symbol prepended to values."),
        ("PositiveColor",   "Color",             "Color(34,139,34)","Colour for gains / positive values."),
        ("NegativeColor",   "Color",             "Color(220,20,60)","Colour for losses / negative values."),
        ("NeutralColor",    "Color",             "Color(128,128,128)","Colour for neutral/zero values."),
    ],
    events=[
        ("ValueClicked",      "BeepEventDataArgs","Fired when the primary value is clicked."),
        ("CardClicked",       "BeepEventDataArgs","Fired when the card body is clicked."),
        ("ActionClicked",     "BeepEventDataArgs","Fired when an action button (e.g. 'View Details') is clicked."),
    ],
    features=[
        ("💰", "Ten Finance Styles","Portfolio to budget — every common fintech card pattern."),
        ("🎨", "Positive/Negative Colors","Automatic green/red colouring driven by value sign."),
        ("💱", "Currency Support","Currency symbol and code properties for localised display."),
        ("📊", "Financial Chart","FinancialChart style renders candlestick or line overlays via GDI+."),
        ("📈", "Budget Progress","BudgetWidget style shows horizontal bars for each budget category."),
        ("🔐", "Masked Card","PaymentCard style renders masked card numbers (•••• •••• •••• 1234)."),
    ],
    code_basic="""var balance = new BeepFinanceWidget
{
    Style          = FinanceWidgetStyle.BalanceCard,
    Title          = "Checking Account",
    Subtitle       = "•••• 4521",
    PrimaryValue   = 12_450.75m,
    SecondaryValue = 3_200.00m,   // monthly income
    CurrencySymbol = "$",
    Size           = new Size(300, 160)
};

var budget = new BeepFinanceWidget
{
    Style        = FinanceWidgetStyle.BudgetWidget,
    Title        = "October Budget",
    PrimaryValue = 2400m,   // total budget
    Percentage   = 67m,     // percent used
    Size         = new Size(320, 200)
};""",
    code_events="""balance.CardClicked += (s, e) => OpenAccountDetails();
balance.ActionClicked += (s, e) => OpenTransferDialog();""",
    related=[
        ("BeepMetricWidget", "beep-metric-widget.html", "Pair with a KPI card for financial headlines."),
        ("BeepChartWidget",  "beep-chart-widget.html",  "Add a line chart to show historical price data."),
        ("BeepListWidget",   "beep-list-widget.html",   "Show a transaction list alongside the balance card."),
    ],
)

# ── BeepFormWidget ────────────────────────────────────────────────────────────
PAGES["beep-form-widget.html"] = widget_page(
    name="BeepFormWidget", href="beep-form-widget.html",
    description="Form layout widget with ten styles: field groups, validation panels, form sections, input cards, multi-step form steps, fieldsets, inline/compact layouts, validated inputs, and form summaries.",
    default_size="360 × 280", icon="📝", category="Forms",
    styles=[
        ("FieldGroup",      "Grouped set of related labelled input fields."),
        ("ValidationPanel", "Form section with live validation state indicators."),
        ("FormSection",     "Titled section dividing a long form into logical parts."),
        ("InputCard",       "Single-input styled card — ideal for prominent single fields."),
        ("FormStep",        "One step in a multi-step form with step number and title."),
        ("FieldSet",        "Traditional HTML-style fieldset with legend."),
        ("InlineForm",      "Horizontal side-by-side label/input layout."),
        ("CompactForm",     "Space-efficient stacked layout for dense UIs."),
        ("ValidatedInput",  "Single input with inline validation message below."),
        ("FormSummary",     "Read-only summary of submitted form data."),
    ],
    props=[
        ("Style",       "FormWidgetStyle","FieldGroup",    "Selects the form layout painter."),
        ("Title",       "string",         "&quot;Form Widget&quot;","Section or card heading."),
        ("Subtitle",    "string",         "&quot;Data Entry&quot;", "Secondary sub-heading."),
        ("Description", "string",         "&quot;&quot;",           "Helper text below the title."),
        ("Fields",      "List&lt;FormField&gt;","(empty)",   "Form fields. Each FormField has Label, Value, Type, IsRequired, IsValid, ErrorMessage."),
        ("IsValid",     "bool",           "true",           "Overall form validity state."),
        ("ShowRequired","bool",           "true",           "Show asterisks on required fields."),
        ("ShowValidation","bool",         "true",           "Show validation icons/messages."),
        ("AccentColor", "Color",          "(theme primary)","Active/focused field border colour."),
        ("ValidColor",  "Color",          "(theme success)","Border/icon colour for valid fields."),
        ("InvalidColor","Color",          "(theme error)",  "Border/icon colour for invalid fields."),
    ],
    events=[
        ("FieldClicked",   "BeepEventDataArgs","Fired when a field area is clicked. Data contains FormField."),
        ("SubmitClicked",  "BeepEventDataArgs","Fired when a submit button (if rendered) is clicked."),
        ("FieldChanged",   "BeepEventDataArgs","Fired when a field value changes. Data contains FormField with new value."),
    ],
    features=[
        ("📝", "Ten Form Styles",      "From compact inline to multi-step wizard — every form layout covered."),
        ("✅", "Validation Display",   "ValidatedInput and ValidationPanel render per-field error messages."),
        ("🔢", "FormStep",             "FormStep style integrates with BeepNavigationWidget WizardSteps for multi-step flows."),
        ("📋", "FormSummary",          "Read-only summary style for review-before-submit screens."),
        ("🎨", "Theme tokens",         "ValidColor, InvalidColor, and AccentColor default to active theme tokens."),
        ("⭐", "Required Indicators",  "ShowRequired renders asterisks on required fields automatically."),
    ],
    code_basic="""var form = new BeepFormWidget
{
    Style    = FormWidgetStyle.ValidationPanel,
    Title    = "User Registration",
    Fields   = new List<FormField>
    {
        new FormField { Label = "First Name", Value = "", IsRequired = true  },
        new FormField { Label = "Last Name",  Value = "", IsRequired = true  },
        new FormField { Label = "Email",      Value = "", IsRequired = true,
                        IsValid = false, ErrorMessage = "Enter a valid email" },
        new FormField { Label = "Phone",      Value = "", IsRequired = false },
    },
    Size = new Size(380, 280)
};
this.Controls.Add(form);""",
    code_events="""form.FieldClicked += (s, e) =>
{
    var field = (FormField)e.Data;
    FocusNativeInput(field.Label);
};

form.SubmitClicked += (s, e) => ValidateAndSubmit();""",
    related=[
        ("BeepControlWidget",      "beep-control-widget.html",      "Use BeepControlWidget for individual interactive inputs."),
        ("BeepNavigationWidget",   "beep-navigation-widget.html",   "Pair FormStep with WizardSteps navigation."),
        ("BeepNotificationWidget", "beep-notification-widget.html", "Show ValidationMessage for field-level errors."),
    ],
)

# ── BeepSocialWidget ──────────────────────────────────────────────────────────
PAGES["beep-social-widget.html"] = widget_page(
    name="BeepSocialWidget", href="beep-social-widget.html",
    description="Social interaction widget with ten styles: profile cards, team grids, message cards, activity streams, user lists, chat interfaces, comment threads, social feeds, user stats, and contact cards.",
    default_size="300 × 200", icon="👥", category="Social",
    styles=[
        ("ProfileCard",    "User profile with avatar, name, role, and social stats."),
        ("TeamMembers",    "Grid of team member avatar thumbnails with name chips."),
        ("MessageCard",    "Single message bubble with sender avatar and timestamp."),
        ("ActivityStream", "Chronological social activity feed."),
        ("UserList",       "Scrollable contact/user list with online indicators."),
        ("ChatWidget",     "Chat interface with sent/received message bubbles."),
        ("CommentThread",  "Threaded comment display with reply indentation."),
        ("SocialFeed",     "Social media style post card with like/share counts."),
        ("UserStats",      "User statistics card (posts, followers, following, etc.)."),
        ("ContactCard",    "Contact information card with action buttons."),
    ],
    props=[
        ("Style",       "SocialWidgetStyle","ProfileCard","Selects the social painter."),
        ("Title",       "string",           "&quot;Social Widget&quot;","Primary heading or user name."),
        ("Subtitle",    "string",           "&quot;Subtitle&quot;",    "Secondary heading or role."),
        ("UserName",    "string",           "&quot;John Doe&quot;",    "Display name for profile/contact styles."),
        ("UserRole",    "string",           "&quot;Developer&quot;",   "Role or position label."),
        ("AvatarPath",  "string",           "&quot;&quot;",            "Path to avatar image."),
        ("SocialItems", "List&lt;SocialItem&gt;","(empty)",   "Data collection for feed, chat, and user list styles."),
        ("AccentColor", "Color",            "(theme primary)","Accent for action buttons and online indicators."),
    ],
    events=[
        ("ProfileClicked",  "BeepEventDataArgs","Fired when the profile/avatar area is clicked."),
        ("ActionClicked",   "BeepEventDataArgs","Fired when a social action button (Follow, Message, etc.) is clicked."),
        ("ItemClicked",     "BeepEventDataArgs","Fired when a feed post, chat message, or list item is clicked."),
    ],
    features=[
        ("👥", "Ten Social Styles",  "Profile to chat thread — complete social UI toolkit in one widget."),
        ("💬", "Chat Bubbles",       "ChatWidget renders sent/received message bubbles with timestamps."),
        ("📊", "UserStats",          "Stats card shows followers, following, posts with numerical badges."),
        ("🟢", "Online Indicators",  "UserList style renders live online status dots per user."),
        ("🖱️", "Action Buttons",    "ProfileCard and ContactCard styles include Follow/Message action buttons."),
        ("🌊", "Activity Stream",    "ActivityStream renders a chronological feed with type icons."),
    ],
    code_basic="""var profile = new BeepSocialWidget
{
    Style      = SocialWidgetStyle.ProfileCard,
    UserName   = "Alex Johnson",
    UserRole   = "Senior Engineer",
    AvatarPath = "avatars/alex.png",
    Size       = new Size(260, 200)
};

var chat = new BeepSocialWidget
{
    Style       = SocialWidgetStyle.ChatWidget,
    Title       = "Support Chat",
    SocialItems = new List<SocialItem>
    {
        new SocialItem { Text = "Hello, how can I help?",  IsSent = false, Sender = "Support" },
        new SocialItem { Text = "I need help with login.", IsSent = true,  Sender = "You"     },
    },
    Size = new Size(340, 260)
};""",
    code_events="""profile.ActionClicked  += (s, e) => SendFollowRequest(profile.UserName);
profile.ProfileClicked += (s, e) => OpenUserProfile(profile.UserName);""",
    related=[
        ("BeepMediaWidget", "beep-media-widget.html", "Avatar groups and profile photos."),
        ("BeepListWidget",  "beep-list-widget.html",  "ProfileList for a leaner user listing."),
        ("BeepNotificationWidget","beep-notification-widget.html","Activity notification banners."),
    ],
)

# ── BeepMapWidget ─────────────────────────────────────────────────────────────
PAGES["beep-map-widget.html"] = widget_page(
    name="BeepMapWidget", href="beep-map-widget.html",
    description="Location and mapping widget with ten styles: live tracking, route display, location cards, geographic heatmaps, address cards, map previews, location lists, travel cards, region maps, and place cards.",
    default_size="320 × 220", icon="🗺️", category="Maps / Location",
    styles=[
        ("LiveTracking",       "Real-time dot-on-map location tracker."),
        ("RouteDisplay",       "Step-by-step route path visualisation."),
        ("LocationCard",       "Card showing address, city, country with a pin icon."),
        ("GeographicHeatmap",  "Grid/region heatmap coloured by data density."),
        ("AddressCard",        "Formatted postal address with copy action."),
        ("MapPreview",         "Small static map preview placeholder with marker."),
        ("LocationList",       "Scrollable list of locations with distance badges."),
        ("TravelCard",         "Origin-to-destination travel card with transport mode."),
        ("RegionMap",          "Simplified regional/country map with data overlay."),
        ("PlaceCard",          "Place/venue card with name, photo, category, and rating."),
    ],
    props=[
        ("Style",      "MapWidgetStyle","LocationCard","Selects the map painter."),
        ("Title",      "string",        "&quot;Map Widget&quot;","Card heading."),
        ("Subtitle",   "string",        "&quot;Location Data&quot;","Secondary heading."),
        ("Address",    "string",        "&quot;&quot;",            "Street address."),
        ("City",       "string",        "&quot;&quot;",            "City name."),
        ("Country",    "string",        "&quot;&quot;",            "Country name."),
        ("Latitude",   "double",        "0.0",          "Decimal latitude for map styles."),
        ("Longitude",  "double",        "0.0",          "Decimal longitude for map styles."),
        ("Locations",  "List&lt;LocationItem&gt;","(empty)","Multiple locations for list/route styles."),
        ("AccentColor","Color",         "(theme primary)","Pin and accent colour."),
    ],
    events=[
        ("LocationClicked","BeepEventDataArgs","Fired when a map marker or location item is clicked. Data contains LocationItem."),
        ("CardClicked",    "BeepEventDataArgs","Fired when the card body is clicked."),
        ("ActionClicked",  "BeepEventDataArgs","Fired when an action button (e.g. 'Get Directions') is clicked."),
    ],
    features=[
        ("🗺️", "Ten Location Styles","From a simple address card to a geographic heatmap — one widget."),
        ("📍", "Static Map Preview","MapPreview renders a styled placeholder map background with a pin marker."),
        ("🌡️", "Geographic Heatmap","GeographicHeatmap colours a grid by value intensity."),
        ("🛣️", "Route Display",    "RouteDisplay renders an origin→destination path with waypoints."),
        ("📋", "Location List",    "LocationList shows multiple locations with distance and direction badges."),
        ("🏨", "Place Cards",      "PlaceCard renders venue name, category, and star rating."),
    ],
    code_basic="""var location = new BeepMapWidget
{
    Style    = MapWidgetStyle.LocationCard,
    Title    = "Our Office",
    Address  = "123 Tech Avenue",
    City     = "San Francisco",
    Country  = "USA",
    Latitude  = 37.7749,
    Longitude = -122.4194,
    Size     = new Size(300, 160)
};

var route = new BeepMapWidget
{
    Style     = MapWidgetStyle.TravelCard,
    Title     = "Delivery Route",
    Locations = new List<LocationItem>
    {
        new LocationItem { Name = "Warehouse",    City = "Oakland"       },
        new LocationItem { Name = "Distribution", City = "San Francisco" },
        new LocationItem { Name = "Customer",     City = "Palo Alto"     },
    },
    Size = new Size(340, 200)
};""",
    code_events="""location.ActionClicked += (s, e) => OpenMapsApp(location.Latitude, location.Longitude);
route.LocationClicked  += (s, e) =>
{
    var loc = (LocationItem)e.Data;
    ShowLocationDetail(loc.Name);
};""",
    related=[
        ("BeepListWidget",    "beep-list-widget.html",   "LocationList — a list of locations with details."),
        ("BeepMetricWidget",  "beep-metric-widget.html", "Pair with KPI cards for delivery/logistics dashboards."),
        ("BeepSocialWidget",  "beep-social-widget.html", "ContactCard includes location information."),
    ],
)

# ── BeepCalendarWidget ────────────────────────────────────────────────────────
PAGES["beep-calendar-widget.html"] = widget_page(
    name="BeepCalendarWidget", href="beep-calendar-widget.html",
    description="Calendar and scheduling widget with ten styles: date grid, time slots, event cards, full calendar view, schedule cards, date pickers, timeline views, week views, event lists, and availability grids.",
    default_size="340 × 280", icon="📅", category="Calendar / Scheduling",
    styles=[
        ("DateGrid",         "Standard monthly calendar grid with date cells."),
        ("TimeSlots",        "Available time slot picker with bookable slots."),
        ("EventCard",        "Single event display card with title, time, and location."),
        ("CalendarView",     "Full month view with event chips on day cells."),
        ("ScheduleCard",     "Daily schedule list with time-sorted appointments."),
        ("DatePicker",       "Inline date-selection picker (no popup)."),
        ("TimelineView",     "Horizontal or vertical timeline of events."),
        ("WeekView",         "Seven-column weekly calendar grid."),
        ("EventList",        "Upcoming events list sorted by date."),
        ("AvailabilityGrid", "Grid showing available/busy/blocked time slots for booking."),
    ],
    props=[
        ("Style",        "CalendarWidgetStyle","CalendarView","Selects the calendar painter."),
        ("Title",        "string",             "&quot;Calendar&quot;","Panel heading."),
        ("Subtitle",     "string",             "&quot;Schedule&quot;","Secondary sub-heading."),
        ("SelectedDate", "DateTime",           "DateTime.Now","Currently selected/focused date."),
        ("DisplayMonth", "DateTime",           "DateTime.Now","Month displayed in CalendarView/WeekView."),
        ("Events",       "List&lt;CalendarEvent&gt;","(empty)", "Event collection. Each CalendarEvent has Title, StartTime, EndTime, Color, Location."),
        ("MinDate",      "DateTime",           "DateTime.MinValue","Minimum selectable date."),
        ("MaxDate",      "DateTime",           "DateTime.MaxValue","Maximum selectable date."),
        ("ShowWeekNumbers","bool",             "false",           "Show ISO week numbers in CalendarView."),
        ("AccentColor",  "Color",              "(theme primary)", "Highlighted date and event chip colour."),
    ],
    events=[
        ("DateClicked",      "BeepEventDataArgs","Fired when a date cell is clicked. Data contains DateTime."),
        ("EventClicked",     "BeepEventDataArgs","Fired when an event chip is clicked. Data contains CalendarEvent."),
        ("SlotClicked",      "BeepEventDataArgs","Fired when a time slot is clicked (TimeSlots / AvailabilityGrid). Data contains slot DateTime."),
        ("MonthChanged",     "BeepEventDataArgs","Fired when navigation changes the displayed month. Data contains new DateTime."),
    ],
    features=[
        ("📅", "Ten Calendar Styles","From a date picker to a full weekly grid — one widget, ten views."),
        ("⏰", "Time Slots",         "TimeSlots style renders bookable time blocks with availability indicators."),
        ("📋", "Event Chips",        "CalendarView and WeekView render per-event coloured chips on day cells."),
        ("🗓️", "Week View",         "Seven-column grid with hourly time slots for appointment scheduling."),
        ("✅", "Availability Grid",  "AvailabilityGrid shows available/busy/blocked states for resource booking."),
        ("🎨", "Per-Event Colors",   "Each CalendarEvent carries its own Color — colour-code by category."),
    ],
    code_basic="""var cal = new BeepCalendarWidget
{
    Style        = CalendarWidgetStyle.CalendarView,
    SelectedDate = DateTime.Today,
    DisplayMonth = DateTime.Today,
    Events       = new List<CalendarEvent>
    {
        new CalendarEvent { Title = "Sprint Planning", StartTime = DateTime.Today.AddHours(9),
                            EndTime = DateTime.Today.AddHours(11), Color = Color.SteelBlue },
        new CalendarEvent { Title = "Design Review",  StartTime = DateTime.Today.AddDays(2).AddHours(14),
                            EndTime = DateTime.Today.AddDays(2).AddHours(15), Color = Color.MediumPurple },
    },
    Size = new Size(380, 300)
};
this.Controls.Add(cal);""",
    code_events="""cal.DateClicked += (s, e) =>
{
    DateTime date = (DateTime)e.Data;
    ShowEventsForDate(date);
};

cal.EventClicked += (s, e) =>
{
    var evt = (CalendarEvent)e.Data;
    OpenEventEditor(evt);
};

cal.MonthChanged += (s, e) =>
{
    DateTime newMonth = (DateTime)e.Data;
    LoadEventsForMonth(newMonth);
};""",
    related=[
        ("BeepNavigationWidget",   "beep-navigation-widget.html",   "Pair with StepIndicator for booking workflows."),
        ("BeepNotificationWidget", "beep-notification-widget.html", "Alert before upcoming events."),
        ("BeepListWidget",         "beep-list-widget.html",         "EventList-style list alongside the calendar grid."),
    ],
)

# ═══════════════════════════════════════════════════════════════════════════════
# Write all pages
# ═══════════════════════════════════════════════════════════════════════════════
def main():
    os.makedirs(DOCS_DIR, exist_ok=True)

    all_pages = {"index.html": gen_index()}
    all_pages.update(PAGES)

    for filename, html in all_pages.items():
        out = os.path.join(DOCS_DIR, filename)
        with open(out, "w", encoding="utf-8") as f:
            f.write(html)
        print(f"  WRITTEN: {filename}")

    print(f"\nDone — {len(all_pages)} files written to {DOCS_DIR}")

if __name__ == "__main__":
    main()
