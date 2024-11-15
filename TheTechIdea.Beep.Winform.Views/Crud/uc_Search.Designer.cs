namespace TheTechIdea.Beep.Winform.Views.Crud
{
    partial class uc_Search
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
            splitContainer1 = new SplitContainer();
            Cancelbutton = new Button();
            Submitbutton = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.FixedSingle;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.AutoScroll = true;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(Cancelbutton);
            splitContainer1.Panel2.Controls.Add(Submitbutton);
            splitContainer1.Size = new Size(428, 808);
            splitContainer1.SplitterDistance = 760;
            splitContainer1.TabIndex = 1;
            // 
            // Cancelbutton
            // 
            Cancelbutton.DialogResult = DialogResult.Cancel;
            Cancelbutton.Location = new Point(3, 3);
            Cancelbutton.Name = "Cancelbutton";
            Cancelbutton.Size = new Size(75, 37);
            Cancelbutton.TabIndex = 1;
            Cancelbutton.Text = "Cancel";
            Cancelbutton.UseVisualStyleBackColor = true;
            // 
            // Submitbutton
            // 
            Submitbutton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Submitbutton.DialogResult = DialogResult.OK;
            Submitbutton.Location = new Point(348, 3);
            Submitbutton.Name = "Submitbutton";
            Submitbutton.Size = new Size(75, 37);
            Submitbutton.TabIndex = 0;
            Submitbutton.Text = "Submit";
            Submitbutton.UseVisualStyleBackColor = true;
            // 
            // uc_Search
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(splitContainer1);
            Name = "uc_Search";
            Size = new Size(428, 808);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private Button Cancelbutton;
        private Button Submitbutton;
    }
}
