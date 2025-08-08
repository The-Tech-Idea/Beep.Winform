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
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Container.Services;

namespace TheTechIdea.Beep.Winform.IDE.Extensions.Controls
{
    /// <summary>
    /// Control for managing data sources in the IDE similar to SQL Server Compact Toolbox
    /// Features BeepTree for data source visualization and management
    /// </summary>
    public partial class BeepDataSourcesControl : UserControl
    {
        #region Fields
        private BeepTree dataSourcesTree;
        private ToolStrip toolStrip;
        private ContextMenuStrip contextMenu;
        private IBeepService beepService;
        private IDMEEditor dmeEditor;
        private List<ConnectionProperties> dataSources;
        private SimpleItem rootNode;
        private Timer refreshTimer;
        private Panel mainPanel;
        private Panel infoPanel;
        private Label statusLabel;
        private ProgressBar progressBar;
        #endregion

        #region Constructor
        public BeepDataSourcesControl()
        {
            InitializeComponent();
            InitializeBeepServices();
            InitializeDataSourcesTree();
            CreateToolStrip();
            CreateContextMenu();
            CreateInfoPanel();
            SetupRefreshTimer();
            LoadDataSources();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current DME Editor instance
        /// </summary>
        public IDMEEditor DMEEditor
        {
            get => dmeEditor;
            set
            {
                dmeEditor = value;
                if (dataSourcesTree != null)
                {
                    LoadDataSources();
                }
            }
        }

        /// <summary>
        /// Gets the currently selected data source
        /// </summary>
        public ConnectionProperties SelectedDataSource
        {
            get
            {
                var selectedNode = dataSourcesTree?.SelectedNode as SimpleItem;
                return selectedNode?.Tag as ConnectionProperties;
            }
        }

        /// <summary>
        /// Gets the currently selected entity
        /// </summary>
        public string SelectedEntity
        {
            get
            {
                var selectedNode = dataSourcesTree?.SelectedNode as SimpleItem;
                if (selectedNode?.Tag is EntityStructure entity)
                {
                    return entity.EntityName;
                }
                if (selectedNode?.Tag is string entityName)
                {
                    return entityName;
                }
                return null;
            }
        }
        #endregion

        #region Initialization
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Main container
            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.Window;
            this.Size = new Size(300, 600);
            this.Name = "BeepDataSourcesControl";
            
            this.ResumeLayout(false);
        }

        private void InitializeBeepServices()
        {
            try
            {
                // Initialize the Beep framework
                beepService = new BeepService();
                dmeEditor = beepService.DMEEditor;
            }
            catch (Exception ex)
            {
                ShowError($"Error initializing Beep services: {ex.Message}");
            }
        }

        private void InitializeDataSourcesTree()
        {
            // Main panel container
            mainPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = SystemColors.Window
            };

            dataSourcesTree = new BeepTree()
            {
                Dock = DockStyle.Fill,
                ShowRootLines = false,
                ShowLines = true,
                FullRowSelect = true,
                HideSelection = false,
                AllowDrop = true,
                Theme = "DefaultTheme",
                ShowCheckBoxes = false,
                ShowImages = true,
                NodeHeight = 22,
                Indent = 20,
                ShowNodeToolTips = true
            };

            // Configure tree events
            dataSourcesTree.NodeDoubleClick += DataSourcesTree_NodeDoubleClick;
            dataSourcesTree.NodeMouseClick += DataSourcesTree_NodeMouseClick;
            dataSourcesTree.DragDrop += DataSourcesTree_DragDrop;
            dataSourcesTree.DragEnter += DataSourcesTree_DragEnter;
            dataSourcesTree.AfterSelect += DataSourcesTree_AfterSelect;
            dataSourcesTree.BeforeExpand += DataSourcesTree_BeforeExpand;

            mainPanel.Controls.Add(dataSourcesTree);
            this.Controls.Add(mainPanel);
        }

