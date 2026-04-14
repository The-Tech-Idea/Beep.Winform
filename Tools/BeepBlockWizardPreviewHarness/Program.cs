using System.Drawing;
using System.Reflection;
using System.Text;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models;

namespace BeepBlockWizardPreviewHarness;

internal static class Program
{
    private static readonly string OutputDirectory = Path.Combine(Path.GetTempPath(), "BeepBlockWizardPreviewHarness");
    private static readonly string LogPath = Path.Combine(OutputDirectory, "capture-log.txt");
    private static readonly string[] DesignToolsRuntimeDirectories =
    {
        @"C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\Windows.Forms\DesignToolsServer\Common",
        @"C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\Windows.Forms\FxDesignToolsServer\Common"
    };

    [STAThread]
    private static void Main()
    {
        PrepareOutputDirectory();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, args) => WriteException("thread-error.txt", args.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, args) => WriteException("unhandled-error.txt", args.ExceptionObject as Exception ?? new Exception(args.ExceptionObject?.ToString()));
        AppDomain.CurrentDomain.AssemblyResolve += ResolveDesignToolsAssembly;

        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Log("Starting preview harness.");

            using Form wizard = CreateWizard();
            wizard.StartPosition = FormStartPosition.Manual;
            wizard.Location = new Point(-2000, -2000);
            wizard.ShowInTaskbar = false;

            Log("Showing wizard.");
            wizard.Show();
            PumpUi();

            NavigateToPresentationStep(wizard);
            PumpUi();

            CaptureStateAsync(wizard, "record-preview", () => SetRecordMode(wizard)).GetAwaiter().GetResult();
            CaptureStateAsync(wizard, "grid-preview", () => SetGridMode(wizard)).GetAwaiter().GetResult();
            CaptureStateAsync(wizard, "designer-stacked-vertical", () => SetDesignerMode(wizard, 0)).GetAwaiter().GetResult();
            CaptureStateAsync(wizard, "designer-label-field-pairs", () => SetDesignerMode(wizard, 1)).GetAwaiter().GetResult();
            CaptureStateAsync(wizard, "designer-grid-layout", () => SetDesignerMode(wizard, 2)).GetAwaiter().GetResult();

