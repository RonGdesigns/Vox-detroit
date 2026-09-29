# City Events, Festivals and Concerts

Detroit should feel different on a major event weekend than on an ordinary Tuesday.

Hart Plaza is an especially important reference point. The City of Detroit describes it as a long-running outdoor cultural, musical, entertainment and special-event space on the riverfront. The real city calendar regularly uses it for large music, cultural and community gatherings.

The game should therefore simulate events as temporary changes to the city rather than decorative stage props.

## Event types

The system supports:
- music festivals,
- concerts,
- cultural festivals,
- pride festivals,
- food festivals,
- markets,
- parades,
- fireworks,
- motorsport weekends,
- community events,
- watch parties,
- nightlife events.

## What an event changes

An active event can affect:
- pedestrian density,
- traffic density,
- road closures later,
- taxi/rideshare demand,
- parcel/food delivery demand,
- restaurant demand,
- retail demand,
- hotel demand,
- security staffing,
- temporary vendor work,
- photography/media jobs,
- cleanup work,
- nightlife activity,
- story encounters,
- underground-opportunity frequency.

The intent is systemic cross-talk. A festival should matter even when the player never buys a ticket.

## Crowd simulation

Do not spawn every attendee as a fully simulated NPC.

Use tiers:

### Near the player

Full NPCs:
- collision,
- animation,
- reactions,
- conversations where relevant,
- queues,
- pathfinding.

### Mid-distance

Lightweight crowd agents:
- simplified navigation,
- basic animations,
- no expensive individual schedules.

### Far distance / packed stage views

Visual crowd representation:
- GPU-instanced figures,
- impostors,
- grouped motion,
- audio crowd beds.

The event stores an expected-attendance value, but rendering chooses an appropriate representation for the machine and camera distance.

## Temporary event geometry

Large events can temporarily add:
- stages,
- barriers,
- tents,
- vendor booths,
- portable lighting,
- signs,
- security checkpoints,
- food-truck zones,
- temporary seating,
- backstage areas.

These should be event overlays rather than permanent voxel edits unless the story requires lasting construction.

## Performers and celebrity system

The base game should use fictional performers.

Each performer has:
- stage name,
- genre,
- fame,
- home region,
- traits,
- an original track catalog.

They can:
- tour through Detroit,
- headline festivals,
- make surprise guest appearances,
- cancel,
- become involved in story missions,
- hire security/drivers/media,
- affect attendance and ticket demand.

A future licensed real-world performer can use the same data structure.

## Original music

Performance tracks are represented by metadata:
- title,
- genre,
- BPM,
- duration,
- audio asset key.

The actual audio can be generated or commissioned as original game music.

Do not imitate a living artist's identifiable voice or copy existing songs. The fictional-artist catalog gives us freedom to create a recognizable in-game music scene without depending on celebrity licensing.

## Job connections

### Taxi / rideshare

Events create:
- pre-show surges,
- post-show surges,
- road congestion,
- premium trips,
- difficult pickup zones.

### Food / restaurants

Nearby restaurants get rush periods.
Festival vendors create temporary jobs.

### Security

Events create:
- gates,
- patrols,
- backstage access,
- crowd incidents,
- lost-person/property calls.

### Photography / media

Assignments can include:
- crowd shots,
- performer photos,
- press access,
- backstage interviews,
- festival recap work.

### Delivery

Temporary vendors/stages need supplies.
Road closures complicate ordinary routes.

### Underground path

Large crowds can increase opportunities but also exposure and consequence risk. The system changes abstract opportunity/heat conditions rather than providing real-world criminal operating instructions.

## Event story hooks

Examples:
- a performer disappears before their set,
- equipment goes missing,
- a storm threatens an outdoor festival,
- a stage loses power,
- rival promoters clash,
- an NPC friend works the event,
- a player-owned restaurant receives a huge rush,
- a taxi passenger becomes a story contact,
- a celebrity hires the player for a private job,
- a major event changes access to part of Downtown.

## Prototype fictional artist roster

- **Nova Ash** — R&B / soul
- **Static Saint** — Detroit techno
- **Jae Meridian** — hip-hop
- **Blue Orbit Quartet** — jazz
- **Velvet Engine** — fusion
- **Sunday Signal** — gospel

Their track names and performance metadata already exist in the prototype catalog. Audio comes later.

## Prototype events

The code includes fictional test events such as:
- River Pulse Weekend at Hart Plaza,
- Jazz on the River at Hart Plaza,
- World Roots Celebration at Hart Plaza,
- Market Night Live at Eastern Market.

These names are original placeholders. Their purpose is to prove crowd/demand/job systems before final worldbuilding.
