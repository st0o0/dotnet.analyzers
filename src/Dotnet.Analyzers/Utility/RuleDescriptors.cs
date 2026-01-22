using Microsoft.CodeAnalysis;

namespace Dotnet.Analyzers.Utility;

public static class RuleDescriptors
{
    private static DiagnosticDescriptor Rule(
        string id,
        string title,
        AnalysisCategory category,
        DiagnosticSeverity defaultSeverity,
        string messageFormat)
    {
        var helpLink = "" + id.ToUpperInvariant() + "";

        return new DiagnosticDescriptor(id, title, messageFormat, category.ToString(), defaultSeverity,
            true, helpLinkUri: helpLink);
    }
}