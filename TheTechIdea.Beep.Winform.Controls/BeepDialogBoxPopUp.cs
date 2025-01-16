using TheTechIdea.Beep.Vis.Modules;
using BeepDialogResult = TheTechIdea.Beep.Vis.Modules.BeepDialogResult;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepDialogBoxPopUp : BeepiForm
    {
        public Dictionary<BeepDialogButtons, BeepButton> Buttons { get; private set; } = new();
        public Dictionary<BeepDialogIcon, BeepImage> Icons { get; private set; } = new();
        public BeepDialogResult Result { get; set; }

        public event EventHandler<BeepDialogResult> ButtonClicked;

        private Func<object> _dataRetriever;

        public BeepDialogBoxPopUp()
        {
            InitializeComponent();
            Result = BeepDialogResult.None;
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

        public void SetDialog<T>(BeepDialogIcon icon, string title, BeepDialogButtonSchema schema, Func<T> dataRetriever = null)
        {
            // Set icon
            if (Icons.TryGetValue(icon, out BeepImage dialogIcon) && dialogIcon != null)
            {
                DialogIconImage.Image = dialogIcon.Image;
            }
            else
            {
                DialogIconImage.Image = null; // No icon
            }

            // Set title
            TitleLabel.Text = title;

            // Generate buttons based on schema
            GenerateButtons(schema);

            // Set the data retriever
            _dataRetriever = dataRetriever == null ? null : new Func<object>(() => dataRetriever());
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
                    if (Result == BeepDialogResult.OK && _dataRetriever != null)
                    {
                        var data = _dataRetriever.Invoke();
                        MessageBox.Show($"Data Returned: {data}"); // Replace with actual handling
                    }
                    Close();
                };

                Buttons[buttonType] = button;
                ButtonsPanel.Controls.Add(button);
            }

            // Add buttons based on the schema
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
                    // Leave for custom handling
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

        public void AddControl(Control control)
        {
            ContentPanel.Controls.Clear();
            ContentPanel.Controls.Add(control);
            control.Dock = DockStyle.Fill;
        }

        public void SetDataRetriever<T>(Func<T> dataRetriever)
        {
            _dataRetriever = new Func<object>(() => dataRetriever());
        }
    }
}
