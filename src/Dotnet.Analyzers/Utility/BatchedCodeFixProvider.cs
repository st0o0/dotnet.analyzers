using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Dotnet.Analyzers.Utility;

public abstract class BatchedCodeFixProvider(params string[] diagnostics) : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = [..diagnostics];

    public sealed override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }
}