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
    public class Core : Mod
    {
        public Core(ModContentPack content) : base(content)
        {
            // Harmony patches are supported but not mandatory. If you copy this, change this ID
            Harmony harmony = new Harmony("rokk.playwright.compat.mortscorporationfaction");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // Register your components here
            ComponentRegistration.RegisterOrigin(new CorporateOrigin());
        }
    }
}
