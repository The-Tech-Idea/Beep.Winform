namespace TheTechIdea.Beep.Winform.Controls.Common
{
      public enum BaseControlPainterKind
        {
            None,           // NEW - No painting, for controls that handle their own rendering
            Classic,
            Material
          
        }

    /// <summary>
    /// Unified visual styles for all Beep navigation controls
    /// Used by BeepSideBar, TopNavBar, BottomNavBar, and other navigation components
    /// Get border styles for various design systems in BorderPainters directory
    /// </summary>
    public enum BeepControlStyle
    {
        /// <summary>No special styling, default appearance</summary>
        None = -1,
        /// <summary>Material Design 3 with tonal surfaces and elevation</summary>
        Material3 = 0,

        /// <summary>iOS 15+ clean minimal with subtle animations</summary>
        iOS15 = 1,

        /// <summary>Ant Design inspired with clean lines and blue accents</summary>
        AntDesign = 2,

        /// <summary>Microsoft Fluent 2 design system with acrylic effects</summary>
        Fluent2 = 3,

        /// <summary>Material You dynamic color system</summary>
        MaterialYou = 4,

        /// <summary>Windows 11 mica material effect</summary>
        Windows11Mica = 5,

        /// <summary>Mac OS Big Sur inspired with vibrancy</summary>
        MacOSBigSur = 6,

        /// <summary>Chakra UI modern design with color modes</summary>
        ChakraUI = 7,

        /// <summary>Tailwind CSS utility-first design</summary>
        TailwindCard = 8,

        /// <summary>Notion minimal clean and content-focused</summary>
        NotionMinimal = 9,

        /// <summary>Ultra-minimal flat design with subtle accents</summary>
        Minimal = 10,

        /// <summary>Vercel clean monochrome black and white</summary>
        VercelClean = 11,

        /// <summary>Stripe dashboard professional indigo and purple</summary>
        StripeDashboard = 12,

        /// <summary>Dark mode optimized with glowing neon accents</summary>
        DarkGlow = 13,

        /// <summary>Discord-inspired blurple and gray</summary>
        DiscordStyle = 14,

        /// <summary>Gradient background with modern glassmorphism</summary>
        GradientModern = 15,

        /// <summary>Glass acrylic with transparency and blur effects</summary>
        GlassAcrylic = 16,

        /// <summary>Neumorphism soft UI with extruded shadows</summary>
        Neumorphism = 17,

        /// <summary>Bootstrap framework styling with utility classes</summary>
        Bootstrap = 18,

        /// <summary>Figma card design with modern shadows and spacing</summary>
        FigmaCard = 19,

        /// <summary>Pill-shaped rail navigation with rounded edges</summary>
        PillRail = 20,

        /// <summary>Apple design language with rounded corners and shadows</summary>
        Apple = 21,

        /// <summary>Microsoft Fluent design system (legacy)</summary>
        Fluent = 22,

        /// <summary>Material Design (legacy) with paper elevation</summary>
        Material = 23,

        /// <summary>Web framework generic styling</summary>
        WebFramework = 24,

        /// <summary>Effect-based borders with custom animations and visual effects</summary>
        Effect = 25,
        
        /// <summary>Terminal/Console style with monospace font and ASCII characters</summary>
        Terminal = 26,
        
        // NEW ADDITIONS - Form Style Mapping (11 values)
        
        /// <summary>Metro style with sharp edges, flat design, and bold colors (Windows 8/10)</summary>
        Metro = 27,
        
        /// <summary>Microsoft Office ribbon UI inspired with professional gradients</summary>
        Office = 28,
        
        /// <summary>GNOME/Adwaita aesthetic with flat design and subtle shadows</summary>
        Gnome = 29,
        
        /// <summary>KDE/Breeze with subtle glow and rounded corners</summary>
        Kde = 30,
        
        /// <summary>Cinnamon desktop environment with larger spacing</summary>
        Cinnamon = 31,
        
        /// <summary>elementary OS Pantheon with clean, minimalist design</summary>
        Elementary = 32,
        
        /// <summary>Neo-brutalist with thick borders, no shadows, and bold colors</summary>
        NeoBrutalist = 33,
        
        /// <summary>Gaming style with angular edges, aggressive design, and RGB effects</summary>
        Gaming = 34,
        
        /// <summary>High contrast accessibility mode with WCAG AAA compliance</summary>
        HighContrast = 35,
        
        /// <summary>Vibrant neon glow with bright saturated colors (distinct from DarkGlow)</summary>
        Neon = 36,
        Retro = 37,
        NeonGlow = 38
    }
}
