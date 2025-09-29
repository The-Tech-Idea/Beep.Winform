# Plan: Implementing BeepAppBar Hit Area Methodology in Painters

## Overview

This plan outlines how to implement the same interactive hit area methodology used in `BeepAppBar` into all Widget Painters, starting with the Finance category painters. The goal is to make painters interactive by adding hit areas for different UI components, similar to how BeepAppBar handles logo, title, search box, and button interactions.

## Analysis of BeepAppBar Methodology

### Key Components in BeepAppBar:

1. **Hit Area Management**: Uses `AddHitArea(string name, Rectangle rect, IBeepUIComponent component, Action hitAction)`
2. **Mouse Event Handling**: Tracks mouse events (click, hover, move) and maps them to hit areas
3. **Visual Feedback**: Updates component appearance based on hover states
4. **Action Execution**: Executes specific actions when hit areas are clicked
5. **Rectangle Calculation**: Dynamically calculates rectangles for each interactive component in `CalculateLayout`

### BeepAppBar Hit Area Pattern:

```csharp
// 1. Define rectangles for interactive areas
Rectangle logoRect, titleRect, searchRect, notificationRect, etc.

// 2. Calculate layout positions
private void CalculateLayout(out Rectangle logoRect, out Rectangle titleRect, ...)

// 3. Handle mouse events
protected override void OnMouseClick(MouseEventArgs e)
{
    if (logoRect.Contains(e.Location)) HandleLogoClick();
    if (titleRect.Contains(e.Location)) HandleTitleClick();
    // etc.
}

// 4. Track hover states
protected override void OnMouseMove(MouseEventArgs e)
{
    _hoveredComponentName = GetHoveredComponent(e.Location);
    Invalidate(); // Redraw with hover effects
}

// 5. Apply hover effects in drawing
if (_hoveredComponentName == "Logo") 
{
    _logo.IsHovered = true;
}
```

## Implementation Plan for Painters

### Phase 1: Enhance WidgetPainterBase

#### 1.1 Add Hit Area Support to WidgetPainterBase

```csharp
internal abstract class WidgetPainterBase : IWidgetPainter
{
    // NEW: Hit area management
    private Dictionary<string, Rectangle> _hitAreas = new Dictionary<string, Rectangle>();
    private Dictionary<string, Action> _hitActions = new Dictionary<string, Action>();
    private string _hoveredArea = null;
    
    // NEW: Events for painter interactions
    public event EventHandler<PainterHitEventArgs> HitAreaClicked;
    public event EventHandler<PainterHoverEventArgs> HitAreaHovered;
    
    // ENHANCED: UpdateHitAreas method
    public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle> notifyAreaHit)
    {
        // Register all hit areas with the owner control
        foreach (var hitArea in _hitAreas)
        {
            owner.AddHitArea(hitArea.Key, hitArea.Value, null, _hitActions.GetValueOrDefault(hitArea.Key));
            notifyAreaHit?.Invoke(hitArea.Key, hitArea.Value);
        }
    }
    
    // NEW: Helper methods for painters
    protected void AddInteractiveArea(string name, Rectangle rect, Action clickAction = null)
    {
        _hitAreas[name] = rect;
        if (clickAction != null)
            _hitActions[name] = clickAction;
    }
    
    protected void ClearInteractiveAreas()
    {
        _hitAreas.Clear();
        _hitActions.Clear();
    }
    
    protected bool IsAreaHovered(string areaName) => _hoveredArea == areaName;
    
    // NEW: Handle mouse events from owner control
    public virtual void HandleMouseMove(Point location)
    {
        string previousHover = _hoveredArea;
        _hoveredArea = null;
        
        foreach (var area in _hitAreas)
        {
            if (area.Value.Contains(location))
            {
                _hoveredArea = area.Key;
                break;
            }
        }
        
        if (previousHover != _hoveredArea)
        {
            HitAreaHovered?.Invoke(this, new PainterHoverEventArgs(_hoveredArea, previousHover));
        }
    }
    
    public virtual void HandleClick(Point location)
    {
        foreach (var area in _hitAreas)
        {
            if (area.Value.Contains(location))
            {
                _hitActions.GetValueOrDefault(area.Key)?.Invoke();
                HitAreaClicked?.Invoke(this, new PainterHitEventArgs(area.Key, location));
                break;
            }
        }
    }
}

// NEW: Event argument classes
public class PainterHitEventArgs : EventArgs
{
    public string AreaName { get; }
    public Point Location { get; }
    public PainterHitEventArgs(string areaName, Point location)
    {
        AreaName = areaName;
        Location = location;
    }
}

public class PainterHoverEventArgs : EventArgs
{
    public string CurrentArea { get; }
    public string PreviousArea { get; }
    public PainterHoverEventArgs(string currentArea, string previousArea)
    {
        CurrentArea = currentArea;
        PreviousArea = previousArea;
    }
}
```

