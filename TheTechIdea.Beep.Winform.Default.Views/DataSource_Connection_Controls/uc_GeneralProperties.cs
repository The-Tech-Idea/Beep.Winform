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
    public partial class uc_GeneralProperties : uc_DataConnectionPropertiesBaseControl
    {
        public uc_GeneralProperties()
        {
            InitializeComponent();
        }
        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            ConnectionPropertytabPage.Text = "General";
            if (conn == null) return;

            // Clear existing bindings
            General_IDbeepTextBox.DataBindings.Clear();
            General_GuidIDbeepTextBox.DataBindings.Clear();
            General_ConnectionNamebeepTextBox.DataBindings.Clear();
            General_CompositeLayerNamebeepTextBox.DataBindings.Clear();

            // Bindings
            General_IDbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ID), true, DataSourceUpdateMode.OnPropertyChanged));
            General_GuidIDbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.GuidID), true, DataSourceUpdateMode.OnPropertyChanged));
            General_ConnectionNamebeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ConnectionName), true, DataSourceUpdateMode.OnPropertyChanged));
            General_CompositeLayerNamebeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.CompositeLayerName), true, DataSourceUpdateMode.OnPropertyChanged));

            General_IDbeepTextBox.ReadOnly = true;
            General_GuidIDbeepTextBox.ReadOnly = true;
        }
    }
}
