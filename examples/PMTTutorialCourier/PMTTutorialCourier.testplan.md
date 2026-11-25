# PMTTutorialCourier test plan
run: pmttutorialcourier-0.1.1
mod: PMTTutorialCourier
version: 0.1.1
zip: PMTTutorialCourier-0.1.1.zip

This plan tests A Courier's Request (PMTTutorialCourier) 0.1.1, the quest mod from the PMT guide's tutorial. The quest has three stages: stage 10 gives you a note when you first reach New Atlantis, stage 20 starts when you read the note and asks for 5 Iron, and stage 30 pays 100 credits, completes and stops the quest once you hold 5 Iron, and shows a HUD line about 3 seconds later. Version 0.1.1 fixes two problems found in the play-test of 0.1.0: the quest was not listed under Completed, and the HUD line was lost among the pickup notices. Start with the smoke test; the deep checks try the edge cases.

## Before you start

- Install PMTTutorialCourier-0.1.1.zip with your mod manager and enable it.
- Turn on Papyrus logging (the mod's README says how), so the log shows the mod's `[PTCR]` lines.
- Start a new character, so an old save doesn't hide problems.

## 1 Install and load

- [ ] Start a new game and finish the character creator.
  Expect: no error message about PMTTutorialCourier.
  smoke: yes
  log: started
- [ ] Open Settings > Gameplay Options.
  Expect: a group named "A Courier's Request (PMT Tutorial)" with "Show Debug Messages (PC only)", set to off.
  smoke: yes
- [ ] Before you reach New Atlantis, open the console and type `getstage PTCR_Quest`.
  Expect: 0, because no stage has been set yet.

## 2 Stage 10: the note

- [ ] Play until you first arrive in New Atlantis (or land at the New Atlantis spaceport later in the game).
  Expect: "A Courier's Request" is added to your inventory, and a new objective, "Read the courier's note", appears.
  smoke: yes
  log: stage-10
- [ ] Open the Missions menu and look under Activities.
  Expect: "A Courier's Request" is listed with the objective "Read the courier's note".
  smoke: yes
- [ ] Look at the ground where you stood when the note arrived.
  Expect: no copy of the note lies in the world; the only copy is in your inventory.
- [ ] Type `getstage PTCR_Quest` in the console.
  Expect: 10.
  note: If stage 10 never starts, type `setstage PTCR_Quest 10` to go on, and mark this group Fail with a note of where you were.

## 3 Stage 20: read the note

- [ ] Read the note from your inventory (Inventory > Notes, then Read).
  Expect: the note's text asks for 5 Iron. The first objective is completed, and "Collect 5 Iron" appears.
  smoke: yes
  log: onread, stage-20
- [ ] Type `getstage PTCR_Quest` in the console.
  Expect: 20.
  note: If reading the note doesn't start stage 20, type `setstage PTCR_Quest 20` to go on, and mark this check Fail.

## 4 Stage 30: collect the Iron

- [ ] Close every menu (inventory, Missions, the console) before you take the 5th Iron, so you can see the HUD line that follows. Then pick up Iron until you hold 5 (or type `player.additem 0000556E 5` in the console, and close it at once).
  Expect: the objective "Collect 5 Iron" is completed and your credits go up by 100.
  smoke: yes
  log: iron-check, stage-30
- [ ] Watch the HUD for about 5 seconds after the reward.
  Expect: a few seconds after the Iron and credits pickup notices, a HUD line says "A Courier's Request: done. The workshop paid you 100 credits."
  smoke: yes
  log: stage-30-hud
- [ ] Open the Missions menu and look at the Completed tab.
  Expect: "A Courier's Request" is listed under Completed, and it is no longer listed as an active quest under Activities.
  smoke: yes
  log: stage-30-stopped
  note: Version 0.1.0 left the quest running, so the Completed tab did not list it. If it still isn't listed, mark this check Fail and note what the Activities tab shows.
- [ ] Type `getstage PTCR_Quest` in the console.
  Expect: 30.

## 5 Edge cases

- [ ] On a second new character, pick up 5 Iron before you reach New Atlantis, then arrive and read the note.
  Expect: the quest completes as soon as you read the note, because you already hold the Iron.
  log: stage-30
- [ ] Leave New Atlantis and come back after the quest has started.
  Expect: no second note, and the stage doesn't change.
- [ ] Turn on Show Debug Messages (PC only), then pick up one more Iron before the quest ends.
  Expect: a `[PTCR] Iron check` line shows on screen as well as in the log.

## 6 Drop the note before reading it

The note is not a quest item, so you can drop, sell or store it. These checks try that. Use a new character, and make a save when the note arrives, before you drop or read it: checks 3, 4 and 5 of this section reload that save.

- [ ] On a new character, when the note arrives, save the game first. Then open your inventory and drop the note without reading it.
  Expect: the note lies on the ground near you, and the objective stays at "Read the courier's note".
  note: The script places the note as a disabled reference before adding it to your inventory, so a dropped note may stay invisible. If you can't see the note on the ground, mark this check Fail (that is the suspected bug), mark checks 2, 3 and 4 of this section Skip, because they need the note on the ground, and go on to check 5 from your save.
- [ ] Pick the note up again and read it from your inventory.
  Expect: the note's text asks for 5 Iron, and "Collect 5 Iron" appears.
  log: onread, stage-20
- [ ] Reload the save you made when the note arrived, drop the note again, and read it from the ground by activating it, without picking it up.
  Expect: the note's text shows, stage 20 starts and "Collect 5 Iron" appears.
  note: Check this on screen: the log already holds the read lines from check 2, so it can't tell this read apart. If activating the note from the ground doesn't start stage 20, mark this check Fail, pick the note up and read it from your inventory to go on.
- [ ] Reload the same save, drop the note, leave New Atlantis and come back.
  Expect: the note is still where you dropped it, and picking it up and reading it from your inventory still starts stage 20.
  note: Check this on screen, as in check 3. This check asks only whether the dropped note survives the trip, so read it from your inventory even if check 3 failed.
- [ ] Reload the same save and sell the note to a vendor, or put it in a container you won't come back to, before you read it.
  Expect: the quest stays at stage 10, because you no longer have the note to read and the quest makes no second copy. This is a known limit of the tutorial, not a bug: mark this check Pass if the quest stays at stage 10, then type `setstage PTCR_Quest 20` to go on.
  note: Type `getstage PTCR_Quest` first; it should say 10. Don't buy the note back or take it out again: reading it would start stage 20. If the quest moved on without a read, mark this check Fail and say how you got rid of the note.

## 7 Save and load

- [ ] Save while the quest is at stage 10 or 20, quit to the main menu and load the save.
  Expect: the objective is still shown, and reading the note or collecting the Iron still moves the quest on.
  smoke: yes

## Log checks

After play, run papyrus-log-check on the Papyrus log with the rule file `PMTTutorialCourier.logrules.json`. A log from a session that ran only section 6 never reaches stage 30, so the three stage-30 rules and the order rule show NOT_SEEN there; that is expected.

- [ ] The quest script started at the beginning of the game.
  Expect: a `[PTCR] started` line.
  log: started
- [ ] No Papyrus error names a PTCR script.
  Expect: no `error:` line that names PTCR:CourierQuestScript or PTCR:CourierPlayerAliasScript.
  log: no-script-errors
