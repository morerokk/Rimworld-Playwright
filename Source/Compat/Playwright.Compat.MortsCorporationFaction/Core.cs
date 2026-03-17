using HarmonyLib;
using Rokk.Playwright.Addons;
using Rokk.Playwright.Compat.MortsCorporationFaction.Components.Origins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Compat.MortsCorporationFaction
{
    // Core class of a compatibility mod layer.
    // As long as the Playwright base mod loads before this compatibility mod in LoadFolders.xml, this will work.
    // For external addons, just ensure your mod has a loadAfter in the About.xml to ensure it loads after Playwright
    public class Core : Mod
    {
        public Core(ModContentPack content) : base(content)
        {
            // Harmony patches are supported but not mandatory. If you copy this, change this ID
            Harmony harmony = new Harmony("rokk.playwright.compat.mortscorporationfaction");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // Register your components with ComponentRegistration if you have any
            var corporateOrigin = new CorporateOrigin();
            if (corporateOrigin.IsAvailable)
            {
                ComponentRegistration.RegisterOrigin(corporateOrigin);
            }

            // If you have any hooks, register them
            //HookRegistration.RegisterScenarioPostMutated((playwrightStructure, scenario, parts) =>
            //{
                //Code you want to run after the scenario has been built
            //});
        }
    }
}
