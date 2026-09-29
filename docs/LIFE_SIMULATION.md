# Life Simulation Foundation

Vox Detroit's life systems are deliberately data-first so they can be simulated without requiring every object to exist as a Unity GameObject.

## Time

`GameClock` stores time as total in-game minutes.

This gives jobs, rent, NPC schedules, businesses and story conditions a shared deterministic clock.

## Money

Money is stored as integer cents. Gameplay money should never use floating-point values.

Initial prototype accounts:
- player cash
- player checking
- player savings

Every credit/debit/transfer can create a ledger transaction.

## Jobs

The prototype catalog currently defines:
- delivery,
- construction,
- auto shop,
- restaurant,
- photography,
- security,
- rideshare.

Jobs track reputation, completed tasks, minutes worked and lifetime earnings.

A shift has an explicit start time and computes wages from actual in-game minutes when ended.

## Recurring obligations

Rent, utilities, insurance, loan payments and similar expenses use `ObligationRecord`.

An obligation contains:
- amount,
- interval,
- next due time,
- payer account,
- missed-payment count,
- related entity.

This lets the economy create real pressure without hard-coding rent logic into the apartment UI.

## Property

Properties have:
- use type,
- owner,
- market/purchase price,
- rent,
- condition,
- enterable flag,
- installed renovations.

Renovations can consume money, improve condition and increase value.

Later the renovation definition will also require physical materials and completed voxel work.

## Businesses

Businesses have their own finance account and persistent state.

The first simulation model tracks:
- open/closed,
- reputation,
- employees,
- baseline daily revenue,
- daily expenses,
- lifetime revenue/expense.

This is intentionally simple. The first restaurant/contractor/shop vertical slice should replace generic revenue with gameplay-driven inputs.

## NPC schedules

NPC schedules resolve by day-of-week and minute-of-day. Important NPCs can therefore move between home, work and activities without writing custom behavior for every story character.

## Reputation

Reputation is context-based rather than a single morality score.

Examples:
- `career:construction`
- `neighborhood:downtown`
- `business:restaurant`
- `contact:<npc-id>`

## Story

Story events are data-driven:
- conditions inspect flags/variables,
- actions mutate flags/variables,
- events can be one-shot.

Jobs, reputation, properties and NPC relationships can therefore expose different story routes without creating separate campaign scripts.

## Persistence

The save schema stores the simulation state plus voxel deltas relative to generated/imported city data.

The project should not save a full copy of Detroit. It saves what changed.
