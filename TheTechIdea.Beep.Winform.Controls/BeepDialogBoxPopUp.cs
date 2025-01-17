using TheTechIdea.Beep.Vis.Modules;
using BeepDialogResult = TheTechIdea.Beep.Vis.Modules.BeepDialogResult;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepDialogBoxPopUp : BeepPopupForm
    {
        public Dictionary<BeepDialogButtons, BeepButton> Buttons { get; private set; } = new();
        public Dictionary<BeepDialogIcon, BeepImage> Icons { get; private set; } = new();
        public BeepDialogResult Result { get; set; }

        public event EventHandler<BeepDialogResult> ButtonClicked;
        private object _dialogData; // Declare this field here
        

        public BeepDialogBoxPopUp()
        {
            InitializeComponent();
            Result = BeepDialogResult.None;
            OnLeave += BeepPopupListForm_OnLeave;
            InitializeIcons();
        }

        private void InitializeIcons()
        {
            Icons[BeepDialogIcon.None] = null;
            Icons[BeepDialogIcon.Information] = new BeepImage { ImagePath="TheTechIdea.Beep.Winform.Controls.information.svg" }; // Replace with actual icons
            Icons[BeepDialogIcon.Warning] = new BeepImage { ImagePath = "TheTechIdea.Beep.Winform.Controls.alarm.svg" };
            Icons[BeepDialogIcon.Error] = new BeepImage { ImagePath = "TheTechIdea.Beep.Winform.Controls.alert.svg" };
            Icons[BeepDialogIcon.Question] = new BeepImage { ImagePath = "TheTechIdea.Beep.Winform.Controls.question.svg" };
        }
        private void BeepPopupListForm_OnLeave(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel; // Mark the dialog as "Cancelled"
            this.Close();
        }



        private void GenerateButtons(BeepDialogButtonSchema schema)
        {
            ButtonsPanel.Controls.Clear();
            Buttons.Clear();

            void AddButton(string text, BeepDialogButtons buttonType)
            {
                var button = new BeepButton
                {
                    Text = text,
                    Dock = DockStyle.Right,
                    Margin = new Padding(5),
                    Width = 100
                };
                button.Click += (sender, e) =>
                {
                    Result = MapDialogResult(buttonType);
                    ButtonClicked?.Invoke(this, Result);

                    DialogResult = buttonType switch
                    {
                        BeepDialogButtons.Ok => DialogResult.OK,
                        BeepDialogButtons.Cancel => DialogResult.Cancel,
                        BeepDialogButtons.Yes => DialogResult.Yes,
                        BeepDialogButtons.No => DialogResult.No,
                        BeepDialogButtons.Abort => DialogResult.Abort,
                        BeepDialogButtons.Retry => DialogResult.Retry,
                        BeepDialogButtons.Ignore => DialogResult.Ignore,
                        BeepDialogButtons.Continue => DialogResult.OK,
                        BeepDialogButtons.Stop => DialogResult.Abort,
                        _ => DialogResult.None,
                    };

                    Close();
                };

                Buttons[buttonType] = button;
                ButtonsPanel.Controls.Add(button);
            }

            switch (schema)
            {
                case BeepDialogButtonSchema.Ok:
                    AddButton("OK", BeepDialogButtons.Ok);
                    break;
                case BeepDialogButtonSchema.OkCancel:
                    AddButton("Cancel", BeepDialogButtons.Cancel);
                    AddButton("OK", BeepDialogButtons.Ok);
                    break;
                case BeepDialogButtonSchema.YesNo:
                    AddButton("No", BeepDialogButtons.No);
                    AddButton("Yes", BeepDialogButtons.Yes);
                    break;
                case BeepDialogButtonSchema.YesNoCancel:
                    AddButton("Cancel", BeepDialogButtons.Cancel);
                    AddButton("No", BeepDialogButtons.No);
                    AddButton("Yes", BeepDialogButtons.Yes);
                    break;
                case BeepDialogButtonSchema.AbortRetryIgnore:
                    AddButton("Ignore", BeepDialogButtons.Ignore);
                    AddButton("Retry", BeepDialogButtons.Retry);
                    AddButton("Abort", BeepDialogButtons.Abort);
                    break;
                case BeepDialogButtonSchema.RetryCancel:
                    AddButton("Cancel", BeepDialogButtons.Cancel);
                    AddButton("Retry", BeepDialogButtons.Retry);
                    break;
                case BeepDialogButtonSchema.ContinueStop:
                    AddButton("Stop", BeepDialogButtons.Stop);
                    AddButton("Continue", BeepDialogButtons.Continue);
                    break;
                case BeepDialogButtonSchema.Custom:
                    // Custom buttons added externally
                    break;
            }
        }
        private BeepDialogResult MapDialogResult(BeepDialogButtons buttonType)
        {
            return buttonType switch
            {
                BeepDialogButtons.Ok => BeepDialogResult.OK,
                BeepDialogButtons.Cancel => BeepDialogResult.Cancel,
                BeepDialogButtons.Yes => BeepDialogResult.Yes,
                BeepDialogButtons.No => BeepDialogResult.No,
                BeepDialogButtons.Abort => BeepDialogResult.Abort,
                BeepDialogButtons.Retry => BeepDialogResult.Retry,
                BeepDialogButtons.Ignore => BeepDialogResult.Ignore,
                BeepDialogButtons.Continue => BeepDialogResult.Continue,
                BeepDialogButtons.Stop => BeepDialogResult.Stop,
                _ => BeepDialogResult.None,
            };
        }
        public T ShowDialog<T>()
        {
            base.ShowDialog(); // Blocks execution until the form is closed
            return (T)_dialogData; // Returns the data retrieved via the _dataRetriever
        }
        public string ShowDialogInputBox(string prompt, string defaultValue = "")
        {
            var inputBox = new TextBox
            {
                Text = defaultValue,
                Dock = DockStyle.Fill,
                Margin = new Padding(10)
            };
            var label = new Label
            {
                Text = prompt,
                Dock = DockStyle.Top,
                Padding = new Padding(10)
            };

            AddControl(inputBox, label);
            GenerateButtons(BeepDialogButtonSchema.OkCancel);

            if (base.ShowDialog() == DialogResult.OK)
            {
                return inputBox.Text;
            }
            return null;
        }
        public T ShowDialogComboBox<T>(string prompt, IEnumerable<T> options, T defaultValue = default)
        {
            var comboBox = new ComboBox
            {
                DataSource = options.ToList(),
                Dock = DockStyle.Fill,
                SelectedItem = defaultValue,
                Margin = new Padding(10)
            };
            var label = new Label
            {
                Text = prompt,
                Dock = DockStyle.Top,
                Padding = new Padding(10)
            };

            AddControl(comboBox, label);
            GenerateButtons(BeepDialogButtonSchema.OkCancel);

            if (base.ShowDialog() == DialogResult.OK)
            {
                return (T)comboBox.SelectedItem;
            }
            return defaultValue;
        }

        public List<T> ShowDialogListBox<T>(string prompt, IEnumerable<T> options, SelectionMode selectionMode = SelectionMode.One)
        {
            var listBox = new ListBox
            {
                DataSource = options.ToList(),
                SelectionMode = selectionMode,
                Dock = DockStyle.Fill,
                Margin = new Padding(10)
            };
            var label = new Label
            {
                Text = prompt,
                Dock = DockStyle.Top,
                Padding = new Padding(10)
            };

            AddControl(listBox, label);
            GenerateButtons(BeepDialogButtonSchema.OkCancel);

            if (base.ShowDialog() == DialogResult.OK)
            {
                return listBox.SelectedItems.Cast<T>().ToList();
            }
            return null;
        }
        public T ShowDialog<T>(Control control, BeepDialogButtonSchema buttonSchema)
        {
            AddControl(control);
            GenerateButtons(buttonSchema);
            if (base.ShowDialog() == DialogResult.OK)
            {
                return (T)_dialogData;
            }
            return default;
        }

        private void AddControl(params Control[] controls)
        {
            ContentPanel.Controls.Clear();
            foreach (var control in controls)
            {
                ContentPanel.Controls.Add(control);
                control.Dock = DockStyle.Top;
            }
        }


    }
}
