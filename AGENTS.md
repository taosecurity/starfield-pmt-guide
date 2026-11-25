# AGENTS.md: Starfield PMT Guide

Instructions for AI assistants working with this repository.

1. **What it is:** the published source of the PMT guide, a static site at https://taosecurity.github.io/starfield-pmt-guide/ (pages at the repository root, served by GitHub Pages from `main`).
2. **The pages are generated.** Every `.html` file outside `examples` is built output. Do not hand-edit it, because the next build overwrites your change. To fix a page, report the problem (see the README) or ask the maintainer to change the source text and rebuild.
3. **The examples** are in `examples/PMTTutorialSecondWind` and `examples/PMTTutorialCourier`: source only (builder, Papyrus source, test plan, log rules, walkthrough). To build one, follow its WALKTHROUGH.md with the PMT tools named in the guide's Getting started page (`start.html`).
4. **Never add game code**: no game files, decompiled code or official Bethesda `.psc` files.
5. Commands in the guide come from each tool's own README at its tagged release. If a command here disagrees with a tool's current README, trust the README and report the difference.
