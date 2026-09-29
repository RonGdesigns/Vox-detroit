# Sports, Game Days and Watchable Matches

Sports are a major part of the Detroit city simulation.

The project uses fictional professional leagues, teams and venue branding for the commercial base game. Real Detroit geography can inspire placement and urban flow, while names, logos, uniforms, mascots, signage and presentation remain original unless separately licensed/cleared.

## Working Detroit teams

These are **working names**, not final trademark clearances:

- Football — **Detroit Chow Dogs**
- Baseball — **Detroit Cheetahs**
- Basketball — **Detroit Stallions**
- Hockey — **Detroit Blue Wings**

The current prototype deliberately gives them color palettes and mascot concepts different from the real Detroit teams.

Blue Wings needs particular reconsideration/clearance because:
- the construction is close to Detroit's existing Red Wings name,
- an active Canadian junior hockey team already uses Perth Blue Wings.

Cheetahs also requires clearance; a Michigan Cheetahs cricket team is currently active in the Detroit area.

## Fictional venue branding

Prototype venues:

- **Brush Street Stadium** — football
- **Motor City Ballpark** — baseball
- **Woodward Arena** — basketball + hockey

These are placeholders.

The final environments can occupy analogous Downtown roles without reproducing sponsor branding or protected team identity.

## Game-day city effects

A home game changes:

- pedestrian density,
- traffic,
- parking demand,
- rideshare/taxi demand,
- restaurant/bar demand,
- retail,
- security staffing,
- hotels,
- vendors,
- delivery difficulty,
- story encounters,
- underground opportunity/exposure.

A 60,000-person football game should affect a much larger area than a 6,000-person concert.

## Watchable games

The game has two layers.

### Sports simulation layer

Responsible for:
- schedule,
- teams,
- team strength,
- scores,
- quarters/periods/innings,
- scoring moments,
- game clock,
- final result.

Results are deterministic from the game's seed so saving/loading does not reroll the score.

### Unity visualizer

Responsible for:
- athletes,
- ball/puck,
- formations,
- basic sport-specific animation,
- crowd reaction,
- scoreboard,
- camera presentation.

The visualizer follows the simulation timeline.

This means we do **not** need Madden/2K/NHL-level sport simulation to let the player attend and watch believable games.

The first target is convincing spectator entertainment:
- teams move with sport-appropriate behavior,
- possessions/plays develop,
- scoring moments match the authoritative score,
- the crowd reacts,
- the scoreboard stays synchronized.

## Spectator experience

The player should be able to:

- buy/receive a ticket,
- enter the venue,
- find a seat,
- buy concessions/merchandise,
- watch the match,
- leave early,
- meet NPCs,
- receive story calls/messages during the game,
- attend with another NPC,
- work the event instead of watching.

Premium or story-related access can expose:
- suites,
- club areas,
- tunnels/backstage,
- media areas,
- staff corridors.

## Jobs around sports

### Security
Gate/security/patrol/event-response shifts.

### Food service
Stadium concessions and nearby restaurant rushes.

### Taxi/rideshare
Pre-game arrival and post-game surge.

### Parking
Temporary event work and traffic management.

### Delivery
Concessions, restaurants and surrounding businesses require supplies.

### Media/photography
Press assignments, crowd shots and game-day content.

### Retail
Team merchandise in the fictional league.

### Underground path
Large crowds can increase abstract opportunity as well as heat/exposure.

## Crowd representation

A 60,000-person event cannot mean 60,000 fully simulated NPCs.

The crowd planner splits attendance into:

1. **Full NPCs** near the player.
2. **Lightweight agents** in nearby concourses/areas.
3. **Visual crowd instances** for stands and far-distance masses.

At a typical 100% performance budget, the prototype caps expensive full NPCs around 120 and lightweight agents around 900, with everyone else represented visually.

The budget can scale with hardware.

## Visual event overlays

Sports games can temporarily add:

- security barriers,
- queue lines,
- temporary parking control,
- vendor setups,
- tailgate zones,
- portable lighting,
- pedestrian-control objects.

Festival overlays use the same system for stages, tents and booths.

## Initial schedule

The prototype seeds four home games several in-game days apart:
- Chow Dogs football
- Cheetahs baseball
- Stallions basketball
- Blue Wings hockey

This is enough to prove game-day traffic/crowd/watch systems before building full fictional seasons.
