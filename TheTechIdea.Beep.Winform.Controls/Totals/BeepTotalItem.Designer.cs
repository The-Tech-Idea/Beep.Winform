namespace TheTechIdea.Beep.Winform.Controls.Totals
{
    partial class BeepTotalItem
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            TotalTextBox = new MaskedTextBox();
            SuspendLayout();
            // 
            // TotalTextBox
            // 
            TotalTextBox.BorderStyle = BorderStyle.FixedSingle;
            TotalTextBox.Dock = DockStyle.Fill;
            TotalTextBox.Location = new Point(0, 0);
            TotalTextBox.Margin = new Padding(4, 3, 4, 3);
            TotalTextBox.Name = "TotalTextBox";
            TotalTextBox.Size = new Size(176, 23);
            TotalTextBox.TabIndex = 0;
            TotalTextBox.TextAlign = HorizontalAlignment.Center;
            // 
            // BeepTotalItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(TotalTextBox);
            Margin = new Padding(5, 3, 5, 3);
            Name = "BeepTotalItem";
            Size = new Size(176, 24);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MaskedTextBox TotalTextBox;
    }
}
