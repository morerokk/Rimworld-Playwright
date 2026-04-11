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
            // Or should we just airdrop X default androids in on game start when this happens, and call it a day?
            if (xenotypeConfigurePart == null)
            {
                Find.WindowStack.Add(new InfoPopupWindow("Playwright.Components.Boons.Compat_VREAndroids_Androids.ErrorUnsupportedConfigPage".Translate()));
                throw new PlaywrightBuilderException("Androids boon: current scenario's config page unsupported, must be ScenPart_ConfigPage_ConfigureStartingPawns or ScenPart_ConfigPage_ConfigureStartingPawns_Xenotypes.");
            }

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
        }
    }
}
