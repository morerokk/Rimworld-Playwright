using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Components.Boons;
using Rokk.Playwright.Exceptions;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Verse;

namespace Rokk.Playwright.Compat.VREAndroids.Components.Boons
{
    public class AndroidsBoon : BoonComponent
    {
        public override string Id => "Boons.Compat_VREAndroids_Androids";

        public int Amount = 3;
        private string AmountBuffer = "3";

        public override void DoSettingsContents(Listing_AutoFitVertical boonContentListing)
        {
            boonContentListing.Label("Playwright.Components.Boons.Compat_VREAndroids_Androids.Amount".Translate());
            boonContentListing.IntEntry(ref Amount, ref AmountBuffer, 1, 1);
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes xenotypeConfigurePart = null;

            ScenPart_ConfigPage_ConfigureStartingPawns regularConfigurePart = scenarioParts
                .Where(part => part.def == ScenPartDefOf.ConfigPage_ConfigureStartingPawns)
                .Cast<ScenPart_ConfigPage_ConfigureStartingPawns>()
                .FirstOrDefault();

            // We need either a ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes or a ScenPart_ConfigPage_ConfigureStartingPawns_KindDefs to add androids specifically.
            // If we have a regular ScenPart_ConfigPage_ConfigureStartingPawns, convert it to the xenotype variant.
            // When we have the xenotype variant, add the androids xenotype to it as requirement.
            // If we have a kind variant, add the androids kind to it as requirement.
            // Anything else is not supported.
            // (Holy shit this is ass. At least the mechanitor start is supported now, so we support everything except "The Anomaly" start.)
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

            if (xenotypeConfigurePart != null)
            {
                xenotypeConfigurePart.pawnChoiceCount += Amount;
                xenotypeConfigurePart.xenotypeCounts.Add(new XenotypeCount()
                {
                    count = Amount,
                    countBuffer = Amount.ToString(),
                    requiredAtStart = true,
                    xenotype = DefOfs.XenotypeDefOf.VREA_AndroidBasic
                });
                xenotypeConfigurePart.overrideKinds.Add(new XenotypePawnKind()
                {
                    xenotype = DefOfs.XenotypeDefOf.VREA_AndroidBasic,
                    pawnKind = DefOfs.PawnKindDefOf.VREA_AndroidBasic
                });
            }
            else
            {
                ScenPart_ConfigPage_ConfigureStartingPawns_KindDefs kindConfigurePart = scenarioParts
                    .Where(part => part.def == Playwright.DefOfs.ScenPartDefOf.ConfigurePawnsKindDefs)
                    .Cast<ScenPart_ConfigPage_ConfigureStartingPawns_KindDefs>()
                    .FirstOrDefault();
                if (kindConfigurePart == null)
                {
                    throw new PlaywrightBuilderException("Playwright.Components.Boons.Compat_VREAndroids_Androids.ErrorUnsupportedConfigPage".Translate());
                }

                kindConfigurePart.pawnChoiceCount += Amount;
                kindConfigurePart.kindCounts.Add(new PawnKindCount()
                {
                    count = Amount,
                    countBuffer = Amount.ToString(),
                    requiredAtStart = true,
                    kindDef = DefOfs.PawnKindDefOf.VREA_AndroidBasic
                });
            }

            // Add research if it doesn't exist yet
            var existingResearchParts = scenarioParts
                .Where(part => part.def == Playwright.DefOfs.ScenPartDefOf.StartingResearch)
                .Cast<ScenPart_StartingResearch>()
                .ToList();
            // Fucking private access modifiers. Should we reconsider the part where we respect access modifiers?
            bool neutroamineFound = false;
            bool androidtechFound = false;
            FieldInfo projectInfo = AccessTools.Field(typeof(ScenPart_StartingResearch), "project");
            foreach (ScenPart_StartingResearch researchPart in existingResearchParts)
            {
                ResearchProjectDef researchProject = (ResearchProjectDef)projectInfo.GetValue(researchPart);
                if (researchProject == DefOfs.ResearchProjectDefOf.VREA_NeutroamineLogistics)
                {
                    neutroamineFound = true;
                }
                if (researchProject == DefOfs.ResearchProjectDefOf.VREA_AndroidTech)
                {
                    androidtechFound = true;
                }
            }

            if (!neutroamineFound)
            {
                scenarioParts.Add(ScenPartUtility.MakeStartingResearchPart(DefOfs.ResearchProjectDefOf.VREA_NeutroamineLogistics));
            }
            if (!androidtechFound)
            {
                scenarioParts.Add(ScenPartUtility.MakeStartingResearchPart(DefOfs.ResearchProjectDefOf.VREA_AndroidTech));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Amount, nameof(Amount), 3);
            AmountBuffer = Amount.ToString();
        }
    }
}
