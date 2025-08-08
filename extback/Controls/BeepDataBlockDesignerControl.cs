using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Container.Services;

namespace TheTechIdea.Beep.Winform.IDE.Extensions.Controls
{
    /// <summary>
    /// Oracle Forms-like Data Block Designer Control
    /// Provides visual design capabilities for BeepDataBlock components
    /// </summary>
    public partial class BeepDataBlockDesignerControl : UserControl
    {
        #region Fields
        private Panel designerSurface;
        private Panel propertiesPanel;
        private Panel toolboxPanel;
        private Splitter verticalSplitter;
        private Splitter horizontalSplitter;
        
        private IBeepDataBlock currentDataBlock;
        private EntityStructure currentEntity;
        private IBeepService beepService;
        private IDMEEditor dmeEditor;
        
        private PropertyGrid propertyGrid;
        private TreeView fieldTreeView;
        private ToolStrip designerToolStrip;
        private StatusStrip statusStrip;
        
        private List<FieldDesignerItem> fieldItems;
        private FieldDesignerItem selectedFieldItem;
        
        private Point dragStartPoint;
        private bool isDragging;
        
        // Oracle Forms-like properties
        private string dataSourceName;
        private string baseTable;
        private int maxRecords = 100;
        private bool queryAllowed = true;
        private bool insertAllowed = true;
        private bool updateAllowed = true;
        private bool deleteAllowed = true;
        #endregion

