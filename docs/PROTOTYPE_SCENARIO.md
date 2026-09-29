# First Playable Scenario

The repository now contains an engine-independent starter-state factory:

`PrototypeScenarioFactory.Create()`

This does not replace the final narrative or real location placement. It creates a coherent simulation state that Unity can bind to immediately.

## Starting state

The player begins with:
- **$60 cash**
- **$400 checking**
- a phone
- a modest Downtown starter apartment
- no owned vehicle
- no owned business
- no active job

The low starting balance is intentional: the first slice should quickly demonstrate why work matters.

## Recurring pressure

Prototype monthly obligations:
- apartment rent: **$950**
- utilities: **$125**

Both become due after the first 30 in-game days. These values are gameplay placeholders and should be balanced later rather than treated as real-world Detroit market claims.

## First economy location

A fictional prototype hardware store is seeded with:
- lumber,
- brick,
- glass,
- a basic tool kit.

The store has finite stock and its own finance account, so purchases move money through the simulation instead of deleting it.

## First job hook

The first offered task is:

**First Downtown Delivery**

It belongs to the entry-level delivery job, has a same-day deadline, pays **$18**, and awards job reputation.

Unity will later attach the task's abstract origin/destination IDs to physical map locations.

## First NPC hooks

Two placeholder roles are seeded:
- **Neighbor**
- **Delivery Dispatcher**

The dispatcher has a weekday work schedule. Names and characters are intentionally generic until the narrative cast is designed.

## Story seed

Initial story state:
- `intro.arrived = true`
- `life.stability = 0`

This gives the first narrative events concrete conditions to build from.

## Why this exists now

When the Unity scene comes online, we should be able to wire UI/interactions into an already functioning loop:

arrive → look for work → accept delivery → travel → complete task → get paid → buy materials / cover expenses → unlock more opportunities.

We should not spend the first Unity session inventing backend state that can be built and tested beforehand.
