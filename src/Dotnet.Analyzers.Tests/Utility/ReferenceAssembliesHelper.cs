using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis.Testing;

namespace Dotnet.Analyzers.Tests.Utility;

internal static class ReferenceAssembliesHelper
{
    public static readonly ReferenceAssemblies CurrentAkka;

#pragma warning disable CA1810
    static ReferenceAssembliesHelper()
#pragma warning restore CA1810
    {
        var defaultAssemblies =
            new ReferenceAssemblies(
                "net10.0",
                new PackageIdentity("Microsoft.NETCore.App.Ref", "10.0.0"),
                Path.Combine("ref", "net10.0")
            );

        // TODO: does this bring all other transitive dependencies?
        CurrentAkka = defaultAssemblies.AddPackages(
            ImmutableArray<PackageIdentity>.Empty
        );
    }
}