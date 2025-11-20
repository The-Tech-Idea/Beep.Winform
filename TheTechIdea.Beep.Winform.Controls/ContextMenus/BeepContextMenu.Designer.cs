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
                try { _submenuTimer?.Stop(); _submenuTimer?.Dispose(); } catch { }
                try { _fadeTimer?.Stop(); _fadeTimer?.Dispose(); } catch { }
                try { _openSubmenu?.Dispose(); } catch { }
                try { if (_scrollBar != null) { if (_scrollBar is VScrollBar v) v.Scroll -= ScrollBar_Scroll; else { var ev = _scrollBar.GetType().GetEvent("ValueChanged"); ev?.RemoveEventHandler(_scrollBar, new EventHandler((s, e) => InternalScrollBarValueChanged(s, e))); } } } catch { }
                try { if (_searchTextBox != null) { _searchTextBox.TextChanged -= SearchTextBox_TextChanged; Controls.Remove(_searchTextBox); _searchTextBox.Dispose(); _searchTextBox = null; } } catch { }
                _submenuTimer?.Stop();
                _submenuTimer?.Dispose();
                _fadeTimer?.Stop();
                _fadeTimer?.Dispose();
                _openSubmenu?.Dispose();
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