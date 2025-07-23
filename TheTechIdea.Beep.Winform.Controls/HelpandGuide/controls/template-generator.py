# Control Documentation Template Generator
# Use this script to quickly generate Sphinx-style documentation for remaining controls

# Template for control documentation
CONTROL_TEMPLATE = '''<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>{{CONTROL_NAME}} - Beep Controls Documentation</title>
    <link rel="stylesheet" href="../sphinx-style.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/themes/prism-tomorrow.min.css">
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css">
</head>
<body>
    <!-- Mobile Menu Toggle -->
    <button class="mobile-menu-toggle" onclick="toggleSidebar()">
        <i class="bi bi-list"></i>
    </button>

    <!-- Theme Toggle -->
    <button class="theme-toggle" onclick="toggleTheme()" title="Toggle theme">
        <i class="bi bi-sun-fill" id="theme-icon"></i>
    </button>

    <div class="container">
        <!-- Sidebar -->
        <aside class="sidebar" id="sidebar">
            <div class="logo">
                <img src="../assets/beep-logo.svg" alt="Beep Controls Logo">
                <div class="logo-text">
                    <h2>Beep Controls</h2>
                    <span class="version">v1.0.164</span>
                </div>
            </div>
            
            <!-- Search -->
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
                        </ul>
                    </li>
                    {{NAVIGATION_MENU}}
                    <li><a href="../api/beep-control-base.html"><i class="bi bi-code-square"></i> API Reference</a></li>
                </ul>
            </nav>
        </aside>

        <!-- Main Content -->
        <main class="content">
            <div class="content-wrapper">
                <!-- Breadcrumb -->
                <nav class="breadcrumb-nav">
                    <a href="../index.html">Home</a>
                    <span>›</span>
                    <a href="../index.html#{{CATEGORY_ANCHOR}}">{{CATEGORY_NAME}}</a>
                    <span>›</span>
                    <span>{{CONTROL_NAME}}</span>
                </nav>

                <!-- Page Header -->
                <div class="page-header">
                    <h1>{{CONTROL_NAME}}</h1>
                    <p class="page-subtitle">{{CONTROL_DESCRIPTION}}</p>
                </div>

                <!-- Table of Contents -->
                <div class="toc">
                    <h3>?? Table of Contents</h3>
                    <ul>
                        <li><a href="#overview">Overview</a></li>
                        <li><a href="#features">Key Features</a></li>
                        <li><a href="#basic-usage">Basic Usage</a></li>
                        <li><a href="#properties">Properties</a></li>
                        <li><a href="#examples">Examples</a></li>
                        <li><a href="#theming">Theming</a></li>
                        <li><a href="#best-practices">Best Practices</a></li>
                    </ul>
                </div>

                <!-- Overview -->
                <section id="overview" class="section">
                    <h2>Overview</h2>
                    <p>
                        <strong>{{CONTROL_NAME}}</strong> is {{OVERVIEW_TEXT}}
                    </p>
                    <p>
                        Built on the <code>BeepControl</code> foundation, {{CONTROL_NAME}} provides {{ADDITIONAL_OVERVIEW}}
                    </p>
                </section>

                <!-- Key Features -->
                <section id="features" class="section">
                    <h2>Key Features</h2>
                    <div class="feature-grid">
                        {{FEATURE_CARDS}}
                    </div>
                </section>

                <!-- Basic Usage -->
                <section id="basic-usage" class="section">
                    <h2>Basic Usage</h2>
                    
                    <div class="code-example">
                        <h3>Simple Example</h3>
                        <pre><code class="language-csharp">{{BASIC_USAGE_CODE}}</code></pre>
                    </div>

                    <div class="tip">
                        <strong>?? Quick Tip</strong>
                        <p>{{USAGE_TIP}}</p>
                    </div>
                </section>

                <!-- Properties -->
                <section id="properties" class="section">
                    <h2>Properties</h2>
                    {{PROPERTIES_CONTENT}}
                </section>

                <!-- Examples -->
                <section id="examples" class="section">
                    <h2>Examples</h2>
                    {{EXAMPLES_CONTENT}}
                </section>

                <!-- Theming -->
                <section id="theming" class="section">
                    <h2>Theming</h2>
                    
                    <p>{{CONTROL_NAME}} integrates seamlessly with the Beep theming system, automatically adapting colors, fonts, and visual effects.</p>

                    <div class="code-example">
                        <h3>Applying Themes</h3>
                        <pre><code class="language-csharp">// Apply theme to individual control
control.Theme = "DarkTheme";
control.ApplyTheme();

// Apply theme to all controls in a container
private void ApplyThemeToControls(Control container, string themeName)
{
    foreach (Control control in container.Controls)
    {
        if (control is {{CONTROL_NAME}} beepControl)
        {
            beepControl.Theme = themeName;
            beepControl.ApplyTheme();
        }
        
        if (control.HasChildren)
        {
            ApplyThemeToControls(control, themeName);
        }
    }
}</code></pre>
                    </div>
                </section>

                <!-- Best Practices -->
                <section id="best-practices" class="section">
                    <h2>Best Practices</h2>
                    {{BEST_PRACTICES_CONTENT}}
                </section>

                <!-- Navigation -->
                <div class="nav-links">
                    <a href="../index.html"><i class="bi bi-arrow-left"></i> Back to Controls</a>
                    <a href="{{NEXT_CONTROL}}.html">{{NEXT_CONTROL_NAME}} <i class="bi bi-arrow-right"></i></a>
                </div>

                <!-- Footer -->
                <footer class="documentation-footer">
                    <div class="footer-content">
                        <div class="footer-copyright">
                            <p>&copy; 2024 TheTechIdea - Beep Controls Documentation</p>
                            <p>Supporting .NET 6, 7, 8, and 9 | Windows Forms Applications</p>
                        </div>
                        <div class="footer-links">
                            <a href="../index.html">Home</a>
                            <a href="../getting-started/installation.html">Getting Started</a>
                            <a href="../api/beep-control-base.html">API Reference</a>
                        </div>
                    </div>
                </footer>
            </div>
        </main>
    </div>

    <!-- Scripts -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/components/prism-core.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/prism/1.29.0/plugins/autoloader/prism-autoloader.min.js"></script>
    
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            const submenus = document.querySelectorAll('.has-submenu > a');
            
            submenus.forEach(item => {
                item.addEventListener('click', function(e) {
                    e.preventDefault();
                    const parent = this.parentElement;
                    parent.classList.toggle('open');
                });
            });
        });

        function toggleTheme() {
            const body = document.body;
            const themeIcon = document.getElementById('theme-icon');
            const currentTheme = body.getAttribute('data-theme');
            
            if (currentTheme === 'dark') {
                body.removeAttribute('data-theme');
                themeIcon.className = 'bi bi-sun-fill';
                localStorage.setItem('theme', 'light');
            } else {
                body.setAttribute('data-theme', 'dark');
                themeIcon.className = 'bi bi-moon-fill';
                localStorage.setItem('theme', 'dark');
            }
        }

        document.addEventListener('DOMContentLoaded', function() {
            const savedTheme = localStorage.getItem('theme');
            if (savedTheme === 'dark') {
                document.body.setAttribute('data-theme', 'dark');
                document.getElementById('theme-icon').className = 'bi bi-moon-fill';
            }
        });

        function toggleSidebar() {
            const sidebar = document.getElementById('sidebar');
            sidebar.classList.toggle('open');
        }

        function searchDocs(query) {
            const links = document.querySelectorAll('.nav-menu a');
            const lowerQuery = query.toLowerCase();
            
            links.forEach(link => {
                const text = link.textContent.toLowerCase();
                const listItem = link.closest('li');
                
                if (text.includes(lowerQuery) || lowerQuery === '') {
                    listItem.style.display = '';
                } else {
                    listItem.style.display = 'none';
                }
            });
        }

        document.addEventListener('DOMContentLoaded', function() {
            const codeBlocks = document.querySelectorAll('pre code');
            
            codeBlocks.forEach(block => {
                const button = document.createElement('button');
                button.textContent = 'Copy';
                button.style.cssText = `
                    position: absolute;
                    top: 8px;
                    right: 8px;
                    background: var(--color-brand-primary);
                    color: white;
                    border: none;
                    border-radius: 4px;
                    padding: 4px 8px;
                    font-size: 12px;
                    cursor: pointer;
                    opacity: 0;
                    transition: opacity 0.3s ease;
                `;
                
                const pre = block.parentElement;
                pre.style.position = 'relative';
                pre.appendChild(button);
                
                pre.addEventListener('mouseenter', () => {
                    button.style.opacity = '1';
                });
                
                pre.addEventListener('mouseleave', () => {
                    button.style.opacity = '0';
                });
                
                button.addEventListener('click', () => {
                    navigator.clipboard.writeText(block.textContent).then(() => {
                        button.textContent = 'Copied!';
                        setTimeout(() => {
                            button.textContent = 'Copy';
                        }, 2000);
                    });
                });
            });
        });
    </script>
</body>
</html>'''

