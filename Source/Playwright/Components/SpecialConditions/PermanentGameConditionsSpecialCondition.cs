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
    public class PermanentGameConditionsSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.PermanentGameConditions";

        public List<GameConditionDef> PermanentGameConditions = new List<GameConditionDef>();

        private IEnumerable<GameConditionDef> GetAllowedGameConditions()
        {
            return DefDatabase<GameConditionDef>.AllDefs
                .Where(def => def.canBePermanent);
        }

        public override void DoSettingsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            if (specialConditionContentListing.ButtonText("Playwright.Components.SpecialConditions.PermanentGameConditions.Add".Translate()))
            {
                var options = new List<FloatMenuOption>();

                foreach (var gameConditionDef in GetAllowedGameConditions())
                {
                    options.Add(new FloatMenuOption(gameConditionDef.LabelCap, () =>
                    {
                        PermanentGameConditions.Add(gameConditionDef);
                        specialConditionContentListing.InvalidateGroup();
                    }));
                }
                PlaywrightUtils.OpenFloatMenu(options);
                SoundUtils.PlayClick();
            }

            DoGameConditionsContents(specialConditionContentListing);
        }

        private void DoGameConditionsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            Texture2D deleteTex = TextureUtils.DeleteButtonTex;

            foreach (var gameConditionDef in PermanentGameConditions.ToList())
            {
                Rect rowRect = specialConditionContentListing.GetRect(25f);
                Rect incidentLabelRect = new Rect(rowRect);
                incidentLabelRect.width *= 0.9f;
                Widgets.Label(incidentLabelRect, gameConditionDef.LabelCap);

                Rect deleteButtonRect = new Rect(rowRect);
                deleteButtonRect.width *= 0.1f;
                deleteButtonRect.x += incidentLabelRect.width;
                if (PlaywrightDrawHelper.DrawButtonInTopRight(deleteButtonRect, deleteTex, 0f, 0.4f))
                {
                    PermanentGameConditions.Remove(gameConditionDef);
                    specialConditionContentListing.InvalidateGroup();
                    SoundUtils.PlayRemove();
                }
            }
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            foreach (var gameConditionDef in PermanentGameConditions)
            {
                if (gameConditionDef == null)
                {
                    continue;
                }
                scenarioParts.Add(ScenPartUtils.MakePermaGameConditionPart(gameConditionDef));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref PermanentGameConditions, nameof(PermanentGameConditions), LookMode.Def);
        }
    }
}
