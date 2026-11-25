# Starfield PMT Guide

This is the source of the Starfield Public Modding Tools (PMT) guide, a set of web pages that teach you to fix, build, test, release and maintain Starfield mods with the PMT command-line tools. It has two halves that mirror each other: a manual half for modding by hand, and an AI-assisted half for Human Developed, Agent Programmed modding, where you design and decide and an AI agent writes the code and runs the checks.

Open the site: https://taosecurity.github.io/starfield-pmt-guide/

For AI assistants: instructions for working with this repository are in [AGENTS.md](AGENTS.md).

## What is in this repository

- The built pages sit at the root of the repository (`index.html`, `start.html`, and the `manual`, `ai` and `appendix` folders), with their styles in `assets`. GitHub Pages publishes them from the `main` branch.
- `examples` holds the source of the two tutorial mods the guide builds step by step:
  - `examples/PMTTutorialSecondWind`: a small mechanics mod.
  - `examples/PMTTutorialCourier`: a small quest mod.

  Each folder has the builder source, the Papyrus script source (`Scripts/Source`), the test plan, the log rules, a README, a CHANGELOG where there is one, and a `WALKTHROUGH.md` that records the commands run. Built plugins, archives and zips are not included. You build them yourself by following the guide.

## Example

The guide's checks look like this. A made-up add-on, `MyMod.esm`, has a script packed in its own archive, `dist\MyMod - Main.ba2`, and `empty\` is an empty folder. sf-mod-check 0.16.2 checks it:

    > sf-mod-check check dist\MyMod.esm --masters-dir empty
    sf-mod-check 0.16.2 check: MyMod.esm (light master, 1 masters, 6 records, 0 overrides (0 ITM), 8 links)
    RESULT: OK, 0 errors, 0 warnings (MyMod.esm: 6 records, 8 links; 3.9 s)

## Report a problem

Found a mistake, an old command or a page that doesn't match what a tool does? Open an issue: https://github.com/taosecurity/starfield-pmt-guide/issues

Please name the page, the command you ran and the text that was printed.

## Credits and License

Richard Bejtlich of [Richard Presents Starfield Essentials](https://www.youtube.com/@RichardBejtlich) made this guide. I am the developer, and Claude is the programmer. Built with Claude Code. More Starfield tools and references are on the [Richard Presents Development Hub](https://taosecurity.github.io/).

This is an unofficial guide, not made or endorsed by Bethesda. Starfield is a trademark of ZeniMax Media. The guide holds no game code or game files.

The pages and the example source are MIT licensed; see [LICENSE](LICENSE).
