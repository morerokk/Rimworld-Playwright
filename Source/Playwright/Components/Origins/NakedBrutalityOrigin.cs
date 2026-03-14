using HarmonyLib;
using RimWorld;
using Rokk.Playwright.DefOfs;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public class NakedBrutalityOrigin : OriginComponent
    {
        public override string Id => "Origins.NakedBrutality";
        public override int StartingColonistsSelectable => 1;

        public override void MutateScenario(Scenario scenario,List<ScenPart> scenarioParts)
        {
            // Start naked
            scenarioParts.Add(ScenPartUtility.MakeNakedPart(1f, PawnGenerationContext.PlayerStarter));
            // Start with no possessions
            scenarioParts.Add(ScenPartUtility.MakeNoPossessionsPart());
        }
    }
}
