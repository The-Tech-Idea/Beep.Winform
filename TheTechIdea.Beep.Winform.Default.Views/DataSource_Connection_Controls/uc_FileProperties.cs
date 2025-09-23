using System;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    public partial class uc_FileProperties : uc_DataConnectionPropertiesBaseControl
    {
        public uc_FileProperties()
        {
            InitializeComponent();
        }
        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            ConnectionPropertytabPage.Text = "File";
            if (conn == null) return;

            File_FilePathbeepTextBox.DataBindings.Clear();
            File_FileNamebeepTextBox.DataBindings.Clear();
            File_ExtbeepTextBox.DataBindings.Clear();
            File_DelimiterbeepTextBox.DataBindings.Clear();

            File_FilePathbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.FilePath), true, DataSourceUpdateMode.OnPropertyChanged));
            File_FileNamebeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.FileName), true, DataSourceUpdateMode.OnPropertyChanged));
            File_ExtbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.Ext), true, DataSourceUpdateMode.OnPropertyChanged));
            // Delimiter is char; bind text and sync via events
            var bind = new Binding("Text", conn, nameof(conn.Delimiter), true, DataSourceUpdateMode.OnPropertyChanged, ' ');
            bind.Format += (s, e) => { if (e.DesiredType == typeof(string)) e.Value = e.Value?.ToString(); };
            bind.Parse += (s, e) => { var str = e.Value?.ToString(); e.Value = string.IsNullOrEmpty(str) ? ' ' : str![0]; };
            File_DelimiterbeepTextBox.DataBindings.Add(bind);
        }
    }
}
