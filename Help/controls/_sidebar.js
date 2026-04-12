// _sidebar.js — Single source of truth for the documentation sidebar.
// Path-aware: detects controls/ vs widgets/ context and prefixes links correctly.
// Accordion click handlers are wired after injection.
// Search filters visible submenu items.
(function () {
    // When loaded inside an iframe (the shell page), hide sidebar and fix layout
    var _isIframe = false;
    try { _isIframe = window.self !== window.top; } catch (e) { _isIframe = true; }
    if (_isIframe) {
        var s = document.createElement('style');
        s.textContent = '.content{margin-left:0!important}.sidebar,#sidebar-placeholder,.mobile-menu-toggle,.theme-toggle{display:none!important}';
        (document.head || document.documentElement).appendChild(s);
        return;
    }
    // ── Directory-context detection ──────────────────────────────────────────
    var _p = window.location.pathname;
    var _inWidgets  = /\/widgets\//i.test(_p);
    var _inControls = /\/controls\//i.test(_p);

    // C = prefix for control-page links  ('' in controls/, '../controls/' in widgets/, 'controls/' at root)
    var C = _inWidgets  ? '../controls/' : (_inControls ? '' : 'controls/');
    // W = prefix for widget-page links   ('../widgets/' in controls/, '' in widgets/, 'widgets/' at root)
    var W = _inControls ? '../widgets/'  : (_inWidgets  ? '' : 'widgets/');
    // R = prefix for root pages          ('../' in any subfolder)
    var R = (_inControls || _inWidgets) ? '../' : '';

    function nav() {
        return ''
        + '<aside class="sidebar" id="sidebar">'
        + '<div class="logo">'
        +   '<img src="' + R + 'assets/beep-logo.svg" alt="Beep Controls Logo">'
        +   '<div class="logo-text"><h2>Beep Controls</h2><span class="version">v1.0.164</span></div>'
        + '</div>'
        + '<div class="search-container">'
        +   '<input type="text" class="search-input" id="sidebar-search-input" placeholder="Search documentation...">'
        + '</div>'
        + '<nav><ul class="nav-menu">'

        // ── Top-level home ────────────────────────────────────────────────────
        + '<li><a href="' + R + 'index.html"><i class="bi bi-house"></i> Home</a></li>'

        // ── Getting Started (pages not yet created) ───────────────────────────
        + '<li class="has-submenu"><a href="#"><i class="bi bi-rocket"></i> Getting Started</a><ul class="submenu">'
        + '<li><a href="#">Installation Guide</a></li>'
        + '<li><a href="#">Quick Start Tutorial</a></li>'
        + '<li><a href="#">Theming System</a></li>'
        + '<li><a href="#">Migration Guide</a></li>'
        + '</ul></li>'

        // ── Input Controls ────────────────────────────────────────────────────
        + '<li class="has-submenu"><a href="#"><i class="bi bi-input-cursor-text"></i> Input Controls</a><ul class="submenu">'
        + '<li><a href="' + C + 'beep-button.html">BeepButton</a></li>'
        + '<li><a href="' + C + 'beep-textbox.html">BeepTextBox</a></li>'
        + '<li><a href="' + C + 'beep-combobox.html">BeepComboBox</a></li>'
        + '<li><a href="' + C + 'beep-checkbox.html">BeepCheckBox</a></li>'
        + '<li><a href="' + C + 'beep-radiobutton.html">BeepRadioGroup</a></li>'
        + '<li><a href="' + C + 'beep-datepicker.html">BeepDatePicker</a></li>'
        + '<li><a href="' + C + 'beep-numericupdown.html">BeepNumericUpDown</a></li>'
        + '<li><a href="' + C + 'beep-switch.html">BeepSwitch</a></li>'
        + '<li><a href="' + C + 'beep-select.html">BeepSelect</a></li>'
        + '<li><a href="' + C + 'beep-listofvaluesbox.html">BeepListofValuesBox</a></li>'
        + '</ul></li>'

        // ── Button Variants ───────────────────────────────────────────────────
        + '<li class="has-submenu"><a href="#"><i class="bi bi-hand-index"></i> Button Variants</a><ul class="submenu">'
        + '<li><a href="' + C + 'beep-circularbutton.html">BeepCircularButton</a></li>'
        + '<li><a href="' + C + 'beep-chevronbutton.html">BeepChevronButton</a></li>'
        + '<li><a href="' + C + 'beep-extendedbutton.html">BeepExtendedButton</a></li>'
        + '<li><a href="' + C + 'beep-advancedbutton.html">BeepAdvancedButton</a></li>'
        + '</ul></li>'

        // ── Display Controls ──────────────────────────────────────────────────
        + '<li class="has-submenu"><a href="#"><i class="bi bi-display"></i> Display Controls</a><ul class="submenu">'
        + '<li><a href="' + C + 'beep-label.html">BeepLabel</a></li>'
        + '<li><a href="' + C + 'beep-image.html">BeepImage</a></li>'
        + '<li><a href="' + C + 'beep-progressbar.html">BeepProgressBar</a></li>'
        + '<li><a href="' + C + 'beep-shape.html">BeepShape</a></li>'
        + '<li><a href="' + C + 'beep-starrating.html">BeepStarRating</a></li>'
        + '<li><a href="' + C + 'beep-marquee.html">BeepMarquee</a></li>'
        + '<li><a href="' + C + 'beep-testimonial.html">BeepTestimonial</a></li>'
        + '<li><a href="' + C + 'beep-dualpercentagecontrol.html">BeepDualPercentage</a></li>'
        + '</ul></li>'

        // ── Layout Controls ───────────────────────────────────────────────────
        + '<li class="has-submenu"><a href="#"><i class="bi bi-layout-split"></i> Layout Controls</a><ul class="submenu">'
        + '<li><a href="' + C + 'beep-panel.html">BeepPanel</a></li>'
        + '<li><a href="' + C + 'beep-card.html">BeepCard</a></li>'
        + '<li><a href="' + C + 'beep-tabcontrol.html">BeepTabs</a></li>'
        + '<li><a href="' + C + 'beep-accordion.html">BeepAccordionMenu</a></li>'
        + '<li><a href="' + C + 'beep-multisplitter.html">BeepMultiSplitter</a></li>'
        + '<li><a href="' + C + 'beep-breadcrumps.html">BeepBreadcrumps</a></li>'
        + '<li><a href="' + C + 'beep-stepper.html">BeepStepperBar</a></li>'
        + '<li><a href="' + C + 'beep-stepperbreadcrumb.html">BeepStepperBreadCrumb</a></li>'
        + '<li><a href="' + C + 'beep-scrollbar.html">BeepScrollBar</a></li>'
        + '</ul></li>'

        // ── Data Controls ─────────────────────────────────────────────────────
        + '<li class="has-submenu"><a href="#"><i class="bi bi-bar-chart"></i> Data Controls</a><ul class="submenu">'
        + '<li><a href="' + C + 'beep-grid.html">BeepSimpleGrid</a></li>'
        + '<li><a href="' + C + 'beep-chart.html">BeepChart</a></li>'
        + '<li><a href="' + C + 'beep-statcard.html">BeepStatCard</a></li>'
        + '<li><a href="' + C + 'beep-metrictile.html">BeepMetricTile</a></li>'
        + '<li><a href="' + C + 'beep-tree.html">BeepTree</a></li>'
        + '<li><a href="' + C + 'beep-listbox.html">BeepListBox</a></li>'
        + '<li><a href="' + C + 'beep-datanavigator.html">BeepDataNavigator</a></li>'
        + '<li><a href="' + C + 'beep-filter.html">BeepFilter</a></li>'
        + '<li><a href="' + C + 'beep-bindingnavigator.html">BeepBindingNavigator</a></li>'
        + '<li><a href="' + C + 'beep-verticaltable.html">BeepVerticalTable</a></li>'
        + '</ul></li>'

        // ── Cards & Project ───────────────────────────────────────────────────
        + '<li class="has-submenu"><a href="#"><i class="bi bi-card-list"></i> Cards &amp; Project</a><ul class="submenu">'
        + '<li><a href="' + C + 'beep-taskcard.html">BeepTaskCard</a></li>'
        + '<li><a href="' + C + 'beep-featurecard.html">BeepFeatureCard</a></li>'
        + '<li><a href="' + C + 'beep-companyprofile.html">BeepCompanyProfile</a></li>'
        + '<li><a href="' + C + 'beep-tasklistitem.html">BeepTaskListItem</a></li>'
        + '</ul></li>'

        // ── Application Structure ─────────────────────────────────────────────
        + '<li class="has-submenu"><a href="#"><i class="bi bi-window-stack"></i> Application Structure</a><ul class="submenu">'
        + '<li><a href="' + C + 'beep-menubar.html">BeepMenuBar</a></li>'
        + '<li><a href="' + C + 'beep-toolstrip.html">BeepToolStrip</a></li>'
        + '<li><a href="' + C + 'beep-appbar.html">BeepAppBar</a></li>'
        + '<li><a href="' + C + 'beep-sidemenu.html">BeepSideMenu</a></li>'
        + '<li><a href="' + C + 'beep-functionspanel.html">BeepFunctionsPanel</a></li>'
        + '<li><a href="' + C + 'beep-form-ui-manager.html">BeepFormUIManager</a></li>'
        + '<li><a href="' + C + 'beep-multichipgroup.html">BeepMultiChipGroup</a></li>'
        + '</ul></li>'

        // ── Menus & Navigation ────────────────────────────────────────────────
        + '<li class="has-submenu"><a href="#"><i class="bi bi-list-nested"></i> Menus &amp; Navigation</a><ul class="submenu">'
        + '<li><a href="' + C + 'beep-dropdownmenu.html">BeepDropDownMenu</a></li>'
        + '<li><a href="' + C + 'beep-flyoutmenu.html">BeepFlyoutMenu</a></li>'
        + '<li><a href="' + C + 'beep-contextmenustrip.html">BeepContextMenu</a></li>'
        + '</ul></li>'

        // ── Dialogs & Popups ──────────────────────────────────────────────────
        + '<li class="has-submenu"><a href="#"><i class="bi bi-chat-square"></i> Dialogs &amp; Popups</a><ul class="submenu">'
        + '<li><a href="' + C + 'beep-dialogbox.html">BeepDialogBox</a></li>'
        + '<li><a href="' + C + 'beep-popupform.html">BeepPopupForm</a></li>'
        + '<li><a href="' + C + 'beep-filedialog.html">BeepFileDialog</a></li>'
        + '<li><a href="' + C + 'beep-wait.html">BeepWait</a></li>'
        + '<li><a href="' + C + 'beep-splashscreen.html">BeepSplashScreen</a></li>'
        + '</ul></li>'

        // ── Specialized Controls ──────────────────────────────────────────────
        + '<li class="has-submenu"><a href="#"><i class="bi bi-tools"></i> Specialized Controls</a><ul class="submenu">'
        + '<li><a href="' + C + 'beep-login.html">BeepLogin</a></li>'
        + '<li><a href="' + C + 'beep-wizard.html">BeepWizard</a></li>'
        + '<li><a href="' + C + 'beep-calendar.html">BeepCalendarView</a></li>'
        + '<li><a href="' + C + 'beep-toggle.html">BeepToggle</a></li>'
        + '<li><a href="' + C + 'beep-themes-manager.html">BeepThemesManager</a></li>'
        + '<li><a href="' + C + 'beep-tooltip.html">BeepToolTip</a></li>'
        + '<li><a href="' + C + 'beep-styling.html">Styling System</a></li>'
        + '<li><a href="' + C + 'beep-filter.html">Filtering System</a></li>'
        + '<li><a href="' + C + 'beep-font-management.html">Font Management</a></li>'
        + '</ul></li>'

        // ── Widgets ───────────────────────────────────────────────────────────
        + '<li class="has-submenu"><a href="#"><i class="bi bi-grid-1x2"></i> Widgets</a><ul class="submenu">'
        + '<li><a href="' + W + 'index.html">Widget Overview</a></li>'
        + '<li><a href="' + W + 'beep-metric-widget.html">BeepMetricWidget</a></li>'
        + '<li><a href="' + W + 'beep-chart-widget.html">BeepChartWidget</a></li>'
        + '<li><a href="' + W + 'beep-list-widget.html">BeepListWidget</a></li>'
        + '<li><a href="' + W + 'beep-dashboard-widget.html">BeepDashboardWidget</a></li>'
        + '<li><a href="' + W + 'beep-control-widget.html">BeepControlWidget</a></li>'
        + '<li><a href="' + W + 'beep-notification-widget.html">BeepNotificationWidget</a></li>'
        + '<li><a href="' + W + 'beep-navigation-widget.html">BeepNavigationWidget</a></li>'
        + '<li><a href="' + W + 'beep-media-widget.html">BeepMediaWidget</a></li>'
        + '<li><a href="' + W + 'beep-finance-widget.html">BeepFinanceWidget</a></li>'
        + '<li><a href="' + W + 'beep-form-widget.html">BeepFormWidget</a></li>'
        + '<li><a href="' + W + 'beep-social-widget.html">BeepSocialWidget</a></li>'
        + '<li><a href="' + W + 'beep-map-widget.html">BeepMapWidget</a></li>'
        + '<li><a href="' + W + 'beep-calendar-widget.html">BeepCalendarWidget</a></li>'
        + '</ul></li>'

        // ── Guides & Examples (pages not yet created) ─────────────────────────
        + '<li class="has-submenu"><a href="#"><i class="bi bi-book"></i> Guides &amp; Examples</a><ul class="submenu">'
        + '<li><a href="#">Basic Examples</a></li>'
        + '<li><a href="#">Advanced Examples</a></li>'
        + '<li><a href="#">Complete Applications</a></li>'
        + '<li><a href="#">Best Practices</a></li>'
        + '<li><a href="#">Performance Guide</a></li>'
        + '<li><a href="#">Accessibility Guide</a></li>'
        + '</ul></li>'

        // ── API Reference (page not yet created) ──────────────────────────────
        + '<li><a href="#"><i class="bi bi-code-square"></i> API Reference</a></li>'

        + '</ul></nav></aside>';
    }

    function initSidebar() {
        var placeholder = document.getElementById('sidebar-placeholder');
        if (!placeholder) return;

        // ── Inject sidebar HTML ──────────────────────────────────────────────
        var tmp = document.createElement('div');
        tmp.innerHTML = nav();
        placeholder.parentNode.replaceChild(tmp.firstChild, placeholder);

        // ── Mark active link; open its parent submenu ────────────────────────
        var currentFile = window.location.pathname.split('/').pop() || 'index.html';
        document.querySelectorAll('aside.sidebar a').forEach(function (a) {
            var href = (a.getAttribute('href') || '');
            if (!href || href === '#') return;
            if (href.split('/').pop() === currentFile) {
                a.classList.add('active');
                var parentGroup = a.closest('.has-submenu');
                if (parentGroup) parentGroup.classList.add('open');
            }
        });

        // ── Accordion: click handler for every group header ──────────────────
        document.querySelectorAll('aside.sidebar .has-submenu > a').forEach(function (a) {
            a.addEventListener('click', function (e) {
                if ((this.getAttribute('href') || '#') === '#') e.preventDefault();
                var li = this.parentElement;
                var wasOpen = li.classList.contains('open');
                // Close all other open groups
                document.querySelectorAll('aside.sidebar .has-submenu.open').forEach(function (el) {
                    if (el !== li) el.classList.remove('open');
                });
                // Toggle this one
                li.classList.toggle('open', !wasOpen);
            });
        });

        // ── Sidebar search ───────────────────────────────────────────────────
        var inp = document.getElementById('sidebar-search-input');
        if (inp) {
            inp.addEventListener('input', function () {
                var q = this.value.trim().toLowerCase();
                document.querySelectorAll('aside.sidebar .submenu li').forEach(function (li) {
                    li.style.display = (!q || li.textContent.toLowerCase().includes(q)) ? '' : 'none';
                });
                if (q) {
                    document.querySelectorAll('aside.sidebar .has-submenu').forEach(function (grp) {
                        var visible = Array.from(grp.querySelectorAll('.submenu li')).some(function (li) {
                            return li.style.display !== 'none';
                        });
                        grp.classList.toggle('open', visible);
                    });
                }
            });
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSidebar);
    } else {
        initSidebar();
    }
})();
