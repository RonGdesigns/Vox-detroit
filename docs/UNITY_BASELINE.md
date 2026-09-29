# Unity Baseline

## Production baseline

Target the **Unity 6.3 LTS** family for the initial Vox Detroit production project.

Why:
- it is an LTS release,
- Unity lists support through December 2027,
- it is appropriate for a commercial project that values a stable production baseline.

Do not pin an exact patch number in source control until the project is opened in Unity Hub and the installed 6.3 LTS patch is confirmed.

## Rendering

Start with URP unless a prototype proves that another pipeline materially improves the project.

The voxel system should not hard-depend on URP-specific code outside the presentation/material layer.

## Project packages

Do not add packages merely because they might be useful later.

Likely early packages after the editor project exists:
- Input System
- Test Framework
- AI Navigation if/when non-voxel navigation needs it

DOTS/Entities should be introduced only for a measured simulation bottleneck, not as a prerequisite for M0/M1.

## Rule

Domain systems (money, jobs, story, property, schedules) should stay as engine-independent as practical.

Unity components should adapt those systems to scenes, UI, audio, physics and input.