### Phase 2: Implement Interactive Finance Painters

#### 2.1 BalanceCardPainter - Enhanced with Hit Areas

**Interactive Elements:**
- **Logo/Icon Area**: Click to show account details
- **Balance Area**: Click to toggle between different views (current/available/etc.)
- **Account Number Area**: Click to copy to clipboard
- **Security Chip**: Click to show security options

```csharp
public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
{
    // Existing layout code...
    
    // NEW: Define interactive areas
    ClearInteractiveAreas();
    
    // Logo/Icon interactive area
    var logoArea = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y, 20, 20);
    AddInteractiveArea("Logo", logoArea, () => HandleLogoClick(ctx));
    
    // Balance interactive area
    AddInteractiveArea("Balance", ctx.ContentRect, () => HandleBalanceClick(ctx));
    
    // Account number area (in FooterRect)
    var accountArea = new Rectangle(ctx.FooterRect.X, ctx.FooterRect.Y, ctx.FooterRect.Width / 2, ctx.FooterRect.Height);
    AddInteractiveArea("AccountNumber", accountArea, () => HandleAccountClick(ctx));
    
    // Security chip area
    var chipArea = new Rectangle(ctx.DrawingRect.Right - 40, ctx.DrawingRect.Y + 12, 24, 16);
    AddInteractiveArea("SecurityChip", chipArea, () => HandleSecurityClick(ctx));
    
    return ctx;
}

public override void DrawContent(Graphics g, WidgetContext ctx)
{
    // Existing drawing code...
    
    // NEW: Apply hover effects
    DrawBalanceHeader(g, ctx, IsAreaHovered("Logo"));
    DrawBalanceAmount(g, ctx, balance, currencySymbol, IsAreaHovered("Balance"));
    DrawAccountInfo(g, ctx, accountNumber, IsAreaHovered("AccountNumber"));
}

public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
{
    // Existing code...
    
    // NEW: Enhanced security chip with hover effect
    var chipRect = new Rectangle(ctx.DrawingRect.Right - 40, ctx.DrawingRect.Y + 12, 24, 16);
    Color chipColor = IsAreaHovered("SecurityChip") 
        ? Color.FromArgb(60, Color.White)  // Lighter when hovered
        : Color.FromArgb(40, Color.White);
    
    using var chipBrush = new SolidBrush(chipColor);
    g.FillRoundedRectangle(chipBrush, chipRect, 3);
    
    var chipIconRect = new Rectangle(chipRect.X + 4, chipRect.Y + 2, 16, 12);
    Color iconColor = IsAreaHovered("SecurityChip") 
        ? Color.FromArgb(200, Color.White)
        : Color.FromArgb(150, Color.White);
    _imagePainter.DrawSvg(g, "cpu", chipIconRect, iconColor, 0.7f);
}

// NEW: Action handlers
private void HandleLogoClick(WidgetContext ctx)
{
    // Show account details popup or navigate to account page
    // This would trigger an event that the hosting control can handle
}

private void HandleBalanceClick(WidgetContext ctx)
{
    // Toggle between different balance views (available, pending, total)
    // Or show balance history
}

private void HandleAccountClick(WidgetContext ctx)
{
    // Copy account number to clipboard
    // Or show full account details
}

private void HandleSecurityClick(WidgetContext ctx)
{
    // Show security options menu
    // Or navigate to security settings
}
```

#### 2.2 PaymentCardPainter - Enhanced with Hit Areas

**Interactive Elements:**
- **Card Details Area**: Click to show/hide full card number
- **Balance Area**: Click to refresh or show transactions
- **Expiry Date Area**: Click to show renewal options

