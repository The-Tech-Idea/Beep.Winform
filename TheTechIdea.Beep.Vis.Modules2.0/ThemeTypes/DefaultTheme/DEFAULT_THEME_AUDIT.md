Default Theme Audit
====================

Location: ThemeTypes/DefaultTheme/Parts/

Summary
-------
This audit inspects color and typography tokens defined for DefaultTheme and checks common usages by controls. It highlights inconsistencies and high-risk tokens that may cause poor contrast or inconsistent appearance.

Files scanned (parts):
- BeepTheme.Core.cs
- BeepTheme.Buttons.cs
- BeepTheme.TextBox.cs
- BeepTheme.Label.cs
- BeepTheme.Dialog.cs
- BeepTheme.ComboBox.cs
- ... (all Parts/*.cs under DefaultTheme were considered during the audit)

Key tokens discovered (representative)
-------------------------------------
From BeepTheme.Core.cs
- ForeColor = Color.White
- BackColor = Color.White
- PanelBackColor = Color.FromArgb(245, 245, 245)
- DisabledBackColor = Color.FromArgb(200, 200, 200)
- BorderColor = Color.FromArgb(200, 200, 200)
- ActiveBorderColor = Color.FromArgb(33, 150, 243)

From BeepTheme.Buttons.cs
- ButtonBackColor = Color.White
- ButtonForeColor = Color.FromArgb(33, 150, 243)
- ButtonBorderColor = Color.FromArgb(33, 150, 243)
- ButtonHoverBackColor = Color.FromArgb(227, 242, 253)
- ButtonPressedBackColor = Color.FromArgb(21, 101, 192)
- ButtonSelectedBackColor = Color.FromArgb(25, 118, 210)
- ButtonSelectedForeColor = Color.White

From BeepTheme.TextBox.cs
- TextBoxBackColor = Color.White
- TextBoxForeColor = Color.Black
- TextBoxBorderColor = Color.Gray

Findings / Issues
-----------------
1) Core ForeColor = White while BackColor = White
   - Many controls fall back to _currentTheme.ForeColor if no control-specific ForeColor is set.
   - Having ForeColor == White on a White background leads to invisible text across the UI.
   - Likely the intended ForeColor for DefaultTheme should be a dark color (e.g., near-black).

2) Button color semantics unclear
   - ButtonBackColor = White and ButtonForeColor = AccentBlue (33,150,243).
   - Some controls set Pressed/Selected states to blue with white text (good for primary/filled variant), but default button uses white background with blue text and blue border (outline style).
   - Controls that intended a filled primary button may instead show outline because ButtonBackColor is White. There is inconsistency between components that expect ButtonBackColor to represent primary filled background vs. outline base.

3) TextBox uses TextBoxForeColor = Black while Core.ForeColor = White
   - Controls that use TextBox* tokens will display correctly, but controls using the generic ForeColor will be broken.

4) Dialog tokens partially present but not consistently used
   - Some dialog implementations use PrimaryTextColor / SecondaryTextColor tokens which may not be present or consistent in DefaultTheme (audit required for Dialog tokens specifically).

5) Contrast and accessibility not verified
   - Several text/background pairs may not meet WCAG AA. Example: ButtonHoverForeColor = AccentBlue on ButtonHoverBackColor (light blue) likely has low contrast in some cases.

Recommendations (next steps)
----------------------------
- Immediate high-priority fix: change Core.ForeColor to a dark value (e.g., Color.FromArgb(33,33,33) or Color.Black) so default text is visible across controls.
- Decide canonical meaning for ButtonBackColor:
  - Option A (outline-centric): Keep ButtonBackColor = White (outline button by default). Introduce ButtonPrimaryBackColor/ButtonPrimaryForeColor for filled primary buttons and update controls accordingly.
  - Option B (filled-centric): Make ButtonBackColor the primary filled color (Accent Blue) and set a ButtonOutlineBackColor = White for outline variant. Update controls that currently use ButtonBackColor expecting outline behavior.
- Add explicit primary/secondary token names to Buttons.cs and provide aliases for backward compatibility.
- Run a control usage search for `_currentTheme.ForeColor` and `_currentTheme.ButtonBackColor` to identify places relying on Core tokens vs control-specific tokens.
- After fixing Core.ForeColor, run the sample app to verify no more invisible text.

Proposed immediate small change (safe): Update Core.ForeColor from White to Color.FromArgb(33,33,33). This resolves the most obvious visibility bug with minimal functional change.

Audit generated: (timestamp) 