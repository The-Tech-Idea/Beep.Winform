# Configuration file for the Sphinx documentation builder.
#
# For the full list of built-in configuration values, see the documentation:
# https://www.sphinx-doc.org/en/master/usage/configuration.html

# -- Project information -----------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#project-information

project = 'Beep Controls'
copyright = '2024, TheTechIdea'
author = 'TheTechIdea'
version = '1.0.164'
release = '1.0.164'

# -- General configuration ---------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#general-configuration

extensions = [
    'sphinx.ext.autodoc',
    'sphinx.ext.viewcode',
    'sphinx.ext.napoleon',
    'sphinx.ext.intersphinx',
    'sphinx.ext.todo',
    'sphinx.ext.coverage',
    'sphinx.ext.mathjax',
    'sphinx.ext.ifconfig',
    'sphinx.ext.githubpages',
    'sphinx_copybutton',
    'sphinx_tabs.tabs',
    'myst_parser',
]

templates_path = ['_templates']
exclude_patterns = []

# The master toctree document.
master_doc = 'index'

# -- Options for HTML output -------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#options-for-html-output

html_theme = 'furo'

html_theme_options = {
    "sidebar_hide_name": False,
    "light_logo": "beep-logo.svg",
    "dark_logo": "beep-logo.svg",
    "light_css_variables": {
        "color-brand-primary": "#0078d7",
        "color-brand-content": "#0078d7",
        "color-admonition-background": "#f0f7ff",
    },
    "dark_css_variables": {
        "color-brand-primary": "#4fc3f7",
        "color-brand-content": "#4fc3f7",
    },
    "navigation_with_keys": True,
    "top_of_page_button": "edit",
    "announcement": None,
}

html_static_path = ['_static']
html_css_files = [
    'custom.css',
]

html_js_files = [
    'custom.js',
]

# -- Options for LaTeX output ------------------------------------------------

latex_elements = {
    # The paper size ('letterpaper' or 'a4paper').
    #
    # 'papersize': 'letterpaper',

    # The font size ('10pt', '11pt' or '12pt').
    #
    # 'pointsize': '10pt',

    # Additional stuff for the LaTeX preamble.
    #
    # 'preamble': '',

    # Latex figure (float) alignment
    #
    # 'figure_align': 'htbp',
}

# Grouping the document tree into LaTeX files. List of tuples
# (source start file, target name, title,
#  author, documentclass [howto, manual, or own class]).
latex_documents = [
    (master_doc, 'BeepControls.tex', 'Beep Controls Documentation',
     'TheTechIdea', 'manual'),
]

# -- Options for manual page output ------------------------------------------

# One entry per manual page. List of tuples
# (source start file, name, description, authors, manual section).
man_pages = [
    (master_doc, 'beepcontrols', 'Beep Controls Documentation',
     [author], 1)
]

# -- Options for Texinfo output ----------------------------------------------

# Grouping the document tree into Texinfo files. List of tuples
# (source start file, target name, title, author,
#  dir menu entry, description, category)
texinfo_documents = [
    (master_doc, 'BeepControls', 'Beep Controls Documentation',
     author, 'BeepControls', 'Modern, themeable WinForms controls for .NET applications.',
     'Miscellaneous'),
]

# -- Extension configuration -------------------------------------------------

# -- Options for intersphinx extension ---------------------------------------

# Example configuration for intersphinx: refer to the Python standard library.
intersphinx_mapping = {
    'python': ('https://docs.python.org/3/', None),
    'sphinx': ('https://www.sphinx-doc.org/en/master/', None),
}

# -- Options for todo extension ----------------------------------------------

# If true, `todo` and `todoList` produce output, else they produce nothing.
todo_include_todos = True

# -- Options for Napoleon extension ------------------------------------------

napoleon_google_docstring = True
napoleon_numpy_docstring = True
napoleon_include_init_with_doc = False
napoleon_include_private_with_doc = False
napoleon_include_special_with_doc = True
napoleon_use_admonition_for_examples = False
napoleon_use_admonition_for_notes = False
napoleon_use_admonition_for_references = False
napoleon_use_ivar = False
napoleon_use_param = True
napoleon_use_rtype = True
napoleon_preprocess_types = False
napoleon_type_aliases = None
napoleon_attr_annotations = True

# -- Syntax highlighting configuration ----------------------------------------

pygments_style = 'sphinx'
pygments_dark_style = 'monokai'

# -- Custom configuration -----------------------------------------------------

# Add any paths that contain custom static files (such as style sheets) here,
# relative to this directory. They are copied after the builtin static files,
# so a file named "default.css" will overwrite the builtin "default.css".

# Custom CSS and JS files
html_title = f"{project} v{version}"
html_short_title = project
html_favicon = '_static/favicon.ico'

# Add version info to footer
html_last_updated_fmt = '%b %d, %Y'
html_show_sourcelink = True
html_show_sphinx = True

# Search language
html_search_language = 'en'