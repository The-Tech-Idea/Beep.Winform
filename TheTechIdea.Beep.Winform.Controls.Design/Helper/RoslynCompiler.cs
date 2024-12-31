using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
namespace TheTechIdea.Beep.Winform.Controls.Design.Helper
{
    public static class RoslynCompiler
    {
        public static Assembly CompileSourceFilesToMemory(List<string> sourceFiles)
        {
            if (sourceFiles == null || sourceFiles.Count == 0)
                throw new ArgumentException("No source files provided for compilation.");

            var syntaxTrees = new List<SyntaxTree>();
            foreach (var sourceFile in sourceFiles)
            {
                if (!File.Exists(sourceFile))
                    throw new FileNotFoundException($"Source file '{sourceFile}' not found.");

                var sourceText = File.ReadAllText(sourceFile);
                var syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
                syntaxTrees.Add(syntaxTree);
            }

            // Add necessary references
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToList();

            // Create a Roslyn compilation
            var compilation = CSharpCompilation.Create(
                "InMemoryAssembly",
                syntaxTrees: syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using (var memoryStream = new MemoryStream())
            {
                EmitResult emitResult = compilation.Emit(memoryStream);

                if (!emitResult.Success)
                {
                    // Handle compilation errors
                    var diagnostics = string.Join(Environment.NewLine,
                        emitResult.Diagnostics
                        .Where(d => d.Severity == DiagnosticSeverity.Error)
                        .Select(d => d.ToString()));

                    throw new InvalidOperationException($"Compilation failed with errors:\n{diagnostics}");
                }

                // Load the assembly from the memory stream
                memoryStream.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(memoryStream.ToArray());
            }
        }
    }

}