        #region Constructor
        public BeepDataBlockDesignerControl()
        {
            InitializeComponent();
            InitializeBeepServices();
            InitializeDesignerSurface();
            InitializeToolboxPanel();
            InitializePropertiesPanel();
            InitializeToolStrip();
            InitializeStatusStrip();
            SetupEventHandlers();
            
            fieldItems = new List<FieldDesignerItem>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current entity being designed
        /// </summary>
        public EntityStructure CurrentEntity
        {
            get => currentEntity;
            set
            {
                currentEntity = value;
                if (value != null)
                {
                    LoadEntityForDesign(value);
                }
            }
        }

        /// <summary>
        /// Oracle Forms-like Data Source Name property
        /// </summary>
        [Category("Data Block Properties")]
        [Description("The name of the data source for this block")]
        public string DataSourceName
        {
            get => dataSourceName;
            set
            {
                dataSourceName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Oracle Forms-like Base Table property
        /// </summary>
        [Category("Data Block Properties")]
        [Description("The base table for this data block")]
        public string BaseTable
        {
            get => baseTable;
            set
            {
                baseTable = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Initialization
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.Window;
            this.Size = new Size(800, 600);
            this.Name = "BeepDataBlockDesignerControl";
            
            this.ResumeLayout(false);
        }

        private void InitializeBeepServices()
        {
            try
            {
                beepService = new BeepService();
                dmeEditor = beepService.DMEEditor;
            }
            catch (Exception ex)
            {
                ShowError($"Error initializing Beep services: {ex.Message}");
            }
        }

        private void InitializeDesignerSurface()
        {
            designerSurface = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowDrop = true,
                AutoScroll = true
            };

            designerSurface.Paint += DesignerSurface_Paint;
            designerSurface.MouseDown += DesignerSurface_MouseDown;
            designerSurface.DragEnter += DesignerSurface_DragEnter;
            designerSurface.DragDrop += DesignerSurface_DragDrop;

            this.Controls.Add(designerSurface);
        }

        private void InitializeToolboxPanel()
        {
            toolboxPanel = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = SystemColors.Control,
                BorderStyle = BorderStyle.FixedSingle
            };

            var toolboxLabel = new Label()
            {
                Text = "Fields",
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = SystemColors.ControlDark
            };

            fieldTreeView = new TreeView()
            {
                Dock = DockStyle.Fill,
                ShowLines = true,
                FullRowSelect = true,
                AllowDrop = true
            };

            fieldTreeView.ItemDrag += FieldTreeView_ItemDrag;
            fieldTreeView.AfterSelect += FieldTreeView_AfterSelect;

            toolboxPanel.Controls.Add(fieldTreeView);
            toolboxPanel.Controls.Add(toolboxLabel);

            verticalSplitter = new Splitter()
            {
                Dock = DockStyle.Left,
                Width = 3,
                BackColor = SystemColors.ControlDark
            };

            this.Controls.Add(verticalSplitter);
            this.Controls.Add(toolboxPanel);
        }

        private void InitializePropertiesPanel()
        {
            propertiesPanel = new Panel()
            {
                Dock = DockStyle.Right,
                Width = 250,
                BackColor = SystemColors.Control,
                BorderStyle = BorderStyle.FixedSingle
            };

            var propertiesLabel = new Label()
            {
                Text = "Properties",
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = SystemColors.ControlDark
            };

            propertyGrid = new PropertyGrid()
            {
                Dock = DockStyle.Fill,
                PropertySort = PropertySort.Categorized,
                HelpVisible = true
            };

            propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;

            propertiesPanel.Controls.Add(propertyGrid);
            propertiesPanel.Controls.Add(propertiesLabel);

            horizontalSplitter = new Splitter()
            {
                Dock = DockStyle.Right,
                Width = 3,
                BackColor = SystemColors.ControlDark
            };

            this.Controls.Add(horizontalSplitter);
            this.Controls.Add(propertiesPanel);
        }

        private void InitializeToolStrip()
        {
            designerToolStrip = new ToolStrip()
            {
                Dock = DockStyle.Top,
                GripStyle = ToolStripGripStyle.Hidden
            };

            var newButton = new ToolStripButton("New Data Block");
            newButton.Click += NewDataBlock_Click;

            var saveButton = new ToolStripButton("Save");
            saveButton.Click += SaveDataBlock_Click;

            var generateButton = new ToolStripButton("Generate Code");
            generateButton.Click += GenerateCode_Click;

            designerToolStrip.Items.AddRange(new ToolStripItem[] 
            {
                newButton, new ToolStripSeparator(), saveButton, generateButton
            });

            this.Controls.Add(designerToolStrip);
        }

        private void InitializeStatusStrip()
        {
            statusStrip = new StatusStrip();
            var statusLabel = new ToolStripStatusLabel("Ready") { Spring = true };
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);
        }

        private void SetupEventHandlers()
        {
            this.Resize += BeepDataBlockDesignerControl_Resize;
        }
        #endregion

        #region Entity Management
        private void LoadEntityForDesign(EntityStructure entity)
        {
            try
            {
                BaseTable = entity.EntityName;
                DataSourceName = entity.DataSourceID;

                fieldTreeView.Nodes.Clear();
                ClearDesignerSurface();

                var fieldsNode = fieldTreeView.Nodes.Add("Fields");

                if (entity.Fields != null)
                {
                    foreach (var field in entity.Fields.OrderBy(f => f.fieldname))
                    {
                        var fieldNode = fieldsNode.Nodes.Add(field.fieldname);
                        fieldNode.Tag = field;
                        fieldNode.ToolTipText = $"{field.fieldtype} - {field.Size}";
                    }
                }

                fieldsNode.Expand();
                propertyGrid.SelectedObject = this;
                UpdateStatus($"Loaded entity: {entity.EntityName} with {entity.Fields?.Count ?? 0} fields");
            }
            catch (Exception ex)
            {
                ShowError($"Error loading entity: {ex.Message}");
            }
        }

        private void ClearDesignerSurface()
        {
            designerSurface.Controls.Clear();
            fieldItems.Clear();
            selectedFieldItem = null;
        }
        #endregion

        #region Event Handlers
        private void DesignerSurface_Paint(object sender, PaintEventArgs e)
        {
            DrawGrid(e.Graphics, designerSurface.ClientRectangle);
        }

        private void DrawGrid(Graphics g, Rectangle bounds)
        {
            int gridSize = 10;
            using (var pen = new Pen(Color.LightGray))
            {
                for (int x = 0; x < bounds.Width; x += gridSize)
                    g.DrawLine(pen, x, 0, x, bounds.Height);
                for (int y = 0; y < bounds.Height; y += gridSize)
                    g.DrawLine(pen, 0, y, bounds.Width, y);
            }
        }

        private void DesignerSurface_MouseDown(object sender, MouseEventArgs e)
        {
            if (selectedFieldItem != null)
            {
                selectedFieldItem.IsSelected = false;
                selectedFieldItem = null;
                propertyGrid.SelectedObject = this;
            }
        }

        private void DesignerSurface_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void DesignerSurface_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                var node = (TreeNode)e.Data.GetData(typeof(TreeNode));
                if (node.Tag is EntityField field)
                {
                    var dropPoint = designerSurface.PointToClient(new Point(e.X, e.Y));
                    dropPoint.X = (dropPoint.X / 10) * 10;
                    dropPoint.Y = (dropPoint.Y / 10) * 10;
                    AddFieldToDesigner(field, dropPoint);
                }
            }
        }