# Control Categories and Navigation
NAVIGATION_MENUS = {
    'input': '''
                    <li class="has-submenu open">
                        <a href="#"><i class="bi bi-input-cursor-text"></i> Input Controls</a>
                        <ul class="submenu">
                            <li><a href="beep-button.html">BeepButton</a></li>
                            <li><a href="beep-textbox.html">BeepTextBox</a></li>
                            <li><a href="beep-combobox.html" class="active">BeepComboBox</a></li>
                            <li><a href="beep-checkbox.html">BeepCheckBox</a></li>
                            <li><a href="beep-radiobutton.html">BeepRadioButton</a></li>
                            <li><a href="beep-datepicker.html">BeepDatePicker</a></li>
                            <li><a href="beep-numericupdown.html">BeepNumericUpDown</a></li>
                            <li><a href="beep-switch.html">BeepSwitch</a></li>
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
                        </ul>
                    </li>''',
    
    'display': '''
                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-input-cursor-text"></i> Input Controls</a>
                        <ul class="submenu">
                            <li><a href="beep-button.html">BeepButton</a></li>
                            <li><a href="beep-textbox.html">BeepTextBox</a></li>
                            <li><a href="beep-combobox.html">BeepComboBox</a></li>
                            <li><a href="beep-checkbox.html">BeepCheckBox</a></li>
                            <li><a href="beep-radiobutton.html">BeepRadioButton</a></li>
                            <li><a href="beep-datepicker.html">BeepDatePicker</a></li>
                            <li><a href="beep-numericupdown.html">BeepNumericUpDown</a></li>
                            <li><a href="beep-switch.html">BeepSwitch</a></li>
                        </ul>
                    </li>
                    <li class="has-submenu open">
                        <a href="#"><i class="bi bi-display"></i> Display Controls</a>
                        <ul class="submenu">
                            <li><a href="beep-label.html">BeepLabel</a></li>
                            <li><a href="beep-image.html">BeepImage</a></li>
                            <li><a href="beep-progressbar.html" class="active">BeepProgressBar</a></li>
                            <li><a href="beep-shape.html">BeepShape</a></li>
                            <li><a href="beep-starrating.html">BeepStarRating</a></li>
                            <li><a href="beep-marquee.html">BeepMarquee</a></li>
                            <li><a href="beep-testimonial.html">BeepTestimonial</a></li>
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
                        </ul>
                    </li>''',
    
    'layout': '''
                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-input-cursor-text"></i> Input Controls</a>
                        <ul class="submenu">
                            <li><a href="beep-button.html">BeepButton</a></li>
                            <li><a href="beep-textbox.html">BeepTextBox</a></li>
                            <li><a href="beep-combobox.html">BeepComboBox</a></li>
                            <li><a href="beep-checkbox.html">BeepCheckBox</a></li>
                            <li><a href="beep-radiobutton.html">BeepRadioButton</a></li>
                            <li><a href="beep-datepicker.html">BeepDatePicker</a></li>
                            <li><a href="beep-numericupdown.html">BeepNumericUpDown</a></li>
                            <li><a href="beep-switch.html">BeepSwitch</a></li>
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
                        </ul>
                    </li>
                    <li class="has-submenu open">
                        <a href="#"><i class="bi bi-layout-split"></i> Layout Controls</a>
                        <ul class="submenu">
                            <li><a href="beep-panel.html">BeepPanel</a></li>
                            <li><a href="beep-card.html">BeepCard</a></li>
                            <li><a href="beep-tabcontrol.html">BeepTabs</a></li>
                            <li><a href="beep-accordion.html">BeepAccordionMenu</a></li>
                            <li><a href="beep-multisplitter.html">BeepMultiSplitter</a></li>
                            <li><a href="beep-breadcrumps.html">BeepBreadcrumps</a></li>
                            <li><a href="beep-stepper.html" class="active">BeepSteppperBar</a></li>
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
                        </ul>
                    </li>''',
    
    'data': '''
                    <li class="has-submenu">
                        <a href="#"><i class="bi bi-input-cursor-text"></i> Input Controls</a>
                        <ul class="submenu">
                            <li><a href="beep-button.html">BeepButton</a></li>
                            <li><a href="beep-textbox.html">BeepTextBox</a></li>
                            <li><a href="beep-combobox.html">BeepComboBox</a></li>
                            <li><a href="beep-checkbox.html">BeepCheckBox</a></li>
                            <li><a href="beep-radiobutton.html">BeepRadioButton</a></li>
                            <li><a href="beep-datepicker.html">BeepDatePicker</a></li>
                            <li><a href="beep-numericupdown.html">BeepNumericUpDown</a></li>
                            <li><a href="beep-switch.html">BeepSwitch</a></li>
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
                        </ul>
                    </li>
                    <li class="has-submenu open">
                        <a href="#"><i class="bi bi-bar-chart"></i> Data Controls</a>
                        <ul class="submenu">
                            <li><a href="beep-grid.html">BeepSimpleGrid</a></li>
                            <li><a href="beep-chart.html">BeepChart</a></li>
                            <li><a href="beep-statcard.html" class="active">BeepStatCard</a></li>
                            <li><a href="beep-metrictile.html">BeepMetricTile</a></li>
                            <li><a href="beep-tree.html">BeepTree</a></li>
                            <li><a href="beep-listbox.html">BeepListBox</a></li>
                        </ul>
                    </li>'''
}

