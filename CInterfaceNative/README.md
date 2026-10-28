# HTA Cable Sizing – Native C Library

Native (NativeAOT) shared library implementing NF C 13-200 part 5-52 cable sizing. It exposes a plain C interface (cdecl) that can be called from C, C++, Delphi or any language able to load a DLL.

Three calculations are available:

| Function | Purpose |
|---|---|
| `hta_calculate_global_factor` | Global correction factor (product of all applicable correction factors) |
| `hta_calculate_allowable_current` | Corrected allowable current for a given cross-section |
| `hta_calculate_minimum_section` | Minimum cross-section for a desired current |

Three helper functions read the details (texts and factors) of the **last** calculation:
`hta_get_warning`, `hta_get_factor`, `hta_get_factor_explanation`.

## Build

```powershell
dotnet publish CInterfaceNative -c Release -r win-x64
```

The native library `CInterfaceNative.dll` is produced in `CInterfaceNative\bin\Release\net10.0\win-x64\publish\`. It is x64 only: the calling application must be 64-bit.

## Conventions

- Calling convention: **cdecl**.
- All structs are allocated and owned by the **caller**. The library never keeps a pointer to them.
- Structs use sequential layout with **8-byte packing** (`#pragma pack(8)` in C, `{$A8}` in Delphi).
- Booleans are `int` (0 or 1). Enums are `int` (numeric values).
- Optional values use a `has_xxx` flag followed by the value. If `has_xxx` is `0`, the value is ignored and treated as "not provided". A missing value is **never** replaced by a default (for example 1).
- Texts are UTF-8, zero-terminated, written into caller-provided buffers.
- Values are never interpolated or extrapolated between table entries.

## Return codes

Calculation functions and `hta_get_factor`:

| Code | Meaning |
|---|---|
| 0 | OK |
| 1 | Null argument |
| 2 | Inconsistent native data (`hta_get_factor`: invalid index) |
| 3 | Internal error |

`hta_get_warning` and `hta_get_factor_explanation` return the number of bytes written (excluding the terminator), or `-1` for invalid arguments.

A return code of `0` means the call itself succeeded. The business outcome is in `CalculationResultNative.status`:

| `status` | Meaning |
|---|---|
| 0 | Complete |
| 1 | Partial (some data is missing, see warnings and factors) |
| 2 | Impossible |

## Functions

```c
int hta_calculate_global_factor(
	InstallationContextNative* ctx,
	CalculationResultNative*   result);

int hta_calculate_allowable_current(
	double                     section_mm2,
	SelectionInputNative*      input,
	InstallationContextNative* ctx,
	CalculationResultNative*   result);

int hta_calculate_minimum_section(
	double                     desired_current,   /* amperes */
	SelectionInputNative*      input,
	InstallationContextNative* ctx,
	CalculationResultNative*   result);

int hta_get_warning(int index, unsigned char* buffer, int buffer_size);
int hta_get_factor(int index, FactorResultNative* output);
int hta_get_factor_explanation(int index, unsigned char* buffer, int buffer_size);
```

`hta_get_warning`, `hta_get_factor` and `hta_get_factor_explanation` refer to the last calculation performed. Use indexes from `0` to `warning_count - 1` and `0` to `factor_count - 1` (both are returned in `CalculationResultNative`).

> The last result is stored globally (under a lock). If several threads call the library, read the details right after your own calculation, or serialize the calls.

## Structs

### CalculationResultNative (output)

```c
typedef struct {
	int    status;               /* 0 Complete, 1 Partial, 2 Impossible */
	int    has_value;            double value;
	int    has_base_current;     double base_current;
	int    has_global_factor;    double global_factor;
	int    has_corrected_current;double corrected_current;
	int    has_selected_section; double selected_section;
	int    reference_table;      /* 0 unknown, 1 = 52G, 2 = 52H, 3 = 52I, 4 = 52J */
	int    warning_count;
	int    factor_count;
} CalculationResultNative;
```

Which fields are filled depends on the function:

| Function | Main fields |
|---|---|
| `hta_calculate_global_factor` | `global_factor` |
| `hta_calculate_allowable_current` | `base_current`, `global_factor`, `corrected_current` |
| `hta_calculate_minimum_section` | `selected_section`, `base_current`, `global_factor`, `corrected_current` |

