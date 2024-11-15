using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Winform.Controls.Basic;

namespace TheTechIdea.Beep.Winform.Controls.Filter
{
    public partial class BeepFilterPanel : uc_Addin
    {
        public EntityStructure Entity { get; set; } = new EntityStructure();
        public BeepFilterPanel()
        {
            InitializeComponent();
            PopulateControls();
        }
        public void SetFilters(EntityStructure entity)
        {
            this.Entity = entity;
            PopulateControls();
        }
        public void PopulateControls()
        {
            // Clear existing controls if necessary
            this.Controls.Clear();

            // Loop through the filters and create corresponding user controls
            int leftPosition = 0;
            foreach (var filter in Entity.Filters)
            {
                BeepFilterItem filterControl = new BeepFilterItem();
                string labelContent = filter.FieldName + " " + filter.Operator + " " + filter.FilterValue;
                if (!string.IsNullOrEmpty(filter.FilterValue1))
                {
                    labelContent += " " + filter.FilterValue1;
                }

                filterControl.LabelText = labelContent;

                // Set the location of the control within the container
                filterControl.Left = leftPosition;
                leftPosition += filterControl.Width; // Set position for next control


                // Add the control to the container
                this.Controls.Add(filterControl);
            }

            // Optionally, resize the container control to fit the contents
            this.Width = leftPosition;
        }
    }

}
