# Changelog

## 2026-11-24

### What's new

First version of the Starfield PMT guide: a manual half and an AI-assisted half, each with the same pages for modders and players, a Getting started page, and an appendix. The `examples` folder holds the source of the two tutorial mods, PMTTutorialSecondWind and PMTTutorialCourier. The guide names sf-mod-check 0.16.2 and sf-patch-day 0.3.0.

### Example

Open the start page of the guide and follow the steps to check a plugin:

    > sf-mod-check check dist\MyMod.esm --masters-dir empty
    sf-mod-check 0.16.2 check: MyMod.esm (light master, 1 masters, 6 records, 0 overrides (0 ITM), 8 links)
    RESULT: OK, 0 errors, 0 warnings (MyMod.esm: 6 records, 8 links; 3.9 s)
