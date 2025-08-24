## ?? **BeepMaterialTextBox - Google Material Design TextBox for WinForms**

A beautiful, feature-rich Material Design text field control that brings Google's Material Design 3.0 guidelines to Windows Forms applications, complete with **BeepSvgPaths integration** for seamless icon support.

## ? **Key Features**

### ?? **Material Design Authentic**
- **Floating Labels**: Smooth animations following MD3 specifications
- **Field Types**: Both `Outlined` and `Filled` variants
- **Interactive States**: Focus, hover, error, and disabled states
- **Typography**: Material Design typography scale integration

### ?? **Advanced Functionality**
- **SVG Icon Support**: Leading and trailing icons using BeepSvgPaths
- **Smart Clear Button**: Auto-appears when content is present
- **Input Validation**: Built-in validation with error states
- **Helper Text**: Contextual help text with error messaging
- **Accessibility**: Full keyboard navigation and screen reader support

### ?? **Rich Visual Experience**
- **Smooth Animations**: 60fps label floating and underline animations
- **Ripple Effects**: Touch-friendly ripple feedback
- **Theme Integration**: Seamless BeepTheme system integration
- **Responsive Layout**: Auto-sizing based on content and context

## ?? **Quick Start**

### **1. Basic Text Field**
```csharp
var textField = new BeepMaterialTextBox
{
    LabelText = "Enter your name",
    FieldType = MaterialTextFieldType.Outlined,
    HelperText = "This is a helper text"
};
```

### **2. With Leading Icon**
```csharp
var searchField = new BeepMaterialTextBox
{
    LabelText = "Search",
    LeadingIconPath = BeepSvgPaths.Search,
    PlaceholderText = "Type to search...",
    FieldType = MaterialTextFieldType.Filled
};
```

### **3. Email Field with Validation**
```csharp
var emailField = new BeepMaterialTextBox
{
    LabelText = "Email Address",
    LeadingIconPath = BeepSvgPaths.Email,
    TrailingIconPath = BeepSvgPaths.CheckCircle,
    IsRequired = true,
    ShowClearButton = true
};

// Add validation
emailField.TextChanged += (s, e) =>
{
    var email = emailField.Text;
    if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
    {
        emailField.ErrorText = "Please enter a valid email address";
    }
    else
    {
        emailField.ErrorText = string.Empty;
    }
};
```

### **4. Password Field**
```csharp
var passwordField = new BeepMaterialTextBox
{
    LabelText = "Password",
    LeadingIconPath = BeepSvgPaths.Keys,
    TrailingIconPath = BeepSvgPaths.DoorClosed, // Show/hide toggle
    IsRequired = true,
    HelperText = "At least 8 characters"
};

// Toggle password visibility
passwordField.TrailingIconClicked += (s, e) =>
{
    passwordField.UseSystemPasswordChar = !passwordField.UseSystemPasswordChar;
    passwordField.TrailingIconPath = passwordField.UseSystemPasswordChar 
        ? BeepSvgPaths.DoorClosed 
        : BeepSvgPaths.DoorOpen;
};
```

## ?? **BeepSvgPaths Integration**

Our control seamlessly integrates with the BeepSvgPaths system for icon support:

### **Available Icon Categories**
```csharp
// ?? Common UI Icons
BeepSvgPaths.Search       // Search functionality
BeepSvgPaths.User         // User profiles
BeepSvgPaths.Email        // Email fields
BeepSvgPaths.Settings     // Configuration
BeepSvgPaths.Close        // Clear buttons

// ?? Form Controls
BeepSvgPaths.Calendar     // Date inputs
BeepSvgPaths.Edit         // Edit actions
BeepSvgPaths.Check        // Validation
BeepSvgPaths.Error        // Error states

// ?? Security
BeepSvgPaths.Keys         // Password fields
BeepSvgPaths.DoorOpen     // Show password
BeepSvgPaths.DoorClosed   // Hide password
```

### **Designer Support**
The control includes a smart TypeConverter that provides a dropdown in Visual Studio's property grid:

```csharp
[TypeConverter(typeof(BeepSvgPathConverter))]
public string LeadingIconPath { get; set; }
```

## ?? **Complete Example: Contact Form**

