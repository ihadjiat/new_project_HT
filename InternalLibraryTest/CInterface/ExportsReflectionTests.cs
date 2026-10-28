using System.Reflection;
using System.Runtime.InteropServices;
using CInterfaceNative;
using FluentAssertions;

namespace HTA.CableSizing.Tests.CInterface;

public class ExportsReflectionTests
{
    [Fact]
    public void ExportsDeclaresTheSixExpectedEntryPoints()
    {
        var entryPoints = typeof(Exports)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Select(method => method.GetCustomAttribute<UnmanagedCallersOnlyAttribute>())
            .Where(attribute => attribute is not null)
            .Select(attribute => attribute!.EntryPoint)
            .ToArray();

        entryPoints.Should().BeEquivalentTo(
        [
            "hta_calculate_global_factor",
            "hta_calculate_allowable_current",
            "hta_calculate_minimum_section",
            "hta_get_warning",
            "hta_get_factor",
            "hta_get_factor_explanation",
        ]);
    }
}
