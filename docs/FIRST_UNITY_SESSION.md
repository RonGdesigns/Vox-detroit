# First Unity Session

The goal of the first editor session is **verification**, not feature development.

## Editor baseline

Use Unity **6.3 LTS**. Do not spend the first session migrating the project to a newer non-LTS Unity line.

Unity 6.3 is the project's production baseline. The exact installed 6.3 patch can be recorded after the project is first opened successfully.

## First steps

1. Clone/pull `RonGdesigns/Vox-detroit`.
2. Open the repository as the Unity project/workspace.
3. Allow Unity to import/compile the assets.
4. Fix any compiler errors before doing scene work.
5. In Unity's menu, choose:
   **Vox Detroit → Setup Downtown Prototype Scene**
6. Open/save the generated scene if Unity does not already show it.
7. Enter Play Mode.

The setup command:
- creates a camera,
- creates a directional light,
- creates the voxel world streamer,
- points the streamer at the camera,
- assigns the committed Downtown Detroit JSON,
- disables fake procedural test chunks,
- saves `Assets/VoxDetroit/Scenes/DowntownPrototype.unity`.

## What should appear

The first render is intentionally prototype-grade.

Expected:
- real Downtown road alignment,
- real building footprints,
- building heights from map data where available,
- taller buildings extending across multiple vertical chunks,
- different placeholder colors for asphalt, concrete and brick,
- streamed geometry around the camera.

Do **not** expect finished Detroit architecture yet.

## First verification checklist

- [ ] Zero C# compiler errors
- [ ] Downtown JSON parses
- [ ] Streamer reports resident chunks
- [ ] Roads appear in plausible alignment
- [ ] Cadillac Tower is not clipped at ~96 m
- [ ] No obvious vertical chunk truncation
- [ ] Adjacent loaded chunks do not expose internal faces
- [ ] Camera sees a recognizable Downtown massing pattern
- [ ] Scene remains responsive at render radius 6
- [ ] Console has no repeated exceptions

## If performance is poor

Do not reduce city accuracy first.

The next optimization order is:
1. reduce render radius temporarily,
2. implement greedy meshing,
3. move mesh generation off the main frame where safe,
4. replace prototype per-block submeshes/materials with an atlas/batched shader,
5. add distance LOD.

The prototype material system deliberately favors visual debugging over draw-call efficiency.

## After the first successful render

Take screenshots from:
- high aerial view,
- street-height view near Woodward,
- view toward the tallest imported buildings.

Then we compare the generated geometry against the real map and tune:
- road width fallbacks,
- building heights,
- architecture rules,
- camera/player scale.
