namespace TheTechIdea.Beep.Winform.Controls.Grid.DesignerForm
{
    partial class DataGridViewColumnDesignerForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.ColumnTypecomboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ToTypecomboBox = new System.Windows.Forms.ComboBox();
            this.Changebutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 1);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(293, 446);
            this.listBox1.TabIndex = 0;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(299, 44);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(500, 403);
            this.propertyGrid1.TabIndex = 1;
            // 
            // ColumnTypecomboBox
            // 
            this.ColumnTypecomboBox.FormattingEnabled = true;
            this.ColumnTypecomboBox.Location = new System.Drawing.Point(311, 12);
            this.ColumnTypecomboBox.Name = "ColumnTypecomboBox";
            this.ColumnTypecomboBox.Size = new System.Drawing.Size(209, 21);
            this.ColumnTypecomboBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(526, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "To";
            // 
            // ToTypecomboBox
            // 
            this.ToTypecomboBox.FormattingEnabled = true;
            this.ToTypecomboBox.Location = new System.Drawing.Point(552, 12);
            this.ToTypecomboBox.Name = "ToTypecomboBox";
            this.ToTypecomboBox.Size = new System.Drawing.Size(209, 21);
            this.ToTypecomboBox.TabIndex = 4;
            // 
            // Changebutton
            // 
        //    this.Changebutton.BackgroundImage = global::TheTechIdea.Beep.Winform.Controls.Properties.Resources._065_check;
            this.Changebutton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Changebutton.Location = new System.Drawing.Point(767, 10);
            this.Changebutton.Name = "Changebutton";
            this.Changebutton.Size = new System.Drawing.Size(20, 23);
            this.Changebutton.TabIndex = 5;
            this.Changebutton.UseVisualStyleBackColor = true;
            // 
            // DataGridViewColumnDesignerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Changebutton);
            this.Controls.Add(this.ToTypecomboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ColumnTypecomboBox);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.listBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DataGridViewColumnDesignerForm";
            this.Text = "DataGridViewColumnDesignerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListBox listBox1;
        private PropertyGrid propertyGrid1;
        private ComboBox ColumnTypecomboBox;
        private Label label1;
        private ComboBox ToTypecomboBox;
        private Button Changebutton;
    }
}