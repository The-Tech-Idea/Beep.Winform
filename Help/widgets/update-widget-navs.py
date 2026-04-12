#!/usr/bin/env python3
"""
update-widget-navs.py
Regenerates the <aside class="sidebar"> block in every widget HTML page
in Documentation/widgets/.

Run from the Documentation/widgets/ directory:
    python update-widget-navs.py

Behaviour:
  - Skips files starting with '_' (templates)
  - Skips non-HTML files (e.g. .py, .md)
  - Replaces the <aside class="sidebar"...>...</aside> block each page
    with the NEW_SIDEBAR_TEMPLATE below, marking the current page as active
    and opening its parent submenu section
"""

import os, re

DOCS_DIR = os.path.dirname(os.path.abspath(__file__))

NEW_SIDEBAR_TEMPLATE = """\
        <aside class="sidebar" id="sidebar">
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
                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-window-stack"></i> Widgets</a>
                        <ul class="submenu">
                            <li><a href="index.html">Widget Overview</a></li>
                            <li><a href="beep-metric-widget.html">BeepMetricWidget</a></li>
                            <li><a href="beep-chart-widget.html">BeepChartWidget</a></li>
                            <li><a href="beep-list-widget.html">BeepListWidget</a></li>
                            <li><a href="beep-dashboard-widget.html">BeepDashboardWidget</a></li>
                            <li><a href="beep-control-widget.html">BeepControlWidget</a></li>
                            <li><a href="beep-notification-widget.html">BeepNotificationWidget</a></li>
                            <li><a href="beep-navigation-widget.html">BeepNavigationWidget</a></li>
                            <li><a href="beep-media-widget.html">BeepMediaWidget</a></li>
                            <li><a href="beep-finance-widget.html">BeepFinanceWidget</a></li>
                            <li><a href="beep-form-widget.html">BeepFormWidget</a></li>
                            <li><a href="beep-social-widget.html">BeepSocialWidget</a></li>
                            <li><a href="beep-map-widget.html">BeepMapWidget</a></li>
                            <li><a href="beep-calendar-widget.html">BeepCalendarWidget</a></li>
                        </ul>
                    </li>
                    <li><a href="../api/beep-control-base.html"><i class="bi bi-code-square"></i> API Reference</a></li>
                </ul>
            </nav>
        </aside>"""


def build_sidebar(active_filename: str) -> str:
    sidebar = NEW_SIDEBAR_TEMPLATE
    if active_filename:
        old = f'href="{active_filename}"'
        new = f'href="{active_filename}" class="active"'
        sidebar = sidebar.replace(old, new, 1)

        lines = sidebar.split('\n')
        active_line = next(
            (i for i, ln in enumerate(lines) if f'href="{active_filename}" class="active"' in ln),
            -1
        )
        if active_line >= 0:
            for i in range(active_line - 1, -1, -1):
                if 'class="has-submenu"' in lines[i]:
                    lines[i] = lines[i].replace('class="has-submenu"', 'class="has-submenu open"', 1)
                    break
            sidebar = '\n'.join(lines)
    return sidebar


def find_aside_bounds(content: str):
    start = content.find('<aside class="sidebar"')
    if start == -1:
        return -1, -1
    depth = 0
    i = start
    while i < len(content):
        if content[i:i+6] == '<aside':
            depth += 1
        elif content[i:i+7] == '</aside':
            depth -= 1
            if depth == 0:
                end = content.index('>', i) + 1
                return start, end
        i += 1
    return -1, -1


def process_file(filepath: str):
    fname = os.path.basename(filepath)
    try:
        try:
            with open(filepath, 'r', encoding='utf-8') as f:
                content = f.read()
        except UnicodeDecodeError:
            with open(filepath, 'r', encoding='latin-1') as f:
                content = f.read()

        # Only process sphinx-style pages
        if 'sphinx-style.css' not in content:
            return 'skip'

        start, end = find_aside_bounds(content)
        if start == -1:
            return 'no-aside'

        new_sidebar = build_sidebar(fname)
        new_content = content[:start] + new_sidebar + content[end:]

        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(new_content)
        return 'updated'

    except Exception as exc:
        return f'error:{exc}'


def main():
    html_files = sorted(
        f for f in os.listdir(DOCS_DIR)
        if f.endswith('.html') and not f.startswith('_')
    )
    print(f"Processing {len(html_files)} HTML files in {DOCS_DIR}")

    updated = skipped = errors = 0
    for fname in html_files:
        result = process_file(os.path.join(DOCS_DIR, fname))
        if result == 'updated':
            print(f"  UPDATED: {fname}")
            updated += 1
        elif result.startswith('error:'):
            print(f"  ERROR  : {fname} — {result[6:]}")
            errors += 1
        else:
            print(f"  SKIP   : {fname} ({result})")
            skipped += 1

    print(f"\nSummary:\n  Updated : {updated}\n  Skipped : {skipped}\n  Errors  : {errors}")


if __name__ == "__main__":
    main()
