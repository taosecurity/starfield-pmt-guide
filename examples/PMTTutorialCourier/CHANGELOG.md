# Changelog

## 0.1.2 (2026-11-24)

Makes the courier's note a quest item, so the quest can't get stuck at stage 10.

- A review found that the note, placed disabled and then added to the inventory, could stay invisible if the player
  dropped it, so it couldn't be read and stage 20 never started. Selling or storing the note before reading it also
  left the quest at stage 10. This was never seen in game.
- The quest has a second alias, Note, with the Quest Item flag (and ALFG and an empty voice type, as the Creation
  Kit writes them). Stage 10 fills it with the note (`ForceRefTo`), so the note can't be dropped, sold or stored
  until the quest ends, as the game's own quest notes can't. Stage 30 clears the alias (`Clear()`) before the quest
  stops, so the note becomes a normal item. Both steps write a `[PTCR]` log line.
- The Player alias also gets the empty voice type, as the Creation Kit writes it.
- The test plan's section 6 now checks that the note can't be dropped or sold, and section 4 checks that it can be
  dropped after the quest ends.
- Built with sf-mod-build 0.4.2, sf-mod-check 0.16.1 and sf-papyrus-ref 0.2.4.
- Passed its in-game test on 2026-11-26. That test also corrected the 0.1.1 entry below: the game never lists a
  finished Activity under Missions > Completed (a vanilla Activity isn't listed either), so the Completed tab was
  never a sign that the quest stopped. The save is the sign: "stopped, completed, stage 30". The test plan's check
  4.3 now expects the quest to leave Activities and not to appear under Completed.

## 0.1.1 (2026-11-24)

Fixes two problems found in the first play-test of 0.1.0.

- The quest now stops when it completes. Version 0.1.0 called `CompleteQuest()` at stage 30 but never `Stop()`, so
  a save showed the quest as "running, completed, stage 30" (the game's own finished quests show "stopped,
  completed") and the Missions menu did not list it under Completed. Stage 30 now calls `CompleteQuest()` and then
  `Stop()`, and stage 30's journal entry has the "Complete Quest" flag, as the game's own quests do.
- The final HUD line ("A Courier's Request: done. The workshop paid you 100 credits.") now shows about 3 seconds
  after the reward (`Utility.Wait`, set by the `DoneMessageDelay` property), so it isn't lost among the Iron and
  credits pickup notices. The script logs both steps with the `[PTCR]` prefix.
- The test plan checks both fixes (Missions > Completed, and the HUD line a few seconds after the reward).
- Built with sf-mod-build 0.4.2, sf-mod-check 0.16.0 and sf-papyrus-ref 0.2.4 (which no longer needs
  `--allow Debug.Notification`).

## 0.1.0 (2026-11-24)

First version: a three-stage quest in New Atlantis (note, read it, collect 5 Iron), with a debug switch and a
Creations set.
