namespace TheTechIdea.Beep.Winform.Controls.Ratings
{
    /// <summary>Sprint 1 — Pre-set size tiers for the rating control.</summary>
    public enum RatingSizeVariant
    {
        /// <summary>Extra-small — 16 px icons, suits inline/compact contexts.</summary>
        XS = 16,
        /// <summary>Small — 20 px icons.</summary>
        SM = 20,
        /// <summary>Medium — 24 px icons (default).</summary>
        MD = 24,
        /// <summary>Large — 32 px icons.</summary>
        LG = 32,
        /// <summary>Extra-large — 48 px icons, suits hero/spotlight contexts.</summary>
        XL = 48
    }

    /// <summary>Sprint 1 — Arrangement of rating items.</summary>
    public enum RatingLayoutMode
    {
        /// <summary>Items arranged in a single horizontal row (default).</summary>
        Horizontal,
        /// <summary>Items stacked vertically.</summary>
        Vertical,
        /// <summary>Items wrap into multiple rows (grid).</summary>
        Grid
    }

    /// <summary>Sprint 7 — Primary rating scale type.</summary>
    public enum RatingMode
    {
        /// <summary>Standard star/icon scale (1–StarCount).</summary>
        Stars,
        /// <summary>Net Promoter Score 0–10 numeric pill scale.</summary>
        NPS
    }
}
