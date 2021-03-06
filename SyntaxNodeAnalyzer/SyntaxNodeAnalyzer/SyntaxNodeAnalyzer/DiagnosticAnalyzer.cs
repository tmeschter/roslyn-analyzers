using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace SyntaxNodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SyntaxNodeAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string spacingRuleId = "IfSpacing";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, //make the id specific
            title: "If statement must have a space between 'if' and the boolean expression", //allow any title
            messageFormat: "If statements must contain a space between the 'if' keyword and the boolean expression", //allow any message
            category: "Syntax", //make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, //possible options
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(Rule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext obj)
        {
            var ifStatement = (IfStatementSyntax)obj.Node;
            var ifKeyword = ifStatement.IfKeyword;
            var openParen = ifStatement.OpenParenToken;
            var diagnosticLocation = Location.Create(ifStatement.SyntaxTree, TextSpan.FromBounds(ifKeyword.Span.Start, openParen.Span.Start));

            if (ifKeyword.HasTrailingTrivia)
            {
                var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                    if (trailingTrivia.ToString() == " ")
                    {
                        return;
                    }
                }
            }

            var diagnostic = Diagnostic.Create(Rule, diagnosticLocation, Rule.MessageFormat);
            obj.ReportDiagnostic(diagnostic);
        }
    }
}
