


namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepPopupModalForm : BeepPopupForm
    {
        Control ControlToDisplay;
        public BeepPopupModalForm()
        {
            InitializeComponent();
        }
        public  void AddControl(Control control, string addiname)
        {
            if (control == null || control == this)
                return;

            //// Get the content area based on border thickness.
            Rectangle contentRect = beepPanel1.ClientRectangle;

            //// Determine if the control's minimum size exceeds the available content area.
            int newContentWidth = contentRect.Width;
            int newContentHeight = contentRect.Height;
            bool needsResize = false;

            if (control.Width > contentRect.Width)
            {
                newContentWidth = control.MinimumSize.Width;
                needsResize = true;
            }
            if (control.Height > contentRect.Height)
            {
                newContentHeight = control.MinimumSize.Height;
                needsResize = true;
            }

            if (needsResize)
            {
                // Calculate a new overall form size: content size plus twice the border thickness.
                int overallWidth = newContentWidth ;
                int overallHeight = newContentHeight ;
                this.Size = new Size(overallWidth, overallHeight);

                // Update the content rectangle after resizing.
                contentRect = beepPanel1.ClientRectangle;
            }

            // Now set the control's bounds to exactly fill the content area.
            // This will align it flush left (and top) based on _borderThickness.

            beepPanel1.Controls.Add(control);
            control.Left = contentRect.Left+1;
            control.Top = contentRect.Top+1;
            control.Width = contentRect.Width-2;
            control.Height = contentRect.Height-2;
            control.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            control.BringToFront();
            ControlToDisplay= control;
        }
      
    }
}
