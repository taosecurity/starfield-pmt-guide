# PMTTutorialCourier test plan
run: pmttutorialcourier-0.1.2
mod: PMTTutorialCourier
version: 0.1.2
zip: PMTTutorialCourier-0.1.2.zip

This plan tests A Courier's Request (PMTTutorialCourier) 0.1.2, the quest mod from the PMT guide's tutorial. The quest has three stages: stage 10 gives you a note (a quest item) when you first reach New Atlantis, stage 20 starts when you read the note and asks for 5 Iron, and stage 30 pays 100 credits, completes and stops the quest once you hold 5 Iron, and shows a HUD line about 3 seconds later. Version 0.1.1 fixes two problems found in the play-test of 0.1.0: the quest kept running after it completed, and the HUD line was lost among the pickup notices. (The 0.1.2 test showed that a finished Activity is never listed under Completed, vanilla ones included, so that tab is not a sign of either.) Version 0.1.2 makes the note a quest item until the quest ends, so you can't drop, sell or store it before you read it (a review found that a dropped note could stay invisible, which would stick the quest at stage 10). Start with the smoke test; the deep checks try the edge cases.

## Before you start

- Install PMTTutorialCourier-0.1.2.zip with your mod manager and enable it.
- Turn on Papyrus logging (the mod's README says how), so the log shows the mod's `[PTCR]` lines.
- Start a new character, so an old save doesn't hide problems.

## 1 Install and load

- [ ] Start a new game and finish the character creator.
  Expect: no error message about PMTTutorialCourier.
  smoke: yes
  log: started
- [ ] Open Settings > Gameplay Options.
  Expect: a group named "A Courier's Request (PMT Tutorial)" with "Show Debug Messages (PC only)", set to off.
- [ ] Before you reach New Atlantis, open the console and type `getstage PTCR_Quest`.
  Expect: 0, because no stage has been set yet.

## 2 Stage 10: the note

- [ ] Play until you first arrive in New Atlantis (or land at the New Atlantis spaceport later in the game).
  Expect: "A Courier's Request" is added to your inventory, and a new objective, "Read the courier's note", appears.
  smoke: yes
  log: stage-10
- [ ] Open the Missions menu and look under Activities.
  Expect: "A Courier's Request" is listed with the objective "Read the courier's note".
- [ ] Look at the ground where you stood when the note arrived.
  Expect: no copy of the note lies in the world; the only copy is in your inventory.
- [ ] Type `getstage PTCR_Quest` in the console.
  Expect: 10.
  note: If stage 10 never starts, note what `getstage PTCR_Quest` says, then type `setstage PTCR_Quest 10` to go on, and mark this group Fail with a note of where you were.

## 3 Stage 20: read the note

- [ ] Read the note from your inventory (Inventory > Notes, then Read).
  Expect: the note's text asks for 5 Iron. The first objective is completed, and "Collect 5 Iron" appears.
  smoke: yes
  log: onread
  note: Only the `OnRead` log line proves the read: `setstage PTCR_Quest 20` writes the same stage 20 line as a read does.
- [ ] Type `getstage PTCR_Quest` in the console.
  Expect: 20.
  log: stage-20
  note: If reading the note doesn't start stage 20, note what `getstage PTCR_Quest` says, then type `setstage PTCR_Quest 20` to go on, and mark this check and the one before it Fail.

## 4 Stage 30: collect the Iron

- [ ] Close every menu (inventory, Missions, the console) before you take the 5th Iron, so you can see the HUD line that follows. Then pick up or buy Iron until you hold 5.
  Expect: the objective "Collect 5 Iron" is completed and your credits go up by 100.
  smoke: yes
  log: iron-check, stage-30
  note: The console shortcut `player.additem 0000556E 5` also completes the quest, but in the 0.1.2 test (2026-11-26) the HUD line of the next check didn't appear after a console add (sent through the Console API), although the log says it was shown. Use picked-up or bought Iron when you test the HUD line.
- [ ] Watch the HUD for a few seconds after the reward.
  Expect: about 3 seconds after the reward, once the Iron and credits pickup notices have cleared, a HUD line says "A Courier's Request: done. The workshop paid you 100 credits."
  smoke: yes
  log: stage-30-hud
- [ ] Open the Missions menu and look at the Completed tab.
  Expect: "A Courier's Request" is no longer listed under Activities, and it isn't listed under Completed either, because the game never lists a finished Activity there (a vanilla Activity isn't listed either).
  smoke: yes
  log: stage-30-stopped
  note: Version 0.1.0 left the quest running, so the Completed tab did not list it. If it still isn't listed, mark this check Fail and note what the Activities tab shows.
- [ ] Type `getstage PTCR_Quest` in the console.
  Expect: 30.
- [ ] Open your inventory, select the note and drop it.
  Expect: the note leaves your inventory, because it is a normal item again once the quest ends.
  log: note-alias-cleared
  note: Whether you can see the dropped note on the ground doesn't matter here; the quest is over.

## 5 Edge cases

- [ ] On a second new character, pick up 5 Iron before you reach New Atlantis, then arrive and read the note.
  Expect: the quest completes as soon as you read the note, because you already hold the Iron.
  log: stage-30
- [ ] Leave New Atlantis and come back after the quest has started.
  Expect: no second note, and the stage doesn't change.
- [ ] Turn on Show Debug Messages (PC only), then pick up one more Iron before the quest ends.
  Expect: a `[PTCR] Iron check` line shows on screen as well as in the log.

## 6 The note is a quest item

Since 0.1.2 the note is a quest item until the quest ends, as the game's own quest notes are. Do this check before you read the note.

- [ ] When the note arrives, open your inventory, select the note and try to drop it. Then, at a vendor, look for the note in your items to sell, and at your ship's cargo hold, try to store it.
  Expect: the note can't be dropped or sold (it's a quest item): the game won't drop it, it isn't in the vendor's sell list, and it can't be stored. The objective stays at "Read the courier's note", and reading the note afterwards still starts stage 20.
  smoke: yes
  log: note-alias
  note: If the note does drop, look for it on the ground and say whether you can see it: 0.1.1 placed it disabled, so a dropped note could stay invisible. If you can't read it, type `getstage PTCR_Quest` (it should say 10), then type `setstage PTCR_Quest 20` to go on. Mark this check Fail either way.

## 7 Save and load

- [ ] Save while the quest is at stage 10 or 20, quit to the main menu and load the save.
  Expect: the objective is still shown, and reading the note or collecting the Iron still moves the quest on.
  smoke: yes

## Log checks

After play, run papyrus-log-check on the Papyrus log with the rule file `PMTTutorialCourier.logrules.json`. A log from a session that stops before stage 30 shows the four stage-30 rules and the order rule as NOT SEEN; that is expected. A skipped check's log rules show as NOT SEEN too (for example `onread` when check 3.1 is skipped), and so does the order rule. A console `setstage` writes the stage's log line, so the `stage-10`, `stage-20` and `stage-30` rules can pass without the play that normally sets the stage; the `onread` and `iron-check` rules are the ones that prove the read and the pickup.

- [ ] The quest script started at the beginning of the game.
  Expect: a `[PTCR] started` line.
  log: started
- [ ] No Papyrus error names a PTCR script.
  Expect: no `error:` line that names PTCR:CourierQuestScript or PTCR:CourierPlayerAliasScript.
  log: no-script-errors
