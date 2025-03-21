using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using TheTechIdea.Beep.ConfigUtil;

using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

using System.Linq.Expressions;



namespace TheTechIdea.Beep.Winform.Controls.Helpers;
public static class ControlExtensions
{
    public delegate T ObjectActivator<T>(params object[] args);
    public static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
    {
        Type type = ctor.DeclaringType;
        ParameterInfo[] paramsInfo = ctor.GetParameters();

        //create a single param of type object[]
        ParameterExpression param =
            Expression.Parameter(typeof(object[]), "args");

        Expression[] argsExp =
            new Expression[paramsInfo.Length];

        //pick each arg from the params array 
        //and create a typed expression of them
        for (int i = 0; i < paramsInfo.Length; i++)
        {
            Expression index = Expression.Constant(i);
            Type paramType = paramsInfo[i].ParameterType;

            Expression paramAccessorExp =
                Expression.ArrayIndex(param, index);

            Expression paramCastExp =
                Expression.Convert(paramAccessorExp, paramType);

            argsExp[i] = paramCastExp;
        }

        //make a NewExpression that calls the
        //ctor with the args we just created
        NewExpression newExp = Expression.New(ctor, argsExp);

        //create a lambda with the New
        //Expression as body and our param object[] as arg
        LambdaExpression lambda =
            Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

        //compile it
        ObjectActivator<T> compiled = (ObjectActivator<T>)lambda.Compile();
        return compiled;
    }
    public static void DoubleBuffered(this Control control, bool enable)
    {
        var doubleBufferProperty = control.GetType().GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (doubleBufferProperty != null)
        {
            doubleBufferProperty.SetValue(control, enable, null);
        }
    }
    public static string GetFormattedText(this Control control, string text, string maskFormat)
    {
        if (string.IsNullOrEmpty(maskFormat))
            return text;

        // Number formatting
        if (decimal.TryParse(text, out var number))
        {
            switch (maskFormat.ToLower())
            {
                case "currency":
                    return number.ToString("C", CultureInfo.CurrentCulture); // Currency format
                case "percentage":
                    return number.ToString("P", CultureInfo.CurrentCulture); // Percentage format
                case "fixedpoint":
                    return number.ToString("F2", CultureInfo.CurrentCulture); // Fixed-point format with 2 decimals
                case "scientific":
                    return number.ToString("E", CultureInfo.CurrentCulture); // Scientific notation
                case "number":
                    return number.ToString("N", CultureInfo.CurrentCulture); // Number with thousand separators
                case "hexadecimal":
                    return ((int)number).ToString("X"); // Hexadecimal format (integers only)
            }
        }

        // Date formatting
        if (DateTime.TryParse(text, out var date))
        {
            switch (maskFormat.ToLower())
            {
                case "shortdate":
                    return date.ToString("d", CultureInfo.CurrentCulture); // Short date
                case "longdate":
                    return date.ToString("D", CultureInfo.CurrentCulture); // Long date
                case "shorttime":
                    return date.ToString("t", CultureInfo.CurrentCulture); // Short time
                case "longtime":
                    return date.ToString("T", CultureInfo.CurrentCulture); // Long time
                case "monthday":
                    return date.ToString("M", CultureInfo.CurrentCulture); // Month day format
                case "yearmonth":
                    return date.ToString("Y", CultureInfo.CurrentCulture); // Year month format
                case "rfc1123":
                    return date.ToString("R", CultureInfo.InvariantCulture); // RFC 1123 format
                case "sortable":
                    return date.ToString("s", CultureInfo.InvariantCulture); // Sortable date/time format
                case "iso":
                    return date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture); // ISO 8601 format
            }
        }

