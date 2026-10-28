using System.Text;
using CInterfaceNative;
using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.CInterface;

public unsafe class NativeInterfaceTests
{
    [Fact]
    public void ContextWithFlagZeroGivesNullEvenWhenValueIsSet()
    {
        var native = new InstallationContextNative { cables_per_phase = 3, laying_depth_m = 0.8 };

        var context = NativeInputMapper.ToInstallationContext(native);

        context.CablesPerPhase.Should().BeNull();
        context.LayingDepthMeters.Should().BeNull();
        context.Environment.Should().BeNull();
        context.AreCablesEnclosed.Should().BeNull();
    }

    [Fact]
    public void ContextWithFlagOneGivesValue()
    {
        var native = new InstallationContextNative
        {
            has_cables_per_phase = 1, cables_per_phase = 3,
            has_laying_depth_m = 1, laying_depth_m = 0.8,
            has_environment = 1, environment = (int)InstallationEnvironment.Buried,
            has_insulation_type = 1, insulation_type = (int)InsulationType.PrEpr,
            arrangement_type = (int)ArrangementType.Flat
        };

        var context = NativeInputMapper.ToInstallationContext(native);

        context.CablesPerPhase.Should().Be(3);
        context.LayingDepthMeters.Should().Be(0.8);
        context.Environment.Should().Be(InstallationEnvironment.Buried);
        context.InsulationType.Should().Be(InsulationType.PrEpr);
        context.ArrangementType.Should().Be(ArrangementType.Flat);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void BoolIsMappedFromInt(int value, bool expected)
    {
        var native = new InstallationContextNative { has_is_exposed_to_direct_sunlight = 1, is_exposed_to_direct_sunlight = value };

        NativeInputMapper.ToInstallationContext(native).IsExposedToDirectSunlight.Should().Be(expected);
    }

    [Fact]
    public void InvalidBoolOrFlagIsRejected()
    {
        var badBool = new InstallationContextNative { has_are_cables_enclosed = 1, are_cables_enclosed = 2 };
        var badFlag = new InstallationContextNative { has_cables_per_phase = 5 };

        FluentActions.Invoking(() => NativeInputMapper.ToInstallationContext(badBool)).Should().Throw<NativeDataException>();
        FluentActions.Invoking(() => NativeInputMapper.ToInstallationContext(badFlag)).Should().Throw<NativeDataException>();
    }

    [Fact]
    public void UnknownEnumValueIsPassedWithoutValidation()
    {
        var native = new InstallationContextNative { has_environment = 1, environment = 42 };

        ((int)NativeInputMapper.ToInstallationContext(native).Environment!.Value).Should().Be(42);
    }

    [Fact]
    public void SelectionInputMapsAbsentAndPresentValues()
    {
        var absent = NativeInputMapper.ToSelectionInput(new SelectionInputNative { u0_kv = 6 });
        absent.CableConstruction.Should().BeNull();
        absent.AssignedVoltage.Should().BeNull();
        absent.ArrangementType.Should().BeNull();

        var present = NativeInputMapper.ToSelectionInput(new SelectionInputNative
        {
            has_cable_construction = 1, cable_construction = (int)CableConstruction.ThreeSingleCoreCables,
            has_conductor_material = 1, conductor_material = (int)ConductorMaterial.Aluminum,
            has_assigned_voltage = 1, u0_kv = 6, u_kv = 10, um_kv = 12
        });
        present.CableConstruction.Should().Be(CableConstruction.ThreeSingleCoreCables);
        present.ConductorMaterial.Should().Be(ConductorMaterial.Aluminum);
        present.AssignedVoltage!.UmKilovolts.Should().Be(12);
        present.ElectricFieldType.Should().BeNull();
    }

    [Fact]
    public void CompleteResultIsMapped()
    {
        var result = new CalculationResult(CalculationStatus.Complete, 120.5, "52G", 150, 0.8, 120.5, 95,
            new[] { new FactorResult("K1", FactorStatus.Applied, 0.8, "e", "s") }, new[] { "w1", "w2" });

        var native = NativeResultMapper.ToNative(result);

        native.status.Should().Be(0);
        (native.has_value, native.value).Should().Be((1, 120.5));
        (native.has_base_current, native.base_current).Should().Be((1, 150.0));
        (native.has_global_factor, native.global_factor).Should().Be((1, 0.8));
        (native.has_corrected_current, native.corrected_current).Should().Be((1, 120.5));
        (native.has_selected_section, native.selected_section).Should().Be((1, 95.0));
        native.reference_table.Should().Be(ReferenceTableCodes.Table52G);
        native.warning_count.Should().Be(2);
        native.factor_count.Should().Be(1);
    }

    [Fact]
    public void PartialResultIsMapped()
    {
        var result = new CalculationResult(CalculationStatus.Partial, 0.9, null, null, 0.9, null, null);

        var native = NativeResultMapper.ToNative(result);

        native.status.Should().Be(1);
        native.has_value.Should().Be(1);
        native.has_global_factor.Should().Be(1);
        native.has_base_current.Should().Be(0);
        native.reference_table.Should().Be(0);
    }

    [Fact]
    public void ImpossibleResultHasNoValues()
    {
        var result = new CalculationResult(CalculationStatus.Impossible, null, "52J", null, null, null, null, null, new[] { "w" });

        var native = NativeResultMapper.ToNative(result);

        native.status.Should().Be(2);
        native.has_value.Should().Be(0);
        native.has_corrected_current.Should().Be(0);
        native.has_selected_section.Should().Be(0);
        native.reference_table.Should().Be(ReferenceTableCodes.Table52J);
        native.warning_count.Should().Be(1);
    }

    [Fact]
    public void Utf8CopyFitsInLargeBuffer()
    {
        var buffer = new byte[32];

        var written = NativeResultMapper.CopyUtf8("abé", buffer);

        written.Should().Be(4);
        Encoding.UTF8.GetString(buffer, 0, written).Should().Be("abé");
        buffer[written].Should().Be(0);
    }

    [Fact]
    public void Utf8CopyTruncatesWithoutSplittingCharacters()
    {
        var buffer = new byte[4];

        var written = NativeResultMapper.CopyUtf8("aéé", buffer);

        written.Should().Be(3);
        Encoding.UTF8.GetString(buffer, 0, written).Should().Be("aé");
        buffer[3].Should().Be(0);
    }

    [Fact]
    public void Utf8CopyIntoEmptyBufferReturnsMinusOne()
    {
        NativeResultMapper.CopyUtf8("a", Span<byte>.Empty).Should().Be(-1);
    }

    [Fact]
    public void WarningAndFactorAccessUseLastResult()
    {
        LastResultStore.Set(new CalculationResult(CalculationStatus.Complete, 1, null, null, 1, null, null,
            new[] { new FactorResult("K1", FactorStatus.NotApplicable, null, "explication", null) }, new[] { "avertissement" }));
        var buffer = new byte[64];
        var output = new FactorResultNative();

        try
        {
            LastResultStore.GetWarning(0, buffer).Should().Be(Encoding.UTF8.GetByteCount("avertissement"));
            LastResultStore.GetWarning(1, buffer).Should().Be(-1);
            LastResultStore.GetWarning(-1, buffer).Should().Be(-1);
            LastResultStore.GetFactorExplanation(0, buffer).Should().Be(Encoding.UTF8.GetByteCount("explication"));
            LastResultStore.GetFactorExplanation(3, buffer).Should().Be(-1);

            NativeApi.GetFactor(0, &output).Should().Be(NativeReturnCodes.Ok);
            output.status.Should().Be((int)FactorStatus.NotApplicable);
            output.has_value.Should().Be(0);
            Encoding.UTF8.GetString(output.name_buffer, 2).Should().Be("K1");
            output.source_buffer[0].Should().Be(0);
            NativeApi.GetFactor(5, &output).Should().Be(NativeReturnCodes.InvalidData);
        }
        finally
        {
            LastResultStore.Clear();
        }
    }

    [Fact]
    public void ApiReturnsOneForNullPointers()
    {
        var context = new InstallationContextNative();
        var input = new SelectionInputNative();
        var result = new CalculationResultNative();

        NativeApi.CalculateGlobalFactor(null, &result).Should().Be(NativeReturnCodes.NullArgument);
        NativeApi.CalculateGlobalFactor(&context, null).Should().Be(NativeReturnCodes.NullArgument);
        NativeApi.CalculateAllowableCurrent(95, null, &context, &result).Should().Be(NativeReturnCodes.NullArgument);
        NativeApi.CalculateAllowableCurrent(95, &input, null, &result).Should().Be(NativeReturnCodes.NullArgument);
        NativeApi.CalculateMinimumSection(100, &input, &context, null).Should().Be(NativeReturnCodes.NullArgument);
        NativeApi.GetFactor(0, null).Should().Be(NativeReturnCodes.NullArgument);
        NativeApi.GetWarning(0, null, 10).Should().Be(-1);
    }

    [Fact]
    public void ApiReturnsTwoForInconsistentData()
    {
        var context = new InstallationContextNative { has_cables_per_phase = 7 };
        var result = new CalculationResultNative();

        NativeApi.CalculateGlobalFactor(&context, &result).Should().Be(NativeReturnCodes.InvalidData);
    }

    [Fact]
    public void ApiWritesResultAndKeepsItForDetails()
    {
        var context = new InstallationContextNative();
        var result = new CalculationResultNative();

        try
        {
            NativeApi.CalculateGlobalFactor(&context, &result).Should().Be(NativeReturnCodes.Ok);

            result.status.Should().BeInRange(0, 2);
            var buffer = new byte[256];
            for (var i = 0; i < result.warning_count; i++)
            {
                LastResultStore.GetWarning(i, buffer).Should().BeGreaterThanOrEqualTo(0);
            }
        }
        finally
        {
            LastResultStore.Clear();
        }
    }
}
