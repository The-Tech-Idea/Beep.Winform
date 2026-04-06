#!/usr/bin/env python3
"""
update-navs.py  (v2 — DRY navigation)

All pages now reference a single _sidebar.js instead of embedding the nav HTML.
This script:
  - Strips the embedded <aside class="sidebar"...>...</aside> from every HTML file
    and replaces it with <div id="sidebar-placeholder"></div>
  - Adds <script src="_sidebar.js"></script> to <head> if not already present
  - Skips files starting with '_' (templates/scripts)
  - Skips files that do NOT use sphinx-style.css (old-format)
  - To change the nav, edit _sidebar.js — only one place to update.

Run from the Documentation/controls/ directory:
    python update-navs.py
"""
import os
import sys

DOCS_DIR = os.path.dirname(os.path.abspath(__file__))

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

SIDEBAR_JS = '_sidebar.js'
SCRIPT_TAG = f'    <script src="{SIDEBAR_JS}"></script>\n'
PLACEHOLDER = '        <div id="sidebar-placeholder"></div>'


def find_aside_bounds(content: str):
    """Return (start, end) of the first <aside class="sidebar"...>...</aside>."""
    start = content.find('<aside class="sidebar"')
    if start == -1:
        return -1, -1
    depth = 0
    i = start
    while i < len(content):
        if content[i:i+6] == '<aside':
            depth += 1
            i += 6
        elif content[i:i+7] == '</aside':
            depth -= 1
            i += 7
            if depth == 0:
                close_gt = content.find('>', i - 1)
                return start, close_gt + 1
        else:
            i += 1
    return start, -1


def is_sphinx_style(content: str) -> bool:
    return 'sphinx-style.css' in content


def process_file(filepath: str) -> str:
    """Strip embedded <aside> and ensure _sidebar.js is referenced. Returns status string."""
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
    except UnicodeDecodeError:
        try:
            with open(filepath, 'r', encoding='latin-1') as f:
                content = f.read()
        except Exception:
            return f"ERROR (encoding): {os.path.basename(filepath)}"

    if not is_sphinx_style(content):
        return f"SKIP (old format): {os.path.basename(filepath)}"

    changed = False

    # 1) Replace embedded <aside> with a lightweight placeholder div
    if '<aside class="sidebar"' in content:
        start, end = find_aside_bounds(content)
        if start == -1 or end == -1:
            return f"ERROR (aside bounds): {os.path.basename(filepath)}"
        content = content[:start] + PLACEHOLDER + content[end:]
        changed = True

    # 2) Inject <script src="_sidebar.js"> into <head> if not already present
    if SIDEBAR_JS not in content:
        content = content.replace('</head>', SCRIPT_TAG + '</head>', 1)
        changed = True

    if not changed:
        return f"OK (already converted): {os.path.basename(filepath)}"

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)
    return f"UPDATED: {os.path.basename(filepath)}"


def main():
    dry_run = '--dry-run' in sys.argv

    html_files = sorted(
        f for f in os.listdir(DOCS_DIR)
        if f.endswith('.html') and not f.startswith('_')
    )

    print(f"Processing {len(html_files)} HTML files  (nav is in {SIDEBAR_JS})")
    if dry_run:
        print("  ** DRY RUN — no files will be written **\n")

    updated = skipped = errors = 0
    for filename in html_files:
        filepath = os.path.join(DOCS_DIR, filename)
        status = process_file(filepath) if not dry_run else f"DRY-RUN: {filename}"
        print(f"  {status}")
        if 'UPDATED' in status:
            updated += 1
        elif 'SKIP' in status or 'already' in status:
            skipped += 1
        elif 'ERROR' in status:
            errors += 1

    print(f"\nDone: {updated} UPDATED, {skipped} SKIPPED, {errors} ERRORS")
    print(f"  To change the nav, edit {SIDEBAR_JS} only.")


if __name__ == '__main__':
    main()