        // Custom formatting (if users want to specify their own format pattern)
        try
        {
            // Assume the user-provided maskFormat is a valid .NET format string
            return string.Format(CultureInfo.CurrentCulture, "{0:" + maskFormat + "}", text);
        }
        catch
        {
            // Return unformatted text if the format string is invalid
            return text;
        }
    }

    // Special thanks to ReaLTaiizor for this code snippet
    public static void DrawRoundedRectangle(this Control control, Graphics G, Pen P, float X, float Y, float W, float H, float Rad)
    {
        using GraphicsPath GP = new();
        GP.AddLine(X + Rad, Y, X + W - (Rad * 2), Y);
        GP.AddArc(X + W - (Rad * 2), Y, Rad * 2, Rad * 2, 270, 90);
        GP.AddLine(X + W, Y + Rad, X + W, Y + H - (Rad * 2));
        GP.AddArc(X + W - (Rad * 2), Y + H - (Rad * 2), Rad * 2, Rad * 2, 0, 90);
        GP.AddLine(X + W - (Rad * 2), Y + H, X + Rad, Y + H);
        GP.AddArc(X, Y + H - (Rad * 2), Rad * 2, Rad * 2, 90, 90);
        GP.AddLine(X, Y + H - (Rad * 2), X, Y + Rad);
        GP.AddArc(X, Y, Rad * 2, Rad * 2, 180, 90);
        GP.CloseFigure();

        G.SmoothingMode = SmoothingMode.AntiAlias;
        G.DrawPath(P, GP);
        G.SmoothingMode = SmoothingMode.Default;

    }
    public static GraphicsPath RoundRect(this Control control, Rectangle Rectangle, int Curve)
    {
        GraphicsPath GP = new();

        int ArcRectangleWidth = Curve * 2;

        GP.AddArc(new(Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -180, 90);
        GP.AddArc(new(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -90, 90);
        GP.AddArc(new(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 0, 90);
        GP.AddArc(new(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 90, 90);
        GP.AddLine(new Point(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y), new Point(Rectangle.X, Curve + Rectangle.Y));

        return GP;
    }

    public static GraphicsPath RoundRect(this Control control, int X, int Y, int Width, int Height, int Curve)
    {
        Rectangle Rectangle = new(X, Y, Width, Height);

        GraphicsPath GP = new();

        int EndArcWidth = Curve * 2;

        GP.AddArc(new(Rectangle.X, Rectangle.Y, EndArcWidth, EndArcWidth), -180, 90);
        GP.AddArc(new(Rectangle.Width - EndArcWidth + Rectangle.X, Rectangle.Y, EndArcWidth, EndArcWidth), -90, 90);
        GP.AddArc(new(Rectangle.Width - EndArcWidth + Rectangle.X, Rectangle.Height - EndArcWidth + Rectangle.Y, EndArcWidth, EndArcWidth), 0, 90);
        GP.AddArc(new(Rectangle.X, Rectangle.Height - EndArcWidth + Rectangle.Y, EndArcWidth, EndArcWidth), 90, 90);
        GP.AddLine(new Point(Rectangle.X, Rectangle.Height - EndArcWidth + Rectangle.Y), new Point(Rectangle.X, Curve + Rectangle.Y));

        return GP;
    }

    public static GraphicsPath RoundedTopRect(this Control control, Rectangle Rectangle, int Curve)
    {
        GraphicsPath GP = new();

        int ArcRectangleWidth = Curve * 2;

        GP.AddArc(new(Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -180, 90);
        GP.AddArc(new(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -90, 90);
        GP.AddLine(new Point(Rectangle.X + Rectangle.Width, Rectangle.Y + ArcRectangleWidth), new Point(Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height - 1));
        GP.AddLine(new Point(Rectangle.X, Rectangle.Height - 1 + Rectangle.Y), new Point(Rectangle.X, Rectangle.Y + Curve));

        return GP;
    }

    public static GraphicsPath CreateRoundRect(this Control control, float X, float Y, float Width, float Height, float Radius)
    {
        GraphicsPath GP = new();
        GP.AddLine(X + Radius, Y, X + Width - (Radius * 2), Y);
        GP.AddArc(X + Width - (Radius * 2), Y, Radius * 2, Radius * 2, 270, 90);

        GP.AddLine(X + Width, Y + Radius, X + Width, Y + Height - (Radius * 2));
        GP.AddArc(X + Width - (Radius * 2), Y + Height - (Radius * 2), Radius * 2, Radius * 2, 0, 90);

        GP.AddLine(X + Width - (Radius * 2), Y + Height, X + Radius, Y + Height);
        GP.AddArc(X, Y + Height - (Radius * 2), Radius * 2, Radius * 2, 90, 90);

        GP.AddLine(X, Y + Height - (Radius * 2), X, Y + Radius);
        GP.AddArc(X, Y, Radius * 2, Radius * 2, 180, 90);

        GP.CloseFigure();

        return GP;
    }

    public static GraphicsPath CreateUpRoundRect(this Control control, float X, float Y, float Width, float Height, float Radius)
    {
        GraphicsPath GP = new();

        GP.AddLine(X + Radius, Y, X + Width - (Radius * 2), Y);
        GP.AddArc(X + Width - (Radius * 2), Y, Radius * 2, Radius * 2, 270, 90);

        GP.AddLine(X + Width, Y + Radius, X + Width, Y + Height - (Radius * 2) + 1);
        GP.AddArc(X + Width - (Radius * 2), Y + Height - (Radius * 2), Radius * 2, 2, 0, 90);

        GP.AddLine(X + Width, Y + Height, X + Radius, Y + Height);
        GP.AddArc(X, Y + Height - (Radius * 2) + 1, Radius * 2, 1, 90, 90);

        GP.AddLine(X, Y + Height, X, Y + Radius);
        GP.AddArc(X, Y, Radius * 2, Radius * 2, 180, 90);

        GP.CloseFigure();

        return GP;
    }

    public static GraphicsPath CreateLeftRoundRect(float X, float Y, float Width, float Height, float Radius)
    {
        GraphicsPath GP = new();
        GP.AddLine(X + Radius, Y, X + Width - (Radius * 2), Y);
        GP.AddArc(X + Width - (Radius * 2), Y, Radius * 2, Radius * 2, 270, 90);

        GP.AddLine(X + Width, Y + 0, X + Width, Y + Height);
        GP.AddArc(X + Width - (Radius * 2), Y + Height - 1, Radius * 2, 1, 0, 90);

        GP.AddLine(X + Width - (Radius * 2), Y + Height, X + Radius, Y + Height);
        GP.AddArc(X, Y + Height - (Radius * 2), Radius * 2, Radius * 2, 90, 90);

        GP.AddLine(X, Y + Height - (Radius * 2), X, Y + Radius);
        GP.AddArc(X, Y, Radius * 2, Radius * 2, 180, 90);

        GP.CloseFigure();

        return GP;
    }
    // based EntityHelper.GetAllAvailableUIComponents function , create a function the select the default BeepControl based on DBFieldCategory and return Control Type
    public static Type GetDefaultControlType(DbFieldCategory category)
    {
        switch (category)
        {
            case DbFieldCategory.String:
                return typeof(BeepTextBox);
            case DbFieldCategory.Numeric:
                return typeof(BeepTextBox);
            case DbFieldCategory.Date:
                return typeof(BeepDatePicker);
            case DbFieldCategory.Boolean:
                return typeof(BeepCheckBoxBool);
            case DbFieldCategory.Binary:
                return typeof(BeepImage);
            case DbFieldCategory.Guid:
                return typeof(BeepTextBox);
            case DbFieldCategory.Json:
                return typeof(BeepTextBox);
            case DbFieldCategory.Xml:
                return typeof(BeepTextBox);
            case DbFieldCategory.Geography:
                return typeof(BeepTextBox);
            case DbFieldCategory.Currency:
                return typeof(BeepTextBox);
            case DbFieldCategory.Enum:
                return typeof(BeepComboBox);
            case DbFieldCategory.Timestamp:
                return typeof(BeepTextBox);
            case DbFieldCategory.Complex:
                return typeof(BeepTextBox);
            default:
                return typeof(BeepTextBox);
        }
    }
    #region "BeepTree Extensions"
    public static BindingList<SimpleItem> GetBranchs(this ITree tree,IDMEEditor DMEEditor)
    {
        BindingList<SimpleItem> simpleItems = new BindingList<SimpleItem>();
        var res=  tree.CreateTreeTuple(DMEEditor);
        tree.GenerBranchs = res.Item2;
        tree.Branches = res.Item1;
        foreach (var item in res.Item2)
        {
            SimpleItem node = new SimpleItem();
            IBranch br = item.Item1;
            node.Text = br.BranchText;
            node.Name = br.Name;
            node.Id = br.ID; 
            node.ImagePath = ImageListHelper.GetImagePathFromName(br.IconImageName);
            node.GuidId = br.GuidID;
            node.PointType = br.BranchType;
            node.ParentID = 0;
            node.Children = new BindingList<SimpleItem>();
            node.Children = GetChildBranch(tree, br);
            DynamicMenuManager.CreateMenuMethods(tree.DMEEditor,br);
            if (br.ObjectType != null && br.BranchClass != null)
            {
                DynamicMenuManager.CreateGlobalMenu(tree.DMEEditor, br);
            }
            simpleItems.Add(node);
        }
        foreach (var item in tree.Branches.Where(p=>p.BranchType== EnumPointType.Root))
        {
            SimpleItem node = new SimpleItem();
            IBranch br = item;
            node.Text = br.BranchText;
            node.Name = br.Name;
            node.Id = br.ID;
            node.ImagePath = ImageListHelper.GetImagePathFromName(br.IconImageName);
            node.GuidId = br.GuidID;
            node.ParentID = 0;
            node.Children = new BindingList<SimpleItem>();
            node.Children = GetChildBranch(tree, br);
            DynamicMenuManager.CreateMenuMethods(tree.DMEEditor, br);
            if (br.ObjectType != null && br.BranchClass != null)
            {
                DynamicMenuManager.CreateGlobalMenu(tree.DMEEditor, br);
            }
            simpleItems.Add(node);
        }
        return simpleItems;
    }
    public static BindingList<SimpleItem> GetBranchs(this ITree tree, Tuple<List<IBranch>, List<Tuple<IBranch, string>>> res)
    {
        BindingList<SimpleItem> simpleItems = new BindingList<SimpleItem>();
        foreach (var item in res.Item2)
        {
            SimpleItem node = new SimpleItem();
            IBranch br = item.Item1;
            node.Text = br.BranchText;
            node.Name = br.Name;
            node.Id = br.ID;
            node.ImagePath = ImageListHelper.GetImagePathFromName(br.IconImageName);
            node.GuidId = br.GuidID;
            node.ParentID = 0;
            node.ObjectType = br.ObjectType;
            node.BranchClass = br.BranchClass;
          
            node.PointType = br.BranchType;
            node.AssemblyClassDefinitionID = br.MiscStringID; ;
            node.Children = new BindingList<SimpleItem>();
            node.Children = GetChildBranch(tree, br);
            DynamicMenuManager.CreateMenuMethods(tree.DMEEditor,br);
            if (br.ObjectType != null && br.BranchClass != null)
            {
                DynamicMenuManager.CreateGlobalMenu(tree.DMEEditor, br);
            }
            simpleItems.Add(node);
        }
        foreach (var item in tree.Branches.Where(p => p.BranchType == EnumPointType.Root && p.ParentBranch==null))
        {
            SimpleItem node = new SimpleItem();
            IBranch br = item;
            node.Text = br.BranchText;
            node.Name = br.Name;
            node.Id = br.ID;
            node.ImagePath = ImageListHelper.GetImagePathFromName(br.IconImageName);
            node.GuidId = br.GuidID;
            node.PointType = br.BranchType;
            node.ParentID = 0;
            node.Children = new BindingList<SimpleItem>();
            node.Children = GetChildBranch(tree, br);
            DynamicMenuManager.CreateMenuMethods(tree.DMEEditor, br);
            if (br.ObjectType != null && br.BranchClass != null)
            {
                DynamicMenuManager.CreateGlobalMenu(tree.DMEEditor, br);
            }
            simpleItems.Add(node);
        }
        
        return simpleItems;
    }
    public static BindingList<SimpleItem> GetChildBranch(this ITree tree,IBranch br)
    {
        BindingList<SimpleItem> Childitems = new BindingList<SimpleItem>();
        foreach (var item1 in br.ChildBranchs)
        {
            SimpleItem node1 = new SimpleItem();
            node1.Text = item1.BranchText;
            node1.Name = item1.Name;
            node1.Id = item1.ID;
            node1.ImagePath = ImageListHelper.GetImagePathFromName(item1.IconImageName);
            node1.GuidId = item1.GuidID;
            node1.ObjectType = item1.ObjectType;
            node1.BranchClass = item1.BranchClass;
            node1.PointType = item1.BranchType;
            node1.AssemblyClassDefinitionID = item1.MiscStringID; ;
            node1.ParentID = br.ID;
            node1.Children = new BindingList<SimpleItem>();
            node1.Children = GetChildBranch(tree, item1);
            DynamicMenuManager.CreateMenuMethods(tree.DMEEditor, item1);
            if (br.ObjectType != null && br.BranchClass != null)
            {
                DynamicMenuManager.CreateGlobalMenu(tree.DMEEditor, item1);
            }
            Childitems.Add(node1);
        }
        return Childitems;
    }
    public static BindingList<SimpleItem> GetSimpleItemsFromExecuteCreateChildsMethods(this ITree tree, IBranch br)
    {
        BindingList<SimpleItem> Childitems = new BindingList<SimpleItem>();
        IErrorsInfo retval = new ErrorsInfo();
        try
        { 
            br.CreateChildNodes();
            Childitems= GetChildBranch(tree, br);
            retval.Flag = Errors.Ok;
            retval.Message = "Childs Created";
        }
        catch (Exception ex)
        {
            retval.Flag = Errors.Failed;
            retval.Message = ex.Message;
        }
        return Childitems;
    }
    //public static IErrorsInfo AddBranch(this ITree tree, IBranch ParentBranch, IBranch Branch)
    //{
    //    SimpleItem parentnode = new SimpleItem();
    //    try
    //    {
    //        if (ParentBranch.ChildBranchs.Where(x => x.BranchText == Branch.BranchText).Any())
    //        {
    //            DMEEditor.AddLogMessage("Error", "Branch already exist", DateTime.Now, -1, null, Errors.Failed);
    //            return DMEEditor.ErrorObject;
    //        }
    //        parentnode = GetNodeByGuidID(ParentBranch.GuidID);
    //        ParentBranch.ChildBranchs.Add(Branch);
    //        Branch.ParentBranch = ParentBranch;
    //        Branch.ParentBranchID = ParentBranch.ID;
    //        Branch.ParentGuidID = ParentBranch.GuidID;
    //        DynamicMenuManager.CreateMenuMethods(DMEEditor, Branch);
    //        DynamicMenuManager.CreateGlobalMenu(DMEEditor, Branch);
    //        Branch.DMEEditor = DMEEditor;
    //        Branch.Visutil = VisManager;
    //        Branch.TreeEditor = this;
    //        if (parentnode != null)
    //        {
    //            SimpleItem item = ControlExtensions.CreateNode(this, Branch.ID, Branch);
    //            parentnode.Children.Add(item);
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        string mes = "Could not Add Branch to " + ParentBranch.BranchText;
    //        DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
    //    };
    //    return DMEEditor.ErrorObject;
    //}
    public static BindingList<SimpleItem> AddBranchToTree(this ITree tree, SimpleItem parent,SimpleItem child,IBranch br)
    {

        DynamicMenuManager.CreateMenuMethods(tree.DMEEditor, br);
        DynamicMenuManager.CreateGlobalMenu(tree.DMEEditor, br);
        parent.Children.Add(child);
        return parent.Children;
    }
    public static BindingList<SimpleItem> AddBranchToTree(this ITree tree, SimpleItem parent, IBranch br)
    {
        br.CreateChildNodes();
        SimpleItem node = new SimpleItem();
        node.Text = br.BranchText;
        node.Name = br.Name;
        node.Id = br.ID;
        node.ImagePath = ImageListHelper.GetImagePathFromName(br.IconImageName);
        node.GuidId = br.GuidID;
        node.ParentID = parent.Id;
        node.ObjectType = br.ObjectType;
        node.BranchClass = br.BranchClass;
        node.PointType = br.BranchType;
        node.AssemblyClassDefinitionID = br.MiscStringID; ;
        node.Children = new BindingList<SimpleItem>();
        node.Children = GetChildBranch(tree, br);
        DynamicMenuManager.CreateMenuMethods(tree.DMEEditor, br);
        if (br.ObjectType != null && br.BranchClass != null)
        {
            DynamicMenuManager.CreateGlobalMenu(tree.DMEEditor, br);
        }
        parent.Children.Add(node);
        return parent.Children;
    }
    public static Tuple <List<IBranch>,List<Tuple<IBranch, string>>> CreateTreeTuple(this ITree tree, IDMEEditor DMEEditor)
    {
        var Branches = new List<IBranch>();
        var GenerBranchs = new List<Tuple<IBranch, string>>();
        
        string packagename = "";
        try
        {
           
            int SeqID = 0;
            //tree. = new TreeNodeDragandDropHandler(DMEEditor, this);
            //tree.Treebranchhandler = new TreeBranchHandler(DMEEditor, this);
          
            IBranch Genrebr = null;
            // AssemblyClassDefinition GenreBrAssembly = DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null && p.VisSchema != null && p.VisSchema.BranchType == EnumPointType.Genre).FirstOrDefault()!;
            foreach (AssemblyClassDefinition GenreBrAssembly in DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null && p.VisSchema != null && p.VisSchema.BranchType == EnumPointType.Genre).OrderBy(x => x.Order))
            {
                SeqID++;
                if (GenreBrAssembly != null)
                {
                    Type adc = DMEEditor.assemblyHandler.GetType(GenreBrAssembly.PackageName);
                    ConstructorInfo ctor = adc.GetConstructors().Where(o => o.GetParameters().Length == 0).FirstOrDefault()!;
                    if (ctor != null)
                    {
                        ObjectActivator<IBranch> createdActivator = GetActivator<IBranch>(ctor);
                        try
                        {
                            Genrebr = createdActivator();
                            if (Genrebr.BranchType == EnumPointType.Genre)
                            {
                                int id = SeqID;
                                Genrebr.Name = GenreBrAssembly.PackageName;
                                packagename = GenreBrAssembly.PackageName;
                                Genrebr.ID = id;
                                Genrebr.BranchID = id;
                                Genrebr.BranchText = GenreBrAssembly.classProperties.Caption;
                                Genrebr.DMEEditor = DMEEditor;
                               // CreateNode(id, Genrebr);
                                //else CreateNode(id, Genrebr);
                                Genrebr.MiscStringID = GenreBrAssembly.GuidID;
                           
                                try
                                {
                                    Genrebr.SetConfig(tree, tree.DMEEditor, Genrebr.ParentBranch, Genrebr.BranchText, Genrebr.ID, Genrebr.BranchType, null);
                                }
                                catch (Exception ex)
                                {

                                }
                              //  Genrebr.CreateChildNodes();
                                GenerBranchs.Add(new Tuple<IBranch, string>(Genrebr, GenreBrAssembly.classProperties.menu));
                            }
                        }
                        catch (Exception ex)
                        {
                            DMEEditor.AddLogMessage("Error", $"Creating StandardTree Root Node {GenreBrAssembly.PackageName} {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);
                        }
                    }
                }
            }

            foreach (AssemblyClassDefinition cls in DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null).OrderBy(x => x.Order))
            {
                Type adc = DMEEditor.assemblyHandler.GetType(cls.PackageName);
                ConstructorInfo ctor = adc.GetConstructors().Where(o => o.GetParameters().Length == 0).FirstOrDefault()!;
                SeqID++;
                if (ctor != null)
                {
                    ObjectActivator<IBranch> createdActivator = GetActivator<IBranch>(ctor);
                    try
                    {
                        IBranch br = createdActivator();
                        if (br.BranchType == EnumPointType.Root)
                        {
                            int id = SeqID;
                            br.Name = cls.PackageName;
                            packagename = cls.PackageName;
                            br.ID = id;
                            br.BranchID = id;
                            br.BranchText = cls.classProperties.Caption;
                            br.DMEEditor = DMEEditor;
                            br.MiscStringID = cls.GuidID;
                            if (cls.classProperties.ObjectType != null)
                            {

                                var tr = GenerBranchs.FirstOrDefault(p => p.Item2.Equals(cls.classProperties.menu, StringComparison.OrdinalIgnoreCase));
                                if (tr != null)
                                {
                                    Genrebr = tr.Item1;
                                }
                                else
                                    Genrebr = null;
                                if (Genrebr != null)
                                {
                                    try
                                    {
                                        br.ParentBranch = Genrebr;
                                        br.SetConfig(tree, tree.DMEEditor, Genrebr, br.BranchText, br.ID, br.BranchType, null);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    Genrebr.ChildBranchs.Add(br);
                                }
                                else
                                {
                                    br.ParentBranch = null;
                                    br.ParentBranchID = -1;
                                    br.ParentGuidID = string.Empty;
                                    try
                                    {
                                        br.ParentBranch = Genrebr;
                                        br.SetConfig(tree, tree.DMEEditor, br.ParentBranch, br.BranchText, br.ID, br.BranchType, null);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }

                            }
                          //  br.CreateChildNodes();
                            Branches.Add(br);
                        }
                    }
                    catch (Exception ex)
                    {
                        DMEEditor.AddLogMessage("Error", $"Creating StandardTree Root Node {cls.PackageName} {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DMEEditor.ErrorObject.Ex = ex;
            DMEEditor.ErrorObject.Flag = Errors.Failed;
            DMEEditor.AddLogMessage("Error", $"Creating StandardTree Root Node {packagename} - {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);

        };

        Tuple<List<IBranch>, List<Tuple<IBranch, string>>> retval = new(Branches, GenerBranchs);
        return  retval;
    }
    #endregion "BeepTree Extensions"
    #region "ITree Extensions"
    //public static void RunFunctionFromExtensions(this ITree tree,SimpleItem item,string MethodName)
    //{
    //    IBranch br = null;
    //    AssemblyClassDefinition assemblydef = new AssemblyClassDefinition();
    //    MethodInfo method = null;
    //    MethodsClass methodsClass;
        
    //    assemblydef = AssemblyDefinitionsHelper.GetAssemblyGlobalFunctionsClassDefinitionByGuid(tree.DMEEditor, item.AssemblyClassDefinitionID);
    //    dynamic fc = tree.DMEEditor.assemblyHandler.CreateInstanceFromString(assemblydef.dllname, assemblydef.type.ToString(), new object[] { tree.DMEEditor, tree.VisManager, tree });
    //    //  dynamic fc = DMEEditor.assemblyHandler.CreateInstanceFromString(assemblydef.type.ToString(), new object[] { DMEEditor, Vismanager, this });
    //    if (fc == null)
    //    {
    //        return;
    //    }

    //    Type t = ((IFunctionExtension)fc).GetType();
    // //   AssemblyClassDefinition cls = tree.DMEEditor.ConfigEditor.GlobalFunctions.Where(x => x.className == t.Name).FirstOrDefault();
       
    //    methodsClass = assemblydef.Methods.Where(x => x.Caption == MethodName).FirstOrDefault();

    //    if (tree.DMEEditor.Passedarguments == null)
    //    {
    //        tree.DMEEditor.Passedarguments = new PassedArgs();
    //    }
    //    if (br != null)
    //    {
    //        tree.DMEEditor.Passedarguments.ObjectName = br.BranchText;
    //        tree.DMEEditor.Passedarguments.DatasourceName = br.DataSourceName;
    //        tree.DMEEditor.Passedarguments.Id = br.BranchID;
    //        tree.DMEEditor.Passedarguments.ParameterInt1 = br.BranchID;
    //        if (!IsMethodApplicabletoNode(assemblydef, br)) return;

    //    }

    //    //if (methodsClass != null)
    //    //{
    //    //    PassedArgs args = new PassedArgs();
    //    //    ErrorsInfo ErrorsandMesseges = new ErrorsInfo();
    //    //    args.Cancel = false;
    //    //    tree.PreCallModule?.Invoke(tree, args);

    //    //    if (args.Cancel)
    //    //    {
    //    //        tree.DMEEditor.AddLogMessage("Beep", $"You dont have Access Privilige on {MethodName}", DateTime.Now, 0, MethodName, Errors.Failed);
    //    //        ErrorsandMesseges.Flag = Errors.Failed;
    //    //        ErrorsandMesseges.Message = $"Function Access Denied";
    //    //        return;
    //    //    }
           
    //    //}
    //    method = methodsClass.Info;
    //    if (method.GetParameters().Length > 0)
    //    {
    //        method.Invoke(fc, new object[] { tree.DMEEditor.Passedarguments });
    //    }
    //    else
    //        method.Invoke(fc, null);
    //}
    private static bool IsMethodApplicabletoNode(AssemblyClassDefinition cls, IBranch br)
    {
        if (cls.classProperties == null)
        {
            return true;
        }
        if (cls.classProperties.ObjectType != null)
        {
            //if (!cls.classProperties.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return false ;
            //}
        }
        return true;
    }
    //public static IErrorsInfo RunMethodFromObject(this ITree tree, object branch, string MethodName)
    //{
    //    try
    //    {
    //        Type t = branch.GetType();
    //        AssemblyClassDefinition cls = tree.DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.className == t.Name).FirstOrDefault();
    //        MethodInfo method = null;
    //        MethodsClass methodsClass;
    //        try
    //        {
    //            methodsClass = cls.Methods.Where(x => x.Caption == MethodName).FirstOrDefault();
    //        }
    //        catch (Exception)
    //        {
    //            methodsClass = null;
    //        }
    //        if (methodsClass != null)
    //        {
    //            if (!IsMethodApplicabletoNode(cls, (IBranch)branch)) return tree.DMEEditor.ErrorObject;
    //            //PassedArgs args = new PassedArgs();
    //            //args.ObjectName = MethodName;
    //            //args.ObjectType = methodsClass.ObjectType;
    //            //args.Cancel = false;
    //            //PreCallModule?.Invoke(this, args);
    //            //if (args.Cancel)
    //            //{
    //            //    DMEEditor.AddLogMessage("Beep", $"You dont have Access Privilige on {MethodName}", DateTime.Now, 0, MethodName, Errors.Failed);
    //            //    ErrorsandMesseges.Flag = Errors.Failed;
    //            //    ErrorsandMesseges.Message = $"Function Access Denied";
    //            //    return ErrorsandMesseges;
    //            //}

    //            method = methodsClass.Info;
    //            if (method.GetParameters().Length > 0)
    //            {
    //                method.Invoke(branch, new object[] { tree.DMEEditor.Passedarguments.Objects[0].obj });
    //            }
    //            else
    //                method.Invoke(branch, null);


    //            //  DMEEditor.AddLogMessage("Success", "Running method", DateTime.Now, 0, null, Errors.Ok);
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        string mes = "Could not Run Method " + MethodName;
    //        tree.DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
    //    };
    //    return tree.DMEEditor.ErrorObject;
    //}
    public static List<IBranch> CreateTree(this ITree tree)
    {
        string packagename = "";
        try
        {
            tree.SeqID = 0;
            //tree. = new TreeNodeDragandDropHandler(DMEEditor, this);
            //tree.Treebranchhandler = new TreeBranchHandler(DMEEditor, this);
            tree.Branches = new List<IBranch>();
            tree.GenerBranchs = new List<Tuple<IBranch, string>>();
            IBranch Genrebr = null;
            foreach (AssemblyClassDefinition GenreBrAssembly in tree.DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null && p.VisSchema != null && p.VisSchema.BranchType == EnumPointType.Genre).OrderBy(x => x.Order))
            {
                tree.SeqID++;
                if (GenreBrAssembly != null)
                {
                    Type adc = tree.DMEEditor.assemblyHandler.GetType(GenreBrAssembly.PackageName);
                    ConstructorInfo ctor = adc.GetConstructors().Where(o => o.GetParameters().Length == 0).FirstOrDefault()!;
                    if (ctor != null)
                    {
                        ObjectActivator<IBranch> createdActivator = GetActivator<IBranch>(ctor);
                        try
                        {
                            Genrebr = createdActivator();
                            if (Genrebr.BranchType == EnumPointType.Genre)
                            {
                                int id = tree.SeqID;
                                Genrebr.Name = GenreBrAssembly.PackageName;
                                packagename = GenreBrAssembly.PackageName;
                                Genrebr.ID = id;
                                Genrebr.BranchID = id;
                                Genrebr.BranchText = GenreBrAssembly.classProperties.Caption;
                                Genrebr.DMEEditor = tree.DMEEditor;
                                tree.CreateNode(id, Genrebr);
                                //else CreateNode(id, Genrebr);
                                Genrebr.MiscStringID = GenreBrAssembly.GuidID;
                                tree.GenerBranchs.Add(new Tuple<IBranch, string>(Genrebr, GenreBrAssembly.classProperties.menu));
                            }
                        }
                        catch (Exception ex)
                        {
                            tree.DMEEditor.AddLogMessage("Error", $"Creating StandardTree Root Node {GenreBrAssembly.PackageName} {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);
                        }
                    }
                }
            }

            foreach (AssemblyClassDefinition cls in tree.DMEEditor.ConfigEditor.BranchesClasses.Where(p => p.classProperties != null).OrderBy(x => x.Order))
            {
                Type adc = tree.DMEEditor.assemblyHandler.GetType(cls.PackageName);
                ConstructorInfo ctor = adc.GetConstructors().Where(o => o.GetParameters().Length == 0).FirstOrDefault()!;
                tree.SeqID++;
                if (ctor != null)
                {
                    ObjectActivator<IBranch> createdActivator = GetActivator<IBranch>(ctor);
                    try
                    {
                        IBranch br = createdActivator();
                        if (br.BranchType == EnumPointType.Root)
                        {
                            int id = tree.SeqID;
                            br.Name = cls.PackageName;
                            packagename = cls.PackageName;
                            br.ID = id;
                            br.BranchID = id;
                            br.BranchText = cls.classProperties.Caption;
                            br.DMEEditor = tree.DMEEditor;
                            Genrebr.MiscStringID = cls.GuidID;
                            if (cls.classProperties.ObjectType != null)
                            {

                                var tr = tree.GenerBranchs.FirstOrDefault(p => p.Item2.Equals(cls.classProperties.menu, StringComparison.OrdinalIgnoreCase));
                                if (tr != null)
                                {
                                    Genrebr = tr.Item1;
                                }
                                else
                                    Genrebr = null;
                                if (Genrebr != null)
                                {
                                    Genrebr.ChildBranchs.Add(br);
                                    if (br.ObjectType != null && br.BranchClass != null)
                                    {
                                        //// Console.WriteLine($"{CreateNode}- br.BranchText");
                                        tree.CreateMenuMethods(br);
                                        tree.CreateGlobalMenu(br);

                                    }
                                    br.CreateChildNodes();

                                }
                                else
                                {
                                    br.ParentBranch = null;
                                    br.ParentBranchID = -1;
                                    br.ParentGuidID = string.Empty;
                                    tree.CreateNode(id, br);
                                    br.CreateChildNodes();
                                }

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        tree.DMEEditor.AddLogMessage("Error", $"Creating StandardTree Root Node {cls.PackageName} {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            tree.DMEEditor.ErrorObject.Ex = ex;
            tree.DMEEditor.ErrorObject.Flag = Errors.Failed;
            tree.DMEEditor.AddLogMessage("Error", $"Creating StandardTree Root Node {packagename} - {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);

        };


        return tree.Branches;
    }
    public static SimpleItem CreateNodeAndChilds(this ITree tree,  int id, IBranch br)
    {
       
        try
        {
            SimpleItem node = new SimpleItem();
            node.Text = br.BranchText;
            node.Name = br.Name;
            node.Id = id;
            node.BranchClass = br.BranchClass;
            node.BranchName = br.Name;
            node.PointType = br.BranchType;
            node.ObjectType = br.ObjectType;
            node.AssemblyClassDefinitionID = br.MiscStringID;
            node.ClassDefinitionID = br.MiscStringID;
            node.ImagePath = ImageListHelper.GetImagePathFromName(br.IconImageName);
            node.GuidId = br.GuidID;
            node.ParentID = 0;
            node.Children = new BindingList<SimpleItem>();
            node.Children = GetChildBranch(tree, br);
            DynamicMenuManager.CreateMenuMethods(tree.DMEEditor, br);
            if (br.ObjectType != null && br.BranchClass != null)
            {
                DynamicMenuManager.CreateGlobalMenu(tree.DMEEditor, br);
            }
            br.DMEEditor = tree.DMEEditor;
            if (!tree.DMEEditor.ConfigEditor.objectTypes.Any(i => i.ObjectType == br.BranchClass && i.ObjectName == br.BranchType.ToString() + "_" + br.BranchClass))
            {
                tree.DMEEditor.ConfigEditor.objectTypes.Add(new TheTechIdea.Beep.Workflow.ObjectTypes { ObjectType = br.BranchClass, ObjectName = br.BranchType.ToString() + "_" + br.BranchClass });
            }
            try
            {
                br.SetConfig(tree, tree.DMEEditor, br.ParentBranch, br.BranchText, br.ID, br.BranchType, null);
            }
            catch (Exception ex)
            {

            }
            tree.Branches.Add(br);
            br.CreateChildNodes();
            return node;
        }
        catch (Exception ex)
        {
            tree.DMEEditor.ErrorObject.Ex = ex;
            tree.DMEEditor.ErrorObject.Flag = Errors.Failed;
            tree.DMEEditor.AddLogMessage("Error", $"Creating Branch Node {br.BranchText} - {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);
            return null;
        }


    }
    public static SimpleItem CreateNode(this ITree tree, int id, IBranch br)
    {
      
        try
        {
            SimpleItem node = new SimpleItem();
            node.Text = br.BranchText;
            node.Name = br.Name;
            node.Id = id;
            node.BranchClass = br.BranchClass;
            node.BranchName = br.Name;
            node.ObjectType = br.ObjectType;
            node.PointType = br.BranchType;
            node.BranchType = br.BranchType;
            node.AssemblyClassDefinitionID = br.MiscStringID;
            node.ClassDefinitionID = br.MiscStringID;
            node.ImagePath = ImageListHelper.GetImagePathFromName(br.IconImageName);
            node.GuidId = br.GuidID;
            node.ParentID = 0;
            node.Children = new BindingList<SimpleItem>();
            node.Children = GetChildBranch(tree, br);
            DynamicMenuManager.CreateMenuMethods(tree.DMEEditor, br);
            if (br.ObjectType != null && br.BranchClass != null)
            {
                DynamicMenuManager.CreateGlobalMenu(tree.DMEEditor, br);
            }
            br.DMEEditor = tree.DMEEditor;
            if (!tree.DMEEditor.ConfigEditor.objectTypes.Any(i => i.ObjectType == br.BranchClass && i.ObjectName == br.BranchType.ToString() + "_" + br.BranchClass))
            {
                tree.DMEEditor.ConfigEditor.objectTypes.Add(new TheTechIdea.Beep.Workflow.ObjectTypes { ObjectType = br.BranchClass, ObjectName = br.BranchType.ToString() + "_" + br.BranchClass });
            }
            try
            {
                br.SetConfig(tree, tree.DMEEditor, br.ParentBranch, br.BranchText, br.ID, br.BranchType, null);
            }
            catch (Exception ex)
            {

            }
     //       tree.Branches.Add(br);
           // br.CreateChildNodes();
            return node;
        }
        catch (Exception ex)
        {
            tree.DMEEditor.ErrorObject.Ex = ex;
            tree.DMEEditor.ErrorObject.Flag = Errors.Failed;
            tree.DMEEditor.AddLogMessage("Error", $"Creating Branch Node {br.BranchText} - {ex.Message} ", DateTime.Now, 0, null, Errors.Failed);
            return null;
        }


    }
    public static bool IsMenuCreated(this ITree tree, IBranch br)
    {
        if (br.ObjectType != null)
        {
            return tree.Menus.Where(p => p.ObjectType != null && p.BranchClass.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase)
            && p.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase)
            && p.PointType == br.BranchType).Any();
        }
        return
            false;
    }
    public static void UpdateMenuItems(this ITree tree, MenuList menu, MenuItem menuItem)
    {
        int menuidx = tree.Menus.FindIndex(p => p.ObjectType != null && p.BranchClass.Equals(menu.BranchClass, StringComparison.InvariantCultureIgnoreCase)
                                  && p.ObjectType.Equals(menu.ObjectType, StringComparison.InvariantCultureIgnoreCase)
                                                                           && p.PointType == menu.PointType);
        if (menuidx > -1)
        {
            int menuitesidx = tree.Menus[menuidx].Items.FindIndex(p => p.Name.Equals(menuItem.Name, StringComparison.InvariantCultureIgnoreCase));
            if (menuitesidx > -1)
            {
                tree.Menus[menuidx].Items[menuitesidx] = menuItem;
            }

        }
    }
    public static void UpdateMenuList(this ITree tree, MenuList menu)
    {
        int menuidx = tree.Menus.FindIndex(p => p.ObjectType != null && p.BranchClass.Equals(menu.BranchClass, StringComparison.InvariantCultureIgnoreCase)
                       && p.ObjectType.Equals(menu.ObjectType, StringComparison.InvariantCultureIgnoreCase)
                                      && p.PointType == menu.PointType);

        if (menuidx > -1)
        {
            tree.Menus[menuidx] = menu;
        }
    }
    public static MenuList GetMenuList(this ITree tree, IBranch br)
    {

        return tree.Menus.Where(p => p.ObjectType != null && p.BranchClass.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase)
            && p.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase)
            && p.PointType == br.BranchType).FirstOrDefault();
    }
    public static List<MenuItem> GetMenuItemsList(this ITree tree, IBranch br)
    {
        List<MenuItem> retval = new List<MenuItem>();
        var ls = tree.Menus.Where(p => p.ObjectType != null && p.BranchClass.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase)
            && p.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase)
            && p.PointType == br.BranchType).FirstOrDefault();
        if (ls != null)
        {
            retval = ls.Items;
        }

        return retval;
    }
    public static List<SimpleItem> GetMenuItemsList(this ITree tree, string brGuidID)
    {
        IBranch br = tree.Branches.Where(p => p.GuidID == brGuidID).FirstOrDefault();
        List<SimpleItem> retval = new List<SimpleItem>();
        var ls = tree.Menus.Where(p => p.ObjectType != null && p.BranchClass.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase)
            && p.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase)
            && p.PointType == br.BranchType).FirstOrDefault();
       
        foreach (var item1 in ls.Items)
        {
            SimpleItem listitem = new SimpleItem { Text = item1.Text, ImagePath = item1.imagename, AssemblyClassDefinitionID = item1.ClassDefinitionID, GuidId = item1.ID };
            retval.Add(listitem);
        }
        return retval;
    }
    public static IErrorsInfo CreateGlobalMenu(this ITree tree,IBranch br)
    {
        try
        {
            MenuList menuList = new MenuList();
            if (!tree.IsMenuCreated(br))
            {
                menuList = new MenuList(br.ObjectType, br.BranchClass, br.BranchType);
                menuList.branchname = br.BranchText;
                tree.Menus.Add(menuList);
                menuList.ObjectType = br.ObjectType;
                menuList.BranchClass = br.BranchClass;
            }
            else
                menuList = tree.GetMenuList(br);
            List<AssemblyClassDefinition> extentions = tree.DMEEditor.ConfigEditor.GlobalFunctions.Where(o => o.classProperties != null && o.classProperties.ObjectType != null && o.classProperties.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase)).OrderBy(p => p.Order).ToList(); //&&  o.classProperties.menu.Equals(br.BranchClass, StringComparison.InvariantCultureIgnoreCase)
            foreach (AssemblyClassDefinition cls in extentions)
            {
                if (!menuList.classDefinitions.Any(p => p.PackageName.Equals(cls.PackageName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    menuList.classDefinitions.Add(cls);
                    foreach (var item in cls.Methods)
                    {
                        if (string.IsNullOrEmpty(item.ClassType))
                        {
                            if (item.PointType == br.BranchType)
                            {
                                MenuItem mi = new MenuItem();
                                mi.Name = item.Caption;
                                mi.MethodName = item.Caption;
                                mi.Text = item.Caption;
                                mi.ObjectType = item.ObjectType;
                                mi.BranchClass = item.ClassType;
                                mi.PointType = item.PointType;
                                mi.ClassDefinition = cls;
                                mi.MethodAttribute = item.CommandAttr;
                                mi.imagename = item.iconimage;
                                menuList.Items.Add(mi);
                            }
                        }
                        else
                        {
                            if ((item.PointType == br.BranchType) && (br.BranchClass.Equals(item.ClassType, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                MenuItem mi = new MenuItem();
                                mi.Name = item.Caption;
                                mi.MethodName = item.Name;
                                mi.Text = item.Caption;
                                mi.ObjectType = item.ObjectType;
                                mi.BranchClass = item.ClassType;
                                mi.PointType = item.PointType;
                                mi.ClassDefinition = cls;
                                mi.imagename = item.iconimage;
                                mi.MethodAttribute = item.CommandAttr;
                                menuList.Items.Add(mi);
                            }
                        }
                    }
                }
            }
            return tree.DMEEditor.ErrorObject;
        }
        catch (Exception ex)
        {
            return tree.DMEEditor.ErrorObject;
        }
    }
    public static List<MenuItem> CreateMenuMethods(this ITree tree, IBranch branch)
    {
        AssemblyClassDefinition cls = tree.DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.PackageName == branch.ToString()).FirstOrDefault();
        MenuList menuList = new MenuList();
        if (!tree.IsMenuCreated(branch))
        {
            menuList = new MenuList(branch.ObjectType, branch.BranchClass, branch.BranchType);
            menuList.branchname = branch.BranchText;
            tree.Menus.Add(menuList);
            menuList.ObjectType = branch.ObjectType;
            menuList.BranchClass = branch.BranchClass;

            menuList.Items = new List<MenuItem>();
        }
        else
            menuList = tree.GetMenuList(branch);
        try
        {

            if (!menuList.classDefinitions.Any(p => p.PackageName.Equals(cls.PackageName, StringComparison.InvariantCultureIgnoreCase)))
            {
                menuList.classDefinitions.Add(cls);
                foreach (var item in cls.Methods.Where(y => y.Hidden == false))
                {
                    MenuItem mi = new MenuItem();
                    mi.Name = item.Caption;
                    mi.MethodName = item.Caption;
                    mi.Text = item.Caption;
                    mi.ObjectType = item.ObjectType;
                    mi.BranchClass = item.ClassType;
                    mi.PointType = item.PointType;
                    mi.ClassDefinition = cls;
                    mi.Category = item.Category;
                    mi.imagename = item.iconimage;
                    mi.MethodAttribute = item.CommandAttr;

                    menuList.Items.Add(mi);
                }
            }
        }
        catch (Exception ex)
        {
            string mes = "Could not add method to menu " + branch.BranchText;
            tree.DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
        };
        return menuList.Items;
    }
    public static void Nodemenu_ItemClicked(this ITree tree, SimpleItem item,  string MethodName)
    {
        AssemblyClassDefinition cls = AssemblyDefinitionsHelper.GetAssemblyBranchsClassDefinitionByGuid(tree.DMEEditor, item.AssemblyClassDefinitionID);
        IBranch br = tree.Branches.Where(p => p.GuidID == item.GuidId).FirstOrDefault();
        if (cls != null)
        {
            if (!IsMethodApplicabletoNode(cls, br)) return;
            if (cls.componentType == "IFunctionExtension")
            {
                tree.RunFunctionFromExtensions(item,MethodName);

            }
            else
            {

                tree.RunMethodFromObject(br, item.Text);
            };

        }
    }
    public static Tuple<MenuList, bool> Nodemenu_MouseClick(this ITree tree, IBranch br, BeepMouseEventArgs e)
    {

        MenuList menuList = null;
        bool runmethod = false;
        if (br != null)
        {
            string clicks = "";
            if (e.Button == BeepMouseEventArgs.BeepMouseButtons.Right)
            {
                if (tree.IsMenuCreated(br))
                {
                    menuList = tree.GetMenuList(br);

                }
            }
            else
            {
                switch (e.Clicks)
                {
                    case 1:
                        clicks = "SingleClick";
                        break;
                    case 2:
                        clicks = "DoubleClick";
                        break;

                    default:
                        break;
                }
                AssemblyClassDefinition cls = tree.DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.PackageName == br.Name && x.Methods.Where(y => y.DoubleClick == true || y.Click == true).Any()).FirstOrDefault();
                if (cls != null)
                {
                    if (!IsMethodApplicabletoNode(cls, br)) runmethod = true;
                    tree.RunMethodFromBranch(br, clicks);

                }
            }

        }
        return new Tuple<MenuList, bool>(menuList, runmethod);
    }
    public static IErrorsInfo RunMethodFromBranch(this ITree tree, object branch, string MethodName)
    {

        try
        {
            Type t = branch.GetType();
            AssemblyClassDefinition cls = tree.DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.className == t.Name).FirstOrDefault();
            MethodInfo method = null;
            MethodsClass methodsClass;
            try
            {
                methodsClass = cls.Methods.Where(x => x.Caption == MethodName).FirstOrDefault();
            }
            catch (Exception)
            {

                methodsClass = null;
            }
            if (methodsClass != null)
            {

                if (!IsMethodApplicabletoNode(cls, (IBranch)branch)) return tree.DMEEditor.ErrorObject;
                //PassedArgs args = new PassedArgs();
                //args.ObjectName = MethodName;
                //args.ObjectType = methodsClass.ObjectType;
                //args.Cancel = false;
                //PreCallModule?.Invoke(this, args);
                //if (args.Cancel)
                //{
                //    DMEEditor.AddLogMessage("Beep", $"You dont have Access Privilige on {MethodName}", DateTime.Now, 0, MethodName, Errors.Failed);
                //    ErrorsandMesseges.Flag = Errors.Failed;
                //    ErrorsandMesseges.Message = $"Function Access Denied";
                //    return ErrorsandMesseges;
                //}

                method = methodsClass.Info;
                if (method.GetParameters().Length > 0)
                {
                    method.Invoke(branch, new object[] { tree.DMEEditor.Passedarguments.Objects[0].obj });
                }
                else
                    method.Invoke(branch, null);


                //  DMEEditor.AddLogMessage("Success", "Running method", DateTime.Now, 0, null, Errors.Ok);
            }

        }
        catch (Exception ex)
        {
            string mes = "Could not Run Method " + MethodName;
            tree.DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
        };
        return tree.DMEEditor.ErrorObject;
    }
    public static ContextMenuStrip CreateContextMenu(this ITree tree, IBranch br)
    {
        ContextMenuStrip nodemenu = new ContextMenuStrip();
        try
        {
            if (br != null)
            {
                if (tree.IsMenuCreated(br))
                {
                    MenuList menuList = tree.GetMenuList(br);
                    foreach (MenuItem item in menuList.Items)
                    {
                        ToolStripItem st = nodemenu.Items.Add(item.Text);
                        st.Tag = item;
                       // st.Click += tree.Nodemenu_ItemClicked;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            string mes = $"Could not add method from Extension {br.Name} to menu ";
            tree.DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
        };
        return nodemenu;
    }
    #endregion "ITree Extensions"
    public static IErrorsInfo CreateFunctionExtensions(this ITree tree,MethodsClass item)
    {
        ContextMenuStrip nodemenu = new ContextMenuStrip();
        try
        {
           
            ToolStripItem st = nodemenu.Items.Add(item.Caption);
            foreach (IBranch br in tree.Branches)
            {
                if (br.BranchType == item.PointType)
                {
                    nodemenu.Name = br.ToString();
                    if (item.iconimage != null)
                    {
                //st.ImageIndex = VisManager.visHelper.GetImageIndex(item.iconimage);
                    }
                    //nodemenu.ItemClicked += Nodemenu_ItemClicked;
                    nodemenu.Tag = br;
                }
            }
        }
        catch (Exception ex)
        {
            string mes = $"Could not add method from Extension {item.Name} to menu ";
            tree.DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
        };
        return tree.DMEEditor.ErrorObject;

    }
    private static Control GetControlCorrectPositiononForm(Control control)
    {
        if (control == null)
            throw new ArgumentNullException(nameof(control));

        // Ensure the control belongs to a form
        Form form = control.FindForm();
        if (form == null)
            throw new InvalidOperationException("The control does not belong to a form.");

        // Start with the control's position and iterate through all parent controls
        Point positionOnForm = control.Location;
        Control parent = control.Parent;

        while (parent != null && parent != form)
        {
            positionOnForm.Offset(parent.Location);
            parent = parent.Parent;
        }

        // At this point, positionOnForm contains the correct position on the form

        // Create a new control with the same size and correct location
        Control newControl = new Control
        {
            Size = control.Size,
            Location = positionOnForm
        };

        return newControl;
    }
    private static Point GetAdjustmentPoint(Control baseControl, Control targetControl)
    {
        if (baseControl == null)
            throw new ArgumentNullException(nameof(baseControl));
        if (targetControl == null)
            throw new ArgumentNullException(nameof(targetControl));

        // Ensure both controls belong to a form
        Form baseForm = baseControl.FindForm();
        Form targetForm = targetControl.FindForm();
        if (baseForm == null || targetForm == null || baseForm != targetForm)
            throw new InvalidOperationException("Both controls must belong to the same form.");

        // Get the positions of both controls relative to the form
        Point baseLocation = GetControlCorrectPointPositiononForm(baseControl);
        Point targetLocation = GetControlCorrectPointPositiononForm(targetControl);

        // Calculate the adjustment point
        return new Point(targetLocation.X - baseLocation.X, targetLocation.Y - baseLocation.Y);
    }
    public static IBeepUIComponent CreateFieldBasedOnCategory(string propertyName, Type propertyType)
    {
        DbFieldCategory category = MapTypeToDbFieldCategory(propertyType);

        switch (category)
        {
            case DbFieldCategory.String:
                return new BeepTextBox { BoundProperty = propertyName, ComponentName = propertyName };

            case DbFieldCategory.Numeric:
            case DbFieldCategory.Currency:
                return new BeepNumericUpDown { BoundProperty = propertyName, ComponentName = propertyName };

            case DbFieldCategory.Date:
            case DbFieldCategory.Timestamp:
                return new BeepDatePicker { BoundProperty = propertyName, ComponentName = propertyName };

            case DbFieldCategory.Boolean:
                return new BeepCheckBoxBool { BoundProperty = propertyName, ComponentName = propertyName };

            case DbFieldCategory.Guid:
                return new BeepTextBox { BoundProperty = propertyName, ComponentName = propertyName, ReadOnly = true };

            case DbFieldCategory.Json:
            case DbFieldCategory.Xml:
                return new BeepTextBox { BoundProperty = propertyName, ComponentName = propertyName };

            default:
                return new BeepTextBox { BoundProperty = propertyName, ComponentName = propertyName };
        }
    }
    public static DbFieldCategory MapTypeToDbFieldCategory(Type type)
    {
        if (type == typeof(string))
            return DbFieldCategory.String;
        if (type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(decimal))
            return DbFieldCategory.Numeric;
        if (type == typeof(DateTime))
            return DbFieldCategory.Date;
        if (type == typeof(bool))
            return DbFieldCategory.Boolean;
        if (type == typeof(Guid))
            return DbFieldCategory.Guid;
        if (type == typeof(byte[]))
            return DbFieldCategory.Binary;
        if (type == typeof(System.Xml.XmlDocument) || type == typeof(System.Xml.Linq.XElement))
            return DbFieldCategory.Xml;
        if (type == typeof(System.Text.Json.JsonDocument))
            return DbFieldCategory.Json;
        if (type.IsEnum)
            return DbFieldCategory.Enum;

        return DbFieldCategory.Complex;
    }
    private static  Point GetControlCorrectPointPositiononForm(Control control)
    {
        if (control == null)
            throw new ArgumentNullException(nameof(control));

        // Ensure the control belongs to a form
        Form form = control.FindForm();
        if (form == null)
            throw new InvalidOperationException("The control does not belong to a form.");

        // Start with the control's position and iterate through all parent controls
        Point positionOnForm = control.Location;
        Control parent = control.Parent;

        while (parent != null && parent != form)
        {
            positionOnForm.Offset(parent.Location);
            parent = parent.Parent;
        }

        // Return the calculated position on the form
        return positionOnForm;
    }
}


