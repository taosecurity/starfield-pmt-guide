# Walkthrough: building A Courier's Request (PMTTutorialCourier) with public PMT tools

This is the record of one real run, made on 2026-11-24, for the guide's page M4 ("Build a quest mod for PC and
console"). Every command below was run exactly as shown, in a Command Prompt (`cmd.exe`), with the tool folders on
PATH. Each command has a short excerpt of its real output and its exit code.

**Paths in this file.** The excerpts write the mod's parent folder as `C:\Mods`, each unzipped tool as
`C:\Tools\<tool>-<version>`, a folder for throwaway reports as `C:\Scratch`, and the user's own folders as `%USERPROFILE%` and
`%LOCALAPPDATA%`. Nothing
else in the output was changed; `...` marks lines left out.

**Tools, versions and setup** are the same as for Second Wind: see `..\PMTTutorialSecondWind\WALKTHROUGH.md`,
sections "Tools and versions" and "0. Set up the tools (once)" (sf-mod-build 0.4.2 and its 0.4.1 template, BuildKit
0.5.0 from NuGet, Mutagen 0.54.4, sf-mod-check 0.16.0, sf-papyrus-ref 0.2.4 with `build-local` done, sf-test-plan
0.4.0, sf-cut-check 0.2.0, the Steam Creation Kit, .NET SDK 9.0.321, Python 3.13). Those are the versions of the
recorded 0.1.0 and 0.1.1 runs. The 0.1.2 build used sf-mod-check 0.16.1, and its test-plan page was rebuilt with
sf-test-plan 0.4.1 on 2026-11-26; the guide's pages name the current versions.

## Version 0.1.2 (2026-11-24, passed 2026-11-26): the note is a quest item

A review of 0.1.1 found a way to get stuck that was never seen in game: the note is placed disabled and then added
to the inventory, so a dropped note could stay invisible, and selling or storing it before reading it also left the
quest at stage 10. The game's own quest notes can't leave the inventory, because they are quest items.

- **Records:** the quest has a second reference alias, `Note` (ID 1), with the Optional and Quest Object flags
  (`AQuestAlias.Flag.Optional | AQuestAlias.Flag.QuestObject`), ALFG 0 and an empty voice type, as the Creation Kit
  writes them. The `Player` alias also gets the empty voice type. The quest script has a new alias property,
  `Alias_Note`.
- **Script:** stage 10 adds the note to the inventory and then runs `Alias_Note.ForceRefTo(NoteRef)`. Stage 30 runs
  `Alias_Note.Clear()` after `CompleteQuest()` and before `Stop()`, so the note becomes a normal item. Both steps
  log a `[PTCR]` line.
- **Test plan:** section 6 checks that the note can't be dropped, sold or stored before it is read, and check 4.5
  that it can be dropped after the quest ends.
- **Build:** sf-mod-build 0.4.2, sf-mod-check 0.16.1 and sf-papyrus-ref 0.2.4. The outputs further down are still
  from the 0.1.1 run.

