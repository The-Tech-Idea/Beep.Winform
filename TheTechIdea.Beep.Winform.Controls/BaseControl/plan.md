# Plan: Material style integration into BaseControl (helpers-first)

Overview
- Use existing helper architecture. No drawing code stays in the control; delegate to a new BaseControlMaterialHelper.
- Wire a partial hook: BaseControl.DrawCustomBorder calls partial DrawCustomBorder_Ext, implemented in BaseControl.Material.cs to call the helper.
- Keep all Material-specific state as properties on the partial class. Helper reads from these properties.

Steps
1) BaseControl.cs: call DrawCustomBorder_Ext(g) from DrawCustomBorder and declare partial method.
2) Add BaseControlMaterialHelper (Helpers/BaseControlMaterialHelper.cs):
   - Computes input rect and icon rects.
   - Renders Material variants (Outlined/Filled/Standard) and icons.
3) BaseControl.Material.cs (partial):
   - Define Material properties: EnableMaterialStyle, MaterialVariant, MaterialBorderRadius, MaterialShowFill, MaterialFillColor, MaterialOutlineColor, MaterialPrimaryColor, LeadingIconPath(s), MaterialIconSize/Padding.
   - Implement DrawCustomBorder_Ext to instantiate and delegate to helper.
   - No duplicate event overrides; leave existing BaseControl input pipeline intact.

Result
- BaseControl gains Material border styling and inline icon rendering using helper classes and partial pattern, consistent with the rest of the architecture.