# Instructions for using this template:
print("""
INSTRUCTIONS FOR CONVERTING REMAINING CONTROL DOCUMENTATION:

1. Copy the CONTROL_TEMPLATE above
2. Replace the following placeholders with actual content:

   {{CONTROL_NAME}} - Name of the control (e.g., "BeepProgressBar")
   {{CONTROL_DESCRIPTION}} - Brief subtitle description
   {{CATEGORY_ANCHOR}} - Anchor for category (input-controls, display-controls, layout-controls, data-controls)
   {{CATEGORY_NAME}} - Category display name (Input Controls, Display Controls, etc.)
   {{NAVIGATION_MENU}} - Use one of the NAVIGATION_MENUS above based on category
   {{OVERVIEW_TEXT}} - Brief overview of what the control does
   {{ADDITIONAL_OVERVIEW}} - Additional overview text
   {{FEATURE_CARDS}} - HTML for 6 feature cards
   {{BASIC_USAGE_CODE}} - Simple C# code example
   {{USAGE_TIP}} - Quick tip text
   {{PROPERTIES_CONTENT}} - Properties tables and content
   {{EXAMPLES_CONTENT}} - Code examples
   {{BEST_PRACTICES_CONTENT}} - Best practices content
   {{NEXT_CONTROL}} - Next control filename
   {{NEXT_CONTROL_NAME}} - Next control display name

3. For feature cards, use this format:
   <div class="feature-card">
       <h3>?? Feature Name</h3>
       <p>Feature description</p>
   </div>

4. For properties tables, use this format:
   <table class="property-table">
       <thead>
           <tr>
               <th>Property</th>
               <th>Type</th>
               <th>Description</th>
           </tr>
       </thead>
       <tbody>
           <tr>
               <td><code>PropertyName</code></td>
               <td>Type</td>
               <td>Description</td>
           </tr>
       </tbody>
   </table>

5. Set the correct navigation menu based on control category and mark the current control as active

REMAINING CONTROLS TO UPDATE:
- beep-combobox.html
- beep-checkbox.html  
- beep-radiobutton.html
- beep-datepicker.html
- beep-numericupdown.html
- beep-switch.html
- beep-label.html
- beep-image.html
- beep-progressbar.html
- beep-shape.html
- beep-starrating.html
- beep-marquee.html
- beep-testimonial.html
- beep-card.html
- beep-tabcontrol.html
- beep-accordion.html
- beep-breadcrumps.html
- beep-stepper.html
- beep-grid.html
- beep-chart.html
- beep-statcard.html
- beep-metrictile.html
- beep-tree.html
- beep-listbox.html
- beep-calendar.html
- beep-dialogbox.html
""")