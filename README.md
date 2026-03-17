# Playwright

An improved scenario editor for the game [RimWorld](https://rimworldgame.com/).

This mod is currently under construction and unreleased! Things will probably be broken, but the mod is in a good enough state right now that it works.

# Installation

[Download the repository as zip file](https://github.com/morerokk/Rimworld-Playwright/archive/refs/heads/master.zip), and extract the zip file into Rimworld/Mods.

# Building from source

Checkout the `master` branch of this git repository, and open the `Source/Playwright.sln` solution file in Visual Studio. I use Visual Studio 2026. Harmony and the Rimworld reference assemblies are loaded as NuGet packages, so the main project should build out of the box.

The `Compat` folder contains mod compatibility layers that depend on other mods and their assemblies. You can unload these projects if you don't need to touch them. The default paths will try to look for the assemblies in your Steam workshop content folder. This only works if you are subscribed to said mod(s) on the Steam workshop. If your workshop content files are under a different path or you want to use non-workshop versions of these mods, you will have to edit these paths in the References menu, or in the `.csproj` files. VS sometimes still misbehaves when finding references, so worst case, you may have to delete the References and re-add them manually.

You should be able to Build/Rebuild the entire solution, and it will compile the mod. Keep in mind that each project has a post-build event defined that automatically copies the output assemblies into the mod's respective `Assemblies` folder.

# Making addons or compatibility layers

If you want to add new content to Playwright from another mod, [check the documentation for the Addon API.](https://github.com/morerokk/Rimworld-Playwright/wiki/Addon-API)

If you want to resolve a mod incompatibility between Playwright and another mod, [check the page for resolving incompatibilities.](https://github.com/morerokk/Rimworld-Playwright/wiki/Resolving-mod-incompatibilities)
