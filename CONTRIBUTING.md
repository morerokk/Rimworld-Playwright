# Contributing to Playwright

On the off chance you want to submit a Pull Request to Playwright, you can. Just be aware that PR's for this repository are meant for changing the functionality of the mod, or adding/improving compatibility with other mods.

The following things should probably go into a separate mod of your own, rather than the main mod:

- **Translations:** The standard way to make translations for other mods is by copying the `Common/Languages/English` folder into your own mod, renaming the folder to the desired language, and translating the text in the XML files. You don't need to copy any of the other files. Make your mod depend on Playwright, and the translations should work.
- **Content:** If you want to add new content (such as Components and/or ScenParts), you should probably copy one of the `Compat` projects instead. If you need something to be added to Playwright's addon API to properly create your content, feel free to either ask, or open a PR with what you need.
