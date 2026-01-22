using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Dotnet.Analyzers.Fixes.Utility;

public abstract class BatchedCodeFixProvider(params string[] diagnostics) : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = diagnostics.ToImmutableArray();

    public sealed override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }
}