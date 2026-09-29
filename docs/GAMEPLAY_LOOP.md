# Gameplay Loop Architecture

The first playable slice should connect systems rather than present them as menus.

## Core loop

Need money / opportunity
→ Find or accept work
→ Receive a physical task
→ Travel through Detroit
→ Perform the task
→ Get paid + reputation
→ Spend on needs/materials/transport/property
→ Unlock better work, relationships and story routes
→ Change the city / player's position in it

## Job tasks

A job is the player's ongoing career/employer relationship.

A job task is a specific playable assignment.

Examples:
- delivery: pickup → destination → handoff,
- construction: acquire materials → reach property → complete repair/build interactions,
- mechanic: inspect vehicle → obtain part → install/test,
- photography: reach location → meet shot requirements → submit,
- security: reach post → patrol/resolve event,
- rideshare: collect passenger → drive to destination.

JobTaskRecord stores the persistent assignment state without deciding how Unity presents the objective.

## Inventory and materials

The inventory is intentionally generic.

Construction/renovation can require lumber, brick, glass, fixtures and tools. Mechanic jobs can require vehicle parts.

Stores can sell those items using the same finance ledger as every other money transaction.

## Stores

A store contains persistent stock and prices.

Buying:
1. checks stock,
2. transfers/debits money,
3. adds inventory,
4. reduces finite stock.

Later the physical store UI, clerk NPC and shelves all call this same domain operation.

## Simulation session

SimulationSession is the main engine-independent facade over the current save.

It owns:
- game clock,
- finance,
- recurring bills,
- employment,
- job tasks,
- inventory,
- stores,
- property,
- businesses,
- reputation,
- story.

Advancing game time also expires overdue tasks and processes due obligations.

This gives Unity one coherent simulation entry point instead of scenes directly mutating save-data lists.
