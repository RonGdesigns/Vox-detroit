# Visual Crowd and Event Overlay Prototype

Two Unity-side prototype components now exist.

## PrototypeCrowdVisual

Purpose:
- visually represent thousands of attendees without thousands of full NPC GameObjects.

Technique:
- generates a very low-cost crossed-billboard person mesh,
- distributes instances deterministically inside a configurable area,
- renders them with GPU instancing in batches,
- caps visual instance count while preserving the represented attendance number.

The domain-level CrowdRepresentationPlanner still decides how many nearby people should eventually be full NPCs or lightweight agents.

The visual crowd component represents the large remainder.

## PrototypeEventOverlay

Purpose:
- prove that venues can physically transform for festivals and game days.

It can create temporary prototype:
- stages,
- vendor booths,
- security checkpoints,
- barrier perimeters,
- tailgate zones.

The generated objects are deliberately simple boxes. They are layout/debug geometry, not final art.

## First Unity use

For Hart Plaza:
1. Create an empty GameObject at the event area.
2. Add PrototypeCrowdVisual.
3. Set expected attendance.
4. Add PrototypeEventOverlay.
5. Choose festival settings.
6. Rebuild both components.

For a stadium:
1. Place a crowd zone in a concourse/tailgate exterior test area.
2. Set PrototypeEventOverlay to sportsGame.
3. Set expected attendance to the scheduled game's expected attendance.
4. Rebuild.

Inside the stadium, later crowd zones should be split by seating section rather than one giant rectangle.

## Next visual step

After the first world render works:
- replace billboard people near the player with animated lightweight agents,
- define venue-specific crowd-zone polygons,
- connect zones automatically to active CityEvent/SportsGame records,
- drive crowd reactions from performer sets and SportsGameMoment excitement values.
