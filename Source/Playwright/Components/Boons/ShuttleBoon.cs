using RimWorld;
using Rokk.Playwright.ScenParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.Boons
{
    public class ShuttleBoon : BoonComponent
    {
        public override string Id => "Boons.Shuttle";
        public override bool IsAvailable => ModsConfig.OdysseyActive;

        public override void MutateScenario(List<ScenPart> scenarioParts)
        {
            ScenPartDef startWithNonMinifiedThingDef = Rokk.Playwright.DefOfs.ScenPartDefOf.Playwright_StartWithNonMinifiedThing;
            StartWithNonMinifiedThing part = (StartWithNonMinifiedThing)ScenarioMaker.MakeScenPart(startWithNonMinifiedThingDef);

            part.thingDef = ThingDefOf.PassengerShuttle;
            
            scenarioParts.Add(part);
        }
    }
}
