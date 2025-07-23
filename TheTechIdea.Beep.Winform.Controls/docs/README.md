# Beep Controls Documentation

This directory contains the Sphinx documentation for the TheTechIdea.Beep.Winform.Controls library.

## Setup

### Prerequisites

- Python 3.8 or higher
- pip (Python package installer)

### Installation

1. Install the required Python packages:

```bash
pip install -r requirements.txt
```

Or install individually:

```bash
pip install sphinx furo sphinx-copybutton sphinx-tabs myst-parser sphinx-design
```

## Building the Documentation

### Using Make (Linux/macOS/Windows with Make)

```bash
# Build HTML documentation
make html

# Build PDF documentation (requires LaTeX)
make latexpdf

# Clean build files
make clean

# View all available targets
make help
```

### Using Windows Batch File

```cmd
# Build HTML documentation
make.bat html

# Build PDF documentation (requires LaTeX)
make.bat latexpdf

# Clean build files
make.bat clean
```

### Using Sphinx Directly

```bash
# Build HTML documentation
sphinx-build -b html source build/html

# Build PDF documentation
sphinx-build -b latex source build/latex
cd build/latex
make
```

## Viewing the Documentation

After building, the HTML documentation will be available at:
- `build/html/index.html` - Open this file in your web browser

## Development

### Adding New Pages

1. Create a new `.rst` file in the appropriate directory under `source/`
2. Add the file to the relevant `toctree` directive in `index.rst` or the parent page
3. Rebuild the documentation

### reStructuredText Syntax

The documentation uses reStructuredText (rST) format. Key syntax:

```rst
Title
=====

Subtitle
--------

**Bold text**
*Italic text*
``Code text``

.. code-block:: csharp

   // C# code example
   var button = new BeepButton();

.. note::
   This is a note admonition.

.. warning::
   This is a warning admonition.

.. tip::
   This is a tip admonition.
```

### Adding Images

1. Place images in `source/_static/images/`
2. Reference them in documentation:

```rst
.. image:: /_static/images/your-image.png
   :alt: Alt text
   :align: center
   :width: 600px
```

### Cross-References

Link to other pages:

```rst
:doc:`installation`
:doc:`../controls/beep-button`
```

## Theme Customization

The documentation uses the Furo theme with custom styling:

- Theme configuration: `source/conf.py`
- Custom CSS: `source/_static/custom.css`
- Custom JavaScript: `source/_static/custom.js`

## Deployment

### GitHub Pages

Add a `.github/workflows/docs.yml` file to automatically build and deploy documentation to GitHub Pages:

```yaml
name: Deploy Documentation

on:
  push:
    branches: [ main ]

jobs:
  docs:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    
    - name: Set up Python
      uses: actions/setup-python@v4
      with:
        python-version: '3.9'
        
    - name: Install dependencies
      run: |
        pip install -r docs/requirements.txt
        
    - name: Build documentation
      run: |
        cd docs
        make html
        
    - name: Deploy to GitHub Pages
      uses: peaceiris/actions-gh-pages@v3
      with:
        github_token: ${{ secrets.GITHUB_TOKEN }}
        publish_dir: docs/build/html
```

### Other Hosting Options

- **Read the Docs**: Connect your repository for automatic builds
- **Netlify**: Deploy the `build/html` directory
- **Azure Static Web Apps**: Use the HTML build output

## File Structure

```
docs/
??? source/
?   ??? _static/          # Static files (CSS, JS, images)
?   ??? _templates/       # Custom templates (optional)
?   ??? controls/         # Control documentation
?   ??? getting-started/  # Getting started guides
?   ??? examples/         # Examples and tutorials
?   ??? guides/           # Best practices and guides
?   ??? api/              # API reference
?   ??? conf.py           # Sphinx configuration
?   ??? index.rst         # Main documentation index
??? build/                # Built documentation (generated)
??? requirements.txt      # Python dependencies
??? Makefile             # Unix makefile
??? make.bat             # Windows batch file
??? README.md            # This file
```

## Contributing

When contributing to the documentation:

1. Follow the existing structure and style
2. Use clear, concise language
3. Include code examples where appropriate
4. Test your changes by building locally
5. Ensure all cross-references work correctly

## Troubleshooting

### Common Issues

**Build fails with import errors**
- Ensure all dependencies are installed: `pip install -r requirements.txt`

**Images not showing**
- Check image paths are correct relative to `source/`
- Ensure images are in `_static/images/` directory

**Cross-references broken**
- Verify file names and paths in `:doc:` directives
- Check that referenced files exist

**Theme issues**
- Clear the build directory: `make clean`
- Rebuild: `make html`

### Getting Help

- [Sphinx Documentation](https://www.sphinx-doc.org/)
- [reStructuredText Primer](https://www.sphinx-doc.org/en/master/usage/restructuredtext/basics.html)
- [Furo Theme Documentation](https://pradyunsg.me/furo/)