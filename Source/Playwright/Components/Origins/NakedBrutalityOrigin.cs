using HarmonyLib;
using RimWorld;
using Rokk.Playwright.DefOfs;
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
            ScenPart_Naked startNakedPart = (ScenPart_Naked)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.Naked);
            FieldInfo chanceInfo = AccessTools.Field(typeof(ScenPart_Naked), "chance");
            chanceInfo.SetValue(startNakedPart, 1f);
            FieldInfo contextInfo = AccessTools.Field(typeof(ScenPart_Naked), "context");
            contextInfo.SetValue(startNakedPart, PawnGenerationContext.PlayerStarter);
            scenarioParts.Add(startNakedPart);

            ScenPart_NoPossessions noPossessionsPart = (ScenPart_NoPossessions)ScenarioMaker.MakeScenPart(DefOfs.ScenPartDefOf.NoPossessions);
            scenarioParts.Add(noPossessionsPart);
        }
    }
}
