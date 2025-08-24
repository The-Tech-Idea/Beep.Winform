using System.Data;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{

    public partial class BeepFileDialog : BeepiForm
    {
        public List<string> SelectedFiles { get; private set; } = new List<string>();
        public string Filter { get; set; } = "All Files (*.*)|*.*";
        public BeepFileDialog()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            Text = "Beep File Dialog";
            Size = new Size(900, 600);

            // Special Folders ComboBox

            foreach (var folder in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                SimpleItem item = new SimpleItem();
                item.Text = folder.ToString();
                item.Value = folder;
                _specialFoldersComboBox.ListItems.Add(item);

            }
            _specialFoldersComboBox.SelectedItemChanged += OnSpecialFolderSelected;



            _searchBox.TextChanged += OnSearchTextChanged;


            // File ListView
            _fileListView = new ListView
            {
                View = View.Details,
                Dock = DockStyle.Left,
                Width = 600,
                FullRowSelect = true,
                MultiSelect = true
            };
            _fileListView.Columns.Add("Name", 400);
            _fileListView.Columns.Add("Value", 100);
            _fileListView.Columns.Add("Type", 200);
            _fileListView.ItemSelectionChanged += OnFileSelected;



            _folderSelectionModeCheckBox.CheckedChanged += OnFolderSelectionModeChanged;


            // File Name TextBox




            _okButton.Click += OnOkClicked;


            _cancelButton.Click += (s, e) => Close();



        }
        private void OnSpecialFolderSelected(object sender, EventArgs e)
        {
            if (_specialFoldersComboBox.SelectedItem.Value is Environment.SpecialFolder folder)
            {
                string folderPath = Environment.GetFolderPath(folder);
                LoadFilesAndDirectories(folderPath);
            }
        }

        private void OnSearchTextChanged(object sender, EventArgs e)
        {
            string searchText = _searchBox.Text.ToLower();
            foreach (ListViewItem item in _fileListView.Items)
            {
                //  item.Visible = item.Text.ToLower().Contains(searchText);
            }
        }

        private void LoadFilesAndDirectories(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return;

            _fileListView.Items.Clear();

            // Load directories
            if (_folderSelectionModeCheckBox.Checked)
            {
                foreach (var directory in Directory.GetDirectories(path))
                {
                    var dirInfo = new DirectoryInfo(directory);
                    _fileListView.Items.Add(new ListViewItem(new[] { dirInfo.Name, "", "Folder" }) { Tag = directory });
                }
            }
            else
            {
                // Load files
                string[] filters = Filter.Split('|').Where((_, i) => i % 2 == 1).ToArray();
                foreach (var file in Directory.GetFiles(path))
                {
                    var fileInfo = new FileInfo(file);
                    if (filters.Any(f => f == "*.*" || file.EndsWith(f, StringComparison.OrdinalIgnoreCase)))
                    {
                        _fileListView.Items.Add(new ListViewItem(new[]
                        {
                            fileInfo.Name,
                            $"{fileInfo.Length / 1024} KB",
                            fileInfo.Extension
                        })
                        { Tag = file });
                    }
                }
            }
        }

        private void OnFolderSelectionModeChanged(object sender, EventArgs e)
        {
            if (_specialFoldersComboBox.SelectedItem.Value is Environment.SpecialFolder folder)
            {
                string folderPath = Environment.GetFolderPath(folder);
                LoadFilesAndDirectories(folderPath);
            }
        }

        private void OnFileSelected(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected && e.Item.Tag is string path)
            {
                _fileNameTextBox.Text = path;
                ShowPreview(path);
            }
        }

        private void ShowPreview(string path)
        {
            _previewPane.Controls.Clear();

            if (File.Exists(path) && (path.EndsWith(".txt") || path.EndsWith(".log")))
            {
                var previewTextBox = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Dock = DockStyle.Fill,
                    Text = File.ReadAllText(path)
                };
                _previewPane.Controls.Add(previewTextBox);
            }
            else if (File.Exists(path) && (path.EndsWith(".jpg") || path.EndsWith(".png")))
            {
                var previewPictureBox = new PictureBox
                {
                    Image = Image.FromFile(path),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock = DockStyle.Fill
                };
                _previewPane.Controls.Add(previewPictureBox);
            }
        }

        private void OnOkClicked(object sender, EventArgs e)
        {
            SelectedFiles.Clear();
            foreach (ListViewItem item in _fileListView.SelectedItems)
            {
                if (item.Tag is string path)
                {
                    SelectedFiles.Add(path);
                }
            }

            if (SelectedFiles.Any())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Please select at least one file or folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static List<string> Show(string filter = "All Files (*.*)|*.*")
        {
            using (var dialog = new BeepFileDialog { Filter = filter })
            {
                return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedFiles : new List<string>();
            }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
