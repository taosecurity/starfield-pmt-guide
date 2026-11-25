# Changelog

## 2026-11-25

### What's new

Update 2 of the guide. It also collects two changes made on 2026-11-24, after the first version: the comparison chart on the "compare" pages became fully checked (every cell starts with Yes, No or Partly, no "Not checked" cells are left, and each answer comes from the tool's own code and documentation), and the xEdit text moved to 4.1.5s. The guide now covers sf-load-order 0.3.0 `sort` and its ordering rules: it builds a load order, moves as few plugins as it can, and never applies the order, so you follow `HOW-TO-APPLY.txt` yourself. The triage pages describe the sf-mod-check 0.17.0 field-by-field rows, including their limits: 200 rows per record (plus up to 200 more for the two translation kinds), 2,000 characters per value, and 10,000 rows in the whole report with real changes first. The guide now names sf-mod-check 0.17.0 and sf-load-order 0.3.0.

### Example

Sort a load order with sf-load-order 0.3.0. It writes its reports, a suggested `plugins.txt` and a `HOW-TO-APPLY.txt` into the report folder, and it never changes your load order itself:

    > .\sf-load-order.cmd sort

## 2026-11-24

### What's new

First version of the Starfield PMT guide: a manual half and an AI-assisted half, each with the same pages for modders and players, a Getting started page, and an appendix. The `examples` folder holds the source of the two tutorial mods, PMTTutorialSecondWind and PMTTutorialCourier. The guide names sf-mod-check 0.16.2 and sf-patch-day 0.3.0.

### Example

Open the start page of the guide and follow the steps to check a plugin:

    > sf-mod-check check dist\MyMod.esm --masters-dir empty
    sf-mod-check 0.16.2 check: MyMod.esm (light master, 1 masters, 6 records, 0 overrides (0 ITM), 8 links)
    RESULT: OK, 0 errors, 0 warnings (MyMod.esm: 6 records, 8 links; 3.9 s)
