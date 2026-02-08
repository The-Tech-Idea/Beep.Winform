# BeepLogin Skill

## Overview
Login form components for authentication UI.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.Logins;
```

## Available Components
- Login forms
- Password fields
- Remember me checkbox
- Social login buttons

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Username` | `string` | Username value |
| `Password` | `string` | Password value |
| `RememberMe` | `bool` | Remember state |
| `ShowSocialLogins` | `bool` | Social login buttons |

## Events
| Event | Description |
|-------|-------------|
| `LoginClicked` | Login button clicked |
| `ForgotPasswordClicked` | Forgot password clicked |
| `RegisterClicked` | Register button clicked |

## Usage
```csharp
var loginForm = new BeepLoginForm
{
    ShowSocialLogins = true
};
loginForm.LoginClicked += async (s, e) =>
{
    var result = await AuthService.LoginAsync(
        loginForm.Username,
        loginForm.Password,
        loginForm.RememberMe
    );
};
```
