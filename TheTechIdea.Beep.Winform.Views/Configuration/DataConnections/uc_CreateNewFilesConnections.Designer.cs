using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Views.Configuration.DataConnections
{
    partial class uc_CreateNewFilesConnections
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
            components = new System.ComponentModel.Container();
            dataConnectionsBindingSource = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)dataConnectionsBindingSource).BeginInit();
            SuspendLayout();
            // 
            // dataConnectionsBindingSource
            // 
            dataConnectionsBindingSource.DataSource = typeof(ConnectionProperties);
            // 
            // uc_CreateNewFilesConnections
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Name = "uc_CreateNewFilesConnections";
            Size = new Size(478, 622);
            ((System.ComponentModel.ISupportInitialize)dataConnectionsBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private BindingSource dataConnectionsBindingSource;
    }
}
