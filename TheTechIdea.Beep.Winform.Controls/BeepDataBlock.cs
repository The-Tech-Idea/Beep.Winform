
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DataNavigator;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum BlockDisplayType
    {
        Record,
        Table
    }

    public class BeepDataBlock : BeepPanel
    {
        protected IUnitofWork _data;


        public IEntityStructure Structure => Data!=null ? (Data.EntityStructure!=null? Data.EntityStructure: null) : null; 
        public List<IBeepUIComponent> UIComponents { get; set; } = new List<IBeepUIComponent>();
        [Browsable(true)]
        [Category("Data")]
        [TypeConverter(typeof(UnitOfWorkConverter))]
        public IUnitofWork Data { get { return _data; } set { _data = value;Refresh(); } }
        protected BlockDisplayType _displaytype = BlockDisplayType.Record;
        protected BlockDisplayType _currentdisplaytype = BlockDisplayType.Record;
        public BlockDisplayType DisplayType { get { return _displaytype; } set { _displaytype=value;ChangeLayout(); } }

        public BeepDataBlock()
        {
        }
        public void Refresh()
        {
            Controls.Clear();
            UIComponents.Clear();

            if (Structure != null)
            {
                
                CreateControlsBasedonStucture();
            }
        }
        public void ChangeLayout()
        {
            // Check if the current layout type is different from the desired display type
            if (_currentdisplaytype == _displaytype)
                return;

            // Clear any existing controls and UI components before changing the layout
            Controls.Clear();
            UIComponents.Clear();

            if (Structure != null && Data != null)
            {
                if (_displaytype == BlockDisplayType.Record)
                {
                    // If display type is Record, create controls based on individual positioning
                    CreateControlsBasedonStucture();
                }
                else if (_displaytype == BlockDisplayType.Table)
                {
                    // If display type is Table, create controls within a TableLayoutPanel
                    CreateControlsInTableLayout();
                }
            }

            // Update the current display type to the new one
            _currentdisplaytype = _displaytype;
        }

        // Function to create controls and bind to data based on Structure
        public void CreateControlsBasedonStucture()
        {
            if (Structure == null || Data == null) return;

            int yPosition = DrawingRect.Top + 10; // Start from the top of DrawingRect with padding
            int xPosition = DrawingRect.Left + 10; // Start slightly offset from the left of DrawingRect

            foreach (var field in Structure.Fields)
            {
                // Create a label for each field
                BeepLabel label = new BeepLabel
                {
                    Text = field.Description,
                    Location = new Point(xPosition, yPosition),
                    Width = 100,
                    ShowAllBorders = true,
                    ShowShadow = false,
                };
                Controls.Add(label);

                // Create and configure BeepControl for field, including binding
                IBeepUIComponent beepControl = CreateAndBindBeepControl(field);
                if (beepControl != null)
                {
                    var control = beepControl as Control;
                    if (control != null)
                    {
                        control.Location = new Point(xPosition + 120, yPosition);
                        control.Width = DrawingRect.Width - xPosition - 140; // Adjust width based on DrawingRect bounds
                        control.Height = 25; // Fixed height for each control
                        Controls.Add(control);

                        // Add to UIComponents for further access
                        UIComponents.Add(beepControl);
                    }
                }

                yPosition += 30; // Increment y position for the next field
            }
        }

        public void CreateControlsInTableLayout()
        {
            if (Structure == null || Data == null) return;

            // Create TableLayoutPanel with 2 columns and set its bounds to fit within DrawingRect
            var tableLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = Structure.Fields.Count,
                Location = new Point(DrawingRect.Left, DrawingRect.Top),  // Position within DrawingRect
                Size = new Size(DrawingRect.Width, DrawingRect.Height),   // Size based on DrawingRect dimensions
                AutoSize = true,
                AutoScroll = true,
            };

            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // 30% for labels
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F)); // 70% for controls

            foreach (var field in Structure.Fields)
            {
                // Create label for each field
                Label label = new Label
                {
                    Text = field.Description,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Dock = DockStyle.Fill
                };
                tableLayout.Controls.Add(label);

                // Create BeepControl and bind it to data
                IBeepUIComponent beepControl = CreateAndBindBeepControl(field);
                if (beepControl != null)
                {
                    var control = beepControl as Control;
                    if (control != null)
                    {
                        control.Dock = DockStyle.Fill;  // Allow control to expand within cell
                        tableLayout.Controls.Add(control);

                        // Add to UIComponents for further access
                        UIComponents.Add(beepControl);
                    }
                }
            }

            // Add the TableLayoutPanel to the BeepDataBlock's Controls collection within DrawingRect
            Controls.Add(tableLayout);
        }

        private IBeepUIComponent CreateAndBindBeepControl(EntityField field)
        {
            IBeepUIComponent beepControl = null;

            // Select BeepControl type based on field category
            switch (field.fieldCategory)
            {
                case DbFieldCategory.String:
                    beepControl = new BeepTextBox { Text = field.fieldname };
                    break;

                case DbFieldCategory.Numeric:
                    beepControl = new BeepNumericUpDown { Text = field.fieldname };
                    break;

                case DbFieldCategory.Date:
                    beepControl = new BeepDatePicker { Text = field.fieldname };
                    break;

                default:
                    beepControl = new BeepTextBox { Text = field.fieldname };
                    break;
            }

            // Bind BeepControl to data
            if (beepControl != null && Data != null)
            {
                var entityData = Data.Units;
                if (entityData != null)
                {
                    beepControl.DataContext = entityData.Current;
                    beepControl.SetBinding(nameof(beepControl.Text), field.fieldname);
                }
            }

            return beepControl;
        }
    }
}
