using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.Components.SpecialConditions
{
    public class DisableMechanoidSignalSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.DisableMechanoidSignal";
        public override bool IsAvailable => ModsConfig.OdysseyActive;

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtility.MakeDisableIncidentPart(DefOfs.IncidentDefOf.GiveQuest_MechanoidSignal));
        }
    }
}
