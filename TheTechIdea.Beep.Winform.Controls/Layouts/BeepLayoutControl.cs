using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Layouts
{
    /// <summary>
    /// Container control for ready-made layout templates with Beep styling and theming support.
    /// Provides a variety of pre-built layout templates that can be customized via LayoutOptions.
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Layout Control")] 
    [Description("Container control for ready-made layout templates with Beep styling.")]
    public partial class BeepLayoutControl : BaseControl
    {
        /// <summary>
        /// Supported layout template types.
        /// </summary>
        public enum TemplateType
        {
            Invoice,
            Product,
            Profile,
            Report,
            VerticalStack,
            HorizontalStack,
            Grid,
            SplitContainer,
            Dock
        }

        private TemplateType _template = TemplateType.VerticalStack;
        private LayoutOptions _layoutOptions;
        private Control _currentLayoutControl;

        /// <summary>
        /// Occurs when a layout has been built.
        /// </summary>
        [Category("Layout")]
        [Description("Occurs when a layout template has been built.")]
        public event EventHandler LayoutBuilt;

        /// <summary>
        /// Occurs when the template type changes.
        /// </summary>
        [Category("Layout")]
        [Description("Occurs when the Template property changes.")]
        public event EventHandler LayoutChanged;

        /// <summary>
        /// Gets or sets which predefined layout template to render.
        /// </summary>
        [Category("Layout")] 
        [Description("Selects which predefined layout template to render.")]
        [DefaultValue(TemplateType.VerticalStack)]
        public TemplateType Template
        {
            get => _template;
            set
            {
                if (_template != value)
                {
                    _template = value;
                    OnLayoutChanged();
                    BuildLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of rows for Grid template. Must be greater than 0.
        /// </summary>
        [Category("Layout")] 
        [Description("Rows for Grid template. Must be greater than 0.")]
        [DefaultValue(3)]
        public int GridRows 
        { 
            get => _gridRows;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("GridRows must be greater than 0", nameof(value));
                if (_gridRows != value)
                {
                    _gridRows = value;
                    if (_template == TemplateType.Grid)
                    {
                        BuildLayout();
                        Invalidate();
                    }
                }
            }
        }
        private int _gridRows = 3;

        /// <summary>
        /// Gets or sets the number of columns for Grid template. Must be greater than 0.
        /// </summary>
        [Category("Layout")] 
        [Description("Columns for Grid template. Must be greater than 0.")]
        [DefaultValue(3)]
        public int GridColumns 
        { 
            get => _gridColumns;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("GridColumns must be greater than 0", nameof(value));
                if (_gridColumns != value)
                {
                    _gridColumns = value;
                    if (_template == TemplateType.Grid)
                    {
                        BuildLayout();
                        Invalidate();
                    }
                }
            }
        }
        private int _gridColumns = 3;

        /// <summary>
        /// Gets or sets the layout configuration options for theming and styling.
        /// If null, creates default options from this control's theme and style settings.
        /// </summary>
        [Category("Layout")]
        [Description("Configuration options for layout theming and styling.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LayoutOptions LayoutOptions
        {
            get
            {
                if (_layoutOptions == null)
                {
                    _layoutOptions = LayoutOptions.FromControl(this);
                }
                return _layoutOptions;
            }
            set
            {
                if (_layoutOptions != value)
                {
                    _layoutOptions = value;
                    BuildLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Raises the LayoutChanged event.
        /// </summary>
        protected virtual void OnLayoutChanged()
        {
            LayoutChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the LayoutBuilt event.
        /// </summary>
        protected virtual void OnLayoutBuilt()
        {
            LayoutBuilt?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (!DesignMode)
            {
                BuildLayout();
            }
        }

        /// <summary>
        /// Rebuilds the current layout template.
        /// Useful when layout options or properties change.
        /// </summary>
        public void RebuildLayout()
        {
            BuildLayout();
        }

        /// <summary>
        /// Builds the layout template based on the current Template property.
        /// </summary>
        public void BuildLayout()
        {
            try
            {
                SuspendLayout();
                Controls.Clear();
                _currentLayoutControl = null;

                // Ensure layout options are initialized from control settings
                if (_layoutOptions == null)
                {
                    _layoutOptions = LayoutOptions.FromControl(this);
                }

                switch (_template)
                {
                    case TemplateType.Invoice:
                        _currentLayoutControl = InvoiceLayoutHelper.Build(this, _layoutOptions);
                        break;
                    case TemplateType.Product:
                        _currentLayoutControl = ProductLayoutHelper.Build(this, _layoutOptions);
                        break;
                    case TemplateType.Profile:
                        _currentLayoutControl = ProfileLayoutHelper.Build(this, _layoutOptions);
                        break;
                    case TemplateType.Report:
                        _currentLayoutControl = ReportLayoutHelper.Build(this, _layoutOptions);
                        break;
                    case TemplateType.VerticalStack:
                        _currentLayoutControl = VerticalStackLayoutHelper.Build(this, _layoutOptions);
                        break;
                    case TemplateType.HorizontalStack:
                        _currentLayoutControl = HorizontalStackLayoutHelper.Build(this, _layoutOptions);
                        break;
                    case TemplateType.Grid:
                        _currentLayoutControl = GridLayoutHelper.Build(this, GridRows, GridColumns, _layoutOptions);
                        break;
                    case TemplateType.SplitContainer:
                        _currentLayoutControl = SplitContainerLayoutHelper.Build(this, Orientation.Vertical, _layoutOptions);
                        break;
                    case TemplateType.Dock:
                        _currentLayoutControl = DockLayoutHelper.Build(this, _layoutOptions);
                        break;
                }

                OnLayoutBuilt();
            }
            catch (Exception ex)
            {
                // Log error or show message in design mode
                if (DesignMode)
                {
                    System.Diagnostics.Debug.WriteLine($"Error building layout: {ex.Message}");
                }
                throw;
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Background and optional Style using BeepStyling
            BeepStyling.PaintStyleBackground(e.Graphics, DrawingRect,ControlStyle);

            // Optional: draw a subtle border using current theme
            var themeBorder = BeepStyling.GetThemeColor("Border");
            using var pen = new Pen(themeBorder.IsEmpty ? Color.Silver : themeBorder);
            e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
        }
    }
}