Always test the matching `has_xxx` flag before reading a value.

### FactorResultNative (output of `hta_get_factor`)

```c
typedef struct {
	int           status;            /* 0 Applied, 1 NotApplicable, 2 Missing, 3 OutOfRange */
	int           has_value;
	double        value;
	unsigned char name_buffer[128];  /* UTF-8, zero-terminated */
	unsigned char source_buffer[128];/* UTF-8, zero-terminated */
} FactorResultNative;
```

Use `hta_get_factor_explanation` to read the explanation text of the factor.

### SelectionInputNative (input, used to select the reference table)

```c
typedef struct {
	int    has_cable_construction;    int cable_construction;
	int    has_electric_field_type;   int electric_field_type;
	int    has_insulation_type;       int insulation_type;
	int    has_conductor_material;    int conductor_material;
	int    has_arrangement_type;      int arrangement_type;
	int    has_assigned_voltage;      /* 0: u0_kv, u_kv, um_kv are ignored */
	double u0_kv;
	double u_kv;
	double um_kv;
} SelectionInputNative;
```

### InstallationContextNative (input)

Describes the installation (ambient temperature, soil, grouping, laying depth, drum, sun exposure, etc.). Every optional value follows the `has_xxx` / value pattern described above. The only value that is always read is `arrangement_type`.

The exact field list and order are defined in `InstallationContextNative.cs`. The Delphi binding in `DelphiExample/HtaCableSizing.pas` mirrors it. Always zero-initialize the struct before filling it, so that all `has_xxx` flags are `0` by default.

Each correction factor reads only the fields it needs. If a required field is not provided, the factor is reported as `Missing` and the result is `Partial`.

## Usage example (C)

```c
#include <stdio.h>
#include <string.h>

/* struct declarations as above (pack 8) */

int main(void)
{
	InstallationContextNative ctx;
	CalculationResultNative   res;
	memset(&ctx, 0, sizeof ctx);
	memset(&res, 0, sizeof res);

	ctx.has_ambient_air_temperature_c = 1;
	ctx.ambient_air_temperature_c     = 40.0;

	int rc = hta_calculate_global_factor(&ctx, &res);
	if (rc != 0) {
		printf("Call failed: %d\n", rc);
		return 1;
	}

	if (res.has_global_factor)
		printf("Global factor: %f\n", res.global_factor);
	printf("Status: %d\n", res.status);

	for (int i = 0; i < res.warning_count; i++) {
		unsigned char text[256];
		if (hta_get_warning(i, text, sizeof text) >= 0)
			printf("Warning: %s\n", text);
	}

	for (int i = 0; i < res.factor_count; i++) {
		FactorResultNative f;
		if (hta_get_factor(i, &f) == 0)
			printf("%s: status=%d\n", (const char*)f.name_buffer, f.status);
	}
	return 0;
}
```

## Complete example (C)

This example computes the three results for a copper, PR/EPR, three-core cable in free air, and prints the status, the values, the warnings and every factor. Enum values used: `cable_construction` 0 = ThreeCoreCable, 1 = ThreeSingleCoreCables; `electric_field_type` 0 = Radial, 1 = NonRadial; `insulation_type` 0 = PVC, 1 = PR/EPR; `conductor_material` 0 = Copper, 1 = Aluminum; `arrangement_type` 0 = Unspecified, 1 = Trefoil, 2 = Flat; `environment` 0 = FreeAir, 1 = Buried.

