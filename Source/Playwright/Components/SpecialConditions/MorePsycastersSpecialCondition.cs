using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components.SpecialConditions
{
    public class MorePsycastersSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.MorePsycasters";
        public override bool IsAvailable => ModsConfig.RoyaltyActive;

        public float Chance = 0.05f;
        // Levels 1-5 have the most interesting psycasts for NPC's to use.
        // 6 is too disruptive with Neuroquake and Berserk Pulse
        public IntRange PsylinkLevelRange = new IntRange(1, 5);
        private FloatRange PsylinkLevelAsFloatRange => new FloatRange(PsylinkLevelRange.min, PsylinkLevelRange.max);

        public override void DoSettingsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            base.DoSettingsContents(specialConditionContentListing);
            Chance = specialConditionContentListing.SliderLabeled("Playwright.Components.SpecialConditions.MorePsycasters.ChancePercentage".Translate(Chance * 100f), Chance, 0f, 1f, 0.5f, "Playwright.Components.SpecialConditions.MorePsycasters.Chance.Help".Translate());
            Chance = MathF.Round(Chance, 2);

            Rect levelTooltipRect = specialConditionContentListing.Label("Playwright.Components.SpecialConditions.MorePsycasters.PsylinkLevel".Translate());
            specialConditionContentListing.IntRange(ref PsylinkLevelRange, 1, 6);
            if (Mouse.IsOver(levelTooltipRect))
            {
                TooltipHandler.TipRegion(levelTooltipRect, "Playwright.Components.SpecialConditions.MorePsycasters.PsylinkLevel.Help".Translate());
            }
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtility.MakeForcedPsylinkLevelPart(PsylinkLevelAsFloatRange, Chance, PawnGenerationContext.NonPlayer));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Chance, nameof(Chance), 0.05f);
            Scribe_Values.Look(ref PsylinkLevelRange, nameof(PsylinkLevelRange), new IntRange(1, 5));
        }
    }
}