        private void CreateToolStrip()
        {
            toolStrip = new ToolStrip()
            {
                Dock = DockStyle.Top,
                GripStyle = ToolStripGripStyle.Hidden,
                RenderMode = ToolStripRenderMode.System,
                ImageScalingSize = new Size(16, 16)
            };

            // Add Data Source button
            var addButton = new ToolStripButton("Add Data Source")
            {
                ToolTipText = "Add a new data source connection",
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                Image = CreateIcon("➕", Color.Green)
            };
            addButton.Click += AddDataSource_Click;

            // Refresh button
            var refreshButton = new ToolStripButton("Refresh")
            {
                ToolTipText = "Refresh data sources",
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                Image = CreateIcon("🔄", Color.Blue)
            };
            refreshButton.Click += Refresh_Click;

            // Test Connection button
            var testButton = new ToolStripButton("Test")
            {
                ToolTipText = "Test selected connection",
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                Image = CreateIcon("🔍", Color.Orange)
            };
            testButton.Click += TestConnection_Click;

            // Import button
            var importButton = new ToolStripButton("Import")
            {
                ToolTipText = "Import data source from file",
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                Image = CreateIcon("📁", Color.Purple)
            };
            importButton.Click += ImportDataSource_Click;

            toolStrip.Items.AddRange(new ToolStripItem[] 
            { 
                addButton, 
                new ToolStripSeparator(),
                refreshButton, 
                testButton,
                new ToolStripSeparator(),
                importButton
            });

            this.Controls.Add(toolStrip);
        }

        private void CreateContextMenu()
        {
            contextMenu = new ContextMenuStrip();
            contextMenu.Opening += ContextMenu_Opening;

            // Create all menu items
            var addItem = new ToolStripMenuItem("Add Data Source", CreateIcon("➕", Color.Green)) 
            { Tag = "add" };
            addItem.Click += AddDataSource_Click;

            var editItem = new ToolStripMenuItem("Edit Connection", CreateIcon("✏️", Color.Blue)) 
            { Tag = "edit" };
            editItem.Click += EditConnection_Click;

            var testItem = new ToolStripMenuItem("Test Connection", CreateIcon("🔍", Color.Orange)) 
            { Tag = "test" };
            testItem.Click += TestConnection_Click;

            var deleteItem = new ToolStripMenuItem("Delete", CreateIcon("🗑️", Color.Red)) 
            { Tag = "delete" };
            deleteItem.Click += DeleteDataSource_Click;

            var sep1 = new ToolStripSeparator() { Tag = "sep1" };

            var createBlockItem = new ToolStripMenuItem("Create Data Block", CreateIcon("🎯", Color.Green)) 
            { Tag = "create_block" };
            createBlockItem.Click += CreateDataBlock_Click;

            var generateFormItem = new ToolStripMenuItem("Generate Form", CreateIcon("📋", Color.Blue)) 
            { Tag = "generate_form" };
            generateFormItem.Click += GenerateForm_Click;

            var viewDataItem = new ToolStripMenuItem("View Data", CreateIcon("👁️", Color.Purple)) 
            { Tag = "view_data" };
            viewDataItem.Click += ViewData_Click;

            var sep2 = new ToolStripSeparator() { Tag = "sep2" };

            var propertiesItem = new ToolStripMenuItem("Properties", CreateIcon("⚙️", Color.Gray)) 
            { Tag = "properties" };
            propertiesItem.Click += ShowProperties_Click;

            contextMenu.Items.AddRange(new ToolStripItem[] {
                addItem, editItem, testItem, deleteItem, sep1,
                createBlockItem, generateFormItem, viewDataItem, sep2, propertiesItem
            });

            dataSourcesTree.ContextMenuStrip = contextMenu;
        }

        private void CreateInfoPanel()
        {
            infoPanel = new Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = SystemColors.Control,
                BorderStyle = BorderStyle.FixedSingle
            };

