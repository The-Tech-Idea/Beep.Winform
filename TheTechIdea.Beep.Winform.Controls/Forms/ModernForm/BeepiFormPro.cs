using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro : Form
    {
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        public BeepiFormPro()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            UpdateStyles();
            BackColor = Color.Transparent; // we paint everything
            UpdateDpiScale();
            _layout = new BeepiFormProLayoutManager(this);
            _hits = new BeepiFormProHitAreaManager(this);
            _interact = new BeepiFormProInteractionManager(this, _hits);
            ActivePainter = new MinimalFormPainter();
            InitializeBuiltInRegions();
            ApplyFormStyle();
        }

        private void ApplyFormStyle()
        {
            switch (FormStyle)
            {
                case FormStyle.Modern:
                case FormStyle.Minimal:
                    FormBorderStyle = FormBorderStyle.None;
                    break;
                case FormStyle.Classic:
                    FormBorderStyle = FormBorderStyle.Sizable;
                    break;
                case FormStyle.MacOS:
                case FormStyle.Fluent:
                case FormStyle.Material:
                    FormBorderStyle = FormBorderStyle.None;
                    break;
            }
            Invalidate();
        }
    }
}
