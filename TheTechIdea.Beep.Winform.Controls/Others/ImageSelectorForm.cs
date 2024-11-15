using Svg;  // Add Svg.NET via NuGet
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;


namespace TheTechIdea.Beep.Winform.Controls.Others
{

    public class ImageSelectorForm : Form
    {
        private ListView imageListView;
        private PictureBox previewPictureBox;
        private Button selectButton;
        private ImageList imageList;
        private TextBox searchTextBox;  // Search box for filtering
        private Label searchLabel;

        public string SelectedImageName { get; private set; }

        public ImageSelectorForm()
        {
            InitializeComponents();
            LoadEmbeddedImages();
        }

        private void InitializeComponents()
        {
            // Setup ListView
            imageListView = new ListView
            {
                View = View.LargeIcon,
                Dock = DockStyle.Left,
                Width = 300,
            };
            imageListView.SelectedIndexChanged += ImageListView_SelectedIndexChanged;

            // Setup ImageList
            imageList = new ImageList
            {
                ImageSize = new Size(64, 64),
                ColorDepth = ColorDepth.Depth32Bit
            };
            imageListView.LargeImageList = imageList;

            // Setup PictureBox for image preview
            previewPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Setup Select button
            selectButton = new Button
            {
                Text = "Select Image",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            selectButton.Click += SelectButton_Click;

            // Setup search label
            searchLabel = new Label
            {
                Text = "Search:",
                Dock = DockStyle.Top,
                Height = 25
            };

            // Setup search TextBox
            searchTextBox = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 25
            };
            searchTextBox.TextChanged += SearchTextBox_TextChanged;

            // Add controls to the form
            this.Controls.Add(imageListView);
            this.Controls.Add(previewPictureBox);
            this.Controls.Add(selectButton);
            this.Controls.Add(searchTextBox);
            this.Controls.Add(searchLabel);
        }

        private void LoadEmbeddedImages()
        {
            // Get the assembly to scan for embedded resources
            var assembly = Assembly.GetExecutingAssembly();

            // Get all resource names
            var resourceNames = assembly.GetManifestResourceNames()
                .Where(res => res.EndsWith(".png") || res.EndsWith(".jpg") || res.EndsWith(".bmp") || res.EndsWith(".svg"))
                .ToList();

            foreach (var resourceName in resourceNames)
            {
                // Load each image as a Bitmap or render SVG
                Image image = null;
                if (resourceName.EndsWith(".svg"))
                {
                    image = LoadSvgImage(assembly, resourceName);
                }
                else
                {
                    image = LoadBitmapImage(assembly, resourceName);
                }

                if (image != null)
                {
                    imageList.Images.Add(resourceName, image);
                    var item = new ListViewItem(Path.GetFileName(resourceName))
                    {
                        ImageKey = resourceName
                    };
                    imageListView.Items.Add(item);
                }
            }
        }

        private Image LoadBitmapImage(Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                return stream != null ? new Bitmap(stream) : null;
            }
        }

        private Image LoadSvgImage(Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    var svgDocument = SvgDocument.Open<SvgDocument>(stream);
                    return svgDocument.Draw(64, 64);  // Draw SVG to fit within 64x64
                }
            }
            return null;
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();

            // Filter items in the ListView based on the search text
            foreach (ListViewItem item in imageListView.Items)
            {
             //   item.Visible = item.Text.ToLower().Contains(searchText);
            }
        }

        private void ImageListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (imageListView.SelectedItems.Count > 0)
            {
                var selectedItem = imageListView.SelectedItems[0];
                var selectedImageKey = selectedItem.ImageKey;

                // Preview the selected image in the PictureBox
                previewPictureBox.Image = imageList.Images[selectedImageKey];
            }
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            if (imageListView.SelectedItems.Count > 0)
            {
                SelectedImageName = imageListView.SelectedItems[0].Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select an image.", "No Image Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}