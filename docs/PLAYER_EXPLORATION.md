# Prototype Player Exploration

The Downtown prototype scene now creates a controllable first-person player instead of a fixed aerial camera.

## Controls

- **WASD** — move
- **Mouse** — look
- **Left/Right Shift** — sprint
- **Space** — jump
- **F2** — toggle debug free-fly
- **Space** while flying — rise
- **Ctrl** while flying — descend
- **F3** — exit fly mode and snap to the lowest nearby walkable surface
- **Esc** — release cursor
- **Left click** — recapture cursor

## Spawn behavior

The scene starts the player above the Downtown prototype near the geographic anchor.

At runtime, after the imported voxel colliders exist, the controller probes a grid of nearby points from above and selects the lowest upward-facing surface it can find. This intentionally prefers street/ground height over nearby rooftops.

F3 repeats this process if the player gets stuck during prototype development.

## Streaming

The VoxelWorldStreamer now follows the player transform.

Walking or flying across chunk boundaries therefore becomes the first real test of:
- chunk residency,
- collider creation/removal,
- neighbor face culling,
- frame-rate behavior,
- tall-building vertical loading.

## Scope

This controller is a development controller, not the final movement system. It exists so every subsequent gameplay system can be tested from street level immediately.
