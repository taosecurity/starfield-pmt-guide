# Changelog

## 2026-11-26

### What's new

The guide now names sf-mod-check 0.23.0, in one update for its four releases from 0.20.0 to 0.23.0. Readers who have 0.19.0 should update. Since 0.20.0, `patch` no longer loses an entry when both mods remove the same quest stage or objective entry and one of them also adds another. For example, with base [A, B], one mod giving [A, C] and the other giving [A], 0.19.0 gave [A] and 0.20.0 gives [A, C]. The releases also added `find --full`, `--versions`, `--refs` and `--layout`, the commands `lines`, `voice-lines`, `ba2` and `voice-format`, and `release` warnings for sound events that no loaded sound bank holds. The guide's pages don't teach these new commands yet. The download table on the Getting started page and every "What you need" list name the new version.

### Example

Check that you have the new version:

    > sf-mod-check --version
    sf-mod-check 0.23.0 by Richard Bejtlich - GPL-3.0 - https://github.com/taosecurity/sf-mod-check

## 2026-11-25 (update 3)

### What's new

Update 3 of the guide covers the day's tool releases. sf-mod-check 0.19.0 checks quest stages (rules `quest-stage-unset` and `quest-stage-missing`, both warnings), and its `patch` merges quests part by part (stages and objectives by Index, aliases by alias ID, script properties one by one) and builds dialogue topics and lines, while scenes and dialogue branches are still listed for xEdit. sf-mod-check 0.18.0 added the `release` rule `voice-missing` for dialogue lines with no voice file and shows `RESULT: NOT CHECKED` from `patch check` for a patch that isn't active. BuildKit 0.6.0 has quest helpers, and BuildKit 0.6.1 brought back its README's "More examples" section, so the add-on example links to the main README again. sf-mod-build 0.5.0 writes one zip per plugin and names PS5 as untested, and in sf-mod-build 0.5.1 the starter template uses BuildKit 0.6.0, so the guide no longer tells you to raise that version by hand. sf-load-order 0.3.2 runs sf-mod-check's conflict check for an MO2 setup by itself. sf-patch-day 0.4.0 lists the Creations that were added, removed or updated since you last looked. The zips of sf-test-plan 0.4.1 and papyrus-log-check 0.4.3 ship a made-up quest example, Tidewater Errand. Every page now fills in each tool's current version from its release, so the version numbers stay in step.

### Example

Check the quest stages of the tutorial quest A Courier's Request with sf-mod-check 0.19.0. With `--verbose`, the log shows each stage and what sets it. These lines come from a run on the tutorial's 0.1.1 plugin on 2026-11-25 (the other log lines left out):

    > sf-mod-check check dist\PMTTutorialCourier.esm --verbose
    log: quest stages: PTCR_Quest [QUST:000802:PMTTutorialCourier.esm]: 3 stage(s), 3 set, unset: none
    RESULT: OK, 0 errors, 0 warnings (PMTTutorialCourier.esm: 5 records, 13 links; 4.0 s)

## 2026-11-25

### What's new

Update 2 of the guide. It also collects two changes made on 2026-11-24, after the first version: the comparison chart on the "compare" pages became fully checked (every cell starts with Yes, No or Partly, no "Not checked" cells are left, and each answer comes from the tool's own code and documentation), and the xEdit text moved to 4.1.5s. The guide now covers sf-load-order 0.3.0 `sort` and its ordering rules: it builds a load order, moves as few plugins as it can, and never applies the order, so you follow `HOW-TO-APPLY.txt` yourself. The triage pages describe the sf-mod-check 0.17.0 field-by-field rows, including their limits: 200 rows per record (plus up to 200 more for the two translation kinds), 2,000 characters per value, and 10,000 rows in the whole report with real changes first. The guide now names sf-mod-check 0.17.0 and sf-load-order 0.3.1. (0.3.1, released later the same day, only fixes 0.3.0: clearer `sort` and `rules` messages, a safer LOOT name check, and MO2's default folders read as MO2 reads them.)

### Example

Sort a load order with sf-load-order 0.3.1. It writes its reports, a suggested `plugins.txt` and a `HOW-TO-APPLY.txt` into the report folder, and it never changes your load order itself:

    > .\sf-load-order.cmd sort

## 2026-11-24

### What's new

First version of the Starfield PMT guide: a manual half and an AI-assisted half, each with the same pages for modders and players, a Getting started page, and an appendix. The `examples` folder holds the source of the two tutorial mods, PMTTutorialSecondWind and PMTTutorialCourier. The guide names sf-mod-check 0.16.2 and sf-patch-day 0.3.0.

### Example

Open the start page of the guide and follow the steps to check a plugin:

    > sf-mod-check check dist\MyMod.esm --masters-dir empty
    sf-mod-check 0.16.2 check: MyMod.esm (light master, 1 masters, 6 records, 0 overrides (0 ITM), 8 links)
    RESULT: OK, 0 errors, 0 warnings (MyMod.esm: 6 records, 8 links; 3.9 s)
