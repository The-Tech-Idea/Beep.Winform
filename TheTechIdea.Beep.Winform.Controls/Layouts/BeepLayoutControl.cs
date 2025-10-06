using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Layouts
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Layout Control")] 
    [Description("Container control for ready-made layout templates with Beep styling.")]
    public partial class BeepLayoutControl : BaseControl
    {
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
        [Category("Layout")] 
        [Description("Selects which predefined layout template to render.")]
        public TemplateType Template
        {
            get => _template;
            set
            {
                if (_template != value)
                {
                    _template = value;
                    BuildLayout();
                    Invalidate();
                }
            }
        }

        [Category("Layout")] 
        [Description("Rows for Grid template.")]
        public int GridRows { get; set; } = 3;

        [Category("Layout")] 
        [Description("Columns for Grid template.")]
        public int GridColumns { get; set; } = 3;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (!DesignMode)
            {
                BuildLayout();
            }
        }

        public void BuildLayout()
        {
            try
            {
                SuspendLayout();
                Controls.Clear();
                switch (_template)
                {
                    case TemplateType.Invoice:
                        InvoiceLayoutHelper.Build(this);
                        break;
                    case TemplateType.Product:
                        ProductLayoutHelper.Build(this);
                        break;
                    case TemplateType.Profile:
                        ProfileLayoutHelper.Build(this);
                        break;
                    case TemplateType.Report:
                        ReportLayoutHelper.Build(this);
                        break;
                    case TemplateType.VerticalStack:
                        VerticalStackLayoutHelper.Build(this);
                        break;
                    case TemplateType.HorizontalStack:
                        HorizontalStackLayoutHelper.Build(this);
                        break;
                    case TemplateType.Grid:
                        GridLayoutHelper.Build(this, Math.Max(1, GridRows), Math.Max(1, GridColumns));
                        break;
                    case TemplateType.SplitContainer:
                        SplitContainerLayoutHelper.Build(this, Orientation.Vertical);
                        break;
                    case TemplateType.Dock:
                        DockLayoutHelper.Build(this);
                        break;
                }
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Background and optional style using BeepStyling
            BeepStyling.PaintStyleBackground(e.Graphics, DrawingRect);

            // Optional: draw a subtle border using current theme
            var themeBorder = BeepStyling.GetThemeColor("Border");
            using var pen = new Pen(themeBorder.IsEmpty ? Color.Silver : themeBorder);
            e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
        }
    }
}
