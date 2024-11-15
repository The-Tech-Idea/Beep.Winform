namespace TheTechIdea.Beep.Winform.Controls.LogPanel
{
    partial class uc_logPanel
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
            LogPanel = new TextBox();
            splitContainer1 = new SplitContainer();
            toolStrip1 = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            toolStripSeparator1 = new ToolStripSeparator();
            CleartoolStripButton = new ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // LogPanel
            // 
            LogPanel.BackColor = Color.White;
            LogPanel.BorderStyle = BorderStyle.FixedSingle;
            LogPanel.Dock = DockStyle.Fill;
            LogPanel.ForeColor = Color.Black;
            LogPanel.Location = new Point(0, 0);
            LogPanel.Margin = new Padding(4, 3, 4, 3);
            LogPanel.Multiline = true;
            LogPanel.Name = "LogPanel";
            LogPanel.ReadOnly = true;
            LogPanel.ScrollBars = ScrollBars.Vertical;
            LogPanel.Size = new Size(954, 187);
            LogPanel.TabIndex = 1;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(LogPanel);
            splitContainer1.Size = new Size(954, 216);
            splitContainer1.SplitterDistance = 25;
            splitContainer1.TabIndex = 2;
            // 
            // toolStrip1
            // 
            toolStrip1.CanOverflow = false;
            toolStrip1.Dock = DockStyle.Fill;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabel1, toolStripSeparator1, CleartoolStripButton });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Margin = new Padding(5);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new Padding(5, 0, 5, 0);
            toolStrip1.Size = new Size(954, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(91, 22);
            toolStripLabel1.Text = "Log and Output";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // CleartoolStripButton
            // 
            CleartoolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CleartoolStripButton.Image = Properties.Resources.ClearWindowContent;
            CleartoolStripButton.ImageTransparentColor = Color.Magenta;
            CleartoolStripButton.Name = "CleartoolStripButton";
            CleartoolStripButton.Size = new Size(23, 22);
            CleartoolStripButton.Text = "Clear";
            // 
            // uc_logPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Margin = new Padding(5, 3, 5, 3);
            Name = "uc_logPanel";
            Size = new Size(954, 216);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox LogPanel;
        private SplitContainer splitContainer1;
        private ToolStrip toolStrip1;
        private ToolStripButton CleartoolStripButton;
        private ToolStripLabel toolStripLabel1;
        private ToolStripSeparator toolStripSeparator1;
    }
}
