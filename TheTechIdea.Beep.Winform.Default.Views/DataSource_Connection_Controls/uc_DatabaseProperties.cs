using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    public partial class uc_DatabaseProperties : uc_DataConnectionPropertiesBaseControl
    {
        public uc_DatabaseProperties()
        {
            InitializeComponent();
        }
        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            ConnectionPropertytabPage.Text = "Database";
            if (conn == null) return;

            Database_HostbeepTextBox.DataBindings.Clear();
            Database_PortbeepTextBox.DataBindings.Clear();
            Database_DatabasebeepTextBox.DataBindings.Clear();
            Database_SchemaNamebeepTextBox.DataBindings.Clear();
            Database_OracleSIDorServicebeepTextBox.DataBindings.Clear();

            Database_HostbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.Host), true, DataSourceUpdateMode.OnPropertyChanged));
            Database_PortbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.Port), true, DataSourceUpdateMode.OnPropertyChanged));
            Database_DatabasebeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.Database), true, DataSourceUpdateMode.OnPropertyChanged));
            Database_SchemaNamebeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.SchemaName), true, DataSourceUpdateMode.OnPropertyChanged));
            Database_OracleSIDorServicebeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.OracleSIDorService), true, DataSourceUpdateMode.OnPropertyChanged));
        }
    }
}