**The in-game test on 2026-11-26** passed every check that was played: 7 of 8 marks passed, and the one failure
was an error in the plan, not the mod (below). The note couldn't be dropped, sold or moved to the ship's cargo
before it was read, and reading it still started stage 20. papyrus-log-check 0.4.3 passed all 13 of its checks (the
rule file's 12 rules and its own script-error check), the save read the quest as "stopped, completed, stage 30",
and sf-save-health found no problems. Two things
the test taught:

- **The Missions menu never lists a finished Activity under Completed.** The plan expected the quest there, so
  check 4.3 failed. The tester then finished a vanilla Activity, the Viewport bartender's, and it wasn't listed
  either. The 0.1.1 section below says that a missing `Stop()` kept the quest out of Completed; that was wrong. The
  missing `Stop()` was real (the save read "running, completed"), but the Completed tab was never the sign of it.
  Check 4.3 now expects the quest to leave Activities and not to appear under Completed.
- **The HUD line didn't appear after a console add.** In one run the Iron came from `player.additem 0000556E 5`,
  sent through the Console API. The quest completed and paid, and the log says the HUD line was shown, but it didn't
  appear on screen. In the two runs with bought Iron, it did. Check 4.1 now says to use picked-up or bought Iron for
  the HUD check.

## Version 0.1.1 (2026-11-24): what changed after the first play-test

The first in-game test of 0.1.0 found two problems. The quest never showed under Missions > Completed (which, the
0.1.2 test found, is true of every finished Activity), and the final HUD line was not seen. A save read with sf-save-reader showed the quest as "running, completed, stage 30",
while the game's own finished quests show "stopped, completed". The cause was in the script: stage 30 called
`CompleteQuest()` but never `Stop()`. The HUD line was probably hidden behind the Iron and credits pickup notices,
which show at the same moment.

The fix, with each new Papyrus name looked up first (`sf-papyrus-ref lookup`, exit code 0 for all):

```
sf-papyrus-ref lookup Quest.Stop
```

```
Quest.Stop  (function)
  Signature (your sources): Function Stop() native
  Flags: none
```

`Utility.Wait` is `Function Wait(float afSeconds) native global` (Latent), `Quest.CompleteQuest` is
`Function CompleteQuest() native`, and `Message.Show` is Latent.

- **Script:** stage 30 now runs `CompleteQuest()`, then `Stop()`, then `Utility.Wait(DoneMessageDelay)` (3.0
  seconds, a new `Float Property` with a default, so the plugin needs no new property entry), then
  `PTCR_Msg_Done.Show()`. It logs "quest stopped, HUD line in 3.000000 seconds" (Papyrus writes a Float with six decimals) and "HUD line shown" with the `[PTCR]`
  prefix. The in-game test of 0.1.1 on 2026-11-24 confirmed that the wait and the message still run after
  `Stop()`: the log shows the HUD line after the quest stopped, and the save shows the quest "stopped, completed,
  stage 30".
- **Records:** stage 30's journal entry now carries the "Complete Quest" flag
  (`QuestLogEntry.Flag.CompleteQuest`), the way the game's own quests do it. How that was checked: a throwaway
  program opened the Steam `Starfield.esm` with Mutagen 0.54.4 and counted the quests whose log entries carry the
  flag. Of 2077 quests, 466 do, and 272 of those flag the quest's highest stage. For example, `MQ101` flags stages
  1800 and 2100. Reading our plugin back with BuildKit's `GameData.OpenPlugin` printed `PTCR_Quest: 10:0, 20:0,
  30:CompleteQuest`. The plugin stays 2384 bytes, because the flag is one byte that was already there.
- **Test plan:** two new checks under stage 30: the HUD line appears a few seconds after the reward, and the quest
  is listed under Missions > Completed. (The 0.1.2 test showed that the game never lists a finished Activity there,
  so that check now expects the quest not to be listed.)
- **Build:** `sfmod.json` says 0.1.1, and the build used sf-mod-build 0.4.2, sf-mod-check 0.16.0 and sf-papyrus-ref
  0.2.4. The outputs below are from this 0.1.1 run. Two lines changed from 0.1.0: the Papyrus check no longer needs
  `--allow Debug.Notification` (see "Check"), and the file sizes and checksum.

## The design in plain words

- **Quest** `PTCR_Quest`, "A Courier's Request": starts with the game, listed under Activities. No dialogue, scenes,
  voice or placed references, so it edits no vanilla record.
- **Stages** are the quest's steps, numbered 10, 20 and 30. Each has a journal entry. The quest script's
  `OnStageSet` event does each stage's work.
- **Objectives** are the lines the player sees in the Missions menu and on the HUD: 10 "Read the courier's note" and
  20 "Collect 5 Iron".
- **Aliases**: a reference alias, `Player` (ID 0), filled with the player, as the game's own quests do it (its unique
  actor is the `Player` NPC record). Its alias script watches where the player goes and what the player picks up.
  Since 0.1.2, a second alias, `Note` (ID 1), holds the note as a quest item from stage 10 to stage 30.
- Stage 10 starts when the player's location is New Atlantis or one of its child locations. The script places the
  mod's note as a disabled reference, adds it to the player's inventory, puts it in the `Note` alias (0.1.2) and
  listens for its `OnRead` event. Stage 20
  starts when that note is read. Stage 30 starts when the player holds 5 Iron; it pays 100 credits, completes and
  stops the quest, and about 3 seconds later shows a HUD Message.
- Logging and the debug switch are the same as in Second Wind, with the prefix `[PTCR]`.

## 1. Make the mod from the starter template

In `C:\Mods`:

```
dotnet new sfmod -n PMTTutorialCourier --prefix PTCR --mod-author "PMT Guide"
```

```
The template "Starfield mod (sf-mod-build)" was created successfully.
```

Exit code 0. Then, in `C:\Mods\PMTTutorialCourier`:

1. In `Builder\PMTTutorialCourier.Builder.csproj`, change the BuildKit pin from `[0.4.2]` to `[0.5.0]` (as for
   Second Wind). This run used the 0.4.1 template. The sf-mod-build 0.4.2 template already pins BuildKit 0.5.0, so
   with it you skip this step.
2. Delete `Scripts\Source\PTCR\StarterQuestScript.psc` and add `CourierQuestScript.psc` and
   `CourierPlayerAliasScript.psc` (in `Scripts\Source\PTCR` in this folder).
3. Replace `Builder\Program.cs` (in this folder).
4. In `sfmod.json`, change `"xbox": false,` to `"xbox": true,`. The `"namespaces": ["PTCR"]` line already picks up
   both scripts.

### Every Papyrus name, looked up first

All were looked up with `sf-papyrus-ref lookup <Script.Name>` after `build-local`. Found on the official list
(exit code 0): `Quest.OnStageSet`, `Quest.SetObjectiveDisplayed`, `Quest.SetObjectiveCompleted`,
`Quest.CompleteQuest`, `Quest.OnQuestInit`, `ObjectReference.PlaceAtMe` (Inventory-safe), `ObjectReference.AddItem`
(Latent, Non-delayed), `ObjectReference.GetItemCount`, `ObjectReference.GetCurrentLocation`, `ObjectReference.OnRead`,
`ScriptObject.RegisterForRemoteEvent` (Non-delayed), `ScriptObject.AddInventoryEventFilter` (Non-delayed),
`Location.IsChild`, `Alias.GetOwningQuest` (Non-delayed), `Alias.OnAliasInit`, `Game.GetPlayer`, `Game.GetCredits`,
`Message.Show` (Latent), `GameplayOption.GetValue`, `Quest.Stop` and `Utility.Wait` (Latent, added in 0.1.1), `Debug.Trace` and `Debug.Notification` (DebugOnly).

With sf-papyrus-ref 0.2.4, `Quest.SetStage`, `Quest.GetStageDone`, `ReferenceAlias.OnLocationChange`,
`ReferenceAlias.OnItemAdded` and `ReferenceAlias.OnRead` are on the shipped list: each `lookup <name> --no-local`
exits 0 and prints "The official wiki has no page for this name; it is defined in the Creation Kit's script
sources." (checked again on 2026-11-24). The rest of this section is the 0.1.0 run, made with sf-papyrus-ref 0.2.3,
where those names were found only in your own Creation Kit sources, after `build-local`:

```
sf-papyrus-ref lookup Quest.SetStage
```

```
Quest.SetStage  (function, in your sources only: Scripts/Source/Quest.psc)
  Signature (your sources): bool Function SetStage(int aiStage)
```

Exit code 0. The same is true of `Quest.GetStageDone`, `ReferenceAlias.OnLocationChange`,
`ReferenceAlias.OnItemAdded` and `ReferenceAlias.OnRead`. Without the cache, the lookup fails:

```
sf-papyrus-ref lookup Quest.SetStage --no-local
```

```
Quest.SetStage: Not in the official list. It may be a script extender (SFSE) function, a function from another mod's script, or a made-up name. The Papyrus compiler is the final judge.
Did you mean: Quest.QuestStage, Quest.SetQuestStage, Quest.SetStageNoWait, Quest.SetCurrentStageID
```

Exit code 1 (sf-papyrus-ref 0.2.3; 0.2.4 fixed it). **Doc gap, fixed in 0.2.4:** with 0.2.3, for quest scripting, `build-local` wasn't optional. The shipped list lacks `Quest.SetStage`
and `Quest.GetStageDone`, two of the most used quest functions, and an agent that follows AGENTS.md ("A name that
comes back `found: false` ... Don't use it") would refuse them.

### Every vanilla record, looked up by Editor ID

As for Second Wind, no PMT command looks a record up by Editor ID, so the builder uses BuildKit's
`GameData.Get<T>(editorId)`, which stops the build when one is missing. With `sf-mod-build build --verbose`, the
builder's line reads:

```
log: vanilla: CityNewAtlantisLocation = 01295A:Starfield.esm (New Atlantis), InorgCommonIron = 00556E:Starfield.esm (Iron), Player = 000007:Starfield.esm, QuestTypeActivities = 0475F8:Starfield.esm
```

- `CityNewAtlantisLocation` (`01295A`), name "New Atlantis". Its child locations include The Well, The Lodge, the
  UC Distribution Center and the embassies, so the alias script accepts the location or any child of it.
- `InorgCommonIron` (`00556E`), a misc item named "Iron". In the console it is `0000556E`.
- `Player` (`000007`), the player's NPC record, used as the alias's unique actor.
- `QuestTypeActivities` (`0475F8`), the keyword that lists the quest under Activities.

How they were found: the Editor IDs `CityNewAtlantisLocation` and `InorgCommonIron` were tried with
`GameData.Get` and confirmed by the names it returned ("New Atlantis", "Iron"). The other two, and the shape of a
plain note (`Trait_KidStuff_Backpack_Note`), came from reading the game's own small quests and notes with BuildKit's
`GameData` in a throwaway program. Nothing here needed xEdit.

### The quest records: BuildKit plus raw Mutagen (GAPS.md G08)

BuildKit 0.5.0 makes the quest shell with `Quests.New(mod, "PTCR_Quest")` (empty condition and alias lists, as the
Creation Kit writes them). It has no helpers for stages, objectives or aliases, so `Builder\Program.cs` writes those
with plain Mutagen 0.54.4 objects:

- Stages: `quest.Stages.Add(new QuestStage { Index = 10, LogEntries = { new QuestLogEntry { Flags = 0, Entry = "..." } } })`.
- Objectives: `quest.Objectives.Add(new QuestObjective { Index = 10, Flags = 0, DisplayText = "Read the courier's note" })`.
- The alias: `new QuestReferenceAlias { ID = 0, Name = "Player", Flags = 0, ALFG = 0 }` with
  `UniqueActor.SetTo(playerNpc)`.
- The alias script: a `QuestFragmentAlias` in the quest's `QuestAdapter.Aliases`, whose `Property` is a
  `ScriptObjectProperty` that points at the quest itself with `Alias = 0`, holding the `ScriptEntry` for
  `PTCR:CourierPlayerAliasScript`.
- The note: a new record of the mod's own, made with Mutagen's `Duplicate` from a plain vanilla note and given its
  own Editor ID, name and text. The vanilla note isn't changed and isn't in the plugin.

**Doc gap:** none of these shapes is in a PMT doc. In particular, how an alias script is stored (in the quest's
script data, pointing at the quest and the alias ID) and that the game's own player aliases use the `Player` NPC as
their unique actor were found only by reading the game's own quests.

Reading the plugin back (BuildKit's `GameData.OpenPlugin` in the same throwaway program) showed what was written:

```
PTCR_Quest 000802:PMTTutorialCourier.esm name=A Courier's Request flags=StartGameEnabled, StartsEnabled, RunOnce prio=50 qtype=0475F8:Starfield.esm
 STAGE 10  LOG entry=A courier's note reached me in New Atlantis. Someone needs Iron.
 STAGE 20  LOG entry=The note asks for 5 Iron for a workshop in the Commercial District.
 STAGE 30  LOG flags=CompleteQuest entry=I collected the Iron, and the workshop paid me for it.
 OBJ 10 text=Read the courier's note
 OBJ 20 text=Collect 5 Iron
 ALIAS ref 0 Player unique=000007:Starfield.esm
 VMAD scripts=PTCR:CourierQuestScript
  VALIAS obj=000802:PMTTutorialCourier.esm alias=0 scripts=PTCR:CourierPlayerAliasScript (2 properties)
```

(That program printed this line by line; it is shortened here.) One difference from the Creation Kit: most vanilla
stages store 171 in a byte Mutagen calls `QuestStage.Unknown`, while ours store 0. 244 vanilla stages also store 0,
and `sf-mod-check gap` reports nothing, so it was left at 0.

## 2. Build

```
sf-mod-build validate
```

```
...
scripts:  from C:\Mods\PMTTutorialCourier\Scripts\Source
          namespaces PTCR
...
xbox:     yes, "PMTTutorialCourier - Main_xbox.ba2" in C:\Mods\PMTTutorialCourier\creations-out
zip:      C:\Mods\PMTTutorialCourier\PMTTutorialCourier-0.1.1.zip
          holds PMTTutorialCourier.esm, PMTTutorialCourier - Main.ba2, README-PMTTutorialCourier.md, LICENSE-PMTTutorialCourier.txt
...
RESULT: OK
```

Exit code 0.

```
sf-mod-build build
```

```
== compile
...
compiled PTCR\CourierPlayerAliasScript.psc
compiled PTCR\CourierQuestScript.psc
RESULT: OK, 2 script(s) compiled into C:\Mods\PMTTutorialCourier\.build\stage\Scripts
== plugin
running the builder: PMTTutorialCourier.Builder.csproj (the first run restores and builds it, which can take a few minutes)
built    C:\Mods\PMTTutorialCourier\dist\PMTTutorialCourier.esm (2384 bytes)
RESULT: OK, PMTTutorialCourier.esm built into C:\Mods\PMTTutorialCourier\dist
== pack
packed   C:\Mods\PMTTutorialCourier\dist\PMTTutorialCourier - Main.ba2 (3396 bytes)
packed   C:\Mods\PMTTutorialCourier\creations-out\PMTTutorialCourier - Main_xbox.ba2 (3396 bytes)
RESULT: OK, C:\Mods\PMTTutorialCourier\dist\PMTTutorialCourier - Main.ba2 and the Creations set in C:\Mods\PMTTutorialCourier\creations-out made
== zip
...
sha256   185651693a4c6c83953bffa7fc3af2d7b9c1d69b77b69c0d4977cd8d63e6eca1
RESULT: OK, C:\Mods\PMTTutorialCourier\PMTTutorialCourier-0.1.1.zip written (4 files, 7649 bytes)
== check
checking PMTTutorialCourier-0.1.1.zip with sf-mod-check.exe
         sf-mod-check 0.16.0 release (basic profile): PMTTutorialCourier-0.1.1.zip (4 files, 1 plugins, 1 archives)
         ...
         NOTE     [plugin-check] PMTTutorialCourier.esm: OK, 0 errors, 0 warnings (light master, 1 masters, 5 records, 0 overrides (0 ITM), 13 links)
         NOTE     [mutagen-gap] PMTTutorialCourier.esm: 5 own records, 0 overrides, 0 compared with their master's copy
         RESULT: OK (0 problems, 0 to review, 0 warnings)
RESULT: OK, PMTTutorialCourier-0.1.1.zip passed the release check
RESULT: OK, built C:\Mods\PMTTutorialCourier\PMTTutorialCourier-0.1.1.zip
```

Exit code 0. With `--verbose`, the builder's lines include:

```
log: ID lock: 5 records, 5 new, 0 moved back to their locked IDs (C:\Mods\PMTTutorialCourier\ids.lock.json)
log:   BuildKit: header interior cell count (INCC) set to 0 (was unset)
log:   BuildKit: own records written in Bethesda's FE FormID form (light plugin)
log:   gap check: running C:\Tools\sf-mod-check-0.16.0\sf-mod-check.exe gap
log:   gap check: PMTTutorialCourier.esm: 5 own records, 0 overrides, 0 compared with their master's copy
log:   gap check:   0 error(s), 0 warning(s)
log:   gap check: RESULT: OK (0 errors, 0 warnings)
```

That was the first build, so all five records were new; later builds say "0 new".

## 3. Check

```
sf-mod-check check dist\PMTTutorialCourier.esm
```

```
sf-mod-check 0.16.0 check: PMTTutorialCourier.esm (light master, 1 masters, 5 records, 0 overrides (0 ITM), 13 links)
RESULT: OK, 0 errors, 0 warnings (PMTTutorialCourier.esm: 5 records, 13 links; 4.3 s)
```

Exit code 0. `check` tests the quest's conditions (alias numbers), scene aliases and dialogue emotion. This quest has
no conditions, scenes or dialogue, so those parts had nothing to find. It doesn't check quest logic: whether the
stages the scripts set (10, 20 and 30) exist in the quest is checked by nobody (GAPS.md G08); here they were
checked by reading the plugin back (above).

```
sf-mod-check gap dist\PMTTutorialCourier.esm
```

```
PMTTutorialCourier.esm: 5 own records, 0 overrides, 0 compared with their master's copy
  0 error(s), 0 warning(s)
RESULT: OK (0 errors, 0 warnings)
```

Exit code 0. This is the guard G08 asks for on records written with raw Mutagen.

```
sf-papyrus-ref check Scripts --creations
```

```
Scripts\Source\PTCR\CourierQuestScript.psc:91: warning: Debug.Trace is DebugOnly: consoles ignore it, so Xbox and PlayStation players see nothing
Scripts\Source\PTCR\CourierQuestScript.psc:93: warning: Debug.Notification is DebugOnly: consoles ignore it, so Xbox and PlayStation players see nothing
It checked 2 files and found 0 errors and 2 warnings.
```

Exit code 0. **Changed from 0.1.0:** sf-papyrus-ref 0.2.3 reported `Debug.Notification` as an error (exit code 1) and
needed `--allow Debug.Trace,Debug.Notification`. Version 0.2.4 reports both Debug calls as warnings and exits 0, so
the check passes without `--allow`. Both calls are the mod's own logging and its debug switch, which is PC only by
design (the player-facing text is the `Message` record). The `--allow` flag still removes the warnings:

```
sf-papyrus-ref check Scripts --allow Debug.Trace,Debug.Notification --creations
```

```
It checked 2 files and found 0 errors and 0 warnings.
```

Exit code 0. The compiled scripts (`.build\stage\Scripts`) gave the same two warnings and exit code 0. Note that
`AddItem`, `Message.Show` and `PlaceAtMe` are not called inside a loop (the new `Utility.Wait` isn't either), so the
Latent-in-loop check had nothing to report.

```
sf-mod-check release PMTTutorialCourier-0.1.1.zip --profile creations
```

```
WARNING  [creations-xbox] PMTTutorialCourier-0.1.1.zip:PMTTutorialCourier - Main.ba2: no PMTTutorialCourier - Main_xbox.ba2, so Xbox gets no archive (the Creation Kit warns "Creation for XBox that has no archives"); build the Xbox archive with Archive2's Xbox settings.
...
RESULT: OK (0 problems, 0 to review, 1 warnings)
```

Exit code 0 with one WARNING, because the zip is the PC download. The Creations set:

```
sf-mod-check release creations-out --profile creations
```

```
sf-mod-check 0.16.0 release (creations profile): creations-out (4 files, 1 plugins, 2 archives)
NOTE     [not-a-zip] creations-out: not a zip, so the zip-only rules (one-plugin, root-md) were skipped; run release on the zip you upload to check them
...
NOTE     [creations-xbox] creations-out: Xbox archive present: PMTTutorialCourier - Main_xbox.ba2.
NOTE     [creations-size] creations-out: plugins and archives: 8.6 KB (a Creation may hold 2 GB).
RESULT: OK (0 problems, 0 to review, 0 warnings)
```

Exit code 0, no warnings.

```
sf-cut-check scan --plugin dist\PMTTutorialCourier.esm --out C:\Scratch\cutcheck-cr
```

```
Checking PMTTutorialCourier.esm against the saved scan of the 18 official files (made 2026-11-24 17:06 UTC;
your game files haven't changed since). Reading only the mod. Add --no-cache to scan the official files again.
...
records PMTTutorialCourier.esm adds, by verdict:
    Reached                5
...
RESULT: PMTTutorialCourier.esm adds 5 records, and the game reaches all of them. None is reached only through a recipe, research project or condition check. None is reached only from test cells.
```

Exit code 0 (1 second, from the scan saved during the Second Wind run).

## 4. Test plan and rule file

```
sf-test-plan new PMTTutorialCourier.testplan.md --mod PMTTutorialCourier
```

```
Starter plan: PMTTutorialCourier.testplan.md. Edit it, then run: sf-test-plan build PMTTutorialCourier.testplan.md
```

Exit code 0. The starter was replaced with the quest's steps (`PMTTutorialCourier.testplan.md` in this folder): one
group per stage, with the console commands `getstage PTCR_Quest`, `setstage PTCR_Quest <stage>` (a way past a stage
that doesn't start) and `player.additem 0000556E 5` in backticks, so the page gives them Copy buttons.

```
sf-test-plan check PMTTutorialCourier.testplan.md
```

```
PMTTutorialCourier.testplan.md: 6 groups, 17 checks, 9 smoke; 0 errors, 0 warnings
```

Exit code 0.

```
sf-test-plan rules PMTTutorialCourier.testplan.md --prefix PTCR --plugin PMTTutorialCourier.esm
```

```
PMTTutorialCourier.testplan.md: 9 log ids on 9 steps
New expect rule 'started' (step 1.1, L.1): re "PTCR started"
New expect rule 'stage-10' (step 2.1): re "PTCR stage-10"
New expect rule 'onread' (step 3.1): re "PTCR onread"
New expect rule 'stage-20' (step 3.1): re "PTCR stage-20"
New expect rule 'iron-check' (step 4.1): re "PTCR iron-check"
New expect rule 'stage-30' (step 4.1, 5.1): re "PTCR stage-30"
New expect rule 'stage-30-hud' (step 4.2): re "PTCR stage-30-hud"
New expect rule 'stage-30-stopped' (step 4.3): re "PTCR stage-30-stopped"
New expect rule 'no-script-errors' (step L.2): re "PTCR no-script-errors"
Rule file: PMTTutorialCourier.logrules.json. Edit each new 're' to match the line your mod writes, and move a rule for a line that must never appear to 'forbid'. Then run: sf-test-plan check PMTTutorialCourier.testplan.md --rules PMTTutorialCourier.logrules.json
```

Exit code 0. The rule file was then edited by hand, as the tool asks: `prefix` `\[PTCR\]`, `scripts` `PTCR:`, each
`re` matched to the real line (for example `(?i)\[PTCR\] stage 20: note read`), an `order` rule (started, then
stages 10, 20 and 30, then the new "quest stopped" and "HUD line shown" lines), and `no-script-errors` moved to `forbid` as `(?i)error:.*PTCR:`. A small hand-written sample
log with the mod's lines passed with papyrus-log-check 0.4.2 (`PASS PMTTutorialCourier 0.1.1`); it proves the
patterns load and match, and it isn't a game log.

```
sf-test-plan check PMTTutorialCourier.testplan.md --rules PMTTutorialCourier.logrules.json
sf-test-plan build PMTTutorialCourier.testplan.md --rules PMTTutorialCourier.logrules.json
```

```
PMTTutorialCourier.testplan.md: 6 groups, 17 checks, 9 smoke; 0 errors, 0 warnings
Page: PMTTutorialCourier.testplan.html
```

Exit code 0 for both.

### Not run yet: reading the quest from a save

```
sf-save-reader quests <save.sfs> --data --filter PTCR --details
```

This needs a save from the in-game test, so it wasn't run. After the test, run it on a save made at stage 20 and one
made after stage 30. (UNVERIFIED: the exact output; sf-save-reader 0.1.3 wasn't downloaded for this run.)

## 5. Testing the stages

- Console: `getstage PTCR_Quest` shows the stage; `setstage PTCR_Quest 20` moves the quest on by hand, which tests
  each stage's work without playing to it.
- Trace lines: each stage writes one `[PTCR] stage <n>: ...` line, and the rule file turns them into PASS, FAIL or
  NOT SEEN.
- Saves: `sf-save-reader quests ... --details` (above).
- **Confirmed by the in-game test on 2026-11-24:** `OnRead` reaches the remote event for a note read from the
  inventory (the stage 20 check passed without `setstage`). The note is a persistent reference (held in a script variable), and the game's own quests listen for `OnRead`
  the same way on aliases. If stage 20 doesn't start, the test plan says to use `setstage PTCR_Quest 20` and mark
  the step Fail.

## Doc gaps and PMT gaps found in this run

The gaps from Second Wind's walkthrough (sf-mod-build's 0.4.0 file names, the template's BuildKit 0.4.2 pin, the
builder's output hidden without `--verbose`, sf-papyrus-ref and sf-mod-check disagreeing about the template's debug
pattern, where to run the Creations check, no record lookup by Editor ID, sf-papyrus-ref's nested zip folder, and
sf-test-plan's bracketed-prefix guesses) all apply here too. New in this run:

1. **sf-papyrus-ref's shipped list lacks `Quest.SetStage` and `Quest.GetStageDone`** (and the `ReferenceAlias`
   events), so without `build-local` the lookup says they may be made-up names and suggests other names. For quest
   scripts, `build-local` is required, and AGENTS.md's "don't use a `found: false` name" rule would block an agent.
   Owner: sf-papyrus-ref (add the non-native functions of Quest.psc, or say in the README and AGENTS.md that quest
   scripting needs `build-local`).
2. **G08 confirmed.** Stages, objectives, the alias and the alias script were all written with raw Mutagen. None of
   their shapes (the alias script's place in the quest's script data, the player alias's unique actor) is in a PMT
   doc. Owner: BuildKit (quest helpers, as G08 says), or at least a worked example.
3. **No check that stages set in scripts exist in the quest** (G08). Here it was checked by reading the plugin back.
   Owner: sf-mod-check.
4. **No quest test template** (G09). The plan was written by hand from the generic starter. Owner: sf-test-plan and
   papyrus-log-check.
5. **SPEC corrections for the page writers (not PMT gaps):** the zip is `PMTTutorialCourier-0.1.1.zip`, the scripts
   are `PTCR:CourierQuestScript` and `PTCR:CourierPlayerAliasScript`, the quest's Editor ID is `PTCR_Quest`, the
   alias is filled through its unique actor (the `Player` NPC) rather than a forced reference, the Papyrus check
   passes with two warnings and no `--allow` (sf-papyrus-ref 0.2.4; 0.2.3 needed `--allow Debug.Trace,Debug.Notification`), a quest
   script ends with `CompleteQuest()` and `Stop()` and its last journal entry carries the "Complete Quest" flag, the Creations check goes on `creations-out`, and the sf-save-reader
   step waits for a save from the in-game test.

Not done with public tools: the in-game test (a person plays it on a new character), console testing and the
Creations upload (both by design).
