# Contributing to Playwright

On the off chance you want to submit a Pull Request to Playwright, you can. Just be aware that PR's for this repository are meant for changing the functionality of the mod, or adding/improving compatibility with other mods when things break.

The following things should probably go into a separate mod of your own, rather than the main mod:

- **Translations:** The standard way to make translations for mods is by copying this mod's `Common/Languages/English` folder into your own mod, renaming the folder to the desired language, and translating the text in the XML files. You don't need to copy any of the other files, just the XML is fine. Make your mod depend on Playwright, and the translations should work.
- **Content:** If you want to add new content (such as Components and/or ScenParts, especially if they're meant to add content based on the presence of other mods), you should probably copy one of the `Compat` projects instead and make a separate addon mod.

If you need something to be added to Playwright's addon API to properly create your content, feel free to either ask, or open a PR with what you need. The goal of the addon API's is to make it easier for you to do what you want, and if you lack something in it, that something will likely be added if you just ask.

## AI Slop

Code changes or additions that are unchanged raw output of LLM's (especially "agents") will likely be rejected. LLM-written code is usually not up to this project's quality standards. LLM assistance during coding is okay, but if people can tell it's AI, you have likely taken the LLM output at face value too much.

Similarly, if your PR description is mostly AI-written, it will likely get rejected if it isn't immediately obvious from the code what it does and whether there are any caveats. If you don't know what your changes do, or if you didn't feel like writing out what you changed, you should not expect others to feel like reading or considering it, either.

Machine translation is okay.

This section only applies to PR's or contributions to this mod (Playwright) directly. Obviously, you are free to do whatever you want in your own addon mods.