```csharp
public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
{
    // Existing layout code...
    
    ClearInteractiveAreas();
    
    // Card details toggle area (eye icon + card number)
    var cardDetailsArea = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y, ctx.ContentRect.Width, 20);
    AddInteractiveArea("CardDetails", cardDetailsArea, () => HandleCardDetailsClick(ctx));
    
    // Balance area
    var balanceArea = new Rectangle(ctx.ContentRect.X, cardDetailsArea.Bottom + 8, ctx.ContentRect.Width, 24);
    AddInteractiveArea("Balance", balanceArea, () => HandleBalanceClick(ctx));
    
    // Expiry area
    var expiryArea = new Rectangle(ctx.ContentRect.X, balanceArea.Bottom + 4, ctx.ContentRect.Width, 16);
    AddInteractiveArea("Expiry", expiryArea, () => HandleExpiryClick(ctx));
    
    return ctx;
}

public override void DrawContent(Graphics g, WidgetContext ctx)
{
    // Enhanced drawing with hover effects
    DrawPaymentHeader(g, ctx, paymentMethod);
    DrawPaymentDetails(g, ctx, cardNumber, expiryDate, balance, currencySymbol);
}

private void DrawPaymentDetails(Graphics g, WidgetContext ctx, string cardNumber, string expiryDate, decimal balance, string currencySymbol)
{
    // Card number with hover effect
    var cardRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y, ctx.ContentRect.Width, 20);
    
    // Eye icon changes based on hover state
    var cardIconRect = new Rectangle(cardRect.X, cardRect.Y + 2, 16, 16);
    string eyeIcon = IsAreaHovered("CardDetails") ? "eye" : "eye-off";
    Color eyeColor = IsAreaHovered("CardDetails") 
        ? Color.FromArgb(180, Theme?.PrimaryColor ?? Color.Blue)
        : Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray);
    _imagePainter.DrawSvg(g, eyeIcon, cardIconRect, eyeColor, 0.7f);
    
    // Card number text with hover effect
    var cardTextRect = new Rectangle(cardIconRect.Right + 6, cardRect.Y, 
        cardRect.Width - cardIconRect.Width - 6, cardRect.Height);
    
    // Show full or masked number based on hover/click state
    bool showFullNumber = ctx.CustomData.ContainsKey("ShowFullNumber") && (bool)ctx.CustomData["ShowFullNumber"];
    string displayNumber = (showFullNumber || IsAreaHovered("CardDetails")) ? "4532 1234 5678 9012" : cardNumber;
    
    using var cardFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
    Color textColor = IsAreaHovered("CardDetails") 
        ? Theme?.PrimaryColor ?? Color.Blue
        : Theme?.ForeColor ?? Color.Black;
    using var cardBrush = new SolidBrush(textColor);
    g.DrawString(displayNumber, cardFont, cardBrush, cardTextRect);
    
    // Balance with hover effect
    var balanceRect = new Rectangle(ctx.ContentRect.X, cardRect.Bottom + 8, ctx.ContentRect.Width, 24);
    using var balanceFont = new Font(Owner.Font.FontFamily, 16f, FontStyle.Bold);
    
    Color balanceColor = IsAreaHovered("Balance")
        ? Color.FromArgb(Math.Min(255, ((Theme?.PrimaryColor ?? Color.Blue).R + 30)), 
                         Math.Min(255, ((Theme?.PrimaryColor ?? Color.Blue).G + 30)),
                         Math.Min(255, ((Theme?.PrimaryColor ?? Color.Blue).B + 30)))
        : Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
    
    using var balanceBrush = new SolidBrush(balanceColor);
    string balanceText = $"{currencySymbol}{balance:N2}";
    g.DrawString(balanceText, balanceFont, balanceBrush, balanceRect);
    
    // Expiry with hover effect
    if (!string.IsNullOrEmpty(expiryDate))
    {
        var expiryRect = new Rectangle(ctx.ContentRect.X, balanceRect.Bottom + 4, ctx.ContentRect.Width, 16);
        using var expiryFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
        
        Color expiryColor = IsAreaHovered("Expiry")
            ? Theme?.PrimaryColor ?? Color.Blue
            : Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray);
        
        using var expiryBrush = new SolidBrush(expiryColor);
        string expiryText = IsAreaHovered("Expiry") ? $"Expires: {expiryDate} - Click to renew" : $"Expires: {expiryDate}";
        g.DrawString(expiryText, expiryFont, expiryBrush, expiryRect);
    }
}

// Action handlers
private void HandleCardDetailsClick(WidgetContext ctx)
{
    // Toggle show/hide full card number
    bool currentState = ctx.CustomData.ContainsKey("ShowFullNumber") && (bool)ctx.CustomData["ShowFullNumber"];
    ctx.CustomData["ShowFullNumber"] = !currentState;
}

private void HandleBalanceClick(WidgetContext ctx)
{
    // Refresh balance or show recent transactions
}

private void HandleExpiryClick(WidgetContext ctx)
{
    // Show card renewal options or expiry notifications
}
```

