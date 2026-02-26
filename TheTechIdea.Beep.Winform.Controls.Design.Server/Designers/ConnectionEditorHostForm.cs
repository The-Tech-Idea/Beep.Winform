using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Services;
using TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class ConnectionEditorHostForm : Form
    {
        private readonly uc_DataConnectionBase _editor;

        public ConnectionEditorHostForm(ConnectionProperties connectionProperties, IBeepService? beepService, bool requireSuccessfulTestBeforeSave)
        {
            if (connectionProperties == null)
            {
                throw new ArgumentNullException(nameof(connectionProperties));
            }

            Text = string.IsNullOrWhiteSpace(connectionProperties.ConnectionName)
                ? "Add Connection"
                : $"Edit Connection - {connectionProperties.ConnectionName}";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(760, 860);
            Size = new Size(800, 900);
            FormBorderStyle = FormBorderStyle.Sizable;

            _editor = new uc_DataConnectionBase
            {
                Dock = DockStyle.Fill,
                BeepService = beepService,
                RequireSuccessfulTestBeforeSave = requireSuccessfulTestBeforeSave
            };
            _editor.InitializeDialog(connectionProperties);

            Controls.Add(_editor);
        }

        public ConnectionProperties GetUpdatedConnection()
        {
            return _editor.GetUpdatedProperties();
        }
    }
}
