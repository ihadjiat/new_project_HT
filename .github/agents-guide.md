# Guide agents — HTA.CableSizing

Lire uniquement les sections utiles à votre tâche (économie de tokens).

## Index
- [1. Objectif](#1-objectif)
- [2. Règles de travail](#2-règles-de-travail)
- [3. Structure du code](#3-structure-du-code)
- [4. Sources métier (lignes de `instruction-metier.md`)](#4-sources-métier)
- [5. Constats sur le code existant](#5-constats-sur-le-code-existant-à-corriger)
- [6. Conception cible](#6-conception-cible)
- [7. Interface C (Delphi)](#7-interface-c-delphi)
- [8. Tâches](#8-tâches)
- [9. Dette technique](#9-dette-technique)

## 1. Objectif
Librairie .NET 10 `HTA.CableSizing` (inspirée NF C 13-200 partie 5-52), exposée en C pour un logiciel Delphi.
Trois fonctions :
1. Facteur de correction global (produit des facteurs applicables).
2. Courant admissible corrigé pour une section donnée.
3. Section minimale pour un courant souhaité.

## 2. Règles de travail
- Code propre, lisible, méthodes courtes, noms explicites, en anglais.
- Toute méthode publique : `<summary>` + tests unitaires (xUnit, projet `InternalLibraryTest`).
- **Ne lancer aucune commande** (build, test, dotnet) : l'utilisateur s'en charge.
- **Ne jamais inventer** une valeur. Donnée absente ou ambiguë : la marquer « à vérifier », ne pas l'implémenter.
- Un facteur nécessaire mais non documenté ne vaut **jamais 1 silencieusement** : statut Partiel ou Impossible.
- Pas d'interpolation ni d'extrapolation (tables, sections, facteurs).
- Un facteur non applicable au contexte retourne 1.0 explicitement (cas « non applicable »), distinct du « manquant ».
- Modifications minimales, une tâche = un périmètre restreint, suivre le style existant.
- Pas de commentaire superflu ; tracer la source (tableau/article) dans le `summary` des données.
- Si la structure de la solution change (fichier/dossier/classe créé, renommé, supprimé), mettre à jour `.github/agents-guide.md` (sections 3 et 8) dans la même tâche, ainsi que tout autre `.md` concerné.

## 3. Structure du code
- `InternalLibrary/` (namespace `HTA.CableSizing`)
  - `AllowableCurrentCalculator.cs` : les 3 fonctions publiques, qui retournent un `CalculationResult` sans exception métier, utilisent `ReferenceCurrentSelectionInput`, `ReferenceCurrentTableSelector`, `IReferenceCurrentTable` et `IResultCorrectionFactor` (facteurs injectables, défaut K1 à K17 + `SymmetryFactor`).
	- `Enums/` : `ArrangementType`, `InsulationType`, `InstallationEnvironment`, `DrumWindingType` (`SingleLayer = 0`, `MultipleLayers = 1`), `CableConstruction`, `ConductorMaterial`, `ElectricFieldType`.
	- `Factors/` : `IResultCorrectionFactor` (contrat unique, `Evaluate`) + `K1..K17CorrectionFactor`, `SymmetryFactor`. Tous implémentent uniquement `IResultCorrectionFactor` ; l'ancien contrat `ICorrectionFactor` et les anciennes méthodes `Calculate` sont supprimés.
	- `Models/InstallationContext.cs` : entrées nullables des facteurs, dont l'environnement d'installation (`InstallationEnvironment?`), l'exposition directe au soleil (`IsExposedToDirectSunlight`, utilisée par K2), la température du sol (`SoilTemperatureCelsius`, utilisée par K12), la résistivité thermique explicite du sol en K·m/W (`SoilThermalResistivityKMW`, utilisée par K13, sans déduction depuis le nom du terrain), les entrées K9 `AreCablesEnclosed` et `EnclosedCableOrGroupCount`, les entrées K10 `IsCableInClosedUnfilledTrenchFlushWithFloor`, `CableConstruction`, `CableTrenchDimensionAMeters`, `CableTrenchDimensionBMeters` et `SumCableCircuitEncombrementsMeters` (mesures en mètres), les entrées K11 `IsCableOnMobileEquipmentDrum`, `DrumWindingType`, `DrumTurnsCount` et `DrumCompletedLayerCount`, les entrées K15 `BuriedCircuitOrCableCount` et `BuriedCircuitSpacingMeters`, les entrées K16 `BuriedConduitLayerCount`, `BuriedConduitsPerLayer`, `CablesPerBuriedConduit` et `BuriedConcreteThermalResistivityKMW`, et l'entrée fs `AreCablePhaseArrangementsSymmetrical`. K10 calcule le coefficient d'occupation `p / somme(e_n)` avec `p = 2a + b` et applique uniquement les coefficients exacts de sa table.
	- `Models/CalculationStatus.cs`, `Models/FactorStatus.cs`, `Models/FactorResult.cs` et `Models/CalculationResult.cs` : statuts et résultats des calculs et facteurs. `AssignedVoltage.cs` décrit U₀/U/Uₘ ; `ReferenceCurrentSelectionInput.cs` et `ReferenceCurrentTableSelectionResult.cs` portent l'entrée et le résultat de sélection, avec `ReferenceCurrentTableSelectionStatus`.
	applique §8.2 avec un résultat impossible explicatif.
- `InternalLibraryTest/` : tests (K1-K17, Symmetry, tables de référence et `ReferenceCurrentTableSelector`).
- `CInterfaceNative/` (namespace `CInterfaceNative`, AOT, `AllowUnsafeBlocks`) : `Exports.cs` (exports `UnmanagedCallersOnly`, fins wrappers), `NativeApi.cs` (corps testables des exports + `NativeReturnCodes`), `NativeInputMapper.cs` / `NativeValueReader.cs` (natif → modèle), `NativeResultMapper.cs` (résultat → natif, copie UTF-8), `LastResultStore.cs` (dernier résultat, sous lock), `InstallationContextNative.cs`, `SelectionInputNative.cs`, `CalculationResultNative.cs`, `FactorResultNative.cs`, `ReferenceTableCodes.cs`, `NativeDataException.cs`.
- `InternalLibraryTest/CInterface/NativeInterfaceTests.cs` : tests du mapping et des wrappers (le projet de tests référence `CInterfaceNative` ; `AllowUnsafeBlocks` activé).
- `DelphiExample/` : exemple Delphi (`HtaCableSizing.pas`, `Example.dpr`, `README.md`), hors solution, non compilé.
- `ConsoleApp1/` : console de démo.

État des facteurs : évaluations structurées (`Evaluate`) implémentées = K1-K17 et fs. K2 est migré vers l'évaluation structurée, mais son coefficient numérique reste indisponible faute de données métier.

## 4. Sources métier
Fichier : `.github/instruction-metier.md` (lire par plage de lignes).
| Sujet | Lignes |
|---|---|
| Consignes « ne pas inventer » | 10-40 |
| Formules (global, corrigé, section min) | 81-131 |
| États Complet / Partiel / Impossible | 133-166 |
| Tables courants 52G / 52H / 52I / 52J | 181 / 207 / 233 / 268 |
| Conditions de référence 52G-52J | 320-340 |
| K1 / K2 | 343 / 373 |
| K3 / K4 / K5 / K6 / K7 | 381 / 404 / 422 / 444 / 460 |
| K8 / K9 / K10 / K11 | 486 / 502 / 518 / 548 |
| K12 / K13 / K14 | 564 / 586 / 610 |
| K15 / K16 / K17 | 618 / 642 / 661 |
| Facteur de symétrie fs | 677-717 |
| Entrées et sélection du tableau | 718-772 |
| Applicabilité et combinaison des facteurs | 773-811 |
| Valeurs hors table (pas d'interpolation, pas d'arrondi prématuré) | 812-827 |
| Résultat et explication exigés (11 points) | 829-852 |
| Questions ouvertes / données à vérifier | 853-875 |
| Critères d'acceptation (tests) | 876-907 |
| Avertissement utilisateur à afficher | ~908-912 |

## 5. Constats sur le code existant (à corriger)
- `ICorrectionFactor.Calculate` retourne un `double` : impossible de distinguer « non applicable », « manquant » et « hors table ». Les facteurs retournent 1.0 quand les données sont absentes (ex. K1, K3, `SymmetryFactor`) et lèvent des exceptions hors table.
- `K1CorrectionFactor` applique la formule sqrt pour toute température ; la norme (§6.1, l.343) impose la valeur tabulée, formule seulement si validée → à signaler partiel.
- `AllowableCurrentCalculator` multiplie tous les K par défaut ; interdit par §9.3 (l.800). K7 se multiplie avec un seul de K3-K6 (ex. 0,73 × 0,75, l.460).
- Tests : xUnit + FluentAssertions, un fichier `Factors/KnCorrectionFactorTests.cs` par facteur, `[Theory]`/`[InlineData]` sur les valeurs des tables.
- `CInterfaceNative` : `PublishAot`, `NativeLib=Shared`, `win-x64`.
- Tables de courant (l.181-318) : 52G (16-300 mm²), 52H et 52I (10-240 mm²), 52J (16-1200 mm², 8 colonnes trèfle/nappe). Colonnes : Cu air, Cu enterré, Al air, Al enterré. Sections différentes selon le tableau. Milliers écrits avec espace (`1 108`) : attention au parsing.
- Les tables ne distinguent que **air** et **enterré**. `InstallationMethod.InConduit` n'a pas de colonne. **Décision validée** : remplacer `InstallationMethod` par un enum de milieu `InstallationEnvironment { FreeAir, Buried }` (choisit la colonne de la table) ; `InConduit` disparaît. Le dispositif (conduit, goulotte, caniveau...) n'est plus un enum : il se déduit des entrées du contexte (K8, K9, K10, K16, K17 selon §9.1 / §9.2).
- Critères de sélection (§8.2, l.760) : `ReferenceCurrentTableSelector.Select` applique construction, champ, isolant, matériau et tension U₀/U/Uₘ ; 52G/52J utilisent les bornes inclusives 3,6/6 (7,2) à 18/30 (36) kV, 52H/52I exactement 6/6 (7,2) kV, et 52J exige trèfle/nappe. Entrées manquantes ou combinaisons non couvertes → résultat Impossible.
- 52I enterré : libellé « température ambiante maximum 20 °C » à vérifier (l.337).
- K10 (l.518) : tableau d'occupation 5-50 ; règle d'encombrement des unipolaires ambiguë → ne pas implémenter ce cas (Partiel). Entrées : p = 2a + b, somme des encombrements.
- K9 (l.502) : la table des câbles enfermés est implémentée avec les entrées `AreCablesEnclosed` et `EnclosedCableOrGroupCount` ; les comptes non tabulés restent hors plage.
- K11 (l.548) : les tables du tambour monocouche (nombre de spires) et multicouche (couches 1, 2, 3, >3) sont implémentées avec les entrées dédiées du contexte.
- K12 (l.564) : table + formule comme K1 (réf. 20 °C), même règle prudente que K1.
- K13 (l.586) : migration vers `IResultCorrectionFactor` terminée ; utilise la résistivité explicitement fournie (`SoilThermalResistivityKMW`) et `CableConstruction`, avec les valeurs exactes de la table (0,5 à 3,0 K·m/W), sans interpolation ni déduction du nom ou de l'humidité du terrain. 1,00 K·m/W = valeur de référence.
- `CableType` ne représente en fait que le matériau (Cuivre/Aluminium).
- Traçabilité (§0, l.26) : indiquer le n° de tableau/article dans le `summary` des données.

## 6. Conception cible
- Les entrées invalides (valeur négative, null) restent des exceptions ; les cas métier ne le sont plus. Les exceptions ne remplacent plus
- `ICorrectionFactor` doit pouvoir signaler : non applicable (1.0), appliqué (valeur), manquant/hors table (partiel).
- La sélection du tableau (52G..52J) est implémentée et testable via `ReferenceCurrentTableSelector.Select` (§8.2) ; les cas manquants ou non couverts retournent un résultat impossible expliqué, sans tableau par défaut.
- Combinaison de `fs` avec les K : non précisée par la norme transcrite → afficher comme hypothèse ou bloquer (§7.3).
- La sélection réutilise `CableConstruction`, `ElectricFieldType`, `InsulationType`, `ConductorMaterial` et `ArrangementType` ; `AssignedVoltage` fournit U₀/U/Uₘ, et `ReferenceCurrentSelectionInput` / `ReferenceCurrentTableSelectionResult` exposent l'entrée et le résultat structuré.

## 7. Interface C (Delphi)
- Une exception .NET ne traverse **pas** la frontière native (avec `UnmanagedCallersOnly`, elle arrête le processus) : chaque export a un `try/catch` global et retourne un code d'erreur.
- Entrées/sorties par **structs C** passées par pointeur : `[StructLayout(LayoutKind.Sequential)]`, champs blittables uniquement.
  - Pas de `string`, `bool`, `Nullable<T>` : utiliser `int`/`double`, enums en `int`, et un champ `has_xxx` (int 0/1) pour chaque valeur optionnelle.
  - Pas de tableau managé ; tableaux de taille fixe ou buffers fournis par l'appelant.
- Exports en snake_case, retour `int` (0 = OK, code d'erreur sinon), résultat écrit dans une struct de sortie.
- Documenter chaque struct côté Delphi (`record` aligné équivalent) dans un fichier `.md` ou `.pas` d'exemple.

### 7.1 Conventions (étape 24)
- Convention d'appel cdecl. Structs `Sequential`, `Pack = 8` : côté Delphi `{$A8}` ; chaque `has_xxx` (Int32) précède sa valeur, un `double` est aligné sur 8 octets (4 octets de remplissage après un Int32 isolé).
- `has_xxx` = 0 donne `null` dans le modèle .NET (aucune valeur par défaut) ; toute valeur différente de 0 ou 1 pour un `has_xxx` ou un booléen natif donne le code retour 2.
- Enums transmis en Int32, sans validation (valeur inconnue gérée par le calculateur : OutOfRange ou Impossible).
- Codes retour des calculs : 0 = OK (résultat écrit, quel que soit le statut métier), 1 = argument nul, 2 = enum/donnée native incohérente, 3 = erreur interne.
- L'appelant alloue tous les structs et buffers. Textes en UTF-8 terminés par 0.

### 7.2 Valeurs d'enums
- ArrangementType : 0 Unspecified, 1 Trefoil, 2 Flat. CableConstruction : 0 ThreeCoreCable, 1 ThreeSingleCoreCables. ConductorMaterial : 0 Copper, 1 Aluminum. ElectricFieldType : 0 Radial, 1 NonRadial. InsulationType : 0 Pvc, 1 PrEpr. InstallationEnvironment : 0 FreeAir, 1 Buried. DrumWindingType : 0 SingleLayer, 1 MultipleLayers.
- CalculationStatus : 0 Complete, 1 Partial, 2 Impossible. FactorStatus : 0 Applied, 1 NotApplicable, 2 Missing, 3 OutOfRange.
- `reference_table` : 0 inconnu, 1 = 52G, 2 = 52H, 3 = 52I, 4 = 52J (code documenté dans `ReferenceTableCodes`).

### 7.3 Exports
- `int hta_calculate_global_factor(InstallationContextNative* ctx, CalculationResultNative* result)`
- `int hta_calculate_allowable_current(double section_mm2, SelectionInputNative* input, InstallationContextNative* ctx, CalculationResultNative* result)`
- `int hta_calculate_minimum_section(double desired_current, SelectionInputNative* input, InstallationContextNative* ctx, CalculationResultNative* result)`
- `int hta_get_warning(int index, char* buffer, int bufferSize)` : octets écrits hors terminateur ; texte tronqué sans couper un caractère UTF-8 si le buffer est trop petit ; -1 si index/buffer/taille invalides ou aucun résultat.
- `int hta_get_factor(int index, FactorResultNative* out)` : 0 OK, 1 pointeur nul, 2 index invalide, 3 erreur interne.
- `int hta_get_factor_explanation(int index, char* buffer, int bufferSize)` : comme `hta_get_warning`.
- Les 6 exports sont des wrappers `UnmanagedCallersOnly` (cdecl) dans `Exports.cs`, qui appellent `NativeApi` (les `try/catch` sont dans `NativeApi`). Codes retour : calculs 0 OK / 1 argument nul / 2 donnée incohérente / 3 erreur interne ; `hta_get_factor` 0 / 1 / 2 index invalide / 3 ; `hta_get_warning` et `hta_get_factor_explanation` : nombre d'octets écrits ou -1. `reference_table` vaut 1 (52G), 2 (52H), 3 (52I), 4 (52J), 0 (inconnu).
- Les index vont de 0 à `warning_count - 1`

### 7.4 Champs natifs (ordre exact)
`CalculationResultNative` : status; has_value, value; has_base_current, base_current; has_global_factor, global_factor; has_corrected_current, corrected_current; has_selected_section, selected_section; reference_table; warning_count; factor_count.

`FactorResultNative` : status; has_value; value; name_buffer[128] (octets); source_buffer[128] (octets).

`SelectionInputNative` : has_cable_construction, cable_construction; has_electric_field_type, electric_field_type; has_insulation_type, insulation_type; has_conductor_material, conductor_material; has_arrangement_type, arrangement_type; has_assigned_voltage; u0_kv, u_kv, um_kv (doubles, ignorés si has_assigned_voltage = 0).

`InstallationContextNative` (chaque champ `xxx` est précédé de `has_xxx` sauf `arrangement_type`, toujours présent, 0 = Unspecified) : cables_in_conduit_count (int); cable_spacing_in_conduit_m (double); cables_per_phase (int); laying_depth_m (double); touching_conduit_layer_count_in_free_air (int); touching_conduits_per_layer_in_free_air (int); ambient_air_temperature_c (double); insulation_type (enum); arrangement_type (enum, sans has_); touching_cables_on_unperforated_surface_count (int); cable_clearance_m (double); largest_cable_diameter_m (double); touching_cables_on_ceiling_count (int); vertical_cable_clearance_m (double); touching_cables_on_perforated_tray_count (int); touching_cables_on_ladder_or_cleat_or_mesh_count (int); touching_cable_layer_count_in_free_air (int); environment (enum); soil_temperature_c (double); buried_circuit_or_cable_count (int); buried_circuit_spacing_m (double); buried_conduit_layer_count (int); buried_conduits_per_layer (int); cables_per_buried_conduit (int); buried_concrete_thermal_resistivity_kmw (double); are_cables_enclosed (bool); enclosed_cable_or_group_count (int); is_cable_on_mobile_equipment_drum (bool); drum_winding_type (enum); drum_turns_count (int); drum_completed_layer_count (int); is_exposed_to_direct_sunlight (bool); solar_radiation_w_m2 (double); cable_surface_absorptivity (double); cable_surface_emissivity (double); is_cable_in_closed_unfilled_trench_flush_with_floor (bool); cable_construction (enum); cable_trench_dimension_a_m (double); cable_trench_dimension_b_m (double); sum_cable_circuit_encombrements_m (double); soil_thermal_resistivity_kmw (double); are_cable_phase_arrangements_symmetrical (bool).

## 8. Tâches
- Tables 52G, 52H, 52I et 52J et sélection selon §8.2 : implémentées et utilisées par `AllowableCurrentCalculator` (étape 23).
- K2 est migré vers `IResultCorrectionFactor` ; son coefficient numérique reste indisponible faute de données métier documentées.
- K10 est migré vers `IResultCorrectionFactor` ; la règle d'encombrement des câbles unipolaires reste ambiguë et ces configurations ne sont pas calculables avec la documentation actuelle.
- K13 est migré vers `IResultCorrectionFactor` (+ tests), avec une résistivité explicite et les valeurs exactes de la table §6.13.
Ordre recommandé ; chaque tâche = lire §2, la section métier concernée, et tester.
1. (fait) Nettoyage des fichiers de démonstration et mise à jour des noms de fichiers et de dossiers.
2. (fait) Définir les enums (matériau, isolant, construction tripolaire/unipolaire, `InstallationEnvironment` FreeAir/Buried en remplacement de `InstallationMethod`) et les modèles de résultat `CalculationStatus` / `CalculationResult`.
3. (fait) Ajouter le contrat de résultat des facteurs (`FactorStatus` / `FactorResult`). L'adaptation de `ICorrectionFactor` et des facteurs existants reste une tâche distincte.
4. (fait) Ajouter `IResultCorrectionFactor`, le nouveau contrat de résultat utilisé lors de la migration des facteurs.
5. (corrigée) Migration d'un facteur par tâche vers `IResultCorrectionFactor` : K1 et K12 (+ tests), en conservant `ICorrectionFactor`.
6. (fait pour K13) Migrer K13 vers `IResultCorrectionFactor` (+ tests), en conservant `ICorrectionFactor`.
7. (fait) Créer le contrat moderne `IReferenceCurrentTable` et implémenter 52G, 52H, 52I et 52J (+ tests de leurs valeurs). `BaseCurrentTable` reste une table de démonstration temporaire et `AllowableCurrentCalculator` n'utilise pas encore le nouveau contrat.
8. (fait) Migrer K5 et K6 vers `IResultCorrectionFactor` (+ tests), en conservant `ICorrectionFactor`.
9. (fait) Migrer K7 et K8 vers `IResultCorrectionFactor` (+ tests), en conservant `ICorrectionFactor`.
10. (fait) Migrer K15 vers `IResultCorrectionFactor` (+ tests), en conservant `ICorrectionFactor`.
11. (fait) Migrer fs (`SymmetryFactor`) vers `IResultCorrectionFactor` (+ tests), en conservant temporairement `ICorrectionFactor`.
12. (fait, étape 23) Adapter `AllowableCurrentCalculator` aux facteurs migrés et à la sélection du tableau (+ tests).
13. (fait, étape 24) Structs C et exports dans `CInterfaceNative` (+ tests des conversions, dans `InternalLibraryTest/CInterface`).
13bis. (fait, étape 24bis) Exports `UnmanagedCallersOnly` dans `Exports.cs` (+ test de réflexion `ExportsReflectionTests`).
14. (fait, étape 25) Exemple Delphi dans `DelphiExample/` (non compilé, non ajouté à la solution).
15. (fait en partie, étape 26) Nettoyage : anciens contrats (`ICorrectionFactor`, `IBaseCurrentTable`, `BaseCurrentTable`), anciennes méthodes `Calculate` et anciens enums (`InstallationMethod`, `CableType`) supprimés, format des `Source` aligné (`instruction-metier.md §x.y`). Reste en dette : §9.
21. (fait) Corrections de revue.
22. (fait) Règle de sélection du tableau (§8.2) et tests des branches, limites de tension, données manquantes et configurations non couvertes.

## 9. Dette technique
- [ ] `AllowableCurrentCalculator` instancié à chaque appel natif.
- [ ] `CInterfaceNative` : le dernier `CalculationResult` est conservé dans un état statique (`LastResultStore`, protégé par lock) ; acceptable pour Delphi mono-thread seulement, un appel concurrent écrase le détail. À remplacer par un handle de résultat si le multi-thread est requis.
- [ ] `CInterfaceNative` : tampons `name_buffer`/`source_buffer` fixés à 128 octets (troncature silencieuse) ; un `AllowableCurrentCalculator` est créé à chaque appel ; `InternalLibraryTest` référence le projet AOT `CInterfaceNative` (à séparer en `CInterfaceNative.Tests` si la référence pose problème) ; les exports eux-mêmes ne sont pas testés (seuls leurs corps `NativeApi`).
- [x] `K2CorrectionFactor.Calculate` retourne encore `1.0` avec un TODO et ne doit pas servir pour une installation exposée au soleil.
  À traiter au plus tard lors du nettoyage final (cocher ou retirer l'entrée une fois faite). Source : revue de code des étapes 1 à 5.
- [x] `K13CorrectionFactor.Calculate` retourne encore `1.0` avec un TODO ; ne pas l'utiliser pour une installation enterrée où K13 s'applique. Utiliser `Evaluate`.
- [x] L'ancien `SymmetryFactor.Calculate` retourne encore `1.0` avec un TODO ; ne pas l'utiliser pour une configuration de câbles unipolaires par phase où fs s'applique. Utiliser `Evaluate`.
- [ ] `InstallationContext` : 19+ paramètres, `Environment`, `SoilTemperatureCelsius` et `SoilThermalResistivityKMW` ajoutés en fin de record par compatibilité → regrouper ou réordonner une fois les anciens appels migrés.
- [ ] `K1CorrectionFactor` : `using System;` et `using System.Collections.Generic;` redondants (ImplicitUsings).
- [ ] K1 et K12 : clés `double` dans les dictionnaires de tables (`TryGetValue(temperature)`), exact pour des températures entières, à revoir si des valeurs non entières sont acceptées.
- [ ] K1 et K12 : `Evaluate` quasi identique → factoriser après migration de tous les facteurs.
- [ ] K3 et K4 : en cas de câbles non jointifs, `CableClearanceMeters` et `LargestCableDiameterMeters` n'identifient pas le support ; `Evaluate` reste manquant. À revoir si un indicateur de type de support est ajouté au contexte.
- [ ] K5 et K6 : pour les câbles non jointifs, les seules mesures d'écartement n'identifient pas le support (tablette perforée ou échelle/corbeau/treillis) ; `Evaluate` reste manquant sans compte propre au facteur. À revoir si un indicateur de type de support est ajouté au contexte.
- [ ] `AllowableCurrentCalculator` : règle K7 + un seul facteur K3 à K6 déduite des statuts (un seul `Applied` retenu, les `Missing` alternatifs ignorés avec avertissement, plusieurs `Applied` = Partial sans en appliquer) ; à confirmer faute d'indicateur de support dans le contexte.
- [ ] `AllowableCurrentCalculator` : l'explication (§11) est portée par `CalculationResult.Warnings` faute de champ dédié ; envisager un champ `Explanation`.
- [ ] `AllowableCurrentCalculator` : relation de fs avec les facteurs K non documentée (§9.3) ; fs est multiplié avec avertissement d'hypothèse.
- [ ] `AllowableCurrentCalculator` : un facteur K2 manquant (soleil) ou toute exception d'un facteur injecté est convertie en Missing ; valeur Partial calculée sans le facteur concerné.
- [ ] `IResultCorrectionFactor` n'expose pas `Name` (seul `FactorResult.Name` le porte) → décider si nécessaire.
- [ ] K14 `Evaluate` compare la profondeur par égalité exacte de `double`.
- [ ] Le format de `Source` est à centraliser dans une classe dédiée.
- [x] Ancien `K14CorrectionFactor.Calculate` interpole
- [x] `K15.Calculate` retourne encore 1.0
- [x] `K16.Calculate` retourne encore 1.0
- [x] `K10.Calculate` retourne encore 1.0
- [x] `K9.Calculate` et `K11.Calculate`
- [x] Ancien contrat `ICorrectionFactor`
- [x] Anciens enums `InstallationMethod`
- [ ] `DelphiExample/` : jamais compilé (pas de Delphi disponible) ; à vérifier manuellement (README, « Vérifications manuelles »).
- [ ] Tests `Models/` (`CalculationResultTests`