            statusLabel = new Label()
            {
                Dock = DockStyle.Top,
                Height = 20,
                Text = "Ready",
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 2, 5, 2)
            };

            progressBar = new ProgressBar()
            {
                Dock = DockStyle.Bottom,
                Height = 20,
                Style = ProgressBarStyle.Continuous,
                Visible = false
            };

            infoPanel.Controls.Add(statusLabel);
            infoPanel.Controls.Add(progressBar);
            this.Controls.Add(infoPanel);
        }

        private void SetupRefreshTimer()
        {
            refreshTimer = new Timer()
            {
                Interval = 30000, // 30 seconds
                Enabled = false
            };
            refreshTimer.Tick += (s, e) => 
            {
                if (!progressBar.Visible)
                {
                    LoadDataSources();
                }
            };
        }
        #endregion

        #region Data Source Management
        private async void LoadDataSources()
        {
            try
            {
                SetStatus("Loading data sources...");
                ShowProgress(true);

                await Task.Run(() =>
                {
                    this.Invoke(new Action(() =>
                    {
                        dataSourcesTree.BeginUpdate();
                        dataSourcesTree.Nodes.Clear();

                        // Create root node
                        rootNode = dataSourcesTree.AddRootNode("Data Sources", "datasources");
                        rootNode.IsExpanded = true;

                        // Load existing data sources
                        if (dmeEditor?.ConfigEditor?.DataConnections != null)
                        {
                            var connections = dmeEditor.ConfigEditor.DataConnections
                                .OrderBy(c => c.Category)
                                .ThenBy(c => c.ConnectionName);

                            var categories = connections.GroupBy(c => c.Category ?? "General");

                            foreach (var category in categories)
                            {
                                var categoryNode = rootNode.AddChild($"{category.Key} ({category.Count()})", "folder");
                                categoryNode.Tag = category.Key;
                                categoryNode.IsExpanded = true;

                                foreach (var connection in category)
                                {
                                    AddDataSourceNode(categoryNode, connection);
                                }
                            }
                        }

                        dataSourcesTree.EndUpdate();
                        rootNode.Expand();
                    }));
                });

                SetStatus($"Loaded {dmeEditor?.ConfigEditor?.DataConnections?.Count ?? 0} data sources");
            }
            catch (Exception ex)
            {
                SetStatus("Error loading data sources");
                ShowError($"Error loading data sources: {ex.Message}");
            }
            finally
            {
                ShowProgress(false);
            }
        }

        private void AddDataSourceNode(SimpleItem parentNode, ConnectionProperties connection)
        {
            var icon = GetDatabaseIcon(connection.DatabaseType);
            var connectionNode = parentNode.AddChild(connection.ConnectionName, icon);
            connectionNode.Tag = connection;
            connectionNode.ToolTipText = $"{connection.DatabaseType}\n{connection.Host ?? connection.FilePath}\nStatus: {GetConnectionStatus(connection)}";

            // Add a placeholder for entities (lazy loading)
            var placeholder = connectionNode.AddChild("Loading...", "loading");
            placeholder.Tag = "placeholder";
        }

        private async void LoadEntitiesForConnection(SimpleItem connectionNode, ConnectionProperties connection)
        {
            try
            {
                SetStatus($"Loading entities for {connection.ConnectionName}...");
                ShowProgress(true);

                await Task.Run(() =>
                {
                    try
                    {
                        var dataSource = dmeEditor.GetDataSource(connection.ConnectionName);
                        if (dataSource != null)
                        {
                            var entities = dataSource.GetEntitesList();
                            var entityStructures = new List<EntityStructure>();

                            foreach (var entityName in entities)
                            {
                                try
                                {
                                    var structure = dataSource.GetEntityStructure(entityName, false);
                                    if (structure != null)
                                    {
                                        entityStructures.Add(structure);
                                    }
                                }
                                catch
                                {
                                    // Create basic structure if we can't get full details
                                    entityStructures.Add(new EntityStructure { EntityName = entityName });
                                }
                            }

                            this.Invoke(new Action(() =>
                            {
                                // Remove placeholder
                                connectionNode.Children.Clear();

                                // Group entities by type
                                var tables = entityStructures.Where(e => e.Viewtype == ViewType.Table || e.Viewtype == ViewType.Unknown).ToList();
                                var views = entityStructures.Where(e => e.Viewtype == ViewType.View).ToList();

                                if (tables.Any())
                                {
                                    var tablesNode = connectionNode.AddChild($"Tables ({tables.Count})", "tables");
                                    foreach (var table in tables.OrderBy(t => t.EntityName))
                                    {
                                        var tableNode = tablesNode.AddChild(table.EntityName, "table");
                                        tableNode.Tag = table;
                                        tableNode.ToolTipText = $"Table: {table.EntityName}\nFields: {table.Fields?.Count ?? 0}";
                                    }
                                }

                                if (views.Any())
                                {
                                    var viewsNode = connectionNode.AddChild($"Views ({views.Count})", "views");
                                    foreach (var view in views.OrderBy(v => v.EntityName))
                                    {
                                        var viewNode = viewsNode.AddChild(view.EntityName, "view");
                                        viewNode.Tag = view;
                                        viewNode.ToolTipText = $"View: {view.EntityName}\nFields: {view.Fields?.Count ?? 0}";
                                    }
                                }

                                connectionNode.Expand();
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() =>
                        {
                            connectionNode.Children.Clear();
                            var errorNode = connectionNode.AddChild($"Error: {ex.Message}", "error");
                            errorNode.Tag = ex;
                        }));
                    }
                });

                SetStatus("Ready");
            }
            catch (Exception ex)
            {
                SetStatus("Error loading entities");
                ShowError($"Error loading entities: {ex.Message}");
            }
            finally
            {
                ShowProgress(false);
            }
        }

        private string GetDatabaseIcon(DataSourceType dbType)
        {
            return dbType switch
            {
                DataSourceType.SqlServer => "sqlserver",
                DataSourceType.Oracle => "oracle", 
                DataSourceType.MySql => "mysql",
                DataSourceType.PostgreSql => "postgresql",
                DataSourceType.SQLite => "sqlite",
                DataSourceType.OleDb => "access",
                DataSourceType.Odbc => "odbc",
                _ => "database"
            };
        }

        private string GetConnectionStatus(ConnectionProperties connection)
        {
            try
            {
                var dataSource = dmeEditor.GetDataSource(connection.ConnectionName);
                return dataSource?.ConnectionStatus.ToString() ?? "Unknown";
            }
            catch
            {
                return "Error";
            }
        }

        private Bitmap CreateIcon(string text, Color color)
        {
            var bitmap = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);
                using (var brush = new SolidBrush(color))
                using (var font = new Font("Segoe UI Emoji", 10, FontStyle.Bold))
                {
                    g.DrawString(text, font, brush, new RectangleF(0, 0, 16, 16));
                }
            }
            return bitmap;
        }
        #endregion

        #region Event Handlers - Part 1
        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            var selectedNode = dataSourcesTree.SelectedNode as SimpleItem;
            
            // Show/hide menu items based on selection
            foreach (ToolStripItem item in contextMenu.Items)
            {
                if (item.Tag == null) continue;

                switch (item.Tag.ToString())
                {
                    case "edit":
                    case "test":
                    case "delete":
                        item.Visible = selectedNode?.Tag is ConnectionProperties;
                        break;
                    case "create_block":
                    case "generate_form":
                    case "view_data":
                        item.Visible = selectedNode?.Tag is EntityStructure;
                        break;
                    case "sep1":
                        item.Visible = selectedNode?.Tag is ConnectionProperties;
                        break;
                    case "sep2":
                        item.Visible = selectedNode?.Tag is EntityStructure;
                        break;
                }
            }
        }

        private void AddDataSource_Click(object sender, EventArgs e)
        {
            try
            {
                // For now, show a simple input dialog
                var dialog = MessageBox.Show("Data Source Configuration Dialog not yet implemented.\nWould you like to create a sample SQLite connection?", 
                    "Add Data Source", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (dialog == DialogResult.Yes)
                {
                    CreateSampleConnection();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error adding data source: {ex.Message}");
            }
        }

        private void CreateSampleConnection()
        {
            try
            {
                var connection = new ConnectionProperties
                {
                    ConnectionName = $"Sample_SQLite_{DateTime.Now:HHmmss}",
                    DatabaseType = DataSourceType.SQLite,
                    ConnectionString = ":memory:",
                    Category = "Sample Connections"
                };

                if (dmeEditor.ConfigEditor.DataConnections.Any(c => c.ConnectionName == connection.ConnectionName))
                {
                    connection.ConnectionName += "_" + Guid.NewGuid().ToString("N")[..8];
                }

                dmeEditor.ConfigEditor.DataConnections.Add(connection);
                dmeEditor.ConfigEditor.SaveConfigurationValues();
                
                LoadDataSources();
                OnDataSourceAdded?.Invoke(this, EventArgs.Empty);
                SetStatus($"Added sample data source: {connection.ConnectionName}");
            }
            catch (Exception ex)
            {
                ShowError($"Error creating sample connection: {ex.Message}");
            }
        }
        #endregion

        #region Event Handlers - Part 2
        private void EditConnection_Click(object sender, EventArgs e)
        {
            var selectedNode = dataSourcesTree.SelectedNode as SimpleItem;
            if (selectedNode?.Tag is ConnectionProperties connection)
            {
                MessageBox.Show($"Edit Connection Dialog not yet implemented.\n\nConnection: {connection.ConnectionName}\nType: {connection.DatabaseType}", 
                    "Edit Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void TestConnection_Click(object sender, EventArgs e)
        {
            var selectedNode = dataSourcesTree.SelectedNode as SimpleItem;
            if (selectedNode?.Tag is ConnectionProperties connection)
            {
                try
                {
                    SetStatus($"Testing connection to {connection.ConnectionName}...");
                    ShowProgress(true);

                    var result = await Task.Run(() =>
                    {
                        try
                        {
                            var dataSource = dmeEditor.GetDataSource(connection.ConnectionName);
                            return dataSource?.ConnectionStatus == ConnectionState.Open;
                        }
                        catch
                        {
                            return false;
                        }
                    });
                    
                    var message = result ? "Connection test successful!" : "Connection test failed!";
                    var icon = result ? MessageBoxIcon.Information : MessageBoxIcon.Warning;
                    
                    MessageBox.Show(message, "Connection Test", MessageBoxButtons.OK, icon);
                    SetStatus($"Connection test {(result ? "successful" : "failed")}");
                }
                catch (Exception ex)
                {
                    SetStatus("Connection test failed");
                    ShowError($"Connection test failed: {ex.Message}");
                }
                finally
                {
                    ShowProgress(false);
                }
            }
        }

        private void DeleteDataSource_Click(object sender, EventArgs e)
        {
            var selectedNode = dataSourcesTree.SelectedNode as SimpleItem;
            if (selectedNode?.Tag is ConnectionProperties connection)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the data source '{connection.ConnectionName}'?\n\nThis action cannot be undone.", 
                    "Delete Data Source", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        dmeEditor.ConfigEditor.DataConnections.Remove(connection);
                        dmeEditor.ConfigEditor.SaveConfigurationValues();
                        LoadDataSources();
                        OnDataSourceDeleted?.Invoke(this, connection);
                        SetStatus($"Deleted data source: {connection.ConnectionName}");
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Error deleting data source: {ex.Message}");
                    }
                }
            }
        }

        private void CreateDataBlock_Click(object sender, EventArgs e)
        {
            var selectedNode = dataSourcesTree.SelectedNode as SimpleItem;
            if (selectedNode?.Tag is EntityStructure entity)
            {
                MessageBox.Show($"Create Data Block Dialog not yet implemented.\n\nEntity: {entity.EntityName}\nFields: {entity.Fields?.Count ?? 0}", 
                    "Create Data Block", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OnDataBlockRequested?.Invoke(this, entity);
            }
        }

        private void GenerateForm_Click(object sender, EventArgs e)
        {
            var selectedNode = dataSourcesTree.SelectedNode as SimpleItem;
            if (selectedNode?.Tag is EntityStructure entity)
            {
                MessageBox.Show($"Form Generation not yet implemented.\n\nEntity: {entity.EntityName}\nThis will generate a complete Windows Form with BeepDataBlock.", 
                    "Generate Form", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OnFormGenerated?.Invoke(this, entity);
                SetStatus($"Generated form for: {entity.EntityName}");
            }
        }

        private void ViewData_Click(object sender, EventArgs e)
        {
            var selectedNode = dataSourcesTree.SelectedNode as SimpleItem;
            if (selectedNode?.Tag is EntityStructure entity)
            {
                MessageBox.Show($"Data Viewer not yet implemented.\n\nEntity: {entity.EntityName}\nThis will open a data grid to view the entity data.", 
                    "View Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OnDataViewRequested?.Invoke(this, entity);
            }
        }

        private void ShowProperties_Click(object sender, EventArgs e)
        {
            var selectedNode = dataSourcesTree.SelectedNode as SimpleItem;
            if (selectedNode?.Tag != null)
            {
                OnPropertiesRequested?.Invoke(this, selectedNode.Tag);
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            LoadDataSources();
        }

        private void ImportDataSource_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Database Files|*.db;*.sqlite;*.mdb;*.accdb|SQLite Files|*.db;*.sqlite|Access Files|*.mdb;*.accdb|All Files|*.*";
                openFileDialog.Title = "Select Database File";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    CreateConnectionFromFile(openFileDialog.FileName);
                }
            }
        }
        #endregion

        #region Tree Event Handlers
        private void DataSourcesTree_NodeDoubleClick(object sender, EventArgs e)
        {
            var selectedNode = dataSourcesTree.SelectedNode as SimpleItem;
            if (selectedNode?.Tag != null)
            {
                if (selectedNode.Tag is ConnectionProperties connection)
                {
                    // Test connection on double-click
                    TestConnection_Click(sender, e);
                }
                else if (selectedNode.Tag is EntityStructure entity)
                {
                    // Create data block on double-click
                    CreateDataBlock_Click(sender, e);
                }
            }
        }

        private void DataSourcesTree_NodeMouseClick(object sender, EventArgs e)
        {
            if (e is MouseEventArgs mouseArgs && mouseArgs.Button == MouseButtons.Right)
            {
                var hitTest = dataSourcesTree.HitTest(mouseArgs.Location);
                if (hitTest.Node != null)
                {
                    dataSourcesTree.SelectedNode = hitTest.Node;
                }
            }
        }

        private void DataSourcesTree_AfterSelect(object sender, EventArgs e)
        {
            var selectedNode = dataSourcesTree.SelectedNode as SimpleItem;
            OnSelectionChanged?.Invoke(this, selectedNode?.Tag);
        }

        private void DataSourcesTree_BeforeExpand(object sender, EventArgs e)
        {
            var node = dataSourcesTree.SelectedNode as SimpleItem;
            if (node?.Tag is ConnectionProperties connection)
            {
                // Check if we need to load entities
                if (node.Children.Any() && node.Children.First().Tag?.ToString() == "placeholder")
                {
                    LoadEntitiesForConnection(node, connection);
                }
            }
        }

        private void DataSourcesTree_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Any(f => IsSupportedDatabaseFile(f)))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void DataSourcesTree_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files.Where(f => IsSupportedDatabaseFile(f)))
                {
                    CreateConnectionFromFile(file);
                }
            }
        }

        private bool IsSupportedDatabaseFile(string filePath)
        {
            var extension = System.IO.Path.GetExtension(filePath).ToLower();
            return extension == ".db" || extension == ".sqlite" || extension == ".mdb" || extension == ".accdb";
        }

        private void CreateConnectionFromFile(string filePath)
        {
            try
            {
                var fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                var extension = System.IO.Path.GetExtension(filePath).ToLower();
                
                DataSourceType dbType = extension switch
                {
                    ".db" or ".sqlite" => DataSourceType.SQLite,
                    ".mdb" or ".accdb" => DataSourceType.OleDb,
                    _ => DataSourceType.SQLite
                };

                var connection = new ConnectionProperties
                {
                    ConnectionName = fileName,
                    DatabaseType = dbType,
                    FilePath = filePath,
                    Category = "File Databases"
                };

                // Set appropriate connection string
                if (dbType == DataSourceType.SQLite)
                {
                    connection.ConnectionString = $"Data Source={filePath};";
                }
                else if (dbType == DataSourceType.OleDb)
                {
                    connection.ConnectionString = extension == ".accdb" 
                        ? $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};"
                        : $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};";
                }

                // Check if connection already exists
                if (dmeEditor.ConfigEditor.DataConnections.Any(c => c.ConnectionName.Equals(fileName, StringComparison.OrdinalIgnoreCase)))
                {
                    var result = MessageBox.Show(
                        $"A data source with the name '{fileName}' already exists. Do you want to replace it?",
                        "Data Source Exists",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                    {
                        return;
                    }

                    // Remove existing connection
                    var existing = dmeEditor.ConfigEditor.DataConnections.First(c => c.ConnectionName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                    dmeEditor.ConfigEditor.DataConnections.Remove(existing);
                }

                // Add new connection
                dmeEditor.ConfigEditor.DataConnections.Add(connection);
                dmeEditor.ConfigEditor.SaveConfigurationValues();
                
                LoadDataSources();
                OnDataSourceAdded?.Invoke(this, EventArgs.Empty);
                SetStatus($"Added data source: {fileName}");
            }
            catch (Exception ex)
            {
                ShowError($"Error creating connection from file: {ex.Message}");
            }
        }
        #endregion

        #region Helper Methods
        private void SetStatus(string message)
        {
            if (statusLabel != null)
            {
                statusLabel.Text = message;
                statusLabel.Refresh();
            }
        }

        private void ShowProgress(bool show)
        {
            if (progressBar != null)
            {
                progressBar.Visible = show;
                if (show)
                {
                    progressBar.Style = ProgressBarStyle.Marquee;
                }
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                refreshTimer?.Dispose();
                contextMenu?.Dispose();
                toolStrip?.Dispose();
                dataSourcesTree?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised when properties should be displayed for an object
        /// </summary>
        public event EventHandler<object> OnPropertiesRequested;

        /// <summary>
        /// Raised when an entity is selected
        /// </summary>
        public event EventHandler<object> OnSelectionChanged;

        /// <summary>
        /// Raised when a data source is added
        /// </summary>
        public event EventHandler OnDataSourceAdded;

        /// <summary>
        /// Raised when a data source is modified
        /// </summary>
        public event EventHandler<ConnectionProperties> OnDataSourceModified;

        /// <summary>
        /// Raised when a data source is deleted
        /// </summary>
        public event EventHandler<ConnectionProperties> OnDataSourceDeleted;

        /// <summary>
        /// Raised when a data block creation is requested
        /// </summary>
        public event EventHandler<EntityStructure> OnDataBlockRequested;

        /// <summary>
        /// Raised when form generation is completed
        /// </summary>
        public event EventHandler<EntityStructure> OnFormGenerated;

        /// <summary>
        /// Raised when data viewing is requested
        /// </summary>
        public event EventHandler<EntityStructure> OnDataViewRequested;
        #endregion
    }
