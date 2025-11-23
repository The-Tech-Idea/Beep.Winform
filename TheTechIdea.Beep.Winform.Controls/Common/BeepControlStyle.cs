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

        /// <summary>FinSet style - compact expanded sidebar inspired by FinSet images</summary>
        FinSet = 20,

        /// <summary>Pill-shaped rail navigation with rounded edges</summary>
        PillRail = 21,

        /// <summary>Apple design language with rounded corners and shadows</summary>
        Apple = 22,

        /// <summary>Microsoft Fluent design system (legacy)</summary>
        Fluent = 23,

        /// <summary>Material Design (legacy) with paper elevation</summary>
        Material = 24,

        /// <summary>Web framework generic styling</summary>
        WebFramework = 25,

        /// <summary>Effect-based borders with custom animations and visual effects</summary>
        Effect = 26,
        
        /// <summary>Terminal/Console Style with monospace font and ASCII characters</summary>
        Terminal = 27,
        
        // NEW ADDITIONS - Form Style Mapping (11 values)
        
        /// <summary>Metro Style with sharp edges, flat design, and bold colors (Windows 8/10)</summary>
        Metro = 28,
        
        /// <summary>Microsoft Office ribbon UI inspired with professional gradients</summary>
        Office = 29,
        
        /// <summary>GNOME/Adwaita aesthetic with flat design and subtle shadows</summary>
        Gnome = 30,
        
        /// <summary>KDE/Breeze with subtle glow and rounded corners</summary>
        Kde = 31,
        
        /// <summary>Cinnamon desktop environment with larger spacing</summary>
        Cinnamon = 32,
        
        /// <summary>elementary OS Pantheon with clean, minimalist design</summary>
        Elementary = 33,
        
        /// <summary>Neo-brutalist with thick borders, no shadows, and bold colors</summary>
        NeoBrutalist = 34,
        
        /// <summary>Gaming Style with angular edges, aggressive design, and RGB effects</summary>
        Gaming = 35,
        
        /// <summary>High contrast accessibility mode with WCAG AAA compliance</summary>
        HighContrast = 36,
        
        /// <summary>Vibrant neon glow with bright saturated colors (distinct from DarkGlow)</summary>
        Neon = 37,
        Retro = 37,
        NeonGlow = 38,

        // Extended mappings aligned with ModernForm painters
        ArcLinux = 39,
        Brutalist = 40,
        Cartoon = 41,
        ChatBubble = 42,
        Cyberpunk = 43,
        Dracula = 44,
        Glassmorphism = 45,
        Holographic = 46,
        GruvBox = 47,
        Metro2 = 48,
        Modern = 49,
        Nord = 50,
        Nordic = 51,
        OneDark = 52,
        Paper = 53,
        Solarized = 54,
        Tokyo = 55,
        Ubuntu = 56
    }
}
