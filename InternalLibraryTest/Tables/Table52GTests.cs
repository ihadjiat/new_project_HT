using HTA.CableSizing.Enums;
using HTA.CableSizing.Tables;
using Xunit;

namespace HTA.CableSizing.Tests.Tables;

public sealed class Table52GTests
{
    private static readonly double[] ExpectedCrossSections = { 16, 25, 35, 50, 70, 95, 120, 150, 185, 240, 300 };

    /// <summary>Returns the reference table number.</summary>
    [Fact]
    public void TableNumberIs52G()
    {
        Assert.Equal("52G", new Table52G().TableNumber);
    }

    /// <summary>Returns every reference current from table 52G regardless of arrangement type.</summary>
    [Theory]
    [InlineData(16, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 120)]
    [InlineData(16, InstallationEnvironment.Buried, ConductorMaterial.Copper, 126)]
    [InlineData(16, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 94)]
    [InlineData(16, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 98)]
    [InlineData(25, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 156)]
    [InlineData(25, InstallationEnvironment.Buried, ConductorMaterial.Copper, 160)]
    [InlineData(25, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 121)]
    [InlineData(25, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 125)]
    [InlineData(35, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 189)]
    [InlineData(35, InstallationEnvironment.Buried, ConductorMaterial.Copper, 192)]
    [InlineData(35, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 146)]
    [InlineData(35, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 148)]
    [InlineData(50, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 227)]
    [InlineData(50, InstallationEnvironment.Buried, ConductorMaterial.Copper, 226)]
    [InlineData(50, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 175)]
    [InlineData(50, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 175)]
    [InlineData(70, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 282)]
    [InlineData(70, InstallationEnvironment.Buried, ConductorMaterial.Copper, 277)]
    [InlineData(70, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 220)]
    [InlineData(70, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 215)]
    [InlineData(95, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 345)]
    [InlineData(95, InstallationEnvironment.Buried, ConductorMaterial.Copper, 331)]
    [InlineData(95, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 268)]
    [InlineData(95, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 257)]
    [InlineData(120, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 400)]
    [InlineData(120, InstallationEnvironment.Buried, ConductorMaterial.Copper, 377)]
    [InlineData(120, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 309)]
    [InlineData(120, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 292)]
    [InlineData(150, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 456)]
    [InlineData(150, InstallationEnvironment.Buried, ConductorMaterial.Copper, 423)]
    [InlineData(150, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 351)]
    [InlineData(150, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 327)]
    [InlineData(185, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 523)]
    [InlineData(185, InstallationEnvironment.Buried, ConductorMaterial.Copper, 478)]
    [InlineData(185, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 407)]
    [InlineData(185, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 372)]
    [InlineData(240, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 619)]
    [InlineData(240, InstallationEnvironment.Buried, ConductorMaterial.Copper, 554)]
    [InlineData(240, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 479)]
    [InlineData(240, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 431)]
    [InlineData(300, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 718)]
    [InlineData(300, InstallationEnvironment.Buried, ConductorMaterial.Copper, 627)]
    [InlineData(300, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 552)]
    [InlineData(300, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 487)]
    public void ReturnsExpectedBaseCurrentForEveryArrangement(double crossSectionMm2, InstallationEnvironment environment, ConductorMaterial conductorMaterial, double expectedCurrentAmps)
    {
        var table = new Table52G();
        ArrangementType?[] arrangements = { null, ArrangementType.Unspecified, ArrangementType.Trefoil, ArrangementType.Flat };

        foreach (var arrangement in arrangements)
        {
            Assert.True(table.TryGetBaseCurrent(crossSectionMm2, environment, conductorMaterial, arrangement, out var currentAmps));
            Assert.Equal(expectedCurrentAmps, currentAmps);
        }
    }

    /// <summary>Returns the exact available sections for each supported environment and conductor material.</summary>
    [Fact]
    public void AvailableCrossSectionsMatchTable52G()
    {
        var table = new Table52G();

        foreach (var environment in new[] { InstallationEnvironment.FreeAir, InstallationEnvironment.Buried })
        {
            foreach (var conductorMaterial in new[] { ConductorMaterial.Copper, ConductorMaterial.Aluminum })
            {
                var crossSections = table.GetAvailableCrossSections(environment, conductorMaterial, null);
                Assert.Equal(ExpectedCrossSections, crossSections);
                Assert.IsAssignableFrom<IList<double>>(crossSections);
                Assert.True(((IList<double>)crossSections).IsReadOnly);
                Assert.Throws<NotSupportedException>(() => ((IList<double>)crossSections).Add(400));
            }
        }
    }

    /// <summary>Returns no cross-sections for an unsupported environment.</summary>
    [Fact]
    public void AvailableCrossSectionsAreEmptyForAnUnsupportedEnvironment()
    {
        var table = new Table52G();

        Assert.Empty(table.GetAvailableCrossSections((InstallationEnvironment)99, ConductorMaterial.Copper, null));
    }

    /// <summary>Returns false and zero for a cross-section not tabulated in table 52G.</summary>
    [Fact]
    public void ReturnsFalseAndZeroForAnUnlistedCrossSection()
    {
        var table = new Table52G();

        Assert.False(table.TryGetBaseCurrent(10, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, null, out var currentAmps));
        Assert.Equal(0, currentAmps);
    }
}
