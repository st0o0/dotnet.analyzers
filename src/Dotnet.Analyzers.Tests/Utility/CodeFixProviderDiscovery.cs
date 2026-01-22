using System.Collections.Immutable;
using Dotnet.Analyzers.Fixes.Utility;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.VisualStudio.Composition;

namespace Dotnet.Analyzers.Tests.Utility;

/**
 * Code copied from https://github.com/xunit/xunit.analyzers/blob/main/src/xunit.analyzers.tests/Utility/CodeFixProviderDiscovery.cs
 */

internal static class CodeFixProviderDiscovery
{
    private static readonly Lazy<IExportProviderFactory> ExportProviderFactory = new(
        () =>
        {
            var discovery = new AttributedPartDiscovery(Resolver.DefaultInstance, true);
            var parts = Task.Run(() => discovery.CreatePartsAsync(typeof(BatchedCodeFixProvider).Assembly)).GetAwaiter()
                .GetResult();
            var catalog = ComposableCatalog.Create(Resolver.DefaultInstance).AddParts(parts);

            var configuration = CompositionConfiguration.Create(catalog);
            var runtimeComposition = RuntimeComposition.CreateRuntimeComposition(configuration);
            return runtimeComposition.CreateExportProviderFactory();
        },
        LazyThreadSafetyMode.ExecutionAndPublication
    );

    public static IEnumerable<CodeFixProvider> GetCodeFixProviders(string language)
    {
        var exportProvider = ExportProviderFactory.Value.CreateExportProvider();
        var exports = exportProvider.GetExports<CodeFixProvider, LanguageMetadata>();

        return exports.Where(export => export.Metadata.Languages.Contains(language)).Select(export => export.Value);
    }

    private sealed class LanguageMetadata
    {
        public LanguageMetadata(IDictionary<string, object> data)
        {
            if (!data.TryGetValue(nameof(ExportCodeFixProviderAttribute.Languages), out var languages))
                languages = Array.Empty<string>();

            Languages = [..(string[])languages];
        }

        public ImmutableArray<string> Languages { get; }
    }
}