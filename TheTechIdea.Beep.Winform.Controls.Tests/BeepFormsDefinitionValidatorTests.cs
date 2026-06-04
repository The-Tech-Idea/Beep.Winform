using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models;
using Xunit;

namespace TheTechIdea.Beep.Winform.Controls.Tests
{
    public class BeepFormsDefinitionValidatorTests
    {
        [Fact]
        public void Validate_NullDefinition_ReportsSingleWarning()
        {
            var issues = BeepFormsDefinitionValidator.Validate(null);
            Assert.Single(issues);
            Assert.Equal(BeepFormsValidationSeverity.Warning, issues[0].Severity);
        }

        [Fact]
        public void Validate_EmptyDefinition_ReportsNameWarningAndTitleInfo()
        {
            var issues = BeepFormsDefinitionValidator.Validate(new BeepFormsDefinition());
            Assert.Contains(issues, i => i.Severity == BeepFormsValidationSeverity.Warning && i.Message.Contains("Form Name"));
            Assert.Contains(issues, i => i.Severity == BeepFormsValidationSeverity.Info && i.Message.Contains("Title"));
        }

        [Fact]
        public void Validate_DuplicateBlockNames_ReportsError()
        {
            var definition = new BeepFormsDefinition
            {
                FormName = "FormA",
                Blocks =
                {
                    new TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models.BeepBlockDefinition { BlockName = "B1" },
                    new TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models.BeepBlockDefinition { BlockName = "B1" }
                }
            };
            var issues = BeepFormsDefinitionValidator.Validate(definition);
            Assert.Contains(issues, i => i.Severity == BeepFormsValidationSeverity.Error && i.Message.Contains("Duplicate"));
        }

        [Fact]
        public void Validate_BlockWithoutEntity_ReportsWarning()
        {
            var definition = new BeepFormsDefinition
            {
                FormName = "FormA",
                Blocks =
                {
                    new TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models.BeepBlockDefinition { BlockName = "B1" }
                }
            };
            var issues = BeepFormsDefinitionValidator.Validate(definition);
            Assert.Contains(issues, i => i.Severity == BeepFormsValidationSeverity.Warning && i.BlockName == "B1");
        }
    }
}
