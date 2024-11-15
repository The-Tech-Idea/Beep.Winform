
using TheTechIdea.Beep.Winform.Controls.Basic;

namespace TheTechIdea.Beep.Winform.Controls.Filter
{
    public partial class BeepFilterItem : uc_Addin
    {
        public BeepFilterItem()
        {
            InitializeComponent();
            // Initialize the layout
            ResizeControl();
        }
        private string filterText;
        public string LabelText
        {
            get { return filterText; }
            set
            {
                filterText=value;
                label1.Text = value;
                ResizeControl();
            }
        }
        public Label FilterText
        {
            get { return label1; }
           
        }
        private void ResizeControl()
        {
            // Measure the string with the current font.
            SizeF stringSize = CreateGraphics().MeasureString(label1.Text, label1.Font);

            // Calculate the new width for the control, considering the width of the button and padding.
            int newWidth = (int)stringSize.Width + Removebutton.Width + 20; // +20 for padding

            // Set the new size.
            this.Width = newWidth;

            // Update the location of the button since it's anchored to the right.
           // Removebutton.Left = this.Width - Removebutton.Width;
        }


    }
}
