namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    partial class BeepContextMenu
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
                _submenuTimer?.Stop();
                _submenuTimer?.Dispose();
                _fadeTimer?.Stop();
                _fadeTimer?.Dispose();
                _openSubmenu?.Dispose();

                if (_scrollBar != null)
                {
                    _scrollBar.ValueChanged -= InternalScrollBarValueChanged;
                    _scrollBar.Dispose();
                }

                if (_searchTextBox != null)
                {
                    _searchTextBox.TextChanged -= SearchTextBox_TextChanged;
                    Controls.Remove(_searchTextBox);
                    _searchTextBox.Dispose();
                    _searchTextBox = null;
                }

                TeardownHostedList();
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
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "BeepContextMenu";
        }

        #endregion
    }
}