using RimWorld;
using Rokk.Playwright.ScenParts;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.Components.Boons
{
    public class ShuttleBoon : BoonComponent
    {
        public override string Id => "Boons.Shuttle";
        public override bool IsAvailable => ModsConfig.OdysseyActive;

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {            
            scenarioParts.Add(ScenPartUtils.MakeStartWithNonMinifiedThingPart(ThingDefOf.PassengerShuttle));
            scenarioParts.Add(ScenPartUtils.MakeStartingThingDefinedPart(ThingDefOf.Chemfuel, null, 400));
        }
    }
}
