using HTA.CableSizing.Enums;
using HTA.CableSizing.Tables;
using Xunit;

namespace HTA.CableSizing.Tests.Tables;

/// <summary>Tests the reference current values and contract behavior of table 52I.</summary>
public sealed class Table52ITests
{
    private static readonly double[] ExpectedCrossSections = { 10, 16, 25, 35, 50, 70, 95, 120, 150, 185, 240 };

    /// <summary>Returns the reference table number.</summary>
    [Fact]
    public void TableNumberIs52I()
    {
        Assert.Equal("52I", new Table52I().TableNumber);
    }

    /// <summary>Returns every reference current from table 52I regardless of arrangement type.</summary>
    [Theory]
    [InlineData(10, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 62)]
    [InlineData(10, InstallationEnvironment.Buried, ConductorMaterial.Copper, 70)]
    [InlineData(10, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 48)]
    [InlineData(10, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 56)]
    [InlineData(16, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 81)]
    [InlineData(16, InstallationEnvironment.Buried, ConductorMaterial.Copper, 94)]
    [InlineData(16, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 62)]
    [InlineData(16, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 72)]
    [InlineData(25, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 105)]
    [InlineData(25, InstallationEnvironment.Buried, ConductorMaterial.Copper, 120)]
    [InlineData(25, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 82)]
    [InlineData(25, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 94)]
    [InlineData(35, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 130)]
    [InlineData(35, InstallationEnvironment.Buried, ConductorMaterial.Copper, 145)]
    [InlineData(35, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 100)]
    [InlineData(35, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 115)]
    [InlineData(50, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 165)]
    [InlineData(50, InstallationEnvironment.Buried, ConductorMaterial.Copper, 185)]
    [InlineData(50, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 130)]
    [InlineData(50, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 145)]
    [InlineData(70, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 205)]
    [InlineData(70, InstallationEnvironment.Buried, ConductorMaterial.Copper, 225)]
    [InlineData(70, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 160)]
    [InlineData(70, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 175)]
    [InlineData(95, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 250)]
    [InlineData(95, InstallationEnvironment.Buried, ConductorMaterial.Copper, 270)]
    [InlineData(95, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 195)]
    [InlineData(95, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 210)]
    [InlineData(120, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 290)]
    [InlineData(120, InstallationEnvironment.Buried, ConductorMaterial.Copper, 310)]
    [InlineData(120, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 225)]
    [InlineData(120, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 240)]
    [InlineData(150, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 330)]
    [InlineData(150, InstallationEnvironment.Buried, ConductorMaterial.Copper, 345)]
    [InlineData(150, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 255)]
    [InlineData(150, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 270)]
    [InlineData(185, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 370)]
    [InlineData(185, InstallationEnvironment.Buried, ConductorMaterial.Copper, 385)]
    [InlineData(185, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 285)]
    [InlineData(185, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 300)]
    [InlineData(240, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 440)]
    [InlineData(240, InstallationEnvironment.Buried, ConductorMaterial.Copper, 445)]
    [InlineData(240, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 345)]
    [InlineData(240, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 350)]
    public void ReturnsExpectedBaseCurrentForEveryArrangement(double crossSectionMm2, InstallationEnvironment environment, ConductorMaterial conductorMaterial, double expectedCurrentAmps)
    {
        var table = new Table52I();
        ArrangementType?[] arrangements = { null, ArrangementType.Unspecified, ArrangementType.Trefoil, ArrangementType.Flat };

        foreach (var arrangement in arrangements)
        {
            Assert.True(table.TryGetBaseCurrent(crossSectionMm2, environment, conductorMaterial, arrangement, out var currentAmps));
            Assert.Equal(expectedCurrentAmps, currentAmps);
        }
    }

    /// <summary>Returns the exact available sections for each supported environment and conductor material.</summary>
    [Fact]
    public void AvailableCrossSectionsMatchTable52I()
    {
        var table = new Table52I();

        foreach (var environment in new[] { InstallationEnvironment.FreeAir, InstallationEnvironment.Buried })
        {
            foreach (var conductorMaterial in new[] { ConductorMaterial.Copper, ConductorMaterial.Aluminum })
            {
                var crossSections = table.GetAvailableCrossSections(environment, conductorMaterial, null);
                Assert.Equal(ExpectedCrossSections, crossSections);
                Assert.IsAssignableFrom<IList<double>>(crossSections);
                Assert.True(((IList<double>)crossSections).IsReadOnly);
                Assert.Throws<NotSupportedException>(() => ((IList<double>)crossSections).Add(300));
            }
        }
    }

    /// <summary>Returns false and zero for a cross-section not tabulated in table 52I.</summary>
    [Fact]
    public void ReturnsFalseAndZeroForAnUnlistedCrossSection()
    {
        var table = new Table52I();

        Assert.False(table.TryGetBaseCurrent(300, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, null, out var currentAmps));
        Assert.Equal(0, currentAmps);
    }

    /// <summary>Returns false and zero for an unsupported configuration.</summary>
    [Fact]
    public void ReturnsFalseAndZeroForAnUnsupportedConfiguration()
    {
        var table = new Table52I();

        Assert.False(table.TryGetBaseCurrent(10, (InstallationEnvironment)99, ConductorMaterial.Copper, null, out var currentAmps));
        Assert.Equal(0, currentAmps);
        Assert.False(table.TryGetBaseCurrent(10, InstallationEnvironment.FreeAir, (ConductorMaterial)99, null, out currentAmps));
        Assert.Equal(0, currentAmps);
        Assert.Empty(table.GetAvailableCrossSections((InstallationEnvironment)99, ConductorMaterial.Copper, null));
        Assert.Empty(table.GetAvailableCrossSections(InstallationEnvironment.FreeAir, (ConductorMaterial)99, null));
    }

    /// <summary>Returns no cross-sections for an unsupported environment.</summary>
    [Fact]
    public void AvailableCrossSectionsAreEmptyForAnUnsupportedEnvironment()
    {
        var table = new Table52I();

        Assert.Empty(table.GetAvailableCrossSections((InstallationEnvironment)99, ConductorMaterial.Copper, null));
    }
}
