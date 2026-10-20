using HTA.CableSizing.Enums;

namespace HTA.CableSizing.Models;

public sealed record InstallationContext(
    int? CablesInConduitCount = null,
    double? CableSpacingInConduitMeters = null,
    int? CablesPerPhase = null,
    double? LayingDepthMeters = null,
    int? TouchingConduitLayerCountInFreeAir = null,
    int? TouchingConduitsPerLayerInFreeAir = null,
    double? AmbientAirTemperatureCelsius = null,
    InsulationType? InsulationType = null,
    ArrangementType ArrangementType = ArrangementType.Unspecified,
    int? TouchingCablesOnUnperforatedSurfaceCount = null,
    double? CableClearanceMeters = null,
    double? LargestCableDiameterMeters = null,
    int? TouchingCablesOnCeilingCount = null,
    double? VerticalCableClearanceMeters = null,
    int? TouchingCablesOnPerforatedTrayCount = null,
    int? TouchingCablesOnLadderOrCleatOrMeshCount = null,
    int? TouchingCableLayerCountInFreeAir = null);