# SCP-3008

An endless survival horror game built in Unity 6, set in a store that never ends.
You scavenge, manage hunger and thirst, and try to last the night.

**Status:** in active development.

---

## Systems

24 C# scripts, all gameplay code written from scratch.

**Inventory**
- Slot-based hotbar with pickup priority
- Bag panel (Tab) with full drag-and-drop between slots
- Interaction system with contextual prompts

**Survival**
- Stamina with regeneration and drain states
- Hunger and thirst tracks driving a survival HUD
- Two-mode consumption: tap `E` to take, hold right-click to eat over time

**Data & tooling**
- Player stats defined as ScriptableObjects, tunable without touching code
- **Six custom Unity editor tools** that build the HUD, hotbar, inventory panel,
  stamina bar and interaction prompt automatically — written because setting these
  scenes up by hand was the slowest part of iterating
- Editor utility for finding and clearing missing script references

## Built with

Unity 6 (6000.0.39f1) · C#

## Development

26 commits of incremental development history — the repository shows how each
system was built and revised, not just the end state. See [`roadmap.html`](roadmap.html)
for progress tracking.

## Credits

SCP-3008 is a concept from the [SCP Foundation](https://scpwiki.com/scp-3008) wiki,
created by *Mortos* and released under
[CC BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/). This project is an
unofficial, non-commercial fan implementation. The SCP concept and setting belong to
their original authors; the source code in this repository is my own work.

## License

Source code: all rights reserved — see [LICENSE](LICENSE), published for portfolio
review only. The SCP-3008 concept remains under CC BY-SA 3.0 as noted above.

---

İsmail Samet Uğurlu · [github.com/Ismailsametugurlu](https://github.com/Ismailsametugurlu)
