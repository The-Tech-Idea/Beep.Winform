# Cursor IDE-Specific Instructions for Beep.Winform

This document provides Cursor IDE-specific guidance for working with the Beep.Winform WinForms control library. For general project guidelines, see [copilot-instructions.md](copilot-instructions.md).

## Table of Contents

- [Codebase Navigation](#codebase-navigation)
- [Multi-File Editing](#multi-file-editing)
- [Codebase Search Strategies](#codebase-search-strategies)
- [Workspace-Specific Features](#workspace-specific-features)
- [Control Development Workflow](#control-development-workflow)
- [Refactoring Patterns](#refactoring-patterns)
- [Quick Reference](#quick-reference)

## Codebase Navigation

### Understanding the Project Structure

The Beep.Winform project uses a modular architecture with clear separation of concerns:

```
Beep.Winform/
├── TheTechIdea.Beep.Winform.Controls/     # Main control library
│   ├── BaseControl/                      # Core base class
│   ├── BeepButton/                       # Example control
│   ├── BeepGridPro/                      # Data grid control
│   ├── ThemeManagement/                  # Theme system
│   └── Styling/                          # Styling helpers
├── TheTechIdea.Beep.Winform.Views/       # View components
├── TheTechIdea.Beep.Winform.Default.Views/ # Default views
└── .github/instructions/                 # AI assistant instructions
```

### Key Navigation Patterns

1. **BaseControl First**: When working on any control, start by examining `BaseControl` to understand the inheritance hierarchy and available methods.

2. **Partial Classes**: Many controls use partial classes organized by functionality:
   - `*.Events.cs` - Event handling
   - `*.Layout.cs` - Layout calculations
   - `*.Methods.cs` - Core methods
   - `*.Win32.cs` - Windows API integration

3. **Helper Classes**: Look for helper classes in the same directory or `Helpers/` folder:
   - `*Helper.cs` - Utility methods
   - `*Painter.cs` - Rendering logic (Strategy pattern)

4. **Documentation Files**: Each major component has a `README.md`:
   - `BaseControl/README.md` - BaseControl architecture
   - `ThemeManagement/README.md` - Theme system
   - `Styling/README.md` - Styling guidelines

### Using Cursor's File Navigation

- **Quick Open (Ctrl+P)**: Use to jump to specific controls (e.g., `BeepButton.cs`)
- **Go to Symbol (Ctrl+Shift+O)**: Navigate to methods/properties within a file
- **Go to Definition (F12)**: Jump to BaseControl or interface definitions
- **Find References (Shift+F12)**: See where a method/property is used across the codebase

## Multi-File Editing

### Common Multi-File Scenarios

#### 1. Creating a New Control

When creating a new Beep control, you'll typically need to edit multiple files:

1. **Control Class File**: `BeepControlName.cs`
   - Main control class inheriting from `BaseControl`
   - Core properties and methods

2. **Partial Class Files** (if needed):
   - `BeepControlName.Events.cs` - Event handlers
   - `BeepControlName.Layout.cs` - Layout calculations
   - `BeepControlName.Methods.cs` - Helper methods

3. **Painter Class** (if using Strategy pattern):
   - `BeepControlNamePainter.cs` - Rendering logic

4. **README.md**: `BeepControlName/README.md`
   - Control-specific documentation

5. **Update Project README**: `TheTechIdea.Beep.Winform.Controls/Readme.md`
   - Add control to component list

**Cursor Tip**: Use Cursor's multi-file editing capabilities to open all related files in tabs, then use "Edit All" to make consistent changes across files.

#### 2. Updating Theme Integration

Theme changes often require updates across multiple files:

1. **Control Class**: Update `ApplyTheme()` method
2. **Painter Class**: Update theme color usage
3. **Theme Management**: Update `BeepThemesManager` if adding new theme properties
4. **Documentation**: Update theme-related README files

**Cursor Tip**: Use Cursor's codebase search to find all `ApplyTheme()` implementations, then use multi-cursor editing to make consistent changes.

#### 3. Refactoring BaseControl Methods

When refactoring BaseControl, consider impact on all derived controls:

1. Search for all usages of the method/property
2. Check if any controls override the method
3. Update documentation if behavior changes
4. Test with multiple control types

**Cursor Tip**: Use "Find All References" (Shift+F12) to see all usages, then use Cursor's refactoring tools to update them systematically.

## Codebase Search Strategies

### Effective Search Patterns

#### 1. Finding Control Implementations

**Query**: "How does BeepButton implement DrawContent?"
- Use semantic search to find the implementation
- Look for `protected override void DrawContent(Graphics g)`

**Query**: "Where are all controls that override ApplyTheme?"
- Use grep: `override.*ApplyTheme`
- Or semantic search: "controls that override ApplyTheme method"

#### 2. Understanding Architecture Patterns

**Query**: "How does the Painter pattern work in Beep controls?"
- Search for `*Painter.cs` files
- Look for `IBeepUIComponent` interface usage
- Examine `StyledImagePainter` as reference implementation

**Query**: "How does BaseControl handle DPI scaling?"
- Search for `GetScaleFactor`, `ScaleValue`, `GetScaledFont`
- Look in `BaseControl` and helper classes

#### 3. Finding Theme Usage

**Query**: "How are theme colors applied to controls?"
- Search for `BeepThemesManager` usage
- Look for `CurrentTheme` property access
- Find `ApplyTheme()` implementations

#### 4. Locating Helper Methods

**Query**: "Where is the method to calculate Material Design layout?"
- Search for `GetMaterialContentRectangle`
- Look in `BaseControlMaterialHelper`
- Check `DrawingRect` property usage

### Search Best Practices

1. **Start Broad, Then Narrow**: Begin with semantic search to understand concepts, then use grep for specific patterns.

2. **Use File Type Filters**: When searching for C# code, use `type:cs` or `glob:*.cs` to focus results.

3. **Combine Semantic and Text Search**: 
   - Semantic: "How does hit-testing work?"
   - Text: `AddHitArea` or `HitTest`

4. **Search in Specific Directories**: When working on a specific control, limit search scope:
   - `path:TheTechIdea.Beep.Winform.Controls/BeepButton`

5. **Look for Patterns**: Use regex patterns for common code structures:
   - `override.*DrawContent` - Find DrawContent overrides
   - `protected.*void.*ApplyTheme` - Find ApplyTheme methods

## Workspace-Specific Features

### Cursor Composer

When using Cursor Composer for complex multi-file changes:

1. **Control Creation**: Composer excels at creating new controls with all required files
2. **Theme Updates**: Use Composer to update theme integration across multiple controls
3. **Refactoring**: Large refactorings benefit from Composer's multi-file awareness

**Example Workflow**:
```
1. Open Composer
2. Describe: "Create a new BeepProgressBar control that inherits from BaseControl"
3. Composer will create:
   - BeepProgressBar.cs
   - BeepProgressBar.Events.cs (if needed)
   - BeepProgressBar/README.md
   - Update main README.md
```

### Cursor Chat

For focused, single-file or small-scope changes:

1. **Quick Fixes**: "Fix the flickering issue in BeepGridPro"
2. **Code Explanations**: "Explain how DrawingRect is calculated"
3. **Code Generation**: "Generate ApplyTheme method for BeepButton"

### Workspace Settings

The workspace uses:
- **.NET Framework 4.7.2+** - Ensure Cursor recognizes C# syntax
- **WinForms** - Windows Forms specific IntelliSense
- **Partial Classes** - Cursor should understand partial class relationships

### File Watching

Cursor automatically detects changes to:
- Control class files
- README.md files (important for documentation)
- Theme configuration files

**Note**: When updating README files, Cursor will detect changes and can help maintain consistency.

## Control Development Workflow

### Step-by-Step with Cursor

#### Phase 1: Research and Planning

1. **Search for Similar Controls**:
   ```
   Query: "How does BeepButton implement BaseControl?"
   ```
   - Review existing control implementations
   - Understand common patterns

2. **Review BaseControl Documentation**:
   - Read `BaseControl/README.md`
   - Check `copilot-instructions.md` for guidelines

3. **Check Theme Integration**:
   ```
   Query: "How do controls integrate with BeepThemesManager?"
   ```

#### Phase 2: Implementation

1. **Create Control Class**:
   - Use Cursor Chat: "Create BeepControlName class inheriting from BaseControl"
   - Cursor will generate boilerplate following project patterns

2. **Implement Core Methods**:
   - `DrawContent(Graphics g)` - Override for custom rendering
   - `ApplyTheme()` - Theme integration
   - `CalculateLayout()` - Layout calculations

3. **Add Partial Classes** (if needed):
   - Use Cursor to split large files into partial classes
   - Organize by functionality (Events, Layout, Methods)

4. **Create Painter Class** (if using Strategy pattern):
   - Use Cursor: "Create BeepControlNamePainter following StyledImagePainter pattern"

#### Phase 3: Integration

1. **Update Documentation**:
   - Create/update `BeepControlName/README.md`
   - Update main `Readme.md` with new control

2. **Theme Integration**:
   - Ensure `ApplyTheme()` propagates to child components
   - Test with different themes

3. **Hit-Testing** (if interactive):
   - Use `AddHitArea()` to register clickable regions
   - Test mouse interactions

#### Phase 4: Testing and Refinement

1. **Visual Testing**: Use Cursor to help create test forms
2. **Performance**: Check for paint cycle violations
3. **Documentation**: Ensure README is complete

### Cursor Shortcuts for Control Development

- **Ctrl+Shift+P → "Generate Control"**: Use Cursor's code generation
- **F12**: Jump to BaseControl method definitions
- **Shift+F12**: Find all usages of a method
- **Ctrl+Click**: Navigate to definitions
- **Alt+F12**: Peek definition (inline view)

## Refactoring Patterns

### Safe Refactoring with Cursor

#### 1. Renaming Methods/Properties

**Before Refactoring**:
1. Use "Find All References" (Shift+F12) to see all usages
2. Check if method is overridden in derived classes
3. Verify no external dependencies

**Cursor Refactoring**:
- Right-click → "Rename Symbol" (F2)
- Cursor will update all references automatically
- Review changes in diff view before applying

#### 2. Extracting Methods

**When to Extract**:
- Long `DrawContent` methods (>50 lines)
- Repeated layout calculations
- Complex hit-testing logic

**Cursor Process**:
1. Select code block
2. Right-click → "Extract Method"
3. Cursor generates method with proper signature
4. Review and adjust as needed

#### 3. Moving to Partial Classes

**When to Split**:
- Control class exceeds 500 lines
- Clear separation of concerns (Events, Layout, Methods)

**Cursor Workflow**:
1. Create new partial class file: `ControlName.Events.cs`
2. Select event-related code
3. Cut and paste to new file
4. Cursor maintains namespace and using statements

#### 4. Updating BaseControl

**Critical Considerations**:
- BaseControl changes affect ALL controls
- Use semantic search to find all overrides
- Test with multiple control types

**Cursor Process**:
1. Make changes to BaseControl
2. Use "Find All References" to see impact
3. Use Cursor Chat to check for breaking changes
4. Update documentation

### Refactoring Safety Rules

1. **Never Modify Paint Methods During Refactoring**: Paint methods must remain read-only
2. **Preserve Virtual Methods**: Don't remove virtual keywords without checking overrides
3. **Maintain Theme Integration**: Ensure `ApplyTheme()` still works after refactoring
4. **Update Documentation**: README files must reflect changes

## Quick Reference

### Common Cursor Commands

| Action | Shortcut | Use Case |
|--------|----------|----------|
| Quick Open | Ctrl+P | Jump to file |
| Go to Symbol | Ctrl+Shift+O | Find method/property in file |
| Go to Definition | F12 | Jump to definition |
| Find References | Shift+F12 | See all usages |
| Peek Definition | Alt+F12 | Inline view of definition |
| Rename Symbol | F2 | Rename with refactoring |
| Format Document | Shift+Alt+F | Format code |
| Multi-cursor | Alt+Click | Edit multiple locations |

### Common Codebase Searches

```bash
# Find all DrawContent overrides
grep: override.*DrawContent

# Find all ApplyTheme implementations
grep: void ApplyTheme

# Find BaseControl usage
semantic: "controls that inherit from BaseControl"

# Find theme color usage
grep: CurrentTheme|Theme\?\.|BeepThemesManager

# Find DPI scaling code
grep: ScaleValue|GetScaleFactor|GetScaledFont

# Find hit-testing code
grep: AddHitArea|HitTest|ClearHitList
```

### File Organization Checklist

When creating/updating controls, ensure:

- [ ] Control class inherits from `BaseControl`
- [ ] Partial classes organized by functionality (if needed)
- [ ] Painter class follows Strategy pattern (if used)
- [ ] `README.md` exists and is up-to-date
- [ ] Main `Readme.md` includes control in component list
- [ ] Theme integration implemented in `ApplyTheme()`
- [ ] Layout uses `DrawingRect` (not `ClientRectangle`)
- [ ] Hit-testing uses `AddHitArea()` for interactive elements

### Documentation Update Triggers

Update README files when:

- Adding new public properties/methods
- Changing control behavior
- Adding theme integration
- Modifying layout logic
- Adding new dependencies
- Changing inheritance hierarchy

## Integration with Existing Tools

### Git Workflow

Cursor integrates with Git for version control:

- **View Changes**: Cursor shows file diffs inline
- **Stage Files**: Use Source Control panel
- **Commit**: Write commit messages with Cursor's help
- **Branch Management**: Create branches for feature work

**Note**: Per project instructions, don't run git commands unless explicitly requested.

### Build and Test

- **Build Errors**: Cursor highlights compilation errors
- **IntelliSense**: Real-time code analysis
- **Code Actions**: Quick fixes for common issues

**Note**: Per project instructions, don't run build/test unless explicitly requested.

### Documentation Tools

- **Markdown Preview**: View README files with Cursor's preview
- **Link Navigation**: Click links in README to navigate
- **Code Blocks**: Syntax highlighting in documentation

## Troubleshooting

### Common Issues

1. **Cursor Not Finding BaseControl**:
   - Ensure solution is loaded
   - Check that `BaseControl.cs` is in the workspace
   - Use absolute paths if needed

2. **Multi-File Edits Not Applying**:
   - Save files individually
   - Check for syntax errors preventing save
   - Use Cursor Composer for complex multi-file changes

3. **Search Not Returning Results**:
   - Try semantic search instead of text search
   - Check file filters (type:cs, glob:*.cs)
   - Verify search scope includes target directories

4. **IntelliSense Not Working**:
   - Ensure .NET Framework is recognized
   - Check that project references are correct
   - Restart Cursor if needed

## Best Practices

1. **Use Semantic Search First**: Understand concepts before diving into code
2. **Leverage Multi-File Editing**: Make consistent changes across related files
3. **Keep Documentation Updated**: Update README files as you work
4. **Follow Project Patterns**: Use existing controls as templates
5. **Test Incrementally**: Make small changes and verify before proceeding
6. **Use Cursor Composer**: For complex, multi-file changes
7. **Preserve Architecture**: Maintain BaseControl patterns and conventions

## Related Documentation

- [copilot-instructions.md](copilot-instructions.md) - General project guidelines
- [CreateUpdateBeepControl.instructions.md](CreateUpdateBeepControl.instructions.md) - Control creation workflow
- `BaseControl/README.md` - BaseControl architecture
- `SKILLS.md` - Project technical competencies

---

**Last Updated**: Based on Beep.Winform project structure  
**For**: Cursor IDE users working with Beep.Winform  
**See Also**: Main [copilot-instructions.md](copilot-instructions.md) for comprehensive project guidelines
