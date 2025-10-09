using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm
    {
        // Public events
        public event EventHandler OnFormClose;
        public event EventHandler OnFormLoad;
        public event EventHandler OnFormShown;
        public event EventHandler<FormClosingEventArgs> PreClose;

        // Lifecycle handlers moved for clarity
        private void BeepiForm_Load(object? sender, EventArgs e)
        {
            if (InDesignHost) return;
            if (BackColor == Color.Transparent || BackColor == Color.Empty)
                BackColor = SystemColors.Control;
            ApplyTheme();
            Invalidate();
            Update();
            OnFormLoad?.Invoke(this, EventArgs.Empty);
        }

        private void BeepiForm_VisibleChanged(object? sender, EventArgs e)
        {
            if (InDesignHost) return;
            if (Visible)
            {
                if (BackColor == Color.Transparent || BackColor == Color.Empty)
                    BackColor = _currentTheme?.BackColor ?? SystemColors.Control;
                Invalidate();
                Update();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (InDesignHost) return;
            try { beepuiManager1.Initialize(this); } catch { }
            OnFormShown?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (InDesignHost) return;
        }
    }
}
