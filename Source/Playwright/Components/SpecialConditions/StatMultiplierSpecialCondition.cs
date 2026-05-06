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
    public class StatMultiplierSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.StatMultiplier";

        public List<StatMultiplierEntry> StatMultipliers = new List<StatMultiplierEntry>();

        private IEnumerable<StatDef> GetAllowedStats()
        {
            return DefDatabase<StatDef>.AllDefs
                .Where(def => !def.forInformationOnly && def.CanShowWithLoadedMods())
                .Where(def => !StatMultipliers.Any(entry => entry.Stat == def));
        }

        public override void DoSettingsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            if (specialConditionContentListing.ButtonText("Playwright.Components.SpecialConditions.StatMultiplier.Add".Translate()))
            {
                StatMultipliers.Add(new StatMultiplierEntry());
                SoundUtils.PlayAdd();
                specialConditionContentListing.InvalidateGroup();
            }

            DoStatMultipliersContents(specialConditionContentListing);
        }

        private void DoStatMultipliersContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            Texture2D deleteTex = TextureUtils.DeleteButtonTex;

            float oldWidth = specialConditionContentListing.ColumnWidth;
            specialConditionContentListing.ColumnWidth *= 0.9f;

            foreach (var statEntry in StatMultipliers.ToList())
            {
                specialConditionContentListing.GapLine();
                // (select stat), input multiplier, "%"
                Rect rowRect = specialConditionContentListing.GetRect(35f);

                // (select stat)
                Rect statButtonRect = new Rect(rowRect);
                statButtonRect.width *= 0.4f;
                if (Widgets.ButtonText(statButtonRect, statEntry.StatDefLabel))
                {
                    var options = new List<FloatMenuOption>();
                    foreach (var statDef in GetAllowedStats())
                    {
                        options.Add(new FloatMenuOption(statDef.LabelCap, () =>
                        {
                            statEntry.Stat = statDef;
                            specialConditionContentListing.InvalidateGroup();
                        }));
                    }
                    PlaywrightUtils.OpenFloatMenu(options);
                    SoundUtils.PlayClick();
                }

                // (input multiplier)
                Rect multiplierRect = new Rect(rowRect);
                multiplierRect.width *= 0.4f;
                multiplierRect.x += statButtonRect.width;
                Widgets.TextFieldNumericLabeled(multiplierRect, "Playwright.Components.SpecialConditions.StatMultiplier.Multiplier".Translate(), ref statEntry.Multiplier, ref statEntry.MultiplierBuffer, 0f, 10000f);

                // %
                Rect percentageRect = new Rect(rowRect);
                percentageRect.width *= 0.1f;
                percentageRect.x += statButtonRect.width + multiplierRect.width;
                float textHeight = Text.CalcHeight("%", percentageRect.width);
                Rect percentageRectVerticalCentered = PlaywrightDrawHelper.GetVerticallyCenteredLabelRect(percentageRect, "%");
                Widgets.Label(percentageRectVerticalCentered, "%");

                // Delete button
                Rect deleteButtonRect = new Rect(rowRect);
                deleteButtonRect.width *= 0.1f;
                deleteButtonRect.x += statButtonRect.width + multiplierRect.width + percentageRect.width;
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    StatMultipliers.Remove(statEntry);
                    specialConditionContentListing.InvalidateGroup();
                    SoundUtils.PlayRemove();
                }
            }

            specialConditionContentListing.ColumnWidth = oldWidth;
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            foreach (var statEntry in StatMultipliers)
            {
                if (statEntry.Stat == null)
                {
                    continue;
                }
                scenarioParts.Add(ScenPartUtils.MakeStatFactorPart(statEntry.Stat, statEntry.Multiplier * 0.01f));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref StatMultipliers, nameof(StatMultipliers), LookMode.Deep);
        }

        public class StatMultiplierEntry : IExposable
        {
            public StatDef Stat;
            public float Multiplier = 100f; // We save these as percentage chances, they're factored (200%->2) when the scenario is mutated
            public string MultiplierBuffer = "100";

            public string StatDefLabel => Stat != null ? Stat.LabelCap.ToString() : "-";

            public void ExposeData()
            {
                Scribe_Defs.Look(ref Stat, nameof(Stat));
                Scribe_Values.Look(ref Multiplier, nameof(Multiplier), 100f);
                MultiplierBuffer = Multiplier.ToString();
            }
        }
    }
}