#### 2.3 InvestmentCardPainter - Enhanced with Hit Areas

**Interactive Elements:**
- **Investment Title**: Click to show detailed portfolio
- **Current Value**: Click to show value history chart
- **Gain/Loss Indicator**: Click to show performance analytics
- **Portfolio Indicator**: Click to show asset allocation

```csharp
public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
{
    // Existing layout...
    
    ClearInteractiveAreas();
    
    // Title area
    if (!ctx.HeaderRect.IsEmpty)
    {
        AddInteractiveArea("Title", ctx.HeaderRect, () => HandleTitleClick(ctx));
    }
    
    // Current value area
    var valueArea = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y, ctx.ContentRect.Width, 30);
    AddInteractiveArea("CurrentValue", valueArea, () => HandleValueClick(ctx));
    
    // Gain/Loss area
    var gainLossArea = new Rectangle(ctx.ContentRect.X, valueArea.Bottom + 8, ctx.ContentRect.Width, 24);
    AddInteractiveArea("GainLoss", gainLossArea, () => HandleGainLossClick(ctx));
    
    // Portfolio indicator area (if visible)
    if (ctx.CustomData.ContainsKey("IsPortfolio") && (bool)ctx.CustomData["IsPortfolio"])
    {
        var portfolioArea = new Rectangle(ctx.DrawingRect.Right - 28, ctx.DrawingRect.Y + 8, 20, 20);
        AddInteractiveArea("Portfolio", portfolioArea, () => HandlePortfolioClick(ctx));
    }
    
    return ctx;
}

public override void DrawContent(Graphics g, WidgetContext ctx)
{
    // Enhanced drawing with interactive feedback
    DrawInvestmentHeader(g, ctx);
    DrawInvestmentValue(g, ctx, currentValue, gainLoss, gainLossPercent, currencySymbol);
}

private void DrawInvestmentHeader(Graphics g, WidgetContext ctx)
{
    if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
    {
        // Icon with hover effect
        var iconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 2, 20, 20);
        Color iconColor = IsAreaHovered("Title") 
            ? Color.FromArgb(Math.Min(255, ((Theme?.PrimaryColor ?? Color.Green).R + 40)),
                             Math.Min(255, ((Theme?.PrimaryColor ?? Color.Green).G + 40)),
                             Math.Min(255, ((Theme?.PrimaryColor ?? Color.Green).B + 40)))
            : Theme?.PrimaryColor ?? Color.FromArgb(76, 175, 80);
        
        _imagePainter.DrawSvg(g, "trending-up", iconRect, iconColor, 0.9f);

        // Title with hover effect
        var titleRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, 
            ctx.HeaderRect.Width - iconRect.Width - 8, ctx.HeaderRect.Height);
        
        using var titleFont = new Font(Owner.Font.FontFamily, 12f, 
            IsAreaHovered("Title") ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
        
        Color titleColor = IsAreaHovered("Title") 
            ? Theme?.PrimaryColor ?? Color.Blue
            : Theme?.ForeColor ?? Color.Black;
        
        using var titleBrush = new SolidBrush(titleColor);
        string titleText = IsAreaHovered("Title") ? $"{ctx.Title} - Click for details" : ctx.Title;
        g.DrawString(titleText, titleFont, titleBrush, titleRect);
    }
}

private void DrawInvestmentValue(Graphics g, WidgetContext ctx, decimal currentValue, decimal gainLoss, decimal gainLossPercent, string currencySymbol)
{
    // Current value with hover effect
    var valueRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y, ctx.ContentRect.Width, 30);
    
    using var valueFont = new Font(Owner.Font.FontFamily, 18f, 
        IsAreaHovered("CurrentValue") ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
    
    Color valueColor = IsAreaHovered("CurrentValue")
        ? Theme?.PrimaryColor ?? Color.Blue
        : Theme?.ForeColor ?? Color.Black;
    
    using var valueBrush = new SolidBrush(valueColor);
    string valueText = IsAreaHovered("CurrentValue") 
        ? $"{currencySymbol}{currentValue:N2} - Click for history"
        : $"{currencySymbol}{currentValue:N2}";
    
    g.DrawString(valueText, valueFont, valueBrush, valueRect);

    // Gain/Loss with enhanced hover effects
    var gainLossRect = new Rectangle(ctx.ContentRect.X, valueRect.Bottom + 8, ctx.ContentRect.Width, 24);
    bool isGain = gainLoss >= 0;
    
    Color gainLossColor = isGain ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
    if (IsAreaHovered("GainLoss"))
    {
        // Brighten color on hover
        gainLossColor = Color.FromArgb(Math.Min(255, gainLossColor.R + 30),
                                     Math.Min(255, gainLossColor.G + 30),
                                     Math.Min(255, gainLossColor.B + 30));
    }
    
    // Icon with animation effect when hovered
    var iconRect = new Rectangle(gainLossRect.X + (gainLossRect.Width - 120) / 2, gainLossRect.Y + 4, 16, 16);
    if (IsAreaHovered("GainLoss"))
    {
        // Slightly larger icon when hovered
        iconRect.Inflate(2, 2);
    }
    
    _imagePainter.DrawSvg(g, isGain ? "trending-up" : "trending-down", iconRect, gainLossColor, 0.9f);
    
    // Gain/Loss text with hover enhancement
    var gainLossTextRect = new Rectangle(iconRect.Right + 4, gainLossRect.Y, 100, gainLossRect.Height);
    using var gainLossFont = new Font(Owner.Font.FontFamily, 10f, 
        IsAreaHovered("GainLoss") ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold);
    
    using var gainLossBrush = new SolidBrush(gainLossColor);
    string gainLossText = IsAreaHovered("GainLoss")
        ? $"{(isGain ? "+" : "")}{currencySymbol}{Math.Abs(gainLoss):N2} ({gainLossPercent:+0.00;-0.00}%) - Click for analytics"
        : $"{(isGain ? "+" : "")}{currencySymbol}{Math.Abs(gainLoss):N2} ({gainLossPercent:+0.00;-0.00}%)";
    
    g.DrawString(gainLossText, gainLossFont, gainLossBrush, gainLossTextRect);
}

public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
{
    // Enhanced portfolio indicator with hover effects
    if (ctx.CustomData.ContainsKey("IsPortfolio") && (bool)ctx.CustomData["IsPortfolio"])
    {
        var portfolioRect = new Rectangle(ctx.DrawingRect.Right - 28, ctx.DrawingRect.Y + 8, 20, 20);
        
        Color bgColor = IsAreaHovered("Portfolio")
            ? Color.FromArgb(50, Theme?.AccentColor ?? Color.Blue)  // More opaque when hovered
            : Color.FromArgb(30, Theme?.AccentColor ?? Color.Blue);
        
        using var portfolioBrush = new SolidBrush(bgColor);
        using var path = CreateRoundedPath(portfolioRect, 4);
        g.FillPath(portfolioBrush, path);
        
        // Icon with hover effect
        var iconRect = new Rectangle(portfolioRect.X + 2, portfolioRect.Y + 2, 16, 16);
        Color iconColor = IsAreaHovered("Portfolio")
            ? Color.FromArgb(220, Theme?.AccentColor ?? Color.Blue)
            : Color.FromArgb(150, Theme?.AccentColor ?? Color.Blue);
        
        _imagePainter.DrawSvg(g, "pie-chart", iconRect, iconColor, 0.8f);
        
        // Tooltip-style text when hovered
        if (IsAreaHovered("Portfolio"))
        {
            var tooltipRect = new Rectangle(portfolioRect.X - 80, portfolioRect.Bottom + 2, 100, 16);
            using var tooltipBrush = new SolidBrush(Color.FromArgb(200, 50, 50, 50));
            using var tooltipPath = CreateRoundedPath(tooltipRect, 4);
            g.FillPath(tooltipBrush, tooltipPath);
            
            using var tooltipFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Regular);
            using var tooltipTextBrush = new SolidBrush(Color.White);
            g.DrawString("View Portfolio", tooltipFont, tooltipTextBrush, tooltipRect);
        }
    }
}

// Action handlers
private void HandleTitleClick(WidgetContext ctx)
{
    // Show detailed investment portfolio or investment details page
}

private void HandleValueClick(WidgetContext ctx)
{
    // Show investment value history chart or performance timeline
}

private void HandleGainLossClick(WidgetContext ctx)
{
    // Show detailed performance analytics or comparison charts
}

private void HandlePortfolioClick(WidgetContext ctx)
{
    // Show asset allocation breakdown or portfolio diversification view
}
```

