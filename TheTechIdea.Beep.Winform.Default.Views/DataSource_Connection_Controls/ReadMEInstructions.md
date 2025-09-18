
1- This is the instruction to create new Control for Beep Framework Data Sources Connection property for each type of connection of Data Source
2- These Controls are built using Windows Forms, and use the same pattern used in uc_RDBMSConnections control
3- These Control will TheTechIdea.Beep.Winforms.Controls
4- These Controls will use UnitofWork and Repository pattern to interact with Data
5- The Unitofwork examples is in TheTechIdea.Beep.MVVM project 
6- Create new ViewModel for the control in TheTechIdea.Beep.MVVM project
7- All Data Source Defined in projct DataManagementEngine:  TheTechIdea.Beep.Helpers ConnectionHelper.cs
8- All Models are in DataManagementModels project.you dont need to create new model if existing model is available
7- Create new DTO Model (if needed) for the control in TheTechIdea.Beep.MVVM project
8- Create new View for the control in TheTechIdea.Beep.Winform.Default.Views project  and in Configuration/DataSource_Connection_Controls folder
9- Create a Plan and Progress Files in Folder, update them accordingly.
10 - Dont edit or update DataManagementengine project .
11- Dont edit or update DataManagementModels project .