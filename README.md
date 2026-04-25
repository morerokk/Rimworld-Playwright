# Playwright

An improved scenario editor for the game [RimWorld](https://rimworldgame.com/), as well as a collection of loose scenario parts usable in the game's own scenario editor.

This mod is currently under construction and unreleased! The mod's foundation is pretty much finished and definitely production-ready, but currently content is still being added.

# Installation

You can download this mod off the Steam workshop (URL pending).

For the non-Steam version of the game (or if you prefer not using the workshop), [download the repository as zip file,](https://github.com/morerokk/Rimworld-Playwright/archive/refs/heads/master.zip) and extract the zip file into your `Rimworld/Mods` folder.

# Building from source

Checkout the `master` branch of this git repository, and open the `Source/Playwright.sln` solution file in Visual Studio. I use Visual Studio 2026. Harmony and the Rimworld reference assemblies are loaded as NuGet packages, so the main project should build out of the box.

The `Compat` folder contains mod compatibility layers that depend on other mods and their assemblies. You can unload these projects if you don't need to touch them. The default paths will try to look for the assemblies in your Steam workshop content folder. This only works if you are subscribed to said mod(s) on the Steam workshop. If your workshop content files are under a different path or you want to use non-workshop versions of these mods, you will have to edit these paths in the References menu, or in the `.csproj` files. VS sometimes still misbehaves when finding references, so worst case, you may have to delete the References and re-add them manually.

You should be able to Build/Rebuild the entire solution, and it will compile the mod. Keep in mind that each project has a post-build event defined that automatically copies the output assemblies into the mod's respective `Assemblies` folder.

# Making addons or compatibility layers

If you want to add new content to Playwright from another mod, [check the documentation for the Addon API.](https://github.com/morerokk/Rimworld-Playwright/wiki/Addon-API)

If you want to resolve a mod incompatibility between Playwright and another mod, [check the page for resolving incompatibilities.](https://github.com/morerokk/Rimworld-Playwright/wiki/Resolving-mod-incompatibilities)

# License

[Playwright is licensed under the MIT License.](https://github.com/morerokk/Rimworld-Playwright/blob/master/LICENSE.txt) That means you can freely use the code for any purpose, but you must include the unmodified license text if you redistribute or intend to redistribute the code, or if you incorporate code directly from Playwright into your own mod.

For posterity: if you make an addon for Playwright, you are not required to include this license file. Usage of the addon API in your own mod does not constitute a "copy or substantial portion of the Software".

However, the [Listing_AutoFitVertical](https://github.com/morerokk/Rimworld-Playwright/blob/master/Source/Playwright/UI/Listing_AutoFitVertical.cs) class, while it is my own work, is not licensed under MIT, as I don't feel like it pertains to this mod specifically. Instead, this particular file and its contents are released to the public domain, to the extent allowed by the RimWorld EULA (which likely means that you can only use it for RimWorld mods, as it extends from RimWorld code. It wouldn't be very useful on its own, anyway!). This has been done in the hope that this class saves someone else the same headaches that it has saved me.
