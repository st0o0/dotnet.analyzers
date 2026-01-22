using System;

namespace Dotnet.Analyzers.Utility;

public static class Guard
{
    public static T AssertIsNotNull<T>(T? arg) where T : class
    {
        return arg ?? throw new ArgumentNullException(nameof(arg));
    }
}