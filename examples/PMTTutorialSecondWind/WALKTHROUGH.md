# Walkthrough: building Second Wind (PMTTutorialSecondWind) with public PMT tools

This is the record of one real run, made on 2026-11-24, for the guide's page M3 ("Build a mechanics mod for PC and
console"). Every command below was run exactly as shown, in a Command Prompt (`cmd.exe`), with the tool folders on
PATH. Each command has a short excerpt of its real output and its exit code. The page writers can copy the commands
from here.

**Paths in this file.** The run used scratch folders. To keep private folder names out of the guide, the excerpts
write the mod's parent folder as `C:\Mods`, each unzipped tool as `C:\Tools\<tool>-<version>`, a folder for
throwaway reports as `C:\Scratch`, and the user's own folders as `%USERPROFILE%` and `%LOCALAPPDATA%`. Nothing else in the output was changed; `...` marks lines left out.

## Tools and versions

| Tool | Version | Where it came from |
|---|---|---|
| sf-mod-build | 0.4.1 | `sf-mod-build-0.4.1-win-x64.zip` from the GitHub release (SHA-256 checked) |
| sf-mod-build starter template | 0.4.1 | `sf-mod-build-template-0.4.1.zip` from the same release (SHA-256 checked) |
| BuildKit | 0.5.0 | NuGet package `Taosecurity.Starfield.BuildKit` (the build restores it) |
| Mutagen.Bethesda.Starfield | 0.54.4 | NuGet (BuildKit's pin) |
| sf-mod-check | 0.15.2 | `sf-mod-check-0.15.2-win-x64.zip` from the GitHub release (SHA-256 checked) |
| sf-papyrus-ref | 0.2.3 | `sf-papyrus-ref-0.2.3.zip` from the GitHub release (SHA-256 checked) |
| sf-test-plan | 0.4.0 | `sf-test-plan-0.4.0.zip` from the GitHub release (SHA-256 checked) |
| sf-cut-check | 0.2.0 | `sf-cut-check-0.2.0-win-x64.zip` from the GitHub release (SHA-256 checked) |
| Starfield, Creation Kit | Steam | PapyrusCompiler 4.7.0.5, Archive2 and ContentResources.zip from the Creation Kit |
| .NET SDK | 9.0.321 (10.0.404 also installed) | Microsoft |
| Python | 3.13 | python.org |

## 0. Set up the tools (once)

Check each download, for example:

```
certutil -hashfile sf-mod-build-template-0.4.1.zip SHA256
```

The hash matched the release's `.sha256` file (`f03eb35f3f3d7ab1c243665b66913d2be13fe5fcb9cb161ac20f072d8910aa3d`).
Unzip each tool into a folder of its own. **Doc gap:** the sf-papyrus-ref zip holds a folder of its own, so
unzipping it into `C:\Tools\sf-papyrus-ref-0.2.3` gives `C:\Tools\sf-papyrus-ref-0.2.3\sf-papyrus-ref-0.2.3`; the
README only says "unzip it anywhere".

Put the tools on PATH for this Command Prompt window:

```
set PATH=C:\Tools\sf-mod-build-0.4.1;C:\Tools\sf-mod-check-0.15.2;C:\Tools\sf-papyrus-ref-0.2.3\sf-papyrus-ref-0.2.3;C:\Tools\sf-test-plan-0.4.0;C:\Tools\sf-cut-check-0.2.0;%PATH%
```

With that, `sf-papyrus-ref` and `sf-test-plan` run through the `.cmd` launchers in their folders, from any folder.

Install the starter template (the folder where `sf-mod-build-template-0.4.1.zip` was unzipped):

```
dotnet new install C:\Tools\sfmod-template
```

```
Success: C:\Tools\sfmod-template installed the following templates:
Template Name                 Short Name  Language  Tags
----------------------------  ----------  --------  -----------------------------
Starfield mod (sf-mod-build)  sfmod       [C#]      Starfield/Mod/Papyrus/Mutagen
```

Exit code 0. Then:

```
sf-mod-build --version
```

```
sf-mod-build 0.4.1 by Richard Bejtlich - GPL-3.0 - https://github.com/taosecurity/sf-mod-build
```

Exit code 0. `sf-mod-check --version` printed `sf-mod-check 0.15.2 ...`, exit code 0.

Build sf-papyrus-ref's local cache once, so lookups show signatures and follow parent scripts:

```
sf-papyrus-ref build-local
```

```
It read 1702 scripts (8381 functions, events and structs) from C:\Program Files (x86)\Steam\steamapps\common\Starfield\Tools\ContentResources.zip.
102 of the 103 scripts on the official list are among them.
It saved the local cache to %LOCALAPPDATA%\sf-papyrus-ref\local.json. The cache stays on this machine, so never share it.
```

Exit code 0.

## 1. Make the mod from the starter template

In `C:\Mods`:

```
dotnet new sfmod -n PMTTutorialSecondWind --prefix PTSW --mod-author "PMT Guide"
```

```
The template "Starfield mod (sf-mod-build)" was created successfully.
```

Exit code 0. It made `C:\Mods\PMTTutorialSecondWind` with `sfmod.json`, `Builder\` (the C# builder),
`Scripts\Source\PTSW\StarterQuestScript.psc`, `README.md`, `LICENSE` and `.gitignore`.

```
cd PMTTutorialSecondWind
sf-mod-build doctor
```

```
FOUND    Starfield: C:\Program Files (x86)\Steam\steamapps\common\Starfield (Steam folder)
FOUND    Papyrus compiler: C:\Program Files (x86)\Steam\steamapps\common\Starfield\Tools\Papyrus Compiler\PapyrusCompiler.exe
FOUND    Archive2: C:\Program Files (x86)\Steam\steamapps\common\Starfield\Tools\Archive2\Archive2.exe
FOUND    Script sources (ContentResources.zip): C:\Program Files (x86)\Steam\steamapps\common\Starfield\Tools\ContentResources.zip
FOUND    .NET SDK: C:\Program Files\dotnet\dotnet.exe (SDK 9.0.321, 10.0.402, 10.0.403, 10.0.404)
FOUND    Checker: C:\Tools\sf-mod-check-0.15.2\sf-mod-check.exe (on PATH)
note: another Starfield install was found (Game Pass (XboxGames folder)): C:\XboxGames\Starfield\Content. ...
setup (not needed to build; recommended for testing and the Creation Kit):
WARNING  StarfieldCustom.ini (Papyrus log, loose files): %USERPROFILE%\Documents\My Games\Starfield\StarfieldCustom.ini: the folder %USERPROFILE%\Documents\My Games\Starfield isn't there
...
WARNING  CreationKitCustom.ini (Creation Kit settings): ...: the file isn't there (or this account can't see it)
...
WARNING  Script sources unpacked (for the Creation Kit): ...\Data\Scripts\Source\Starfield_Papyrus_Flags.flg: not there (or this account can't see it)
...
RESULT: OK, ready to build (with 3 WARNINGS: see above)
```

Exit code 0. The three WARNINGs are setup advice for playing and for the Creation Kit (this build account had never
started the game); none stops a build. Each one prints the exact lines to add.

The unchanged starter builds on the first try (`sf-mod-build validate`, exit 0, then `sf-mod-build build`, exit 0,
ending `RESULT: OK, built C:\Mods\PMTTutorialSecondWind\PMTTutorialSecondWind-0.1.0.zip`; the starter plugin was 861
bytes with 3 records).

## 2. Change the starter into Second Wind

1. **BuildKit 0.5.0.** In `Builder\PMTTutorialSecondWind.Builder.csproj`, the template pins BuildKit 0.4.2. Change
   the line to `<PackageReference Include="Taosecurity.Starfield.BuildKit" Version="[0.5.0]" />` and leave
   `Mutagen.Bethesda.Starfield` at `[0.54.4]`. No code change was needed. The next build updates
   `Builder\packages.lock.json`. **Doc gap:** sf-mod-build 0.4.1's README and template still say "BuildKit 0.4.2 is
   the BuildKit version sf-mod-build 0.4.0 was tested with", and its AGENTS.md says not to change the pin unless the
   person asks. Nothing says whether 0.5.0 is supported; it worked here.
2. **The script.** Delete `Scripts\Source\PTSW\StarterQuestScript.psc` and add
   `Scripts\Source\PTSW\SecondWindQuestScript.psc` (in this folder). It keeps the starter's `Log()` function
   (`Debug.Trace` with the `[PTSW]` prefix, plus `Debug.Notification` only when the debug switch is on), registers
   for hit events on the player, and checks health, combat and a cooldown timer.
3. **The builder.** Replace `Builder\Program.cs` (in this folder). Besides the starter's quest and debug switch, it
   makes the magic effect, the spell and the HUD message, and fills the script's properties.
4. **Xbox archives.** In `sfmod.json`, change `"xbox": false,` to `"xbox": true,`.
5. README and LICENSE (MIT) were rewritten for the mod.

### Every Papyrus name, looked up first

Each function and event in the script was looked up with sf-papyrus-ref before it was written, for example:

```
sf-papyrus-ref lookup ScriptObject.RegisterForHitEvent
```

```
ScriptObject.RegisterForHitEvent  (function)
  Signature (your sources): Function RegisterForHitEvent(ScriptObject akTarget, ScriptObject akAggressorFilter = None, Form akSourceFilter = None, Form akProjectileFilter = None, int aiPowerFilter = -1, int aiSneakFilter = -1, int aiBashFilter = -1, int aiBlockFilter = -1, bool abMatch = true) native protected selfonly
  Flags: Non-delayed
  Official page: https://starfield.creationkit.bethesda.net/w/index.php?title=RegisterForHitEvent_-_ScriptObject
```

Exit code 0. All of these were found on the official list (exit code 0 each):

| Name | Flags | Note |
|---|---|---|
| `ScriptObject.RegisterForHitEvent` | Non-delayed | One hit per registration, so the script registers again after each hit. |
| `ScriptObject.OnHit` | none | The event the registration sends. |
| `Actor.IsInCombat` | none | |
| `ObjectReference.GetValuePercentage` | none | Takes an ActorValue (Health); returns 0 to 1. |
| `ScriptObject.StartTimer`, `ScriptObject.OnTimer` | Non-delayed / none | The 120-second cooldown. |
| `Spell.Cast` | Latent, Non-delayed | Casts the mod's spell on the player. |
| `Message.Show` | Latent | The HUD message; works on consoles. |
| `Game.GetPlayer` | Non-delayed | |
| `GameplayOption.GetValue` | none | Reads the debug switch. |
| `Quest.OnQuestInit` | none | |
| `Debug.Trace` | Non-delayed, DebugOnly | The log line (PC only, by design). |
| `Debug.Notification` | DebugOnly | Only behind the debug switch (PC only, by design). |

### Every vanilla record, looked up by Editor ID

The mod points to two vanilla records and changes neither. **PMT gap:** no PMT command looks a record up by Editor
ID (sf-mod-check, sf-load-order and sf-cut-check have none). The builder does it with BuildKit's
`GameData.Get<T>(editorId)`, which stops the build when the record isn't there, and prints what it found. With
`sf-mod-build build --verbose` (section 3), the builder's line reads:

```
log: vanilla: Health = 0002D4:Starfield.esm, EitherHand = 013F44:Starfield.esm
```

- `Health` (actor value, `0002D4`): what the effect restores and the script reads.
- `EitherHand` (equip type, `013F44`): the equip type the game's own Fire and Forget self spells use.

The effect's shape was copied from the game's own restore-health effects (read with BuildKit's `GameData` in a
throwaway program): archetype Value Modifier, cast type Fire and Forget, target Self, flag No Area. **Doc gap:** in
Mutagen 0.54.4 the Value Modifier's actor value is the effect's `SecondActorValue` property, which nothing in
BuildKit's docs says; BuildKit has `Effects.New` for the spell's effect entry but no helper or example for the
magic effect record itself.

## 3. Build

```
sf-mod-build validate
```

```
project:  C:\Mods\PMTTutorialSecondWind\sfmod.json
mod:      PMTTutorialSecondWind 0.1.0
plugin:   PMTTutorialSecondWind.esm (archive "PMTTutorialSecondWind - Main.ba2")
...
runs:     dotnet run -c Release --project C:\Mods\PMTTutorialSecondWind\Builder\PMTTutorialSecondWind.Builder.csproj -- {data} {dist} {scripts}
...
xbox:     yes, "PMTTutorialSecondWind - Main_xbox.ba2" in C:\Mods\PMTTutorialSecondWind\creations-out
...
RESULT: OK
```

Exit code 0.

```
sf-mod-build build
```

```
== compile
game script sources: %LOCALAPPDATA%\sf-mod-build\vanilla\b5f3f66d...\Scripts\Source
compiled PTSW\SecondWindQuestScript.psc
RESULT: OK, 1 script(s) compiled into C:\Mods\PMTTutorialSecondWind\.build\stage\Scripts
== plugin
running the builder: PMTTutorialSecondWind.Builder.csproj (the first run restores and builds it, which can take a few minutes)
built    C:\Mods\PMTTutorialSecondWind\dist\PMTTutorialSecondWind.esm (1713 bytes)
RESULT: OK, PMTTutorialSecondWind.esm built into C:\Mods\PMTTutorialSecondWind\dist
== pack
packed   C:\Mods\PMTTutorialSecondWind\dist\PMTTutorialSecondWind - Main.ba2 (1737 bytes)
packed   C:\Mods\PMTTutorialSecondWind\creations-out\PMTTutorialSecondWind - Main_xbox.ba2 (1737 bytes)
RESULT: OK, C:\Mods\PMTTutorialSecondWind\dist\PMTTutorialSecondWind - Main.ba2 and the Creations set in C:\Mods\PMTTutorialSecondWind\creations-out made
== zip
zipped   LICENSE-PMTTutorialSecondWind.txt
zipped   PMTTutorialSecondWind - Main.ba2
zipped   PMTTutorialSecondWind.esm
zipped   README-PMTTutorialSecondWind.md
sha256   ac55522d9df128a656950adb0bcd883507a7207869223ce0a8e76436e7d0115c
RESULT: OK, C:\Mods\PMTTutorialSecondWind\PMTTutorialSecondWind-0.1.0.zip written (4 files, 5283 bytes)
== check
checking PMTTutorialSecondWind-0.1.0.zip with sf-mod-check.exe
         sf-mod-check 0.15.2 release (basic profile): PMTTutorialSecondWind-0.1.0.zip (4 files, 1 plugins, 1 archives)
         NOTE     [dmmdeps] DMMDeps wasn't given (--dmmdeps), ...
         NOTE     [plugin-check] PMTTutorialSecondWind.esm: OK, 0 errors, 0 warnings (light master, 1 masters, 6 records, 0 overrides (0 ITM), 8 links)
         NOTE     [mutagen-gap] PMTTutorialSecondWind.esm: 6 own records, 0 overrides, 0 compared with their master's copy
         RESULT: OK (0 problems, 0 to review, 0 warnings)
RESULT: OK, PMTTutorialSecondWind-0.1.0.zip passed the release check
RESULT: OK, built C:\Mods\PMTTutorialSecondWind\PMTTutorialSecondWind-0.1.0.zip
```

Exit code 0. The BA2 and zip bytes and the sha256 change with every compile, because the Papyrus compiler writes the
compile time into each `.pex` (the README says so); the plugin is the same 1713 bytes every time.

The builder's own lines (BuildKit's changes and its gap check) show only with `--verbose`:

```
sf-mod-build build --verbose
```

```
...
log: vanilla: Health = 0002D4:Starfield.esm, EitherHand = 013F44:Starfield.esm
log: ID lock: 6 records, 0 new, 4 moved back to their locked IDs (C:\Mods\PMTTutorialSecondWind\ids.lock.json)
log:   BuildKit: EFIF/MUID filled on 1 effect entries
log:   BuildKit: form version 582 set on 2 own records left at Mutagen's 576
log:   BuildKit: header interior cell count (INCC) set to 0 (was unset)
log:   BuildKit: own records written in Bethesda's FE FormID form (light plugin)
log:   gap check: running C:\Tools\sf-mod-check-0.15.2\sf-mod-check.exe gap
log:   gap check: PMTTutorialSecondWind.esm: 6 own records, 0 overrides, 0 compared with their master's copy
log:   gap check:   0 error(s), 0 warning(s)
log:   gap check: RESULT: OK (0 errors, 0 warnings)
...
```

Exit code 0. "EFIF/MUID filled on 1 effect entries" is BuildKit's `FixEffects` at work on the spell's effect.
**Doc gap:** sf-mod-build's README example of `build` shows no builder lines and doesn't say that `--verbose` is the
way to see BuildKit's lines and the gap check's result during a build.

## 4. Check

```
sf-mod-check check dist\PMTTutorialSecondWind.esm
```

```
sf-mod-check 0.15.2 check: PMTTutorialSecondWind.esm (light master, 1 masters, 6 records, 0 overrides (0 ITM), 8 links)
RESULT: OK, 0 errors, 0 warnings (PMTTutorialSecondWind.esm: 6 records, 8 links; 3.9 s)
```

Exit code 0.

```
sf-mod-check gap dist\PMTTutorialSecondWind.esm
```

```
PMTTutorialSecondWind.esm: 6 own records, 0 overrides, 0 compared with their master's copy
  0 error(s), 0 warning(s)
RESULT: OK (0 errors, 0 warnings)
```

Exit code 0.

### The Papyrus check for Creations

The SPEC's command, `sf-papyrus-ref check Scripts --creations`, with no `--allow`:

```
sf-papyrus-ref check Scripts --creations
```

```
Scripts\Source\PTSW\SecondWindQuestScript.psc:59: error: Debug.Trace is DebugOnly: it does nothing in release builds on Xbox and PlayStation
Scripts\Source\PTSW\SecondWindQuestScript.psc:61: error: Debug.Notification is DebugOnly: it does nothing in release builds on Xbox and PlayStation
It checked 1 file and found 2 errors and 0 warnings.
```

Exit code 1. sf-papyrus-ref's own README and AGENTS.md give the Creations command with `--allow Debug.Trace`:

```
sf-papyrus-ref check Scripts --allow Debug.Trace --creations
```

```
Scripts\Source\PTSW\SecondWindQuestScript.psc:61: error: Debug.Notification is DebugOnly: it does nothing in release builds on Xbox and PlayStation
It checked 1 file and found 1 error and 0 warnings.
```

Exit code 1. Line 61 is the starter template's own pattern: `Debug.Notification` inside
`If PTSW_Option_Debug.GetValue() != 0.0`, so it shows only on PC with the debug switch on. sf-mod-check's
`creations-debug` rule treats exactly this case as debug-only and doesn't report it (sf-mod-check README, release
rules: "A call counts as debug-only, and isn't reported, ... when it sits inside an `If` that tests a debug switch").
sf-papyrus-ref doesn't look at the `If`. The fix used here is to allow both calls in sf-papyrus-ref and let
sf-mod-check's `release --profile creations` (below) judge whether a notification is player-facing:

```
sf-papyrus-ref check Scripts --allow Debug.Trace,Debug.Notification --creations
```

```
It checked 1 file and found 0 errors and 0 warnings.
```

Exit code 0. The same command on the compiled scripts, `sf-papyrus-ref check .build\stage\Scripts --allow
Debug.Trace,Debug.Notification --creations`, also found 0 errors (exit code 0). **Doc gap:** the two tools disagree
about the starter template's debug pattern, and no doc says which to follow; the template's own script fails
`sf-papyrus-ref check --creations` even with the README's `--allow Debug.Trace`.

### The Creations release check

On the zip, as in the SPEC:

```
sf-mod-check release PMTTutorialSecondWind-0.1.0.zip --profile creations
```

```
sf-mod-check 0.15.2 release (creations profile): PMTTutorialSecondWind-0.1.0.zip (4 files, 1 plugins, 1 archives)
WARNING  [creations-xbox] PMTTutorialSecondWind-0.1.0.zip:PMTTutorialSecondWind - Main.ba2: no PMTTutorialSecondWind - Main_xbox.ba2, so Xbox gets no archive (the Creation Kit warns "Creation for XBox that has no archives"); build the Xbox archive with Archive2's Xbox settings.
...
NOTE     [creations-size] PMTTutorialSecondWind-0.1.0.zip: plugins and archives: 3.4 KB (a Creation may hold 2 GB).
RESULT: OK (0 problems, 0 to review, 1 warnings)
```

Exit code 0, with a WARNING: the zip is the PC download (for Vortex and MO2), and sf-mod-build puts the Xbox
archive only in `creations-out`. The Creations set is `creations-out`, so check that folder:

```
sf-mod-check release creations-out --profile creations
```

```
sf-mod-check 0.15.2 release (creations profile): creations-out (4 files, 1 plugins, 2 archives)
NOTE     [not-a-zip] creations-out: not a zip, so the zip-only rules (one-plugin, root-md) were skipped; run release on the zip you upload to check them
...
NOTE     [plugin-check] PMTTutorialSecondWind.esm: OK, 0 errors, 0 warnings (light master, 1 masters, 6 records, 0 overrides (0 ITM), 8 links)
NOTE     [mutagen-gap] PMTTutorialSecondWind.esm: 6 own records, 0 overrides, 0 compared with their master's copy
NOTE     [creations-xbox] creations-out: Xbox archive present: PMTTutorialSecondWind - Main_xbox.ba2.
NOTE     [creations-size] creations-out: plugins and archives: 5.1 KB (a Creation may hold 2 GB).
RESULT: OK (0 problems, 0 to review, 0 warnings)
```

Exit code 0, no warnings. No `creations-debug` WARNING appeared on either run, which confirms that the
`Debug.Notification` behind the debug switch isn't player-facing. **Doc gap:** no doc says to run
`release --profile creations` on `creations-out` rather than on the zip, and the `not-a-zip` NOTE tells you to "run
release on the zip you upload", although a Creation is uploaded through the Creation Kit, not as a zip. The
`creations-out` folder holds `PMTTutorialSecondWind.esm`, `PMTTutorialSecondWind - Main.ba2` and
`PMTTutorialSecondWind - Main_xbox.ba2` (plus sf-mod-build's `.sf-mod-build-made` list, which stays behind when you
upload).

### Records the game can reach

```
sf-cut-check scan --plugin dist\PMTTutorialSecondWind.esm --out C:\Scratch\cutcheck-sw
```

```
Checking PMTTutorialSecondWind.esm. A mod's records count as used only if the game can reach them, often through
Bethesda's own records, so the 18 official files are scanned first (about 2-3 minutes).
...
   121.6s official scan done and saved; now reading PMTTutorialSecondWind.esm
...
records PMTTutorialSecondWind.esm adds, by verdict:
    Reached                6
...
RESULT: PMTTutorialSecondWind.esm adds 6 records, and the game reaches all of them. None is reached only through a recipe, research project or condition check. None is reached only from test cells.
```

Exit code 0. The first run took 122 seconds; later runs use the saved scan and take about a second. (`--out` keeps
the report files, `report.csv`, `report.json` and `report.html`, out of the mod folder.)

## 5. Test plan

```
sf-test-plan new PMTTutorialSecondWind.testplan.md --mod PMTTutorialSecondWind
```

```
Starter plan: PMTTutorialSecondWind.testplan.md. Edit it, then run: sf-test-plan build PMTTutorialSecondWind.testplan.md
```

Exit code 0. The starter was then replaced with the mod's own steps (`PMTTutorialSecondWind.testplan.md` in this
folder: 4 groups, 9 checks, 5 smoke steps, and 2 log checks).

```
sf-test-plan check PMTTutorialSecondWind.testplan.md
```

```
PMTTutorialSecondWind.testplan.md: 4 groups, 9 checks, 5 smoke; 0 errors, 0 warnings
```

Exit code 0.

```
sf-test-plan rules PMTTutorialSecondWind.testplan.md --prefix PTSW --plugin PMTTutorialSecondWind.esm
```

```
PMTTutorialSecondWind.testplan.md: 4 log ids on 6 steps
New expect rule 'started' (step 1.1, L.1): re "PTSW started"
New expect rule 'second-wind' (step 2.1, 2.3, 4.1): re "PTSW second-wind"
New expect rule 'cooldown-over' (step 2.3): re "PTSW cooldown-over"
New expect rule 'no-script-errors' (step L.2): re "PTSW no-script-errors"
Rule file: PMTTutorialSecondWind.logrules.json. Edit each new 're' to match the line your mod writes, and move a rule for a line that must never appear to 'forbid'. Then run: sf-test-plan check PMTTutorialSecondWind.testplan.md --rules PMTTutorialSecondWind.logrules.json
```

Exit code 0. As the tool says, its patterns are only guesses: `PTSW started` never matches the real line
`[PTSW] started; ...`. The rule file was edited by hand: `prefix` set to `\[PTSW\]`, `scripts` set to `PTSW:`, each
`re` written to match the real line (for example `(?i)\[PTSW\] health at .*second wind starts`), and
`no-script-errors` moved to `forbid` with `(?i)error:.*PTSW:`. A small hand-written sample log with the mod's lines
passed with papyrus-log-check 0.4.2 (`PASS PMTTutorialSecondWind 0.1.0`), which proves the patterns load and match;
it isn't a game log.

```
sf-test-plan check PMTTutorialSecondWind.testplan.md --rules PMTTutorialSecondWind.logrules.json
sf-test-plan build PMTTutorialSecondWind.testplan.md --rules PMTTutorialSecondWind.logrules.json
```

```
PMTTutorialSecondWind.testplan.md: 4 groups, 9 checks, 5 smoke; 0 errors, 0 warnings
Page: PMTTutorialSecondWind.testplan.html
```

Exit code 0 for both.

## 6. What "console-ready" means for this mod

- Text the player must see is a Message record (`PTSW_Msg_SecondWind`, made with BuildKit's `Messages.Hud`) shown
  with `Message.Show()`, which works on Xbox and PlayStation.
- `Debug.Trace` and the on-screen debug lines are PC-only troubleshooting, behind a switch that is off by default.
- `"xbox": true` makes `PMTTutorialSecondWind - Main_xbox.ba2` in `creations-out`.
- `sf-mod-check release creations-out --profile creations` passes with no warnings.
- `sf-papyrus-ref check ... --creations` passes once the PC-only debug calls are allowed (see the doc gap above).
- Not covered by PMT: testing on a console, the upload itself (the Creation Kit does it) and PlayStation packaging
  (UNVERIFIED: no PMT doc says what a PlayStation Creation needs; GAPS.md G10).

## Doc gaps and PMT gaps found in this run

1. **sf-mod-build 0.4.1 README and AGENTS.md name 0.4.0 files** (`sf-mod-build.0.4.0.nupkg`,
   `sf-mod-build-0.4.0-win-x64.zip`, `sf-mod-build-template-0.4.0.zip`), and AGENTS.md opens "with sf-mod-build
   0.4.0". The release has only 0.4.1 files. (Confirms G16.)
2. **The 0.4.1 starter template pins BuildKit 0.4.2**, and the README and AGENTS.md say not to change it, while
   BuildKit 0.5.0 (the release whose gap check runs the public sf-mod-check) is current. Moving the pin to 0.5.0
   needed no code change. Owner: sf-mod-build (template and README).
3. **The builder's output is hidden during `sf-mod-build build`** unless you add `--verbose`, so you can't see that
   BuildKit's gap check ran or what it changed. The README doesn't say so. Owner: sf-mod-build.
4. **sf-papyrus-ref `check --creations` fails the starter template's own debug pattern**, even with the README's
   `--allow Debug.Trace`, while sf-mod-check's `creations-debug` rule accepts it. No doc says which to follow.
   Workaround: `--allow Debug.Trace,Debug.Notification` plus `sf-mod-check release --profile creations`. Owner:
   sf-papyrus-ref (skip DebugOnly calls inside an `If` that tests a debug switch, as sf-mod-check does) or the
   template docs.
5. **Where to run the Creations check.** The SPEC's `release <zip> --profile creations` gives a `creations-xbox`
   WARNING, because the Xbox archive is only in `creations-out`. No doc says to check `creations-out`, and its
   `not-a-zip` NOTE points back at "the zip you upload". Owner: sf-mod-build README (Creations section) and
   sf-mod-check README (`not-a-zip`).
6. **No PMT lookup of a vanilla record by Editor ID.** Workaround: BuildKit's `GameData.Get<T>` in the builder
   (BuildKit's README doesn't say that `Get` throws `no record X in Starfield.esm` when the record is missing, which
   is useful). Owner: sf-mod-check (a `find` or `lookup` command) or a BuildKit doc example.
7. **No BuildKit help for magic effects and spells.** `Effects.New` makes the effect entry, but the magic effect
   record has no helper or example, and Mutagen's name for the Value Modifier's actor value (`SecondActorValue`)
   isn't in any PMT doc. Owner: BuildKit (an example spell in the README).
8. **sf-papyrus-ref's zip unpacks into a nested folder**, which the install steps don't mention. Owner:
   sf-papyrus-ref.
9. **sf-test-plan `rules --prefix PTSW` writes `PTSW <id>` patterns**, which can't match a bracketed prefix such as
   `[PTSW]`. The tool says to edit each pattern, so this works as documented; an example with a bracketed prefix
   would help. Owner: sf-test-plan.
10. **SPEC corrections for the page writers (not PMT gaps):** the zip is `PMTTutorialSecondWind-0.1.0.zip` (0.1.0
    while untested), the script is `PTSW:SecondWindQuestScript`, the Papyrus check needs
    `--allow Debug.Trace,Debug.Notification` (or the launcher `sf-papyrus-ref` on PATH; `python -I sf_papyrus_ref.py`
    works only from the tool's folder or with its full path), and `sf-cut-check scan --plugin` takes the plugin's
    path (`dist\PMTTutorialSecondWind.esm`).

Not done with public tools: the in-game test (a person plays it on a new character), console testing and the
Creations upload (both by design).
