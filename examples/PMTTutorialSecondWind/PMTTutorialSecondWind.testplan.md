# PMTTutorialSecondWind test plan
run: pmttutorialsecondwind-0.1.0
mod: PMTTutorialSecondWind
version: 0.1.0
zip: PMTTutorialSecondWind-0.1.0.zip

This plan tests Second Wind (PMTTutorialSecondWind) 0.1.0, the mechanics mod from the PMT guide's tutorial. When an enemy hits you while your health is below 25 percent in combat, you regenerate 10 Health a second for 10 seconds and see "Second wind!" on the HUD. It can happen once every 120 seconds. Start with the smoke test; the deep checks try the edge cases.

## Before you start

- Install PMTTutorialSecondWind-0.1.0.zip with your mod manager and enable it.
- Turn on Papyrus logging (the mod's README says how), so the log shows the mod's `[PTSW]` lines.
- Start a new character, so an old save doesn't hide problems.

## 1 Install and load

- [ ] Start a new game and finish the character creator.
  Expect: no error message about PMTTutorialSecondWind.
  smoke: yes
  log: started
- [ ] Open Settings > Gameplay Options.
  Expect: a group named "Second Wind (PMT Tutorial)" with "Show Debug Messages (PC only)", set to off.
  smoke: yes

## 2 The second wind

The first fight in the Vectera mine (the pirates outside, early in the main quest) is a good place for these checks.

- [ ] In a fight, let enemies hit you until your health is below a quarter, then take one more hit. To get there faster, type `player.damageav health 100` in the console (repeat it until the bar is low), close the console and let an enemy hit you.
  Expect: "Second wind!" shows on the HUD, and your health bar climbs for about 10 seconds.
  smoke: yes
  log: second-wind
- [ ] Right after that, drop below a quarter again within two minutes and take another hit.
  Expect: no second "Second wind!" message, because the mod waits 120 seconds between them.
  smoke: yes
- [ ] Wait at least two minutes with the game running (not paused in a menu), then drop below a quarter in a fight and take a hit.
  Expect: "Second wind!" shows again.
  log: cooldown-over, second-wind
- [ ] Take hits in a fight while your health is above a quarter.
  Expect: nothing from the mod happens.

## 3 Debug switch

- [ ] Turn on Settings > Gameplay Options > Second Wind (PMT Tutorial) > Show Debug Messages (PC only), then start a second wind.
  Expect: a `[PTSW] health at ...` line shows on screen as well as "Second wind!".
- [ ] Turn the switch off again and start a second wind after the cooldown.
  Expect: only "Second wind!" shows; the `[PTSW]` line is in the log only.

## 4 Save and load

- [ ] Save, quit to the main menu, load the save, and start a second wind in a fight.
  Expect: the second wind still starts after loading.
  smoke: yes
  log: second-wind

## Log checks

After play, run papyrus-log-check on the Papyrus log with the rule file `PMTTutorialSecondWind.logrules.json`.

- [ ] The quest script started at the beginning of the game.
  Expect: a `[PTSW] started` line.
  log: started
- [ ] No Papyrus error names the PTSW script.
  Expect: no `error:` line that names PTSW:SecondWindQuestScript.
  log: no-script-errors
