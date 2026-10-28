using HTA.CableSizing.Enums;
using HTA.CableSizing.Tables;
using Xunit;

namespace HTA.CableSizing.Tests.Tables;

public sealed class Table52HTests
{
    private static readonly double[] ExpectedCrossSections = { 10, 16, 25, 35, 50, 70, 95, 120, 150, 185, 240 };

    /// <summary>Returns the reference table number.</summary>
    [Fact]
    public void TableNumberIs52H()
    {
        Assert.Equal("52H", new Table52H().TableNumber);
    }

    /// <summary>Returns every reference current from table 52H regardless of arrangement type.</summary>
    [Theory]
    [InlineData(10, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 78)]
    [InlineData(10, InstallationEnvironment.Buried, ConductorMaterial.Copper, 86)]
    [InlineData(10, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 60)]
    [InlineData(10, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 67)]
    [InlineData(16, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 100)]
    [InlineData(16, InstallationEnvironment.Buried, ConductorMaterial.Copper, 110)]
    [InlineData(16, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 79)]
    [InlineData(16, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 86)]
    [InlineData(25, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 130)]
    [InlineData(25, InstallationEnvironment.Buried, ConductorMaterial.Copper, 145)]
    [InlineData(25, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 105)]
    [InlineData(25, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 110)]
    [InlineData(35, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 165)]
    [InlineData(35, InstallationEnvironment.Buried, ConductorMaterial.Copper, 170)]
    [InlineData(35, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 125)]
    [InlineData(35, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 135)]
    [InlineData(50, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 205)]
    [InlineData(50, InstallationEnvironment.Buried, ConductorMaterial.Copper, 215)]
    [InlineData(50, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 160)]
    [InlineData(50, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 165)]
    [InlineData(70, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 255)]
    [InlineData(70, InstallationEnvironment.Buried, ConductorMaterial.Copper, 260)]
    [InlineData(70, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 195)]
    [InlineData(70, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 205)]
    [InlineData(95, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 310)]
    [InlineData(95, InstallationEnvironment.Buried, ConductorMaterial.Copper, 315)]
    [InlineData(95, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 240)]
    [InlineData(95, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 245)]
    [InlineData(120, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 360)]
    [InlineData(120, InstallationEnvironment.Buried, ConductorMaterial.Copper, 360)]
    [InlineData(120, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 280)]
    [InlineData(120, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 280)]
    [InlineData(150, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 410)]
    [InlineData(150, InstallationEnvironment.Buried, ConductorMaterial.Copper, 405)]
    [InlineData(150, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 320)]
    [InlineData(150, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 315)]
    [InlineData(185, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 460)]
    [InlineData(185, InstallationEnvironment.Buried, ConductorMaterial.Copper, 450)]
    [InlineData(185, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 360)]
    [InlineData(185, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 350)]
    [InlineData(240, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, 550)]
    [InlineData(240, InstallationEnvironment.Buried, ConductorMaterial.Copper, 525)]
    [InlineData(240, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, 430)]
    [InlineData(240, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, 410)]
    public void ReturnsExpectedBaseCurrentForEveryArrangement(double crossSectionMm2, InstallationEnvironment environment, ConductorMaterial conductorMaterial, double expectedCurrentAmps)
    {
        var table = new Table52H();
        ArrangementType?[] arrangements = { null, ArrangementType.Unspecified, ArrangementType.Trefoil, ArrangementType.Flat };

        foreach (var arrangement in arrangements)
        {
            Assert.True(table.TryGetBaseCurrent(crossSectionMm2, environment, conductorMaterial, arrangement, out var currentAmps));
            Assert.Equal(expectedCurrentAmps, currentAmps);
        }
    }

    /// <summary>Returns the exact available sections for each supported environment and conductor material.</summary>
    [Fact]
    public void AvailableCrossSectionsMatchTable52H()
    {
        var table = new Table52H();

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

    /// <summary>Returns no cross-sections for an unsupported environment.</summary>
    [Fact]
    public void AvailableCrossSectionsAreEmptyForAnUnsupportedEnvironment()
    {
        var table = new Table52H();

        Assert.Empty(table.GetAvailableCrossSections((InstallationEnvironment)99, ConductorMaterial.Copper, null));
    }

    /// <summary>Returns false and zero for a cross-section not tabulated in table 52H.</summary>
    [Fact]
    public void ReturnsFalseAndZeroForAnUnlistedCrossSection()
    {
        var table = new Table52H();

        Assert.False(table.TryGetBaseCurrent(300, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, null, out var currentAmps));
        Assert.Equal(0, currentAmps);
    }
}
