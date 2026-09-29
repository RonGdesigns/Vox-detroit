# Testing Strategy

## Before Unity is available

The repository includes a standalone .NET smoke project:

`tools/DomainSmoke/DomainSmoke.csproj`

It links the engine-independent source files directly from `Assets/VoxDetroit/Scripts` and checks:
- game clock math,
- negative voxel/chunk coordinates,
- money transfers,
- job wages,
- property purchase/renovation,
- recurring obligations,
- business cashflow,
- NPC schedules,
- story events,
- reputation,
- voxel deltas,
- synthetic Detroit GIS rasterization.

GitHub Actions runs this smoke project on pushes to `main` and on pull requests.

This does **not** prove Unity rendering/physics/editor compatibility. It catches a different class of errors early: domain compilation, deterministic math and state transitions.

## When Unity is available

Add Unity EditMode tests for:
- JSON save/load round-trips,
- Unity serialization assumptions,
- mesh generation,
- importer parsing,
- material assignment.

Add PlayMode tests for:
- chunk streaming,
- chunk-boundary seams,
- collider behavior,
- interaction flow,
- scene transitions.

## Rule

A gameplay rule that does not need Unity should not require Unity to test it.
