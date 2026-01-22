using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dotnet.Analyzers.Utility;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Dotnet.Analyzers.Tests.Utility;

/**
 * Code inspired by https://github.com/xunit/xunit.analyzers/blob/main/src/xunit.analyzers.tests/Utility/CSharpVerifier.cs
 */
[SuppressMessage("Design", "CA1000:Do not declare static members on generic types")]
public sealed class Verifier<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
{
    public static DiagnosticResult Diagnostic()
    {
        return CSharpCodeFixVerifier<TAnalyzer, EmptyCodeFixProvider, DefaultVerifier>.Diagnostic();
    }

    public static DiagnosticResult Diagnostic(string id)
    {
        return CSharpCodeFixVerifier<TAnalyzer, EmptyCodeFixProvider, DefaultVerifier>.Diagnostic(id);
    }

    public static Task VerifyAnalyzer(string source, params DiagnosticResult[] diagnostics)
    {
        return VerifyAnalyzer([source], diagnostics);
    }

    public static Task VerifyAnalyzer(string[] sources, params DiagnosticResult[] diagnostics)
    {
        Guard.AssertIsNotNull(sources);

        var test = new Test();
#pragma warning disable CA1062
        foreach (var source in sources)
        {
            test.TestState.Sources.Add(source);
        }
#pragma warning restore CA1062

        test.ExpectedDiagnostics.AddRange(diagnostics);
        return test.RunAsync();
    }

    public static Task VerifyCodeFix(string before, string after, string fixerActionKey,
        params DiagnosticResult[] diagnostics)
    {
        Guard.AssertIsNotNull(before);
        Guard.AssertIsNotNull(after);

        var test = new Test
        {
            TestCode = before,
            FixedCode = after,
            CodeActionEquivalenceKey = fixerActionKey
        };
        test.TestState.ExpectedDiagnostics.AddRange(diagnostics);
        return test.RunAsync();
    }

    public static Task VerifyCodeFix(
        string before,
        string after,
        string fixerActionKey,
        DiagnosticResult[] diagnostics,
        DiagnosticResult[] fixedDiagnostics)
    {
        Guard.AssertIsNotNull(before);
        Guard.AssertIsNotNull(after);

        var test = new Test
        {
            TestCode = before,
            FixedCode = after,
            CodeActionEquivalenceKey = fixerActionKey,
        };
        test.TestState.ExpectedDiagnostics.AddRange(diagnostics);
        test.FixedState.ExpectedDiagnostics.AddRange(fixedDiagnostics);
        return test.RunAsync();
    }

    public static Task VerifyCodeFix(
        string before,
        string after,
        string fixerActionKey,
        int incrementalIterations,
        CodeFixTestBehaviors codeFixBehaviors,
        DiagnosticResult[] diagnostics,
        DiagnosticResult[] fixedDiagnostics)
    {
        Guard.AssertIsNotNull(before);
        Guard.AssertIsNotNull(after);

        var test = new Test
        {
            TestCode = before,
            FixedCode = after,
            CodeActionEquivalenceKey = fixerActionKey,
            NumberOfIncrementalIterations = incrementalIterations,
            CodeFixTestBehaviors = codeFixBehaviors
        };
        test.TestState.ExpectedDiagnostics.AddRange(diagnostics);
        test.FixedState.ExpectedDiagnostics.AddRange(fixedDiagnostics);
        return test.RunAsync();
    }

    private sealed class Test() : TestBase(ReferenceAssembliesHelper.CurrentAkka);

    private class TestBase : CSharpCodeFixTest<TAnalyzer, EmptyCodeFixProvider, DefaultVerifier>
    {
        protected TestBase(ReferenceAssemblies referenceAssemblies)
        {
            ReferenceAssemblies = referenceAssemblies;
            TestBehaviors |= TestBehaviors.SkipGeneratedCodeCheck;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        }

        protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
        {
            var analyzer = new TAnalyzer();
            foreach (var provider in CodeFixProviderDiscovery.GetCodeFixProviders(Language))
            {
                if (analyzer.SupportedDiagnostics.Any(diagnostic =>
                        provider.FixableDiagnosticIds.Contains(diagnostic.Id)))
                {
                    yield return provider;
                }
            }
        }
    }
}