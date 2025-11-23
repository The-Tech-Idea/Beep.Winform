MinimalSideBarPainter
====================

This file replaces the old `MinimalSideBarPainter.cs` which was consolidated into `NotionMinimalSideBarPainter.cs`.

The `.cs` source file has been removed from active compilation and replaced with a deprecation note. If you need to restore the Minimal painter in the future, re-add a class in `MinimalSideBarPainter.cs` implementing `BaseSideBarPainter` and map `BeepControlStyle.Minimal` to it in `BeepSideBar.Painters.cs`.

History
-------
- 2025-11-23: Minimal style consolidated into NotionMinimal. The source file was removed from active compilation.

