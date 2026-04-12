using RimWorld;
using Rokk.Playwright.Compat.MortsCorporationFaction.Components.Boons;
using Rokk.Playwright.Compat.MortsCorporationFaction.Components.WinConditions;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Components.Factions;
using Rokk.Playwright.Components.Origins;
using Rokk.Playwright.Components.WinConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            new ExtraItemsBoon()
            {
                Items = new List<ExtraItemsBoon.ExtraItemEntry>()
                {
                    new ExtraItemsBoon.ExtraItemEntry()
                    {
                        Thing = DefOfs.ThingDefOf.MF_CorporateSimpleOfficeDesk,
                        Count = 1,
                        CountBuffer = "1",
                        Stuff = ThingDefOf.WoodLog
                    },
                    new ExtraItemsBoon.ExtraItemEntry()
                    {
                        Thing = DefOfs.ThingDefOf.MF_CorporateOfficeChairSimple,
                        Count = 2,
                        CountBuffer = "2",
                        Stuff = ThingDefOf.Cloth,
                        Quality = QualityCategory.Poor
                    }
                }
            }
        };

        public override List<FactionComponent> DefaultAllies => new List<FactionComponent>()
        {
            new SpecificFaction()
            {
                Faction = DefOfs.FactionDefOf.MF_Corporation
            }
        };

        public override List<FactionComponent> DefaultEnemies => new List<FactionComponent>()
        {
            new SpecificFaction()
            {
                Faction = DefOfs.FactionDefOf.MF_CyberPunks
            }
        };

        public override List<WinConditionComponent> DefaultWinConditions => new List<WinConditionComponent>()
        {
            new PaperworkWinCondition()
        };
    }
}
