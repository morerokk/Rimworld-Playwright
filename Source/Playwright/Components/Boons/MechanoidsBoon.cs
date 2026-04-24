using HarmonyLib;
using RimWorld;
using Rokk.Playwright.Exceptions;
using Rokk.Playwright.ScenParts;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Verse;

namespace Rokk.Playwright.Components.Boons
{
    public class MechanoidsBoon : BoonComponent
    {
        public override string Id => "Boons.Mechanoids";
        public override bool IsAvailable => ModsConfig.BiotechActive;

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            ScenPart_ConfigPage_ConfigureStartingPawns_KindDefs kindsConfigurePart = null;

            ScenPart_ConfigPage_ConfigureStartingPawns regularConfigurePart = scenarioParts
                .Where(part => part.def == ScenPartDefOf.ConfigPage_ConfigureStartingPawns)
                .Cast<ScenPart_ConfigPage_ConfigureStartingPawns>()
                .FirstOrDefault();

            // We need either a regular configpage part, or a kinddef part.
            // Other types are not supported and will throw an error.
            if (regularConfigurePart != null)
            {
                kindsConfigurePart = ScenPartUtility.ConvertConfigureStartingPawnsToKindDefs(regularConfigurePart);
                scenarioParts.Remove(regularConfigurePart);
                scenarioParts.Add(kindsConfigurePart);
            }
            else
            {
                kindsConfigurePart = scenarioParts
                    .Where(part => part.def == Playwright.DefOfs.ScenPartDefOf.ConfigurePawnsKindDefs)
                    .Cast<ScenPart_ConfigPage_ConfigureStartingPawns_KindDefs>()
                    .FirstOrDefault();
            }

            if (kindsConfigurePart == null)
            {
                throw new PlaywrightBuilderException("Playwright.Components.Boons.Mechanoids.ErrorUnsupportedConfigPage".Translate());
            }

            // If a mechanitor pawnkind is already called for, add another.
            // If none are called for yet, add one.
            var mechanitorPawnKindCount = kindsConfigurePart.kindCounts
                .FirstOrDefault(kind => kind.kindDef == DefOfs.PawnKindDefOf.Mechanitor); // The Mechanitor scenario uses Mechanitor, NOT the Mechanitor_Basic that's been built into the default defofs
            if (mechanitorPawnKindCount != null)
            {
                mechanitorPawnKindCount.count += 1;
                mechanitorPawnKindCount.countBuffer = mechanitorPawnKindCount.count.ToString();
            }
            else
            {
                kindsConfigurePart.kindCounts.Add(new PawnKindCount()
                {
                    count = 1,
                    countBuffer = "1",
                    kindDef = DefOfs.PawnKindDefOf.Mechanitor,
                    requiredAtStart = true
                });
            }

            // If possible, deduct 1 regular Colonist from the preparation screen.
            // Not an error if none such exist, the player will just start with an extra colonist (big whoop).
            var colonistPawnKindCount = kindsConfigurePart.kindCounts
                .FirstOrDefault(kind => kind.kindDef == PawnKindDefOf.Colonist);
            if (colonistPawnKindCount != null)
            {
                colonistPawnKindCount.count -= 1;
                colonistPawnKindCount.countBuffer = colonistPawnKindCount.count.ToString();
            }

            // Add Lifter and Constructoid
            scenarioParts.Add(ScenPartUtility.MakeStartingMechPart(DefOfs.PawnKindDefOf.Mech_Lifter, 1));
            scenarioParts.Add(ScenPartUtility.MakeStartingMechPart(DefOfs.PawnKindDefOf.Mech_Constructoid, 1));

            // Add starting research for microelectronics, batteries and basic mechtech if the scenario doesn't explicitly have it already.
            var existingResearchParts = scenarioParts
                .Where(part => part.def == DefOfs.ScenPartDefOf.StartingResearch)
                .Cast<ScenPart_StartingResearch>()
                .ToList();
            // Thanks private access modifiers
            bool microelectronicsFound = false;
            bool batteriesFound = false;
            bool basicMechtechFound = false;
            FieldInfo projectInfo = AccessTools.Field(typeof(ScenPart_StartingResearch), "project");
            foreach (ScenPart_StartingResearch researchPart in existingResearchParts)
            {
                ResearchProjectDef researchProject = (ResearchProjectDef)projectInfo.GetValue(researchPart);
                if (researchProject == ResearchProjectDefOf.MicroelectronicsBasics)
                {
                    microelectronicsFound = true;
                }
                if (researchProject == ResearchProjectDefOf.Batteries)
                {
                    batteriesFound = true;
                }
                if (researchProject == ResearchProjectDefOf.BasicMechtech)
                {
                    basicMechtechFound = true;
                }
            }

            if (!microelectronicsFound)
            {
                scenarioParts.Add(ScenPartUtility.MakeStartingResearchPart(ResearchProjectDefOf.MicroelectronicsBasics));
            }
            if (!batteriesFound)
            {
                scenarioParts.Add(ScenPartUtility.MakeStartingResearchPart(ResearchProjectDefOf.Batteries));
            }
            if (!basicMechtechFound)
            {
                scenarioParts.Add(ScenPartUtility.MakeStartingResearchPart(ResearchProjectDefOf.BasicMechtech));
            }
        }
    }
}
