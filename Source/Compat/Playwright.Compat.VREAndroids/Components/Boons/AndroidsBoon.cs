using RimWorld;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Exceptions;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Compat.VREAndroids.Components.Boons
{
    public class AndroidsBoon : BoonComponent
    {
        public override string Id => "Boons.Compat_VREAndroids_Androids";

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes xenotypeConfigurePart = null;

            ScenPart_ConfigPage_ConfigureStartingPawns regularConfigurePart = scenarioParts
                .Where(part => part.def == ScenPartDefOf.ConfigPage_ConfigureStartingPawns)
                .Cast<ScenPart_ConfigPage_ConfigureStartingPawns>()
                .FirstOrDefault();
            if (regularConfigurePart != null)
            {
                xenotypeConfigurePart = ScenPartUtility.ConvertConfigureStartingPawnsToXenotypes(regularConfigurePart);
                scenarioParts.Remove(regularConfigurePart);
                scenarioParts.Add(xenotypeConfigurePart);
            }
            else
            {
                xenotypeConfigurePart = scenarioParts
                    .Where(part => part.def == Playwright.DefOfs.ScenPartDefOf.ConfigurePawnsXenotypes)
                    .Cast<ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes>()
                    .FirstOrDefault();
            }
            // TODO: Add support for the other "weird" configurestartingpawns parts?
            // Or should we just airdrop 3 default androids in on game start when this happens, and call it a day?
            if (xenotypeConfigurePart == null)
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.Boons.Compat_VREAndroids_Androids.ErrorUnsupportedConfigPage".Translate()));
                throw new PlaywrightBuilderException("Current scenario's config page unsupported, must be ScenPart_ConfigPage_ConfigureStartingPawns or ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes.");
            }

            xenotypeConfigurePart.pawnChoiceCount += 3;
            xenotypeConfigurePart.xenotypeCounts.Add(new XenotypeCount()
            {
                count = 3,
                countBuffer = "3",
                requiredAtStart = true,
                xenotype = DefOfs.XenotypeDefOf.VREA_AndroidBasic
            });
            xenotypeConfigurePart.overrideKinds.Add(new XenotypePawnKind()
            {
                xenotype = DefOfs.XenotypeDefOf.VREA_AndroidBasic,
                pawnKind = DefOfs.PawnKindDefOf.VREA_AndroidBasic
            });
        }
    }
}
