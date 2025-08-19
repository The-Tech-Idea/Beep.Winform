using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Simple test form to verify the BeepSimpleTextBox fixes
    /// </summary>
    public partial class TestBeepSimpleTextBoxFixes : Form
    {
        private BeepSimpleTextBox textBox1;
        private BeepSimpleTextBox textBox2;
        private BeepSimpleTextBox textBox3;
        private Button testButton;
        
        public TestBeepSimpleTextBoxFixes()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form setup
            this.Text = "BeepSimpleTextBox: Image & Line Number Fixes Test";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Test 1: Basic textbox WITHOUT line numbers
            textBox1 = new BeepSimpleTextBox();
            textBox1.Location = new Point(20, 20);
            textBox1.Size = new Size(300, 30);
            textBox1.PlaceholderText = "No line numbers - full width available";
            textBox1.PlaceholderTextColor = Color.Gray;
            textBox1.ShowLineNumbers = false; // Explicitly false
            textBox1.Multiline = false;
            
            // Test 2: Multiline textbox WITH line numbers
            textBox2 = new BeepSimpleTextBox();
            textBox2.Location = new Point(20, 70);
            textBox2.Size = new Size(400, 150);
            textBox2.Multiline = true;
            textBox2.ShowLineNumbers = true; // Should show line numbers
            textBox2.PlaceholderText = "Multiline with line numbers";
            textBox2.Text = "Line 1: This should have line numbers\nLine 2: Space should be reserved for line numbers\nLine 3: Text should be properly indented";
            
            // Test 3: Textbox WITHOUT image initially
            textBox3 = new BeepSimpleTextBox();
            textBox3.Location = new Point(20, 240);
            textBox3.Size = new Size(300, 30);
            textBox3.PlaceholderText = "No image - full width available";
            textBox3.ImageVisible = true; // Ready for image but no path set yet
            
            // Test button for dynamic testing
            testButton = new Button();
            testButton.Location = new Point(20, 290);
            testButton.Size = new Size(200, 30);
            testButton.Text = "Toggle Image & Line Numbers";
            testButton.Click += TestButton_Click;
            
            // Add labels
            Label label1 = new Label();
            label1.Text = "Single Line WITHOUT Line Numbers (should use full width):";
            label1.Location = new Point(20, 0);
            label1.Size = new Size(400, 20);
            
            Label label2 = new Label();
            label2.Text = "Multiline WITH Line Numbers (should reserve space):";
            label2.Location = new Point(20, 50);
            label2.Size = new Size(400, 20);
            
            Label label3 = new Label();
            label3.Text = "Textbox WITHOUT Image (should use full width):";
            label3.Location = new Point(20, 220);
            label3.Size = new Size(400, 20);
            
            // Add info label
            Label infoLabel = new Label();
            infoLabel.Text = "Image & Line Number Test Instructions:\n" +
                           "1. Check that textbox 1 uses FULL WIDTH (no line number space)\n" +
                           "2. Check that textbox 2 has LINE NUMBERS and reserved space\n" +
                           "3. Check that textbox 3 uses FULL WIDTH (no image space)\n" +
                           "4. Click 'Toggle' to test dynamic image/line number changes\n" +
                           "5. Verify layout updates properly when features are toggled";
            infoLabel.Location = new Point(20, 330);
            infoLabel.Size = new Size(750, 100);
            infoLabel.BackColor = Color.LightGreen;
            infoLabel.BorderStyle = BorderStyle.FixedSingle;
            
            // Add controls to form
            this.Controls.AddRange(new Control[] 
            {
                label1, textBox1,
                label2, textBox2,
                label3, textBox3,
                testButton,
                infoLabel
            });
            
            this.ResumeLayout(false);
        }
        
        private void TestButton_Click(object sender, EventArgs e)
        {
            // Test 1: Toggle line numbers for textbox1 (should change layout)
            textBox1.ShowLineNumbers = !textBox1.ShowLineNumbers;
            textBox1.Multiline = textBox1.ShowLineNumbers; // Need multiline for line numbers
            
            // Test 2: Toggle image for textbox3 (should change layout)
            if (string.IsNullOrEmpty(textBox3.ImagePath))
            {
                // Add a simple image (you can use any valid image path or embedded resource)
                textBox3.ImagePath = "search"; // This would be a resource or file path
                textBox3.Text = "Now has image - space reserved";
            }
            else
            {
                textBox3.ImagePath = "";
                textBox3.Text = "Image removed - full width restored";
            }
            
            // Test 3: Show status
            string status = $"TextBox1 LineNumbers: {textBox1.ShowLineNumbers}, TextBox3 HasImage: {!string.IsNullOrEmpty(textBox3.ImagePath)}";
            MessageBox.Show(status, "Toggle Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}