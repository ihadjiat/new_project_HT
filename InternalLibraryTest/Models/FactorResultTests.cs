using FluentAssertions;
using HTA.CableSizing.Models;
using Xunit;

namespace InternalLibraryTest.Models;

public sealed class FactorResultTests
{
    [Fact]
    public void Constructor_ShouldPreserveProvidedValues()
    {
        var result = new FactorResult("K1", FactorStatus.Applied, 0.91, "Applied for the configuration.", "Table 52K");

        result.Name.Should().Be("K1");
        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(0.91);
        result.Explanation.Should().Be("Applied for the configuration.");
        result.Source.Should().Be("Table 52K");
    }
}
