using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Default.Views
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.beepButton1.Click += BeepButton1_Click;
            BeepThemesManager.ThemeChanged += BeepThemesManager_ThemeChanged;
        }

        private void BeepThemesManager_ThemeChanged(object? sender, ThemeChangeEventArgs e)
        {
           beepButton1.Theme = e.NewTheme.ThemeName;
            beepAppBar1.Theme = e.NewTheme.ThemeName;
            beepCheckBoxBool1.Theme = e.NewTheme.ThemeName;
            beepMenuBar1.Theme = e.NewTheme.ThemeName;
            beepDatePicker1.Theme = e.NewTheme.ThemeName;
            beepTextBox1.Theme = e.NewTheme.ThemeName;
            beepNumericUpDown1.Theme = e.NewTheme.ThemeName;
            beepProgressBar1.Theme = e.NewTheme.ThemeName;
            beepStarRating1.Theme = e.NewTheme.ThemeName;

        }

        int i = 0;
        private void BeepButton1_Click(object? sender, EventArgs e)
        {
            i++;
            if (i >= BeepThemesManager._themes.Count)
                i = 0;
            // Set the current theme to the next one in the list
            BeepThemesManager.SetCurrentTheme(BeepThemesManager._themes[i]);
        }
    }
}