# ---------------------------------------------------------------------------
# LEGACY — kept for reference, no longer used
# ---------------------------------------------------------------------------
_LEGACY_SIDEBAR_HTML = """\
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

                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-rocket"></i> Getting Started</a>
                        <ul class="submenu">
                            <li><a href="../getting-started/installation.html">Installation Guide</a></li>
                            <li><a href="../getting-started/quick-start.html">Quick Start Tutorial</a></li>
                            <li><a href="../getting-started/theming.html">Theming System</a></li>
                            <li><a href="../getting-started/migration.html">Migration Guide</a></li>
                        </ul>
                    </li>

                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-input-cursor-text"></i> Input Controls</a>
                        <ul class="submenu">
                            <li><a href="beep-button.html">BeepButton</a></li>
                            <li><a href="beep-textbox.html">BeepTextBox</a></li>
                            <li><a href="beep-combobox.html">BeepComboBox</a></li>
                            <li><a href="beep-checkbox.html">BeepCheckBox</a></li>
                            <li><a href="beep-radiobutton.html">BeepRadioGroup</a></li>
                            <li><a href="beep-datepicker.html">BeepDatePicker</a></li>
                            <li><a href="beep-numericupdown.html">BeepNumericUpDown</a></li>
                            <li><a href="beep-switch.html">BeepSwitch</a></li>
                            <li><a href="beep-select.html">BeepSelect</a></li>
                            <li><a href="beep-listofvaluesbox.html">BeepListofValuesBox</a></li>
                        </ul>
                    </li>

                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-hand-index"></i> Button Variants</a>
                        <ul class="submenu">
                            <li><a href="beep-circularbutton.html">BeepCircularButton</a></li>
                            <li><a href="beep-chevronbutton.html">BeepChevronButton</a></li>
                            <li><a href="beep-extendedbutton.html">BeepExtendedButton</a></li>
                            <li><a href="beep-advancedbutton.html">BeepAdvancedButton</a></li>
                        </ul>
                    </li>

                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-display"></i> Display Controls</a>
                        <ul class="submenu">
                            <li><a href="beep-label.html">BeepLabel</a></li>
                            <li><a href="beep-image.html">BeepImage</a></li>
                            <li><a href="beep-progressbar.html">BeepProgressBar</a></li>
                            <li><a href="beep-shape.html">BeepShape</a></li>
                            <li><a href="beep-starrating.html">BeepStarRating</a></li>
                            <li><a href="beep-marquee.html">BeepMarquee</a></li>
                            <li><a href="beep-testimonial.html">BeepTestimonial</a></li>
                            <li><a href="beep-dualpercentagecontrol.html">BeepDualPercentage</a></li>
                        </ul>
                    </li>

                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-layout-split"></i> Layout Controls</a>
                        <ul class="submenu">
                            <li><a href="beep-panel.html">BeepPanel</a></li>
                            <li><a href="beep-card.html">BeepCard</a></li>
                            <li><a href="beep-tabcontrol.html">BeepTabs</a></li>
                            <li><a href="beep-accordion.html">BeepAccordionMenu</a></li>
                            <li><a href="beep-multisplitter.html">BeepMultiSplitter</a></li>
                            <li><a href="beep-breadcrumps.html">BeepBreadcrumps</a></li>
                            <li><a href="beep-stepper.html">BeepSteppperBar</a></li>
                            <li><a href="beep-stepperbreadcrumb.html">BeepStepperBreadCrumb</a></li>
                            <li><a href="beep-scrollbar.html">BeepScrollBar</a></li>
                        </ul>
                    </li>

                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-bar-chart"></i> Data Controls</a>
                        <ul class="submenu">
                            <li><a href="beep-grid.html">BeepSimpleGrid</a></li>
                            <li><a href="beep-chart.html">BeepChart</a></li>
                            <li><a href="beep-statcard.html">BeepStatCard</a></li>
                            <li><a href="beep-metrictile.html">BeepMetricTile</a></li>
                            <li><a href="beep-tree.html">BeepTree</a></li>
                            <li><a href="beep-listbox.html">BeepListBox</a></li>
                            <li><a href="beep-datanavigator.html">BeepDataNavigator</a></li>
                            <li><a href="beep-filter.html">BeepFilter</a></li>
                            <li><a href="beep-bindingnavigator.html">BeepBindingNavigator</a></li>
                            <li><a href="beep-verticaltable.html">BeepVerticalTable</a></li>
                        </ul>
                    </li>

                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-card-list"></i> Cards &amp; Project</a>
                        <ul class="submenu">
                            <li><a href="beep-taskcard.html">BeepTaskCard</a></li>
                            <li><a href="beep-featurecard.html">BeepFeatureCard</a></li>
                            <li><a href="beep-companyprofile.html">BeepCompanyProfile</a></li>
                            <li><a href="beep-tasklistitem.html">BeepTaskListItem</a></li>
                        </ul>
                    </li>

                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-window-stack"></i> Application Structure</a>
                        <ul class="submenu">
                            <li><a href="beep-menubar.html">BeepMenuBar</a></li>
                            <li><a href="beep-toolstrip.html">BeepToolStrip</a></li>
                            <li><a href="beep-appbar.html">BeepAppBar</a></li>
                            <li><a href="beep-sidemenu.html">BeepSideMenu</a></li>
                            <li><a href="beep-functionspanel.html">BeepFunctionsPanel</a></li>
                            <li><a href="beep-form-ui-manager.html">BeepFormUIManager</a></li>
                            <li><a href="beep-multichipgroup.html">BeepMultiChipGroup</a></li>
                        </ul>
                    </li>

                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-list-nested"></i> Menus &amp; Navigation</a>
                        <ul class="submenu">
                            <li><a href="beep-dropdownmenu.html">BeepDropDownMenu</a></li>
                            <li><a href="beep-flyoutmenu.html">BeepFlyoutMenu</a></li>
                            <li><a href="beep-contextmenustrip.html">BeepContextMenu</a></li>
                        </ul>
                    </li>

                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-chat-square"></i> Dialogs &amp; Popups</a>
                        <ul class="submenu">
                            <li><a href="beep-dialogbox.html">BeepDialogBox</a></li>
                            <li><a href="beep-popupform.html">BeepPopupForm</a></li>
                            <li><a href="beep-filedialog.html">BeepFileDialog</a></li>
                            <li><a href="beep-wait.html">BeepWait</a></li>
                            <li><a href="beep-splashscreen.html">BeepSplashScreen</a></li>
                        </ul>
                    </li>

                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-tools"></i> Specialized Controls</a>
                        <ul class="submenu">
                            <li><a href="beep-login.html">BeepLogin</a></li>
                            <li><a href="beep-wizard.html">BeepWizard</a></li>
                            <li><a href="beep-calendar.html">BeepCalendarView</a></li>
                            <li><a href="beep-toggle.html">BeepToggle</a></li>
                            <li><a href="beep-themes-manager.html">BeepThemesManager</a></li>
                        </ul>
                    </li>

                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-window-stack"></i> Widgets</a>
                        <ul class="submenu">
                            <li><a href="../widgets/index.html">Widget Overview</a></li>
                            <li><a href="../widgets/beep-metric-widget.html">BeepMetricWidget</a></li>
                            <li><a href="../widgets/beep-chart-widget.html">BeepChartWidget</a></li>
                            <li><a href="../widgets/beep-list-widget.html">BeepListWidget</a></li>
                            <li><a href="../widgets/beep-dashboard-widget.html">BeepDashboardWidget</a></li>
                            <li><a href="../widgets/beep-control-widget.html">BeepControlWidget</a></li>
                            <li><a href="../widgets/beep-notification-widget.html">BeepNotificationWidget</a></li>
                            <li><a href="../widgets/beep-navigation-widget.html">BeepNavigationWidget</a></li>
                            <li><a href="../widgets/beep-media-widget.html">BeepMediaWidget</a></li>
                            <li><a href="../widgets/beep-finance-widget.html">BeepFinanceWidget</a></li>
                            <li><a href="../widgets/beep-form-widget.html">BeepFormWidget</a></li>
                            <li><a href="../widgets/beep-social-widget.html">BeepSocialWidget</a></li>
                            <li><a href="../widgets/beep-map-widget.html">BeepMapWidget</a></li>
                            <li><a href="../widgets/beep-calendar-widget.html">BeepCalendarWidget</a></li>
                        </ul>
                    </li>

                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-book"></i> Guides &amp; Examples</a>
                        <ul class="submenu">
                            <li><a href="../examples/basic-examples.html">Basic Examples</a></li>
                            <li><a href="../examples/advanced-examples.html">Advanced Examples</a></li>
                            <li><a href="../examples/complete-applications.html">Complete Applications</a></li>
                            <li><a href="../guides/best-practices.html">Best Practices</a></li>
                            <li><a href="../guides/performance.html">Performance Guide</a></li>
                            <li><a href="../guides/accessibility.html">Accessibility Guide</a></li>
                        </ul>
                    </li>

                    <li><a href="../api/beep-control-base.html"><i class="bi bi-code-square"></i> API Reference</a></li>
                </ul>
            </nav>
        </aside>"""
