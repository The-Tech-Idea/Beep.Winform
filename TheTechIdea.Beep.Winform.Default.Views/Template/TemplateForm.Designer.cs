namespace TheTechIdea.Beep.Winform.Default.Views.Template
{
    partial class TemplateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateForm));
            SuspendLayout();
            // 
            // TemplateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderColor = Color.FromArgb(200, 200, 200);
            BorderRadius = 0;
            CaptionHeight = 44;
            CaptionRenderer = Winform.Controls.Forms.Caption.CaptionRendererKind.Artistic;
            ClientSize = new Size(816, 492);
            FormStyle = Winform.Controls.BeepFormStyle.Artistic;
            GlowColor = Color.FromArgb(140, 200, 50, 150);
            GlowSpread = 25F;
            Name = "TemplateForm";
            Padding = new Padding(3);
            ShadowDepth = 15;
            StylePresets.Presets = (Dictionary<string, Controls.BeepFormStyleMetrics>)resources.GetObject("TemplateForm.StylePresets.Presets");
            Text = "TemplateForm";
            Theme = "DefaultType";
            ResumeLayout(false);
        }

        #endregion
    }
}