            Log("Completed all captures.");
            wizard.Close();
        }
        catch (Exception ex)
        {
            WriteException("startup-error.txt", ex);
            throw;
        }
    }

    private static Form CreateWizard()
    {
        Log("Creating preview block.");
        BeepBlock block = CreatePreviewBlock();
        Log("Loading design server assembly.");
        Type wizardType = Assembly.Load("TheTechIdea.Beep.Winform.Controls.Design.Server")
            .GetType("TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.BeepBlockSetupWizardForm", throwOnError: true)!;

        Log("Instantiating wizard form.");
        object? wizardObject = Activator.CreateInstance(
            wizardType,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            args: new object?[] { block, null },
            culture: null);

        if (wizardObject is not Form wizard)
        {
            throw new InvalidOperationException("Unable to create BeepBlock setup wizard preview form.");
        }

        wizard.Text = "BeepBlock Setup Wizard Preview Harness";
        return wizard;
    }

    private static BeepBlock CreatePreviewBlock()
    {
        var entity = new BeepBlockEntityDefinition
        {
            ConnectionName = "PreviewConnection",
            EntityName = "Customers",
            DatasourceEntityName = "Customers",
            Caption = "Customers"
        };

        entity.Fields.Add(new BeepBlockEntityFieldDefinition
        {
            FieldName = "CustomerId",
            Label = "Customer ID",
            DataType = "int",
            Order = 0,
            IsIdentity = true,
            IsReadOnly = true
        });
        entity.Fields.Add(new BeepBlockEntityFieldDefinition
        {
            FieldName = "CompanyName",
            Label = "Company Name",
            DataType = "nvarchar",
            Order = 1,
            Size = 80
        });
        entity.Fields.Add(new BeepBlockEntityFieldDefinition
        {
            FieldName = "ContactName",
            Label = "Contact Name",
            DataType = "nvarchar",
            Order = 2,
            Size = 60
        });
        entity.Fields.Add(new BeepBlockEntityFieldDefinition
        {
            FieldName = "IsActive",
            Label = "Active",
            DataType = "bit",
            Order = 3,
            IsCheck = true
        });

        var definition = new BeepBlockDefinition
        {
            BlockName = "CustomersBlock",
            ManagerBlockName = "CustomersBlock",
            Caption = "Customers",
            PresentationMode = BeepBlockPresentationMode.DesignerGenerated,
            Entity = entity,
            Navigation = new BeepBlockNavigationDefinition(),
            Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["FieldControlsLayoutMode"] = "StackedVertical"
            }
        };

        definition.Fields = entity.CreateFieldDefinitions();

        return new BeepBlock
        {
            Name = "PreviewBeepBlock",
            BlockName = definition.BlockName,
            Definition = definition,
            Size = new Size(420, 280)
        };
    }

    private static async Task CaptureStateAsync(Form wizard, string fileName, Action applyState)
    {
        Log($"Applying state '{fileName}'.");
        applyState();
        wizard.PerformLayout();
        wizard.Refresh();
        Application.DoEvents();
        await Task.Delay(150);
        wizard.Refresh();
        Application.DoEvents();

        string outputPath = Path.Combine(OutputDirectory, fileName + ".png");
        using var bitmap = new Bitmap(wizard.ClientSize.Width, wizard.ClientSize.Height);
        wizard.DrawToBitmap(bitmap, new Rectangle(Point.Empty, wizard.ClientSize));
        bitmap.Save(outputPath);
        Log($"Saved {Path.GetFileName(outputPath)}.");
    }

    private static void NavigateToPresentationStep(Form wizard)
    {
        Log("Navigating to presentation step.");
        Type type = wizard.GetType();
        type.GetField("_currentStepIndex", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(wizard, 3);
        type.GetMethod("UpdateStepUi", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(wizard, null);
    }

    private static void SetRecordMode(Form wizard)
    {
        GetControl<RadioButton>(wizard, "_recordControlsRadio").Checked = true;
    }

    private static void SetGridMode(Form wizard)
    {
        GetControl<RadioButton>(wizard, "_gridRadio").Checked = true;
    }

    private static void SetDesignerMode(Form wizard, int layoutIndex)
    {
        GetControl<RadioButton>(wizard, "_designerGeneratedRadio").Checked = true;
        ComboBox comboBox = GetControl<ComboBox>(wizard, "_layoutModeCombo");
        comboBox.SelectedIndex = Math.Max(0, Math.Min(layoutIndex, comboBox.Items.Count - 1));
    }

    private static TControl GetControl<TControl>(Form wizard, string fieldName)
        where TControl : class
    {
        object? value = wizard.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(wizard);
        return value as TControl
            ?? throw new InvalidOperationException($"Unable to resolve field '{fieldName}' from wizard.");
    }

    private static void PrepareOutputDirectory()
    {
        if (Directory.Exists(OutputDirectory))
        {
            foreach (string file in Directory.GetFiles(OutputDirectory))
            {
                File.Delete(file);
            }
        }
        else
        {
            Directory.CreateDirectory(OutputDirectory);
        }
    }

    private static void PumpUi(int iterations = 3)
    {
        for (int index = 0; index < iterations; index++)
        {
            Application.DoEvents();
            Task.Delay(50).GetAwaiter().GetResult();
        }
    }

    private static void Log(string message)
    {
        Directory.CreateDirectory(OutputDirectory);
        string line = $"{DateTime.UtcNow:O} {message}{Environment.NewLine}";
        File.AppendAllText(LogPath, line, Encoding.UTF8);
    }

    private static void WriteException(string fileName, Exception ex)
    {
        Directory.CreateDirectory(OutputDirectory);
        File.WriteAllText(Path.Combine(OutputDirectory, fileName), ex.ToString());
        Log($"Wrote exception to {fileName}.");
    }

    private static Assembly? ResolveDesignToolsAssembly(object? sender, ResolveEventArgs args)
    {
        string? assemblyName = new AssemblyName(args.Name).Name;
        if (string.IsNullOrWhiteSpace(assemblyName))
        {
            return null;
        }

        foreach (string runtimeDirectory in DesignToolsRuntimeDirectories)
        {
            string candidatePath = Path.Combine(runtimeDirectory, assemblyName + ".dll");
            if (File.Exists(candidatePath))
            {
                Log($"Resolving {assemblyName} from {runtimeDirectory}.");
                return Assembly.LoadFrom(candidatePath);
            }
        }

        return null;
    }
}