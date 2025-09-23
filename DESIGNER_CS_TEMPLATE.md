# Designer.cs Template for Connection Controls

This template creates a proper WinForms Designer.cs file for connection controls. Controls and TabPages must be created in Designer.cs, and added to `beepTabs1`.

## Template Structure:

```csharp
namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    partial class {CONTROL_NAME}
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
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "{CONTROL_NAME}";
            this.Size = new System.Drawing.Size(800, 600);

            // Create TabPages and controls
            // Example:
            // this.{TABPAGE_VAR} = new System.Windows.Forms.TabPage();
            // this.{CONTROL_VAR} = new TheTechIdea.Beep.Winform.Controls.{BeepControlType}();
            // this.{TABPAGE_VAR}.Text = "{TAB_CAPTION}";
            // this.{TABPAGE_VAR}.Controls.Add(this.{CONTROL_VAR});
            // this.beepTabs1.Controls.Add(this.{TABPAGE_VAR});

            // {SERVICE_TYPE}-specific controls
            {CONTROL_DECLARATIONS}
        }

        #endregion

    // {SERVICE_TYPE}-specific controls field declarations (including TabPages)
        {FIELD_DECLARATIONS}
    }
}
```

## Usage Instructions:

1. Replace `{CONTROL_NAME}` with actual control name (e.g., uc_AWSSNSConnection)
2. Replace `{SERVICE_TYPE}` with service description (e.g., AWS SNS)
3. Replace `{CONTROL_DECLARATIONS}` with proper control instantiation code
4. Replace `{FIELD_DECLARATIONS}` with private field declarations

## Control Declaration Template:

For each control referenced in the .cs file:

```csharp
// Control instantiation
this.{controlName} = new TheTechIdea.Beep.Winform.Controls.{BeepControlType}();

// Basic configuration
this.{controlName}.Location = new System.Drawing.Point(24, {yPosition});
this.{controlName}.Name = "{controlName}";
this.{controlName}.PlaceholderText = "{placeholder}";
this.{controlName}.Size = new System.Drawing.Size(380, 40);
this.{controlName}.Multiline = false;
this.{controlName}.WordWrap = false;
this.{controlName}.AcceptsTab = false;
this.{controlName}.HelpText = "{description}";

// For password fields:
this.{controlName}.UseSystemPasswordChar = true;

// For ComboBox:
this.{controlName}.Items.AddRange(new object[] { "Option1", "Option2", "Option3" });
this.{controlName}.SelectedIndex = 0;
```

## Field Declaration Template:

```csharp
private TheTechIdea.Beep.Winform.Controls.{BeepControlType} {controlName};
```

## Completed Example:
- ✅ uc_AWSSNSConnection - 6 controls (topicArnTextBox, regionTextBox, accessKeyTextBox, secretKeyTextBox, sessionTokenTextBox, messageFormatComboBox)

## Remaining Controls to Fix:
- uc_AWSAthenaConnection
- uc_AWSGlueConnection  
- uc_AWSRedshiftConnection
- uc_ArangoDBConnection
- uc_AzureCloudConnection
- uc_ClickHouseConnection
- uc_CouchbaseConnection
- uc_DataBricksConnection
- uc_ActivitiConnection
- uc_ApacheAirflowConnection
- uc_Neo4jConnection
- uc_OrientDBConnection
- uc_SparkConnection
- uc_FlinkConnection
- uc_KafkaStreamsConnection
- uc_HadoopConnection
- uc_FirebaseConnection