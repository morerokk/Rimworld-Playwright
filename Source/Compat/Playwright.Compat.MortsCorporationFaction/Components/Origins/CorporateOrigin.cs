using RimWorld;
using Rokk.Playwright.Compat.MortsCorporationFaction.Components.Boons;
using Rokk.Playwright.Compat.MortsCorporationFaction.Components.WinConditions;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Factions;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Components.WinConditions;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Compat.MortsCorporationFaction.Components.Origins
{
    public class CorporateOrigin : OriginComponent
    {
        // ID's must be unique, so prefix it with something distinct
        public override string Id => "Origins.Compat_MF_Corporate";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.Playwright_MF_Corporate;

        public override List<BoonComponent> DefaultBoons => new List<BoonComponent>()
        {
            new CorporateGoonsBoon(),
            new BionicArmBoon()
        };

        public override List<FactionComponent> DefaultAllies => new List<FactionComponent>()
        {
            new SpecificFaction()
            {
                Faction = DefOfs.FactionDefOf.MF_Corporation
            },
            new AllOtherFactions()
        };

        public override List<FactionComponent> DefaultEnemies => new List<FactionComponent>()
        {
            new SpecificFaction()
            {
                Faction = DefOfs.FactionDefOf.MF_CyberPunks
            },
            new InsectoidHiveFaction(),
            new MechanoidHiveFaction(),
            new AllOtherFactions()
        };

        public override List<WinConditionComponent> DefaultWinConditions => new List<WinConditionComponent>()
        {
            new PaperworkWinCondition()
        };
    }
}