```c
#include <stdio.h>
#include <string.h>

/* struct declarations as above (pack 8) */

static const char* status_name(int s)
{
    return s == 0 ? "Complete" : s == 1 ? "Partial" : "Impossible";
}

static const char* factor_status_name(int s)
{
    switch (s) {
        case 0:  return "Applied";
        case 1:  return "NotApplicable";
        case 2:  return "Missing";
        default: return "OutOfRange";
    }
}

/* Prints the details of the last calculation. Call right after the calculation. */
static void print_details(const CalculationResultNative* res)
{
    printf("Status: %s, reference table code: %d\n", status_name(res->status), res->reference_table);

    if (res->has_base_current)      printf("Base current:      %.2f A\n", res->base_current);
    if (res->has_global_factor)     printf("Global factor:     %.4f\n",   res->global_factor);
    if (res->has_corrected_current) printf("Corrected current: %.2f A\n", res->corrected_current);
    if (res->has_selected_section)  printf("Selected section:  %.1f mm2\n", res->selected_section);

    for (int i = 0; i < res->warning_count; i++) {
        unsigned char text[512];
        if (hta_get_warning(i, text, (int)sizeof text) >= 0)
            printf("Warning: %s\n", (const char*)text);
    }

    for (int i = 0; i < res->factor_count; i++) {
        FactorResultNative f;
        unsigned char explanation[512];
        memset(&f, 0, sizeof f);

        if (hta_get_factor(i, &f) != 0)
            continue;

        printf("  %s [%s]", (const char*)f.name_buffer, factor_status_name(f.status));
        if (f.has_value)
            printf(" = %.4f", f.value);
        printf(" (%s)\n", (const char*)f.source_buffer);

        if (hta_get_factor_explanation(i, explanation, (int)sizeof explanation) >= 0)
            printf("    %s\n", (const char*)explanation);
    }
}

int main(void)
{
    InstallationContextNative ctx;
    SelectionInputNative      input;
    CalculationResultNative   res;
    int rc;

    /* Zero-initialize: every has_xxx flag is 0 (not provided) by default. */
    memset(&ctx,   0, sizeof ctx);
    memset(&input, 0, sizeof input);

    /* Installation: free air, 40 degrees C ambient, one cable per phase. */
    ctx.has_environment               = 1; ctx.environment               = 0;  /* FreeAir */
    ctx.has_ambient_air_temperature_c = 1; ctx.ambient_air_temperature_c = 40.0;
    ctx.has_cables_per_phase          = 1; ctx.cables_per_phase          = 1;
    ctx.has_insulation_type           = 1; ctx.insulation_type           = 1;  /* PR/EPR */
    ctx.has_cable_construction        = 1; ctx.cable_construction        = 0;  /* ThreeCoreCable */
    ctx.arrangement_type              = 0;                                   /* Unspecified */

    /* Selection of the reference table: copper, PR/EPR, three-core cable. */
    input.has_cable_construction  = 1; input.cable_construction  = 0;  /* ThreeCoreCable */
    input.has_electric_field_type = 1; input.electric_field_type = 0;  /* Radial */
    input.has_insulation_type     = 1; input.insulation_type     = 1;  /* PR/EPR */
    input.has_conductor_material  = 1; input.conductor_material  = 0;  /* Copper */
    input.has_arrangement_type    = 1; input.arrangement_type    = 0;  /* Unspecified */

    /* 1. Global correction factor */
    memset(&res, 0, sizeof res);
    rc = hta_calculate_global_factor(&ctx, &res);
    if (rc != 0) { printf("hta_calculate_global_factor failed: %d\n", rc); return 1; }
    printf("--- Global correction factor ---\n");
    print_details(&res);

    /* 2. Corrected allowable current for a 25 mm2 section */
    memset(&res, 0, sizeof res);
    rc = hta_calculate_allowable_current(25.0, &input, &ctx, &res);
    if (rc != 0) { printf("hta_calculate_allowable_current failed: %d\n", rc); return 1; }
    printf("--- Allowable current, 25 mm2 ---\n");
    print_details(&res);

    /* 3. Minimum section for a desired current of 80 A */
    memset(&res, 0, sizeof res);
    rc = hta_calculate_minimum_section(80.0, &input, &ctx, &res);
    if (rc != 0) { printf("hta_calculate_minimum_section failed: %d\n", rc); return 1; }
    printf("--- Minimum section for 80 A ---\n");
    print_details(&res);

    return 0;
}
```

Notes:
- Each factor reads only the fields it needs. A field that is not provided makes the matching factor `Missing`, so the status is `Partial` and a warning is returned. The list of factors with their status and explanation shows exactly what is still missing.
- Call `print_details` (or any `hta_get_*` function) right after the calculation it refers to, because the details of the last result are overwritten by the next calculation.
- Check `rc` first (call succeeded), then `res.status` (business outcome), then the `has_xxx` flags before reading a value.

## Delphi

A Delphi binding
