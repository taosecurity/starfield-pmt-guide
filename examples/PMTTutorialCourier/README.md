# A Courier's Request (PMTTutorialCourier)

A Courier's Request is the quest mod from the PMT guide's tutorial. It is a small, unvoiced quest with three stages:

1. Stage 10: the first time you arrive in New Atlantis, a courier's note is added to your inventory, and the
   objective "Read the courier's note" appears.
2. Stage 20: when you read the note, the objective "Collect 5 Iron" appears.
3. Stage 30: when you hold 5 Iron, the workshop pays you 100 credits, the quest completes (it moves to Completed in the Missions menu), and about 3 seconds later the HUD says so.
   You keep the Iron.

- Version 0.1.1 (untested in game; it fixes two problems found in the play-test of 0.1.0, see CHANGELOG.md).
- Plugin: `PMTTutorialCourier.esm`, a light master. Its only master is Starfield.esm.
- It edits no vanilla records and places nothing in vanilla cells. It adds one quest (with one alias, the player),
  one note, one HUD message and a "Show Debug Messages (PC only)" switch in its own Gameplay Options group.
- The quest is listed under Activities in the Missions menu. The end message is a Message record, so it also shows
  on Xbox and PlayStation.

## Install

Install `PMTTutorialCourier-0.1.1.zip` with Vortex or Mod Organizer 2, or copy `PMTTutorialCourier.esm` and
`PMTTutorialCourier - Main.ba2` into the game's `Data` folder and add the plugin to your load order. A new
character is the cleanest test. On a character who is already in New Atlantis, the quest starts the next time you
arrive there.

## Settings

Settings > Gameplay Options > A Courier's Request (PMT Tutorial) > Show Debug Messages (PC only). It is off by
default. When it is on, the mod's log lines also show on screen (PC only).

## If a stage doesn't start

Open the console and type `getstage PTCR_Quest` to see the stage. To move the quest on by hand, type
`setstage PTCR_Quest 10`, `setstage PTCR_Quest 20` or `setstage PTCR_Quest 30`.

## Turning on Papyrus logging

The mod writes what it does to the Papyrus log, in lines that start with `[PTCR]`. The log is written only when
logging is on. Add these lines to `StarfieldCustom.ini` in `Documents\My Games\Starfield` (make the file if it isn't
there):

```
[Papyrus]
bEnableLogging=1
bEnableTrace=1
bLoadDebugInformation=1
```

Start the game, play, then quit. The log is `Documents\My Games\Starfield\Logs\Script\Papyrus.0.log`. Look for lines
starting `[PTCR]`, such as `[PTCR] stage 20: note read, objective 20 shown`. Xbox and PlayStation can't write the
Papyrus log or show debug lines.

## Build it yourself

You need the .NET 9 SDK, Starfield on Steam with the Creation Kit installed (for its Papyrus compiler and
Archive2), sf-mod-build 0.4.2 and sf-mod-check 0.16.0. Then, in this folder:

```
sf-mod-build doctor
sf-mod-build build
```

`build` compiles the two scripts in `Scripts\Source\PTCR`, runs the builder in `Builder` (BuildKit 0.5.0 and Mutagen
0.54.4; the quest's stages, objectives and alias are written with plain Mutagen calls), packs the scripts into
`dist\PMTTutorialCourier - Main.ba2`, writes `PMTTutorialCourier-0.1.1.zip` and checks it. With `"xbox": true` in
`sfmod.json`, it also writes the Creations set, with `PMTTutorialCourier - Main_xbox.ba2`, into `creations-out`.
`WALKTHROUGH.md` has every command and its output, and `PMTTutorialCourier.testplan.md` is the test plan.

## Credits and license

Made for the PMT guide. The builder uses BuildKit and Mutagen (GPL-3.0); the mod's own plugin, scripts and text are
under the MIT license (see LICENSE). Starfield, the Creation Kit, its Papyrus compiler and Archive2 belong to
Bethesda Softworks.
