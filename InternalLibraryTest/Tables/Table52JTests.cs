using HTA.CableSizing.Enums;
using HTA.CableSizing.Tables;
using Xunit;

namespace HTA.CableSizing.Tests.Tables;

/// <summary>Tests the reference current values and contract behavior of table 52J.</summary>
public sealed class Table52JTests
{
    private static readonly double[] ExpectedCrossSections = { 16, 25, 35, 50, 70, 95, 120, 150, 185, 240, 300, 400, 500, 630, 800, 1000, 1200 };

    private static readonly (InstallationEnvironment Environment, ConductorMaterial Material, ArrangementType Arrangement)[] Configurations =
    {
        (InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil),
        (InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat),
        (InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil),
        (InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat),
        (InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil),
        (InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat),
        (InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil),
        (InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)
    };

    private static readonly double[][] ExpectedCurrents =
    {
        new double[] { 120, 123, 126, 129, 94, 96, 98, 101 },
        new double[] { 157, 161, 161, 165, 122, 125, 125, 128 },
        new double[] { 190, 195, 192, 197, 147, 151, 149, 153 },
        new double[] { 233, 244, 225, 231, 185, 189, 175, 179 },
        new double[] { 292, 304, 276, 283, 226, 236, 214, 220 },
        new double[] { 356, 369, 330, 338, 266, 285, 252, 262 },
        new double[] { 409, 423, 375, 383, 318, 330, 291, 299 },
        new double[] { 465, 478, 420, 430, 360, 370, 325, 334 },
        new double[] { 533, 549, 474, 484, 417, 430, 370, 379 },
        new double[] { 630, 646, 549, 559, 490, 504, 428, 439 },
        new double[] { 724, 735, 619, 623, 567, 579, 485, 492 },
        new double[] { 836, 838, 698, 703, 662, 669, 554, 562 },
        new double[] { 959, 958, 786, 785, 771, 776, 631, 637 },
        new double[] { 1108, 1108, 887, 886, 897, 905, 720, 727 },
        new double[] { 1255, 1244, 980, 970, 1037, 1040, 810, 812 },
        new double[] { 1390, 1366, 1063, 1042, 1165, 1160, 895, 890 },
        new double[] { 1480, 1445, 1117, 1087, 1264, 1252, 957, 945 }
    };

    /// <summary>Returns the reference table number.</summary>
    [Fact]
    public void TableNumberIs52J()
    {
        Assert.Equal("52J", new Table52J().TableNumber);
    }

    /// <summary>Returns all 136 reference currents from table 52J in the specified column order.</summary>
    [Fact]
    public void ReturnsEveryReferenceCurrentForEachConfiguration()
    {
        var table = new Table52J();

        for (var sectionIndex = 0; sectionIndex < ExpectedCrossSections.Length; sectionIndex++)
        {
            for (var configurationIndex = 0; configurationIndex < Configurations.Length; configurationIndex++)
            {
                var configuration = Configurations[configurationIndex];
                Assert.True(table.TryGetBaseCurrent(
                    ExpectedCrossSections[sectionIndex],
                    configuration.Environment,
                    configuration.Material,
                    configuration.Arrangement,
                    out var currentAmps));
                Assert.Equal(ExpectedCurrents[sectionIndex][configurationIndex], currentAmps);
            }
        }
    }

    /// <summary>Returns the exact available sections for each of the eight supported configurations.</summary>
    [Fact]
    public void AvailableCrossSectionsMatchTable52JForEveryConfiguration()
    {
        var table = new Table52J();

        foreach (var configuration in Configurations)
        {
            var crossSections = table.GetAvailableCrossSections(
                configuration.Environment,
                configuration.Material,
                configuration.Arrangement);

            Assert.Equal(ExpectedCrossSections, crossSections);
            Assert.IsAssignableFrom<IList<double>>(crossSections);
            Assert.True(((IList<double>)crossSections).IsReadOnly);
            Assert.Throws<NotSupportedException>(() => ((IList<double>)crossSections).Add(1300));
        }
    }

    /// <summary>Returns no available sections when the arrangement is absent, unspecified, or unknown.</summary>
    [Fact]
    public void ReturnsNoAvailableSectionsForAnInvalidArrangement()
    {
        var table = new Table52J();

        Assert.Empty(table.GetAvailableCrossSections(InstallationEnvironment.FreeAir, ConductorMaterial.Copper, null));
        Assert.Empty(table.GetAvailableCrossSections(InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Unspecified));
        Assert.Empty(table.GetAvailableCrossSections(InstallationEnvironment.FreeAir, ConductorMaterial.Copper, (ArrangementType)99));
    }

    /// <summary>Returns false and zero when the arrangement is absent, unspecified, or unknown.</summary>
    [Fact]
    public void ReturnsFalseAndZeroForAnInvalidArrangement()
    {
        var table = new Table52J();

        Assert.False(table.TryGetBaseCurrent(16, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, null, out var currentAmps));
        Assert.Equal(0, currentAmps);
        Assert.False(table.TryGetBaseCurrent(16, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Unspecified, out currentAmps));
        Assert.Equal(0, currentAmps);
        Assert.False(table.TryGetBaseCurrent(16, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, (ArrangementType)99, out currentAmps));
        Assert.Equal(0, currentAmps);
    }

    /// <summary>Returns false and zero for an unlisted section or unsupported configuration.</summary>
    [Fact]
    public void ReturnsFalseAndZeroForAnUnlistedSectionOrConfiguration()
    {
        var table = new Table52J();

        Assert.False(table.TryGetBaseCurrent(10, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil, out var currentAmps));
        Assert.Equal(0, currentAmps);
        Assert.False(table.TryGetBaseCurrent(16, (InstallationEnvironment)99, ConductorMaterial.Copper, ArrangementType.Trefoil, out currentAmps));
        Assert.Equal(0, currentAmps);
        Assert.False(table.TryGetBaseCurrent(16, InstallationEnvironment.FreeAir, (ConductorMaterial)99, ArrangementType.Trefoil, out currentAmps));
        Assert.Equal(0, currentAmps);
        Assert.Empty(table.GetAvailableCrossSections((InstallationEnvironment)99, ConductorMaterial.Copper, ArrangementType.Trefoil));
        Assert.Empty(table.GetAvailableCrossSections(InstallationEnvironment.FreeAir, (ConductorMaterial)99, ArrangementType.Trefoil));
    }
}
