using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace TodoCommentReporter
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TodoCommentReporterAnalyzer : DiagnosticAnalyzer
    {
        private static DiagnosticDescriptor Rule { get; } = new DiagnosticDescriptor("TD01", "TODO", "{0}", "Todo.Design", DiagnosticSeverity.Info, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxTreeAction(c =>
            {
                foreach (var todoComment in c.Tree.GetRoot().DescendantTrivia().Where(x => (x.IsKind(SyntaxKind.SingleLineCommentTrivia) || x.IsKind(SyntaxKind.MultiLineCommentTrivia)) && new string(x.ToString().SkipWhile(ch => !char.IsLetterOrDigit(ch)).ToArray()).StartsWith("TODO", StringComparison.OrdinalIgnoreCase)))
                    c.ReportDiagnostic(Diagnostic.Create(Rule, todoComment.GetLocation(), $"TODO: {todoComment.ToString().Substring(todoComment.ToString().IndexOf("TODO", StringComparison.OrdinalIgnoreCase) + 5).Trim()}"));
            });
        }
    }
}