```csharp
public partial class ContactForm : Form
{
    public ContactForm()
    {
        InitializeComponent();
        CreateContactFields();
    }
    
    private void CreateContactFields()
    {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        
        // First Name
        var firstName = new BeepMaterialTextBox
        {
            Location = new Point(20, 20),
            Size = new Size(200, 72),
            LabelText = "First Name",
            LeadingIconPath = BeepSvgPaths.User,
            IsRequired = true,
            FieldType = MaterialTextFieldType.Outlined
        };
        
        // Email with validation
        var email = new BeepMaterialTextBox
        {
            Location = new Point(20, 100),
            Size = new Size(420, 72),
            LabelText = "Email Address",
            LeadingIconPath = BeepSvgPaths.Email,
            ShowClearButton = true,
            IsRequired = true,
            FieldType = MaterialTextFieldType.Filled
        };
        
        // Phone number
        var phone = new BeepMaterialTextBox
        {
            Location = new Point(20, 180),
            Size = new Size(200, 72),
            LabelText = "Phone Number",
            PlaceholderText = "(555) 123-4567",
            HelperText = "Optional"
        };
        
        // Message (multiline)
        var message = new BeepMaterialTextBox
        {
            Location = new Point(20, 260),
            Size = new Size(420, 120),
            LabelText = "Message",
            Multiline = true,
            HelperText = "Tell us how we can help you",
            FieldType = MaterialTextFieldType.Outlined
        };
        
        panel.Controls.AddRange(new Control[] { firstName, email, phone, message });
        Controls.Add(panel);
    }
}
```

## ?? **Material Design Field Types**

### **Outlined Fields**
```csharp
var outlinedField = new BeepMaterialTextBox
{
    FieldType = MaterialTextFieldType.Outlined,
    LabelText = "Outlined Field"
};
```
- Clear border definition
- Label floats above the border
- Best for forms with lots of fields

### **Filled Fields**
```csharp
var filledField = new BeepMaterialTextBox
{
    FieldType = MaterialTextFieldType.Filled,
    LabelText = "Filled Field"
};
```
- Subtle background fill
- Animated bottom border
- Great for cleaner, modern interfaces

## ?? **Theme Integration**

The control fully integrates with the Beep theme system:

```csharp
// Apply dark theme
textField.Theme = "DarkTheme";

// Theme colors automatically applied:
// - Primary color for focus states
// - Surface colors for backgrounds
// - Error colors for validation
// - Text colors for readability
```

## ?? **Advanced Configuration**

### **Event Handling**
```csharp
// Text changes
textField.TextChanged += (s, e) => Console.WriteLine($"Text: {textField.Text}");

// Icon interactions
textField.LeadingIconClicked += (s, e) => ShowSearchDialog();
textField.TrailingIconClicked += (s, e) => ProcessAction();
textField.ClearButtonClicked += (s, e) => ResetForm();
```

### **Validation Integration**
```csharp
public bool ValidateEmail()
{
    string message;
    bool isValid = emailField.ValidateData(out message);
    
    if (!isValid)
    {
        emailField.ErrorText = message;
    }
    
    return isValid;
}
```

## ?? **Responsive Design**

The control automatically adapts to different screen sizes and DPI settings:

```csharp
// Standard size for desktop
textField.Size = new Size(280, 72);

// Compact size for dense layouts
textField.Size = new Size(200, 56);

// Wide size for large screens
textField.Size = new Size(400, 72);
```

## ?? **Best Practices**

### **? Do's**
- Use consistent field types within the same form
- Provide helpful, actionable helper text
- Use appropriate icons that match the field purpose
- Group related fields visually
- Implement proper validation with clear error messages

### **? Don'ts**
- Mix outlined and filled fields randomly
- Use overly long labels or helper text
- Implement validation that's too restrictive
- Forget to handle icon click events when icons are interactive
- Use icons that don't match the content context

## ?? **Performance & Compatibility**

- **Framework**: .NET 8+ / .NET Framework 4.8+
- **Rendering**: Hardware-accelerated with double buffering
- **Memory**: Optimized with caching and disposal patterns
- **Accessibility**: NVDA, JAWS, and Windows Narrator compatible
- **DPI**: Full High-DPI and scaling support

## ?? **Technical Architecture**

The BeepMaterialTextBox is built with clean separation of concerns:

```
BeepMaterialTextBox (UserControl)
??? ?? Material Design Rendering
??? ? Animation System (Label floating, Ripples)
??? ??? Icon Management (BeepImage integration)
??? ?? Input Handling (Focus, Selection, Validation)
??? ?? Theme Integration (IBeepTheme support)
??? ?? Layout Management (Responsive sizing)
```

This architecture ensures smooth performance, easy maintenance, and excellent extensibility for future Material Design updates.

---

**BeepMaterialTextBox** brings the beauty and functionality of Google's Material Design to Windows Forms applications, providing developers with a powerful, flexible, and beautiful text input solution! ???