        private void FieldTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Item is TreeNode node && node.Tag is EntityField)
            {
                fieldTreeView.DoDragDrop(node, DragDropEffects.Copy);
            }
        }

        private void FieldTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is EntityField field)
            {
                propertyGrid.SelectedObject = field;
            }
        }

        private void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            OnPropertyChanged();
        }

        private void NewDataBlock_Click(object sender, EventArgs e)
        {
            ClearDesignerSurface();
            currentEntity = null;
            propertyGrid.SelectedObject = this;
            UpdateStatus("New data block created");
        }

        private void SaveDataBlock_Click(object sender, EventArgs e)
        {
            OnDataBlockSaved?.Invoke(this, EventArgs.Empty);
            UpdateStatus("Data block saved");
        }

        private void GenerateCode_Click(object sender, EventArgs e)
        {
            OnCodeGenerationRequested?.Invoke(this, EventArgs.Empty);
            UpdateStatus("Code generation requested");
        }

        private void BeepDataBlockDesignerControl_Resize(object sender, EventArgs e)
        {
            designerSurface.Invalidate();
        }
        #endregion

        #region Field Management
        private void AddFieldToDesigner(EntityField field, Point location)
        {
            try
            {
                var fieldItem = new FieldDesignerItem(field)
                {
                    Location = location,
                    Size = new Size(150, 25)
                };

                fieldItem.MouseDown += FieldItem_MouseDown;
                fieldItem.Click += FieldItem_Click;

                fieldItems.Add(fieldItem);
                designerSurface.Controls.Add(fieldItem);

                UpdateStatus($"Added field: {field.fieldname}");
            }
            catch (Exception ex)
            {
                ShowError($"Error adding field: {ex.Message}");
            }
        }

        private void FieldItem_MouseDown(object sender, MouseEventArgs e)
        {
            var fieldItem = sender as FieldDesignerItem;
            if (fieldItem != null)
            {
                if (selectedFieldItem != null)
                    selectedFieldItem.IsSelected = false;

                selectedFieldItem = fieldItem;
                selectedFieldItem.IsSelected = true;
                propertyGrid.SelectedObject = selectedFieldItem.Field;
            }
        }

        private void FieldItem_Click(object sender, EventArgs e)
        {
            var fieldItem = sender as FieldDesignerItem;
            if (fieldItem != null)
            {
                propertyGrid.SelectedObject = fieldItem.Field;
            }
        }
        #endregion

        #region Helper Methods
        private void UpdateStatus(string message)
        {
            if (statusStrip.Items.Count > 0)
                statusStrip.Items[0].Text = message;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Events
        public event EventHandler OnDataBlockSaved;
        public event EventHandler OnCodeGenerationRequested;
        public event EventHandler PropertyChanged;
        #endregion
    }

    #region Supporting Classes
    /// <summary>
    /// Visual representation of a field in the designer
    /// </summary>
    public class FieldDesignerItem : Panel
    {
        public EntityField Field { get; private set; }
        private bool isSelected;
        private Label labelControl;

        public FieldDesignerItem(EntityField field)
        {
            Field = field;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            
            labelControl = new Label
            {
                Text = field.fieldname,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 2, 5, 2)
            };

            Controls.Add(labelControl);
            BackColor = Color.LightBlue;
            BorderStyle = BorderStyle.FixedSingle;
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                BackColor = value ? Color.LightCoral : Color.LightBlue;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (IsSelected)
            {
                using (var pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, 1, 1, Width - 3, Height - 3);
                }
            }
        }
    }
    #endregion
}
