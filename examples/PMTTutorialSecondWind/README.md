# Second Wind (PMTTutorialSecondWind)

Second Wind is the mechanics mod from the PMT guide's tutorial. When an enemy hits you while your health is below
25 percent in combat, you regenerate 10 Health a second for 10 seconds, and the HUD says "Second wind!". It can
happen once every 120 seconds.

- Version 0.1.0 passed its in-game test on 2026-11-24.
- Plugin: `PMTTutorialSecondWind.esm`, a light master. Its only master is Starfield.esm.
- It edits no vanilla records. It adds one quest that starts with the game, one magic effect, one spell, one HUD
  message and a "Show Debug Messages (PC only)" switch in its own Gameplay Options group.
- The HUD message is a Message record, so it also shows on Xbox and PlayStation.

## Install

Install `PMTTutorialSecondWind-0.1.0.zip` with Vortex or Mod Organizer 2, or copy `PMTTutorialSecondWind.esm` and
`PMTTutorialSecondWind - Main.ba2` into the game's `Data` folder and add the plugin to your load order. A new
character is the cleanest test.

To remove it, uninstall the zip. The quest's script stops running, and nothing else in your game was changed.

## Settings

Settings > Gameplay Options > Second Wind (PMT Tutorial) > Show Debug Messages (PC only). It is off by default.
When it is on, the mod's log lines also show on screen (PC only).

## Turning on Papyrus logging

The mod writes what it does to the Papyrus log, in lines that start with `[PTSW]`. The log is written only when
logging is on. Add these lines to `StarfieldCustom.ini` in `Documents\My Games\Starfield` (make the file if it isn't
there):

```
[Papyrus]
bEnableLogging=1
bEnableTrace=1
bLoadDebugInformation=1
```

Start the game, play, then quit. The log is `Documents\My Games\Starfield\Logs\Script\Papyrus.0.log`. Look for lines
starting `[PTSW]`, such as `[PTSW] health at 0.210000, second wind starts; cooldown 120.000000 s`. Xbox and
PlayStation can't write the Papyrus log or show debug lines.

## Build it yourself

You need the .NET 9 SDK, Starfield on Steam with the Creation Kit installed (for its Papyrus compiler and
Archive2), sf-mod-build 0.4.1 and sf-mod-check 0.15.2. Then, in this folder:

```
sf-mod-build doctor
sf-mod-build build
```

`build` compiles `Scripts\Source\PTSW\SecondWindQuestScript.psc`, runs the builder in `Builder` (BuildKit 0.5.0 and
Mutagen 0.54.4), packs the script into `dist\PMTTutorialSecondWind - Main.ba2`, writes `PMTTutorialSecondWind-0.1.0.zip`
and checks it. With `"xbox": true` in `sfmod.json`, it also writes the Creations set, with
`PMTTutorialSecondWind - Main_xbox.ba2`, into `creations-out`. `WALKTHROUGH.md` has every command and its output.

## Credits and license

Made for the PMT guide. The builder uses BuildKit and Mutagen (GPL-3.0); the mod's own plugin, script and text are
under the MIT license (see LICENSE). Starfield, the Creation Kit, its Papyrus compiler and Archive2 belong to
Bethesda Softworks.
