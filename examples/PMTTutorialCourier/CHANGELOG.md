# Changelog

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
