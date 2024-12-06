using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Design.Forms
{
    public partial class DataBlocksCollectionForm : Form
    {
        private readonly CollectionEditor _editor;
        private readonly List<BeepDataBlock> _existingBlocks; // Existing blocks on the form
        private readonly List<BeepDataBlock> _selectedBlocks = new List<BeepDataBlock>(); // Selected blocks

        public DataBlocksCollectionForm(CollectionEditor editor, List<BeepDataBlock> existingBlocks)
        {
            InitializeComponent();
            _editor = editor;
            _existingBlocks = existingBlocks;

            InitializeUI();
        }

        private void InitializeUI()
        {
            // Form properties
            this.Text = "Select Data Blocks";
            this.Width = 400;
            this.Height = 300;

            // CheckedListBox to display existing blocks
            var blocksList = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                CheckOnClick = true,
                DataSource = _existingBlocks,
                DisplayMember = "Name", // Assuming BeepDataBlock has a Name property
                ValueMember = "ID"     // Assuming BeepDataBlock has a unique ID property
            };
            this.Controls.Add(blocksList);

            // Buttons for actions
            var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            var okButton = new Button { Text = "OK", DialogResult = DialogResult.OK, Dock = DockStyle.Right };
            var cancelButton = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Dock = DockStyle.Left };

            okButton.Click += (s, e) =>
            {
                // Gather selected blocks
                foreach (var item in blocksList.CheckedItems)
                {
                    if (item is BeepDataBlock block)
                        _selectedBlocks.Add(block);
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            buttonPanel.Controls.Add(okButton);
            buttonPanel.Controls.Add(cancelButton);
            this.Controls.Add(buttonPanel);
        }

        public List<BeepDataBlock> GetSelectedBlocks()
        {
            return _selectedBlocks;
        }
    }
}