### Phase 3: Implementation Steps

#### 3.1 Priority Order for Finance Painters:

1. **BalanceCardPainter** - Start here (simplest interactions)
2. **PaymentCardPainter** - Medium complexity 
3. **InvestmentCardPainter** - More complex interactions
4. **TransactionCardPainter** - Transaction-specific interactions
5. **ExpenseCardPainter** - Expense tracking interactions
6. **CryptoWidgetPainter** - Crypto-specific interactions  
7. **BudgetWidgetPainter** - Budget management interactions
8. **PortfolioCardPainter** - Portfolio management interactions

#### 3.2 Implementation Checklist per Painter:

- [ ] Identify interactive UI elements
- [ ] Define hit area rectangles in `AdjustLayout`
- [ ] Add `AddInteractiveArea` calls for each element
- [ ] Implement hover effects in drawing methods
- [ ] Create action handler methods
- [ ] Add visual feedback (hover colors, cursor changes, tooltips)
- [ ] Test mouse interactions
- [ ] Add events for parent controls to handle

### Phase 4: Integration with Widget Controls

#### 4.1 Widget Control Changes:

The widget controls that use these painters need to:

```csharp
public class BeepFinanceWidget : BaseControl
{
    private WidgetPainterBase _currentPainter;
    
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        _currentPainter?.HandleMouseMove(e.Location);
        
        // Update cursor based on interactive areas
        bool overInteractiveArea = _currentPainter?.IsOverInteractiveArea(e.Location) ?? false;
        Cursor = overInteractiveArea ? Cursors.Hand : Cursors.Default;
    }
    
    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        _currentPainter?.HandleClick(e.Location);
    }
    
    // Event handlers for painter interactions
    private void OnPainterHitAreaClicked(object sender, PainterHitEventArgs e)
    {
        // Handle painter-specific interactions
        switch (e.AreaName)
        {
            case "Balance":
                // Show balance details
                break;
            case "CardDetails":
                // Toggle card number visibility
                break;
            // etc.
        }
    }
}
```

## Expected Benefits

1. **Enhanced User Experience**: Interactive widgets that respond to user actions
2. **Consistent Interaction Pattern**: Same methodology as BeepAppBar across all painters
3. **Visual Feedback**: Hover effects, cursor changes, and tooltips
4. **Extensible Architecture**: Easy to add new interactive elements
5. **Event-Driven Design**: Parent controls can handle painter interactions through events

## Testing Strategy

1. **Visual Testing**: Verify hover effects and visual feedback
2. **Interaction Testing**: Test click handlers and action execution
3. **Performance Testing**: Ensure smooth mouse tracking and redrawing
4. **Integration Testing**: Test with actual widget controls
5. **Cross-Painter Testing**: Ensure consistency across different painters

## Future Enhancements

1. **Animation Support**: Smooth transitions for hover effects
2. **Accessibility**: Keyboard navigation support
3. **Touch Support**: Touch-friendly interactions for tablet interfaces
4. **Context Menus**: Right-click context menus for interactive areas
5. **Drag & Drop**: Support for dragging elements between widgets